﻿using CalamityMod.CalPlayer;
using CalamityMod.CalPlayer.Dashes;
using CalamityMod.Items.Materials;
using CalamityMod.Rarities;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Items.Armor.GodSlayer
{
    [AutoloadEquip(EquipType.Head)]
    [LegacyName("GodSlayerHelm")]
    public class GodSlayerHeadMelee : ModItem, ILocalizedModType
    {
        public string LocalizationCategory => "Items.Armor";
        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.value = CalamityGlobalItem.Rarity14BuyPrice;
            Item.defense = 48; //96
            Item.rare = ModContent.RarityType<DarkBlue>();
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<GodSlayerChestplate>() && legs.type == ModContent.ItemType<GodSlayerLeggings>();
        }

        public override void ArmorSetShadows(Player player)
        {
            player.armorEffectDrawShadow = true;
        }

        public override void UpdateArmorSet(Player player)
        {
            var modPlayer = player.Calamity();
            modPlayer.godSlayer = true;
            modPlayer.godSlayerDamage = true;
            var hotkey = CalamityKeybinds.GodSlayerDashHotKey.TooltipHotkeyString();
            player.setBonus = "Allows you to dash for an immense distance in 8 directions\n" +
            "Press " + hotkey + " while holding down the movement keys in the direction you want to dash\n" +
            "Enemies you dash through take massive damage\n" +
            "During the dash you are immune to most debuffs\n" +
            "The dash has a " + GodslayerArmorDash.GodslayerCooldown + " second cooldown\n" +
            "Enemies are more likely to target you\n" +
            "Taking over 80 damage in one hit will cause you to release a swarm of high-damage god killer darts\n" +
            "Enemies take a lot of damage when they hit you\n" +
            "An attack that would deal 80 damage or less will have its damage reduced to 1";
            player.thorns += 2.5f;
            player.aggro += 1000;

            if (modPlayer.godSlayerDashHotKeyPressed || (player.dashDelay != 0 && modPlayer.LastUsedDashID == GodslayerArmorDash.ID))
            {
                modPlayer.DeferredDashID = GodslayerArmorDash.ID;
                player.dash = 0;
            }
        }

        public override void UpdateEquip(Player player)
        {
            player.GetDamage<MeleeDamageClass>() += 0.14f;
            player.GetCritChance<MeleeDamageClass>() += 14;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<CosmiliteBar>(7).
                AddIngredient<AscendantSpiritEssence>(2).
                AddTile<CosmicAnvil>().
                Register();
        }
    }
}
