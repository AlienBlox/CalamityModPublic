﻿using CalamityMod.Tiles.Furniture.CraftingStations;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Placeables.FurnitureAbyss
{
    [LegacyName("AbyssWorkbench")]
    public class AbyssWorkBench : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.SetNameOverride("Abyss Work Bench");
            Item.width = 28;
            Item.height = 14;
            Item.maxStack = Item.CommonMaxStack;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.consumable = true;
            Item.value = 0;
            Item.createTile = ModContent.TileType<Tiles.FurnitureAbyss.AbyssWorkBench>();

            // This is Ozz's item of choice for placing inactive Power Cell Factories for lab schematics. It should not do this normally.
            // item.createTile = ModContent.TileType<Tiles.DraedonStructures.InactivePowerCellFactory>();
        }

        public override void AddRecipes()
        {
            CreateRecipe(1).AddIngredient(ModContent.ItemType<SmoothAbyssGravel>(), 10).AddTile(ModContent.TileType<VoidCondenser>()).Register();
        }
    }
}
