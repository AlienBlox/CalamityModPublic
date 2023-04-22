﻿using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Dyes.HairDye
{
    public class StealthHairDye : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Stealth Hair Dye");
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 26;
            Item.maxStack = 9999;
            Item.value = Item.buyPrice(gold: 5);
            Item.rare = ItemRarityID.Green;
            Item.UseSound = SoundID.Item3;
            Item.useStyle = ItemUseStyleID.DrinkLiquid;
            Item.useTurn = true;
            Item.useAnimation = 17;
            Item.useTime = 17;
            Item.consumable = true;
            Item.ResearchUnlockCount = 1;
        }
    }
}
