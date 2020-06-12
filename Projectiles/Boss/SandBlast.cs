﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Boss
{
    public class SandBlast : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sand Blast");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            projectile.width = 10;
            projectile.height = 10;
            projectile.hostile = true;
            projectile.ignoreWater = true;
            projectile.penetrate = 1;
            projectile.timeLeft = 600;
            projectile.alpha = 255;
        }

        public override void AI()
        {
            if (projectile.ai[1] == 0f)
            {
                for (int num621 = 0; num621 < 2; num621++)
                {
                    int num622 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 85, 0f, 0f, 100, default, 1f);
                    Main.dust[num622].velocity *= 3f;
                    if (Main.rand.NextBool(2))
                    {
                        Main.dust[num622].scale = 0.5f;
                        Main.dust[num622].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                    }
                }
                projectile.ai[1] = 1f;
                Main.PlaySound(2, (int)projectile.position.X, (int)projectile.position.Y, 21);
            }
            projectile.rotation = (float)Math.Atan2(projectile.velocity.Y, projectile.velocity.X) + 1.57f;
            projectile.alpha -= 50;
            if (projectile.alpha < 0)
            {
                projectile.alpha = 0;
            }
            int num469 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 85, 0f, 0f, 100, default, 0.8f);
            Main.dust[num469].noGravity = true;
            Main.dust[num469].velocity *= 0f;
        }

        public override void Kill(int timeLeft)
        {
			Main.PlaySound(SoundID.Item14, projectile.position);
			for (int dust = 0; dust <= 3; dust++)
            {
                float num463 = Main.rand.Next(-10, 11);
                float num464 = Main.rand.Next(-10, 11);
                float num465 = Main.rand.Next(3, 9);
                float num466 = (float)Math.Sqrt(num463 * num463 + num464 * num464);
                num466 = num465 / num466;
                num463 *= num466;
                num464 *= num466;
                int num467 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 85, 0f, 0f, 100, default, 1.2f);
                Main.dust[num467].noGravity = true;
                Main.dust[num467].position.X = projectile.Center.X;
                Main.dust[num467].position.Y = projectile.Center.Y;
                Dust expr_149DF_cp_0 = Main.dust[num467];
                expr_149DF_cp_0.position.X += Main.rand.Next(-10, 11);
                Dust expr_14A09_cp_0 = Main.dust[num467];
                expr_14A09_cp_0.position.Y += Main.rand.Next(-10, 11);
                Main.dust[num467].velocity.X = num463;
                Main.dust[num467].velocity.Y = num464;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityGlobalProjectile.DrawCenteredAndAfterimage(projectile, lightColor, ProjectileID.Sets.TrailingMode[projectile.type], 1);
            return false;
        }
    }
}
