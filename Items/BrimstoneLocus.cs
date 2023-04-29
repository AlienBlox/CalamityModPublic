﻿using CalamityMod.Items.Materials;
using CalamityMod.Rarities;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items
{
    public class BrimstoneLocus : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Brimstone Locus");
            /* Tooltip.SetDefault("Not only can I enhance your equipment with potent magic, I can also draw out the true strength and ascend\n" +
                "some of your weaponry to entirely new forms\n" +
                "Such items are revealed while this item is in your inventory"); */
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.rare = ModContent.RarityType<Violet>();
            Item.value = 0;
        }

		public override void ModifyResearchSorting(ref ContentSamples.CreativeHelper.ItemGroup itemGroup)
		{
			itemGroup = (ContentSamples.CreativeHelper.ItemGroup)CalamityResearchSorting.ToolsOther;
		}

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<AshesofAnnihilation>().
                AddIngredient<BloodstoneCore>(3).
                Register();
        }
    }
}
