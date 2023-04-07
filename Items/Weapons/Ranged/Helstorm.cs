﻿using Terraria.DataStructures;
using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class Helstorm : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Helstorm");
            /* Tooltip.SetDefault("Fires two bullets at once\n" +
                "The gun also deals damage to enemies that touch it"); */
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 31;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 50;
            Item.height = 24;
            Item.useTime = 7;
            Item.useAnimation = 7;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 2.5f;
            Item.value = CalamityGlobalItem.Rarity8BuyPrice;
            Item.rare = ItemRarityID.Yellow;
            Item.UseSound = SoundID.Item11;
            Item.autoReuse = true;
            Item.shoot = ProjectileID.PurificationPowder;
            Item.shootSpeed = 11.5f;
            Item.useAmmo = AmmoID.Bullet;
            Item.Calamity().canFirePointBlankShots = true;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-5, 0);
        }

        //Custom melee hitbox
        public override bool? CanHitNPC(Player player, NPC target)
        {
            Rectangle targetHitbox = target.Hitbox;

            float collisionPoint = 0f;
            float gunLength = 60f;
            float gunHeight = 26f;

            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), player.MountedCenter, player.MountedCenter + ((player.itemRotation + (player.direction < 0 ? MathHelper.Pi : 0f)).ToRotationVector2() * gunLength), gunHeight, ref collisionPoint) ? null : false;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            for (int index = 0; index < 2; ++index)
            {
                float SpeedX = velocity.X + (float)Main.rand.Next(-10, 11) * 0.05f;
                float SpeedY = velocity.Y + (float)Main.rand.Next(-10, 11) * 0.05f;
                Projectile.NewProjectile(source, position.X, position.Y, SpeedX, SpeedY, type, damage, knockback, player.whoAmI, 0f, 0f);
            }
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<ScoriaBar>(7).
                AddTile(TileID.MythrilAnvil).
                Register();
        }

        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.OnFire3, 360);
        }

        public override void OnHitPvp(Player player, Player target, Player.HurtInfo hurtInfo)
        {
            target.AddBuff(BuffID.OnFire3, 360);
        }
    }
}
