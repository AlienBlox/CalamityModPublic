﻿using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Items
{
    public class GrandScale : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Grand Scale");
            Tooltip.SetDefault("Large scale of an apex predator");
        }

        public override void SetDefaults()
        {
            item.width = 15;
            item.height = 12;
            item.maxStack = 999;
            item.value = Item.buyPrice(0, 4, 50, 0);
            item.rare = 7;
        }
    }
}
