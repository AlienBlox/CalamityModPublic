﻿using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Items
{
    [AutoloadEquip(EquipType.Head)]
    public class WulfrumHelm : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Wulfrum Helm");
            Tooltip.SetDefault("3% increased melee damage");
        }

        public override void SetDefaults()
        {
            item.width = 18;
            item.height = 18;
            item.value = Item.buyPrice(0, 0, 75, 0);
            item.rare = 1;
            item.defense = 3; //8
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<WulfrumArmor>() && legs.type == ModContent.ItemType<WulfrumLeggings>();
        }

        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = "+3 defense\n" +
                "+5 defense when below 50% life";
            player.statDefense += 3; //11
            if (player.statLife <= (int)((double)player.statLifeMax2 * 0.5))
            {
                player.statDefense += 5; //16
            }
        }

        public override void UpdateEquip(Player player)
        {
            player.meleeDamage += 0.03f;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<WulfrumShard>(), 5);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
