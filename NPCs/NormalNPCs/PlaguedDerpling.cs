﻿using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Projectiles;

namespace CalamityMod.NPCs.NormalNPCs
{
	public class PlaguedDerpling : ModNPC
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Viruling");
			Main.npcFrameCount[npc.type] = 5;
		}
		
		public override void SetDefaults()
		{
            npc.noGravity = true;
            npc.aiStyle = -1;
            aiType = -1;
			npc.damage = 60;
			npc.width = 58; //324
			npc.height = 44; //216
			npc.defense = 38;
			npc.lifeMax = 400;
			npc.knockBackResist = 0f;
			npc.value = Item.buyPrice(0, 0, 10, 0);
			npc.HitSound = SoundID.NPCHit22;
			npc.DeathSound = SoundID.NPCDeath25;
            npc.buffImmune[189] = true;
            npc.buffImmune[153] = true;
            npc.buffImmune[70] = true;
            npc.buffImmune[69] = true;
            npc.buffImmune[44] = true;
            npc.buffImmune[39] = true;
            npc.buffImmune[24] = true;
            npc.buffImmune[20] = true;
            npc.buffImmune[mod.BuffType("BrimstoneFlames")] = true;
            npc.buffImmune[mod.BuffType("HolyLight")] = true;
            npc.buffImmune[mod.BuffType("Plague")] = true;
			banner = npc.type;
			bannerItem = mod.ItemType("VirulingBanner");
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
            if (npc.target < 0 || npc.target == 255 || Main.player[npc.target].dead)
            {
                npc.TargetClosest(true);
            }
            float num = 6f; //2
            float num2 = 0.05f;
            Vector2 vector = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
            float num4 = Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2);
            float num5 = Main.player[npc.target].position.Y + (float)(Main.player[npc.target].height / 2);
            num4 = (float)((int)(num4 / 8f) * 8);
            num5 = (float)((int)(num5 / 8f) * 8);
            vector.X = (float)((int)(vector.X / 8f) * 8);
            vector.Y = (float)((int)(vector.Y / 8f) * 8);
            num4 -= vector.X;
            num5 -= vector.Y;
            float num6 = (float)Math.Sqrt((double)(num4 * num4 + num5 * num5));
            float num7 = num6;
            if (num6 == 0f)
            {
                num4 = npc.velocity.X;
                num5 = npc.velocity.Y;
            }
            else
            {
                num6 = num / num6;
                num4 *= num6;
                num5 *= num6;
            }
            if (Main.player[npc.target].dead)
            {
                num4 = (float)npc.direction * num / 2f;
                num5 = -num / 2f;
            }
            if (npc.velocity.X < num4)
            {
                npc.velocity.X = npc.velocity.X + num2;
            }
            else if (npc.velocity.X > num4)
            {
                npc.velocity.X = npc.velocity.X - num2;
            }
            if (npc.velocity.Y < num5)
            {
                npc.velocity.Y = npc.velocity.Y + num2;
            }
            else if (npc.velocity.Y > num5)
            {
                npc.velocity.Y = npc.velocity.Y - num2;
            }
            if (num4 > 0f)
            {
                npc.spriteDirection = 1;
                npc.rotation = (float)Math.Atan2((double)num5, (double)num4);
            }
            else if (num4 < 0f)
            {
                npc.spriteDirection = -1;
                npc.rotation = (float)Math.Atan2((double)num5, (double)num4) + 3.14f;
            }
            float num12 = 0.7f;
            if (npc.collideX)
            {
                npc.netUpdate = true;
                npc.velocity.X = npc.oldVelocity.X * -num12;
                if (npc.direction == -1 && npc.velocity.X > 0f && npc.velocity.X < 2f)
                {
                    npc.velocity.X = 2f;
                }
                if (npc.direction == 1 && npc.velocity.X < 0f && npc.velocity.X > -2f)
                {
                    npc.velocity.X = -2f;
                }
            }
            if (npc.collideY)
            {
                npc.netUpdate = true;
                npc.velocity.Y = npc.oldVelocity.Y * -num12;
                if (npc.velocity.Y > 0f && (double)npc.velocity.Y < 1.5)
                {
                    npc.velocity.Y = 2f;
                }
                if (npc.velocity.Y < 0f && (double)npc.velocity.Y > -1.5)
                {
                    npc.velocity.Y = -2f;
                }
            }
            if (((npc.velocity.X > 0f && npc.oldVelocity.X < 0f) || (npc.velocity.X < 0f && npc.oldVelocity.X > 0f) || (npc.velocity.Y > 0f && npc.oldVelocity.Y < 0f) || (npc.velocity.Y < 0f && npc.oldVelocity.Y > 0f)) && !npc.justHit)
            {
                npc.netUpdate = true;
            }
            int num13 = Dust.NewDust(new Vector2(npc.position.X - npc.velocity.X, npc.position.Y - npc.velocity.Y), npc.width, npc.height, 46, npc.velocity.X * 0.2f, npc.velocity.Y * 0.2f, 100, default(Color), 2f);
            Main.dust[num13].noGravity = true;
            Dust expr_F26_cp_0 = Main.dust[num13];
            expr_F26_cp_0.velocity.X = expr_F26_cp_0.velocity.X * 0.3f;
            Dust expr_F44_cp_0 = Main.dust[num13];
            expr_F44_cp_0.velocity.Y = expr_F44_cp_0.velocity.Y * 0.3f;
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
		{
            if (spawnInfo.playerSafe || !NPC.downedGolemBoss || spawnInfo.player.GetModPlayer<CalamityPlayer>(mod).ZoneSunkenSea)
            {
                return 0f;
            }
            return SpawnCondition.HardmodeJungle.Chance * 0.09f;
        }
		
		public override void OnHitPlayer(Player player, int damage, bool crit)
		{
            player.AddBuff(mod.BuffType("Plague"), 300, true);
        }
		
		public override void HitEffect(int hitDirection, double damage)
		{
			for (int k = 0; k < 5; k++)
			{
				Dust.NewDust(npc.position, npc.width, npc.height, 46, hitDirection, -1f, 0, default(Color), 1f);
			}
			if (npc.life <= 0)
			{
				for (int k = 0; k < 20; k++)
				{
					Dust.NewDust(npc.position, npc.width, npc.height, 46, hitDirection, -1f, 0, default(Color), 1f);
				}
				Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/Viruling"), 1f);
				Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/Viruling2"), 1f);
				Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/Viruling3"), 1f);
			}
		}
		
		public override void NPCLoot()
		{
            Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("PlagueCellCluster"), Main.rand.Next(1, 3));
        }
	}
}
