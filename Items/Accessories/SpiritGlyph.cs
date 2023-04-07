﻿using CalamityMod.CalPlayer;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    [LegacyName("SpiritGenerator")]
    public class SpiritGlyph : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
            // DisplayName.SetDefault("Spirit Glyph");
            /* Tooltip.SetDefault("Whenever your minions hit an enemy you will gain a random buff\n" +
                "These buffs will either boost your defense, summon damage, or life regen for a while"); */
        }

        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 30;
            Item.value = CalamityGlobalItem.Rarity2BuyPrice;
            Item.rare = ItemRarityID.Green;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.sGlyph = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.Diamond, 5).
                AddIngredient(ItemID.Obsidian, 15).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}
