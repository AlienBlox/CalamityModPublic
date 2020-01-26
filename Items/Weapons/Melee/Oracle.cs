﻿using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Melee.Yoyos;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class Oracle : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("The Oracle");
            Tooltip.SetDefault("Emits an aura of red lightning which charges up when hitting enemies\n" +
			"Fires auric orbs when supercharged\n" +
			"An exceptionally agile yoyo\n" +
			"Gaze into the past, the present, the future... and the circumstances of your inevitable demise");
            ItemID.Sets.Yoyo[item.type] = true;
            ItemID.Sets.GamepadExtraRange[item.type] = 15;
            ItemID.Sets.GamepadSmartQuickReach[item.type] = true;
        }

        public override void SetDefaults()
        {
            item.width = 54;
            item.height = 42;
            item.melee = true;
            item.damage = 420;
            item.knockBack = 4f;
            item.useTime = 20;
            item.useAnimation = 20;
            item.autoReuse = true;

            item.useStyle = 5;
            item.UseSound = SoundID.Item1;
            item.channel = true;
            item.noUseGraphic = true;
            item.noMelee = true;

            item.shoot = ModContent.ProjectileType<OracleYoyo>();
            item.shootSpeed = 16f;

            item.rare = 10;
            item.Calamity().postMoonLordRarity = 21;
            item.value = Item.buyPrice(platinum: 2, gold: 50);
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Projectile.NewProjectile(position.X, position.Y, speedX, speedY, type, damage, knockBack, player.whoAmI, 0.0f, 0.0f);
            return false;
        }

        public override void AddRecipes()
        {
            ModRecipe r = new ModRecipe(mod);
            r.SetResult(this);
            r.AddTile(ModContent.TileType<DraedonsForge>());
            r.AddIngredient(ModContent.ItemType<TheObliterator>());
            r.AddIngredient(ModContent.ItemType<Lacerator>());
            r.AddIngredient(ModContent.ItemType<Verdant>());
            r.AddIngredient(ModContent.ItemType<Chaotrix>());
            r.AddIngredient(ModContent.ItemType<Quagmire>());
            r.AddIngredient(ModContent.ItemType<Shimmerspark>());
			r.AddIngredient(ModContent.ItemType<AuricBar>(), 4);
			r.AddRecipe();
        }
    }
}
