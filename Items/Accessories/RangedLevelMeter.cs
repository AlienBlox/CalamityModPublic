﻿using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using CalamityMod.CalPlayer;

namespace CalamityMod.Items.Accessories
{
    public class RangedLevelMeter : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ranged Level Meter");
            Tooltip.SetDefault("Tells you how high your ranged proficiency is");
        }

        public override void SetDefaults()
        {
            item.width = 26;
            item.height = 26;
            item.value = Item.buyPrice(0, 6, 0, 0);
            item.rare = 1;
            item.accessory = true;
        }

        public override void ModifyTooltips(List<TooltipLine> list)
        {
            int level = Main.player[Main.myPlayer].GetCalamityPlayer().rangedLevel;
            int exactLevel = Main.player[Main.myPlayer].GetCalamityPlayer().exactRangedLevel;
            int damageGain = 0;
            int moveSpeed = 0;
            int critGain = 0;
            switch (exactLevel)
            {
                case 0:
                    break;
                case 1:
                    damageGain = 1;
                    break;
                case 2:
                    damageGain = 2;
                    break;
                case 3:
                    damageGain = 2;
                    critGain = 1;
                    break;
                case 4:
                    damageGain = 3;
                    critGain = 1;
                    moveSpeed = 1;
                    break;
                case 5:
                    damageGain = 3;
                    moveSpeed = 2;
                    critGain = 1;
                    break;
                case 6:
                    damageGain = 4;
                    moveSpeed = 3;
                    critGain = 1;
                    break;
                case 7:
                    damageGain = 4;
                    moveSpeed = 4;
                    critGain = 2;
                    break;
                case 8:
                    damageGain = 5;
                    moveSpeed = 5;
                    critGain = 2;
                    break;
                case 9:
                    damageGain = 5;
                    moveSpeed = 5;
                    critGain = 3;
                    break;
                case 10:
                    damageGain = 6;
                    moveSpeed = 6;
                    critGain = 3;
                    break;
                case 11:
                    damageGain = 7;
                    moveSpeed = 7;
                    critGain = 4;
                    break;
                case 12:
                    damageGain = 8;
                    moveSpeed = 8;
                    critGain = 4;
                    break;
                case 13:
                    damageGain = 9;
                    moveSpeed = 9;
                    critGain = 5;
                    break;
                case 14:
                    damageGain = 10;
                    moveSpeed = 10;
                    critGain = 5;
                    break;
                case 15:
                    damageGain = 12;
                    moveSpeed = 12;
                    critGain = 6;
                    break;
            }
            foreach (TooltipLine line2 in list)
            {
                if (line2.mod == "Terraria" && line2.Name == "Tooltip0")
                {
                    line2.text = "Tells you how high your ranged proficiency is\n" +
                "While equipped you will gain ranged proficiency faster\n" +
                "The higher your ranged level the higher your ranged damage, critical strike chance, and movement speed\n" +
                "Ranged proficiency (max of 12500): " + (level - (level > 12500 ? 1 : 0)) + "\n" +
                "Ranged level (max of 15): " + exactLevel + "\n" +
                "Ranged damage increase: " + damageGain + "%\n" +
                "Movement speed increase: " + moveSpeed + "%\n" +
                "Ranged crit increase: " + critGain + "%";
                }
            }
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.GetCalamityPlayer();
            modPlayer.fasterRangedLevel = true;
        }
    }
}
