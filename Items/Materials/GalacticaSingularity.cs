﻿using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Materials
{
    public class GalacticaSingularity : ModItem, ILocalizedModType
    {
        public string LocalizationCategory => "Items.Materials";
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 25;
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(4, 24));
            ItemID.Sets.AnimatesAsSoul[Type] = true;
            ItemID.Sets.ItemNoGravity[Item.type] = true;
			ItemID.Sets.SortingPriorityMaterials[Type] = 99; // Luminite
        }

        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 28;
            Item.maxStack = 9999;
            Item.value = Item.sellPrice(silver: 96);
            Item.rare = ItemRarityID.Red;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.FragmentSolar).
                AddIngredient(ItemID.FragmentVortex).
                AddIngredient(ItemID.FragmentStardust).
                AddIngredient(ItemID.FragmentNebula).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }

        public override void Update(ref float gravity, ref float maxFallSpeed)
        {
            float num = (float)Main.rand.Next(90, 111) * 0.01f;
            num *= Main.essScale;
            Lighting.AddLight((int)((Item.position.X + (float)(Item.width / 2)) / 16f), (int)((Item.position.Y + (float)(Item.height / 2)) / 16f), 1f * num, 0.3f * num, 0.3f * num);
        }
    }
}
