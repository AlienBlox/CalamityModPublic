﻿using CalamityMod.Items.Materials;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Tools
{
    [LegacyName("GallantPickaxe")]
    public class GenesisPickaxe : ModItem
    {
        public override void SetDefaults()
        {
            // These stats exactly match vanilla's Luminite pickaxes.
            Item.damage = 80;
            Item.knockBack = 5.5f;
            Item.useTime = 6;
            Item.useAnimation = 12;
            Item.pick = 225;
            Item.tileBoost += 4;

            Item.DamageType = DamageClass.Melee;
            Item.width = 84;
            Item.height = 80;
            Item.useTurn = true;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.value = CalamityGlobalItem.Rarity10BuyPrice;
            Item.rare = ItemRarityID.Red;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.useTurn = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<MeldConstruct>(12)
                .AddIngredient(ItemID.LunarBar, 10)
                .AddTile(TileID.LunarCraftingStation)
                .Register();
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            if (Main.rand.NextBool(5))
            {
                int dust = Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, 62);
            }
        }
    }
}
