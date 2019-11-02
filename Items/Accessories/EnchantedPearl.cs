﻿using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables;

namespace CalamityMod.Items.Accessories
{
    public class EnchantedPearl : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Enchanted Pearl");
            Tooltip.SetDefault("Increases fishing skill\n" +
				"Increases chance to catch crates");
        }

        public override void SetDefaults()
        {
            item.width = 26;
            item.height = 26;
            item.value = Item.buyPrice(0, 9, 0, 0);
            item.rare = 3;
            item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.fishingSkill += 10;
			player.Calamity().enchantedPearl = true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.FishingPotion);
            recipe.AddIngredient(ItemID.CratePotion, 8);
            recipe.AddIngredient(ItemID.TissueSample, 5);
            recipe.AddIngredient(ModContent.ItemType<SeaPrism>(), 10);
            recipe.AddIngredient(ModContent.ItemType<VictideBar>(), 3);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
            recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.FishingPotion);
            recipe.AddIngredient(ItemID.CratePotion, 8);
            recipe.AddIngredient(ItemID.ShadowScale, 5);
            recipe.AddIngredient(ModContent.ItemType<SeaPrism>(), 10);
            recipe.AddIngredient(ModContent.ItemType<VictideBar>(), 3);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
