﻿using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables;
using CalamityMod.Items.Potions;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.SummonItems
{
    public class Starcore : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
            // DisplayName.SetDefault("Starcore");
            /* Tooltip.SetDefault("May the stars guide your way\n" +
                "Summons Astrum Deus at the Astral Beacon, but is not consumed\n" +
                "Enrages during the day"); */
			ItemID.Sets.SortingPriorityBossSpawns[Type] = 16; // Solar Tablet / Bloody Tear
        }

        public override void SetDefaults()
        {
            Item.width = 34;
            Item.height = 40;
            Item.rare = ItemRarityID.Cyan;
        }

		public override void ModifyResearchSorting(ref ContentSamples.CreativeHelper.ItemGroup itemGroup)
		{
			itemGroup = ContentSamples.CreativeHelper.ItemGroup.BossItem;
		}

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<Stardust>(25).
                AddIngredient<AureusCell>(8).
                AddIngredient<AstralBar>(4).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}
