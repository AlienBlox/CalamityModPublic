﻿using CalamityMod.Events;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.NPCs.VanillaNPCOverrides.Bosses
{
    public static class EyeOfCthulhuAI
    {
        public static bool BuffedEyeofCthulhuAI(NPC npc, Mod mod)
        {
            CalamityGlobalNPC calamityGlobalNPC = npc.Calamity();

            // Percent life remaining
            float lifeRatio = npc.life / (float)npc.lifeMax;

            bool bossRush = BossRushEvent.BossRushActive;
            bool death = CalamityWorld.death || bossRush;
            bool phase2 = lifeRatio < 0.6f;
            bool phase3 = lifeRatio < 0.3f;
            bool finalPhaseRev = lifeRatio < 0.15f;
            bool penultimatePhaseDeath = lifeRatio < 0.2f;
            bool finalPhaseDeath = lifeRatio < 0.1f;

            float lineUpDist = death ? 15f : 20f;

            // Servant and projectile velocity, the projectile velocity is multiplied by 2
            float servantAndProjectileVelocity = death ? 8f : 6f;

            float enrageScale = bossRush ? 1f : 0f;
            if (Main.dayTime || bossRush)
            {
                npc.Calamity().CurrentlyEnraged = !bossRush;
                enrageScale += 2f;
            }

            npc.reflectsProjectiles = false;

            // Get a target
            if (npc.target < 0 || npc.target == Main.maxPlayers || Main.player[npc.target].dead || !Main.player[npc.target].active)
                npc.TargetClosest();

            // Despawn safety, make sure to target another player if the current player target is too far away
            if (Vector2.Distance(Main.player[npc.target].Center, npc.Center) > CalamityGlobalNPC.CatchUpDistance200Tiles)
                npc.TargetClosest();

            bool dead = Main.player[npc.target].dead;
            float targetXDistance = npc.position.X + (npc.width / 2) - Main.player[npc.target].position.X - (Main.player[npc.target].width / 2);
            float targetYDistance = npc.position.Y + npc.height - 59f - Main.player[npc.target].position.Y - (Main.player[npc.target].height / 2);
            float eyeRotation = (float)Math.Atan2(targetYDistance, targetXDistance) + MathHelper.PiOver2;

            if (eyeRotation < 0f)
                eyeRotation += MathHelper.TwoPi;
            else if (eyeRotation > MathHelper.TwoPi)
                eyeRotation -= MathHelper.TwoPi;

            float eyeRotationAcceleration = 0f;
            if (npc.ai[0] == 0f && npc.ai[1] == 0f)
                eyeRotationAcceleration = 0.04f;
            if (npc.ai[0] == 0f && npc.ai[1] == 2f && npc.ai[2] > 40f)
                eyeRotationAcceleration = 0.1f;
            if (npc.ai[0] == 3f && npc.ai[1] == 0f)
                eyeRotationAcceleration = 0.1f;
            if (npc.ai[0] == 3f && npc.ai[1] == 2f && npc.ai[2] > 40f)
                eyeRotationAcceleration = 0.16f;
            if (npc.ai[0] == 3f && npc.ai[1] == 4f && npc.ai[2] > lineUpDist)
                eyeRotationAcceleration = 0.3f;
            if (npc.ai[0] == 3f && npc.ai[1] == 5f)
                eyeRotationAcceleration = 0.1f;

            if (npc.rotation < eyeRotation)
            {
                if ((eyeRotation - npc.rotation) > MathHelper.Pi)
                    npc.rotation -= eyeRotationAcceleration;
                else
                    npc.rotation += eyeRotationAcceleration;
            }
            else if (npc.rotation > eyeRotation)
            {
                if ((npc.rotation - eyeRotation) > MathHelper.Pi)
                    npc.rotation += eyeRotationAcceleration;
                else
                    npc.rotation -= eyeRotationAcceleration;
            }

            if (npc.rotation > eyeRotation - eyeRotationAcceleration && npc.rotation < eyeRotation + eyeRotationAcceleration)
                npc.rotation = eyeRotation;
            if (npc.rotation < 0f)
                npc.rotation += MathHelper.TwoPi;
            else if (npc.rotation > MathHelper.TwoPi)
                npc.rotation -= MathHelper.TwoPi;
            if (npc.rotation > eyeRotation - eyeRotationAcceleration && npc.rotation < eyeRotation + eyeRotationAcceleration)
                npc.rotation = eyeRotation;

            if (Main.rand.NextBool(5))
            {
                int randomBlood = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y + npc.height * 0.25f), npc.width, (int)(npc.height * 0.5f), 5, npc.velocity.X, 2f, 0, default, 1f);
                Dust dust = Main.dust[randomBlood];
                dust.velocity.X *= 0.5f;
                dust.velocity.Y *= 0.1f;
            }

            bool shootProjectile = Collision.CanHitLine(npc.Center, 1, 1, Main.player[npc.target].Center, 1, 1) &&
                npc.SafeDirectionTo(Main.player[npc.target].Center).AngleBetween((npc.rotation + MathHelper.PiOver2).ToRotationVector2()) < MathHelper.ToRadians(18f) &&
                Vector2.Distance(npc.Center, Main.player[npc.target].Center) > 240f;

            if (dead)
            {
                npc.velocity.Y -= 0.04f;

                if (npc.timeLeft > 10)
                    npc.timeLeft = 10;
            }

            else if (npc.ai[0] == 0f)
            {
                if (npc.ai[1] == 0f)
                {
                    // Avoid cheap bullshit
                    npc.damage = 0;

                    float hoverSpeed = 7f;
                    float hoverAcceleration = 0.15f;
                    hoverSpeed += 5f * enrageScale;
                    hoverAcceleration += 0.1f * enrageScale;

                    if (death)
                    {
                        hoverSpeed += 7f * (1f - lifeRatio);
                        hoverAcceleration += 0.15f * (1f - lifeRatio);
                    }

                    if (Main.getGoodWorld)
                    {
                        hoverSpeed += 3f;
                        hoverAcceleration += 0.08f;
                    }

                    Vector2 hoverDestination = Main.player[npc.target].Center - Vector2.UnitY * 400f;
                    Vector2 idealVelocity = npc.SafeDirectionTo(hoverDestination) * hoverSpeed;
                    npc.SimpleFlyMovement(idealVelocity, hoverAcceleration);

                    npc.ai[2] += 1f;
                    float attackSwitchTimer = 180f - (death ? 180f * (1f - lifeRatio) : 0f);
                    if (npc.ai[2] >= attackSwitchTimer)
                    {
                        npc.ai[1] = 1f;
                        npc.ai[2] = 0f;
                        npc.ai[3] = 0f;
                        npc.TargetClosest();
                        npc.netUpdate = true;
                    }
                    else if (npc.WithinRange(hoverDestination, 900f))
                    {
                        if (!Main.player[npc.target].dead)
                            npc.ai[3] += 1f;

                        float servantSpawnGateValue = death ? 20f : 40f;
                        if (Main.getGoodWorld)
                            servantSpawnGateValue *= 0.8f;

                        if (npc.ai[3] >= servantSpawnGateValue && shootProjectile)
                        {
                            npc.ai[3] = 0f;
                            npc.rotation = eyeRotation;

                            Vector2 servantSpawnVelocity = npc.SafeDirectionTo(Main.player[npc.target].Center) * servantAndProjectileVelocity;
                            Vector2 servantSpawnCenter = npc.Center + servantSpawnVelocity * 10f;
                            int maxServants = 4;
                            bool spawnServant = NPC.CountNPCS(NPCID.ServantofCthulhu) < maxServants;
                            if (spawnServant)
                                SoundEngine.PlaySound(SoundID.NPCHit1, servantSpawnCenter);
                            else
                                SoundEngine.PlaySound(SoundID.NPCHit18, servantSpawnCenter);

                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                if (spawnServant)
                                {
                                    int eye = NPC.NewNPC(npc.GetSource_FromAI(), (int)servantSpawnCenter.X, (int)servantSpawnCenter.Y, NPCID.ServantofCthulhu, 0, 0f, 0f, enrageScale);
                                    Main.npc[eye].velocity = servantSpawnVelocity;

                                    if (Main.netMode == NetmodeID.Server && eye < Main.maxNPCs)
                                        NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, eye);
                                }
                                else
                                {
                                    int projType = ProjectileID.BloodNautilusShot;
                                    int projDamage = npc.GetProjectileDamage(projType);
                                    Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center + Vector2.Normalize(servantSpawnVelocity) * 10f, servantSpawnVelocity * 2f, projType, projDamage, 0f, Main.myPlayer);
                                }
                            }

                            for (int m = 0; m < 10; m++)
                                Dust.NewDust(servantSpawnCenter, 20, 20, 5, servantSpawnVelocity.X * 0.4f, servantSpawnVelocity.Y * 0.4f, 0, default, 1f);
                        }
                    }
                }
                else if (npc.ai[1] == 1f)
                {
                    // Set damage
                    npc.damage = npc.defDamage;

                    npc.rotation = eyeRotation;
                    float additionalVelocityPerCharge = 2f;
                    float chargeSpeed = 6f + npc.ai[3] * additionalVelocityPerCharge;
                    chargeSpeed += 5f * enrageScale;
                    if (death)
                        chargeSpeed += 10f * (1f - lifeRatio);
                    if (Main.getGoodWorld)
                        chargeSpeed += 4f;

                    npc.velocity = npc.SafeDirectionTo(Main.player[npc.target].Center) * chargeSpeed;

                    npc.ai[1] = 2f;
                    npc.netUpdate = true;

                    if (npc.netSpam > 10)
                        npc.netSpam = 10;
                }
                else if (npc.ai[1] == 2f)
                {
                    // Set damage
                    npc.damage = npc.defDamage;

                    int chargeDelay = 90;
                    if (death)
                        chargeDelay -= (int)Math.Round(40f * (1f - lifeRatio));
                    if (Main.getGoodWorld)
                        chargeDelay -= 30;

                    float slowDownGateValue = chargeDelay * (death ? 0.5f : 0.44f);

                    npc.ai[2] += 1f;
                    if (npc.ai[2] >= slowDownGateValue)
                    {
                        // Avoid cheap bullshit
                        npc.damage = 0;

                        float decelerationScalar = death ? ((lifeRatio - 0.6f) / 0.4f) : 1f;
                        if (decelerationScalar < 0f)
                            decelerationScalar = 0f;

                        npc.velocity *= (MathHelper.Lerp(0.925f, 0.975f, decelerationScalar));
                        if (Main.getGoodWorld)
                            npc.velocity *= 0.99f;

                        if (npc.velocity.X > -0.1 && npc.velocity.X < 0.1)
                            npc.velocity.X = 0f;
                        if (npc.velocity.Y > -0.1 && npc.velocity.Y < 0.1)
                            npc.velocity.Y = 0f;
                    }
                    else
                        npc.rotation = npc.velocity.ToRotation() - MathHelper.PiOver2;

                    if (npc.ai[2] >= chargeDelay)
                    {
                        npc.ai[3] += 1f;
                        npc.ai[2] = 0f;
                        npc.TargetClosest();
                        npc.rotation = eyeRotation;

                        float numCharges = death ? 4f : 3f;
                        if (npc.ai[3] >= numCharges)
                        {
                            // Avoid cheap bullshit
                            npc.damage = 0;

                            npc.ai[1] = 0f;
                            npc.ai[3] = 0f;
                        }
                        else
                            npc.ai[1] = 1f;
                    }
                }

                if (phase2)
                {
                    // Avoid cheap bullshit
                    npc.damage = 0;

                    npc.ai[0] = 1f;
                    npc.ai[1] = 0f;
                    npc.ai[2] = 0f;
                    npc.ai[3] = 0f;
                    npc.TargetClosest();
                    npc.netUpdate = true;

                    if (npc.netSpam > 10)
                        npc.netSpam = 10;
                }
            }

            else if (npc.ai[0] == 1f || npc.ai[0] == 2f)
            {
                // Avoid cheap bullshit
                npc.damage = 0;

                if (Main.getGoodWorld)
                    npc.reflectsProjectiles = true;

                if (npc.ai[0] == 1f)
                {
                    npc.ai[2] += 0.005f;
                    if (npc.ai[2] > 0.5f)
                        npc.ai[2] = 0.5f;
                }
                else
                {
                    npc.ai[2] -= 0.005f;
                    if (npc.ai[2] < 0f)
                        npc.ai[2] = 0f;
                }

                npc.rotation += npc.ai[2];

                float phaseChangeRate = death ? 2f : 1f;
                float servantSpawnGateValue = Main.getGoodWorld ? 4f : 20f;
                npc.ai[1] += phaseChangeRate;
                if (npc.ai[1] % servantSpawnGateValue == 0f)
                {
                    Vector2 servantSpawnVelocity = Main.rand.NextVector2CircularEdge(5.65f, 5.65f);
                    if (Main.getGoodWorld)
                        servantSpawnVelocity *= 3f;

                    Vector2 servantSpawnCenter = npc.Center + servantSpawnVelocity * 10f;
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        int servantSpawn = NPC.NewNPC(npc.GetSource_FromAI(), (int)servantSpawnCenter.X, (int)servantSpawnCenter.Y, NPCID.ServantofCthulhu, 0, 0f, 0f, enrageScale);
                        Main.npc[servantSpawn].velocity.X = servantSpawnVelocity.X;
                        Main.npc[servantSpawn].velocity.Y = servantSpawnVelocity.Y;

                        if (Main.netMode == NetmodeID.Server && servantSpawn < Main.maxNPCs)
                            NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, servantSpawn);

                        if (CalamityWorld.LegendaryMode)
                        {
                            int type = ProjectileID.BloodNautilusShot;
                            Vector2 projectileVelocity = Main.rand.NextVector2CircularEdge(15f, 15f);
                            int numProj = 3;
                            int spread = 20;
                            float rotation = MathHelper.ToRadians(spread);
                            for (int i = 0; i < numProj; i++)
                            {
                                Vector2 perturbedSpeed = projectileVelocity.RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (float)(numProj - 1)));
                                Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center + Vector2.Normalize(perturbedSpeed) * 10f, perturbedSpeed, type, 15, 0f, Main.myPlayer);
                            }
                        }
                    }

                    for (int n = 0; n < 10; n++)
                        Dust.NewDust(servantSpawnCenter, 20, 20, 5, servantSpawnVelocity.X * 0.4f, servantSpawnVelocity.Y * 0.4f, 0, default, 1f);
                }

                if (npc.ai[1] == 100f)
                {
                    npc.ai[0] += 1f;
                    npc.ai[1] = 0f;

                    if (npc.ai[0] == 3f)
                    {
                        npc.ai[2] = 0f;
                    }
                    else
                    {
                        SoundEngine.PlaySound(SoundID.NPCHit1, npc.Center);

                        if (Main.netMode != NetmodeID.Server)
                        {
                            for (int phase2Gore = 0; phase2Gore < 2; phase2Gore++)
                            {
                                Gore.NewGore(npc.GetSource_FromAI(), npc.position, new Vector2(Main.rand.Next(-30, 31) * 0.2f, Main.rand.Next(-30, 31) * 0.2f), 8, 1f);
                                Gore.NewGore(npc.GetSource_FromAI(), npc.position, new Vector2(Main.rand.Next(-30, 31) * 0.2f, Main.rand.Next(-30, 31) * 0.2f), 7, 1f);
                                Gore.NewGore(npc.GetSource_FromAI(), npc.position, new Vector2(Main.rand.Next(-30, 31) * 0.2f, Main.rand.Next(-30, 31) * 0.2f), 6, 1f);
                            }
                        }

                        for (int i = 0; i < 20; i++)
                            Dust.NewDust(npc.position, npc.width, npc.height, 5, Main.rand.Next(-30, 31) * 0.2f, Main.rand.Next(-30, 31) * 0.2f, 0, default, 1f);

                        SoundEngine.PlaySound(SoundID.Roar, npc.Center);
                    }
                }

                Dust.NewDust(npc.position, npc.width, npc.height, 5, Main.rand.Next(-30, 31) * 0.2f, Main.rand.Next(-30, 31) * 0.2f, 0, default, 1f);
                npc.velocity *= 0.98f;

                if (npc.velocity.X > -0.1 && npc.velocity.X < 0.1)
                    npc.velocity.X = 0f;
                if (npc.velocity.Y > -0.1 && npc.velocity.Y < 0.1)
                    npc.velocity.Y = 0f;
            }

            else
            {
                npc.defense = 0;
                int setDamage = (int)(npc.defDamage * (phase3 ? 1.4f : 1.2f));

                if (npc.ai[1] == 0f & phase3)
                    npc.ai[1] = 5f;

                if (npc.ai[1] == 0f)
                {
                    // Avoid cheap bullshit
                    npc.damage = 0;

                    float hoverSpeed = 5.5f + 3f * (0.6f - lifeRatio);
                    float hoverAcceleration = 0.06f + 0.02f * (0.6f - lifeRatio);
                    hoverSpeed += 4f * enrageScale;
                    hoverAcceleration += 0.04f * enrageScale;

                    if (death)
                    {
                        hoverSpeed += 5.5f * (0.6f - lifeRatio);
                        hoverAcceleration += 0.06f * (0.6f - lifeRatio);
                    }

                    Vector2 hoverDestination = Main.player[npc.target].Center - Vector2.UnitY * 400f;
                    float distanceFromHoverDestination = npc.Distance(hoverDestination);

                    if (distanceFromHoverDestination > 400f)
                    {
                        hoverSpeed += 1.25f;
                        hoverAcceleration += 0.075f;
                        if (distanceFromHoverDestination > 600f)
                        {
                            hoverSpeed += 1.25f;
                            hoverAcceleration += 0.075f;
                            if (distanceFromHoverDestination > 800f)
                            {
                                hoverSpeed += 1.25f;
                                hoverAcceleration += 0.075f;
                            }
                        }
                    }

                    if (Main.getGoodWorld)
                    {
                        hoverSpeed += 1f;
                        hoverAcceleration += 0.1f;
                    }

                    Vector2 idealHoverVelocity = npc.SafeDirectionTo(hoverDestination) * hoverSpeed;
                    npc.SimpleFlyMovement(idealHoverVelocity, hoverAcceleration);

                    npc.ai[2] += 1f;
                    float phaseLimit = 200f - (death ? 150f * (0.6f - lifeRatio) : 0f);
                    float projectileGateValue = (lifeRatio < 0.5f && death) ? 50f : 80f;
                    if (npc.ai[2] % projectileGateValue == 0f && shootProjectile)
                    {
                        Vector2 projectileVelocity = Vector2.Normalize(Main.player[npc.target].Center - npc.Center) * servantAndProjectileVelocity * 2f;
                        Vector2 projectileSpawnCenter = npc.Center + projectileVelocity;
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            int type = ProjectileID.BloodNautilusShot;
                            int numProj = 3;
                            int spread = 10;
                            float rotation = MathHelper.ToRadians(spread);
                            for (int i = 0; i < numProj; i++)
                            {
                                Vector2 perturbedSpeed = projectileVelocity.RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (float)(numProj - 1)));
                                Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center + Vector2.Normalize(perturbedSpeed) * 10f, perturbedSpeed, type, 15, 0f, Main.myPlayer);
                            }
                        }

                        SoundEngine.PlaySound(SoundID.NPCHit18, projectileSpawnCenter);

                        for (int m = 0; m < 10; m++)
                            Dust.NewDust(projectileSpawnCenter, 20, 20, 5, projectileVelocity.X * 0.4f, projectileVelocity.Y * 0.4f, 0, default, 1f);
                    }

                    if (npc.ai[2] >= phaseLimit)
                    {
                        npc.ai[1] = 1f;
                        npc.ai[2] = 0f;
                        npc.ai[3] = 0f;
                        npc.TargetClosest();
                        npc.netUpdate = true;
                    }
                }

                else if (npc.ai[1] == 1f)
                {
                    // Set damage
                    npc.damage = setDamage;

                    SoundEngine.PlaySound(SoundID.ForceRoar, npc.Center);
                    npc.rotation = eyeRotation;

                    float additionalVelocityPerCharge = 3f;
                    float chargeSpeed = 9f + (3.5f * (0.6f - lifeRatio)) + npc.ai[3] * additionalVelocityPerCharge;
                    chargeSpeed += 4f * enrageScale;
                    if (death)
                        chargeSpeed += 6.5f * (0.6f - lifeRatio);
                    if (npc.ai[3] == 1f)
                        chargeSpeed *= 1.15f;
                    if (npc.ai[3] == 2f)
                        chargeSpeed *= 1.3f;
                    if (Main.getGoodWorld)
                        chargeSpeed *= 1.2f;

                    npc.velocity = npc.SafeDirectionTo(Main.player[npc.target].Center) * chargeSpeed;
                    npc.ai[1] = 2f;
                    npc.netUpdate = true;

                    if (npc.netSpam > 10)
                        npc.netSpam = 10;
                }

                else if (npc.ai[1] == 2f)
                {
                    // Set damage
                    npc.damage = setDamage;

                    int phase2ChargeDelay = 80;
                    if (death)
                        phase2ChargeDelay -= (int)Math.Round(35f * (0.6f - lifeRatio));

                    float slowDownGateValue = phase2ChargeDelay * 0.75f;

                    npc.ai[2] += 1f;
                    if (npc.ai[2] >= slowDownGateValue)
                    {
                        // Avoid cheap bullshit
                        npc.damage = 0;

                        float decelerationScalar = death ? ((lifeRatio - 0.3f) / 0.3f) : 1f;
                        if (decelerationScalar < 0f)
                            decelerationScalar = 0f;

                        npc.velocity *= (MathHelper.Lerp(0.9f, 0.96f, decelerationScalar));
                        if (npc.velocity.X > -0.1 && npc.velocity.X < 0.1)
                            npc.velocity.X = 0f;
                        if (npc.velocity.Y > -0.1 && npc.velocity.Y < 0.1)
                            npc.velocity.Y = 0f;
                    }
                    else
                        npc.rotation = npc.velocity.ToRotation() - MathHelper.PiOver2;

                    if (npc.ai[2] >= phase2ChargeDelay)
                    {
                        npc.ai[3] += 1f;
                        npc.ai[2] = 0f;
                        npc.TargetClosest();
                        npc.rotation = eyeRotation;

                        float numCharges = death ? 4f : 3f;
                        if (npc.ai[3] >= numCharges)
                        {
                            // Avoid cheap bullshit
                            npc.damage = 0;

                            npc.ai[1] = 0f;
                            npc.ai[3] = 0f;
                            npc.netUpdate = true;

                            if (npc.netSpam > 10)
                                npc.netSpam = 10;
                        }
                        else
                            npc.ai[1] = 1f;
                    }
                }

                else if (npc.ai[1] == 3f)
                {
                    if ((npc.ai[3] == 4f & phase3) && npc.Center.Y > Main.player[npc.target].Center.Y)
                    {
                        // Avoid cheap bullshit
                        npc.damage = 0;

                        npc.TargetClosest();
                        npc.ai[1] = 0f;
                        npc.ai[2] = 0f;
                        npc.ai[3] = 0f;
                        npc.netUpdate = true;

                        if (npc.netSpam > 10)
                            npc.netSpam = 10;
                    }
                    else if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        // Set damage
                        npc.damage = setDamage;

                        float speedBoost = death ? 10f * (0.3f - lifeRatio) : 7f * (0.3f - lifeRatio);
                        float finalChargeSpeed = 18f + speedBoost;
                        finalChargeSpeed += 10f * enrageScale;

                        Vector2 eyeChargeDirection = npc.Center;
                        float targetX = Main.player[npc.target].position.X + (Main.player[npc.target].width / 2) - eyeChargeDirection.X;
                        float targetY = Main.player[npc.target].position.Y + (Main.player[npc.target].height / 2) - eyeChargeDirection.Y;
                        float targetVelocity = Math.Abs(Main.player[npc.target].velocity.X) + Math.Abs(Main.player[npc.target].velocity.Y) / 4f;
                        targetVelocity += 10f - targetVelocity;

                        if (targetVelocity < 5f)
                            targetVelocity = 5f;
                        if (targetVelocity > 15f)
                            targetVelocity = 15f;

                        if (npc.ai[2] == -1f)
                        {
                            targetVelocity *= 4f;
                            finalChargeSpeed *= 1.3f;
                        }

                        targetX -= Main.player[npc.target].velocity.X * targetVelocity;
                        targetY -= Main.player[npc.target].velocity.Y * targetVelocity / 4f;

                        float targetDistance = (float)Math.Sqrt(targetX * targetX + targetY * targetY);
                        float targetDistCopy = targetDistance;

                        targetDistance = finalChargeSpeed / targetDistance;
                        npc.velocity.X = targetX * targetDistance;
                        npc.velocity.Y = targetY * targetDistance;

                        if (targetDistCopy < 100f)
                        {
                            if (Math.Abs(npc.velocity.X) > Math.Abs(npc.velocity.Y))
                            {
                                float absoluteXVel = Math.Abs(npc.velocity.X);
                                float absoluteYVel = Math.Abs(npc.velocity.Y);

                                if (npc.Center.X > Main.player[npc.target].Center.X)
                                    absoluteYVel *= -1f;
                                if (npc.Center.Y > Main.player[npc.target].Center.Y)
                                    absoluteXVel *= -1f;

                                npc.velocity.X = absoluteYVel;
                                npc.velocity.Y = absoluteXVel;
                            }
                        }
                        else if (Math.Abs(npc.velocity.X) > Math.Abs(npc.velocity.Y))
                        {
                            float absoluteEyeVel = (Math.Abs(npc.velocity.X) + Math.Abs(npc.velocity.Y)) / 2f;
                            float absoluteEyeVelBackup = absoluteEyeVel;

                            if (npc.Center.X > Main.player[npc.target].Center.X)
                                absoluteEyeVelBackup *= -1f;
                            if (npc.Center.Y > Main.player[npc.target].Center.Y)
                                absoluteEyeVel *= -1f;

                            npc.velocity.X = absoluteEyeVelBackup;
                            npc.velocity.Y = absoluteEyeVel;
                        }

                        npc.ai[1] = 4f;
                        npc.netUpdate = true;

                        if (npc.netSpam > 10)
                            npc.netSpam = 10;
                    }
                }

                else if (npc.ai[1] == 4f)
                {
                    // Set damage
                    npc.damage = setDamage;

                    if (npc.ai[2] == 0f)
                        SoundEngine.PlaySound(SoundID.ForceRoar, npc.Center);

                    float lineUpDistControl = lineUpDist;
                    npc.ai[2] += 1f;

                    if (npc.ai[2] == lineUpDistControl && Vector2.Distance(npc.position, Main.player[npc.target].position) < 200f)
                        npc.ai[2] -= 1f;

                    if (npc.ai[2] >= lineUpDistControl)
                    {
                        // Avoid cheap bullshit
                        npc.damage = 0;

                        npc.velocity *= 0.95f;
                        if (npc.velocity.X > -0.1 && npc.velocity.X < 0.1)
                            npc.velocity.X = 0f;
                        if (npc.velocity.Y > -0.1 && npc.velocity.Y < 0.1)
                            npc.velocity.Y = 0f;
                    }
                    else
                        npc.rotation = npc.velocity.ToRotation() - MathHelper.PiOver2;

                    float lineUpDistNetUpdate = lineUpDistControl + 13f;
                    if (npc.ai[2] >= lineUpDistNetUpdate)
                    {
                        npc.netUpdate = true;

                        if (npc.netSpam > 10)
                            npc.netSpam = 10;

                        npc.ai[3] += 1f;
                        npc.ai[2] = 0f;

                        float maxCharges = death ? (finalPhaseDeath ? 0f : penultimatePhaseDeath ? 1f : 2f) : finalPhaseRev ? 2f : 3f;
                        if (npc.ai[3] >= maxCharges)
                        {
                            // Avoid cheap bullshit
                            npc.damage = 0;

                            npc.ai[1] = 0f;
                            npc.ai[3] = 0f;
                        }
                        else
                            npc.ai[1] = 3f;
                    }
                }

                else if (npc.ai[1] == 5f)
                {
                    // Avoid cheap bullshit
                    npc.damage = 0;

                    float offset = death ? 540f : 600f;
                    float speedBoost = death ? 15f * (0.3f - lifeRatio) : 5f * (0.3f - lifeRatio);
                    float accelerationBoost = death ? 0.425f * (0.3f - lifeRatio) : 0.125f * (0.3f - lifeRatio);
                    float hoverSpeed = 8f + speedBoost;
                    float hoverAcceleration = 0.25f + accelerationBoost;

                    Vector2 eyeLineUpChargeDirection = npc.Center;
                    float lineUpChargeTargetX = Main.player[npc.target].position.X + (Main.player[npc.target].width / 2) - eyeLineUpChargeDirection.X;
                    float lineUpChargeTargetY = Main.player[npc.target].position.Y + (Main.player[npc.target].height / 2) + offset - eyeLineUpChargeDirection.Y;
                    Vector2 hoverDestination = Main.player[npc.target].Center + Vector2.UnitY * offset;

                    bool horizontalCharge = calamityGlobalNPC.newAI[0] == 1f || calamityGlobalNPC.newAI[0] == 3f;
                    if (horizontalCharge)
                    {
                        float horizontalChargeOffset = death ? 450f : 500f;
                        offset = calamityGlobalNPC.newAI[0] == 1f ? -horizontalChargeOffset : horizontalChargeOffset;
                        hoverSpeed *= 1.5f;
                        hoverAcceleration *= 1.5f;
                        hoverDestination = Main.player[npc.target].Center + Vector2.UnitX * offset;
                    }

                    Vector2 idealHoverVelocity = npc.SafeDirectionTo(hoverDestination) * hoverSpeed;
                    npc.SimpleFlyMovement(idealHoverVelocity, hoverAcceleration);

                    float timeGateValue = horizontalCharge ? (100f - (death ? 80f * (0.3f - lifeRatio) : 0f)) : (85f - (death ? 70f * (0.3f - lifeRatio) : 0f));
                    float servantSpawnGateValue = horizontalCharge ? (death ? 23f : 35f) : (death ? 17f : 27f);
                    float maxServantSpawnsPerAttack = 2f;

                    npc.ai[2] += 1f;
                    if (npc.ai[2] % servantSpawnGateValue == 0f && shootProjectile && npc.ai[2] <= servantSpawnGateValue * maxServantSpawnsPerAttack)
                    {
                        Vector2 servantSpawnVelocity = Vector2.Normalize(Main.player[npc.target].Center - npc.Center) * servantAndProjectileVelocity;
                        Vector2 servantSpawnCenter = npc.Center + servantSpawnVelocity * 10f;

                        int maxServants = death ? (finalPhaseDeath ? 1 : penultimatePhaseDeath ? 2 : 3) : (finalPhaseRev ? 2 : 4);
                        bool spawnServant = NPC.CountNPCS(NPCID.ServantofCthulhu) < maxServants;
                        if (spawnServant)
                            SoundEngine.PlaySound(SoundID.NPCDeath13, servantSpawnCenter);
                        else
                            SoundEngine.PlaySound(SoundID.NPCHit18, servantSpawnCenter);

                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            if (spawnServant)
                            {
                                int eye = NPC.NewNPC(npc.GetSource_FromAI(), (int)servantSpawnCenter.X, (int)servantSpawnCenter.Y, NPCID.ServantofCthulhu, 0, 0f, 0f, enrageScale);
                                Main.npc[eye].velocity.X = servantSpawnVelocity.X;
                                Main.npc[eye].velocity.Y = servantSpawnVelocity.Y;

                                if (Main.netMode == NetmodeID.Server && eye < Main.maxNPCs)
                                    NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, eye);
                            }
                            else if (!CalamityWorld.LegendaryMode)
                            {
                                int projType = ProjectileID.BloodNautilusShot;
                                int projDamage = npc.GetProjectileDamage(projType);
                                Projectile.NewProjectile(npc.GetSource_FromAI(), servantSpawnCenter, servantSpawnVelocity * 2f, projType, projDamage, 0f, Main.myPlayer);
                            }

                            if (CalamityWorld.LegendaryMode)
                            {
                                int type = ProjectileID.BloodNautilusShot;
                                Vector2 projectileVelocity = servantSpawnVelocity * 3f;
                                int numProj = 3;
                                int spread = 20;
                                float rotation = MathHelper.ToRadians(spread);
                                for (int i = 0; i < numProj; i++)
                                {
                                    Vector2 perturbedSpeed = projectileVelocity.RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (float)(numProj - 1)));
                                    Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center + Vector2.Normalize(perturbedSpeed) * 10f, perturbedSpeed, type, 15, 0f, Main.myPlayer);
                                }
                            }
                        }

                        for (int m = 0; m < 10; m++)
                            Dust.NewDust(servantSpawnCenter, 20, 20, 5, servantSpawnVelocity.X * 0.4f, servantSpawnVelocity.Y * 0.4f, 0, default, 1f);
                    }

                    if (npc.ai[2] >= timeGateValue)
                    {
                        switch ((int)calamityGlobalNPC.newAI[0])
                        {
                            case 0: // Normal Eye behavior
                                npc.ai[1] = 3f;
                                npc.ai[2] = -1f;
                                npc.ai[3] = -1f;
                                break;

                            case 1: // Charge from the left
                                npc.ai[1] = 6f;
                                npc.ai[2] = 0f;
                                break;

                            case 2: // Normal Eye behavior
                                npc.ai[1] = 3f;
                                npc.ai[2] = -1f;
                                break;

                            case 3: // Charge from the right
                                npc.ai[1] = 6f;
                                npc.ai[2] = 0f;
                                break;

                            default:
                                break;
                        }

                        npc.TargetClosest();
                        calamityGlobalNPC.newAI[0] += 1f;
                        if (calamityGlobalNPC.newAI[0] > 3f)
                            calamityGlobalNPC.newAI[0] = 0f;

                        npc.SyncExtraAI();
                    }

                    npc.netUpdate = true;

                    if (npc.netSpam > 10)
                        npc.netSpam = 10;
                }

                else if (npc.ai[1] == 6f)
                {
                    // Set damage
                    npc.damage = setDamage;

                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        float speedBoost = death ? 15f * (0.3f - lifeRatio) : 5f * (0.3f - lifeRatio);
                        float chargeSpeed = 18f + speedBoost;
                        chargeSpeed += 10f * enrageScale;
                        npc.velocity = npc.SafeDirectionTo(Main.player[npc.target].Center) * chargeSpeed;

                        npc.ai[1] = 7f;
                        npc.netUpdate = true;

                        if (npc.netSpam > 10)
                            npc.netSpam = 10;
                    }
                }

                else if (npc.ai[1] == 7f)
                {
                    // Set damage
                    npc.damage = setDamage;

                    if (npc.ai[2] == 0f)
                        SoundEngine.PlaySound(SoundID.Roar, npc.Center);

                    float lineUpDistControl = (float)Math.Round(lineUpDist * 2.5f);
                    npc.ai[2] += 1f;

                    if (npc.ai[2] == lineUpDistControl && Vector2.Distance(npc.position, Main.player[npc.target].position) < 200f)
                        npc.ai[2] -= 1f;

                    if (npc.ai[2] >= lineUpDistControl)
                    {
                        // Avoid cheap bullshit
                        npc.damage = 0;

                        npc.velocity *= 0.95f;
                        if (npc.velocity.X > -0.1 && npc.velocity.X < 0.1)
                            npc.velocity.X = 0f;
                        if (npc.velocity.Y > -0.1 && npc.velocity.Y < 0.1)
                            npc.velocity.Y = 0f;
                    }
                    else
                        npc.rotation = npc.velocity.ToRotation() - MathHelper.PiOver2;

                    float lineUpDistNetUpdate = lineUpDistControl + 13f;
                    if (npc.ai[2] >= lineUpDistNetUpdate)
                    {
                        // Avoid cheap bullshit
                        npc.damage = 0;

                        npc.netUpdate = true;

                        if (npc.netSpam > 10)
                            npc.netSpam = 10;

                        npc.ai[2] = 0f;
                        npc.ai[1] = 0f;
                    }
                }
            }

            return false;
        }
    }
}
