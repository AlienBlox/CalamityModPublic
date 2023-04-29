﻿using CalamityMod.Tiles.Furniture.CraftingStations;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Items.Placeables.FurnitureAbyss
{
    public class AbyssClock : ModItem
    {
        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 22;
            Item.maxStack = 9999;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.consumable = true;
            Item.value = 0;
            Item.createTile = ModContent.TileType<Tiles.FurnitureAbyss.AbyssClock>();
        }

        public override void AddRecipes()
        {
            CreateRecipe(1).
                AddRecipeGroup("IronBar", 3).
                AddIngredient(ItemID.Glass, 6).
                AddIngredient(ModContent.ItemType<SmoothAbyssGravel>(), 10).
                AddTile(ModContent.TileType<VoidCondenser>()).
                Register();
        }
    }
}
