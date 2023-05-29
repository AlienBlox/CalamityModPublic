﻿using Terraria.DataStructures;
using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Ranged;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class CosmicBolter : ModItem, ILocalizedModType
    {
        public string LocalizationCategory => "Items.Weapons.Ranged";
        public override void SetDefaults()
        {
            Item.damage = 48;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 40;
            Item.height = 76;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 2.75f;
            Item.value = CalamityGlobalItem.Rarity8BuyPrice;
            Item.rare = ItemRarityID.Yellow;
            Item.UseSound = SoundID.Item75;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<LunarBolt2>();
            Item.shootSpeed = 10f;
            Item.useAmmo = AmmoID.Arrow;
            Item.Calamity().canFirePointBlankShots = true;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo spawnSource, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Vector2 source = player.RotatedRelativePoint(player.MountedCenter, true);
            float piOver10 = MathHelper.Pi * 0.1f;
            int projAmt = 3;

            velocity.Normalize();
            velocity *= 30f;
            bool canHit = Collision.CanHit(source, 0, 0, source + velocity, 0, 0);
            for (int i = 0; i < projAmt; i++)
            {
                float offsetAmt = i - (projAmt - 1f) / 2f;
                Vector2 offset = velocity.RotatedBy(piOver10 * offsetAmt, default);
                if (!canHit)
                    offset -= velocity;

                if (CalamityUtils.CheckWoodenAmmo(type, player))
                    Projectile.NewProjectile(spawnSource, source + offset, velocity, ModContent.ProjectileType<LunarBolt2>(), (int)(damage * 1.2), knockback * 1.2f, player.whoAmI);
                else
                {
                    int proj = Projectile.NewProjectile(spawnSource, source + offset, velocity, type, damage, knockback, player.whoAmI);
                    Main.projectile[proj].noDropItem = true;
                }
            }
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<LunarianBow>().
                AddIngredient(ItemID.HallowedRepeater).
                AddIngredient<LivingShard>(12).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
