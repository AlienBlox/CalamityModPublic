using CalamityMod.Events;
using CalamityMod.Items.TreasureBags;
using CalamityMod.Projectiles.Boss;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.SlimeGod
{
	[AutoloadBossHead]
    public class SlimeGodRunSplit : ModNPC
    {
        private float bossLife;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Crimulan Slime God");
            Main.npcFrameCount[npc.type] = 6;
        }

        public override void SetDefaults()
        {
			npc.Calamity().canBreakPlayerDefense = true;
			npc.LifeMaxNERB(1005, 1205, 80000);
            double HPBoost = CalamityConfig.Instance.BossHealthBoost * 0.01;
            npc.lifeMax += (int)(npc.lifeMax * HPBoost);
			npc.GetNPCDamage();
			npc.width = 150;
            npc.height = 92;
            npc.scale = 0.8f;
            npc.defense = 10;
            npc.knockBackResist = 0f;
            animationType = NPCID.KingSlime;
            npc.value = Item.buyPrice(0, 1, 0, 0);
            npc.alpha = 55;
            npc.lavaImmune = false;
            npc.noGravity = false;
            npc.noTileCollide = false;
            npc.HitSound = SoundID.NPCHit1;
            npc.DeathSound = SoundID.NPCDeath1;
            music = CalamityMod.Instance.GetMusicFromMusicMod("SlimeGod") ?? MusicID.Boss1;
            npc.aiStyle = -1;
            aiType = -1;
            bossBag = ModContent.ItemType<SlimeGodBag>();
			npc.Calamity().VulnerableToHeat = true;
			npc.Calamity().VulnerableToSickness = false;
		}

		public override void SendExtraAI(BinaryWriter writer)
		{
			writer.Write(npc.localAI[1]);
		}

		public override void ReceiveExtraAI(BinaryReader reader)
		{
			npc.localAI[1] = reader.ReadSingle();
		}

		public override void AI()
        {
			CalamityGlobalNPC calamityGlobalNPC = npc.Calamity();

			if (CalamityGlobalNPC.slimeGodRed < 0 || !Main.npc[CalamityGlobalNPC.slimeGodRed].active)
				CalamityGlobalNPC.slimeGodRed = npc.whoAmI;

			bool enraged = calamityGlobalNPC.enraged > 0;
			bool malice = CalamityWorld.malice || BossRushEvent.BossRushActive || enraged;
			bool expertMode = Main.expertMode || malice;
            bool revenge = CalamityWorld.revenge || malice;
			bool death = CalamityWorld.death || npc.localAI[1] == 1f || malice;
            npc.Calamity().CurrentlyEnraged = (!BossRushEvent.BossRushActive && malice) || enraged;

            Vector2 vector = npc.Center;

			float lifeRatio = npc.life / (float)npc.lifeMax;

			if (revenge)
			{
				// Increase aggression if player is taking a long time to kill the boss
				if (lifeRatio > calamityGlobalNPC.killTimeRatio_IncreasedAggression)
					lifeRatio = calamityGlobalNPC.killTimeRatio_IncreasedAggression;
			}

			npc.defense = npc.defDefense;
			npc.damage = npc.defDamage;
			if (npc.localAI[1] == 1f)
			{
				npc.defense = npc.defDefense + 20;
				npc.damage = npc.defDamage + 22;
			}

			npc.aiAction = 0;
			npc.noTileCollide = false;
			npc.noGravity = false;

			// Get a target
			if (npc.target < 0 || npc.target == Main.maxPlayers || Main.player[npc.target].dead || !Main.player[npc.target].active)
				npc.TargetClosest();

			// Despawn safety, make sure to target another player if the current player target is too far away
			if (Vector2.Distance(Main.player[npc.target].Center, vector) > CalamityGlobalNPC.CatchUpDistance200Tiles)
				npc.TargetClosest();

			Player player = Main.player[npc.target];

			if (npc.ai[0] != 6f && (player.dead || !player.active))
			{
				npc.TargetClosest();
				player = Main.player[npc.target];
				if (player.dead || !player.active)
				{
					npc.ai[0] = 6f;
					npc.ai[1] = 0f;
					npc.ai[2] = 0f;
					npc.ai[3] = 0f;
                    npc.netUpdate = true;
                }
			}
			else if (npc.timeLeft < 1800)
				npc.timeLeft = 1800;

			float distanceSpeedBoost = Vector2.Distance(player.Center, vector) * (malice ? 0.008f : 0.005f);

			bool flag100 = false;
            bool hyperMode = npc.localAI[1] == 1f;
            if (CalamityGlobalNPC.slimeGodPurple != -1)
            {
                if (Main.npc[CalamityGlobalNPC.slimeGodPurple].active)
                {
                    flag100 = true;
                }
            }
            if (CalamityGlobalNPC.slimeGod < 0 || !Main.npc[CalamityGlobalNPC.slimeGod].active)
            {
				npc.localAI[1] = 0f;
				hyperMode = true;
                flag100 = false;
            }
			if (malice)
			{
				flag100 = false;
				hyperMode = true;
			}

			if (npc.localAI[1] != 1f)
			{
				if (!flag100)
					npc.defense = npc.defDefense * 2;
			}

			if (Main.netMode != NetmodeID.MultiplayerClient)
			{
				npc.localAI[0] += flag100 ? 0.5f : 1f;
				if (revenge)
					npc.localAI[0] += 0.5f;
				if (malice)
					npc.localAI[0] += 1f;

				if (npc.localAI[0] >= 450f && Vector2.Distance(player.Center, npc.Center) > 160f)
				{
					npc.localAI[0] = 0f;
					float num179 = expertMode ? 12f : 10f;
					Vector2 value9 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
					float num180 = player.position.X + (float)player.width * 0.5f - value9.X;
					float num181 = Math.Abs(num180) * 0.1f;
					float num182 = player.position.Y + (float)player.height * 0.5f - value9.Y - num181;
					float num183 = (float)Math.Sqrt((double)(num180 * num180 + num182 * num182));
					num183 = num179 / num183;
					num180 *= num183;
					num182 *= num183;
					int type = ModContent.ProjectileType<AbyssBallVolley2>();
					int damage = npc.GetProjectileDamage(type);
					value9.X += num180;
					value9.Y += num182;
					num180 = player.position.X + (float)player.width * 0.5f - value9.X;
					num182 = player.position.Y + (float)player.height * 0.5f - value9.Y;
					num183 = (float)Math.Sqrt((double)(num180 * num180 + num182 * num182));
					num183 = num179 / num183;
					num180 *= num183;
					num182 *= num183;
					Projectile.NewProjectile(value9.X, value9.Y, num180, num182, type, damage, 0f, Main.myPlayer, 0f, 0f);
				}
			}

            if (npc.ai[0] == 0f)
            {
                npc.ai[0] = 1f;
                npc.ai[1] = 0f;
                npc.netUpdate = true;
            }
            else if (npc.ai[0] == 1f)
            {
                if ((player.Center - vector).Length() > (hyperMode ? 1200f : 3600f))
                {
                    npc.ai[0] = 4f;
                    npc.ai[1] = 0f;
                    npc.ai[2] = 0f;
                    npc.ai[3] = 0f;
                    npc.netUpdate = true;
                }
                if (npc.velocity.Y == 0f)
                {
                    npc.TargetClosest();
                    npc.velocity.X *= 0.8f;
                    npc.ai[1] += 1f;
                    float num1879 = 35f;
                    float num1880 = death ? 5.5f : revenge ? 5f : expertMode ? 4.5f : 4f;
					if (revenge)
					{
						float moveBoost = death ? 14f * (1f - lifeRatio) : 7f * (1f - lifeRatio);
						float speedBoost = death ? 4f * (1f - lifeRatio) : 2f * (1f - lifeRatio);
						num1879 -= moveBoost;
						num1880 += speedBoost;
					}
					float num1881 = 4f;
                    if (!Collision.CanHit(vector, 1, 1, player.Center, 1, 1))
                    {
                        num1881 += 2f;
                    }
                    if (npc.ai[1] > num1879)
                    {
                        num1881 *= 0.75f;
                        num1880 *= 1.25f;
                        npc.ai[1] = 0f;
                        npc.velocity.Y -= num1881;
						npc.velocity.X = (num1880 + distanceSpeedBoost) * npc.direction;
                        npc.netUpdate = true;
                    }
                }
                else
                {
                    npc.velocity.X *= 0.99f;
                    if (npc.direction < 0 && npc.velocity.X > -1f)
                    {
                        npc.velocity.X = -1f;
                    }
                    if (npc.direction > 0 && npc.velocity.X < 1f)
                    {
                        npc.velocity.X = 1f;
                    }
                }
                npc.ai[2] += 1f;
				if (revenge)
				{
					npc.ai[2] += death ? 1f - lifeRatio : 0.5f * (1f - lifeRatio);
				}
				if (npc.ai[2] >= 180f && npc.velocity.Y == 0f && Main.netMode != NetmodeID.MultiplayerClient)
                {
					int random = Main.rand.Next(2) + (lifeRatio < 0.5f ? 1 : 0);
					switch (random)
					{
						case 0:
							npc.ai[0] = 2f;
							break;
						case 1:
							npc.ai[0] = 3f;
							npc.noTileCollide = true;
							npc.velocity.Y = death ? -11f : revenge ? -10f : expertMode ? -9f : -8f;
							break;
						case 2:
							npc.ai[0] = 5f;
							break;
						default:
							break;
					}
					npc.ai[1] = 0f;
                    npc.ai[2] = 0f;
                    npc.ai[3] = 0f;
                    npc.netUpdate = true;
                }
            }
            else if (npc.ai[0] == 2f)
            {
                npc.velocity.X *= 0.85f;
                npc.ai[1] += 1f;
                if (npc.ai[1] >= 30f)
                {
                    npc.ai[0] = 1f;
                    npc.ai[1] = 0f;
                    npc.netUpdate = true;
                }
            }
            else if (npc.ai[0] == 3f)
            {
                npc.noTileCollide = true;
                npc.noGravity = true;
                if (npc.velocity.X < 0f)
                {
                    npc.direction = -1;
                }
                else
                {
                    npc.direction = 1;
                }
                npc.spriteDirection = npc.direction;
                npc.TargetClosest();
                Vector2 center40 = player.Center;
                center40.Y -= 350f;
                Vector2 vector272 = center40 - vector;
                if (npc.ai[2] == 1f)
                {
                    npc.ai[1] += 1f;
                    vector272 = player.Center - vector;
                    vector272.Normalize();
                    vector272 *= death ? 11f : revenge ? 10f : expertMode ? 9f : 8f;
                    npc.velocity = (npc.velocity * 4f + vector272) / 5f;
                    if (npc.ai[1] > 12f)
                    {
                        npc.ai[1] = 0f;
                        npc.ai[0] = 3.1f;
                        npc.ai[2] = 0f;
                        npc.velocity = vector272;
                        npc.netUpdate = true;
                    }
                }
                else
                {
                    if (Math.Abs(vector.X - player.Center.X) < 40f && vector.Y < player.Center.Y - 300f)
                    {
                        npc.ai[1] = 0f;
                        npc.ai[2] = 1f;
                        return;
                    }
                    vector272.Normalize();
					vector272 *= (death ? 13f : revenge ? 12f : expertMode ? 11f : 10f) + distanceSpeedBoost;
					npc.velocity = (npc.velocity * 5f + vector272) / 6f;
                }
            }
            else if (npc.ai[0] == 3.1f)
            {
				bool atTargetPosition = npc.position.Y + npc.height >= player.position.Y;
				if (npc.ai[2] == 0f && (atTargetPosition || npc.localAI[1] == 0f) && Collision.CanHit(vector, 1, 1, player.Center, 1, 1) && !Collision.SolidCollision(npc.position, npc.width, npc.height))
                {
                    npc.ai[2] = 1f;
                }
                if (atTargetPosition || npc.velocity.Y <= 0f)
                {
                    npc.ai[1] += 1f;
                    if (npc.ai[1] > 10f)
                    {
                        npc.ai[0] = 1f;
                        npc.ai[1] = 0f;
                        npc.ai[2] = 0f;
                        npc.ai[3] = 0f;
                        if (Collision.SolidCollision(npc.position, npc.width, npc.height))
                            npc.ai[0] = 4f;

                        npc.netUpdate = true;
                    }
                }
                else if (npc.ai[2] == 0f)
                {
                    npc.noTileCollide = true;
                }
				npc.noGravity = true;
				npc.velocity.Y += 0.5f;
				float velocityLimit = malice ? 22f : death ? 16f : revenge ? 15f : expertMode ? 14f : 13f;
				if (npc.velocity.Y > velocityLimit)
                {
                    npc.velocity.Y = velocityLimit;
                }
            }
            else
            {
                if (npc.ai[0] == 4f)
                {
                    if (npc.velocity.X > 0f)
                    {
                        npc.direction = 1;
                    }
                    else
                    {
                        npc.direction = -1;
                    }
                    npc.spriteDirection = npc.direction;
                    npc.noTileCollide = true;
                    npc.noGravity = true;
                    Vector2 value74 = player.Center - vector;
                    value74.Y -= 40f;
                    if (value74.Length() < 280f && !Collision.SolidCollision(npc.position, npc.width, npc.height))
                    {
                        npc.ai[0] = 1f;
                        npc.ai[1] = 0f;
                        npc.ai[2] = 0f;
                        npc.ai[3] = 0f;
                        npc.netUpdate = true;
                    }
                    if (value74.Length() > 50f)
                    {
                        value74.Normalize();
						value74 *= (death ? 13f : revenge ? 12f : expertMode ? 11f : 10f) + distanceSpeedBoost;
					}
                    npc.velocity = (npc.velocity * 4f + value74) / 5f;
                    return;
                }
                if (npc.ai[0] == 5f)
                {
                    if (npc.velocity.Y == 0f)
                    {
                        npc.TargetClosest();
                        npc.velocity.X *= 0.8f;
                        npc.ai[1] += 1f;
                        if (npc.ai[1] > 5f)
                        {
                            npc.ai[1] = 0f;
                            npc.velocity.Y -= 4f;
                            if (player.position.Y + player.height < vector.Y)
                            {
                                npc.velocity.Y -= 1.25f;
                            }
                            if (player.position.Y + player.height < vector.Y - 40f)
                            {
                                npc.velocity.Y -= 1.5f;
                            }
                            if (player.position.Y + player.height < vector.Y - 80f)
                            {
                                npc.velocity.Y -= 1.75f;
                            }
                            if (player.position.Y + player.height < vector.Y - 120f)
                            {
                                npc.velocity.Y -= 2f;
                            }
                            if (player.position.Y + player.height < vector.Y - 160f)
                            {
                                npc.velocity.Y -= 2.25f;
                            }
                            if (player.position.Y + player.height < vector.Y - 200f)
                            {
                                npc.velocity.Y -= 2.5f;
                            }
                            if (!Collision.CanHit(vector, 1, 1, player.Center, 1, 1))
                            {
                                npc.velocity.Y -= 2f;
                            }
							npc.velocity.X = ((death ? 12f : revenge ? 11f : expertMode ? 10f : 9f) + distanceSpeedBoost) * npc.direction;
							npc.ai[2] += 1f;
                        }
                    }
                    else
                    {
                        npc.velocity.X *= 0.98f;
						float velocityLimit = (death ? 7f : revenge ? 6f : expertMode ? 5f : 4f) + distanceSpeedBoost;
						if (npc.direction < 0 && npc.velocity.X > -velocityLimit)
                        {
                            npc.velocity.X = -velocityLimit;
                        }
                        if (npc.direction > 0 && npc.velocity.X < velocityLimit)
                        {
                            npc.velocity.X = velocityLimit;
                        }
                    }
                    if (npc.ai[2] >= 3f && npc.velocity.Y == 0f)
                    {
                        npc.ai[0] = 1f;
                        npc.ai[1] = 0f;
                        npc.ai[2] = 0f;
                        npc.ai[3] = 0f;
                        npc.netUpdate = true;
                    }
                }
                else if (npc.ai[0] == 6f)
                {
                    npc.life = npc.lifeMax;
                    npc.defense = 9999;
                    npc.noTileCollide = true;
                    npc.alpha += 7;
                    if (npc.timeLeft > 10)
                    {
                        npc.timeLeft = 10;
                    }
                    if (npc.alpha > 255)
                    {
                        npc.alpha = 255;
                    }
                    npc.velocity.X *= 0.98f;
                }
            }
            int num658 = Dust.NewDust(npc.position, npc.width, npc.height, 260, npc.velocity.X, npc.velocity.Y, 255, new Color(0, 80, 255, 80), npc.scale * 1.5f);
            Main.dust[num658].noGravity = true;
            Main.dust[num658].velocity *= 0.5f;
            if (bossLife == 0f && npc.life > 0)
            {
                bossLife = npc.lifeMax;
            }
            float num644 = 1f;
            if (npc.life > 0)
            {
                float num659 = lifeRatio;
                num659 = num659 * 0.5f + 0.75f;
                num659 *= num644;
                if (num659 != npc.scale)
                {
                    npc.position.X = npc.position.X + (float)(npc.width / 2);
                    npc.position.Y = npc.position.Y + (float)npc.height;
                    npc.scale = num659 * 0.75f;
                    npc.width = (int)(150f * npc.scale);
                    npc.height = (int)(92f * npc.scale);
                    npc.position.X = npc.position.X - (float)(npc.width / 2);
                    npc.position.Y = npc.position.Y - (float)npc.height;
                }
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    int num660 = (int)((double)npc.lifeMax * 0.15);
					if ((float)(npc.life + num660) < bossLife)
					{
						bossLife = (float)npc.life;
						int x = (int)(npc.position.X + (float)Main.rand.Next(npc.width - 32));
						int y = (int)(npc.position.Y + (float)Main.rand.Next(npc.height - 32));
						int num663 = ModContent.NPCType<SlimeSpawnCrimson>();
						if (Main.rand.NextBool(3))
						{
							num663 = ModContent.NPCType<SlimeSpawnCrimson2>();
						}
						int num664 = NPC.NewNPC(x, y, num663, 0, 0f, 0f, 0f, 0f, 255);
						Main.npc[num664].SetDefaults(num663, -1f);
						Main.npc[num664].velocity.X = (float)Main.rand.Next(-15, 16) * 0.1f;
						Main.npc[num664].velocity.Y = (float)Main.rand.Next(-30, 1) * 0.1f;
						Main.npc[num664].ai[0] = (float)(-1000 * Main.rand.Next(3));
						Main.npc[num664].ai[1] = 0f;
						if (Main.netMode == NetmodeID.Server && num664 < 200)
						{
							NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, num664, 0f, 0f, 0f, 0, 0, 0);
						}
					}
                }
            }
        }

		public override Color? GetAlpha(Color drawColor)
		{
			Color lightColor = new Color(Main.DiscoR, 100, 150, npc.alpha);
			Color newColor = npc.localAI[1] == 1f ? lightColor : drawColor;
			return newColor;
		}

		public override void NPCLoot()
        {
			if (!CalamityWorld.malice && !CalamityWorld.revenge)
			{
				int heartAmt = Main.rand.Next(3) + 3;
				for (int i = 0; i < heartAmt; i++)
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ItemID.Heart);
			}

			bool otherSlimeGodsAlive =
                NPC.AnyNPCs(ModContent.NPCType<SlimeGodCore>()) ||
                NPC.AnyNPCs(ModContent.NPCType<SlimeGod>()) ||
                NPC.AnyNPCs(ModContent.NPCType<SlimeGodSplit>()) ||
                NPC.AnyNPCs(ModContent.NPCType<SlimeGodRun>()) ||
                NPC.CountNPCS(ModContent.NPCType<SlimeGodRunSplit>()) > 1; // the other crimulan split is alive
            if (!otherSlimeGodsAlive)
                SlimeGodCore.DropSlimeGodLoot(npc);
        }

        public override bool CheckActive()
        {
            if (CalamityGlobalNPC.slimeGod != -1)
            {
                if (Main.npc[CalamityGlobalNPC.slimeGod].active)
                    return false;
            }
            return true;
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(npc.position, npc.width, npc.height, 4, hitDirection, -1f, 0, default, 1f);
            }
            if (npc.life <= 0)
            {
                npc.position.X = npc.position.X + (float)(npc.width / 2);
                npc.position.Y = npc.position.Y + (float)(npc.height / 2);
                npc.width = 50;
                npc.height = 50;
                npc.position.X = npc.position.X - (float)(npc.width / 2);
                npc.position.Y = npc.position.Y - (float)(npc.height / 2);
                for (int num621 = 0; num621 < 40; num621++)
                {
                    int num622 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 4, 0f, 0f, 100, default, 2f);
                    Main.dust[num622].velocity *= 3f;
                    if (Main.rand.NextBool(2))
                    {
                        Main.dust[num622].scale = 0.5f;
                        Main.dust[num622].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                    }
                }
                for (int num623 = 0; num623 < 70; num623++)
                {
                    int num624 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 4, 0f, 0f, 100, default, 3f);
                    Main.dust[num624].noGravity = true;
                    Main.dust[num624].velocity *= 5f;
                    num624 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 4, 0f, 0f, 100, default, 2f);
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
			player.AddBuff(BuffID.Darkness, 120, true);
		}
    }
}
