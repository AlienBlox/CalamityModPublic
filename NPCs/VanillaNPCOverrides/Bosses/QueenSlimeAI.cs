﻿using CalamityMod.Events;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.NPCs.VanillaNPCOverrides.Bosses
{
    public static class QueenSlimeAI
    {
        public static bool BuffedQueenSlimeAI(NPC npc, Mod mod)
        {
            // Difficulty bools
            bool bossRush = BossRushEvent.BossRushActive;
            bool death = CalamityWorld.death || bossRush;

            // Percent life remaining
            float lifeRatio = npc.life / (float)npc.lifeMax;

            float num3 = 1f;
            bool flag = false;
            bool phase2 = lifeRatio <= 0.5f;
            bool phase3 = lifeRatio <= 0.4f;
            bool phase4 = lifeRatio <= 0.2f;

            // Reset damage
            npc.damage = npc.defDamage;

            // Spawn settings
            if (npc.localAI[0] == 0f)
            {
                npc.ai[1] = -20f;
                npc.localAI[0] = npc.lifeMax;
                npc.TargetClosest();
                npc.netUpdate = true;
            }

            // Emit light
            Lighting.AddLight(npc.Center, 1f, 0.7f, 0.9f);

            // Despawn
            int despawnDistanceInTiles = 500;
            if (Main.player[npc.target].dead || Math.Abs(npc.Center.X - Main.player[npc.target].Center.X) / 16f > despawnDistanceInTiles)
            {
                npc.TargetClosest();
                if (Main.player[npc.target].dead || Math.Abs(npc.Center.X - Main.player[npc.target].Center.X) / 16f > despawnDistanceInTiles)
                {
                    npc.EncourageDespawn(10);
                    if (Main.player[npc.target].Center.X < npc.Center.X)
                        npc.direction = 1;
                    else
                        npc.direction = -1;
                }
            }

            // Teleport
            if (!Main.player[npc.target].dead && npc.timeLeft > 10 && !phase2 && npc.ai[3] >= 300f && npc.ai[0] == 0f && npc.velocity.Y == 0f)
            {
                // Avoid cheap bullshit
                npc.damage = 0;

                npc.ai[0] = 2f;
                npc.ai[1] = 0f;
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    npc.netUpdate = true;
                    npc.TargetClosest(faceTarget: false);
                    Point point = npc.Center.ToTileCoordinates();
                    Point point2 = Main.player[npc.target].Center.ToTileCoordinates();
                    Vector2 vector = Main.player[npc.target].Center - npc.Center;
                    int num5 = 10;
                    int num6 = 0;
                    int num7 = 7;
                    int num8 = 0;
                    bool flag3 = false;
                    if (npc.ai[3] >= 360f || vector.Length() > 2000f)
                    {
                        if (npc.ai[3] > 360f)
                            npc.ai[3] = 360f;

                        flag3 = true;
                        num8 = 100;
                    }

                    while (!flag3 && num8 < 100)
                    {
                        num8++;
                        int num9 = Main.rand.Next(point2.X - num5, point2.X + num5 + 1);
                        int num10 = Main.rand.Next(point2.Y - num5, point2.Y + 1);
                        if ((num10 >= point2.Y - num7 && num10 <= point2.Y + num7 && num9 >= point2.X - num7 && num9 <= point2.X + num7) || (num10 >= point.Y - num6 && num10 <= point.Y + num6 && num9 >= point.X - num6 && num9 <= point.X + num6) || Main.tile[num9, num10].HasUnactuatedTile)
                            continue;

                        int num11 = num10;
                        int i = 0;
                        if (Main.tile[num9, num11].HasUnactuatedTile && Main.tileSolid[Main.tile[num9, num11].TileType] && !Main.tileSolidTop[Main.tile[num9, num11].TileType])
                        {
                            i = 1;
                        }
                        else
                        {
                            for (; i < 150 && num11 + i < Main.maxTilesY; i++)
                            {
                                int num12 = num11 + i;
                                if (Main.tile[num9, num12].HasUnactuatedTile && Main.tileSolid[Main.tile[num9, num12].TileType] && !Main.tileSolidTop[Main.tile[num9, num12].TileType])
                                {
                                    i--;
                                    break;
                                }
                            }
                        }

                        num10 += i;
                        bool flag4 = true;
                        if (flag4 && Main.tile[num9, num10].LiquidType == LiquidID.Lava)
                            flag4 = false;

                        if (flag4 && !Collision.CanHitLine(npc.Center, 0, 0, Main.player[npc.target].Center, 0, 0))
                            flag4 = false;

                        if (flag4)
                        {
                            npc.localAI[1] = num9 * 16 + 8;
                            npc.localAI[2] = num10 * 16 + 16;
                            break;
                        }
                    }

                    if (num8 >= 100)
                    {
                        Vector2 bottom = Main.player[Player.FindClosest(npc.position, npc.width, npc.height)].Bottom;
                        npc.localAI[1] = bottom.X;
                        npc.localAI[2] = bottom.Y;
                        npc.ai[3] = 0f;
                    }
                }
            }

            // Get ready to teleport by increasing ai[3]
            if (!phase2 && (!Collision.CanHitLine(npc.Center, 0, 0, Main.player[npc.target].Center, 0, 0) || Math.Abs(npc.Top.Y - Main.player[npc.target].Bottom.Y) > 320f))
            {
                npc.ai[3] += death ? 3f : 1.5f;
            }
            else
            {
                float num13 = npc.ai[3];
                npc.ai[3] -= 1f;
                if (npc.ai[3] < 0f)
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient && num13 > 0f)
                        npc.netUpdate = true;

                    npc.ai[3] = 0f;
                }
            }

            // Reset variables if despawning
            if (npc.timeLeft <= 10 && ((phase2 && npc.ai[0] != 0f) || (!phase2 && npc.ai[0] != 3f)))
            {
                if (phase2)
                    npc.ai[0] = 0f;
                else
                    npc.ai[0] = 3f;

                npc.ai[1] = 0f;
                npc.ai[2] = 0f;
                npc.ai[3] = 0f;
                npc.netUpdate = true;
            }

            npc.noTileCollide = false;
            npc.noGravity = false;

            // Frame data shit I guess?
            if (phase2)
            {
                npc.localAI[3] += 1f;
                if (npc.localAI[3] >= 24f)
                    npc.localAI[3] = 0f;

                if ((npc.ai[0] == 4f || npc.ai[0] == 6f) && npc.ai[2] == 1f)
                    npc.localAI[3] = 6f;

                if (npc.ai[0] == 5f && npc.ai[2] != 1f)
                    npc.localAI[3] = 7f;
            }

            // Phases
            switch ((int)npc.ai[0])
            {
                // Phase switch phase
                case 0:

                    if (phase2)
                    {
                        QueenSlime_FlyMovement(npc);
                    }
                    else
                    {
                        npc.noTileCollide = false;
                        npc.noGravity = false;
                        if (npc.velocity.Y == 0f)
                        {
                            npc.velocity.X *= 0.8f;
                            if (npc.velocity.X > -0.1 && npc.velocity.X < 0.1)
                                npc.velocity.X = 0f;
                        }
                    }

                    if (npc.timeLeft <= 10 || (!phase2 && npc.velocity.Y != 0f))
                        break;

                    npc.ai[1] += 1f;
                    int idleTime = bossRush ? 20 : death ? 30 : 40;
                    if (phase2)
                        idleTime = bossRush ? 40 : death ? 60 : 80;
                    if (phase4)
                        idleTime /= 2;

                    if (!(npc.ai[1] > idleTime))
                        break;

                    npc.ai[1] = 0f;
                    if (phase2)
                    {
                        Player player = Main.player[npc.target];

                        switch ((int)npc.Calamity().newAI[0])
                        {
                            default:
                                npc.ai[0] = Main.rand.NextBool() ? 6f : 5f;
                                break;
                            case 5:
                                npc.ai[0] = phase4 ? 6f : Main.rand.NextBool() ? 4f : 6f;
                                break;
                            case 6:
                                npc.ai[0] = phase4 ? 5f : Main.rand.NextBool() ? 5f : 4f;
                                break;
                        }

                        if (npc.ai[0] == 4f || npc.ai[0] == 6f)
                        {
                            npc.ai[2] = 1f;
                            if (player != null && player.active && !player.dead && (player.Bottom.Y < npc.Bottom.Y || Math.Abs(player.Center.X - npc.Center.X) > 450f))
                            {
                                npc.ai[0] = 5f;
                                npc.ai[2] = 0f;
                            }
                        }
                    }
                    else
                    {
                        switch ((int)npc.Calamity().newAI[0])
                        {
                            default:
                                npc.ai[0] = Main.rand.NextBool() ? 5f : 4f;
                                break;
                            case 4:
                                npc.ai[0] = Main.rand.NextBool() ? 3f : 5f;
                                break;
                            case 5:
                                npc.ai[0] = Main.rand.NextBool() ? 4f : 3f;
                                break;
                        }
                    }

                    npc.netUpdate = true;
                    break;

                // Enlarge after teleport
                case 1:

                    // Avoid cheap bullshit
                    npc.damage = 0;

                    npc.rotation = 0f;
                    npc.ai[1] += 1f;
                    float teleportEndTime = bossRush ? 10f : death ? 15f : 20f;
                    num3 = MathHelper.Clamp(npc.ai[1] / teleportEndTime, 0f, 1f);
                    num3 = 0.5f + num3 * 0.5f;
                    if (npc.ai[1] >= teleportEndTime && Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        npc.ai[0] = 0f;
                        npc.ai[1] = 0f;
                        npc.netUpdate = true;
                        npc.TargetClosest();
                    }

                    if (Main.netMode == NetmodeID.MultiplayerClient && npc.ai[1] >= teleportEndTime * 2f)
                    {
                        npc.ai[0] = 0f;
                        npc.ai[1] = 0f;
                        npc.TargetClosest();
                    }

                    // Emit teleport dust
                    Color newColor2 = NPC.AI_121_QueenSlime_GetDustColor();
                    newColor2.A = 150;
                    for (int num26 = 0; num26 < 10; num26++)
                    {
                        int num27 = Dust.NewDust(npc.position + Vector2.UnitX * -20f, npc.width + 40, npc.height, 4, npc.velocity.X, npc.velocity.Y, 50, newColor2, 1.5f);
                        Main.dust[num27].noGravity = true;
                        Main.dust[num27].velocity *= 2f;
                    }

                    break;

                // Shrink and spawn teleport gore and dust
                case 2:

                    // Avoid cheap bullshit
                    npc.damage = 0;

                    npc.rotation = 0f;
                    npc.ai[1] += 1f;
                    float teleportTime = bossRush ? 20f : death ? 30f : 40f;
                    num3 = MathHelper.Clamp((teleportTime - npc.ai[1]) / teleportTime, 0f, 1f);
                    num3 = 0.5f + num3 * 0.5f;

                    if (npc.ai[1] >= teleportTime)
                        flag = true;

                    // Spawn crown gore
                    if (npc.ai[1] == teleportTime)
                        Gore.NewGore(npc.GetSource_FromAI(), npc.Center + new Vector2(-40f, -npc.height / 2), npc.velocity, GoreID.QueenSlimeCrown);

                    if (npc.ai[1] >= teleportTime && Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        npc.Bottom = new Vector2(npc.localAI[1], npc.localAI[2]);
                        npc.ai[0] = 1f;
                        npc.ai[1] = 0f;
                        npc.netUpdate = true;
                    }

                    if (Main.netMode == NetmodeID.MultiplayerClient && npc.ai[1] >= teleportTime * 2f)
                    {
                        npc.ai[0] = 1f;
                        npc.ai[1] = 0f;
                    }

                    // Emit teleport dust
                    if (!flag)
                    {
                        Color newColor = NPC.AI_121_QueenSlime_GetDustColor();
                        newColor.A = 150;
                        for (int n = 0; n < 10; n++)
                        {
                            int num25 = Dust.NewDust(npc.position + Vector2.UnitX * -20f, npc.width + 40, npc.height, 4, npc.velocity.X, npc.velocity.Y, 50, newColor, 1.5f);
                            Main.dust[num25].noGravity = true;
                            Main.dust[num25].velocity *= 0.5f;
                        }
                    }

                    break;

                // She jump
                case 3:

                    // Faster fall
                    if (npc.velocity.Y > 0f)
                        npc.velocity.Y += bossRush ? 0.1f : death ? 0.05f : 0f;

                    npc.rotation = 0f;
                    if (npc.velocity.Y == 0f)
                    {
                        npc.velocity.X *= 0.8f;
                        if (npc.velocity.X > -0.1 && npc.velocity.X < 0.1)
                            npc.velocity.X = 0f;

                        float timerIncrement = bossRush ? 7f : death ? 6f : 5f;
                        npc.ai[1] += timerIncrement;
                        if (lifeRatio < 0.85f)
                            npc.ai[1] += timerIncrement;
                        if (lifeRatio < 0.7f)
                            npc.ai[1] += timerIncrement;

                        if (!(npc.ai[1] >= 0f))
                            break;

                        float distanceBelowTarget = npc.position.Y - (Main.player[npc.target].position.Y + 80f);
                        float speedMult = 1f;
                        if (distanceBelowTarget > 0f)
                            speedMult += distanceBelowTarget * 0.002f;

                        if (speedMult > 2f)
                            speedMult = 2f;

                        npc.netUpdate = true;
                        npc.TargetClosest();
                        if (npc.ai[2] == 3f)
                        {
                            npc.velocity.Y = -13f * speedMult;
                            npc.velocity.X += (bossRush ? 7f : death ? 6f : 5.5f) * npc.direction;
                            npc.ai[1] = 0f;
                            npc.ai[2] = 0f;
                            if (npc.timeLeft > 10)
                            {
                                npc.Calamity().newAI[0] = npc.ai[0];
                                npc.SyncExtraAI();
                                npc.ai[0] = 0f;
                            }
                            else
                                npc.ai[1] = -60f;
                        }
                        else if (npc.ai[2] == 2f)
                        {
                            npc.velocity.Y = -(bossRush ? 10f : death ? 8f : 6f) * speedMult;
                            npc.velocity.X += (bossRush ? 8.5f : death ? 7.5f : 7f) * npc.direction;
                            npc.ai[1] = -40f;
                            npc.ai[2] += 1f;
                        }
                        else
                        {
                            npc.velocity.Y = -(bossRush ? 12f : death ? 10f : 8f) * speedMult;
                            npc.velocity.X += (bossRush ? 7.5f : death ? 6.5f : 6f) * npc.direction;
                            npc.ai[1] = -40f;
                            npc.ai[2] += 1f;
                        }
                    }
                    else
                    {
                        if (npc.target >= Main.maxPlayers)
                            break;

                        float num19 = bossRush ? 6.5f : death ? 5.5f : 4.5f;
                        if (Main.getGoodWorld)
                            num19 = 7f;

                        if ((npc.direction == 1 && npc.velocity.X < num19) || (npc.direction == -1 && npc.velocity.X > 0f - num19))
                        {
                            if ((npc.direction == -1 && npc.velocity.X < 0.1) || (npc.direction == 1 && npc.velocity.X > -0.1))
                                npc.velocity.X += (bossRush ? 0.5f : death ? 0.35f : 0.3f) * npc.direction;
                            else
                                npc.velocity.X *= bossRush ? 0.88f : death ? 0.9f : 0.91f;
                        }
                    }

                    break;

                // Slam down and create shockwave
                // Create a cascade of crystals while falling down in phase 3 and the case is 4
                // Release a massive eruption of crystals in phase 3 and the case is 6
                case 4:
                case 6:

                    npc.rotation *= 0.9f;
                    npc.noTileCollide = true;
                    npc.noGravity = true;

                    if (npc.ai[2] == 1f)
                    {
                        npc.noTileCollide = false;
                        npc.noGravity = false;

                        int num20 = 30;
                        if (phase2)
                            num20 = 10;

                        if (Main.getGoodWorld)
                            num20 = 0;

                        if (npc.velocity.Y == 0f)
                        {
                            SoundEngine.PlaySound(SoundID.Item167, npc.Center);
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                int type = ProjectileID.QueenSlimeSmash;
                                int damage = npc.GetProjectileDamage(type);
                                Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Bottom, Vector2.Zero, type, damage, 0f, Main.myPlayer);

                                // Eruption of crystals in phase 3
                                if (npc.ai[0] == 6f && phase3)
                                {
                                    float projectileVelocity = 12f;
                                    type = ProjectileID.QueenSlimeMinionBlueSpike;
                                    damage = npc.GetProjectileDamage(type);
                                    Vector2 destination = new Vector2(npc.Center.X, npc.Center.Y - 100f) - npc.Center;
                                    destination.Normalize();
                                    destination *= projectileVelocity;
                                    int numProj = 20;
                                    float rotation = MathHelper.ToRadians(100);
                                    for (int i = 0; i < numProj; i++)
                                    {
                                        Vector2 perturbedSpeed = destination.RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (float)(numProj - 1)));
                                        Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center, perturbedSpeed, type, damage, 0f, Main.myPlayer, 0f, -2f);
                                    }
                                }
                            }

                            for (int l = 0; l < 20; l++)
                            {
                                int num21 = Dust.NewDust(npc.Bottom - new Vector2(npc.width / 2, 30f), npc.width, 30, 31, npc.velocity.X, npc.velocity.Y, 40, NPC.AI_121_QueenSlime_GetDustColor());
                                Main.dust[num21].noGravity = true;
                                Main.dust[num21].velocity.Y = -5f + Main.rand.NextFloat() * -3f;
                                Main.dust[num21].velocity.X *= 7f;
                            }

                            npc.Calamity().newAI[0] = npc.ai[0];
                            npc.SyncExtraAI();
                            npc.ai[0] = 0f;
                            npc.ai[1] = 0f;
                            npc.ai[2] = 0f;
                            npc.netUpdate = true;
                        }
                        else if (npc.ai[1] >= num20)
                        {
                            for (int m = 0; m < 4; m++)
                            {
                                Vector2 position = npc.Bottom - new Vector2(Main.rand.NextFloatDirection() * 16f, Main.rand.Next(8));
                                int num22 = Dust.NewDust(position, 2, 2, 31, npc.velocity.X, npc.velocity.Y, 40, NPC.AI_121_QueenSlime_GetDustColor(), 1.4f);
                                Main.dust[num22].position = position;
                                Main.dust[num22].noGravity = true;
                                Main.dust[num22].velocity.Y = npc.velocity.Y * 0.9f;
                                Main.dust[num22].velocity.X = (Main.rand.NextBool() ? (-10f) : 10f) + Main.rand.NextFloatDirection() * 3f;
                            }
                        }

                        npc.velocity.X *= 0.8f;
                        float num23 = npc.ai[1];
                        npc.ai[1] += 1f;
                        if (npc.ai[1] >= num20)
                        {
                            if (num23 < num20)
                                npc.netUpdate = true;

                            if (phase2 && npc.ai[1] > (num20 + 120))
                            {
                                npc.Calamity().newAI[0] = npc.ai[0];
                                npc.SyncExtraAI();
                                npc.ai[0] = 0f;
                                npc.ai[1] = 0f;
                                npc.ai[2] = 0f;
                                npc.velocity.Y *= 0.8f;
                                npc.netUpdate = true;
                                break;
                            }

                            npc.velocity.Y += bossRush ? 2f : death ? 1.75f : 1.5f;
                            float num24 = bossRush ? 15.99f : death ? 15.5f : 15f;
                            if (Main.getGoodWorld)
                            {
                                npc.velocity.Y += 1f;
                                num24 = 15.99f;
                            }

                            if (npc.velocity.Y == 0f)
                                npc.velocity.Y = 0.01f;

                            if (npc.velocity.Y >= num24)
                                npc.velocity.Y = num24;

                            // Cascade of crystals in phase 3 or 4 while falling down
                            if (((npc.ai[0] == 4f && phase3) || phase4) && npc.ai[1] % 12f == 0f)
                            {
                                SoundEngine.PlaySound(SoundID.Item154, npc.Center);
                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                {
                                    Vector2 fireFrom = npc.Center;
                                    int projectileAmt = 2;
                                    int type = ProjectileID.QueenSlimeMinionBlueSpike;
                                    int damage = npc.GetProjectileDamage(type);
                                    for (int i = 0; i < projectileAmt; i++)
                                    {
                                        int totalProjectiles = 2;
                                        float radians = MathHelper.TwoPi / totalProjectiles;
                                        for (int j = 0; j < totalProjectiles; j++)
                                        {
                                            Vector2 projVelocity = npc.velocity.RotatedBy(radians * j + MathHelper.PiOver2);
                                            Projectile.NewProjectile(npc.GetSource_FromAI(), fireFrom, projVelocity, type, damage, 0f, Main.myPlayer, 0f, -1f);
                                        }
                                    }
                                }
                            }
                        }
                        else
                            npc.velocity.Y *= 0.8f;

                        break;
                    }

                    if (Main.netMode != NetmodeID.MultiplayerClient && npc.ai[1] == 0f)
                    {
                        npc.TargetClosest();
                        npc.netUpdate = true;
                    }

                    npc.ai[1] += 1f;
                    if (!(npc.ai[1] >= 30f))
                        break;

                    if (npc.ai[1] >= 60f)
                    {
                        npc.ai[1] = 60f;
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            npc.ai[1] = 0f;
                            npc.ai[2] = 1f;
                            npc.velocity.Y = -3f;
                            npc.netUpdate = true;
                        }
                    }

                    Player player3 = Main.player[npc.target];
                    Vector2 center = npc.Center;
                    if (!player3.dead && player3.active && Math.Abs(npc.Center.X - player3.Center.X) / 16f <= despawnDistanceInTiles)
                        center = player3.Center;

                    center.Y -= 384f;
                    if (npc.velocity.Y == 0f)
                    {
                        npc.velocity = center - npc.Center;
                        npc.velocity = npc.velocity.SafeNormalize(Vector2.Zero);
                        npc.velocity *= bossRush ? 30f : death ? 26f : 24f;
                    }
                    else
                        npc.velocity.Y *= 0.95f;

                    break;

                // Fire spread of gel projectiles
                case 5:

                    npc.rotation *= 0.9f;
                    npc.noTileCollide = true;
                    npc.noGravity = true;

                    if (phase2)
                        npc.ai[3] = 0f;

                    if (npc.ai[2] == 1f)
                    {
                        npc.ai[1] += 1f;
                        if (!(npc.ai[1] >= 10f))
                            break;

                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            int numGelProjectiles = phase4 ? Main.rand.Next(9, 12) : phase2 ? Main.rand.Next(6, 9) : 12;
                            if (Main.getGoodWorld)
                                numGelProjectiles = 15;

                            float projectileVelocity = death ? 12f : 10.5f;
                            int type = ProjectileID.QueenSlimeGelAttack;
                            int damage = npc.GetProjectileDamage(type);
                            if (phase2)
                            {
                                Vector2 destination = new Vector2(npc.Center.X, npc.Center.Y + 100f) - npc.Center;
                                destination.Normalize();
                                destination *= projectileVelocity;
                                float rotation = MathHelper.ToRadians(120);
                                for (int i = 0; i < numGelProjectiles; i++)
                                {
                                    Vector2 perturbedSpeed = destination.RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (float)(numGelProjectiles - 1)));
                                    int proj = Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center, perturbedSpeed, type, damage, 0f, Main.myPlayer, 0f, -2f);
                                    Main.projectile[proj].timeLeft = 900;
                                }
                            }
                            else
                            {
                                for (int j = 0; j < numGelProjectiles; j++)
                                {
                                    Vector2 spinningpoint = new Vector2(projectileVelocity, 0f);
                                    spinningpoint = spinningpoint.RotatedBy((-j) * ((float)Math.PI * 2f) / numGelProjectiles, Vector2.Zero);
                                    int proj = Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center, spinningpoint, type, damage, 0f, Main.myPlayer, 0f, -2f);
                                    Main.projectile[proj].timeLeft = 900;
                                }
                            }

                            // Fire gel balls directly at players with a max of 3
                            List<int> targets = new List<int>();
                            for (int p = 0; p < Main.maxPlayers; p++)
                            {
                                if (Main.player[p].active && !Main.player[p].dead)
                                    targets.Add(p);

                                if (targets.Count > 2)
                                    break;
                            }
                            foreach (int t in targets)
                            {
                                Vector2 velocity2 = Vector2.Normalize(Main.player[t].Center - npc.Center) * projectileVelocity;
                                int proj = Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center, velocity2, type, damage, 0f, Main.myPlayer, 0f, -2f);
                                Main.projectile[proj].timeLeft = 900;
                            }
                        }

                        SoundEngine.PlaySound(SoundID.Item155, npc.Center);
                        npc.Calamity().newAI[0] = npc.ai[0];
                        npc.SyncExtraAI();
                        npc.ai[0] = 0f;
                        npc.ai[1] = 0f;
                        npc.ai[2] = 0f;
                        npc.netUpdate = true;
                        break;
                    }

                    if (Main.netMode != NetmodeID.MultiplayerClient && npc.ai[1] == 0f)
                    {
                        npc.TargetClosest();
                        npc.netUpdate = true;
                    }

                    npc.ai[1] += 1f;
                    if (npc.ai[1] >= 50f)
                    {
                        npc.ai[1] = 50f;
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            npc.ai[1] = 0f;
                            npc.ai[2] = 1f;
                            npc.netUpdate = true;
                        }
                    }

                    float num16 = 100f;
                    for (int k = 0; k < 4; k++)
                    {
                        Vector2 vector2 = npc.Center + Main.rand.NextVector2CircularEdge(num16, num16);
                        if (!phase2)
                            vector2 += new Vector2(0f, 20f);

                        Vector2 v = vector2 - npc.Center;
                        v = v.SafeNormalize(Vector2.Zero) * -8f;
                        int num17 = Dust.NewDust(vector2, 2, 2, 31, v.X, v.Y, 40, NPC.AI_121_QueenSlime_GetDustColor(), 1.8f);
                        Main.dust[num17].position = vector2;
                        Main.dust[num17].noGravity = true;
                        Main.dust[num17].alpha = 250;
                        Main.dust[num17].velocity = v;
                        Main.dust[num17].customData = npc;
                    }

                    if (phase2)
                        QueenSlime_FlyMovement(npc);

                    break;
            }

            // Don't take damage while teleporting
            npc.dontTakeDamage = npc.hide = flag;

            // Adjust size with HP
            if (num3 != npc.scale)
            {
                npc.position.X += npc.width / 2;
                npc.position.Y += npc.height;
                npc.scale = num3;
                npc.width = (int)(114f * npc.scale);
                npc.height = (int)(100f * npc.scale);
                npc.position.X -= npc.width / 2;
                npc.position.Y -= npc.height;
            }

            // Spawn small slimes
            // Don't spawn any slimes in final phase
            if (npc.life <= 0 || phase4)
                return false;

            if (Main.netMode == NetmodeID.MultiplayerClient)
                return false;

            // Reset numerous variables when phase 2 begins
            if (npc.localAI[0] >= (npc.lifeMax / 2) && npc.life < npc.lifeMax / 2)
            {
                npc.localAI[0] = npc.life;
                npc.ai[0] = 0f;
                npc.ai[1] = 0f;
                npc.ai[2] = 0f;
                npc.netUpdate = true;
            }

            int num28 = (int)(npc.lifeMax * (phase3 ? 0.04f : phase2 ? 0.03f : 0.025f));
            if (!((npc.life + num28) < npc.localAI[0]))
                return false;

            npc.localAI[0] = npc.life;
            int x = (int)(npc.position.X + Main.rand.Next(npc.width - 32));
            int y = (int)(npc.position.Y + Main.rand.Next(npc.height - 32));

            int random = Main.rand.Next(2);
            if (phase2)
                random += 1;
            if (phase3)
                random = 2;

            int typeToSpawn = NPCID.QueenSlimeMinionBlue;
            switch (random)
            {
                case 0:
                    typeToSpawn = NPCID.QueenSlimeMinionBlue;
                    break;
                case 1:
                    typeToSpawn = NPCID.QueenSlimeMinionPink;
                    break;
                case 2:
                    typeToSpawn = NPCID.QueenSlimeMinionPurple;
                    break;
            }

            int num32 = NPC.NewNPC(npc.GetSource_FromAI(), x, y, typeToSpawn);
            Main.npc[num32].SetDefaults(typeToSpawn);
            Main.npc[num32].velocity.X = Main.rand.Next(-15, 16) * 0.1f;
            Main.npc[num32].velocity.Y = Main.rand.Next(-30, 1) * 0.1f;
            Main.npc[num32].ai[0] = -500 * Main.rand.Next(3);
            Main.npc[num32].ai[1] = 0f;
            if (Main.netMode == NetmodeID.Server && num32 < 200)
                NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, num32);

            return false;
        }

        public static void QueenSlime_FlyMovement(NPC npc)
        {
            // Difficulty bools
            bool bossRush = BossRushEvent.BossRushActive;
            bool death = CalamityWorld.death;

            npc.noTileCollide = true;
            npc.noGravity = true;

            float flyVelocity = bossRush ? 20f : death ? 18f : 16f;
            float flyAcceleration = bossRush ? 0.18f : death ? 0.14f : 0.12f;
            float flyDistanceY = 450f;

            npc.TargetClosest();

            Vector2 desiredVelocity = npc.Center;

            if (npc.timeLeft > 10)
            {
                if (!Collision.CanHit(npc, Main.player[npc.target]))
                {
                    bool flyToSolidTilesAboveTarget = false;
                    Vector2 center = Main.player[npc.target].Center;
                    for (int i = 0; i < 16; i++)
                    {
                        float tileDistanceAboveTarget = 16 * i;
                        Point point = (center + new Vector2(0f, 0f - tileDistanceAboveTarget)).ToTileCoordinates();
                        if (WorldGen.SolidOrSlopedTile(point.X, point.Y))
                        {
                            desiredVelocity = center + new Vector2(0f, 0f - tileDistanceAboveTarget + 16f) - npc.Center;
                            flyToSolidTilesAboveTarget = true;
                            break;
                        }
                    }

                    if (!flyToSolidTilesAboveTarget)
                        desiredVelocity = center - npc.Center;
                }
                else
                    desiredVelocity = Main.player[npc.target].Center + new Vector2(0f, -flyDistanceY) - npc.Center;
            }
            else
                desiredVelocity = npc.Center + new Vector2(500f * npc.direction, -flyDistanceY) - npc.Center;

            float distanceFromFlightTarget = desiredVelocity.Length();
            if (Math.Abs(desiredVelocity.X) < 40f)
                desiredVelocity.X = npc.velocity.X;

            if (distanceFromFlightTarget > 100f && ((npc.velocity.X < -12f && desiredVelocity.X > 0f) || (npc.velocity.X > 12f && desiredVelocity.X < 0f)))
                flyAcceleration = 0.2f;

            if (distanceFromFlightTarget < 40f)
            {
                desiredVelocity = npc.velocity;
            }
            else if (distanceFromFlightTarget < 80f)
            {
                desiredVelocity.Normalize();
                desiredVelocity *= flyVelocity * 0.65f;
            }
            else
            {
                desiredVelocity.Normalize();
                desiredVelocity *= flyVelocity;
            }

            npc.SimpleFlyMovement(desiredVelocity, flyAcceleration);
            npc.rotation = npc.velocity.X * 0.1f;
            if (npc.rotation > 0.5f)
                npc.rotation = 0.5f;

            if (npc.rotation < -0.5f)
                npc.rotation = -0.5f;
        }
    }
}
