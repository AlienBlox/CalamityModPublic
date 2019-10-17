﻿using CalamityMod.CalPlayer;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Items
{
    [AutoloadEquip(EquipType.Head)]
    public class VictideHelm : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Victide Helm");
            Tooltip.SetDefault("5% increased melee damage");
        }

        public override void SetDefaults()
        {
            item.width = 18;
            item.height = 18;
            item.value = Item.buyPrice(0, 1, 50, 0);
            item.rare = 2;
            item.defense = 4; //11
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<VictideBreastplate>() && legs.type == ModContent.ItemType<VictideLeggings>();
        }

        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = "Increased life regen and melee damage while submerged in liquid\n" +
                    "When using any weapon you have a 10% chance to throw a returning seashell projectile\n" +
                    "This seashell does true damage and does not benefit from any damage class\n" +
                    "Slightly reduces breath loss in the abyss";
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.victideSet = true;
            player.ignoreWater = true;
            if (Collision.DrownCollision(player.position, player.width, player.height, player.gravDir))
            {
                player.meleeDamage += 0.1f;
                player.lifeRegen += 3;
            }
        }

        public override void UpdateEquip(Player player)
        {
            player.meleeDamage += 0.05f;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "VictideBar", 3);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
