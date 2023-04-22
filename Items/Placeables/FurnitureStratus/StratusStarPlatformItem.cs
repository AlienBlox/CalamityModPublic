﻿using CalamityMod.Tiles.FurnitureStratus;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Placeables.FurnitureStratus
{
    public class StratusStarPlatformItem : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Stratus Star Platform");
            Item.ResearchUnlockCount = 200;
        }

        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 26;
            Item.maxStack = 9999;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.consumable = true;
            Item.createTile = ModContent.TileType<StratusStarPlatform>();
        }

        public override void AddRecipes()
        {
            CreateRecipe(2).AddIngredient(ModContent.ItemType<StratusBricks>()).AddTile(TileID.LunarCraftingStation).Register();
        }
    }
}
