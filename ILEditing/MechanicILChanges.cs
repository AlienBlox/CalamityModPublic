using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.CalPlayer;
using CalamityMod.NPCs.Astral;
using CalamityMod.NPCs.AstrumAureus;
using CalamityMod.NPCs.Crabulon;
using CalamityMod.NPCs.Ravager;
using CalamityMod.Particles;
using CalamityMod.Projectiles;
using CalamityMod.Waters;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;
using Terraria;
using Terraria.GameContent.Events;
using Terraria.GameContent.Liquid;
using Terraria.GameInput;
using Terraria.Graphics;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI.Gamepad;
using Terraria.Utilities;

namespace CalamityMod.ILEditing
{
    public partial class ILChanges
    {
        private static int labDoorOpen = -1;
        private static int labDoorClosed = -1;
        private static int aLabDoorOpen = -1;
        private static int aLabDoorClosed = -1;
        private static int exoDoorOpen = -1;
        private static int exoDoorClosed = -1;

        // Holds the vanilla game function which spawns town NPCs, wrapped in a delegate for reflection purposes.
        // This function is (optionally) invoked manually in an IL edit to enable NPCs to spawn at night.
        private static Action VanillaSpawnTownNPCs;

        #region Enabling of Triggered NPC Platform Fallthrough
        // Why this isn't a mechanism provided by TML itself or vanilla itself is beyond me.
        private static void AllowTriggeredFallthrough(On.Terraria.NPC.orig_ApplyTileCollision orig, NPC self, bool fall, Vector2 cPosition, int cWidth, int cHeight)
        {
            if (self.active && self.Calamity().ShouldFallThroughPlatforms)
                fall = true;
            orig(self, fall, cPosition, cWidth, cHeight);
        }
        #endregion Enabling of Triggered NPC Platform Fallthrough

        #region Town NPC Spawning Improvements
        private static void PermitNighttimeTownNPCSpawning(ILContext il)
        {
            // Don't do town NPC spawning at the end (which lies after a !Main.dayTime return).
            // Do it at the beginning, without the arbitrary time restriction.
            var cursor = new ILCursor(il);
            cursor.EmitDelegate<Action>(() =>
            {
                // A cached delegate is used here instead of direct reflection for performance reasons
                // since UpdateTime is called every frame.
                if (Main.dayTime || CalamityConfig.Instance.CanTownNPCsSpawnAtNight)
                    VanillaSpawnTownNPCs();
            });

            if (!cursor.TryGotoNext(MoveType.Before, i => i.MatchCallOrCallvirt<Main>("UpdateTime_SpawnTownNPCs")))
            {
                CalamityMod.Instance.Logger.Warn("Town NPC spawn editing code failed.");
                return;
            }

            cursor.Emit(OpCodes.Ret);
        }

        private static void AlterTownNPCSpawnRate(On.Terraria.Main.orig_UpdateTime_SpawnTownNPCs orig)
        {
            int oldWorldRate = Main.worldRate;
            Main.worldRate *= CalamityConfig.Instance.TownNPCSpawnRateMultiplier;
            orig();
            Main.worldRate = oldWorldRate;
        }
        #endregion Town NPC Spawning Improvements

        #region Removal of Black Belt Dodge RNG
        private static void RemoveRNGFromBlackBelt(ILContext il)
        {
            // Change the random chance of the Black Belt to 100%, but don't let it work if Calamity's cooldown is active.
            var cursor = new ILCursor(il);
            if (!cursor.TryGotoNext(MoveType.Before, i => i.MatchLdcI4(10))) // 1 in 10 Main.rand call for Black Belt activation.
            {
                LogFailure("No RNG Black Belt", "Could not locate the dodge chance.");
                return;
            }
            cursor.Remove();
            cursor.Emit(OpCodes.Ldc_I4_1); // Replace with Main.rand.Next(1), aka 100% chance.

            // Move forwards past the Main.rand.Next call now that it has been edited.
            if (!cursor.TryGotoNext(MoveType.After, i => i.MatchCallvirt<UnifiedRandom>("Next")))
            {
                LogFailure("No RNG Black Belt", "Could not locate the Random.Next call.");
                return;
            }

            // Load the player itself onto the stack so that it becomes an argument for the following delegate.
            cursor.Emit(OpCodes.Ldarg_0);

            // Emit a delegate which places the player's Calamity dodge cooldown onto the stack.
            // If your dodges are universally disabled by Armageddon, then they simply "never come off cooldown" and always have 1 frame left.
            cursor.EmitDelegate<Func<Player, int>>((Player p) =>
            {
                CalamityPlayer mp = p.Calamity();
                return mp.disableAllDodges ? 1 : mp.dodgeCooldownTimer;
            });

            // Bitwise OR the "RNG result" (always zero) with the dodge cooldown. This will only return zero if both values were zero.
            // The code path which calls NinjaDodge can ONLY occur if the result of this operation is zero,
            // because it is now the value checked by the immediately following branch-if-true.
            cursor.Emit(OpCodes.Or);

            // Move forwards past the NinjaDodge call. We need to set the dodge cooldown here.
            if (!cursor.TryGotoNext(MoveType.After, i => i.MatchCall<Player>("NinjaDodge")))
            {
                LogFailure("No RNG Black Belt", "Could not locate the Player.NinjaDodge call.");
                return;
            }

            // Load the player itself onto the stack so that it becomes an argument for the following delegate.
            cursor.Emit(OpCodes.Ldarg_0);

            // Emit a delegate which sets the player's Calamity dodge cooldown and sends a sync packet appropriately.
            cursor.EmitDelegate<Action<Player>>((Player p) =>
            {
                CalamityPlayer calPlayer = p.Calamity();
                calPlayer.dodgeCooldownTimer = CalamityPlayer.BeltDodgeCooldown;
                if (Main.netMode == NetmodeID.MultiplayerClient)
                    calPlayer.SyncDodgeCooldown(false);
            });
        }
        #endregion Removal of Black Belt Dodge RNG

        #region Vanilla Dash Shield Improvements
        private static void FixVanillaShieldSlams(ILContext il)
        {
            // Remove Shield of Cthulhu setting your iframes to an exact number and instead run Calamity's utility to safely provide iframes.
            var cursor = new ILCursor(il);
            if (!cursor.TryGotoNext(MoveType.After, i => i.MatchStfld<Player>("immuneNoBlink")))
            {
                LogFailure("Vanilla Shield Slam Fix", "Could not locate Shield of Cthulhu immunity blinking set.");
                return;
            }

            // Destroy the entire operation which sets your iframes to exactly 4:
            // ldarg.0 (load "this", aka the Player)
            // ldc.i4.4 (load 4)
            // stfld int32 Terraria.Player::immuneTime
            cursor.RemoveRange(3);

            // Load the player itself onto the stack so that it becomes an argument for the following delegate.
            cursor.Emit(OpCodes.Ldarg_0);
            cursor.EmitDelegate<Action<Player>>((Player p) => p.GiveIFrames(CalamityPlayer.ShieldOfCthulhuIFrames, false));

            // Move onto the next dash (Solar Flare set bonus) by looking for the base damage of the direct contact strike.
            if (!cursor.TryGotoNext(MoveType.Before, i => i.MatchLdcR4(150f)))
            {
                LogFailure("Vanilla Shield Slam Fix", "Could not locate Solar Flare Armor shield slam base damage.");
                return;
            }

            // Replace vanilla's base damage of 150 with Calamity's custom base damage.
            cursor.Next.Operand = CalamityPlayer.SolarFlareBaseDamage;

            // Now that the new base damage has been applied to the direct contact strike, also apply it to the Solar Counter projectile.
            if (!cursor.TryGotoNext(MoveType.Before, i => i.MatchLdcI4(150)))
            {
                LogFailure("Vanilla Shield Slam Fix", "Could not locate Solar Flare Armor \"Solar Counter\" base damage.");
                return;
            }

            // Replace vanilla's flat 150 damage (doesn't even scale with melee stats!) with Calamity's calculation.
            cursor.Remove();
            cursor.Emit(OpCodes.Ldloc, 12);
            cursor.Emit(OpCodes.Conv_I4);

            // Move to the immunity frame setting code for the Solar Flare set bonus.
            if (!cursor.TryGotoNext(MoveType.After, i => i.MatchStfld<Player>("immuneNoBlink")))
            {
                LogFailure("Vanilla Shield Slam Fix", "Could not locate Solar Flare Armor shield slam base damage.");
                return;
            }

            // Destroy the entire operation which sets your iframes to exactly 4:
            // ldarg.0 (load "this", aka the Player)
            // ldc.i4.4 (load 4)
            // stfld int32 Terraria.Player::immuneTime
            cursor.RemoveRange(3);

            // Load the player itself onto the stack so that it becomes an argument for the following delegate.
            cursor.Emit(OpCodes.Ldarg_0);
            cursor.EmitDelegate<Action<Player>>((Player p) => p.GiveIFrames(CalamityPlayer.SolarFlareIFrames, false));
        }

        private static void NerfShieldOfCthulhuBonkSafety(ILContext il)
        {
            // Reduce the number of "no-collide frames" (they are NOT iframes) granted by the Shield of Cthulhu bonk.
            var cursor = new ILCursor(il);
            if (!cursor.TryGotoNext(MoveType.Before, i => i.MatchLdfld<Player>("eocDash"))) // Loading the remaining frames of the SoC dash
            {
                LogFailure("Shield of Cthulhu Bonk Nerf", "Could not locate Shield of Cthulhu dash remaining frame counter.");
                return;
            }

            // Find the 0 this is normally compared to. We will be replacing this value.
            if (!cursor.TryGotoNext(MoveType.Before, i => i.MatchLdcI4(0)))
            {
                LogFailure("Shield of Cthulhu Bonk Nerf", "Could not locate the zero comparison.");
                return;
            }

            // Remove the zero and replace it with a calculated value.
            // This is the total length of the EoC bonk (10) minus the number of safe frames allowed by Calamity.
            cursor.Remove();
            cursor.Emit(OpCodes.Ldc_I4, 10 - CalamityPlayer.ShieldOfCthulhuBonkNoCollideFrames);
        }
        #endregion Vanilla Dash Shield Improvements

        #region Custom Gate Door Logic
        private static bool OpenDoor_LabDoorOverride(On.Terraria.WorldGen.orig_OpenDoor orig, int i, int j, int direction)
        {
            Tile tile = Main.tile[i, j];
            // If the tile is somehow null, that's vanilla's problem, we're outta here
            if (tile is null)
                return orig(i, j, direction);

            // If it's one of the two lab doors, use custom code to open the door and sync tiles in multiplayer.
            else if (tile.type == labDoorClosed)
                return OpenLabDoor(tile, i, j, labDoorOpen);
            else if (tile.type == aLabDoorClosed)
                return OpenLabDoor(tile, i, j, aLabDoorOpen);
            else if (tile.type == exoDoorClosed)
                return OpenLabDoor(tile, i, j, exoDoorOpen);

            // If it's anything else, let vanilla and/or TML handle it.
            return orig(i, j, direction);
        }

        private static bool CloseDoor_LabDoorOverride(On.Terraria.WorldGen.orig_CloseDoor orig, int i, int j, bool forced)
        {
            Tile tile = Main.tile[i, j];
            // If the tile is somehow null, that's vanilla's problem, we're outta here
            if (tile is null)
                return orig(i, j, forced);

            // If it's one of the two lab doors, use custom code to open the door and sync tiles in multiplayer.
            else if (tile.type == labDoorOpen)
                return CloseLabDoor(tile, i, j, labDoorClosed);
            else if (tile.type == aLabDoorOpen)
                return CloseLabDoor(tile, i, j, aLabDoorClosed);
            else if (tile.type == exoDoorOpen)
                return CloseLabDoor(tile, i, j, exoDoorClosed);

            // If it's anything else, let vanilla and/or TML handle it.
            return orig(i, j, forced);
        }
        #endregion Custom Gate Door Logic

        #region Platform Collision Checks for Grounded Bosses
        private static bool EnableCalamityBossPlatformCollision(On.Terraria.NPC.orig_Collision_DecideFallThroughPlatforms orig, NPC self)
        {
            if ((self.type == ModContent.NPCType<AstrumAureus>() || self.type == ModContent.NPCType<CrabulonIdle>() || self.type == ModContent.NPCType<RavagerBody>() ||
                self.type == ModContent.NPCType<RockPillar>() || self.type == ModContent.NPCType<FlamePillar>()) &&
                self.target >= 0 && Main.player[self.target].position.Y > self.position.Y + self.height)
                return true;

            return orig(self);
        }
        #endregion Platform Collision Checks for Grounded Bosses

        #region Teleporter Disabling During Boss Fights
        private static void DisableTeleporters(On.Terraria.Wiring.orig_Teleport orig)
        {
            if (CalamityPlayer.areThereAnyDamnBosses)
                return;

            orig();
        }
        #endregion Teleporter Disabling During Boss Fights

        #region Incorporate Enchantments in Item Names
        private static string IncorporateEnchantmentInAffix(On.Terraria.Item.orig_AffixName orig, Item self)
        {
            string result = orig(self);
            if (!self.IsAir && self.Calamity().AppliedEnchantment.HasValue)
                result = $"{self.Calamity().AppliedEnchantment.Value.Name} {result}";
            return result;
        }
        #endregion Incorporate Enchantments in Item Names

        #region Hellbound Enchantment Projectile Creation Effects
        private static int IncorporateMinionExplodingCountdown(On.Terraria.Projectile.orig_NewProjectile_float_float_float_float_int_int_float_int_float_float orig, float x, float y, float xSpeed, float ySpeed, int type, int damage, float knockback, int owner, float ai0, float ai1)
        {
            // This is unfortunately not something that can be done via SetDefaults since owner is set
            // after that method is called. Doing it directly when the projectile is spawned appears to be the only reasonable way.
            int proj = orig(x, y, xSpeed, ySpeed, type, damage, knockback, owner, ai0, ai1);
            Projectile projectile = Main.projectile[proj];
            if (projectile.minion)
            {
                Player player = Main.player[projectile.owner];
                CalamityPlayerMiscEffects.EnchantHeldItemEffects(player, player.Calamity(), player.ActiveItem());
                if (player.Calamity().explosiveMinionsEnchant)
                    projectile.Calamity().ExplosiveEnchantCountdown = CalamityGlobalProjectile.ExplosiveEnchantTime;
            }
            return proj;
        }
        #endregion Hellbound Enchantment Projectile Creation Effects

        #region Mana Sickness Replacement for Chaos Stone
        private static void ApplyManaBurnIfNeeded(ILContext il)
        {
            ILCursor cursor = new ILCursor(il);

            if (!cursor.TryGotoNext(c => c.MatchLdcI4(BuffID.ManaSickness)))
            {
                LogFailure("Mana Burn Application", "Could not locate the mana sickness buff ID.");
                return;
            }

            cursor.Remove();
            cursor.Emit(OpCodes.Ldarg_0);
            cursor.EmitDelegate<Func<Player, int>>(player =>
            {
                if (!player.active || !player.Calamity().ChaosStone)
                    return BuffID.ManaSickness;
                return ModContent.BuffType<ManaBurn>();
            });
        }

        private static void AllowBuffTimeStackingForManaBurn(ILContext il)
        {
            ILCursor cursor = new ILCursor(il);

            // Get a label that points to the final return before doing anything else.
            cursor.GotoFinalRet();

            ILLabel finalReturn = cursor.DefineLabel();
            cursor.MarkLabel(finalReturn);

            // After the label has been created go back to the beginning of the method.
            cursor.Goto(0);

            if (!cursor.TryGotoNext(MoveType.Before, c => c.MatchStloc(4)))
            {
                LogFailure("Mana Burn Time Stacking", "Could not locate the buff loop incremental variable.");
                return;
            }

            int startOfBuffTimeLogic = cursor.Index - 1;

            if (!cursor.TryGotoNext(MoveType.Before, c => c.MatchLdsfld<Main>("vanityPet")))
            {
                LogFailure("Mana Burn Time Stacking", "Could not locate the Main.vanityPet load.");
                return;
            }

            // Clear away the vanilla logic and re-add it with a delegate.
            // The alternative is a mess of various labels and branches.
            int endOfBuffTimeLogic = cursor.Index;
            cursor.Goto(startOfBuffTimeLogic);
            cursor.RemoveRange(endOfBuffTimeLogic - startOfBuffTimeLogic);

            cursor.Emit(OpCodes.Ldarg_0);
            cursor.Emit(OpCodes.Ldarg_1);
            cursor.Emit(OpCodes.Ldloc_0);
            cursor.EmitDelegate<Func<Player, int, int, bool>>((player, type, buffTime) =>
            {
                for (int j = 0; j < Player.MaxBuffs; j++)
                {
                    if (player.buffType[j] == type)
                    {
                        if (!BuffLoader.ReApply(type, player, buffTime, j))
                        {
                            if (type == BuffID.ManaSickness || type == ModContent.BuffType<ManaBurn>())
                            {
                                player.buffTime[j] += buffTime;
                                if (player.buffTime[j] > Player.manaSickTimeMax)
                                    player.buffTime[j] = Player.manaSickTimeMax;

                            }
                            else if (player.buffTime[j] < buffTime)
                                player.buffTime[j] = buffTime;
                        }
                        return true;
                    }
                }
                return false;
            });
            cursor.Emit(OpCodes.Brtrue, finalReturn);
        }
        #endregion Mana Sickness Replacement for Chaos Stone

        #region Fire Cursor Effect for the Calamity Accessory
        private static void UseCoolFireCursorEffect(On.Terraria.Main.orig_DrawCursor orig, Vector2 bonus, bool smart)
        {
            // Do nothing special if the player has a regular mouse or is on the menu.
            if (Main.gameMenu || !Main.LocalPlayer.Calamity().blazingCursorVisuals)
            {
                orig(bonus, smart);
                return;
            }

            if (Main.LocalPlayer.dead)
            {
                Main.SmartInteractShowingGenuine = false;
                Main.SmartInteractShowingFake = false;
                Main.SmartInteractNPC = -1;
                Main.SmartInteractNPCsNearby.Clear();
                Main.SmartInteractTileCoords.Clear();
                Main.SmartInteractTileCoordsSelected.Clear();
                Main.TileInteractionLX = (Main.TileInteractionHX = (Main.TileInteractionLY = (Main.TileInteractionHY = -1)));
            }

            Color flameColor = Color.Lerp(Color.DarkRed, Color.OrangeRed, (float)Math.Cos(Main.GlobalTime * 7.4f) * 0.5f + 0.5f);
            Color cursorColor = flameColor * 1.9f;
            Vector2 baseDrawPosition = Main.MouseScreen + bonus;

            // Draw the mouse as usual if the player isn't using the gamepad.
            if (!PlayerInput.UsingGamepad)
            {
                int cursorIndex = smart.ToInt();

                Color desaturatedCursorColor = cursorColor;
                desaturatedCursorColor.R /= 5;
                desaturatedCursorColor.G /= 5;
                desaturatedCursorColor.B /= 5;
                desaturatedCursorColor.A /= 2;

                Vector2 drawPosition = baseDrawPosition;
                Vector2 desaturatedDrawPosition = drawPosition + Vector2.One;

                // If the blazing mouse is actually going to do damage, draw an indicator aura.
                if (Main.LocalPlayer.Calamity().blazingCursorDamage && !Main.mapFullscreen)
                {
                    Texture2D auraTexture = ModContent.GetTexture("CalamityMod/ExtraTextures/CalamityAura");
                    Rectangle auraFrame = auraTexture.Frame(1, 6, 0, (int)(Main.GlobalTime * 12.3f) % 6);
                    float auraScale = MathHelper.Lerp(0.95f, 1f, (float)Math.Sin(Main.GlobalTime * 1.1f) * 0.5f + 0.5f);

                    for (int i = 0; i < 12; i++)
                    {
                        Color auraColor = Color.Orange * Main.LocalPlayer.Calamity().blazingMouseAuraFade * 0.125f;
                        auraColor.A = 0;
                        Vector2 offsetDrawPosition = baseDrawPosition + (MathHelper.TwoPi * i / 12f + Main.GlobalTime * 5f).ToRotationVector2() * 2.5f;
                        offsetDrawPosition.Y -= 18f;

                        Main.spriteBatch.Draw(auraTexture, offsetDrawPosition, auraFrame, auraColor, 0f, auraFrame.Size() * 0.5f, Main.cursorScale * auraScale, SpriteEffects.None, 0f);
                    }
                }

                Main.spriteBatch.Draw(Main.cursorTextures[cursorIndex], drawPosition, null, desaturatedCursorColor, 0f, Vector2.Zero, Main.cursorScale, SpriteEffects.None, 0f);

                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.instance.Rasterizer, null, Main.UIScaleMatrix);
                GameShaders.Misc["CalamityMod:FireMouse"].UseColor(Color.Red);
                GameShaders.Misc["CalamityMod:FireMouse"].UseSecondaryColor(Color.Lerp(Color.Red, Color.Orange, 0.75f));
                GameShaders.Misc["CalamityMod:FireMouse"].Apply();

                Main.spriteBatch.Draw(Main.cursorTextures[cursorIndex], desaturatedDrawPosition, null, cursorColor, 0f, Vector2.Zero, Main.cursorScale * 1.075f, SpriteEffects.None, 0f);
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.instance.Rasterizer, null, Main.GameViewMatrix.EffectMatrix);
                return;
            }

            // Don't bother doing any more drawing if the player is dead.
            if (Main.LocalPlayer.dead && !Main.LocalPlayer.ghost && !Main.gameMenu)
                return;

            if (PlayerInput.InvisibleGamepadInMenus)
                return;

            // Draw a white circle instead if using the smart cursor.
            if (smart && !(UILinkPointNavigator.Available && !PlayerInput.InBuildingMode))
            {
                cursorColor = Color.White * Main.GamepadCursorAlpha;
                int frameX = 0;
                Texture2D smartCursorTexture = Main.cursorTextures[13];
                Rectangle frame = smartCursorTexture.Frame(2, 1, frameX, 0);
                Main.spriteBatch.Draw(smartCursorTexture, baseDrawPosition, frame, cursorColor, 0f, frame.Size() * 0.5f, Main.cursorScale, SpriteEffects.None, 0f);
                return;
            }

            // Otherwise draw an ordinary crosshair at the mouse position.
            cursorColor = Color.White;
            Texture2D crosshairTexture = Main.cursorTextures[15];
            Main.spriteBatch.Draw(crosshairTexture, baseDrawPosition, null, cursorColor, 0f, crosshairTexture.Size() * 0.5f, Main.cursorScale, SpriteEffects.None, 0f);
        }
        #endregion Fire Cursor Effect for the Calamity Accessory

        #region Fusable Particle Rendering
        private static void DrawFusableParticles(ILContext il)
        {
            ILCursor cursor = new ILCursor(il);

            // Over NPCs but before Projectiles.
            if (!cursor.TryGotoNext(c => c.MatchCallOrCallvirt<Main>("SortDrawCacheWorms")))
            {
                LogFailure("Fusable Particle Rendering", "Could not locate the SortDrawCacheWorms reference method to attach to.");
                return;
            }
            cursor.EmitDelegate<Action>(() => FusableParticleManager.RenderAllFusableParticles(FusableParticleRenderLayer.OverNPCsBeforeProjectiles));

            // Over Players.
            if (!cursor.TryGotoNext(MoveType.After, c => c.MatchCallOrCallvirt<Main>("DrawPlayers")))
            {
                LogFailure("Fusable Particle Rendering", "Could not locate the DrawPlayers reference method to attach to.");
                return;
            }
            cursor.EmitDelegate<Action>(() => FusableParticleManager.RenderAllFusableParticles(FusableParticleRenderLayer.OverPlayers));

            // Over Water.
            if (!cursor.TryGotoNext(c => c.MatchCallOrCallvirt<MoonlordDeathDrama>("DrawWhite")))
            {
                LogFailure("Fusable Particle Rendering", "Could not locate the DrawWhite reference method to attach to.");
                return;
            }
            cursor.EmitDelegate<Action>(() => FusableParticleManager.RenderAllFusableParticles(FusableParticleRenderLayer.OverWater));
        }
        #endregion Fusable Particle Rendering

        #region Ash Particle Rendering
        private static void DrawAshParticles(ILContext il)
        {
            ILCursor cursor = new ILCursor(il);

            // Over NPCs but before Projectiles.
            if (!cursor.TryGotoNext(c => c.MatchCallOrCallvirt<Main>("DrawDust")))
            {
                LogFailure("Ash Particle Rendering", "Could not locate the DrawDust reference method to attach to.");
                return;
            }
            cursor.EmitDelegate<Action>(DeathAshParticle.DrawAll);
        }
        #endregion Ash Particle Rendering

        #region General Particle Rendering
        private static void DrawParticles(On.Terraria.Main.orig_DrawInterface orig, Main self, GameTime gameTime)
        {
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, default, default, default, default, Main.GameViewMatrix.ZoomMatrix);
            GeneralParticleHandler.DrawAllParticles(Main.spriteBatch);
            Main.spriteBatch.End();

            orig(self, gameTime);
        }

        #endregion General Particle Rendering

        #region Custom Lava Visuals
        private static void ResetRenderTargetSizes(On.Terraria.Main.orig_SetDisplayMode orig, int width, int height, bool fullscreen)
        {
            FusableParticleManager.LoadParticleRenderSets(true, width, height);
            orig(width, height, fullscreen);
        }

        private static void DrawCustomLava(ILContext il)
        {
            ILCursor cursor = new ILCursor(il);

            if (!cursor.TryGotoNext(c => c.MatchLdsfld<Main>("liquidTexture")))
            {
                LogFailure("Custom Lava Drawing", "Could not locate the liquid texture array load.");
                return;
            }

            // While this may seem crazy, under no circumstances should there not be a load after exactly 3 instructions.
            // The order is load is texture array field -> load index -> load the reference to the texture at that index.
            cursor.Index += 3;
            cursor.EmitDelegate<Func<Texture2D, Texture2D>>(initialTexture => SelectLavaTexture(initialTexture, true));

            if (!cursor.TryGotoNext(MoveType.After, c => c.MatchLdloc(155)))
            {
                LogFailure("Custom Lava Drawing", "Could not locate the liquid light color.");
                return;
            }

            // Pass the texture in so that the method can ensure it is not messing around with non-lava textures.
            cursor.Emit(OpCodes.Ldsfld, typeof(Main).GetField("liquidTexture"));
            cursor.Emit(OpCodes.Ldloc, 151);
            cursor.Emit(OpCodes.Ldelem_Ref);
            cursor.EmitDelegate<Func<Color, Texture2D, Color>>((initialColor, initialTexture) => SelectLavaColor(initialTexture, initialColor));
        }

        private static void DrawCustomLava2(ILContext il)
        {
            ILCursor cursor = new ILCursor(il);

            if (!cursor.TryGotoNext(c => c.MatchLdfld<LiquidRenderer>("_liquidTextures")))
            {
                LogFailure("Custom Lava Drawing", "Could not locate the liquid texture array load.");
                return;
            }

            // While this may seem crazy, under no circumstances should there not be a load after exactly 3 instructions.
            // The order is load is texture array field -> load index -> load the reference to the texture at that index.
            cursor.Index += 3;
            cursor.EmitDelegate<Func<Texture2D, Texture2D>>(initialTexture => SelectLavaTexture(initialTexture, false));

            if (!cursor.TryGotoNext(MoveType.After, c => c.MatchLdloc(9)))
            {
                LogFailure("Custom Lava Drawing", "Could not locate the liquid light color.");
                return;
            }

            // Pass the texture in so that the method can ensure it is not messing around with non-lava textures.
            cursor.Emit(OpCodes.Ldarg_0);
            cursor.Emit(OpCodes.Ldfld, typeof(LiquidRenderer).GetField("_liquidTextures"));
            cursor.Emit(OpCodes.Ldloc, 8);
            cursor.Emit(OpCodes.Ldelem_Ref);
            cursor.EmitDelegate<Func<VertexColors, Texture2D, VertexColors>>((initialColor, initialTexture) =>
            {
                initialColor.TopLeftColor = SelectLavaColor(initialTexture, initialColor.TopLeftColor);
                initialColor.TopRightColor = SelectLavaColor(initialTexture, initialColor.TopRightColor);
                initialColor.BottomLeftColor = SelectLavaColor(initialTexture, initialColor.BottomLeftColor);
                initialColor.BottomRightColor = SelectLavaColor(initialTexture, initialColor.BottomRightColor);
                return initialColor;
            });
        }

        private static void DrawCustomLava3(ILContext il)
        {
            ILCursor cursor = new ILCursor(il);

            // Select the lava color.
            if (!cursor.TryGotoNext(MoveType.After, c => c.MatchCallOrCallvirt<Lighting>("get_NotRetro")))
            {
                LogFailure("Custom Lava Drawing", "Could not locate the retro style check.");
                return;
            }

            // Pass the texture in so that the method can ensure it is not messing around with non-lava textures.
            cursor.Emit(OpCodes.Ldloc, 13);
            cursor.Emit(OpCodes.Ldsfld, typeof(Main).GetField("liquidTexture"));
            cursor.Emit(OpCodes.Ldloc, 15);
            cursor.Emit(OpCodes.Ldelem_Ref);
            cursor.EmitDelegate<Func<Color, Texture2D, Color>>((initialColor, initialTexture) => SelectLavaColor(initialTexture, initialColor));
            cursor.Emit(OpCodes.Stloc, 13);

            // Go back to the start and change textures as necessary.
            cursor.Index = 0;

            while (cursor.TryGotoNext(c => c.MatchLdsfld<Main>("liquidTexture")))
            {
                // While this may seem crazy, under no circumstances should there not be a load after exactly 3 instructions.
                // The order is load is texture array field -> load index -> load the reference to the texture at that index.
                cursor.Index += 3;
                cursor.EmitDelegate<Func<Texture2D, Texture2D>>(initialTexture => SelectLavaTexture(initialTexture, true));
            }
        }

        private static void DrawCustomLavafalls(ILContext il)
        {
            ILCursor cursor = new ILCursor(il);

            // Search for the color and alter it based on the same conditions as the lava.
            if (!cursor.TryGotoNext(c => c.MatchCallOrCallvirt(typeof(Color).GetConstructor(new Type[] { typeof(int), typeof(int), typeof(int), typeof(int) }))))
            {
                LogFailure("Custom Lavafall Drawing", "Could not locate the waterfall color.");
                return;
            }

            // Determine the waterfall type. This happens after all the "If Lava do blahblahblah" color checks, meaning it will have the same
            // color properties as lava.
            cursor.Emit(OpCodes.Ldloc, 12);
            cursor.EmitDelegate<Func<int, int>>(initialWaterfallStyle => CustomLavaManagement.SelectLavafallStyle(initialWaterfallStyle));
            cursor.Emit(OpCodes.Stloc, 12);

            cursor.Emit(OpCodes.Ldloc, 12);
            cursor.Emit(OpCodes.Ldloc, 51);
            cursor.EmitDelegate<Func<int, Color, Color>>((initialWaterfallStyle, initialLavafallColor) => CustomLavaManagement.SelectLavafallColor(initialWaterfallStyle, initialLavafallColor));
            cursor.Emit(OpCodes.Stloc, 51);
        }
        #endregion Custom Lava Visuals

		#region Statue Additions
		/// <summary>
		/// Change the following code sequence in Wiring.HitWireSingle
		/// num8 = (int) Utils.SelectRandom<short>(Main.rand, new short[2]
		/// {
		/// 	355,
		/// 	358
		/// });
		/// 
		/// to 
		/// 
		/// var arr = new short[2]
		/// {
		/// 	355,
		/// 	358
		/// });
		/// arr = arr.ToList().Add(id).ToArray();
		/// num8 = Utils.SelectRandom(Main.rand, arr);
		/// 
		/// </summary>
		/// <param name="il"></param>
		private static void AddTwinklersToStatue(ILContext il)
		{
			// obtain a cursor positioned before the first instruction of the method
			// the cursor is used for navigating and modifying the il
			var c = new ILCursor(il);

			// the exact location for this hook is very complex to search for due to the hook instructions not being unique, and buried deep in control flow
			// switch statements are sometimes compiled to if-else chains, and debug builds litter the code with no-ops and redundant locals

			// in general you want to search using structure and function rather than numerical constants which may change across different versions or compile settings
			// using local variable indices is almost always a bad idea

			// we can search for
			// switch (*)
			//   case 54:
			//     Utils.SelectRandom *

			// in general you'd want to look for a specific switch variable, or perhaps the containing switch (type) { case 105:
			// but the generated IL is really variable and hard to match in this case

			// we'll just use the fact that there are no other switch statements with case 54, followed by a SelectRandom

			ILLabel[] targets = null;
			while (c.TryGotoNext(i => i.MatchSwitch(out targets)))
			{
				// some optimising compilers generate a sub so that all the switch cases start at 0
				// ldc.i4.s 51
				// sub
				// switch
				int offset = 0;
				if (c.Prev.MatchSub() && c.Prev.Previous.MatchLdcI4(out offset))
				{
					;
				}

				// get the label for case 54: if it exists
				int case54Index = 54 - offset;
				if (case54Index < 0 || case54Index >= targets.Length || !(targets[case54Index] is ILLabel target))
				{
					continue;
				}

				// move the cursor to case 54:
				c.GotoLabel(target);
				// there's lots of extra checks we could add here to make sure we're at the right spot, such as not encountering any branching instructions
				c.GotoNext(i => i.MatchCall(typeof(Utils), nameof(Utils.SelectRandom)));

				// goto next positions us before the instruction we searched for, so we can insert our array modifying code right here
				c.EmitDelegate<Func<short[], short[]>>(arr =>
				{
					// resize the array and add our custom firefly
					Array.Resize(ref arr, arr.Length+1);
					arr[arr.Length-1] = (short)ModContent.NPCType<Twinkler>();
					return arr;
				});

				// hook applied successfully
				return;
			}

			// couldn't find the right place to insert
			throw new Exception("Hook location not found, switch(*) { case 54: ...");
		}
		#endregion Statue Additions
    }
}
