﻿using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Melee
{
    public class Leech : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Leech");
        }

        public override void SetDefaults()
        {
            projectile.width = 4;
            projectile.height = 4;
            projectile.friendly = true;
            projectile.penetrate = 2;
            projectile.timeLeft = 180;
            projectile.melee = true;
        }

        public override void AI()
        {
            projectile.localAI[0] += 1f;
            if (projectile.localAI[0] > 4f)
            {
                for (int num468 = 0; num468 < 2; num468++)
                {
                    int num469 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 14, 0f, 0f, 100, default, 1f);
                    Main.dust[num469].noGravity = true;
                    Main.dust[num469].velocity *= 0f;
                }
            }

			CalamityGlobalProjectile.HomeInOnNPC(projectile, false, 400f, 6f, 20f);
        }

        public override void Kill(int timeLeft)
        {
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 14, projectile.oldVelocity.X * 0.5f, projectile.oldVelocity.Y * 0.5f);
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
			if (Main.player[projectile.owner].moonLeech)
				return;
            Player player = Main.player[projectile.owner];
            player.statLife += 5;
            player.HealEffect(5);
        }
    }
}
