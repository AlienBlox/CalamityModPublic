﻿using CalamityMod.Items.Placeables.FurnitureCosmilite;
using Terraria.ModLoader;
using Terraria.ID;
namespace CalamityMod.Items.Placeables.FurnitureOtherworldly
{
    [LegacyName("OccultLantern")]
    public class OtherworldlyLantern : ModItem, ILocalizedModType
    {
        public string LocalizationCategory => "Items.Placeables";
        public override void SetDefaults()
        {
            Item.SetNameOverride("Otherworldly Lantern");
            Item.width = 28;
            Item.height = 20;
            Item.maxStack = 9999;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.consumable = true;
            Item.createTile = ModContent.TileType<Tiles.FurnitureOtherworldly.OtherworldlyLantern>();
        }

        public override void AddRecipes()
        {
            CreateRecipe(1).AddIngredient(ModContent.ItemType<OtherworldlyStone>(), 18).AddIngredient(ModContent.ItemType<CosmiliteBrick>(), 1).AddTile(TileID.LunarCraftingStation).Register();
        }
    }
}
