﻿using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Items.Materials
{
    public class ReaperTooth : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Reaper Tooth");
            Tooltip.SetDefault("Sharp enough to cut diamonds");
        }

        public override void SetDefaults()
        {
            item.width = 26;
            item.height = 26;
            item.maxStack = 999;
            item.value = Item.buyPrice(0, 7, 0, 0);
            item.rare = 10;
            item.Calamity().customRarity = CalamityRarity.PureGreen;
        }
    }
}
