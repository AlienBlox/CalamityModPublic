﻿using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items.CalamityCustomThrowingDamage;

namespace CalamityMod.Items.Armor
{
    [AutoloadEquip(EquipType.Body)]
    public class VictideBreastplate : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Victide Breastplate");
            Tooltip.SetDefault("5% increased damage reduction and critical strike chance\n" +
                "Defense increased while submerged in liquid");
        }

        public override void SetDefaults()
        {
            item.width = 18;
            item.height = 18;
			item.value = Item.buyPrice(0, 1, 0, 0);
			item.rare = 2;
            item.defense = 5; //9
        }

        public override void UpdateEquip(Player player)
        {
            player.endurance += 0.05f;
            player.meleeCrit += 5;
            player.rangedCrit += 5;
            player.Calamity().throwingCrit += 5;
            player.magicCrit += 5;
            if (Collision.DrownCollision(player.position, player.width, player.height, player.gravDir))
            {
                player.statDefense += 5;
                player.endurance += 0.1f;
            }
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "VictideBar", 5);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
