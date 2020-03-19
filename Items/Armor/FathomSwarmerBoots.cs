﻿using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Armor
{
    [AutoloadEquip(EquipType.Legs)]
    public class FathomSwarmerBoots : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Fathom Swarmer Greaves");
            Tooltip.SetDefault("5% increased minion damage\n" +
				"Grants the ability to swim\n" +
                "Speed greatly increased while submerged in liquid");
        }

        public override void SetDefaults()
        {
            item.width = 18;
            item.height = 18;
            item.value = Item.buyPrice(0, 18, 0, 0);
            item.rare = 7;
            item.defense = 15;
        }

        public override void UpdateEquip(Player player)
        {
            if (Collision.DrownCollision(player.position, player.width, player.height, player.gravDir))
            {
                player.moveSpeed += 0.5f;
            }
			player.ignoreWater = true;
            if (player.wingTime <= 0) //ignore flippers while the player can fly
				player.accFlipper = true;
        }
		
		public override void UpdateVanity(Player player, EquipType type)
		{
			player.Calamity().fathomSwarmerTail = true;
		}

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.SpiderGreaves);
            recipe.AddIngredient(ModContent.ItemType<VictideBar>(), 9);
            recipe.AddIngredient(ModContent.ItemType<PlantyMush>(), 8);
            recipe.AddIngredient(ModContent.ItemType<AbyssGravel>(), 15);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}