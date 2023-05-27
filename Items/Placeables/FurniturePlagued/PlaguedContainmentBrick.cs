﻿using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Walls;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Items.Placeables.FurniturePlagued
{
    [LegacyName("PlaguedPlate")]
    public class PlaguedContainmentBrick : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 100;
        }

        public override void SetDefaults()
        {
            Item.width = 12;
            Item.height = 12;
            Item.maxStack = 9999;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.consumable = true;
            Item.createTile = ModContent.TileType<Tiles.FurniturePlaguedPlate.PlaguedPlate>();
        }

        public override void AddRecipes()
        {
            CreateRecipe(50).
                AddRecipeGroup("AnyStoneBlock", 50).
                AddIngredient<PlagueCellCanister>().
                AddTile<PlagueInfuser>().
                Register();
            CreateRecipe().
                AddIngredient<PlaguedPlateWall>(4).
                AddTile<PlagueInfuser>().
                Register();
            CreateRecipe().
                AddIngredient<PlaguedPlatePlatform>(2).
                AddTile<PlagueInfuser>().
                Register();
        }
    }
}
