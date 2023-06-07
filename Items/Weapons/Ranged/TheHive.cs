﻿using Terraria.DataStructures;
using CalamityMod.Projectiles.Ranged;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class TheHive : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Ranged";
        public override void SetDefaults()
        {
            Item.damage = 80;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 62;
            Item.height = 30;
            Item.useTime = 21;
            Item.useAnimation = 21;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 3.5f;
            Item.value = CalamityGlobalItem.Rarity8BuyPrice;
            Item.rare = ItemRarityID.Yellow;
            Item.UseSound = SoundID.Item61;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<BeeRPG>();
            Item.shootSpeed = 13f;
            Item.useAmmo = AmmoID.Rocket;
        }

        public override Vector2? HoldoutOffset() => new Vector2(-10, 0);

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int rocket = Utils.SelectRandom(Main.rand, new int[]
            {
                ModContent.ProjectileType<GoliathRocket>(),
                ModContent.ProjectileType<HiveMissile>(),
                ModContent.ProjectileType<HiveBomb>(),
                ModContent.ProjectileType<BeeRPG>()
            });
            Projectile.NewProjectile(source, position.X, position.Y, velocity.X, velocity.Y, rocket, damage, knockback, player.whoAmI, 0f, 0f);
            return false;
        }
    }
}
