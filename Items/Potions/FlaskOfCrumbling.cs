﻿using CalamityMod.Buffs.Potions;
using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Potions
{
    [LegacyName("CrumblingPotion")]
    public class FlaskOfCrumbling : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 20;
            // DisplayName.SetDefault("Flask of Crumbling");
            // Tooltip.SetDefault("Melee, Whip, and Rogue attacks inflict Armor Crunch on enemies");
        }

        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 18;
            Item.useTurn = true;
            Item.maxStack = 30;
            Item.useAnimation = 17;
            Item.useTime = 17;
            Item.useStyle = ItemUseStyleID.DrinkLiquid;
            Item.UseSound = SoundID.Item3;
            Item.value = Item.buyPrice(0, 2, 0, 0);
            Item.rare = ItemRarityID.LightRed;
            Item.consumable = true;
            Item.buffType = ModContent.BuffType<WeaponImbueCrumbling>();
            Item.buffTime = CalamityUtils.SecondsToFrames(1200f);
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.BottledWater).
                AddIngredient<EssenceofSunlight>(2).
                AddTile(TileID.ImbuingStation).
                Register();
        }
    }
}
