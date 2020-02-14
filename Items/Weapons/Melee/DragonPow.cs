﻿using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Melee;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class DragonPow : ModItem
    {
        public static float Speed = 13f;
        public static float ReturnSpeed = 20f;
        public static float SparkSpeed = 0.6f;
        public static float MinPetalSpeed = 24f;
        public static float MaxPetalSpeed = 30f;
        public static float MinWaterfallSpeed = 12f;
        public static float MaxWaterfallSpeed = 15.5f;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Dragon Pow");
            Tooltip.SetDefault(@"Fires a dragon head that releases draconic sparks
Summons a barrage of petals and waterfalls on enemy hits");
        }

        public override void SetDefaults()
        {
            item.width = 76;
            item.height = 82;
            item.melee = true;
            item.damage = 4500;
            item.knockBack = 9f;
            item.useAnimation = 20;
            item.useTime = 20;
            item.autoReuse = true;
            item.noMelee = true;
            item.noUseGraphic = true;

            item.useStyle = 5;
            item.UseSound = mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/YharonRoarShort");
            item.channel = true;

            item.rare = 10;
            item.Calamity().customRarity = CalamityRarity.Dedicated;
            item.value = Item.buyPrice(2, 50, 0, 0);

            item.shoot = ModContent.ProjectileType<DragonPowFlail>();
            item.shootSpeed = Speed;
        }

        public override void AddRecipes()
        {
            ModRecipe r = new ModRecipe(mod);
            r.SetResult(this);
            r.AddTile(ModContent.TileType<DraedonsForge>());
            r.AddIngredient(ModContent.ItemType<Mourningstar>());
            r.AddIngredient(ItemID.DaoofPow);
            r.AddIngredient(ItemID.FlowerPow);
            r.AddIngredient(ItemID.Flairon);
            r.AddIngredient(ModContent.ItemType<BallOFugu>());
            r.AddIngredient(ModContent.ItemType<Tumbleweed>());
            r.AddIngredient(ModContent.ItemType<UrchinFlail>());
			r.AddIngredient(ModContent.ItemType<AuricBar>(), 4);
			r.AddIngredient(ModContent.ItemType<HellcasterFragment>(), 4);
			r.AddRecipe();
        }
    }
}
