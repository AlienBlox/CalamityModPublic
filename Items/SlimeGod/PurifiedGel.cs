﻿using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Items.SlimeGod
{
    public class PurifiedGel : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Purified Gel");
        }

        public override void SetDefaults()
        {
            item.width = 16;
            item.height = 14;
            item.maxStack = 999;
            item.value = Item.buyPrice(0, 2, 50, 0);
            item.rare = 4;
        }
    }
}
