﻿using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Armor.Umbraphile
{
    [AutoloadEquip(EquipType.Legs)]
    public class UmbraphileBoots : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 22;
            Item.height = 18;
            Item.value = CalamityGlobalItem.RarityLimeBuyPrice;
            Item.rare = ItemRarityID.Lime;
            Item.defense = 12;
        }

        public override void UpdateEquip(Player player)
        {
            player.moveSpeed += 0.2f;
            player.GetDamage<ThrowingDamageClass>() += 0.09f;
            player.GetCritChance<ThrowingDamageClass>() += 6;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<SolarVeil>(14).
                AddIngredient(ItemID.HallowedBar, 11).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
