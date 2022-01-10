using CalamityMod.Buffs.StatDebuffs;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Boss
{
    public class IceBomb : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ice Bomb");
        }

        public override void SetDefaults()
        {
			projectile.Calamity().canBreakPlayerDefense = true;
			projectile.width = 30;
            projectile.height = 30;
			projectile.scale = 0.5f;
            projectile.hostile = true;
            projectile.penetrate = 1;
            projectile.timeLeft = 300;
			projectile.Calamity().affectedByMaliceModeVelocityMultiplier = true;
		}

		public override bool CanHitPlayer(Player target) => projectile.ai[0] >= 120f;

		public override void AI()
		{
			projectile.velocity *= 0.99f;

			if (projectile.localAI[0] == 0f)
			{
				projectile.scale += 0.01f;
				projectile.alpha -= 50;
				if (projectile.alpha <= 0)
				{
					projectile.localAI[0] = 1f;
					projectile.alpha = 0;
				}
			}
			else
			{
				projectile.scale -= 0.01f;
				projectile.alpha += 50;
				if (projectile.alpha >= 255)
				{
					projectile.localAI[0] = 0f;
					projectile.alpha = 255;
				}
			}

			if (projectile.ai[0] < 120f)
			{
				projectile.ai[0] += 1f;
				if (projectile.ai[0] == 120f)
				{
					for (int num621 = 0; num621 < 8; num621++)
					{
						int num622 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 67, 0f, 0f, 100, default, 2f);
						Main.dust[num622].velocity *= 3f;
						if (Main.rand.NextBool(2))
						{
							Main.dust[num622].scale = 0.5f;
							Main.dust[num622].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
						}
					}
					for (int num623 = 0; num623 < 14; num623++)
					{
						int num624 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 67, 0f, 0f, 100, default, 3f);
						Main.dust[num624].noGravity = true;
						Main.dust[num624].velocity *= 5f;
						num624 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 67, 0f, 0f, 100, default, 2f);
						Main.dust[num624].velocity *= 2f;
					}

					projectile.scale = 1f;
					CalamityGlobalProjectile.ExpandHitboxBy(projectile, (int)(30f * projectile.scale));
					Main.PlaySound(SoundID.Item30, projectile.Center);
				}
			}
		}

		public override Color? GetAlpha(Color lightColor)
		{
			return Main.dayTime ? new Color(50, 50, 255, projectile.alpha) : new Color(255, 255, 255, projectile.alpha);
		}

		public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.Item27, projectile.position);

            float spread = 60f * 0.0174f;
            double startAngle = Math.Atan2(projectile.velocity.X, projectile.velocity.Y) - spread / 2;
            double deltaAngle = spread / 8f;
            double offsetAngle;
            int i;
			float velocity = 7f;
            if (projectile.owner == Main.myPlayer)
            {
                for (i = 0; i < 3; i++)
                {
                    offsetAngle = startAngle + deltaAngle * (i + i * i) / 2f + 32f * i;
                    Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, (float)(Math.Sin(offsetAngle) * velocity), (float)(Math.Cos(offsetAngle) * velocity), ModContent.ProjectileType<IceRain>(), (int)Math.Round(projectile.damage * 0.75), projectile.knockBack, projectile.owner, 1f, 0f);
                    Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, (float)(-Math.Sin(offsetAngle) * velocity), (float)(-Math.Cos(offsetAngle) * velocity), ModContent.ProjectileType<IceRain>(), (int)Math.Round(projectile.damage * 0.75), projectile.knockBack, projectile.owner, 1f, 0f);
                }
            }

            for (int k = 0; k < 10; k++)
                Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 67, projectile.oldVelocity.X * 0.5f, projectile.oldVelocity.Y * 0.5f);
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
			if (projectile.ai[0] >= 120f)
			{
				target.AddBuff(BuffID.Frostburn, 120, true);
				target.AddBuff(BuffID.Chilled, 90, true);
				target.AddBuff(ModContent.BuffType<GlacialState>(), 60);
			}
        }
    }
}
