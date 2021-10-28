﻿using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.CalPlayer;
using CalamityMod.Dusts;
using CalamityMod.Events;
using CalamityMod.Items.Armor.Vanity;
using CalamityMod.Items.LoreItems;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Mounts;
using CalamityMod.Items.Placeables.Furniture.Trophies;
using CalamityMod.Items.Placeables.FurnitureCosmilite;
using CalamityMod.Items.Potions;
using CalamityMod.Items.TreasureBags;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.NPCs.TownNPCs;
using CalamityMod.Projectiles.Boss;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.DevourerofGods
{
    public class DevourerofGodsHead : ModNPC
    {
		public static int phase1IconIndex;
		public static int phase2IconIndex;

		internal static void LoadHeadIcons()
		{
			string phase1IconPath = "CalamityMod/NPCs/DevourerofGods/DevourerofGodsHead_Head_Boss";
			string phase2IconPath = "CalamityMod/NPCs/DevourerofGods/DevourerofGodsHeadS_Head_Boss";

			CalamityMod.Instance.AddBossHeadTexture(phase1IconPath, -1);
			phase1IconIndex = ModContent.GetModBossHeadSlot(phase1IconPath);

			CalamityMod.Instance.AddBossHeadTexture(phase2IconPath, -1);
			phase2IconIndex = ModContent.GetModBossHeadSlot(phase2IconPath);
		}

		// Phase 1 variables

		// Enums
		private enum LaserWallType
		{
			DiagonalRight = 0,
			DiagonalLeft = 1,
			DiagonalHorizontal = 2,
			DiagonalCross = 3
		}

		// Laser spread variables
		private const int shotSpacingMax = 750;
		private int shotSpacing = shotSpacingMax;
		private const int spacingVar = 250;
		private const int totalShots = 6;
		private int laserWallType = 0;
		private const float laserWallSpacingOffset = 16f;

		// Continuously reset variables
		public bool AttemptingToEnterPortal = false;
		public int PortalIndex = -1;

		// Spawn variables
		private bool tail = false;
        private const int minLength = 100;
        private const int maxLength = 101;

		// Phase variables
        private bool spawnedGuardians = false;
        private bool spawnedGuardians2 = false;
        private int spawnDoGCountdown = 0;
		private bool hasCreatedPhase1Portal = false;
		private bool phase2Started = false;

		// Phase 2 variables

		// Enums
		private enum LaserWallPhase
		{
			SetUp = 0,
			FireLaserWalls = 1,
			End = 2
		}
		private enum LaserWallType_Phase2
		{
			Normal = 0,
			Offset = 1,
			MultiLayered = 2,
			DiagonalHorizontal = 3,
			DiagonalVertical = 4
		}

		// Laser wall variables
		private const int shotSpacingMax_Phase2 = 1050;
		private int[] shotSpacing_Phase2 = new int[4] { shotSpacingMax_Phase2, shotSpacingMax_Phase2, shotSpacingMax_Phase2, shotSpacingMax_Phase2 };
		private const int spacingVar_Phase2 = 105;
		private const int diagonalSpacingVar = 350;
		private const int totalShots_Phase2 = 20;
		private const int totalDiagonalShots = 6;
		private int laserWallType_Phase2 = 0;
		public int laserWallPhase = 0;

		// Phase variables
		private const int idleCounterMax = 360;
		private int idleCounter = idleCounterMax;
		private int postTeleportTimer = 0;
		private int teleportTimer = -1;
		private bool spawnedGuardians3 = false;
		private int preventBullshitHitsAtStartofFinalPhaseTimer = 0;
		private const float alphaGateValue = 669f;

		public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("The Devourer of Gods");
        }

        public override void SetDefaults()
        {
			npc.Calamity().canBreakPlayerDefense = true;
			npc.GetNPCDamage();
			npc.npcSlots = 5f;
            npc.width = 104;
            npc.height = 104;
            npc.defense = 50;
			npc.LifeMaxNERB(888750, 1066500, 1500000); // Phase 1 is 371250, Phase 2 is 517500
			double HPBoost = CalamityConfig.Instance.BossHealthBoost * 0.01;
            npc.lifeMax += (int)(npc.lifeMax * HPBoost);
            npc.takenDamageMultiplier = 1.1f;
            npc.aiStyle = -1;
            aiType = -1;
            npc.knockBackResist = 0f;
            npc.boss = true;
            npc.value = Item.buyPrice(1, 20, 0, 0);
            npc.alpha = 255;
            npc.behindTiles = true;
            npc.noGravity = true;
            npc.noTileCollide = true;
			npc.DeathSound = SoundID.NPCDeath14;
            npc.netAlways = true;
            music = CalamityMod.Instance.GetMusicFromMusicMod("ScourgeofTheUniverse") ?? MusicID.Boss3;
			bossBag = ModContent.ItemType<DevourerofGodsBag>();
		}

		public override void BossHeadSlot(ref int index)
		{
			if (phase2Started && CalamityWorld.DoGSecondStageCountdown > 60)
				index = -1;
			else if (phase2Started)
				index = phase2IconIndex;
			else
				index = phase1IconIndex;
		}

		public override void BossHeadRotation(ref float rotation)
		{
			if (phase2Started && CalamityWorld.DoGSecondStageCountdown <= 60)
				rotation = npc.rotation;
		}

		public override void SendExtraAI(BinaryWriter writer)
        {
			// Phase 1 syncs
			writer.Write(npc.dontTakeDamage);
			writer.Write(spawnedGuardians);
            writer.Write(spawnedGuardians2);
			writer.Write(spawnedGuardians3);
			writer.Write(phase2Started);
			writer.Write(hasCreatedPhase1Portal);
			writer.Write(spawnDoGCountdown);
			writer.Write(shotSpacing);
			writer.Write(laserWallType);
			writer.Write(PortalIndex);
            for (int i = 0; i < 4; i++)
                writer.Write(npc.Calamity().newAI[i]);

			// Phase 2 syncs
			writer.Write(shotSpacing_Phase2[0]);
			writer.Write(shotSpacing_Phase2[1]);
			writer.Write(shotSpacing_Phase2[2]);
			writer.Write(shotSpacing_Phase2[3]);
			writer.Write(idleCounter);
			writer.Write(laserWallPhase);
			writer.Write(laserWallType_Phase2);
			writer.Write(postTeleportTimer);
			writer.Write(preventBullshitHitsAtStartofFinalPhaseTimer);
			writer.Write(teleportTimer);
			writer.Write(npc.alpha);
		}

        public override void ReceiveExtraAI(BinaryReader reader)
        {
			// Phase 1 syncs
			npc.dontTakeDamage = reader.ReadBoolean();
			spawnedGuardians = reader.ReadBoolean();
			spawnedGuardians2 = reader.ReadBoolean();
			spawnedGuardians3 = reader.ReadBoolean();
			phase2Started = reader.ReadBoolean();
			hasCreatedPhase1Portal = reader.ReadBoolean();
			spawnDoGCountdown = reader.ReadInt32();
			shotSpacing = reader.ReadInt32();
			laserWallType = reader.ReadInt32();
			PortalIndex = reader.ReadInt32();
			for (int i = 0; i < 4; i++)
                npc.Calamity().newAI[i] = reader.ReadSingle();

			// Phase 2 syncs
			shotSpacing_Phase2[0] = reader.ReadInt32();
			shotSpacing_Phase2[1] = reader.ReadInt32();
			shotSpacing_Phase2[2] = reader.ReadInt32();
			shotSpacing_Phase2[3] = reader.ReadInt32();
			idleCounter = reader.ReadInt32();
			laserWallPhase = reader.ReadInt32();
			laserWallType_Phase2 = reader.ReadInt32();
			postTeleportTimer = reader.ReadInt32();
			preventBullshitHitsAtStartofFinalPhaseTimer = reader.ReadInt32();
			teleportTimer = reader.ReadInt32();
			npc.alpha = reader.ReadInt32();
		}

        public override void AI()
        {
			CalamityGlobalNPC calamityGlobalNPC = npc.Calamity();

            // whoAmI variable
            CalamityGlobalNPC.DoGHead = npc.whoAmI;

            // Stop rain
            CalamityMod.StopRain();

            // Variables
            Vector2 vector = npc.Center;
            bool flies = npc.ai[3] == 0f;
			bool enraged = calamityGlobalNPC.enraged > 0;
			bool malice = CalamityWorld.malice || BossRushEvent.BossRushActive;
			bool expertMode = Main.expertMode || malice;
			bool revenge = CalamityWorld.revenge || malice;
			bool death = CalamityWorld.death || malice;
			npc.Calamity().CurrentlyEnraged = (!BossRushEvent.BossRushActive && malice) || enraged;

			// Percent life remaining
			float lifeRatio = npc.life / (float)npc.lifeMax;

			if (revenge)
			{
				// Increase aggression if player is taking a long time to kill the boss
				if (lifeRatio > calamityGlobalNPC.killTimeRatio_IncreasedAggression)
					lifeRatio = calamityGlobalNPC.killTimeRatio_IncreasedAggression;
			}

			// Phase 1 phases
			bool phase2 = lifeRatio < 0.9f;
			bool phase3 = lifeRatio < 0.68f;
			bool summonSentinels = npc.life / (float)npc.lifeMax < 0.6f;

			// Phase 2 phases
			bool phase4 = lifeRatio < 0.48f;
			bool phase5 = lifeRatio < 0.36f;
			bool phase6 = lifeRatio < 0.18f;
			bool phase7 = lifeRatio < 0.09f;

			// Continuously reset certain things.
			AttemptingToEnterPortal = false;

			// Light
			Lighting.AddLight((int)((npc.position.X + (npc.width / 2)) / 16f), (int)((npc.position.Y + (npc.height / 2)) / 16f), 0.2f, 0.05f, 0.2f);

            // Worm variable
            if (npc.ai[2] > 0f)
                npc.realLife = (int)npc.ai[2];

			// Get a target
			if (npc.target < 0 || npc.target == Main.maxPlayers || Main.player[npc.target].dead || !Main.player[npc.target].active)
				npc.TargetClosest();

			// Despawn safety, make sure to target another player if the current player target is too far away
			if (Vector2.Distance(Main.player[npc.target].Center, npc.Center) > CalamityGlobalNPC.CatchUpDistance200Tiles)
				npc.TargetClosest();

			Player player = Main.player[npc.target];

			float distanceFromTarget = Vector2.Distance(player.Center, vector);
			bool increaseSpeed = distanceFromTarget > CalamityGlobalNPC.CatchUpDistance200Tiles;
			bool increaseSpeedMore = distanceFromTarget > CalamityGlobalNPC.CatchUpDistance350Tiles;

			float takeLessDamageDistance = 1600f;
			if (distanceFromTarget > takeLessDamageDistance)
			{
				float damageTakenScalar = MathHelper.Clamp(1f - ((distanceFromTarget - takeLessDamageDistance) / takeLessDamageDistance), 0f, 1f);
				npc.takenDamageMultiplier = MathHelper.Lerp(1f, 1.1f, damageTakenScalar);
			}
			else
				npc.takenDamageMultiplier = 1.1f;

			// Start sentinel phases, only run things that have to happen once in here
			if (summonSentinels)
			{
				if (!phase2Started)
				{
					phase2Started = true;

					// Timed DR and aggression
					calamityGlobalNPC.KillTime = CalamityGlobalNPC.DoGPhase2KillTime;
					calamityGlobalNPC.AITimer = 0;
					calamityGlobalNPC.AIIncreasedAggressionTimer = 0;
					calamityGlobalNPC.killTimeRatio_IncreasedAggression = 0f;

					// Reset important shit
					npc.ai[3] = 0f;
					calamityGlobalNPC.newAI[1] = 0f;
					calamityGlobalNPC.newAI[2] = 0f;

					// Skip the sentinel phase entirely if DoG has already been killed
					CalamityWorld.DoGSecondStageCountdown = (CalamityWorld.downedDoG || CalamityWorld.downedSecondSentinels) ? 600 : 21600;

					if (Main.netMode == NetmodeID.Server)
					{
						var netMessage = mod.GetPacket();
						netMessage.Write((byte)CalamityModMessageType.DoGCountdownSync);
						netMessage.Write(CalamityWorld.DoGSecondStageCountdown);
						netMessage.Send();
					}
				}

				// Play music after the transiton BS
				if (CalamityWorld.DoGSecondStageCountdown == 530)
					music = CalamityMod.Instance.GetMusicFromMusicMod("UniversalCollapse") ?? MusicID.LunarBoss;

				// Once before DoG spawns, set new size and become visible again.
				if (CalamityWorld.DoGSecondStageCountdown == 60)
				{
					npc.position = npc.Center;
					npc.width = 186;
					npc.height = 186;
					npc.position -= npc.Size * 0.5f;
					npc.alpha = 0;
				}

				// Dialogue the moment the second phase starts 
				if (CalamityWorld.DoGSecondStageCountdown == 60)
				{
					string key = "Mods.CalamityMod.EdgyBossText10";
					Color messageColor = Color.Cyan;
					CalamityUtils.DisplayLocalizedText(key, messageColor);
				}
			}

			// Begin phase 2 once all sentinels are down
			if (phase2Started)
			{
				// Go immune and invisible if sentinels are alive
				if (CalamityWorld.DoGSecondStageCountdown > 60)
				{
					// Don't take damage
					npc.dontTakeDamage = true;

					// Adjust movement speed. Direction is unaltered.
					// A portal will be created ahead of where DoG is moving that he will enter before Phase 2 begins.
					float idealFlySpeed = 14f;

					float oldVelocity = npc.velocity.Length();
					npc.velocity = npc.velocity.SafeNormalize(-Vector2.UnitY) * MathHelper.Lerp(oldVelocity, idealFlySpeed, 0.1f);
					npc.rotation = npc.velocity.ToRotation() + MathHelper.PiOver2;

					if (PortalIndex != -1)
					{
						Projectile portal = Main.projectile[PortalIndex];
						float newOpacity = 1f - Utils.InverseLerp(200f, 130f, npc.Distance(portal.Center), true);
						if (Main.netMode != NetmodeID.MultiplayerClient && newOpacity > 0f && npc.Opacity > newOpacity)
						{
							npc.Opacity = newOpacity;
							npc.netUpdate = true;
						}

						if (npc.Opacity < 0.2f)
							npc.Opacity = 0f;

						// Ensure the portal is pointing in the direction of the head at first, to prevent direction offsets.
						if (CalamityWorld.DoGSecondStageCountdown > 360f)
							Main.projectile[PortalIndex].Center = npc.Center + npc.SafeDirectionTo(Main.projectile[PortalIndex].Center) * npc.Distance(Main.projectile[PortalIndex].Center);
					}

					if (Main.netMode != NetmodeID.MultiplayerClient && !hasCreatedPhase1Portal)
					{
						Vector2 portalSpawnPosition = npc.Center + npc.velocity.SafeNormalize(-Vector2.UnitY) * 1000f;
						PortalIndex = Projectile.NewProjectile(portalSpawnPosition, Vector2.Zero, ModContent.ProjectileType<DoGP1EndPortal>(), 0, 0f);

						hasCreatedPhase1Portal = true;
						npc.netUpdate = true;
					}

					AttemptingToEnterPortal = true;
				}

				// Phase 2
				else
				{
					// Immunity after teleport
					npc.dontTakeDamage = postTeleportTimer > 0 || preventBullshitHitsAtStartofFinalPhaseTimer > 0;

					// Teleport
					if (teleportTimer >= 0)
					{
						teleportTimer--;
						if (teleportTimer == 0)
							Teleport(player, malice, death, revenge, expertMode, phase5);
					}

					// Laser walls
					if (phase4 && !phase6 && postTeleportTimer <= 0)
					{
						if (laserWallPhase == (int)LaserWallPhase.SetUp)
						{
							// Increment next laser wall phase timer
							calamityGlobalNPC.newAI[3] += 1f;

							// Set alpha value prior to firing laser walls
							if (calamityGlobalNPC.newAI[3] > alphaGateValue)
							{
								// Disable teleports
								if (teleportTimer >= 0)
								{
									GetRiftLocation(false);
									teleportTimer = -1;
								}

								npc.alpha = (int)MathHelper.Clamp((calamityGlobalNPC.newAI[3] - alphaGateValue) * 5f, 0f, 255f);
							}

							// Fire laser walls every 12 seconds after a laser wall phase ends
							if (calamityGlobalNPC.newAI[3] >= 720f)
							{
								npc.alpha = 255;

								// Reset laser wall timer to 0
								calamityGlobalNPC.newAI[1] = 0f;

								calamityGlobalNPC.newAI[3] = 0f;
								laserWallPhase = (int)LaserWallPhase.FireLaserWalls;
							}
						}
						else if (laserWallPhase == (int)LaserWallPhase.FireLaserWalls)
						{
							// Remain in laser wall firing phase for 6 seconds
							idleCounter--;
							if (idleCounter <= 0)
							{
								SpawnTeleportLocation(player);
								laserWallPhase = (int)LaserWallPhase.End;
								idleCounter = idleCounterMax;
							}
						}
						else if (laserWallPhase == (int)LaserWallPhase.End)
						{
							// End laser wall phase after 4.25 seconds
							npc.alpha -= 1;
							if (npc.alpha <= 0)
							{
								npc.alpha = 0;
								laserWallPhase = (int)LaserWallPhase.SetUp;
							}
						}
					}
					else
					{
						// Set alpha after teleport
						if (postTeleportTimer > 0)
						{
							postTeleportTimer--;
							if (postTeleportTimer < 0)
								postTeleportTimer = 0;

							npc.alpha = postTeleportTimer;
						}
						else
						{
							npc.alpha -= 6;
							if (npc.alpha < 0)
								npc.alpha = 0;
						}

						// This exists so that DoG doesn't sometimes instantly kill the player when he goes to final phase
						if (preventBullshitHitsAtStartofFinalPhaseTimer > 0)
						{
							preventBullshitHitsAtStartofFinalPhaseTimer--;

							if (npc.alpha < 1)
								npc.alpha = 1;
						}

						// Reset laser wall phase
						if (laserWallPhase > (int)LaserWallPhase.SetUp)
							laserWallPhase = (int)LaserWallPhase.SetUp;

						// Enter final phase
						if (!spawnedGuardians3 && phase6)
						{
							SpawnTeleportLocation(player);

							preventBullshitHitsAtStartofFinalPhaseTimer = 180;

							// Anger message
							string key = "Mods.CalamityMod.EdgyBossText11";
							Color messageColor = Color.Cyan;
							CalamityUtils.DisplayLocalizedText(key, messageColor);

							// Summon Thots
							if (Main.netMode != NetmodeID.MultiplayerClient)
							{
								Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/DevourerAttack"), (int)player.position.X, (int)player.position.Y);

								for (int i = 0; i < 3; i++)
									NPC.SpawnOnPlayer(npc.FindClosestPlayer(), ModContent.NPCType<DevourerofGodsHead2>());
							}

							spawnedGuardians3 = true;
						}
					}

					// Spawn segments and fire projectiles
					if (Main.netMode != NetmodeID.MultiplayerClient)
					{
						// Fireballs
						if (npc.alpha <= 0 && distanceFromTarget > 500f)
						{
							calamityGlobalNPC.newAI[0] += 1f;
							if (calamityGlobalNPC.newAI[0] >= 150f && calamityGlobalNPC.newAI[0] % (phase7 ? 60f : 120f) == 0f)
							{
								Vector2 vector44 = new Vector2(npc.position.X + npc.width * 0.5f, npc.position.Y + npc.height * 0.5f);
								float num427 = player.position.X + (player.width / 2) - vector44.X;
								float num428 = player.position.Y + (player.height / 2) - vector44.Y;
								float num430 = 8f;
								float num429 = (float)Math.Sqrt(num427 * num427 + num428 * num428);
								num429 = num430 / num429;
								num427 *= num429;
								num428 *= num429;
								num428 += npc.velocity.Y * 0.5f;
								num427 += npc.velocity.X * 0.5f;
								vector44.X -= num427;
								vector44.Y -= num428;

								int type = ModContent.ProjectileType<DoGFire>();
								int damage = npc.GetProjectileDamage(type);
								Projectile.NewProjectile(vector44.X, vector44.Y, num427, num428, type, damage, 0f, Main.myPlayer);
							}
						}
						else if (distanceFromTarget < 250f)
							calamityGlobalNPC.newAI[0] = 0f;

						// Laser walls
						if (!phase6 && laserWallPhase == (int)LaserWallPhase.FireLaserWalls)
						{
							float speed = 12f;
							float spawnOffset = 1500f;
							float divisor = enraged ? 80f : malice ? 100f : 120f;

							if (calamityGlobalNPC.newAI[1] % divisor == 0f)
							{
								Main.PlaySound(SoundID.Item12, player.position);

								// Side walls
								float targetPosY = player.position.Y;
								int type = ModContent.ProjectileType<DoGDeath>();
								int damage = npc.GetProjectileDamage(type);
								int halfTotalDiagonalShots = totalDiagonalShots / 2;
								Vector2 start = default;
								Vector2 velocity = default;
								Vector2 aim = expertMode ? player.Center + player.velocity * 20f : Vector2.Zero;

								switch (laserWallType_Phase2)
								{
									case (int)LaserWallType_Phase2.Normal:

										for (int x = 0; x < totalShots_Phase2; x++)
										{
											Projectile.NewProjectile(player.position.X + spawnOffset, targetPosY + shotSpacing_Phase2[0], -speed, 0f, type, damage, 0f, Main.myPlayer);
											Projectile.NewProjectile(player.position.X - spawnOffset, targetPosY + shotSpacing_Phase2[0], speed, 0f, type, damage, 0f, Main.myPlayer);
											shotSpacing_Phase2[0] -= spacingVar_Phase2;
										}

										if (expertMode)
										{
											Projectile.NewProjectile(player.position.X + spawnOffset, player.Center.Y, -speed, 0f, type, damage, 0f, Main.myPlayer);
											Projectile.NewProjectile(player.position.X - spawnOffset, player.Center.Y, speed, 0f, type, damage, 0f, Main.myPlayer);
										}

										laserWallType_Phase2 = (int)LaserWallType_Phase2.Offset;
										break;

									case (int)LaserWallType_Phase2.Offset:

										targetPosY += 50f;
										for (int x = 0; x < totalShots_Phase2; x++)
										{
											Projectile.NewProjectile(player.position.X + spawnOffset, targetPosY + shotSpacing_Phase2[0], -speed, 0f, type, damage, 0f, Main.myPlayer);
											Projectile.NewProjectile(player.position.X - spawnOffset, targetPosY + shotSpacing_Phase2[0], speed, 0f, type, damage, 0f, Main.myPlayer);
											shotSpacing_Phase2[0] -= spacingVar_Phase2;
										}

										if (expertMode)
										{
											Projectile.NewProjectile(player.position.X + spawnOffset, player.Center.Y, -speed, 0f, type, damage, 0f, Main.myPlayer);
											Projectile.NewProjectile(player.position.X - spawnOffset, player.Center.Y, speed, 0f, type, damage, 0f, Main.myPlayer);
										}

										laserWallType_Phase2 = revenge ? (int)LaserWallType_Phase2.MultiLayered : expertMode ? (int)LaserWallType_Phase2.DiagonalHorizontal : (int)LaserWallType_Phase2.Normal;
										break;

									case (int)LaserWallType_Phase2.MultiLayered:

										for (int x = 0; x < totalShots_Phase2; x++)
										{
											Projectile.NewProjectile(player.position.X + spawnOffset, targetPosY + shotSpacing_Phase2[0], -speed, 0f, type, damage, 0f, Main.myPlayer);
											Projectile.NewProjectile(player.position.X - spawnOffset, targetPosY + shotSpacing_Phase2[0], speed, 0f, type, damage, 0f, Main.myPlayer);
											shotSpacing_Phase2[0] -= spacingVar_Phase2;
										}

										int totalBonusLasers = totalShots_Phase2 / 2;
										for (int x = 0; x < totalBonusLasers; x++)
										{
											Projectile.NewProjectile(player.position.X + spawnOffset, targetPosY + shotSpacing_Phase2[3], -speed, 0f, type, damage, 0f, Main.myPlayer);
											Projectile.NewProjectile(player.position.X - spawnOffset, targetPosY + shotSpacing_Phase2[3], speed, 0f, type, damage, 0f, Main.myPlayer);
											shotSpacing_Phase2[3] -= Main.rand.NextBool(2) ? 180 : 200;
										}

										Projectile.NewProjectile(player.position.X + spawnOffset, player.Center.Y, -speed, 0f, type, damage, 0f, Main.myPlayer);
										Projectile.NewProjectile(player.position.X - spawnOffset, player.Center.Y, speed, 0f, type, damage, 0f, Main.myPlayer);

										laserWallType_Phase2 = (int)LaserWallType_Phase2.DiagonalHorizontal;
										break;

									case (int)LaserWallType_Phase2.DiagonalHorizontal:

										for (int x = 0; x < totalDiagonalShots + 1; x++)
										{
											start = new Vector2(player.position.X + spawnOffset, targetPosY + shotSpacing_Phase2[0]);
											aim.Y += laserWallSpacingOffset * (x - halfTotalDiagonalShots);
											velocity = Vector2.Normalize(aim - start) * speed;
											Projectile.NewProjectile(start, velocity, type, damage, 0f, Main.myPlayer);

											start = new Vector2(player.position.X - spawnOffset, targetPosY + shotSpacing_Phase2[0]);
											velocity = Vector2.Normalize(aim - start) * speed;
											Projectile.NewProjectile(start, velocity, type, damage, 0f, Main.myPlayer);

											shotSpacing_Phase2[0] -= diagonalSpacingVar;
										}

										Projectile.NewProjectile(player.Center.X, targetPosY + spawnOffset, 0f, -speed, type, damage, 0f, Main.myPlayer);
										Projectile.NewProjectile(player.Center.X, targetPosY - spawnOffset, 0f, speed, type, damage, 0f, Main.myPlayer);

										laserWallType_Phase2 = revenge ? (int)LaserWallType_Phase2.DiagonalVertical : (int)LaserWallType_Phase2.Normal;
										break;

									case (int)LaserWallType_Phase2.DiagonalVertical:

										for (int x = 0; x < totalDiagonalShots + 1; x++)
										{
											start = new Vector2(player.position.X + shotSpacing_Phase2[0], targetPosY + spawnOffset);
											aim.X += laserWallSpacingOffset * (x - halfTotalDiagonalShots);
											velocity = Vector2.Normalize(aim - start) * speed;
											Projectile.NewProjectile(start, velocity, type, damage, 0f, Main.myPlayer);

											start = new Vector2(player.position.X + shotSpacing_Phase2[0], targetPosY - spawnOffset);
											velocity = Vector2.Normalize(aim - start) * speed;
											Projectile.NewProjectile(start, velocity, type, damage, 0f, Main.myPlayer);

											shotSpacing_Phase2[0] -= diagonalSpacingVar;
										}

										Projectile.NewProjectile(player.position.X + spawnOffset, player.Center.Y, -speed, 0f, type, damage, 0f, Main.myPlayer);
										Projectile.NewProjectile(player.position.X - spawnOffset, player.Center.Y, speed, 0f, type, damage, 0f, Main.myPlayer);

										laserWallType_Phase2 = (int)LaserWallType_Phase2.Normal;
										break;
								}

								// Lower wall
								for (int x = 0; x < totalShots_Phase2; x++)
								{
									Projectile.NewProjectile(player.position.X + shotSpacing_Phase2[1], player.position.Y + spawnOffset, 0f, -speed, type, damage, 0f, Main.myPlayer);
									shotSpacing_Phase2[1] -= spacingVar_Phase2;
								}

								// Upper wall
								for (int x = 0; x < totalShots_Phase2; x++)
								{
									Projectile.NewProjectile(player.position.X + shotSpacing_Phase2[2], player.position.Y - spawnOffset, 0f, speed, type, damage, 0f, Main.myPlayer);
									shotSpacing_Phase2[2] -= spacingVar_Phase2;
								}

								for (int i = 0; i < shotSpacing_Phase2.Length; i++)
									shotSpacing_Phase2[i] = shotSpacingMax_Phase2;
							}

							calamityGlobalNPC.newAI[1] += 1f;
						}
					}

					float fallSpeed = enraged ? 21f : malice ? 19.5f : death ? 17.75f : 16f;

					if (expertMode)
						fallSpeed += 3.5f * (1f - lifeRatio);

					if (player.dead)
					{
						flies = true;

						npc.velocity.Y -= 1f;
						if ((double)npc.position.Y < Main.topWorld + 16f)
						{
							npc.velocity.Y -= 1f;
							fallSpeed = 32f;
						}

						int bodyType = ModContent.NPCType<DevourerofGodsBody>();
						int tailType = ModContent.NPCType<DevourerofGodsTail>();
						if ((double)npc.position.Y < Main.topWorld + 16f)
						{
							for (int a = 0; a < Main.maxNPCs; a++)
							{
								if (Main.npc[a].type != npc.type && Main.npc[a].type != bodyType && Main.npc[a].type != tailType)
									continue;

								Main.npc[a].active = false;
								Main.npc[a].netUpdate = true;
							}
						}
					}

					// Movement
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

					if (npc.velocity.X < 0f)
						npc.spriteDirection = -1;
					else if (npc.velocity.X > 0f)
						npc.spriteDirection = 1;

					int phaseLimit = death ? 600 : 900;

					// Flight
					if (npc.ai[3] == 0f)
					{
						if (Main.netMode != NetmodeID.Server)
						{
							if (!Main.player[Main.myPlayer].dead && Main.player[Main.myPlayer].active && Vector2.Distance(Main.player[Main.myPlayer].Center, vector) < CalamityGlobalNPC.CatchUpDistance350Tiles)
								Main.player[Main.myPlayer].AddBuff(ModContent.BuffType<Warped>(), 2);
						}

						// Charge in a direction for a second until the timer is back at 0
						if (postTeleportTimer > 0)
						{
							npc.rotation = (float)Math.Atan2(npc.velocity.Y, npc.velocity.X) + MathHelper.PiOver2;
							return;
						}

						calamityGlobalNPC.newAI[2] += 1f;

						npc.localAI[1] = 0f;

						float speed = enraged ? 20f : malice ? 18f : death ? 16.5f : 15f;
						float turnSpeed = enraged ? 0.4f : malice ? 0.36f : death ? 0.33f : 0.3f;
						float homingSpeed = enraged ? 40f : malice ? 36f : death ? 30f : 24f;
						float homingTurnSpeed = enraged ? 0.55f : malice ? 0.48f : death ? 0.405f : 0.33f;

						if (expertMode)
						{
							phaseLimit /= 1 + (int)(5f * (1f - lifeRatio));

							if (phaseLimit < 180)
								phaseLimit = 180;

							speed += 3f * (1f - lifeRatio);
							turnSpeed += 0.06f * (1f - lifeRatio);
							homingSpeed += 12f * (1f - lifeRatio);
							homingTurnSpeed += 0.15f * (1f - lifeRatio);
						}

						// Go to ground phase sooner
						if (increaseSpeedMore)
						{
							if (laserWallPhase == (int)LaserWallPhase.SetUp && calamityGlobalNPC.newAI[3] <= alphaGateValue)
								SpawnTeleportLocation(player);
							else
								calamityGlobalNPC.newAI[2] += 10f;
						}
						else
							calamityGlobalNPC.newAI[2] += 2f;

						float num188 = speed;
						float num189 = turnSpeed;
						Vector2 vector18 = npc.Center;
						float num191 = player.Center.X;
						float num192 = player.Center.Y;
						int num42 = -1;
						int num43 = (int)(player.Center.X / 16f);
						int num44 = (int)(player.Center.Y / 16f);

						// Charge at target for 1.5 seconds
						bool flyAtTarget = (!phase4 || phase6) && calamityGlobalNPC.newAI[2] > phaseLimit - 90 && revenge;

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

						if (!flyAtTarget)
						{
							if (num42 > 0)
							{
								num42 *= 16;
								float num47 = num42 - 800;
								if (player.position.Y > num47)
								{
									num192 = num47;
									if (Math.Abs(npc.Center.X - player.Center.X) < 500f)
									{
										if (npc.velocity.X > 0f)
											num191 = player.Center.X + 600f;
										else
											num191 = player.Center.X - 600f;
									}
								}
							}
						}
						else
						{
							num188 = homingSpeed;
							num189 = homingTurnSpeed;
						}

						if (expertMode)
						{
							num188 += Vector2.Distance(player.Center, npc.Center) * 0.005f * (1f - lifeRatio);
							num189 += Vector2.Distance(player.Center, npc.Center) * 0.0001f * (1f - lifeRatio);
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
								npc.velocity.X += num189;
							else
							{
								if (npc.velocity.X > num191)
									npc.velocity.X -= num189;
							}

							if (npc.velocity.Y < num192)
								npc.velocity.Y += num189;
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

						if (calamityGlobalNPC.newAI[2] > phaseLimit)
						{
							npc.ai[3] = 1f;
							calamityGlobalNPC.newAI[2] = 0f;
							npc.TargetClosest();
							npc.netUpdate = true;
						}
					}

					// Ground
					else
					{
						if (Main.netMode != NetmodeID.Server)
						{
							if (!Main.player[Main.myPlayer].dead && Main.player[Main.myPlayer].active && Vector2.Distance(Main.player[Main.myPlayer].Center, vector) < CalamityGlobalNPC.CatchUpDistance350Tiles)
								Main.player[Main.myPlayer].AddBuff(ModContent.BuffType<ExtremeGrav>(), 2);
						}

						// Charge in a direction for a second until the timer is back at 0
						if (postTeleportTimer > 0)
						{
							npc.rotation = (float)Math.Atan2(npc.velocity.Y, npc.velocity.X) + MathHelper.PiOver2;
							return;
						}

						calamityGlobalNPC.newAI[2] += 1f;

						float turnSpeed = enraged ? 0.36f : malice ? 0.3f : death ? 0.24f : 0.18f;

						if (expertMode)
						{
							turnSpeed += 0.1f * (1f - lifeRatio);
							turnSpeed += Vector2.Distance(player.Center, npc.Center) * 0.00005f * (1f - lifeRatio);
						}

						// Enrage
						if (increaseSpeedMore)
						{
							if (laserWallPhase == (int)LaserWallPhase.SetUp && calamityGlobalNPC.newAI[3] <= alphaGateValue)
								SpawnTeleportLocation(player);
							else
							{
								fallSpeed *= 3f;
								turnSpeed *= 6f;
							}
						}
						else if (increaseSpeed)
						{
							fallSpeed *= 1.5f;
							turnSpeed *= 3f;
						}

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

							int num954 = death ? 1125 : 1200;
							if (lifeRatio < 0.8f && lifeRatio > 0.2f && !death)
								num954 = 1400;

							if (expertMode)
								num954 -= (int)(150f * (1f - lifeRatio));

							if (num954 < 1050)
								num954 = 1050;

							bool flag95 = true;
							if (npc.position.Y > player.position.Y)
							{
								for (int num955 = 0; num955 < Main.maxPlayers; num955++)
								{
									if (Main.player[num955].active)
									{
										Rectangle rectangle13 = new Rectangle((int)Main.player[num955].position.X - 1000, (int)Main.player[num955].position.Y - 1000, 2000, num954);
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

						float num189 = turnSpeed;
						Vector2 vector18 = npc.Center;
						float num191 = player.Center.X;
						float num192 = player.Center.Y;
						num191 = (int)(num191 / 16f) * 16;
						num192 = (int)(num192 / 16f) * 16;
						vector18.X = (int)(vector18.X / 16f) * 16;
						vector18.Y = (int)(vector18.Y / 16f) * 16;
						num191 -= vector18.X;
						num192 -= vector18.Y;
						float num193 = (float)Math.Sqrt(num191 * num191 + num192 * num192);

						if (!flies)
						{
							npc.velocity.Y += turnSpeed;
							if (npc.velocity.Y > fallSpeed)
								npc.velocity.Y = fallSpeed;

							if ((Math.Abs(npc.velocity.X) + Math.Abs(npc.velocity.Y)) < fallSpeed * 2.2)
							{
								if (npc.velocity.X < 0f)
									npc.velocity.X -= num189 * 1.1f;
								else
									npc.velocity.X += num189 * 1.1f;
							}
							else if (npc.velocity.Y == fallSpeed)
							{
								if (npc.velocity.X < num191)
									npc.velocity.X += num189;
								else if (npc.velocity.X > num191)
									npc.velocity.X -= num189;
							}
							else if (npc.velocity.Y > 4f)
							{
								if (npc.velocity.X < 0f)
									npc.velocity.X += num189 * 0.9f;
								else
									npc.velocity.X -= num189 * 0.9f;
							}
						}
						else
						{
							double maximumSpeed1 = enraged ? 0.6 : malice ? 0.52 : death ? 0.46 : 0.4;
							double maximumSpeed2 = enraged ? 1.4 : malice ? 1.25 : death ? 1.125 : 1D;

							if (increaseSpeedMore)
							{
								maximumSpeed1 *= 4;
								maximumSpeed2 *= 4;
							}
							if (increaseSpeed)
							{
								maximumSpeed1 *= 2;
								maximumSpeed2 *= 2;
							}

							if (expertMode)
							{
								maximumSpeed1 += 0.1f * (1f - lifeRatio);
								maximumSpeed2 += 0.2f * (1f - lifeRatio);
							}

							num193 = (float)Math.Sqrt(num191 * num191 + num192 * num192);
							float num25 = Math.Abs(num191);
							float num26 = Math.Abs(num192);
							float num27 = fallSpeed / num193;
							num191 *= num27;
							num192 *= num27;

							if (((npc.velocity.X > 0f && num191 > 0f) || (npc.velocity.X < 0f && num191 < 0f)) && ((npc.velocity.Y > 0f && num192 > 0f) || (npc.velocity.Y < 0f && num192 < 0f)))
							{
								if (npc.velocity.X < num191)
									npc.velocity.X += turnSpeed * 1.5f;
								else if (npc.velocity.X > num191)
									npc.velocity.X -= turnSpeed * 1.5f;

								if (npc.velocity.Y < num192)
									npc.velocity.Y += turnSpeed * 1.5f;
								else if (npc.velocity.Y > num192)
									npc.velocity.Y -= turnSpeed * 1.5f;
							}

							if ((npc.velocity.X > 0f && num191 > 0f) || (npc.velocity.X < 0f && num191 < 0f) || (npc.velocity.Y > 0f && num192 > 0f) || (npc.velocity.Y < 0f && num192 < 0f))
							{
								if (npc.velocity.X < num191)
									npc.velocity.X += turnSpeed;
								else if (npc.velocity.X > num191)
									npc.velocity.X -= turnSpeed;

								if (npc.velocity.Y < num192)
									npc.velocity.Y += turnSpeed;
								else if (npc.velocity.Y > num192)
									npc.velocity.Y -= turnSpeed;

								if (Math.Abs(num192) < fallSpeed * maximumSpeed1 && ((npc.velocity.X > 0f && num191 < 0f) || (npc.velocity.X < 0f && num191 > 0f)))
								{
									if (npc.velocity.Y > 0f)
										npc.velocity.Y += turnSpeed * 2f;
									else
										npc.velocity.Y -= turnSpeed * 2f;
								}

								if (Math.Abs(num191) < fallSpeed * maximumSpeed1 && ((npc.velocity.Y > 0f && num192 < 0f) || (npc.velocity.Y < 0f && num192 > 0f)))
								{
									if (npc.velocity.X > 0f)
										npc.velocity.X += turnSpeed * 2f;
									else
										npc.velocity.X -= turnSpeed * 2f;
								}
							}
							else if (num25 > num26)
							{
								if (npc.velocity.X < num191)
									npc.velocity.X += turnSpeed * 1.1f;
								else if (npc.velocity.X > num191)
									npc.velocity.X -= turnSpeed * 1.1f;

								if ((Math.Abs(npc.velocity.X) + Math.Abs(npc.velocity.Y)) < fallSpeed * maximumSpeed2)
								{
									if (npc.velocity.Y > 0f)
										npc.velocity.Y += turnSpeed;
									else
										npc.velocity.Y -= turnSpeed;
								}
							}
							else
							{
								if (npc.velocity.Y < num192)
									npc.velocity.Y += turnSpeed * 1.1f;
								else if (npc.velocity.Y > num192)
									npc.velocity.Y -= turnSpeed * 1.1f;

								if ((Math.Abs(npc.velocity.X) + Math.Abs(npc.velocity.Y)) < fallSpeed * maximumSpeed2)
								{
									if (npc.velocity.X > 0f)
										npc.velocity.X += turnSpeed;
									else
										npc.velocity.X -= turnSpeed;
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
							npc.netUpdate = true;

						if (calamityGlobalNPC.newAI[2] > phaseLimit)
						{
							npc.ai[3] = 0f;
							calamityGlobalNPC.newAI[2] = 0f;
							npc.TargetClosest();
							npc.netUpdate = true;
						}
					}
				}
			}
			else
			{
				// Spawn Guardians
				if (phase3)
				{
					if (!spawnedGuardians)
					{
						if (revenge)
							spawnDoGCountdown = 10;

						string key = "Mods.CalamityMod.EdgyBossText";
						Color messageColor = Color.Cyan;
						CalamityUtils.DisplayLocalizedText(key, messageColor);

						npc.TargetClosest();
						spawnedGuardians = true;
					}

					if (spawnDoGCountdown > 0)
					{
						spawnDoGCountdown--;
						if (spawnDoGCountdown == 0 && Main.netMode != NetmodeID.MultiplayerClient)
						{
							Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/DevourerAttack"), (int)player.position.X, (int)player.position.Y);

							for (int i = 0; i < 2; i++)
								NPC.SpawnOnPlayer(npc.FindClosestPlayer(), ModContent.NPCType<DevourerofGodsHead2>());
						}
					}
				}
				else if (phase2)
				{
					if (!spawnedGuardians2)
					{
						if (revenge)
							spawnDoGCountdown = 10;

						spawnedGuardians2 = true;
					}

					if (spawnDoGCountdown > 0)
					{
						spawnDoGCountdown--;
						if (spawnDoGCountdown == 0 && Main.netMode != NetmodeID.MultiplayerClient)
						{
							Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/DevourerAttack"), (int)player.position.X, (int)player.position.Y);

							NPC.SpawnOnPlayer(npc.FindClosestPlayer(), ModContent.NPCType<DevourerofGodsHead2>());
						}
					}
				}

				// Alpha
				npc.alpha -= 12;
				if (npc.alpha < 0)
					npc.alpha = 0;

				// Spawn segments
				if (Main.netMode != NetmodeID.MultiplayerClient)
				{
					if (!tail && npc.ai[0] == 0f)
					{
						int Previous = npc.whoAmI;
						for (int segmentSpawn = 0; segmentSpawn < maxLength; segmentSpawn++)
						{
							int segment;
							if (segmentSpawn >= 0 && segmentSpawn < minLength)
								segment = NPC.NewNPC((int)npc.position.X + (npc.width / 2), (int)npc.position.Y + (npc.height / 2), ModContent.NPCType<DevourerofGodsBody>(), npc.whoAmI);
							else
								segment = NPC.NewNPC((int)npc.position.X + (npc.width / 2), (int)npc.position.Y + (npc.height / 2), ModContent.NPCType<DevourerofGodsTail>(), npc.whoAmI);

							Main.npc[segment].realLife = npc.whoAmI;
							Main.npc[segment].ai[2] = npc.whoAmI;
							Main.npc[segment].ai[1] = Previous;
							Main.npc[Previous].ai[0] = segment;
							NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, segment, 0f, 0f, 0f, 0);
							Previous = segment;
						}
						tail = true;
					}

					if (phase2)
					{
						float speed = 12f;
						float spawnOffset = 1500f;
						float divisor = malice ? 240f : death ? 360f : 480f;

						if (calamityGlobalNPC.newAI[1] % divisor == 0f)
						{
							Main.PlaySound(SoundID.Item12, player.position);

							// Side walls
							int type = ModContent.ProjectileType<DoGDeath>();
							int damage = npc.GetProjectileDamage(type);
							Vector2 start = default;
							Vector2 velocity = default;
							Vector2 aim = expertMode ? player.Center + player.velocity * 20f : Vector2.Zero;
							Vector2 aimClone = aim;

							switch (laserWallType)
							{
								case (int)LaserWallType.DiagonalRight:

									for (int x = 0; x < totalShots + 1; x++)
									{
										start = new Vector2(player.position.X + spawnOffset, player.position.Y + shotSpacing);
										aim.Y += laserWallSpacingOffset * (x - 3);
										velocity = Vector2.Normalize(aim - start) * speed;
										Projectile.NewProjectile(start, velocity, type, damage, 0f, Main.myPlayer);

										shotSpacing -= spacingVar;
									}

									if (expertMode)
										Projectile.NewProjectile(player.position.X + spawnOffset, player.Center.Y, -speed, 0f, type, damage, 0f, Main.myPlayer);

									laserWallType = (int)LaserWallType.DiagonalLeft;
									break;

								case (int)LaserWallType.DiagonalLeft:

									for (int x = 0; x < totalShots + 1; x++)
									{
										start = new Vector2(player.position.X - spawnOffset, player.position.Y + shotSpacing);
										aim.Y += laserWallSpacingOffset * (x - 3);
										velocity = Vector2.Normalize(aim - start) * speed;
										Projectile.NewProjectile(start, velocity, type, damage, 0f, Main.myPlayer);

										shotSpacing -= spacingVar;
									}

									if (expertMode)
										Projectile.NewProjectile(player.position.X - spawnOffset, player.Center.Y, speed, 0f, type, damage, 0f, Main.myPlayer);

									laserWallType = expertMode ? (int)LaserWallType.DiagonalHorizontal : (int)LaserWallType.DiagonalRight;
									break;

								case (int)LaserWallType.DiagonalHorizontal:

									for (int x = 0; x < totalShots + 1; x++)
									{
										start = new Vector2(player.position.X + spawnOffset, player.position.Y + shotSpacing);
										aim.Y += laserWallSpacingOffset * (x - 3);
										velocity = Vector2.Normalize(aim - start) * speed;
										Projectile.NewProjectile(start, velocity, type, damage, 0f, Main.myPlayer);

										start = new Vector2(player.position.X - spawnOffset, player.position.Y + shotSpacing);
										velocity = Vector2.Normalize(aim - start) * speed;
										Projectile.NewProjectile(start, velocity, type, damage, 0f, Main.myPlayer);

										shotSpacing -= spacingVar;
									}

									if (expertMode)
									{
										Projectile.NewProjectile(player.position.X + spawnOffset, player.Center.Y, -speed, 0f, type, damage, 0f, Main.myPlayer);
										Projectile.NewProjectile(player.position.X - spawnOffset, player.Center.Y, speed, 0f, type, damage, 0f, Main.myPlayer);
									}

									laserWallType = revenge ? (int)LaserWallType.DiagonalCross : (int)LaserWallType.DiagonalRight;
									break;

								case (int)LaserWallType.DiagonalCross:

									for (int x = 0; x < totalShots + 1; x++)
									{
										start = new Vector2(player.position.X + spawnOffset, player.position.Y + shotSpacing);
										aim.Y += laserWallSpacingOffset * (x - 3);
										velocity = Vector2.Normalize(aim - start) * speed;
										Projectile.NewProjectile(start, velocity, type, damage, 0f, Main.myPlayer);

										start = new Vector2(player.position.X - spawnOffset, player.position.Y + shotSpacing);
										velocity = Vector2.Normalize(aim - start) * speed;
										Projectile.NewProjectile(start, velocity, type, damage, 0f, Main.myPlayer);

										start = new Vector2(player.position.X + shotSpacing, player.position.Y + spawnOffset);
										aimClone.X += laserWallSpacingOffset * (x - 3);
										velocity = Vector2.Normalize(aimClone - start) * speed;
										Projectile.NewProjectile(start, velocity, type, damage, 0f, Main.myPlayer);

										start = new Vector2(player.position.X + shotSpacing, player.position.Y - spawnOffset);
										velocity = Vector2.Normalize(aimClone - start) * speed;
										Projectile.NewProjectile(start, velocity, type, damage, 0f, Main.myPlayer);

										shotSpacing -= spacingVar;
									}

									if (expertMode)
									{
										Projectile.NewProjectile(player.position.X + spawnOffset, player.Center.Y, -speed, 0f, type, damage, 0f, Main.myPlayer);
										Projectile.NewProjectile(player.position.X - spawnOffset, player.Center.Y, speed, 0f, type, damage, 0f, Main.myPlayer);
									}

									laserWallType = (int)LaserWallType.DiagonalRight;
									break;
							}
							shotSpacing = shotSpacingMax;
						}

						calamityGlobalNPC.newAI[1] += 1f;
					}
					else
						calamityGlobalNPC.newAI[1] = 0f;
				}

				// Despawn
				if (player.dead)
				{
					flies = true;

					npc.velocity.Y -= 1f;
					if ((double)npc.position.Y < Main.topWorld + 16f)
						npc.velocity.Y -= 1f;

					int bodyType = ModContent.NPCType<DevourerofGodsBody>();
					int tailType = ModContent.NPCType<DevourerofGodsTail>();
					if ((double)npc.position.Y < Main.topWorld + 16f)
					{
						for (int a = 0; a < Main.maxNPCs; a++)
						{
							if (Main.npc[a].type != npc.type && Main.npc[a].type != bodyType && Main.npc[a].type != tailType)
								continue;

							Main.npc[a].active = false;
							Main.npc[a].netUpdate = true;
						}
					}
				}

				// Movement
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

				if (npc.velocity.X < 0f)
					npc.spriteDirection = -1;
				else if (npc.velocity.X > 0f)
					npc.spriteDirection = 1;

				// Flight
				if (npc.ai[3] == 0f)
				{
					if (Main.netMode != NetmodeID.Server)
					{
						if (!Main.player[Main.myPlayer].dead && Main.player[Main.myPlayer].active && Vector2.Distance(Main.player[Main.myPlayer].Center, vector) < CalamityGlobalNPC.CatchUpDistance350Tiles)
							Main.player[Main.myPlayer].AddBuff(ModContent.BuffType<Warped>(), 2);
					}

					// Flying movement
					npc.localAI[1] = 0f;

					float phaseChangeRate = 1f + (expertMode ? 9f * (1f - lifeRatio) : 0f);
					calamityGlobalNPC.newAI[2] += phaseChangeRate;

					float speed = malice ? 18f : death ? 16.5f : 15f;
					float turnSpeed = malice ? 0.36f : death ? 0.33f : 0.3f;
					float homingSpeed = malice ? 27f : death ? 22.5f : 18f;
					float homingTurnSpeed = malice ? 0.48f : death ? 0.405f : 0.33f;

					if (expertMode)
					{
						speed += 3f * (1f - lifeRatio);
						turnSpeed += 0.06f * (1f - lifeRatio);
						homingSpeed += 9f * (1f - lifeRatio);
						homingTurnSpeed += 0.15f * (1f - lifeRatio);
					}

					// Go to ground phase sooner
					if (increaseSpeedMore)
						calamityGlobalNPC.newAI[2] += 10f;
					else if (increaseSpeed)
						calamityGlobalNPC.newAI[2] += 2f;

					float num188 = speed;
					float num189 = turnSpeed;
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

					if (num42 > 0)
					{
						num42 *= 16;
						float num47 = num42 - 800;
						if (player.position.Y > num47)
						{
							num192 = num47;
							if (Math.Abs(npc.Center.X - player.Center.X) < 500f)
							{
								if (npc.velocity.X > 0f)
									num191 = player.Center.X + 600f;
								else
									num191 = player.Center.X - 600f;
							}
						}
					}
					else
					{
						num188 = homingSpeed;
						num189 = homingTurnSpeed;
					}

					if (expertMode)
					{
						num188 += distanceFromTarget * 0.005f * (1f - lifeRatio);
						num189 += distanceFromTarget * 0.0001f * (1f - lifeRatio);
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
							npc.velocity.X += num189;
						else
						{
							if (npc.velocity.X > num191)
								npc.velocity.X -= num189;
						}

						if (npc.velocity.Y < num192)
							npc.velocity.Y += num189;
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

					if (calamityGlobalNPC.newAI[2] > 900f)
					{
						npc.ai[3] = 1f;
						calamityGlobalNPC.newAI[2] = 0f;
						npc.TargetClosest();
						npc.netUpdate = true;
					}
				}

				// Ground
				else
				{
					if (Main.netMode != NetmodeID.Server)
					{
						if (!Main.player[Main.myPlayer].dead && Main.player[Main.myPlayer].active && Vector2.Distance(Main.player[Main.myPlayer].Center, vector) < CalamityGlobalNPC.CatchUpDistance350Tiles)
							Main.player[Main.myPlayer].AddBuff(ModContent.BuffType<ExtremeGrav>(), 2);
					}

					calamityGlobalNPC.newAI[2] += 1f;

					float fallSpeed = malice ? 19.5f : death ? 17.75f : 16f;
					float speed = malice ? 0.26f : death ? 0.22f : 0.18f;
					float turnSpeed = malice ? 0.24f : death ? 0.18f : 0.12f;

					if (expertMode)
					{
						fallSpeed += 3.5f * (1f - lifeRatio);
						speed += 0.08f * (1f - lifeRatio);
						turnSpeed += 0.12f * (1f - lifeRatio);
						speed += distanceFromTarget * 0.00005f * (1f - lifeRatio);
						turnSpeed += distanceFromTarget * 0.00005f * (1f - lifeRatio);
					}

					// Enrage
					if (increaseSpeedMore)
					{
						fallSpeed *= 3f;
						speed *= 4f;
						turnSpeed *= 6f;
					}
					else if (increaseSpeed)
					{
						fallSpeed *= 1.5f;
						speed *= 2f;
						turnSpeed *= 3f;
					}

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

						int num954 = death ? 1100 : 1175;
						if (expertMode)
							num954 -= (int)(150f * (1f - lifeRatio));
						if (num954 < 1025)
							num954 = 1025;

						bool flag95 = true;
						if (npc.position.Y > player.position.Y)
						{
							for (int num955 = 0; num955 < 255; num955++)
							{
								if (Main.player[num955].active)
								{
									Rectangle rectangle13 = new Rectangle((int)Main.player[num955].position.X - 1000, (int)Main.player[num955].position.Y - 1000, 2000, num954);
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

					Vector2 vector3 = new Vector2(npc.position.X + npc.width * 0.5f, npc.position.Y + npc.height * 0.5f);
					float num20 = player.position.X + (player.width / 2);
					float num21 = player.position.Y + (player.height / 2);
					num20 = (int)(num20 / 16f) * 16;
					num21 = (int)(num21 / 16f) * 16;
					vector3.X = (int)(vector3.X / 16f) * 16;
					vector3.Y = (int)(vector3.Y / 16f) * 16;
					num20 -= vector3.X;
					num21 -= vector3.Y;
					float num22 = (float)Math.Sqrt(num20 * num20 + num21 * num21);

					if (!flies)
					{
						npc.velocity.Y += turnSpeed;
						if (npc.velocity.Y > fallSpeed)
							npc.velocity.Y = fallSpeed;

						if ((Math.Abs(npc.velocity.X) + Math.Abs(npc.velocity.Y)) < fallSpeed * 1.8)
						{
							if (npc.velocity.X < 0f)
								npc.velocity.X -= speed * 1.1f;
							else
								npc.velocity.X += speed * 1.1f;
						}
						else if (npc.velocity.Y == fallSpeed)
						{
							if (npc.velocity.X < num20)
								npc.velocity.X += speed;
							else if (npc.velocity.X > num20)
								npc.velocity.X -= speed;
						}
						else if (npc.velocity.Y > 4f)
						{
							if (npc.velocity.X < 0f)
								npc.velocity.X += speed * 0.9f;
							else
								npc.velocity.X -= speed * 0.9f;
						}
					}
					else
					{

						double maximumSpeed1 = malice ? 0.48 : death ? 0.44 : 0.4;
						double maximumSpeed2 = malice ? 1.16 : death ? 1.08 : 1D;

						if (increaseSpeedMore)
						{
							maximumSpeed1 *= 4;
							maximumSpeed2 *= 4;
						}
						if (increaseSpeed)
						{
							maximumSpeed1 *= 2;
							maximumSpeed2 *= 2;
						}

						if (expertMode)
						{
							maximumSpeed1 += 0.08f * (1f - lifeRatio);
							maximumSpeed2 += 0.16f * (1f - lifeRatio);
						}

						num22 = (float)Math.Sqrt(num20 * num20 + num21 * num21);
						float num25 = Math.Abs(num20);
						float num26 = Math.Abs(num21);
						float num27 = fallSpeed / num22;
						num20 *= num27;
						num21 *= num27;

						if (((npc.velocity.X > 0f && num20 > 0f) || (npc.velocity.X < 0f && num20 < 0f)) && ((npc.velocity.Y > 0f && num21 > 0f) || (npc.velocity.Y < 0f && num21 < 0f)))
						{
							if (npc.velocity.X < num20)
								npc.velocity.X += turnSpeed * 1.3f;
							else if (npc.velocity.X > num20)
								npc.velocity.X -= turnSpeed * 1.3f;

							if (npc.velocity.Y < num21)
								npc.velocity.Y += turnSpeed * 1.3f;
							else if (npc.velocity.Y > num21)
								npc.velocity.Y -= turnSpeed * 1.3f;
						}

						if ((npc.velocity.X > 0f && num20 > 0f) || (npc.velocity.X < 0f && num20 < 0f) || (npc.velocity.Y > 0f && num21 > 0f) || (npc.velocity.Y < 0f && num21 < 0f))
						{
							if (npc.velocity.X < num20)
								npc.velocity.X += speed;
							else if (npc.velocity.X > num20)
								npc.velocity.X -= speed;
							if (npc.velocity.Y < num21)
								npc.velocity.Y += speed;
							else if (npc.velocity.Y > num21)
								npc.velocity.Y -= speed;

							if (Math.Abs(num21) < fallSpeed * maximumSpeed1 && ((npc.velocity.X > 0f && num20 < 0f) || (npc.velocity.X < 0f && num20 > 0f)))
							{
								if (npc.velocity.Y > 0f)
									npc.velocity.Y += speed * 2f;
								else
									npc.velocity.Y -= speed * 2f;
							}
							if (Math.Abs(num20) < fallSpeed * maximumSpeed1 && ((npc.velocity.Y > 0f && num21 < 0f) || (npc.velocity.Y < 0f && num21 > 0f)))
							{
								if (npc.velocity.X > 0f)
									npc.velocity.X += speed * 2f;
								else
									npc.velocity.X -= speed * 2f;
							}
						}
						else if (num25 > num26)
						{
							if (npc.velocity.X < num20)
								npc.velocity.X += speed * 1.1f;
							else if (npc.velocity.X > num20)
								npc.velocity.X -= speed * 1.1f;

							if ((Math.Abs(npc.velocity.X) + Math.Abs(npc.velocity.Y)) < fallSpeed * maximumSpeed2)
							{
								if (npc.velocity.Y > 0f)
									npc.velocity.Y += speed;
								else
									npc.velocity.Y -= speed;
							}
						}
						else
						{
							if (npc.velocity.Y < num21)
								npc.velocity.Y += speed * 1.1f;
							else if (npc.velocity.Y > num21)
								npc.velocity.Y -= speed * 1.1f;

							if ((Math.Abs(npc.velocity.X) + Math.Abs(npc.velocity.Y)) < fallSpeed * maximumSpeed2)
							{
								if (npc.velocity.X > 0f)
									npc.velocity.X += speed;
								else
									npc.velocity.X -= speed;
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
						npc.netUpdate = true;

					if (calamityGlobalNPC.newAI[2] > 900f)
					{
						npc.ai[3] = 0f;
						calamityGlobalNPC.newAI[2] = 0f;
						npc.TargetClosest();
						npc.netUpdate = true;
					}
				}
			}

			if (npc.life > Main.npc[(int)npc.ai[0]].life)
				npc.life = Main.npc[(int)npc.ai[0]].life;
		}

		private void SpawnTeleportLocation(Player player, bool phase2EnterPortal = true)
		{
			if (teleportTimer > -1 || player.dead || !player.active)
				return;

			teleportTimer = (CalamityWorld.death || CalamityWorld.malice) ? 120 : CalamityWorld.revenge ? 140 : Main.expertMode ? 160 : 180;
			if (Main.netMode != NetmodeID.MultiplayerClient)
			{
				int randomRange = 48;
				float distance = 640f;
				Vector2 targetVector = player.Center + player.velocity.SafeNormalize(Vector2.UnitX) * distance + new Vector2(Main.rand.Next(-randomRange, randomRange + 1), Main.rand.Next(-randomRange, randomRange + 1));
				Main.PlaySound(SoundID.Item109, player.Center);

				int portalType = phase2EnterPortal ? ModContent.ProjectileType<DoGP1EndPortal>() : ModContent.ProjectileType<DoGTeleportRift>();
				int fuck = Projectile.NewProjectile(targetVector, Vector2.Zero, portalType, 0, 0f, Main.myPlayer, npc.whoAmI);
				if (Main.projectile.IndexInRange(fuck) && phase2EnterPortal)
					Main.projectile[fuck].ai[1] = teleportTimer;
			}
		}

		private void Teleport(Player player, bool malice, bool death, bool revenge, bool expertMode, bool phase5)
		{
			Vector2 newPosition = GetRiftLocation(true);

			if (player.dead || !player.active || newPosition == default)
				return;

			if (phase5)
			{
				if (Main.netMode != NetmodeID.MultiplayerClient)
				{
					int type = ModContent.ProjectileType<DoGFire>();
					int damage = npc.GetProjectileDamage(type);
					float finalVelocity = 10f;
					int totalSpreads = revenge ? 6 : 3;
					float mult = revenge ? 1.5f : 3f;
					for (int i = 0; i < totalSpreads; i++)
					{
						int totalProjectiles = malice ? 18 : 12;
						float radians = MathHelper.TwoPi / totalProjectiles;
						float newVelocity = finalVelocity - i * mult;
						float velocityMult = 1f + ((finalVelocity - newVelocity) / (newVelocity * 2f) / 100f);
						double angleA = radians * 0.5;
						double angleB = MathHelper.ToRadians(90f) - angleA;
						float velocityX = (float)(newVelocity * Math.Sin(angleA) / Math.Sin(angleB));
						Vector2 spinningPoint = i < 3 ? new Vector2(0f, -newVelocity) : new Vector2(-velocityX, -newVelocity);
						float finalVelocityReduction = (float)Math.Pow(1.25, i) - 1f;
						for (int k = 0; k < totalProjectiles; k++)
						{
							Vector2 vector255 = spinningPoint.RotatedBy(radians * k);
							Projectile.NewProjectile(newPosition, vector255, type, damage, 0f, Main.myPlayer, velocityMult, finalVelocity - finalVelocityReduction);
						}
					}
				}
			}

			npc.TargetClosest();
			npc.position = newPosition;
			float chargeVelocity = malice ? 30f : death ? 26f : revenge ? 24f : expertMode ? 22f : 20f;
			float maxChargeDistance = 1600f;
			postTeleportTimer = (int)Math.Round(maxChargeDistance / chargeVelocity);
			npc.alpha = postTeleportTimer;
			npc.velocity = Vector2.Normalize(player.Center + player.velocity * 40f - npc.Center) * chargeVelocity;
			npc.netUpdate = true;

			for (int i = 0; i < Main.maxNPCs; i++)
			{
				if (Main.npc[i].active && (Main.npc[i].type == ModContent.NPCType<DevourerofGodsBody>() || Main.npc[i].type == ModContent.NPCType<DevourerofGodsTail>()))
				{
					Main.npc[i].position = newPosition;

					if (Main.npc[i].type == ModContent.NPCType<DevourerofGodsTail>())
						((DevourerofGodsTail)Main.npc[i].modNPC).setInvulTime(720);

					Main.npc[i].netUpdate = true;
				}
			}

			Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/DevourerAttack"), player.Center);
		}

		private Vector2 GetRiftLocation(bool spawnDust)
		{
			for (int i = 0; i < Main.maxProjectiles; i++)
			{
				if (Main.projectile[i].type == ModContent.ProjectileType<DoGTeleportRift>() || Main.projectile[i].type == ModContent.ProjectileType<DoGP1EndPortal>())
				{
					if (!spawnDust)
						Main.projectile[i].ai[0] = -1f;

					Main.projectile[i].Kill();
					return Main.projectile[i].Center;
				}
			}
			return default;
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			SpriteEffects spriteEffects = SpriteEffects.None;
			if (npc.spriteDirection == 1)
				spriteEffects = SpriteEffects.FlipHorizontally;

			bool useOtherTextures = phase2Started && CalamityWorld.DoGSecondStageCountdown <= 60;
			Texture2D texture2D15 = useOtherTextures ? ModContent.GetTexture("CalamityMod/NPCs/DevourerofGods/DevourerofGodsHeadS") : Main.npcTexture[npc.type];
			Vector2 vector11 = new Vector2(Main.npcTexture[npc.type].Width / 2, Main.npcTexture[npc.type].Height / 2);

			Vector2 vector43 = npc.Center - Main.screenPosition;
			vector43 -= new Vector2(texture2D15.Width, texture2D15.Height) * npc.scale / 2f;
			vector43 += vector11 * npc.scale + new Vector2(0f, npc.gfxOffY);
			spriteBatch.Draw(texture2D15, vector43, npc.frame, npc.GetAlpha(lightColor), npc.rotation, vector11, npc.scale, spriteEffects, 0f);

			if (!npc.dontTakeDamage)
			{
				texture2D15 = useOtherTextures ? ModContent.GetTexture("CalamityMod/NPCs/DevourerofGods/DevourerofGodsHeadSGlow") : ModContent.GetTexture("CalamityMod/NPCs/DevourerofGods/DevourerofGodsHeadGlow");
				Color color37 = Color.Lerp(Color.White, Color.Fuchsia, 0.5f);

				spriteBatch.Draw(texture2D15, vector43, npc.frame, color37, npc.rotation, vector11, npc.scale, spriteEffects, 0f);

				texture2D15 = useOtherTextures ? ModContent.GetTexture("CalamityMod/NPCs/DevourerofGods/DevourerofGodsHeadSGlow2") : ModContent.GetTexture("CalamityMod/NPCs/DevourerofGods/DevourerofGodsHeadGlow2");
				color37 = Color.Lerp(Color.White, Color.Cyan, 0.5f);

				spriteBatch.Draw(texture2D15, vector43, npc.frame, color37, npc.rotation, vector11, npc.scale, spriteEffects, 0f);
			}

			return false;
		}

		public override void BossLoot(ref string name, ref int potionType)
		{
			potionType = ModContent.ItemType<CosmiliteBrick>();
		}

		public override bool SpecialNPCLoot()
		{
			int closestSegmentID = DropHelper.FindClosestWormSegment(npc,
				ModContent.NPCType<DevourerofGodsHead>(),
				ModContent.NPCType<DevourerofGodsBody>(),
				ModContent.NPCType<DevourerofGodsTail>());
			npc.position = Main.npc[closestSegmentID].position;
			return false;
		}

		public override void NPCLoot()
		{
			// Stop the countdown -- if you kill DoG in less than 60 frames, this will stop another one from spawning.
			CalamityWorld.DoGSecondStageCountdown = 0;

			CalamityGlobalNPC.SetNewBossJustDowned(npc);

			DropHelper.DropBags(npc);

			// Legendary drops for DoG
			DropHelper.DropItemCondition(npc, ModContent.ItemType<CosmicDischarge>(), true, CalamityWorld.malice);
			DropHelper.DropItemCondition(npc, ModContent.ItemType<Norfleet>(), true, CalamityWorld.malice);
			DropHelper.DropItemCondition(npc, ModContent.ItemType<Skullmasher>(), true, CalamityWorld.malice);

			DropHelper.DropItem(npc, ModContent.ItemType<OmegaHealingPotion>(), 5, 15);
			DropHelper.DropItemChance(npc, ModContent.ItemType<DevourerofGodsTrophy>(), 10);
			DropHelper.DropItemCondition(npc, ModContent.ItemType<KnowledgeDevourerofGods>(), true, !CalamityWorld.downedDoG);

			CalamityGlobalTownNPC.SetNewShopVariable(new int[] { ModContent.NPCType<THIEF>() }, CalamityWorld.downedDoG);

			// Mount
			CalamityPlayer mp = Main.player[Player.FindClosest(npc.position, npc.width, npc.height)].Calamity();
			DropHelper.DropItemCondition(npc, ModContent.ItemType<Fabsol>(), true, mp.fabsolVodka);

			// All other drops are contained in the bag, so they only drop directly on Normal
			if (!Main.expertMode)
			{
				// Materials
				DropHelper.DropItem(npc, ModContent.ItemType<CosmiliteBar>(), 25, 35);
				DropHelper.DropItem(npc, ModContent.ItemType<CosmiliteBrick>(), 150, 250);

				// Weapons
				float w = DropHelper.NormalWeaponDropRateFloat;
				DropHelper.DropEntireWeightedSet(npc,
					DropHelper.WeightStack<Excelsus>(w),
					DropHelper.WeightStack<TheObliterator>(w),
					DropHelper.WeightStack<Deathwind>(w),
					DropHelper.WeightStack<DeathhailStaff>(w),
					DropHelper.WeightStack<StaffoftheMechworm>(w),
					DropHelper.WeightStack<Eradicator>(w)
				);

				// Vanity
				DropHelper.DropItemChance(npc, ModContent.ItemType<DevourerofGodsMask>(), 7);
				if (Main.rand.NextBool(5))
				{
					DropHelper.DropItem(npc, ModContent.ItemType<SilvaHelm>());
					DropHelper.DropItem(npc, ModContent.ItemType<SilvaHornedHelm>());
					DropHelper.DropItem(npc, ModContent.ItemType<SilvaMask>());
				}
			}

			// If DoG has not been killed yet, notify players that the holiday moons are buffed
			if (!CalamityWorld.downedDoG)
			{
				string key = "Mods.CalamityMod.DoGBossText";
				Color messageColor = Color.Cyan;
				string key2 = "Mods.CalamityMod.DoGBossText2";
				Color messageColor2 = Color.Orange;
				string key3 = "Mods.CalamityMod.DargonBossText";

				CalamityUtils.DisplayLocalizedText(key, messageColor);
				CalamityUtils.DisplayLocalizedText(key2, messageColor2);
				CalamityUtils.DisplayLocalizedText(key3, messageColor2);
			}

			// Mark DoG as dead
			CalamityWorld.downedDoG = true;
			CalamityNetcode.SyncWorld();
		}

		// Can only hit the target if within certain distance
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

			return minDist <= (phase2Started ? 80f : 55f) && (npc.alpha <= 0 || postTeleportTimer > 0) && preventBullshitHitsAtStartofFinalPhaseTimer <= 0;
		}

        public override bool StrikeNPC(ref double damage, int defense, ref float knockback, int hitDirection, ref bool crit)
        {
            if (CalamityUtils.AntiButcher(npc, ref damage, 0.5f))
            {
                string key = "Mods.CalamityMod.EdgyBossText2";
                Color messageColor = Color.Cyan;
                CalamityUtils.DisplayLocalizedText(key, messageColor);
                return false;
            }
            return true;
        }

        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
        {
            scale = 2f;
            return null;
        }

        public override bool CheckActive()
        {
            return false;
        }

		public override void HitEffect(int hitDirection, double damage)
		{
			if (npc.soundDelay == 0)
			{
				npc.soundDelay = 8;
				Main.PlaySound(mod.GetLegacySoundSlot(SoundType.NPCHit, "Sounds/NPCHit/OtherworldlyHit"), npc.Center);
			}
			if (npc.life <= 0)
			{
				Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/DoGS"), 1f);
				Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/DoGS2"), 1f);
				Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/DoGS5"), 1f);
				npc.position.X = npc.position.X + (npc.width / 2);
				npc.position.Y = npc.position.Y + (npc.height / 2);
				npc.width = 50;
				npc.height = 50;
				npc.position.X = npc.position.X - (npc.width / 2);
				npc.position.Y = npc.position.Y - (npc.height / 2);
				for (int num621 = 0; num621 < 15; num621++)
				{
					int num622 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, (int)CalamityDusts.PurpleCosmilite, 0f, 0f, 100, default, 2f);
					Main.dust[num622].velocity *= 3f;
					if (Main.rand.NextBool(2))
					{
						Main.dust[num622].scale = 0.5f;
						Main.dust[num622].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
					}
				}
				for (int num623 = 0; num623 < 30; num623++)
				{
					int num624 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, (int)CalamityDusts.PurpleCosmilite, 0f, 0f, 100, default, 3f);
					Main.dust[num624].noGravity = true;
					Main.dust[num624].velocity *= 5f;
					num624 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, (int)CalamityDusts.PurpleCosmilite, 0f, 0f, 100, default, 2f);
					Main.dust[num624].velocity *= 2f;
				}
			}
		}

		public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            npc.lifeMax = (int)(npc.lifeMax * 0.8f * bossLifeScale);
        }

		public override void OnHitPlayer(Player player, int damage, bool crit)
		{
			player.AddBuff(ModContent.BuffType<GodSlayerInferno>(), 300, true);
			player.AddBuff(ModContent.BuffType<WhisperingDeath>(), 420, true);
			/*if ((CalamityWorld.death || BossRushEvent.BossRushActive) && (npc.alpha <= 0 || postTeleportTimer > 0) && !player.Calamity().lol && preventBullshitHitsAtStartofFinalPhaseTimer <= 0)
            {
                player.KillMe(PlayerDeathReason.ByCustomReason(player.name + "'s essence was consumed by the devourer."), 1000.0, 0, false);
            }*/

			if (player.Calamity().dogTextCooldown <= 0)
			{
				string text = Utils.SelectRandom(Main.rand, new string[]
				{
					"Mods.CalamityMod.EdgyBossText3",
					"Mods.CalamityMod.EdgyBossText4",
					"Mods.CalamityMod.EdgyBossText5",
					"Mods.CalamityMod.EdgyBossText6",
					"Mods.CalamityMod.EdgyBossText7"
				});
				Color messageColor = Color.Cyan;
				Rectangle location = new Rectangle((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height);
				CombatText.NewText(location, messageColor, Language.GetTextValue(text), true);
				player.Calamity().dogTextCooldown = 60;
			}
		}
	}
}
