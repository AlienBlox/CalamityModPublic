﻿using Terraria.DataStructures;
using CalamityMod.Projectiles.Summon;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Summon
{
    public class TundraFlameBlossomsStaff : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Tundra Flame Blossoms Staff");
            Tooltip.SetDefault("Summons three flarefrost orchids over your head\n" +
                "Each flower consumes one minion slot");
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 40;
            Item.mana = 10;
            Item.width = 50;
            Item.height = 60;
            Item.useTime = Item.useAnimation = 25;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = true;
            Item.knockBack = 3f;
            Item.value = CalamityGlobalItem.Rarity5BuyPrice;
            Item.rare = ItemRarityID.Pink;
            Item.UseSound = SoundID.Item46;
            Item.shoot = ModContent.ProjectileType<TundraFlameBlossom>();
            Item.DamageType = DamageClass.Summon;
        }

        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[Item.shoot] < 3; //If you already have all 3, no need to resummon

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            CalamityUtils.KillShootProjectiles(false, type, player);
            for (int i = 0; i < 3; i++)
            {
                Projectile blossom = Projectile.NewProjectileDirect(source, player.Center, Vector2.Zero, type, damage, knockback, player.whoAmI, 0f, 0f);
                blossom.ai[1] = (int)(MathHelper.TwoPi / 3f * i * 32f);
                blossom.originalDamage = Item.damage;
            }
            return false;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<CinderBlossomStaff>().
                AddIngredient<FrostBlossomStaff>().
                AddRecipeGroup("AnyMythrilBar", 5).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
