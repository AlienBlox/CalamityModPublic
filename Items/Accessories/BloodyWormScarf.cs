﻿using CalamityMod.CalPlayer;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    [AutoloadEquip(EquipType.Neck)]
    public class BloodyWormScarf : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Accessories";
        public override void SetDefaults()
        {
            Item.defense = 7;
            Item.width = 26;
            Item.height = 42;
            Item.value = CalamityGlobalItem.Rarity4BuyPrice;
            Item.rare = ItemRarityID.LightRed;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.bloodyWormTooth = true;
            player.endurance += 0.07f;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<BloodyWormTooth>().
                AddIngredient(ItemID.WormScarf).
                AddIngredient(ItemID.SoulofNight, 3).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}
