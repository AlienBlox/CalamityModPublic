﻿using CalamityMod.Buffs.Alcohol;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Potions.Alcohol
{
    public class MoscowMule : ModItem
    {
        internal static readonly int CritBoost = 3;

        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 5;
            // DisplayName.SetDefault("Moscow Mule");
            // Tooltip.SetDefault(@"I once heard the copper mug can be toxic and I told 'em 'listen dummy, I'm already poisoning myself'
//Boosts damage by 9%, knockback by 50% and critical strike chance by 3%
//Reduces life regen by 4");
        }

        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 34;
            Item.useTurn = true;
            Item.maxStack = Item.CommonMaxStack;
            Item.rare = ItemRarityID.Yellow;
            Item.useAnimation = 17;
            Item.useTime = 17;
            Item.useStyle = ItemUseStyleID.DrinkLiquid;
            Item.UseSound = SoundID.Item3;
            Item.consumable = true;
            Item.buffType = ModContent.BuffType<MoscowMuleBuff>();
            Item.buffTime = CalamityUtils.SecondsToFrames(480f);
            Item.value = Item.buyPrice(0, 5, 30, 0);
        }
    }
}
