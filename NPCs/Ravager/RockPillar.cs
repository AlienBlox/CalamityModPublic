﻿using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Events;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.NPCs.Ravager
{
    public class RockPillar : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Rock Pillar");
        }

        public override void SetDefaults()
        {
            NPC.Calamity().canBreakPlayerDefense = true;
            NPC.GetNPCDamage();
            NPC.width = 60;
            NPC.height = 300;
            NPC.defense = 50;
            NPC.DR_NERD(0.3f);
            NPC.chaseable = false;
            NPC.canGhostHeal = false;
            NPC.lifeMax = DownedBossSystem.downedProvidence ? 20000 : 5000;
            NPC.alpha = 255;
            NPC.aiStyle = -1;
            AIType = -1;
            NPC.knockBackResist = 0f;
            NPC.HitSound = SoundID.NPCHit41;
            NPC.DeathSound = SoundID.NPCDeath14;
            NPC.Calamity().VulnerableToSickness = false;
            NPC.Calamity().VulnerableToWater = true;
        }

        public override void AI()
        {
            if (CalamityGlobalNPC.scavenger < 0 || !Main.npc[CalamityGlobalNPC.scavenger].active)
            {
                NPC.life = 0;
                HitEffect(NPC.direction, 9999);
                NPC.netUpdate = true;
                return;
            }

            if (NPC.timeLeft < 1800)
                NPC.timeLeft = 1800;

            if (NPC.alpha > 0)
            {
                NPC.damage = 0;

                NPC.alpha -= 10;
                if (NPC.alpha < 0)
                    NPC.alpha = 0;
            }
            else
            {
                if (DownedBossSystem.downedProvidence && !BossRushEvent.BossRushActive)
                    NPC.damage = (int)(NPC.defDamage * 1.5);
                else
                    NPC.damage = NPC.defDamage;
            }

            if (NPC.ai[0] == 0f)
            {
                if (NPC.velocity.Y == 0f)
                {
                    if (NPC.ai[1] == -1f)
                    {
                        SoundEngine.PlaySound(SoundID.Item62, NPC.position);

                        for (int num621 = 0; num621 < 10; num621++)
                        {
                            int num622 = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, DustID.Iron, 0f, 0f, 100, default, 2f);
                            Main.dust[num622].velocity *= 3f;
                            if (Main.rand.NextBool(2))
                            {
                                Main.dust[num622].scale = 0.5f;
                                Main.dust[num622].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                            }
                        }
                        for (int num623 = 0; num623 < 10; num623++)
                        {
                            int num624 = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, DustID.Stone, 0f, 0f, 100, default, 3f);
                            Main.dust[num624].noGravity = true;
                            Main.dust[num624].velocity *= 5f;
                            num624 = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, DustID.Iron, 0f, 0f, 100, default, 2f);
                            Main.dust[num624].velocity *= 2f;
                        }

                        NPC.noTileCollide = true;
                        NPC.velocity.X = ((CalamityWorld.malice || BossRushEvent.BossRushActive) ? 15 : 12) * NPC.direction;
                        NPC.velocity.Y = -28.5f;
                        NPC.ai[0] = 1f;
                        NPC.ai[1] = 0f;
                    }
                }
            }
            else
            {
                if (NPC.velocity.Y == 0f || Vector2.Distance(NPC.Center, Main.npc[CalamityGlobalNPC.scavenger].Center) > 2800f)
                {
                    SoundEngine.PlaySound(SoundID.Item14, NPC.position);
                    NPC.ai[0] = 0f;
                    NPC.life = 0;
                    HitEffect(NPC.direction, 9999);
                    NPC.netUpdate = true;
                    return;
                }
                else
                {
                    NPC.velocity.Y += 0.2f;

                    if (NPC.velocity.Y >= 0f && !Collision.SolidCollision(NPC.position, NPC.width, NPC.height))
                        NPC.noTileCollide = false;
                }
            }
        }

        public override bool CheckActive() => false;

        public override void OnHitPlayer(Player player, int damage, bool crit)
        {
            player.AddBuff(ModContent.BuffType<ArmorCrunch>(), 180, true);
            SoundEngine.PlaySound(SoundID.Item14, NPC.position);
            NPC.ai[0] = 0f;
            NPC.life = 0;
            HitEffect(NPC.direction, 9999);
            NPC.netUpdate = true;
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            if (NPC.life <= 0)
            {
                NPC.position.X = NPC.position.X + (NPC.width / 2);
                NPC.position.Y = NPC.position.Y + (NPC.height / 2);
                NPC.width = 80;
                NPC.height = 360;
                NPC.position.X = NPC.position.X - (NPC.width / 2);
                NPC.position.Y = NPC.position.Y - (NPC.height / 2);
                for (int num621 = 0; num621 < 30; num621++)
                {
                    int num622 = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, DustID.Iron, 0f, 0f, 100, default, 2f);
                    Main.dust[num622].velocity *= 3f;
                    if (Main.rand.NextBool(2))
                    {
                        Main.dust[num622].scale = 0.5f;
                        Main.dust[num622].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                    }
                }
                for (int num623 = 0; num623 < 30; num623++)
                {
                    int num624 = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, DustID.Stone, 0f, 0f, 100, default, 3f);
                    Main.dust[num624].noGravity = true;
                    Main.dust[num624].velocity *= 5f;
                    num624 = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, DustID.Iron, 0f, 0f, 100, default, 2f);
                    Main.dust[num624].velocity *= 2f;
                }

                if (Main.netMode != NetmodeID.Server)
                {
                    float y = NPC.height / 6f;
                    float randomVelocityScale = 0.25f;
                    for (int i = 0; i < 2; i++)
                    {
                        Vector2 randomVelocity = NPC.velocity * Main.rand.NextFloat() * randomVelocityScale;
                        Gore.NewGore(NPC.position, NPC.velocity + randomVelocity, Mod.Find<ModGore>("RockPillar").Type, 1f);
                        Gore.NewGore(NPC.position + Vector2.UnitY * y, NPC.velocity + randomVelocity, Mod.Find<ModGore>("RockPillar2").Type, 1f);
                        Gore.NewGore(NPC.position + Vector2.UnitY * y * 2f, NPC.velocity + randomVelocity, Mod.Find<ModGore>("RockPillar3").Type, 1f);
                        Gore.NewGore(NPC.position + Vector2.UnitY * y * 3f, NPC.velocity + randomVelocity, Mod.Find<ModGore>("RockPillar4").Type, 1f);
                        Gore.NewGore(NPC.position + Vector2.UnitY * y * 4f, NPC.velocity + randomVelocity, Mod.Find<ModGore>("RockPillar5").Type, 1f);
                        Gore.NewGore(NPC.position + Vector2.UnitY * y * 5f, NPC.velocity + randomVelocity, Mod.Find<ModGore>("RockPillar6").Type, 1f);
                    }
                }
            }
            else
            {
                for (int num621 = 0; num621 < 2; num621++)
                {
                    int num622 = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, DustID.Iron, 0f, 0f, 100, default, 2f);
                    Main.dust[num622].velocity *= 3f;
                    if (Main.rand.NextBool(2))
                    {
                        Main.dust[num622].scale = 0.5f;
                        Main.dust[num622].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                    }
                }
                for (int num623 = 0; num623 < 2; num623++)
                {
                    int num624 = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, DustID.Stone, 0f, 0f, 100, default, 3f);
                    Main.dust[num624].noGravity = true;
                    Main.dust[num624].velocity *= 5f;
                    num624 = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, DustID.Iron, 0f, 0f, 100, default, 2f);
                    Main.dust[num624].velocity *= 2f;
                }
            }
        }
    }
}
