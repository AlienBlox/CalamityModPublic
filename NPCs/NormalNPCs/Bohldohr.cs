﻿using CalamityMod.Items.Placeables.Banners;
using CalamityMod.World;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;
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
            NPC.Calamity().canBreakPlayerDefense = true;
            NPC.aiStyle = -1;
            AIType = -1;
            NPC.damage = 80;
            NPC.width = 40;
            NPC.height = 40;
            NPC.defense = 18;
            NPC.lifeMax = 300;
            NPC.knockBackResist = 0.95f;
            NPC.value = Item.buyPrice(0, 0, 10, 0);
            NPC.HitSound = SoundID.NPCHit7;
            NPC.DeathSound = SoundID.NPCDeath35;
            NPC.behindTiles = true;
            Banner = NPC.type;
            BannerItem = ModContent.ItemType<BohldohrBanner>();
            NPC.Calamity().VulnerableToSickness = false;
            NPC.Calamity().VulnerableToWater = true;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.TheTemple,

				// Will move to localization whenever that is cleaned up.
				new FlavorTextBestiaryInfoElement("A distant relative to the gem-covered lizards found within the caverns. This species instead covers itself with the smooth bricks of the Temple and curls into a ball for locomotion.")
            });
        }

        public override void AI()
        {
            CalamityAI.UnicornAI(NPC, Mod, true, CalamityWorld.death ? 8f : CalamityWorld.revenge ? 6f : 4f, 5f, 0.2f);
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.PlayerSafe)
            {
                return 0f;
            }
            return SpawnCondition.JungleTemple.Chance * 0.1f;
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, 155, hitDirection, -1f, 0, default, 1f);
            }
            if (NPC.life <= 0)
            {
                for (int k = 0; k < 20; k++)
                {
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, 155, hitDirection, -1f, 0, default, 1f);
                }
                if (Main.netMode != NetmodeID.Server)
                {
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("Bohldohr").Type, 1f);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("Bohldohr2").Type, 1f);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("Bohldohr3").Type, 1f);
                }
            }
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemID.LihzahrdBrick, 1, 10, 26);
            npcLoot.Add(ItemID.LunarTabletFragment, 7, 10, 26);
            npcLoot.Add(ItemID.LihzahrdPowerCell, 50);
        }
    }
}
