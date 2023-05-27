﻿using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Items.Placeables.Furniture
{
    [LegacyName("BlueCandle")]
    public class WeightlessCandle : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 40;
            Item.maxStack = 9999;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.consumable = true;
            Item.value = Item.buyPrice(0, 25, 0, 0);
            Item.rare = ItemRarityID.LightRed;
            Item.createTile = ModContent.TileType<Tiles.Furniture.BlueCandle>();
        }
    }
}
