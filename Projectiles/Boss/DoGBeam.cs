﻿using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Boss
{
	public class DoGBeam : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Portal Laser");
			Main.projFrames[projectile.type] = 2;
		}

		public override void SetDefaults()
		{
			projectile.width = 6;
			projectile.height = 6;
			projectile.hostile = true;
			projectile.scale = 2f;
			projectile.ignoreWater = true;
			projectile.tileCollide = false;
			projectile.penetrate = 1;
			projectile.timeLeft = 960;
			cooldownSlot = 1;
		}

		public override void AI()
		{
			projectile.frameCounter++;
			if (projectile.frameCounter > 4)
			{
				projectile.frame++;
				projectile.frameCounter = 0;
			}
			if (projectile.frame > 1)
			{
				projectile.frame = 0;
			}
			Lighting.AddLight(projectile.Center, ((255 - projectile.alpha) * 0f) / 255f, ((255 - projectile.alpha) * 0.35f) / 255f, ((255 - projectile.alpha) * 0.45f) / 255f);
			projectile.rotation = (float)Math.Atan2((double)projectile.velocity.Y, (double)projectile.velocity.X) + 1.57f;
			int num103 = (int)Player.FindClosest(projectile.Center, 1, 1);
			projectile.ai[1] += 1f;
			if (projectile.ai[1] < 120f && projectile.ai[1] > 30f)
			{
				float scaleFactor2 = projectile.velocity.Length();
				Vector2 vector11 = Main.player[num103].Center - projectile.Center;
				vector11.Normalize();
				vector11 *= scaleFactor2;
				projectile.velocity = (projectile.velocity * 20f + vector11) / 21f;
				projectile.velocity.Normalize();
				projectile.velocity *= scaleFactor2;
			}
		}

		public override Color? GetAlpha(Color lightColor)
		{
			if (projectile.timeLeft > 950)
			{
				return new Color(0, 0, 0, 0);
			}
			if (projectile.timeLeft < 85)
			{
				byte b2 = (byte)(projectile.timeLeft * 3);
				byte a2 = (byte)(100f * ((float)b2 / 255f));
				return new Color((int)b2, (int)b2, (int)b2, (int)a2);
			}
			return new Color(255, 255, 255, 100);
		}
	}
}