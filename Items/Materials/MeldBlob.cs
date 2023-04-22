﻿using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Items.Materials
{
    public class MeldBlob : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 100;
            // DisplayName.SetDefault("Meld Blob");
        }

        public override void SetDefaults()
        {
            Item.width = 16;
            Item.height = 14;
            Item.maxStack = 9999;
            Item.value = Item.sellPrice(silver: 20);
            Item.rare = ItemRarityID.Cyan;
        }
    }
}
