﻿using Terraria.DataStructures;
using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Ranged;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class Shredder : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Shredder");
            Tooltip.SetDefault("The myth, the legend, the weapon that drops more frames than any other\n" +
                "Fires a barrage of energy bolts that split and bounce\n" +
                "Right click to fire a barrage of normal bullets");
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 27;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 56;
            Item.height = 24;
            Item.useTime = 4;
            Item.reuseDelay = 12;
            Item.useAnimation = 32;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 1.5f;
            Item.value = CalamityGlobalItem.Rarity11BuyPrice;
            Item.rare = ItemRarityID.Purple;
            Item.UseSound = SoundID.Item31;
            Item.autoReuse = true;
            Item.shoot = ProjectileID.Bullet;
            Item.shootSpeed = 12f;
            Item.useAmmo = AmmoID.Bullet;
            Item.Calamity().canFirePointBlankShots = true;
        }

        public override Vector2? HoldoutOffset() => new Vector2(-5, 0);

        public override bool AltFunctionUse(Player player) => true;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int bulletAmt = 4;
            if (player.altFunctionUse == 2)
            {
                for (int index = 0; index < bulletAmt; ++index)
                {
                    float SpeedX = velocity.X + Main.rand.Next(-30, 31) * 0.05f;
                    float SpeedY = velocity.Y + Main.rand.Next(-30, 31) * 0.05f;
                    int shot = Projectile.NewProjectile(source, position.X, position.Y, SpeedX, SpeedY, type, damage, knockback, player.whoAmI, 0f, 0f);
                    Main.projectile[shot].timeLeft = 180;
                }
                return false;
            }
            else
            {
                for (int index = 0; index < bulletAmt; ++index)
                {
                    float SpeedX = velocity.X + Main.rand.Next(-30, 31) * 0.05f;
                    float SpeedY = velocity.Y + Main.rand.Next(-30, 31) * 0.05f;
                    int shredderBoltDamage = (int)(0.85f * damage);
                    int shot = Projectile.NewProjectile(source, position.X, position.Y, SpeedX, SpeedY, ModContent.ProjectileType<ChargedBlast>(), shredderBoltDamage, knockback, player.whoAmI, 0f, 0f);
                    Main.projectile[shot].timeLeft = 180;
                }
                return false;
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<ChargedDartRifle>().
                AddIngredient<FrostbiteBlaster>().
                AddIngredient(ItemID.Shotgun).
                AddIngredient<GalacticaSingularity>(5).
                AddIngredient<BarofLife>(5).
                AddIngredient(ItemID.LunarBar, 5).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}
