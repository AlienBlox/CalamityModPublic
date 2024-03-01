﻿using CalamityMod.Projectiles.Magic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Magic
{
    public class HarvestStaff : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Magic";
        public override void SetStaticDefaults()
        {
            Item.staff[Item.type] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 46;
            Item.height = 44;
            Item.damage = 22;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 5;
            Item.useTime = 23;
            Item.useAnimation = 23;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 5;
            Item.value = CalamityGlobalItem.Rarity2BuyPrice;
            Item.rare = ItemRarityID.Green;
            Item.UseSound = SoundID.Item20;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<FlamingPumpkin>();
            Item.shootSpeed = 13f;
            Item.scale = 0.9f;
        }


        public override void AddRecipes()
        {
            CreateRecipe().
                AddRecipeGroup("Wood", 20).
                AddIngredient(ItemID.Pumpkin, 20).
                AddIngredient(ItemID.PumpkinSeed, 5).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}
