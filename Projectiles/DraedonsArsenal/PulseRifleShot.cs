using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.DraedonsArsenal
{
    public class PulseRifleShot : ModProjectile
    {
		private int dust1 = 27; //purple
		private int dust2 = 173; //shortlived purple
		private int dust3 = 234; //cyan and pink
		private bool hasHit = false;

    	public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Pulse Bolt");
		}

        public override void SetDefaults()
        {
            projectile.width = 4;
            projectile.height = 4;
            projectile.friendly = true;
            projectile.ranged = true;
            projectile.penetrate = 1;
            projectile.extraUpdates = 100;
            projectile.timeLeft = 300;
        }

        public override void AI()
        {
			bool notSplitProjectile = projectile.ai[1] == 0f;

			Lighting.AddLight(projectile.Center, 0.3f, 0f, 0.5f);

			float createDustVar = 10f;
			projectile.localAI[0] += 1f;
			if (projectile.localAI[0] > createDustVar)
			{
				for (int num447 = 0; num447 < 2; num447++)
				{
					Vector2 vector33 = projectile.position;
					vector33 -= projectile.velocity * ((float)num447 * 0.25f);
					int num448 = Dust.NewDust(vector33, 1, 1, dust1, 0f, 0f, 0, default(Color), 1f);
					Main.dust[num448].noGravity = true;
					Main.dust[num448].position = vector33;
					Main.dust[num448].scale = (float)Main.rand.Next(70, 110) * 0.013f;

					int num449 = Dust.NewDust(vector33, 1, 1, dust3, 0f, 0f, 0, default(Color), 1f);
					Main.dust[num449].noGravity = true;
					Main.dust[num449].position = vector33;
					Main.dust[num449].scale = (float)Main.rand.Next(70, 110) * 0.013f;
				}

				if (notSplitProjectile)
				{
					Vector2 value7 = new Vector2(5f, 10f);
					Vector2 value8 = Vector2.UnitX * -12f;

					for (int num41 = 0; num41 < 2; num41++)
					{
						value8 = -Vector2.UnitY.RotatedBy((double)(24f * 0.1308997f + 0f * 3.14159274f), default(Vector2)) * value7;
						int num42 = Dust.NewDust(projectile.Center, 0, 0, dust2, 0f, 0f, 160, default(Color), 1f);
						Main.dust[num42].scale = 1.5f;
						Main.dust[num42].noGravity = true;
						Main.dust[num42].position = projectile.Center + value8;
					}

					for (int num41 = 0; num41 < 2; num41++)
					{
						value8 = -Vector2.UnitY.RotatedBy((double)(24f * 0.1308997f + 1f * 3.14159274f), default(Vector2)) * value7;
						int num42 = Dust.NewDust(projectile.Center, 0, 0, dust2, 0f, 0f, 160, default(Color), 1f);
						Main.dust[num42].scale = 1.5f;
						Main.dust[num42].noGravity = true;
						Main.dust[num42].position = projectile.Center + value8;
					}
				}
			}

			if (projectile.localAI[0] == createDustVar && notSplitProjectile)
				PulseBurst(4f, 5f);
		}

		public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
		{
			if (projectile.ai[1] < 5f && !hasHit)
			{
				hasHit = true;

				Vector2 vector35 = new Vector2(projectile.position.X + (float)projectile.width * 0.5f, projectile.position.Y + (float)projectile.height * 0.5f);
				float num474 = 600f;

				int alreadyTargetedNPCType = 0;
				if (projectile.ai[1] > 0f)
					alreadyTargetedNPCType = (int)projectile.ai[0];

				for (int i = 0; i < 200; i++)
				{
					if (Main.npc[i].CanBeChasedBy(projectile, false) && Collision.CanHit(projectile.Center, 1, 1, Main.npc[i].Center, 1, 1))
					{
						if (alreadyTargetedNPCType != Main.npc[i].whoAmI)
						{
							float num476 = Main.npc[i].position.X + (float)(Main.npc[i].width / 2);
							float num477 = Main.npc[i].position.Y + (float)(Main.npc[i].height / 2);

							float num478 = Math.Abs(projectile.position.X + (float)(projectile.width / 2) - num476) + Math.Abs(projectile.position.Y + (float)(projectile.height / 2) - num477);
							if (num478 < num474)
							{
								num474 = num478;

								float num484 = num476 - vector35.X;
								float num485 = num477 - vector35.Y;
								float num486 = (float)Math.Sqrt((double)(num484 * num484 + num485 * num485));

								num486 = 2f / num486;
								num484 *= num486;
								num485 *= num486;

								Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, num484, num485, projectile.type, projectile.damage / 2, 0f, projectile.owner, (float)target.whoAmI, projectile.ai[1] + 1f);
								break;
							}
						}
					}
				}
			}

			if (!target.boss)
			{
				if (target.HitSound != SoundID.NPCHit1 && target.HitSound != SoundID.NPCHit6 && target.HitSound != SoundID.NPCHit7 &&
					target.HitSound != SoundID.NPCHit8 && target.HitSound != SoundID.NPCHit9 && target.HitSound != SoundID.NPCHit12 &&
					target.HitSound != SoundID.NPCHit13 && target.HitSound != SoundID.NPCHit14 && target.HitSound != SoundID.NPCHit18 &&
					target.HitSound != SoundID.NPCHit19 && target.HitSound != SoundID.NPCHit20 && target.HitSound != SoundID.NPCHit21 &&
					target.HitSound != SoundID.NPCHit22 && target.HitSound != SoundID.NPCHit23 && target.HitSound != SoundID.NPCHit24 &&
					target.HitSound != SoundID.NPCHit25 && target.HitSound != SoundID.NPCHit26 && target.HitSound != SoundID.NPCHit27 &&
					target.HitSound != SoundID.NPCHit28 && target.HitSound != SoundID.NPCHit29 && target.HitSound != SoundID.NPCHit31 &&
					target.HitSound != SoundID.NPCHit32 && target.HitSound != SoundID.NPCHit33 && target.HitSound != SoundID.NPCHit35 &&
					target.HitSound != SoundID.NPCHit37 && target.HitSound != SoundID.NPCHit38 && target.HitSound != SoundID.NPCHit40 &&
					target.HitSound != SoundID.NPCHit43 && target.HitSound != SoundID.NPCHit44 && target.HitSound != SoundID.NPCHit45 &&
					target.HitSound != SoundID.NPCHit46 && target.HitSound != SoundID.NPCHit47 && target.HitSound != SoundID.NPCHit48 &&
					target.HitSound != SoundID.NPCHit50 && target.HitSound != SoundID.NPCHit51 && target.HitSound != SoundID.NPCHit55 &&
					target.HitSound != SoundID.NPCHit56 && target.HitSound != SoundID.NPCHit57)
				{
					damage += target.lifeMax / 250;
				}
			}
		}

		public override void Kill(int timeLeft)
		{
			int timesSplit = (int)projectile.ai[1];

			int height = 40;
			int totalDust = 400;
			float speed1 = 10f;
			switch (timesSplit)
			{
				case 1:
					height = 35;
					totalDust = 350;
					speed1 = 8.5f;
					break;
				case 2:
					height = 30;
					totalDust = 300;
					speed1 = 7f;
					break;
				case 3:
					height = 25;
					totalDust = 250;
					speed1 = 5.5f;
					break;
				case 4:
					height = 20;
					totalDust = 200;
					speed1 = 4f;
					break;
				case 5:
					height = 15;
					totalDust = 150;
					speed1 = 2.5f;
					break;
				default:
					break;
			}

			projectile.position = projectile.Center;
			projectile.width = (projectile.height = height);
			projectile.Center = projectile.position;
			projectile.maxPenetrate = -1;
			projectile.penetrate = -1;
			projectile.usesLocalNPCImmunity = true;
			projectile.localNPCHitCooldown = 10;
			projectile.Damage();

			Main.PlaySound(SoundID.Item93, projectile.Center);

			int fourth = totalDust / 4;
			int half = totalDust / 2;
			int x = (int)((float)totalDust * 0.625f);

			int num3;
			for (int num647 = 0; num647 < totalDust; num647 = num3 + 1)
			{
				float num648 = 2f * ((float)num647 / (float)fourth);
				int dustType = dust1;
				if (num647 > fourth)
				{
					num648 = speed1;
				}
				if (num647 > x)
				{
					num648 = speed1 * 1.3f;
					dustType = dust3;
				}

				int num650 = Dust.NewDust(projectile.Center, 6, 6, dustType, 0f, 0f, 100, default(Color), 1f);
				float num651 = Main.dust[num650].velocity.X;
				float num652 = Main.dust[num650].velocity.Y;
				if (num651 == 0f && num652 == 0f)
				{
					num651 = 1f;
				}

				float num653 = (float)Math.Sqrt((double)(num651 * num651 + num652 * num652));
				num653 = num648 / num653;
				if (num647 <= half)
				{
					num651 *= num653;
					num652 *= num653;
				}
				else
				{
					num651 = num651 * num653 * 1.25f;
					num652 = num652 * num653 * 0.75f;
				}

				Dust dust2 = Main.dust[num650];
				dust2.velocity *= 0.5f;
				dust2.velocity.X = dust2.velocity.X + num651;
				dust2.velocity.Y = dust2.velocity.Y + num652;

				if (num647 > fourth)
				{
					dust2.scale = 1.3f;
				}

				dust2.noGravity = true;

				num3 = num647;
			}
		}

		private void PulseBurst(float speed1, float speed2)
		{
			float angleRandom = 0.05f;

			int num3;
			for (int num53 = 0; num53 < 50; num53 = num3 + 1)
			{
				float dustSpeed = Main.rand.NextFloat(speed1, speed2);
				Vector2 dustVel = new Vector2(dustSpeed, 0.0f).RotatedBy(projectile.velocity.ToRotation());
				dustVel = dustVel.RotatedBy(-angleRandom);
				dustVel = dustVel.RotatedByRandom(2.0f * angleRandom);

				int num54 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, dust3, dustVel.X, dustVel.Y, 200, default(Color), 1.7f);
				Main.dust[num54].position = projectile.Center + Vector2.UnitY.RotatedByRandom(3.1415927410125732) * (float)Main.rand.NextDouble() * (float)projectile.width / 2f;
				Main.dust[num54].noGravity = true;

				Dust dust = Main.dust[num54];
				dust.velocity *= 3f;
				dust = Main.dust[num54];

				num54 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, dust1, dustVel.X, dustVel.Y, 100, default(Color), 1f);
				Main.dust[num54].position = projectile.Center + Vector2.UnitY.RotatedByRandom(3.1415927410125732) * (float)Main.rand.NextDouble() * (float)projectile.width / 2f;

				dust = Main.dust[num54];
				dust.velocity *= 2f;

				Main.dust[num54].noGravity = true;
				Main.dust[num54].fadeIn = 1f;
				Main.dust[num54].color = Color.Green * 0.5f;

				dust = Main.dust[num54];

				num3 = num53;
			}
			for (int num55 = 0; num55 < 25; num55 = num3 + 1)
			{
				float dustSpeed = Main.rand.NextFloat(speed1, speed2);
				Vector2 dustVel = new Vector2(dustSpeed, 0.0f).RotatedBy(projectile.velocity.ToRotation());
				dustVel = dustVel.RotatedBy(-angleRandom);
				dustVel = dustVel.RotatedByRandom(2.0f * angleRandom);

				int num56 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, dust2, dustVel.X, dustVel.Y, 0, default(Color), 3f);
				Main.dust[num56].position = projectile.Center + Vector2.UnitX.RotatedByRandom(3.1415927410125732).RotatedBy((double)projectile.velocity.ToRotation(), default(Vector2)) * (float)projectile.width / 3f;
				Main.dust[num56].noGravity = true;

				Dust dust = Main.dust[num56];
				dust.velocity *= 0.5f;
				dust = Main.dust[num56];

				num3 = num55;
			}
		}
	}
}
