﻿using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Items.Accessories
{
    public class VoltaicJelly : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Voltaic Jelly");
            // Tooltip.SetDefault("Minion attacks inflict Electrified");
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 22;
            Item.value = CalamityGlobalItem.Rarity2BuyPrice;
            Item.accessory = true;
            Item.rare = ItemRarityID.Green;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.Calamity().voltaicJelly = true;
        }
    }
}
