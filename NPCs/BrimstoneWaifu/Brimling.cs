﻿using System;
using System.IO;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.World;

namespace CalamityMod.NPCs.BrimstoneWaifu
{
    public class Brimling : ModNPC
	{
		private bool boostDR = false;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Brimling");
			Main.npcFrameCount[npc.type] = 8;
		}

		public override void SetDefaults()
		{
			npc.aiStyle = -1;
			aiType = -1;
			npc.damage = 0;
			npc.width = 60;
			npc.height = 60;
			npc.lifeMax = 4000;
			if (CalamityWorld.death)
			{
				npc.lifeMax = 6000;
			}
			npc.knockBackResist = 0f;
			for (int k = 0; k < npc.buffImmune.Length; k++)
			{
				npc.buffImmune[k] = true;
			}
			npc.buffImmune[BuffID.Ichor] = false;
			npc.buffImmune[mod.BuffType("MarkedforDeath")] = false;
			npc.buffImmune[BuffID.CursedInferno] = false;
			npc.buffImmune[BuffID.Daybreak] = false;
			npc.buffImmune[mod.BuffType("AbyssalFlames")] = false;
			npc.buffImmune[mod.BuffType("ArmorCrunch")] = false;
			npc.buffImmune[mod.BuffType("DemonFlames")] = false;
			npc.buffImmune[mod.BuffType("GodSlayerInferno")] = false;
			npc.buffImmune[mod.BuffType("HolyLight")] = false;
			npc.buffImmune[mod.BuffType("Nightwither")] = false;
			npc.buffImmune[mod.BuffType("Plague")] = false;
			npc.buffImmune[mod.BuffType("Shred")] = false;
			npc.buffImmune[mod.BuffType("WhisperingDeath")] = false;
			npc.buffImmune[mod.BuffType("SilvaStun")] = false;
			npc.noGravity = true;
			npc.noTileCollide = true;
			npc.canGhostHeal = false;
			npc.HitSound = SoundID.NPCHit23;
			npc.DeathSound = SoundID.NPCDeath39;
			if (CalamityWorld.downedProvidence)
			{
				npc.lifeMax = 40000;
			}
			if (CalamityWorld.bossRushActive)
			{
				npc.lifeMax = 100000;
			}
		}

		public override void SendExtraAI(BinaryWriter writer)
		{
			writer.Write(boostDR);
			writer.Write(npc.chaseable);
		}

		public override void ReceiveExtraAI(BinaryReader reader)
		{
			boostDR = reader.ReadBoolean();
			npc.chaseable = reader.ReadBoolean();
		}

		public override void AI()
		{
			Lighting.AddLight((int)((npc.position.X + (float)(npc.width / 2)) / 16f), (int)((npc.position.Y + (float)(npc.height / 2)) / 16f), 1f, 0f, 0f);
			if (CalamityGlobalNPC.brimstoneElemental < 0 || !Main.npc[CalamityGlobalNPC.brimstoneElemental].active)
			{
				npc.active = false;
				npc.netUpdate = true;
				return;
			}
			bool goIntoShell = (double)npc.life <= (double)npc.lifeMax * 0.1;
			bool provy = (CalamityWorld.downedProvidence && !CalamityWorld.bossRushActive);
			if (goIntoShell || Main.npc[CalamityGlobalNPC.brimstoneElemental].ai[0] == 4f)
			{
				boostDR = true;
				npc.chaseable = false;
			}
			else
			{
				boostDR = false;
				npc.chaseable = true;
			}
			float num1446 = goIntoShell ? 1f : (CalamityWorld.bossRushActive ? 12f : 6f);
			int num1447 = 480;
			float num244;
			if (npc.localAI[1] == 1f)
			{
				npc.localAI[1] = 0f;
				if (Main.rand.NextBool(4))
				{
					npc.ai[0] = (float)num1447;
				}
			}
			npc.TargetClosest(true);
			npc.rotation = Math.Abs(npc.velocity.X) * (float)npc.direction * 0.1f;
			npc.spriteDirection = ((npc.direction > 0) ? 1 : -1);
			Vector2 value53 = npc.Center + new Vector2((float)(npc.direction * 20), 6f);
			Vector2 vector251 = Main.player[npc.target].Center - value53;
			bool flag104 = Collision.CanHit(npc.Center, 1, 1, Main.player[npc.target].Center, 1, 1);
			npc.localAI[0] += 1f;
			if (Main.netMode != NetmodeID.MultiplayerClient && npc.localAI[0] >= 300f && Main.npc[CalamityGlobalNPC.brimstoneElemental].ai[0] != 4f)
			{
				npc.localAI[0] = 0f;
				if (Collision.CanHit(npc.position, npc.width, npc.height, Main.player[npc.target].position, Main.player[npc.target].width, Main.player[npc.target].height))
				{
					float speed = CalamityWorld.bossRushActive ? 7.5f : 5f;
					Vector2 vector = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)(npc.height / 2));
					float num6 = Main.player[npc.target].position.X + (float)Main.player[npc.target].width * 0.5f - vector.X + (float)Main.rand.Next(-10, 11);
					float num7 = Main.player[npc.target].position.Y + (float)Main.player[npc.target].height * 0.5f - vector.Y + (float)Main.rand.Next(-10, 11);
					float num8 = (float)Math.Sqrt((double)(num6 * num6 + num7 * num7));
					num8 = speed / num8;
					num6 *= num8;
					num7 *= num8;
					int projectileDamage = Main.expertMode ? 28 : 35;
					Projectile.NewProjectile(npc.Center.X, npc.Center.Y, num6, num7, mod.ProjectileType("BrimstoneHellfireball"), projectileDamage + (provy ? 30 : 0), 0f, Main.myPlayer, 0f, 0f);
				}
			}
			if (vector251.Length() > 400f || !flag104)
			{
				Vector2 value54 = vector251;
				if (value54.Length() > num1446)
				{
					value54.Normalize();
					value54 *= num1446;
				}
				int num1448 = 30;
				npc.velocity = (npc.velocity * (float)(num1448 - 1) + value54) / (float)num1448;
			}
			else
			{
				npc.velocity *= 0.98f;
			}
			if (npc.ai[2] != 0f && npc.ai[3] != 0f)
			{
				Main.PlaySound(SoundID.Item8, npc.Center);
				int num;
				for (int num1449 = 0; num1449 < 20; num1449 = num + 1)
				{
					int num1450 = Dust.NewDust(npc.position, npc.width, npc.height, 235, 0f, 0f, 100, Color.Transparent, 1f);
					Dust dust = Main.dust[num1450];
					dust.velocity *= 3f;
					Main.dust[num1450].noGravity = true;
					Main.dust[num1450].scale = 2.5f;
					num = num1449;
				}
				npc.Center = new Vector2(npc.ai[2] * 16f, npc.ai[3] * 16f);
				npc.velocity = Vector2.Zero;
				npc.ai[2] = 0f;
				npc.ai[3] = 0f;
				Main.PlaySound(SoundID.Item8, npc.Center);
				for (int num1451 = 0; num1451 < 20; num1451 = num + 1)
				{
					int num1452 = Dust.NewDust(npc.position, npc.width, npc.height, 235, 0f, 0f, 100, Color.Transparent, 1f);
					Dust dust = Main.dust[num1452];
					dust.velocity *= 3f;
					Main.dust[num1452].noGravity = true;
					Main.dust[num1452].scale = 2.5f;
					num = num1451;
				}
			}
			float[] var_9_48E3C_cp_0 = npc.ai;
			int var_9_48E3C_cp_1 = 0;
			num244 = var_9_48E3C_cp_0[var_9_48E3C_cp_1];
			var_9_48E3C_cp_0[var_9_48E3C_cp_1] = num244 + 1f;
			if (npc.ai[0] >= (float)num1447 && Main.netMode != NetmodeID.MultiplayerClient)
			{
				if (npc.localAI[0] > 260f)
				{
					npc.localAI[0] -= 60f;
				}
				npc.ai[0] = 0f;
				Point point12 = npc.Center.ToTileCoordinates();
				Point point13 = Main.player[npc.target].Center.ToTileCoordinates();
				int num1453 = 20;
				int num1454 = 3;
				int num1455 = 10;
				int num1456 = 1;
				int num1457 = 0;
				bool flag106 = false;
				if (vector251.Length() > 2000f)
				{
					flag106 = true;
				}
				while (!flag106 && num1457 < 100)
				{
					num1457++;
					int num1458 = Main.rand.Next(point13.X - num1453, point13.X + num1453 + 1);
					int num1459 = Main.rand.Next(point13.Y - num1453, point13.Y + num1453 + 1);
					if ((num1459 < point13.Y - num1455 || num1459 > point13.Y + num1455 || num1458 < point13.X - num1455 || num1458 > point13.X + num1455) && (num1459 < point12.Y - num1454 || num1459 > point12.Y + num1454 || num1458 < point12.X - num1454 || num1458 > point12.X + num1454) && !Main.tile[num1458, num1459].nactive())
					{
						bool flag107 = true;
						if (flag107 && Main.tile[num1458, num1459].lava())
						{
							flag107 = false;
						}
						if (flag107 && Collision.SolidTiles(num1458 - num1456, num1458 + num1456, num1459 - num1456, num1459 + num1456))
						{
							flag107 = false;
						}
						if (flag107)
						{
							npc.ai[2] = (float)num1458;
							npc.ai[3] = (float)num1459;
							break;
						}
					}
				}
				npc.netUpdate = true;
			}
		}

		public override bool StrikeNPC(ref double damage, int defense, ref float knockback, int hitDirection, ref bool crit)
		{
			double multiplier = 0.8;
			if (boostDR)
			{
				multiplier = 0.2;
			}
			damage = (int)((double)damage * multiplier);
			return true;
		}

		public override void FindFrame(int frameHeight)
		{
			npc.frameCounter += 1.0;
			if (!boostDR)
			{
				if (npc.frameCounter > 12.0)
				{
					npc.frame.Y = npc.frame.Y + frameHeight;
					npc.frameCounter = 0.0;
				}
				if (npc.frame.Y >= frameHeight * 4)
				{
					npc.frame.Y = 0;
				}
			}
			else
			{
				if (npc.frameCounter > 12.0)
				{
					npc.frame.Y = npc.frame.Y + frameHeight;
					npc.frameCounter = 0.0;
				}
				if (npc.frame.Y < frameHeight * 4)
				{
					npc.frame.Y = frameHeight * 4;
				}
				if (npc.frame.Y >= frameHeight * 8)
				{
					npc.frame.Y = frameHeight * 4;
				}
			}
		}

		public override void HitEffect(int hitDirection, double damage)
		{
			for (int k = 0; k < 5; k++)
			{
				Dust.NewDust(npc.position, npc.width, npc.height, 235, hitDirection, -1f, 0, default, 1f);
			}
			if (npc.life <= 0)
			{
				for (int k = 0; k < 20; k++)
				{
					Dust.NewDust(npc.position, npc.width, npc.height, 235, hitDirection, -1f, 0, default, 1f);
				}
			}
		}
	}
}
