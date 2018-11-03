﻿using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items.Armor;

namespace CalamityMod.Items.Armor
{
    [AutoloadEquip(EquipType.Head)]
    public class BloodflareMask : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Bloodflare Mask");
            Tooltip.SetDefault("You can move freely through liquids and you are immune to lava\n" +
                "10% increased melee damage and critical strike chance");
        }

        public override void SetDefaults()
        {
            item.width = 18;
            item.height = 18;
            item.value = 1750000;
            item.defense = 49; //85
        }

        public override void ModifyTooltips(List<TooltipLine> list)
        {
            foreach (TooltipLine line2 in list)
            {
                if (line2.mod == "Terraria" && line2.Name == "ItemName")
                {
                    line2.overrideColor = new Color(0, 255, 0);
                }
            }
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == mod.ItemType("BloodflareBodyArmor") && legs.type == mod.ItemType("BloodflareCuisses");
        }

        public override void ArmorSetShadows(Player player)
        {
            player.armorEffectDrawShadowSubtle = true;
        }

        public override void UpdateArmorSet(Player player)
        {
            CalamityPlayer modPlayer = player.GetModPlayer<CalamityPlayer>(mod);
            modPlayer.bloodflareSet = true;
            modPlayer.bloodflareMelee = true;
            player.setBonus = "Greatly increases life regen\n" +
                "Enemies are more likely to target you\n" +
                "Enemies below 50% life have a chance to drop hearts when struck\n" +
                "Enemies above 50% life have a chance to drop mana stars when struck\n" +
                "Enemies killed during a Blood Moon have a much higher chance to drop Blood Orbs\n" +
                "True melee strikes will heal you\n" +
                "After striking an enemy 15 times with true melee you will enter a blood frenzy for 5 seconds\n" +
                "During this you will gain 25% increased melee damage, critical strike chance, and contact damage is halved\n" +
                "This effect has a 30 second cooldown";
            player.crimsonRegen = true;
            player.aggro += 900;
        }

        public override void UpdateEquip(Player player)
        {
            player.lavaImmune = true;
            player.ignoreWater = true;
            player.meleeDamage += 0.1f;
            player.meleeCrit += 10;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "BloodstoneCore", 11);
            recipe.AddIngredient(null, "RuinousSoul", 2);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}