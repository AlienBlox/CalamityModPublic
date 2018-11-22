using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items;

namespace CalamityMod.Items.Weapons.HiveMind
{
	public class ShaderainStaff : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Shaderain Staff");
			Tooltip.SetDefault("Fires a shade storm cloud");
		}

	    public override void SetDefaults()
	    {
	        item.damage = 17;
	        item.magic = true;
	        item.mana = 10;
	        item.width = 34;
	        item.height = 34;
	        item.useTime = 25;
	        item.useAnimation = 25;
	        item.useStyle = 1;
	        item.noMelee = true;
	        item.knockBack = 0f;
	        item.value = 40000;
	        item.rare = 2;
	        item.UseSound = SoundID.Item66;
	        item.shoot = mod.ProjectileType("ShadeNimbus");
	        item.shootSpeed = 16f;
	    }
	    
	    public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
		{
			float num72 = item.shootSpeed;
	    	Vector2 vector2 = player.RotatedRelativePoint(player.MountedCenter, true);
	    	float num78 = (float)Main.mouseX + Main.screenPosition.X - vector2.X;
			float num79 = (float)Main.mouseY + Main.screenPosition.Y - vector2.Y;
			if (player.gravDir == -1f)
			{
				num79 = Main.screenPosition.Y + (float)Main.screenHeight - (float)Main.mouseY - vector2.Y;
			}
			float num80 = (float)Math.Sqrt((double)(num78 * num78 + num79 * num79));
			float num81 = num80;
			if ((float.IsNaN(num78) && float.IsNaN(num79)) || (num78 == 0f && num79 == 0f))
			{
				num78 = (float)player.direction;
				num79 = 0f;
				num80 = num72;
			}
			else
			{
				num80 = num72 / num80;
			}
	    	num78 *= num80;
			num79 *= num80;
			int num154 = Projectile.NewProjectile(vector2.X, vector2.Y, num78, num79, mod.ProjectileType("ShadeNimbusCloud"), damage, knockBack, player.whoAmI, 0f, 0f);
			Main.projectile[num154].ai[0] = (float)Main.mouseX + Main.screenPosition.X;
			Main.projectile[num154].ai[1] = (float)Main.mouseY + Main.screenPosition.Y;
	    	return false;
		}
	    
	    public override void AddRecipes()
	    {
	        ModRecipe recipe = new ModRecipe(mod);
	        recipe.AddIngredient(ItemID.RottenChunk, 2);
	        recipe.AddIngredient(ItemID.DemoniteBar, 3);
	        recipe.AddIngredient(null, "TrueShadowScale", 12);
	        recipe.AddTile(TileID.DemonAltar);
	        recipe.SetResult(this);
	        recipe.AddRecipe();
	    }
	}
}