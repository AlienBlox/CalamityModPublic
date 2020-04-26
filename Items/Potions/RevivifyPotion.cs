﻿using CalamityMod.Buffs.Potions;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Fishing.SunkenSeaCatches;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Potions
{
    public class RevivifyPotion : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Revivify Potion");
            Tooltip.SetDefault("Causes enemy attacks to heal you for a fraction of their damage");
        }

        public override void SetDefaults()
        {
            item.width = 28;
            item.height = 36;
            item.useTurn = true;
            item.maxStack = 999;
            item.rare = 3;
            item.useAnimation = 17;
            item.useTime = 17;
            item.useStyle = 2;
            item.UseSound = SoundID.Item3;
            item.consumable = true;
			item.buffType = ModContent.BuffType<Revivify>();
			item.buffTime = 3600;
			item.value = Item.buyPrice(0, 2, 0, 0);
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.HolyWater, 5);
            recipe.AddIngredient(ModContent.ItemType<Stardust>(), 20);
            recipe.AddIngredient(ItemID.CrystalShard, 5);
            recipe.AddIngredient(ModContent.ItemType<EssenceofCinder>(), 3);
            recipe.AddIngredient(ModContent.ItemType<ScarredAngelfish>());
            recipe.AddTile(TileID.AlchemyTable);
            recipe.SetResult(this, 5);
            recipe.AddRecipe();
            recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.HolyWater, 5);
            recipe.AddIngredient(ModContent.ItemType<BloodOrb>(), 50);
            recipe.AddTile(TileID.AlchemyTable);
            recipe.SetResult(this, 5);
            recipe.AddRecipe();
        }
    }
}
