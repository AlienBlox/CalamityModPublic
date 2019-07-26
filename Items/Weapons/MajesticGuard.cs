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
	public class MajesticGuard : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Majestic Guard");
			Tooltip.SetDefault("Has a chance to lower enemy defense by 10 when striking them\n" +
				"If enemy defense is 0 or below your attacks will heal you");
		}

		public override void SetDefaults()
		{
			item.width = 98;
			item.damage = 60;
			item.melee = true;
			item.useAnimation = 22;
			item.useStyle = 1;
			item.useTime = 22;
			item.useTurn = true;
			item.knockBack = 7.5f;
			item.UseSound = SoundID.Item1;
			item.autoReuse = true;
			item.height = 98;
            item.value = Item.buyPrice(0, 36, 0, 0);
            item.rare = 5;
		}

		public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit)
	    {
			if (Main.rand.Next(5) == 0)
			{
				target.defense -= 10;
			}
			if (target.defense <= 0 && target.canGhostHeal)
			{
		    	player.statLife += 3;
		    	player.HealEffect(3);
			}
		}

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.AdamantiteSword);
			recipe.AddIngredient(ItemID.SoulofMight, 15);
	        recipe.AddTile(TileID.MythrilAnvil);
	        recipe.SetResult(this);
	        recipe.AddRecipe();
	        recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.TitaniumSword);
			recipe.AddIngredient(ItemID.SoulofMight, 15);
	        recipe.AddTile(TileID.MythrilAnvil);
	        recipe.SetResult(this);
	        recipe.AddRecipe();
		}
	}
}
