﻿using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    public class StatisNinjaBelt : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Statis' Ninja Belt");
            /* Tooltip.SetDefault("6% increased jump speed and allows constant jumping\n" +
                "Grants the ability to swim\n" +
                "Increased fall damage resistance by 35 blocks\n" +
                "Can climb walls, dash, and dodge attacks\n" +
                "The dodge has a 90 second cooldown\n" +
                "This cooldown is shared with all other dodges and reflects"); */
        }

        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 32;
            Item.value = CalamityGlobalItem.Rarity11BuyPrice;
            Item.rare = ItemRarityID.Purple;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.autoJump = true;
            player.jumpSpeedBoost += 0.3f;
            player.extraFall += 35;
            player.blackBelt = true;
            player.dashType = 1;
            player.Calamity().DashID = string.Empty;
            player.spikedBoots = 2;
            player.accFlipper = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.MasterNinjaGear).
                AddIngredient(ItemID.FrogGear).
                AddIngredient<PurifiedGel>(50).
                AddIngredient<Phantoplasm>(5).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}
