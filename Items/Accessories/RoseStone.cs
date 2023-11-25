﻿using CalamityMod.Buffs.Summon;
using CalamityMod.CalPlayer;
using CalamityMod.Projectiles.Summon;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using CalamityMod.Items.Potions.Alcohol;

namespace CalamityMod.Items.Accessories
{
    public class RoseStone : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Accessories";
        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
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
            modPlayer.brimstoneWaifu = true;
            if (player.whoAmI == Main.myPlayer)
            {
                var source = player.GetSource_Accessory(Item);
                if (player.FindBuffIndex(ModContent.BuffType<BrimstoneWaifu>()) == -1)
                {
                    player.AddBuff(ModContent.BuffType<BrimstoneWaifu>(), 3600, true);
                }
                if (player.ownedProjectileCounts[ModContent.ProjectileType<BrimstoneElementalMinion>()] < 1)
                {
                    int baseDamage = 60;
                    if (modPlayer.oldFashioned)
                        baseDamage = (int)(baseDamage * OldFashioned.AccessoryAndSetBonusDamageMultiplier);

                    int damage = (int)player.GetTotalDamage<SummonDamageClass>().ApplyTo(baseDamage);
                    int p = Projectile.NewProjectile(source, player.Center.X, player.Center.Y, 0f, -1f, ModContent.ProjectileType<BrimstoneElementalMinion>(), damage, 2f, Main.myPlayer, 0f, 0f);
                    if (Main.projectile.IndexInRange(p))
                        Main.projectile[p].originalDamage = baseDamage;
                }
            }
        }

        public override void UpdateVanity(Player player)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.brimstoneWaifuVanity = true;
            if (player.whoAmI == Main.myPlayer)
            {
                var source = player.GetSource_Accessory(Item);
                if (player.FindBuffIndex(ModContent.BuffType<BrimstoneWaifu>()) == -1)
                {
                    player.AddBuff(ModContent.BuffType<BrimstoneWaifu>(), 3600, true);
                }
                if (player.ownedProjectileCounts[ModContent.ProjectileType<BrimstoneElementalMinion>()] < 1)
                {
                    int baseDamage = 60;
                    if (modPlayer.oldFashioned)
                        baseDamage = (int)(baseDamage * OldFashioned.AccessoryAndSetBonusDamageMultiplier);

                    int damage = (int)player.GetTotalDamage<SummonDamageClass>().ApplyTo(baseDamage);
                    int p = Projectile.NewProjectile(source, player.Center.X, player.Center.Y, 0f, -1f, ModContent.ProjectileType<BrimstoneElementalMinion>(), damage, 2f, Main.myPlayer, 0f, 0f);
                    if (Main.projectile.IndexInRange(p))
                        Main.projectile[p].originalDamage = baseDamage;
                }
            }
        }
    }
}
