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
	public class Effervescence : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Effervescence");
			Tooltip.SetDefault("Shoots a massive spread of bubbles");
		}

		public override void SetDefaults()
		{
			item.damage = 24;
			item.magic = true;
			item.mana = 17;
			item.width = 56;
			item.height = 26;
			item.useTime = 12;
			item.useAnimation = 12;
			item.useStyle = 5;
			item.noMelee = true;
			item.knockBack = 3.75f;
			item.value = Item.buyPrice(1, 20, 0, 0);
			item.rare = 10;
			item.UseSound = SoundID.Item95;
			item.autoReuse = true;
			item.shootSpeed = 13f;
			item.shoot = mod.ProjectileType("UberBubble");
			item.GetGlobalItem<CalamityGlobalItem>(mod).postMoonLordRarity = 12;
		}
	    
	    public override bool Shoot(Player player, ref Microsoft.Xna.Framework.Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
		{
			for (int randomBullets = 0; randomBullets <= 4; randomBullets++)
			{
				float SpeedX = speedX + (float) Main.rand.Next(-25, 26) * 0.05f;
	            float SpeedY = speedY + (float) Main.rand.Next(-25, 26) * 0.05f;
				Projectile.NewProjectile(position.X, position.Y, SpeedX, SpeedY, type, damage, knockBack, player.whoAmI, 0f, 0f);
			}
	    	return false;
		}
	
	    public override void AddRecipes()
	    {
	        ModRecipe recipe = new ModRecipe(mod);
	        recipe.AddIngredient(ItemID.BubbleGun);
	        recipe.AddIngredient(ItemID.Xenopopper);
	        recipe.AddIngredient(ItemID.LunarBar, 5);
	        recipe.AddTile(TileID.LunarCraftingStation);
	        recipe.SetResult(this);
	        recipe.AddRecipe();
	    }
	}
}