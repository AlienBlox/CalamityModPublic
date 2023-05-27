﻿using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Magic;
using CalamityMod.Rarities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Magic
{
    public class MagneticMeltdown : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.staff[Item.type] = true;
        }

        public override void SetDefaults()
        {
            Item.damage = 100;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 40;
            Item.width = 78;
            Item.height = 78;
            Item.useTime = 49;
            Item.useAnimation = 49;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 4f;
            Item.UseSound = SoundID.Item20;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<MagneticOrb>();
            Item.shootSpeed = 12f;

            Item.value = CalamityGlobalItem.Rarity12BuyPrice;
            Item.rare = ModContent.RarityType<Turquoise>();
        }

        
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Vector2 v = velocity;
            float offset = 3f;

            // Fire four orbs at once
            Projectile.NewProjectile(source, position, v + offset * Vector2.UnitX, type, damage, knockback, player.whoAmI, 1f);
            Projectile.NewProjectile(source, position, v - offset * Vector2.UnitX, type, damage, knockback, player.whoAmI, 1f);
            Projectile.NewProjectile(source, position, v + offset * Vector2.UnitY, type, damage, knockback, player.whoAmI, 1f);
            Projectile.NewProjectile(source, position, v - offset * Vector2.UnitY, type, damage, knockback, player.whoAmI, 1f);
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.SpectreStaff).
                AddIngredient(ItemID.MagnetSphere).
                AddIngredient<DarkPlasma>(3).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}
