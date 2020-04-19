﻿using CalamityMod.CalPlayer;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    public class MomentumCapacitor : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Momentum Capacitor");
            Tooltip.SetDefault("TOOLTIP LINE HERE\n" +
                               "Rogue projectiles that enter the field get a constant acceleration and 15% damage boost\n" +
                               "These boosts can only happen to a projectile once\n" +
                               "There can only be one field");
        }

        public override void SetDefaults()
        {
            item.width = 28;
            item.height = 32;
            item.value = Item.buyPrice(0, 36, 0, 0);
            item.accessory = true;
            item.rare = 6;
        }

        public override void ModifyTooltips(List<TooltipLine> list)
        {
            string hotkey = CalamityMod.MomentumCapacitatorHotkey.TooltipHotkeyString();
            foreach (TooltipLine line2 in list)
            {
                if (line2.mod == "Terraria" && line2.Name == "Tooltip0")
                {
                    line2.text = "Press " + hotkey + " to consume 30% of your maximum stealth to create an energy field at the cursor position";
                }
            }
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.momentumCapacitor = true;
        }
    }
}
