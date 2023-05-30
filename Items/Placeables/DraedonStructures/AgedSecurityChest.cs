﻿using CalamityMod.Items.Materials;
using CalamityMod.Rarities;
using CalamityMod.Tiles.DraedonStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Placeables.DraedonStructures
{
    public class AgedSecurityChest : ModItem, ILocalizedModType
    {
        public string LocalizationCategory => "Items.Placeables";
        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 26;
            Item.maxStack = 9999;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.useAnimation = 15;
            Item.useTime = 14;
            Item.rare = ModContent.RarityType<DarkOrange>();
            Item.useStyle = ItemUseStyleID.Swing;
            Item.consumable = true;
            Item.value = 500;
            Item.createTile = ModContent.TileType<AgedSecurityChestTile>();
        }

        public override void AddRecipes()
        {
            CreateRecipe(1).
                AddIngredient(ModContent.ItemType<MysteriousCircuitry>(), 4).
                AddIngredient(ModContent.ItemType<DubiousPlating>(), 4).
                AddIngredient(ModContent.ItemType<RustedPlating>(), 10).
                AddRecipeGroup("IronBar", 2).
                AddTile(TileID.Anvils).Register();
        }
    }
}
