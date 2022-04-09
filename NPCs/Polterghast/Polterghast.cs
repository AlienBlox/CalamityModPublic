﻿using CalamityMod.Dusts;
using CalamityMod.Events;
using CalamityMod.Items.Accessories;
using CalamityMod.Items.Armor.Vanity;
using CalamityMod.Items.LoreItems;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Furniture.Trophies;
using CalamityMod.Items.Potions;
using CalamityMod.Items.TreasureBags;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.NPCs.NormalNPCs;
using CalamityMod.Projectiles.Boss;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Terraria.GameContent.ItemDropRules;

namespace CalamityMod.NPCs.Polterghast
{
    public class Polterghast : ModNPC
    {
        public static int phase1IconIndex;
        public static int phase3IconIndex;

        internal static void LoadHeadIcons()
        {
            string phase1IconPath = "CalamityMod/NPCs/Polterghast/Polterghast_Head_Boss";
            string phase3IconPath = "CalamityMod/NPCs/Polterghast/Necroplasm_Head_Boss";

            CalamityMod.Instance.AddBossHeadTexture(phase1IconPath, -1);
            phase1IconIndex = ModContent.GetModBossHeadSlot(phase1IconPath);

            CalamityMod.Instance.AddBossHeadTexture(phase3IconPath, -1);
            phase3IconIndex = ModContent.GetModBossHeadSlot(phase3IconPath);
        }

        private int despawnTimer = 600;
        private bool reachedChargingPoint = false;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Polterghast");
            Main.npcFrameCount[NPC.type] = 12;
            NPCID.Sets.TrailingMode[NPC.type] = 1;
        }

        public override void SetDefaults()
        {
            NPC.Calamity().canBreakPlayerDefense = true;
            NPC.npcSlots = 50f;
            NPC.GetNPCDamage();
            NPC.width = 90;
            NPC.height = 120;
            NPC.defense = 90;
            NPC.DR_NERD(0.2f);
            NPC.LifeMaxNERB(350000, 420000, 325000);
            double HPBoost = CalamityConfig.Instance.BossHealthBoost * 0.01;
            NPC.lifeMax += (int)(NPC.lifeMax * HPBoost);
            NPC.knockBackResist = 0f;
            NPC.aiStyle = -1;
            AIType = -1;
            NPC.value = Item.buyPrice(3, 50, 0, 0);
            NPC.boss = true;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.netAlways = true;
            Music = CalamityMod.Instance.GetMusicFromMusicMod("Polterghast") ?? MusicID.Plantera;
            NPC.HitSound = SoundID.NPCHit7;
            NPC.DeathSound = SoundID.NPCDeath39;
            NPC.Calamity().VulnerableToSickness = false;
        }

        public override void BossHeadSlot(ref int index)
        {
            bool death = CalamityWorld.death || BossRushEvent.BossRushActive;
            bool revenge = CalamityWorld.revenge || BossRushEvent.BossRushActive;
            bool expertMode = Main.expertMode || BossRushEvent.BossRushActive;

            bool phase3 = NPC.life / (float)NPC.lifeMax < (death ? 0.6f : revenge ? 0.5f : expertMode ? 0.35f : 0.2f);
            if (phase3)
                index = phase3IconIndex;
            else
                index = phase1IconIndex;
        }

        public override void BossHeadRotation(ref float rotation)
        {
            if (NPC.HasValidTarget && NPC.Calamity().newAI[3] == 0f)
                rotation = (Main.player[NPC.TranslatedTargetIndex].Center - NPC.Center).ToRotation() + MathHelper.PiOver2;
            else
                rotation = NPC.velocity.ToRotation() + MathHelper.PiOver2;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(despawnTimer);
            writer.Write(reachedChargingPoint);
            CalamityGlobalNPC cgn = NPC.Calamity();
            writer.Write(cgn.newAI[0]);
            writer.Write(cgn.newAI[1]);
            writer.Write(cgn.newAI[2]);
            writer.Write(cgn.newAI[3]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            despawnTimer = reader.ReadInt32();
            reachedChargingPoint = reader.ReadBoolean();
            CalamityGlobalNPC cgn = NPC.Calamity();
            cgn.newAI[0] = reader.ReadSingle();
            cgn.newAI[1] = reader.ReadSingle();
            cgn.newAI[2] = reader.ReadSingle();
            cgn.newAI[3] = reader.ReadSingle();
        }

        public override void AI()
        {
            CalamityGlobalNPC calamityGlobalNPC = NPC.Calamity();

            // Emit light
            Lighting.AddLight((int)((NPC.position.X + (NPC.width / 2)) / 16f), (int)((NPC.position.Y + (NPC.height / 2)) / 16f), 0.1f, 0.5f, 0.5f);

            // whoAmI variable
            CalamityGlobalNPC.ghostBoss = NPC.whoAmI;

            // Detect clone
            bool cloneAlive = false;
            if (CalamityGlobalNPC.ghostBossClone != -1)
                cloneAlive = Main.npc[CalamityGlobalNPC.ghostBossClone].active;

            // Percent life remaining
            float lifeRatio = NPC.life / (float)NPC.lifeMax;

            // Variables
            Vector2 vector = NPC.Center;
            bool malice = CalamityWorld.malice || BossRushEvent.BossRushActive;
            bool speedBoost = false;
            bool despawnBoost = false;
            bool death = CalamityWorld.death || BossRushEvent.BossRushActive;
            bool revenge = CalamityWorld.revenge || BossRushEvent.BossRushActive;
            bool expertMode = Main.expertMode || BossRushEvent.BossRushActive;

            // Phases
            bool phase2 = lifeRatio < (death ? 0.9f : revenge ? 0.8f : expertMode ? 0.65f : 0.5f);
            bool phase3 = lifeRatio < (death ? 0.6f : revenge ? 0.5f : expertMode ? 0.35f : 0.2f);
            bool phase4 = lifeRatio < (death ? 0.45f : revenge ? 0.35f : expertMode ? 0.2f : 0.1f);
            bool phase5 = lifeRatio < (death ? 0.2f : revenge ? 0.15f : expertMode ? 0.1f : 0.05f);

            // Get angry if the clone is dead and in phase 3
            bool getPissed = !cloneAlive && phase3;

            // Velocity and acceleration
            calamityGlobalNPC.newAI[0] += 1f;
            bool chargePhase = calamityGlobalNPC.newAI[0] >= 480f;
            int chargeAmt = getPissed ? 4 : phase3 ? 3 : phase2 ? 2 : 1;
            float chargeVelocity = getPissed ? 28f : phase3 ? 24f : phase2 ? 22f : 20f;
            float chargeAcceleration = getPissed ? 0.7f : phase3 ? 0.6f : phase2 ? 0.55f : 0.5f;
            float chargeDistance = 480f;
            bool charging = NPC.ai[2] >= 300f;
            bool reset = NPC.ai[2] >= 600f;
            float speedUpDistance = 480f - 360f * (1f - lifeRatio);

            // Only get a new target while not charging
            if (!chargePhase)
            {
                // Get a target
                if (NPC.target < 0 || NPC.target == Main.maxPlayers || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
                    NPC.TargetClosest();

                // Despawn safety, make sure to target another player if the current player target is too far away
                if (Vector2.Distance(Main.player[NPC.target].Center, NPC.Center) > CalamityGlobalNPC.CatchUpDistance200Tiles)
                    NPC.TargetClosest();
            }

            Player player = Main.player[NPC.target];
            bool speedUp = Vector2.Distance(player.Center, vector) > speedUpDistance; // 30 or 40 tile distance
            float velocity = 10f; // Max should be 21
            float acceleration = 0.05f; // Max should be 0.13

            if (!player.active || player.dead)
            {
                NPC.TargetClosest(false);
                player = Main.player[NPC.target];
                if (!player.active || player.dead)
                {
                    speedBoost = true;
                    despawnBoost = true;
                    reachedChargingPoint = false;
                    NPC.ai[1] = 0f;
                    calamityGlobalNPC.newAI[0] = 0f;
                    calamityGlobalNPC.newAI[1] = 0f;
                    calamityGlobalNPC.newAI[2] = 0f;
                    calamityGlobalNPC.newAI[3] = 0f;

                    if (cloneAlive)
                    {
                        Main.npc[CalamityGlobalNPC.ghostBossClone].ai[0] = 0f;
                        Main.npc[CalamityGlobalNPC.ghostBossClone].ai[1] = 0f;
                        Main.npc[CalamityGlobalNPC.ghostBossClone].Calamity().newAI[0] = 0f;
                        Main.npc[CalamityGlobalNPC.ghostBossClone].Calamity().newAI[1] = 0f;
                        Main.npc[CalamityGlobalNPC.ghostBossClone].Calamity().newAI[2] = 0f;
                        Main.npc[CalamityGlobalNPC.ghostBossClone].Calamity().newAI[3] = 0f;
                    }
                }
            }

            // Stop rain
            CalamityMod.StopRain();

            // Set time left
            if (NPC.timeLeft < 1800)
                NPC.timeLeft = 1800;

            // Spawn hooks
            if (NPC.localAI[0] == 0f && Main.netMode != NetmodeID.MultiplayerClient)
            {
                NPC.localAI[0] = 1f;
                NPC.NewNPC(NPC.GetSpawnSourceForNPCFromNPCAI(), (int)vector.X, (int)vector.Y, ModContent.NPCType<PolterghastHook>(), NPC.whoAmI, 0f, 0f, 0f, 0f, 255);
                NPC.NewNPC(NPC.GetSpawnSourceForNPCFromNPCAI(), (int)vector.X, (int)vector.Y, ModContent.NPCType<PolterghastHook>(), NPC.whoAmI, 0f, 0f, 0f, 0f, 255);
                NPC.NewNPC(NPC.GetSpawnSourceForNPCFromNPCAI(), (int)vector.X, (int)vector.Y, ModContent.NPCType<PolterghastHook>(), NPC.whoAmI, 0f, 0f, 0f, 0f, 255);
                NPC.NewNPC(NPC.GetSpawnSourceForNPCFromNPCAI(), (int)vector.X, (int)vector.Y, ModContent.NPCType<PolterghastHook>(), NPC.whoAmI, 0f, 0f, 0f, 0f, 255);
            }

            if (!player.ZoneDungeon && !BossRushEvent.BossRushActive && player.position.Y < Main.worldSurface * 16.0)
            {
                despawnTimer--;
                if (despawnTimer <= 0)
                {
                    despawnBoost = true;
                    NPC.ai[1] = 0f;
                    calamityGlobalNPC.newAI[0] = 0f;
                    calamityGlobalNPC.newAI[1] = 0f;
                    calamityGlobalNPC.newAI[2] = 0f;
                    calamityGlobalNPC.newAI[3] = 0f;
                }

                speedBoost = true;
                velocity += 5f;
                acceleration += 0.05f;
            }
            else
                despawnTimer++;

            // Despawn
            if (Vector2.Distance(player.Center, vector) > (despawnBoost ? 1500f : 6000f))
            {
                NPC.active = false;
                NPC.netUpdate = true;
                return;
            }

            if (phase2)
            {
                velocity += 2.5f;
                acceleration += 0.02f;
            }

            if (!phase3)
            {
                if (charging)
                {
                    velocity += phase2 ? 4.5f : 3.5f;
                    acceleration += phase2 ? 0.03f : 0.025f;
                }
            }
            else
            {
                if (charging)
                {
                    velocity += phase5 ? 8.5f : 4.5f;
                    acceleration += phase5 ? 0.06f : 0.03f;
                }
                else
                {
                    if (phase5)
                    {
                        velocity += 1.5f;
                        acceleration += 0.015f;
                    }
                    else if (phase4)
                    {
                        velocity += 1f;
                        acceleration += 0.01f;
                    }
                    else
                    {
                        velocity += 0.5f;
                        acceleration += 0.005f;
                    }
                }
            }

            if (expertMode)
            {
                chargeVelocity += revenge ? 4f : 2f;
                velocity += revenge ? 5f : 3.5f;
                acceleration += revenge ? 0.035f : 0.025f;
            }

            // Slow down if close to target and not inside tiles
            if (!speedUp && !Collision.SolidCollision(NPC.position, NPC.width, NPC.height) && Collision.CanHit(NPC.position, NPC.width, NPC.height, player.position, player.width, player.height) && !charging && !chargePhase)
            {
                velocity = 8f;
                acceleration = 0.035f;
            }

            // Detect active tiles around Polterghast
            int radius = 30; // 30 tile radius
            int diameter = radius * 2;
            int npcCenterX = (int)(vector.X / 16f);
            int npcCenterY = (int)(vector.Y / 16f);
            Rectangle area = new Rectangle(npcCenterX - radius, npcCenterY - radius, diameter, diameter);
            int nearbyActiveTiles = 0; // 0 to 3600
            for (int x = area.Left; x < area.Right; x++)
            {
                for (int y = area.Top; y < area.Bottom; y++)
                {
                    if (Main.tile[x, y] != null)
                    {
                        if (Main.tile[x, y].HasUnactuatedTile && Main.tileSolid[Main.tile[x, y].TileType] && !Main.tileSolidTop[Main.tile[x, y].TileType] && !TileID.Sets.Platforms[Main.tile[x, y].TileType])
                            nearbyActiveTiles++;
                    }
                }
            }

            // Scale multiplier based on nearby active tiles
            float tileEnrageMult = 1f;
            if (nearbyActiveTiles < 1000)
                tileEnrageMult += (1000 - nearbyActiveTiles) * 0.00075f; // Ranges from 1f to 1.75f

            if (malice)
                tileEnrageMult = 1.75f;

            NPC.Calamity().CurrentlyEnraged = !BossRushEvent.BossRushActive && tileEnrageMult >= 1.6f;

            // Used to inform clone and hooks about number of active tiles nearby
            NPC.ai[3] = tileEnrageMult;

            // Increase projectile fire rate based on number of nearby active tiles
            float amount = 1f - ((tileEnrageMult - 1f) / 0.75f);
            if (amount < 0f)
                amount = 0f;
            float projectileFireRateMultiplier = MathHelper.Lerp(1f, 2f, amount);

            // Increase projectile stats based on number of nearby active tiles
            int baseProjectileTimeLeft = (int)(1200f * tileEnrageMult);
            int baseProjectileAmt = (int)(4f * tileEnrageMult);
            int baseProjectileSpread = (int)(45f * tileEnrageMult);
            float baseProjectileVelocity = 5f * tileEnrageMult;
            if (speedBoost)
                baseProjectileVelocity *= 1.25f;

            // Predictiveness
            Vector2 predictionVector = chargePhase && malice ? player.velocity * 20f : Vector2.Zero;
            Vector2 lookAt = player.Center + predictionVector;
            Vector2 rotationVector = lookAt - vector;

            // Rotation
            if (calamityGlobalNPC.newAI[3] == 0f)
            {
                float num740 = player.Center.X + predictionVector.X - vector.X;
                float num741 = player.Center.Y + predictionVector.Y - vector.Y;
                NPC.rotation = (float)Math.Atan2(num741, num740) + MathHelper.PiOver2;
            }
            else
                NPC.rotation = NPC.velocity.ToRotation() + MathHelper.PiOver2;

            if (!chargePhase)
            {
                NPC.ai[2] += 1f;
                if (reset)
                {
                    NPC.ai[2] = 0f;
                    NPC.netUpdate = true;
                }

                float movementLimitX = 0f;
                float movementLimitY = 0f;
                int numHooks = 4;
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    if (Main.npc[i].active && Main.npc[i].type == ModContent.NPCType<PolterghastHook>())
                    {
                        movementLimitX += Main.npc[i].Center.X;
                        movementLimitY += Main.npc[i].Center.Y;
                    }
                }
                movementLimitX /= numHooks;
                movementLimitY /= numHooks;

                Vector2 vector91 = new Vector2(movementLimitX, movementLimitY);
                float num736 = player.Center.X - vector91.X;
                float num737 = player.Center.Y - vector91.Y;

                if (despawnBoost)
                {
                    num737 *= -1f;
                    num736 *= -1f;
                    velocity += 10f;
                }

                float num738 = (float)Math.Sqrt(num736 * num736 + num737 * num737);
                float maxDistanceFromHooks = expertMode ? 650f : 500f;
                if (speedBoost || malice)
                    maxDistanceFromHooks += 250f;
                if (death)
                    maxDistanceFromHooks += maxDistanceFromHooks * 0.1f * (1f - lifeRatio);

                // Increase speed based on nearby active tiles
                velocity *= tileEnrageMult;
                acceleration *= tileEnrageMult;

                if (death)
                {
                    velocity += velocity * 0.15f * (1f - lifeRatio);
                    acceleration += acceleration * 0.15f * (1f - lifeRatio);
                }

                if (num738 >= maxDistanceFromHooks)
                {
                    num738 = maxDistanceFromHooks / num738;
                    num736 *= num738;
                    num737 *= num738;
                }

                movementLimitX += num736;
                movementLimitY += num737;
                num736 = movementLimitX - vector.X;
                num737 = movementLimitY - vector.Y;
                num738 = (float)Math.Sqrt(num736 * num736 + num737 * num737);

                if (num738 < velocity)
                {
                    num736 = NPC.velocity.X;
                    num737 = NPC.velocity.Y;
                }
                else
                {
                    num738 = velocity / num738;
                    num736 *= num738;
                    num737 *= num738;
                }

                if (NPC.velocity.X < num736)
                {
                    NPC.velocity.X += acceleration;
                    if (NPC.velocity.X < 0f && num736 > 0f)
                        NPC.velocity.X += acceleration * 2f;
                }
                else if (NPC.velocity.X > num736)
                {
                    NPC.velocity.X -= acceleration;
                    if (NPC.velocity.X > 0f && num736 < 0f)
                        NPC.velocity.X -= acceleration * 2f;
                }
                if (NPC.velocity.Y < num737)
                {
                    NPC.velocity.Y += acceleration;
                    if (NPC.velocity.Y < 0f && num737 > 0f)
                        NPC.velocity.Y += acceleration * 2f;
                }
                else if (NPC.velocity.Y > num737)
                {
                    NPC.velocity.Y -= acceleration;
                    if (NPC.velocity.Y > 0f && num737 < 0f)
                        NPC.velocity.Y -= acceleration * 2f;
                }

                // Slow down considerably if near player
                if (!speedUp && nearbyActiveTiles > 1000 && !Collision.SolidCollision(NPC.position, NPC.width, NPC.height) && Collision.CanHit(NPC.position, NPC.width, NPC.height, player.position, player.width, player.height) && !charging)
                {
                    if (NPC.velocity.Length() > velocity)
                        NPC.velocity *= 0.97f;
                }
            }
            else
            {
                // Charge
                if (calamityGlobalNPC.newAI[3] == 1f)
                {
                    reachedChargingPoint = false;

                    if (calamityGlobalNPC.newAI[1] == 0f)
                    {
                        NPC.velocity = Vector2.Normalize(rotationVector) * chargeVelocity;
                        calamityGlobalNPC.newAI[1] = 1f;
                    }
                    else
                    {
                        calamityGlobalNPC.newAI[2] += 1f;

                        // Slow down for a few frames
                        float totalChargeTime = chargeDistance * 4f / chargeVelocity;
                        float slowDownTime = chargeVelocity;
                        if (calamityGlobalNPC.newAI[2] >= totalChargeTime - slowDownTime)
                            NPC.velocity *= 0.9f;

                        // Reset and either go back to normal or charge again
                        if (calamityGlobalNPC.newAI[2] >= totalChargeTime)
                        {
                            calamityGlobalNPC.newAI[1] = 0f;
                            calamityGlobalNPC.newAI[2] = 0f;
                            calamityGlobalNPC.newAI[3] = 0f;
                            NPC.ai[1] += 1f;

                            if (NPC.ai[1] >= chargeAmt)
                            {
                                // Reset and return to normal movement
                                calamityGlobalNPC.newAI[0] = 0f;
                                NPC.ai[1] = 0f;
                            }
                            else
                            {
                                // Get a new target and charge again
                                NPC.TargetClosest();
                            }
                        }
                    }
                }
                else
                {
                    // Pick a charging location
                    // Set charge locations X
                    if (vector.X >= player.Center.X)
                        calamityGlobalNPC.newAI[1] = player.Center.X + chargeDistance;
                    else
                        calamityGlobalNPC.newAI[1] = player.Center.X - chargeDistance;

                    // Set charge locations Y
                    if (vector.Y >= player.Center.Y)
                        calamityGlobalNPC.newAI[2] = player.Center.Y + chargeDistance;
                    else
                        calamityGlobalNPC.newAI[2] = player.Center.Y - chargeDistance;

                    Vector2 chargeVector = new Vector2(calamityGlobalNPC.newAI[1], calamityGlobalNPC.newAI[2]);
                    Vector2 chargeLocationVelocity = Vector2.Normalize(chargeVector - vector) * chargeVelocity;
                    Vector2 cloneChargeVector = cloneAlive ? new Vector2(Main.npc[CalamityGlobalNPC.ghostBossClone].Calamity().newAI[1], Main.npc[CalamityGlobalNPC.ghostBossClone].Calamity().newAI[2]) : default;

                    // If clone is alive and not at proper location then keep trying to line up until it gets into position
                    float chargeDistanceGateValue = 40f;
                    bool clonePositionCheck = cloneAlive ? Vector2.Distance(Main.npc[CalamityGlobalNPC.ghostBossClone].Center, cloneChargeVector) <= chargeDistanceGateValue : true;

                    // Line up a charge
                    if (Vector2.Distance(vector, chargeVector) <= chargeDistanceGateValue || reachedChargingPoint)
                    {
                        // Emit dust
                        if (!reachedChargingPoint)
                        {
                            SoundEngine.PlaySound(SoundID.Item125, NPC.position);
                            for (int i = 0; i < 30; i++)
                            {
                                int dust = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, (int)CalamityDusts.Ectoplasm, 0f, 0f, 100, default, 3f);
                                Main.dust[dust].noGravity = true;
                                Main.dust[dust].velocity *= 5f;
                            }
                        }

                        reachedChargingPoint = true;
                        NPC.velocity = Vector2.Zero;
                        NPC.Center = chargeVector;

                        if (clonePositionCheck)
                        {
                            // Initiate charge
                            calamityGlobalNPC.newAI[1] = 0f;
                            calamityGlobalNPC.newAI[2] = 0f;
                            calamityGlobalNPC.newAI[3] = 1f;

                            // Tell clone to charge
                            if (cloneAlive)
                            {
                                Main.npc[CalamityGlobalNPC.ghostBossClone].ai[0] = 0f;
                                Main.npc[CalamityGlobalNPC.ghostBossClone].Calamity().newAI[1] = 0f;
                                Main.npc[CalamityGlobalNPC.ghostBossClone].Calamity().newAI[2] = 0f;
                                Main.npc[CalamityGlobalNPC.ghostBossClone].Calamity().newAI[3] = 1f;

                                //
                                // CODE TWEAKED BY: OZZATRON
                                // September 21st, 2020
                                // reason: fixing Polter charge MP desync bug
                                //
                                // removed Polter syncing the clone's newAI array. The clone now auto syncs its own newAI every frame.
                            }
                        }
                    }
                    else
                        NPC.SimpleFlyMovement(chargeLocationVelocity, chargeAcceleration);
                }

                NPC.netUpdate = true;

                if (NPC.netSpam > 10)
                    NPC.netSpam = 10;

                if (Main.netMode == NetmodeID.Server)
                    NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, NPC.whoAmI, 0f, 0f, 0f, 0, 0, 0);
            }

            if (!phase2 && !phase3)
            {
                NPC.damage = NPC.defDamage;
                NPC.defense = NPC.defDefense;

                if (Main.netMode != NetmodeID.MultiplayerClient && !charging && !chargePhase)
                {
                    NPC.localAI[1] += expertMode ? 1.5f : 1f;
                    if (speedBoost)
                        NPC.localAI[1] += 2f;

                    if (NPC.localAI[1] >= 90f * projectileFireRateMultiplier)
                    {
                        NPC.localAI[1] = 0f;

                        bool flag47 = Collision.CanHit(NPC.position, NPC.width, NPC.height, player.position, player.width, player.height);
                        if (NPC.localAI[3] > 0f)
                        {
                            flag47 = true;
                            NPC.localAI[3] = 0f;
                        }

                        if (flag47)
                        {
                            int type = ModContent.ProjectileType<PhantomShot>();
                            if (Main.rand.NextBool(3))
                            {
                                NPC.localAI[1] = -30f;
                                type = ModContent.ProjectileType<PhantomBlast>();
                            }

                            int damage = NPC.GetProjectileDamage(type);

                            Vector2 vector93 = vector;
                            float num743 = player.Center.X - vector93.X;
                            float num744 = player.Center.Y - vector93.Y;
                            float num745 = (float)Math.Sqrt(num743 * num743 + num744 * num744);

                            num745 = baseProjectileVelocity / num745;
                            num743 *= num745;
                            num744 *= num745;
                            vector93.X += num743 * 3f;
                            vector93.Y += num744 * 3f;

                            float rotation = MathHelper.ToRadians(baseProjectileSpread);
                            float baseSpeed = (float)Math.Sqrt(num743 * num743 + num744 * num744);
                            double startAngle = Math.Atan2(num743, num744) - rotation / 2;
                            double deltaAngle = rotation / baseProjectileAmt;
                            double offsetAngle;
                            for (int i = 0; i < baseProjectileAmt; i++)
                            {
                                offsetAngle = startAngle + deltaAngle * i;
                                int proj = Projectile.NewProjectile(NPC.GetSpawnSource_ForProjectile(), vector93.X, vector93.Y, baseSpeed * (float)Math.Sin(offsetAngle), baseSpeed * (float)Math.Cos(offsetAngle), type, damage, 0f, Main.myPlayer, 0f, 0f);
                                Main.projectile[proj].timeLeft = type == ModContent.ProjectileType<PhantomBlast>() ? baseProjectileTimeLeft / 4 : baseProjectileTimeLeft;
                            }
                        }
                        else
                        {
                            int type = ModContent.ProjectileType<PhantomBlast>();
                            int damage = NPC.GetProjectileDamage(type);

                            Vector2 vector93 = vector;
                            float num743 = player.Center.X - vector93.X;
                            float num744 = player.Center.Y - vector93.Y;
                            float num745 = (float)Math.Sqrt(num743 * num743 + num744 * num744);

                            num745 = (baseProjectileVelocity + 5f) / num745;
                            num743 *= num745;
                            num744 *= num745;
                            vector93.X += num743 * 3f;
                            vector93.Y += num744 * 3f;

                            float rotation = MathHelper.ToRadians(baseProjectileSpread + 15);
                            float baseSpeed = (float)Math.Sqrt(num743 * num743 + num744 * num744);
                            double startAngle = Math.Atan2(num743, num744) - rotation / 2;
                            double deltaAngle = rotation / baseProjectileAmt;
                            double offsetAngle;
                            for (int i = 0; i < baseProjectileAmt; i++)
                            {
                                offsetAngle = startAngle + deltaAngle * i;
                                int proj = Projectile.NewProjectile(NPC.GetSpawnSource_ForProjectile(), vector93.X, vector93.Y, baseSpeed * (float)Math.Sin(offsetAngle), baseSpeed * (float)Math.Cos(offsetAngle), type, damage, 0f, Main.myPlayer, 0f, 0f);
                                Main.projectile[proj].timeLeft = baseProjectileTimeLeft / 4;
                            }
                        }
                    }
                }
            }
            else if (!phase3)
            {
                if (NPC.ai[0] == 0f)
                {
                    NPC.ai[0] = 1f;

                    // Reset charge attack arrays to prevent problems
                    NPC.ai[1] = 0f;
                    calamityGlobalNPC.newAI[0] = 0f;
                    calamityGlobalNPC.newAI[1] = 0f;
                    calamityGlobalNPC.newAI[2] = 0f;
                    calamityGlobalNPC.newAI[3] = 0f;

                    SoundEngine.PlaySound(SoundID.Item122, NPC.Center);

                    if (Main.netMode != NetmodeID.Server)
                    {
                        Gore.NewGore(NPC.position, NPC.velocity, Mod.Find<ModGore>("Polt").Type, 1f);
                        Gore.NewGore(NPC.position, NPC.velocity, Mod.Find<ModGore>("Polt2").Type, 1f);
                        Gore.NewGore(NPC.position, NPC.velocity, Mod.Find<ModGore>("Polt3").Type, 1f);
                        Gore.NewGore(NPC.position, NPC.velocity, Mod.Find<ModGore>("Polt4").Type, 1f);
                        Gore.NewGore(NPC.position, NPC.velocity, Mod.Find<ModGore>("Polt5").Type, 1f);
                    }

                    for (int num621 = 0; num621 < 10; num621++)
                    {
                        int num622 = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, (int)CalamityDusts.Phantoplasm, 0f, 0f, 100, default, 2f);
                        Main.dust[num622].velocity *= 3f;
                        Main.dust[num622].noGravity = true;
                        if (Main.rand.NextBool(2))
                        {
                            Main.dust[num622].scale = 0.5f;
                            Main.dust[num622].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                        }
                    }
                    for (int num623 = 0; num623 < 30; num623++)
                    {
                        int num624 = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, (int)CalamityDusts.Ectoplasm, 0f, 0f, 100, default, 3f);
                        Main.dust[num624].noGravity = true;
                        Main.dust[num624].velocity *= 5f;
                        num624 = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, (int)CalamityDusts.Ectoplasm, 0f, 0f, 100, default, 2f);
                        Main.dust[num624].velocity *= 2f;
                    }
                }

                NPC.GivenName = "Necroghast";

                NPC.damage = (int)(NPC.defDamage * 1.2f);
                NPC.defense = (int)(NPC.defDefense * 0.8f);

                if (Main.netMode != NetmodeID.MultiplayerClient && !charging && !chargePhase)
                {
                    NPC.localAI[1] += expertMode ? 1.5f : 1f;
                    if (speedBoost)
                        NPC.localAI[1] += 2f;

                    if (NPC.localAI[1] >= 150f * projectileFireRateMultiplier)
                    {
                        NPC.localAI[1] = 0f;

                        bool flag47 = Collision.CanHit(NPC.position, NPC.width, NPC.height, player.position, player.width, player.height);
                        if (NPC.localAI[3] > 0f)
                        {
                            flag47 = true;
                            NPC.localAI[3] = 0f;
                        }

                        if (flag47)
                        {
                            int type = ModContent.ProjectileType<PhantomShot2>();
                            if (Main.rand.NextBool(3))
                            {
                                NPC.localAI[1] = -30f;
                                type = ModContent.ProjectileType<PhantomBlast2>();
                            }

                            int damage = NPC.GetProjectileDamage(type);

                            Vector2 vector93 = vector;
                            float num743 = player.Center.X - vector93.X;
                            float num744 = player.Center.Y - vector93.Y;
                            float num745 = (float)Math.Sqrt(num743 * num743 + num744 * num744);

                            num745 = (baseProjectileVelocity + 1f) / num745;
                            num743 *= num745;
                            num744 *= num745;
                            vector93.X += num743 * 3f;
                            vector93.Y += num744 * 3f;

                            int numProj = baseProjectileAmt + 1;
                            float rotation = MathHelper.ToRadians(baseProjectileSpread + 15);
                            float baseSpeed = (float)Math.Sqrt(num743 * num743 + num744 * num744);
                            double startAngle = Math.Atan2(num743, num744) - rotation / 2;
                            double deltaAngle = rotation / numProj;
                            double offsetAngle;
                            for (int i = 0; i < numProj; i++)
                            {
                                offsetAngle = startAngle + deltaAngle * i;
                                int proj = Projectile.NewProjectile(NPC.GetSpawnSource_ForProjectile(), vector93.X, vector93.Y, baseSpeed * (float)Math.Sin(offsetAngle), baseSpeed * (float)Math.Cos(offsetAngle), type, damage, 0f, Main.myPlayer, 0f, 0f);
                                Main.projectile[proj].timeLeft = type == ModContent.ProjectileType<PhantomBlast2>() ? baseProjectileTimeLeft / 4 : baseProjectileTimeLeft;
                            }
                        }
                        else
                        {
                            int type = ModContent.ProjectileType<PhantomBlast2>();
                            int damage = NPC.GetProjectileDamage(type);

                            Vector2 vector93 = vector;
                            float num743 = player.Center.X - vector93.X;
                            float num744 = player.Center.Y - vector93.Y;
                            float num745 = (float)Math.Sqrt(num743 * num743 + num744 * num744);

                            num745 = (baseProjectileVelocity + 5f) / num745;
                            num743 *= num745;
                            num744 *= num745;
                            vector93.X += num743 * 3f;
                            vector93.Y += num744 * 3f;

                            int numProj = baseProjectileAmt + 1;
                            float rotation = MathHelper.ToRadians(baseProjectileSpread + 35);
                            float baseSpeed = (float)Math.Sqrt(num743 * num743 + num744 * num744);
                            double startAngle = Math.Atan2(num743, num744) - rotation / 2;
                            double deltaAngle = rotation / numProj;
                            double offsetAngle;
                            for (int i = 0; i < numProj; i++)
                            {
                                offsetAngle = startAngle + deltaAngle * i;
                                int proj = Projectile.NewProjectile(NPC.GetSpawnSource_ForProjectile(), vector93.X, vector93.Y, baseSpeed * (float)Math.Sin(offsetAngle), baseSpeed * (float)Math.Cos(offsetAngle), type, damage, 0f, Main.myPlayer, 0f, 0f);
                                Main.projectile[proj].timeLeft = baseProjectileTimeLeft / 4;
                            }
                        }
                    }
                }
            }
            else
            {
                NPC.HitSound = SoundID.NPCHit36;

                if (NPC.ai[0] == 1f)
                {
                    NPC.ai[0] = 2f;

                    // Reset charge attack arrays to prevent problems
                    NPC.ai[1] = 0f;
                    calamityGlobalNPC.newAI[0] = 0f;
                    calamityGlobalNPC.newAI[1] = 0f;
                    calamityGlobalNPC.newAI[2] = 0f;
                    calamityGlobalNPC.newAI[3] = 0f;

                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        NPC.NewNPC(NPC.GetSpawnSourceForNPCFromNPCAI(), (int)vector.X, (int)vector.Y, ModContent.NPCType<PolterPhantom>());

                        if (expertMode)
                        {
                            for (int I = 0; I < 3; I++)
                            {
                                int spawn = NPC.NewNPC(NPC.GetSpawnSourceForNPCFromNPCAI(), (int)(vector.X + (Math.Sin(I * 120) * 500)), (int)(vector.Y + (Math.Cos(I * 120) * 500)), ModContent.NPCType<PhantomFuckYou>(), NPC.whoAmI, 0, 0, 0, -1);
                                NPC npc2 = Main.npc[spawn];
                                npc2.ai[0] = I * 120;
                            }
                        }
                    }

                    SoundEngine.PlaySound(SoundID.Item122, NPC.Center);

                    if (Main.netMode != NetmodeID.Server)
                    {
                        Gore.NewGore(NPC.position, NPC.velocity, Mod.Find<ModGore>("Polt").Type, 1f);
                        Gore.NewGore(NPC.position, NPC.velocity, Mod.Find<ModGore>("Polt2").Type, 1f);
                        Gore.NewGore(NPC.position, NPC.velocity, Mod.Find<ModGore>("Polt3").Type, 1f);
                        Gore.NewGore(NPC.position, NPC.velocity, Mod.Find<ModGore>("Polt4").Type, 1f);
                        Gore.NewGore(NPC.position, NPC.velocity, Mod.Find<ModGore>("Polt5").Type, 1f);
                    }

                    for (int num621 = 0; num621 < 10; num621++)
                    {
                        int num622 = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, (int)CalamityDusts.Phantoplasm, 0f, 0f, 100, default, 2f);
                        Main.dust[num622].velocity *= 3f;
                        Main.dust[num622].noGravity = true;
                        if (Main.rand.NextBool(2))
                        {
                            Main.dust[num622].scale = 0.5f;
                            Main.dust[num622].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                        }
                    }
                    for (int num623 = 0; num623 < 30; num623++)
                    {
                        int num624 = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, (int)CalamityDusts.Ectoplasm, 0f, 0f, 100, default, 3f);
                        Main.dust[num624].noGravity = true;
                        Main.dust[num624].velocity *= 5f;
                        num624 = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, (int)CalamityDusts.Ectoplasm, 0f, 0f, 100, default, 2f);
                        Main.dust[num624].velocity *= 2f;
                    }
                }

                NPC.GivenName = "Necroplasm";

                NPC.damage = (int)(NPC.defDamage * 1.4f);
                NPC.defense = (int)(NPC.defDefense * 0.5f);

                NPC.localAI[1] += 1f;
                if (NPC.localAI[1] >= (getPissed ? 150f : 210f) * projectileFireRateMultiplier && Collision.CanHit(NPC.position, NPC.width, NPC.height, player.position, player.width, player.height))
                {
                    NPC.localAI[1] = 0f;
                    if (Main.netMode != NetmodeID.MultiplayerClient && !charging && !chargePhase)
                    {
                        Vector2 vector93 = vector;
                        float num743 = player.Center.X - vector93.X;
                        float num744 = player.Center.Y - vector93.Y;
                        float num745 = (float)Math.Sqrt(num743 * num743 + num744 * num744);

                        num745 = baseProjectileVelocity / num745;
                        num743 *= num745;
                        num744 *= num745;
                        vector93.X += num743 * 3f;
                        vector93.Y += num744 * 3f;

                        int numProj = baseProjectileAmt + (getPissed ? 4 : 2);
                        float rotation = MathHelper.ToRadians(baseProjectileSpread + (getPissed ? 60 : 45));
                        float baseSpeed = (float)Math.Sqrt(num743 * num743 + num744 * num744);
                        double startAngle = Math.Atan2(num743, num744) - rotation / 2;
                        double deltaAngle = rotation / numProj;
                        double offsetAngle;

                        int type = Main.rand.NextBool(2) ? ModContent.ProjectileType<PhantomShot2>() : ModContent.ProjectileType<PhantomShot>();
                        int damage = NPC.GetProjectileDamage(type);

                        for (int i = 0; i < numProj; i++)
                        {
                            offsetAngle = startAngle + deltaAngle * i;
                            Projectile.NewProjectile(NPC.GetSpawnSource_ForProjectile(), vector93.X, vector93.Y, baseSpeed * (float)Math.Sin(offsetAngle), baseSpeed * (float)Math.Cos(offsetAngle), type, damage, 0f, Main.myPlayer, 0f, 0f);
                        }
                    }
                }

                if (phase4)
                {
                    NPC.localAI[2] += 1f;
                    if (NPC.localAI[2] >= (getPissed ? 300f : 420f))
                    {
                        NPC.localAI[2] = 0f;

                        float num757 = 6f;
                        Vector2 vector94 = vector;
                        float num758 = player.Center.X - vector94.X;
                        float num760 = player.Center.Y - vector94.Y;
                        float num761 = (float)Math.Sqrt(num758 * num758 + num760 * num760);
                        num761 = num757 / num761;
                        num758 *= num761;
                        num760 *= num761;
                        vector94.X += num758 * 3f;
                        vector94.Y += num760 * 3f;

                        if (NPC.CountNPCS(ModContent.NPCType<PhantomSpiritL>()) < 2 && Main.netMode != NetmodeID.MultiplayerClient && !charging && !chargePhase)
                        {
                            int num762 = NPC.NewNPC(NPC.GetSpawnSourceForNPCFromNPCAI(), (int)vector.X, (int)vector.Y, ModContent.NPCType<PhantomSpiritL>());
                            Main.npc[num762].velocity.X = num758;
                            Main.npc[num762].velocity.Y = num760;
                            Main.npc[num762].netUpdate = true;
                        }
                    }
                }
            }
        }

        public override void BossLoot(ref string name, ref int potionType)
        {
            potionType = ModContent.ItemType<SupremeHealingPotion>();
        }

        public override void OnKill()
        {
            CalamityGlobalNPC.SetNewBossJustDowned(NPC);

            CalamityGlobalNPC.SetNewShopVariable(new int[] { NPCID.Cyborg }, DownedBossSystem.downedPolterghast);

            // If Polterghast has not been killed, notify players about the Abyss minibosses now dropping items
            if (!DownedBossSystem.downedPolterghast)
            {
                if (!Main.player[Main.myPlayer].dead && Main.player[Main.myPlayer].active)
                    SoundEngine.PlaySound(SoundLoader.GetLegacySoundSlot(Mod, "Sounds/Custom/ReaperSearchRoar"), (int)Main.player[Main.myPlayer].position.X, (int)Main.player[Main.myPlayer].position.Y);

                string key = "Mods.CalamityMod.GhostBossText";
                Color messageColor = Color.RoyalBlue;
                string sulfSeaBoostMessage = "Mods.CalamityMod.GhostBossText4";
                Color sulfSeaBoostColor = AcidRainEvent.TextColor;

                if (Main.rand.NextBool(20) && DateTime.Now.Month == 4 && DateTime.Now.Day == 1)
                {
                    sulfSeaBoostMessage = "Mods.CalamityMod.AprilFools2"; // Goddamn boomer duke moments
                }

                CalamityUtils.DisplayLocalizedText(key, messageColor);
                CalamityUtils.DisplayLocalizedText(sulfSeaBoostMessage, sulfSeaBoostColor);
            }

            // Mark Polterghast as dead
            DownedBossSystem.downedPolterghast = true;
            CalamityNetcode.SyncWorld();
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.BossBag(ModContent.ItemType<PolterghastBag>()));

            // Normal drops: Everything that would otherwise be in the bag
            var normalOnly = npcLoot.DefineNormalOnlyDropSet();
            {
                // Weapons
                int[] weapons = new int[]
                {
                    ModContent.ItemType<TerrorBlade>(),
                    ModContent.ItemType<BansheeHook>(),
                    ModContent.ItemType<DaemonsFlame>(),
                    ModContent.ItemType<FatesReveal>(),
                    ModContent.ItemType<GhastlyVisage>(),
                    ModContent.ItemType<EtherealSubjugator>(),
                    ModContent.ItemType<GhoulishGouger>(),
                };
                normalOnly.Add(DropHelper.CalamityStyle(DropHelper.NormalWeaponDropRateFraction, weapons));

                // Equipment
                normalOnly.Add(DropHelper.PerPlayer(ModContent.ItemType<Affliction>()));

                // Materials
                normalOnly.Add(DropHelper.PerPlayer(ModContent.ItemType<RuinousSoul>(), 1, 7, 15));
                normalOnly.Add(ModContent.ItemType<Phantoplasm>(), 1, 30, 40);

                // Vanity
                normalOnly.Add(ModContent.ItemType<PolterghastMask>(), 7);
            }

            npcLoot.Add(ModContent.ItemType<PolterghastTrophy>(), 10);

            // Lore
            npcLoot.AddConditionalPerPlayer(() => !DownedBossSystem.downedPolterghast, ModContent.ItemType<KnowledgePolterghast>());
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (NPC.spriteDirection == 1)
                spriteEffects = SpriteEffects.FlipHorizontally;

            Texture2D texture2D15 = TextureAssets.Npc[NPC.type].Value;
            Texture2D texture2D16 = ModContent.Request<Texture2D>("CalamityMod/NPCs/Polterghast/PolterghastGlow2").Value;
            Vector2 vector11 = new Vector2(TextureAssets.Npc[NPC.type].Value.Width / 2, TextureAssets.Npc[NPC.type].Value.Height / Main.npcFrameCount[NPC.type] / 2);
            Color color36 = Color.White;
            float amount9 = 0.5f;
            int num153 = 7;

            if (CalamityConfig.Instance.Afterimages)
            {
                for (int num155 = 1; num155 < num153; num155 += 2)
                {
                    Color color38 = drawColor;
                    color38 = Color.Lerp(color38, color36, amount9);
                    color38 = NPC.GetAlpha(color38);
                    color38 *= (num153 - num155) / 15f;
                    Vector2 vector41 = NPC.oldPos[num155] + new Vector2(NPC.width, NPC.height) / 2f - screenPos;
                    vector41 -= new Vector2(texture2D15.Width, texture2D15.Height / Main.npcFrameCount[NPC.type]) * NPC.scale / 2f;
                    vector41 += vector11 * NPC.scale + new Vector2(0f, NPC.gfxOffY);
                    spriteBatch.Draw(texture2D15, vector41, NPC.frame, color38, NPC.rotation, vector11, NPC.scale, spriteEffects, 0f);
                }
            }

            Vector2 vector43 = NPC.Center - screenPos;
            vector43 -= new Vector2(texture2D15.Width, texture2D15.Height / Main.npcFrameCount[NPC.type]) * NPC.scale / 2f;
            vector43 += vector11 * NPC.scale + new Vector2(0f, NPC.gfxOffY);
            spriteBatch.Draw(texture2D15, vector43, NPC.frame, NPC.GetAlpha(drawColor), NPC.rotation, vector11, NPC.scale, spriteEffects, 0f);

            texture2D15 = ModContent.Request<Texture2D>("CalamityMod/NPCs/Polterghast/PolterghastGlow").Value;

            Color color37 = Color.Lerp(Color.White, Color.Cyan, 0.5f);
            Color lightRed = new Color(255, 100, 100, 255);
            if (NPC.Calamity().newAI[0] > 300f)
                color37 = Color.Lerp(color37, lightRed, MathHelper.Clamp((NPC.Calamity().newAI[0] - 300f) / 120f, 0f, 1f));

            Color color42 = Color.Lerp(Color.White, (NPC.ai[2] >= 300f || NPC.Calamity().newAI[0] > 300f) ? Color.Red : Color.Black, 0.5f);

            if (CalamityConfig.Instance.Afterimages)
            {
                for (int num163 = 1; num163 < num153; num163++)
                {
                    Color color41 = color37;
                    color41 = Color.Lerp(color41, color36, amount9);
                    color41 = NPC.GetAlpha(color41);
                    color41 *= (num153 - num163) / 15f;
                    Vector2 vector44 = NPC.oldPos[num163] + new Vector2(NPC.width, NPC.height) / 2f - screenPos;
                    vector44 -= new Vector2(texture2D15.Width, texture2D15.Height / Main.npcFrameCount[NPC.type]) * NPC.scale / 2f;
                    vector44 += vector11 * NPC.scale + new Vector2(0f, NPC.gfxOffY);
                    spriteBatch.Draw(texture2D15, vector44, NPC.frame, color41, NPC.rotation, vector11, NPC.scale, spriteEffects, 0f);

                    Color color43 = color42;
                    color43 = Color.Lerp(color43, color36, amount9);
                    color43 = NPC.GetAlpha(color43);
                    color43 *= (num153 - num163) / 15f;
                    spriteBatch.Draw(texture2D16, vector44, NPC.frame, color43, NPC.rotation, vector11, NPC.scale, spriteEffects, 0f);
                }
            }

            spriteBatch.Draw(texture2D15, vector43, NPC.frame, color37, NPC.rotation, vector11, NPC.scale, spriteEffects, 0f);

            spriteBatch.Draw(texture2D16, vector43, NPC.frame, color42, NPC.rotation, vector11, NPC.scale, spriteEffects, 0f);

            return false;
        }

        public override void FindFrame(int frameHeight)
        {
            // Percent life remaining
            float lifeRatio = NPC.life / (float)NPC.lifeMax;

            bool expertMode = Main.expertMode || BossRushEvent.BossRushActive;
            bool revenge = CalamityWorld.revenge || BossRushEvent.BossRushActive;
            bool death = CalamityWorld.death || BossRushEvent.BossRushActive;

            bool phase2 = lifeRatio < (death ? 0.9f : revenge ? 0.8f : expertMode ? 0.65f : 0.5f);
            bool phase3 = lifeRatio < (death ? 0.6f : revenge ? 0.5f : expertMode ? 0.35f : 0.2f);

            NPC.frameCounter += 1D;
            if (NPC.frameCounter > 6D)
            {
                NPC.frameCounter = 0D;
                NPC.frame.Y += frameHeight;
            }
            if (phase3)
            {
                if (NPC.frame.Y < frameHeight * 8)
                {
                    NPC.frame.Y = frameHeight * 8;
                }
                if (NPC.frame.Y > frameHeight * 11)
                {
                    NPC.frame.Y = frameHeight * 8;
                }
            }
            else if (phase2)
            {
                if (NPC.frame.Y < frameHeight * 4)
                {
                    NPC.frame.Y = frameHeight * 4;
                }
                if (NPC.frame.Y > frameHeight * 7)
                {
                    NPC.frame.Y = frameHeight * 4;
                }
            }
            else
            {
                if (NPC.frame.Y > frameHeight * 3)
                {
                    NPC.frame.Y = 0;
                }
            }
        }

        public override void OnHitPlayer(Player player, int damage, bool crit)
        {
            player.AddBuff(BuffID.MoonLeech, 900, true);
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            cooldownSlot = 1;
            return true;
        }

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            NPC.lifeMax = (int)(NPC.lifeMax * 0.8f * bossLifeScale);
            NPC.damage = (int)(NPC.damage * NPC.GetExpertDamageMultiplier());
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            Dust.NewDust(NPC.position, NPC.width, NPC.height, (int)CalamityDusts.Ectoplasm, hitDirection, -1f, 0, default, 1f);
            if (NPC.life <= 0)
            {
                NPC.position.X = NPC.position.X + (NPC.width / 2);
                NPC.position.Y = NPC.position.Y + (NPC.height / 2);
                NPC.width = 90;
                NPC.height = 90;
                NPC.position.X = NPC.position.X - (NPC.width / 2);
                NPC.position.Y = NPC.position.Y - (NPC.height / 2);
                for (int num621 = 0; num621 < 10; num621++)
                {
                    int num622 = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, (int)CalamityDusts.Phantoplasm, 0f, 0f, 100, default, 2f);
                    Main.dust[num622].velocity *= 3f;
                    if (Main.rand.NextBool(2))
                    {
                        Main.dust[num622].scale = 0.5f;
                        Main.dust[num622].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                    }
                }
                for (int num623 = 0; num623 < 60; num623++)
                {
                    int num624 = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, (int)CalamityDusts.Ectoplasm, 0f, 0f, 100, default, 3f);
                    Main.dust[num624].noGravity = true;
                    Main.dust[num624].velocity *= 5f;
                    num624 = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, (int)CalamityDusts.Ectoplasm, 0f, 0f, 100, default, 2f);
                    Main.dust[num624].velocity *= 2f;
                }
            }
        }
    }
}
