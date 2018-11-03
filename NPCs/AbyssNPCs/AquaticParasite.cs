﻿using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Projectiles;

namespace CalamityMod.NPCs.AbyssNPCs
{
	public class AquaticParasite : ModNPC
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Aquatic Parasite");
		}
		
		public override void SetDefaults()
		{
            npc.noGravity = true;
			npc.damage = 15;
			npc.width = 28;
			npc.height = 28;
			npc.defense = 5;
			npc.lifeMax = 30;
            if (CalamityWorld.bossRushActive)
            {
                npc.lifeMax = 30000;
            }
            npc.aiStyle = -1;
			aiType = -1;
            for (int k = 0; k < npc.buffImmune.Length; k++)
            {
                npc.buffImmune[k] = true;
            }
            npc.value = Item.buyPrice(0, 0, 1, 0);
			npc.HitSound = SoundID.NPCHit1;
			npc.DeathSound = SoundID.NPCDeath1;
        }

        public override void AI()
        {
            bool flag15 = false;
            if (npc.wet && npc.ai[1] == 1f)
            {
                flag15 = true;
            }
            else
            {
                npc.dontTakeDamage = false;
            }
            float num262 = 1f;
            if (flag15)
            {
                num262 += 0.5f;
            }
            if (npc.direction == 0)
            {
                npc.TargetClosest(true);
            }
            if (flag15)
            {
                return;
            }
            if (!npc.wet)
            {
                npc.rotation += npc.velocity.X * 0.1f;
                if (npc.velocity.Y == 0f)
                {
                    npc.velocity.X = npc.velocity.X * 0.98f;
                    if ((double)npc.velocity.X > -0.01 && (double)npc.velocity.X < 0.01)
                    {
                        npc.velocity.X = 0f;
                    }
                }
                npc.velocity.Y = npc.velocity.Y + 0.2f;
                if (npc.velocity.Y > 10f)
                {
                    npc.velocity.Y = 10f;
                }
                npc.ai[0] = 1f;
                return;
            }
            if (npc.collideX)
            {
                npc.velocity.X = npc.velocity.X * -1f;
                npc.direction *= -1;
            }
            if (npc.collideY)
            {
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
            bool flag16 = false;
            if (!npc.friendly)
            {
                npc.TargetClosest(false);
                if (!Main.player[npc.target].dead)
                {
                    flag16 = true;
                }
            }
            if (flag16)
            {
                npc.localAI[2] = 1f;
                npc.rotation = (float)Math.Atan2((double)npc.velocity.Y, (double)npc.velocity.X) + 1.57f;
                npc.velocity *= 0.975f;
                float num263 = 3.2f;
                if (npc.velocity.X > -num263 && npc.velocity.X < num263 && npc.velocity.Y > -num263 && npc.velocity.Y < num263)
                {
                    npc.TargetClosest(true);
                    float num264 = 15f;
                    Vector2 vector31 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
                    float num265 = Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2) - vector31.X;
                    float num266 = Main.player[npc.target].position.Y + (float)(Main.player[npc.target].height / 2) - vector31.Y;
                    float num267 = (float)Math.Sqrt((double)(num265 * num265 + num266 * num266));
                    num267 = num264 / num267;
                    num265 *= num267;
                    num266 *= num267;
                    npc.velocity.X = num265;
                    npc.velocity.Y = num266;
                    return;
                }
            }
            else
            {
                npc.localAI[2] = 0f;
                npc.velocity.X = npc.velocity.X + (float)npc.direction * 0.02f;
                npc.rotation = npc.velocity.X * 0.4f;
                if (npc.velocity.X < -1f || npc.velocity.X > 1f)
                {
                    npc.velocity.X = npc.velocity.X * 0.95f;
                }
                if (npc.ai[0] == -1f)
                {
                    npc.velocity.Y = npc.velocity.Y - 0.01f;
                    if (npc.velocity.Y < -1f)
                    {
                        npc.ai[0] = 1f;
                    }
                }
                else
                {
                    npc.velocity.Y = npc.velocity.Y + 0.01f;
                    if (npc.velocity.Y > 1f)
                    {
                        npc.ai[0] = -1f;
                    }
                }
                int num268 = (int)(npc.position.X + (float)(npc.width / 2)) / 16;
                int num269 = (int)(npc.position.Y + (float)(npc.height / 2)) / 16;
                if (Main.tile[num268, num269 - 1] == null)
                {
                    Main.tile[num268, num269 - 1] = new Tile();
                }
                if (Main.tile[num268, num269 + 1] == null)
                {
                    Main.tile[num268, num269 + 1] = new Tile();
                }
                if (Main.tile[num268, num269 + 2] == null)
                {
                    Main.tile[num268, num269 + 2] = new Tile();
                }
                if (Main.tile[num268, num269 - 1].liquid > 128)
                {
                    if (Main.tile[num268, num269 + 1].active())
                    {
                        npc.ai[0] = -1f;
                    }
                    else if (Main.tile[num268, num269 + 2].active())
                    {
                        npc.ai[0] = -1f;
                    }
                }
                else
                {
                    npc.ai[0] = 1f;
                }
                if ((double)npc.velocity.Y > 1.2 || (double)npc.velocity.Y < -1.2)
                {
                    npc.velocity.Y = npc.velocity.Y * 0.99f;
                    return;
                }
            }
        }

        public override void FindFrame(int frameHeight)
        {
            npc.frameCounter += 0.15f;
            npc.frameCounter %= Main.npcFrameCount[npc.type];
            int frame = (int)npc.frameCounter;
            npc.frame.Y = frame * frameHeight;
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.playerSafe)
            {
                return 0f;
            }
            if (spawnInfo.player.GetModPlayer<CalamityPlayer>(mod).ZoneSulphur && spawnInfo.water)
            {
                return 0.2f;
            }
            return 0f;
        }
		
		public override void OnHitPlayer(Player player, int damage, bool crit)
		{
			player.AddBuff(BuffID.Venom, 240, true);
		}
		
		public override void HitEffect(int hitDirection, double damage)
		{
			for (int k = 0; k < 3; k++)
			{
				Dust.NewDust(npc.position, npc.width, npc.height, 5, hitDirection, -1f, 0, default(Color), 1f);
			}
			if (npc.life <= 0)
			{
				for (int k = 0; k < 15; k++)
				{
					Dust.NewDust(npc.position, npc.width, npc.height, 5, hitDirection, -1f, 0, default(Color), 1f);
				}
			}
		}
	}
}