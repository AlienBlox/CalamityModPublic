﻿using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Magic
{
	public class GhastlyExplosionShard : ModProjectile
    {
    	public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Explosion");
		}
    	
        public override void SetDefaults()
        {
            projectile.width = 8;
            projectile.height = 8;
            projectile.friendly = true;
            projectile.alpha = 255;
            projectile.penetrate = 1;
            projectile.magic = true;
        }

        public override void AI()
        {
        	int num3 = 0;
			int num332 = (int)projectile.ai[0];
			projectile.ai[1] += 1f;
			float num333 = (60f - projectile.ai[1]) / 60f;
			if (projectile.ai[1] > 40f) 
			{
				projectile.Kill();
			}
			projectile.velocity.Y = projectile.velocity.Y + 0.2f;
			if (projectile.velocity.Y > 18f) 
			{
				projectile.velocity.Y = 18f;
			}
			projectile.velocity.X = projectile.velocity.X * 0.98f;
			for (int num334 = 0; num334 < 2; num334 = num3 + 1) 
			{
				int num335 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, num332, projectile.velocity.X, projectile.velocity.Y, 50, default(Color), 1.1f);
				Main.dust[num335].position = (Main.dust[num335].position + projectile.Center) / 2f;
				Main.dust[num335].noGravity = true;
				Dust dust = Main.dust[num335];
				dust.velocity *= 0.3f;
				dust = Main.dust[num335];
				dust.scale *= num333;
				num3 = num334;
			}
			for (int num336 = 0; num336 < 1; num336 = num3 + 1) 
			{
				int num335 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, num332, projectile.velocity.X, projectile.velocity.Y, 50, default(Color), 0.6f);
				Main.dust[num335].position = (Main.dust[num335].position + projectile.Center * 5f) / 6f;
				Dust dust = Main.dust[num335];
				dust.velocity *= 0.1f;
				Main.dust[num335].noGravity = true;
				Main.dust[num335].fadeIn = 0.9f * num333;
				dust = Main.dust[num335];
				dust.scale *= num333;
				num3 = num336;
			}
			return;
        }

        public override void Kill(int timeLeft)
        {
			int num3;
			for (int num114 = 0; num114 < 10; num114 = num3 + 1)
			{
				int num115 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, (int)projectile.ai[0], projectile.velocity.X * 0.1f, projectile.velocity.Y * 0.1f, 0, default(Color), 0.5f);
				Dust dust;
				Main.dust[num115].scale = 1f + (float)Main.rand.Next(-10, 11) * 0.01f;
				Main.dust[num115].noGravity = true;
				dust = Main.dust[num115];
				dust.velocity *= 1.25f;
				dust = Main.dust[num115];
				dust.velocity -= projectile.oldVelocity / 10f;
				num3 = num114;
			}
        }
    }
}
