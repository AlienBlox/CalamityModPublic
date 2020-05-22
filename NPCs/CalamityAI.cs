using CalamityMod.CalPlayer;
using CalamityMod.Dusts;
using CalamityMod.Events;
using CalamityMod.NPCs.Abyss;
using CalamityMod.NPCs.AquaticScourge;
using CalamityMod.NPCs.AstrumAureus;
using CalamityMod.NPCs.AstrumDeus;
using CalamityMod.NPCs.BrimstoneElemental;
using CalamityMod.NPCs.Bumblebirb;
using CalamityMod.NPCs.Calamitas;
using CalamityMod.NPCs.CeaselessVoid;
using CalamityMod.NPCs.Crags;
using CalamityMod.NPCs.OldDuke;
using CalamityMod.NPCs.NormalNPCs;
using CalamityMod.NPCs.SulphurousSea;
using CalamityMod.NPCs.SunkenSea;
using CalamityMod.Projectiles.Boss;
using CalamityMod.Projectiles.Enemy;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using System;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.NPCs
{
    public class CalamityAI
    {
		#region Aquatic Scourge
		public static void AquaticScourgeAI(NPC npc, Mod mod, bool head)
		{
			CalamityGlobalNPC calamityGlobalNPC = npc.Calamity();
			bool revenge = CalamityWorld.revenge || CalamityWorld.bossRushActive;
			bool death = CalamityWorld.death || CalamityWorld.bossRushActive;

			// Adjust hostility and stats
			if (npc.justHit || npc.life <= npc.lifeMax * 0.99 || CalamityWorld.bossRushActive ||
				Main.npc[(int)npc.ai[1]].life <= Main.npc[(int)npc.ai[1]].lifeMax * 0.99)
			{
				calamityGlobalNPC.newAI[0] = 1f;
				npc.damage = npc.defDamage;
				npc.boss = head;
			}
			else
				npc.damage = 0;

			// Homing only works if the boss is hostile
			if (head)
				npc.chaseable = calamityGlobalNPC.newAI[0] == 1f;

			// Set worm variable
			if (npc.ai[3] > 0f)
				npc.realLife = (int)npc.ai[3];

			// Get a target
			if (npc.target < 0 || npc.target == 255 || Main.player[npc.target].dead || !Main.player[npc.target].active)
				npc.TargetClosest(true);

			Player player = Main.player[npc.target];

			npc.velocity.Length();

			// Circular movement
			bool doSpiral = false;
			if (head && calamityGlobalNPC.newAI[0] == 1f && calamityGlobalNPC.newAI[2] == 1f && revenge)
			{
				float ai3 = 600f;
				calamityGlobalNPC.newAI[3] += 1f;
				doSpiral = calamityGlobalNPC.newAI[1] == 0f && calamityGlobalNPC.newAI[3] >= ai3;
				if (doSpiral)
				{
					int npcPosX = (int)(npc.position.X + (npc.width / 2)) / 16;
					int npcPosY = (int)(npc.position.Y + (npc.height / 2)) / 16;

					// Barf out enemies
					int variable = 10;
					if (calamityGlobalNPC.newAI[3] % 60f == 0f && npcPosX > variable && npcPosY > variable && npcPosX < Main.maxTilesX - variable && npcPosY < Main.maxTilesY - variable)
					{
						Main.PlaySound(4, (int)npc.position.X, (int)npc.position.Y, 13);

						Vector2 spawnPosition = new Vector2(npcPosX * 16, npcPosY * 16);

						if (Main.tile[npcPosX, npcPosY] != null)
						{
							if (!Main.tile[npcPosX, npcPosY].active() && Main.netMode != NetmodeID.MultiplayerClient)
							{
								if (!NPC.AnyNPCs(ModContent.NPCType<AquaticSeekerHead>()))
									NPC.NewNPC((int)spawnPosition.X, (int)spawnPosition.Y, ModContent.NPCType<AquaticSeekerHead>());
								else if (!NPC.AnyNPCs(ModContent.NPCType<AquaticUrchin>()))
									NPC.NewNPC((int)spawnPosition.X, (int)spawnPosition.Y, ModContent.NPCType<AquaticUrchin>());
								else if (NPC.CountNPCS(ModContent.NPCType<AquaticParasite>()) < 2)
									NPC.NewNPC((int)spawnPosition.X, (int)spawnPosition.Y, ModContent.NPCType<AquaticParasite>());
							}
						}
					}

					// Velocity boost
					if (calamityGlobalNPC.newAI[3] == ai3)
					{
						npc.velocity.Normalize();
						npc.velocity *= 24f;
					}

					 // Spin velocity
					float velocity = (float)(Math.PI * 2D) / 120f;
					npc.velocity = npc.velocity.RotatedBy(-(double)velocity * npc.localAI[1]);
					npc.rotation = (float)Math.Atan2(npc.velocity.Y, npc.velocity.X) + MathHelper.PiOver2;

					// Reset and charge at target
					if (calamityGlobalNPC.newAI[3] >= ai3 + 180f)
					{
						npc.TargetClosest(true);
						calamityGlobalNPC.newAI[3] = 0f;
						float chargeVelocity = death ? 16f : 12f;
						npc.velocity = Vector2.Normalize(player.Center - npc.Center) * chargeVelocity;
					}
				}
				else
					npc.localAI[1] = (npc.Center.X - player.Center.X < 0 ? 1f : -1f);
			}

			if (Main.netMode != NetmodeID.MultiplayerClient)
			{
				if (head)
				{
					// Spawn segments
					if (calamityGlobalNPC.newAI[2] == 0f && npc.ai[0] == 0f)
					{
						int maxLength = death ? 41 : 31;
						int Previous = npc.whoAmI;
						for (int num36 = 0; num36 < maxLength; num36++)
						{
							int lol;
							if (num36 >= 0 && num36 < maxLength - 1)
							{
								if (num36 % 2 == 0)
									lol = NPC.NewNPC((int)npc.position.X + (npc.width / 2), (int)npc.position.Y + (npc.height / 2), ModContent.NPCType<AquaticScourgeBody>(), npc.whoAmI);
								else
									lol = NPC.NewNPC((int)npc.position.X + (npc.width / 2), (int)npc.position.Y + (npc.height / 2), ModContent.NPCType<AquaticScourgeBodyAlt>(), npc.whoAmI);
							}
							else
								lol = NPC.NewNPC((int)npc.position.X + (npc.width / 2), (int)npc.position.Y + (npc.height / 2), ModContent.NPCType<AquaticScourgeTail>(), npc.whoAmI);

							Main.npc[lol].realLife = npc.whoAmI;
							Main.npc[lol].ai[2] = npc.whoAmI;
							Main.npc[lol].ai[1] = Previous;
							Main.npc[Previous].ai[0] = lol;
							NetMessage.SendData(23, -1, -1, null, lol, 0f, 0f, 0f, 0);
							Previous = lol;
						}
						calamityGlobalNPC.newAI[2] = 1f;
					}

					// Big barf attack
					if (calamityGlobalNPC.newAI[0] == 1f && !doSpiral)
					{
						npc.localAI[0] += 1f;
						if (player.gravDir == -1f)
							npc.localAI[0] += 2f;

						if (npc.localAI[0] >= (CalamityWorld.bossRushActive ? 300f : (revenge ? 360f : 420f)))
						{
							int npcPosX = (int)(npc.position.X + (npc.width / 2)) / 16;
							int npcPosY = (int)(npc.position.Y + (npc.height / 2)) / 16;

							if (npcPosX < 0)
								npcPosX = 0;
							if (npcPosX > Main.maxTilesX)
								npcPosX = Main.maxTilesX;
							if (npcPosY < 0)
								npcPosY = 0;
							if (npcPosY > Main.maxTilesY)
								npcPosY = Main.maxTilesY;

							if (!Main.tile[npcPosX, npcPosY].active() && Vector2.Distance(player.Center, npc.Center) > 300f)
							{
								npc.localAI[0] = 0f;
								npc.netUpdate = true;
								Main.PlaySound(4, (int)npc.position.X, (int)npc.position.Y, 13);

								if (Main.netMode != NetmodeID.MultiplayerClient)
								{
									int damageBoom = Main.expertMode ? 23 : 28;
									float velocity = revenge ? 7.5f : 6.5f;
									if (death)
										velocity += 2f;
									if (CalamityWorld.bossRushActive)
										velocity = 11f;

									int totalProjectiles = 30;
									float radians = MathHelper.TwoPi / totalProjectiles;
									for (int i = 0; i < totalProjectiles; i++)
									{
										int projectileType = Main.rand.NextBool(2) ? ModContent.ProjectileType<SandTooth>() : ModContent.ProjectileType<SandBlast>();
										if (projectileType == ModContent.ProjectileType<SandTooth>())
											damageBoom = Main.expertMode ? 25 : 30;

										Vector2 vector255 = new Vector2(0f, -velocity).RotatedBy(radians * i);
										Projectile.NewProjectile(npc.Center, vector255, projectileType, damageBoom, 0f, Main.myPlayer, 0f, 0f);
									}

									damageBoom = Main.expertMode ? 28 : 33;
									int num320 = Main.rand.Next(5, 9);
									int num3;
									for (int num321 = 0; num321 < num320; num321 = num3 + 1)
									{
										Vector2 vector15 = new Vector2(Main.rand.Next(-100, 101), Main.rand.Next(-100, 101));
										vector15.Normalize();
										vector15 *= Main.rand.Next(50, 401) * (CalamityWorld.bossRushActive ? 0.02f : 0.01f);
										if (death)
											vector15 *= 1.1f;
										Projectile.NewProjectile(npc.Center.X, npc.Center.Y, vector15.X, vector15.Y, ModContent.ProjectileType<SandPoisonCloud>(), damageBoom, 0f, Main.myPlayer, 0f, Main.rand.Next(-45, 1));
										num3 = num321;
									}
								}
							}
						}
					}
				}

				// Fire sand blasts or teeth depending on body type
				else
				{
					if (calamityGlobalNPC.newAI[0] == 1f)
					{
						if (npc.type == ModContent.NPCType<AquaticScourgeBody>())
						{
							int num = (Main.expertMode || CalamityWorld.bossRushActive) ? 4 : 3;
							npc.localAI[0] += Main.rand.Next(num);
							if (npc.localAI[0] >= Main.rand.Next(700, 10000))
							{
								npc.localAI[0] = 0f;
								npc.TargetClosest(true);
								if (Collision.CanHit(npc.position, npc.width, npc.height, player.position, player.width, player.height))
								{
									Main.PlaySound(2, (int)npc.position.X, (int)npc.position.Y, 17);
									Vector2 vector104 = new Vector2(npc.position.X + npc.width * 0.5f, npc.position.Y + (npc.height / 2));
									float num942 = player.position.X + player.width * 0.5f - vector104.X;
									float num943 = player.position.Y + player.height * 0.5f - vector104.Y;
									float num944 = (float)Math.Sqrt(num942 * num942 + num943 * num943);
									int projectileType = ModContent.ProjectileType<SandTooth>();
									int damage = Main.expertMode ? 25 : 30;
									float num941 = CalamityWorld.bossRushActive ? 7.5f : 5f;
									num944 = num941 / num944;
									num942 *= num944;
									num943 *= num944;
									vector104.X += num942 * 5f;
									vector104.Y += num943 * 5f;
									Projectile.NewProjectile(vector104.X, vector104.Y, num942, num943, projectileType, damage, 0f, Main.myPlayer, 0f, 0f);
									npc.netUpdate = true;
								}
							}
						}
						else if (npc.type == ModContent.NPCType<AquaticScourgeBodyAlt>())
						{
							npc.localAI[0] += Main.rand.Next(4);
							if (npc.localAI[0] >= Main.rand.Next(700, 10000))
							{
								npc.localAI[0] = 0f;
								npc.TargetClosest(true);
								if (Collision.CanHit(npc.position, npc.width, npc.height, player.position, player.width, player.height))
								{
									Vector2 vector104 = new Vector2(npc.position.X + npc.width * 0.5f, npc.position.Y + (npc.height / 2));
									float num942 = player.position.X + player.width * 0.5f - vector104.X;
									float num943 = player.position.Y + player.height * 0.5f - vector104.Y;
									float num944 = (float)Math.Sqrt(num942 * num942 + num943 * num943);
									int projectileType = ModContent.ProjectileType<SandBlast>();
									int damage = Main.expertMode ? 23 : 28;
									float num941 = CalamityWorld.bossRushActive ? 12f : 8f;
									num944 = num941 / num944;
									num942 *= num944;
									num943 *= num944;
									vector104.X += num942 * 5f;
									vector104.Y += num943 * 5f;
									Projectile.NewProjectile(vector104.X, vector104.Y, num942, num943, projectileType, damage, 0f, Main.myPlayer, 0f, 0f);
									npc.netUpdate = true;
								}
							}
						}
					}
				}
			}

			// Kill body and tail
			if (!head)
			{
				bool shouldDespawn = true;
				for (int i = 0; i < Main.maxNPCs; i++)
				{
					if (Main.npc[i].active && Main.npc[i].type == ModContent.NPCType<AquaticScourgeHead>())
						shouldDespawn = false;
				}
				if (!shouldDespawn)
				{
					if (npc.ai[1] > 0f)
						shouldDespawn = false;
					else if (Main.npc[(int)npc.ai[1]].life > 0)
						shouldDespawn = false;
				}
				if (shouldDespawn)
				{
					npc.life = 0;
					npc.HitEffect(0, 10.0);
					npc.checkDead();
					npc.active = false;
				}
			}

			// Despawn
			bool notOcean = player.position.Y < 800f ||
				player.position.Y > Main.worldSurface * 16.0 ||
				(player.position.X > 6400f && player.position.X < (Main.maxTilesX * 16 - 6400));

			if (player.dead || (notOcean && !CalamityWorld.bossRushActive && !player.Calamity().ZoneSulphur))
			{
				calamityGlobalNPC.newAI[1] = 1f;
				npc.TargetClosest(false);
				npc.velocity.Y += 2f;

				if (npc.position.Y > Main.worldSurface * 16.0)
					npc.velocity.Y += 2f;

				if (npc.position.Y > Main.worldSurface * 16.0)
				{
					for (int a = 0; a < Main.npc.Length; a++)
					{
						int type = Main.npc[a].type;
						if (type == ModContent.NPCType<AquaticScourgeHead>() || type == ModContent.NPCType<AquaticScourgeBody>() ||
							type == ModContent.NPCType<AquaticScourgeBodyAlt>() || type == ModContent.NPCType<AquaticScourgeTail>())
						{
							Main.npc[a].active = false;
						}
					}
				}
			}
			else
				calamityGlobalNPC.newAI[1] = 0f;

			// Change direction
			if (npc.velocity.X < 0f)
				npc.spriteDirection = -1;
			else if (npc.velocity.X > 0f)
				npc.spriteDirection = 1;

			// Alpha changes
			if (head || Main.npc[(int)npc.ai[1]].alpha < 128)
			{
				npc.alpha -= 42;
				if (npc.alpha < 0)
					npc.alpha = 0;
			}

			// Velocity and movement
			float num188 = 5f;
			float num189 = 0.08f;
			Vector2 vector18 = new Vector2(npc.position.X + npc.width * 0.5f, npc.position.Y + npc.height * 0.5f);
			float num191 = player.position.X + (player.width / 2);
			float num192 = player.position.Y + (player.height / 2);
			int num42 = -1;
			int num43 = (int)(player.Center.X / 16f);
			int num44 = (int)(player.Center.Y / 16f);

			if (head && !doSpiral)
			{
				for (int num45 = num43 - 2; num45 <= num43 + 2; num45++)
				{
					for (int num46 = num44; num46 <= num44 + 15; num46++)
					{
						if (WorldGen.SolidTile2(num45, num46))
						{
							num42 = num46;
							break;
						}
					}
					if (num42 > 0)
						break;
				}

				if (num42 > 0)
				{
					num42 *= 16;
					float num47 = num42 + (notOcean ? 800 : 400); //800
					if (calamityGlobalNPC.newAI[0] != 1f)
					{
						num192 = num47;
						if (Math.Abs(npc.Center.X - player.Center.X) < (notOcean ? 500f : 400f)) //500
						{
							if (npc.velocity.X > 0f)
								num191 = player.Center.X + (notOcean ? 600f : 480f); //600
							else
								num191 = player.Center.X - (notOcean ? 600f : 480f); //600
						}
					}
				}

				if (calamityGlobalNPC.newAI[0] == 1f)
				{
					num188 = revenge ? 15f : 13f;
					num189 = 0.16f;
					if (notOcean)
					{
						num188 = 17f;
						num189 = 0.18f;
					}
					if (player.gravDir == -1f)
					{
						num188 = 20f;
						num189 = 0.2f;
					}
					if (CalamityWorld.bossRushActive)
					{
						num188 = 22f;
						num189 = 0.24f;
					}
					if (death)
					{
						num188 += 1f;
						num189 += 0.02f;
					}
				}

				float num48 = num188 * 1.3f;
				float num49 = num188 * 0.7f;
				float num50 = npc.velocity.Length();
				if (num50 > 0f)
				{
					if (num50 > num48)
					{
						npc.velocity.Normalize();
						npc.velocity *= num48;
					}
					else if (num50 < num49)
					{
						npc.velocity.Normalize();
						npc.velocity *= num49;
					}
				}
			}

			num191 = (int)(num191 / 16f) * 16;
			num192 = (int)(num192 / 16f) * 16;
			vector18.X = (int)(vector18.X / 16f) * 16;
			vector18.Y = (int)(vector18.Y / 16f) * 16;
			num191 -= vector18.X;
			num192 -= vector18.Y;
			float num193 = (float)Math.Sqrt(num191 * num191 + num192 * num192);

			if (!head)
			{
				if (npc.ai[1] > 0f && npc.ai[1] < Main.npc.Length)
				{
					try
					{
						vector18 = new Vector2(npc.position.X + npc.width * 0.5f, npc.position.Y + npc.height * 0.5f);
						num191 = Main.npc[(int)npc.ai[1]].position.X + (Main.npc[(int)npc.ai[1]].width / 2) - vector18.X;
						num192 = Main.npc[(int)npc.ai[1]].position.Y + (Main.npc[(int)npc.ai[1]].height / 2) - vector18.Y;
					}
					catch
					{
					}
					npc.rotation = (float)Math.Atan2(num192, num191) + MathHelper.PiOver2;
					num193 = (float)Math.Sqrt(num191 * num191 + num192 * num192);
					int num194 = npc.width;
					num193 = (num193 - num194) / num193;
					num191 *= num193;
					num192 *= num193;
					npc.velocity = Vector2.Zero;
					npc.position.X = npc.position.X + num191;
					npc.position.Y = npc.position.Y + num192;
					if (num191 < 0f)
					{
						npc.spriteDirection = -1;
					}
					else if (num191 > 0f)
					{
						npc.spriteDirection = 1;
					}
				}
			}
			else if (!doSpiral)
			{
				float num196 = Math.Abs(num191);
				float num197 = Math.Abs(num192);
				float num198 = num188 / num193;
				num191 *= num198;
				num192 *= num198;

				if ((npc.velocity.X > 0f && num191 > 0f) || (npc.velocity.X < 0f && num191 < 0f) || (npc.velocity.Y > 0f && num192 > 0f) || (npc.velocity.Y < 0f && num192 < 0f))
				{
					if (npc.velocity.X < num191)
					{
						npc.velocity.X += num189;
					}
					else
					{
						if (npc.velocity.X > num191)
							npc.velocity.X -= num189;
					}
					if (npc.velocity.Y < num192)
					{
						npc.velocity.Y += num189;
					}
					else
					{
						if (npc.velocity.Y > num192)
							npc.velocity.Y -= num189;
					}
					if (Math.Abs(num192) < num188 * 0.2 && ((npc.velocity.X > 0f && num191 < 0f) || (npc.velocity.X < 0f && num191 > 0f)))
					{
						if (npc.velocity.Y > 0f)
							npc.velocity.Y += num189 * 2f;
						else
							npc.velocity.Y -= num189 * 2f;
					}
					if (Math.Abs(num191) < num188 * 0.2 && ((npc.velocity.Y > 0f && num192 < 0f) || (npc.velocity.Y < 0f && num192 > 0f)))
					{
						if (npc.velocity.X > 0f)
							npc.velocity.X += num189 * 2f;
						else
							npc.velocity.X -= num189 * 2f;
					}
				}
				else
				{
					if (num196 > num197)
					{
						if (npc.velocity.X < num191)
							npc.velocity.X += num189 * 1.1f;
						else if (npc.velocity.X > num191)
							npc.velocity.X -= num189 * 1.1f;

						if ((Math.Abs(npc.velocity.X) + Math.Abs(npc.velocity.Y)) < num188 * 0.5)
						{
							if (npc.velocity.Y > 0f)
								npc.velocity.Y += num189;
							else
								npc.velocity.Y -= num189;
						}
					}
					else
					{
						if (npc.velocity.Y < num192)
							npc.velocity.Y += num189 * 1.1f;
						else if (npc.velocity.Y > num192)
							npc.velocity.Y -= num189 * 1.1f;

						if ((Math.Abs(npc.velocity.X) + Math.Abs(npc.velocity.Y)) < num188 * 0.5)
						{
							if (npc.velocity.X > 0f)
								npc.velocity.X += num189;
							else
								npc.velocity.X -= num189;
						}
					}
				}
				npc.rotation = (float)Math.Atan2(npc.velocity.Y, npc.velocity.X) + MathHelper.PiOver2;
			}
		}
		#endregion

		#region Brimstone Elemental
		public static void BrimstoneElementalAI(NPC npc, Mod mod)
		{
			CalamityGlobalNPC calamityGlobalNPC = npc.Calamity();

			// Used for Brimling AI states
			CalamityGlobalNPC.brimstoneElemental = npc.whoAmI;

			// Emit light
			Lighting.AddLight((int)((npc.position.X + (npc.width / 2)) / 16f), (int)((npc.position.Y + (npc.height / 2)) / 16f), 1.2f, 0f, 0f);

			// Center
			Vector2 vectorCenter = npc.Center;

			// Get a target
			if (npc.target < 0 || npc.target == 255 || Main.player[npc.target].dead || !Main.player[npc.target].active)
				npc.TargetClosest(true);

			Player player = Main.player[npc.target];
			if (!player.active || player.dead || Vector2.Distance(player.Center, vectorCenter) > 5600f)
			{
				npc.TargetClosest(false);
				player = Main.player[npc.target];
				if (!player.active || player.dead || Vector2.Distance(player.Center, vectorCenter) > 5600f)
				{
					npc.rotation = npc.velocity.X * 0.04f;

					if (npc.velocity.Y > 3f)
						npc.velocity.Y = 3f;
					npc.velocity.Y -= 0.1f;
					if (npc.velocity.Y < -12f)
						npc.velocity.Y = -12f;

					if (npc.timeLeft > 60)
						npc.timeLeft = 60;

					if (npc.ai[0] != 0f)
					{
						npc.ai[0] = 0f;
						npc.ai[1] = 0f;
						npc.ai[2] = 0f;
						npc.ai[3] = 0f;
						npc.localAI[0] = 0f;
						npc.localAI[1] = 0f;
						npc.netUpdate = true;
					}
					return;
				}
			}
			else if (npc.timeLeft < 1800)
				npc.timeLeft = 1800;

			CalamityPlayer modPlayer = player.Calamity();

			// Reset defense
			npc.defense = npc.defDefense;

			// Percent life remaining
			float lifeRatio = npc.life / (float)npc.lifeMax;

			// Variables for buffing the AI
			bool provy = CalamityWorld.downedProvidence && !CalamityWorld.bossRushActive;
			bool expertMode = Main.expertMode || CalamityWorld.bossRushActive;
			bool revenge = CalamityWorld.revenge || CalamityWorld.bossRushActive;
			bool death = CalamityWorld.death || CalamityWorld.bossRushActive;
			bool calamity = modPlayer.ZoneCalamity;
			bool phase2 = (lifeRatio < 0.5f && revenge) || death;

			// Emit dust
			int dustAmt = (npc.ai[0] == 2f) ? 2 : 1;
			int size = (npc.ai[0] == 2f) ? 50 : 35;
			if (npc.ai[0] != 1f)
			{
				for (int num1011 = 0; num1011 < 2; num1011++)
				{
					if (Main.rand.Next(3) < dustAmt)
					{
						int dust = Dust.NewDust(vectorCenter - new Vector2(size), size * 2, size * 2, 235, npc.velocity.X * 0.5f, npc.velocity.Y * 0.5f, 90, default, 1.5f);
						Main.dust[dust].noGravity = true;
						Main.dust[dust].velocity *= 0.2f;
						Main.dust[dust].fadeIn = 1f;
					}
				}
			}

			// Speed while moving in phase 1
			float speed = expertMode ? 5f : 4.5f;
			if (CalamityWorld.bossRushActive)
				speed = 12f;
			else if (!calamity)
				speed = 7f;
			else if (death)
				speed = 6f;
			else if (revenge)
				speed = 5.5f;
			float speedBoost = death ? 2f : 2f * (1f - lifeRatio);
			speed += speedBoost;

			// Variables for target location relative to npc location
			float xDistance = player.Center.X - vectorCenter.X;
			float yDistance = player.Center.Y - vectorCenter.Y;
			float totalDistance = (float)Math.Sqrt(xDistance * xDistance + yDistance * yDistance);

			// Static movement towards target
			if (npc.ai[0] <= 2f || npc.ai[0] == 5f)
			{
				npc.rotation = npc.velocity.X * 0.04f;
				if (npc.ai[0] != 5 || (npc.ai[1] < 180f && npc.ai[0] == 5f))
				{
					float playerLocation = vectorCenter.X - player.Center.X;
					npc.direction = playerLocation < 0f ? 1 : -1;
					npc.spriteDirection = npc.direction;
					totalDistance = (npc.ai[0] == 5f ? speed * 0.15f : speed) / totalDistance;
					xDistance *= totalDistance;
					yDistance *= totalDistance;
					npc.velocity.X = (npc.velocity.X * 50f + xDistance) / 51f;
					npc.velocity.Y = (npc.velocity.Y * 50f + yDistance) / 51f;
				}
			}

			// Pick a location to teleport to
			if (npc.ai[0] == 0f)
			{
				npc.chaseable = true;
				if (Main.netMode != NetmodeID.MultiplayerClient)
				{
					npc.localAI[1] += 1f;
					if (npc.localAI[1] >= (CalamityWorld.bossRushActive ? 90f : 180f))
					{
						npc.TargetClosest(true);
						npc.localAI[1] = 0f;
						int timer = 0;
						int playerPosX;
						int playerPosY;
						while (true)
						{
							timer++;
							playerPosX = (int)player.Center.X / 16;
							playerPosY = (int)player.Center.Y / 16;

							int min = 12;
							int max = 16;

							if (Main.rand.NextBool(2))
								playerPosX += Main.rand.Next(min, max);
							else
								playerPosX -= Main.rand.Next(min, max);

							if (Main.rand.NextBool(2))
								playerPosY += Main.rand.Next(min, max);
							else
								playerPosY -= Main.rand.Next(min, max);

							if (!WorldGen.SolidTile(playerPosX, playerPosY))
								break;

							if (timer > 100)
								return;
						}
						npc.ai[0] = 1f;
						npc.ai[1] = playerPosX;
						npc.ai[2] = playerPosY;
						npc.netUpdate = true;
					}
				}
			}

			// Teleport to location
			else if (npc.ai[0] == 1f)
			{
				npc.chaseable = true;
				Vector2 position = new Vector2(npc.ai[1] * 16f - (npc.width / 2), npc.ai[2] * 16f - (npc.height / 2));
				for (int m = 0; m < 5; m++)
				{
					int dust = Dust.NewDust(position, npc.width, npc.height, 235, 0f, -1f, 90, default, 2f);
					Main.dust[dust].noGravity = true;
					Main.dust[dust].fadeIn = 1f;
				}
				npc.alpha += 2;
				if (npc.alpha >= 255)
				{
					if (Main.netMode != NetmodeID.MultiplayerClient && NPC.CountNPCS(ModContent.NPCType<Brimling>()) < 2 && revenge)
					{
						NPC.NewNPC((int)vectorCenter.X, (int)vectorCenter.Y, ModContent.NPCType<Brimling>(), 0, 0f, 0f, 0f, 0f, 255);
					}
					Main.PlaySound(SoundID.Item8, vectorCenter);
					npc.alpha = 255;
					npc.position = position;
					for (int n = 0; n < 15; n++)
					{
						int warpDust = Dust.NewDust(npc.position, npc.width, npc.height, 235, 0f, -1f, 90, default, 3f);
						Main.dust[warpDust].noGravity = true;
					}
					npc.ai[0] = 2f;
					npc.netUpdate = true;
				}
			}

			// Either teleport again or go to next AI state
			else if (npc.ai[0] == 2f)
			{
				npc.alpha -= 50;
				if (npc.alpha <= 0)
				{
					npc.chaseable = true;
					npc.ai[3] += 1f;
					npc.alpha = 0;
					if (npc.ai[3] >= 2f || phase2)
					{
						npc.ai[0] = (phase2 ? 5f : 3f);
						npc.ai[1] = 0f;
						npc.ai[2] = 0f;
						npc.ai[3] = 0f;
					}
					else
					{
						npc.ai[0] = 0f;
					}
					npc.netUpdate = true;
				}
			}

			// Float above target and fire projectiles
			else if (npc.ai[0] == 3f)
			{
				npc.chaseable = true;
				npc.rotation = npc.velocity.X * 0.04f;
				float playerLocation = vectorCenter.X - player.Center.X;
				npc.direction = playerLocation < 0f ? 1 : -1;
				npc.spriteDirection = npc.direction;
				npc.ai[1] += 1f;

				bool shootProjectile = false;
				if (lifeRatio < 0.1f || CalamityWorld.bossRushActive)
				{
					if (npc.ai[1] % 30f == 29f)
						shootProjectile = true;
				}
				else if (lifeRatio < 0.5f || death)
				{
					if (npc.ai[1] % 35f == 34f)
						shootProjectile = true;
				}
				else if (npc.ai[1] % 40f == 39f)
					shootProjectile = true;

				if (shootProjectile)
				{
					if (Main.netMode != NetmodeID.MultiplayerClient)
					{
						float num742 = CalamityWorld.bossRushActive ? 6f : 4f;
						float num743 = player.position.X + player.width * 0.5f - vectorCenter.X;
						float num744 = player.position.Y + player.height * 0.5f - vectorCenter.Y;
						float num745 = (float)Math.Sqrt(num743 * num743 + num744 * num744);

						num745 = num742 / num745;
						num743 *= num745;
						num744 *= num745;
						vectorCenter.X += num743 * 3f;
						vectorCenter.Y += num744 * 3f;

						int damage = expertMode ? 25 : 30;
						int numProj = 4;
						int spread = 45;
						float rotation = MathHelper.ToRadians(spread);
						float baseSpeed = (float)Math.Sqrt(num743 * num743 + num744 * num744);
						double startAngle = Math.Atan2(num743, num744) - rotation / 2;
						double deltaAngle = rotation / numProj;
						double offsetAngle;

						for (int i = 0; i < numProj; i++)
						{
							offsetAngle = startAngle + deltaAngle * i;
							Projectile.NewProjectile(vectorCenter.X, vectorCenter.Y, baseSpeed * (float)Math.Sin(offsetAngle), baseSpeed * (float)Math.Cos(offsetAngle), ModContent.ProjectileType<BrimstoneBarrage>(), damage + (provy ? 30 : 0), 0f, Main.myPlayer, 1f, 0f);
						}

						float projectileSpeed = CalamityWorld.bossRushActive ? 7f : 5f;
						if (calamityGlobalNPC.enraged > 0 || (CalamityMod.CalamityConfig.BossRushXerocCurse && CalamityWorld.bossRushActive))
							projectileSpeed += 4f;
						if (revenge)
							projectileSpeed += 1f;
						if (!calamity)
							projectileSpeed += 2f;
						float projectileSpeedBoost = death ? 3f : 3f * (1f - lifeRatio);
						projectileSpeed += projectileSpeedBoost;

						vectorCenter = npc.Center;
						float relativeSpeedX = player.position.X + player.width * 0.5f - vectorCenter.X;
						float relativeSpeedY = player.position.Y + player.height * 0.5f - vectorCenter.Y;
						float totalRelativeSpeed = (float)Math.Sqrt(relativeSpeedX * relativeSpeedX + relativeSpeedY * relativeSpeedY);
						totalRelativeSpeed = projectileSpeed / totalRelativeSpeed;
						relativeSpeedX *= totalRelativeSpeed;
						relativeSpeedY *= totalRelativeSpeed;
						vectorCenter.X += relativeSpeedX * 3f;
						vectorCenter.Y += relativeSpeedY * 3f;
						int projectileDamage = expertMode ? 28 : 35;
						int projectileType = ModContent.ProjectileType<BrimstoneHellfireball>();
						int projectileShot = Projectile.NewProjectile(vectorCenter.X, vectorCenter.Y, relativeSpeedX, relativeSpeedY, projectileType, projectileDamage + (provy ? 30 : 0), 0f, Main.myPlayer, 0f, 0f);
						Main.projectile[projectileShot].timeLeft = 240;
					}
				}

				if (npc.position.Y > player.position.Y - 150f) //200
				{
					if (npc.velocity.Y > 0f)
						npc.velocity.Y *= 0.98f;

					npc.velocity.Y -= (CalamityWorld.bossRushActive ? 0.15f : 0.1f);

					if (npc.velocity.Y > 3f)
						npc.velocity.Y = 3f;
				}
				else if (npc.position.Y < player.position.Y - 350f) //500
				{
					if (npc.velocity.Y < 0f)
						npc.velocity.Y *= 0.98f;

					npc.velocity.Y += (CalamityWorld.bossRushActive ? 0.15f : 0.1f);

					if (npc.velocity.Y < -3f)
						npc.velocity.Y = -3f;
				}
				if (npc.position.X + (npc.width / 2) > player.position.X + (player.width / 2) + 150f) //100
				{
					if (npc.velocity.X > 0f)
						npc.velocity.X *= 0.985f;

					npc.velocity.X -= (CalamityWorld.bossRushActive ? 0.15f : 0.1f);

					if (npc.velocity.X > 8f)
						npc.velocity.X = 8f;
				}
				if (npc.position.X + (npc.width / 2) < player.position.X + (player.width / 2) - 150f) //100
				{
					if (npc.velocity.X < 0f)
						npc.velocity.X *= 0.985f;

					npc.velocity.X += (CalamityWorld.bossRushActive ? 0.15f : 0.1f);

					if (npc.velocity.X < -8f)
						npc.velocity.X = -8f;
				}

				if (npc.ai[1] >= 300f)
				{
					npc.TargetClosest(true);
					npc.ai[0] = 4f;
					npc.ai[1] = 0f;
					npc.ai[2] = 0f;
					npc.ai[3] = 0f;
					npc.netUpdate = true;
				}
			}

			// Cocoon bullet hell
			else if (npc.ai[0] == 4f)
			{
				npc.defense = 99999;
				npc.chaseable = false;
				if (Main.netMode != NetmodeID.MultiplayerClient)
				{
					float shootBoost = death ? 2f : 2f * (1f - lifeRatio);
					npc.localAI[0] += 1f + shootBoost;
					if (calamityGlobalNPC.enraged > 0 || (CalamityMod.CalamityConfig.BossRushXerocCurse && CalamityWorld.bossRushActive))
						npc.localAI[0] += 2f;
					if (death || !calamity)
						npc.localAI[0] += 1f;

					if (npc.localAI[0] >= 120f)
					{
						npc.localAI[0] = 0f;

						float projectileSpeed = revenge ? 8f : 6f;
						if (CalamityWorld.bossRushActive)
							projectileSpeed = 12f;

						int damage = expertMode ? 25 : 30;

						vectorCenter = player.Center - vectorCenter;

						float radialOffset = 0.2f;
						float diameter = 80f;

						vectorCenter.Y -= Math.Abs(vectorCenter.X) * radialOffset;
						vectorCenter = Vector2.Normalize(vectorCenter) * projectileSpeed;

						Vector2 velocity = vectorCenter;
						velocity.Normalize();
						velocity *= diameter;

						int totalProjectiles = 6;
						float offsetAngle = (float)Math.PI * radialOffset;
						for (int j = 0; j < totalProjectiles; j++)
						{
							float radians = j - (totalProjectiles - 1f) / 2f;
							Vector2 offset = velocity.RotatedBy(offsetAngle * radians, default);
							int proj = Projectile.NewProjectile(npc.Center + offset, vectorCenter, ModContent.ProjectileType<BrimstoneHellblast>(), damage + (provy ? 30 : 0), 0f, Main.myPlayer, 1f, 0f);
							Main.projectile[proj].timeLeft = 300;
							Main.projectile[proj].tileCollide = false;
						}

						totalProjectiles = 12;
						float radians2 = MathHelper.TwoPi / totalProjectiles;
						for (int k = 0; k < totalProjectiles; k++)
						{
							Vector2 vector255 = new Vector2(0f, -projectileSpeed).RotatedBy(radians2 * k);
							Projectile.NewProjectile(npc.Center, vector255, ModContent.ProjectileType<BrimstoneBarrage>(), damage + (provy ? 30 : 0), 0f, Main.myPlayer, 1f, 0f);
						}
					}
				}

				npc.velocity *= 0.95f;
				npc.rotation = npc.velocity.X * 0.04f;
				float playerLocation = vectorCenter.X - player.Center.X;
				npc.direction = playerLocation < 0f ? 1 : -1;
				npc.spriteDirection = npc.direction;

				npc.ai[1] += 1f;
				if (npc.ai[1] >= 300f)
				{
					npc.TargetClosest(true);
					npc.ai[0] = 0f;
					npc.ai[1] = 0f;
					npc.ai[2] = 0f;
					npc.ai[3] = 0f;
					npc.netUpdate = true;
				}
			}

			// Laser beam attack
			else if (npc.ai[0] == 5f)
			{
				npc.defense = npc.defDefense * 3;

				Vector2 source = new Vector2(vectorCenter.X + (npc.spriteDirection > 0 ? 34f : -34f), vectorCenter.Y - 74f);
				Vector2 aimAt = player.Center + player.velocity * 20f;
				float aimResponsiveness = 0.4f;

				switch ((int)npc.ai[2])
				{
					case 0:
						break;
					case 1:
						aimResponsiveness = 0.25f;
						break;
					case 2:
						aimResponsiveness = 0.1f;
						break;
				}

				Vector2 aimVector = Vector2.Normalize(aimAt - source);
				if (aimVector.HasNaNs())
					aimVector = -Vector2.UnitY;
				aimVector = Vector2.Normalize(Vector2.Lerp(aimVector, Vector2.Normalize(npc.velocity), aimResponsiveness));
				aimVector *= 6f;

				Vector2 laserVelocity = Vector2.Normalize(aimVector);
				if (laserVelocity.HasNaNs())
					laserVelocity = -Vector2.UnitY;

				calamityGlobalNPC.newAI[1] = laserVelocity.X;
				calamityGlobalNPC.newAI[2] = laserVelocity.Y;

				npc.ai[1] += 1f;
				if (npc.ai[1] >= 300f)
				{
					npc.TargetClosest(true);
					npc.ai[2] += 1f;
					npc.localAI[0] = 0f;
					npc.localAI[1] = 0f;
					if (npc.ai[2] >= 3f)
					{
						npc.ai[0] = 3f;
						npc.ai[1] = 0f;
						npc.ai[2] = 0f;
						calamityGlobalNPC.newAI[0] = 0f;
					}
					else
					{
						npc.ai[1] = 0f;
						calamityGlobalNPC.newAI[0] = 0f;
					}
				}
				else if (npc.ai[1] >= 180f)
				{
					npc.velocity *= 0.95f;
					if (Main.netMode != NetmodeID.MultiplayerClient)
					{
						if (npc.ai[1] == 180f)
						{
							Main.PlaySound(29, (int)npc.position.X, (int)npc.position.Y, 104);
							Vector2 laserVelocity2 = new Vector2(npc.localAI[0], npc.localAI[1]);
							laserVelocity2.Normalize();
							Projectile.NewProjectile(source, laserVelocity2, ModContent.ProjectileType<BrimstoneRay>(), 40 + (provy ? 30 : 0), 0f, Main.myPlayer, 0f, npc.whoAmI);
						}
					}
				}
				else
				{
					float playSoundTimer = 20f;
					if (npc.ai[1] < 150f)
					{
						switch ((int)npc.ai[2])
						{
							case 0:
								break;
							case 1:
								npc.ai[1] += 0.5f;
								playSoundTimer = 30f;
								break;
							case 2:
								npc.ai[1] += 1f;
								playSoundTimer = 40f;
								break;
						}
					}

					if (npc.ai[1] % playSoundTimer == 0f)
						Main.PlaySound(2, (int)npc.position.X, (int)npc.position.Y, 20);

					if (Main.netMode != NetmodeID.MultiplayerClient)
					{
						if (npc.ai[1] < 150f && calamityGlobalNPC.newAI[0] == 0f)
						{
							Projectile.NewProjectile(source, laserVelocity, ModContent.ProjectileType<BrimstoneTargetRay>(), 0, 0f, Main.myPlayer, 0f, npc.whoAmI);
							calamityGlobalNPC.newAI[0] = 1f;
						}
						else
						{
							if (npc.ai[1] == 150f)
							{
								npc.localAI[0] = laserVelocity.X;
								npc.localAI[1] = laserVelocity.Y;
								Projectile.NewProjectile(source.X, source.Y, npc.localAI[0], npc.localAI[1], ModContent.ProjectileType<BrimstoneTargetRay>(), 0, 0f, Main.myPlayer, 1f, npc.whoAmI);
							}
						}
					}
				}
			}
		}
		#endregion

		#region Calamitas Clone
		public static void CalamitasCloneAI(NPC npc, Mod mod, bool phase2)
		{
			CalamityGlobalNPC calamityGlobalNPC = npc.Calamity();

			// Emit light
			Lighting.AddLight((int)((npc.position.X + (npc.width / 2)) / 16f), (int)((npc.position.Y + (npc.height / 2)) / 16f), 1f, 0f, 0f);

			// Percent life remaining
			float lifeRatio = npc.life / (float)npc.lifeMax;

			// Spawn phase 2 Cal
			if (lifeRatio <= 0.75f && Main.netMode != NetmodeID.MultiplayerClient && !phase2)
			{
				NPC.NewNPC((int)npc.Center.X, (int)npc.position.Y + npc.height, ModContent.NPCType<CalamitasRun3>(), npc.whoAmI, 0f, 0f, 0f, 0f, 255);
				string key = "Mods.CalamityMod.CalamitasBossText";
				Color messageColor = Color.Orange;
				if (Main.netMode == NetmodeID.SinglePlayer)
				{
					Main.NewText(Language.GetTextValue(key), messageColor);
				}
				else if (Main.netMode == NetmodeID.Server)
				{
					NetMessage.BroadcastChatMessage(NetworkText.FromKey(key), messageColor);
				}
				npc.active = false;
				npc.netUpdate = true;
				return;
			}

			// Variables for increasing difficulty
			bool death = CalamityWorld.death || CalamityWorld.bossRushActive;
			bool revenge = CalamityWorld.revenge || CalamityWorld.bossRushActive;
			bool expertMode = Main.expertMode || CalamityWorld.bossRushActive;
			bool dayTime = Main.dayTime && !CalamityWorld.bossRushActive;
			bool provy = CalamityWorld.downedProvidence && !CalamityWorld.bossRushActive;

			// Variable for live brothers
			bool brotherAlive = false;

			if (phase2)
			{
				// For seekers
				CalamityGlobalNPC.calamitas = npc.whoAmI;

				// Seeker ring
				if (calamityGlobalNPC.newAI[1] == 0f && lifeRatio <= 0.35f)
				{
					if (Main.netMode != NetmodeID.MultiplayerClient)
					{
						Main.PlaySound(2, (int)npc.position.X, (int)npc.position.Y, 74);
						for (int I = 0; I < 5; I++)
						{
							int FireEye = NPC.NewNPC((int)(npc.Center.X + (Math.Sin(I * 72) * 150)), (int)(npc.Center.Y + (Math.Cos(I * 72) * 150)), ModContent.NPCType<SoulSeeker>(), npc.whoAmI, 0, 0, 0, -1);
							NPC Eye = Main.npc[FireEye];
							Eye.ai[0] = I * 72;
						}
					}

					string key = "Mods.CalamityMod.CalamitasBossText3";
					Color messageColor = Color.Orange;
					if (Main.netMode == NetmodeID.SinglePlayer)
						Main.NewText(Language.GetTextValue(key), messageColor);
					else if (Main.netMode == NetmodeID.Server)
						NetMessage.BroadcastChatMessage(NetworkText.FromKey(key), messageColor);

					calamityGlobalNPC.newAI[1] = 1f;
				}

				// Spawn brothers
				if (calamityGlobalNPC.newAI[0] == 0f && npc.life > 0)
					calamityGlobalNPC.newAI[0] = npc.lifeMax;

				if (npc.life > 0)
				{
					if (Main.netMode != NetmodeID.MultiplayerClient)
					{
						int num660 = (int)(npc.lifeMax * 0.3); //70%, 40%, and 10%
						if ((npc.life + num660) < calamityGlobalNPC.newAI[0])
						{
							calamityGlobalNPC.newAI[0] = npc.life;
							if (calamityGlobalNPC.newAI[0] <= (float)npc.lifeMax * 0.1)
							{
								NPC.NewNPC((int)npc.Center.X, (int)npc.position.Y + npc.height, ModContent.NPCType<CalamitasRun>(), npc.whoAmI, 0f, 0f, 0f, 0f, 255);
								NPC.NewNPC((int)npc.Center.X, (int)npc.position.Y + npc.height, ModContent.NPCType<CalamitasRun2>(), npc.whoAmI, 0f, 0f, 0f, 0f, 255);

								string key = "Mods.CalamityMod.CalamitasBossText2";
								Color messageColor = Color.Orange;
								if (Main.netMode == NetmodeID.SinglePlayer)
									Main.NewText(Language.GetTextValue(key), messageColor);
								else if (Main.netMode == NetmodeID.Server)
									NetMessage.BroadcastChatMessage(NetworkText.FromKey(key), messageColor);
							}
							else if (calamityGlobalNPC.newAI[0] <= (float)npc.lifeMax * 0.4)
								NPC.NewNPC((int)npc.Center.X, (int)npc.position.Y + npc.height, ModContent.NPCType<CalamitasRun2>(), npc.whoAmI, 0f, 0f, 0f, 0f, 255);
							else
								NPC.NewNPC((int)npc.Center.X, (int)npc.position.Y + npc.height, ModContent.NPCType<CalamitasRun>(), npc.whoAmI, 0f, 0f, 0f, 0f, 255);
						}
					}
				}

				// Huge defense boost if brothers are alive
				int num568 = 0;
				if (expertMode)
				{
					if (CalamityGlobalNPC.cataclysm != -1)
					{
						if (Main.npc[CalamityGlobalNPC.cataclysm].active)
						{
							brotherAlive = true;
							num568 += 255;
						}
					}
					if (CalamityGlobalNPC.catastrophe != -1)
					{
						if (Main.npc[CalamityGlobalNPC.catastrophe].active)
						{
							brotherAlive = true;
							num568 += 255;
						}
					}
					npc.defense += num568 * 50;
					if (!brotherAlive)
						npc.defense = provy ? 150 : 25;
				}

				// Disable homing if brothers are alive
				npc.chaseable = !brotherAlive;
			}

			// Get a target
			if (npc.target < 0 || npc.target == 255 || Main.player[npc.target].dead || !Main.player[npc.target].active)
				npc.TargetClosest(true);

			// Target variable
			Player player = Main.player[npc.target];

			// Rotation
			float num801 = npc.position.X + (npc.width / 2) - player.position.X - (player.width / 2);
			float num802 = npc.position.Y + npc.height - 59f - player.position.Y - (player.height / 2);
			float num803 = (float)Math.Atan2(num802, num801) + MathHelper.PiOver2;
			if (num803 < 0f)
				num803 += MathHelper.TwoPi;
			else if (num803 > MathHelper.TwoPi)
				num803 -= MathHelper.TwoPi;

			float num804 = 0.1f;
			if (npc.rotation < num803)
			{
				if ((num803 - npc.rotation) > MathHelper.Pi)
					npc.rotation -= num804;
				else
					npc.rotation += num804;
			}
			else if (npc.rotation > num803)
			{
				if ((npc.rotation - num803) > MathHelper.Pi)
					npc.rotation += num804;
				else
					npc.rotation -= num804;
			}

			if (npc.rotation > num803 - num804 && npc.rotation < num803 + num804)
				npc.rotation = num803;
			if (npc.rotation < 0f)
				npc.rotation += MathHelper.TwoPi;
			else if (npc.rotation > MathHelper.TwoPi)
				npc.rotation -= MathHelper.TwoPi;
			if (npc.rotation > num803 - num804 && npc.rotation < num803 + num804)
				npc.rotation = num803;

			// Despawn
			if (!player.active || player.dead || (dayTime && !Main.eclipse))
			{
				npc.TargetClosest(false);
				player = Main.player[npc.target];
				if (!player.active || player.dead || (dayTime && !Main.eclipse))
				{
					if (npc.velocity.Y > 3f)
						npc.velocity.Y = 3f;
					npc.velocity.Y -= 0.1f;
					if (npc.velocity.Y < -12f)
						npc.velocity.Y = -12f;

					if (npc.timeLeft > 60)
						npc.timeLeft = 60;

					if (npc.ai[1] != 0f)
					{
						npc.ai[1] = 0f;
						npc.ai[2] = 0f;
						npc.netUpdate = true;
					}
					return;
				}
			}
			else if (npc.timeLeft < 1800)
				npc.timeLeft = 1800;

			// Float above target and fire lasers or fireballs
			if (npc.ai[1] == 0f)
			{
				float num823 = expertMode ? 9.5f : 8f;
				float num824 = expertMode ? 0.175f : 0.15f;
				if (phase2)
				{
					num823 = expertMode ? 10f : 8.5f;
					num824 = expertMode ? 0.18f : 0.155f;
				}
				if (death)
				{
					num823 += 1f;
					num824 += 0.02f;
				}
				if (provy)
				{
					num823 *= 1.25f;
					num824 *= 1.25f;
				}
				if (CalamityWorld.bossRushActive)
				{
					num823 *= 1.5f;
					num824 *= 1.5f;
				}

				Vector2 vector82 = new Vector2(npc.position.X + npc.width * 0.5f, npc.position.Y + npc.height * 0.5f);
				float num825 = player.position.X + (player.width / 2) - vector82.X;
				float num826 = player.position.Y + (player.height / 2) - ((CalamityWorld.bossRushActive ? 400f : 300f) + (phase2 ? 60f : 0f)) - vector82.Y;
				float num827 = (float)Math.Sqrt(num825 * num825 + num826 * num826);
				num827 = num823 / num827;
				num825 *= num827;
				num826 *= num827;

				if (npc.velocity.X < num825)
				{
					npc.velocity.X += num824;
					if (npc.velocity.X < 0f && num825 > 0f)
						npc.velocity.X += num824;
				}
				else if (npc.velocity.X > num825)
				{
					npc.velocity.X -= num824;
					if (npc.velocity.X > 0f && num825 < 0f)
						npc.velocity.X -= num824;
				}
				if (npc.velocity.Y < num826)
				{
					npc.velocity.Y += num824;
					if (npc.velocity.Y < 0f && num826 > 0f)
						npc.velocity.Y += num824;
				}
				else if (npc.velocity.Y > num826)
				{
					npc.velocity.Y -= num824;
					if (npc.velocity.Y > 0f && num826 < 0f)
						npc.velocity.Y -= num824;
				}

				npc.ai[2] += 1f;
				if (npc.ai[2] >= (phase2 ? 200f : 300f))
				{
					npc.ai[1] = 1f;
					npc.ai[2] = 0f;
					npc.TargetClosest(true);
					npc.netUpdate = true;
				}

				num825 = player.position.X + (player.width / 2) - vector82.X;
				num826 = player.position.Y + (player.height / 2) - vector82.Y;

				if (Main.netMode != NetmodeID.MultiplayerClient)
				{
					npc.localAI[1] += 1f;
					if (phase2)
					{
						if (!brotherAlive)
						{
							float shootBoost = death ? 1f : 1f * (1f - lifeRatio);
							npc.localAI[1] += shootBoost;
							if (revenge)
								npc.localAI[1] += 0.5f;
						}

						if (npc.localAI[1] > 180f)
						{
							npc.localAI[1] = 0f;
							float num828 = CalamityWorld.bossRushActive ? 16f : (expertMode ? 14f : 12.5f);
							if (calamityGlobalNPC.enraged > 0 || (CalamityMod.CalamityConfig.BossRushXerocCurse && CalamityWorld.bossRushActive))
								num828 += 5f;

							int num829 = expertMode ? 34 : 42;
							int num830 = ModContent.ProjectileType<BrimstoneHellfireball>();
							num827 = (float)Math.Sqrt(num825 * num825 + num826 * num826);
							num827 = num828 / num827;
							num825 *= num827;
							num826 *= num827;
							vector82.X += num825 * 6f;
							vector82.Y += num826 * 6f;
							if (!Collision.CanHit(npc.position, npc.width, npc.height, player.position, player.width, player.height))
							{
								int proj = Projectile.NewProjectile(vector82.X, vector82.Y, num825, num826, num830, num829 + (provy ? 30 : 0), 0f, Main.myPlayer, player.Center.X, player.Center.Y);
								Main.projectile[proj].tileCollide = false;
							}
							else
								Projectile.NewProjectile(vector82.X, vector82.Y, num825, num826, num830, num829 + (provy ? 30 : 0), 0f, Main.myPlayer, 0f, 0f);
						}
					}
					else
					{
						if (revenge)
							npc.localAI[1] += 0.5f;

						if (npc.localAI[1] > 180f && Collision.CanHit(npc.position, npc.width, npc.height, player.position, player.width, player.height))
						{
							npc.localAI[1] = 0f;
							float num828 = CalamityWorld.bossRushActive ? 16f : (expertMode ? 13f : 10.5f);
							int num829 = expertMode ? 28 : 35;
							int num830 = ModContent.ProjectileType<BrimstoneLaser>();
							num827 = (float)Math.Sqrt(num825 * num825 + num826 * num826);
							num827 = num828 / num827;
							num825 *= num827;
							num826 *= num827;
							vector82.X += num825 * 12f;
							vector82.Y += num826 * 12f;
							Projectile.NewProjectile(vector82.X, vector82.Y, num825, num826, num830, num829 + (provy ? 30 : 0), 0f, Main.myPlayer, 0f, 0f);
						}
					}
				}
			}

			// Float to the side of the target and fire lasers
			else if (npc.ai[1] == 1f)
			{
				int num831 = 1;
				if (npc.position.X + (npc.width / 2) < player.position.X + player.width)
					num831 = -1;

				float num832 = expertMode ? 9.5f : 8f;
				float num833 = expertMode ? 0.25f : 0.2f;
				if (phase2)
				{
					num832 = expertMode ? 10f : 8.5f;
					num833 = expertMode ? 0.255f : 0.205f;
				}
				if (death)
				{
					num832 += 1f;
					num833 += 0.02f;
				}
				if (provy)
				{
					num832 *= 1.25f;
					num833 *= 1.25f;
				}
				if (CalamityWorld.bossRushActive)
				{
					num832 *= 1.5f;
					num833 *= 1.5f;
				}

				Vector2 vector83 = new Vector2(npc.position.X + npc.width * 0.5f, npc.position.Y + npc.height * 0.5f);
				float num834 = player.position.X + (player.width / 2) + (num831 * (CalamityWorld.bossRushActive ? 460 : 360)) - vector83.X;
				float num835 = player.position.Y + (player.height / 2) - vector83.Y;
				float num836 = (float)Math.Sqrt(num834 * num834 + num835 * num835);
				num836 = num832 / num836;
				num834 *= num836;
				num835 *= num836;

				if (npc.velocity.X < num834)
				{
					npc.velocity.X += num833;
					if (npc.velocity.X < 0f && num834 > 0f)
						npc.velocity.X += num833;
				}
				else if (npc.velocity.X > num834)
				{
					npc.velocity.X -= num833;
					if (npc.velocity.X > 0f && num834 < 0f)
						npc.velocity.X -= num833;
				}
				if (npc.velocity.Y < num835)
				{
					npc.velocity.Y += num833;
					if (npc.velocity.Y < 0f && num835 > 0f)
						npc.velocity.Y += num833;
				}
				else if (npc.velocity.Y > num835)
				{
					npc.velocity.Y -= num833;
					if (npc.velocity.Y > 0f && num835 < 0f)
						npc.velocity.Y -= num833;
				}

				num834 = player.position.X + (player.width / 2) - vector83.X;
				num835 = player.position.Y + (player.height / 2) - vector83.Y;

				if (Main.netMode != NetmodeID.MultiplayerClient)
				{
					npc.localAI[1] += 1f;
					if (phase2)
					{
						if (!brotherAlive)
						{
							if (revenge)
								npc.localAI[1] += 0.5f;
							if (calamityGlobalNPC.enraged > 0 || (CalamityMod.CalamityConfig.BossRushXerocCurse && CalamityWorld.bossRushActive))
								npc.localAI[1] += 0.5f;
							if (expertMode)
								npc.localAI[1] += 0.5f;
						}

						if (npc.localAI[1] >= 60f && Collision.CanHit(npc.position, npc.width, npc.height, player.position, player.width, player.height))
						{
							npc.localAI[1] = 0f;
							float num837 = CalamityWorld.bossRushActive ? 15f : 11f;
							int num838 = brotherAlive ? (expertMode ? 34 : 42) : (expertMode ? 28 : 35);
							int num839 = brotherAlive ? ModContent.ProjectileType<BrimstoneHellfireball>() : ModContent.ProjectileType<BrimstoneLaser>();
							num836 = (float)Math.Sqrt(num834 * num834 + num835 * num835);
							num836 = num837 / num836;
							num834 *= num836;
							num835 *= num836;
							vector83.X += num834 * 12f;
							vector83.Y += num835 * 12f;
							if (!Collision.CanHit(npc.position, npc.width, npc.height, player.position, player.width, player.height))
							{
								int proj = Projectile.NewProjectile(vector83.X, vector83.Y, num834, num835, ModContent.ProjectileType<BrimstoneHellfireball>(), (expertMode ? 34 : 42) + (provy ? 30 : 0), 0f, Main.myPlayer, player.Center.X, player.Center.Y);
								Main.projectile[proj].tileCollide = false;
							}
							else
								Projectile.NewProjectile(vector83.X, vector83.Y, num834, num835, num839, num838 + (provy ? 30 : 0), 0f, Main.myPlayer, 0f, 0f);
						}
					}
					else
					{
						if (revenge)
							npc.localAI[1] += 0.5f;

						if (npc.localAI[1] >= 60f && Collision.CanHit(npc.position, npc.width, npc.height, player.position, player.width, player.height))
						{
							npc.localAI[1] = 0f;
							float num837 = CalamityWorld.bossRushActive ? 14f : 10.5f;
							int num838 = expertMode ? 20 : 24;
							int num839 = ModContent.ProjectileType<BrimstoneLaser>();
							num836 = (float)Math.Sqrt(num834 * num834 + num835 * num835);
							num836 = num837 / num836;
							num834 *= num836;
							num835 *= num836;
							vector83.X += num834 * 12f;
							vector83.Y += num835 * 12f;
							Projectile.NewProjectile(vector83.X, vector83.Y, num834, num835, num839, num838 + (provy ? 30 : 0), 0f, Main.myPlayer, 0f, 0f);
						}
					}
				}

				npc.ai[2] += 1f;
				if (npc.ai[2] >= (phase2 ? 120f : 180f))
				{
					npc.ai[1] = (phase2 && !brotherAlive && lifeRatio < 0.7f && revenge ? 4f : 0f);
					npc.ai[2] = 0f;
					npc.TargetClosest(true);
					npc.netUpdate = true;
				}
			}
			else if (npc.ai[1] == 2f)
			{
				npc.rotation = num803;

				float chargeVelocity = (CalamityWorld.death || CalamityWorld.bossRushActive) ? 27f : 25f;

				if (provy)
					chargeVelocity *= 1.25f;

				if (CalamityWorld.bossRushActive)
					chargeVelocity *= 1.5f;

				Vector2 vector = Vector2.Normalize(player.Center + player.velocity * 20f - npc.Center);
				npc.velocity = vector * chargeVelocity;

				npc.ai[1] = 3f;
			}
			else if (npc.ai[1] == 3f)
			{
				npc.ai[2] += 1f;

				float chargeTime = 70f;
				if (CalamityWorld.bossRushActive)
					chargeTime *= 0.8f;

				if (npc.ai[2] >= chargeTime)
				{
					npc.velocity *= 0.93f;
					if (npc.velocity.X > -0.1 && npc.velocity.X < 0.1)
						npc.velocity.X = 0f;
					if (npc.velocity.Y > -0.1 && npc.velocity.Y < 0.1)
						npc.velocity.Y = 0f;
				}
				else
					npc.rotation = (float)Math.Atan2(npc.velocity.Y, npc.velocity.X) - MathHelper.PiOver2;

				if (npc.ai[2] >= chargeTime + 15f)
				{
					npc.ai[3] += 1f;
					npc.ai[2] = 0f;
					npc.target = 255;
					npc.rotation = num803;
					if (npc.ai[3] > 1f)
					{
						npc.ai[1] = 0f;
						npc.ai[3] = 0f;
						return;
					}
					npc.ai[1] = 4f;
				}
			}
			else
			{
				int num62 = 500;
				float num63 = (calamityGlobalNPC.enraged > 0 || (CalamityMod.CalamityConfig.BossRushXerocCurse && CalamityWorld.bossRushActive)) ? 20f : 14f;
				float num64 = (calamityGlobalNPC.enraged > 0 || (CalamityMod.CalamityConfig.BossRushXerocCurse && CalamityWorld.bossRushActive)) ? 0.5f : 0.35f;

				if (provy)
				{
					num63 *= 1.25f;
					num64 *= 1.25f;
				}

				if (CalamityWorld.bossRushActive)
				{
					num63 *= 1.5f;
					num64 *= 1.5f;
				}

				int num408 = 1;
				if (npc.position.X + (npc.width / 2) < Main.player[npc.target].position.X + Main.player[npc.target].width)
					num408 = -1;

				Vector2 vector11 = new Vector2(npc.position.X + npc.width * 0.5f, npc.position.Y + npc.height * 0.5f);
				float num65 = Main.player[npc.target].position.X + (Main.player[npc.target].width / 2) + (num62 * num408) - vector11.X;
				float num66 = Main.player[npc.target].position.Y + (Main.player[npc.target].height / 2) - vector11.Y;
				float num67 = (float)Math.Sqrt(num65 * num65 + num66 * num66);

				num67 = num63 / num67;
				num65 *= num67;
				num66 *= num67;

				if (npc.velocity.X < num65)
				{
					npc.velocity.X += num64;
					if (npc.velocity.X < 0f && num65 > 0f)
						npc.velocity.X += num64;
				}
				else if (npc.velocity.X > num65)
				{
					npc.velocity.X -= num64;
					if (npc.velocity.X > 0f && num65 < 0f)
						npc.velocity.X -= num64;
				}
				if (npc.velocity.Y < num66)
				{
					npc.velocity.Y += num64;
					if (npc.velocity.Y < 0f && num66 > 0f)
						npc.velocity.Y += num64;
				}
				else if (npc.velocity.Y > num66)
				{
					npc.velocity.Y -= num64;
					if (npc.velocity.Y > 0f && num66 < 0f)
						npc.velocity.Y -= num64;
				}

				npc.ai[2] += 1f;
				if (npc.ai[2] >= 45f)
				{
					npc.TargetClosest(true);
					npc.ai[1] = 2f;
					npc.ai[2] = 0f;
					npc.netUpdate = true;
				}
			}
		}

		public static void CataclysmAI(NPC npc, Mod mod)
		{
			CalamityGlobalNPC calamityGlobalNPC = npc.Calamity();

			// Emit light
			Lighting.AddLight((int)((npc.position.X + (npc.width / 2)) / 16f), (int)((npc.position.Y + (npc.height / 2)) / 16f), 1f, 0f, 0f);

			CalamityGlobalNPC.cataclysm = npc.whoAmI;

			bool death = CalamityWorld.death || CalamityWorld.bossRushActive;
			bool revenge = CalamityWorld.revenge || CalamityWorld.bossRushActive;
			bool expertMode = Main.expertMode || CalamityWorld.bossRushActive;
			bool dayTime = Main.dayTime && !CalamityWorld.bossRushActive;
			bool provy = CalamityWorld.downedProvidence && !CalamityWorld.bossRushActive;

			if (npc.target < 0 || npc.target == 255 || Main.player[npc.target].dead || !Main.player[npc.target].active)
				npc.TargetClosest(true);

			Player player = Main.player[npc.target];

			float num840 = npc.position.X + (npc.width / 2) - player.position.X - (player.width / 2);
			float num841 = npc.position.Y + npc.height - 59f - player.position.Y - (player.height / 2);
			float num842 = (float)Math.Atan2(num841, num840) + MathHelper.PiOver2;
			if (num842 < 0f)
				num842 += MathHelper.TwoPi;
			else if (num842 > MathHelper.TwoPi)
				num842 -= MathHelper.TwoPi;

			float num843 = 0.15f;
			if (npc.rotation < num842)
			{
				if ((num842 - npc.rotation) > MathHelper.Pi)
					npc.rotation -= num843;
				else
					npc.rotation += num843;
			}
			else if (npc.rotation > num842)
			{
				if ((npc.rotation - num842) > MathHelper.Pi)
					npc.rotation += num843;
				else
					npc.rotation -= num843;
			}

			if (npc.rotation > num842 - num843 && npc.rotation < num842 + num843)
				npc.rotation = num842;
			if (npc.rotation < 0f)
				npc.rotation += MathHelper.TwoPi;
			else if (npc.rotation > MathHelper.TwoPi)
				npc.rotation -= MathHelper.TwoPi;
			if (npc.rotation > num842 - num843 && npc.rotation < num842 + num843)
				npc.rotation = num842;

			if (!player.active || player.dead || (dayTime && !Main.eclipse))
			{
				npc.TargetClosest(false);
				player = Main.player[npc.target];
				if (!player.active || player.dead || (dayTime && !Main.eclipse))
				{
					if (npc.velocity.Y > 3f)
						npc.velocity.Y = 3f;
					npc.velocity.Y -= 0.1f;
					if (npc.velocity.Y < -12f)
						npc.velocity.Y = -12f;

					calamityGlobalNPC.newAI[0] = 1f;

					if (npc.timeLeft > 60)
						npc.timeLeft = 60;

					if (npc.ai[1] != 0f)
					{
						npc.ai[1] = 0f;
						npc.ai[2] = 0f;
						npc.ai[3] = 0f;
						npc.netUpdate = true;
					}
					return;
				}
			}
			else
				calamityGlobalNPC.newAI[0] = 0f;

			if (npc.ai[1] == 0f)
			{
				float num861 = 5f;
				float num862 = 0.1f;
				if (provy)
				{
					num861 *= 1.25f;
					num862 *= 1.25f;
				}
				if (CalamityWorld.bossRushActive)
				{
					num861 *= 1.5f;
					num862 *= 1.5f;
				}

				int num863 = 1;
				if (npc.position.X + (npc.width / 2) < player.position.X + player.width)
					num863 = -1;

				Vector2 vector86 = new Vector2(npc.position.X + npc.width * 0.5f, npc.position.Y + npc.height * 0.5f);
				float num864 = player.position.X + (player.width / 2) + (num863 * (CalamityWorld.bossRushActive ? 270 : 180)) - vector86.X;
				float num865 = player.position.Y + (player.height / 2) - vector86.Y;
				float num866 = (float)Math.Sqrt(num864 * num864 + num865 * num865);

				if (expertMode || provy)
				{
					if (num866 > 300f)
						num861 += 0.5f;
					if (num866 > 400f)
						num861 += 0.5f;
					if (num866 > 500f)
						num861 += 0.55f;
					if (num866 > 600f)
						num861 += 0.55f;
					if (num866 > 700f)
						num861 += 0.6f;
					if (num866 > 800f)
						num861 += 0.6f;
				}

				num866 = num861 / num866;
				num864 *= num866;
				num865 *= num866;

				if (npc.velocity.X < num864)
				{
					npc.velocity.X += num862;
					if (npc.velocity.X < 0f && num864 > 0f)
						npc.velocity.X += num862;
				}
				else if (npc.velocity.X > num864)
				{
					npc.velocity.X -= num862;
					if (npc.velocity.X > 0f && num864 < 0f)
						npc.velocity.X -= num862;
				}
				if (npc.velocity.Y < num865)
				{
					npc.velocity.Y += num862;
					if (npc.velocity.Y < 0f && num865 > 0f)
						npc.velocity.Y += num862;
				}
				else if (npc.velocity.Y > num865)
				{
					npc.velocity.Y -= num862;
					if (npc.velocity.Y > 0f && num865 < 0f)
						npc.velocity.Y -= num862;
				}

				npc.ai[2] += (calamityGlobalNPC.enraged > 0 || (CalamityMod.CalamityConfig.BossRushXerocCurse && CalamityWorld.bossRushActive)) ? 2f : 1f;
				if (npc.ai[2] >= 240f)
				{
					npc.TargetClosest(true);
					npc.ai[1] = 1f;
					npc.ai[2] = 0f;
					npc.target = 255;
					npc.netUpdate = true;
				}

				bool fireDelay = npc.ai[2] > 120f || npc.life < npc.lifeMax * 0.9;
				if (Collision.CanHit(npc.position, npc.width, npc.height, player.position, player.width, player.height) && fireDelay)
				{
					npc.localAI[2] += 1f;
					if (npc.localAI[2] > 22f)
					{
						npc.localAI[2] = 0f;
						Main.PlaySound(2, (int)npc.position.X, (int)npc.position.Y, 34);
					}

					if (Main.netMode != NetmodeID.MultiplayerClient)
					{
						npc.localAI[1] += 1f;
						if (revenge)
							npc.localAI[1] += 0.5f;

						if (npc.localAI[1] > 12f)
						{
							npc.localAI[1] = 0f;
							float num867 = CalamityWorld.bossRushActive ? 9f : 6f;
							int num868 = expertMode ? 30 : 38;
							int num869 = ModContent.ProjectileType<BrimstoneFire>();
							vector86 = new Vector2(npc.position.X + npc.width * 0.5f, npc.position.Y + npc.height * 0.5f);
							num864 = player.position.X + (player.width / 2) - vector86.X;
							num865 = player.position.Y + (player.height / 2) - vector86.Y;
							num866 = (float)Math.Sqrt(num864 * num864 + num865 * num865);
							num866 = num867 / num866;
							num864 *= num866;
							num865 *= num866;
							num865 += npc.velocity.Y * 0.5f;
							num864 += npc.velocity.X * 0.5f;
							vector86.X -= num864 * 1f;
							vector86.Y -= num865 * 1f;
							Projectile.NewProjectile(vector86.X, vector86.Y, num864, num865, num869, num868 + (provy ? 30 : 0), 0f, Main.myPlayer, 0f, 0f);
						}
					}
				}
			}
			else
			{
				if (npc.ai[1] == 1f)
				{
					Main.PlaySound(15, (int)npc.position.X, (int)npc.position.Y, 0);
					npc.rotation = num842;

					float num870 = 14f;
					if (expertMode)
						num870 += 2f;
					if (revenge)
						num870 += 2f;
					if (death)
						num870 += 2f;
					if (calamityGlobalNPC.enraged > 0 || (CalamityMod.CalamityConfig.BossRushXerocCurse && CalamityWorld.bossRushActive))
						num870 += 4f;
					if (provy)
						num870 *= 1.15f;
					if (CalamityWorld.bossRushActive)
						num870 *= 1.25f;

					Vector2 vector87 = new Vector2(npc.position.X + npc.width * 0.5f, npc.position.Y + npc.height * 0.5f);
					float num871 = player.position.X + (player.width / 2) - vector87.X;
					float num872 = player.position.Y + (player.height / 2) - vector87.Y;
					float num873 = (float)Math.Sqrt(num871 * num871 + num872 * num872);
					num873 = num870 / num873;
					npc.velocity.X = num871 * num873;
					npc.velocity.Y = num872 * num873;
					npc.ai[1] = 2f;
					return;
				}

				if (npc.ai[1] == 2f)
				{
					npc.ai[2] += 1f;
					if (expertMode)
						npc.ai[2] += 0.25f;
					if (revenge)
						npc.ai[2] += 0.25f;
					if (CalamityWorld.bossRushActive)
						npc.ai[2] += 0.25f;

					if (npc.ai[2] >= 75f)
					{
						npc.velocity.X *= 0.93f;
						npc.velocity.Y *= 0.93f;

						if (npc.velocity.X > -0.1 && npc.velocity.X < 0.1)
							npc.velocity.X = 0f;
						if (npc.velocity.Y > -0.1 && npc.velocity.Y < 0.1)
							npc.velocity.Y = 0f;
					}
					else
						npc.rotation = (float)Math.Atan2(npc.velocity.Y, npc.velocity.X) - MathHelper.PiOver2;

					if (npc.ai[2] >= 105f)
					{
						npc.ai[3] += 1f;
						npc.ai[2] = 0f;
						npc.target = 255;
						npc.rotation = num842;
						if (npc.ai[3] >= 3f)
						{
							npc.ai[1] = 0f;
							npc.ai[3] = 0f;
							return;
						}
						npc.ai[1] = 1f;
					}
				}
			}
		}

		public static void CatastropheAI(NPC npc, Mod mod)
		{
			CalamityGlobalNPC calamityGlobalNPC = npc.Calamity();

			// Emit light
			Lighting.AddLight((int)((npc.position.X + (npc.width / 2)) / 16f), (int)((npc.position.Y + (npc.height / 2)) / 16f), 1f, 0f, 0f);

			CalamityGlobalNPC.catastrophe = npc.whoAmI;

			bool death = CalamityWorld.death || CalamityWorld.bossRushActive;
			bool revenge = CalamityWorld.revenge || CalamityWorld.bossRushActive;
			bool expertMode = Main.expertMode || CalamityWorld.bossRushActive;
			bool dayTime = Main.dayTime && !CalamityWorld.bossRushActive;
			bool provy = CalamityWorld.downedProvidence && !CalamityWorld.bossRushActive;

			if (npc.target < 0 || npc.target == 255 || Main.player[npc.target].dead || !Main.player[npc.target].active)
				npc.TargetClosest(true);

			Player player = Main.player[npc.target];

			float num840 = npc.position.X + (npc.width / 2) - player.position.X - (player.width / 2);
			float num841 = npc.position.Y + npc.height - 59f - player.position.Y - (player.height / 2);
			float num842 = (float)Math.Atan2(num841, num840) + MathHelper.PiOver2;
			if (num842 < 0f)
				num842 += MathHelper.TwoPi;
			else if (num842 > MathHelper.TwoPi)
				num842 -= MathHelper.TwoPi;

			float num843 = 0.15f;
			if (npc.rotation < num842)
			{
				if ((num842 - npc.rotation) > MathHelper.Pi)
					npc.rotation -= num843;
				else
					npc.rotation += num843;
			}
			else if (npc.rotation > num842)
			{
				if ((npc.rotation - num842) > MathHelper.Pi)
					npc.rotation += num843;
				else
					npc.rotation -= num843;
			}

			if (npc.rotation > num842 - num843 && npc.rotation < num842 + num843)
				npc.rotation = num842;
			if (npc.rotation < 0f)
				npc.rotation += MathHelper.TwoPi;
			else if (npc.rotation > MathHelper.TwoPi)
				npc.rotation -= MathHelper.TwoPi;
			if (npc.rotation > num842 - num843 && npc.rotation < num842 + num843)
				npc.rotation = num842;

			if (!player.active || player.dead || (dayTime && !Main.eclipse))
			{
				npc.TargetClosest(false);
				player = Main.player[npc.target];
				if (!player.active || player.dead || (dayTime && !Main.eclipse))
				{
					if (npc.velocity.Y > 3f)
						npc.velocity.Y = 3f;
					npc.velocity.Y -= 0.1f;
					if (npc.velocity.Y < -12f)
						npc.velocity.Y = -12f;

					calamityGlobalNPC.newAI[0] = 1f;

					if (npc.timeLeft > 60)
						npc.timeLeft = 60;

					if (npc.ai[1] != 0f)
					{
						npc.ai[1] = 0f;
						npc.ai[2] = 0f;
						npc.ai[3] = 0f;
						npc.netUpdate = true;
					}
					return;
				}
			}
			else
				calamityGlobalNPC.newAI[0] = 0f;

			if (npc.ai[1] == 0f)
			{
				float num861 = 4.5f;
				float num862 = 0.2f;
				if (provy)
				{
					num861 *= 1.25f;
					num862 *= 1.25f;
				}
				if (CalamityWorld.bossRushActive)
				{
					num861 *= 1.5f;
					num862 *= 1.5f;
				}

				int num863 = 1;
				if (npc.position.X + (npc.width / 2) < player.position.X + player.width)
					num863 = -1;

				Vector2 vector86 = new Vector2(npc.position.X + npc.width * 0.5f, npc.position.Y + npc.height * 0.5f);
				float num864 = player.position.X + (player.width / 2) + (num863 * (CalamityWorld.bossRushActive ? 270 : 180)) - vector86.X;
				float num865 = player.position.Y + (player.height / 2) - vector86.Y;
				float num866 = (float)Math.Sqrt(num864 * num864 + num865 * num865);

				if (expertMode || provy)
				{
					if (num866 > 300f)
						num861 += 0.5f;
					if (num866 > 400f)
						num861 += 0.5f;
					if (num866 > 500f)
						num861 += 0.55f;
					if (num866 > 600f)
						num861 += 0.55f;
					if (num866 > 700f)
						num861 += 0.6f;
					if (num866 > 800f)
						num861 += 0.6f;
				}

				num866 = num861 / num866;
				num864 *= num866;
				num865 *= num866;

				if (npc.velocity.X < num864)
				{
					npc.velocity.X += num862;
					if (npc.velocity.X < 0f && num864 > 0f)
						npc.velocity.X += num862;
				}
				else if (npc.velocity.X > num864)
				{
					npc.velocity.X -= num862;
					if (npc.velocity.X > 0f && num864 < 0f)
						npc.velocity.X -= num862;
				}
				if (npc.velocity.Y < num865)
				{
					npc.velocity.Y += num862;
					if (npc.velocity.Y < 0f && num865 > 0f)
						npc.velocity.Y += num862;
				}
				else if (npc.velocity.Y > num865)
				{
					npc.velocity.Y -= num862;
					if (npc.velocity.Y > 0f && num865 < 0f)
						npc.velocity.Y -= num862;
				}

				npc.ai[2] += (calamityGlobalNPC.enraged > 0 || (CalamityMod.CalamityConfig.BossRushXerocCurse && CalamityWorld.bossRushActive)) ? 2f : 1f;
				if (npc.ai[2] >= 180f)
				{
					npc.TargetClosest(true);
					npc.ai[1] = 1f;
					npc.ai[2] = 0f;
					npc.target = 255;
					npc.netUpdate = true;
				}

				bool fireDelay = npc.ai[2] > 120f || npc.life < npc.lifeMax * 0.9;
				if (Collision.CanHit(npc.position, npc.width, npc.height, player.position, player.width, player.height) && fireDelay)
				{
					npc.localAI[2] += 1f;
					if (npc.localAI[2] > 36f)
					{
						npc.localAI[2] = 0f;
						Main.PlaySound(2, (int)npc.position.X, (int)npc.position.Y, 34);
					}

					if (Main.netMode != NetmodeID.MultiplayerClient)
					{
						npc.localAI[1] += 1f;
						if (revenge)
							npc.localAI[1] += 0.5f;

						if (npc.localAI[1] > 50f)
						{
							npc.localAI[1] = 0f;
							float num867 = CalamityWorld.bossRushActive ? 18f : 12f;
							int num868 = expertMode ? 29 : 36;
							int num869 = ModContent.ProjectileType<BrimstoneBall>();
							vector86 = new Vector2(npc.position.X + npc.width * 0.5f, npc.position.Y + npc.height * 0.5f);
							num864 = player.position.X + (player.width / 2) - vector86.X;
							num865 = player.position.Y + (player.height / 2) - vector86.Y;
							num866 = (float)Math.Sqrt(num864 * num864 + num865 * num865);
							num866 = num867 / num866;
							num864 *= num866;
							num865 *= num866;
							num865 += npc.velocity.Y * 0.5f;
							num864 += npc.velocity.X * 0.5f;
							vector86.X -= num864 * 1f;
							vector86.Y -= num865 * 1f;
							Projectile.NewProjectile(vector86.X, vector86.Y, num864, num865, num869, num868 + (provy ? 30 : 0), 0f, Main.myPlayer, 0f, 0f);
						}
					}
				}
			}
			else
			{
				if (npc.ai[1] == 1f)
				{
					Main.PlaySound(15, (int)npc.position.X, (int)npc.position.Y, 0);
					npc.rotation = num842;

					float num870 = 16f;
					if (expertMode)
						num870 += 2f;
					if (revenge)
						num870 += 2f;
					if (death)
						num870 += 2f;
					if (calamityGlobalNPC.enraged > 0 || (CalamityMod.CalamityConfig.BossRushXerocCurse && CalamityWorld.bossRushActive))
						num870 += 4f;
					if (provy)
						num870 *= 1.15f;
					if (CalamityWorld.bossRushActive)
						num870 *= 1.25f;

					Vector2 vector87 = new Vector2(npc.position.X + npc.width * 0.5f, npc.position.Y + npc.height * 0.5f);
					float num871 = player.position.X + (player.width / 2) - vector87.X;
					float num872 = player.position.Y + (player.height / 2) - vector87.Y;
					float num873 = (float)Math.Sqrt(num871 * num871 + num872 * num872);
					num873 = num870 / num873;
					npc.velocity.X = num871 * num873;
					npc.velocity.Y = num872 * num873;
					npc.ai[1] = 2f;
					return;
				}

				if (npc.ai[1] == 2f)
				{
					npc.ai[2] += 1f;
					if (expertMode)
						npc.ai[2] += 0.25f;
					if (revenge)
						npc.ai[2] += 0.25f;
					if (CalamityWorld.bossRushActive)
						npc.ai[2] += 0.25f;

					if (npc.ai[2] >= 60f) //50
					{
						npc.velocity.X *= 0.93f;
						npc.velocity.Y *= 0.93f;

						if (npc.velocity.X > -0.1 && npc.velocity.X < 0.1)
							npc.velocity.X = 0f;
						if (npc.velocity.Y > -0.1 && npc.velocity.Y < 0.1)
							npc.velocity.Y = 0f;
					}
					else
						npc.rotation = (float)Math.Atan2(npc.velocity.Y, npc.velocity.X) - MathHelper.PiOver2;

					if (npc.ai[2] >= 90f) //80
					{
						npc.ai[3] += 1f;
						npc.ai[2] = 0f;
						npc.target = 255;
						npc.rotation = num842;
						if (npc.ai[3] >= 4f)
						{
							npc.ai[1] = 0f;
							npc.ai[3] = 0f;
							return;
						}
						npc.ai[1] = 1f;
					}
				}
			}
		}
		#endregion

		#region Astrum Aureus
		public static void AstrumAureusAI(NPC npc, Mod mod)
        {
			npc.gfxOffY = -46;

			CalamityGlobalNPC calamityGlobalNPC = npc.Calamity();

			// Percent life remaining
			float lifeRatio = npc.life / (float)npc.lifeMax;

            // Variables
            bool expertMode = Main.expertMode || CalamityWorld.bossRushActive;
            bool revenge = CalamityWorld.revenge || CalamityWorld.bossRushActive;
			bool death = CalamityWorld.death || CalamityWorld.bossRushActive;
			int shootBuff = death ? 1 : (int)(2f * (1f - lifeRatio));
            float shootTimer = 1f + shootBuff;
            bool dayTime = Main.dayTime;

			// Get a target
			if (npc.target < 0 || npc.target == 255 || Main.player[npc.target].dead || !Main.player[npc.target].active)
				npc.TargetClosest(true);

			Player player = Main.player[npc.target];

			// Direction
            npc.spriteDirection = (npc.direction > 0) ? 1 : -1;

			// Phases
			bool phase2 = lifeRatio < 0.75f || death;
			bool phase3 = lifeRatio < 0.5f || death;

			// Despawn
			if (!player.active || player.dead || dayTime)
            {
                npc.TargetClosest(false);
                player = Main.player[npc.target];
                if (!player.active || player.dead || dayTime)
                {
                    npc.noTileCollide = true;

					if (npc.velocity.Y < -3f)
						npc.velocity.Y = -3f;
					npc.velocity.Y += 0.1f;
					if (npc.velocity.Y > 12f)
						npc.velocity.Y = 12f;

					if (npc.timeLeft > 60)
                        npc.timeLeft = 60;

					if (npc.ai[0] != 1f)
					{
						npc.ai[0] = 1f;
						npc.ai[1] = 0f;
						npc.ai[2] = 0f;
						npc.ai[3] = 0f;
						npc.netUpdate = true;
					}
					return;
                }
            }
            else if (npc.timeLeft < 1800)
				npc.timeLeft = 1800;

			// Emit light when not Idle
			if (npc.ai[0] != 1f)
                Lighting.AddLight((int)((npc.position.X + (npc.width / 2)) / 16f), (int)((npc.position.Y + (npc.height / 2)) / 16f), 1.3f, 0.5f, 0f);

            // Fire projectiles while walking, teleporting, or falling
            if (npc.ai[0] == 2f || npc.ai[0] >= 5f || (npc.ai[0] == 4f && npc.velocity.Y > 0f) ||
                calamityGlobalNPC.enraged > 0 || (CalamityMod.CalamityConfig.BossRushXerocCurse && CalamityWorld.bossRushActive))
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    npc.localAI[0] += (npc.ai[0] == 2f || (npc.ai[0] == 4f && npc.velocity.Y > 0f && expertMode)) ? 4f : shootTimer;
                    if (npc.localAI[0] >= 180f)
                    {
                        npc.localAI[0] = 0f;
                        Main.PlaySound(2, (int)npc.position.X, (int)npc.position.Y, 33);
                        int laserDamage = expertMode ? 32 : 37;
                        if (NPC.downedMoonlord && revenge && !CalamityWorld.bossRushActive)
                            laserDamage *= 3;

                        // Fire astral flames while teleporting
                        if ((npc.ai[0] >= 5f && npc.ai[0] != 7) || calamityGlobalNPC.enraged > 0 || (CalamityMod.CalamityConfig.BossRushXerocCurse && CalamityWorld.bossRushActive))
                        {
                            float velocity = CalamityWorld.bossRushActive ? 10f : 7f;
							int totalProjectiles = 8;
							float radians = MathHelper.TwoPi / totalProjectiles;
							for (int i = 0; i < totalProjectiles; i++)
							{
								Vector2 vector255 = new Vector2(0f, -velocity).RotatedBy(radians * i);
								Projectile.NewProjectile(npc.Center, vector255, ModContent.ProjectileType<AstralFlame>(), laserDamage, 0f, Main.myPlayer, 0f, 0f);
							}
                        }

                        // Fire astral lasers while falling or walking
                        else if ((npc.ai[0] == 4f && npc.velocity.Y > 0f && expertMode) || npc.ai[0] == 2f)
                        {
                            float num179 = CalamityWorld.bossRushActive ? 24f : 18.5f;
                            Vector2 value9 = new Vector2(npc.position.X + npc.width * 0.5f, npc.position.Y + npc.height * 0.5f);
                            float num180 = player.position.X + player.width * 0.5f - value9.X;
                            float num181 = Math.Abs(num180) * 0.1f;
                            float num182 = player.position.Y + player.height * 0.5f - value9.Y - num181;
							float num183 = (float)Math.Sqrt(num180 * num180 + num182 * num182);
                            npc.netUpdate = true;
                            num183 = num179 / num183;
                            num180 *= num183;
                            num182 *= num183;
                            int num185 = ModContent.ProjectileType<AstralLaser>();
                            value9.X += num180;
                            value9.Y += num182;
                            for (int num186 = 0; num186 < 5; num186++)
                            {
                                num180 = player.position.X + player.width * 0.5f - value9.X;
                                num182 = player.position.Y + player.height * 0.5f - value9.Y;
                                num183 = (float)Math.Sqrt(num180 * num180 + num182 * num182);
                                num183 = num179 / num183;
                                num180 += Main.rand.Next(-60, 61);
                                num182 += Main.rand.Next(-60, 61);
                                num180 *= num183;
                                num182 *= num183;
                                Projectile.NewProjectile(value9.X, value9.Y, num180, num182, num185, laserDamage, 0f, Main.myPlayer, 0f, 0f);
                            }
                        }
                    }
                }
            }

            // Start up
            if (npc.ai[0] == 0f)
            {
                // If hit or after two seconds start Idle phase
                npc.ai[1] += 1f;
                if (npc.justHit || npc.ai[1] >= 120f)
                {
                    // Set AI to next phase (Idle) and reset other AI
                    npc.ai[0] = 1f;
                    npc.ai[1] = 0f;
                    npc.netUpdate = true;
                }
            }

            // Idle
            else if (npc.ai[0] == 1f)
            {
                // Decrease defense
                npc.defense = 0;

                // Slow down
                npc.velocity.X *= 0.98f;
                npc.velocity.Y *= 0.98f;

                // Stay vulnerable for a maximum of 1.5 or 2.5 seconds
                npc.ai[1] += 1f;
                if (npc.ai[1] >= ((phase3 || calamityGlobalNPC.enraged > 0 || (CalamityMod.CalamityConfig.BossRushXerocCurse && CalamityWorld.bossRushActive)) ? 90f : 150f))
                {
                    // Increase defense
                    npc.defense = 70;

                    // Stop colliding with tiles
                    npc.noGravity = true;
                    npc.noTileCollide = true;

					// Set AI to next phase (Walk) and reset other AI
					npc.TargetClosest(true);
					npc.ai[0] = 2f;
                    npc.ai[1] = 0f;
                    npc.netUpdate = true;
                }
            }

            // Walk
            else if (npc.ai[0] == 2f)
            {
				// Set walking speed
				float speedBoost = death ? 3f : 3f * (1f - lifeRatio);
                float num823 = (CalamityWorld.bossRushActive ? 8f : 5f) + speedBoost;

                // Set walking direction
                if (Math.Abs(npc.Center.X - player.Center.X) < 200f)
                {
                    npc.velocity.X *= 0.9f;
                    if (npc.velocity.X > -0.1 && npc.velocity.X < 0.1)
                        npc.velocity.X = 0f;
                }
                else
                {
                    float playerLocation = npc.Center.X - player.Center.X;
                    npc.direction = playerLocation < 0 ? 1 : -1;

                    if (npc.direction > 0)
                        npc.velocity.X = (npc.velocity.X * 20f + num823) / 21f;
                    if (npc.direction < 0)
                        npc.velocity.X = (npc.velocity.X * 20f - num823) / 21f;
                }

                // Walk through tiles if colliding with tiles and player is out of reach
                int num854 = 80;
                int num855 = 20;
                Vector2 position2 = new Vector2(npc.Center.X - (num854 / 2), npc.position.Y + npc.height - num855);

                bool flag52 = false;
                if (npc.position.X < player.position.X && npc.position.X + npc.width > player.position.X + player.width && npc.position.Y + npc.height < player.position.Y + player.height - 16f)
                    flag52 = true;

                if (flag52)
                    npc.velocity.Y += 0.5f;
                else if (Collision.SolidCollision(position2, num854, num855))
                {
                    if (npc.velocity.Y > 0f)
                        npc.velocity.Y = 0f;

                    if (npc.velocity.Y > -0.2)
                        npc.velocity.Y -= 0.025f;
                    else
                        npc.velocity.Y -= 0.2f;

                    if (npc.velocity.Y < -4f)
                        npc.velocity.Y = -4f;
                }
                else
                {
                    if (npc.velocity.Y < 0f)
                        npc.velocity.Y = 0f;

                    if (npc.velocity.Y < 0.1)
                        npc.velocity.Y += 0.025f;
                    else
                        npc.velocity.Y += 0.5f;
                }

                // Walk for a maximum of 6 seconds
                npc.ai[1] += 1f;
                if (npc.ai[1] >= 360f)
                {
                    // Collide with tiles again
                    npc.noGravity = false;
                    npc.noTileCollide = false;

					// Set AI to next phase (Jump) and reset other AI
					npc.TargetClosest(true);
					npc.ai[0] = 3f;
                    npc.ai[1] = 0f;
                    npc.netUpdate = true;
                }

                // Limit downward velocity
                if (npc.velocity.Y > 10f)
                    npc.velocity.Y = 10f;
            }

            // Jump
            else if (npc.ai[0] == 3f)
            {
                npc.noTileCollide = false;
                if (npc.velocity.Y == 0f)
                {
                    // Slow down
                    npc.velocity.X *= 0.8f;

                    // Half second delay before jumping
                    npc.ai[1] += 1f;
                    if (npc.ai[1] >= 30f)
                        npc.ai[1] = -20f;
                    else if (npc.ai[1] == -1f)
                    {
                        // Set jump velocity, reset and set AI to next phase (Stomp)
						float velocityBoost = death ? 6f : 6f * (1f - lifeRatio);
                        float velocityX = (CalamityWorld.bossRushActive ? 9f : 6f) + velocityBoost;
                        npc.velocity.X = velocityX * npc.direction;

                        if (revenge)
                        {
                            if (player.position.Y < npc.Bottom.Y)
                                npc.velocity.Y = -14.5f;
                            else
                                npc.velocity.Y = 1f;

                            npc.noTileCollide = true;
                        }
                        else
                            npc.velocity.Y = -14.5f;

                        npc.ai[0] = 4f;
                        npc.ai[1] = 0f;
                    }
                }
            }

            // Stomp
            else if (npc.ai[0] == 4f)
            {
                if (npc.velocity.Y == 0f)
                {
                    // Play stomp sound
                    Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/LegStomp"), (int)npc.position.X, (int)npc.position.Y);

					// Stomp and jump again, if stomped twice then reset and set AI to next phase (Teleport or Idle)
					npc.TargetClosest(true);
					npc.ai[2] += 1f;
                    if (npc.ai[2] >= 3f)
                    {
                        npc.ai[0] = (phase2 || revenge) ? 5f : 1f;
                        npc.ai[2] = 0f;
                    }
                    else
                        npc.ai[0] = 3f;

                    // Spawn dust for visual effect
                    for (int num622 = (int)npc.position.X - 20; num622 < (int)npc.position.X + npc.width + 40; num622 += 20)
                    {
                        for (int num623 = 0; num623 < 4; num623++)
                        {
                            int num624 = Dust.NewDust(new Vector2(npc.position.X - 20f, npc.position.Y + npc.height), npc.width + 20, 4, ModContent.DustType<AstralOrange>(), 0f, 0f, 100, default, 1.5f);
                            Main.dust[num624].velocity *= 0.2f;
                        }
                    }
                }
                else
                {
					// Set velocities while falling, this happens before the stomp
					// Fall through
					if (!player.dead && revenge)
					{
						if ((player.position.Y > npc.Bottom.Y && npc.velocity.Y > 0f) || (player.position.Y < npc.Bottom.Y && npc.velocity.Y < 0f))
							npc.noTileCollide = true;
						else if ((npc.velocity.Y > 0f && npc.Bottom.Y > Main.player[npc.target].Top.Y) || (Collision.CanHit(npc.position, npc.width, npc.height, Main.player[npc.target].Center, 1, 1) && !Collision.SolidCollision(npc.position, npc.width, npc.height)))
							npc.noTileCollide = false;
					}

					if (npc.position.X < player.position.X && npc.position.X + npc.width > player.position.X + player.width)
                    {
                        npc.velocity.X *= 0.9f;

						if (npc.Bottom.Y < player.position.Y)
						{
							float fallSpeedBoost = death ? 0.8f : 0.8f * (1f - lifeRatio);
                            float fallSpeed = 0.8f + fallSpeedBoost;
                            npc.velocity.Y += fallSpeed;
                        }
                    }
                    else
                    {
                        if (npc.direction < 0)
                            npc.velocity.X -= 0.2f;
                        else if (npc.direction > 0)
                            npc.velocity.X += 0.2f;

						float velocityBoost = death ? 6f : 6f * (1f - lifeRatio);
                        float num626 = (CalamityWorld.bossRushActive ? 12f : 9f) + velocityBoost;
                        if (npc.velocity.X < -num626)
                            npc.velocity.X = -num626;
                        if (npc.velocity.X > num626)
                            npc.velocity.X = num626;
                    }
                }
            }

            // Teleport
            else if (npc.ai[0] == 5f)
            {
                // Slow down
                npc.velocity *= 0.95f;

                // Spawn slimes and start teleport
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    npc.localAI[1] += 1f;
                    if (!Collision.CanHit(npc.position, npc.width, npc.height, player.position, player.width, player.height))
                        npc.localAI[1] += 5f;

                    if (npc.localAI[1] >= 240f)
                    {
                        // Spawn slimes
                        bool spawnFlag = revenge;
                        if (NPC.CountNPCS(ModContent.NPCType<AureusSpawn>()) > 1)
                            spawnFlag = false;
                        if (spawnFlag && Main.netMode != NetmodeID.MultiplayerClient)
                            NPC.NewNPC((int)npc.Center.X, (int)npc.Center.Y - 25, ModContent.NPCType<AureusSpawn>(), 0, 0f, 0f, 0f, 0f, 255);

						// Reset localAI and find a teleport destination
						npc.TargetClosest(true);
						npc.localAI[1] = 0f;
                        int num1249 = 0;
                        int num1250;
                        int num1251;

                        while (true)
                        {
                            num1249++;
                            num1250 = (int)player.Center.X / 16;
                            num1251 = (int)player.Center.Y / 16;
                            num1250 += Main.rand.Next(-30, 31);
                            num1251 += Main.rand.Next(-30, 31);

                            if (!WorldGen.SolidTile(num1250, num1251) && Collision.CanHit(new Vector2(num1250 * 16, num1251 * 16), 1, 1, player.position, player.width, player.height))
                                break;

                            if (num1249 > 100)
                                goto Block;
                        }

                        // Set AI to next phase (Mid-teleport), set AI 2 and 3 to teleport coordinates X and Y respectively
                        npc.ai[0] = 6f;
                        npc.ai[2] = num1250;
                        npc.ai[3] = num1251;
                        npc.netUpdate = true;
                        Block:
                        ;
                    }
                }
            }

            // Mid-teleport
            else if (npc.ai[0] == 6f)
            {
                // Become immune
                npc.chaseable = false;
                npc.dontTakeDamage = true;

                // Turn invisible
                npc.alpha += 10;
                if (npc.alpha >= 255)
                {
                    // Set position to teleport destination
                    npc.position.X = npc.ai[2] * 16f - (npc.width / 2);
                    npc.position.Y = npc.ai[3] * 16f - (npc.height / 2);

                    // Reset alpha and set AI to next phase (End of teleport)
                    npc.alpha = 255;
                    npc.ai[0] = 7f;
                    npc.netUpdate = true;
                }

                // Play sound for cool effect
                if (npc.soundDelay == 0)
                {
                    npc.soundDelay = 15;
                    Main.PlaySound(2, (int)npc.position.X, (int)npc.position.Y, 109);
                }

                // Emit dust to make the teleport pretty
                int num;
                for (int num245 = 0; num245 < 10; num245 = num + 1)
                {
                    int num244 = Dust.NewDust(npc.position, npc.width, npc.height, ModContent.DustType<AstralOrange>(), npc.velocity.X, npc.velocity.Y, 255, default, 2f);
                    Main.dust[num244].noGravity = true;
                    Main.dust[num244].velocity *= 0.5f;
                    num = num245;
                }
            }

            // End of teleport
            else if (npc.ai[0] == 7f)
            {
                // Turn visible
                npc.alpha -= 10;
                if (npc.alpha <= 0)
                {
                    // Spawn slimes
                    bool spawnFlag = revenge;
                    if (NPC.CountNPCS(ModContent.NPCType<AureusSpawn>()) > 1)
                        spawnFlag = false;
                    if (spawnFlag && Main.netMode != NetmodeID.MultiplayerClient)
                        NPC.NewNPC((int)npc.Center.X, (int)npc.Center.Y - 25, ModContent.NPCType<AureusSpawn>(), 0, 0f, 0f, 0f, 0f, 255);

                    // Become vulnerable
                    npc.chaseable = true;
                    npc.dontTakeDamage = false;

                    // Reset alpha and set AI to next phase (Idle)
                    npc.alpha = 0;
                    npc.ai[0] = 1f;
                    npc.ai[2] = 0f;
                    npc.netUpdate = true;
                }

                // Play sound at teleport destination for cool effect
                if (npc.soundDelay == 0)
                {
                    npc.soundDelay = 15;
                    Main.PlaySound(2, (int)npc.position.X, (int)npc.position.Y, 109);
                }

                // Emit dust to make the teleport pretty
                int num;
                for (int num245 = 0; num245 < 10; num245 = num + 1)
                {
                    int num244 = Dust.NewDust(npc.position, npc.width, npc.height, ModContent.DustType<AstralOrange>(), npc.velocity.X, npc.velocity.Y, 255, default, 2f);
                    Main.dust[num244].noGravity = true;
                    Main.dust[num244].velocity *= 0.5f;
                    num = num245;
                }
            }
        }
		#endregion

		#region Astrum Deus
		public static void AstrumDeusAI(NPC npc, Mod mod, bool head, bool finalWorm)
		{
			// All parts of every worm use the following code
			CalamityGlobalNPC calamityGlobalNPC = npc.Calamity();

			// Difficulty variables
			bool expertMode = Main.expertMode || CalamityWorld.bossRushActive;
			bool revenge = CalamityWorld.revenge || CalamityWorld.bossRushActive;
			bool death = CalamityWorld.death || CalamityWorld.bossRushActive;
			bool enraged = calamityGlobalNPC.enraged > 0 || (CalamityMod.CalamityConfig.BossRushXerocCurse && CalamityWorld.bossRushActive);

			// Emit light
			Lighting.AddLight((int)((npc.position.X + (npc.width / 2)) / 16f), (int)((npc.position.Y + (npc.height / 2)) / 16f), 0.2f, 0.05f, 0.2f);

			// Dust and alpha effects
			if (head || Main.npc[(int)npc.ai[1]].alpha < 128)
			{
				// Spawn dust
				if (npc.alpha != 0)
				{
					for (int num934 = 0; num934 < 2; num934++)
					{
						int num935 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 182, 0f, 0f, 100, default, 2f);
						Main.dust[num935].noGravity = true;
						Main.dust[num935].noLight = true;
					}
				}

				// Alpha changes
				npc.alpha -= 42;
				if (npc.alpha < 0)
					npc.alpha = 0;
			}

			// Set worm variable
			if (npc.ai[3] > 0f)
				npc.realLife = (int)npc.ai[3];

			// Get a target
			if (npc.target < 0 || npc.target == 255 || Main.player[npc.target].dead || !Main.player[npc.target].active)
				npc.TargetClosest(true);

			Player player = Main.player[npc.target];

			// Velocity
			npc.velocity.Length();

			// Direction
			if (npc.velocity.X < 0f)
				npc.spriteDirection = -1;
			else if (npc.velocity.X > 0f)
				npc.spriteDirection = 1;

			// Despawn
			float maxSpeed = 20f;
			bool flies = false;
			if (player.dead || Main.dayTime || (!finalWorm && (CalamityGlobalNPC.astrumDeusHeadMain < 0 || !Main.npc[CalamityGlobalNPC.astrumDeusHeadMain].active)))
			{
				flies = true;
				npc.TargetClosest(false);
				float velocity = finalWorm ? 10f : 3f;
				npc.velocity.Y -= velocity;
				if ((double)npc.position.Y < Main.topWorld + 16f)
				{
					maxSpeed = 40f;
					npc.velocity.Y -= velocity;
				}

				if ((double)npc.position.Y < Main.topWorld + 16f)
				{
					for (int num957 = 0; num957 < Main.maxNPCs; num957++)
					{
						if (Main.npc[num957].aiStyle == npc.aiStyle || Main.npc[num957].type == ModContent.NPCType<AstrumDeusHeadSpectral>() ||
							Main.npc[num957].type == ModContent.NPCType<AstrumDeusBodySpectral>() || Main.npc[num957].type == ModContent.NPCType<AstrumDeusTailSpectral>())
							Main.npc[num957].active = false;
					}
				}
			}

			// All heads use this code
			if (head)
			{
				if (Main.netMode != NetmodeID.MultiplayerClient)
				{
					if (calamityGlobalNPC.newAI[1] == 0f && npc.ai[0] == 0f)
					{
						int maxLength = finalWorm ? 13 : 5;
						int Previous = npc.whoAmI;
						int bodyType = finalWorm ? ModContent.NPCType<AstrumDeusBodySpectral>() : ModContent.NPCType<AstrumDeusBody>();
						int tailType = finalWorm ? ModContent.NPCType<AstrumDeusTailSpectral>() : ModContent.NPCType<AstrumDeusTail>();
						for (int num36 = 0; num36 < maxLength; num36++)
						{
							int lol;
							if (num36 >= 0 && num36 < maxLength - 1)
								lol = NPC.NewNPC((int)npc.position.X + (npc.width / 2), (int)npc.position.Y + (npc.height / 2), bodyType, npc.whoAmI);
							else
								lol = NPC.NewNPC((int)npc.position.X + (npc.width / 2), (int)npc.position.Y + (npc.height / 2), tailType, npc.whoAmI);

							if (num36 % 2 == 0)
								Main.npc[lol].localAI[3] = 1f;

							Main.npc[lol].realLife = npc.whoAmI;
							Main.npc[lol].ai[2] = npc.whoAmI;
							Main.npc[lol].ai[1] = Previous;
							Main.npc[Previous].ai[0] = lol;
							NetMessage.SendData(23, -1, -1, null, lol, 0f, 0f, 0f, 0);
							Previous = lol;
						}
						calamityGlobalNPC.newAI[1] = 1f;
					}
				}
			}

			// All body and tail segments use this code
			else
			{
				// Kill segments
				bool flag = false;
				if (npc.ai[1] <= 0f)
					flag = true;
				else if (Main.npc[(int)npc.ai[1]].life <= 0)
					flag = true;

				if (flag)
				{
					npc.life = 0;
					npc.HitEffect(0, 10.0);
					npc.checkDead();
				}
			}

			// Only the main worm uses this code
			if (finalWorm)
			{
				// Only the main worm head uses this code
				if (head)
				{
					// For other worms and probes
					CalamityGlobalNPC.astrumDeusHeadMain = npc.whoAmI;

					// Change color depending on phase
					if (NPC.AnyNPCs(ModContent.NPCType<AstrumDeusHead>()))
						calamityGlobalNPC.newAI[0] = 1f;
					else
						calamityGlobalNPC.newAI[0] = 0f;

					// Calculate phases based on worm HP
					int astrumDeusTotalHP = 0;
					double mainDeusHPRatio = npc.life / (double)npc.lifeMax;
					if (calamityGlobalNPC.newAI[0] == 1f)
					{
						for (int i = 0; i < Main.maxNPCs; i++)
						{
							if (Main.npc[i].type == ModContent.NPCType<AstrumDeusHead>())
							{
								astrumDeusTotalHP += Main.npc[i].life;
								if (calamityGlobalNPC.newAI[2] < 10f)
								{
									calamityGlobalNPC.newAI[2] += 1f;
									calamityGlobalNPC.newAI[3] += Main.npc[i].lifeMax;
								}
							}
						}

						double astrumDeusHPRatio = astrumDeusTotalHP / calamityGlobalNPC.newAI[3];
						if (astrumDeusHPRatio < 0.33)
						{
							if (mainDeusHPRatio > 0.33)
								calamityGlobalNPC.newAI[0] = 0f;
						}
						else if (astrumDeusHPRatio < 0.66)
						{
							if (mainDeusHPRatio > 0.66)
								calamityGlobalNPC.newAI[0] = 0f;
						}
					}

					// Change vulnerability based on phase (indicated by color)
					npc.dontTakeDamage = calamityGlobalNPC.newAI[0] == 1f;
					npc.chaseable = calamityGlobalNPC.newAI[0] != 1f;

					// Speed and turn speed
					float speedLimit = revenge ? 7f : 6f;
					float turnSpeedLimit = revenge ? 0.11f : 0.1f;
					if (CalamityWorld.bossRushActive)
					{
						speedLimit *= 1.5f;
						turnSpeedLimit *= 1.5f;
					}

					// Speed and turn speed boosts based on remaining HP
					float speedBoost = death ? speedLimit : speedLimit * (1f - (float)mainDeusHPRatio);
					float turnSpeedBoost = death ? turnSpeedLimit : turnSpeedLimit * (1f - (float)mainDeusHPRatio);
					if (player.gravDir == -1f)
					{
						speedBoost = speedLimit;
						turnSpeedBoost = turnSpeedLimit;
					}

					// Velocity
					float num188 = 10f + speedBoost;
					float num189 = 0.2f + turnSpeedBoost;

					Vector2 vector18 = new Vector2(npc.position.X + npc.width * 0.5f, npc.position.Y + npc.height * 0.5f);
					float num191 = player.position.X + (player.width / 2);
					float num192 = player.position.Y + (player.height / 2);

					int num42 = -1;
					int num43 = (int)(player.Center.X / 16f);
					int num44 = (int)(player.Center.Y / 16f);

					for (int num45 = num43 - 2; num45 <= num43 + 2; num45++)
					{
						for (int num46 = num44; num46 <= num44 + 15; num46++)
						{
							if (WorldGen.SolidTile2(num45, num46))
							{
								num42 = num46;
								break;
							}
						}
						if (num42 > 0)
							break;
					}

					float num48 = num188 * 1.3f;
					float num49 = num188 * 0.7f;
					float num50 = npc.velocity.Length();
					if (num50 > 0f)
					{
						if (num50 > num48)
						{
							npc.velocity.Normalize();
							npc.velocity *= num48;
						}
						else if (num50 < num49)
						{
							npc.velocity.Normalize();
							npc.velocity *= num49;
						}
					}

					num191 = (int)(num191 / 16f) * 16;
					num192 = (int)(num192 / 16f) * 16;
					vector18.X = (int)(vector18.X / 16f) * 16;
					vector18.Y = (int)(vector18.Y / 16f) * 16;
					num191 -= vector18.X;
					num192 -= vector18.Y;
					float num193 = (float)Math.Sqrt(num191 * num191 + num192 * num192);
					float num196 = Math.Abs(num191);
					float num197 = Math.Abs(num192);
					float num198 = num188 / num193;
					num191 *= num198;
					num192 *= num198;

					if ((npc.velocity.X > 0f && num191 > 0f) || (npc.velocity.X < 0f && num191 < 0f) || (npc.velocity.Y > 0f && num192 > 0f) || (npc.velocity.Y < 0f && num192 < 0f))
					{
						if (npc.velocity.X < num191)
						{
							npc.velocity.X += num189;
						}
						else
						{
							if (npc.velocity.X > num191)
								npc.velocity.X -= num189;
						}

						if (npc.velocity.Y < num192)
						{
							npc.velocity.Y += num189;
						}
						else
						{
							if (npc.velocity.Y > num192)
								npc.velocity.Y -= num189;
						}

						if (Math.Abs(num192) < num188 * 0.2 && ((npc.velocity.X > 0f && num191 < 0f) || (npc.velocity.X < 0f && num191 > 0f)))
						{
							if (npc.velocity.Y > 0f)
								npc.velocity.Y += num189 * 2f;
							else
								npc.velocity.Y -= num189 * 2f;
						}

						if (Math.Abs(num191) < num188 * 0.2 && ((npc.velocity.Y > 0f && num192 < 0f) || (npc.velocity.Y < 0f && num192 > 0f)))
						{
							if (npc.velocity.X > 0f)
								npc.velocity.X += num189 * 2f;
							else
								npc.velocity.X -= num189 * 2f;
						}
					}
					else
					{
						if (num196 > num197)
						{
							if (npc.velocity.X < num191)
								npc.velocity.X += num189 * 1.1f;
							else if (npc.velocity.X > num191)
								npc.velocity.X -= num189 * 1.1f;

							if ((Math.Abs(npc.velocity.X) + Math.Abs(npc.velocity.Y)) < num188 * 0.5)
							{
								if (npc.velocity.Y > 0f)
									npc.velocity.Y += num189;
								else
									npc.velocity.Y -= num189;
							}
						}
						else
						{
							if (npc.velocity.Y < num192)
								npc.velocity.Y += num189 * 1.1f;
							else if (npc.velocity.Y > num192)
								npc.velocity.Y -= num189 * 1.1f;

							if ((Math.Abs(npc.velocity.X) + Math.Abs(npc.velocity.Y)) < num188 * 0.5)
							{
								if (npc.velocity.X > 0f)
									npc.velocity.X += num189;
								else
									npc.velocity.X -= num189;
							}
						}
					}
					npc.rotation = (float)Math.Atan2(npc.velocity.Y, npc.velocity.X) + MathHelper.PiOver2;
				}

				// Only the main worm body and tail use this code
				else
				{
					// Change vulnerability based on phase (indicated by color)
					if (CalamityGlobalNPC.astrumDeusHeadMain != -1)
					{
						npc.dontTakeDamage = Main.npc[CalamityGlobalNPC.astrumDeusHeadMain].dontTakeDamage;
						npc.chaseable = Main.npc[CalamityGlobalNPC.astrumDeusHeadMain].chaseable;
					}

					// Shoot lasers
					if (npc.type == ModContent.NPCType<AstrumDeusBodySpectral>() && Main.netMode != NetmodeID.MultiplayerClient)
					{
						int shootTime = 4;
						if (npc.life <= npc.lifeMax * 0.65 || death || player.gravDir == -1f)
						{
							shootTime += death ? 4 : 2;
						}
						if (npc.life <= npc.lifeMax * 0.3 || death || player.gravDir == -1f)
						{
							shootTime += death ? 6 : 2;
						}
						npc.localAI[0] += Main.rand.Next(shootTime);
						if (npc.localAI[0] >= Main.rand.Next(1400, 20000))
						{
							npc.localAI[0] = 0f;
							npc.TargetClosest(true);
							if (Collision.CanHit(npc.position, npc.width, npc.height, player.position, player.width, player.height))
							{
								float num941 = revenge ? 14f : 13f;
								if (CalamityWorld.bossRushActive)
								{
									num941 = 24f;
								}
								else if (death)
								{
									num941 = 16f;
								}
								if (player.gravDir == -1f)
								{
									num941 *= 2f;
								}
								Vector2 vector104 = new Vector2(npc.position.X + npc.width * 0.5f, npc.position.Y + (npc.height / 2));
								float num942 = player.position.X + player.width * 0.5f - vector104.X;
								float num943 = player.position.Y + player.height * 0.5f - vector104.Y;
								float num944 = (float)Math.Sqrt(num942 * num942 + num943 * num943);
								num944 = num941 / num944;
								num942 *= num944;
								num943 *= num944;
								int num945 = expertMode ? 38 : 45;
								int num946 = ModContent.ProjectileType<AstralShot2>();
								vector104.X += num942 * 5f;
								vector104.Y += num943 * 5f;
								int num947 = Projectile.NewProjectile(vector104.X, vector104.Y, num942, num943, num946, num945, 0f, Main.myPlayer, 0f, 0f);
								npc.netUpdate = true;
							}
						}
					}

					// Follow the head
					Vector2 vector18 = new Vector2(npc.position.X + npc.width * 0.5f, npc.position.Y + npc.height * 0.5f);
					float num191 = player.position.X + (player.width / 2);
					float num192 = player.position.Y + (player.height / 2);
					num191 = (int)(num191 / 16f) * 16;
					num192 = (int)(num192 / 16f) * 16;
					vector18.X = (int)(vector18.X / 16f) * 16;
					vector18.Y = (int)(vector18.Y / 16f) * 16;
					num191 -= vector18.X;
					num192 -= vector18.Y;
					float num193 = (float)Math.Sqrt(num191 * num191 + num192 * num192);

					if (npc.ai[1] > 0f && npc.ai[1] < Main.npc.Length)
					{
						try
						{
							vector18 = new Vector2(npc.position.X + npc.width * 0.5f, npc.position.Y + npc.height * 0.5f);
							num191 = Main.npc[(int)npc.ai[1]].position.X + (Main.npc[(int)npc.ai[1]].width / 2) - vector18.X;
							num192 = Main.npc[(int)npc.ai[1]].position.Y + (Main.npc[(int)npc.ai[1]].height / 2) - vector18.Y;
						}
						catch
						{
						}

						npc.rotation = (float)Math.Atan2(num192, num191) + MathHelper.PiOver2;
						num193 = (float)Math.Sqrt(num191 * num191 + num192 * num192);
						int num194 = npc.width;
						num193 = (num193 - num194) / num193;
						num191 *= num193;
						num192 *= num193;
						npc.velocity = Vector2.Zero;
						npc.position.X = npc.position.X + num191;
						npc.position.Y = npc.position.Y + num192;

						if (num191 < 0f)
							npc.spriteDirection = -1;
						else if (num191 > 0f)
							npc.spriteDirection = 1;
					}
				}
			}

			// Only the smaller worms use this code
			else
			{
				// Change vulnerability based on phase (indicated by final worm color)
				if (CalamityGlobalNPC.astrumDeusHeadMain != -1)
				{
					if (Main.npc[CalamityGlobalNPC.astrumDeusHeadMain].active)
					{
						npc.dontTakeDamage = !Main.npc[CalamityGlobalNPC.astrumDeusHeadMain].dontTakeDamage;
						npc.chaseable = !Main.npc[CalamityGlobalNPC.astrumDeusHeadMain].chaseable;
					}
				}

				// Life remaining
				float deusHPRatio = npc.life / (float)npc.lifeMax;

				// Only the smaller worm heads use this code
				if (head)
				{
					// Spawn mine probes
					float firstProbeSpawnThreshold = death ? 0.9f : 0.66f;
					float secondProbeSpawnThreshold = death ? 0.8f : 0.33f;

					if (deusHPRatio <= firstProbeSpawnThreshold)
					{
						if (calamityGlobalNPC.newAI[0] == 0f && Main.netMode != NetmodeID.MultiplayerClient)
						{
							Main.PlaySound(2, (int)npc.position.X, (int)npc.position.Y, 74);
							for (int I = 0; I < 3; I++)
							{
								int FireEye = NPC.NewNPC((int)(npc.Center.X + (Math.Sin(I * 120) * 75)), (int)(npc.Center.Y + (Math.Cos(I * 120) * 75)), ModContent.NPCType<AstrumDeusProbe>(), npc.whoAmI, 0, 0, 0, -1);
								NPC Eye = Main.npc[FireEye];
								Eye.ai[0] = I * 120;
								Eye.ai[3] = I * 120;
							}
							calamityGlobalNPC.newAI[0] = 1f;
						}
					}
					if (deusHPRatio <= secondProbeSpawnThreshold)
					{
						if (calamityGlobalNPC.newAI[0] == 1f && Main.netMode != NetmodeID.MultiplayerClient)
						{
							Main.PlaySound(2, (int)npc.position.X, (int)npc.position.Y, 74);
							for (int I = 0; I < 5; I++)
							{
								int FireEye = NPC.NewNPC((int)(npc.Center.X + (Math.Sin(I * 72) * 150)), (int)(npc.Center.Y + (Math.Cos(I * 72) * 150)), ModContent.NPCType<AstrumDeusProbe2>(), npc.whoAmI, 0, 0, 0, -1);
								NPC Eye = Main.npc[FireEye];
								Eye.ai[0] = I * 72;
								Eye.ai[3] = I * 72;
							}
							calamityGlobalNPC.newAI[0] = 2f;
						}
					}

					// Velocity variables
					float speedAdd = expertMode ? 0.06f : 0.045f;
					float turnSpeedAdd = expertMode ? 0.04f : 0.03f;
					float newSpeedBoost = death ? speedAdd : speedAdd * (1f - deusHPRatio);
					float newTurnSpeedBoost = death ? turnSpeedAdd : turnSpeedAdd * (1f - deusHPRatio);
					float newSpeed = 0.12f + newSpeedBoost;
					float newTurnSpeed = 0.08f + newTurnSpeedBoost;

					// Movement variables
					int num180 = (int)(npc.position.X / 16f) - 1;
					int num181 = (int)((npc.position.X + npc.width) / 16f) + 2;
					int num182 = (int)(npc.position.Y / 16f) - 1;
					int num183 = (int)((npc.position.Y + npc.height) / 16f) + 2;

					if (num180 < 0)
						num180 = 0;
					if (num181 > Main.maxTilesX)
						num181 = Main.maxTilesX;
					if (num182 < 0)
						num182 = 0;
					if (num183 > Main.maxTilesY)
						num183 = Main.maxTilesY;

					// Fly or not
					if (!flies)
					{
						for (int num952 = num180; num952 < num181; num952++)
						{
							for (int num953 = num182; num953 < num183; num953++)
							{
								if (Main.tile[num952, num953] != null && ((Main.tile[num952, num953].nactive() && (Main.tileSolid[Main.tile[num952, num953].type] || (Main.tileSolidTop[Main.tile[num952, num953].type] && Main.tile[num952, num953].frameY == 0))) || Main.tile[num952, num953].liquid > 64))
								{
									Vector2 vector105;
									vector105.X = num952 * 16;
									vector105.Y = num953 * 16;
									if (npc.position.X + npc.width > vector105.X && npc.position.X < vector105.X + 16f && npc.position.Y + npc.height > vector105.Y && npc.position.Y < vector105.Y + 16f)
									{
										flies = true;
										break;
									}
								}
							}
						}
					}

					if (!flies)
					{
						npc.localAI[1] = 1f;
						Rectangle rectangle12 = new Rectangle((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height);

						int rectX = (int)(600f * deusHPRatio);
						if (rectX < 200)
							rectX = 200;

						int rectY = (int)(enraged ? 200f : 400f * deusHPRatio);
						if (rectY < (enraged ? 75 : 150))
							rectY = enraged ? 75 : 150;

						bool flag95 = true;
						if (npc.position.Y > player.position.Y)
						{
							for (int num955 = 0; num955 < Main.maxPlayers; num955++)
							{
								if (Main.player[num955].active)
								{
									Rectangle rectangle13 = new Rectangle((int)Main.player[num955].position.X - rectX, (int)Main.player[num955].position.Y - rectY, rectX * 2, rectY * 2);
									if (rectangle12.Intersects(rectangle13))
									{
										flag95 = false;
										break;
									}
								}
							}
							if (flag95)
								flies = true;
						}
					}
					else
						npc.localAI[1] = 0f;

					// Velocity
					float num188 = newSpeed;
					float num189 = newTurnSpeed;
					Vector2 vector18 = new Vector2(npc.position.X + npc.width * 0.5f, npc.position.Y + npc.height * 0.5f);
					float num191 = player.position.X + (player.width / 2);
					float num192 = player.position.Y + (player.height / 2);

					// Spread the worms out based on proximity to other worms
					for (int i = 0; i < Main.maxNPCs; i++)
					{
						if (Main.npc[i].active && Main.npc[i].type == npc.type && i != npc.whoAmI)
						{
							Vector2 vector = Main.npc[i].Center - npc.Center;
							if (vector.Length() < 80f)
							{
								vector.Normalize();
								vector *= 300f;
								num191 -= vector.X;
								num192 -= vector.Y;
							}
						}
					}

					num191 = (int)(num191 / 16f) * 16;
					num192 = (int)(num192 / 16f) * 16;
					vector18.X = (int)(vector18.X / 16f) * 16;
					vector18.Y = (int)(vector18.Y / 16f) * 16;
					num191 -= vector18.X;
					num192 -= vector18.Y;
					float num193 = (float)Math.Sqrt(num191 * num191 + num192 * num192);

					if (!flies)
					{
						npc.TargetClosest(true);

						npc.velocity.Y += 0.15f;
						if (npc.velocity.Y > maxSpeed)
							npc.velocity.Y = maxSpeed;

						if ((Math.Abs(npc.velocity.X) + Math.Abs(npc.velocity.Y)) < maxSpeed * 0.4)
						{
							if (npc.velocity.X < 0f)
								npc.velocity.X -= num188 * 1.1f;
							else
								npc.velocity.X += num188 * 1.1f;
						}
						else if (npc.velocity.Y == maxSpeed)
						{
							if (npc.velocity.X < num191)
								npc.velocity.X += num188;
							else if (npc.velocity.X > num191)
								npc.velocity.X -= num188;
						}
						else if (npc.velocity.Y > 4f)
						{
							if (npc.velocity.X < 0f)
								npc.velocity.X += num188 * 0.9f;
							else
								npc.velocity.X -= num188 * 0.9f;
						}
					}
					else
					{
						if (npc.behindTiles && npc.soundDelay == 0)
						{
							float num195 = num193 / 40f;
							if (num195 < 10f)
								num195 = 10f;
							if (num195 > 20f)
								num195 = 20f;

							npc.soundDelay = (int)num195;
							Main.PlaySound(15, (int)npc.position.X, (int)npc.position.Y, 1);
						}

						num193 = (float)Math.Sqrt(num191 * num191 + num192 * num192);
						float num196 = Math.Abs(num191);
						float num197 = Math.Abs(num192);
						float num198 = maxSpeed / num193;
						num191 *= num198;
						num192 *= num198;

						if (((npc.velocity.X > 0f && num191 > 0f) || (npc.velocity.X < 0f && num191 < 0f)) && ((npc.velocity.Y > 0f && num192 > 0f) || (npc.velocity.Y < 0f && num192 < 0f)))
						{
							if (npc.velocity.X < num191)
								npc.velocity.X += num189;
							else if (npc.velocity.X > num191)
								npc.velocity.X -= num189;

							if (npc.velocity.Y < num192)
								npc.velocity.Y += num189;
							else if (npc.velocity.Y > num192)
								npc.velocity.Y -= num189;
						}

						if ((npc.velocity.X > 0f && num191 > 0f) || (npc.velocity.X < 0f && num191 < 0f) || (npc.velocity.Y > 0f && num192 > 0f) || (npc.velocity.Y < 0f && num192 < 0f))
						{
							if (npc.velocity.X < num191)
							{
								npc.velocity.X += num188;
							}
							else
							{
								if (npc.velocity.X > num191)
									npc.velocity.X -= num188;
							}

							if (npc.velocity.Y < num192)
							{
								npc.velocity.Y += num188;
							}
							else
							{
								if (npc.velocity.Y > num192)
									npc.velocity.Y -= num188;
							}

							if (Math.Abs(num192) < maxSpeed * 0.2 && ((npc.velocity.X > 0f && num191 < 0f) || (npc.velocity.X < 0f && num191 > 0f)))
							{
								if (npc.velocity.Y > 0f)
									npc.velocity.Y += num188 * 2f;
								else
									npc.velocity.Y -= num188 * 2f;
							}

							if (Math.Abs(num191) < maxSpeed * 0.2 && ((npc.velocity.Y > 0f && num192 < 0f) || (npc.velocity.Y < 0f && num192 > 0f)))
							{
								if (npc.velocity.X > 0f)
									npc.velocity.X += num188 * 2f;
								else
									npc.velocity.X -= num188 * 2f;
							}
						}
						else
						{
							if (num196 > num197)
							{
								if (npc.velocity.X < num191)
									npc.velocity.X += num188 * 1.1f;
								else if (npc.velocity.X > num191)
									npc.velocity.X -= num188 * 1.1f;

								if ((Math.Abs(npc.velocity.X) + Math.Abs(npc.velocity.Y)) < maxSpeed * 0.5)
								{
									if (npc.velocity.Y > 0f)
										npc.velocity.Y += num188;
									else
										npc.velocity.Y -= num188;
								}
							}
							else
							{
								if (npc.velocity.Y < num192)
									npc.velocity.Y += num188 * 1.1f;
								else if (npc.velocity.Y > num192)
									npc.velocity.Y -= num188 * 1.1f;

								if ((Math.Abs(npc.velocity.X) + System.Math.Abs(npc.velocity.Y)) < maxSpeed * 0.5)
								{
									if (npc.velocity.X > 0f)
										npc.velocity.X += num188;
									else
										npc.velocity.X -= num188;
								}
							}
						}

						npc.rotation = (float)Math.Atan2(npc.velocity.Y, npc.velocity.X) + MathHelper.PiOver2;

						if (flies)
						{
							if (npc.localAI[0] != 1f)
								npc.netUpdate = true;

							npc.localAI[0] = 1f;
						}
						else
						{
							if (npc.localAI[0] != 0f)
								npc.netUpdate = true;

							npc.localAI[0] = 0f;
						}
						if (((npc.velocity.X > 0f && npc.oldVelocity.X < 0f) || (npc.velocity.X < 0f && npc.oldVelocity.X > 0f) || (npc.velocity.Y > 0f && npc.oldVelocity.Y < 0f) || (npc.velocity.Y < 0f && npc.oldVelocity.Y > 0f)) && !npc.justHit)
						{
							npc.netUpdate = true;
						}
					}
				}
			}
		}
		#endregion

		#region Ceaseless Void
		public static void CeaselessVoidAI(NPC npc, Mod mod)
		{
			CalamityGlobalNPC calamityGlobalNPC = npc.Calamity();

			if (CalamityWorld.bossRushActive)
			{
				calamityGlobalNPC.DR = 0.999999f;
				calamityGlobalNPC.unbreakableDR = true;
			}

			double lifeRatio = npc.life / (double)npc.lifeMax;
			int lifePercentage = (int)(100.0 * lifeRatio);
			bool expertMode = Main.expertMode || CalamityWorld.bossRushActive;
			bool revenge = CalamityWorld.revenge || CalamityWorld.bossRushActive;
			bool death = CalamityWorld.death || CalamityWorld.bossRushActive;

			CalamityGlobalNPC.voidBoss = npc.whoAmI;
			Vector2 vector = npc.Center;

			// Get a target
			npc.TargetClosest(true);

			Player player = Main.player[npc.target];

			if (NPC.CountNPCS(ModContent.NPCType<DarkEnergy>()) > 0 || NPC.CountNPCS(ModContent.NPCType<DarkEnergy2>()) > 0 || NPC.CountNPCS(ModContent.NPCType<DarkEnergy3>()) > 0)
				npc.dontTakeDamage = true;
			else
				npc.dontTakeDamage = false;

			if (!player.active || player.dead || Vector2.Distance(player.Center, vector) > 5600f)
			{
				npc.TargetClosest(false);
				player = Main.player[npc.target];
				if (!player.active || player.dead || Vector2.Distance(player.Center, vector) > 5600f)
				{
					if (npc.velocity.Y > 3f)
						npc.velocity.Y = 3f;
					npc.velocity.Y -= 0.1f;
					if (npc.velocity.Y < -12f)
						npc.velocity.Y = -12f;

					if (npc.timeLeft < 10)
					{
						CalamityWorld.DoGSecondStageCountdown = 0;
						if (Main.netMode == NetmodeID.Server)
						{
							var netMessage = mod.GetPacket();
							netMessage.Write((byte)CalamityModMessageType.DoGCountdownSync);
							netMessage.Write(CalamityWorld.DoGSecondStageCountdown);
							netMessage.Send();
						}
					}

					if (npc.timeLeft > 60)
						npc.timeLeft = 60;

					return;
				}
			}
			else if (npc.timeLeft < 1800)
				npc.timeLeft = 1800;

			if (lifePercentage < 90)
			{
				float num472 = npc.Center.X;
				float num473 = npc.Center.Y;
				float num474 = (float)(500.0 * (1.0 - lifeRatio));
				if (!player.ZoneDungeon)
					num474 *= 1.25f;

				npc.ai[0] += 1f;
				if (npc.ai[0] == 60f)
				{
					npc.ai[0] = 0f;

					int numDust = (int)(0.2f * MathHelper.TwoPi * num474);
					float angleIncrement = MathHelper.TwoPi / numDust;
					Vector2 dustOffset = new Vector2(num474, 0f);
					dustOffset = dustOffset.RotatedByRandom(MathHelper.TwoPi);

					for (int i = 0; i < numDust; i++)
					{
						dustOffset = dustOffset.RotatedBy(angleIncrement);
						int dust = Dust.NewDust(npc.Center, 1, 1, 173);
						Main.dust[dust].position = npc.Center + dustOffset;
						Main.dust[dust].noGravity = true;
						Main.dust[dust].fadeIn = 1f;
						Main.dust[dust].velocity *= 0f;
						Main.dust[dust].scale = 0.5f;
					}

					for (int num475 = 0; num475 < Main.maxPlayers; num475++)
					{
						if (Collision.CanHit(npc.Center, 1, 1, Main.player[num475].Center, 1, 1))
						{
							float num476 = Main.player[num475].position.X + (Main.player[num475].width / 2);
							float num477 = Main.player[num475].position.Y + (Main.player[num475].height / 2);
							float num478 = Math.Abs(npc.position.X + (npc.width / 2) - num476) + Math.Abs(npc.position.Y + (npc.height / 2) - num477);

							if (num478 < num474)
							{
								if (Main.player[num475].position.X < num472)
									Main.player[num475].velocity.X += 15f;
								else
									Main.player[num475].velocity.X -= 15f;
								if (Main.player[num475].position.Y < num473)
									Main.player[num475].velocity.Y += 15f;
								else
									Main.player[num475].velocity.Y -= 15f;
							}
						}
					}
				}
			}

			if (Main.netMode != NetmodeID.MultiplayerClient)
			{
				calamityGlobalNPC.newAI[1] += expertMode ? 1.5f : 1f;
				calamityGlobalNPC.newAI[1] += calamityGlobalNPC.newAI[2];
				if (calamityGlobalNPC.enraged > 0 || (CalamityMod.CalamityConfig.BossRushXerocCurse && CalamityWorld.bossRushActive))
					calamityGlobalNPC.newAI[1] += 2f;

				if (calamityGlobalNPC.newAI[1] >= 900f)
				{
					calamityGlobalNPC.newAI[1] = 0f;
					if (Collision.CanHit(npc.position, npc.width, npc.height, player.position, player.width, player.height))
					{
						float num941 = 3f;
						Vector2 vector104 = new Vector2(npc.position.X + npc.width * 0.5f, npc.position.Y + (npc.height / 2));
						float num942 = player.position.X + player.width * 0.5f - vector104.X + Main.rand.Next(-20, 21);
						float num943 = player.position.Y + player.height * 0.5f - vector104.Y + Main.rand.Next(-20, 21);
						float num944 = (float)Math.Sqrt(num942 * num942 + num943 * num943);
						num944 = num941 / num944;
						num942 *= num944;
						num943 *= num944;
						int num945 = expertMode ? 50 : 60;
						int num946 = ModContent.ProjectileType<DoGBeamPortal>();
						vector104.X += num942 * 5f;
						vector104.Y += num943 * 5f;
						int num947 = Projectile.NewProjectile(vector104.X, vector104.Y, num942, num943, num946, num945, 0f, Main.myPlayer, 0f, 0f);
						Main.projectile[num947].timeLeft = 300;
						npc.netUpdate = true;
					}

					if (revenge)
					{
						int damage = expertMode ? 50 : 60;
						if (lifePercentage < 50 || death || !player.ZoneDungeon)
						{
							for (int i = 0; i < 12; i++)
							{
								float ai1 = player.ZoneDungeon ? 0f : 1f;
								Projectile.NewProjectile(npc.Center.X, npc.Center.Y, 0f, 0f, ModContent.ProjectileType<DarkEnergyBall2>(), damage, 0f, Main.myPlayer, i * 30, ai1);
							}
						}
						else
						{
							float spread = 45f * 0.0174f;
							double startAngle = Math.Atan2(npc.velocity.X, npc.velocity.Y) - spread / 2;
							double deltaAngle = spread / 8f;
							double offsetAngle;
							int i;
							float passedVar = 1f;

							for (i = 0; i < 4; i++)
							{
								offsetAngle = startAngle + deltaAngle * (i + i * i) / 2f + 32f * i;
								Projectile.NewProjectile(npc.Center.X, npc.Center.Y, (float)(Math.Sin(offsetAngle) * 3f), (float)(Math.Cos(offsetAngle) * 3f), ModContent.ProjectileType<DarkEnergyBall>(), damage, 0f, Main.myPlayer, passedVar, 0f);
								passedVar += 1f;
							}
						}
					}
				}
			}

			float num823 = 7.5f;
			float num824 = 0.08f;
			if (!player.ZoneDungeon || CalamityWorld.bossRushActive)
				num823 = 25f;

			Vector2 vector82 = new Vector2(npc.position.X + npc.width * 0.5f, npc.position.Y + npc.height * 0.5f);
			float num825 = player.position.X + (player.width / 2) - vector82.X;
			float num826 = player.position.Y + (player.height / 2) - vector82.Y;
			float num827 = (float)Math.Sqrt(num825 * num825 + num826 * num826);

			num827 = num823 / num827;
			num825 *= num827;
			num826 *= num827;

			if (npc.velocity.X < num825)
			{
				npc.velocity.X += num824;
				if (npc.velocity.X < 0f && num825 > 0f)
					npc.velocity.X += num824;
			}
			else if (npc.velocity.X > num825)
			{
				npc.velocity.X -= num824;
				if (npc.velocity.X > 0f && num825 < 0f)
					npc.velocity.X -= num824;
			}
			if (npc.velocity.Y < num826)
			{
				npc.velocity.Y += num824;
				if (npc.velocity.Y < 0f && num826 > 0f)
					npc.velocity.Y += num824;
			}
			else if (npc.velocity.Y > num826)
			{
				npc.velocity.Y -= num824;
				if (npc.velocity.Y > 0f && num826 < 0f)
					npc.velocity.Y -= num824;
			}

			if (calamityGlobalNPC.newAI[0] == 0f && npc.life > 0)
				calamityGlobalNPC.newAI[0] = npc.lifeMax;

			if (npc.life > 0)
			{
				if (Main.netMode != NetmodeID.MultiplayerClient)
				{
					int num660 = (int)(npc.lifeMax * 0.26);
					if ((npc.life + num660) < calamityGlobalNPC.newAI[0])
					{
						calamityGlobalNPC.newAI[0] = npc.life;
						calamityGlobalNPC.newAI[2] += 1f;
						int glob = revenge ? 8 : 4;
						if (calamityGlobalNPC.newAI[0] <= 0.5f)
							glob = revenge ? 16 : 8;

						for (int num662 = 0; num662 < glob; num662++)
						{
							NPC.NewNPC((int)npc.Center.X - 200, (int)npc.Center.Y - 200, ModContent.NPCType<DarkEnergy>());
							NPC.NewNPC((int)npc.Center.X + 200, (int)npc.Center.Y - 200, ModContent.NPCType<DarkEnergy2>());
							NPC.NewNPC((int)npc.Center.X, (int)npc.Center.Y + 200, ModContent.NPCType<DarkEnergy3>());
						}
					}
				}
			}
		}
		#endregion

		#region Bumblebirb
		public static void BumblebirbAI(NPC npc, Mod mod)
		{
			CalamityGlobalNPC calamityGlobalNPC = npc.Calamity();

			// Get a target
			if (npc.target < 0 || npc.target == 255 || Main.player[npc.target].dead || !Main.player[npc.target].active)
				npc.TargetClosest(true);

			Player player = Main.player[npc.target];

			// Variables
			float rotationMult = 3f;
			float rotationAmt = 0.03f;
			bool revenge = CalamityWorld.revenge || CalamityWorld.bossRushActive;
			bool death = CalamityWorld.death || CalamityWorld.bossRushActive;
			Vector2 vector = npc.Center;

			// Despawn
			if (!player.active || player.dead || Vector2.Distance(player.Center, vector) > 5600f)
			{
				npc.TargetClosest(false);
				player = Main.player[npc.target];

				if (!player.active || player.dead || Vector2.Distance(player.Center, vector) > 5600f)
				{
					npc.rotation = (npc.rotation * rotationMult + npc.velocity.X * rotationAmt) / 10f;

					if (npc.velocity.Y > 3f)
						npc.velocity.Y = 3f;
					npc.velocity.Y -= 0.2f;
					if (npc.velocity.Y < -12f)
						npc.velocity.Y = -12f;

					npc.noTileCollide = true;

					if (npc.timeLeft > 60)
						npc.timeLeft = 60;

					if (npc.ai[0] != 0f)
					{
						npc.ai[0] = 0f;
						npc.ai[1] = 0f;
						npc.ai[2] = 0f;
						npc.ai[3] = 0f;
						npc.netUpdate = true;
					}

					return;
				}
			}
			else if (npc.timeLeft < 1800)
				npc.timeLeft = 1800;

			// Percent life remaining
			float lifeRatio = npc.life / (float)npc.lifeMax;

			// Phases
			bool phase2 = lifeRatio < (revenge ? 0.75f : 0.5f) || death;
			bool phase3 = lifeRatio < (death ? 0.4f : revenge ? 0.25f : 0.1f);

			float newPhaseTimer = 180f;
			bool phaseSwitchPhase = (phase2 && calamityGlobalNPC.newAI[0] < newPhaseTimer && calamityGlobalNPC.newAI[2] != 1f) ||
				(phase3 && calamityGlobalNPC.newAI[1] < newPhaseTimer && calamityGlobalNPC.newAI[3] != 1f);

			npc.dontTakeDamage = phaseSwitchPhase;

			calamityGlobalNPC.DR = npc.ai[0] == 5f ? 0.75f : 0.1f;

			if (phaseSwitchPhase)
			{
				npc.collideX = false;
				npc.collideY = false;
				npc.noTileCollide = true;

				if (npc.velocity.X < 0f)
					npc.direction = -1;
				else if (npc.velocity.X > 0f)
					npc.direction = 1;

				npc.spriteDirection = npc.direction;
				npc.rotation = (npc.rotation * rotationMult + npc.velocity.X * rotationAmt) / 10f;

				if (phase3)
				{
					calamityGlobalNPC.newAI[1] += 1f;

					// Sound
					if (calamityGlobalNPC.newAI[1] == newPhaseTimer - 60f)
					{
						SoundEffectInstance sound = Main.PlaySound(SoundID.DD2_BetsyScream, (int)npc.position.X, (int)npc.position.Y);
						if (sound != null)
						{
							sound.Pitch = 0.25f;
						}
					}

					if (calamityGlobalNPC.newAI[1] >= newPhaseTimer)
					{
						calamityGlobalNPC.newAI[1] = 0f;
						calamityGlobalNPC.newAI[3] = 1f;
						npc.ai[0] = 0f;
						npc.ai[1] = 0f;
						npc.ai[2] = 0f;
						npc.ai[3] = 0f;
					}
				}
				else
				{
					calamityGlobalNPC.newAI[0] += 1f;

					// Sound
					if (calamityGlobalNPC.newAI[0] == newPhaseTimer - 60f)
					{
						SoundEffectInstance sound = Main.PlaySound(SoundID.DD2_BetsyScream, (int)npc.position.X, (int)npc.position.Y);
						if (sound != null)
						{
							sound.Pitch = 0.25f;
						}
					}

					if (calamityGlobalNPC.newAI[0] >= newPhaseTimer)
					{
						calamityGlobalNPC.newAI[0] = 0f;
						calamityGlobalNPC.newAI[2] = 1f;
						npc.ai[0] = 0f;
						npc.ai[1] = 0f;
						npc.ai[2] = 0f;
						npc.ai[3] = 0f;
					}
				}

				Vector2 value52 = player.Center - npc.Center;
				float scaleFactor16 = 4f + value52.Length() / 100f;
				float num1308 = 25f;
				value52.Normalize();
				value52 *= scaleFactor16;
				npc.velocity = (npc.velocity * (num1308 - 1f) + value52) / num1308;
				npc.netSpam = 5;
				if (Main.netMode == NetmodeID.Server)
				{
					NetMessage.SendData(23, -1, -1, null, npc.whoAmI, 0f, 0f, 0f, 0, 0, 0);
				}
				return;
			}

			// Max spawn amount
			int num1305 = revenge ? 3 : 2;
			if (phase2)
				num1305 = death ? 2 : 1;

			// Variable for charging
			float chargeDistance = 600f;
			if (phase2)
				chargeDistance -= 50f;
			if (phase3)
				chargeDistance -= 50f;

			// Don't collide with tiles, disable gravity
			npc.noTileCollide = false;
			npc.noGravity = true;

			// Reset damage
			npc.damage = npc.defDamage;

			// Phase switch
			if (npc.ai[0] == 0f)
			{
				if (npc.Center.X < player.Center.X - 2f)
					npc.direction = 1;
				if (npc.Center.X > player.Center.X + 2f)
					npc.direction = -1;

				// Direction and rotation
				npc.spriteDirection = npc.direction;
				npc.rotation = (npc.rotation * rotationMult + npc.velocity.X * rotationAmt * 1.25f) / 10f;

				// Slow down if colliding with tiles
				if (npc.collideX)
				{
					npc.velocity.X *= (-npc.oldVelocity.X * 0.5f);
					if (npc.velocity.X > 4f)
						npc.velocity.X = 4f;
					if (npc.velocity.X < -4f)
						npc.velocity.X = -4f;
				}
				if (npc.collideY)
				{
					npc.velocity.Y *= (-npc.oldVelocity.Y * 0.5f);
					if (npc.velocity.Y > 4f)
						npc.velocity.Y = 4f;
					if (npc.velocity.Y < -4f)
						npc.velocity.Y = -4f;
				}

				// Fly to target if target is too far away, otherwise get close to target and then slow down
				Vector2 value51 = player.Center - npc.Center;
				value51.Y -= 200f;
				if (value51.Length() > 2800f)
				{
					npc.TargetClosest(true);
					npc.ai[0] = 1f;
					npc.ai[1] = 0f;
					npc.ai[2] = 0f;
					npc.ai[3] = 0f;
				}
				else if (value51.Length() > 240f)
				{
					float scaleFactor15 = 12f;
					float num1306 = 30f;
					value51.Normalize();
					value51 *= scaleFactor15;
					npc.velocity = (npc.velocity * (num1306 - 1f) + value51) / num1306;
				}
				else if (npc.velocity.Length() > 2f)
					npc.velocity *= 0.95f;
				else if (npc.velocity.Length() < 1f)
					npc.velocity *= 1.05f;

				// Phase switch
				npc.ai[1] += 1f;
				if (npc.ai[1] >= 30f && Main.netMode != NetmodeID.MultiplayerClient)
				{
					npc.ai[1] = 0f;
					npc.ai[2] = 0f;
					npc.ai[3] = 0f;
					npc.netUpdate = true;
					while (npc.ai[0] == 0f)
					{
						if (phase2)
							npc.localAI[0] += 1f;

						bool canHit = Collision.CanHit(npc.Center, 1, 1, player.Center, 1, 1);

						if (npc.localAI[0] >= (phase3 ? 7 : 9) && canHit)
						{
							npc.TargetClosest(true);
							npc.ai[0] = 5f;
							npc.localAI[0] = 0f;
						}
						else
						{
							int num1307 = phase2 ? Main.rand.Next(2) + 1 : Main.rand.Next(3);
							if (phase3)
								num1307 = 1;

							int damage = Main.expertMode ? 50 : 60;

							if (num1307 == 0 && canHit)
							{
								npc.TargetClosest(true);
								npc.ai[0] = 2f;
							}
							else if (num1307 == 1)
							{
								npc.TargetClosest(true);
								npc.ai[0] = 3f;
								if (phase2)
								{
									Main.PlaySound(SoundID.Item102, (int)npc.position.X, (int)npc.position.Y);
									Projectile.NewProjectile(npc.Center.X, npc.Center.Y, Main.rand.Next(-2, 3), -4f, ModContent.ProjectileType<RedLightningFeather>(), damage, 0f, Main.myPlayer, 0f, 0f);
								}
							}
							else if (NPC.CountNPCS(ModContent.NPCType<Bumblefuck2>()) < num1305)
							{
								npc.TargetClosest(true);
								npc.ai[0] = 4f;
								Main.PlaySound(SoundID.Item102, (int)npc.position.X, (int)npc.position.Y);
								int featherAmt = phase2 ? 3 : 6;
								if (death)
									featherAmt *= 2;
								for (int num186 = 0; num186 < featherAmt; num186++)
								{
									Projectile.NewProjectile(npc.Center.X, npc.Center.Y, Main.rand.Next(-4, 5), -3f, ModContent.ProjectileType<RedLightningFeather>(), damage, 0f, Main.myPlayer, 0f, 0f);
								}
							}
						}
					}
				}
			}

			// Fly to target
			else if (npc.ai[0] == 1f)
			{
				npc.collideX = false;
				npc.collideY = false;
				npc.noTileCollide = true;

				if (npc.velocity.X < 0f)
					npc.direction = -1;
				else if (npc.velocity.X > 0f)
					npc.direction = 1;

				npc.spriteDirection = npc.direction;
				npc.rotation = (npc.rotation * rotationMult + npc.velocity.X * rotationAmt) / 10f;

				Vector2 value52 = player.Center - npc.Center;
				if (value52.Length() < 800f && !Collision.SolidCollision(npc.position, npc.width, npc.height))
				{
					npc.TargetClosest(true);
					npc.ai[0] = 0f;
					npc.ai[1] = 0f;
					npc.ai[2] = 0f;
					npc.ai[3] = 0f;
				}

				float scaleFactor16 = 14f + value52.Length() / 100f; //7
				float num1308 = 25f;
				value52.Normalize();
				value52 *= scaleFactor16;
				npc.velocity = (npc.velocity * (num1308 - 1f) + value52) / num1308;
				npc.netSpam = 5;
				if (Main.netMode == NetmodeID.Server)
				{
					NetMessage.SendData(23, -1, -1, null, npc.whoAmI, 0f, 0f, 0f, 0, 0, 0);
				}
			}

			// Fly towards target quickly
			else if (npc.ai[0] == 2f)
			{
				if (npc.target < 0 || !player.active || player.dead)
				{
					npc.TargetClosest(true);
					npc.ai[0] = 0f;
					npc.ai[1] = 0f;
					npc.ai[2] = 0f;
					npc.ai[3] = 0f;
				}

				if (player.Center.X - 10f < npc.Center.X)
					npc.direction = -1;
				else if (player.Center.X + 10f > npc.Center.X)
					npc.direction = 1;

				npc.spriteDirection = npc.direction;
				npc.rotation = (npc.rotation * rotationMult * 0.5f + npc.velocity.X * rotationAmt * 1.25f) / 5f;

				if (npc.collideX)
				{
					npc.velocity.X *= (-npc.oldVelocity.X * 0.5f);
					if (npc.velocity.X > 4f)
						npc.velocity.X = 4f;
					if (npc.velocity.X < -4f)
						npc.velocity.X = -4f;
				}
				if (npc.collideY)
				{
					npc.velocity.Y *= (-npc.oldVelocity.Y * 0.5f);
					if (npc.velocity.Y > 4f)
						npc.velocity.Y = 4f;
					if (npc.velocity.Y < -4f)
						npc.velocity.Y = -4f;
				}

				Vector2 value53 = player.Center - npc.Center;
				value53.Y -= 20f;
				npc.ai[2] += 0.0222222228f;
				if (Main.expertMode)
					npc.ai[2] += 0.0166666675f;

				float scaleFactor17 = 8f + npc.ai[2] + value53.Length() / 120f; //4
				float num1309 = 20f;
				value53.Normalize();
				value53 *= scaleFactor17;
				npc.velocity = (npc.velocity * (num1309 - 1f) + value53) / num1309;

				npc.ai[1] += 1f;
				if (npc.ai[1] >= 120f || !Collision.CanHit(npc.Center, 1, 1, player.Center, 1, 1))
				{
					npc.TargetClosest(true);
					npc.ai[0] = 0f;
					npc.ai[1] = 0f;
					npc.ai[2] = 0f;
					npc.ai[3] = 0f;
				}
			}

			// Line up charge
			else if (npc.ai[0] == 3f)
			{
				npc.noTileCollide = true;

				if (npc.velocity.X < 0f)
					npc.direction = -1;
				else
					npc.direction = 1;

				npc.spriteDirection = npc.direction;
				npc.rotation = (npc.rotation * rotationMult * 0.5f + npc.velocity.X * rotationAmt * 0.85f) / 5f;

				Vector2 value54 = player.Center - npc.Center;
				value54.Y -= 12f;
				if (npc.Center.X > player.Center.X)
					value54.X += chargeDistance;
				else
					value54.X -= chargeDistance;

				if (Math.Abs(npc.Center.X - player.Center.X) > chargeDistance - 50f && Math.Abs(npc.Center.Y - player.Center.Y) < (phase3 ? 100f : 20f))
				{
					npc.ai[0] = 3.1f;
					npc.ai[1] = 0f;
				}

				npc.ai[1] += 0.0333333351f;
				float scaleFactor18 = 18f + npc.ai[1];
				float num1310 = 4f;
				value54.Normalize();
				value54 *= scaleFactor18;
				npc.velocity = (npc.velocity * (num1310 - 1f) + value54) / num1310;
			}

			// Prepare to charge
			else if (npc.ai[0] == 3.1f)
			{
				npc.noTileCollide = true;

				npc.rotation = (npc.rotation * rotationMult * 0.5f + npc.velocity.X * rotationAmt * 0.85f) / 5f;

				Vector2 vector206 = player.Center - npc.Center;
				vector206.Y -= 12f;
				float scaleFactor19 = 32f; //16
				float num1311 = 8f;
				vector206.Normalize();
				vector206 *= scaleFactor19;
				npc.velocity = (npc.velocity * (num1311 - 1f) + vector206) / num1311;

				if (npc.velocity.X < 0f)
					npc.direction = -1;
				else
					npc.direction = 1;

				npc.spriteDirection = npc.direction;

				npc.ai[1] += 1f;
				if (npc.ai[1] > 10f)
				{
					npc.velocity = vector206;

					if (npc.velocity.X < 0f)
						npc.direction = -1;
					else
						npc.direction = 1;

					npc.ai[0] = 3.2f;
					npc.ai[1] = 0f;
					npc.ai[1] = npc.direction;
				}
			}

			// Charge
			else if (npc.ai[0] == 3.2f)
			{
				npc.damage = (int)(npc.defDamage * 1.5);

				npc.collideX = false;
				npc.collideY = false;
				npc.noTileCollide = true;

				npc.ai[2] += 0.0333333351f;
				npc.velocity.X = (32f + npc.ai[2]) * npc.ai[1];

				if ((npc.ai[1] > 0f && npc.Center.X > player.Center.X + (chargeDistance - 140f)) || (npc.ai[1] < 0f && npc.Center.X < player.Center.X - (chargeDistance - 140f)))
				{
					if (!Collision.SolidCollision(npc.position, npc.width, npc.height))
					{
						npc.TargetClosest(true);
						npc.ai[0] = 0f;
						npc.ai[1] = 0f;
						npc.ai[2] = 0f;
						npc.ai[3] = 0f;
					}
					else if (Math.Abs(npc.Center.X - player.Center.X) > chargeDistance + 200f)
					{
						npc.TargetClosest(true);
						npc.ai[0] = 1f;
						npc.ai[1] = 0f;
						npc.ai[2] = 0f;
						npc.ai[3] = 0f;
					}
				}

				npc.rotation = (npc.rotation * rotationMult * 0.5f + npc.velocity.X * rotationAmt * 0.85f) / 5f;
			}

			// Find tile coordinates for birb spawn
			else if (npc.ai[0] == 4f)
			{
				npc.ai[0] = 0f;

				if (Main.netMode != NetmodeID.MultiplayerClient)
				{
					npc.ai[1] = -1f;
					npc.ai[2] = -1f;

					for (int num1312 = 0; num1312 < 1000; num1312++)
					{
						int num1313 = (int)player.Center.X / 16;
						int num1314 = (int)player.Center.Y / 16;

						int num1315 = 30 + num1312 / 50;
						int num1316 = 20 + num1312 / 75;

						num1313 += Main.rand.Next(-num1315, num1315 + 1);
						num1314 += Main.rand.Next(-num1316, num1316 + 1);

						if (!WorldGen.SolidTile(num1313, num1314))
						{
							while (!WorldGen.SolidTile(num1313, num1314) && num1314 < Main.worldSurface)
								num1314++;

							if ((new Vector2(num1313 * 16 + 8, num1314 * 16 + 8) - player.Center).Length() < 1200f)
							{
								npc.ai[0] = 4.1f;
								npc.ai[1] = num1313;
								npc.ai[2] = num1314;
								break;
							}
						}
					}
				}

				npc.netUpdate = true;
			}

			// Move to birb spawn location
			else if (npc.ai[0] == 4.1f)
			{
				if (npc.velocity.X < -2f)
					npc.direction = -1;
				else if (npc.velocity.X > 2f)
					npc.direction = 1;

				npc.spriteDirection = npc.direction;
				npc.rotation = (npc.rotation * rotationMult + npc.velocity.X * rotationAmt * 1.25f) / 10f;

				npc.noTileCollide = true;

				int num1317 = (int)npc.ai[1];
				int num1318 = (int)npc.ai[2];

				float x2 = num1317 * 16 + 8;
				float y2 = num1318 * 16 - 20;

				Vector2 vector207 = new Vector2(x2, y2);
				vector207 -= npc.Center;
				float num1319 = 12f + vector207.Length() / 150f;
				if (num1319 > 20f)
					num1319 = 20f;

				float num1320 = 10f;
				if (vector207.Length() < 50f)
					npc.ai[0] = 4.2f;

				vector207.Normalize();
				vector207 *= num1319;
				npc.velocity = (npc.velocity * (num1320 - 1f) + vector207) / num1320;
			}

			// Spawn birbs
			else if (npc.ai[0] == 4.2f)
			{
				npc.rotation = (npc.rotation * rotationMult + npc.velocity.X * rotationAmt * 1.25f) / 10f;

				npc.noTileCollide = true;

				int num1321 = (int)npc.ai[1];
				int num1322 = (int)npc.ai[2];

				float x3 = num1321 * 16 + 8;
				float y3 = num1322 * 16 - 20;

				Vector2 vector208 = new Vector2(x3, y3);
				vector208 -= npc.Center;

				float num1323 = 4f; //4
				float num1324 = 2f; //2

				if (Main.netMode != NetmodeID.MultiplayerClient && vector208.Length() < 20f)
				{
					int num1325 = 10;
					if (Main.expertMode)
						num1325 = (int)(num1325 * 0.75);

					npc.ai[3] += 1f;
					if (npc.ai[3] == num1325)
					{
						SoundEffectInstance sound = Main.PlaySound(SoundID.DD2_BetsySummon, (int)npc.position.X, (int)npc.position.Y);
						if (sound != null)
						{
							sound.Pitch = 0.25f;
						}

						NPC.NewNPC(num1321 * 16 + 8, num1322 * 16, ModContent.NPCType<Bumblefuck2>(), npc.whoAmI, 0f, 0f, 0f, 0f, 255);
					}
					else if (npc.ai[3] == num1325 * 2)
					{
						npc.TargetClosest(true);
						npc.ai[0] = 0f;
						npc.ai[1] = 0f;
						npc.ai[2] = 0f;
						npc.ai[3] = 0f;

						if (NPC.CountNPCS(ModContent.NPCType<Bumblefuck2>()) < num1305 && Main.rand.NextBool(2))
							npc.ai[0] = 4f;
						else if (Collision.SolidCollision(npc.position, npc.width, npc.height))
							npc.ai[0] = 1f;
					}
				}

				if (vector208.Length() > num1323)
				{
					vector208.Normalize();
					vector208 *= num1323;
				}

				npc.velocity = (npc.velocity * (num1324 - 1f) + vector208) / num1324;
			}

			// Spit homing aura sphere
			else if (npc.ai[0] == 5f)
			{
				// Velocity
				npc.velocity *= 0.98f;
				npc.rotation = (npc.rotation * rotationMult + npc.velocity.X * rotationAmt) / 10f;

				// Play sound
				float aiGateValue = 120f;
				if (npc.ai[1] == aiGateValue - 30f)
				{
					Main.PlaySound(SoundID.DD2_BetsyFireballShot, (int)npc.position.X, (int)npc.position.Y);

					if (Main.netMode != NetmodeID.MultiplayerClient)
					{
						Vector2 vector7 = npc.rotation.ToRotationVector2() * (Vector2.UnitX * npc.direction) * (npc.width + 20) / 2f + vector;
						Projectile.NewProjectile(vector7.X, vector7.Y, 0f, 0f, ModContent.ProjectileType<BirbAuraFlare>(), 0, 0f, Main.myPlayer, phase3 ? 1f : 0f, npc.target + 1);
					}
				}

				npc.ai[1] += 1f;
				if (npc.ai[1] >= aiGateValue)
				{
					npc.ai[0] = 0f;
					npc.ai[1] = 0f;
					npc.netUpdate = true;
				}
			}
		}
		#endregion

		#region Old Duke
		public static void OldDukeAI(NPC npc, Mod mod)
		{
			CalamityGlobalNPC calamityGlobalNPC = npc.Calamity();

			// Variables
			bool expertMode = Main.expertMode || CalamityWorld.bossRushActive;
			bool revenge = CalamityWorld.revenge || CalamityWorld.bossRushActive;
			bool death = CalamityWorld.death || CalamityWorld.bossRushActive;
			bool phase2 = npc.life <= npc.lifeMax * (revenge ? 0.7 : 0.5) || death;
			bool phase3 = npc.life <= npc.lifeMax * (death ? 0.5 : (revenge ? 0.35 : 0.2)) && expertMode;
			bool phase2AI = npc.ai[0] > 4f;
			bool phase3AI = npc.ai[0] > 9f;
			bool charging = npc.ai[3] < 10f;
			float pie = (float)Math.PI;

			if (calamityGlobalNPC.newAI[0] >= 300f)
				calamityGlobalNPC.newAI[1] = 1f;

			if (calamityGlobalNPC.newAI[1] == 1f)
			{
				// Play tired sound
				if (calamityGlobalNPC.newAI[0] % 60f == 0f)
					Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/OldDukeHuff"), (int)npc.position.X, (int)npc.position.Y);

				calamityGlobalNPC.newAI[0] -= 1f;
				if (calamityGlobalNPC.newAI[0] <= 0f)
					calamityGlobalNPC.newAI[1] = 0f;
			}

			// Adjust stats
			calamityGlobalNPC.DR = calamityGlobalNPC.newAI[1] == 1f ? 0.15f : 0.6f;
			npc.defense = calamityGlobalNPC.newAI[1] == 1f ? npc.defDefense / 4 : npc.defDefense;
			if (phase3AI)
			{
				npc.damage = (int)(npc.defDamage * 1.32f);
			}
			else if (phase2AI)
			{
				npc.damage = (int)(npc.defDamage * 1.44f);
			}
			else
			{
				npc.damage = npc.defDamage;
			}

			int num2 = 60;
			float num3 = 0.6f;
			float scaleFactor = 10f;
			if (phase3AI)
			{
				num3 = 0.75f;
				scaleFactor = 13f;
			}
			else if (phase2AI & charging)
			{
				num3 = 0.65f;
				scaleFactor = 11f;
			}

			int chargeTime = 36;
			float chargeVelocity = 19f;
			if (phase3AI)
			{
				chargeTime = 30;
				chargeVelocity = 25f;
			}
			else if (charging & phase2AI)
			{
				chargeTime = 33;
				chargeVelocity = 23f;
			}

			if (death)
			{
				num2 = 54;
				num3 *= 1.05f;
				scaleFactor *= 1.08f;
				chargeTime -= 2;
				chargeVelocity *= 1.13f;
			}
			else if (revenge)
			{
				num2 = 57;
				num3 *= 1.025f;
				scaleFactor *= 1.04f;
				chargeTime -= 1;
				chargeVelocity *= 1.065f;
			}
			
			if (CalamityWorld.bossRushActive)
			{
				num2 = 45;
				num3 *= 1.1f;
				scaleFactor *= 1.15f;
				chargeTime -= 3;
				chargeVelocity *= 1.25f;
			}

			if (calamityGlobalNPC.newAI[1] == 1f)
				scaleFactor *= 0.25f;

			// Variables
			int num6 = 120;
			int num7 = 24;
			float num8 = 0.3f;
			float scaleFactor2 = 6f;
			int num9 = 120;
			int num10 = 180;
			int num11 = 180;
			int num12 = 30;
			int num13 = 120;
			int num14 = 24;
			float spinTime = num13 / 2;
			float scaleFactor3 = 9f;
			float scaleFactor4 = 22f;
			float num15 = MathHelper.TwoPi / spinTime;
			int num16 = 75;

			Vector2 vector = npc.Center;

			Player player = Main.player[npc.target];

			// Get target
			if (npc.target < 0 || npc.target == 255 || player.dead || !player.active)
			{
				npc.TargetClosest(true);
				player = Main.player[npc.target];
				npc.netUpdate = true;
			}

			// Despawn
			if (player.dead || Vector2.Distance(player.Center, vector) > 5600f)
			{
				npc.velocity.Y -= 0.4f;

				if (npc.timeLeft > 10)
					npc.timeLeft = 10;

				if (npc.timeLeft == 1)
				{
					CalamityWorld.acidRainPoints = 0;
					CalamityWorld.triedToSummonOldDuke = false;
					AcidRainEvent.UpdateInvasion(false);
					npc.timeLeft = 0;
				}

				if (npc.ai[0] > 4f)
					npc.ai[0] = 5f;
				else
					npc.ai[0] = 0f;

				npc.ai[2] = 0f;
			}

			// Enrage variable
			bool enrage = !CalamityWorld.bossRushActive &&
				(player.position.Y < 300f || player.position.Y > Main.worldSurface * 16.0 ||
				(player.position.X > 7200f && player.position.X < (Main.maxTilesX * 16 - 7200)));

			// If the player isn't in the ocean biome or Old Duke is transitioning between phases, become immune
			if (!phase3AI)
				npc.dontTakeDamage = npc.ai[0] == -1f || npc.ai[0] == 4f || npc.ai[0] == 9f || enrage;

			// Enrage
			if (enrage)
			{
				num2 = 40;
				npc.damage = npc.defDamage * 2;
				npc.defense = npc.defDefense * 2;
				npc.ai[3] = 0f;
				chargeVelocity += 8f;
			}

			// Set variables for spawn effects
			if (npc.localAI[0] == 0f)
			{
				npc.localAI[0] = 1f;
				npc.alpha = 255;
				npc.rotation = 0f;
				if (Main.netMode != NetmodeID.MultiplayerClient)
				{
					npc.ai[0] = -1f;
					npc.netUpdate = true;
				}
			}

			// Rotation
			float num17 = (float)Math.Atan2(player.Center.Y - vector.Y, player.Center.X - vector.X);
			if (npc.spriteDirection == 1)
				num17 += pie;
			if (num17 < 0f)
				num17 += MathHelper.TwoPi;
			if (num17 > MathHelper.TwoPi)
				num17 -= MathHelper.TwoPi;
			if (npc.ai[0] == -1f || npc.ai[0] == 3f || npc.ai[0] == 4f)
				num17 = 0f;
			if (npc.ai[0] == 8f || npc.ai[0] == 13f)
			{
				num17 = pie * 0.1666666667f * npc.spriteDirection;
			}

			float num18 = 0.04f;
			if (npc.ai[0] == 1f || npc.ai[0] == 6f || npc.ai[0] == 7f || npc.ai[0] == 14f)
				num18 = 0f;
			if (npc.ai[0] == 3f || npc.ai[0] == 4f)
				num18 = 0.01f;
			if (npc.ai[0] == 8f || npc.ai[0] == 13f)
				num18 = 0.05f;

			if (npc.rotation < num17)
			{
				if (num17 - npc.rotation > pie)
					npc.rotation -= num18;
				else
					npc.rotation += num18;
			}
			if (npc.rotation > num17)
			{
				if (npc.rotation - num17 > pie)
					npc.rotation += num18;
				else
					npc.rotation -= num18;
			}

			if ((npc.ai[0] != 8f && npc.ai[0] != 13f) || npc.spriteDirection == 1)
			{
				if (npc.rotation > num17 - num18 && npc.rotation < num17 + num18)
					npc.rotation = num17;
				if (npc.rotation < 0f)
					npc.rotation += MathHelper.TwoPi;
				if (npc.rotation > MathHelper.TwoPi)
					npc.rotation -= MathHelper.TwoPi;
				if (npc.rotation > num17 - num18 && npc.rotation < num17 + num18)
					npc.rotation = num17;
			}

			// Alpha adjustments
			if (npc.ai[0] != -1f && (npc.ai[0] < 9f || npc.ai[0] > 12f))
			{
				if (Collision.SolidCollision(npc.position, npc.width, npc.height))
					npc.alpha += 15;
				else
					npc.alpha -= 15;

				if (npc.alpha < 0)
					npc.alpha = 0;
				if (npc.alpha > 150)
					npc.alpha = 150;
			}

			// Spawn effects
			if (npc.ai[0] == -1f)
			{
				// Velocity
				if (npc.Calamity().newAI[3] == 0f)
					npc.velocity *= 0.98f;

				// Direction
				int num19 = Math.Sign(player.Center.X - vector.X);
				if (num19 != 0)
				{
					npc.direction = num19;
					npc.spriteDirection = -npc.direction;
				}

				// Alpha
				if (npc.ai[2] > 20f)
				{
					if (npc.Calamity().newAI[3] == 0f)
						npc.velocity.Y = -2f;

					npc.alpha -= 5;
					if (Collision.SolidCollision(npc.position, npc.width, npc.height))
						npc.alpha += 15;
					if (npc.alpha < 0)
						npc.alpha = 0;
					if (npc.alpha > 150)
						npc.alpha = 150;
				}

				// Spawn dust and play sound
				if (npc.ai[2] == num9 - 30)
				{
					int num20 = 36;
					for (int i = 0; i < num20; i++)
					{
						Vector2 dust = (Vector2.Normalize(npc.velocity) * new Vector2(npc.width / 2f, npc.height) * 0.75f * 0.5f).RotatedBy((i - (num20 / 2 - 1)) * MathHelper.TwoPi / num20) + npc.Center;
						Vector2 vector2 = dust - npc.Center;
						int num21 = Dust.NewDust(dust + vector2, 0, 0, (int)CalamityDusts.SulfurousSeaAcid, vector2.X * 2f, vector2.Y * 2f, 100, default, 1.4f);
						Main.dust[num21].noGravity = true;
						Main.dust[num21].noLight = true;
						Main.dust[num21].velocity = Vector2.Normalize(vector2) * 3f;
					}

					Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/OldDukeRoar"), (int)npc.position.X, (int)npc.position.Y);
				}

				npc.ai[2] += 1f;
				if (npc.ai[2] >= num16)
				{
					npc.ai[0] = 0f;
					npc.ai[1] = 0f;
					npc.ai[2] = 0f;
					npc.netUpdate = true;
				}
			}

			// Phase 1
			else if (npc.ai[0] == 0f && !player.dead)
			{
				// Velocity
				if (npc.ai[1] == 0f)
					npc.ai[1] = 500 * Math.Sign((vector - player.Center).X);

				Vector2 vector3 = Vector2.Normalize(player.Center + new Vector2(npc.ai[1], -300f) - vector - npc.velocity) * scaleFactor;
				if (npc.velocity.X < vector3.X)
				{
					npc.velocity.X += num3;
					if (npc.velocity.X < 0f && vector3.X > 0f)
						npc.velocity.X += num3;
				}
				else if (npc.velocity.X > vector3.X)
				{
					npc.velocity.X -= num3;
					if (npc.velocity.X > 0f && vector3.X < 0f)
						npc.velocity.X -= num3;
				}
				if (npc.velocity.Y < vector3.Y)
				{
					npc.velocity.Y += num3;
					if (npc.velocity.Y < 0f && vector3.Y > 0f)
						npc.velocity.Y += num3;
				}
				else if (npc.velocity.Y > vector3.Y)
				{
					npc.velocity.Y -= num3;
					if (npc.velocity.Y > 0f && vector3.Y < 0f)
						npc.velocity.Y -= num3;
				}

				// Rotation and direction
				int num22 = Math.Sign(player.Center.X - vector.X);
				if (num22 != 0)
				{
					if (npc.ai[2] == 0f && num22 != npc.direction)
						npc.rotation += pie;

					npc.direction = num22;

					if (npc.spriteDirection != -npc.direction)
						npc.rotation += pie;

					npc.spriteDirection = -npc.direction;
				}

				// Phase switch
				if (calamityGlobalNPC.newAI[1] != 1f || (phase2 && !phase2AI))
				{
					npc.ai[2] += 1f;
					if (npc.ai[2] >= num2 || (phase2 && !phase2AI))
					{
						int num23 = 0;
						switch ((int)npc.ai[3])
						{
							case 0:
							case 1:
							case 2:
							case 3:
							case 4:
							case 5:
								num23 = 1;
								break;
							case 6:
								npc.ai[3] = 1f;
								num23 = 2;
								break;
							case 7:
								npc.ai[3] = 0f;
								num23 = 3;
								break;
						}

						if (phase2)
							num23 = 4;

						// Set velocity for charge
						if (num23 == 1)
						{
							npc.ai[0] = 1f;
							npc.ai[1] = 0f;
							npc.ai[2] = 0f;

							// Velocity
							Vector2 distanceVector = player.Center + player.velocity * 20f - vector;
							npc.velocity = Vector2.Normalize(distanceVector) * chargeVelocity;
							npc.rotation = (float)Math.Atan2(npc.velocity.Y, npc.velocity.X);

							// Direction
							if (num22 != 0)
							{
								npc.direction = num22;

								if (npc.spriteDirection == 1)
									npc.rotation += pie;

								npc.spriteDirection = -npc.direction;
							}
						}

						// Tooth Balls
						else if (num23 == 2)
						{
							npc.ai[0] = 2f;
							npc.ai[1] = 0f;
							npc.ai[2] = 0f;
						}

						// Call sharks from the sides of the screen
						else if (num23 == 3)
						{
							npc.ai[0] = 3f;
							npc.ai[1] = 0f;
							npc.ai[2] = 0f;
						}

						// Go to phase 2
						else if (num23 == 4)
						{
							npc.ai[0] = 4f;
							npc.ai[1] = 0f;
							npc.ai[2] = 0f;
						}

						npc.netUpdate = true;
					}
				}
			}

			// Charge
			else if (npc.ai[0] == 1f)
			{
				// Accelerate
				npc.velocity *= 1.01f;

				// Spawn dust
				int num24 = 7;
				for (int j = 0; j < num24; j++)
				{
					Vector2 arg_E1C_0 = (Vector2.Normalize(npc.velocity) * new Vector2((npc.width + 50) / 2f, npc.height) * 0.75f).RotatedBy((j - (num24 / 2 - 1)) * pie / num24) + vector;
					Vector2 vector4 = ((float)(Main.rand.NextDouble() * pie) - MathHelper.PiOver2).ToRotationVector2() * Main.rand.Next(3, 8);
					int num25 = Dust.NewDust(arg_E1C_0 + vector4, 0, 0, (int)CalamityDusts.SulfurousSeaAcid, vector4.X * 2f, vector4.Y * 2f, 100, default, 1.4f);
					Main.dust[num25].noGravity = true;
					Main.dust[num25].noLight = true;
					Main.dust[num25].velocity /= 4f;
					Main.dust[num25].velocity -= npc.velocity;
				}

				npc.ai[2] += 1f;
				if (npc.ai[2] >= chargeTime)
				{
					calamityGlobalNPC.newAI[0] += 30f;
					npc.ai[0] = 0f;
					npc.ai[1] = 0f;
					npc.ai[2] = 0f;
					npc.ai[3] += 2f;
					npc.netUpdate = true;
				}
			}

			// Tooth Ball belch
			else if (npc.ai[0] == 2f)
			{
				// Velocity
				if (npc.ai[1] == 0f)
					npc.ai[1] = 500 * Math.Sign((vector - player.Center).X);

				Vector2 vector5 = Vector2.Normalize(player.Center + new Vector2(npc.ai[1], -300f) - vector - npc.velocity) * scaleFactor2;
				if (npc.velocity.X < vector5.X)
				{
					npc.velocity.X += num8;
					if (npc.velocity.X < 0f && vector5.X > 0f)
						npc.velocity.X += num8;
				}
				else if (npc.velocity.X > vector5.X)
				{
					npc.velocity.X -= num8;
					if (npc.velocity.X > 0f && vector5.X < 0f)
						npc.velocity.X -= num8;
				}
				if (npc.velocity.Y < vector5.Y)
				{
					npc.velocity.Y += num8;
					if (npc.velocity.Y < 0f && vector5.Y > 0f)
						npc.velocity.Y += num8;
				}
				else if (npc.velocity.Y > vector5.Y)
				{
					npc.velocity.Y -= num8;
					if (npc.velocity.Y > 0f && vector5.Y < 0f)
						npc.velocity.Y -= num8;
				}

				// Play sounds and spawn Tooth Balls
				if (npc.ai[2] == 0f)
					Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/OldDukeRoar"), (int)npc.position.X, (int)npc.position.Y);

				if (npc.ai[2] % num7 == 0f)
				{
					if (npc.ai[2] % 40f == 0f && npc.ai[2] != 0f)
						Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/OldDukeVomit"), (int)npc.position.X, (int)npc.position.Y);

					if (Main.netMode != NetmodeID.MultiplayerClient)
					{
						Vector2 vector6 = Vector2.Normalize(player.Center - vector) * (npc.width + 20) / 2f + vector;
						NPC.NewNPC((int)vector6.X, (int)vector6.Y + 45, ModContent.NPCType<OldDukeToothBall>(), 0, 0f, 0f, 0f, 0f, 255);
					}
				}

				// Direction
				int num26 = Math.Sign(player.Center.X - vector.X);
				if (num26 != 0)
				{
					npc.direction = num26;
					if (npc.spriteDirection != -npc.direction)
						npc.rotation += pie;
					npc.spriteDirection = -npc.direction;
				}

				npc.ai[2] += 1f;
				if (npc.ai[2] >= num6)
				{
					calamityGlobalNPC.newAI[0] += 30f;
					npc.ai[0] = 0f;
					npc.ai[1] = 0f;
					npc.ai[2] = 0f;
					npc.netUpdate = true;
				}
			}

			// Call sharks from the sides of the screen
			else if (npc.ai[0] == 3f)
			{
				// Velocity
				npc.velocity *= 0.98f;
				npc.velocity.Y = MathHelper.Lerp(npc.velocity.Y, 0f, 0.02f);

				// Play sound and spawn sharks
				if (npc.ai[2] == num9 - 30)
					Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/OldDukeVomit"), (int)npc.position.X, (int)npc.position.Y);

				if (npc.ai[2] >= num9 - 90)
				{
					if (Main.netMode != NetmodeID.MultiplayerClient && npc.ai[2] % 18f == 0f)
					{
						calamityGlobalNPC.newAI[2] += 100f;
						NPC.NewNPC((int)(vector.X + 900f), (int)(vector.Y - calamityGlobalNPC.newAI[2]), ModContent.NPCType<OldDukeSharkron>(), 0, 0f, 0f, npc.whoAmI, 0f, 255);
						NPC.NewNPC((int)(vector.X - 900f), (int)(vector.Y - calamityGlobalNPC.newAI[2]), ModContent.NPCType<OldDukeSharkron>(), 0, 0f, 0f, npc.whoAmI, 0f, 255);
					}
				}

				npc.ai[2] += 1f;
				if (npc.ai[2] >= num9)
				{
					calamityGlobalNPC.newAI[0] += 30f;
					calamityGlobalNPC.newAI[2] = 0f;
					npc.ai[0] = 0f;
					npc.ai[1] = 0f;
					npc.ai[2] = 0f;
					npc.netUpdate = true;
				}
			}

			// Transition to phase 2 and call sharks from below
			else if (npc.ai[0] == 4f)
			{
				// Velocity
				npc.velocity *= 0.98f;
				npc.velocity.Y = MathHelper.Lerp(npc.velocity.Y, 0f, 0.02f);

				// Sound
				if (npc.ai[2] == num10 - 60)
					Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/OldDukeRoar"), (int)npc.position.X, (int)npc.position.Y);

				if (npc.ai[2] >= num10 - 60)
				{
					if (Main.netMode != NetmodeID.MultiplayerClient && npc.ai[2] % 18f == 0f)
					{
						calamityGlobalNPC.newAI[2] += 150f;
						NPC.NewNPC((int)(vector.X + 50f + calamityGlobalNPC.newAI[2]), (int)(vector.Y + 540f), ModContent.NPCType<OldDukeSharkron>(), 0, 0f, 0f, 1f, -12f, 255);
						NPC.NewNPC((int)(vector.X - 50f - calamityGlobalNPC.newAI[2]), (int)(vector.Y + 540f), ModContent.NPCType<OldDukeSharkron>(), 0, 0f, 0f, -1f, -12f, 255);
					}
				}

				npc.ai[2] += 1f;
				if (npc.ai[2] >= num10)
				{
					calamityGlobalNPC.newAI[0] = 0f;
					calamityGlobalNPC.newAI[1] = 0f;
					calamityGlobalNPC.newAI[2] = 0f;
					npc.ai[0] = 5f;
					npc.ai[1] = 0f;
					npc.ai[2] = 0f;
					npc.ai[3] = 0f;
					npc.netUpdate = true;
				}
			}

			// Phase 2
			else if (npc.ai[0] == 5f && !player.dead)
			{
				// Velocity
				if (npc.ai[1] == 0f)
					npc.ai[1] = 500 * Math.Sign((vector - player.Center).X);

				Vector2 vector8 = Vector2.Normalize(player.Center + new Vector2(npc.ai[1], -300f) - vector - npc.velocity) * scaleFactor;
				if (npc.velocity.X < vector8.X)
				{
					npc.velocity.X += num3;
					if (npc.velocity.X < 0f && vector8.X > 0f)
						npc.velocity.X += num3;
				}
				else if (npc.velocity.X > vector8.X)
				{
					npc.velocity.X -= num3;
					if (npc.velocity.X > 0f && vector8.X < 0f)
						npc.velocity.X -= num3;
				}
				if (npc.velocity.Y < vector8.Y)
				{
					npc.velocity.Y += num3;
					if (npc.velocity.Y < 0f && vector8.Y > 0f)
						npc.velocity.Y += num3;
				}
				else if (npc.velocity.Y > vector8.Y)
				{
					npc.velocity.Y -= num3;
					if (npc.velocity.Y > 0f && vector8.Y < 0f)
						npc.velocity.Y -= num3;
				}

				// Direction and rotation
				int num27 = Math.Sign(player.Center.X - vector.X);
				if (num27 != 0)
				{
					if (npc.ai[2] == 0f && num27 != npc.direction)
						npc.rotation += pie;

					npc.direction = num27;

					if (npc.spriteDirection != -npc.direction)
						npc.rotation += pie;

					npc.spriteDirection = -npc.direction;
				}

				// Phase switch
				if (calamityGlobalNPC.newAI[1] != 1f || (phase3 && !phase3AI))
				{
					npc.ai[2] += 1f;
					if (npc.ai[2] >= num2 || (phase3 && !phase3AI))
					{
						int num28 = 0;
						switch ((int)npc.ai[3])
						{
							case 0:
							case 1:
							case 2:
							case 3:
								num28 = 1;
								break;
							case 4:
								npc.ai[3] = 1f;
								num28 = 2;
								break;
							case 5:
								npc.ai[3] = 0f;
								num28 = 3;
								break;
						}

						if (phase3)
							num28 = 4;

						// Set velocity for charge
						if (num28 == 1)
						{
							npc.ai[0] = 6f;
							npc.ai[1] = 0f;
							npc.ai[2] = 0f;

							// Velocity and rotation
							Vector2 distanceVector = player.Center + player.velocity * 20f - vector;
							npc.velocity = Vector2.Normalize(distanceVector) * chargeVelocity;
							npc.rotation = (float)Math.Atan2(npc.velocity.Y, npc.velocity.X);

							// Direction
							if (num27 != 0)
							{
								npc.direction = num27;

								if (npc.spriteDirection == 1)
									npc.rotation += pie;

								npc.spriteDirection = -npc.direction;
							}
						}

						// Set velocity for spin
						else if (num28 == 2)
						{
							// Velocity and rotation
							npc.velocity = Vector2.Normalize(player.Center - vector) * scaleFactor4;
							npc.rotation = (float)Math.Atan2(npc.velocity.Y, npc.velocity.X);

							// Direction
							if (num27 != 0)
							{
								npc.direction = num27;

								if (npc.spriteDirection == 1)
									npc.rotation += pie;

								npc.spriteDirection = -npc.direction;
							}

							npc.ai[0] = 7f;
							npc.ai[1] = 0f;
							npc.ai[2] = 0f;
						}

						else if (num28 == 3)
						{
							npc.ai[0] = 8f;
							npc.ai[1] = 0f;
							npc.ai[2] = 0f;
						}

						// Go to next phase
						else if (num28 == 4)
						{
							npc.ai[0] = 9f;
							npc.ai[1] = 0f;
							npc.ai[2] = 0f;
						}

						npc.netUpdate = true;
					}
				}
			}

			// Charge
			else if (npc.ai[0] == 6f)
			{
				// Accelerate
				npc.velocity *= 1.01f;

				// Spawn dust
				int num29 = 7;
				for (int k = 0; k < num29; k++)
				{
					Vector2 arg_1A97_0 = (Vector2.Normalize(npc.velocity) * new Vector2((npc.width + 50) / 2f, npc.height) * 0.75f).RotatedBy((k - (num29 / 2 - 1)) * pie / num29) + vector;
					Vector2 vector9 = ((float)(Main.rand.NextDouble() * pie) - MathHelper.PiOver2).ToRotationVector2() * Main.rand.Next(3, 8);
					int num30 = Dust.NewDust(arg_1A97_0 + vector9, 0, 0, (int)CalamityDusts.SulfurousSeaAcid, vector9.X * 2f, vector9.Y * 2f, 100, default, 1.4f);
					Main.dust[num30].noGravity = true;
					Main.dust[num30].noLight = true;
					Main.dust[num30].velocity /= 4f;
					Main.dust[num30].velocity -= npc.velocity;
				}

				npc.ai[2] += 1f;
				if (npc.ai[2] >= chargeTime)
				{
					calamityGlobalNPC.newAI[0] += 30f;
					npc.ai[0] = 5f;
					npc.ai[1] = 0f;
					npc.ai[2] = 0f;
					npc.ai[3] += 2f;
					npc.netUpdate = true;
				}
			}

			// Tooth Ball and Vortex spin
			else if (npc.ai[0] == 7f)
			{
				// Play sounds and spawn Tooth Balls and a Vortex
				if (npc.ai[2] == 0f)
				{
					Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/OldDukeRoar"), (int)npc.position.X, (int)npc.position.Y);

					int damage = expertMode ? 100 : 140;
					Vector2 vortexSpawn = vector + npc.velocity.RotatedBy(MathHelper.PiOver2 * -npc.direction) * spinTime / MathHelper.TwoPi;
					if (Main.netMode != NetmodeID.MultiplayerClient)
					{
						Projectile.NewProjectile(vortexSpawn.X, vortexSpawn.Y, 0f, 0f, ModContent.ProjectileType<OldDukeVortex>(), damage, 0f, Main.myPlayer, vortexSpawn.X, vortexSpawn.Y);
					}
				}

				if (npc.ai[2] % num14 == 0f)
				{
					if (npc.ai[2] % 45f == 0f && npc.ai[2] != 0f)
						Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/OldDukeVomit"), (int)npc.position.X, (int)npc.position.Y);

					if (Main.netMode != NetmodeID.MultiplayerClient)
					{
						Vector2 vector10 = Vector2.Normalize(npc.velocity) * (npc.width + 20) / 2f + vector;
						int num31 = NPC.NewNPC((int)vector10.X, (int)vector10.Y + 45, ModContent.NPCType<OldDukeToothBall>(), 0, 0f, 0f, 0f, 0f, 255);
						Main.npc[num31].target = npc.target;
						Main.npc[num31].velocity = Vector2.Normalize(npc.velocity).RotatedBy(MathHelper.PiOver2 * npc.direction) * scaleFactor3;
						Main.npc[num31].netUpdate = true;
						Main.npc[num31].ai[3] = Main.rand.Next(30, 181);
					}
				}

				// Velocity and rotation
				npc.velocity = npc.velocity.RotatedBy(-(double)num15 * (float)npc.direction);
				npc.rotation -= num15 * npc.direction;

				npc.ai[2] += 1f;
				if (npc.ai[2] >= num13)
				{
					calamityGlobalNPC.newAI[0] += 30f;
					npc.ai[0] = 5f;
					npc.ai[1] = 0f;
					npc.ai[2] = 0f;
					npc.netUpdate = true;
				}
			}

			// Vomit a huge amount of gore into the sky and call sharks from the sides of the screen
			else if (npc.ai[0] == 8f)
			{
				// Velocity
				npc.velocity *= 0.98f;
				npc.velocity.Y = MathHelper.Lerp(npc.velocity.Y, 0f, 0.02f);

				// Play sound
				if (npc.ai[2] == num9 - 30)
				{
					Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/OldDukeVomit"), (int)npc.position.X, (int)npc.position.Y);

					if (Main.netMode != NetmodeID.MultiplayerClient)
					{
						Vector2 vector7 = npc.rotation.ToRotationVector2() * (Vector2.UnitX * npc.direction) * (npc.width + 20) / 2f + vector;
						int damage = expertMode ? 55 : 70;
						for (int i = 0; i < 20; i++)
						{
							float velocityX = npc.direction * 6 * (Main.rand.NextFloat() + 0.5f);
							float velocityY = 8f * (Main.rand.NextFloat() + 0.5f);
							Projectile.NewProjectile(vector7.X, vector7.Y, velocityX, -velocityY, ModContent.ProjectileType<OldDukeGore>(), damage, 0f, Main.myPlayer, 0f, 0f);
						}
					}
				}

				if (npc.ai[2] >= num9 - 90)
				{
					if (Main.netMode != NetmodeID.MultiplayerClient && npc.ai[2] % 18f == 0f)
					{
						calamityGlobalNPC.newAI[2] += 100f;
						float x = 900f - calamityGlobalNPC.newAI[2];
						NPC.NewNPC((int)(vector.X + x), (int)(vector.Y - calamityGlobalNPC.newAI[2]), ModContent.NPCType<OldDukeSharkron>(), 0, 0f, 0f, npc.whoAmI, 0f, 255);
						NPC.NewNPC((int)(vector.X - x), (int)(vector.Y - calamityGlobalNPC.newAI[2]), ModContent.NPCType<OldDukeSharkron>(), 0, 0f, 0f, npc.whoAmI, 0f, 255);
					}
				}

				npc.ai[2] += 1f;
				if (npc.ai[2] >= num9)
				{
					calamityGlobalNPC.newAI[0] += 30f;
					calamityGlobalNPC.newAI[2] = 0f;
					npc.ai[0] = 5f;
					npc.ai[1] = 0f;
					npc.ai[2] = 0f;
					npc.netUpdate = true;
				}
			}

			// Transition to phase 3 and summon sharks from above
			else if (npc.ai[0] == 9f)
			{
				// Velocity
				npc.velocity *= 0.98f;
				npc.velocity.Y = MathHelper.Lerp(npc.velocity.Y, 0f, 0.02f);

				// Sound
				if (npc.ai[2] == num11 - 60)
					Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/OldDukeRoar"), (int)npc.position.X, (int)npc.position.Y);

				if (npc.ai[2] >= num11 - 60)
				{
					if (Main.netMode != NetmodeID.MultiplayerClient && npc.ai[2] % 18f == 0f)
					{
						calamityGlobalNPC.newAI[2] += 200f;
						NPC.NewNPC((int)(vector.X + 50f + calamityGlobalNPC.newAI[2]), (int)(vector.Y - 540f), ModContent.NPCType<OldDukeSharkron>(), 0, 0f, 0f, 1f, 12f, 255);
						NPC.NewNPC((int)(vector.X - 50f - calamityGlobalNPC.newAI[2]), (int)(vector.Y - 540f), ModContent.NPCType<OldDukeSharkron>(), 0, 0f, 0f, -1f, 12f, 255);
					}
				}

				npc.ai[2] += 1f;
				if (npc.ai[2] >= num11)
				{
					calamityGlobalNPC.newAI[0] = 0f;
					calamityGlobalNPC.newAI[1] = 0f;
					calamityGlobalNPC.newAI[2] = 0f;
					npc.ai[0] = 10f;
					npc.ai[1] = 0f;
					npc.ai[2] = 0f;
					npc.ai[3] = 0f;
					npc.netUpdate = true;
				}
			}

			// Phase 3
			else if (npc.ai[0] == 10f && !player.dead)
			{
				// Alpha
				npc.alpha -= 25;
				if (npc.alpha < 0)
					npc.alpha = 0;

				// Teleport location
				if (npc.ai[1] == 0f)
					npc.ai[1] = 540 * Math.Sign((vector - player.Center).X);

				Vector2 desiredVelocity = Vector2.Normalize(player.Center + new Vector2(-npc.ai[1], -300f) - vector - npc.velocity) * scaleFactor;
				npc.SimpleFlyMovement(desiredVelocity, num3);

				// Rotation and direction
				int num32 = Math.Sign(player.Center.X - vector.X);
				if (num32 != 0)
				{
					if (npc.ai[2] == 0f && num32 != npc.direction)
					{
						npc.rotation += pie;
						for (int l = 0; l < npc.oldPos.Length; l++)
							npc.oldPos[l] = Vector2.Zero;
					}

					npc.direction = num32;

					if (npc.spriteDirection != -npc.direction)
						npc.rotation += pie;

					npc.spriteDirection = -npc.direction;
				}

				// Phase switch
				if (calamityGlobalNPC.newAI[1] != 1f)
				{
					npc.ai[2] += 1f;
					if (npc.ai[2] >= num2)
					{
						int num33 = 0;
						switch ((int)npc.ai[3])
						{
							case 0:
							case 2:
							case 3:
							case 5:
							case 6:
							case 7:
								num33 = 1;
								break;
							case 1:
							case 8:
								num33 = 2;
								break;
							case 4:
								npc.ai[3] = 1f;
								num33 = 3;
								break;
							case 9:
								npc.ai[3] = 6f;
								num33 = 4;
								break;
						}

						// Set velocity for charge
						if (num33 == 1)
						{
							npc.ai[0] = 11f;
							npc.ai[1] = 0f;
							npc.ai[2] = 0f;

							// Velocity and rotation
							Vector2 distanceVector = player.Center + player.velocity * 20f - vector;
							npc.velocity = Vector2.Normalize(distanceVector) * chargeVelocity;
							npc.rotation = (float)Math.Atan2(npc.velocity.Y, npc.velocity.X);

							// Direction
							if (num32 != 0)
							{
								npc.direction = num32;

								if (npc.spriteDirection == 1)
									npc.rotation += pie;

								npc.spriteDirection = -npc.direction;
							}
						}

						// Pause
						else if (num33 == 2)
						{
							npc.ai[0] = 12f;
							npc.ai[1] = 0f;
							npc.ai[2] = 0f;
						}

						else if (num33 == 3)
						{
							npc.ai[0] = 13f;
							npc.ai[1] = 0f;
							npc.ai[2] = 0f;
						}

						// Set velocity for spin
						else if (num33 == 4)
						{
							// Velocity and rotation
							npc.velocity = Vector2.Normalize(player.Center - vector) * scaleFactor4;
							npc.rotation = (float)Math.Atan2(npc.velocity.Y, npc.velocity.X);

							// Direction
							if (num32 != 0)
							{
								npc.direction = num32;

								if (npc.spriteDirection == 1)
									npc.rotation += pie;

								npc.spriteDirection = -npc.direction;
							}

							npc.ai[0] = 14f;
							npc.ai[1] = 0f;
							npc.ai[2] = 0f;
						}

						npc.netUpdate = true;
					}
				}
			}

			// Charge
			else if (npc.ai[0] == 11f)
			{
				// Accelerate
				npc.velocity *= 1.01f;

				// Spawn dust
				int num34 = 7;
				for (int m = 0; m < num34; m++)
				{
					Vector2 arg_2444_0 = (Vector2.Normalize(npc.velocity) * new Vector2((npc.width + 50) / 2f, npc.height) * 0.75f).RotatedBy((m - (num34 / 2 - 1)) * pie / num34) + vector;
					Vector2 vector11 = ((float)(Main.rand.NextDouble() * pie) - MathHelper.PiOver2).ToRotationVector2() * Main.rand.Next(3, 8);
					int num35 = Dust.NewDust(arg_2444_0 + vector11, 0, 0, (int)CalamityDusts.SulfurousSeaAcid, vector11.X * 2f, vector11.Y * 2f, 100, default, 1.4f);
					Main.dust[num35].noGravity = true;
					Main.dust[num35].noLight = true;
					Main.dust[num35].velocity /= 4f;
					Main.dust[num35].velocity -= npc.velocity;
				}

				npc.ai[2] += 1f;
				if (npc.ai[2] >= chargeTime)
				{
					calamityGlobalNPC.newAI[0] += 30f;
					npc.ai[0] = 10f;
					npc.ai[1] = 0f;
					npc.ai[2] = 0f;
					npc.ai[3] += 2f;
					npc.netUpdate = true;
				}
			}

			// Pause before teleport
			else if (npc.ai[0] == 12f)
			{
				npc.dontTakeDamage = true;

				// Alpha
				if (npc.alpha < 255 && npc.ai[2] >= num12 - 15f)
				{
					npc.alpha += 17;
					if (npc.alpha > 255)
						npc.alpha = 255;
				}

				// Velocity
				npc.velocity *= 0.98f;
				npc.velocity.Y = MathHelper.Lerp(npc.velocity.Y, 0f, 0.02f);

				// Play sound
				if (npc.ai[2] == num12 / 2)
					Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/OldDukeRoar"), (int)npc.position.X, (int)npc.position.Y);

				if (Main.netMode != NetmodeID.MultiplayerClient && npc.ai[2] == num12 / 2)
				{
					// Teleport location
					if (npc.ai[1] == 0f)
						npc.ai[1] = 480 * Math.Sign((vector - player.Center).X);

					// Rotation and direction
					Vector2 center = player.Center + new Vector2(npc.ai[1], -300f);
					vector = npc.Center = center;
					int num36 = Math.Sign(player.Center.X - vector.X);
					if (num36 != 0)
					{
						if (npc.ai[2] == 0f && num36 != npc.direction)
						{
							npc.rotation += pie;
							for (int n = 0; n < npc.oldPos.Length; n++)
								npc.oldPos[n] = Vector2.Zero;
						}

						npc.direction = num36;

						if (npc.spriteDirection != -npc.direction)
							npc.rotation += pie;

						npc.spriteDirection = -npc.direction;
					}
				}

				npc.ai[2] += 1f;
				if (npc.ai[2] >= num12)
				{
					npc.ai[0] = 10f;
					npc.ai[1] = 0f;
					npc.ai[2] = 0f;

					npc.ai[3] += 2f;
					if (npc.ai[3] >= 9f)
						npc.ai[3] = 0f;

					npc.dontTakeDamage = false;

					npc.netUpdate = true;
				}
			}

			// Vomit a huge amount of gore into the sky and call sharks from the sides of the screen
			else if (npc.ai[0] == 13f)
			{
				// Velocity
				npc.velocity *= 0.98f;
				npc.velocity.Y = MathHelper.Lerp(npc.velocity.Y, 0f, 0.02f);

				// Play sound
				if (npc.ai[2] == num9 - 30)
				{
					Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/OldDukeVomit"), (int)npc.position.X, (int)npc.position.Y);

					if (Main.netMode != NetmodeID.MultiplayerClient)
					{
						Vector2 vector7 = npc.rotation.ToRotationVector2() * (Vector2.UnitX * npc.direction) * (npc.width + 20) / 2f + vector;
						int damage = expertMode ? 55 : 70;
						for (int i = 0; i < 20; i++)
						{
							float velocityX = npc.direction * 6 * (Main.rand.NextFloat() + 0.5f);
							float velocityY = 8f * (Main.rand.NextFloat() + 0.5f);
							Projectile.NewProjectile(vector7.X, vector7.Y, velocityX, -velocityY, ModContent.ProjectileType<OldDukeGore>(), damage, 0f, Main.myPlayer, 0f, 0f);
						}
					}
				}

				if (npc.ai[2] >= num9 - 90)
				{
					if (Main.netMode != NetmodeID.MultiplayerClient && npc.ai[2] % 18f == 0f)
					{
						calamityGlobalNPC.newAI[2] += 150f;
						float x = 900f - calamityGlobalNPC.newAI[2];
						NPC.NewNPC((int)(vector.X + x), (int)(vector.Y - calamityGlobalNPC.newAI[2]), ModContent.NPCType<OldDukeSharkron>(), 0, 0f, 0f, npc.whoAmI, 0f, 255);
						NPC.NewNPC((int)(vector.X - x), (int)(vector.Y - calamityGlobalNPC.newAI[2]), ModContent.NPCType<OldDukeSharkron>(), 0, 0f, 0f, npc.whoAmI, 0f, 255);
					}
				}

				npc.ai[2] += 1f;
				if (npc.ai[2] >= num9)
				{
					calamityGlobalNPC.newAI[0] += 30f;
					calamityGlobalNPC.newAI[2] = 0f;
					npc.ai[0] = 10f;
					npc.ai[1] = 0f;
					npc.ai[2] = 0f;
					npc.netUpdate = true;
				}
			}

			// Tooth Ball and Vortex spin
			else if (npc.ai[0] == 14f)
			{
				// Play sounds and spawn Tooth Balls and a Vortex
				if (npc.ai[2] == 0f)
				{
					Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/OldDukeRoar"), (int)npc.position.X, (int)npc.position.Y);

					int damage = expertMode ? 100 : 140;
					Vector2 vortexSpawn = vector + npc.velocity.RotatedBy(MathHelper.PiOver2 * -npc.direction) * spinTime / MathHelper.TwoPi;
					if (Main.netMode != NetmodeID.MultiplayerClient)
					{
						Projectile.NewProjectile(vortexSpawn.X, vortexSpawn.Y, 0f, 0f, ModContent.ProjectileType<OldDukeVortex>(), damage, 0f, Main.myPlayer, vortexSpawn.X, vortexSpawn.Y);
					}
				}

				if (npc.ai[2] % num14 == 0f)
				{
					if (npc.ai[2] % 45f == 0f && npc.ai[2] != 0f)
						Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/OldDukeVomit"), (int)npc.position.X, (int)npc.position.Y);

					if (Main.netMode != NetmodeID.MultiplayerClient)
					{
						Vector2 vector10 = Vector2.Normalize(npc.velocity) * (float)(npc.width + 20) / 2f + vector;
						int num31 = NPC.NewNPC((int)vector10.X, (int)vector10.Y + 45, ModContent.NPCType<OldDukeToothBall>(), 0, 0f, 0f, 0f, 0f, 255);
						Main.npc[num31].target = npc.target;
						Main.npc[num31].velocity = Vector2.Normalize(npc.velocity).RotatedBy(MathHelper.PiOver2 * npc.direction) * scaleFactor3;
						Main.npc[num31].netUpdate = true;
						Main.npc[num31].ai[3] = Main.rand.Next(30, 361);
					}
				}

				// Velocity and rotation
				npc.velocity = npc.velocity.RotatedBy(-(double)num15 * (float)npc.direction);
				npc.rotation -= num15 * npc.direction;

				npc.ai[2] += 1f;
				if (npc.ai[2] >= num13)
				{
					calamityGlobalNPC.newAI[0] += 30f;
					npc.ai[0] = 10f;
					npc.ai[1] = 0f;
					npc.ai[2] = 0f;
					npc.netUpdate = true;
				}
			}
		}
		#endregion

		// This AI is for testing purposes, don't remove it
		#region Brimstone Elemental2
		public static void BrimstoneElementalAI2(NPC npc, Mod mod)
		{
			CalamityGlobalNPC calamityGlobalNPC = npc.Calamity();

			// Used for Brimling AI states
			CalamityGlobalNPC.brimstoneElemental = npc.whoAmI;

			// Center
			Vector2 vectorCenter = npc.Center;

			// Get a target
			if (npc.target < 0 || npc.target == 255 || Main.player[npc.target].dead || !Main.player[npc.target].active)
				npc.TargetClosest(true);

			Player player = Main.player[npc.target];
			if (!player.active || player.dead || Vector2.Distance(player.Center, vectorCenter) > 5600f)
			{
				npc.TargetClosest(false);
				player = Main.player[npc.target];
				if (!player.active || player.dead || Vector2.Distance(player.Center, vectorCenter) > 5600f)
				{
					npc.rotation = npc.velocity.X * 0.04f;

					if (npc.velocity.Y > 3f)
						npc.velocity.Y = 3f;
					npc.velocity.Y -= 0.1f;
					if (npc.velocity.Y < -12f)
						npc.velocity.Y = -12f;

					if (npc.timeLeft > 60)
						npc.timeLeft = 60;

					if (npc.ai[0] != 0f)
					{
						npc.ai[0] = 0f;
						npc.ai[1] = 0f;
						npc.ai[2] = 0f;
						npc.ai[3] = 0f;
						npc.localAI[0] = 0f;
						npc.localAI[1] = 0f;
						npc.netUpdate = true;
					}
					return;
				}
			}
			else if (npc.timeLeft < 1800)
				npc.timeLeft = 1800;

			CalamityPlayer modPlayer = player.Calamity();

			// Reset defense
			npc.defense = npc.defDefense;

			// Hide the sprite
			npc.alpha = 255;

			// Percent life remaining
			float lifeRatio = npc.life / (float)npc.lifeMax;

			// Variables for buffing the AI
			bool provy = CalamityWorld.downedProvidence && !CalamityWorld.bossRushActive;
			bool expertMode = Main.expertMode || CalamityWorld.bossRushActive;
			bool revenge = CalamityWorld.revenge || CalamityWorld.bossRushActive;
			bool death = CalamityWorld.death || CalamityWorld.bossRushActive;
			bool calamity = modPlayer.ZoneCalamity;
			bool phase2 = (lifeRatio < 0.5f && revenge) || death;

			// Speed while moving in phase 1
			float speed = expertMode ? 5f : 4.5f;
			if (CalamityWorld.bossRushActive)
				speed = 12f;
			else if (!calamity)
				speed = 7f;
			else if (death)
				speed = 6f;
			else if (revenge)
				speed = 5.5f;
			float speedBoost = death ? 2f : 2f * (1f - lifeRatio);
			speed += speedBoost;

			int damage = expertMode ? 125 : 200;
			float radialOffset = 32f;
			float xDistance = player.Center.X - vectorCenter.X;
			float yDistance = player.Center.Y - vectorCenter.Y;
			float totalDistance = (float)Math.Sqrt(xDistance * xDistance + yDistance * yDistance);

			// Static movement towards target
			if (npc.ai[0] == 5f)
			{
				npc.rotation = npc.velocity.X * 0.04f;
				if (npc.ai[1] < 180f)
				{
					float playerLocation = vectorCenter.X - player.Center.X;
					npc.direction = playerLocation < 0f ? 1 : -1;
					npc.spriteDirection = npc.direction;
					totalDistance = speed * 0.15f / totalDistance;
					xDistance *= totalDistance;
					yDistance *= totalDistance;
					npc.velocity.X = (npc.velocity.X * 50f + xDistance) / 51f;
					npc.velocity.Y = (npc.velocity.Y * 50f + yDistance) / 51f;
				}
			}

			if (npc.ai[0] == 0f)
			{
				npc.chaseable = true;
				npc.rotation = npc.velocity.X * 0.04f;
				float playerLocation = vectorCenter.X - player.Center.X;
				npc.direction = playerLocation < 0f ? 1 : -1;
				npc.spriteDirection = npc.direction;
				npc.ai[1] += 1f;

				bool shootProjectile = false;
				float fireRate = 60f;
				if (npc.ai[1] % fireRate == fireRate - 1f)
					shootProjectile = true;

				if (shootProjectile)
				{
					if (Main.netMode != NetmodeID.MultiplayerClient)
					{
						float num742 = 16f;
						float num743 = player.position.X + player.width * 0.5f - vectorCenter.X;
						float num744 = player.position.Y + player.height * 0.5f - vectorCenter.Y;
						float num745 = (float)Math.Sqrt(num743 * num743 + num744 * num744);

						num745 = num742 / num745;
						num743 *= num745;
						num744 *= num745;

						vectorCenter.X += num743 * 3f;
						vectorCenter.Y += num744 * 3f;

						float radians = MathHelper.TwoPi / 3f;

						for (int i = 0; i < 3; i++)
						{
							Vector2 vector = new Vector2(0f, -3f).RotatedBy(radians * i);
							int proj = Projectile.NewProjectile(vectorCenter, vector, ModContent.ProjectileType<DarkOrb>(), damage, 0f, Main.myPlayer, 8f, 0f);
							Main.projectile[proj].scale = 4f;
						}

						//Projectile.NewProjectile(vectorCenter.X, vectorCenter.Y, num743, num744, ModContent.ProjectileType<DarkOrb>(), damage, 0f, Main.myPlayer, 4f, 0f);

						int numProj = 4;
						int spread = 45;
						float rotation = MathHelper.ToRadians(spread);
						float baseSpeed = (float)Math.Sqrt(num743 * num743 + num744 * num744);
						double startAngle = Math.Atan2(num743, num744) - rotation / 2;
						double deltaAngle = rotation / numProj;
						double offsetAngle;

						for (int i = 0; i < numProj; i++)
						{
							offsetAngle = startAngle + deltaAngle * i;
							Projectile.NewProjectile(vectorCenter.X, vectorCenter.Y, baseSpeed * (float)Math.Sin(offsetAngle), baseSpeed * (float)Math.Cos(offsetAngle), ModContent.ProjectileType<DarkOrb>(), damage, 0f, Main.myPlayer, 10f, 0f);
						}
					}
				}

				if (npc.position.Y > player.position.Y - 150f) //200
				{
					if (npc.velocity.Y > 0f)
						npc.velocity.Y *= 0.98f;

					npc.velocity.Y -= CalamityWorld.bossRushActive ? 0.15f : 0.1f;

					if (npc.velocity.Y > 3f)
						npc.velocity.Y = 3f;
				}
				else if (npc.position.Y < player.position.Y - 350f) //500
				{
					if (npc.velocity.Y < 0f)
						npc.velocity.Y *= 0.98f;

					npc.velocity.Y += CalamityWorld.bossRushActive ? 0.15f : 0.1f;

					if (npc.velocity.Y < -3f)
						npc.velocity.Y = -3f;
				}
				if (npc.position.X + (npc.width / 2) > player.position.X + (player.width / 2) + 150f) //100
				{
					if (npc.velocity.X > 0f)
						npc.velocity.X *= 0.985f;

					npc.velocity.X -= CalamityWorld.bossRushActive ? 0.15f : 0.1f;

					if (npc.velocity.X > 8f)
						npc.velocity.X = 8f;
				}
				if (npc.position.X + (npc.width / 2) < player.position.X + (player.width / 2) - 150f) //100
				{
					if (npc.velocity.X < 0f)
						npc.velocity.X *= 0.985f;

					npc.velocity.X += (CalamityWorld.bossRushActive ? 0.15f : 0.1f);

					if (npc.velocity.X < -8f)
						npc.velocity.X = -8f;
				}

				if (npc.ai[1] >= 300f)
				{
					npc.TargetClosest(true);
					npc.ai[0] = 1f;
					npc.ai[1] = 0f;
					npc.ai[2] = 0f;
					npc.ai[3] = 0f;
					npc.netUpdate = true;
				}
			}

			// Cocoon bullet hell
			else if (npc.ai[0] == 1f)
			{
				npc.defense = 99999;
				npc.chaseable = false;
				if (Main.netMode != NetmodeID.MultiplayerClient)
				{
					npc.localAI[0] += 1f;

					float projectileSpeed = revenge ? 8f : 6f;
					if (CalamityWorld.bossRushActive)
						projectileSpeed = 12f;

					Vector2 targetVector = player.Center - vectorCenter;
					float offsetAngle = 0f;
					int totalProjectiles = 0;
					int chains = 0;

					// Parabolic Arcs
					if (npc.localAI[0] % 60f == 0f)
					{
						radialOffset = 0.1f;
						float diameter = 90f;

						// Offset the arc so the central projectile(s) point towards the target
						targetVector.Y -= Math.Abs(targetVector.X) * radialOffset;

						// Velocity of the arc moving towards the target
						targetVector = Vector2.Normalize(targetVector) * projectileSpeed;

						// Offset the arc
						Vector2 velocity = targetVector;
						velocity.Normalize();
						velocity *= diameter;

						totalProjectiles = 16;
						int half = totalProjectiles / 2;
						float scalar = 0.2f;
						float velocityMult = (totalProjectiles - 1f) / 2f;
						offsetAngle = (float)Math.PI * radialOffset;

						for (int j = 0; j < totalProjectiles; j++)
						{
							float radians = j - velocityMult;
							Vector2 offset = velocity.RotatedBy(offsetAngle * radians, default);
							int proj = Projectile.NewProjectile(vectorCenter + offset, targetVector * (1f - (Math.Abs(radians) / half * scalar)), ModContent.ProjectileType<DarkOrb>(), damage, 0f, Main.myPlayer, -1f, 0f);
						}
					}

					// Arcs
					/*if (npc.localAI[0] % 60f == 0f)
					{
						radialOffset = 0.1f;
						float diameter = 120f;

						targetVector.Y -= Math.Abs(targetVector.X) * radialOffset;
						targetVector = Vector2.Normalize(targetVector) * projectileSpeed;

						Vector2 velocity = targetVector;
						velocity.Normalize();
						velocity *= diameter;

						totalProjectiles = 8;
						offsetAngle = (float)Math.PI * radialOffset;

						for (int j = 0; j < totalProjectiles; j++)
						{
							float radians = j - (totalProjectiles - 1f) / 2f;
							Vector2 offset = velocity.RotatedBy(offsetAngle * radians, default);
							int proj = Projectile.NewProjectile(vectorCenter + offset, targetVector, ModContent.ProjectileType<DarkOrb>(), damage, 0f, Main.myPlayer, -1f, 0f);
						}
					}

					if (npc.localAI[0] % 120f == 0f)
					{
						totalProjectiles = 24;
						offsetAngle = 360 / totalProjectiles;
						for (int i = 0; i < totalProjectiles; i++)
						{
							Projectile.NewProjectile(vectorCenter, Vector2.Zero, ModContent.ProjectileType<DarkOrb>(), damage, 0f, Main.myPlayer, 15f, i * offsetAngle);
						}
						
						totalProjectiles = 16;
						chains = 4;
						float velocityTimer = 90f / (totalProjectiles / chains);
						double radians = MathHelper.TwoPi / totalProjectiles;
						int j = 0;

						for (int i = 0; i < totalProjectiles; i++)
						{
							Vector2 vector = new Vector2(0f, -3f).RotatedBy(radians * i, default);
							Projectile.NewProjectile(vectorCenter, vector, ModContent.ProjectileType<DarkOrb>(), damage, 0f, Main.myPlayer, 16f, j * velocityTimer);

							j++;
							if (j % 4 == 0)
								j = 0;
						}
					}

					totalProjectiles = 36;
					float divisor = 6f;
					chains = 3;
					float interval = totalProjectiles / chains * divisor;
					double patternInterval = Math.Floor(npc.localAI[0] / interval);

					if (patternInterval % 2 == 0)
					{
						if (npc.localAI[0] % divisor == 0f)
						{
							double radians = (patternInterval % 4 == 0 ? MathHelper.TwoPi + MathHelper.Pi / 3 : MathHelper.TwoPi) / chains;
							radialOffset = MathHelper.ToRadians(npc.ai[2]);

							for (int i = 0; i < chains; i++)
							{
								Vector2 vector = new Vector2(0f, -3f).RotatedBy(radians * i + radialOffset, default);
								Projectile.NewProjectile(vectorCenter, vector, ModContent.ProjectileType<DarkOrb>(), damage, 0f, Main.myPlayer, 3f, 0f);
							}

							// Radial offset
							npc.ai[2] += 5f;
						}
					}
					else
						npc.ai[2] = 0f;*/

					/*if (npc.localAI[0] % 180f == 0f)
					{
						totalProjectiles = 12;
						offsetAngle = 360 / totalProjectiles;

						for (int i = 0; i < totalProjectiles; i++)
						{
							Projectile.NewProjectile(vectorCenter, Vector2.Zero, ModContent.ProjectileType<DarkOrb>(), damage, 0f, Main.myPlayer, 0f, i * offsetAngle);
							Projectile.NewProjectile(vectorCenter, Vector2.Zero, ModContent.ProjectileType<DarkOrb>(), damage, 0f, Main.myPlayer, 12f, i * offsetAngle);
						}
					}*/
				}

				if (npc.velocity.Length() > 0f)
					npc.velocity = Vector2.Zero;

				npc.rotation = npc.velocity.X * 0.04f;

				float playerLocation = vectorCenter.X - player.Center.X;
				npc.direction = playerLocation < 0f ? 1 : -1;
				npc.spriteDirection = npc.direction;

				npc.ai[1] += 1f;
				if (npc.ai[1] >= 600f)
				{
					npc.TargetClosest(true);
					npc.ai[0] = 0f;
					npc.ai[1] = 0f;
					npc.ai[2] = 0f;
					npc.ai[3] = 0f;
					npc.localAI[0] = 0f;
					npc.netUpdate = true;
				}
			}

			// Laser beam attack
			else if (npc.ai[0] == 2f)
			{
				npc.defense = npc.defDefense * 3;

				Vector2 source = new Vector2(vectorCenter.X + (npc.spriteDirection > 0 ? 34f : -34f), vectorCenter.Y - 74f);
				Vector2 aimAt = player.Center + player.velocity * 20f;
				float aimResponsiveness = 0.4f;

				switch ((int)npc.ai[2])
				{
					case 0:
						break;
					case 1:
						aimResponsiveness = 0.25f;
						break;
					case 2:
						aimResponsiveness = 0.1f;
						break;
				}

				Vector2 aimVector = Vector2.Normalize(aimAt - source);
				if (aimVector.HasNaNs())
					aimVector = -Vector2.UnitY;
				aimVector = Vector2.Normalize(Vector2.Lerp(aimVector, Vector2.Normalize(npc.velocity), aimResponsiveness));
				aimVector *= 6f;

				Vector2 laserVelocity = Vector2.Normalize(aimVector);
				if (laserVelocity.HasNaNs())
					laserVelocity = -Vector2.UnitY;

				calamityGlobalNPC.newAI[1] = laserVelocity.X;
				calamityGlobalNPC.newAI[2] = laserVelocity.Y;

				npc.ai[1] += 1f;
				if (npc.ai[1] >= 300f)
				{
					npc.TargetClosest(true);
					npc.ai[2] += 1f;
					npc.localAI[0] = 0f;
					npc.localAI[1] = 0f;
					if (npc.ai[2] >= 3f)
					{
						npc.ai[0] = 0f;
						npc.ai[1] = 0f;
						npc.ai[2] = 0f;
						calamityGlobalNPC.newAI[0] = 0f;
					}
					else
					{
						npc.ai[1] = 0f;
						calamityGlobalNPC.newAI[0] = 0f;
					}
				}
				else if (npc.ai[1] >= 180f)
				{
					npc.velocity *= 0.95f;
					if (Main.netMode != NetmodeID.MultiplayerClient)
					{
						if (npc.ai[1] == 180f)
						{
							Main.PlaySound(29, (int)npc.position.X, (int)npc.position.Y, 104);
							Vector2 laserVelocity2 = new Vector2(npc.localAI[0], npc.localAI[1]);
							laserVelocity2.Normalize();

							int totalProjectiles = 4;
							float spread = MathHelper.ToRadians(90); // 30 degrees in radians = 0.523599
							double startAngle = Math.Atan2(laserVelocity2.X, laserVelocity2.Y) - spread / 2; // Where the projectiles start spawning at, don't change this
							double deltaAngle = spread / totalProjectiles; // Angle between each projectile, 0.04363325
							double offsetAngle;

							int i;
							for (i = 0; i < 2; i++)
							{
								offsetAngle = startAngle + deltaAngle * (i + i * i) / 2f + radialOffset * i;
								Projectile.NewProjectile(source.X, source.Y, (float)Math.Sin(offsetAngle), (float)Math.Cos(offsetAngle), ModContent.ProjectileType<BrimstoneRay>(), 40, 0f, Main.myPlayer, 1f, npc.whoAmI);
								Projectile.NewProjectile(source.X, source.Y, (float)-Math.Sin(offsetAngle), (float)-Math.Cos(offsetAngle), ModContent.ProjectileType<BrimstoneRay>(), 40, 0f, Main.myPlayer, 1f, npc.whoAmI);
							}
						}
					}
				}
				else
				{
					float playSoundTimer = 20f;
					if (npc.ai[1] < 150f)
					{
						switch ((int)npc.ai[2])
						{
							case 0:
								break;
							case 1:
								npc.ai[1] += 0.5f;
								playSoundTimer = 30f;
								break;
							case 2:
								npc.ai[1] += 1f;
								playSoundTimer = 40f;
								break;
						}
					}

					if (npc.ai[1] % playSoundTimer == 0f)
						Main.PlaySound(2, (int)npc.position.X, (int)npc.position.Y, 20);

					if (Main.netMode != NetmodeID.MultiplayerClient)
					{
						if (npc.ai[1] < 150f && calamityGlobalNPC.newAI[0] == 0f)
						{
							Projectile.NewProjectile(source, laserVelocity, ModContent.ProjectileType<BrimstoneTargetRay>(), 0, 0f, Main.myPlayer, 0f, npc.whoAmI);
							calamityGlobalNPC.newAI[0] = 1f;
						}
						else
						{
							if (npc.ai[1] == 150f)
							{
								npc.localAI[0] = laserVelocity.X;
								npc.localAI[1] = laserVelocity.Y;

								Vector2 laserVelocity2 = new Vector2(npc.localAI[0], npc.localAI[1]);
								laserVelocity2.Normalize();

								int totalProjectiles = 4;
								float spread = MathHelper.ToRadians(90); // 30 degrees in radians = 0.523599
								double startAngle = Math.Atan2(laserVelocity2.X, laserVelocity2.Y) - spread / 2; // Where the projectiles start spawning at, don't change this
								double deltaAngle = spread / totalProjectiles; // Angle between each projectile, 0.04363325
								double offsetAngle;

								int i;
								for (i = 0; i < 2; i++)
								{
									offsetAngle = startAngle + deltaAngle * (i + i * i) / 2f + radialOffset * i;
									Projectile.NewProjectile(source.X, source.Y, (float)Math.Sin(offsetAngle), (float)Math.Cos(offsetAngle), ModContent.ProjectileType<BrimstoneTargetRay>(), 0, 0f, Main.myPlayer, 1f, npc.whoAmI);
									Projectile.NewProjectile(source.X, source.Y, (float)-Math.Sin(offsetAngle), (float)-Math.Cos(offsetAngle), ModContent.ProjectileType<BrimstoneTargetRay>(), 0, 0f, Main.myPlayer, 1f, npc.whoAmI);
								}
							}
						}
					}
				}
			}
		}
		#endregion

		#region Gem Crawler AI
		public static void GemCrawlerAI(NPC npc, Mod mod, float speedDetect, float speedAdditive)
        {
            int num19 = 30;
            int num20 = 10;
            bool flag19 = false;
            bool flag20 = false;
            bool flag30 = false;
            if (npc.velocity.Y == 0f && ((npc.velocity.X > 0f && npc.direction > 0) || (npc.velocity.X < 0f && npc.direction < 0)))
            {
                flag20 = true;
                npc.ai[3] += 1f;
            }
            if ((npc.position.X == npc.oldPosition.X || npc.ai[3] >= (float)num19) | flag20)
            {
                npc.ai[3] += 1f;
                flag30 = true;
            }
            else if (npc.ai[3] > 0f)
            {
                npc.ai[3] -= 1f;
            }
            if (npc.ai[3] > (float)(num19 * num20))
            {
                npc.ai[3] = 0f;
            }
            if (npc.justHit)
            {
                npc.ai[3] = 0f;
            }
            if (npc.ai[3] == (float)num19)
            {
                npc.netUpdate = true;
            }
            Vector2 npcPos = new Vector2(npc.Center.X, npc.Center.Y);
            float xDist = Main.player[npc.target].Center.X - npcPos.X;
            float yDist = Main.player[npc.target].Center.Y - npcPos.Y;
            float targetDist = (float)Math.Sqrt((double)(xDist * xDist + yDist * yDist));
            if (targetDist < 200f && !flag30)
            {
                npc.ai[3] = 0f;
            }
            if (npc.ai[3] < (float)num19)
            {
                npc.TargetClosest(true);
            }
            else
            {
                if (npc.velocity.X == 0f)
                {
                    if (npc.velocity.Y == 0f)
                    {
                        npc.ai[0] += 1f;
                        if (npc.ai[0] >= 2f)
                        {
                            npc.direction *= -1;
                            npc.spriteDirection = -npc.direction;
                            npc.ai[0] = 0f;
                        }
                    }
                }
                else
                {
                    npc.ai[0] = 0f;
                }
                npc.directionY = -1;
                if (npc.direction == 0)
                {
                    npc.direction = 1;
                }
            }
            float num6 = speedDetect; //5
            float num70 = speedAdditive; //0.05
            if (!flag19 && (npc.velocity.Y == 0f || npc.wet || (npc.velocity.X <= 0f && npc.direction > 0) || (npc.velocity.X >= 0f && npc.direction < 0)))
            {
                if (npc.velocity.X < -num6 || npc.velocity.X > num6)
                {
                    if (npc.velocity.Y == 0f)
                    {
                        npc.velocity *= 0.8f;
                    }
                }
                else if (npc.velocity.X < num6 && npc.direction == -1)
                {
                    npc.velocity.X = npc.velocity.X + num70;
                    if (npc.velocity.X > num6)
                    {
                        npc.velocity.X = num6;
                    }
                }
                else if (npc.velocity.X > -num6 && npc.direction == 1)
                {
                    npc.velocity.X = npc.velocity.X - num70;
                    if (npc.velocity.X < -num6)
                    {
                        npc.velocity.X = -num6;
                    }
                }
            }
            if (npc.velocity.Y >= 0f)
            {
                int num9 = 0;
                if (npc.velocity.X < 0f)
                {
                    num9 = -1;
                }
                if (npc.velocity.X > 0f)
                {
                    num9 = 1;
                }
                Vector2 position = npc.position;
                position.X += npc.velocity.X;
                int x = (int)((position.X + (float)(npc.width / 2) + (float)((npc.width / 2 + 1) * num9)) / 16f);
                int y = (int)((position.Y + (float)npc.height - 1f) / 16f);
                if (Main.tile[x, y] == null)
                {
                    Main.tile[x, y] = new Tile();
                }
                if (Main.tile[x, y - 1] == null)
                {
                    Main.tile[x, y - 1] = new Tile();
                }
                if (Main.tile[x, y - 2] == null)
                {
                    Main.tile[x, y - 2] = new Tile();
                }
                if (Main.tile[x, y - 3] == null)
                {
                    Main.tile[x, y - 3] = new Tile();
                }
                if (Main.tile[x, y + 1] == null)
                {
                    Main.tile[x, y + 1] = new Tile();
                }
                if ((float)(x * 16) < position.X + (float)npc.width && (float)(x * 16 + 16) > position.X && ((Main.tile[x, y].nactive() && !Main.tile[x, y].topSlope() && !Main.tile[x, y - 1].topSlope() && Main.tileSolid[(int)Main.tile[x, y].type] && !Main.tileSolidTop[(int)Main.tile[x, y].type]) || (Main.tile[x, y - 1].halfBrick() && Main.tile[x, y - 1].nactive())) && (!Main.tile[x, y - 1].nactive() || !Main.tileSolid[(int)Main.tile[x, y - 1].type] || Main.tileSolidTop[(int)Main.tile[x, y - 1].type] || (Main.tile[x, y - 1].halfBrick() && (!Main.tile[x, y - 4].nactive() || !Main.tileSolid[(int)Main.tile[x, y - 4].type] || Main.tileSolidTop[(int)Main.tile[x, y - 4].type]))) && (!Main.tile[x, y - 2].nactive() || !Main.tileSolid[(int)Main.tile[x, y - 2].type] || Main.tileSolidTop[(int)Main.tile[x, y - 2].type]) && (!Main.tile[x, y - 3].nactive() || !Main.tileSolid[(int)Main.tile[x, y - 3].type] || Main.tileSolidTop[(int)Main.tile[x, y - 3].type]) && (!Main.tile[x - num9, y - 3].nactive() || !Main.tileSolid[(int)Main.tile[x - num9, y - 3].type]))
                {
                    float npcBottom = (float)(y * 16);
                    if (Main.tile[x, y].halfBrick())
                    {
                        npcBottom += 8f;
                    }
                    if (Main.tile[x, y - 1].halfBrick())
                    {
                        npcBottom -= 8f;
                    }
                    if (npcBottom < position.Y + (float)npc.height)
                    {
                        float num13 = position.Y + (float)npc.height - npcBottom;
                        if (num13 <= 16.1f)
                        {
                            npc.gfxOffY += npc.position.Y + (float)npc.height - npcBottom;
                            npc.position.Y = npcBottom - (float)npc.height;
                            if (num13 < 9f)
                            {
                                npc.stepSpeed = 1f;
                            }
                            else
                            {
                                npc.stepSpeed = 2f;
                            }
                        }
                    }
                }
            }
		}
		#endregion

		#region Dungeon Spirit AI
		public static void DungeonSpiritAI(NPC npc, Mod mod, float speed, float rotation, bool lantern = false)
        {
            npc.TargetClosest(true);
            Vector2 npcPos = new Vector2(npc.Center.X, npc.Center.Y);
            float xDist = Main.player[npc.target].Center.X - npcPos.X;
            float yDist = Main.player[npc.target].Center.Y - npcPos.Y;
            float targetDist = (float)Math.Sqrt((double)(xDist * xDist + yDist * yDist));
            float homingSpeed = speed;

			if (lantern)
			{
				if (npc.localAI[0] < 85f)
				{
					homingSpeed = 0.1f;
					targetDist = homingSpeed / targetDist;
					xDist *= targetDist;
					yDist *= targetDist;
					npc.velocity = (npc.velocity * 100f + new Vector2(xDist, yDist)) / 101f;
					npc.localAI[0] += 1f;
					return;
				}

				npc.dontTakeDamage = false;
				npc.chaseable = true;
			}

            targetDist = homingSpeed / targetDist;
            xDist *= targetDist;
            yDist *= targetDist;
            npc.velocity.X = (npc.velocity.X * 100f + xDist) / 101f;
            npc.velocity.Y = (npc.velocity.Y * 100f + yDist) / 101f;

			if (lantern)
			{
				npc.rotation = npc.velocity.X * 0.08f;
				npc.spriteDirection = (npc.direction > 0) ? 1 : -1;
			}
			else
				npc.rotation = (float)Math.Atan2((double)yDist, (double)xDist) + rotation;
        }
		#endregion

		#region Unicorn AI
		public static void UnicornAI(NPC npc, Mod mod, bool spin, float bounciness, float speedDetect, float speedAdditive, float bouncy1 = -8.5f, float bouncy2 = -7.5f, float bouncy3 = -7f, float bouncy4 = -6f, float bouncy5 = -8f)
        {
            bool DogPhase1 = npc.type == ModContent.NPCType<AngryDog>() && (double)npc.life > (double)npc.lifeMax * (CalamityWorld.death ? 0.9 : 0.5);
            bool DogPhase2 = npc.type == ModContent.NPCType<AngryDog>() && (double)npc.life <= (double)npc.lifeMax * (CalamityWorld.death ? 0.9 : 0.5);
            int num = 30;
            bool flag2 = false;
            bool flag3 = false;
            if (npc.velocity.Y == 0f && ((npc.velocity.X > 0f && npc.direction < 0) || (npc.velocity.X < 0f && npc.direction > 0)))
            {
                flag2 = true;
                npc.ai[3] += 1f;
            }
            int num2 = DogPhase1 ? 10 : 4;
			if (!DogPhase1)
			{
				bool flag4 = npc.velocity.Y == 0f;
				for (int i = 0; i < Main.maxNPCs; i++)
				{
					if (i != npc.whoAmI && Main.npc[i].active && Main.npc[i].type == npc.type && Math.Abs(npc.position.X - Main.npc[i].position.X) + Math.Abs(npc.position.Y - Main.npc[i].position.Y) < (float)npc.width)
					{
						if (npc.position.X < Main.npc[i].position.X)
						{
							npc.velocity.X = npc.velocity.X - 0.05f;
						}
						else
						{
							npc.velocity.X = npc.velocity.X + 0.05f;
						}
						if (npc.position.Y < Main.npc[i].position.Y)
						{
							npc.velocity.Y = npc.velocity.Y - 0.05f;
						}
						else
						{
							npc.velocity.Y = npc.velocity.Y + 0.05f;
						}
					}
				}
				if (flag4)
				{
					npc.velocity.Y = 0f;
				}
			}
            if (npc.position.X == npc.oldPosition.X || npc.ai[3] >= (float)num || flag2)
            {
                npc.ai[3] += 1f;
                flag3 = true;
            }
            else if (npc.ai[3] > 0f)
            {
                npc.ai[3] -= 1f;
            }
            if (npc.ai[3] > (float)(num * num2))
            {
                npc.ai[3] = 0f;
            }
            if (npc.justHit)
            {
                npc.ai[3] = 0f;
            }
            if (npc.ai[3] == (float)num)
            {
                npc.netUpdate = true;
            }
            Vector2 npcPos = new Vector2(npc.Center.X, npc.Center.Y);
            float xDist = Main.player[npc.target].Center.X - npcPos.X;
            float yDist = Main.player[npc.target].Center.Y - npcPos.Y;
            float targetDist = (float)Math.Sqrt((double)(xDist * xDist + yDist * yDist));
            if (targetDist < 200f && !flag3)
            {
                npc.ai[3] = 0f;
            }
			if (!DogPhase1)
			{
				if (npc.velocity.Y == 0f && Math.Abs(npc.velocity.X) > 3f && ((npc.Center.X < Main.player[npc.target].Center.X && npc.velocity.X > 0f) || (npc.Center.X > Main.player[npc.target].Center.X && npc.velocity.X < 0f)))
				{
					npc.velocity.Y -= bounciness;
					if (npc.type == ModContent.NPCType<DespairStone>())
					{
						Main.PlaySound(2, npc.Center, 14);
						for (int k = 0; k < 10; k++)
						{
							Dust.NewDust(npc.position, npc.width, npc.height, (int)CalamityDusts.Brimstone, 0f, -1f, 0, default, 1f);
						}
					}
					if (npc.type == ModContent.NPCType<Bohldohr>())
					{
						Main.PlaySound(3, npc.Center, 7);
					}
					if (DogPhase2)
					{
						for (int k = 0; k < 5; k++)
						{
							Dust.NewDust(npc.position, npc.width, npc.height, 33, 0f, -1f, 0, default, 1f);
						}
					}
					if (npc.type == ModContent.NPCType<AquaticUrchin>() || npc.type == ModContent.NPCType<SeaUrchin>())
					{
						for (int k = 0; k < 5; k++)
						{
							Dust.NewDust(npc.position, npc.width, npc.height, 33, 0f, -1f, 0, default, 1f);
						}
					}
				}
			}
            if (npc.ai[3] < (float)num)
            {
                npc.TargetClosest(true);
            }
            else
            {
                if (npc.velocity.X == 0f)
                {
                    if (npc.velocity.Y == 0f)
                    {
                        npc.ai[0] += 1f;
                        if (npc.ai[0] >= 2f)
                        {
                            npc.direction *= -1;
                            npc.spriteDirection = npc.direction;
                            npc.ai[0] = 0f;
                        }
                    }
                }
                else
                {
                    npc.ai[0] = 0f;
                }
                npc.directionY = -1;
                if (npc.direction == 0)
                {
                    npc.direction = 1;
                }
            }

            if (npc.velocity.Y == 0f || npc.wet || (npc.velocity.X <= 0f && npc.direction < 0) || (npc.velocity.X >= 0f && npc.direction > 0))
            {
                if (Math.Sign(npc.velocity.X) != npc.direction && !DogPhase1)
                {
                    npc.velocity.X = npc.velocity.X * 0.92f;
                }
                float num9 = MathHelper.Lerp(0.6f, 1f, Math.Abs(Main.windSpeedSet)) * (float)Math.Sign(Main.windSpeedSet);
                if (!Main.player[npc.target].ZoneSandstorm)
                {
                    num9 = 0f;
                }
                float num7 = speedDetect;
                float num8 = speedAdditive;
                if (npc.velocity.X < -num7 || npc.velocity.X > num7)
                {
                    if (npc.velocity.Y == 0f)
                    {
                        npc.velocity *= 0.8f;
                    }
                }
                else if (npc.velocity.X < num7 && npc.direction == 1)
                {
                    npc.velocity.X += num8;
                    if (npc.velocity.X > num7)
                    {
                        npc.velocity.X = num7;
                    }
                }
                else if (npc.velocity.X > -num7 && npc.direction == -1)
                {
                    npc.velocity.X -= num8;
                    if (npc.velocity.X < -num7)
                    {
                        npc.velocity.X = -num7;
                    }
                }
            }
            if (npc.velocity.Y >= 0f)
            {
                int num10 = 0;
                if (npc.velocity.X < 0f)
                {
                    num10 = -1;
                }
                if (npc.velocity.X > 0f)
                {
                    num10 = 1;
                }
                Vector2 position = npc.position;
                position.X += npc.velocity.X;
                int x = (int)((position.X + (float)(npc.width / 2) + (float)((npc.width / 2 + 1) * num10)) / 16f);
                int y = (int)((position.Y + (float)npc.height - 1f) / 16f);
                if (Main.tile[x, y] == null)
                {
                    Main.tile[x, y] = new Tile();
                }
                if (Main.tile[x, y - 1] == null)
                {
                    Main.tile[x, y - 1] = new Tile();
                }
                if (Main.tile[x, y - 2] == null)
                {
                    Main.tile[x, y - 2] = new Tile();
                }
                if (Main.tile[x, y - 3] == null)
                {
                    Main.tile[x, y - 3] = new Tile();
                }
                if (Main.tile[x, y + 1] == null)
                {
                    Main.tile[x, y + 1] = new Tile();
                }
                if ((float)(x * 16) < position.X + (float)npc.width && (float)(x * 16 + 16) > position.X && ((Main.tile[x, y].nactive() && !Main.tile[x, y].topSlope() && !Main.tile[x, y - 1].topSlope() && Main.tileSolid[(int)Main.tile[x, y].type] && !Main.tileSolidTop[(int)Main.tile[x, y].type]) || (Main.tile[x, y - 1].halfBrick() && Main.tile[x, y - 1].nactive())) && (!Main.tile[x, y - 1].nactive() || !Main.tileSolid[(int)Main.tile[x, y - 1].type] || Main.tileSolidTop[(int)Main.tile[x, y - 1].type] || (Main.tile[x, y - 1].halfBrick() && (!Main.tile[x, y - 4].nactive() || !Main.tileSolid[(int)Main.tile[x, y - 4].type] || Main.tileSolidTop[(int)Main.tile[x, y - 4].type]))) && (!Main.tile[x, y - 2].nactive() || !Main.tileSolid[(int)Main.tile[x, y - 2].type] || Main.tileSolidTop[(int)Main.tile[x, y - 2].type]) && (!Main.tile[x, y - 3].nactive() || !Main.tileSolid[(int)Main.tile[x, y - 3].type] || Main.tileSolidTop[(int)Main.tile[x, y - 3].type]) && (!Main.tile[x - num10, y - 3].nactive() || !Main.tileSolid[(int)Main.tile[x - num10, y - 3].type]))
                {
                    float num13 = (float)(y * 16);
                    if (Main.tile[x, y].halfBrick())
                    {
                        num13 += 8f;
                    }
                    if (Main.tile[x, y - 1].halfBrick())
                    {
                        num13 -= 8f;
                    }
                    if (num13 < position.Y + (float)npc.height)
                    {
                        float num14 = position.Y + (float)npc.height - num13;
                        if ((double)num14 <= 16.1)
                        {
                            npc.gfxOffY += npc.position.Y + (float)npc.height - num13;
                            npc.position.Y = num13 - (float)npc.height;
                            if (num14 < 9f)
                            {
                                npc.stepSpeed = 1f;
                            }
                            else
                            {
                                npc.stepSpeed = 2f;
                            }
                        }
                    }
                }
            }
            if (npc.velocity.Y == 0f)
            {
                int num15 = (int)((npc.position.X + (float)(npc.width / 2) + (float)((npc.width / 2 + 2) * npc.direction) + npc.velocity.X * 5f) / 16f);
                int num16 = (int)((npc.position.Y + (float)npc.height - 15f) / 16f);
                if (Main.tile[num15, num16] == null)
                {
                    Main.tile[num15, num16] = new Tile();
                }
                if (Main.tile[num15, num16 - 1] == null)
                {
                    Main.tile[num15, num16 - 1] = new Tile();
                }
                if (Main.tile[num15, num16 - 2] == null)
                {
                    Main.tile[num15, num16 - 2] = new Tile();
                }
                if (Main.tile[num15, num16 - 3] == null)
                {
                    Main.tile[num15, num16 - 3] = new Tile();
                }
                if (Main.tile[num15, num16 + 1] == null)
                {
                    Main.tile[num15, num16 + 1] = new Tile();
                }
                if (Main.tile[num15 + npc.direction, num16 - 1] == null)
                {
                    Main.tile[num15 + npc.direction, num16 - 1] = new Tile();
                }
                if (Main.tile[num15 + npc.direction, num16 + 1] == null)
                {
                    Main.tile[num15 + npc.direction, num16 + 1] = new Tile();
                }
                if (Main.tile[num15 - npc.direction, num16 + 1] == null)
                {
                    Main.tile[num15 - npc.direction, num16 + 1] = new Tile();
                }
                int num17 = npc.spriteDirection;
                num17 *= -1;
                if ((npc.velocity.X < 0f && num17 == -1) || (npc.velocity.X > 0f && num17 == 1))
                {
                    float num18 = 3f;
                    if (Main.tile[num15, num16 - 2].nactive() && Main.tileSolid[(int)Main.tile[num15, num16 - 2].type])
                    {
                        if (Main.tile[num15, num16 - 3].nactive() && Main.tileSolid[(int)Main.tile[num15, num16 - 3].type])
                        {
                            npc.velocity.Y = bouncy1;
                            npc.netUpdate = true;
                        }
                        else
                        {
                            npc.velocity.Y = bouncy2;
                            npc.netUpdate = true;
                        }
                    }
                    else if (Main.tile[num15, num16 - 1].nactive() && !Main.tile[num15, num16 - 1].topSlope() && Main.tileSolid[(int)Main.tile[num15, num16 - 1].type])
                    {
                        npc.velocity.Y = bouncy3;
                        npc.netUpdate = true;
                    }
                    else if (npc.position.Y + (float)npc.height - (float)(num16 * 16) > 20f && Main.tile[num15, num16].nactive() && !Main.tile[num15, num16].topSlope() && Main.tileSolid[(int)Main.tile[num15, num16].type])
                    {
                        npc.velocity.Y = bouncy4;
                        npc.netUpdate = true;
                    }
                    else if ((npc.directionY < 0 || Math.Abs(npc.velocity.X) > num18) && (!Main.tile[num15, num16 + 1].nactive() || !Main.tileSolid[(int)Main.tile[num15, num16 + 1].type]) && (!Main.tile[num15, num16 + 2].nactive() || !Main.tileSolid[(int)Main.tile[num15, num16 + 2].type]) && (!Main.tile[num15 + npc.direction, num16 + 3].nactive() || !Main.tileSolid[(int)Main.tile[num15 + npc.direction, num16 + 3].type]))
                    {
                        npc.velocity.Y = bouncy5;
                        npc.netUpdate = true;
                    }
                }
            }
			if (spin)
			{
				npc.rotation += npc.velocity.X * 0.05f;
				npc.spriteDirection = -npc.direction;
			}
		}
		#endregion

		#region Swimming AI
		public static void PassiveSwimmingAI(NPC npc, Mod mod, int passiveness, float detectRange, float xSpeed, float ySpeed, float speedLimitX, float speedLimitY, float rotation, bool spriteFacesLeft = true)
        {
			if (spriteFacesLeft)
				npc.spriteDirection = (npc.direction > 0) ? 1 : -1;
			else
				npc.spriteDirection = (npc.direction > 0) ? -1 : 1;

            npc.noGravity = true;
            if (npc.direction == 0)
            {
                npc.TargetClosest(true);
            }
            if (npc.justHit && passiveness != 3)
            {
                npc.chaseable = true;
            }
            if (npc.wet)
            {
                bool flag14 = npc.chaseable;
                npc.TargetClosest(false);
				if (passiveness != 2)
				{
					if (npc.type == ModContent.NPCType<Frogfish>())
					{
						if (Main.player[npc.target].wet && !Main.player[npc.target].dead)
						{
							flag14 = true;
							npc.chaseable = true; //once the enemy has detected the player, let minions fuck it up
						}
					}
					if (npc.type == ModContent.NPCType<Flounder>())
					{
						if (!Main.player[npc.target].dead)
						{
							flag14 = true;
							npc.chaseable = true; //once the enemy has detected the player, let minions fuck it up
						}
					}
					else if (Main.player[npc.target].wet && !Main.player[npc.target].dead &&
						(Main.player[npc.target].Center - npc.Center).Length() < detectRange &&
						Collision.CanHit(npc.position, npc.width, npc.height, Main.player[npc.target].position, Main.player[npc.target].width, Main.player[npc.target].height))
					{
						flag14 = true;
						npc.chaseable = true; //once the enemy has detected the player, let minions fuck it up
					}
					else
					{
						if (passiveness == 1)
						{
							flag14 = false;
						}
					}
				}
                if (Main.player[npc.target].dead && flag14)
                {
                    flag14 = false;
                }
                if (!flag14 || passiveness == 0)
                {
					if (passiveness == 0)
						npc.TargetClosest(true);
                    if (npc.collideX)
                    {
                        npc.velocity.X = npc.velocity.X * -1f;
                        npc.direction *= -1;
                        npc.netUpdate = true;
                    }
                    if (npc.collideY)
                    {
                        npc.netUpdate = true;
                        if (npc.velocity.Y > 0f)
                        {
                            npc.velocity.Y = Math.Abs(npc.velocity.Y) * -1f;
                            npc.directionY = -1;
                            npc.ai[0] = -1f;
                        }
                        else if (npc.velocity.Y < 0f)
                        {
                            npc.velocity.Y = Math.Abs(npc.velocity.Y);
                            npc.directionY = 1;
                            npc.ai[0] = 1f;
                        }
                    }
                }
                if (flag14 && passiveness != 2)
                {
                    npc.TargetClosest(true);
					if (passiveness == 3)
					{
						npc.velocity.X = npc.velocity.X - (float)npc.direction * xSpeed;
						npc.velocity.Y = npc.velocity.Y - (float)npc.directionY * ySpeed;
					}
					else
					{
						npc.velocity.X = npc.velocity.X + (float)npc.direction * (CalamityWorld.death ? 2f * xSpeed : xSpeed);
						npc.velocity.Y = npc.velocity.Y + (float)npc.directionY * (CalamityWorld.death ? 2f * ySpeed : ySpeed);
					}
					float velocityCapX = CalamityWorld.death && passiveness != 3 ? 2f * speedLimitX : speedLimitX;
					float velocityCapY = CalamityWorld.death && passiveness != 3 ? 2f * speedLimitY : speedLimitY;
                    npc.velocity.X = MathHelper.Clamp(npc.velocity.X, -velocityCapX, velocityCapX);
                    npc.velocity.Y = MathHelper.Clamp(npc.velocity.Y, -velocityCapY, velocityCapY);
					if (npc.type == ModContent.NPCType<Laserfish>())
					{
						npc.localAI[0] += (CalamityWorld.death ? 2f : 1f);
						if (Main.netMode != NetmodeID.MultiplayerClient && npc.localAI[0] >= 120f)
						{
							npc.localAI[0] = 0f;
							if (Collision.CanHit(npc.position, npc.width, npc.height, Main.player[npc.target].position, Main.player[npc.target].width, Main.player[npc.target].height))
							{
								float speed = 5f;
								Vector2 vector = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)(npc.height / 2));
								float num6 = Main.player[npc.target].position.X + (float)Main.player[npc.target].width * 0.5f - vector.X + (float)Main.rand.Next(-20, 21);
								float num7 = Main.player[npc.target].position.Y + (float)Main.player[npc.target].height * 0.5f - vector.Y + (float)Main.rand.Next(-20, 21);
								float num8 = (float)Math.Sqrt((double)(num6 * num6 + num7 * num7));
								num8 = speed / num8;
								num6 *= num8;
								num7 *= num8;
								int damage = 40;
								if (Main.expertMode)
								{
									damage = 30;
								}
								int beam = Projectile.NewProjectile(npc.Center.X + (npc.spriteDirection == 1 ? 25f : -25f), npc.Center.Y + (Main.player[npc.target].position.Y > npc.Center.Y ? 5f : -5f), num6, num7, ProjectileID.EyeBeam, damage, 0f, Main.myPlayer, 0f, 0f);
								Main.projectile[beam].tileCollide = true;
							}
						}
					}
					if (npc.type == ModContent.NPCType<Flounder>())
					{
                        if ((Main.player[npc.target].Center - npc.Center).Length() < 350f)
                        {
                            npc.localAI[0] += (CalamityWorld.death ? 3f : 1f);
                            if (Main.netMode != NetmodeID.MultiplayerClient && npc.localAI[0] >= 180f)
                            {
                                npc.localAI[0] = 0f;
                                if (Collision.CanHit(npc.position, npc.width, npc.height, Main.player[npc.target].position, Main.player[npc.target].width, Main.player[npc.target].height))
                                {
                                    float speed = 4f;
                                    Vector2 vector = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)(npc.height / 2));
                                    float num6 = Main.player[npc.target].position.X + (float)Main.player[npc.target].width * 0.5f - vector.X + (float)Main.rand.Next(-20, 21);
                                    float num7 = Main.player[npc.target].position.Y + (float)Main.player[npc.target].height * 0.5f - vector.Y + (float)Main.rand.Next(-20, 21);
                                    float num8 = (float)Math.Sqrt((double)(num6 * num6 + num7 * num7));
                                    num8 = speed / num8;
                                    num6 *= num8;
                                    num7 *= num8;
                                    int damage = 25;
                                    if (Main.expertMode)
                                    {
                                        damage = 19;
                                    }
                                    int beam = Projectile.NewProjectile(npc.Center.X + (npc.spriteDirection == 1 ? 10f : -10f), npc.Center.Y, num6, num7, ModContent.ProjectileType<SulphuricAcidMist>(), damage, 0f, Main.myPlayer, 0f, 0f);
                                }
                            }
                        }
					}
					if (npc.type == ModContent.NPCType<SeaMinnow>())
					{
						npc.direction *= -1;
					}
                }
                else
                {
                    npc.velocity.X += (float)npc.direction * 0.1f;
                    if (npc.velocity.X < -2.5f || npc.velocity.X > 2.5f)
                    {
                        npc.velocity.X *= 0.95f;
                    }
                    if (npc.ai[0] == -1f)
                    {
                        npc.velocity.Y -= 0.01f;
                        if (npc.velocity.Y < -0.3f)
                        {
                            npc.ai[0] = 1f;
                        }
                    }
                    else
                    {
                        npc.velocity.Y += 0.01f;
                        if (npc.velocity.Y > 0.3f)
                        {
                            npc.ai[0] = -1f;
                        }
                    }
                }
                int num258 = (int)(npc.position.X + (float)(npc.width / 2)) / 16;
                int num259 = (int)(npc.position.Y + (float)(npc.height / 2)) / 16;
                if (Main.tile[num258, num259 - 1] == null)
                {
                    Main.tile[num258, num259 - 1] = new Tile();
                }
                if (Main.tile[num258, num259 + 1] == null)
                {
                    Main.tile[num258, num259 + 1] = new Tile();
                }
                if (Main.tile[num258, num259 + 2] == null)
                {
                    Main.tile[num258, num259 + 2] = new Tile();
                }
                if (Main.tile[num258, num259 - 1].liquid > 128)
                {
                    if (Main.tile[num258, num259 + 1].active())
                    {
                        npc.ai[0] = -1f;
                    }
                    else if (Main.tile[num258, num259 + 2].active())
                    {
                        npc.ai[0] = -1f;
                    }
                }
                if (npc.velocity.Y > 0.4f || npc.velocity.Y < -0.4f)
                {
                    npc.velocity.Y = npc.velocity.Y * 0.95f;
                }
            }
            else
            {
                if (npc.velocity.Y == 0f)
                {
                    npc.velocity.X = npc.velocity.X * 0.94f;
                    if (npc.velocity.X > -0.2f && npc.velocity.X < 0.2f)
                    {
                        npc.velocity.X = 0f;
                    }
                }
                npc.velocity.Y = npc.velocity.Y + 0.4f;
                if (npc.velocity.Y > 12f)
                {
                    npc.velocity.Y = 12f;
                }
                npc.ai[0] = 1f;
            }
            npc.rotation = npc.velocity.Y * (float)npc.direction * rotation;
			float rotationLimit = 2f * rotation;
			npc.rotation = MathHelper.Clamp(npc.rotation, -rotationLimit, rotationLimit);
		}
		#endregion
	}
}
