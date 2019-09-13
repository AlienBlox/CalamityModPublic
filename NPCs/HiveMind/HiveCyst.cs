﻿using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.HiveMind
{
    public class HiveCyst : ModNPC
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Hive Cyst");
			Main.npcFrameCount[npc.type] = 4;
		}

		public override void SetDefaults()
		{
			npc.npcSlots = 0f;
			npc.aiStyle = -1;
			aiType = -1;
			npc.damage = 0;
			npc.width = 30; //324
			npc.height = 30; //216
			npc.defense = 0;
			npc.lifeMax = 1000;
			npc.knockBackResist = 0f;
			npc.chaseable = false;
			npc.HitSound = SoundID.NPCHit1;
			npc.rarity = 2;
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
			if (spawnInfo.playerSafe || NPC.AnyNPCs(mod.NPCType("HiveCyst")) || NPC.AnyNPCs(mod.NPCType("HiveMind")) || NPC.AnyNPCs(mod.NPCType("HiveMindP2")))
			{
				return 0f;
			}
			return SpawnCondition.Corruption.Chance * (Main.hardMode ? 0.05f : 0.5f);
		}

		public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
		{
			npc.lifeMax = 2000;
			npc.damage = 0;
		}

		public override void HitEffect(int hitDirection, double damage)
		{
			for (int k = 0; k < 5; k++)
			{
				Dust.NewDust(npc.position, npc.width, npc.height, 14, hitDirection, -1f, 0, default, 1f);
			}
			if (npc.life <= 0)
			{
				for (int k = 0; k < 20; k++)
				{
					Dust.NewDust(npc.position, npc.width, npc.height, 14, hitDirection, -1f, 0, default, 1f);
				}
				if (Main.netMode != NetmodeID.MultiplayerClient && NPC.CountNPCS(mod.NPCType("HiveMind")) < 1)
				{
					Vector2 spawnAt = npc.Center + new Vector2(0f, (float)npc.height / 2f);
					NPC.NewNPC((int)spawnAt.X, (int)spawnAt.Y, mod.NPCType("HiveMind"));
				}
			}
		}
	}
}
