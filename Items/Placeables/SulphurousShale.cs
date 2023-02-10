﻿using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Items.Placeables
{
    public class SulphurousShale : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 100;
            DisplayName.SetDefault("Sulphurous Shale");
        }

        public override void SetDefaults()
        {
            Item.createTile = ModContent.TileType<Tiles.Abyss.SulphurousShale>();
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTurn = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.autoReuse = true;
            Item.consumable = true;
            Item.width = 16;
            Item.height = 16;
            Item.maxStack = 999;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<Walls.SulphurousShaleWall>(4).
                AddTile(TileID.WorkBenches).
                Register();
        }
    }
}