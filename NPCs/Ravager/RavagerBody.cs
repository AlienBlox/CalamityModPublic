using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Items.Accessories;
using CalamityMod.Items.Armor.Vanity;
using CalamityMod.Items.LoreItems;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Furniture.Trophies;
using CalamityMod.Items.TreasureBags;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.Ravager
{
    [AutoloadBossHead]
    public class RavagerBody : ModNPC
    {
		private float velocityY = -16f;

		public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ravager");
            Main.npcFrameCount[npc.type] = 7;
        }

        public override void SetDefaults()
        {
            npc.lavaImmune = true;
            npc.npcSlots = 20f;
            npc.aiStyle = -1;
            npc.damage = 120;
            npc.width = 332;
            npc.height = 214;
            npc.defense = 55;
            npc.value = Item.buyPrice(0, 25, 0, 0);
			npc.DR_NERD(0.4f);
            npc.LifeMaxNERB(42700, 53500, 4600000);
            if (CalamityWorld.downedProvidence && !CalamityWorld.bossRushActive)
            {
                npc.damage *= 2;
                npc.defense *= 2;
                npc.lifeMax *= 7;
                npc.value *= 1.5f;
            }
            double HPBoost = CalamityConfig.Instance.BossHealthBoost * 0.01;
            npc.lifeMax += (int)(npc.lifeMax * HPBoost);
            npc.knockBackResist = 0f;
            aiType = -1;
            for (int k = 0; k < npc.buffImmune.Length; k++)
            {
                npc.buffImmune[k] = true;
            }
            npc.buffImmune[BuffID.Ichor] = false;
            npc.buffImmune[BuffID.CursedInferno] = false;
			npc.buffImmune[BuffID.Frostburn] = false;
			npc.buffImmune[BuffID.Daybreak] = false;
			npc.buffImmune[BuffID.BetsysCurse] = false;
			npc.buffImmune[BuffID.StardustMinionBleed] = false;
			npc.buffImmune[BuffID.DryadsWardDebuff] = false;
			npc.buffImmune[BuffID.Oiled] = false;
            npc.buffImmune[ModContent.BuffType<AstralInfectionDebuff>()] = false;
            npc.buffImmune[ModContent.BuffType<AbyssalFlames>()] = false;
            npc.buffImmune[ModContent.BuffType<ArmorCrunch>()] = false;
            npc.buffImmune[ModContent.BuffType<DemonFlames>()] = false;
            npc.buffImmune[ModContent.BuffType<GodSlayerInferno>()] = false;
            npc.buffImmune[ModContent.BuffType<HolyFlames>()] = false;
            npc.buffImmune[ModContent.BuffType<Nightwither>()] = false;
            npc.buffImmune[ModContent.BuffType<Shred>()] = false;
            npc.buffImmune[ModContent.BuffType<WhisperingDeath>()] = false;
            npc.buffImmune[ModContent.BuffType<SilvaStun>()] = false;
            npc.boss = true;
            npc.alpha = 255;
            npc.HitSound = SoundID.NPCHit41;
            npc.DeathSound = SoundID.NPCDeath14;
            Mod calamityModMusic = ModLoader.GetMod("CalamityModMusic");
            if (calamityModMusic != null)
                music = calamityModMusic.GetSoundSlot(SoundType.Music, "Sounds/Music/Ravager");
            else
                music = MusicID.Boss4;
            bossBag = ModContent.ItemType<RavagerBag>();
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(npc.dontTakeDamage);
			writer.Write(npc.noGravity);
			writer.Write(velocityY);
		}

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            npc.dontTakeDamage = reader.ReadBoolean();
			npc.noGravity = reader.ReadBoolean();
			velocityY = reader.ReadSingle();
		}

        public override void FindFrame(int frameHeight)
        {
            npc.frameCounter += 0.15f;
            npc.frameCounter %= Main.npcFrameCount[npc.type];
            int frame = (int)npc.frameCounter;
            npc.frame.Y = frame * frameHeight;
        }

        public override void AI()
        {
            bool provy = CalamityWorld.downedProvidence && !CalamityWorld.bossRushActive;
            bool expertMode = Main.expertMode || CalamityWorld.bossRushActive;
			bool revenge = CalamityWorld.revenge || CalamityWorld.bossRushActive;
			bool death = CalamityWorld.death || CalamityWorld.bossRushActive;

			// Percent life remaining
			float lifeRatio = npc.life / (float)npc.lifeMax;

            // Large fire light
            Lighting.AddLight((int)(npc.Center.X - 110f) / 16, (int)(npc.Center.Y - 30f) / 16, 0f, 0.5f, 2f);
            Lighting.AddLight((int)(npc.Center.X + 110f) / 16, (int)(npc.Center.Y - 30f) / 16, 0f, 0.5f, 2f);

            // Small fire light
            Lighting.AddLight((int)(npc.Center.X - 40f) / 16, (int)(npc.Center.Y - 60f) / 16, 0f, 0.25f, 1f);
            Lighting.AddLight((int)(npc.Center.X + 40f) / 16, (int)(npc.Center.Y - 60f) / 16, 0f, 0.25f, 1f);

            CalamityGlobalNPC.scavenger = npc.whoAmI;

            if (npc.localAI[0] == 0f && Main.netMode != NetmodeID.MultiplayerClient)
            {
                npc.localAI[0] = 1f;
                NPC.NewNPC((int)npc.Center.X - 70, (int)npc.Center.Y + 88, ModContent.NPCType<RavagerLegLeft>(), 0, 0f, 0f, 0f, 0f, 255);
                NPC.NewNPC((int)npc.Center.X + 70, (int)npc.Center.Y + 88, ModContent.NPCType<RavagerLegRight>(), 0, 0f, 0f, 0f, 0f, 255);
                NPC.NewNPC((int)npc.Center.X - 120, (int)npc.Center.Y + 50, ModContent.NPCType<RavagerClawLeft>(), 0, 0f, 0f, 0f, 0f, 255);
                NPC.NewNPC((int)npc.Center.X + 120, (int)npc.Center.Y + 50, ModContent.NPCType<RavagerClawRight>(), 0, 0f, 0f, 0f, 0f, 255);
                NPC.NewNPC((int)npc.Center.X + 1, (int)npc.Center.Y - 20, ModContent.NPCType<RavagerHead>(), 0, 0f, 0f, 0f, 0f, 255);
            }

            if (npc.target >= 0 && Main.player[npc.target].dead)
            {
                npc.TargetClosest(true);
                if (Main.player[npc.target].dead)
                    npc.noTileCollide = true;
            }

			Player player = Main.player[npc.target];

            if (npc.alpha > 0)
            {
                npc.alpha -= 10;
                if (npc.alpha < 0)
                    npc.alpha = 0;

                npc.ai[1] = 0f;
            }

            bool leftLegActive = false;
            bool rightLegActive = false;
            bool headActive = false;
            bool rightClawActive = false;
            bool leftClawActive = false;

            for (int num619 = 0; num619 < 200; num619++)
            {
                if (Main.npc[num619].active && Main.npc[num619].type == ModContent.NPCType<RavagerHead>())
                    headActive = true;
                if (Main.npc[num619].active && Main.npc[num619].type == ModContent.NPCType<RavagerClawRight>())
                    rightClawActive = true;
                if (Main.npc[num619].active && Main.npc[num619].type == ModContent.NPCType<RavagerClawLeft>())
                    leftClawActive = true;
                if (Main.npc[num619].active && Main.npc[num619].type == ModContent.NPCType<RavagerLegRight>())
                    rightLegActive = true;
                if (Main.npc[num619].active && Main.npc[num619].type == ModContent.NPCType<RavagerLegLeft>())
                    leftLegActive = true;
            }

			bool immunePhase = headActive || rightClawActive || leftClawActive || rightLegActive || leftLegActive;
			bool finalPhase = !leftClawActive && !rightClawActive && !headActive && !leftLegActive && !rightLegActive;

			bool enrage = false;
            if (player.position.Y + (player.height / 2) > npc.position.Y + (npc.height / 2) + 10f)
                enrage = true;

            if (immunePhase)
                npc.dontTakeDamage = true;
            else
            {
                npc.dontTakeDamage = false;
                if (Main.netMode != NetmodeID.Server)
                {
                    if (!Main.player[Main.myPlayer].dead && Main.player[Main.myPlayer].active && revenge)
                        Main.player[Main.myPlayer].AddBuff(ModContent.BuffType<WeakPetrification>(), 2);
                }
            }

            if (!headActive)
            {
                int rightDust = Dust.NewDust(new Vector2(npc.Center.X, npc.Center.Y - 30f), 8, 8, 5, 0f, 0f, 100, default, 2.5f);
                Main.dust[rightDust].alpha += Main.rand.Next(100);
                Main.dust[rightDust].velocity *= 0.2f;

                Dust rightDustExpr = Main.dust[rightDust];
                rightDustExpr.velocity.Y -= 3f + Main.rand.Next(10) * 0.1f;
                Main.dust[rightDust].fadeIn = 0.5f + Main.rand.Next(10) * 0.1f;

                if (Main.rand.NextBool(10))
                {
                    rightDust = Dust.NewDust(new Vector2(npc.Center.X, npc.Center.Y - 30f), 8, 8, 6, 0f, 0f, 0, default, 1.5f);
                    if (Main.rand.Next(20) != 0)
                    {
                        Main.dust[rightDust].noGravity = true;
                        Main.dust[rightDust].scale *= 1f + Main.rand.Next(10) * 0.1f;
                        Dust rightDustExpr2 = Main.dust[rightDust];
                        rightDustExpr2.velocity.Y -= 4f;
                    }
                }

				if (!finalPhase)
				{
					if (Main.netMode != NetmodeID.MultiplayerClient)
					{
						npc.localAI[1] += enrage ? 6f : 1f;
						if (npc.localAI[1] >= 600f)
						{
							npc.localAI[1] = 0f;
							npc.TargetClosest(true);
							if (Collision.CanHit(npc.position, npc.width, npc.height, player.position, player.width, player.height))
							{
								float velocity = CalamityWorld.bossRushActive ? 10f : 7f;
								int totalProjectiles = 8;
								float radians = MathHelper.TwoPi / totalProjectiles;
								int laserDamage = 45;
								for (int i = 0; i < totalProjectiles; i++)
								{
									Vector2 vector255 = new Vector2(0f, -velocity).RotatedBy(radians * i);
									Projectile.NewProjectile(npc.Center, vector255, ProjectileID.EyeBeam, laserDamage + (provy ? 30 : 0), 0f, Main.myPlayer, 0f, 0f);
								}
							}
						}
					}
				}
            }

            if (!rightClawActive)
            {
                int rightDust = Dust.NewDust(new Vector2(npc.Center.X + 80f, npc.Center.Y + 45f), 8, 8, 5, 0f, 0f, 100, default, 3f);
                Main.dust[rightDust].alpha += Main.rand.Next(100);
                Main.dust[rightDust].velocity *= 0.2f;

                Dust rightDustExpr = Main.dust[rightDust];
                rightDustExpr.velocity.X += 3f + Main.rand.Next(10) * 0.1f;
                Main.dust[rightDust].fadeIn = 0.5f + Main.rand.Next(10) * 0.1f;

                if (Main.rand.NextBool(10))
                {
                    rightDust = Dust.NewDust(new Vector2(npc.Center.X + 80f, npc.Center.Y + 45f), 8, 8, 6, 0f, 0f, 0, default, 2f);
                    if (Main.rand.Next(20) != 0)
                    {
                        Main.dust[rightDust].noGravity = true;
                        Main.dust[rightDust].scale *= 1f + Main.rand.Next(10) * 0.1f;
                        Dust rightDustExpr2 = Main.dust[rightDust];
                        rightDustExpr2.velocity.X += 4f;
                    }
                }

				if (!finalPhase)
				{
					if (Main.netMode != NetmodeID.MultiplayerClient)
					{
						npc.localAI[2] += enrage ? 2f : 1f;
						if (npc.localAI[2] >= 480f)
						{
							Main.PlaySound(SoundID.Item20, npc.position);
							npc.localAI[2] = 0f;
							Vector2 shootFromVector = new Vector2(npc.Center.X + 80f, npc.Center.Y + 45f);
							int damage = 40;
							float velocity = CalamityWorld.bossRushActive ? 18f : 12f;
							Projectile.NewProjectile(shootFromVector.X, shootFromVector.Y, velocity, 0f, ProjectileID.Fireball, damage + (provy ? 30 : 0), 0f, Main.myPlayer, 0f, 0f);
						}
					}
				}
            }

            if (!leftClawActive)
            {
                int leftDust = Dust.NewDust(new Vector2(npc.Center.X - 80f, npc.Center.Y + 45f), 8, 8, 5, 0f, 0f, 100, default, 3f);
                Main.dust[leftDust].alpha += Main.rand.Next(100);
                Main.dust[leftDust].velocity *= 0.2f;

                Dust leftDustExpr = Main.dust[leftDust];
                leftDustExpr.velocity.X -= 3f + Main.rand.Next(10) * 0.1f;
                Main.dust[leftDust].fadeIn = 0.5f + Main.rand.Next(10) * 0.1f;

                if (Main.rand.NextBool(10))
                {
                    leftDust = Dust.NewDust(new Vector2(npc.Center.X - 80f, npc.Center.Y + 45f), 8, 8, 6, 0f, 0f, 0, default, 2f);
                    if (Main.rand.Next(20) != 0)
                    {
                        Main.dust[leftDust].noGravity = true;
                        Main.dust[leftDust].scale *= 1f + Main.rand.Next(10) * 0.1f;
                        Dust leftDustExpr2 = Main.dust[leftDust];
                        leftDustExpr2.velocity.X -= 4f;
                    }
                }

				if (!finalPhase)
				{
					if (Main.netMode != NetmodeID.MultiplayerClient)
					{
						npc.localAI[3] += enrage ? 2f : 1f;
						if (npc.localAI[3] >= 480f)
						{
							Main.PlaySound(SoundID.Item20, npc.position);
							npc.localAI[3] = 0f;
							Vector2 shootFromVector = new Vector2(npc.Center.X - 80f, npc.Center.Y + 45f);
							int damage = 40;
							float velocity = CalamityWorld.bossRushActive ? -18f : -12f;
							Projectile.NewProjectile(shootFromVector.X, shootFromVector.Y, velocity, 0f, ProjectileID.Fireball, damage + (provy ? 30 : 0), 0f, Main.myPlayer, 0f, 0f);
						}
					}
				}
            }

            if (!rightLegActive)
            {
                int rightDust = Dust.NewDust(new Vector2(npc.Center.X + 60f, npc.Center.Y + 60f), 8, 8, 5, 0f, 0f, 100, default, 2f);
                Main.dust[rightDust].alpha += Main.rand.Next(100);
                Main.dust[rightDust].velocity *= 0.2f;

                Dust rightDustExpr = Main.dust[rightDust];
                rightDustExpr.velocity.Y += 0.5f + Main.rand.Next(10) * 0.1f;
                Main.dust[rightDust].fadeIn = 0.5f + Main.rand.Next(10) * 0.1f;

                if (Main.rand.NextBool(10))
                {
                    rightDust = Dust.NewDust(new Vector2(npc.Center.X + 60f, npc.Center.Y + 60f), 8, 8, 6, 0f, 0f, 0, default, 1.5f);
                    if (Main.rand.Next(20) != 0)
                    {
                        Main.dust[rightDust].noGravity = true;
                        Main.dust[rightDust].scale *= 1f + Main.rand.Next(10) * 0.1f;
                        Dust rightDustExpr2 = Main.dust[rightDust];
                        rightDustExpr2.velocity.Y += 1f;
                    }
                }

				if (!finalPhase)
				{
					if (Main.netMode != NetmodeID.MultiplayerClient)
					{
						npc.ai[2] += 1f;
						if (npc.ai[2] >= 300f)
						{
							npc.ai[2] = 0f;
							Vector2 shootFromVector = new Vector2(npc.Center.X + 60f, npc.Center.Y + 60f);
							int damage = 35;
							int fire = Projectile.NewProjectile(shootFromVector.X, shootFromVector.Y, 0f, 2f, ProjectileID.GreekFire1 + Main.rand.Next(3), damage + (provy ? 30 : 0), 0f, Main.myPlayer, 0f, 0f);
							Main.projectile[fire].timeLeft = 180;
						}
					}
				}
            }

            if (!leftLegActive)
            {
                int leftDust = Dust.NewDust(new Vector2(npc.Center.X - 60f, npc.Center.Y + 60f), 8, 8, 5, 0f, 0f, 100, default, 2f);
                Main.dust[leftDust].alpha += Main.rand.Next(100);
                Main.dust[leftDust].velocity *= 0.2f;

                Dust leftDustExpr = Main.dust[leftDust];
                leftDustExpr.velocity.Y += 0.5f + Main.rand.Next(10) * 0.1f;
                Main.dust[leftDust].fadeIn = 0.5f + Main.rand.Next(10) * 0.1f;

                if (Main.rand.NextBool(10))
                {
                    leftDust = Dust.NewDust(new Vector2(npc.Center.X - 60f, npc.Center.Y + 60f), 8, 8, 6, 0f, 0f, 0, default, 1.5f);
                    if (Main.rand.Next(20) != 0)
                    {
                        Main.dust[leftDust].noGravity = true;
                        Main.dust[leftDust].scale *= 1f + Main.rand.Next(10) * 0.1f;
                        Dust leftDustExpr2 = Main.dust[leftDust];
                        leftDustExpr2.velocity.Y += 1f;
                    }
                }

				if (!finalPhase)
				{
					if (Main.netMode != NetmodeID.MultiplayerClient)
					{
						npc.ai[3] += 1f;
						if (npc.ai[3] >= 300f)
						{
							npc.ai[3] = 0f;
							Vector2 shootFromVector = new Vector2(npc.Center.X - 60f, npc.Center.Y + 60f);
							int damage = 35;
							int fire = Projectile.NewProjectile(shootFromVector.X, shootFromVector.Y, 0f, 2f, ProjectileID.GreekFire1 + Main.rand.Next(3), damage + (provy ? 30 : 0), 0f, Main.myPlayer, 0f, 0f);
							Main.projectile[fire].timeLeft = 180;
						}
					}
				}
            }

            if (npc.ai[0] == 0f)
            {
                npc.noTileCollide = false;

                if (npc.velocity.Y == 0f)
                {
                    npc.velocity.X *= 0.8f;

                    npc.ai[1] += 1f;
                    if (npc.ai[1] > 0f)
                    {
						if (revenge)
						{
							if (npc.Calamity().newAI[0] % 3f == 0f)
								npc.ai[1] += 1f;
							else if (npc.Calamity().newAI[0] % 2f == 0f)
								npc.ai[1] += 1f;
						}

						if ((!rightClawActive && !leftClawActive) || death || npc.Calamity().enraged > 0 || (CalamityConfig.Instance.BossRushXerocCurse && CalamityWorld.bossRushActive))
                            npc.ai[1] += 1f;
                        if (!headActive || death || npc.Calamity().enraged > 0 || (CalamityConfig.Instance.BossRushXerocCurse && CalamityWorld.bossRushActive))
                            npc.ai[1] += 1f;
                        if ((!rightLegActive && !leftLegActive) || death || npc.Calamity().enraged > 0 || (CalamityConfig.Instance.BossRushXerocCurse && CalamityWorld.bossRushActive))
                            npc.ai[1] += 1f;
                    }

                    if (npc.ai[1] >= 300f)
                        npc.ai[1] = -20f;
                    else if (npc.ai[1] == -1f)
                    {
                        npc.TargetClosest(true);

						bool shouldFall = player.position.Y >= npc.Bottom.Y;
						float velocityXBoost = death ? 4f : 4f * (1f - lifeRatio);
						float velocityX = ((enrage || npc.Calamity().enraged > 0 || (CalamityConfig.Instance.BossRushXerocCurse && CalamityWorld.bossRushActive)) ? 8f : 4f) + velocityXBoost;
						velocityY = -16f;

						float distanceBelowTarget = npc.position.Y - (player.position.Y + 80f);
						float speedMult = 1f;

						if (revenge)
						{
							if (distanceBelowTarget > 0f)
								speedMult += distanceBelowTarget * 0.002f;

							if (speedMult > 2f)
								speedMult = 2f;

							velocityY *= speedMult;
						}

						if (expertMode)
						{
							npc.noTileCollide = true;
							if (shouldFall)
								velocityY = 1f;

							if (npc.Calamity().newAI[0] % 3f == 0f)
							{
								velocityX *= 2f;
								if (!shouldFall)
									velocityY *= 0.5f;
							}
							else if (npc.Calamity().newAI[0] % 2f == 0f)
							{
								velocityX *= 1.5f;
								if (!shouldFall)
									velocityY *= 0.75f;
							}
						}

						npc.velocity.X = velocityX * npc.direction;
                        npc.velocity.Y = velocityY;

                        npc.ai[0] = finalPhase && !shouldFall ? 2f : 1f;
                        npc.ai[1] = 0f;

						if (npc.ai[0] == 2f)
							npc.noGravity = true;
					}
                }
            }
            else if (npc.ai[0] >= 1f)
            {
                if (npc.velocity.Y == 0f && (npc.ai[1] == 31f || npc.ai[0] == 1f))
                {
					Main.PlaySound(SoundID.Item, (int)npc.position.X, (int)npc.position.Y, 14, 1.25f, -0.25f);

					npc.ai[0] = 0f;
					npc.ai[1] = 0f;

					if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
						if (expertMode)
						{
							for (int i = 0; i < Main.maxNPCs; i++)
							{
								if (Main.npc[i].type == ModContent.NPCType<RockPillar>() && Main.npc[i].ai[0] == 0f)
								{
									Main.npc[i].ai[1] = -1f;
									Main.npc[i].direction = npc.direction;
								}
							}

							int spawnDistance = 360;

							if (NPC.CountNPCS(ModContent.NPCType<RockPillar>()) < 2)
							{
								NPC.NewNPC((int)npc.Center.X - spawnDistance, (int)npc.Center.Y - 10, ModContent.NPCType<RockPillar>(), 0, 0f, 0f, 0f, 0f, 255);
								NPC.NewNPC((int)npc.Center.X + spawnDistance, (int)npc.Center.Y - 10, ModContent.NPCType<RockPillar>(), 0, 0f, 0f, 0f, 0f, 255);
							}
							else if (NPC.CountNPCS(ModContent.NPCType<FlamePillar>()) < 2)
							{
								NPC.NewNPC((int)npc.Center.X - spawnDistance * 2, (int)npc.Center.Y - 10, ModContent.NPCType<FlamePillar>(), 0, 0f, 0f, 0f, 0f, 255);
								NPC.NewNPC((int)npc.Center.X + spawnDistance * 2, (int)npc.Center.Y - 10, ModContent.NPCType<FlamePillar>(), 0, 0f, 0f, 0f, 0f, 255);
							}
						}
                    }

					if (revenge)
						npc.Calamity().newAI[0] += 1f;

					for (int stompDustArea = (int)npc.position.X - 30; stompDustArea < (int)npc.position.X + npc.width + 60; stompDustArea += 30)
                    {
                        for (int stompDustAmount = 0; stompDustAmount < 6; stompDustAmount++)
                        {
                            int stompDust = Dust.NewDust(new Vector2(npc.position.X - 30f, npc.position.Y + npc.height), npc.width + 30, 4, 31, 0f, 0f, 100, default, 1.5f);
                            Main.dust[stompDust].velocity *= 0.2f;
                        }

                        int stompGore = Gore.NewGore(new Vector2(stompDustArea - 30, npc.position.Y + npc.height - 12f), default, Main.rand.Next(61, 64), 1f);
                        Main.gore[stompGore].velocity *= 0.4f;
                    }
                }
                else
                {
                    npc.TargetClosest(true);

					// Fall through
					if (!player.dead && expertMode)
					{
						if ((player.position.Y > npc.Bottom.Y && npc.velocity.Y > 0f) || (player.position.Y < npc.Bottom.Y && npc.velocity.Y < 0f))
							npc.noTileCollide = true;
						else if ((npc.velocity.Y > 0f && npc.Bottom.Y > Main.player[npc.target].Top.Y) || (Collision.CanHit(npc.position, npc.width, npc.height, Main.player[npc.target].Center, 1, 1) && !Collision.SolidCollision(npc.position, npc.width, npc.height)))
							npc.noTileCollide = false;
					}

					Vector2 targetVector = npc.ai[0] == 2f ? player.Center + player.velocity.SafeNormalize(Vector2.UnitY) * 40f : player.Center;

					if (npc.ai[0] == 2f && npc.ai[1] == 0f)
					{
						float aimY = targetVector.Y - 320f;
						if (npc.Top.Y > aimY)
						{
							if (npc.velocity.Y > velocityY)
								npc.velocity.Y -= 0.2f + Math.Abs(npc.Center.Y - aimY) * 0.001f;
						}
						else
						{
							if (npc.velocity.Y < velocityY)
								npc.velocity.Y += 0.2f + Math.Abs(npc.Center.Y - aimY) * 0.001f;
						}
					}

					Vector2 aimAt = npc.ai[0] == 2f ? targetVector : player.position;

					if ((npc.position.X < aimAt.X && npc.position.X + npc.width > aimAt.X + player.width) || npc.ai[1] > 0f)
                    {
						if (npc.ai[0] == 2f)
						{
							float stopBeforeFallTime = 30f;
							if (expertMode)
								stopBeforeFallTime -= 15f * (1f - lifeRatio);

							if (npc.ai[1] < 30f)
							{
								npc.ai[1] += 1f;
								npc.velocity = Vector2.Zero;
							}
							else if (npc.Bottom.Y < player.position.Y)
							{
								float fallSpeedBoost = death ? 1.2f : 1.2f * (1f - lifeRatio);
								float fallSpeed = 1.2f + fallSpeedBoost;
								npc.velocity.Y += fallSpeed;

								if (npc.velocity.Y > 24f)
									npc.velocity.Y = 24f;

								npc.ai[1] = 31f;
							}
						}
						else
						{
							npc.velocity.X *= 0.9f;

							if (npc.Bottom.Y < player.position.Y)
							{
								float fallSpeedBoost = death ? 0.6f : 0.6f * (1f - lifeRatio);
								float fallSpeed = 0.6f + fallSpeedBoost;
								npc.velocity.Y += fallSpeed;
							}
						}
                    }
                    else
                    {
						float velocityXChange = 0.2f + Math.Abs(npc.Center.X - targetVector.X) * 0.001f;

						if (npc.direction < 0)
                            npc.velocity.X -= velocityXChange;
                        else if (npc.direction > 0)
                            npc.velocity.X += velocityXChange;

						float velocityXBoost = death ? 4f : 4f * (1f - lifeRatio);
                        float velocityX = 8f + velocityXBoost + Math.Abs(npc.Center.X - targetVector.X) * 0.001f;

                        if (npc.Calamity().enraged > 0 || (CalamityConfig.Instance.BossRushXerocCurse && CalamityWorld.bossRushActive))
                            velocityX += 3f;
                        if (!rightClawActive || death)
                            velocityX += 1f;
                        if (!leftClawActive || death)
                            velocityX += 1f;
                        if (!headActive || death)
                            velocityX += 1f;
                        if (!rightLegActive || death)
                            velocityX += 1f;
                        if (!leftLegActive || death)
                            velocityX += 1f;

                        if (npc.velocity.X < -velocityX)
                            npc.velocity.X = -velocityX;
                        if (npc.velocity.X > velocityX)
                            npc.velocity.X = velocityX;
                    }
                }
            }

			player = Main.player[npc.target];
			if (npc.target <= 0 || npc.target == 255 || player.dead || !player.active)
			{
				npc.TargetClosest(true);
				player = Main.player[npc.target];
			}

            int distanceFromTarget = 5600;
            if (Vector2.Distance(npc.Center, player.Center) > distanceFromTarget)
            {
                npc.TargetClosest(true);
				player = Main.player[npc.target];

				if (Vector2.Distance(npc.Center, player.Center) > distanceFromTarget)
                {
                    npc.active = false;
                    npc.netUpdate = true;
                }
            }
        }

        public override void PostDraw(SpriteBatch spriteBatch, Color drawColor)
        {
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (npc.spriteDirection == 1)
            {
                spriteEffects = SpriteEffects.FlipHorizontally;
            }
            Vector2 center = new Vector2(npc.Center.X, npc.Center.Y);
            Vector2 vector11 = new Vector2(Main.npcTexture[npc.type].Width / 2, Main.npcTexture[npc.type].Height / Main.npcFrameCount[npc.type] / 2);
            Vector2 vector = center - Main.screenPosition;
            vector -= new Vector2(ModContent.GetTexture("CalamityMod/NPCs/Ravager/RavagerBodyGlow").Width, ModContent.GetTexture("CalamityMod/NPCs/Ravager/RavagerBodyGlow").Height / Main.npcFrameCount[npc.type]) * 1f / 2f;
            vector += vector11 * 1f + new Vector2(0f, 0f + 4f + npc.gfxOffY);
            Color color = new Color(127 - npc.alpha, 127 - npc.alpha, 127 - npc.alpha, 0).MultiplyRGBA(Color.Blue);
            spriteBatch.Draw(ModContent.GetTexture("CalamityMod/NPCs/Ravager/RavagerBodyGlow"), vector,
                npc.frame, color, npc.rotation, vector11, 1f, spriteEffects, 0f);
            Color color2 = Lighting.GetColor((int)center.X / 16, (int)(center.Y / 16f));
            spriteBatch.Draw(ModContent.GetTexture("CalamityMod/NPCs/Ravager/RavagerLegRight"), new Vector2(center.X - Main.screenPosition.X + 28f, center.Y - Main.screenPosition.Y + 20f), //72
                new Rectangle?(new Rectangle(0, 0, ModContent.GetTexture("CalamityMod/NPCs/Ravager/RavagerLegRight").Width, ModContent.GetTexture("CalamityMod/NPCs/Ravager/RavagerLegRight").Height)),
                color2, 0f, default, 1f, SpriteEffects.None, 0f);
            spriteBatch.Draw(ModContent.GetTexture("CalamityMod/NPCs/Ravager/RavagerLegLeft"), new Vector2(center.X - Main.screenPosition.X - 112f, center.Y - Main.screenPosition.Y + 20f), //72
                new Rectangle?(new Rectangle(0, 0, ModContent.GetTexture("CalamityMod/NPCs/Ravager/RavagerLegLeft").Width, ModContent.GetTexture("CalamityMod/NPCs/Ravager/RavagerLegLeft").Height)),
                color2, 0f, default, 1f, SpriteEffects.None, 0f);
            if (NPC.AnyNPCs(ModContent.NPCType<RavagerHead>()))
            {
                spriteBatch.Draw(ModContent.GetTexture("CalamityMod/NPCs/Ravager/RavagerHead"), new Vector2(center.X - Main.screenPosition.X - 70f, center.Y - Main.screenPosition.Y - 75f),
                    new Rectangle?(new Rectangle(0, 0, ModContent.GetTexture("CalamityMod/NPCs/Ravager/RavagerHead").Width, ModContent.GetTexture("CalamityMod/NPCs/Ravager/RavagerHead").Height)),
                    color2, 0f, default, 1f, SpriteEffects.None, 0f);
            }
        }

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            npc.lifeMax = (int)(npc.lifeMax * 0.8f * bossLifeScale);
            npc.damage = (int)(npc.damage * 0.8f);
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            for (int k = 0; k < 3; k++)
            {
                Dust.NewDust(npc.position, npc.width, npc.height, 5, hitDirection, -1f, 0, default, 2f);
                Dust.NewDust(npc.position, npc.width, npc.height, 6, hitDirection, -1f, 0, default, 1f);
            }
            if (npc.life <= 0)
            {
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/ScavengerGores/ScavengerBody"), 1f);
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/ScavengerGores/ScavengerBody2"), 1f);
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/ScavengerGores/ScavengerBody3"), 1f);
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/ScavengerGores/ScavengerBody4"), 1f);
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/ScavengerGores/ScavengerBody5"), 1f);
                for (int k = 0; k < 50; k++)
                {
                    Dust.NewDust(npc.position, npc.width, npc.height, 5, hitDirection, -1f, 0, default, 2f);
                    Dust.NewDust(npc.position, npc.width, npc.height, 6, hitDirection, -1f, 0, default, 1f);
                }
            }
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            cooldownSlot = 1;
            return true;
        }

        public override void OnHitPlayer(Player player, int damage, bool crit)
        {
            if (CalamityWorld.revenge)
            {
                player.AddBuff(ModContent.BuffType<Horror>(), 600, true);
            }
        }

        public override void BossLoot(ref string name, ref int potionType)
        {
            name = "Ravager";
            potionType = ItemID.GreaterHealingPotion;
        }

        public override void NPCLoot()
        {
            DropHelper.DropBags(npc);

            DropHelper.DropItemChance(npc, ModContent.ItemType<RavagerTrophy>(), 10);
            DropHelper.DropItemCondition(npc, ModContent.ItemType<KnowledgeRavager>(), true, !CalamityWorld.downedScavenger);
            DropHelper.DropResidentEvilAmmo(npc, CalamityWorld.downedScavenger, 4, 2, 1);

			npc.Calamity().SetNewShopVariable(new int[] { NPCID.WitchDoctor }, CalamityWorld.downedScavenger);

			// All other drops are contained in the bag, so they only drop directly on Normal
			if (!Main.expertMode)
            {
                // Materials
                int barMin = CalamityWorld.downedProvidence ? 5 : 1;
                int barMax = CalamityWorld.downedProvidence ? 10 : 3;
                int coreMin = CalamityWorld.downedProvidence ? 1 : 1;
                int coreMax = CalamityWorld.downedProvidence ? 3 : 2;
                DropHelper.DropItemCondition(npc, ModContent.ItemType<Bloodstone>(), CalamityWorld.downedProvidence, 50, 60);
                DropHelper.DropItem(npc, ModContent.ItemType<VerstaltiteBar>(), barMin, barMax);
                DropHelper.DropItem(npc, ModContent.ItemType<DraedonBar>(), barMin, barMax);
                DropHelper.DropItem(npc, ModContent.ItemType<CruptixBar>(), barMin, barMax);
                DropHelper.DropItem(npc, ModContent.ItemType<CoreofCinder>(), coreMin, coreMax);
                DropHelper.DropItem(npc, ModContent.ItemType<CoreofEleum>(), coreMin, coreMax);
                DropHelper.DropItem(npc, ModContent.ItemType<CoreofChaos>(), coreMin, coreMax);
                DropHelper.DropItemCondition(npc, ModContent.ItemType<BarofLife>(), CalamityWorld.downedProvidence, 2, 1, 1);
                DropHelper.DropItemCondition(npc, ModContent.ItemType<CoreofCalamity>(), CalamityWorld.downedProvidence, 3, 1, 1);

                // Weapons
                DropHelper.DropItemChance(npc, ModContent.ItemType<UltimusCleaver>(), 4);
                DropHelper.DropItemChance(npc, ModContent.ItemType<RealmRavager>(), 4);
                DropHelper.DropItemChance(npc, ModContent.ItemType<Hematemesis>(), 4);
                DropHelper.DropItemChance(npc, ModContent.ItemType<SpikecragStaff>(), 4);
                DropHelper.DropItemChance(npc, ModContent.ItemType<CraniumSmasher>(), 4);

                // Equipment
                DropHelper.DropItemChance(npc, ModContent.ItemType<BloodPact>(), 3);
                DropHelper.DropItemChance(npc, ModContent.ItemType<FleshTotem>(), 3);

                // Vanity
                DropHelper.DropItemChance(npc, ModContent.ItemType<RavagerMask>(), 7);
            }

            // Mark Ravager as dead
            CalamityWorld.downedScavenger = true;
            CalamityMod.UpdateServerBoolean();
        }
    }
}
