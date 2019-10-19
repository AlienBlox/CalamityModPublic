﻿using Terraria;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.ID;
using CalamityMod.Items.Materials;

namespace CalamityMod.Items.Accessories
{
    public class SigilofCalamitas : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sigil of Calamitas");
            Tooltip.SetDefault("10% increased magic damage and 10% decreased mana usage\n" +
                "Increases pickup range for mana stars and you restore mana when damaged\n" +
                "+100 max mana and reveals treasure locations if visibility is on");
            Main.RegisterItemAnimation(item.type, new DrawAnimationVertical(6, 8));
        }

        public override void SetDefaults()
        {
            item.width = 28;
            item.height = 32;
            item.value = Item.buyPrice(0, 30, 0, 0);
            item.rare = 9;
            item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.magicCuffs = true;
            player.manaMagnet = true;
            if (!hideVisual)
				player.findTreasure = true;
            player.statManaMax2 += 100;
            player.magicDamage += 0.1f;
            player.manaCost *= 0.9f;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.SorcererEmblem);
            recipe.AddIngredient(ItemID.CelestialCuffs);
            recipe.AddIngredient(ItemID.CrystalShard, 20);
            recipe.AddIngredient(ModContent.ItemType<CalamityDust>(), 5);
            recipe.AddIngredient(ModContent.ItemType<CoreofChaos>(), 5);
            recipe.AddIngredient(ModContent.ItemType<CruptixBar>(), 2);
            recipe.AddIngredient(ModContent.ItemType<ChaosAmulet>());
            recipe.AddIngredient(ItemID.UnholyWater, 10);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
            recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.SorcererEmblem);
            recipe.AddIngredient(ItemID.CelestialCuffs);
            recipe.AddIngredient(ItemID.CrystalShard, 20);
            recipe.AddIngredient(ModContent.ItemType<CalamityDust>(), 5);
            recipe.AddIngredient(ModContent.ItemType<CoreofChaos>(), 5);
            recipe.AddIngredient(ModContent.ItemType<CruptixBar>(), 2);
            recipe.AddIngredient(ModContent.ItemType<ChaosAmulet>());
            recipe.AddIngredient(ItemID.BloodWater, 10);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
