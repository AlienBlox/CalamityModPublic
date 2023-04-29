﻿using CalamityMod.Buffs.Summon;
using CalamityMod.CalPlayer;
using CalamityMod.Projectiles.Summon;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework;

namespace CalamityMod.Items.Accessories
{
    public class EyeoftheStorm : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Eye of the Storm");
            // Tooltip.SetDefault("Summons a cloud elemental to fight for you");
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 26;
            Item.value = CalamityGlobalItem.Rarity5BuyPrice;
            Item.rare = ItemRarityID.Pink;
            Item.accessory = true;
        }

        public override bool CanEquipAccessory(Player player, int slot, bool modded)
        {
            CalamityPlayer modPlayer = player.Calamity();
            if (modPlayer.elementalHeart)
            {
                return false;
            }
            return true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.cloudWaifu = true;
            if (player.whoAmI == Main.myPlayer)
            {
                if (player.FindBuffIndex(ModContent.BuffType<CloudyWaifu>()) == -1)
                {
                    player.AddBuff(ModContent.BuffType<CloudyWaifu>(), 3600, true);
                }
                if (player.ownedProjectileCounts[ModContent.ProjectileType<CloudElementalMinion>()] < 1)
                {
                    var source = player.GetSource_Accessory(Item);
                    int damage = (int)player.GetTotalDamage<SummonDamageClass>().ApplyTo(45);
                    var p = Projectile.NewProjectileDirect(source, player.Center, -Vector2.UnitY, ModContent.ProjectileType<CloudElementalMinion>(), damage, 2f, Main.myPlayer, 0f, 0f);
                    p.originalDamage = 45;
                }
            }
        }

        public override void UpdateVanity(Player player)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.cloudWaifuVanity = true;
            if (player.whoAmI == Main.myPlayer)
            {
                if (player.FindBuffIndex(ModContent.BuffType<CloudyWaifu>()) == -1)
                {
                    player.AddBuff(ModContent.BuffType<CloudyWaifu>(), 3600, true);
                }
                if (player.ownedProjectileCounts[ModContent.ProjectileType<CloudElementalMinion>()] < 1)
                {
                    var source = player.GetSource_Accessory(Item);
                    int damage = (int)player.GetTotalDamage<SummonDamageClass>().ApplyTo(45);
                    var p = Projectile.NewProjectileDirect(source, player.Center, -Vector2.UnitY, ModContent.ProjectileType<CloudElementalMinion>(), damage, 2f, Main.myPlayer, 0f, 0f);
                    p.originalDamage = 45;
                }
            }
        }
    }
}
