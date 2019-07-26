﻿using System;
using System.IO;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Boss
{
	public class GreatSandBlast : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Great Sand Blast");
		}

		public override void SetDefaults()
		{
			projectile.width = 10;
			projectile.height = 10;
			projectile.hostile = true;
			projectile.tileCollide = false;
			projectile.penetrate = 1;
			projectile.timeLeft = 300;
			projectile.aiStyle = 1;
			projectile.alpha = 255;
		}

		public override void SendExtraAI(BinaryWriter writer)
		{
			writer.Write(projectile.localAI[0]);
		}

		public override void ReceiveExtraAI(BinaryReader reader)
		{
			projectile.localAI[0] = reader.ReadSingle();
		}

		public override void AI()
		{
			projectile.tileCollide = projectile.timeLeft < 240;
			projectile.rotation = (float)Math.Atan2((double)projectile.velocity.Y, (double)projectile.velocity.X) + 1.57f;
			projectile.localAI[0] += 1f;
			if (projectile.localAI[0] > 4f)
			{
				projectile.alpha -= 50;
				if (projectile.alpha < 0)
				{
					projectile.alpha = 0;
				}
				int num469 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 85, 0f, 0f, 100, default(Color), 1f);
				Main.dust[num469].noGravity = true;
				Main.dust[num469].velocity *= 0f;
			}
		}

		public override void Kill(int timeLeft)
		{
			Main.PlaySound(2, (int)projectile.position.X, (int)projectile.position.Y, 14);
			projectile.position = projectile.Center;
			projectile.width = (projectile.height = 20);
			projectile.position.X = projectile.position.X - (float)(projectile.width / 2);
			projectile.position.Y = projectile.position.Y - (float)(projectile.height / 2);
			int num226 = 36;
			for (int num227 = 0; num227 < num226; num227++)
			{
				Vector2 vector6 = Vector2.Normalize(projectile.velocity) * new Vector2((float)projectile.width / 2f, (float)projectile.height) * 0.75f;
				vector6 = vector6.RotatedBy((double)((float)(num227 - (num226 / 2 - 1)) * 6.28318548f / (float)num226), default(Vector2)) + projectile.Center;
				Vector2 vector7 = vector6 - projectile.Center;
				int num228 = Dust.NewDust(vector6 + vector7, 0, 0, 85, vector7.X * 1.5f, vector7.Y * 1.5f, 100, default(Color), 1.2f);
				Main.dust[num228].noGravity = true;
				Main.dust[num228].noLight = true;
				Main.dust[num228].velocity = vector7;
			}
			projectile.Damage();
		}

		public override void OnHitPlayer(Player target, int damage, bool crit)
		{
			target.velocity.X *= 0.5f;
			target.velocity.Y *= 0.5f;
		}
	}
}
