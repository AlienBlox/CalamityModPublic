using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.NPCs.BrimstoneElemental;
using CalamityMod.Dusts;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.Enums;
using Terraria.GameContent.Achievements;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Melee
{
    public class CrystylCrusherRay : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Crystyl Crusher Ray");
        }

		// Use a different style for constant so it is very clear in code when a constant is used

		// The maximum charge value
		private const float MAX_CHARGE = 100f;
		//The distance charge particle from the player center
		private const float MOVE_DISTANCE = 100f;

		// The actual distance is stored in the ai0 field
		// By making a property to handle this it makes our life easier, and the accessibility more readable
		public float Distance
		{
			get => projectile.ai[0];
			set => projectile.ai[0] = value;
		}

		// The actual charge value is stored in the localAI0 field
		public float Charge
		{
			get => projectile.localAI[0];
			set => projectile.localAI[0] = value;
		}

		// Are we at max charge? With c#6 you can simply use => which indicates this is a get only property
		public bool IsAtMaxCharge => Charge == MAX_CHARGE;

		private bool playedSound = false;

		public override void SetDefaults()
		{
			projectile.width = 10;
			projectile.height = 10;
			projectile.scale = 1.5f;
			projectile.friendly = true;
			projectile.penetrate = -1;
			projectile.tileCollide = false;
			projectile.melee = true;
			projectile.hide = true;
			projectile.timeLeft = 300;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 1;
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			// We start drawing the laser if we have charged up
			if (IsAtMaxCharge)
			{
				Vector2 maxLength = Main.MouseWorld - Main.player[projectile.owner].Center;

				DrawLaser(spriteBatch, Main.projectileTexture[projectile.type], Main.player[projectile.owner].Center,
					projectile.velocity, 15f, projectile.damage, -1.57f, projectile.scale, maxLength.Length(), new Color(Main.DiscoR, 0, 255), (int)MOVE_DISTANCE);
			}
			return false;
		}

		// The core function of drawing a laser
		public void DrawLaser(SpriteBatch spriteBatch, Texture2D texture, Vector2 start, Vector2 unit, float step, int damage, float rotation = 0f, float scale = 1f, float maxDist = 2000f, Color color = default(Color), int transDist = 50)
		{
			float r = unit.ToRotation() + rotation;

			// Draws the laser 'body'
			for (float i = transDist; i <= Distance; i += step)
			{
				Color c = new Color(Main.DiscoR, 0, 255);
				var origin = start + i * unit;
				spriteBatch.Draw(texture, origin - Main.screenPosition,
					new Rectangle(0, 26, 28, 26), i < transDist ? Color.Transparent : c, r,
					new Vector2(28 * .5f, 26 * .5f), scale, 0, 0);
			}

			// Draws the laser 'tail'
			spriteBatch.Draw(texture, start + unit * (transDist - step) - Main.screenPosition,
				new Rectangle(0, 0, 28, 26), new Color(Main.DiscoR, 0, 255), r, new Vector2(28 * .5f, 26 * .5f), scale, 0, 0);

			// Draws the laser 'head'
			spriteBatch.Draw(texture, start + (Distance + step) * unit - Main.screenPosition,
				new Rectangle(0, 52, 28, 26), new Color(Main.DiscoR, 0, 255), r, new Vector2(28 * .5f, 26 * .5f), scale, 0, 0);
		}

		// Change the way of collision check of the projectile
		public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
		{
			// We can only collide if we are at max charge, which is when the laser is actually fired
			if (!IsAtMaxCharge) return false;

			Player player = Main.player[projectile.owner];
			Vector2 unit = projectile.velocity;
			float point = 0f;
			// Run an AABB versus Line check to look for collisions, look up AABB collision first to see how it works
			// It will look for collisions on the given line using AABB
			return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), player.Center,
				player.Center + unit * Distance, 22, ref point);
		}

		// The AI of the projectile
		public override void AI()
		{
			Player player = Main.player[projectile.owner];
			projectile.position = player.Center + projectile.velocity * MOVE_DISTANCE;
			projectile.timeLeft = 2;

			// By separating large AI into methods it becomes very easy to see the flow of the AI in a broader sense
			// First we update player variables that are needed to channel the laser
			// Then we run our charging laser logic
			// If we are fully charged, we proceed to update the laser's position
			// Finally we spawn some effects like dusts and light

			UpdatePlayer(player);
			ChargeLaser(player);

			// If laser is not charged yet, stop the AI here.
			if (Charge < MAX_CHARGE) return;

			//Play cool sound when fully charged
			if (playedSound == false)
			{
				Main.PlaySound(SoundID.Item68, (int)projectile.position.X, (int)projectile.position.Y);
				playedSound = true;
			}

			SetLaserPosition(player);
			SpawnDusts(player);
			CastLights();
			DestroyTiles();
		}

		private void SpawnDusts(Player player)
		{
			Vector2 unit = projectile.velocity * -1;
			Vector2 dustPos = player.Center + projectile.velocity * Distance;

			for (int i = 0; i < 2; ++i)
			{
				float num1 = projectile.velocity.ToRotation() + (Main.rand.Next(2) == 1 ? -1.0f : 1.0f) * 1.57f;
				float num2 = (float)(Main.rand.NextDouble() * 0.8f + 1.0f);
				Vector2 dustVel = new Vector2((float)Math.Cos(num1) * num2, (float)Math.Sin(num1) * num2);
				Dust dust = Main.dust[Dust.NewDust(dustPos, 0, 0, 173, dustVel.X, dustVel.Y)];
				dust.noGravity = true;
				dust.scale = 1.2f;
				dust = Dust.NewDustDirect(Main.player[projectile.owner].Center, 0, 0, 31,
					-unit.X * Distance, -unit.Y * Distance);
				dust.fadeIn = 0f;
				dust.color = new Color(Main.DiscoR, 0, 255);
				dust.noGravity = true;
				dust.scale = 0.88f;
			}

			if (Main.rand.NextBool(5))
			{
				Vector2 offset = projectile.velocity.RotatedBy(1.57f) * ((float)Main.rand.NextDouble() - 0.5f) * projectile.width;
				Dust dust = Main.dust[Dust.NewDust(dustPos + offset - Vector2.One * 4f, 8, 8, 57, 0.0f, 0.0f, 100, new Color(Main.DiscoR, 0, 255), 1.5f)];
				dust.velocity *= 0.5f;
				dust.velocity.Y = -Math.Abs(dust.velocity.Y);
				unit = dustPos - Main.player[projectile.owner].Center;
				unit.Normalize();
				dust = Main.dust[Dust.NewDust(Main.player[projectile.owner].Center + 55 * unit, 8, 8, 58, 0.0f, 0.0f, 100, new Color(Main.DiscoR, 0, 255), 1.5f)];
				dust.velocity = dust.velocity * 0.5f;
				dust.velocity.Y = -Math.Abs(dust.velocity.Y);
			}
		}

		/*
		 * Sets the end of the laser position based on where it collides with something
		 */
		private void SetLaserPosition(Player player)
		{
			Vector2 maxLength = Main.MouseWorld - Main.player[projectile.owner].Center;
			for (Distance = MOVE_DISTANCE; Distance <= maxLength.Length(); Distance += 5f)
			{
				var start = player.Center + projectile.velocity * Distance;
				if (!Collision.CanHit(player.Center, 1, 1, start, 1, 1))
				{
					Distance -= 5f;
					break;
				}
			}
		}

		private void ChargeLaser(Player player)
		{
			// Kill the projectile if the player stops channeling
			if (!player.channel)
			{
				projectile.Kill();
			}
			else
			{
				Vector2 offset = projectile.velocity;
				offset *= MOVE_DISTANCE - 20;
				Vector2 pos = player.Center + offset - new Vector2(15f, 15f);
				if (Charge < MAX_CHARGE)
				{
					Charge++;
				}
				int chargeFact = (int)(Charge / 20f);
				Vector2 dustVelocity = Vector2.UnitX * 18f;
				dustVelocity = dustVelocity.RotatedBy(projectile.rotation - 1.57f);
				Vector2 spawnPos = projectile.Center + dustVelocity;
				for (int k = 0; k < chargeFact + 1; k++)
				{
					int dustType = Main.rand.Next(3);
					switch (dustType)
					{
						case 0:
							dustType = 173;
							break;
						case 1:
							dustType = 57;
							break;
						case 2:
							dustType = 58;
							break;
						default:
							break;
					}
					Vector2 spawn = spawnPos + ((float)Main.rand.NextDouble() * 6.28f).ToRotationVector2() * (12f - chargeFact * 2);
					Dust dust = Main.dust[Dust.NewDust(pos, 20, 20, dustType, projectile.velocity.X / 2f, projectile.velocity.Y / 2f)];
					dust.velocity = Vector2.Normalize(spawnPos - spawn) * 1.5f * (10f - chargeFact * 2f) / 10f;
					dust.noGravity = true;
					dust.color = new Color(Main.DiscoR, 0, 255);
					dust.scale = Main.rand.Next(10, 20) * 0.05f;
				}
			}
		}

		private void UpdatePlayer(Player player)
		{
			// Multiplayer support here, only run this code if the client running it is the owner of the projectile
			if (projectile.owner == Main.myPlayer)
			{
				Vector2 diff = Main.MouseWorld - player.Center;
				diff.Normalize();
				projectile.velocity = diff;
				projectile.direction = Main.MouseWorld.X > player.position.X ? 1 : -1;
				projectile.netUpdate = true;
			}
			int dir = projectile.direction;
			player.ChangeDir(dir); // Set player direction to where we are shooting
			player.heldProj = projectile.whoAmI; // Update player's held projectile
			player.itemTime = 2; // Set item time to 2 frames while we are used
			player.itemAnimation = 2; // Set item animation time to 2 frames while we are used
			player.itemRotation = (float)Math.Atan2(projectile.velocity.Y * dir, projectile.velocity.X * dir); // Set the item rotation to where we are shooting
		}

		private void CastLights()
		{
			// Cast a light along the line of the laser
			DelegateMethods.v3_1 = new Vector3(0.8f, 0.8f, 1f);
			Utils.PlotTileLine(projectile.Center, projectile.Center + projectile.velocity * (Distance - MOVE_DISTANCE), 26, DelegateMethods.CastLight);
		}

		public override bool ShouldUpdatePosition() => false;

		/*
		 * Update CutTiles so the laser will cut tiles (like grass)
		 */
		public override void CutTiles()
		{
			DelegateMethods.tilecut_0 = TileCuttingContext.AttackProjectile;
			Vector2 unit = projectile.velocity;
			Utils.PlotTileLine(projectile.Center, projectile.Center + unit * Distance, projectile.width + 16, DelegateMethods.CutTiles);
		}

		private void DestroyTiles()
		{
			if (projectile.owner == Main.myPlayer)
			{
				Vector2 destroyVector = projectile.Center + projectile.velocity * (Distance - MOVE_DISTANCE);
				int num814 = 3;
				int num815 = (int)(destroyVector.X / 16f - (float)num814);
				int num816 = (int)(destroyVector.X / 16f + (float)num814);
				int num817 = (int)(destroyVector.Y / 16f - (float)num814);
				int num818 = (int)(destroyVector.Y / 16f + (float)num814);
				if (num815 < 0)
				{
					num815 = 0;
				}
				if (num816 > Main.maxTilesX)
				{
					num816 = Main.maxTilesX;
				}
				if (num817 < 0)
				{
					num817 = 0;
				}
				if (num818 > Main.maxTilesY)
				{
					num818 = Main.maxTilesY;
				}
				AchievementsHelper.CurrentlyMining = true;
				for (int num824 = num815; num824 <= num816; num824++)
				{
					for (int num825 = num817; num825 <= num818; num825++)
					{
						float num826 = Math.Abs((float)num824 - destroyVector.X / 16f);
						float num827 = Math.Abs((float)num825 - destroyVector.Y / 16f);
						double num828 = Math.Sqrt((double)(num826 * num826 + num827 * num827));
						if (num828 < (double)num814)
						{
							if (Main.tile[num824, num825] != null && Main.tile[num824, num825].active())
							{
								WorldGen.KillTile(num824, num825, false, false, false);
								DustExplosion(destroyVector);
								if (!Main.tile[num824, num825].active() && Main.netMode != NetmodeID.SinglePlayer)
								{
									NetMessage.SendData(MessageID.TileChange, -1, -1, null, 0, (float)num824, (float)num825, 0f, 0, 0, 0);
								}
							}
						}
					}
				}
				AchievementsHelper.CurrentlyMining = false;
				if (Main.netMode != NetmodeID.SinglePlayer)
				{
					NetMessage.SendData(MessageID.KillProjectile, -1, -1, null, projectile.identity, (float)projectile.owner, 0f, 0f, 0, 0, 0);
				}
			}
		}

		private void DustExplosion(Vector2 vector)
		{
			int num226 = 12;
			for (int num227 = 0; num227 < num226; num227++)
			{
				int dustType = Main.rand.Next(3);
				switch (dustType)
				{
					case 0:
						dustType = 173;
						break;
					case 1:
						dustType = 57;
						break;
					case 2:
						dustType = 58;
						break;
					default:
						break;
				}
				Vector2 vector6 = Vector2.Normalize(projectile.velocity) * new Vector2((float)projectile.width / 2f, (float)projectile.height) * 0.75f;
				vector6 = vector6.RotatedBy((double)((float)(num227 - (num226 / 2 - 1)) * 6.28318548f / (float)num226), default) + vector;
				Vector2 vector7 = vector6 - vector;
				int num228 = Dust.NewDust(vector6 + vector7, 0, 0, dustType, vector7.X * 0.5f, vector7.Y * 0.5f, 100, new Color(Main.DiscoR, 0, 255), 1f);
				Main.dust[num228].noGravity = true;
				Main.dust[num228].noLight = true;
				Main.dust[num228].velocity = vector7;
			}
		}
	}
}
