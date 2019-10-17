﻿using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Items
{
    public class AlluringBait : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Alluring Bait");
            Tooltip.SetDefault("30% increased fishing power during the day\n" +
                "45% increased fishing power during the night\n" +
                "60% increased fishing power during a solar eclipse");
        }

        public override void SetDefaults()
        {
            item.width = 26;
            item.height = 26;
            item.value = Item.buyPrice(0, 9, 0, 0);
            item.rare = 3;
            item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            if (Main.eclipse)
            { player.fishingSkill += 60; }
            else if (!Main.dayTime)
            { player.fishingSkill += 45; }
            else
            { player.fishingSkill += 30; }
        }
    }
}
