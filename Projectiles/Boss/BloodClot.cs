﻿using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Boss
{
	public class BloodClot : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Blood Clot");
		}

		public override void SetDefaults()
		{
			projectile.width = 6;
			projectile.height = 6;
			projectile.hostile = true;
			projectile.penetrate = 1;
			projectile.aiStyle = 1;
			aiType = 1;
		}

		public override void AI()
		{
			for (int num468 = 0; num468 < 2; num468++)
			{
				int num469 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 235, 0f, 0f, 100, default(Color), 1.25f);
				Main.dust[num469].noGravity = true;
				Main.dust[num469].velocity *= 0f;
			}
		}

		public override void OnHitPlayer(Player target, int damage, bool crit)
		{
			target.AddBuff(mod.BuffType("BurningBlood"), 120);
		}
	}
}