using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Items.Accessories;
using CalamityMod.Items.Armor.Vanity;
using CalamityMod.Items.LoreItems;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Furniture.Trophies;
using CalamityMod.Items.TreasureBags;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.Projectiles.Boss;
using CalamityMod.Tiles.Ores;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;
using CalamityMod;
/* states:
 * 0 = slow drift
 * 1 = reelback and teleport after spawn enemy
 * 2 = reelback for spin lunge + death legacy
 * 3 = spin lunge
 * 4 = semicircle spawn arc
 * 5 = raindash
 * 6 = deceleration
 */

namespace CalamityMod.NPCs.HiveMind
{
    [AutoloadBossHead]
    public class HiveMindP2 : ModNPC
    {
        //this block of values can be modified in SetDefaults() based on difficulty mode or something
        int minimumDriftTime = 300;
        int teleportRadius = 300;
        int decelerationTime = 30;
        int reelbackFade = 3;       //divide 255 by this for duration of reelback in ticks
        float arcTime = 45f;        //ticks needed to complete movement for spawn and rain attacks (DEATH ONLY)
        float driftSpeed = 2f;      //default speed when slowly floating at player
        float driftBoost = 1f;      //max speed added as health decreases
        int lungeDelay = 90;        //# of ticks long hive mind spends sliding to a stop before lunging
        int lungeTime = 30;
        int lungeFade = 15;         //divide 255 by this for duration of hive mind spin before slowing for lunge
        double lungeRots = 0.2;     //number of revolutions made while spinning/fading in for lunge
        bool dashStarted = false;
        int phase2timer = 360;
        int rotationDirection;
        double rotation;
        double rotationIncrement;
        int state = 0;
        int previousState = 0;
        int nextState = 0;
        int reelCount = 0;
        Vector2 deceleration;
        int counter = 0;
        bool initialised = false;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("The Hive Mind");
            Main.npcFrameCount[npc.type] = 16;
        }

        public override void SetDefaults()
        {
            npc.npcSlots = 5f;
            npc.damage = 35;
            npc.width = 177;
            npc.height = 142;
            npc.defense = 5;
            npc.LifeMaxNERB(5800, 7560, 3000000);
            double HPBoost = CalamityMod.CalamityConfig.BossHealthPercentageBoost * 0.01;
            npc.lifeMax += (int)(npc.lifeMax * HPBoost);
            npc.aiStyle = -1;
            aiType = -1;
            npc.buffImmune[ModContent.BuffType<GlacialState>()] = true;
            npc.buffImmune[ModContent.BuffType<TemporalSadness>()] = true;
            npc.knockBackResist = 0f;
            npc.value = Item.buyPrice(0, 6, 0, 0);
            npc.boss = true;
            npc.noGravity = true;
            npc.noTileCollide = true;
            npc.HitSound = SoundID.NPCHit1;
            npc.DeathSound = SoundID.NPCDeath1;
            Mod calamityModMusic = ModLoader.GetMod("CalamityModMusic");
            if (calamityModMusic != null)
                music = calamityModMusic.GetSoundSlot(SoundType.Music, "Sounds/Music/HiveMind");
            else
                music = MusicID.Boss2;
            bossBag = ModContent.ItemType<HiveMindBag>();
            NPCID.Sets.TrailCacheLength[npc.type] = 8;
            NPCID.Sets.TrailingMode[npc.type] = 1;
            if (Main.expertMode)
            {
                minimumDriftTime = 120;
                reelbackFade = 5;
            }
            if (CalamityWorld.revenge)
            {
                lungeRots = 0.3;
                minimumDriftTime = 90;
                reelbackFade = 6;
                lungeTime = 25;
                driftSpeed = 3f;
                driftBoost = 2f;
            }
            if (CalamityWorld.death)
            {
                lungeRots = 0.4;
                minimumDriftTime = 60;
                reelbackFade = 7;
                lungeTime = 20;
                driftSpeed = 4f;
                driftBoost = 1f;
            }
            if (CalamityWorld.bossRushActive)
            {
                lungeRots = 0.4;
                minimumDriftTime = 20;
                reelbackFade = 15;
                lungeTime = 10;
                driftSpeed = 12f;
            }
            phase2timer = minimumDriftTime;
            rotationIncrement = 0.0246399424 * lungeRots * lungeFade;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(state);
            writer.Write(nextState);
            writer.Write(phase2timer);
            writer.Write(dashStarted);
            writer.Write(rotationDirection);
            writer.Write(rotation);
            writer.Write(previousState);
            writer.Write(reelCount);
            writer.Write(counter);
            writer.Write(initialised);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            state = reader.ReadInt32();
            nextState = reader.ReadInt32();
            phase2timer = reader.ReadInt32();
            dashStarted = reader.ReadBoolean();
            rotationDirection = reader.ReadInt32();
            rotation = reader.ReadDouble();
            previousState = reader.ReadInt32();
            reelCount = reader.ReadInt32();
            counter = reader.ReadInt32();
            initialised = reader.ReadBoolean();
        }

        public override void FindFrame(int frameHeight)
        {
            int width = npc.width;
            int height = npc.height;

            if (!initialised)
            {
                counter = 8;
                npc.frameCounter = 6;
                initialised = true;
            }

            //ensure width and height are set.
            npc.frame.Width = width;
            npc.frame.Height = height;
            npc.frameCounter++;
            if (npc.frameCounter > 6)
            {
                npc.frame.X = counter >= 8 ? width + 3 : 0;
                if (counter == 8)
                    npc.frame.Y = 0;
                else
                    npc.frame.Y += height;
                npc.frameCounter = 0;
                counter++;
            }
            if (counter == 16)
            {
                counter = 1;
                npc.frame.Y = 0;
                npc.frame.X = 0;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            SpriteEffects spriteEffects = SpriteEffects.None;
            Color color24 = lightColor;
            color24 = npc.GetAlpha(color24);
            Color color25 = Lighting.GetColor((int)((double)npc.position.X + (double)npc.width * 0.5) / 16, (int)(((double)npc.position.Y + (double)npc.height * 0.5) / 16.0));
            Texture2D texture2D3 = ModContent.GetTexture("CalamityMod/NPCs/HiveMind/HiveMindP2");
            int num156 = Main.npcTexture[npc.type].Height / 8;
            Rectangle rectangle = new Rectangle(npc.frame.X, npc.frame.Y, npc.frame.X, num156);
            Vector2 origin2 = rectangle.Size() / 2f;
            int num157 = 8;
            int num158 = 2;
            int num159 = 1;
            float num160 = 0f;
            int num161 = num159;
            while (state != 0 && Lighting.NotRetro && ((num158 > 0 && num161 < num157) || (num158 < 0 && num161 > num157)))
            {
                Color color26 = color25;
                color26 = npc.GetAlpha(color26);
                {
                    goto IL_6899;
                }
                IL_6881:
                num161 += num158;
                continue;
                IL_6899:
                float num164 = (float)(num157 - num161);
                if (num158 < 0)
                {
                    num164 = (float)(num159 - num161);
                }
                color26 *= num164 / ((float)NPCID.Sets.TrailCacheLength[npc.type] * 1.5f);
                Vector2 value4 = npc.oldPos[num161];
                float num165 = npc.rotation;
                SpriteEffects effects = spriteEffects;
                Main.spriteBatch.Draw(texture2D3, value4 + npc.Size / 2f - Main.screenPosition + new Vector2(0, npc.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color26, num165 + npc.rotation * num160 * (float)(num161 - 1) * -(float)spriteEffects.HasFlag(SpriteEffects.FlipHorizontally).ToDirectionInt(), origin2, npc.scale, effects, 0f);
                goto IL_6881;
            }



            var something = npc.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            spriteBatch.Draw(Main.npcTexture[npc.type], npc.Center - Main.screenPosition + new Vector2(0, npc.gfxOffY), npc.frame, color24, npc.rotation, npc.frame.Size() / 2, npc.scale, something, 0);
            return false;
        }

        private void SpawnStuff()
        {
            Player player = Main.player[npc.target];
            for (int i = 0; i < 5; i++)
            {
                bool spawnedSomething = false;
                int type = NPCID.EaterofSouls;
                int maxAmount = 0;
                int random = Collision.CanHit(npc.Center, 1, 1, player.position, player.width, player.height) ? 6 : 4;
                switch (Main.rand.Next(random))
                {
                    case 0:
                        type = NPCID.DevourerHead;
                        maxAmount = 1;
                        break;
                    case 1:
                        type = ModContent.NPCType<DankCreeper>();
                        maxAmount = 1;
                        break;
                    case 2:
                        type = ModContent.NPCType<DankCreeper>();
                        maxAmount = 1;
                        break;
                    case 3:
                        type = ModContent.NPCType<HiveBlob2>();
                        maxAmount = 2;
                        break;
                    case 4:
                        type = NPCID.EaterofSouls;
                        maxAmount = 2;
                        break;
                    case 5:
                        type = ModContent.NPCType<DarkHeart>();
                        maxAmount = 1;
                        break;
                }
                int numToSpawn = maxAmount - NPC.CountNPCS(type);
                while (numToSpawn > 0)
                {
                    numToSpawn--;
                    spawnedSomething = true;
                    int spawn = NPC.NewNPC((int)npc.position.X + Main.rand.Next(npc.width), (int)npc.position.Y + Main.rand.Next(npc.height), type);
                    Main.npc[spawn].velocity.X = (float)Main.rand.Next(-15, 16) * 0.1f;
                    Main.npc[spawn].velocity.Y = (float)Main.rand.Next(-30, 1) * 0.1f;
                }
                if (spawnedSomething)
                    return;
            }
        }

        private void ReelBack()
        {
            npc.alpha = 0;
            phase2timer = 0;
            deceleration = npc.velocity / 255f * reelbackFade;
            if (CalamityWorld.revenge || CalamityWorld.bossRushActive)
            {
                state = 2;
                Main.PlaySound(36, (int)npc.Center.X, (int)npc.Center.Y, -1, 1f, 0f);
            }
            else
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                    SpawnStuff();
                state = nextState;
                nextState = 0;
                if (state == 2)
                {
                    Main.PlaySound(15, (int)npc.Center.X, (int)npc.Center.Y, 0);
                }
                else
                {
                    Main.PlaySound(36, (int)npc.Center.X, (int)npc.Center.Y, -1, 1f, 0f);
                }
            }
        }

        public override void AI()
        {
			// Target
			if (npc.target < 0 || npc.target == 255 || Main.player[npc.target].dead || !Main.player[npc.target].active)
				npc.TargetClosest(true);

			Player player = Main.player[npc.target];

            npc.defense = (player.ZoneCorrupt || CalamityWorld.bossRushActive) ? 5 : 9999;

            CalamityGlobalNPC.hiveMind = npc.whoAmI;

            if (npc.alpha != 0)
            {
                if (npc.damage != 0)
                {
                    npc.damage = 0;
                }
            }
            else
            {
                npc.damage = npc.defDamage;
            }
            switch (state)
            {
                case 0: //slowdrift
                    if (npc.alpha > 0)
                        npc.alpha -= 3;
                    if (nextState == 0)
                    {
						npc.TargetClosest(true);
						if ((CalamityWorld.revenge && (double)npc.life < (double)npc.lifeMax * 0.66) || CalamityWorld.death || CalamityWorld.bossRushActive)
                        {
							if (CalamityWorld.death || CalamityWorld.bossRushActive)
							{
								do
									nextState = Main.rand.Next(3, 6);
								while (nextState == previousState);
								previousState = nextState;
							}
							else if ((double)npc.life < (double)npc.lifeMax * 0.33)
							{
								do
									nextState = Main.rand.Next(3, 6);
								while (nextState == previousState);
								previousState = nextState;
							}
							else
							{
								do
									nextState = Main.rand.Next(3, 5);
								while (nextState == previousState);
								previousState = nextState;
							}
                        }
                        else
                        {
                            if (CalamityWorld.revenge && (Main.rand.NextBool(3) || reelCount == 2))
                            {
                                reelCount = 0;
                                nextState = 2;
                            }
                            else
                            {
                                reelCount++;
								if (Main.expertMode && reelCount == 2)
								{
									reelCount = 0;
									nextState = 2;
								}
								else
									nextState = 1;

                                npc.ai[1] = 0f;
                                npc.ai[2] = 0f;
                            }
                        }
                        if (nextState == 3)
                            rotation = MathHelper.ToRadians(Main.rand.Next(360));
                        npc.netUpdate = true;
                    }

                    if (!player.active || player.dead || Vector2.Distance(npc.Center, player.Center) > 5000f)
                    {
                        npc.TargetClosest(false);
						player = Main.player[npc.target];
						if (!player.active || player.dead || Vector2.Distance(npc.Center, player.Center) > 5000f)
						{
							if (npc.timeLeft > 60)
								npc.timeLeft = 60;
							if (npc.localAI[3] < 120f)
							{
								float[] aiArray = npc.localAI;
								int number = 3;
								float num244 = aiArray[number];
								aiArray[number] = num244 + 1f;
							}
							if (npc.localAI[3] > 60f)
							{
								npc.velocity.Y += (npc.localAI[3] - 60f) * 0.5f;
							}
							return;
						}
                    }
					else if (npc.timeLeft < 1800)
						npc.timeLeft = 1800;

					if (npc.localAI[3] > 0f)
                    {
                        float[] aiArray = npc.localAI;
                        int number = 3;
                        float num244 = aiArray[number];
                        aiArray[number] = num244 - 1f;
                        return;
                    }

                    npc.velocity = player.Center - npc.Center;
                    phase2timer--;
                    if (phase2timer <= -180) //no stalling drift mode forever
                    {
                        npc.velocity *= 2f / 255f * reelbackFade;
                        ReelBack();
                        npc.netUpdate = true;
                    }
                    else
                    {
                        npc.velocity.Normalize();
                        if (Main.expertMode || CalamityWorld.bossRushActive) //variable velocity in expert and up
                        {
                            npc.velocity *= driftSpeed + driftBoost * (npc.lifeMax - npc.life) / npc.lifeMax;
                        }
                        else
                        {
                            npc.velocity *= driftSpeed;
                        }
                    }
                    break;
                case 1: //reelback and teleport
                    npc.alpha += reelbackFade;
                    npc.velocity -= deceleration;
                    if (npc.alpha >= 255)
                    {
                        npc.alpha = 255;
                        npc.velocity = Vector2.Zero;
                        state = 0;
                        if (Main.netMode != NetmodeID.MultiplayerClient && npc.ai[1] != 0f && npc.ai[2] != 0f)
                        {
                            npc.position.X = npc.ai[1] * 16 - npc.width / 2;
                            npc.position.Y = npc.ai[2] * 16 - npc.height / 2;
                        }
                        phase2timer = minimumDriftTime + Main.rand.Next(121);
                        npc.netUpdate = true;
                    }
                    else if (npc.ai[1] == 0f && npc.ai[2] == 0f)
                    {
                        for (int i = 0; i < 10; i++)
                        {
                            int posX = (int)player.Center.X / 16 + Main.rand.Next(15, 46) * (Main.rand.NextBool(2) ? -1 : 1);
                            int posY = (int)player.Center.Y / 16 + Main.rand.Next(15, 46) * (Main.rand.NextBool(2) ? -1 : 1);
                            if (!WorldGen.SolidTile(posX, posY) && Collision.CanHit(new Vector2(posX * 16, posY * 16), 1, 1, player.position, player.width, player.height))
                            {
                                npc.ai[1] = posX;
                                npc.ai[2] = posY;
                                npc.netUpdate = true;
                                break;
                            }
                        }
                    }
                    break;
                case 2: //reelback for lunge + death legacy
                    npc.alpha += reelbackFade;
                    npc.velocity -= deceleration;
                    if (npc.alpha >= 255)
                    {
                        npc.alpha = 255;
                        npc.velocity = Vector2.Zero;
                        dashStarted = false;
                        if ((CalamityWorld.revenge && (double)npc.life < (double)npc.lifeMax * 0.66) || CalamityWorld.death || CalamityWorld.bossRushActive)
                        {
							state = nextState;
                            nextState = 0;
                            previousState = state;
                        }
                        else
                        {
                            state = 3;
                        }
                        if (player.velocity.X > 0)
                            rotationDirection = 1;
                        else if (player.velocity.X < 0)
                            rotationDirection = -1;
                        else
                            rotationDirection = player.direction;
                    }
                    break;
                case 3: //lunge
                    npc.netUpdate = true;
                    if (npc.alpha > 0)
                    {
                        npc.alpha -= lungeFade;
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            npc.Center = player.Center + new Vector2(teleportRadius, 0).RotatedBy(rotation);
                        }
                        rotation += rotationIncrement * rotationDirection;
                        phase2timer = lungeDelay;
                    }
                    else
                    {
                        phase2timer--;
                        if (!dashStarted)
                        {
                            if (phase2timer <= 0)
                            {
                                phase2timer = lungeTime;
                                npc.velocity = player.Center - npc.Center;
                                npc.velocity.Normalize();
                                npc.velocity *= teleportRadius / lungeTime;
                                dashStarted = true;
                                Main.PlaySound(15, (int)npc.Center.X, (int)npc.Center.Y, 0);
                            }
                            else
                            {
                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                {
                                    npc.Center = player.Center + new Vector2(teleportRadius, 0).RotatedBy(rotation);
                                }
                                rotation += rotationIncrement * rotationDirection * phase2timer / lungeDelay;
                            }
                        }
                        else
                        {
                            if (phase2timer <= 0)
                            {
                                state = 6;
                                phase2timer = 0;
                                deceleration = npc.velocity / decelerationTime;
                            }
                        }
                    }
                    break;
                case 4: //enemy spawn arc
                    if (npc.alpha > 0)
                    {
                        npc.alpha -= 5;
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            npc.Center = player.Center;
                            npc.position.Y += teleportRadius;
                        }
                        npc.netUpdate = true;
                    }
                    else
                    {
                        if (!dashStarted)
                        {
                            dashStarted = true;
                            Main.PlaySound(15, (int)npc.Center.X, (int)npc.Center.Y, 0);
                            npc.velocity.X = 3.14159265f * teleportRadius / arcTime;
                            npc.velocity *= rotationDirection;
                            npc.netUpdate = true;
                        }
                        else
                        {
                            npc.velocity = npc.velocity.RotatedBy(3.14159265 / arcTime * -rotationDirection);
                            phase2timer++;
                            if (phase2timer == (int)arcTime / 6)
                            {
                                phase2timer = 0;
                                npc.ai[0]++;
                                if (Main.netMode != NetmodeID.MultiplayerClient && Collision.CanHit(npc.Center, 1, 1, player.position, player.width, player.height)) //draw line of sight
                                {
                                    if (npc.ai[0] == 2 || npc.ai[0] == 4)
                                    {
                                        if ((Main.expertMode || CalamityWorld.bossRushActive) && !NPC.AnyNPCs(ModContent.NPCType<DarkHeart>()))
                                        {
                                            NPC.NewNPC((int)npc.Center.X, (int)npc.Center.Y, ModContent.NPCType<DarkHeart>());
                                        }
                                    }
                                    else if (NPC.CountNPCS(NPCID.EaterofSouls) < 2)
                                    {
                                        NPC.NewNPC((int)npc.Center.X, (int)npc.Center.Y, NPCID.EaterofSouls);
                                    }
                                }
                                if (npc.ai[0] == 6)
                                {
                                    npc.velocity = npc.velocity.RotatedBy(3.14159265 / arcTime * -rotationDirection);
                                    SpawnStuff();
                                    state = 6;
                                    npc.ai[0] = 0;
                                    deceleration = npc.velocity / decelerationTime;
                                }
                            }
                        }
                    }
                    break;
                case 5: //raindash
                    if (npc.alpha > 0)
                    {
                        npc.alpha -= 5;
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            npc.Center = player.Center;
                            npc.position.Y -= teleportRadius;
                            npc.position.X += teleportRadius * rotationDirection;
                        }
                        npc.netUpdate = true;
                    }
                    else
                    {
                        if (!dashStarted)
                        {
                            dashStarted = true;
                            Main.PlaySound(15, (int)npc.Center.X, (int)npc.Center.Y, 0);
                            npc.velocity.X = teleportRadius / arcTime * 3;
                            npc.velocity *= -rotationDirection;
                            npc.netUpdate = true;
                        }
                        else
                        {
                            phase2timer++;
                            if (phase2timer == (int)arcTime / 20)
                            {
                                phase2timer = 0;
                                npc.ai[0]++;
                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                {
                                    int damage = Main.expertMode ? 14 : 18;
                                    Projectile.NewProjectile(npc.position.X + Main.rand.Next(npc.width), npc.position.Y + Main.rand.Next(npc.height), 0, 0, ModContent.ProjectileType<ShadeNimbusHostile>(), damage, 0, Main.myPlayer, 11, 0);
                                }
                                if (npc.ai[0] == 10)
                                {
                                    state = 6;
                                    npc.ai[0] = 0;
                                    deceleration = npc.velocity / decelerationTime;
                                }
                            }
                        }
                    }
                    break;
                case 6: //deceleration
                    npc.velocity -= deceleration;
                    phase2timer++;
                    if (phase2timer == decelerationTime)
                    {
                        phase2timer = minimumDriftTime + Main.rand.Next(121);
                        state = 0;
                        npc.netUpdate = true;
                    }
                    break;
            }
        }

        public override bool? CanHitNPC(NPC target)
        {
            if (npc.alpha > 0)
                return false;
            return null;
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            return npc.alpha <= 0; //no damage when not fully visible
        }

        public override bool StrikeNPC(ref double damage, int defense, ref float knockback, int hitDirection, ref bool crit)
        {
            if (phase2timer < 0 && damage > 1)
            {
                npc.velocity *= -4f;
                ReelBack();
                npc.netUpdate = true;
            }
            return true;
        }

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            npc.lifeMax = (int)(npc.lifeMax * 0.8f * bossLifeScale);
            npc.damage = (int)(npc.damage * 0.9f);
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            for (int k = 0; k < damage / npc.lifeMax * 100.0; k++)
            {
                Dust.NewDust(npc.position, npc.width, npc.height, 14, hitDirection, -1f, 0, default, 1f);
            }
            if (Main.netMode != NetmodeID.MultiplayerClient && Main.rand.NextBool(15) && NPC.CountNPCS(ModContent.NPCType<HiveBlob2>()) < 2)
            {
                Vector2 spawnAt = npc.Center + new Vector2(0f, (float)npc.height / 2f);
                NPC.NewNPC((int)spawnAt.X, (int)spawnAt.Y, ModContent.NPCType<HiveBlob2>());
            }
            if (npc.life <= 0)
            {
                int goreAmount = 10;
                for (int i = 1; i <= goreAmount; i++)
                    Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/HiveMindGores/HiveMindP2Gore" + i), 1f);
                npc.position.X = npc.position.X + (float)(npc.width / 2);
                npc.position.Y = npc.position.Y + (float)(npc.height / 2);
                npc.width = 200;
                npc.height = 150;
                npc.position.X = npc.position.X - (float)(npc.width / 2);
                npc.position.Y = npc.position.Y - (float)(npc.height / 2);
                for (int num621 = 0; num621 < 40; num621++)
                {
                    int num622 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 14, 0f, 0f, 100, default, 2f);
                    Main.dust[num622].velocity *= 3f;
                    if (Main.rand.NextBool(2))
                    {
                        Main.dust[num622].scale = 0.5f;
                        Main.dust[num622].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                    }
                }
                for (int num623 = 0; num623 < 70; num623++)
                {
                    int num624 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 14, 0f, 0f, 100, default, 3f);
                    Main.dust[num624].noGravity = true;
                    Main.dust[num624].velocity *= 5f;
                    num624 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 14, 0f, 0f, 100, default, 2f);
                    Main.dust[num624].velocity *= 2f;
                }
            }
        }

        public override void BossLoot(ref string name, ref int potionType)
        {
            potionType = ItemID.HealingPotion;
        }

        public override void NPCLoot()
        {
            DropHelper.DropBags(npc);

            DropHelper.DropItemChance(npc, ModContent.ItemType<HiveMindTrophy>(), 10);
            DropHelper.DropItemCondition(npc, ModContent.ItemType<KnowledgeHiveMind>(), true, !CalamityWorld.downedHiveMind);
            DropHelper.DropResidentEvilAmmo(npc, CalamityWorld.downedHiveMind, 2, 0, 0);

            // All other drops are contained in the bag, so they only drop directly on Normal
            if (!Main.expertMode)
            {
                // Materials
                DropHelper.DropItemSpray(npc, ModContent.ItemType<TrueShadowScale>(), 25, 30);
                DropHelper.DropItemSpray(npc, ItemID.DemoniteBar, 7, 10);
                DropHelper.DropItemSpray(npc, ItemID.RottenChunk, 9, 15);
                if (Main.hardMode)
                    DropHelper.DropItemSpray(npc, ItemID.CursedFlame, 10, 20);

                // Weapons
                DropHelper.DropItemChance(npc, ModContent.ItemType<PerfectDark>(), 4);
                DropHelper.DropItemChance(npc, ModContent.ItemType<LeechingDagger>(), 4);
                DropHelper.DropItemChance(npc, ModContent.ItemType<Shadethrower>(), 4);
                DropHelper.DropItemChance(npc, ModContent.ItemType<ShadowdropStaff>(), 4);
                DropHelper.DropItemChance(npc, ModContent.ItemType<ShaderainStaff>(), 4);
                DropHelper.DropItemChance(npc, ModContent.ItemType<DankStaff>(), 4);
                DropHelper.DropItemChance(npc, ModContent.ItemType<RotBall>(), 4, 25, 50);

				//Equipment
                DropHelper.DropItemChance(npc, ModContent.ItemType<FilthyGlove>(), 4);

                // Vanity
                DropHelper.DropItemChance(npc, ModContent.ItemType<HiveMindMask>(), 7);
            }

            // If neither The Hive Mind nor The Perforator Hive have been killed yet, notify players of Aerialite Ore
            if (!CalamityWorld.downedHiveMind && !CalamityWorld.downedPerforator)
            {
                string key = "Mods.CalamityMod.SkyOreText";
                Color messageColor = Color.Cyan;
                WorldGenerationMethods.SpawnOre(ModContent.TileType<AerialiteOre>(), 12E-05, .4f, .6f);

                if (Main.netMode == NetmodeID.SinglePlayer)
                    Main.NewText(Language.GetTextValue(key), messageColor);
                else if (Main.netMode == NetmodeID.Server)
                    NetMessage.BroadcastChatMessage(NetworkText.FromKey(key), messageColor);
            }

            // Mark The Hive Mind as dead
            CalamityWorld.downedHiveMind = true;
            CalamityMod.UpdateServerBoolean();
        }

        public override void OnHitPlayer(Player player, int damage, bool crit)
        {
            if (CalamityWorld.revenge)
            {
                player.AddBuff(ModContent.BuffType<Horror>(), 300, true);
            }
        }
    }
}
