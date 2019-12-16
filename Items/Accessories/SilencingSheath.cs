﻿using CalamityMod.CalPlayer;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    public class SilencingSheath : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Silencing Sheath");
            Tooltip.SetDefault("+50 maximum stealth\n" +
                "10% increased stealth regeneration while standing still");
        }

        public override void SetDefaults()
        {
            item.width = 32;
            item.height = 34;
            item.value = Item.buyPrice(0, 2, 0, 0);
            item.rare = 2;
            item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.rogueStealthMax += 0.5f;
            modPlayer.stealthGenStandstill += 0.1f;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddRecipeGroup("AnyEvilBar", 8);
            recipe.AddIngredient(ItemID.Silk, 10);
            recipe.AddRecipeGroup("Boss2Material", 3);
            recipe.AddTile(TileID.TinkerersWorkbench);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
