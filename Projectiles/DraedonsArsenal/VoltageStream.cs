﻿using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.DraedonsArsenal
{
    public class VoltageStream : ModProjectile
    {
        public float Time
        {
            get => projectile.ai[0];
            set => projectile.ai[0] = value;
        }
        public NPC Target
        {
            get => Main.npc[(int)projectile.ai[1]];
            set => projectile.ai[1] = value.whoAmI;
        }
        public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Voltage Stream");
		}

        public override void SetDefaults()
        {
            projectile.width = 8;
            projectile.height = 8;
            projectile.friendly = true;
            projectile.melee = true;
            projectile.penetrate = -1;
            projectile.timeLeft = 600;
			projectile.usesLocalNPCImmunity = true;
			projectile.localNPCHitCooldown = 9;
        }

        public override void AI()
        {
			Lighting.AddLight(projectile.Center, Color.SkyBlue.ToVector3());
            if (!Target.active)
            {
                projectile.Kill();
                return;
            }
            projectile.Center = Target.Center;
            if (Time < 90f)
            {
                float completionRatio = Utils.InverseLerp(0f, 90f, Time, true);
                float offsetRatioOnSprite = (float)Math.Sin(completionRatio * MathHelper.ToRadians(720f)) * 0.5f + 0.5f;
                Vector2 dustInitialPosition = Vector2.Lerp(Target.Top, Target.Bottom, offsetRatioOnSprite);
                for (int i = 0; i < 4; i++)
                {
                    float angularOffset = MathHelper.TwoPi / 4f * i + Time / 90f * MathHelper.ToRadians(1080f);
                    Vector2 dustSpawnPosition = dustInitialPosition + angularOffset.ToRotationVector2() * 4f;
                    dustSpawnPosition.X += 10f * (float)Math.Sin(completionRatio * MathHelper.ToRadians(360f));

                    Dust dust = Dust.NewDustPerfect(dustSpawnPosition, 261);
                    dust.velocity = Vector2.Zero;
                    dust.noGravity = true;

                    dustSpawnPosition = dustInitialPosition + angularOffset.ToRotationVector2() * 4f;
                    dustSpawnPosition.X -= 10f * (float)Math.Sin(completionRatio * MathHelper.ToRadians(360f));
                    dust = Dust.NewDustPerfect(dustSpawnPosition, 261);
                    dust.velocity = Vector2.Zero;
                    dust.noGravity = true;
                }
            }
            else if (Time < 150f)
            {
                for (int i = 0; i < 50; i++)
                {
                    float angle = MathHelper.TwoPi / 50f * i + Utils.InverseLerp(90f, 150f, Time, true) * MathHelper.ToRadians(1080f);
                    float radius = MathHelper.Lerp(0f, 25f, Utils.InverseLerp(90f, 150f, Time, true));
                    Dust dust = Dust.NewDustPerfect(Target.Center + angle.ToRotationVector2() * radius, 226);
                    dust.velocity = Vector2.Zero;
                    if (Main.rand.NextBool(6))
                    {
                        dust.velocity = Target.DirectionTo(dust.position) * 4.5f;
                    }
                    dust.noGravity = true;
                }
            }
            else
            {
                for (int i = 0; i < 120; i++)
                {
                    float angle = MathHelper.TwoPi / 120f * i;
                    Dust dust = Dust.NewDustPerfect(Target.Center + angle.ToRotationVector2() * 25f, 226);
                    dust.velocity = angle.ToRotationVector2() * Main.rand.NextFloat(2f, 9f) * Main.rand.NextBool(2).ToDirectionInt();
                    dust.velocity = dust.velocity.RotatedBy(dust.velocity.ToRotation() * -0.02f);
                    dust.velocity *= 2.1f;
                    dust.noGravity = true;
                }
                Target.AddBuff(BuffID.Electrified, 60 * 10);
                projectile.Kill();
            }
            Time++;

			if (projectile.damage <= 0)
				projectile.Kill();
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
			ReduceDamage();
		}

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
			ReduceDamage();
		}

		private void ReduceDamage()
		{
			projectile.damage = (int)(projectile.damage * 0.75);
		}

		public override void Kill(int timeLeft)
		{
		}
	}
}
