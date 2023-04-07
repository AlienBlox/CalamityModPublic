﻿using CalamityMod.Events;
using CalamityMod.Items.Materials;
using CalamityMod.NPCs.DevourerofGods;
using CalamityMod.Rarities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.SummonItems
{
    public class CosmicWorm : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
            // DisplayName.SetDefault("Cosmic Worm");
            /* Tooltip.SetDefault("Summons the Devourer of Gods\n" +
                "Not consumable"); */
			ItemID.Sets.SortingPriorityBossSpawns[Type] = 17; // Celestial Sigil
        }

        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 18;
            Item.useAnimation = 10;
            Item.useTime = 10;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.consumable = false;
            Item.rare = ModContent.RarityType<Turquoise>();
        }

		public override void ModifyResearchSorting(ref ContentSamples.CreativeHelper.ItemGroup itemGroup)
		{
			itemGroup = ContentSamples.CreativeHelper.ItemGroup.BossItem;
		}

        public override bool CanUseItem(Player player)
        {
            return !NPC.AnyNPCs(ModContent.NPCType<DevourerofGodsHead>()) && !BossRushEvent.BossRushActive;
        }

        public override bool? UseItem(Player player)
        {
            string key = "Mods.CalamityMod.EdgyBossText7";
            Color messageColor = Color.Cyan;
            CalamityUtils.DisplayLocalizedText(key, messageColor);

            SoundEngine.PlaySound(DevourerofGodsHead.SpawnSound, player.Center);
            if (Main.netMode != NetmodeID.MultiplayerClient)
                NPC.SpawnOnPlayer(player.whoAmI, ModContent.NPCType<DevourerofGodsHead>());
            else
                NetMessage.SendData(MessageID.SpawnBossUseLicenseStartEvent, -1, -1, null, player.whoAmI, ModContent.NPCType<DevourerofGodsHead>());

            return true;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<ArmoredShell>().
                AddIngredient<TwistingNether>().
                AddIngredient<DarkPlasma>().
                AddTile(TileID.LunarCraftingStation).
                Register();

            // sequence breaking recipe
            CreateRecipe().
                AddIngredient(ItemID.LunarBar, 40).
                AddIngredient<GalacticaSingularity>(10).
                AddIngredient<Phantoplasm>(40).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}
