﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Ranged
{
    public class HiveBomb : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Bomb");
        }

        public override void SetDefaults()
        {
            projectile.width = 14;
            projectile.height = 14;
            projectile.friendly = true;
            projectile.penetrate = 1;
            projectile.timeLeft = 95;
            projectile.ranged = true;
        }

        public override void AI()
        {
            if (projectile.owner == Main.myPlayer && projectile.timeLeft <= 3)
            {
                projectile.tileCollide = false;
                projectile.ai[1] = 0f;
                projectile.alpha = 255;
                projectile.position.X = projectile.position.X + (float)(projectile.width / 2);
                projectile.position.Y = projectile.position.Y + (float)(projectile.height / 2);
                projectile.width = 200;
                projectile.height = 200;
                projectile.position.X = projectile.position.X - (float)(projectile.width / 2);
                projectile.position.Y = projectile.position.Y - (float)(projectile.height / 2);
                projectile.knockBack = 10f;
            }
            else
            {
                if (Math.Abs(projectile.velocity.X) >= 8f || Math.Abs(projectile.velocity.Y) >= 8f)
                {
                    for (int num246 = 0; num246 < 2; num246++)
                    {
                        float num247 = 0f;
                        float num248 = 0f;
                        if (num246 == 1)
                        {
                            num247 = projectile.velocity.X * 0.5f;
                            num248 = projectile.velocity.Y * 0.5f;
                        }
                        int num249 = Dust.NewDust(new Vector2(projectile.position.X + 3f + num247, projectile.position.Y + 3f + num248) - projectile.velocity * 0.5f, projectile.width - 8, projectile.height - 8, 6, 0f, 0f, 100, default, 1f);
                        Main.dust[num249].scale *= 2f + (float)Main.rand.Next(10) * 0.1f;
                        Main.dust[num249].velocity *= 0.2f;
                        Main.dust[num249].noGravity = true;
                        num249 = Dust.NewDust(new Vector2(projectile.position.X + 3f + num247, projectile.position.Y + 3f + num248) - projectile.velocity * 0.5f, projectile.width - 8, projectile.height - 8, 31, 0f, 0f, 100, default, 0.5f);
                        Main.dust[num249].fadeIn = 1f + (float)Main.rand.Next(5) * 0.1f;
                        Main.dust[num249].velocity *= 0.05f;
                    }
                }
                if (Math.Abs(projectile.velocity.X) < 15f && Math.Abs(projectile.velocity.Y) < 15f)
                {
                    projectile.velocity *= 1.1f;
                }
                else if (Main.rand.NextBool(2))
                {
                    int num252 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 31, 0f, 0f, 100, default, 1f);
                    Main.dust[num252].scale = 0.1f + (float)Main.rand.Next(5) * 0.1f;
                    Main.dust[num252].fadeIn = 1.5f + (float)Main.rand.Next(5) * 0.1f;
                    Main.dust[num252].noGravity = true;
                    Main.dust[num252].position = projectile.Center + new Vector2(0f, (float)(-(float)projectile.height / 2)).RotatedBy((double)projectile.rotation, default) * 1.1f;
                    Main.rand.Next(2);
                    num252 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 6, 0f, 0f, 100, default, 1f);
                    Main.dust[num252].scale = 1f + (float)Main.rand.Next(5) * 0.1f;
                    Main.dust[num252].noGravity = true;
                    Main.dust[num252].position = projectile.Center + new Vector2(0f, (float)(-(float)projectile.height / 2 - 6)).RotatedBy((double)projectile.rotation, default) * 1.1f;
                }
            }
            projectile.ai[0] += 1f;
            projectile.rotation = (float)Math.Atan2((double)projectile.velocity.Y, (double)projectile.velocity.X) + 1.57f;
            if (projectile.ai[0] > 10f || projectile.ai[0] > 5f)
            {
                projectile.ai[0] = 10f;
                if (projectile.velocity.Y == 0f && projectile.velocity.X != 0f)
                {
                    projectile.velocity.X = projectile.velocity.X * 0.97f;
                    if ((double)projectile.velocity.X > -0.01 && (double)projectile.velocity.X < 0.01)
                    {
                        projectile.velocity.X = 0f;
                        projectile.netUpdate = true;
                    }
                }
                projectile.velocity.Y = projectile.velocity.Y + 0.2f;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D tex = Main.projectileTexture[projectile.type];
            spriteBatch.Draw(tex, projectile.Center - Main.screenPosition, null, projectile.GetAlpha(lightColor), projectile.rotation, tex.Size() / 2f, projectile.scale, SpriteEffects.None, 0f);
            return false;
        }

        public override void Kill(int timeLeft)
        {
            projectile.position = projectile.Center;
            projectile.width = projectile.height = 64;
            projectile.position.X = projectile.position.X - (float)(projectile.width / 2);
            projectile.position.Y = projectile.position.Y - (float)(projectile.height / 2);
            projectile.maxPenetrate = -1;
            projectile.penetrate = -1;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 10;
            projectile.Damage();
            if (projectile.owner == Main.myPlayer)
            {
                int num516 = 6;
                for (int num517 = 0; num517 < num516; num517++)
                {
                    if (num517 % 2 != 1 || Main.rand.NextBool(3))
                    {
                        Vector2 value20 = projectile.position;
                        Vector2 value21 = projectile.oldVelocity;
                        value21.Normalize();
                        value21 *= 8f;
                        float num518 = (float)Main.rand.Next(-35, 36) * 0.01f;
                        float num519 = (float)Main.rand.Next(-35, 36) * 0.01f;
                        value20 -= value21 * (float)num517;
                        num518 += projectile.oldVelocity.X / 6f;
                        num519 += projectile.oldVelocity.Y / 6f;
                        int num520 = Projectile.NewProjectile(value20.X, value20.Y, num518, num519, Main.player[projectile.owner].beeType(), Main.player[projectile.owner].beeDamage(projectile.damage / 3), Main.player[projectile.owner].beeKB(0f), Main.myPlayer, 0f, 0f);
                        Main.projectile[num520].penetrate = 2;
                        Main.projectile[num520].Calamity().forceRanged = true;
                    }
                }
            }
            Main.PlaySound(2, (int)projectile.Center.X, (int)projectile.Center.Y, 14);
            for (int num621 = 0; num621 < 20; num621++)
            {
                int num622 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 89, 0f, 0f, 100, default, 2f);
                Main.dust[num622].velocity *= 3f;
                if (Main.rand.NextBool(2))
                {
                    Main.dust[num622].scale = 0.5f;
                    Main.dust[num622].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                }
            }
            for (int num623 = 0; num623 < 30; num623++)
            {
                int num624 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 89, 0f, 0f, 100, default, 3f);
                Main.dust[num624].noGravity = true;
                Main.dust[num624].velocity *= 5f;
                num624 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 89, 0f, 0f, 100, default, 2f);
                Main.dust[num624].velocity *= 2f;
            }
            for (int num625 = 0; num625 < 3; num625++)
            {
                float scaleFactor10 = 0.33f;
                if (num625 == 1)
                {
                    scaleFactor10 = 0.66f;
                }
                if (num625 == 2)
                {
                    scaleFactor10 = 1f;
                }
                int num626 = Gore.NewGore(new Vector2(projectile.position.X + (float)(projectile.width / 2) - 24f, projectile.position.Y + (float)(projectile.height / 2) - 24f), default, Main.rand.Next(61, 64), 1f);
                Main.gore[num626].velocity *= scaleFactor10;
                Gore expr_13AB6_cp_0 = Main.gore[num626];
                expr_13AB6_cp_0.velocity.X += 1f;
                Gore expr_13AD6_cp_0 = Main.gore[num626];
                expr_13AD6_cp_0.velocity.Y += 1f;
                num626 = Gore.NewGore(new Vector2(projectile.position.X + (float)(projectile.width / 2) - 24f, projectile.position.Y + (float)(projectile.height / 2) - 24f), default, Main.rand.Next(61, 64), 1f);
                Main.gore[num626].velocity *= scaleFactor10;
                Gore expr_13B79_cp_0 = Main.gore[num626];
                expr_13B79_cp_0.velocity.X -= 1f;
                Gore expr_13B99_cp_0 = Main.gore[num626];
                expr_13B99_cp_0.velocity.Y += 1f;
                num626 = Gore.NewGore(new Vector2(projectile.position.X + (float)(projectile.width / 2) - 24f, projectile.position.Y + (float)(projectile.height / 2) - 24f), default, Main.rand.Next(61, 64), 1f);
                Main.gore[num626].velocity *= scaleFactor10;
                Gore expr_13C3C_cp_0 = Main.gore[num626];
                expr_13C3C_cp_0.velocity.X += 1f;
                Gore expr_13C5C_cp_0 = Main.gore[num626];
                expr_13C5C_cp_0.velocity.Y -= 1f;
                num626 = Gore.NewGore(new Vector2(projectile.position.X + (float)(projectile.width / 2) - 24f, projectile.position.Y + (float)(projectile.height / 2) - 24f), default, Main.rand.Next(61, 64), 1f);
                Main.gore[num626].velocity *= scaleFactor10;
                Gore expr_13CFF_cp_0 = Main.gore[num626];
                expr_13CFF_cp_0.velocity.X -= 1f;
                Gore expr_13D1F_cp_0 = Main.gore[num626];
                expr_13D1F_cp_0.velocity.Y -= 1f;
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(mod.BuffType("Plague"), 900);
        }
    }
}
