﻿using CalamityMod.Buffs.Summon;
using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables;
using CalamityMod.Projectiles.Summon;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace CalamityMod.Items.Armor.Mollusk
{
    [AutoloadEquip(EquipType.Head)]
    public class MolluskShellmet : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
            // DisplayName.SetDefault("Mollusk Shellmet");
            /* Tooltip.SetDefault("5% increased damage and 4% increased critical strike chance\n" +
                               "You can move freely through liquids"); */
        }

        public override void SetDefaults()
        {
            Item.width = 22;
            Item.height = 22;
            Item.value = CalamityGlobalItem.Rarity5BuyPrice;
            Item.rare = ItemRarityID.Pink;
            Item.defense = 18;
        }

        public override void UpdateEquip(Player player)
        {
            player.ignoreWater = true;
            player.GetDamage<GenericDamageClass>() += 0.05f;
            player.GetCritChance<GenericDamageClass>() += 4;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<MolluskShellplate>() && legs.type == ModContent.ItemType<MolluskShelleggings>();
        }

        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = "Two shellfishes aid you in combat\n" +
                              "10% increased damage\n" +
                              "Your horizontal movement is slowed";
            var modPlayer = player.Calamity();
            player.GetDamage<GenericDamageClass>() += 0.1f;
            modPlayer.molluskSet = true;
            player.maxMinions += 4;
            if (player.whoAmI == Main.myPlayer)
            {
                var source = player.GetSource_ItemUse(Item);
                if (player.FindBuffIndex(ModContent.BuffType<ShellfishBuff>()) == -1)
                {
                    player.AddBuff(ModContent.BuffType<ShellfishBuff>(), 3600, true);
                }
                if (player.ownedProjectileCounts[ModContent.ProjectileType<Shellfish>()] < 2)
                {
                    Projectile clam = Projectile.NewProjectileDirect(source, player.Center, -Vector2.UnitY, ModContent.ProjectileType<Shellfish>(), 140, 0f, player.whoAmI);
                    clam.originalDamage = 140;
                }
            }
            player.Calamity().wearingRogueArmor = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<MolluskHusk>(6).
                AddIngredient<SeaPrism>(15).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}
