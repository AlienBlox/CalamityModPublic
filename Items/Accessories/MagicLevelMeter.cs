using CalamityMod.CalPlayer;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    public class MagicLevelMeter : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Magic Level Meter");
            Tooltip.SetDefault("Tells you how high your magic proficiency is");
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
            int level = Main.player[Main.myPlayer].Calamity().magicLevel;
            int exactLevel = Main.player[Main.myPlayer].Calamity().exactMagicLevel;
            int damageGain = 0;
            int manaUsage = 0;
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
                    manaUsage = 1;
                    break;
                case 4:
                    damageGain = 3;
                    manaUsage = 1;
                    critGain = 1;
                    break;
                case 5:
                    damageGain = 3;
                    manaUsage = 2;
                    critGain = 1;
                    break;
                case 6:
                    damageGain = 4;
                    manaUsage = 3;
                    critGain = 1;
                    break;
                case 7:
                    damageGain = 4;
                    manaUsage = 4;
                    critGain = 2;
                    break;
                case 8:
                    damageGain = 5;
                    manaUsage = 5;
                    critGain = 2;
                    break;
                case 9:
                    damageGain = 5;
                    manaUsage = 5;
                    critGain = 3;
                    break;
                case 10:
                    damageGain = 6;
                    manaUsage = 6;
                    critGain = 3;
                    break;
                case 11:
                    damageGain = 7;
                    manaUsage = 7;
                    critGain = 4;
                    break;
                case 12:
                    damageGain = 8;
                    manaUsage = 8;
                    critGain = 4;
                    break;
                case 13:
                    damageGain = 9;
                    manaUsage = 9;
                    critGain = 5;
                    break;
                case 14:
                    damageGain = 10;
                    manaUsage = 10;
                    critGain = 5;
                    break;
                case 15:
                    damageGain = 12;
                    manaUsage = 12;
                    critGain = 6;
                    break;
            }
            foreach (TooltipLine line2 in list)
            {
                if (line2.mod == "Terraria" && line2.Name == "Tooltip0")
                {
                    line2.text = "Tells you how high your magic proficiency is\n" +
                "While equipped you will gain magic proficiency faster\n" +
                "The higher your magic level the higher your magic damage, critical strike chance, and the lower your mana cost\n" +
                "Magic proficiency (max of 12500): " + (level - (level > 12500 ? 1 : 0)) + "\n" +
                "Magic level (max of 15): " + exactLevel + "\n" +
                "Magic damage increase: " + damageGain + "%\n" +
                "Mana usage decrease: " + manaUsage + "%\n" +
                "Magic crit increase: " + critGain + "%";
                }
            }
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.fasterMagicLevel = true;
        }
    }
}
