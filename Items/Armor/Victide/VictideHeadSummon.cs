﻿using CalamityMod.Buffs.Summon;
using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Summon;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Armor.Victide
{
    [AutoloadEquip(EquipType.Head)]
    [LegacyName("VictideHelmet")]
    public class VictideHeadSummon : ModItem, ILocalizedModType
    {
        public string LocalizationCategory => "Items.Armor";

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.value = CalamityGlobalItem.Rarity2BuyPrice;
            Item.rare = ItemRarityID.Green;
            Item.defense = 1; //8
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<VictideBreastplate>() && legs.type == ModContent.ItemType<VictideGreaves>();
        }

        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = "+1 max minion\n" +
                "Summons a sea snail to protect you\n" +
                "When using any weapon you have a 10% chance to throw a returning seashell projectile\n" +
                "This seashell does true damage and does not benefit from any damage class\n" +
                "Provides increased underwater mobility and slightly reduces breath loss in the abyss\n" +
                "+3 life regen and 10% increased minion damage while submerged in liquid";
            var modPlayer = player.Calamity();
            modPlayer.victideSet = true;
            modPlayer.victideSummoner = true;
            player.maxMinions++;
            if (player.whoAmI == Main.myPlayer)
            {
                if (player.FindBuffIndex(ModContent.BuffType<SeaSnailBuff>()) == -1)
                {
                    player.AddBuff(ModContent.BuffType<SeaSnailBuff>(), 3600, true);
                }
                var source = player.GetSource_ItemUse(Item);
                if (player.ownedProjectileCounts[ModContent.ProjectileType<VictideSeaSnail>()] < 1)
                {
                    var baseDamage = 7;
                    var minionDamage = (int)player.GetTotalDamage<SummonDamageClass>().ApplyTo(baseDamage);
                    var p = Projectile.NewProjectile(source, player.Center, -Vector2.UnitY, ModContent.ProjectileType<VictideSeaSnail>(), minionDamage, 0f, Main.myPlayer, 0f, 0f);
                    if (Main.projectile.IndexInRange(p))
                        Main.projectile[p].originalDamage = baseDamage;
                }
            }
            player.ignoreWater = true;
            if (Collision.DrownCollision(player.position, player.width, player.height, player.gravDir))
            {
                player.GetDamage<SummonDamageClass>() += 0.1f;
                player.lifeRegen += 3;
            }
        }

        public override void UpdateEquip(Player player)
        {
            player.GetDamage<SummonDamageClass>() += 0.1f;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<SeaRemains>(3).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}
