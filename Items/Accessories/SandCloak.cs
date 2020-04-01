﻿using CalamityMod.CalPlayer;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    public class SandCloak : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sand Cloak");
            Tooltip.SetDefault("+1 defense and 5% increased movement speed\n" +
                "This line is modified below\n" + 
                "This effect has a 30 second cooldown before it can be used again");
        }

        public override void SetDefaults()
        {
            item.width = 30;
            item.height = 44;
            item.value = Item.buyPrice(0, 2, 0, 0);
            item.rare = 2;
            item.accessory = true;
        }

        public override void ModifyTooltips(List<TooltipLine> list)
        {
            string hotkey = CalamityMod.SandCloakHotkey.GetAssignedKeys().Count > 0 ? CalamityMod.SandCloakHotkey.GetAssignedKeys()[0] : "C";
            foreach (TooltipLine line2 in list)
            {
                if (line2.mod == "Terraria" && line2.Name == "Tooltip1")
                {
                    line2.text = "Press " + hotkey + " to consume 25% of your maximum stealth to create a protective dust veil which provides +6 defense and +2 life regen";
                }
            }
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.statDefense += 1;
            player.moveSpeed += 0.05f;
            player.Calamity().sandCloak = true;
        }
    }
}
