﻿using CalamityMod.Items.Placeables.Ores;
using CalamityMod.Rarities;
using CalamityMod.Tiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Materials
{
    [LegacyName("UeliaceBar")]
    public class UelibloomBar : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 25;
            // DisplayName.SetDefault("Uelibloom Bar"); // Yoo-luh Bloom
			ItemID.Sets.SortingPriorityMaterials[Type] = 106;
        }

        public override void SetDefaults()
        {
            Item.createTile = ModContent.TileType<Tiles.UelibloomBar>();
            Item.width = 15;
            Item.height = 12;
            Item.maxStack = Item.CommonMaxStack;
            Item.value = Item.sellPrice(gold: 5);
            Item.rare = ModContent.RarityType<Turquoise>();
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTurn = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.autoReuse = true;
            Item.consumable = true;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<UelibloomOre>(4).
                AddTile(TileID.AdamantiteForge).
                Register();
        }
    }
}
