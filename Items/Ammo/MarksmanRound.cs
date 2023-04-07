﻿using CalamityMod.Projectiles.Ranged;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Ammo
{
    public class MarksmanRound : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 99;
            // DisplayName.SetDefault("Marksman Round");
            /* Tooltip.SetDefault("A carefully crafted round which can be ricocheted off of midair coins\n" +
                "This allows other guns to utilize the coins tossed by Crackshot Colt and Midas Prime"); */
        }

        public override void SetDefaults()
        {
            Item.damage = 12;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 12;
            Item.height = 26;
            Item.maxStack = 999;
            Item.consumable = true;
            Item.knockBack = 2.25f;
            Item.value = Item.sellPrice(copper: 10);
            Item.rare = ItemRarityID.LightRed;
            Item.shoot = ModContent.ProjectileType<MarksmanShot>();
            Item.shootSpeed = 1f;
            Item.ammo = AmmoID.Bullet;
        }

        public override void AddRecipes()
        {
            CreateRecipe(999).
                AddIngredient(ItemID.GoldenBullet, 999).
                AddIngredient(ItemID.GoldCoin).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}
