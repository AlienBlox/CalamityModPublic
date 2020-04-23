﻿using CalamityMod.Items.Placeables.Banners;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.NPCs.NormalNPCs
{
    public class Bohldohr : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Bohldohr");
        }

        public override void SetDefaults()
        {
            npc.aiStyle = -1;
            aiType = -1;
            npc.damage = 150;
            npc.width = 40;
            npc.height = 40;
            npc.defense = 18;
            npc.lifeMax = 300;
            npc.knockBackResist = 0.95f;
            npc.value = Item.buyPrice(0, 0, 10, 0);
            npc.HitSound = SoundID.NPCHit7;
            npc.DeathSound = SoundID.NPCDeath35;
            npc.behindTiles = true;
            banner = npc.type;
            bannerItem = ModContent.ItemType<BOHLDOHRBanner>();
        }

        public override void AI()
        {
            CalamityAI.UnicornAI(npc, mod, true, 2);
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.playerSafe)
            {
                return 0f;
            }
            return SpawnCondition.JungleTemple.Chance * 0.05f;
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(npc.position, npc.width, npc.height, 155, hitDirection, -1f, 0, default, 1f);
            }
            if (npc.life <= 0)
            {
                for (int k = 0; k < 20; k++)
                {
                    Dust.NewDust(npc.position, npc.width, npc.height, 155, hitDirection, -1f, 0, default, 1f);
                }
            }
        }

        public override void NPCLoot()
        {
            if (CalamityWorld.downedSCal)
            {
                // RIP LORDE
                // Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ModContent.ItemType<NO>());
            }
            DropHelper.DropItem(npc, ItemID.LihzahrdBrick, 10, 30);
            DropHelper.DropItemChance(npc, ItemID.LunarTabletFragment, 7, 1, 3); //solar tablet fragment
            DropHelper.DropItemChance(npc, ItemID.LihzahrdPowerCell, 50);
        }
    }
}
