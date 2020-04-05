﻿using CalamityMod.CalPlayer;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    public class StressPills : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Stress Pills");
            Tooltip.SetDefault("Boosts your defense by 4 and max movement speed and acceleration by 5%\n" +
                               "Receiving a hit causes you to only lose half of your max adrenaline rather than all of it\n" +
							   "Revengeance drop");
        }

        public override void SetDefaults()
        {
            item.width = 26;
            item.height = 26;
            item.value = Item.buyPrice(0, 6, 0, 0);
            item.rare = 3;
            item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
			player.statDefense += 4;
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.stressPills = true;
        }
    }
}
