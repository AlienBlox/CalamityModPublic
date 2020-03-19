﻿using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Banners;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.World;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.NPCs.NormalNPCs
{
    public class StormlionCharger : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Stormlion");
            Main.npcFrameCount[npc.type] = 6;
        }

        public override void SetDefaults()
        {
            npc.damage = 20;
            npc.aiStyle = 3;
            npc.width = 33;
            npc.height = 31;
            npc.defense = 8;
            npc.lifeMax = 80;
            npc.knockBackResist = 0.2f;
            animationType = 508;
            npc.value = Item.buyPrice(0, 0, 2, 0);
            npc.HitSound = SoundID.NPCHit31;
            npc.DeathSound = SoundID.NPCDeath34;
            banner = npc.type;
            bannerItem = ModContent.ItemType<StormlionBanner>();
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(npc.position, npc.width, npc.height, 5, hitDirection, -1f, 0, default, 1f);
            }
            if (npc.life <= 0)
            {
                for (int k = 0; k < 20; k++)
                {
                    Dust.NewDust(npc.position, npc.width, npc.height, 5, hitDirection, -1f, 0, default, 1f);
                }
            }
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.playerSafe || spawnInfo.player.Calamity().ZoneSunkenSea)
            {
                return 0f;
            }
            return SpawnCondition.DesertCave.Chance * 0.2f;
        }

        public override void OnHitPlayer(Player player, int damage, bool crit)
        {
            player.AddBuff(BuffID.Electrified, 90, true);
        }

        public override void NPCLoot()
        {
            DropHelper.DropItemChance(npc, ModContent.ItemType<StormjawStaff>(), 0.2f, 1, 1);
            //100% chance if DS isn't dead yet, otherwise 50%
            DropHelper.DropItemChance(npc, ModContent.ItemType<StormlionMandible>(), !CalamityWorld.downedDesertScourge ? 1f : 0.5f, 1, 1);
        }
    }
}
