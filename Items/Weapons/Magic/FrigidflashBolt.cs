﻿using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Magic;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Magic
{
    public class FrigidflashBolt : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Frigidflash Bolt");
            Tooltip.SetDefault("Casts a slow-moving ball of flash-freezing magma");
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 80;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 13;
            Item.width = 28;
            Item.height = 30;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 5.5f;
            Item.value = CalamityGlobalItem.Rarity4BuyPrice;
            Item.rare = ItemRarityID.LightRed;
            Item.UseSound = SoundID.Item21;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<FrigidflashBoltProjectile>();
            Item.shootSpeed = 9f;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<FrostBolt>().
                AddIngredient<FlareBolt>().
                AddIngredient<EssenceofEleum>(2).
                AddIngredient<EssenceofHavoc>(2).
                AddTile(TileID.Bookcases).
                Register();
        }
    }
}
