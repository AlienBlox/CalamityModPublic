﻿using Terraria.DataStructures;
using CalamityMod.Items.Materials;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class BladedgeGreatbow : ModItem, ILocalizedModType
    {
        public string LocalizationCategory => "Items.Weapons.Ranged";
        public override void SetDefaults()
        {
            Item.damage = 35;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 74;
            Item.height = 24;
            Item.useTime = 26;
            Item.useAnimation = 26;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 3.5f;
            Item.value = CalamityGlobalItem.Rarity7BuyPrice;
            Item.rare = ItemRarityID.Lime;
            Item.UseSound = SoundID.Item5;
            Item.autoReuse = true;
            Item.shoot = ProjectileID.PurificationPowder;
            Item.shootSpeed = 14f;
            Item.useAmmo = AmmoID.Arrow;
            Item.Calamity().canFirePointBlankShots = true;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-10, 0);
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            for (int i = 0; i < 4; i++)
            {
                float SpeedX = velocity.X + (float)Main.rand.Next(-60, 61) * 0.05f;
                float SpeedY = velocity.Y + (float)Main.rand.Next(-60, 61) * 0.05f;
                int index = Projectile.NewProjectile(source, position.X, position.Y, SpeedX, SpeedY, type, damage, knockback, player.whoAmI);
                Main.projectile[index].noDropItem = true;
            }
            Vector2 vector2 = player.RotatedRelativePoint(player.MountedCenter, true);
            float PiOver10 = MathHelper.Pi * 0.1f;
            Vector2 vector7 = velocity;
            vector7.Normalize();
            vector7 *= 10f;
            bool flag11 = Collision.CanHit(vector2, 0, 0, vector2 + vector7, 0, 0);
            for (int num119 = 0; num119 < 2; num119++)
            {
                float num120 = (float)num119 - 1f / 2f;
                Vector2 value9 = vector7.RotatedBy((double)(PiOver10 * num120), default);
                if (!flag11)
                {
                    value9 -= vector7;
                }
                int projectile = Projectile.NewProjectile(source, vector2.X + value9.X, vector2.Y + value9.Y, velocity.X, velocity.Y, ProjectileID.Leaf, damage, 0f, player.whoAmI);
                if (projectile.WithinBounds(Main.maxProjectiles))
                    Main.projectile[projectile].DamageType = DamageClass.Ranged;
            }
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<PerennialBar>(12).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
