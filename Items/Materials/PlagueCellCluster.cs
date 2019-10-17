﻿using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Items
{
    public class PlagueCellCluster : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Plague Cell Canister");
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.maxStack = 999;
            item.value = Item.buyPrice(0, 5, 0, 0);
            item.rare = 8;
        }
    }
}
