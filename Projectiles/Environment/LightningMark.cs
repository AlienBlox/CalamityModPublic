﻿using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Environment
{
    public class LightningMark : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Lightning Mark");
        }

        public override void SetDefaults()
        {
            projectile.width = 6;
            projectile.height = 12;
            projectile.hostile = true;
            projectile.penetrate = -1;
            projectile.timeLeft = 1800;
			projectile.extraUpdates = 10;
        }

        public override void AI()
        {
			if (projectile.timeLeft <= 600)
			{
				if (projectile.ai[0] == 0f && projectile.owner == Main.myPlayer)
				{
					projectile.ai[0] = 1f;

					Vector2 fireFrom = new Vector2(projectile.Center.X, projectile.Center.Y - 900f);
					int tries = 0;
					while (Framing.GetTileSafely((int)(fireFrom.X / 16f), (int)(fireFrom.Y / 16f)).active())
					{
						fireFrom.Y += 16f;

						tries += 1;
						if (tries > 25)
							return;
					}

					Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/LightningStrike"), (int)projectile.Center.X, (int)projectile.Center.Y);
					Vector2 ai0 = projectile.Center - fireFrom;
					float ai = (float)Main.rand.Next(100);
					Vector2 velocity = Vector2.Normalize(ai0.RotatedByRandom(0.78539818525314331)) * 7f;
					int proj = Projectile.NewProjectile(fireFrom.X, fireFrom.Y, velocity.X, velocity.Y, ProjectileID.CultistBossLightningOrbArc, 50, 0f, projectile.owner, ai0.ToRotation(), ai);
					Main.projectile[proj].extraUpdates += 6;
					Main.projectile[proj].friendly = true;
					Main.projectile[proj].Calamity().lineColor = 1;
				}
			}
			else if (projectile.velocity.Y == 0f)
			{
				Lighting.AddLight(projectile.Center, 0.6f, 0.9f, 1f);

				if (Main.rand.NextBool(3))
				{
					int num199 = Dust.NewDust(projectile.position, projectile.width, projectile.height, 226, 0f, 0f, 100, default, 1f);
					Main.dust[num199].position.X -= 2f;
					Main.dust[num199].position.Y += 2f;
					Main.dust[num199].scale = 0.7f;
					Main.dust[num199].noGravity = true;
					Main.dust[num199].velocity.Y -= 2f;
				}

				if (Main.rand.NextBool(15))
				{
					int num200 = Dust.NewDust(projectile.position, projectile.width, projectile.height, 31, 0f, 0f, 100, default, 1f);
					Main.dust[num200].position.X -= 2f;
					Main.dust[num200].position.Y += 2f;
					Main.dust[num200].scale = 0.7f;
					Main.dust[num200].noGravity = true;
					Main.dust[num200].velocity *= 0.1f;
				}
			}

			if (projectile.velocity.Y != projectile.velocity.Y && projectile.velocity.Y > 1f)
				projectile.velocity.Y *= -0.5f;

			projectile.velocity.Y += 0.2f;

			if (projectile.velocity.Y > 16f)
				projectile.velocity.Y = 16f;
		}

		public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough)
		{
			fallThrough = false;
			return true;
		}

		public override bool OnTileCollide(Vector2 oldVelocity)
		{
			return false;
		}
	}
}
