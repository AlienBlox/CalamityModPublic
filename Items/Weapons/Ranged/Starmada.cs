﻿using Terraria.DataStructures;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Ores;
using CalamityMod.Projectiles.Ranged;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
    [LegacyName("StarfleetMK2")]
    public class Starmada : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Starmada");
            Tooltip.SetDefault("Fires a barrage of stars and plasma blasts");
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 135;
            Item.knockBack = 15f;
            Item.shootSpeed = 16f;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useAnimation = 27;
            Item.useTime = 27;
            Item.reuseDelay = 0;
            Item.width = 122;
            Item.height = 50;
            Item.UseSound = SoundID.Item92;
            Item.shoot = ModContent.ProjectileType<StarfleetMK2Gun>();
            Item.value = Item.buyPrice(1, 80, 0, 0);
            Item.rare = ItemRarityID.Red;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.DamageType = DamageClass.Ranged;
            Item.channel = true;
            Item.useTurn = false;
            Item.useAmmo = AmmoID.FallenStar;
            Item.autoReuse = true;
            Item.Calamity().customRarity = CalamityRarity.DarkBlue;
        }

        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[Item.shoot] <= 0;

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-10, 0);
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(source, position.X, position.Y, velocity.X, velocity.Y, ModContent.ProjectileType<StarfleetMK2Gun>(), 0, 0f, player.whoAmI);
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<AquashardShotgun>().
                AddIngredient<Starfleet>().
                AddIngredient<StarSputter>().
                AddIngredient<ExodiumCluster>(15).
                AddIngredient<CosmiliteBar>(8).
                AddIngredient<DarksunFragment>(8).
                AddTile(ModContent.TileType<CosmicAnvil>()).
                Register();
        }
    }
}
