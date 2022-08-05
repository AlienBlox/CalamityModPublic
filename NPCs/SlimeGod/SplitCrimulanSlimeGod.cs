﻿using CalamityMod.Events;
using CalamityMod.Projectiles.Boss;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.SlimeGod
{
    [AutoloadBossHead]
    public class SplitCrimulanSlimeGod : ModNPC
    {
        private float bossLife;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Crimulan Slime God");
            Main.npcFrameCount[NPC.type] = 6;
            NPCID.Sets.BossBestiaryPriority.Add(Type);
            NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers(0)
            {
                Scale = 0.6f,
                PortraitScale = 1f,
                PortraitPositionYOverride = 0
            };
            value.Position.Y += 10;
            NPCID.Sets.NPCBestiaryDrawOffset[Type] = value;
        }

        public override void SetDefaults()
        {
            NPC.Calamity().canBreakPlayerDefense = true;
            NPC.LifeMaxNERB(1800, 2160, 80000);
            double HPBoost = CalamityConfig.Instance.BossHealthBoost * 0.01;
            NPC.lifeMax += (int)(NPC.lifeMax * HPBoost);
            NPC.GetNPCDamage();
            NPC.width = 150;
            NPC.height = 92;
            NPC.scale = 0.8f;
            NPC.defense = 10;
            NPC.knockBackResist = 0f;
            AnimationType = NPCID.KingSlime;
            NPC.value = Item.buyPrice(0, 1, 0, 0);
            NPC.Opacity = 0.8f;
            NPC.lavaImmune = false;
            NPC.noGravity = false;
            NPC.noTileCollide = false;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            Music = CalamityMod.Instance.GetMusicFromMusicMod("SlimeGod") ?? MusicID.Boss1;
            NPC.aiStyle = -1;
            AIType = -1;
            NPC.Calamity().VulnerableToHeat = true;
            NPC.Calamity().VulnerableToSickness = false;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.TheCorruption,
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.TheCrimson,

				// Will move to localization whenever that is cleaned up.
				new FlavorTextBestiaryInfoElement("Flanking their creator and serving as its body, these are concentrated, swirling masses of the world’s evils, each utilizing the rot and corrosion to eat away and assimilate those who oppose it.")
            });
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(NPC.localAI[0]);
            writer.Write(NPC.localAI[1]);
            writer.Write(NPC.localAI[2]);
            writer.Write(NPC.localAI[3]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            NPC.localAI[0] = reader.ReadSingle();
            NPC.localAI[1] = reader.ReadSingle();
            NPC.localAI[2] = reader.ReadSingle();
            NPC.localAI[3] = reader.ReadSingle();
        }

        public override void AI()
        {
            if (CalamityGlobalNPC.slimeGodRed < 0 || !Main.npc[CalamityGlobalNPC.slimeGodRed].active)
                CalamityGlobalNPC.slimeGodRed = NPC.whoAmI;

            bool bossRush = BossRushEvent.BossRushActive;
            bool expertMode = Main.expertMode || bossRush;
            bool revenge = CalamityWorld.revenge || bossRush;
            bool death = CalamityWorld.death || NPC.localAI[1] == 1f || bossRush;

            float lifeRatio = NPC.life / (float)NPC.lifeMax;

            NPC.defense = NPC.defDefense;
            NPC.damage = NPC.defDamage;
            if (NPC.localAI[1] == 1f)
            {
                NPC.defense = NPC.defDefense + 20;
                NPC.damage = NPC.defDamage + 22;
            }

            float scale = Main.getGoodWorld ? 0.8f : 1f;
            NPC.aiAction = 0;
            NPC.noTileCollide = false;
            NPC.noGravity = false;

            // Get a target
            if (NPC.target < 0 || NPC.target == Main.maxPlayers || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
                NPC.TargetClosest();

            // Despawn safety, make sure to target another player if the current player target is too far away
            if (Vector2.Distance(Main.player[NPC.target].Center, NPC.Center) > CalamityGlobalNPC.CatchUpDistance200Tiles)
                NPC.TargetClosest();

            Player player = Main.player[NPC.target];

            if (NPC.ai[0] != 3f && (player.dead || !player.active))
            {
                NPC.TargetClosest();
                player = Main.player[NPC.target];
                if (player.dead || !player.active)
                {
                    NPC.ai[0] = 3f;
                    NPC.ai[1] = 0f;
                    NPC.ai[2] = 0f;
                    NPC.ai[3] = 0f;
                    NPC.netUpdate = true;
                }
            }
            else if (NPC.timeLeft < 1800)
                NPC.timeLeft = 1800;

            bool enraged = true;
            if (CalamityGlobalNPC.slimeGodPurple != -1)
            {
                if (Main.npc[CalamityGlobalNPC.slimeGodPurple].active)
                {
                    enraged = false;
                }
            }
            if (bossRush)
            {
                enraged = true;
            }

            if (NPC.localAI[1] != 1f)
            {
                if (enraged)
                    NPC.defense = NPC.defDefense * 2;
            }

            // Teleport
            float teleportGateValue = 720f;
            if (!player.dead && NPC.timeLeft > 10 && NPC.ai[3] >= teleportGateValue && NPC.ai[0] == 0f && NPC.velocity.Y == 0f)
            {
                // Avoid cheap bullshit
                NPC.damage = 0;

                // Slow down dramatically
                NPC.velocity.X *= 0.5f;

                NPC.ai[0] = 5f;
                NPC.ai[1] = 0f;
                NPC.ai[2] = 0f;
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    NPC.netUpdate = true;
                    NPC.TargetClosest(false);
                    player = Main.player[NPC.target];
                    Vector2 vectorAimedAheadOfTarget = player.Center + new Vector2((float)Math.Round(player.velocity.X), 0f).SafeNormalize(Vector2.Zero) * 1120f;
                    Point point2 = vectorAimedAheadOfTarget.ToTileCoordinates();
                    int num5 = 5;
                    int num8 = 0;
                    while (num8 < 100)
                    {
                        num8++;
                        int num9 = Main.rand.Next(point2.X - num5, point2.X + num5 + 1);
                        int num10 = Main.rand.Next(point2.Y - num5, point2.Y);
                        if (Main.tile[num9, num10].HasUnactuatedTile)
                            continue;

                        bool flag4 = true;
                        if (flag4 && Main.tile[num9, num10].LiquidType == LiquidID.Lava)
                            flag4 = false;
                        if (flag4 && !Collision.CanHitLine(NPC.Center, 0, 0, vectorAimedAheadOfTarget, 0, 0))
                            flag4 = false;

                        if (flag4)
                        {
                            NPC.localAI[0] = num9 * 16 + 8;
                            NPC.localAI[3] = num10 * 16 + 16;
                            NPC.ai[3] = 0f;
                            break;
                        }
                    }

                    // Default teleport if the above conditions aren't met in 100 iterations
                    if (num8 >= 100)
                    {
                        Vector2 bottom = Main.player[Player.FindClosest(NPC.position, NPC.width, NPC.height)].Bottom;
                        NPC.localAI[0] = bottom.X;
                        NPC.localAI[3] = bottom.Y;
                        NPC.ai[3] = 0f;
                    }
                }
            }

            // Get ready to teleport by increasing ai[3]
            if (NPC.ai[3] < teleportGateValue)
            {
                if (!Collision.CanHitLine(NPC.Center, 0, 0, player.Center, 0, 0) || Math.Abs(NPC.Top.Y - player.Bottom.Y) > 320f)
                    NPC.ai[3] += death ? 3f : 2f;
                else
                    NPC.ai[3] += 1f;
            }

            float distanceSpeedBoost = Vector2.Distance(player.Center, NPC.Center) * (bossRush ? 0.008f : 0.005f);

            if (NPC.ai[0] == 0f)
            {
                bool phaseThroughTilesToReachTarget = Vector2.Distance(player.Center, NPC.Center) > 2400f || !Collision.CanHit(NPC.position, NPC.width, NPC.height, player.position, player.width, player.height);
                if (Main.netMode != NetmodeID.MultiplayerClient && phaseThroughTilesToReachTarget)
                {
                    NPC.ai[0] = 4f;
                    NPC.ai[1] = 0f;
                    NPC.ai[2] = 0f;
                    NPC.netUpdate = true;
                }

                if (NPC.velocity.Y == 0f)
                {
                    NPC.TargetClosest();
                    NPC.velocity.X *= 0.8f;
                    NPC.ai[1] += 1f;

                    float jumpGateValue = 35f;
                    float velocityX = death ? 11.5f : revenge ? 10.5f : expertMode ? 9.5f : 7.5f;
                    if (revenge)
                    {
                        float moveBoost = death ? 11f * (1f - lifeRatio) : 7f * (1f - lifeRatio);
                        float speedBoost = death ? 2.25f * (1f - lifeRatio) : 1.5f * (1f - lifeRatio);
                        jumpGateValue -= moveBoost;
                        velocityX += speedBoost;
                    }

                    float distanceBelowTarget = NPC.position.Y - (player.position.Y + 80f);
                    float speedMult = 1f;
                    if (distanceBelowTarget > 0f)
                        speedMult += distanceBelowTarget * 0.002f;

                    if (speedMult > 2f)
                        speedMult = 2f;

                    float velocityY = 4f;
                    if (!Collision.CanHit(NPC.Center, 1, 1, player.Center, 1, 1))
                        velocityY += 2f;

                    if (NPC.ai[1] > jumpGateValue)
                    {
                        velocityX *= 1.25f;
                        NPC.ai[1] = 0f;
                        NPC.velocity.Y -= velocityY * speedMult;
                        NPC.velocity.X = (velocityX + distanceSpeedBoost) * NPC.direction;
                        NPC.noTileCollide = true;
                        NPC.netUpdate = true;
                    }
                    else if (NPC.ai[1] >= jumpGateValue - 30f)
                        NPC.aiAction = 1;
                }
                else
                {
                    NPC.velocity.X *= 0.99f;
                    if (NPC.direction < 0 && NPC.velocity.X > -1f)
                        NPC.velocity.X = -1f;
                    if (NPC.direction > 0 && NPC.velocity.X < 1f)
                        NPC.velocity.X = 1f;

                    if (!player.dead)
                    {
                        if (NPC.velocity.Y > 0f && NPC.Bottom.Y > player.Top.Y)
                            NPC.noTileCollide = false;
                        else if (Collision.CanHit(NPC.position, NPC.width, NPC.height, player.Center, 1, 1) && !Collision.SolidCollision(NPC.position, NPC.width, NPC.height))
                            NPC.noTileCollide = false;
                        else
                            NPC.noTileCollide = true;
                    }
                }

                NPC.ai[2] += 1f;
                if (revenge)
                    NPC.ai[2] += death ? 1f * (1f - lifeRatio) : 0.5f * (1f - lifeRatio);

                if (NPC.velocity.Y == 0f)
                {
                    float phaseSwitchGateValue = 180f;
                    bool switchPhase = NPC.ai[2] >= phaseSwitchGateValue;
                    if (switchPhase)
                    {
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            switch ((int)NPC.localAI[2])
                            {
                                default:
                                case 1:
                                    NPC.ai[0] = 2f;
                                    break;

                                case 2:
                                    NPC.ai[0] = 1f;
                                    break;
                            }

                            if (NPC.ai[0] == 1f)
                            {
                                NPC.noTileCollide = true;
                                NPC.velocity.Y = death ? -11f : revenge ? -10f : expertMode ? -9f : -8f;
                            }

                            NPC.ai[1] = 0f;
                            NPC.ai[2] = 0f;
                            NPC.netUpdate = true;
                        }
                    }
                    else if (NPC.ai[1] >= phaseSwitchGateValue - 30f)
                        NPC.aiAction = 1;
                }
            }
            else if (NPC.ai[0] == 1f)
            {
                NPC.noTileCollide = true;
                NPC.noGravity = true;

                if (NPC.velocity.X < 0f)
                    NPC.direction = -1;
                else
                    NPC.direction = 1;

                NPC.spriteDirection = NPC.direction;
                NPC.TargetClosest();
                Vector2 center40 = player.Center;
                center40.Y -= 350f;
                Vector2 vector272 = center40 - NPC.Center;

                if (NPC.ai[2] == 1f)
                {
                    NPC.ai[1] += 1f;
                    vector272 = player.Center - NPC.Center;
                    vector272.Normalize();
                    vector272 *= death ? 11f : revenge ? 10f : expertMode ? 9f : 8f;
                    NPC.velocity = (NPC.velocity * 4f + vector272) / 5f;

                    if (NPC.ai[1] > 12f)
                    {
                        NPC.ai[1] = 0f;
                        NPC.ai[0] = 1.1f;
                        NPC.ai[2] = 0f;
                        NPC.velocity = vector272;
                    }
                }
                else
                {
                    if (Math.Abs(NPC.Center.X - player.Center.X) < 40f && NPC.Center.Y < player.Center.Y - 300f)
                    {
                        NPC.ai[1] = 0f;
                        NPC.ai[2] = 1f;
                        return;
                    }

                    vector272.Normalize();
                    vector272 *= (death ? 13f : revenge ? 12f : expertMode ? 11f : 10f) + distanceSpeedBoost;
                    NPC.velocity = (NPC.velocity * 5f + vector272) / 6f;
                }
            }
            else if (NPC.ai[0] == 1.1f)
            {
                bool atTargetPosition = NPC.position.Y + NPC.height >= player.position.Y;
                if (NPC.ai[2] == 0f && (atTargetPosition || NPC.localAI[1] == 0f) && Collision.CanHit(NPC.Center, 1, 1, player.Center, 1, 1) && !Collision.SolidCollision(NPC.position, NPC.width, NPC.height))
                {
                    NPC.ai[2] = 1f;
                    NPC.netUpdate = true;
                }

                if (atTargetPosition || NPC.velocity.Y <= 0f)
                {
                    NPC.ai[1] += 1f;
                    if (NPC.ai[1] > 10f)
                    {
                        SoundEngine.PlaySound(SoundID.Item33, NPC.Center);
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            // Eruption of slime balls that fall down
                            float projectileVelocity = 8f;
                            int type = ModContent.ProjectileType<UnstableCrimulanGlob>();
                            int damage = NPC.GetProjectileDamage(type);
                            Vector2 destination = new Vector2(NPC.Center.X, NPC.Center.Y - 100f) - NPC.Center;
                            destination.Normalize();
                            destination *= projectileVelocity;
                            int numProj = 3;
                            float rotation = MathHelper.ToRadians(45);
                            for (int i = 0; i < numProj; i++)
                            {
                                Vector2 perturbedSpeed = destination.RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (float)(numProj - 1)));
                                Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + Vector2.Normalize(perturbedSpeed) * 30f * NPC.scale, perturbedSpeed * 1.5f, type, damage, 0f, Main.myPlayer, 1f, 0f);
                            }

                            // Fire slime balls directly at players with a max of 2
                            if (enraged && expertMode)
                            {
                                List<int> targets = new List<int>();
                                for (int p = 0; p < Main.maxPlayers; p++)
                                {
                                    if (Main.player[p].active && !Main.player[p].dead)
                                        targets.Add(p);

                                    if (targets.Count > 1)
                                        break;
                                }
                                foreach (int t in targets)
                                {
                                    Vector2 velocity2 = Vector2.Normalize(Main.player[t].Center - NPC.Center) * projectileVelocity;
                                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + Vector2.Normalize(velocity2) * 30f * NPC.scale, velocity2, type, damage, 0f, Main.myPlayer);
                                }
                            }
                        }

                        NPC.localAI[2] = NPC.ai[0] - 0.1f;
                        NPC.ai[0] = 0f;
                        NPC.ai[1] = 0f;
                        NPC.ai[2] = 0f;
                        NPC.netUpdate = true;
                    }
                }
                else if (NPC.ai[2] == 0f)
                    NPC.noTileCollide = true;

                NPC.noGravity = true;

                NPC.velocity.Y += 0.5f;
                float velocityLimit = bossRush ? 22f : death ? 16f : revenge ? 15f : expertMode ? 14f : 13f;
                if (NPC.velocity.Y > velocityLimit)
                    NPC.velocity.Y = velocityLimit;
            }
            else if (NPC.ai[0] == 2f)
            {
                if (NPC.velocity.Y == 0f)
                {
                    NPC.TargetClosest();
                    NPC.velocity.X *= 0.8f;

                    NPC.ai[1] += 1f;
                    if (NPC.ai[1] > 15f)
                    {
                        NPC.ai[1] = 0f;

                        NPC.velocity.Y -= 3f;
                        if (player.position.Y + player.height < NPC.Center.Y)
                            NPC.velocity.Y -= 1.25f;
                        if (player.position.Y + player.height < NPC.Center.Y - 40f)
                            NPC.velocity.Y -= 1.5f;
                        if (player.position.Y + player.height < NPC.Center.Y - 80f)
                            NPC.velocity.Y -= 1.75f;
                        if (player.position.Y + player.height < NPC.Center.Y - 120f)
                            NPC.velocity.Y -= 2f;
                        if (player.position.Y + player.height < NPC.Center.Y - 160f)
                            NPC.velocity.Y -= 2.25f;
                        if (player.position.Y + player.height < NPC.Center.Y - 200f)
                            NPC.velocity.Y -= 2.5f;
                        if (!Collision.CanHit(NPC.Center, 1, 1, player.Center, 1, 1))
                            NPC.velocity.Y -= 2f;

                        NPC.velocity.X = ((death ? 14f : revenge ? 13f : expertMode ? 12f : 11f) + distanceSpeedBoost) * NPC.direction;
                        NPC.ai[2] += 1f;
                    }
                    else
                        NPC.aiAction = 1;
                }
                else
                {
                    NPC.velocity.X *= 0.98f;
                    float velocityLimit = (death ? 5.5f : revenge ? 5f : expertMode ? 4.5f : 4f);
                    if (NPC.direction < 0 && NPC.velocity.X > -velocityLimit)
                        NPC.velocity.X = -velocityLimit;
                    if (NPC.direction > 0 && NPC.velocity.X < velocityLimit)
                        NPC.velocity.X = velocityLimit;
                }

                if (NPC.ai[2] >= 3f && NPC.velocity.Y == 0f)
                {
                    NPC.localAI[2] = NPC.ai[0];
                    NPC.ai[0] = 0f;
                    NPC.ai[1] = 0f;
                    NPC.ai[2] = 0f;
                    NPC.netUpdate = true;
                }
            }
            else if (NPC.ai[0] == 3f)
            {
                NPC.noTileCollide = true;
                NPC.Opacity -= 0.03f;

                if (NPC.timeLeft > 10)
                    NPC.timeLeft = 10;

                if (NPC.Opacity < 0f)
                    NPC.Opacity = 0f;

                NPC.velocity.X *= 0.98f;
            }
            else if (NPC.ai[0] == 4f)
            {
                if (NPC.velocity.X > 0f)
                    NPC.direction = 1;
                else
                    NPC.direction = -1;

                NPC.spriteDirection = NPC.direction;
                NPC.noTileCollide = true;
                NPC.noGravity = true;
                NPC.knockBackResist = 0f;
                Vector2 distanceFromTarget = player.Center - NPC.Center;
                distanceFromTarget.Y -= 16f;
                if (Main.netMode != NetmodeID.MultiplayerClient && distanceFromTarget.Length() < 500f && !Collision.SolidCollision(NPC.position, NPC.width, NPC.height) &&
                    Collision.CanHit(NPC.position, NPC.width, NPC.height, player.position, player.width, player.height))
                {
                    NPC.ai[0] = 0f;
                    NPC.ai[1] = 0f;
                    NPC.ai[2] = 0f;
                    NPC.netUpdate = true;
                }

                if (distanceFromTarget.Length() > 12f)
                {
                    distanceFromTarget.Normalize();
                    distanceFromTarget *= 12f;
                }

                NPC.velocity = (NPC.velocity * 4f + distanceFromTarget) / 5f;
            }
            else if (NPC.ai[0] == 5f)
            {
                // Avoid cheap bullshit
                NPC.damage = 0;

                NPC.aiAction = 1;
                NPC.ai[1] += 1f;
                float teleportTime = bossRush ? 20f : death ? 30f : 40f;
                scale = MathHelper.Clamp((teleportTime - NPC.ai[1]) / teleportTime, 0f, 1f);
                scale = 0.5f + scale * 0.5f;
                if (NPC.ai[1] >= teleportTime && Main.netMode != NetmodeID.MultiplayerClient)
                {
                    NPC.Bottom = new Vector2(NPC.localAI[0], NPC.localAI[3]);
                    NPC.ai[0] = 6f;
                    NPC.ai[1] = 0f;
                    NPC.netUpdate = true;
                }

                if (Main.netMode == NetmodeID.MultiplayerClient && NPC.ai[1] >= teleportTime * 2f)
                {
                    NPC.ai[0] = 6f;
                    NPC.ai[1] = 0f;
                }

                // Emit teleport dust
                Color dustColor = Color.Crimson;
                dustColor.A = 150;
                for (int num245 = 0; num245 < 10; num245++)
                {
                    int num246 = Dust.NewDust(NPC.position + Vector2.UnitX * -20f, NPC.width + 40, NPC.height, 4, NPC.velocity.X, NPC.velocity.Y, NPC.alpha, dustColor, 2f);
                    Main.dust[num246].noGravity = true;
                    Main.dust[num246].velocity *= 0.5f;
                }
            }
            else if (NPC.ai[0] == 6f)
            {
                // Avoid cheap bullshit
                NPC.damage = 0;

                NPC.ai[1] += 1f;
                float teleportEndTime = bossRush ? 10f : death ? 15f : 20f;
                scale = MathHelper.Clamp(NPC.ai[1] / teleportEndTime, 0f, 1f);
                scale = 0.5f + scale * 0.5f;
                if (NPC.ai[1] >= teleportEndTime && Main.netMode != NetmodeID.MultiplayerClient)
                {
                    NPC.ai[0] = 0f;
                    NPC.ai[1] = 0f;
                    NPC.netUpdate = true;
                    NPC.TargetClosest();
                }

                if (Main.netMode == NetmodeID.MultiplayerClient && NPC.ai[1] >= teleportEndTime * 2f)
                {
                    NPC.ai[0] = 0f;
                    NPC.ai[1] = 0f;
                    NPC.TargetClosest();
                }

                // Emit teleport dust
                Color dustColor = Color.Crimson;
                dustColor.A = 150;
                for (int num245 = 0; num245 < 10; num245++)
                {
                    int num246 = Dust.NewDust(NPC.position + Vector2.UnitX * -20f, NPC.width + 40, NPC.height, 4, NPC.velocity.X, NPC.velocity.Y, NPC.alpha, dustColor, 2f);
                    Main.dust[num246].noGravity = true;
                    Main.dust[num246].velocity *= 0.5f;
                }
            }

            if (bossLife == 0f && NPC.life > 0)
                bossLife = NPC.lifeMax;

            if (NPC.life > 0)
            {
                float num659 = lifeRatio;
                num659 = num659 * 0.5f + 0.75f;
                num659 *= scale;

                if (num659 != NPC.scale)
                {
                    NPC.position.X = NPC.position.X + (float)(NPC.width / 2);
                    NPC.position.Y = NPC.position.Y + (float)NPC.height;
                    NPC.scale = num659 * 0.75f;
                    NPC.width = (int)(150f * NPC.scale);
                    NPC.height = (int)(92f * NPC.scale);
                    NPC.position.X = NPC.position.X - (float)(NPC.width / 2);
                    NPC.position.Y = NPC.position.Y - (float)NPC.height;
                }

                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    int num660 = (int)((double)NPC.lifeMax * 0.15);
                    if ((float)(NPC.life + num660) < bossLife)
                    {
                        bossLife = (float)NPC.life;
                        int x = (int)(NPC.position.X + (float)Main.rand.Next(NPC.width - 32));
                        int y = (int)(NPC.position.Y + (float)Main.rand.Next(NPC.height - 32));
                        int num663 = ModContent.NPCType<CrimsonSlimeSpawn>();
                        if (Main.rand.NextBool(3))
                        {
                            num663 = ModContent.NPCType<CrimsonSlimeSpawn2>();
                        }
                        int num664 = NPC.NewNPC(NPC.GetSource_FromAI(), x, y, num663, 0, 0f, 0f, 0f, 0f, 255);
                        Main.npc[num664].SetDefaults(num663);
                        Main.npc[num664].velocity.X = (float)Main.rand.Next(-15, 16) * 0.1f;
                        Main.npc[num664].velocity.Y = (float)Main.rand.Next(-30, 1) * 0.1f;
                        Main.npc[num664].ai[0] = (float)(-1000 * Main.rand.Next(3));
                        Main.npc[num664].ai[1] = 0f;
                        if (Main.netMode == NetmodeID.Server && num664 < 200)
                            NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, num664, 0f, 0f, 0f, 0, 0, 0);
                    }
                }
            }
        }

        public override Color? GetAlpha(Color drawColor)
        {
            Color lightColor = new Color(Main.DiscoR, 100, 150, NPC.alpha);
            Color newColor = NPC.localAI[1] == 1f ? lightColor : new Color(255, 255, 255, drawColor.A);
            return newColor * NPC.Opacity;
        }

        public override void OnKill()
        {
            int heartAmt = Main.rand.Next(3) + 3;
            for (int i = 0; i < heartAmt; i++)
                Item.NewItem(NPC.GetSource_Loot(), (int)NPC.position.X, (int)NPC.position.Y, NPC.width, NPC.height, ItemID.Heart);
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            // Every Slime God piece drops Gel, even if it's not the last one.
            npcLoot.Add(ItemID.Gel, 1, 32, 48);
        }

        public override bool CheckActive() => false;

        public override void HitEffect(int hitDirection, double damage)
        {
            Color dustColor = Color.Crimson;
            dustColor.A = 150;
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, 4, hitDirection, -1f, NPC.alpha, dustColor, 1f);
            }
            if (NPC.life <= 0)
            {
                NPC.position.X = NPC.position.X + (float)(NPC.width / 2);
                NPC.position.Y = NPC.position.Y + (float)(NPC.height / 2);
                NPC.width = 50;
                NPC.height = 50;
                NPC.position.X = NPC.position.X - (float)(NPC.width / 2);
                NPC.position.Y = NPC.position.Y - (float)(NPC.height / 2);
                for (int num621 = 0; num621 < 40; num621++)
                {
                    int num622 = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, 4, 0f, 0f, NPC.alpha, dustColor, 2f);
                    Main.dust[num622].velocity *= 3f;
                    if (Main.rand.NextBool(2))
                    {
                        Main.dust[num622].scale = 0.5f;
                        Main.dust[num622].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                    }
                }
                for (int num623 = 0; num623 < 70; num623++)
                {
                    int num624 = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, 4, 0f, 0f, NPC.alpha, dustColor, 3f);
                    Main.dust[num624].noGravity = true;
                    Main.dust[num624].velocity *= 5f;
                    num624 = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, 4, 0f, 0f, NPC.alpha, dustColor, 2f);
                    Main.dust[num624].velocity *= 2f;
                }
            }
        }

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            NPC.lifeMax = (int)(NPC.lifeMax * 0.8f * bossLifeScale);
        }

        public override void OnHitPlayer(Player player, int damage, bool crit)
        {
            player.AddBuff(BuffID.Darkness, 240, true);
        }
    }
}
