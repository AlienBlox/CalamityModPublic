﻿using System;
using CalamityMod.Items.Materials;
using CalamityMod.Particles;
using CalamityMod.Projectiles;
using Microsoft.CodeAnalysis;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class AGunofFireAndIce : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Ranged";
        public bool swapType = false;
        public override void SetDefaults()
        {
            Item.width = 56;
            Item.height = 36;
            Item.scale = 0.75f;
            Item.damage = 45;
            Item.DamageType = DamageClass.Ranged;
            Item.useTime = 16;
            Item.useAnimation = 16;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 2.25f;
            Item.value = CalamityGlobalItem.Rarity4BuyPrice;
            Item.rare = ItemRarityID.LightRed;
            Item.UseSound = SoundID.Item41;
            Item.autoReuse = true;
            Item.shootSpeed = 12f;
            Item.shoot = ProjectileID.PurificationPowder;
            Item.useAmmo = AmmoID.Bullet;
            Item.Calamity().canFirePointBlankShots = true;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            float itemRotation = player.compositeFrontArm.rotation + MathHelper.PiOver2 * player.gravDir;

            Vector2 itemPosition = player.MountedCenter + itemRotation.ToRotationVector2() * 7f;

            // Ice bullet
            if (!swapType)
            {
                for (int k = 0; k < 8; k++)
                {
                    CritSpark spark = new CritSpark(itemPosition + velocity.RotatedBy(-0.6 * player.direction) + velocity * 0.5f, velocity.RotatedByRandom(0.25) * Main.rand.NextFloat(0.2f, 1.5f), Main.rand.NextBool() ? Color.DeepSkyBlue : Color.LightSkyBlue, Color.White, Main.rand.NextFloat(0.3f, 0.7f), Main.rand.Next(10, 15 + 1), Main.rand.NextFloat(-2f, 2f), 1.5f);
                    GeneralParticleHandler.SpawnParticle(spark);
                }
                Projectile iceShot = Projectile.NewProjectileDirect(source, itemPosition + velocity.RotatedBy(-0.6 * player.direction) - velocity * 0.5f, velocity, type, damage, knockback, player.whoAmI);
                CalamityGlobalProjectile cgp = iceShot.Calamity();
                cgp.iceBullet = true;
            }
            // Fire bullet
            if (swapType)
            {
                for (int k = 0; k < 8; k++)
                {
                    CritSpark spark = new CritSpark(itemPosition + velocity.RotatedBy(-0.6 * player.direction) + velocity * 0.5f, velocity.RotatedByRandom(0.25) * Main.rand.NextFloat(0.2f, 1.5f), Main.rand.NextBool() ? Color.Orange : Color.OrangeRed, Color.Yellow, Main.rand.NextFloat(0.3f, 0.7f), Main.rand.Next(10, 15 + 1), Main.rand.NextFloat(-2f, 2f), 1.5f);
                    GeneralParticleHandler.SpawnParticle(spark);
                }
                Projectile fireShot = Projectile.NewProjectileDirect(source, itemPosition + velocity.RotatedBy(-0.6 * player.direction) - velocity * 0.5f, velocity, type, damage, knockback, player.whoAmI);
                CalamityGlobalProjectile cgp = fireShot.Calamity();
                cgp.fireBullet = true;
            }

            swapType = !swapType;
            return false;
        }
        public override void UseStyle(Player player, Rectangle heldItemFrame)
        {
            player.ChangeDir(Math.Sign((player.Calamity().mouseWorld - player.Center).X));
            float itemRotation = player.compositeFrontArm.rotation + MathHelper.PiOver2 * player.gravDir;

            Vector2 itemPosition = player.MountedCenter + itemRotation.ToRotationVector2() * 7f;
            Vector2 itemSize = new Vector2(56, 36);
            Vector2 itemOrigin = new Vector2(-24, 3);

            CalamityUtils.CleanHoldStyle(player, itemRotation, itemPosition, itemSize, itemOrigin);

            base.UseStyle(player, heldItemFrame);
        }

        public override void UseItemFrame(Player player)
        {
            player.ChangeDir(Math.Sign((player.Calamity().mouseWorld - player.Center).X));

            float animProgress = 0.5f - player.itemTime / (float)player.itemTimeMax;
            float rotation = (player.Center - player.Calamity().mouseWorld).ToRotation() * player.gravDir + MathHelper.PiOver2;
            if (animProgress < 0.4f)
                rotation += -0.05f * (float)Math.Pow((0.6f - animProgress) / 0.6f, 2) * player.direction;

            player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, rotation);
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.PhoenixBlaster).
                AddIngredient<EssenceofEleum>(5).
                AddIngredient<EssenceofHavoc>(5).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}
