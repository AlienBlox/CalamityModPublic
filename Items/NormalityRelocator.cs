﻿using CalamityMod.CalPlayer;
using CalamityMod.Items.Placeables.Plates;
using CalamityMod.Items.Placeables.Ores;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using System.Linq;

namespace CalamityMod.Items
{
    public class NormalityRelocator : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Normality Relocator");
            /* Tooltip.SetDefault("I'll be there in the blink of an eye\n" +
                "This line is modified below\n" +
                "Fall speed is doubled for 30 frames after teleporting\n" +
                "Teleportation is disabled while Chaos State is active\n" +
                "Works while in the inventory"); */
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(6, 7));
            ItemID.Sets.AnimatesAsSoul[Type] = true;
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 38;
            Item.height = 38;
            Item.value = CalamityGlobalItem.Rarity10BuyPrice;
            Item.rare = ItemRarityID.Red;
            Item.Calamity().donorItem = true;
        }

        public override void ModifyTooltips(List<TooltipLine> list)
        {
            string hotkey = CalamityKeybinds.NormalityRelocatorHotKey.TooltipHotkeyString();
            TooltipLine line = list.FirstOrDefault(x => x.Mod == "Terraria" && x.Name == "Tooltip1");

            if (line != null)
                line.Text = "Press " + hotkey + " to teleport to the position of the mouse";
        }

		public override void ModifyResearchSorting(ref ContentSamples.CreativeHelper.ItemGroup itemGroup)
		{
			itemGroup = (ContentSamples.CreativeHelper.ItemGroup)CalamityResearchSorting.ToolsOther;
		}

        public override void UpdateInventory(Player player)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.normalityRelocator = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.RodofDiscord).
                AddIngredient<Cinderplate>(5).
                AddIngredient<ExodiumCluster>(10).
                AddIngredient(ItemID.FragmentStardust, 30).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}
