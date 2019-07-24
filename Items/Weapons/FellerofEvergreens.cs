using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items;

namespace CalamityMod.Items.Weapons 
{
	public class FellerofEvergreens : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Feller of Evergreens");
		}

	    public override void SetDefaults()
	    {
	        item.damage = 15;
	        item.melee = true;
	        item.width = 36;
	        item.height = 36;
	        item.useTime = 25;
	        item.useAnimation = 25;
	        item.useTurn = true;
	        item.axe = 15;
	        item.useStyle = 1;
	        item.knockBack = 5;
            item.value = Item.buyPrice(0, 2, 0, 0);
            item.rare = 2;
	        item.UseSound = SoundID.Item1;
	        item.autoReuse = true;
	    }
	
	    public override void AddRecipes()
	    {
	        ModRecipe recipe = new ModRecipe(mod);
	        recipe.AddIngredient(ItemID.Wood, 15);
            recipe.anyWood = true;
            recipe.AddIngredient(ItemID.TungstenBar, 10);
	        recipe.AddIngredient(ItemID.TungstenAxe);
	        recipe.AddTile(TileID.Anvils);
	        recipe.SetResult(this);
	        recipe.AddRecipe();
	        recipe = new ModRecipe(mod);
	        recipe.AddIngredient(ItemID.Wood, 15);
            recipe.anyWood = true;
            recipe.AddIngredient(ItemID.SilverBar, 10);
	        recipe.AddIngredient(ItemID.SilverAxe);
	        recipe.AddTile(TileID.Anvils);
	        recipe.SetResult(this);
	        recipe.AddRecipe();
	    }
	}
}