﻿using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items.CalamityCustomThrowingDamage;

namespace CalamityMod.Items.Armor
{
    [AutoloadEquip(EquipType.Head)]
    public class WulfrumMask : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Wulfrum Mask");
            Tooltip.SetDefault("3% increased rogue damage");
        }

        public override void SetDefaults()
        {
            item.width = 18;
            item.height = 18;
			item.value = Item.buyPrice(0, 0, 75, 0);
			item.rare = 1;
            item.defense = 1; //6
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == mod.ItemType("WulfrumArmor") && legs.type == mod.ItemType("WulfrumLeggings");
        }

        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = "+3 defense\n" +
                "+5 defense when below 50% life";
            player.statDefense += 3; //9
            if (player.statLife <= (player.statLifeMax2 * 0.5f))
            {
                player.statDefense += 5; //14
            }
        }

        public override void UpdateEquip(Player player)
        {
            CalamityCustomThrowingDamagePlayer.ModPlayer(player).throwingDamage += 0.03f;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "WulfrumShard", 5);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
