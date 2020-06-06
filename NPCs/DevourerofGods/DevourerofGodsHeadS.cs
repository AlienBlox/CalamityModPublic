﻿using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Items.Armor.Vanity;
using CalamityMod.Items.LoreItems;
using CalamityMod.Items.Materials;
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
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.DevourerofGods
{
    [AutoloadBossHead]
    public class DevourerofGodsHeadS : ModNPC
    {
        private bool tail = false;
        private const int minLength = 100;
        private const int maxLength = 101;
        private bool halfLife = false;
        private int[] shotSpacing = new int[4] { 1050, 1050, 1050, 1050 };
        private const int spacingVar = 105;
        private const int totalShots = 20;
		private const int idleCounterMax = 360;
        private int idleCounter = idleCounterMax;
        public int laserWallPhase = 0;
		private int postTeleportTimer = 0;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("The Devourer of Gods");
        }

        public override void SetDefaults()
        {
            npc.damage = 300;
            npc.npcSlots = 5f;
            npc.width = 186;
            npc.height = 186;
            npc.defense = 50;
            npc.LifeMaxNERB(1150000, 1350000, 9200000);
            double HPBoost = CalamityConfig.Instance.BossHealthBoost * 0.01;
            npc.lifeMax += (int)(npc.lifeMax * HPBoost);
            npc.takenDamageMultiplier = 1.25f;
            npc.aiStyle = -1;
            aiType = -1;
            npc.knockBackResist = 0f;
            npc.boss = true;
            npc.value = Item.buyPrice(1, 0, 0, 0);
            npc.alpha = 255;
            npc.behindTiles = true;
            npc.noGravity = true;
            npc.noTileCollide = true;
			npc.DeathSound = SoundID.NPCDeath14;
            npc.netAlways = true;
            for (int k = 0; k < npc.buffImmune.Length; k++)
            {
                npc.buffImmune[k] = true;
            }
            Mod calamityModMusic = ModLoader.GetMod("CalamityModMusic");
            if (calamityModMusic != null)
                music = calamityModMusic.GetSoundSlot(SoundType.Music, "Sounds/Music/UniversalCollapse");
            else
                music = MusicID.LunarBoss;
            bossBag = ModContent.ItemType<DevourerofGodsBag>();
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
			writer.Write(npc.dontTakeDamage);
            writer.Write(halfLife);
            writer.Write(shotSpacing[0]);
            writer.Write(shotSpacing[1]);
            writer.Write(shotSpacing[2]);
            writer.Write(shotSpacing[3]);
            writer.Write(idleCounter);
            writer.Write(laserWallPhase);
			writer.Write(postTeleportTimer);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
			npc.dontTakeDamage = reader.ReadBoolean();
            halfLife = reader.ReadBoolean();
            shotSpacing[0] = reader.ReadInt32();
            shotSpacing[1] = reader.ReadInt32();
            shotSpacing[2] = reader.ReadInt32();
            shotSpacing[3] = reader.ReadInt32();
            idleCounter = reader.ReadInt32();
            laserWallPhase = reader.ReadInt32();
			postTeleportTimer = reader.ReadInt32();
        }

        public override void BossHeadRotation(ref float rotation)
        {
            rotation = npc.rotation;
        }

        public override void AI()
        {
            CalamityGlobalNPC calamityGlobalNPC = npc.Calamity();

            // whoAmI variable
            CalamityGlobalNPC.DoGHead = npc.whoAmI;

            // Percent life remaining
            float lifeRatio = npc.life / (float)npc.lifeMax;

            // Variables
            Vector2 vector = npc.Center;
            bool flies = npc.ai[2] == 0f;
            bool expertMode = Main.expertMode || CalamityWorld.bossRushActive;
			bool revenge = CalamityWorld.revenge || CalamityWorld.bossRushActive;
			bool death = CalamityWorld.death || CalamityWorld.bossRushActive;
            bool speedBoost = lifeRatio < 0.6 || (death && lifeRatio < 0.9);
            bool speedBoost2 = lifeRatio < 0.2;
            bool breathFireMore = lifeRatio < 0.15 || death;

			// Light
			Lighting.AddLight((int)((npc.position.X + (npc.width / 2)) / 16f), (int)((npc.position.Y + (npc.height / 2)) / 16f), 0.2f, 0.05f, 0.2f);

			// Worm shit again
			if (npc.ai[3] > 0f)
				npc.realLife = (int)npc.ai[3];

			// Target
			if (npc.target < 0 || npc.target == 255 || Main.player[npc.target].dead || !Main.player[npc.target].active)
				npc.TargetClosest(true);

			Player player = Main.player[npc.target];

			float distanceFromTarget = Vector2.Distance(player.Center, vector);
			bool tooFarAway = distanceFromTarget > 5600f;

			// Immunity after teleport
			npc.dontTakeDamage = postTeleportTimer > 0;

			// Laser walls
			if (speedBoost && !speedBoost2 && postTeleportTimer <= 0)
            {
                if (laserWallPhase == 0) // Start laser wall phase
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
						calamityGlobalNPC.newAI[3] += 1f;
                        if (calamityGlobalNPC.newAI[3] >= 720f)
                        {
							calamityGlobalNPC.newAI[3] = 0f;
                            laserWallPhase = 1;
                        }
                    }
                }
                else if (laserWallPhase == 1) // Turn invisible and fire laser walls
                {
                    npc.alpha += 5;
                    if (npc.alpha == 255)
                        calamityGlobalNPC.newAI[1] = 0f;

                    if (npc.alpha >= 255)
                    {
                        npc.alpha = 255;
                        idleCounter--;
                        if (idleCounter <= 0)
                        {
                            laserWallPhase = 2;
                            idleCounter = idleCounterMax;
                        }
                    }
                }
                else if (laserWallPhase == 2) // Turn visible
                {
					if (distanceFromTarget > 500f && revenge)
						Teleport(player);
					else
					{
						npc.alpha -= 1;
						if (npc.alpha <= 0)
						{
							npc.alpha = 0;
							laserWallPhase = 0;
						}
					}
                }
            }
            else
            {
				if (postTeleportTimer > 0)
				{
					postTeleportTimer -= 4;
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

                if (laserWallPhase > 0)
                    laserWallPhase = 0;
            }

			// Anger message
			if (speedBoost2)
            {
                if (!halfLife)
                {
                    string key = "Mods.CalamityMod.EdgyBossText11";
                    Color messageColor = Color.Cyan;
                    if (Main.netMode == NetmodeID.SinglePlayer)
                        Main.NewText(Language.GetTextValue(key), messageColor);
                    else if (Main.netMode == NetmodeID.Server)
                        NetMessage.BroadcastChatMessage(NetworkText.FromKey(key), messageColor);

                    halfLife = true;
                }
            }

            // Spawn segments and fire projectiles
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                // Segments
                if (!tail && npc.ai[0] == 0f)
                {
                    int Previous = npc.whoAmI;
                    for (int segmentSpawn = 0; segmentSpawn < maxLength; segmentSpawn++)
                    {
                        int segment;
                        if (segmentSpawn >= 0 && segmentSpawn < minLength)
                            segment = NPC.NewNPC((int)npc.position.X + (npc.width / 2), (int)npc.position.Y + (npc.height / 2), ModContent.NPCType<DevourerofGodsBodyS>(), npc.whoAmI);
                        else
                            segment = NPC.NewNPC((int)npc.position.X + (npc.width / 2), (int)npc.position.Y + (npc.height / 2), ModContent.NPCType<DevourerofGodsTailS>(), npc.whoAmI);

                        Main.npc[segment].realLife = npc.whoAmI;
                        Main.npc[segment].ai[2] = npc.whoAmI;
                        Main.npc[segment].ai[1] = Previous;
                        Main.npc[Previous].ai[0] = segment;
                        NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, segment, 0f, 0f, 0f, 0);
                        Previous = segment;
                    }
                    tail = true;
                }

                int projectileDamage = expertMode ? 69 : 80;

                // Fireballs
                if (npc.alpha <= 0 && distanceFromTarget > 500f)
                {
                    calamityGlobalNPC.newAI[0] += 1f;
                    if (calamityGlobalNPC.newAI[0] >= 150f && calamityGlobalNPC.newAI[0] % (breathFireMore ? 60f : 120f) == 0f)
                    {
                        Vector2 vector44 = new Vector2(npc.position.X + npc.width * 0.5f, npc.position.Y + npc.height * 0.5f);
                        float num427 = player.position.X + (player.width / 2) - vector44.X;
                        float num428 = player.position.Y + (player.height / 2) - vector44.Y;
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            float num430 = 16f;
                            int num432 = ModContent.ProjectileType<DoGFire>();

                            float num429 = (float)Math.Sqrt(num427 * num427 + num428 * num428);
                            num429 = num430 / num429;
                            num427 *= num429;
                            num428 *= num429;
                            num428 += npc.velocity.Y * 0.5f;
                            num427 += npc.velocity.X * 0.5f;
                            vector44.X -= num427 * 1f;
                            vector44.Y -= num428 * 1f;

                            Projectile.NewProjectile(vector44.X, vector44.Y, num427, num428, num432, projectileDamage, 0f, Main.myPlayer, 0f, 0f);
                        }
                    }
                }
                else if (distanceFromTarget < 250f)
                    calamityGlobalNPC.newAI[0] = 0f;

                // Laser walls
                if (!speedBoost2 && (laserWallPhase == 1 || calamityGlobalNPC.enraged > 0 || (CalamityConfig.Instance.BossRushXerocCurse && CalamityWorld.bossRushActive)))
                {
					calamityGlobalNPC.newAI[1] += 1f;

                    float speed = 4f;
                    float divisor = 120f;

					if (calamityGlobalNPC.newAI[1] % divisor == 0f)
					{
						if (calamityGlobalNPC.newAI[1] % idleCounterMax == 0f)
						{
							int cosmicGuardianCount = NPC.CountNPCS(ModContent.NPCType<DevourerofGodsHead2>());
							if (cosmicGuardianCount < 2)
							{
								Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/DevourerSpawn"), (int)player.position.X, (int)player.position.Y);

								int spawnAmt = cosmicGuardianCount == 0 ? 2 : 1;

								for (int i = 0; i < spawnAmt; i++)
									NPC.SpawnOnPlayer(npc.FindClosestPlayer(), ModContent.NPCType<DevourerofGodsHead2>());
							}
						}
						else
						{
							Main.PlaySound(2, (int)player.position.X, (int)player.position.Y, 12);

							float targetPosY = player.position.Y + (Main.rand.NextBool(2) ? 50f : 0f);

							// Side walls
							for (int x = 0; x < totalShots; x++)
							{
								Projectile.NewProjectile(player.position.X + 1000f, targetPosY + shotSpacing[0], -speed, 0f, ModContent.ProjectileType<DoGDeath>(), projectileDamage, 0f, Main.myPlayer, 0f, 0f);
								Projectile.NewProjectile(player.position.X - 1000f, targetPosY + shotSpacing[0], speed, 0f, ModContent.ProjectileType<DoGDeath>(), projectileDamage, 0f, Main.myPlayer, 0f, 0f);
								shotSpacing[0] -= spacingVar;
							}

							if (Main.rand.NextBool(2) && revenge)
							{
								for (int x = 0; x < 10; x++)
								{
									Projectile.NewProjectile(player.position.X + 1000f, targetPosY + shotSpacing[3], -speed, 0f, ModContent.ProjectileType<DoGDeath>(), projectileDamage, 0f, Main.myPlayer, 0f, 0f);
									Projectile.NewProjectile(player.position.X - 1000f, targetPosY + shotSpacing[3], speed, 0f, ModContent.ProjectileType<DoGDeath>(), projectileDamage, 0f, Main.myPlayer, 0f, 0f);
									shotSpacing[3] -= Main.rand.NextBool(2) ? 180 : 200;
								}
								shotSpacing[3] = 1050;
							}
							shotSpacing[0] = 1050;

							// Lower wall
							for (int x = 0; x < totalShots; x++)
							{
								Projectile.NewProjectile(player.position.X + shotSpacing[1], player.position.Y + 1000f, 0f, -speed, ModContent.ProjectileType<DoGDeath>(), projectileDamage, 0f, Main.myPlayer, 0f, 0f);
								shotSpacing[1] -= spacingVar;
							}
							shotSpacing[1] = 1050;

							// Upper wall
							for (int x = 0; x < totalShots; x++)
							{
								Projectile.NewProjectile(player.position.X + shotSpacing[2], player.position.Y - 1000f, 0f, speed, ModContent.ProjectileType<DoGDeath>(), projectileDamage, 0f, Main.myPlayer, 0f, 0f);
								shotSpacing[2] -= spacingVar;
							}
							shotSpacing[2] = 1050;
						}
					}
                }
            }

            // Despawn
            if (!NPC.AnyNPCs(ModContent.NPCType<DevourerofGodsTailS>()))
                npc.active = false;

            float fallSpeed = death ? 17.75f : 16f;

			if (expertMode)
				fallSpeed += 3.5f * (1f - lifeRatio);

			if (player.dead)
            {
				npc.TargetClosest(false);
				flies = true;
                npc.velocity.Y -= 3f;
                if ((double)npc.position.Y < Main.topWorld + 16f)
                {
                    npc.velocity.Y -= 3f;
                    fallSpeed = 32f;
                }
                if ((double)npc.position.Y < Main.topWorld + 16f)
                {
                    for (int a = 0; a < 200; a++)
                    {
                        if (Main.npc[a].type == ModContent.NPCType<DevourerofGodsHeadS>() || Main.npc[a].type == ModContent.NPCType<DevourerofGodsBodyS>() || Main.npc[a].type == ModContent.NPCType<DevourerofGodsTailS>())
                            Main.npc[a].active = false;
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
            if (npc.ai[2] == 0f)
            {
                if (Main.netMode != NetmodeID.Server)
                {
                    if (!Main.player[Main.myPlayer].dead && Main.player[Main.myPlayer].active && Vector2.Distance(Main.player[Main.myPlayer].Center, vector) < 5600f)
                        Main.player[Main.myPlayer].AddBuff(ModContent.BuffType<Warped>(), 2);
                }

				calamityGlobalNPC.newAI[2] += 1f;

                npc.localAI[1] = 0f;

				int phaseLimit = death ? 600 : 900;
				float speed = death ? 16.5f : 15f;
                float turnSpeed = death ? 0.33f : 0.3f;
                float homingSpeed = death ? 30f : 24f;
                float homingTurnSpeed = death ? 0.405f : 0.33f;

				if (expertMode)
				{
					phaseLimit /= (1 + (int)(5f * (1f - lifeRatio)));

					if (phaseLimit < 180)
						phaseLimit = 180;

					speed += 3f * (1f - lifeRatio);
					turnSpeed += 0.06f * (1f - lifeRatio);
					homingSpeed += 12f * (1f - lifeRatio);
					homingTurnSpeed += 0.15f * (1f - lifeRatio);
				}

				// Go to ground phase sooner
				if (tooFarAway)
				{
					if (revenge && laserWallPhase == 0 && !player.dead && player.active)
						Teleport(player);
					else
						calamityGlobalNPC.newAI[2] += 10f;
				}

				float num188 = speed;
                float num189 = turnSpeed;
                Vector2 vector18 = new Vector2(npc.position.X + npc.width * 0.5f, npc.position.Y + npc.height * 0.5f);
                float num191 = player.position.X + (player.width / 2);
                float num192 = player.position.Y + (player.height / 2);
                int num42 = -1;
                int num43 = (int)(player.Center.X / 16f);
                int num44 = (int)(player.Center.Y / 16f);

                // Charge at target for 1.5 seconds
                bool flyAtTarget = (!speedBoost || speedBoost2) && calamityGlobalNPC.newAI[2] > phaseLimit - 90 && revenge;

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

                npc.rotation = (float)Math.Atan2(npc.velocity.Y, npc.velocity.X) + 1.57f;

                if (calamityGlobalNPC.newAI[2] > phaseLimit)
                {
                    npc.ai[2] = 1f;
					calamityGlobalNPC.newAI[2] = 0f;
                    npc.netUpdate = true;
                }
            }

            // Ground
            else
            {
                if (Main.netMode != NetmodeID.Server)
                {
                    if (!Main.player[Main.myPlayer].dead && Main.player[Main.myPlayer].active && Vector2.Distance(Main.player[Main.myPlayer].Center, vector) < 5600f)
                        Main.player[Main.myPlayer].AddBuff(ModContent.BuffType<ExtremeGrav>(), 2);
                }

				calamityGlobalNPC.newAI[2] += 1f;

                float turnSpeed = death ? 0.24f : 0.18f;

				if (expertMode)
					turnSpeed += 0.12f * (1f - lifeRatio);

                bool increaseSpeed = distanceFromTarget > 3200f;

				// Enrage
				if (tooFarAway)
				{
					if (revenge && laserWallPhase == 0 && !player.dead && player.active)
						Teleport(player);
					else
						turnSpeed *= 6f;
				}
				else if (increaseSpeed)
					turnSpeed *= 3f;

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

                float num189 = turnSpeed;
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

                if (!flies)
                {
                    npc.TargetClosest(true);

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
                    double maximumSpeed1 = death ? 0.46 : 0.4;
                    double maximumSpeed2 = death ? 1.125 : 1D;

					if (increaseSpeed)
					{
						maximumSpeed1 += 0.8;
						maximumSpeed2 += 2D;
					}

					if (expertMode)
					{
						maximumSpeed1 += 0.12f * (1f - lifeRatio);
						maximumSpeed2 += 0.25f * (1f - lifeRatio);
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

                npc.rotation = (float)Math.Atan2(npc.velocity.Y, npc.velocity.X) + 1.57f;

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

                if (calamityGlobalNPC.newAI[2] > (death ? 600f : 900f))
                {
                    npc.ai[2] = 0f;
					calamityGlobalNPC.newAI[2] = 0f;
                    npc.netUpdate = true;
                }
            }
        }

		private void Teleport(Player player)
		{
			postTeleportTimer = 255;
			npc.alpha = postTeleportTimer;

			int playerWidth = player.width / 2;
			int playerHeight = player.height / 2;

			float playerVelocityX = player.velocity.X;
			float playerVelocityY = player.velocity.Y;

			int x = (int)player.position.X + playerWidth - 25;
			int y = (int)player.position.Y + playerHeight - 25;

			float velocityThreshold = 0.05f;
			int vectorAdjustment = 500;
			if (playerVelocityX >= velocityThreshold)
			{
				x += vectorAdjustment;
				yAdjustment();
			}
			else if (playerVelocityX <= -velocityThreshold)
			{
				x -= vectorAdjustment;
				yAdjustment();
			}

			void yAdjustment()
			{
				if (playerVelocityY >= velocityThreshold)
					y += vectorAdjustment;
				else if (playerVelocityY <= -velocityThreshold)
					y -= vectorAdjustment;
			}

			int x2 = x + 50;
			int y2 = y + 50;

			float locationX = Main.rand.Next(x, x2);
			float locationY = Main.rand.Next(y, y2);
			Vector2 teleportLocation = new Vector2(locationX, locationY);

			npc.position = teleportLocation;
			npc.netUpdate = true;

			for (int i = 0; i < 200; i++)
			{
				if (Main.npc[i].active && (Main.npc[i].type == ModContent.NPCType<DevourerofGodsBodyS>() || Main.npc[i].type == ModContent.NPCType<DevourerofGodsTailS>()))
				{
					Main.npc[i].position = teleportLocation;
                    if (Main.npc[i].type == ModContent.NPCType<DevourerofGodsTailS>())
                    {
                        ((DevourerofGodsTailS)Main.npc[i].modNPC).setInvulTime(720);
                    }
					Main.npc[i].netUpdate = true;
				}
			}

			Vector2 npcCenter = new Vector2(npc.position.X + (npc.width / 2), npc.position.Y + (npc.height / 2));
			Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Item, "Sounds/Item/LargeMechGaussRifle"), (int)npcCenter.X, (int)npcCenter.Y);

			int dustAmt = 50;
			int random = 5;

			for (int j = 0; j < 10; j++)
			{
				random += j * 2;
				int dustAmtSpawned = 0;
				int scale = random * 13;
				float dustPositionX = npcCenter.X - (scale / 2);
				float dustPositionY = npcCenter.Y - (scale / 2);
				while (dustAmtSpawned < dustAmt)
				{
					float dustVelocityX = Main.rand.Next(-random, random);
					float dustVelocityY = Main.rand.Next(-random, random);
					float dustVelocityScalar = random * 2f;
					float dustVelocity = (float)Math.Sqrt(dustVelocityX * dustVelocityX + dustVelocityY * dustVelocityY);
					dustVelocity = dustVelocityScalar / dustVelocity;
					dustVelocityX *= dustVelocity;
					dustVelocityY *= dustVelocity;
					int dust = Dust.NewDust(new Vector2(dustPositionX, dustPositionY), scale, scale, 173, 0f, 0f, 100, default, 5f);
					Main.dust[dust].noGravity = true;
					Main.dust[dust].position.X = teleportLocation.X;
					Main.dust[dust].position.Y = teleportLocation.Y;
					Main.dust[dust].position.X += Main.rand.Next(-10, 11);
					Main.dust[dust].position.Y += Main.rand.Next(-10, 11);
					Main.dust[dust].velocity.X = dustVelocityX;
					Main.dust[dust].velocity.Y = dustVelocityY;
					dustAmtSpawned++;
				}
			}
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			SpriteEffects spriteEffects = SpriteEffects.None;
			if (npc.spriteDirection == 1)
				spriteEffects = SpriteEffects.FlipHorizontally;

			Texture2D texture2D15 = Main.npcTexture[npc.type];
			Vector2 vector11 = new Vector2(Main.npcTexture[npc.type].Width / 2, Main.npcTexture[npc.type].Height / 2);

			Vector2 vector43 = npc.Center - Main.screenPosition;
			vector43 -= new Vector2(texture2D15.Width, texture2D15.Height) * npc.scale / 2f;
			vector43 += vector11 * npc.scale + new Vector2(0f, 4f + npc.gfxOffY);
			spriteBatch.Draw(texture2D15, vector43, npc.frame, npc.GetAlpha(lightColor), npc.rotation, vector11, npc.scale, spriteEffects, 0f);

			if (!npc.dontTakeDamage)
			{
				texture2D15 = ModContent.GetTexture("CalamityMod/NPCs/DevourerofGods/DevourerofGodsHeadSGlow");
				Color color37 = Color.Lerp(Color.White, Color.Fuchsia, 0.5f);

				spriteBatch.Draw(texture2D15, vector43, npc.frame, color37, npc.rotation, vector11, npc.scale, spriteEffects, 0f);

				texture2D15 = ModContent.GetTexture("CalamityMod/NPCs/DevourerofGods/DevourerofGodsHeadSGlow2");
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
                ModContent.NPCType<DevourerofGodsHeadS>(),
                ModContent.NPCType<DevourerofGodsBodyS>(),
                ModContent.NPCType<DevourerofGodsTailS>());
            npc.position = Main.npc[closestSegmentID].position;
            return false;
        }

        public override void NPCLoot()
        {
            // Stop the countdown -- if you kill DoG in less than 60 frames, this will stop another one from spawning.
            CalamityWorld.DoGSecondStageCountdown = 0;

            DropHelper.DropBags(npc);

            DropHelper.DropItem(npc, ModContent.ItemType<SupremeHealingPotion>(), 8, 14);
            DropHelper.DropItemChance(npc, ModContent.ItemType<DevourerofGodsTrophy>(), 10);
            DropHelper.DropItemCondition(npc, ModContent.ItemType<KnowledgeDevourerofGods>(), true, !CalamityWorld.downedDoG);
            DropHelper.DropResidentEvilAmmo(npc, CalamityWorld.downedDoG, 6, 3, 2);

			npc.Calamity().SetNewShopVariable(new int[] { ModContent.NPCType<THIEF>() }, CalamityWorld.downedDoG);

			// All other drops are contained in the bag, so they only drop directly on Normal
			if (!Main.expertMode)
            {
                // Materials
                DropHelper.DropItem(npc, ModContent.ItemType<CosmiliteBar>(), 25, 35);
                DropHelper.DropItem(npc, ModContent.ItemType<CosmiliteBrick>(), 150, 250);

                // Weapons
                DropHelper.DropItemChance(npc, ModContent.ItemType<Excelsus>(), 4);
                DropHelper.DropItemChance(npc, ModContent.ItemType<TheObliterator>(), 4);
                DropHelper.DropItemChance(npc, ModContent.ItemType<Deathwind>(), 4);
                DropHelper.DropItemChance(npc, ModContent.ItemType<Skullmasher>(), DropHelper.RareVariantDropRateInt);
                DropHelper.DropItemChance(npc, ModContent.ItemType<Norfleet>(), DropHelper.RareVariantDropRateInt);
                DropHelper.DropItemChance(npc, ModContent.ItemType<DeathhailStaff>(), 4);
                DropHelper.DropItemChance(npc, ModContent.ItemType<StaffoftheMechworm>(), 4);
				DropHelper.DropItemFromSetChance(npc, 0.25f, ModContent.ItemType<EradicatorMelee>(), ModContent.ItemType<Eradicator>());

                // Vanity
                DropHelper.DropItemChance(npc, ModContent.ItemType<DevourerofGodsMask>(), 7);
            }

            // If DoG has not been killed yet, notify players that the holiday moons are buffed
            if (!CalamityWorld.downedDoG)
            {
                string key = "Mods.CalamityMod.DoGBossText";
                Color messageColor = Color.Cyan;
                string key2 = "Mods.CalamityMod.DoGBossText2";
                Color messageColor2 = Color.Orange;

                if (Main.netMode == NetmodeID.SinglePlayer)
                {
                    Main.NewText(Language.GetTextValue(key), messageColor);
                    Main.NewText(Language.GetTextValue(key2), messageColor2);
                }
                else if (Main.netMode == NetmodeID.Server)
                {
                    NetMessage.BroadcastChatMessage(NetworkText.FromKey(key), messageColor);
                    NetMessage.BroadcastChatMessage(NetworkText.FromKey(key2), messageColor2);
                }
            }

            // Mark DoG as dead
            CalamityWorld.downedDoG = true;
            CalamityMod.UpdateServerBoolean();
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

            return minDist <= 80f && npc.alpha <= 0;
        }

        // Projectiles can only hit within certain distance
        /*public override bool? CanBeHitByProjectile(Projectile projectile)
        {
            Rectangle projectileHitbox = projectile.Hitbox;

            float dist1 = Vector2.Distance(npc.Center, projectileHitbox.TopLeft());
            float dist2 = Vector2.Distance(npc.Center, projectileHitbox.TopRight());
            float dist3 = Vector2.Distance(npc.Center, projectileHitbox.BottomLeft());
            float dist4 = Vector2.Distance(npc.Center, projectileHitbox.BottomRight());

            float minDist = dist1;
            if (dist2 < minDist) minDist = dist2;
            if (dist3 < minDist) minDist = dist3;
            if (dist4 < minDist) minDist = dist4;

            return minDist <= 80f;
        }*/

        // Melee hitboxes are fucked so I have no clue what to do here
        /*public override bool? CanBeHitByItem(Player player, Item item)
        {
            Rectangle targetHitbox = target.Hitbox;

            float dist1 = Vector2.Distance(npc.Center, targetHitbox.TopLeft());
            float dist2 = Vector2.Distance(npc.Center, targetHitbox.TopRight());
            float dist3 = Vector2.Distance(npc.Center, targetHitbox.BottomLeft());
            float dist4 = Vector2.Distance(npc.Center, targetHitbox.BottomRight());

            float minDist = dist1;
            if (dist2 < minDist) minDist = dist2;
            if (dist3 < minDist) minDist = dist3;
            if (dist4 < minDist) minDist = dist4;

            return minDist <= 80f && npc.alpha == 0;
        }*/

        public override bool StrikeNPC(ref double damage, int defense, ref float knockback, int hitDirection, ref bool crit)
        {
            if (CalamityUtils.AntiButcher(npc, ref damage, 0.5f))
            {
                string key = "Mods.CalamityMod.EdgyBossText2";
                Color messageColor = Color.Cyan;
                if (Main.netMode == NetmodeID.SinglePlayer)
                    Main.NewText(Language.GetTextValue(key), messageColor);
                else if (Main.netMode == NetmodeID.Server)
                    NetMessage.BroadcastChatMessage(NetworkText.FromKey(key), messageColor);
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
                    int num622 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 173, 0f, 0f, 100, default, 2f);
                    Main.dust[num622].velocity *= 3f;
                    if (Main.rand.NextBool(2))
                    {
                        Main.dust[num622].scale = 0.5f;
                        Main.dust[num622].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                    }
                }
                for (int num623 = 0; num623 < 30; num623++)
                {
                    int num624 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 173, 0f, 0f, 100, default, 3f);
                    Main.dust[num624].noGravity = true;
                    Main.dust[num624].velocity *= 5f;
                    num624 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 173, 0f, 0f, 100, default, 2f);
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
            player.AddBuff(BuffID.Frostburn, 300, true);
            if ((CalamityWorld.death || CalamityWorld.bossRushActive) && npc.alpha <= 0)
            {
                player.KillMe(PlayerDeathReason.ByCustomReason(player.name + "'s essence was consumed by the devourer."), 1000.0, 0, false);
            }

			// TODO: don't talk if the player has iframes
            if (player.immuneTime > 0 || player.immune)
                return;

            int num = Main.rand.Next(5);
            string key = "Mods.CalamityMod.EdgyBossText3";
            if (num == 0)
            {
                key = "Mods.CalamityMod.EdgyBossText3";
            }
            else if (num == 1)
            {
                key = "Mods.CalamityMod.EdgyBossText4";
            }
            else if (num == 2)
            {
                key = "Mods.CalamityMod.EdgyBossText5";
            }
            else if (num == 3)
            {
                key = "Mods.CalamityMod.EdgyBossText6";
            }
            else if (num == 4)
            {
                key = "Mods.CalamityMod.EdgyBossText7";
            }
            Color messageColor = Color.Cyan;
            if (Main.netMode == NetmodeID.SinglePlayer)
            {
                Main.NewText(Language.GetTextValue(key), messageColor);
            }
            else if (Main.netMode == NetmodeID.Server)
            {
                NetMessage.BroadcastChatMessage(NetworkText.FromKey(key), messageColor);
            }
        }
    }
}
