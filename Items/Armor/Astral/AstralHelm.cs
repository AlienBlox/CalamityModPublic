﻿using CalamityMod.CalPlayer;
using CalamityMod.Items.Placeables;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Armor.Astral
{
    [AutoloadEquip(EquipType.Head)]
    public class AstralHelm : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.value = CalamityGlobalItem.Rarity9BuyPrice;
            Item.rare = ItemRarityID.Cyan;
            Item.defense = 17; //63
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<AstralBreastplate>() && legs.type == ModContent.ItemType<AstralLeggings>();
        }

        public override void ArmorSetShadows(Player player)
        {
            player.armorEffectDrawShadow = true;
        }

        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = "5% increased movement speed and +3 max minions\n" +
                "35% increased damage and 25% increased critical strike chance\n" +
                "Whenever you crit an enemy, a barrage of stars will rain down\n" +
                "This effect has a 1 second cooldown before it can trigger again";
            var modPlayer = player.Calamity();
            modPlayer.astralStarRain = true;
            player.moveSpeed += 0.05f;
            player.GetDamage<GenericDamageClass>() += 0.35f;
            player.maxMinions += 3;
            player.GetCritChance<GenericDamageClass>() += 25;
            player.Calamity().wearingRogueArmor = true;
        }

        public override void UpdateEquip(Player player)
        {
            player.dangerSense = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<AstralBar>(8).
                AddIngredient(ItemID.MeteoriteBar, 6).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}
