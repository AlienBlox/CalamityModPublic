﻿using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Melee.Shortswords;
using CalamityMod.Rarities;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class CosmicShiv : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Cosmic Shiv");
            /* Tooltip.SetDefault("Fires a cosmic beam that homes in on enemies\n" +
                "Upon hitting an enemy, a barrage of offscreen objects home in on the enemy as well as raining stars\n" +
                "Receives 33% benefit from melee speed bonuses"); */
            ItemID.Sets.BonusAttackSpeedMultiplier[Item.type] = 0.33f;
        }

        public override void SetDefaults()
        {
            Item.useStyle = ItemUseStyleID.Rapier;
            Item.DamageType = DamageClass.Melee;
            Item.useAnimation = 18;
            Item.useTime = 18;
            Item.width = 44;
            Item.height = 44;
            Item.damage = 218;
            Item.knockBack = 9f;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.shoot = ModContent.ProjectileType<CosmicShivProj>();
            Item.shootSpeed = 2.4f;

            Item.value = CalamityGlobalItem.Rarity14BuyPrice;
            Item.rare = ModContent.RarityType<DarkBlue>();
            Item.Calamity().donorItem = true; //Yatagarasu#0001
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<ElementalShiv>().
                AddIngredient<CosmiliteBar>(8).
                AddTile(ModContent.TileType<CosmicAnvil>()).
                Register();
        }
    }
}
