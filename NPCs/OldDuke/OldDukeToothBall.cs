﻿using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Projectiles.Boss;
using CalamityMod.World;
using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Dusts;
using CalamityMod.Events;

namespace CalamityMod.NPCs.OldDuke
{
    public class OldDukeToothBall : ModNPC
    {
        public override void SetStaticDefaults()
        {
            this.HideFromBestiary();
        }

        public override void SetDefaults()
        {
            NPC.Calamity().canBreakPlayerDefense = true;
            NPC.aiStyle = -1;
            AIType = -1;
            NPC.GetNPCDamage();
            NPC.width = 40;
            NPC.height = 40;
            NPC.defense = 0;
            NPC.lifeMax = 8000;
            if (BossRushEvent.BossRushActive)
            {
                NPC.lifeMax = 16000;
            }
            double HPBoost = CalamityConfig.Instance.BossHealthBoost * 0.01;
            NPC.lifeMax += (int)(NPC.lifeMax * HPBoost);
            NPC.knockBackResist = 0f;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath11;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.Calamity().VulnerableToHeat = false;
            NPC.Calamity().VulnerableToSickness = false;
            NPC.Calamity().VulnerableToElectricity = true;
            NPC.Calamity().VulnerableToWater = false;
        }

        public override void AI()
        {
            Lighting.AddLight((int)((NPC.position.X + (NPC.width / 2)) / 16f), (int)((NPC.position.Y + (NPC.height / 2)) / 16f), 0.65f, 0.55f, 0f);

            bool bossRush = BossRushEvent.BossRushActive;
            bool expertMode = Main.expertMode || bossRush;
            bool revenge = CalamityWorld.revenge || bossRush;
            bool death = CalamityWorld.death || bossRush;

            NPC.rotation += NPC.velocity.X * 0.05f;

            NPC.TargetClosest(false);
            Player player = Main.player[NPC.target];
            if (!player.active || player.dead)
            {
                NPC.TargetClosest(false);
                player = Main.player[NPC.target];
                if (!player.active || player.dead)
                {
                    if (NPC.timeLeft > 10)
                        NPC.timeLeft = 10;

                    return;
                }
            }
            else if (NPC.timeLeft < 600)
                NPC.timeLeft = 600;

            Vector2 vector = player.Center - NPC.Center;
            float cannonballMovementGateValue = 120f;
            float slowDownGateValue = cannonballMovementGateValue + 300f;
            float dieGateValue = slowDownGateValue + 60f;
            NPC.ai[3] += 1f;
            if (vector.Length() < 40f || NPC.ai[3] >= dieGateValue)
            {
                NPC.life = 0;
                NPC.HitEffect();
                NPC.checkDead();
                return;
            }

            if (NPC.ai[3] < cannonballMovementGateValue)
            {
                Vector2 finalCannonballVelocity = new Vector2(NPC.ai[0], NPC.ai[1]);
                if (NPC.velocity.Length() < finalCannonballVelocity.Length())
                {
                    NPC.velocity *= 1.01f;
                    if (NPC.velocity.Length() > finalCannonballVelocity.Length())
                    {
                        NPC.velocity.Normalize();
                        NPC.velocity *= finalCannonballVelocity.Length();
                    }
                }

                return;
            }

            if (NPC.ai[3] > slowDownGateValue)
            {
                NPC.velocity *= 0.95f;
                return;
            }

            float velocity = death ? 14f : revenge ? 13f : 12f;
            if (expertMode)
            {
                float speedUpMult = bossRush ? 0.01f : 0.005f;
                velocity += Vector2.Distance(player.Center, NPC.Center) * speedUpMult;
            }

            Vector2 vector167 = new Vector2(NPC.Center.X + NPC.direction * 20, NPC.Center.Y + 6f);
            float num1373 = player.position.X + player.width * 0.5f - vector167.X;
            float num1374 = player.Center.Y - vector167.Y;
            float num1375 = (float)Math.Sqrt(num1373 * num1373 + num1374 * num1374);
            float num1376 = velocity / num1375;
            num1373 *= num1376;
            num1374 *= num1376;

            NPC.ai[2] -= Main.rand.Next(6);
            if (num1375 < 300f || NPC.ai[2] > 0f)
            {
                if (num1375 < 300f)
                    NPC.ai[2] = 100f;

                if (NPC.velocity.X < 0f)
                    NPC.direction = -1;
                else
                    NPC.direction = 1;

                return;
            }

            float inertia = 50f;
            NPC.velocity.X = (NPC.velocity.X * inertia + num1373) / (inertia + 1f);
            NPC.velocity.Y = (NPC.velocity.Y * inertia + num1374) / (inertia + 1f);

            float num1247 = bossRush ? 0.65f : 0.5f;
            for (int num1248 = 0; num1248 < Main.maxNPCs; num1248++)
            {
                if (Main.npc[num1248].active)
                {
                    if (num1248 != NPC.whoAmI && Main.npc[num1248].type == NPC.type)
                    {
                        if (Vector2.Distance(NPC.Center, Main.npc[num1248].Center) < 48f)
                        {
                            if (NPC.position.X < Main.npc[num1248].position.X)
                                NPC.velocity.X -= num1247;
                            else
                                NPC.velocity.X += num1247;

                            if (NPC.position.Y < Main.npc[num1248].position.Y)
                                NPC.velocity.Y -= num1247;
                            else
                                NPC.velocity.Y += num1247;
                        }
                    }
                }
            }
        }

        public override void OnKill()
        {
            int closestPlayer = Player.FindClosest(NPC.Center, 1, 1);
            if (Main.rand.NextBool(8) && Main.player[closestPlayer].statLife < Main.player[closestPlayer].statLifeMax2)
                Item.NewItem(NPC.GetSource_Loot(), (int)NPC.position.X, (int)NPC.position.Y, NPC.width, NPC.height, ItemID.Heart);

            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                int totalProjectiles = CalamityWorld.death ? 5 : CalamityWorld.revenge ? 4 : 3;
                float radians = MathHelper.TwoPi / totalProjectiles;
                int type = ModContent.ProjectileType<OldDukeToothBallSpike>();
                int damage = NPC.GetProjectileDamage(type);
                float velocity = 10f;
                double angleA = radians * 0.5;
                double angleB = MathHelper.ToRadians(90f) - angleA;
                float velocityX = (float)(velocity * Math.Sin(angleA) / Math.Sin(angleB));
                Vector2 spinningPoint = Main.rand.NextBool() ? new Vector2(0f, -velocity) : new Vector2(-velocityX, -velocity);
                for (int k = 0; k < totalProjectiles; k++)
                {
                    Vector2 vector255 = spinningPoint.RotatedBy(radians * k);
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, vector255 * 0.1f, type, damage, 0f, Main.myPlayer, vector255.X, vector255.Y);
                }

                if (Main.expertMode)
                {
                    type = ModContent.ProjectileType<SandPoisonCloudOldDuke>();
                    damage = NPC.GetProjectileDamage(type);
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero, type, damage, 0f, Main.myPlayer);
                }
            }

            if (Main.zenithWorld)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    int spawnX = NPC.width / 2;
                    int type = ModContent.ProjectileType<OldDukeGore>();
                    for (int i = 0; i < 2; i++)
                        Projectile.NewProjectile(NPC.GetSource_Death(), NPC.Center.X + Main.rand.Next(-spawnX, spawnX), NPC.Center.Y,
                            Main.rand.Next(-1, 2), Main.rand.Next(-6, -3), type, NPC.damage / 2, 0f, Main.myPlayer);
                }
            }
        }

        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)/* tModPorter Note: bossLifeScale -> balance (bossAdjustment is different, see the docs for details) */
        {
            NPC.damage = (int)(NPC.damage * NPC.GetExpertDamageMultiplier());
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            cooldownSlot = ImmunityCooldownID.Bosses;
            return true;
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {
            if (hurtInfo.Damage > 0)
                target.AddBuff(ModContent.BuffType<Irradiated>(), 240);
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            for (int k = 0; k < 3; k++)
                Dust.NewDust(NPC.position, NPC.width, NPC.height, (int)CalamityDusts.SulfurousSeaAcid, hit.HitDirection, -1f, 0, default, 1f);

            if (NPC.life <= 0)
            {
                for (int k = 0; k < 15; k++)
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, (int)CalamityDusts.SulfurousSeaAcid, hit.HitDirection, -1f, 0, default, 1f);

                NPC.position.X = NPC.position.X + NPC.width / 2;
                NPC.position.Y = NPC.position.Y + NPC.height / 2;
                NPC.width = NPC.height = 96;
                NPC.position.X = NPC.position.X - NPC.width / 2;
                NPC.position.Y = NPC.position.Y - NPC.height / 2;

                for (int num621 = 0; num621 < 15; num621++)
                {
                    int num622 = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, DustID.Blood, 0f, 0f, 100, default, 2f);
                    Main.dust[num622].velocity *= 3f;
                    if (Main.rand.NextBool())
                    {
                        Main.dust[num622].scale = 0.5f;
                        Main.dust[num622].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                    }
                    Main.dust[num622].noGravity = true;
                }

                for (int num623 = 0; num623 < 30; num623++)
                {
                    int num624 = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, (int)CalamityDusts.SulfurousSeaAcid, 0f, 0f, 100, default, 3f);
                    Main.dust[num624].noGravity = true;
                    Main.dust[num624].velocity *= 5f;
                    num624 = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, (int)CalamityDusts.SulfurousSeaAcid, 0f, 0f, 100, default, 2f);
                    Main.dust[num624].velocity *= 2f;
                    Main.dust[num624].noGravity = true;
                }

                if (Main.netMode != NetmodeID.Server)
                {
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("OldDukeToothBallGore").Type, NPC.scale);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("OldDukeToothBallGore2").Type, NPC.scale);
                }
            }
        }
    }
}
