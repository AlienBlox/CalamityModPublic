using CalamityMod.Events;
using CalamityMod.Particles;
using CalamityMod.Projectiles.Boss;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.ExoMechs.Thanatos
{
	public class ThanatosTail : ModNPC
    {
		public static int normalIconIndex;
		public static int vulnerableIconIndex;

		internal static void LoadHeadIcons()
		{
			string normalIconPath = "CalamityMod/NPCs/ExoMechs/Thanatos/ThanatosNormalTail";
			string vulnerableIconPath = "CalamityMod/NPCs/ExoMechs/Thanatos/ThanatosVulnerableTail";

			CalamityMod.Instance.AddBossHeadTexture(normalIconPath, -1);
			normalIconIndex = ModContent.GetModBossHeadSlot(normalIconPath);

			CalamityMod.Instance.AddBossHeadTexture(vulnerableIconPath, -1);
			vulnerableIconIndex = ModContent.GetModBossHeadSlot(vulnerableIconPath);
		}

		// Whether the tail is venting heat or not, it is vulnerable to damage during venting
		private bool vulnerable = false;

		public ThanatosSmokeParticleSet SmokeDrawer = new ThanatosSmokeParticleSet(-1, 3, 0f, 16f, 1.5f);

		// Timer to prevent Thanatos from dealing contact damage for a bit
		private int noContactDamageTimer = 0;

		private const float timeToOpenAndFireLasers = 36f;

		private const float segmentCloseTimerDecrement = 0.2f;

		public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("XM-05 Thanatos");
			Main.npcFrameCount[npc.type] = 5;
		}

        public override void SetDefaults()
        {
			npc.Calamity().canBreakPlayerDefense = true;
			npc.npcSlots = 5f;
			npc.GetNPCDamage();
			npc.width = 76;
            npc.height = 110;
            npc.defense = 120;
			npc.DR_NERD(0.9999f);
			npc.Calamity().unbreakableDR = true;
			npc.LifeMaxNERB(1000000, 1150000, 500000);
			double HPBoost = CalamityConfig.Instance.BossHealthBoost * 0.01;
			npc.lifeMax += (int)(npc.lifeMax * HPBoost);
			npc.aiStyle = -1;
            aiType = -1;
            npc.knockBackResist = 0f;
			npc.Opacity = 0f;
			npc.canGhostHeal = false;
			npc.behindTiles = true;
            npc.noGravity = true;
            npc.noTileCollide = true;
            npc.DeathSound = SoundID.NPCDeath14;
            npc.netAlways = true;
            npc.dontCountMe = true;
            npc.chaseable = false;
			npc.boss = true;
			music = /*CalamityMod.Instance.GetMusicFromMusicMod("AdultEidolonWyrm") ??*/ MusicID.Boss3;
		}

		public override void BossHeadSlot(ref int index)
		{
			if (Main.npc[(int)npc.ai[2]].Calamity().newAI[1] == (float)ThanatosHead.SecondaryPhase.PassiveAndImmune)
				index = -1;
			else if (vulnerable)
				index = vulnerableIconIndex;
			else
				index = normalIconIndex;
		}

		public override void BossHeadRotation(ref float rotation)
		{
			rotation = npc.rotation;
		}

		public override void SendExtraAI(BinaryWriter writer)
		{
			writer.Write(npc.chaseable);
			writer.Write(npc.dontTakeDamage);
			writer.Write(noContactDamageTimer);
			writer.Write(vulnerable);
			writer.Write(npc.localAI[0]);
			writer.Write(npc.localAI[1]);
			writer.Write(npc.localAI[2]);
			for (int i = 0; i < 4; i++)
				writer.Write(npc.Calamity().newAI[i]);
		}

		public override void ReceiveExtraAI(BinaryReader reader)
		{
			npc.chaseable = reader.ReadBoolean();
			npc.dontTakeDamage = reader.ReadBoolean();
			noContactDamageTimer = reader.ReadInt32();
			vulnerable = reader.ReadBoolean();
			npc.localAI[0] = reader.ReadSingle();
			npc.localAI[1] = reader.ReadSingle();
			npc.localAI[2] = reader.ReadSingle();
			for (int i = 0; i < 4; i++)
				npc.Calamity().newAI[i] = reader.ReadSingle();
		}

		public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position) => false;

        public override void AI()
        {
            if (npc.ai[2] > 0f)
                npc.realLife = (int)npc.ai[2];

			if (npc.life > Main.npc[(int)npc.ai[1]].life)
				npc.life = Main.npc[(int)npc.ai[1]].life;

			// Difficulty modes
			bool malice = CalamityWorld.malice || BossRushEvent.BossRushActive;
			bool death = CalamityWorld.death || malice;
			bool revenge = CalamityWorld.revenge || malice;
			bool expertMode = Main.expertMode || malice;

			// Check if other segments are still alive, if not, die
			bool shouldDespawn = true;
			for (int i = 0; i < Main.maxNPCs; i++)
			{
				if (Main.npc[i].active && Main.npc[i].type == ModContent.NPCType<ThanatosHead>())
				{
					shouldDespawn = false;
					break;
				}
			}
			if (!shouldDespawn)
			{
				if (npc.ai[1] <= 0f)
					shouldDespawn = true;
				else if (Main.npc[(int)npc.ai[1]].life <= 0)
					shouldDespawn = true;
			}
			if (shouldDespawn)
			{
				npc.life = 0;
				npc.HitEffect(0, 10.0);
				npc.checkDead();
				npc.active = false;
				return;
			}

			// Set vulnerable to false by default
			vulnerable = false;

			CalamityGlobalNPC calamityGlobalNPC_Head = Main.npc[(int)npc.ai[2]].Calamity();

			bool invisiblePhase = calamityGlobalNPC_Head.newAI[1] == (float)ThanatosHead.SecondaryPhase.PassiveAndImmune;
			npc.dontTakeDamage = Main.npc[(int)npc.ai[2]].dontTakeDamage;
			if (!invisiblePhase)
			{
				if (Main.npc[(int)npc.ai[1]].Opacity > 0.5f)
				{
					if (noContactDamageTimer > 0)
						noContactDamageTimer--;

					npc.Opacity += 0.2f;
					if (npc.Opacity > 1f)
						npc.Opacity = 1f;
				}
				else
				{
					// Deal no contact damage for 3 seconds after becoming visible
					noContactDamageTimer = 185;
				}
			}
			else
			{
				// Deal no contact damage for 3 seconds after becoming visible
				noContactDamageTimer = 185;

				npc.Opacity -= 0.05f;
				if (npc.Opacity < 0f)
					npc.Opacity = 0f;
			}

			// Number of body segments
			int numSegments = ThanatosHead.minLength;

			// Set timer to whoAmI so that segments don't all fire lasers at the same time
			if (npc.localAI[2] == 0f)
			{
				npc.localAI[2] = npc.ai[0];
				if (npc.localAI[2] > numSegments)
					npc.localAI[2] -= numSegments;
			}

			// Percent life remaining
			float lifeRatio = npc.life / (float)npc.lifeMax;

			// Check if the other exo mechs are alive
			int otherExoMechsAlive = 0;
			if (CalamityGlobalNPC.draedonExoMechPrime != -1)
			{
				if (Main.npc[CalamityGlobalNPC.draedonExoMechPrime].active)
					otherExoMechsAlive++;
			}
			if (CalamityGlobalNPC.draedonExoMechTwinGreen != -1)
			{
				if (Main.npc[CalamityGlobalNPC.draedonExoMechTwinGreen].active)
					otherExoMechsAlive++;
			}

			// Set the AI to become more aggressive if head is berserk
			bool berserk = lifeRatio < 0.4f || (otherExoMechsAlive == 0 && lifeRatio < 0.7f);

			bool shootLasers = (calamityGlobalNPC_Head.newAI[0] == (float)ThanatosHead.Phase.Charge || calamityGlobalNPC_Head.newAI[0] == (float)ThanatosHead.Phase.UndergroundLaserBarrage) && calamityGlobalNPC_Head.newAI[2] > 0f;
			if (shootLasers && !invisiblePhase)
			{
				if (Main.netMode != NetmodeID.MultiplayerClient)
				{
					// Only charge up lasers if not venting or firing lasers
					if (npc.Calamity().newAI[0] == 0f)
						npc.ai[3] += 1f;

					double numSegmentsAbleToFire = malice ? 35D : death ? 30D : revenge ? 28D : expertMode ? 25D : 20D;
					if (berserk)
						numSegmentsAbleToFire *= 1.5;

					float segmentDivisor = (float)Math.Round(numSegments / numSegmentsAbleToFire);

					if (calamityGlobalNPC_Head.newAI[0] == (float)ThanatosHead.Phase.Charge)
					{
						float divisor = 120f;
						if ((npc.ai[3] % divisor == 0f && npc.localAI[2] % segmentDivisor == 0f) || npc.Calamity().newAI[0] > 0f)
						{
							// Body is vulnerable while firing lasers
							vulnerable = true;

							if (npc.Calamity().newAI[1] == 0f)
							{
								npc.Calamity().newAI[0] += 1f;
								if (npc.Calamity().newAI[0] >= timeToOpenAndFireLasers)
								{
									npc.ai[3] = 0f;
									npc.Calamity().newAI[1] = 1f;
									if (Main.netMode != NetmodeID.MultiplayerClient)
									{
										int maxTargets = 3;
										int[] whoAmIArray = new int[maxTargets];
										Vector2[] targetCenterArray = new Vector2[maxTargets];
										int numProjectiles = 0;
										float maxDistance = 2400f;

										for (int i = 0; i < Main.maxPlayers; i++)
										{
											if (!Main.player[i].active || Main.player[i].dead)
												continue;

											Vector2 playerCenter = Main.player[i].Center;
											float distance = Vector2.Distance(playerCenter, npc.Center);
											if (distance < maxDistance)
											{
												whoAmIArray[numProjectiles] = i;
												targetCenterArray[numProjectiles] = playerCenter;
												int projectileLimit = numProjectiles + 1;
												numProjectiles = projectileLimit;
												if (projectileLimit >= targetCenterArray.Length)
													break;
											}
										}

										SoundEffectInstance sound = Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Item, "Sounds/Item/LaserCannon"), npc.Center);
										if (sound != null)
											sound.Volume *= 0.1f;

										for (int i = 0; i < numProjectiles; i++)
										{
											// Normal laser
											int type = ModContent.ProjectileType<ThanatosLaser>();
											int damage = npc.GetProjectileDamage(type);
											Projectile.NewProjectile(npc.Center, targetCenterArray[i], type, damage, 0f, Main.myPlayer, 0f, npc.whoAmI);
										}
									}
								}
							}
							else
							{
								npc.Calamity().newAI[0] -= segmentCloseTimerDecrement;
								if (npc.Calamity().newAI[0] <= 0f)
								{
									npc.Calamity().newAI[0] = 0f;
									npc.Calamity().newAI[1] = 0f;
								}
							}
						}
					}
					else
					{
						float divisor = npc.localAI[2] * 3f; // Ranges from 3 to 300
						if ((npc.ai[3] == divisor && npc.localAI[2] % segmentDivisor == 0f) || npc.Calamity().newAI[0] > 0f)
						{
							// Body is vulnerable while firing lasers
							vulnerable = true;

							if (npc.Calamity().newAI[1] == 0f)
							{
								npc.Calamity().newAI[0] += 1f;
								if (npc.Calamity().newAI[0] >= timeToOpenAndFireLasers)
								{
									npc.ai[3] = 0f;
									npc.Calamity().newAI[1] = 1f;
									if (Main.netMode != NetmodeID.MultiplayerClient)
									{
										int maxTargets = 3;
										int[] whoAmIArray = new int[maxTargets];
										Vector2[] targetCenterArray = new Vector2[maxTargets];
										int numProjectiles = 0;
										float maxDistance = 2400f;

										for (int i = 0; i < Main.maxPlayers; i++)
										{
											if (!Main.player[i].active || Main.player[i].dead)
												continue;

											Vector2 playerCenter = Main.player[i].Center;
											float distance = Vector2.Distance(playerCenter, npc.Center);
											if (distance < maxDistance)
											{
												whoAmIArray[numProjectiles] = i;
												targetCenterArray[numProjectiles] = playerCenter;
												int projectileLimit = numProjectiles + 1;
												numProjectiles = projectileLimit;
												if (projectileLimit >= targetCenterArray.Length)
													break;
											}
										}

										float predictionAmt = malice ? 30f : death ? 20f : revenge ? 17.5f : expertMode ? 15f : 10f;
										int type = ModContent.ProjectileType<ThanatosLaser>();
										int damage = npc.GetProjectileDamage(type);
										Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Item, "Sounds/Item/LaserCannon"), npc.Center);
										for (int i = 0; i < numProjectiles; i++)
										{
											// Fire normal lasers if head is in passive state
											if (calamityGlobalNPC_Head.newAI[1] == (float)ThanatosHead.SecondaryPhase.Passive)
											{
												// Normal laser
												Projectile.NewProjectile(npc.Center, targetCenterArray[i], type, damage, 0f, Main.myPlayer, 0f, npc.whoAmI);
											}
											else
											{
												// Normal laser
												if (malice && berserk)
													Projectile.NewProjectile(npc.Center, targetCenterArray[i], type, damage, 0f, Main.myPlayer, 0f, npc.whoAmI);

												// Predictive laser
												Vector2 projectileDestination = targetCenterArray[i] + Main.player[whoAmIArray[i]].velocity * predictionAmt;
												Projectile.NewProjectile(npc.Center, projectileDestination, type, damage, 0f, Main.myPlayer, 0f, npc.whoAmI);

												// Opposite laser
												projectileDestination = targetCenterArray[i] - Main.player[whoAmIArray[i]].velocity * predictionAmt;
												Projectile.NewProjectile(npc.Center, projectileDestination, type, damage, 0f, Main.myPlayer, 0f, npc.whoAmI);
											}
										}
									}
								}
							}
							else
							{
								npc.Calamity().newAI[0] -= segmentCloseTimerDecrement;
								if (npc.Calamity().newAI[0] <= 0f)
								{
									npc.Calamity().newAI[0] = 0f;
									npc.Calamity().newAI[1] = 0f;
								}
							}
						}
					}
				}
			}
			else
			{
				if (npc.ai[3] > 0f)
					npc.ai[3] = 0f;

				// Set alternating laser-firing body segments every 3 seconds
				npc.localAI[1] += 1f;
				if (npc.localAI[1] >= 180f)
				{
					npc.localAI[1] = 0f;
					npc.localAI[2] += 1f;
					if (npc.localAI[2] > numSegments)
						npc.localAI[2] -= numSegments;
				}

				npc.Calamity().newAI[0] -= segmentCloseTimerDecrement;
				if (npc.Calamity().newAI[0] <= 0f)
				{
					npc.Calamity().newAI[0] = 0f;
					npc.Calamity().newAI[1] = 0f;
				}
				else
				{
					// Body is vulnerable while venting
					vulnerable = true;
				}
			}

			// Do not deal contact damage for 5 seconds after spawning
			if (npc.Calamity().newAI[2] == 0f)
				noContactDamageTimer = 300;

			if (npc.Calamity().newAI[2] < ThanatosHead.immunityTime)
				npc.Calamity().newAI[2] += 1f;

			// Homing only works if vulnerable is true
			npc.chaseable = vulnerable;

			// Adjust DR based on vulnerable
			npc.Calamity().DR = vulnerable ? 0f : 0.9999f;
			npc.Calamity().unbreakableDR = !vulnerable;

			// Increase overall damage taken while vulnerable
			float damageMult = malice ? 1.1f : death ? 1.2f : revenge ? 1.25f : expertMode ? 1.3f : 1.4f;
			if (berserk)
				damageMult -= malice ? 0.1f : death ? 0.175f : revenge ? 0.2f : expertMode ? 0.225f : 0.3f;

			npc.takenDamageMultiplier = vulnerable ? damageMult : 1f;

			// Vent noise and steam
			SmokeDrawer.ParticleSpawnRate = 9999999;
			if (vulnerable)
			{
				// Light
				Lighting.AddLight(npc.Center, 0.35f * npc.Opacity, 0.05f * npc.Opacity, 0.05f * npc.Opacity);

				// Noise
				float volume = calamityGlobalNPC_Head.newAI[0] == (float)ThanatosHead.Phase.Charge ? 0.1f : 1f;
				if (npc.localAI[0] == 0f)
				{
					SoundEffectInstance sound = Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/ThanatosVent"), npc.Center);
					if (sound != null)
						sound.Volume *= volume;
				}

				// Steam
				npc.localAI[0] += 1f;
				if (npc.localAI[0] < ThanatosHead.ventDuration)
				{
					SmokeDrawer.BaseMoveRotation = npc.rotation - MathHelper.PiOver2;
					SmokeDrawer.ParticleSpawnRate = ThanatosHead.ventCloudSpawnRate;
				}
			}
			else
			{
				// Light
				Lighting.AddLight(npc.Center, 0.05f * npc.Opacity, 0.2f * npc.Opacity, 0.2f * npc.Opacity);

				npc.localAI[0] = 0f;
			}

			SmokeDrawer.Update();

			Player player = Main.player[Main.npc[CalamityGlobalNPC.draedonExoMechWorm].target];

			Vector2 npcCenter = npc.Center;
			float targetCenterX = player.Center.X;
			float targetCenterY = player.Center.Y;
			targetCenterX = (int)(targetCenterX / 16f) * 16;
			targetCenterY = (int)(targetCenterY / 16f) * 16;
			npcCenter.X = (int)(npcCenter.X / 16f) * 16;
			npcCenter.Y = (int)(npcCenter.Y / 16f) * 16;
			targetCenterX -= npcCenter.X;
			targetCenterY -= npcCenter.Y;

			float distanceFromTarget = (float)Math.Sqrt(targetCenterX * targetCenterX + targetCenterY * targetCenterY);
			if (npc.ai[1] > 0f && npc.ai[1] < Main.npc.Length)
			{
				try
				{
					npcCenter = new Vector2(npc.position.X + npc.width * 0.5f, npc.position.Y + npc.height * 0.5f);
					targetCenterX = Main.npc[(int)npc.ai[1]].position.X + (Main.npc[(int)npc.ai[1]].width / 2) - npcCenter.X;
					targetCenterY = Main.npc[(int)npc.ai[1]].position.Y + (Main.npc[(int)npc.ai[1]].height / 2) - npcCenter.Y;
				}
				catch
				{
				}

				npc.rotation = (float)Math.Atan2(targetCenterY, targetCenterX) + MathHelper.PiOver2;
				distanceFromTarget = (float)Math.Sqrt(targetCenterX * targetCenterX + targetCenterY * targetCenterY);
				distanceFromTarget = (distanceFromTarget - npc.width) / distanceFromTarget;
				targetCenterX *= distanceFromTarget;
				targetCenterY *= distanceFromTarget;
				npc.velocity = Vector2.Zero;
				npc.position.X = npc.position.X + targetCenterX;
				npc.position.Y = npc.position.Y + targetCenterY;

				if (targetCenterX < 0f)
					npc.spriteDirection = -1;
				else if (targetCenterX > 0f)
					npc.spriteDirection = 1;
			}
		}

		public override bool CanHitPlayer(Player target, ref int cooldownSlot)
		{
			cooldownSlot = 1;

			Rectangle targetHitbox = target.Hitbox;

			float dist1 = Vector2.Distance(npc.Center, targetHitbox.TopLeft());
			float dist2 = Vector2.Distance(npc.Center, targetHitbox.TopRight());
			float dist3 = Vector2.Distance(npc.Center, targetHitbox.BottomLeft());
			float dist4 = Vector2.Distance(npc.Center, targetHitbox.BottomRight());

			float minDist = dist1;
			if (dist2 < minDist)
				minDist = dist2;
			if (dist3 < minDist)
				minDist = dist3;
			if (dist4 < minDist)
				minDist = dist4;

			return minDist <= 45f && npc.Opacity == 1f && noContactDamageTimer <= 0;
		}

		public override bool StrikeNPC(ref double damage, int defense, ref float knockback, int hitDirection, ref bool crit)
		{
			if (npc.Calamity().newAI[2] < ThanatosHead.immunityTime)
				damage *= 0.01;

			return !CalamityUtils.AntiButcher(npc, ref damage, 0.5f);
		}

		public override void FindFrame(int frameHeight) // 5 total frames
		{
			if (!Main.npc[(int)npc.ai[2]].active || Main.npc[(int)npc.ai[2]].life <= 0)
				return;

			// Swap between venting and non-venting frames
			CalamityGlobalNPC calamityGlobalNPC_Head = Main.npc[(int)npc.ai[2]].Calamity();
			bool invisiblePhase = calamityGlobalNPC_Head.newAI[1] == (float)ThanatosHead.SecondaryPhase.PassiveAndImmune;
			bool shootLasers = (calamityGlobalNPC_Head.newAI[0] == (float)ThanatosHead.Phase.Charge || calamityGlobalNPC_Head.newAI[0] == (float)ThanatosHead.Phase.UndergroundLaserBarrage) && calamityGlobalNPC_Head.newAI[2] > 0f;
			npc.frameCounter += 1D;
			if (npc.Calamity().newAI[0] > 0f)
			{
				if (npc.frameCounter >= 6D)
				{
					npc.frame.Y += frameHeight;
					npc.frameCounter = 0D;
				}
				int finalFrame = Main.npcFrameCount[npc.type] - 1;
				if (npc.frame.Y > frameHeight * finalFrame)
					npc.frame.Y = frameHeight * finalFrame;
			}
			else
			{
				if (npc.frameCounter >= 6D)
				{
					npc.frame.Y -= frameHeight;
					npc.frameCounter = 0D;
				}
				if (npc.frame.Y < 0)
					npc.frame.Y = 0;
			}
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Color drawColor)
		{
			SpriteEffects spriteEffects = SpriteEffects.None;
			if (npc.spriteDirection == 1)
				spriteEffects = SpriteEffects.FlipHorizontally;

			Texture2D texture = Main.npcTexture[npc.type];
			Vector2 vector = new Vector2(Main.npcTexture[npc.type].Width / 2, Main.npcTexture[npc.type].Height / Main.npcFrameCount[npc.type] / 2);

			Vector2 center = npc.Center - Main.screenPosition;
			center -= new Vector2(texture.Width, texture.Height / Main.npcFrameCount[npc.type]) * npc.scale / 2f;
			center += vector * npc.scale + new Vector2(0f, npc.gfxOffY);
			spriteBatch.Draw(texture, center, npc.frame, npc.GetAlpha(drawColor), npc.rotation, vector, npc.scale, spriteEffects, 0f);

			texture = ModContent.GetTexture("CalamityMod/NPCs/ExoMechs/Thanatos/ThanatosTailGlow");
			spriteBatch.Draw(texture, center, npc.frame, Color.White * npc.Opacity, npc.rotation, vector, npc.scale, spriteEffects, 0f);

			SmokeDrawer.DrawSet(npc.Center);

			return false;
		}

		public override bool CheckActive() => false;

		public override bool PreNPCLoot() => false;

		public override void HitEffect(int hitDirection, double damage)
		{
			if (npc.soundDelay == 0)
			{
				npc.soundDelay = 8;

				if (vulnerable)
					Main.PlaySound(mod.GetLegacySoundSlot(SoundType.NPCHit, "Sounds/NPCHit/OtherworldlyHit"), npc.Center);
				else
					Main.PlaySound(SoundID.NPCHit4, npc.Center);
			}

			int baseDust = vulnerable ? 3 : 1;
			for (int k = 0; k < baseDust; k++)
				Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 107, 0f, 0f, 100, new Color(0, 255, 255), 1f);

			if (npc.life <= 0)
			{
				for (int num193 = 0; num193 < 2; num193++)
				{
					Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 107, 0f, 0f, 100, new Color(0, 255, 255), 1.5f);
				}
				for (int num194 = 0; num194 < 20; num194++)
				{
					int num195 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 107, 0f, 0f, 0, new Color(0, 255, 255), 2.5f);
					Main.dust[num195].noGravity = true;
					Main.dust[num195].velocity *= 3f;
					num195 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 107, 0f, 0f, 100, new Color(0, 255, 255), 1.5f);
					Main.dust[num195].velocity *= 2f;
					Main.dust[num195].noGravity = true;
				}

				Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/Thanatos/ThanatosTail"), 1f);
				Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/Thanatos/ThanatosTail2"), 1f);
				Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/Thanatos/ThanatosTail3"), 1f);
				Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/Thanatos/ThanatosTail4"), 1f);
			}
		}

		public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
		{
			npc.lifeMax = (int)(npc.lifeMax * 0.8f * bossLifeScale);
			npc.damage = (int)(npc.damage * npc.GetExpertDamageMultiplier());
		}
	}
}
