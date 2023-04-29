﻿using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Items.Placeables.FurnitureOtherworldly
{
    [LegacyName("OccultBookcase")]
    public class OtherworldlyBookcase : ModItem
    {
        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            Item.SetNameOverride("Otherworldly Bookcase");
            Item.width = 28;
            Item.height = 20;
            Item.maxStack = 9999;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.consumable = true;
            Item.createTile = ModContent.TileType<Tiles.FurnitureOtherworldly.OtherworldlyBookcase>();
        }

        public override void AddRecipes()
        {
            CreateRecipe(1).AddIngredient(ModContent.ItemType<OtherworldlyStone>(), 20).AddIngredient(ItemID.Book, 10).AddTile(TileID.LunarCraftingStation).Register();
        }
    }
}
