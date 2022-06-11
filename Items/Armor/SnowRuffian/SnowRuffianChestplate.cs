﻿using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Armor.SnowRuffian
{
    [AutoloadEquip(EquipType.Body)]
    public class SnowRuffianChestplate : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
            DisplayName.SetDefault("Snow Ruffian Chestplate");
            Tooltip.SetDefault("3% increased rogue critical strike chance");
        }

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.value = Item.buyPrice(0, 0, 75, 0);
            Item.rare = ItemRarityID.Blue;
            Item.defense = 2; //4
        }

        public override void UpdateEquip(Player player)
        {
            player.GetCritChance<ThrowingDamageClass>() += 3;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddRecipeGroup("AnySnowBlock", 30).
                AddRecipeGroup("AnyIceBlock", 15).
                AddIngredient(ItemID.BorealWood, 45).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}