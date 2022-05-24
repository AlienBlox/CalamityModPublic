﻿using Terraria.DataStructures;
using Terraria.DataStructures;
using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Ranged;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class ScorchedEarth : ModItem
    {
        public static readonly SoundStyle ShootSound = new("CalamityMod/Sounds/Item/ScorchedEarthShot", 3);

        private int counter = 0;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Scorched Earth");
            Tooltip.SetDefault("Fires a burst of four fuel-air rockets which explode into cluster bombs\n" +
            "Each burst consumes two rockets each\n" +
            "Burns your targets to a fine crisp");
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 500;
            Item.DamageType = DamageClass.Ranged;
            Item.useTime = 8;
            Item.useAnimation = 32; // 4 shots in just over half a second
            Item.reuseDelay = 60; // 1 second recharge
            Item.width = 104;
            Item.height = 44;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 8.7f;
            Item.value = CalamityGlobalItem.Rarity14BuyPrice;
            Item.rare = ItemRarityID.Purple;
            Item.Calamity().customRarity = CalamityRarity.DarkBlue;
            Item.autoReuse = true;
            Item.shootSpeed = 12.6f;
            Item.shoot = ModContent.ProjectileType<ScorchedEarthRocket>();
            Item.useAmmo = AmmoID.Rocket;
            Item.Calamity().donorItem = true;
        }

        // Consume two ammo per fire
        public override bool CanConsumeAmmo(Item ammo, Player player) => counter % 2 == 0;

        public override Vector2? HoldoutOffset() => new Vector2(-30, 0);

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<ScorchedEarthRocket>(), damage, knockback, player.whoAmI);

            if (counter == 0)
            {
                SoundEngine.PlaySound(ShootSound, position);
            }

            counter++;
            if (counter == 4)
                counter = 0;
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<BlissfulBombardier>().
                AddIngredient<DarksunFragment>(10).
                AddIngredient(ItemID.FragmentSolar, 50).
                AddRecipeGroup("AnyAdamantiteBar", 15).
                AddTile<CosmicAnvil>().
                Register();
        }
    }
}
