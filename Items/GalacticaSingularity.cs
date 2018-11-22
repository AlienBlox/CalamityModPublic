﻿using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items;

namespace CalamityMod.Items {
public class GalacticaSingularity : ModItem
{
	public override void SetStaticDefaults()
	{
		DisplayName.SetDefault("Galactica Singularity");
		Tooltip.SetDefault("A shard of the cosmos");
		Main.RegisterItemAnimation(item.type, new DrawAnimationVertical(3, 19));
	}
		
	public override void SetDefaults()
	{
		item.width = 28;
		item.height = 28;
		item.maxStack = 69;
		item.value = 500000;
		item.rare = 10;
	}
	
	public override void AddRecipes()
	{
		ModRecipe recipe = new ModRecipe(mod);
		recipe.AddIngredient(ItemID.FragmentSolar);
		recipe.AddIngredient(ItemID.FragmentVortex);
		recipe.AddIngredient(ItemID.FragmentStardust);
		recipe.AddIngredient(ItemID.FragmentNebula);
        recipe.AddTile(TileID.LunarCraftingStation);
        recipe.SetResult(this);
        recipe.AddRecipe();
	}
	
	public override void Update(ref float gravity, ref float maxFallSpeed)
    {
		maxFallSpeed *= 0.0001f;
        float num = (float)Main.rand.Next(90, 111) * 0.01f;
        num *= Main.essScale;
        Lighting.AddLight((int)((item.position.X + (float)(item.width / 2)) / 16f), (int)((item.position.Y + (float)(item.height / 2)) / 16f), 1f * num, 0.3f * num, 0.3f * num);
    }
}}