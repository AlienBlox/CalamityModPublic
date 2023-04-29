﻿using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Magic;
using CalamityMod.Rarities;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Magic
{
    public class PrimordialAncient : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Primordial Ancient");
            /* Tooltip.SetDefault("An ancient relic from an ancient land\n" +
                "Casts a gigantic blast of dust"); */
        }

        public override void SetDefaults()
        {
            Item.damage = 170;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 20;
            Item.width = 40;
            Item.height = 56;
            Item.useTime = 18;
            Item.useAnimation = 18;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 5;
            Item.value = CalamityGlobalItem.RarityDarkBlueBuyPrice;
            Item.UseSound = SoundID.Item20;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<Ancient>();
            Item.shootSpeed = 8f;
            Item.rare = ModContent.RarityType<DarkBlue>();
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ModContent.ItemType<PrimordialEarth>()).
                AddIngredient(ItemID.AncientBattleArmorMaterial, 5).
                AddIngredient<CosmiliteBar>(8).
                AddIngredient<EndothermicEnergy>(20).
                AddTile(TileID.Bookcases).
                Register();
        }
    }
}
