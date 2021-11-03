using CalamityMod.Events;
using CalamityMod.Items.Potions;
using CalamityMod.Items.TreasureBags;
using CalamityMod.NPCs.ExoMechs.Ares;
using CalamityMod.NPCs.ExoMechs.Thanatos;
using CalamityMod.Projectiles.Boss;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Graphics.Shaders;
using System.Collections.Generic;
using CalamityMod.Skies;

namespace CalamityMod.NPCs.ExoMechs.Apollo
{
	public class Apollo : ModNPC
    {
		public static int phase1IconIndex;
		public static int phase2IconIndex;

		internal static void LoadHeadIcons()
		{
			string phase1IconPath = "CalamityMod/NPCs/ExoMechs/Apollo/ApolloHead";
			string phase2IconPath = "CalamityMod/NPCs/ExoMechs/Apollo/ApolloPhase2Head";

			CalamityMod.Instance.AddBossHeadTexture(phase1IconPath, -1);
			phase1IconIndex = ModContent.GetModBossHeadSlot(phase1IconPath);

			CalamityMod.Instance.AddBossHeadTexture(phase2IconPath, -1);
			phase2IconIndex = ModContent.GetModBossHeadSlot(phase2IconPath);
		}

		public enum Phase
		{
			Normal = 0,
			RocketBarrage = 1,
			LineUpChargeCombo = 2,
			ChargeCombo = 3,
			PhaseTransition = 4
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

		// Number of frames on the X and Y axis
		private const int maxFramesX = 10;
		private const int maxFramesY = 9;

		// Counters for frames on the X and Y axis
		private int frameX = 0;
		private int frameY = 0;

		// Frame limit per animation, these are the specific frames where each animation ends
		private const int normalFrameLimit_Phase1 = 9;
		private const int chargeUpFrameLimit_Phase1 = 19;
		private const int attackFrameLimit_Phase1 = 29;
		private const int phaseTransitionFrameLimit = 59; // The lens pops off on frame 37
		private const int normalFrameLimit_Phase2 = 69;
		private const int chargeUpFrameLimit_Phase2 = 79;
		private const int attackFrameLimit_Phase2 = 89;

		// Default life ratio for the other mechs
		private const float defaultLifeRatio = 5f;

		// Max distance from the target before they are unable to hear sound telegraphs
		private const float soundDistance = 2800f;

		// Normal animation duration
		private const float defaultAnimationDuration = 60f;

		// Total duration of the phase transition
		private const float phaseTransitionDuration = 180f;

		// Where the timer should be when the lens pops off
		private const float lensPopTime = 48f;

		// Variable to pick a different location after each attack
		private bool pickNewLocation = false;

		// Charge locations during the charge combo
		private const int maxCharges = 4;
		public Vector2[] chargeLocations = new Vector2[maxCharges] { default, default, default, default };

		// Intensity of flash effects during the charge combo
		public float ChargeComboFlash;

		// Primitive trail drawers for thrusters when charging
		public PrimitiveTrail ChargeFlameTrail = null;
		public PrimitiveTrail ChargeFlameTrailBig = null;

		// Primitive trail drawer for the ribbon things
		public PrimitiveTrail RibbonTrail = null;

		public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("XS-03 Apollo");
			NPCID.Sets.TrailingMode[npc.type] = 3;
			NPCID.Sets.TrailCacheLength[npc.type] = 15;
		}

        public override void SetDefaults()
        {
			npc.Calamity().canBreakPlayerDefense = true;
			npc.npcSlots = 5f;
			npc.GetNPCDamage();
			npc.width = 204;
            npc.height = 226;
            npc.defense = 80;
			npc.DR_NERD(0.25f);
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
			music = /*CalamityMod.Instance.GetMusicFromMusicMod("AdultEidolonWyrm") ??*/ MusicID.Boss3;
			bossBag = ModContent.ItemType<DraedonTreasureBag>();
		}
		
		public override void BossHeadSlot(ref int index)
		{
			if (SecondaryAIState == (float)SecondaryPhase.PassiveAndImmune)
				index = -1;
			else if (npc.life / (float)npc.lifeMax < 0.6f)
				index = phase2IconIndex;
			else
				index = phase1IconIndex;
		}

		public override void SendExtraAI(BinaryWriter writer)
        {
			writer.Write(frameX);
			writer.Write(frameY);
			writer.Write(pickNewLocation);
            writer.Write(npc.dontTakeDamage);
			writer.Write(npc.localAI[0]);
			writer.Write(npc.localAI[1]);
			writer.Write(npc.localAI[2]);
			writer.Write(npc.localAI[3]);
			for (int i = 0; i < 4; i++)
			{
				writer.Write(npc.Calamity().newAI[i]);
				writer.WriteVector2(chargeLocations[i]);
			}
		}

        public override void ReceiveExtraAI(BinaryReader reader)
        {
			frameX = reader.ReadInt32();
			frameY = reader.ReadInt32();
			pickNewLocation = reader.ReadBoolean();
			npc.dontTakeDamage = reader.ReadBoolean();
			npc.localAI[0] = reader.ReadSingle();
			npc.localAI[1] = reader.ReadSingle();
			npc.localAI[2] = reader.ReadSingle();
			npc.localAI[3] = reader.ReadSingle();
			for (int i = 0; i < 4; i++)
			{
				npc.Calamity().newAI[i] = reader.ReadSingle();
				chargeLocations[i] = reader.ReadVector2();
			}
		}

        public override void AI()
        {
			CalamityGlobalNPC calamityGlobalNPC = npc.Calamity();

			CalamityGlobalNPC.draedonExoMechTwinGreen = npc.whoAmI;

			npc.frame = new Rectangle(npc.width * frameX, npc.height * frameY, npc.width, npc.height);

			// Difficulty modes
			bool malice = CalamityWorld.malice || BossRushEvent.BossRushActive;
			bool death = CalamityWorld.death || malice;
			bool revenge = CalamityWorld.revenge || malice;
			bool expertMode = Main.expertMode || malice;

			// Get a target
			if (npc.target < 0 || npc.target == Main.maxPlayers || Main.player[npc.target].dead || !Main.player[npc.target].active)
				npc.TargetClosest();

			// Despawn safety, make sure to target another player if the current player target is too far away
			if (Vector2.Distance(Main.player[npc.target].Center, npc.Center) > CalamityGlobalNPC.CatchUpDistance200Tiles)
				npc.TargetClosest();

			// Target variable
			Player player = Main.player[npc.target];

			// Check if the other exo mechs are alive
			int otherExoMechsAlive = 0;
			bool exoWormAlive = false;
			bool exoPrimeAlive = false;
			if (CalamityGlobalNPC.draedonExoMechTwinRed != -1)
			{
				if (Main.npc[CalamityGlobalNPC.draedonExoMechTwinRed].active)
				{
					// Link the HP of both twins
					if (npc.life > Main.npc[CalamityGlobalNPC.draedonExoMechTwinRed].life)
						npc.life = Main.npc[CalamityGlobalNPC.draedonExoMechTwinRed].life;
				}
			}
			if (CalamityGlobalNPC.draedonExoMechWorm != -1)
			{
				if (Main.npc[CalamityGlobalNPC.draedonExoMechWorm].active)
				{
					// Set target to Thanatos' target if Thanatos is alive
					player = Main.player[Main.npc[CalamityGlobalNPC.draedonExoMechWorm].target];

					otherExoMechsAlive++;
					exoWormAlive = true;
				}
			}
			if (CalamityGlobalNPC.draedonExoMechPrime != -1)
			{
				if (Main.npc[CalamityGlobalNPC.draedonExoMechPrime].active)
				{
					// Set target to Ares' target if Ares is alive
					player = Main.player[Main.npc[CalamityGlobalNPC.draedonExoMechPrime].target];

					otherExoMechsAlive++;
					exoPrimeAlive = true;
				}
			}

			// Percent life remaining
			float lifeRatio = npc.life / (float)npc.lifeMax;

			// These are 5 by default to avoid triggering passive phases after the other mechs are dead
			float exoWormLifeRatio = defaultLifeRatio;
			float exoPrimeLifeRatio = defaultLifeRatio;
			if (exoWormAlive)
				exoWormLifeRatio = Main.npc[CalamityGlobalNPC.draedonExoMechWorm].life / (float)Main.npc[CalamityGlobalNPC.draedonExoMechWorm].lifeMax;
			if (exoPrimeAlive)
				exoPrimeLifeRatio = Main.npc[CalamityGlobalNPC.draedonExoMechPrime].life / (float)Main.npc[CalamityGlobalNPC.draedonExoMechPrime].lifeMax;
			float totalOtherExoMechLifeRatio = exoWormLifeRatio + exoPrimeLifeRatio;

			// Check if any of the other mechs are passive
			bool exoWormPassive = false;
			bool exoPrimePassive = false;
			if (exoWormAlive)
				exoWormPassive = Main.npc[CalamityGlobalNPC.draedonExoMechWorm].Calamity().newAI[1] == (float)ThanatosHead.SecondaryPhase.Passive;
			if (exoPrimeAlive)
				exoPrimePassive = Main.npc[CalamityGlobalNPC.draedonExoMechPrime].Calamity().newAI[1] == (float)AresBody.SecondaryPhase.Passive;
			bool anyOtherExoMechPassive = exoWormPassive || exoPrimePassive;

			// Used to nerf Artemis and Apollo if fighting alongside Ares, because otherwise it's too difficult
			bool nerfedAttacks = false;
			if (exoPrimeAlive)
				nerfedAttacks = Main.npc[CalamityGlobalNPC.draedonExoMechPrime].Calamity().newAI[1] != (float)AresBody.SecondaryPhase.PassiveAndImmune;

			// Check if any of the other mechs were spawned first
			bool exoWormWasFirst = false;
			bool exoPrimeWasFirst = false;
			if (exoWormAlive)
				exoWormWasFirst = Main.npc[CalamityGlobalNPC.draedonExoMechWorm].ai[3] == 1f;
			if (exoPrimeAlive)
				exoPrimeWasFirst = Main.npc[CalamityGlobalNPC.draedonExoMechPrime].ai[3] == 1f;
			bool otherExoMechWasFirst = exoWormWasFirst || exoPrimeWasFirst;

			// Check for Draedon
			bool draedonAlive = false;
			if (CalamityGlobalNPC.draedon != -1)
			{
				if (Main.npc[CalamityGlobalNPC.draedon].active)
					draedonAlive = true;
			}

			// Prevent mechs from being respawned
			if (otherExoMechWasFirst)
				npc.ai[3] = 1f;

			// Phases
			bool phase2 = lifeRatio < 0.6f;
			bool spawnOtherExoMechs = lifeRatio < 0.7f && npc.ai[3] == 0f;
			bool berserk = lifeRatio < 0.4f || (otherExoMechsAlive == 0 && lifeRatio < 0.7f);
			bool lastMechAlive = berserk && otherExoMechsAlive == 0;

			// If Artemis and Apollo don't go berserk
			bool otherMechIsBerserk = exoWormLifeRatio < 0.4f || exoPrimeLifeRatio < 0.4f;

			// Spawn Artemis if it doesn't exist after the first 10 frames have passed
			if (npc.ai[0] < 10f)
			{
				npc.ai[0] += 1f;
				if (npc.ai[0] == 10f && !NPC.AnyNPCs(ModContent.NPCType<Artemis.Artemis>()))
					NPC.SpawnOnPlayer(player.whoAmI, ModContent.NPCType<Artemis.Artemis>());
			}
			else
			{
				if (!NPC.AnyNPCs(ModContent.NPCType<Artemis.Artemis>()))
				{
					npc.active = false;
					npc.netUpdate = true;
				}
			}

			// General AI pattern
			// 0 - Fly to the right of the target and fire plasma when ready
			// 1 - Fly to the right of the target and fire homing rockets, the rockets home in until close to the target and then fly off in the direction they were moving in
			// 2 - Fly below the target, create several line telegraphs to show the dash pattern and then dash along those lines extremely quickly
			// 3 - Go passive and fly to the right of the target while firing less projectiles
			// 4 - Go passive, immune and invisible; fly far to the left of the target and do nothing until next phase

			// Attack patterns
			// If spawned first
			// Phase 1 - 0, 1
			// Phase 2 - 4
			// Phase 3 - 3

			// If berserk, this is the last phase of Artemis and Apollo
			// Phase 4 - 0, 1, 2

			// If not berserk
			// Phase 4 - 4
			// Phase 5 - 0, 1

			// If berserk, this is the last phase of Artemis and Apollo
			// Phase 6 - 0, 1, 2

			// If not berserk
			// Phase 6 - 4

			// Berserk, final phase of Artemis and Apollo
			// Phase 7 - 0, 1, 2

			// Adjust opacity
			bool invisiblePhase = SecondaryAIState == (float)SecondaryPhase.PassiveAndImmune;
			npc.dontTakeDamage = invisiblePhase || AIState == (float)Phase.PhaseTransition;
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

			// Predictiveness
			float predictionAmt = malice ? 16f : death ? 12f : revenge ? 11f : expertMode ? 10f : 8f;
			if (nerfedAttacks)
				predictionAmt *= 0.5f;
			if (SecondaryAIState == (int)SecondaryPhase.Passive)
				predictionAmt *= 0.5f;

			// Gate values
			float reducedTimeForGateValue = malice ? 60f : death ? 40f : revenge ? 30f : expertMode ? 20f : 0f;
			float attackPhaseGateValue = (lastMechAlive ? 360f : 480f) - reducedTimeForGateValue;
			float timeToLineUpAttack = 30f;

			// Distance where Apollo stops moving
			float movementDistanceGateValue = 100f;
			float chargeLocationDistanceGateValue = 20f;

			// Velocity and acceleration values
			float baseVelocityMult = (berserk ? 0.25f : 0f) + (malice ? 1.15f : death ? 1.1f : revenge ? 1.075f : expertMode ? 1.05f : 1f);
			float baseVelocity = (AIState == (int)Phase.LineUpChargeCombo ? 30f : 20f) * baseVelocityMult;

			// Attack gate values
			bool lineUpAttack = calamityGlobalNPC.newAI[3] >= attackPhaseGateValue + 2f;
			bool doBigAttack = calamityGlobalNPC.newAI[3] >= attackPhaseGateValue + 2f + timeToLineUpAttack;

			// Charge velocity
			float chargeVelocity = malice ? 115f : death ? 105f : revenge ? 101.25f : expertMode ? 97.5f : 90f;

			// Charge phase variables
			double chargeDistance = Math.Sqrt(500D * 500D + 800D * 800D);
			float chargeTime = (float)chargeDistance / chargeVelocity;

			// Plasma and rocket projectile velocities
			float projectileVelocity = 14f;
			if (lastMechAlive)
				projectileVelocity *= 1.2f;
			else if (berserk)
				projectileVelocity *= 1.1f;

			// Rocket phase variables
			float rocketPhaseDuration = lastMechAlive ? 60f : 90f;
			int numRockets = lastMechAlive ? 4 : nerfedAttacks ? 2 : 3;

			// Default vector to fly to
			bool flyRight = npc.ai[0] % 2f == 0f || npc.ai[0] < 10f || !revenge;
			float destinationX = flyRight ? 750f : -750f;
			float destinationY = player.Center.Y;
			float chargeComboXOffset = flyRight ? -500f : 500f;
			float chargeComboYOffset = npc.ai[2] == 0f ? 400f : -400f;
			Vector2 destination = SecondaryAIState == (float)SecondaryPhase.PassiveAndImmune ? new Vector2(player.Center.X + destinationX * 1.6f, destinationY) : AIState == (float)Phase.LineUpChargeCombo ? new Vector2(player.Center.X + destinationX, destinationY + chargeComboYOffset) : new Vector2(player.Center.X + destinationX, destinationY);

			// If Apollo can fire projectiles, cannot fire if too close to the target
			bool canFire = Vector2.Distance(npc.Center, player.Center) > 320f;

			// Rotation
			Vector2 predictionVector = player.velocity * predictionAmt;
			Vector2 aimedVector = player.Center + predictionVector - npc.Center;
			float rateOfRotation = 0.1f;
			Vector2 rotateTowards = player.Center - npc.Center;
			bool readyToCharge = AIState == (float)Phase.LineUpChargeCombo && (Vector2.Distance(npc.Center, destination) <= chargeLocationDistanceGateValue || calamityGlobalNPC.newAI[2] > 0f);
			if (AIState == (int)Phase.ChargeCombo)
			{
				rateOfRotation = 0f;
				npc.rotation = npc.velocity.ToRotation() + MathHelper.PiOver2;
			}
			else if (AIState == (float)Phase.LineUpChargeCombo && chargeLocations[1] != default)
			{
				float x = chargeLocations[1].X - npc.Center.X;
				float y = chargeLocations[1].Y - npc.Center.Y;
				rotateTowards = Vector2.Normalize(new Vector2(x, y)) * baseVelocity;
			}
			else
			{
				float x = player.Center.X + predictionVector.X - npc.Center.X;
				float y = player.Center.Y + predictionVector.Y - npc.Center.Y;
				rotateTowards = Vector2.Normalize(new Vector2(x, y)) * baseVelocity;
			}

			// Do not set this during charge or deathray phases
			if (rateOfRotation != 0f)
				npc.rotation = npc.rotation.AngleTowards((float)Math.Atan2(rotateTowards.Y, rotateTowards.X) + MathHelper.PiOver2, rateOfRotation);

			// Light
			Lighting.AddLight(npc.Center, 0.05f * npc.Opacity, 0.25f * npc.Opacity, 0.15f * npc.Opacity);

			// Despawn if target is dead
			if (player.dead)
			{
				npc.TargetClosest(false);
				player = Main.player[npc.target];
				if (player.dead)
				{
					AIState = (float)Phase.Normal;
					npc.localAI[0] = 0f;
					npc.localAI[1] = 0f;
					npc.localAI[2] = 0f;
					calamityGlobalNPC.newAI[2] = 0f;
					calamityGlobalNPC.newAI[3] = 0f;
					for (int i = 0; i < maxCharges; i++)
						chargeLocations[i] = default;

					npc.dontTakeDamage = true;

					npc.velocity.Y -= 1f;
					if ((double)npc.position.Y < Main.topWorld + 16f)
						npc.velocity.Y -= 1f;

					if ((double)npc.position.Y < Main.topWorld + 16f)
					{
						for (int a = 0; a < Main.maxNPCs; a++)
						{
							if (Main.npc[a].type == npc.type || Main.npc[a].type == ModContent.NPCType<Artemis.Artemis>() || Main.npc[a].type == ModContent.NPCType<AresBody>() ||
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

			// Add some random distance to the destination after certain attacks
			if (pickNewLocation)
			{
				pickNewLocation = false;

				npc.localAI[0] = Main.rand.Next(-50, 51);
				npc.localAI[1] = Main.rand.Next(-250, 251);
				if (AIState == (float)Phase.RocketBarrage)
				{
					npc.localAI[0] *= 0.5f;
					npc.localAI[1] *= 0.5f;
				}

				npc.netUpdate = true;
			}

			// Add a bit of randomness to the destination, but only in specific phases where it's necessary
			if (AIState == (float)Phase.Normal || AIState == (float)Phase.RocketBarrage || AIState == (float)Phase.PhaseTransition)
			{
				destination.X += npc.localAI[0];
				destination.Y += npc.localAI[1];
			}

			// Cause the charge visual effects to begin while performing or preparing for a combo
			if (AIState == (float)Phase.LineUpChargeCombo || AIState == (float)Phase.ChargeCombo)
				ChargeComboFlash = MathHelper.Clamp(ChargeComboFlash + 0.08f, 0f, 1f);

			// And have them go away afterwards
			else
				ChargeComboFlash = MathHelper.Clamp(ChargeComboFlash - 0.1f, 0f, 1f);

			// Destination variables
			Vector2 distanceFromDestination = destination - npc.Center;
			Vector2 desiredVelocity = Vector2.Normalize(distanceFromDestination) * baseVelocity;

			// Set to transition to phase 2 if it hasn't happened yet
			if (phase2 && npc.localAI[3] == 0f)
			{
				AIState = (float)Phase.PhaseTransition;
				npc.localAI[3] = 1f;
				calamityGlobalNPC.newAI[2] = 0f;
				calamityGlobalNPC.newAI[3] = 0f;

				// Set frames to phase transition frames, which begin on frame 30
				// Reset the frame counter
				npc.frameCounter = 0D;

				// X = 3 sets to frame 27
				frameX = 3;

				// Y = 3 sets to frame 30
				frameY = 3;
			}

			// Passive and Immune phases
			switch ((int)SecondaryAIState)
			{
				case (int)SecondaryPhase.Nothing:

					// Spawn the other mechs if Artemis and Apollo are first
					if (otherExoMechsAlive == 0)
					{
						if (spawnOtherExoMechs)
						{
							// Reset everything
							if (npc.ai[0] < 10f)
								npc.ai[0] = 10f;
							npc.ai[0] += 1f;
							npc.ai[3] = 1f;
							SecondaryAIState = (float)SecondaryPhase.PassiveAndImmune;
							npc.localAI[0] = 0f;
							npc.localAI[1] = 0f;
							npc.localAI[2] = 0f;
							calamityGlobalNPC.newAI[2] = 0f;
							calamityGlobalNPC.newAI[3] = 0f;
							for (int i = 0; i < maxCharges; i++)
								chargeLocations[i] = default;

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
								NPC.SpawnOnPlayer(player.whoAmI, ModContent.NPCType<AresBody>());
							}
						}
					}
					else
					{
						// If not spawned first, go to passive state if any other mech is passive or if Artemis and Apollo are under 70% life
						// Do not run this if berserk
						// Do not run this if any exo mech is dead
						if ((anyOtherExoMechPassive || lifeRatio < 0.7f) && !berserk && totalOtherExoMechLifeRatio < 5f)
						{
							// Tells Apollo to return to the battle in passive state and reset everything
							SecondaryAIState = (float)SecondaryPhase.Passive;
							npc.localAI[0] = 0f;
							npc.localAI[1] = 0f;
							npc.localAI[2] = 0f;
							calamityGlobalNPC.newAI[2] = 0f;
							calamityGlobalNPC.newAI[3] = 0f;
							for (int i = 0; i < maxCharges; i++)
								chargeLocations[i] = default;

							npc.TargetClosest();
						}

						// Go passive and immune if one of the other mechs is berserk
						// This is only called if two exo mechs are alive
						if (otherMechIsBerserk)
						{
							// Reset everything
							if (npc.ai[0] < 10f)
								npc.ai[0] = 10f;
							npc.ai[0] += 1f;
							SecondaryAIState = (float)SecondaryPhase.PassiveAndImmune;
							npc.localAI[0] = 0f;
							npc.localAI[1] = 0f;
							npc.localAI[2] = 0f;
							calamityGlobalNPC.newAI[2] = 0f;
							calamityGlobalNPC.newAI[3] = 0f;
							for (int i = 0; i < maxCharges; i++)
								chargeLocations[i] = default;

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

					// Fire plasma while passive
					AIState = (float)Phase.Normal;

					// Enter passive and invincible phase if one of the other exo mechs is berserk
					if (otherMechIsBerserk)
					{
						// Reset everything
						if (npc.ai[0] < 10f)
							npc.ai[0] = 10f;
						npc.ai[0] += 1f;
						SecondaryAIState = (float)SecondaryPhase.PassiveAndImmune;
						npc.localAI[0] = 0f;
						npc.localAI[1] = 0f;
						npc.localAI[2] = 0f;
						calamityGlobalNPC.newAI[2] = 0f;
						calamityGlobalNPC.newAI[3] = 0f;
						for (int i = 0; i < maxCharges; i++)
							chargeLocations[i] = default;

						npc.TargetClosest();
					}

					// If Artemis and Apollo are the first mechs to go berserk
					if (berserk)
					{
						// Reset everything
						npc.localAI[0] = 0f;
						npc.localAI[1] = 0f;
						npc.localAI[2] = 0f;
						calamityGlobalNPC.newAI[2] = 0f;
						calamityGlobalNPC.newAI[3] = 0f;
						for (int i = 0; i < maxCharges; i++)
							chargeLocations[i] = default;

						npc.TargetClosest();

						// Never be passive if berserk
						SecondaryAIState = (float)SecondaryPhase.Nothing;

						// Phase 4, when 1 mech goes berserk and the other 2 leave
						if (exoWormAlive && exoPrimeAlive)
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

					// Do nothing while immune
					AIState = (float)Phase.Normal;

					// Enter the fight again if any of the other exo mechs is below 70% and the other mechs aren't berserk
					if ((exoWormLifeRatio < 0.7f || exoPrimeLifeRatio < 0.7f) && !otherMechIsBerserk)
					{
						// Tells Artemis and Apollo to return to the battle in passive state and reset everything
						// Return to normal phases if one or more mechs have been downed
						SecondaryAIState = totalOtherExoMechLifeRatio > 5f ? (float)SecondaryPhase.Nothing : (float)SecondaryPhase.Passive;
						npc.localAI[0] = 0f;
						npc.localAI[1] = 0f;
						npc.localAI[2] = 0f;
						calamityGlobalNPC.newAI[2] = 0f;
						calamityGlobalNPC.newAI[3] = 0f;
						for (int i = 0; i < maxCharges; i++)
							chargeLocations[i] = default;

						npc.TargetClosest();

						// Phase 3, when all 3 mechs attack at the same time
						if (exoWormAlive && exoPrimeAlive)
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
						npc.localAI[0] = 0f;
						npc.localAI[1] = 0f;
						npc.localAI[2] = 0f;
						calamityGlobalNPC.newAI[2] = 0f;
						calamityGlobalNPC.newAI[3] = 0f;
						for (int i = 0; i < maxCharges; i++)
							chargeLocations[i] = default;

						npc.TargetClosest();

						// Never be passive if berserk
						SecondaryAIState = (float)SecondaryPhase.Nothing;
					}

					break;
			}

			// Attacking phases
			switch ((int)AIState)
			{
				// Fly to the right of the target
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

					// Default animation for 60 frames and then go to telegraph animation
					// newAI[3] tells Apollo what animation state it's currently in
					bool attacking = calamityGlobalNPC.newAI[3] >= 2f;
					bool firingPlasma = attacking && calamityGlobalNPC.newAI[3] + 2f < attackPhaseGateValue;

					// Only increase attack timer if not in immune phase
					if (SecondaryAIState != (float)SecondaryPhase.PassiveAndImmune)
						calamityGlobalNPC.newAI[2] += 1f;

					if (calamityGlobalNPC.newAI[2] >= defaultAnimationDuration || attacking)
					{
						if (firingPlasma)
						{
							// Fire plasma
							float divisor = nerfedAttacks ? 60f : lastMechAlive ? 36f : 40f;
							float plasmaTimer = calamityGlobalNPC.newAI[3] - 2f;
							if (plasmaTimer % divisor == 0f && canFire)
							{
								pickNewLocation = true;
								if (Main.netMode != NetmodeID.MultiplayerClient)
								{
									int type = ModContent.ProjectileType<ApolloFireball>();
									int damage = npc.GetProjectileDamage(type);
									Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Item, "Sounds/Item/PlasmaCasterFire"), npc.Center);
									Vector2 plasmaVelocity = Vector2.Normalize(aimedVector) * projectileVelocity;
									Vector2 offset = Vector2.Normalize(plasmaVelocity) * 70f;
									Projectile.NewProjectile(npc.Center + offset, plasmaVelocity, type, damage, 0f, Main.myPlayer, player.Center.X, player.Center.Y);
								}
							}
						}
						else
							calamityGlobalNPC.newAI[2] = 0f;

						// Enter rocket phase after a certain time has passed
						// Enter charge phase if in phase 2 and the localAI[2] variable is set to do so
						calamityGlobalNPC.newAI[3] += 1f;
						if (lineUpAttack)
						{
							// Return to normal plasma phase if in passive state
							if (SecondaryAIState == (float)SecondaryPhase.Passive)
							{
								pickNewLocation = true;
								calamityGlobalNPC.newAI[2] = 0f;
								calamityGlobalNPC.newAI[3] = 0f;
								for (int i = 0; i < maxCharges; i++)
									chargeLocations[i] = default;

								npc.TargetClosest();
							}
							else if (doBigAttack)
							{
								pickNewLocation = npc.localAI[2] == 0f;
								calamityGlobalNPC.newAI[2] = 0f;
								calamityGlobalNPC.newAI[3] = 0f;

								if (phase2)
									AIState = npc.localAI[2] == 1f ? (float)Phase.LineUpChargeCombo : (float)Phase.RocketBarrage;
								else
									AIState = (float)Phase.RocketBarrage;
							}
						}
					}

					break;

				// Charge
				case (int)Phase.RocketBarrage:

					// Inverse lerp returns the percentage of progress between A and B
					float lerpValue2 = Utils.InverseLerp(movementDistanceGateValue, 2400f, distanceFromDestination.Length(), true);

					// Min velocity
					float minVelocity2 = distanceFromDestination.Length();
					float minVelocityCap2 = baseVelocity;
					if (minVelocity2 > minVelocityCap2)
						minVelocity2 = minVelocityCap2;

					// Max velocity
					Vector2 maxVelocity2 = distanceFromDestination / 24f;
					float maxVelocityCap2 = minVelocityCap2 * 3f;
					if (maxVelocity2.Length() > maxVelocityCap2)
						maxVelocity2 = distanceFromDestination.SafeNormalize(Vector2.Zero) * maxVelocityCap2;

					npc.velocity = Vector2.Lerp(distanceFromDestination.SafeNormalize(Vector2.Zero) * minVelocity2, maxVelocity2, lerpValue2);

					calamityGlobalNPC.newAI[2] += 1f;
					if (calamityGlobalNPC.newAI[2] % (rocketPhaseDuration / numRockets) == 0f && canFire)
					{
						pickNewLocation = true;
						if (Main.netMode != NetmodeID.MultiplayerClient)
						{
							int type = ModContent.ProjectileType<ApolloRocket>();
							int damage = npc.GetProjectileDamage(type);
							Main.PlaySound(SoundID.Item36, npc.Center);
							Vector2 rocketVelocity = Vector2.Normalize(aimedVector) * projectileVelocity;
							Vector2 offset = Vector2.Normalize(rocketVelocity) * 70f;
							Projectile.NewProjectile(npc.Center + offset, rocketVelocity, type, damage, 0f, Main.myPlayer, 0f, player.Center.Y);
						}
					}

					// Reset phase and variables
					if (calamityGlobalNPC.newAI[2] >= rocketPhaseDuration)
					{
						// Go back to normal phase
						AIState = (float)Phase.Normal;
						npc.localAI[2] = berserk ? 1f : 0f;
						calamityGlobalNPC.newAI[2] = 0f;
						npc.TargetClosest();
					}

					break;

				// Get in position for the charge combo, don't do anything else until in position
				// Fill up the charge location array with the positions Apollo will charge from
				// Create telegraph beams between the charge location array positions
				case (int)Phase.LineUpChargeCombo:

					if (!readyToCharge)
					{
						// Inverse lerp returns the percentage of progress between A and B
						float lerpValue3 = Utils.InverseLerp(movementDistanceGateValue, 2400f, distanceFromDestination.Length(), true);

						// Min velocity
						float minVelocity3 = distanceFromDestination.Length();
						float minVelocityCap3 = baseVelocity;
						if (minVelocity3 > minVelocityCap3)
							minVelocity3 = minVelocityCap3;

						// Max velocity
						Vector2 maxVelocity3 = distanceFromDestination / 24f;
						float maxVelocityCap3 = minVelocityCap3 * 3f;
						if (maxVelocity3.Length() > maxVelocityCap3)
							maxVelocity3 = distanceFromDestination.SafeNormalize(Vector2.Zero) * maxVelocityCap3;

						npc.velocity = Vector2.Lerp(distanceFromDestination.SafeNormalize(Vector2.Zero) * minVelocity3, maxVelocity3, lerpValue3);
					}
					else
					{
						// Save the charge locations and create telegraph beams
						int type = ModContent.ProjectileType<ApolloChargeTelegraph>();

						for (int i = 0; i < maxCharges; i++)
						{
							if (chargeLocations[i] == default)
							{
								switch (i)
								{
									case 0:
										chargeLocations[i] = npc.Center;
										break;
									case 1:
										chargeLocations[i] = chargeLocations[0] + new Vector2(chargeComboXOffset, -chargeComboYOffset * 2f);
										break;
									case 2:
										chargeLocations[i] = chargeLocations[1] + new Vector2(chargeComboXOffset, chargeComboYOffset * 2f);
										break;
									case 3:
										chargeLocations[i] = chargeLocations[2] + new Vector2(chargeComboXOffset, -chargeComboYOffset * 2f);
										break;
									default:
										break;
								}

								if (i == 0)
								{
									// Draw telegraph beams
									if (Main.netMode != NetmodeID.MultiplayerClient)
									{
										int telegraph = Projectile.NewProjectile(chargeLocations[0], Vector2.Zero, type, 0, 0f, Main.myPlayer, 0f, npc.whoAmI);
										if (Main.projectile.IndexInRange(telegraph))
										{
											Main.projectile[telegraph].ModProjectile<ApolloChargeTelegraph>().ChargePositions = chargeLocations;
											Main.projectile[telegraph].netUpdate = true;
										}
									}

									// Play a charge sound
									Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Item, "Sounds/Item/LaserCannon"), npc.Center);
								}
							}
						}

						// Don't move
						npc.velocity = Vector2.Zero;

						// Go to charge phase, create lightning bolts in the sky, and reset
						calamityGlobalNPC.newAI[2] += 1f;
						if (calamityGlobalNPC.newAI[2] >= timeToLineUpAttack)
						{
							ExoMechsSky.CreateLightningBolt(10);

							AIState = (float)Phase.ChargeCombo;
							npc.localAI[2] = 0f;
							calamityGlobalNPC.newAI[2] = 0f;
						}
					}

					break;

				// Charge to several locations almost instantly (Apollo doesn't teleport here, he's just moving very fast :D)
				case (int)Phase.ChargeCombo:

					// Set charge velocity and fire halos of plasma bolts
					if (npc.localAI[2] == 0f)
					{
						Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Item, "Sounds/Item/ELRFire"), npc.Center);
						npc.velocity = Vector2.Normalize(chargeLocations[(int)calamityGlobalNPC.newAI[2] + 1] - chargeLocations[(int)calamityGlobalNPC.newAI[2]]) * chargeVelocity;
						npc.localAI[2] = 1f;
						npc.netUpdate = true;
						npc.netSpam -= 5;

						// Plasma bolts on charge
						if (Main.netMode != NetmodeID.MultiplayerClient)
						{
							int totalProjectiles = CalamityWorld.malice ? 12 : 8;
							float radians = MathHelper.TwoPi / totalProjectiles;
							int type = ModContent.ProjectileType<AresPlasmaBolt>();
							int damage = (int)(npc.GetProjectileDamage(ModContent.ProjectileType<ApolloFireball>()) * 0.8);
							float velocity = 1f;
							double angleA = radians * 0.5;
							double angleB = MathHelper.ToRadians(90f) - angleA;
							float velocityX2 = (float)(velocity * Math.Sin(angleA) / Math.Sin(angleB));
							Vector2 spinningPoint = Main.rand.NextBool() ? new Vector2(0f, -velocity) : new Vector2(-velocityX2, -velocity);
							for (int k = 0; k < totalProjectiles; k++)
							{
								Vector2 velocity2 = spinningPoint.RotatedBy(radians * k);
								Projectile.NewProjectile(npc.Center, velocity2, type, damage, 0f, Main.myPlayer);
							}
						}

						// Dust rings
						for (int i = 0; i < 200; i++)
						{
							float dustVelocity = 16f;
							if (i < 150)
								dustVelocity = 12f;
							if (i < 100)
								dustVelocity = 8f;
							if (i < 50)
								dustVelocity = 4f;

							int dust1 = Dust.NewDust(npc.Center, 6, 6, Main.rand.NextBool(2) ? 107 : 110, 0f, 0f, 100, default, 1f);
							float dustVelX = Main.dust[dust1].velocity.X;
							float dustVelY = Main.dust[dust1].velocity.Y;

							if (dustVelX == 0f && dustVelY == 0f)
								dustVelX = 1f;

							float dustVelocity2 = (float)Math.Sqrt(dustVelX * dustVelX + dustVelY * dustVelY);
							dustVelocity2 = dustVelocity / dustVelocity2;
							dustVelX *= dustVelocity2;
							dustVelY *= dustVelocity2;

							float scale = 1f;
							switch ((int)dustVelocity)
							{
								case 4:
									scale = 1.2f;
									break;
								case 8:
									scale = 1.1f;
									break;
								case 12:
									scale = 1f;
									break;
								case 16:
									scale = 0.9f;
									break;
								default:
									break;
							}

							Dust dust2 = Main.dust[dust1];
							dust2.velocity *= 0.5f;
							dust2.velocity.X = dust2.velocity.X + dustVelX;
							dust2.velocity.Y = dust2.velocity.Y + dustVelY;
							dust2.scale = scale;
							dust2.noGravity = true;
						}
					}

					// Initiate next charge if close enough to next charge location
					calamityGlobalNPC.newAI[3] += 1f;
					if (calamityGlobalNPC.newAI[3] >= chargeTime)
					{
						// Set Apollo's location to the next charge location
						npc.Center = chargeLocations[(int)calamityGlobalNPC.newAI[2] + 1];

						// Reset velocity to 0
						npc.velocity = Vector2.Zero;

						// Increase newAI[2] whenever a charge ends
						calamityGlobalNPC.newAI[2] += 1f;
						calamityGlobalNPC.newAI[3] = 0f;
						npc.localAI[2] = 0f;
					}

					// Reset phase and variables
					if (calamityGlobalNPC.newAI[2] >= maxCharges - 1)
					{
						pickNewLocation = true;
						AIState = (float)Phase.Normal;
						npc.localAI[2] = 0f;
						calamityGlobalNPC.newAI[2] = 0f;
						for (int i = 0; i < maxCharges; i++)
							chargeLocations[i] = default;
						ChargeComboFlash = 0f;

						npc.TargetClosest();
					}

					break;

				// Phase transition animation, that's all this exists for
				case (int)Phase.PhaseTransition:

					// Inverse lerp returns the percentage of progress between A and B
					float lerpValue4 = Utils.InverseLerp(movementDistanceGateValue, 2400f, distanceFromDestination.Length(), true);

					// Min velocity
					float minVelocity4 = distanceFromDestination.Length();
					float minVelocityCap4 = baseVelocity;
					if (minVelocity4 > minVelocityCap4)
						minVelocity4 = minVelocityCap4;

					// Max velocity
					Vector2 maxVelocity4 = distanceFromDestination / 24f;
					float maxVelocityCap4 = minVelocityCap4 * 3f;
					if (maxVelocity4.Length() > maxVelocityCap4)
						maxVelocity4 = distanceFromDestination.SafeNormalize(Vector2.Zero) * maxVelocityCap4;

					npc.velocity = Vector2.Lerp(distanceFromDestination.SafeNormalize(Vector2.Zero) * minVelocity4, maxVelocity4, lerpValue4);

					// Shoot lens gore at the target at the proper time
					if (calamityGlobalNPC.newAI[2] == lensPopTime)
					{
						Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Item, "Sounds/Item/LargeWeaponFire"), npc.Center);
						Vector2 lensDirection = Vector2.Normalize(aimedVector);
						Vector2 offset = lensDirection * 70f;

						if (Main.netMode != NetmodeID.MultiplayerClient)
							Projectile.NewProjectile(npc.Center + offset, lensDirection * 24f, ModContent.ProjectileType<BrokenApolloLens>(), 0, 0f);
					}

					// Reset phase and variables
					calamityGlobalNPC.newAI[2] += 1f;
					if (calamityGlobalNPC.newAI[2] >= phaseTransitionDuration)
					{
						pickNewLocation = true;
						AIState = (float)Phase.Normal;
						npc.localAI[0] = 0f;
						npc.localAI[1] = 0f;
						calamityGlobalNPC.newAI[2] = 0f;
						calamityGlobalNPC.newAI[3] = 0f;
						npc.TargetClosest();
					}

					break;
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

			return minDist <= 100f && npc.Opacity == 1f && AIState == (float)Phase.ChargeCombo;
		}

		public override bool StrikeNPC(ref double damage, int defense, ref float knockback, int hitDirection, ref bool crit) => !CalamityUtils.AntiButcher(npc, ref damage, 0.5f);

		public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
		{
			scale = 2f;
			return null;
		}

		public override void FindFrame(int frameHeight)
		{
			// Use telegraph frames before each attack
			bool phase2 = npc.life / (float)npc.lifeMax < 0.6f;
			npc.frameCounter += 1D;
			if (AIState == (float)Phase.PhaseTransition)
			{
				if (npc.frameCounter >= 6D)
				{
					// Reset frame counter
					npc.frameCounter = 0D;

					// Increment the Y frame
					frameY++;

					// Reset the Y frame if greater than 9
					if (frameY == maxFramesY)
					{
						frameX++;
						frameY = 0;
					}
				}
			}
			else
			{
				if (AIState == (float)Phase.Normal)
				{
					int frameLimit = phase2 ? (npc.Calamity().newAI[3] == 0f ? normalFrameLimit_Phase2 : npc.Calamity().newAI[3] == 1f ? chargeUpFrameLimit_Phase2 : attackFrameLimit_Phase2) :
						(npc.Calamity().newAI[3] == 0f ? normalFrameLimit_Phase1 : npc.Calamity().newAI[3] == 1f ? chargeUpFrameLimit_Phase1 : attackFrameLimit_Phase1);

					if (npc.frameCounter >= 6D)
					{
						// Reset frame counter
						npc.frameCounter = 0D;

						// Increment the Y frame
						frameY++;

						// Reset the Y frame if greater than 9
						if (frameY == maxFramesY)
						{
							frameX++;
							frameY = 0;
						}

						// Reset the frames
						int currentFrame = (frameX * maxFramesY) + frameY;
						if (currentFrame > frameLimit)
							frameX = frameY = phase2 ? (npc.Calamity().newAI[3] == 0f ? 6 : npc.Calamity().newAI[3] == 1f ? 7 : 8) : (npc.Calamity().newAI[3] == 0f ? 0 : npc.Calamity().newAI[3] == 1f ? 1 : 2);
					}
				}
				else if (AIState == (float)Phase.RocketBarrage || AIState == (float)Phase.LineUpChargeCombo || AIState == (float)Phase.ChargeCombo)
				{
					int frameLimit = phase2 ? attackFrameLimit_Phase2 : attackFrameLimit_Phase1;
					if (npc.frameCounter >= 6D)
					{
						// Reset frame counter
						npc.frameCounter = 0D;

						// Increment the Y frame
						frameY++;

						// Reset the Y frame if greater than 9
						if (frameY == maxFramesY)
						{
							frameX++;
							frameY = 0;
						}

						// Reset the frames
						int currentFrame = (frameX * maxFramesY) + frameY;
						if (currentFrame > frameLimit)
							frameX = frameY = phase2 ? 8 : 2;
					}
				}
			}
		}

		public float FlameTrailWidthFunction(float completionRatio) => MathHelper.SmoothStep(21f, 8f, completionRatio) * ChargeComboFlash;

		public float FlameTrailWidthFunctionBig(float completionRatio) => MathHelper.SmoothStep(34f, 12f, completionRatio) * ChargeComboFlash;

		public float RibbonTrailWidthFunction(float completionRatio)
		{
			float baseWidth = Utils.InverseLerp(1f, 0.54f, completionRatio, true) * 5f;
			float endTipWidth = CalamityUtils.Convert01To010(Utils.InverseLerp(0.96f, 0.89f, completionRatio, true)) * 2.4f;
			return baseWidth + endTipWidth;
		}

		public Color FlameTrailColorFunction(float completionRatio)
		{
			float trailOpacity = Utils.InverseLerp(0.8f, 0.27f, completionRatio, true) * Utils.InverseLerp(0f, 0.067f, completionRatio, true);
			Color startingColor = Color.Lerp(Color.White, Color.Cyan, 0.27f);
			Color middleColor = Color.Lerp(Color.Orange, Color.ForestGreen, 0.74f);
			Color endColor = Color.Lime;
			return CalamityUtils.MulticolorLerp(completionRatio, startingColor, middleColor, endColor) * ChargeComboFlash * trailOpacity;
		}

		public Color FlameTrailColorFunctionBig(float completionRatio)
		{
			float trailOpacity = Utils.InverseLerp(0.8f, 0.27f, completionRatio, true) * Utils.InverseLerp(0f, 0.067f, completionRatio, true) * 0.56f;
			Color startingColor = Color.Lerp(Color.White, Color.Cyan, 0.25f);
			Color middleColor = Color.Lerp(Color.Blue, Color.White, 0.35f);
			Color endColor = Color.Lerp(Color.DarkBlue, Color.White, 0.47f);
			Color color = CalamityUtils.MulticolorLerp(completionRatio, startingColor, middleColor, endColor) * ChargeComboFlash * trailOpacity;
			color.A = 0;
			return color;
		}

		public Color RibbonTrailColorFunction(float completionRatio)
		{
			Color startingColor = new Color(34, 40, 48);
			Color endColor = new Color(40, 160, 32);
			return Color.Lerp(startingColor, endColor, (float)Math.Pow(completionRatio, 1.5D)) * npc.Opacity;
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Color drawColor)
		{
			// Declare the trail drawers if they have yet to be defined.
			if (ChargeFlameTrail is null)
				ChargeFlameTrail = new PrimitiveTrail(FlameTrailWidthFunction, FlameTrailColorFunction, null, GameShaders.Misc["CalamityMod:ImpFlameTrail"]);

			if (ChargeFlameTrailBig is null)
				ChargeFlameTrailBig = new PrimitiveTrail(FlameTrailWidthFunctionBig, FlameTrailColorFunctionBig, null, GameShaders.Misc["CalamityMod:ImpFlameTrail"]);

			if (RibbonTrail is null)
				RibbonTrail = new PrimitiveTrail(RibbonTrailWidthFunction, RibbonTrailColorFunction);

			// Prepare the flame trail shader with its map texture.
			GameShaders.Misc["CalamityMod:ImpFlameTrail"].SetShaderTexture(ModContent.GetTexture("CalamityMod/ExtraTextures/ScarletDevilStreak"));

			int numAfterimages = ChargeComboFlash > 0f ? 0 : 5;
			Texture2D texture = Main.npcTexture[npc.type];
			Rectangle frame = new Rectangle(npc.width * frameX, npc.height * frameY, npc.width, npc.height);
			Vector2 origin = npc.Size * 0.5f;
			Vector2 center = npc.Center - Main.screenPosition;
			Color afterimageBaseColor = Color.White;

			// Draws a single instance of a regular, non-glowmask based Artemis.
			// This is created to allow easy duplication of them when drawing the charge.
			void drawInstance(Vector2 drawOffset, Color baseColor)
			{
				if (CalamityConfig.Instance.Afterimages)
				{
					for (int i = 1; i < numAfterimages; i += 2)
					{
						Color afterimageColor = baseColor;
						afterimageColor = Color.Lerp(afterimageColor, afterimageBaseColor, 0.5f);
						afterimageColor = npc.GetAlpha(afterimageColor);
						afterimageColor *= (numAfterimages - i) / 15f;
						Vector2 afterimageCenter = npc.oldPos[i] + new Vector2(npc.width, npc.height) / 2f - Main.screenPosition;
						afterimageCenter -= new Vector2(texture.Width, texture.Height) / new Vector2(maxFramesX, maxFramesY) * npc.scale / 2f;
						afterimageCenter += origin * npc.scale + new Vector2(0f, npc.gfxOffY);
						afterimageCenter += drawOffset;
						spriteBatch.Draw(texture, afterimageCenter, npc.frame, afterimageColor, npc.rotation, origin, npc.scale, SpriteEffects.None, 0f);
					}
				}

				spriteBatch.Draw(texture, center + drawOffset, frame, npc.GetAlpha(baseColor), npc.rotation, origin, npc.scale, SpriteEffects.None, 0f);
			}

			// Draw ribbons near the main thruster
			for (int direction = -1; direction <= 1; direction += 2)
			{
				Vector2 ribbonOffset = -Vector2.UnitY.RotatedBy(npc.rotation) * 14f;
				ribbonOffset += Vector2.UnitX.RotatedBy(npc.rotation) * direction * 26f;

				float currentSegmentRotation = npc.rotation;
				List<Vector2> ribbonDrawPositions = new List<Vector2>();
				for (int i = 0; i < 12; i++)
				{
					float ribbonCompletionRatio = i / 12f;
					float wrappedAngularOffset = MathHelper.WrapAngle(npc.oldRot[i + 1] - currentSegmentRotation) * 0.3f;
					float segmentRotationOffset = MathHelper.Clamp(wrappedAngularOffset, -0.12f, 0.12f);

					// Add a sinusoidal offset that goes based on time and completion ratio to create a waving-flag-like effect.
					// This is dampened for the first few points to prevent weird offsets. It is also dampened by high velocity.
					float sinusoidalRotationOffset = (float)Math.Sin(ribbonCompletionRatio * 2.22f + Main.GlobalTime * 3.4f) * 1.36f;
					float sinusoidalRotationOffsetFactor = Utils.InverseLerp(0f, 0.37f, ribbonCompletionRatio, true) * direction * 24f;
					sinusoidalRotationOffsetFactor *= Utils.InverseLerp(24f, 16f, npc.velocity.Length(), true);

					Vector2 sinusoidalOffset = Vector2.UnitY.RotatedBy(npc.rotation + sinusoidalRotationOffset) * sinusoidalRotationOffsetFactor;
					Vector2 ribbonSegmentOffset = Vector2.UnitY.RotatedBy(currentSegmentRotation) * ribbonCompletionRatio * 540f + sinusoidalOffset;
					ribbonDrawPositions.Add(npc.Center + ribbonSegmentOffset + ribbonOffset);

					currentSegmentRotation += segmentRotationOffset;
				}
				RibbonTrail.Draw(ribbonDrawPositions, -Main.screenPosition, 66);
			}

			int instanceCount = (int)MathHelper.Lerp(1f, 15f, ChargeComboFlash);
			Color baseInstanceColor = Color.Lerp(drawColor, Color.White, ChargeComboFlash);
			baseInstanceColor.A = (byte)(int)(255f - ChargeComboFlash * 255f);

			spriteBatch.EnterShaderRegion();

			drawInstance(Vector2.Zero, baseInstanceColor);
			if (instanceCount > 1)
			{
				baseInstanceColor *= 0.04f;
				float backAfterimageOffset = MathHelper.SmoothStep(0f, 2f, ChargeComboFlash);
				for (int i = 0; i < instanceCount; i++)
				{
					Vector2 drawOffset = (MathHelper.TwoPi * i / instanceCount + Main.GlobalTime * 0.8f).ToRotationVector2() * backAfterimageOffset;
					drawInstance(drawOffset, baseInstanceColor);
				}
			}

			texture = ModContent.GetTexture("CalamityMod/NPCs/ExoMechs/Apollo/ApolloGlow");
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
					afterimageCenter += origin * npc.scale + new Vector2(0f, npc.gfxOffY);
					spriteBatch.Draw(texture, afterimageCenter, npc.frame, afterimageColor, npc.rotation, origin, npc.scale, SpriteEffects.None, 0f);
				}
			}

			spriteBatch.Draw(texture, center, frame, Color.White * npc.Opacity, npc.rotation, origin, npc.scale, SpriteEffects.None, 0f);

			spriteBatch.ExitShaderRegion();

			// Draw a flame trail on the thrusters if needed. This happens during charges.
			if (ChargeComboFlash > 0f)
			{
				for (int direction = -1; direction <= 1; direction++)
				{
					Vector2 baseDrawOffset = new Vector2(0f, direction == 0f ? 18f : 60f).RotatedBy(npc.rotation);
					baseDrawOffset += new Vector2(direction * 64f, 0f).RotatedBy(npc.rotation);

					float backFlameLength = direction == 0f ? 700f : 190f;
					Vector2 drawStart = npc.Center + baseDrawOffset;
					Vector2 drawEnd = drawStart - (npc.rotation - MathHelper.PiOver2).ToRotationVector2() * ChargeComboFlash * backFlameLength;
					Vector2[] drawPositions = new Vector2[]
					{
						drawStart,
						drawEnd
					};

					if (direction == 0)
					{
						for (int i = 0; i < 4; i++)
						{
							Vector2 drawOffset = (MathHelper.TwoPi * i / 4f).ToRotationVector2() * 8f;
							ChargeFlameTrailBig.Draw(drawPositions, drawOffset - Main.screenPosition, 70);
						}
					}
					else
						ChargeFlameTrail.Draw(drawPositions, -Main.screenPosition, 70);
				}
			}

			return false;
		}

		public override bool SpecialNPCLoot()
		{
			int closestSegmentID = DropHelper.FindClosestWormSegment(npc,
				ModContent.NPCType<Artemis.Artemis>(),
				ModContent.NPCType<Apollo>());
			npc.position = Main.npc[closestSegmentID].position;
			return false;
		}

		public override void BossLoot(ref string name, ref int potionType)
		{
			potionType = ModContent.ItemType<OmegaHealingPotion>();
		}

		public override void NPCLoot()
        {
			// Check if the other exo mechs are alive
			bool exoWormAlive = false;
			bool exoPrimeAlive = false;
			if (CalamityGlobalNPC.draedonExoMechWorm != -1)
			{
				if (Main.npc[CalamityGlobalNPC.draedonExoMechWorm].active)
					exoWormAlive = true;
			}
			if (CalamityGlobalNPC.draedonExoMechPrime != -1)
			{
				if (Main.npc[CalamityGlobalNPC.draedonExoMechPrime].active)
					exoPrimeAlive = true;
			}

			// Check for Draedon
			bool draedonAlive = false;
			if (CalamityGlobalNPC.draedon != -1)
			{
				if (Main.npc[CalamityGlobalNPC.draedon].active)
					draedonAlive = true;
			}

			// Phase 5, when 1 mech dies and the other 2 return to fight
			if (exoWormAlive && exoPrimeAlive)
			{
				if (draedonAlive)
				{
					Main.npc[CalamityGlobalNPC.draedon].localAI[0] = 4f;
					Main.npc[CalamityGlobalNPC.draedon].ai[0] = Draedon.ExoMechPhaseDialogueTime;
				}
			}

			// Phase 7, when 1 mech dies and the final one returns to the fight
			else if (exoWormAlive || exoPrimeAlive)
			{
				if (draedonAlive)
				{
					Main.npc[CalamityGlobalNPC.draedon].localAI[0] = 6f;
					Main.npc[CalamityGlobalNPC.draedon].ai[0] = Draedon.ExoMechPhaseDialogueTime;
				}
			}

			// Mark Exo Mechs as dead and drop loot
			else
				AresBody.DropExoMechLoot(npc, (int)AresBody.MechType.ArtemisAndApollo);
		}

		// Needs edits
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

				Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/Apollo/Apollo1"), 1f);
				Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/Apollo/Apollo2"), 1f);
				Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/Apollo/Apollo3"), 1f);
				Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/Apollo/Apollo4"), 1f);
				Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/Apollo/Apollo5"), 1f);
			}
		}

		public override bool CheckDead()
		{
			// Kill Artemis if he's still alive when Apollo dies
			for (int i = 0; i < Main.maxNPCs; i++)
			{
				NPC nPC = Main.npc[i];
				if (nPC.active && nPC.type == ModContent.NPCType<Artemis.Artemis>() && nPC.life > 0)
				{
					nPC.life = 0;
					nPC.HitEffect(0, 10.0);
					nPC.checkDead();
					nPC.active = false;
				}
			}
			return true;
		}

		public override bool CheckActive() => false;

		public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
		{
			npc.lifeMax = (int)(npc.lifeMax * 0.8f * bossLifeScale);
			npc.damage = (int)(npc.damage * npc.GetExpertDamageMultiplier());
		}
    }
}
