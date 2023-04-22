﻿using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Fishing
{
    public class FishofFlight : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Fish of Flight");
            // Tooltip.SetDefault("Right click to extract souls");
            Item.ResearchUnlockCount = 10;
            ItemID.Sets.CanBePlacedOnWeaponRacks[Item.type] = true;
        }

        public override void SetDefaults()
        {
            Item.maxStack = 9999;
            Item.consumable = true;
            Item.width = 30;
            Item.height = 30;
            Item.rare = ItemRarityID.Orange;
            Item.value = Item.sellPrice(gold: 1);
        }

		public override void ModifyResearchSorting(ref ContentSamples.CreativeHelper.ItemGroup itemGroup)
		{
			itemGroup = ContentSamples.CreativeHelper.ItemGroup.GoodieBags;
		}

        public override bool CanRightClick() => true;
        public override void ModifyItemLoot(ItemLoot itemLoot) => itemLoot.Add(ItemID.SoulofFlight, 1, 5, 8);
    }
}
