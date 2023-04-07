﻿using Terraria.DataStructures;
using CalamityMod.Projectiles.Ranged;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class FlakToxicannon : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Flak Toxicannon");
            /* Tooltip.SetDefault("Fires angled shots in the direction of the cursor\n" +
                               "Can only be shot in a cone direction above the player\n" +
                               "High IQ required"); */
            Item.ResearchUnlockCount = 1;
        }
        public override void SetDefaults()
        {
            Item.damage = 60;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 88;
            Item.height = 28;
            Item.useTime = Item.useAnimation = 8;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useTurn = false;
            Item.noMelee = true;
            Item.knockBack = 7f;
            Item.value = CalamityGlobalItem.Rarity5BuyPrice;
            Item.rare = ItemRarityID.Pink;
            Item.UseSound = SoundID.Item109;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<ToxicannonShot>();
            Item.shootSpeed = 16f;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            float angle = velocity.ToRotation() + MathHelper.PiOver2;
            if (angle <= -MathHelper.PiOver4 || angle >= MathHelper.PiOver4)
                return false;

            Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<ToxicannonShot>(), damage, knockback, player.whoAmI);
            return false;
        }
    }
}
