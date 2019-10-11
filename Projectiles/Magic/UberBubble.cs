﻿using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Magic
{
    public class UberBubble : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Bubble");
        }

        public override void SetDefaults()
        {
            projectile.width = 10;
            projectile.height = 10;
            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.extraUpdates = 1;
            projectile.alpha = 255;
            projectile.ignoreWater = true;
            projectile.magic = true;
        }

        public override void AI()
        {
            projectile.velocity.X *= 0.975f;
            projectile.velocity.Y *= 0.975f;
            if (projectile.alpha > 0)
            {
                projectile.alpha -= 30;
            }
            if (projectile.alpha < 0)
            {
                projectile.alpha = 0;
            }
            Vector2 v2 = projectile.ai[0].ToRotationVector2();
            float num743 = projectile.velocity.ToRotation();
            float num744 = v2.ToRotation();
            double num745 = (double)(num744 - num743);
            if (num745 > 3.1415926535897931)
            {
                num745 -= 6.2831853071795862;
            }
            if (num745 < -3.1415926535897931)
            {
                num745 += 6.2831853071795862;
            }
            projectile.rotation = projectile.velocity.ToRotation() - 1.57079637f;
            if (Main.myPlayer == projectile.owner && projectile.timeLeft > 30)
                projectile.timeLeft = 30;
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(2, (int)projectile.position.X, (int)projectile.position.Y, 96);
            int num190 = Main.rand.Next(4, 6);
            for (int num191 = 0; num191 < num190; num191++)
            {
                int num192 = Dust.NewDust(projectile.Center, 0, 0, 171, 0f, 0f, 100, default, 1.4f);
                Main.dust[num192].velocity *= 0.8f;
                Main.dust[num192].position = Vector2.Lerp(Main.dust[num192].position, projectile.Center, 0.5f);
                Main.dust[num192].noGravity = true;
            }
            if (projectile.owner == Main.myPlayer)
            {
                for (int numBubbles = 0; numBubbles <= (Main.rand.Next(3, 7)); numBubbles++)
                {
                    Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, projectile.velocity.X * (Main.rand.NextFloat() * 2f), projectile.velocity.Y * (Main.rand.NextFloat() * 2f), mod.ProjectileType("BlueBubble"), projectile.damage, projectile.knockBack, projectile.owner, 0f, 0f);
                }
            }
        }
    }
}
