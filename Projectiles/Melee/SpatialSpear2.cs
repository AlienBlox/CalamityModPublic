﻿using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Melee
{
    public class SpatialSpear2 : ModProjectile
    {
    	public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Spear");
		}

        public override void SetDefaults()
        {
            projectile.width = 10;
            projectile.height = 10;
            projectile.friendly = true;
            projectile.melee = true;
            projectile.tileCollide = false;
            projectile.penetrate = 2;
            projectile.timeLeft = 120;
            projectile.usesLocalNPCImmunity = true;
			projectile.localNPCHitCooldown = 6;
        }

        public override void AI()
        {
        	Lighting.AddLight(projectile.Center, 0.05f, 0.05f, 1f);
			projectile.rotation = (float)Math.Atan2((double)projectile.velocity.Y, (double)projectile.velocity.X) + 0.785f;
			if (projectile.localAI[1] == 0f)
			{
				projectile.scale -= 0.01f;
				projectile.alpha += 15;
				if (projectile.alpha >= 125)
				{
					projectile.alpha = 130;
					projectile.localAI[1] = 1f;
				}
			}
			else if (projectile.localAI[1] == 1f)
			{
				projectile.scale += 0.01f;
				projectile.alpha -= 15;
				if (projectile.alpha <= 0)
				{
					projectile.alpha = 0;
					projectile.localAI[1] = 0f;
				}
			}
			if (Main.rand.NextBool(8))
			{
				Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 56, projectile.velocity.X * 0.1f, projectile.velocity.Y * 0.1f);
			}
			projectile.localAI[0] += 1f;
			if (projectile.localAI[0] >= 60f)
			{
				projectile.localAI[0] = 0f;
				int numProj = 2;
				float rotation = MathHelper.ToRadians(20);
				if (projectile.owner == Main.myPlayer)
				{
					for (int i = 0; i < numProj + 1; i++)
					{
						Vector2 perturbedSpeed = new Vector2(projectile.velocity.X, projectile.velocity.Y).RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (numProj - 1)));
						Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, perturbedSpeed.X * 0.5f, perturbedSpeed.Y * 0.5f, mod.ProjectileType("SpatialSpear3"), (int)((double)projectile.damage * 0.5), projectile.knockBack * 0.5f, projectile.owner, 0f, 0f);
					}
				}
			}
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			Texture2D tex = Main.projectileTexture[projectile.type];
			spriteBatch.Draw(tex, projectile.Center - Main.screenPosition, null, projectile.GetAlpha(lightColor), projectile.rotation, tex.Size() / 2f, projectile.scale, SpriteEffects.None, 0f);
			return false;
		}

		public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
        	target.AddBuff(mod.BuffType("BrimstoneFlames"), 120);
        	target.AddBuff(mod.BuffType("GlacialState"), 120);
        	target.AddBuff(mod.BuffType("Plague"), 120);
        	target.AddBuff(mod.BuffType("HolyLight"), 120);
        }

		public override void Kill(int timeLeft)
		{
			int num3;
			for (int num795 = 4; num795 < 12; num795 = num3 + 1)
			{
				float num796 = projectile.oldVelocity.X * (30f / (float)num795);
				float num797 = projectile.oldVelocity.Y * (30f / (float)num795);
				int num798 = Dust.NewDust(new Vector2(projectile.oldPosition.X - num796, projectile.oldPosition.Y - num797), 8, 8, 56, projectile.oldVelocity.X, projectile.oldVelocity.Y, 100, default, 1.8f);
				Main.dust[num798].noGravity = true;
				Dust dust = Main.dust[num798];
				dust.velocity *= 0.5f;
				num798 = Dust.NewDust(new Vector2(projectile.oldPosition.X - num796, projectile.oldPosition.Y - num797), 8, 8, 56, projectile.oldVelocity.X, projectile.oldVelocity.Y, 100, default, 1.4f);
				dust = Main.dust[num798];
				dust.velocity *= 0.05f;
				num3 = num795;
			}
		}
	}
}
