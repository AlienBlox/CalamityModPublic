﻿using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Boss
{
    public class HiveBombGoliath : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Hive Bomb");
			Main.projFrames[projectile.type] = 4;
			ProjectileID.Sets.TrailCacheLength[projectile.type] = 4;
			ProjectileID.Sets.TrailingMode[projectile.type] = 0;
		}

		public override void SetDefaults()
		{
			projectile.width = 14;
			projectile.height = 14;
			projectile.hostile = true;
			projectile.tileCollide = false;
			projectile.penetrate = 1;
		}

		public override void AI()
		{
			projectile.velocity.X *= 1.01f;
			projectile.velocity.Y *= 1.01f;

			if (projectile.position.Y > projectile.ai[1])
				projectile.tileCollide = true;

			projectile.frameCounter++;
			if (projectile.frameCounter > 4)
			{
				projectile.frame++;
				projectile.frameCounter = 0;
			}
			if (projectile.frame > 3)
			{
				projectile.frame = 0;
			}
			if (Math.Abs(projectile.velocity.X) >= 3f || Math.Abs(projectile.velocity.Y) >= 3f)
			{
				float num247 = 0f;
				float num248 = 0f;
				if (Main.rand.Next(2) == 1)
				{
					num247 = projectile.velocity.X * 0.5f;
					num248 = projectile.velocity.Y * 0.5f;
				}
				int num249 = Dust.NewDust(new Vector2(projectile.position.X + 3f + num247, projectile.position.Y + 3f + num248) - projectile.velocity * 0.5f, projectile.width - 8, projectile.height - 8, 6, 0f, 0f, 100, default, 0.5f);
				Main.dust[num249].scale *= 2f + (float)Main.rand.Next(10) * 0.1f;
				Main.dust[num249].velocity *= 0.2f;
				Main.dust[num249].noGravity = true;
				num249 = Dust.NewDust(new Vector2(projectile.position.X + 3f + num247, projectile.position.Y + 3f + num248) - projectile.velocity * 0.5f, projectile.width - 8, projectile.height - 8, 31, 0f, 0f, 100, default, 0.25f);
				Main.dust[num249].fadeIn = 1f + (float)Main.rand.Next(5) * 0.1f;
				Main.dust[num249].velocity *= 0.05f;
			}
			else if (Main.rand.NextBool(4))
			{
				int num252 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 31, 0f, 0f, 100, default, 0.5f);
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
			projectile.rotation = (float)Math.Atan2((double)projectile.velocity.Y, (double)projectile.velocity.X) + 1.57f;
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			CalamityGlobalProjectile.DrawCenteredAndAfterimage(projectile, lightColor, ProjectileID.Sets.TrailingMode[projectile.type], 1);
			return false;
		}

		public override Color? GetAlpha(Color lightColor)
		{
			return new Color(50, 250, 50, projectile.alpha);
		}

		public override void Kill(int timeLeft)
		{
			projectile.position = projectile.Center;
			projectile.width = (projectile.height = 64);
			projectile.position.X = projectile.position.X - (float)(projectile.width / 2);
			projectile.position.Y = projectile.position.Y - (float)(projectile.height / 2);
			projectile.Damage();
			Main.PlaySound(2, (int)projectile.position.X, (int)projectile.position.Y, 14);
			for (int num621 = 0; num621 < 8; num621++)
			{
				int num622 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 89, 0f, 0f, 100, default, 2f);
				Main.dust[num622].velocity *= 3f;
				if (Main.rand.NextBool(2))
				{
					Main.dust[num622].scale = 0.5f;
					Main.dust[num622].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
				}
			}
			for (int num623 = 0; num623 < 10; num623++)
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
				expr_13AB6_cp_0.velocity.X = expr_13AB6_cp_0.velocity.X + 1f;
				Gore expr_13AD6_cp_0 = Main.gore[num626];
				expr_13AD6_cp_0.velocity.Y = expr_13AD6_cp_0.velocity.Y + 1f;
				num626 = Gore.NewGore(new Vector2(projectile.position.X + (float)(projectile.width / 2) - 24f, projectile.position.Y + (float)(projectile.height / 2) - 24f), default, Main.rand.Next(61, 64), 1f);
				Main.gore[num626].velocity *= scaleFactor10;
				Gore expr_13B79_cp_0 = Main.gore[num626];
				expr_13B79_cp_0.velocity.X = expr_13B79_cp_0.velocity.X - 1f;
				Gore expr_13B99_cp_0 = Main.gore[num626];
				expr_13B99_cp_0.velocity.Y = expr_13B99_cp_0.velocity.Y + 1f;
				num626 = Gore.NewGore(new Vector2(projectile.position.X + (float)(projectile.width / 2) - 24f, projectile.position.Y + (float)(projectile.height / 2) - 24f), default, Main.rand.Next(61, 64), 1f);
				Main.gore[num626].velocity *= scaleFactor10;
				Gore expr_13C3C_cp_0 = Main.gore[num626];
				expr_13C3C_cp_0.velocity.X = expr_13C3C_cp_0.velocity.X + 1f;
				Gore expr_13C5C_cp_0 = Main.gore[num626];
				expr_13C5C_cp_0.velocity.Y = expr_13C5C_cp_0.velocity.Y - 1f;
				num626 = Gore.NewGore(new Vector2(projectile.position.X + (float)(projectile.width / 2) - 24f, projectile.position.Y + (float)(projectile.height / 2) - 24f), default, Main.rand.Next(61, 64), 1f);
				Main.gore[num626].velocity *= scaleFactor10;
				Gore expr_13CFF_cp_0 = Main.gore[num626];
				expr_13CFF_cp_0.velocity.X = expr_13CFF_cp_0.velocity.X - 1f;
				Gore expr_13D1F_cp_0 = Main.gore[num626];
				expr_13D1F_cp_0.velocity.Y = expr_13D1F_cp_0.velocity.Y - 1f;
			}
		}

		public override void OnHitPlayer(Player target, int damage, bool crit)
		{
			target.AddBuff(mod.BuffType("Plague"), 300);
		}
	}
}
