using CalamityMod.CalPlayer;
using CalamityMod.World;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using System.Collections.Generic;

namespace CalamityMod.Items.Accessories
{
    [AutoloadEquip(EquipType.Neck)]
    public class CounterScarf : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Counter Scarf");
            Tooltip.SetDefault("True melee strikes deal 10% more damage\n" +
                "Grants the ability to dash; dashing into an attack will cause you to dodge it\n" +
                "After a successful dodge you must wait 15 seconds before you can dodge again\n" +
                "This cooldown will be twice as long if you have Chaos State\n" +
                "While on cooldown, Chaos State will last twice as long");
        }

        public override void SetDefaults()
        {
            item.width = 26;
            item.height = 26;
            item.value = CalamityGlobalItem.Rarity2BuyPrice;
            item.rare = ItemRarityID.Green;
            item.accessory = true;
		}

        public override void ModifyTooltips(List<TooltipLine> list)
        {
            if (CalamityWorld.death)
            {
                foreach (TooltipLine line2 in list)
                {
                    if (line2.mod == "Terraria" && line2.Name == "Tooltip4")
                    {
                        line2.text = "While on cooldown, Chaos State will last twice as long\n" +
                        "Provides cold protection in Death Mode";
                    }
                }
            }
        }

        public override bool CanEquipAccessory(Player player, int slot) => !player.Calamity().dodgeScarf;

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.dodgeScarf = true;
            modPlayer.dashMod = 1;
        }
    }
}
