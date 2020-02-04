﻿using CalamityMod.CalPlayer;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Items.LoreItems
{
    public class KnowledgeMoonLord : LoreItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Moon Lord");
            Tooltip.SetDefault("What a waste.\n" +
                "Had it been fully restored it would have been a force to behold, but what you fought was an empty shell.\n" +
                "However, that doesn't diminish the immense potential locked within it, released upon its death.\n" +
                "Place in your inventory to gain an improved Gravity Globe that gives you an increase to all stats while upside down.\n" +
				"However, while not upside down you have permanent featherfall.");
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.rare = 10;
            item.consumable = false;
        }

        public override bool CanUseItem(Player player)
        {
            return false;
        }

        public override void UpdateInventory(Player player)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.moonLordLore = true;
        }
    }
}
