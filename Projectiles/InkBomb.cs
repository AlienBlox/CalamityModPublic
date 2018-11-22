﻿using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles
{
    public class InkBomb : ModProjectile
    {
    	public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Ink Bomb");
		}
    	
        public override void SetDefaults()
        {
            projectile.width = 30;
            projectile.height = 30;
            projectile.hostile = true;
            projectile.ignoreWater = true;
            projectile.penetrate = 1;
            projectile.timeLeft = 300;
        }

        public override void AI()
        {
			projectile.ai[0] += 1f;
			if (projectile.ai[0] > 10f)
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
				projectile.velocity.Y = projectile.velocity.Y - 0.01f;
			}
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(4, (int)projectile.position.X, (int)projectile.position.Y, 28);
            int num251 = Main.rand.Next(5, 8);
            if (projectile.owner == Main.myPlayer)
            {
                int num320 = Main.rand.Next(15, 21);
                int num3;
                for (int num321 = 0; num321 < num320; num321 = num3 + 1)
                {
                    Vector2 vector15 = new Vector2((float)Main.rand.Next(-100, 101), (float)Main.rand.Next(-100, 101));
                    vector15.Normalize();
                    vector15 *= (float)Main.rand.Next(50, 401) * 0.01f;
                    Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, vector15.X, vector15.Y, mod.ProjectileType("InkPoisonCloud") + Main.rand.Next(3), (int)((double)projectile.damage * 0.66), 1f, projectile.owner, 0f, (float)Main.rand.Next(-45, 1));
                    num3 = num321;
                }
            }
            projectile.position.X = projectile.position.X + (float)(projectile.width / 2);
            projectile.position.Y = projectile.position.Y + (float)(projectile.height / 2);
            projectile.width = 60;
            projectile.height = 60;
            projectile.position.X = projectile.position.X - (float)(projectile.width / 2);
            projectile.position.Y = projectile.position.Y - (float)(projectile.height / 2);
            for (int num621 = 0; num621 < 10; num621++)
            {
                int num622 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 54, 0f, 0f, 100, default(Color), 2f);
                Main.dust[num622].velocity *= 3f;
                if (Main.rand.Next(2) == 0)
                {
                    Main.dust[num622].scale = 0.5f;
                    Main.dust[num622].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                }
            }
            for (int num623 = 0; num623 < 15; num623++)
            {
                int num624 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 109, 0f, 0f, 100, default(Color), 3f);
                Main.dust[num624].noGravity = true;
                Main.dust[num624].velocity *= 5f;
                num624 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 109, 0f, 0f, 100, default(Color), 2f);
                Main.dust[num624].velocity *= 2f;
            }
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.AddBuff(BuffID.Venom, 240);
            target.AddBuff(BuffID.Darkness, 600, true);
            projectile.Kill();
        }
    }
}