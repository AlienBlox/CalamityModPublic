﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles; using CalamityMod.Tiles; using CalamityMod.Walls;

namespace CalamityMod.Projectiles
{
    public class FlareBoltProjectile : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Flare Bolt");
			ProjectileID.Sets.TrailCacheLength[projectile.type] = 2;
			ProjectileID.Sets.TrailingMode[projectile.type] = 0;
		}

        public override void SetDefaults()
        {
            projectile.width = 16;
            projectile.height = 16;
            projectile.friendly = true;
            projectile.alpha = 255;
            projectile.penetrate = 4;
            projectile.timeLeft = 480;
            projectile.magic = true;
        }

        public override void AI()
        {
			if (projectile.alpha > 0)
			{
				projectile.alpha -= 25;
				if (projectile.alpha < 0)
					projectile.alpha = 0;
			}
			Lighting.AddLight(projectile.Center, (255 - projectile.alpha) * 0.45f / 255f, (255 - projectile.alpha) * 0.2f / 255f, (255 - projectile.alpha) * 0.1f / 255f);
            for (int num92 = 0; num92 < 2; num92++)
            {
                float num93 = projectile.velocity.X / 3f * (float)num92;
                float num94 = projectile.velocity.Y / 3f * (float)num92;
                int num95 = 4;
                int num96 = Dust.NewDust(new Vector2(projectile.position.X + (float)num95, projectile.position.Y + (float)num95), projectile.width - num95 * 2, projectile.height - num95 * 2, 174, 0f, 0f, 100, default, 1.2f);
                Main.dust[num96].noGravity = true;
                Main.dust[num96].velocity *= 0.1f;
                Main.dust[num96].velocity += projectile.velocity * 0.1f;
                Dust expr_47FA_cp_0 = Main.dust[num96];
                expr_47FA_cp_0.position.X -= num93;
                Dust expr_4815_cp_0 = Main.dust[num96];
                expr_4815_cp_0.position.Y -= num94;
            }
            if (Main.rand.NextBool(10))
            {
                int num97 = 4;
                int num98 = Dust.NewDust(new Vector2(projectile.position.X + (float)num97, projectile.position.Y + (float)num97), projectile.width - num97 * 2, projectile.height - num97 * 2, 174, 0f, 0f, 100, default, 0.6f);
                Main.dust[num98].velocity *= 0.25f;
                Main.dust[num98].velocity += projectile.velocity * 0.5f;
            }
            projectile.rotation += 0.3f * (float)projectile.direction;
        }

		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			CalamityGlobalProjectile.DrawCenteredAndAfterimage(projectile, lightColor, ProjectileID.Sets.TrailingMode[projectile.type], 1);
			return false;
		}

		public override bool OnTileCollide(Vector2 oldVelocity)
        {
            projectile.penetrate--;
            if (projectile.penetrate <= 0)
            {
                projectile.Kill();
            }
            else
            {
                if (projectile.velocity.X != oldVelocity.X)
                {
                    projectile.velocity.X = -oldVelocity.X;
                }
                if (projectile.velocity.Y != oldVelocity.Y)
                {
                    projectile.velocity.Y = -oldVelocity.Y;
                }
                Main.PlaySound(2, (int)projectile.position.X, (int)projectile.position.Y, 10);
            }
            return false;
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(2, (int)projectile.position.X, (int)projectile.position.Y, 10);
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 174, projectile.oldVelocity.X * 0.5f, projectile.oldVelocity.Y * 0.5f);
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.OnFire, 240);
        }
    }
}
