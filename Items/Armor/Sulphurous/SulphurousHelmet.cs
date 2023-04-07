﻿using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Armor.Sulphurous
{
    [AutoloadEquip(EquipType.Head)]
    [LegacyName("SulfurHelmet")]
    public class SulphurousHelmet : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
            // DisplayName.SetDefault("Sulphurous Helmet");
            /* Tooltip.SetDefault("4% increased rogue damage\n" +
                "2% increased rogue critical strike chance\n" +
                "Grants underwater breathing"); */
        }

        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 26;
            Item.value = CalamityGlobalItem.Rarity2BuyPrice;
            Item.rare = ItemRarityID.Green;
            Item.defense = 5;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<SulphurousBreastplate>() && legs.type == ModContent.ItemType<SulphurousLeggings>();
        }

        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = "+70 maximum stealth\n" +
				"Attacking and being attacked by enemies inflicts poison\n" +
                "Grants an additional jump that summons a sulphurous bubble\n" +
                "Provides increased underwater mobility and reduces the severity of the sulphuric waters";
            var modPlayer = player.Calamity();
            modPlayer.sulfurSet = true;
            modPlayer.sulfurJump = true;
            modPlayer.rogueStealthMax += 0.7f;
            modPlayer.wearingRogueArmor = true;
            player.ignoreWater = true;
        }

        public override void UpdateEquip(Player player)
        {
            player.GetDamage<ThrowingDamageClass>() += 0.04f;
            player.GetCritChance<ThrowingDamageClass>() += 2;
            player.gills = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<Acidwood>(10).
                AddIngredient<SulphuricScale>(10).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}
