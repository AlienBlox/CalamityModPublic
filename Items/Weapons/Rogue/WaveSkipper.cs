﻿using Terraria.DataStructures;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables;
using CalamityMod.Projectiles.Rogue;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Rogue
{
    [LegacyName("DuneHopper")]
    public class WaveSkipper : RogueWeapon
    {
        public override void SetDefaults()
        {
            Item.width = 44;
            Item.damage = 80;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.useAnimation = Item.useTime = 22;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 4f;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.height = 44;
            Item.value = CalamityGlobalItem.Rarity5BuyPrice;
            Item.rare = ItemRarityID.Pink;
            Item.shoot = ModContent.ProjectileType<DuneHopperProjectile>();
            Item.shootSpeed = 12f;
            Item.DamageType = RogueDamageClass.Instance;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.Calamity().StealthStrikeAvailable())
            {
                int numProj = 3;
                float rotation = MathHelper.ToRadians(3);
                for (int i = 0; i < numProj; i++)
                {
                    Vector2 perturbedSpeed = new Vector2(velocity.X - 3f, velocity.Y - 3f).RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (float)(numProj - 1)));
                    int stealth = Projectile.NewProjectile(source, position, perturbedSpeed, ModContent.ProjectileType<DuneHopperProjectile>(), damage, knockback, player.whoAmI);
                    if (stealth.WithinBounds(Main.maxProjectiles))
                        Main.projectile[stealth].Calamity().stealthStrike = true;
                }
                return false;
            }
            return true;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<ScourgeoftheDesert>().
                AddIngredient<MolluskHusk>(5).
                AddIngredient<SeaPrism>(15).
                AddIngredient<PrismShard>(20).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}
