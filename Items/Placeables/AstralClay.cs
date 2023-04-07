﻿using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Placeables
{
    public class AstralClay : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Astral Clay");
            Item.ResearchUnlockCount = 100;
        }

        public override void SetDefaults()
        {
            Item.createTile = ModContent.TileType<Tiles.Astral.AstralClay>();
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
    }
}
