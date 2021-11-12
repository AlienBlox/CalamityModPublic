using CalamityMod.Events;
using CalamityMod.Items.Armor.Vanity;
using CalamityMod.Items.LoreItems;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Mounts;
using CalamityMod.Items.Placeables.Furniture.Trophies;
using CalamityMod.Items.Potions;
using CalamityMod.Items.TreasureBags;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.NPCs.ExoMechs.Thanatos;
using CalamityMod.Particles;
using CalamityMod.Projectiles.Boss;
using CalamityMod.Skies;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.ExoMechs.Ares
{
	[AutoloadBossHead]
	public class AresBody : ModNPC
	{
		// Used for loot
		public enum MechType
		{
			Ares = 0,
			Thanatos = 1,
			ArtemisAndApollo = 2
		}

		public enum Phase
		{
			Normal = 0,
			Deathrays = 1
		}

		public float AIState
		{
			get => npc.Calamity().newAI[0];
			set => npc.Calamity().newAI[0] = value;
		}

		public enum SecondaryPhase
		{
			Nothing = 0,
			Passive = 1,
			PassiveAndImmune = 2
		}

		public float SecondaryAIState
		{
			get => npc.Calamity().newAI[1];
			set => npc.Calamity().newAI[1] = value;
		}

		public enum Enraged
		{
			No = 0,
			Yes = 1
		}

		public float EnragedState
		{
			get => npc.localAI[1];
			set => npc.localAI[1] = value;
		}

		public ThanatosSmokeParticleSet SmokeDrawer = new ThanatosSmokeParticleSet(-1, 3, 0f, 16f, 1.5f);

		// Spawn rate for enrage steam
		public const int ventCloudSpawnRate = 3;

		// Number of frames on the X and Y axis
		private const int maxFramesX = 6;
		private const int maxFramesY = 8;

		// Counters for frames on the X and Y axis
		private int frameX = 0;
		private int frameY = 0;

		// Frame limit per animation, these are the specific frames where each animation ends
		private const int normalFrameLimit = 11;
		private const int firstStageDeathrayChargeFrameLimit = 23;
		private const int secondStageDeathrayChargeFrameLimit = 35;
		private const int finalStageDeathrayChargeFrameLimit = 47;

		// Default life ratio for the other mechs
		private const float defaultLifeRatio = 5f;

		// Variable used to stop the arm spawning loop
		private bool armsSpawned = false;

		// Total duration of the deathray telegraph
		public const float deathrayTelegraphDuration = 240f;

		// Total duration of the deathrays
		public const float deathrayDuration = 600f;

		// Max distance from the target before they are unable to hear sound telegraphs
		private const float soundDistance = 4800f;

		// Drawers for arm segments.
		public static PrimitiveTrail LightningDrawer;
		public static PrimitiveTrail LightningBackgroundDrawer;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("XF-09 Ares");
			NPCID.Sets.TrailingMode[npc.type] = 3;
			NPCID.Sets.TrailCacheLength[npc.type] = npc.oldPos.Length;
		}

		public override void SetDefaults()
		{
			npc.npcSlots = 5f;
			npc.damage = 100;
			npc.width = 220;
			npc.height = 252;
			npc.defense = 100;
			npc.DR_NERD(0.35f);
			npc.LifeMaxNERB(1300000, 1495000, 500000);
			double HPBoost = CalamityConfig.Instance.BossHealthBoost * 0.01;
			npc.lifeMax += (int)(npc.lifeMax * HPBoost);
			npc.aiStyle = -1;
			aiType = -1;
			npc.Opacity = 0f;
			npc.knockBackResist = 0f;
			npc.value = Item.buyPrice(3, 33, 0, 0);
			npc.noGravity = true;
			npc.noTileCollide = true;
			npc.HitSound = SoundID.NPCHit4;
			npc.DeathSound = SoundID.NPCDeath14;
			npc.netAlways = true;
			npc.boss = true;
			music = CalamityMod.Instance.GetMusicFromMusicMod("ExoMechs") ?? MusicID.Boss3;
			bossBag = ModContent.ItemType<DraedonTreasureBag>();
		}

		public override void BossHeadSlot(ref int index)
		{
			if (SecondaryAIState == (float)SecondaryPhase.PassiveAndImmune)
				index = -1;
		}

		public override void SendExtraAI(BinaryWriter writer)
		{
			writer.Write(frameX);
			writer.Write(frameY);
			writer.Write(armsSpawned);
			writer.Write(npc.dontTakeDamage);
			writer.Write(npc.localAI[0]);
			writer.Write(npc.localAI[1]);
			for (int i = 0; i < 4; i++)
				writer.Write(npc.Calamity().newAI[i]);
		}

		public override void ReceiveExtraAI(BinaryReader reader)
		{
			frameX = reader.ReadInt32();
			frameY = reader.ReadInt32();
			armsSpawned = reader.ReadBoolean();
			npc.dontTakeDamage = reader.ReadBoolean();
			npc.localAI[0] = reader.ReadSingle();
			npc.localAI[1] = reader.ReadSingle();
			for (int i = 0; i < 4; i++)
				npc.Calamity().newAI[i] = reader.ReadSingle();
		}

		public override void AI()
		{
			CalamityGlobalNPC calamityGlobalNPC = npc.Calamity();

			CalamityGlobalNPC.draedonExoMechPrime = npc.whoAmI;

			npc.frame = new Rectangle(npc.width * frameX, npc.height * frameY, npc.width, npc.height);

			// Difficulty modes
			bool malice = CalamityWorld.malice || BossRushEvent.BossRushActive;
			bool death = CalamityWorld.death || malice;
			bool revenge = CalamityWorld.revenge || malice;
			bool expertMode = Main.expertMode || malice;

			if (npc.ai[2] > 0f)
				npc.realLife = (int)npc.ai[2];

			// Spawn arms
			if (Main.netMode != NetmodeID.MultiplayerClient)
			{
				if (!armsSpawned && npc.ai[0] == 0f)
				{
					int totalArms = 4;
					int Previous = npc.whoAmI;
					for (int i = 0; i < totalArms; i++)
					{
						int lol = 0;
						switch (i)
						{
							case 0:
								lol = NPC.NewNPC((int)npc.position.X + (npc.width / 2), (int)npc.position.Y + (npc.height / 2), ModContent.NPCType<AresLaserCannon>(), npc.whoAmI);
								break;
							case 1:
								lol = NPC.NewNPC((int)npc.position.X + (npc.width / 2), (int)npc.position.Y + (npc.height / 2), ModContent.NPCType<AresPlasmaFlamethrower>(), npc.whoAmI);
								break;
							case 2:
								lol = NPC.NewNPC((int)npc.position.X + (npc.width / 2), (int)npc.position.Y + (npc.height / 2), ModContent.NPCType<AresTeslaCannon>(), npc.whoAmI);
								break;
							case 3:
								lol = NPC.NewNPC((int)npc.position.X + (npc.width / 2), (int)npc.position.Y + (npc.height / 2), ModContent.NPCType<AresGaussNuke>(), npc.whoAmI);
								break;
							default:
								break;
						}

						Main.npc[lol].realLife = npc.whoAmI;
						Main.npc[lol].ai[2] = npc.whoAmI;
						Main.npc[lol].ai[1] = Previous;
						Main.npc[Previous].ai[0] = lol;
						NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, lol, 0f, 0f, 0f, 0);
						Previous = lol;
					}
					armsSpawned = true;
				}
			}

			if (npc.life > Main.npc[(int)npc.ai[0]].life)
				npc.life = Main.npc[(int)npc.ai[0]].life;

			// Percent life remaining
			float lifeRatio = npc.life / (float)npc.lifeMax;

			// Check if the other exo mechs are alive
			int otherExoMechsAlive = 0;
			bool exoWormAlive = false;
			bool exoTwinsAlive = false;
			if (CalamityGlobalNPC.draedonExoMechWorm != -1)
			{
				if (Main.npc[CalamityGlobalNPC.draedonExoMechWorm].active)
				{
					otherExoMechsAlive++;
					exoWormAlive = true;
				}
			}

			// There is no point in checking for the other twin because they have linked HP
			if (CalamityGlobalNPC.draedonExoMechTwinGreen != -1)
			{
				if (Main.npc[CalamityGlobalNPC.draedonExoMechTwinGreen].active)
				{
					otherExoMechsAlive++;
					exoTwinsAlive = true;
				}
			}

			// These are 5 by default to avoid triggering passive phases after the other mechs are dead
			float exoWormLifeRatio = defaultLifeRatio;
			float exoTwinsLifeRatio = defaultLifeRatio;
			if (exoWormAlive)
				exoWormLifeRatio = Main.npc[CalamityGlobalNPC.draedonExoMechWorm].life / (float)Main.npc[CalamityGlobalNPC.draedonExoMechWorm].lifeMax;
			if (exoTwinsAlive)
				exoTwinsLifeRatio = Main.npc[CalamityGlobalNPC.draedonExoMechTwinGreen].life / (float)Main.npc[CalamityGlobalNPC.draedonExoMechTwinGreen].lifeMax;
			float totalOtherExoMechLifeRatio = exoWormLifeRatio + exoTwinsLifeRatio;

			// Check if any of the other mechs are passive
			bool exoWormPassive = false;
			bool exoTwinsPassive = false;
			if (exoWormAlive)
				exoWormPassive = Main.npc[CalamityGlobalNPC.draedonExoMechWorm].Calamity().newAI[1] == (float)ThanatosHead.SecondaryPhase.Passive;
			if (exoTwinsAlive)
				exoTwinsPassive = Main.npc[CalamityGlobalNPC.draedonExoMechTwinGreen].Calamity().newAI[1] == (float)Apollo.Apollo.SecondaryPhase.Passive;
			bool anyOtherExoMechPassive = exoWormPassive || exoTwinsPassive;

			// Check if any of the other mechs were spawned first
			bool exoWormWasFirst = false;
			bool exoTwinsWereFirst = false;
			if (exoWormAlive)
				exoWormWasFirst = Main.npc[CalamityGlobalNPC.draedonExoMechWorm].ai[3] == 1f;
			if (exoTwinsAlive)
				exoTwinsWereFirst = Main.npc[CalamityGlobalNPC.draedonExoMechTwinGreen].ai[3] == 1f;
			bool otherExoMechWasFirst = exoWormWasFirst || exoTwinsWereFirst;

			// Check for Draedon
			bool draedonAlive = false;
			if (CalamityGlobalNPC.draedon != -1)
			{
				if (Main.npc[CalamityGlobalNPC.draedon].active)
					draedonAlive = true;
			}

			// Prevent mechs from being respawned
			if (otherExoMechWasFirst)
			{
				if (npc.ai[3] < 1f)
					npc.ai[3] = 1f;
			}

			// Phases
			bool spawnOtherExoMechs = lifeRatio < 0.7f && npc.ai[3] == 0f;
			bool berserk = lifeRatio < 0.4f || (otherExoMechsAlive == 0 && lifeRatio < 0.7f);
			bool lastMechAlive = berserk && otherExoMechsAlive == 0;

			// If Ares doesn't go berserk
			bool otherMechIsBerserk = exoWormLifeRatio < 0.4f || exoTwinsLifeRatio < 0.4f;

			// Get a target
			if (npc.target < 0 || npc.target == Main.maxPlayers || Main.player[npc.target].dead || !Main.player[npc.target].active)
				npc.TargetClosest();

			// Despawn safety, make sure to target another player if the current player target is too far away
			if (Vector2.Distance(Main.player[npc.target].Center, npc.Center) > CalamityGlobalNPC.CatchUpDistance200Tiles)
				npc.TargetClosest();

			// Target variable
			Player player = Main.player[npc.target];

			// General AI pattern
			// 0 - Fly above target
			// 1 - Fly towards the target, slow down when close enough
			// 2 - Fire deathrays from telegraph locations to avoid cheap hits and rotate them around for 10 seconds while the plasma and tesla arms fire projectiles to make dodging difficult
			// 3 - Go passive and fly above the target while firing less projectiles
			// 4 - Go passive, immune and invisible; fly above the target and do nothing until next phase

			// Attack patterns
			// If spawned first
			// Phase 1 - 0
			// Phase 2 - 4
			// Phase 3 - 3

			// If berserk, this is the last phase of Ares
			// Phase 4 - 1, 2

			// If not berserk
			// Phase 4 - 4
			// Phase 5 - 0

			// If berserk, this is the last phase of Ares
			// Phase 6 - 1, 2

			// If not berserk
			// Phase 6 - 4

			// Berserk, final phase of Ares
			// Phase 7 - 1, 2

			// Adjust opacity
			bool invisiblePhase = SecondaryAIState == (float)SecondaryPhase.PassiveAndImmune;
			npc.dontTakeDamage = invisiblePhase;
			if (!invisiblePhase)
			{
				npc.Opacity += 0.2f;
				if (npc.Opacity > 1f)
					npc.Opacity = 1f;
			}
			else
			{
				npc.Opacity -= 0.05f;
				if (npc.Opacity < 0f)
					npc.Opacity = 0f;
			}

			// Rotation
			npc.rotation = npc.velocity.X * 0.003f;

			// Light and enrage check
			if (EnragedState == (float)Enraged.Yes)
			{
				Lighting.AddLight(npc.Center, 0.5f * npc.Opacity, 0f, 0f);
				npc.Calamity().CurrentlyEnraged = true;
			}
			else
			{
				float lightScale = 510f;
				Lighting.AddLight(npc.Center, Main.DiscoR / lightScale * npc.Opacity, Main.DiscoG / lightScale * npc.Opacity, Main.DiscoB / lightScale * npc.Opacity);
			}

			// Despawn if target is dead
			if (player.dead)
			{
				npc.TargetClosest(false);
				player = Main.player[npc.target];
				if (player.dead)
				{
					AIState = (float)Phase.Normal;
					calamityGlobalNPC.newAI[2] = 0f;
					calamityGlobalNPC.newAI[3] = 0f;
					npc.dontTakeDamage = true;

					npc.velocity.Y -= 1f;
					if ((double)npc.position.Y < Main.topWorld + 16f)
						npc.velocity.Y -= 1f;

					if ((double)npc.position.Y < Main.topWorld + 16f)
					{
						for (int a = 0; a < Main.maxNPCs; a++)
						{
							if (Main.npc[a].type == npc.type || Main.npc[a].type == ModContent.NPCType<Artemis.Artemis>() || Main.npc[a].type == ModContent.NPCType<Apollo.Apollo>() ||
								Main.npc[a].type == ModContent.NPCType<AresLaserCannon>() || Main.npc[a].type == ModContent.NPCType<AresPlasmaFlamethrower>() ||
								Main.npc[a].type == ModContent.NPCType<AresTeslaCannon>() || Main.npc[a].type == ModContent.NPCType<AresGaussNuke>() ||
								Main.npc[a].type == ModContent.NPCType<ThanatosHead>() || Main.npc[a].type == ModContent.NPCType<ThanatosBody1>() ||
								Main.npc[a].type == ModContent.NPCType<ThanatosBody2>() || Main.npc[a].type == ModContent.NPCType<ThanatosTail>())
								Main.npc[a].active = false;
						}
					}

					return;
				}
			}

			// Default vector to fly to
			Vector2 destination = SecondaryAIState == (float)SecondaryPhase.PassiveAndImmune ? new Vector2(player.Center.X, player.Center.Y - 800f) : AIState != (float)Phase.Deathrays ? new Vector2(player.Center.X, player.Center.Y - 425f) : player.Center;

			// Velocity and acceleration values
			float baseVelocityMult = (berserk ? 0.25f : 0f) + (malice ? 1.15f : death ? 1.1f : revenge ? 1.075f : expertMode ? 1.05f : 1f);
			float baseVelocity = (EnragedState == (float)Enraged.Yes ? 28f : 20f) * baseVelocityMult;
			float baseAcceleration = berserk ? 1.25f : 1f;
			float decelerationVelocityMult = 0.85f;
			Vector2 distanceFromDestination = destination - npc.Center;
			Vector2 desiredVelocity = Vector2.Normalize(distanceFromDestination) * baseVelocity;

			// Distance from target
			float distanceFromTarget = Vector2.Distance(npc.Center, player.Center);

			// Distance where Ares stops moving
			float movementDistanceGateValue = 50f;

			// Gate values
			float deathrayPhaseGateValue = lastMechAlive ? 630f : 900f;
			float deathrayDistanceGateValue = 480f;

			// Enter deathray phase again more quickly if enraged
			if (EnragedState == (float)Enraged.Yes)
				deathrayPhaseGateValue *= 0.5f;

			// Emit steam while enraged
			SmokeDrawer.ParticleSpawnRate = 9999999;
			if (EnragedState == (float)Enraged.Yes)
			{
				SmokeDrawer.ParticleSpawnRate = ventCloudSpawnRate;
				SmokeDrawer.BaseMoveRotation = npc.rotation + MathHelper.PiOver2;
				SmokeDrawer.SpawnAreaCompactness = 80f;

				// Increase DR during enrage
				npc.Calamity().DR = 0.85f;
			}
			else
				npc.Calamity().DR = 0.35f;

			SmokeDrawer.Update();

			// Passive and Immune phases
			switch ((int)SecondaryAIState)
			{
				case (int)SecondaryPhase.Nothing:

					// Spawn the other mechs if Ares is first
					if (otherExoMechsAlive == 0)
					{
						if (spawnOtherExoMechs)
						{
							// Reset everything
							if (npc.ai[3] < 1f)
								npc.ai[3] = 1f;

							SecondaryAIState = (float)SecondaryPhase.PassiveAndImmune;
							npc.TargetClosest();

							// Draedon text for the start of phase 2
							if (draedonAlive)
							{
								Main.npc[CalamityGlobalNPC.draedon].localAI[0] = 1f;
								Main.npc[CalamityGlobalNPC.draedon].ai[0] = Draedon.ExoMechPhaseDialogueTime;
							}

							if (Main.netMode != NetmodeID.MultiplayerClient)
							{
								// Spawn the fuckers
								NPC.SpawnOnPlayer(player.whoAmI, ModContent.NPCType<ThanatosHead>());
								NPC.SpawnOnPlayer(player.whoAmI, ModContent.NPCType<Artemis.Artemis>());
								NPC.SpawnOnPlayer(player.whoAmI, ModContent.NPCType<Apollo.Apollo>());
							}
						}
					}
					else
					{
						// If not spawned first, go to passive state if any other mech is passive or if Ares is under 70% life
						// Do not run this if berserk
						// Do not run this if any exo mech is dead
						if ((anyOtherExoMechPassive || lifeRatio < 0.7f) && !berserk && totalOtherExoMechLifeRatio < 5f)
						{
							// Tells Ares to return to the battle in passive state and reset everything
							SecondaryAIState = (float)SecondaryPhase.Passive;
							npc.TargetClosest();
						}

						// Go passive and immune if one of the other mechs is berserk
						// This is only called if two exo mechs are alive
						if (otherMechIsBerserk)
						{
							// Reset everything
							if (npc.ai[3] < 2f)
								npc.ai[3] = 2f;

							SecondaryAIState = (float)SecondaryPhase.PassiveAndImmune;
							npc.TargetClosest();

							// Phase 6, when 1 mech goes berserk and the other one leaves
							if (draedonAlive)
							{
								Main.npc[CalamityGlobalNPC.draedon].localAI[0] = 5f;
								Main.npc[CalamityGlobalNPC.draedon].ai[0] = Draedon.ExoMechPhaseDialogueTime;
							}
						}
					}

					break;

				// Fire projectiles less often
				case (int)SecondaryPhase.Passive:

					// Enter passive and invincible phase if one of the other exo mechs is berserk
					if (otherMechIsBerserk)
					{
						// Reset everything
						if (npc.ai[3] < 2f)
							npc.ai[3] = 2f;

						SecondaryAIState = (float)SecondaryPhase.PassiveAndImmune;
						npc.TargetClosest();
					}

					// If Ares is the first mech to go berserk
					if (berserk)
					{
						// Reset everything
						npc.TargetClosest();

						// Never be passive if berserk
						SecondaryAIState = (float)SecondaryPhase.Nothing;

						// Phase 4, when 1 mech goes berserk and the other 2 leave
						if (exoWormAlive && exoTwinsAlive)
						{
							if (draedonAlive)
							{
								Main.npc[CalamityGlobalNPC.draedon].localAI[0] = 3f;
								Main.npc[CalamityGlobalNPC.draedon].ai[0] = Draedon.ExoMechPhaseDialogueTime;
							}
						}
					}

					break;

				// Fly above target and become immune
				case (int)SecondaryPhase.PassiveAndImmune:

					// Enter the fight again if any of the other exo mechs is below 70% and the other mechs aren't berserk
					if ((exoWormLifeRatio < 0.7f || exoTwinsLifeRatio < 0.7f) && !otherMechIsBerserk)
					{
						// Tells Ares to return to the battle in passive state and reset everything
						// Return to normal phases if one or more mechs have been downed
						SecondaryAIState = totalOtherExoMechLifeRatio > 5f ? (float)SecondaryPhase.Nothing : (float)SecondaryPhase.Passive;
						npc.TargetClosest();

						// Phase 3, when all 3 mechs attack at the same time
						if (exoWormAlive && exoTwinsAlive)
						{
							if (draedonAlive)
							{
								Main.npc[CalamityGlobalNPC.draedon].localAI[0] = 2f;
								Main.npc[CalamityGlobalNPC.draedon].ai[0] = Draedon.ExoMechPhaseDialogueTime;
							}
						}
					}

					if (berserk)
					{
						// Reset everything
						npc.TargetClosest();

						// Never be passive if berserk
						SecondaryAIState = (float)SecondaryPhase.Nothing;
					}

					break;
			}

			// Attacking phases
			switch ((int)AIState)
			{
				// Fly above the target
				case (int)Phase.Normal:

					// Inverse lerp returns the percentage of progress between A and B
					float lerpValue = Utils.InverseLerp(movementDistanceGateValue, 2400f, distanceFromDestination.Length(), true);

					// Min velocity
					float minVelocity = distanceFromDestination.Length();
					float minVelocityCap = baseVelocity;
					if (minVelocity > minVelocityCap)
						minVelocity = minVelocityCap;

					// Max velocity
					Vector2 maxVelocity = distanceFromDestination / 24f;
					float maxVelocityCap = minVelocityCap * 3f;
					if (maxVelocity.Length() > maxVelocityCap)
						maxVelocity = distanceFromDestination.SafeNormalize(Vector2.Zero) * maxVelocityCap;

					npc.velocity = Vector2.Lerp(distanceFromDestination.SafeNormalize(Vector2.Zero) * minVelocity, maxVelocity, lerpValue);

					if (berserk)
					{
						calamityGlobalNPC.newAI[2] += 1f;
						if (calamityGlobalNPC.newAI[2] > deathrayPhaseGateValue)
						{
							calamityGlobalNPC.newAI[2] = 0f;
							AIState = (float)Phase.Deathrays;

							// Cancel enrage state if Ares is enraged
							if (EnragedState == (float)Enraged.Yes)
								EnragedState = (float)Enraged.No;
						}
					}

					break;

				// Move close to target, reduce velocity when close enough, create telegraph beams, fire deathrays
				case (int)Phase.Deathrays:

					if (distanceFromTarget > deathrayDistanceGateValue && calamityGlobalNPC.newAI[3] == 0f)
					{
						Vector2 desiredVelocity2 = Vector2.Normalize(distanceFromDestination) * baseVelocity;
						npc.SimpleFlyMovement(desiredVelocity2, baseAcceleration);
					}
					else
					{
						// Enrage if the target is more than the deathray length away
						if (distanceFromTarget > 3600f && EnragedState == (float)Enraged.No)
						{
							// Play enrage sound
							if (Main.player[Main.myPlayer].active && !Main.player[Main.myPlayer].dead && Vector2.Distance(Main.player[Main.myPlayer].Center, npc.Center) < soundDistance)
							{
								Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/AresEnraged"),
									(int)Main.player[Main.myPlayer].position.X, (int)Main.player[Main.myPlayer].position.Y);
							}

							// Draedon comments on how foolish it is to run
							if (Main.netMode != NetmodeID.MultiplayerClient)
								CalamityUtils.DisplayLocalizedText("Mods.CalamityMod.DraedonAresEnrageText", new Color(155, 255, 255));

							// Enrage
							EnragedState = (float)Enraged.Yes;
						}

						calamityGlobalNPC.newAI[3] = 1f;
						npc.velocity *= decelerationVelocityMult;

						int totalProjectiles = malice ? 12 : death ? 10 : revenge ? 9 : expertMode ? 8 : 6;
						float radians = MathHelper.TwoPi / totalProjectiles;
						Vector2 laserSpawnPoint = new Vector2(npc.Center.X, npc.Center.Y);
						bool normalLaserRotation = npc.localAI[0] % 2f == 0f;
						float velocity = 6f;
						double angleA = radians * 0.5;
						double angleB = MathHelper.ToRadians(90f) - angleA;
						float velocityX2 = (float)(velocity * Math.Sin(angleA) / Math.Sin(angleB));
						Vector2 spinningPoint = normalLaserRotation ? new Vector2(0f, -velocity) : new Vector2(-velocityX2, -velocity);
						spinningPoint.Normalize();

						calamityGlobalNPC.newAI[2] += (EnragedState == (float)Enraged.Yes && calamityGlobalNPC.newAI[2] % 2f == 0f) ? 2f : 1f;
						if (calamityGlobalNPC.newAI[2] < deathrayTelegraphDuration)
						{
							// Fire deathray telegraph beams
							if (calamityGlobalNPC.newAI[2] == 1f)
							{
								// Set frames to deathray charge up frames, which begin on frame 12
								// Reset the frame counter
								npc.frameCounter = 0D;

								// X = 1 sets to frame 8
								frameX = 1;

								// Y = 4 sets to frame 12
								frameY = 4;

								// Create a bunch of lightning bolts in the sky
								ExoMechsSky.CreateLightningBolt(12);

								if (Main.netMode != NetmodeID.MultiplayerClient)
								{
									Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Item, "Sounds/Item/LaserCannon"), npc.Center);
									int type = ModContent.ProjectileType<AresDeathBeamTelegraph>();
									Vector2 spawnPoint = npc.Center + new Vector2(-1f, 23f);
									for (int k = 0; k < totalProjectiles; k++)
									{
										Vector2 laserVelocity = spinningPoint.RotatedBy(radians * k);
										Projectile.NewProjectile(spawnPoint + Vector2.Normalize(laserVelocity) * 17f, laserVelocity, type, 0, 0f, Main.myPlayer, 0f, npc.whoAmI);
									}
								}
							}
						}
						else
						{
							// Fire deathrays
							if (calamityGlobalNPC.newAI[2] == deathrayTelegraphDuration)
							{
								if (Main.netMode != NetmodeID.MultiplayerClient)
								{
									Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Item, "Sounds/Item/TeslaCannonFire"), npc.Center);
									int type = ModContent.ProjectileType<AresDeathBeamStart>();
									int damage = npc.GetProjectileDamage(type);
									Vector2 spawnPoint = npc.Center + new Vector2(-1f, 23f);
									for (int k = 0; k < totalProjectiles; k++)
									{
										Vector2 laserVelocity = spinningPoint.RotatedBy(radians * k);
										Projectile.NewProjectile(spawnPoint + Vector2.Normalize(laserVelocity) * 35f, laserVelocity, type, damage, 0f, Main.myPlayer, 0f, npc.whoAmI);
									}
								}
							}
						}

						if (calamityGlobalNPC.newAI[2] >= deathrayTelegraphDuration + deathrayDuration)
						{
							AIState = (float)Phase.Normal;
							calamityGlobalNPC.newAI[2] = 0f;
							calamityGlobalNPC.newAI[3] = 0f;

							/* Normal positions: Laser = 0, Tesla = 1, Plasma = 2, Gauss = 3
							 * 0 = Laser = 0, Tesla = 1, Plasma = 2, Gauss = 3
							 * 1 = Laser = 3, Tesla = 1, Plasma = 2, Gauss = 0
							 * 2 = Laser = 3, Tesla = 2, Plasma = 1, Gauss = 0
							 * 3 = Laser = 0, Tesla = 2, Plasma = 1, Gauss = 3
							 * 4 = Laser = 0, Tesla = 1, Plasma = 2, Gauss = 3
							 * 5 = Laser = 3, Tesla = 1, Plasma = 2, Gauss = 0
							 */
							if (revenge)
							{
								npc.ai[3] += 1f + Main.rand.Next(2);
								if (npc.ai[3] > 5f)
									npc.ai[3] -= 4f;
							}
							else if (expertMode)
							{
								npc.ai[3] += Main.rand.Next(2);
								if (npc.ai[3] > 3f)
									npc.ai[3] -= 2f;
							}

							npc.localAI[0] += 1f;
							npc.TargetClosest();
							npc.netUpdate = true;
						}
					}

					break;
			}
		}

		public override bool CanHitPlayer(Player target, ref int cooldownSlot) => false;

		public override bool StrikeNPC(ref double damage, int defense, ref float knockback, int hitDirection, ref bool crit) => !CalamityUtils.AntiButcher(npc, ref damage, 0.5f);

		public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
		{
			scale = 2f;
			return null;
		}

		public override void FindFrame(int frameHeight)
		{
			// Use telegraph frames when using deathrays
			npc.frameCounter += 1D;
			if (AIState == (float)Phase.Normal || npc.Calamity().newAI[3] == 0f)
			{
				if (npc.frameCounter >= 6D)
				{
					// Reset frame counter
					npc.frameCounter = 0D;

					// Increment the Y frame
					frameY++;

					// Reset the Y frame if greater than 8
					if (frameY == maxFramesY)
					{
						frameX++;
						frameY = 0;
					}

					// Reset the frames to frame 0
					if ((frameX * maxFramesY) + frameY > normalFrameLimit)
						frameX = frameY = 0;
				}
			}
			else
			{
				if (npc.frameCounter >= 6D)
				{
					// Reset frame counter
					npc.frameCounter = 0D;

					// Increment the Y frame
					frameY++;

					// Reset the Y frame if greater than 8
					if (frameY == maxFramesY)
					{
						frameX++;
						frameY = 0;
					}

					// Reset the frames to frame 36, the start of the deathray firing animation loop
					if ((frameX * maxFramesY) + frameY > finalStageDeathrayChargeFrameLimit)
						frameX = frameY = 4;
				}
			}
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Color drawColor)
		{
			// Draw the enrage smoke behind Ares
			SmokeDrawer.DrawSet(npc.Center);

			// Draw arms.
			int laserArm = NPC.FindFirstNPC(ModContent.NPCType<AresLaserCannon>());
			int gaussArm = NPC.FindFirstNPC(ModContent.NPCType<AresGaussNuke>());
			int teslaArm = NPC.FindFirstNPC(ModContent.NPCType<AresTeslaCannon>());
			int plasmaArm = NPC.FindFirstNPC(ModContent.NPCType<AresPlasmaFlamethrower>());
			Color afterimageBaseColor = EnragedState == (float)Enraged.Yes ? Color.Red : Color.White;
			Color armGlowmaskColor = afterimageBaseColor;
			armGlowmaskColor.A = 184;

			(int, bool)[] armProperties = new (int, bool)[]
			{
				// Laser arm.
				(-1, true),

				// Gauss arm.
				(1, true),

				// Telsa arm.
				(-1, false),

				// Plasma arm.
				(1, false),
			};

			// Swap out arm positions as necessary.
			// Normal Position: Laser, Tesla, Plasma, Laser
			switch ((int)npc.ai[3])
			{
				case 0:
					if (AIState == (int)Phase.Deathrays)
					{
						CalamityUtils.SwapArrayIndices(ref armProperties, 1, 3);
						CalamityUtils.SwapArrayIndices(ref armProperties, 0, 1);
					}
					break;
				case 1:
					CalamityUtils.SwapArrayIndices(ref armProperties, 0, 1);
					if (AIState == (int)Phase.Deathrays)
						CalamityUtils.SwapArrayIndices(ref armProperties, 0, 3);
					break;
				case 2:
					if (AIState != (int)Phase.Deathrays)
					{
						CalamityUtils.SwapArrayIndices(ref armProperties, 0, 1);
						CalamityUtils.SwapArrayIndices(ref armProperties, 2, 3);
					}
					else
					{
						CalamityUtils.SwapArrayIndices(ref armProperties, 0, 1);
						CalamityUtils.SwapArrayIndices(ref armProperties, 2, 3);
						CalamityUtils.SwapArrayIndices(ref armProperties, 0, 2);
					}
					break;
				case 3:
					CalamityUtils.SwapArrayIndices(ref armProperties, 2, 3);
					break;
				case 4:
					CalamityUtils.SwapArrayIndices(ref armProperties, 1, 3);
					break;
				case 5:
					if (AIState != (int)Phase.Deathrays)
						CalamityUtils.SwapArrayIndices(ref armProperties, 0, 1);
					else
					{
						CalamityUtils.SwapArrayIndices(ref armProperties, 0, 3);
						CalamityUtils.SwapArrayIndices(ref armProperties, 1, 3);
					}
					break;
			}

			if (laserArm != -1)
				DrawArm(spriteBatch, Main.npc[laserArm].Center, armGlowmaskColor, armProperties[0].Item1, armProperties[0].Item2);
			if (gaussArm != -1)
				DrawArm(spriteBatch, Main.npc[gaussArm].Center, armGlowmaskColor, armProperties[1].Item1, armProperties[1].Item2);
			if (teslaArm != -1)
				DrawArm(spriteBatch, Main.npc[teslaArm].Center, armGlowmaskColor, armProperties[2].Item1, armProperties[2].Item2);
			if (plasmaArm != -1)
				DrawArm(spriteBatch, Main.npc[plasmaArm].Center, armGlowmaskColor, armProperties[3].Item1, armProperties[3].Item2);

			Texture2D texture = Main.npcTexture[npc.type];
			Rectangle frame = new Rectangle(npc.width * frameX, npc.height * frameY, npc.width, npc.height);
			Vector2 vector = new Vector2(npc.width / 2, npc.height / 2);
			int numAfterimages = 5;

			if (CalamityConfig.Instance.Afterimages)
			{
				for (int i = 1; i < numAfterimages; i += 2)
				{
					Color afterimageColor = drawColor;
					afterimageColor = Color.Lerp(afterimageColor, afterimageBaseColor, 0.5f);
					afterimageColor = npc.GetAlpha(afterimageColor);
					afterimageColor *= (numAfterimages - i) / 15f;
					Vector2 afterimageCenter = npc.oldPos[i] + new Vector2(npc.width, npc.height) / 2f - Main.screenPosition;
					afterimageCenter -= new Vector2(texture.Width, texture.Height) / new Vector2(maxFramesX, maxFramesY) * npc.scale / 2f;
					afterimageCenter += vector * npc.scale + new Vector2(0f, npc.gfxOffY);
					spriteBatch.Draw(texture, afterimageCenter, npc.frame, afterimageColor, npc.oldRot[i], vector, npc.scale, SpriteEffects.None, 0f);
				}
			}

			Vector2 center = npc.Center - Main.screenPosition;
			spriteBatch.Draw(texture, center, frame, npc.GetAlpha(drawColor), npc.rotation, vector, npc.scale, SpriteEffects.None, 0f);

			texture = ModContent.GetTexture("CalamityMod/NPCs/ExoMechs/Ares/AresBodyGlow");

			if (CalamityConfig.Instance.Afterimages)
			{
				for (int i = 1; i < numAfterimages; i += 2)
				{
					Color afterimageColor = drawColor;
					afterimageColor = Color.Lerp(afterimageColor, afterimageBaseColor, 0.5f);
					afterimageColor = npc.GetAlpha(afterimageColor);
					afterimageColor *= (numAfterimages - i) / 15f;
					Vector2 afterimageCenter = npc.oldPos[i] + new Vector2(npc.width, npc.height) / 2f - Main.screenPosition;
					afterimageCenter -= new Vector2(texture.Width, texture.Height) / new Vector2(maxFramesX, maxFramesY) * npc.scale / 2f;
					afterimageCenter += vector * npc.scale + new Vector2(0f, npc.gfxOffY);
					spriteBatch.Draw(texture, afterimageCenter, npc.frame, afterimageColor, npc.oldRot[i], vector, npc.scale, SpriteEffects.None, 0f);
				}
			}

			spriteBatch.Draw(texture, center, frame, afterimageBaseColor * npc.Opacity, npc.rotation, vector, npc.scale, SpriteEffects.None, 0f);

			return false;
		}
		internal float WidthFunction(float completionRatio)
		{
			return MathHelper.Lerp(0.5f, 1.3f, (float)Math.Sin(MathHelper.Pi * completionRatio)) * npc.scale;
		}

		internal Color ColorFunction(float completionRatio)
		{
			Color baseColor1 = EnragedState == (float)Enraged.Yes ? Color.Red : Color.Cyan;
			Color baseColor2 = EnragedState == (float)Enraged.Yes ? Color.IndianRed : Color.Cyan;

			float fadeToWhite = MathHelper.Lerp(0f, 0.65f, (float)Math.Sin(MathHelper.TwoPi * completionRatio + Main.GlobalTime * 4f) * 0.5f + 0.5f);
			Color baseColor = Color.Lerp(baseColor1, Color.White, fadeToWhite);
			Color color = Color.Lerp(baseColor, baseColor2, ((float)Math.Sin(MathHelper.Pi * completionRatio + Main.GlobalTime * 4f) * 0.5f + 0.5f) * 0.8f) * 0.65f;
			color.A = 84;
			if (npc.Opacity <= 0f)
				return Color.Transparent;
			return color;
		}

		internal float BackgroundWidthFunction(float completionRatio) => WidthFunction(completionRatio) * 4f;

		public Color BackgroundColorFunction(float completionRatio)
		{
			Color backgroundColor = EnragedState == (float)Enraged.Yes ? Color.Crimson : Color.CornflowerBlue;
			Color color = backgroundColor * npc.Opacity * 0.4f;
			return color;
		}

		public void DrawArm(SpriteBatch spriteBatch, Vector2 handPosition, Color glowmaskColor, int direction, bool backArm)
		{
			if (LightningDrawer is null)
				LightningDrawer = new PrimitiveTrail(WidthFunction, ColorFunction, PrimitiveTrail.RigidPointRetreivalFunction);
			if (LightningBackgroundDrawer is null)
				LightningBackgroundDrawer = new PrimitiveTrail(BackgroundWidthFunction, BackgroundColorFunction, PrimitiveTrail.RigidPointRetreivalFunction);

			SpriteEffects spriteDirection = direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
			float distanceFromHand = npc.Distance(handPosition);

			// Draw back arms.
			if (backArm)
			{
				Texture2D shoulderTexture = ModContent.GetTexture("CalamityMod/ExtraTextures/AresArmTopShoulder");
				Texture2D armTexture1 = ModContent.GetTexture("CalamityMod/ExtraTextures/AresArmTopPart1");
				Texture2D armSegmentTexture = ModContent.GetTexture("CalamityMod/ExtraTextures/AresArmTopSegment");
				Texture2D armTexture2 = ModContent.GetTexture("CalamityMod/ExtraTextures/AresArmTopPart2");

				Texture2D shoulderGlowmask = ModContent.GetTexture("CalamityMod/ExtraTextures/AresArmTopShoulderGlow");
				Texture2D armSegmentGlowmask = ModContent.GetTexture("CalamityMod/ExtraTextures/AresArmTopSegmentGlow");
				Texture2D armGlowmask2 = ModContent.GetTexture("CalamityMod/ExtraTextures/AresArmTopPart2Glow");

				Vector2 shoulderDrawPosition = npc.Center + new Vector2(direction * 176f, -100f);
				Vector2 arm1DrawPosition = shoulderDrawPosition + new Vector2(direction * (shoulderTexture.Width + 16f), 10f);
				Vector2 armSegmentDrawPosition = arm1DrawPosition;

				Vector2 arm1Origin = armTexture1.Size() * new Vector2((direction == 1).ToInt(), 0.5f);
				Vector2 arm2Origin = armTexture2.Size() * new Vector2((direction == 1).ToInt(), 0.5f);

				float arm1Rotation = MathHelper.Clamp(distanceFromHand * direction / 1200f, -0.12f, 0.12f);
				float arm2Rotation = (handPosition - armSegmentDrawPosition).ToRotation();
				if (direction == 1)
					arm2Rotation += MathHelper.Pi;
				float armSegmentRotation = arm2Rotation;

				// Handle offsets for points.
				armSegmentDrawPosition += arm1Rotation.ToRotationVector2() * direction * -14f;
				armSegmentDrawPosition -= arm2Rotation.ToRotationVector2() * direction * 20f;
				Vector2 arm2DrawPosition = armSegmentDrawPosition;
				arm2DrawPosition -= arm2Rotation.ToRotationVector2() * direction * 40f;
				arm2DrawPosition += (arm2Rotation - MathHelper.PiOver2).ToRotationVector2() * 14f;

				// Calculate colors.
				Color shoulderLightColor = npc.GetAlpha(Lighting.GetColor((int)shoulderDrawPosition.X / 16, (int)shoulderDrawPosition.Y / 16));
				Color arm1LightColor = npc.GetAlpha(Lighting.GetColor((int)arm1DrawPosition.X / 16, (int)arm1DrawPosition.Y / 16));
				Color armSegmentLightColor = npc.GetAlpha(Lighting.GetColor((int)armSegmentDrawPosition.X / 16, (int)armSegmentDrawPosition.Y / 16));
				Color arm2LightColor = npc.GetAlpha(Lighting.GetColor((int)arm2DrawPosition.X / 16, (int)arm2DrawPosition.Y / 16));
				Color glowmaskAlphaColor = npc.GetAlpha(glowmaskColor);

				// Draw electricity between arms.
				if (npc.Opacity > 0f)
				{
					List<Vector2> arm2ElectricArcPoints = AresTeslaOrb.DetermineElectricArcPoints(armSegmentDrawPosition, arm2DrawPosition + arm2Rotation.ToRotationVector2() * -direction * 20f, 250290787);
					LightningBackgroundDrawer.Draw(arm2ElectricArcPoints, -Main.screenPosition, 90);
					LightningDrawer.Draw(arm2ElectricArcPoints, -Main.screenPosition, 90);

					// Draw electricity between the final arm and the hand.
					List<Vector2> handElectricArcPoints = AresTeslaOrb.DetermineElectricArcPoints(arm2DrawPosition - arm2Rotation.ToRotationVector2() * direction * 100f, handPosition, 27182);
					LightningBackgroundDrawer.Draw(handElectricArcPoints, -Main.screenPosition, 90);
					LightningDrawer.Draw(handElectricArcPoints, -Main.screenPosition, 90);
				}

				shoulderDrawPosition += Vector2.UnitY * npc.gfxOffY - Main.screenPosition;
				arm1DrawPosition += Vector2.UnitY * npc.gfxOffY - Main.screenPosition;
				armSegmentDrawPosition += Vector2.UnitY * npc.gfxOffY - Main.screenPosition;
				arm2DrawPosition += Vector2.UnitY * npc.gfxOffY - Main.screenPosition;

				spriteBatch.Draw(armTexture1, arm1DrawPosition, null, arm1LightColor, arm1Rotation, arm1Origin, npc.scale, spriteDirection ^ SpriteEffects.FlipHorizontally, 0f);
				spriteBatch.Draw(shoulderTexture, shoulderDrawPosition, null, shoulderLightColor, 0f, shoulderTexture.Size() * 0.5f, npc.scale, spriteDirection, 0f);
				spriteBatch.Draw(shoulderGlowmask, shoulderDrawPosition, null, glowmaskAlphaColor, 0f, shoulderTexture.Size() * 0.5f, npc.scale, spriteDirection, 0f);
				spriteBatch.Draw(armSegmentTexture, armSegmentDrawPosition, null, armSegmentLightColor, armSegmentRotation, armSegmentTexture.Size() * 0.5f, npc.scale, spriteDirection ^ SpriteEffects.FlipHorizontally, 0f);
				spriteBatch.Draw(armSegmentGlowmask, armSegmentDrawPosition, null, glowmaskAlphaColor, armSegmentRotation, armSegmentTexture.Size() * 0.5f, npc.scale, spriteDirection ^ SpriteEffects.FlipHorizontally, 0f);
				spriteBatch.Draw(armTexture2, arm2DrawPosition, null, arm2LightColor, arm2Rotation, arm2Origin, npc.scale, spriteDirection ^ SpriteEffects.FlipVertically, 0f);
				spriteBatch.Draw(armGlowmask2, arm2DrawPosition, null, glowmaskAlphaColor, arm2Rotation, arm2Origin, npc.scale, spriteDirection ^ SpriteEffects.FlipVertically, 0f);
			}
			else
			{
				Texture2D shoulderTexture = ModContent.GetTexture("CalamityMod/ExtraTextures/AresBottomArmShoulder");
				Texture2D armTexture1 = ModContent.GetTexture("CalamityMod/ExtraTextures/AresBottomArmPart1");
				Texture2D armTexture2 = ModContent.GetTexture("CalamityMod/ExtraTextures/AresBottomArmPart2");

				Texture2D shoulderGlowmask = ModContent.GetTexture("CalamityMod/ExtraTextures/AresBottomArmShoulderGlow");
				Texture2D armTexture1Glowmask = ModContent.GetTexture("CalamityMod/ExtraTextures/AresBottomArmPart1Glow");
				Texture2D armTexture2Glowmask = ModContent.GetTexture("CalamityMod/ExtraTextures/AresBottomArmPart2Glow");

				Vector2 shoulderDrawPosition = npc.Center + new Vector2(direction * 110f, -30f);
				Vector2 arm1DrawPosition = shoulderDrawPosition;

				Vector2 arm1Origin = armTexture1.Size() * new Vector2((direction == 1).ToInt(), 0.5f);
				Vector2 arm2Origin = armTexture2.Size() * new Vector2((direction == 1).ToInt(), 0.5f);

				float arm1Rotation = CalamityUtils.WrapAngle90Degrees((handPosition - shoulderDrawPosition).ToRotation()) * 0.5f;
				arm1DrawPosition += arm1Rotation.ToRotationVector2() * direction * (armTexture1.Width - 14f);
				float arm2Rotation = CalamityUtils.WrapAngle90Degrees((handPosition - arm1DrawPosition).ToRotation());

				Vector2 arm2DrawPosition = arm1DrawPosition + arm2Rotation.ToRotationVector2() * direction * (armTexture2.Width + 16f) - Vector2.UnitY * 16f;

				Color shoulderLightColor = npc.GetAlpha(Lighting.GetColor((int)shoulderDrawPosition.X / 16, (int)shoulderDrawPosition.Y / 16));
				Color arm1LightColor = npc.GetAlpha(Lighting.GetColor((int)arm1DrawPosition.X / 16, (int)arm1DrawPosition.Y / 16));
				Color arm2LightColor = npc.GetAlpha(Lighting.GetColor((int)arm2DrawPosition.X / 16, (int)arm2DrawPosition.Y / 16));
				Color glowmaskAlphaColor = npc.GetAlpha(glowmaskColor);

				// Draw electricity between arms.
				if (npc.Opacity > 0f)
				{
					List<Vector2> arm2ElectricArcPoints = AresTeslaOrb.DetermineElectricArcPoints(arm1DrawPosition - arm2Rotation.ToRotationVector2() * 10f, arm1DrawPosition + arm2Rotation.ToRotationVector2() * 20f, 31416);
					LightningBackgroundDrawer.Draw(arm2ElectricArcPoints, -Main.screenPosition, 90);
					LightningDrawer.Draw(arm2ElectricArcPoints, -Main.screenPosition, 90);

					// Draw electricity between the final arm and the hand.
					List<Vector2> handElectricArcPoints = AresTeslaOrb.DetermineElectricArcPoints(arm2DrawPosition - arm2Rotation.ToRotationVector2() * 20f, handPosition, 27182);
					LightningBackgroundDrawer.Draw(handElectricArcPoints, -Main.screenPosition, 90);
					LightningDrawer.Draw(handElectricArcPoints, -Main.screenPosition, 90);
				}

				shoulderDrawPosition += Vector2.UnitY * npc.gfxOffY - Main.screenPosition;
				arm1DrawPosition += Vector2.UnitY * npc.gfxOffY - Main.screenPosition;
				arm2DrawPosition += Vector2.UnitY * npc.gfxOffY - Main.screenPosition;

				spriteBatch.Draw(shoulderTexture, shoulderDrawPosition, null, shoulderLightColor, arm1Rotation, shoulderTexture.Size() * 0.5f, npc.scale, spriteDirection ^ SpriteEffects.FlipHorizontally, 0f);
				spriteBatch.Draw(shoulderGlowmask, shoulderDrawPosition, null, glowmaskAlphaColor, arm1Rotation, shoulderTexture.Size() * 0.5f, npc.scale, spriteDirection ^ SpriteEffects.FlipHorizontally, 0f);
				spriteBatch.Draw(armTexture1, arm1DrawPosition, null, arm1LightColor, arm1Rotation, arm1Origin, npc.scale, spriteDirection ^ SpriteEffects.FlipHorizontally, 0f);
				spriteBatch.Draw(armTexture1Glowmask, arm1DrawPosition, null, glowmaskAlphaColor, arm1Rotation, arm1Origin, npc.scale, spriteDirection ^ SpriteEffects.FlipHorizontally, 0f);
				spriteBatch.Draw(armTexture2, arm2DrawPosition, null, arm2LightColor, arm2Rotation, arm2Origin, npc.scale, spriteDirection ^ SpriteEffects.FlipHorizontally, 0f);
				spriteBatch.Draw(armTexture2Glowmask, arm2DrawPosition, null, glowmaskAlphaColor, arm2Rotation, arm2Origin, npc.scale, spriteDirection ^ SpriteEffects.FlipHorizontally, 0f);
			}
		}

		public override void BossLoot(ref string name, ref int potionType)
		{
			potionType = ModContent.ItemType<OmegaHealingPotion>();
		}

		public override void NPCLoot()
		{
			// Check if the other exo mechs are alive
			bool exoWormAlive = false;
			bool exoTwinsAlive = false;
			if (CalamityGlobalNPC.draedonExoMechWorm != -1)
			{
				if (Main.npc[CalamityGlobalNPC.draedonExoMechWorm].active)
					exoWormAlive = true;
			}
			if (CalamityGlobalNPC.draedonExoMechTwinGreen != -1)
			{
				if (Main.npc[CalamityGlobalNPC.draedonExoMechTwinGreen].active)
					exoTwinsAlive = true;
			}

			// Check for Draedon
			bool draedonAlive = false;
			if (CalamityGlobalNPC.draedon != -1)
			{
				if (Main.npc[CalamityGlobalNPC.draedon].active)
					draedonAlive = true;
			}

			// Phase 5, when 1 mech dies and the other 2 return to fight
			if (exoWormAlive && exoTwinsAlive)
			{
				if (draedonAlive)
				{
					Main.npc[CalamityGlobalNPC.draedon].localAI[0] = 4f;
					Main.npc[CalamityGlobalNPC.draedon].ai[0] = Draedon.ExoMechPhaseDialogueTime;
				}
			}

			// Phase 7, when 1 mech dies and the final one returns to the fight
			else if (exoWormAlive || exoTwinsAlive)
			{
				if (draedonAlive)
				{
					Main.npc[CalamityGlobalNPC.draedon].localAI[0] = 6f;
					Main.npc[CalamityGlobalNPC.draedon].ai[0] = Draedon.ExoMechPhaseDialogueTime;
				}
			}

			// Mark Exo Mechs as dead and drop loot
			else
				DropExoMechLoot(npc, (int)MechType.Ares);
		}

		public static void DropExoMechLoot(NPC npc, int mechType)
		{
			// Dropped before the downed variable is set to true
			DropHelper.DropItemCondition(npc, ModContent.ItemType<KnowledgeExoMechs>(), true, !CalamityWorld.downedExoMechs);

			switch (mechType)
			{
				case (int)MechType.Ares:

					DropHelper.DropItem(npc, ModContent.ItemType<AresTrophy>());

					CalamityWorld.downedAres = true;
					CalamityWorld.downedExoMechs = true;
					CalamityNetcode.SyncWorld();

					break;

				case (int)MechType.Thanatos:

					DropHelper.DropItem(npc, ModContent.ItemType<ThanatosTrophy>());

					CalamityWorld.downedThanatos = true;
					CalamityWorld.downedExoMechs = true;
					CalamityNetcode.SyncWorld();

					break;

				case (int)MechType.ArtemisAndApollo:

					DropHelper.DropItem(npc, ModContent.ItemType<ArtemisTrophy>());
					DropHelper.DropItem(npc, ModContent.ItemType<ApolloTrophy>());

					CalamityWorld.downedArtemisAndApollo = true;
					CalamityWorld.downedExoMechs = true;
					CalamityNetcode.SyncWorld();

					break;
			}

			CalamityGlobalNPC.SetNewBossJustDowned(npc);

			DropHelper.DropBags(npc);

			// All other drops are contained in the bag, so they only drop directly on Normal
			if (!Main.expertMode)
			{
				// Materials
				DropHelper.DropItem(npc, ModContent.ItemType<ExoPrism>(), true, 24, 32);

				// Weapons
				// Higher chance due to how the drops work
				float w = DropHelper.NormalWeaponDropRateFloat * 2f;
				if (CalamityWorld.downedAres)
				{
					DropHelper.DropEntireWeightedSet(npc,
						DropHelper.WeightStack<PhotonRipper>(w),
						DropHelper.WeightStack<TheJailor>(w)
					);
				}
				if (CalamityWorld.downedThanatos)
				{
					DropHelper.DropEntireWeightedSet(npc,
						DropHelper.WeightStack<SpineOfThanatos>(w),
						DropHelper.WeightStack<RefractionRotor>(w)
					);
				}
				if (CalamityWorld.downedArtemisAndApollo)
				{
					DropHelper.DropEntireWeightedSet(npc,
						DropHelper.WeightStack<SurgeDriver>(w),
						DropHelper.WeightStack<TheAtomSplitter>(w)
					);
				}

				// Equipment
				DropHelper.DropItemChance(npc, ModContent.ItemType<ExoThrone>(), 5);

				// Vanity
				// Higher chance due to how the drops work
				float maskDropRate = 1f / 3.5f;
				if (CalamityWorld.downedThanatos)
					DropHelper.DropItemChance(npc, ModContent.ItemType<ThanatosMask>(), maskDropRate);

				if (CalamityWorld.downedArtemisAndApollo)
				{
					DropHelper.DropItemChance(npc, ModContent.ItemType<ArtemisMask>(), maskDropRate);
					DropHelper.DropItemChance(npc, ModContent.ItemType<ApolloMask>(), maskDropRate);
				}

				if (CalamityWorld.downedAres)
					DropHelper.DropItemChance(npc, ModContent.ItemType<AresMask>(), maskDropRate);

				DropHelper.DropItemChance(npc, ModContent.ItemType<DraedonMask>(), maskDropRate);
			}
		}

		public override void HitEffect(int hitDirection, double damage)
		{
			for (int k = 0; k < 3; k++)
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

				Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/Ares/AresBody1"), 1f);
				Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/Ares/AresBody2"), 1f);
				Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/Ares/AresBody3"), 1f);
				Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/Ares/AresBody4"), 1f);
				Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/Ares/AresBody5"), 1f);
				Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/Ares/AresBody6"), 1f);
				Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/Ares/AresBody7"), 1f);
			}
		}

		public override bool CheckActive() => false;

		public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
		{
			npc.lifeMax = (int)(npc.lifeMax * 0.8f * bossLifeScale);
			npc.damage = (int)(npc.damage * 0.8f);
		}
	}
}
