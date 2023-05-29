﻿using CalamityMod.Projectiles.Magic;
using CalamityMod.Rarities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Magic
{
    [LegacyName("Vehemenc")]
    public class Vehemence : ModItem, ILocalizedModType
    {
        public string LocalizationCategory => "Items.Weapons.Magic";
        public const int BaseDamage = 5185;
        public const float SkullRatio = 0.08f;

        public override void SetStaticDefaults()
        {
            Item.staff[Item.type] = true;
        }

        public override void SetDefaults()
        {
            Item.damage = BaseDamage;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 41;
            Item.width = 44;
            Item.height = 44;
            Item.useTime = Item.useAnimation = 43;
            Item.noUseGraphic = true;
            Item.channel = true;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 5.75f;
            Item.UseSound = SoundID.Item73;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<VehemenceHoldout>();
            Item.shootSpeed = 16f;

            Item.rare = ModContent.RarityType<Violet>();
            Item.value = CalamityGlobalItem.Rarity15BuyPrice;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Item v = player.ActiveItem();
            int chargeTime = 2 * v.useAnimation;
            Projectile.NewProjectile(source, position, Vector2.Zero, type, damage, knockback, player.whoAmI, 0f, chargeTime);
            return false;
        }

        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[Item.shoot] <= 0;
    }
}
