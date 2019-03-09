﻿using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items;

namespace CalamityMod.Items.Armor
{
    [AutoloadEquip(EquipType.Head)]
    public class GodSlayerHelmet : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("God Slayer Helmet");
            Tooltip.SetDefault("14% increased ranged damage and critical strike chance");
        }

        public override void SetDefaults()
        {
            item.width = 18;
            item.height = 18;
			item.value = Item.buyPrice(0, 75, 0, 0);
			item.defense = 35; //96
			item.GetGlobalItem<CalamityGlobalItem>(mod).postMoonLordRarity = 14;
		}

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == mod.ItemType("GodSlayerChestplate") && legs.type == mod.ItemType("GodSlayerLeggings");
        }

        public override void ArmorSetShadows(Player player)
        {
            player.armorEffectDrawShadow = true;
        }

        public override void UpdateArmorSet(Player player)
        {
            CalamityPlayer modPlayer = player.GetModPlayer<CalamityPlayer>(mod);
            modPlayer.godSlayer = true;
            modPlayer.godSlayerRanged = true;
            player.setBonus = "You will survive fatal damage and will be healed 150 HP if an attack would have killed you\n" +
                "This effect can only occur once every 45 seconds\n" +
                "While the cooldown for this effect is active you gain a 10% increase to all damage\n" +
                "Your ranged critical hits have a chance to critically hit, causing 4 times the damage\n" +
                "You have a chance to fire a god killer shrapnel round while firing ranged weapons";
        }

        public override void UpdateEquip(Player player)
        {
            player.rangedDamage += 0.14f;
            player.rangedCrit += 14;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "CosmiliteBar", 14);
            recipe.AddIngredient(null, "NightmareFuel", 8);
            recipe.AddIngredient(null, "EndothermicEnergy", 8);
            recipe.AddTile(null, "DraedonsForge");
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}