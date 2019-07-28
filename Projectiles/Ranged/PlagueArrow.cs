﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Ranged
{
    public class PlagueArrow : ModProjectile
    {
    	public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Arrow");
			ProjectileID.Sets.TrailCacheLength[projectile.type] = 10;
			ProjectileID.Sets.TrailingMode[projectile.type] = 1;
		}

        public override void SetDefaults()
        {
            projectile.width = 10;
            projectile.height = 10;
            projectile.friendly = true;
            projectile.ranged = true;
            projectile.penetrate = 1;
            projectile.aiStyle = 1;
            projectile.timeLeft = 600;
            aiType = 1;
		}

        public override void Kill(int timeLeft)
        {
        	Main.PlaySound(2, (int)projectile.position.X, (int)projectile.position.Y, 14);
        	if (projectile.owner == Main.myPlayer)
			{
        		Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, 0f, 0f, mod.ProjectileType("PlagueExplosionFriendly"), projectile.damage, projectile.knockBack, projectile.owner, 0f, 0f);
				int num516 = 6;
				for (int num517 = 0; num517 < num516; num517++)
				{
					if (num517 % 2 != 1 || Main.rand.Next(3) == 0)
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
						int num520 = Projectile.NewProjectile(value20.X, value20.Y, num518, num519, Main.player[projectile.owner].beeType(), Main.player[projectile.owner].beeDamage(projectile.damage / 2), Main.player[projectile.owner].beeKB(0f), Main.myPlayer, 0f, 0f);
						Main.projectile[num520].GetGlobalProjectile<CalamityGlobalProjectile>(mod).forceRanged = true;
						Main.projectile[num520].penetrate = 2;
					}
				}
			}
        }

		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			CalamityGlobalProjectile.DrawCenteredAndAfterimage(projectile, lightColor, ProjectileID.Sets.TrailingMode[projectile.type], 2);
			return false;
		}
	}
}
