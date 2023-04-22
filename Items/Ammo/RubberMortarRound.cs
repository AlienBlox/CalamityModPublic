﻿using CalamityMod.Projectiles.Ranged;
using CalamityMod.Rarities;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Ammo
{
    public class RubberMortarRound : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 99;
            // DisplayName.SetDefault("Rubber Mortar Round");
            /* Tooltip.SetDefault("Large blast radius\n" +
                "Will destroy tiles on each bounce\n" +
                "Used by normal guns"); */
        }

        public override void SetDefaults()
        {
            Item.damage = 25;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 20;
            Item.height = 14;
            Item.maxStack = Item.CommonMaxStack;
            Item.consumable = true;
            Item.knockBack = 7.5f;
            Item.value = Item.sellPrice(copper: 20);
            Item.rare = ModContent.RarityType<Turquoise>();
            Item.ammo = AmmoID.Bullet;
            Item.shoot = ModContent.ProjectileType<RubberMortarRoundProj>();
        }

        public override void AddRecipes()
        {
            CreateRecipe(100).
                AddIngredient<MortarRound>(100).
                AddIngredient(ItemID.PinkGel, 5).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}
