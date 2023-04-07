﻿using CalamityMod.Items.Placeables.Walls;
using Terraria.ID;
using Terraria.ModLoader; // If you are using c# 6, you can use: "using static Terraria.Localization.GameCulture;" which would mean you could just write "DisplayName.AddTranslation(German, "");"

namespace CalamityMod.Items.Placeables.FurnitureProfaned
{
    public class RunicProfanedBrick : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 100;
        }

        public override void SetDefaults()
        {
            Item.width = 12;
            Item.height = 12;
            Item.maxStack = 999;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.consumable = true;
            Item.createTile = ModContent.TileType<Tiles.FurnitureProfaned.RunicProfanedBrick>();
        }

        public override void AddRecipes()
        {
            CreateRecipe(5).AddIngredient(ModContent.ItemType<ProfanedRock>(), 4).AddIngredient(ModContent.ItemType<ProfanedCrystal>(), 1).AddTile(TileID.LunarCraftingStation).Register();
            CreateRecipe(1).AddIngredient(ModContent.ItemType<RunicProfanedBrickWall>(), 4).AddTile(TileID.WorkBenches).Register();
        }
    }
}
