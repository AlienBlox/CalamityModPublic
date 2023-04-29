﻿using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Armor.Sulphurous
{
    [AutoloadEquip(EquipType.Legs)]
    [LegacyName("SulfurLeggings")]
    public class SulphurousLeggings : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Sulphurous Leggings");
            /* Tooltip.SetDefault("Movement speed increased by 10%\n" +
                "Movement speed increased by 35% while submerged in liquid"); */
        }

        public override void SetDefaults()
        {
            Item.width = 22;
            Item.height = 16;
            Item.value = CalamityGlobalItem.Rarity2BuyPrice;
            Item.defense = 5;
            Item.rare = ItemRarityID.Green;
        }

        public override void UpdateEquip(Player player)
        {
            player.moveSpeed += Collision.DrownCollision(player.position, player.width, player.height, player.gravDir) ? 0.35f : 0.1f;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<Acidwood>(15).
                AddIngredient<SulphuricScale>(15).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}
