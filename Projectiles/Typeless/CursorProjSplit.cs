﻿using System;
using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
namespace CalamityMod.Projectiles.Typeless
{
    public class CursorProjSplit : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Typeless";
        public override string Texture => "CalamityMod/Projectiles/Typeless/CursorProj";

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 1;
        }

        public override void SetDefaults()
        {
            Projectile.width = 12;
            Projectile.height = 12;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.extraUpdates = 1;
            Projectile.alpha = 255;
            Projectile.timeLeft = 300;
        }

        public override void AI()
        {
            if (Projectile.alpha > 0)
                Projectile.alpha -= 3;

            Projectile.rotation = (float)Math.Atan2((double)Projectile.velocity.Y, (double)Projectile.velocity.X) + 1.57f;

            float num29 = 5f;
            float num30 = 300f;
            float scaleFactor = 6f;
            Vector2 value7 = new Vector2(10f, 20f);
            //float num31 = 1f;
            int num32 = 3 * Projectile.MaxUpdates;
            int num33 = Utils.SelectRandom(Main.rand, new int[]
            {
                246,
                242,
                229,
                226,
                247
            });
            int num34 = 255;
            if (Projectile.ai[1] == 0f)
            {
                Projectile.ai[1] = 1f;
                Projectile.localAI[0] = (float)-(float)Main.rand.Next(48);
            }
            else if (Projectile.ai[1] == 1f && Projectile.owner == Main.myPlayer)
            {
                if (Projectile.alpha < 128)
                {
                    int num35 = -1;
                    float num36 = num30;
                    for (int num37 = 0; num37 < Main.maxNPCs; num37++)
                    {
                        if (Main.npc[num37].active && Main.npc[num37].CanBeChasedBy(Projectile, false))
                        {
                            Vector2 center3 = Main.npc[num37].Center;
                            float num38 = Vector2.Distance(center3, Projectile.Center);
                            if (num38 < num36 && num35 == -1 && Collision.CanHitLine(Projectile.Center, 1, 1, center3, 1, 1))
                            {
                                num36 = num38;
                                num35 = num37;
                            }
                        }
                    }
                    if (num36 < 4f)
                    {
                        Projectile.Kill();
                        return;
                    }
                    if (num35 != -1)
                    {
                        Projectile.ai[1] = num29 + 1f;
                        Projectile.ai[0] = (float)num35;
                        Projectile.netUpdate = true;
                    }
                }
            }
            else if (Projectile.ai[1] > num29)
            {
                Projectile.ai[1] += 1f;
                int num39 = (int)Projectile.ai[0];
                if (!Main.npc[num39].active || !Main.npc[num39].CanBeChasedBy(Projectile, false))
                {
                    Projectile.ai[1] = 1f;
                    Projectile.ai[0] = 0f;
                    Projectile.netUpdate = true;
                }
                else
                {
                    Projectile.velocity.ToRotation();
                    Vector2 vector6 = Main.npc[num39].Center - Projectile.Center;
                    if (vector6.Length() < 10f)
                    {
                        Projectile.Kill();
                        return;
                    }
                    if (vector6 != Vector2.Zero)
                    {
                        vector6.Normalize();
                        vector6 *= scaleFactor;
                    }
                    float num40 = 30f;
                    Projectile.velocity = (Projectile.velocity * (num40 - 1f) + vector6) / num40;
                }
            }
            if (Projectile.ai[1] >= 1f && Projectile.ai[1] < num29)
            {
                Projectile.ai[1] += 1f;
                if (Projectile.ai[1] == num29)
                {
                    Projectile.ai[1] = 1f;
                }
            }
            Projectile.localAI[0] += 1f;
            if (Projectile.localAI[0] == 48f)
            {
                Projectile.localAI[0] = 0f;
            }
            if (Main.rand.NextBool(12))
            {
                Vector2 value9 = -Vector2.UnitX.RotatedByRandom(0.19634954631328583).RotatedBy((double)Projectile.velocity.ToRotation(), default);
                int num44 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, num34, 0f, 0f, 100, default, 1f);
                Main.dust[num44].velocity *= 0.1f;
                Main.dust[num44].position = Projectile.Center + value9 * (float)Projectile.width / 2f + Projectile.velocity * 2f;
                Main.dust[num44].fadeIn = 0.9f;
            }
            if (Main.rand.NextBool(18))
            {
                Vector2 value10 = -Vector2.UnitX.RotatedByRandom(0.39269909262657166).RotatedBy((double)Projectile.velocity.ToRotation(), default);
                int num46 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 107, 0f, 0f, 155, default, 0.8f);
                Main.dust[num46].velocity *= 0.3f;
                Main.dust[num46].position = Projectile.Center + value10 * (float)Projectile.width / 2f;
                if (Main.rand.NextBool(2))
                {
                    Main.dust[num46].fadeIn = 1.4f;
                }
            }
            if (Main.rand.NextBool(8))
            {
                Vector2 value11 = -Vector2.UnitX.RotatedByRandom(0.78539818525314331).RotatedBy((double)Projectile.velocity.ToRotation(), default);
                int num48 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, num33, 0f, 0f, 0, default, 1f);
                Main.dust[num48].velocity *= 0.3f;
                Main.dust[num48].noGravity = true;
                Main.dust[num48].position = Projectile.Center + value11 * (float)Projectile.width / 2f;
                if (Main.rand.NextBool(2))
                {
                    Main.dust[num48].fadeIn = 1.4f;
                }
            }
            if (Main.rand.NextBool(6))
            {
                Vector2 value13 = -Vector2.UnitX.RotatedByRandom(0.19634954631328583).RotatedBy((double)Projectile.velocity.ToRotation(), default);
                int num50 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, num34, 0f, 0f, 100, default, 1f);
                Main.dust[num50].velocity *= 0.3f;
                Main.dust[num50].position = Projectile.Center + value13 * (float)Projectile.width / 2f;
                Main.dust[num50].fadeIn = 1.2f;
                Main.dust[num50].scale = 1.5f;
                Main.dust[num50].noGravity = true;
            }
            Lighting.AddLight(Projectile.Center, (255 - Projectile.alpha) * 0.2f / 255f, (255 - Projectile.alpha) * 0.2f / 255f, (255 - Projectile.alpha) * 0.2f / 255f);
            int num154 = 14;
            int num155 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width - num154 * 2, Projectile.height - num154 * 2, 234, 0f, 0f, 100, default, 0.8f);
            Main.dust[num155].velocity *= 0.1f;
            Main.dust[num155].velocity += Projectile.velocity * 0.5f;
            Main.dust[num155].noGravity = true;
            if (Main.rand.NextBool(12))
            {
                int num156 = 16;
                int num157 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width - num156 * 2, Projectile.height - num156 * 2, 159, 0f, 0f, 100, default, 1f);
                Main.dust[num157].velocity *= 0.25f;
                Main.dust[num157].velocity += Projectile.velocity * 0.5f;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 2);
            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(ModContent.BuffType<Vaporfied>(), 60);

        public override void OnHitPlayer(Player target, Player.HurtInfo info) => target.AddBuff(ModContent.BuffType<Vaporfied>(), 60);

        public override bool? CanHitNPC(NPC target)
        {
            if (Projectile.alpha >= 128)
            {
                return false;
            }
            return null;
        }

        public override bool CanHitPvp(Player target) => Projectile.alpha < 128;

        public override void Kill(int timeLeft)
        {
            int num47 = Utils.SelectRandom(Main.rand, new int[]
            {
                246,
                242,
                229,
                226,
                247
            });

            int num48 = 187;
            int num49 = 234;
            //int height = 50;
            float num50 = 1.2f;
            float num51 = 0.6f;
            float num52 = 1.5f;

            Vector2 value3 = (Projectile.rotation - 1.57079637f).ToRotationVector2();
            Vector2 value4 = value3 * Projectile.velocity.Length() * (float)Projectile.MaxUpdates;

            SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);

            int num3;
            for (int num53 = 0; num53 < 20; num53 = num3 + 1)
            {
                int num54 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, num47, 0f, 0f, 200, default, num50);
                Dust dust = Main.dust[num54];
                dust.position = Projectile.Center + Vector2.UnitY.RotatedByRandom(3.1415927410125732) * (float)Main.rand.NextDouble() * (float)Projectile.width / 2f;
                dust.noGravity = true;
                dust.velocity.Y -= 6f;
                dust.velocity *= 3f;
                dust.velocity += value4 * Main.rand.NextFloat();
                num54 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, num48, 0f, 0f, 100, default, num51);
                dust.position = Projectile.Center + Vector2.UnitY.RotatedByRandom(3.1415927410125732) * (float)Main.rand.NextDouble() * (float)Projectile.width / 2f;
                dust.velocity.Y -= 6f;
                dust.velocity *= 2f;
                dust.noGravity = true;
                dust.fadeIn = 1f;
                dust.color = Color.Cyan * 0.5f;
                dust.velocity += value4 * Main.rand.NextFloat();
                num3 = num53;
            }

            for (int num55 = 0; num55 < 10; num55 = num3 + 1)
            {
                int num56 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, num49, 0f, 0f, 0, default, num52);
                Main.dust[num56].position = Projectile.Center + Vector2.UnitX.RotatedByRandom(3.1415927410125732).RotatedBy((double)Projectile.velocity.ToRotation(), default) * (float)Projectile.width / 3f;
                Dust dust = Main.dust[num56];
                dust.noGravity = true;
                dust.velocity.Y -= 6f;
                dust.velocity *= 0.5f;
                dust.velocity += value4 * (0.6f + 0.6f * Main.rand.NextFloat());
                num3 = num55;
            }
        }
    }
}
