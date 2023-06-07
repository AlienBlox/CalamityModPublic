﻿using CalamityMod.Buffs.Summon;
using CalamityMod.CalPlayer;
using CalamityMod.Projectiles.Summon;
using CalamityMod.Rarities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    public class MutatedTruffle : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Accessories";
        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 26;
            Item.value = CalamityGlobalItem.Rarity13BuyPrice;
            Item.rare = ModContent.RarityType<PureGreen>();
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.miniOldDuke = true;
            if (player.whoAmI == Main.myPlayer)
            {
                var source = player.GetSource_Accessory(Item);
                if (player.FindBuffIndex(ModContent.BuffType<MiniOldDukeBuff>()) == -1)
                {
                    player.AddBuff(ModContent.BuffType<MiniOldDukeBuff>(), 3600, true);
                }
                if (player.ownedProjectileCounts[ModContent.ProjectileType<YoungDuke>()] < 1)
                {
                    const int baseDamage = 1200;
                    int damage = (int)player.GetTotalDamage<SummonDamageClass>().ApplyTo(baseDamage);
                    var duke = Projectile.NewProjectileDirect(source, player.Center, Vector2.Zero,
                        ModContent.ProjectileType<YoungDuke>(),
                        damage,
                        6.5f, Main.myPlayer, 0f, 0f);

                    duke.originalDamage = baseDamage;
                }
            }
        }

        public override void UpdateVanity(Player player)
        {
            player.Calamity().miniOldDukeVanity = true;
            if (player.whoAmI == Main.myPlayer)
            {
                var source = player.GetSource_Accessory(Item);
                if (player.FindBuffIndex(ModContent.BuffType<MiniOldDukeBuff>()) == -1)
                {
                    player.AddBuff(ModContent.BuffType<MiniOldDukeBuff>(), 3600, true);
                }
                if (player.ownedProjectileCounts[ModContent.ProjectileType<YoungDuke>()] < 1)
                {
                    const int baseDamage = 1200;
                    int damage = (int)player.GetTotalDamage<SummonDamageClass>().ApplyTo(baseDamage);
                    var duke = Projectile.NewProjectileDirect(source, player.Center, Vector2.Zero,
                        ModContent.ProjectileType<YoungDuke>(),
                        damage,
                        6.5f, Main.myPlayer, 0f, 0f);

                    duke.originalDamage = baseDamage;
                }
            }
        }
    }
}
