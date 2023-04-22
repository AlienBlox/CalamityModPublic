﻿using CalamityMod.Rarities;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Materials
{
    public class BloodstoneCore : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 25;
            // DisplayName.SetDefault("Bloodstone Core");
			ItemID.Sets.SortingPriorityMaterials[Type] = 113;
        }

        public override void SetDefaults()
        {
            Item.width = 15;
            Item.height = 12;
            Item.maxStack = Item.CommonMaxStack;
            Item.value = Item.sellPrice(gold: 4);
            Item.rare = ModContent.RarityType<Turquoise>();
        }

        public override void AddRecipes()
        {
            CreateRecipe(2).
                AddIngredient<Bloodstone>(5).
                AddIngredient<BloodOrb>().
                AddIngredient<Phantoplasm>().
                AddTile(TileID.AdamantiteForge).
                Register();
        }
    }
}
