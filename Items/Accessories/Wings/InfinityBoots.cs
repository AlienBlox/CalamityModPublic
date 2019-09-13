﻿using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.CalPlayer;

namespace CalamityMod.Items.Accessories.Wings
{
    [AutoloadEquip(EquipType.Wings, EquipType.Shoes)]
	public class InfinityBoots : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Seraph Tracers");
			Tooltip.SetDefault("Counts as wings\n" +
				"Horizontal speed: 9\n" +
				"Acceleration multiplier: 2.5\n" +
				"Good vertical speed\n" +
				"Flight time: 120\n" +
				"Ludicrous speed!\n" +
				"Greater mobility on ice\n" +
				"Water and lava walking\n" +
				"Temporary immunity to lava");
		}

		public override void SetDefaults()
		{
			item.width = 36;
			item.height = 32;
			item.value = Item.buyPrice(0, 60, 0, 0);
			item.accessory = true;
			item.GetGlobalItem<CalamityGlobalItem>(mod).postMoonLordRarity = 12;
		}

		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			if (player.controlJump && player.wingTime > 0f && !player.jumpAgainCloud && player.jump == 0 && player.velocity.Y != 0f && !hideVisual)
			{
				int num59 = 4;
				if (player.direction == 1)
				{
					num59 = -40;
				}
				int num60 = Dust.NewDust(new Vector2(player.position.X + (float)(player.width / 2) + (float)num59, player.position.Y + (float)(player.height / 2) - 15f), 30, 30, 107, 0f, 0f, 100, default(Color), 2.4f);
				Main.dust[num60].noGravity = true;
				Main.dust[num60].velocity *= 0.3f;
				if (Main.rand.Next(10) == 0)
				{
					Main.dust[num60].fadeIn = 2f;
				}
				Main.dust[num60].shader = GameShaders.Armor.GetSecondaryShader(player.cWings, player);
			}
			CalamityPlayer modPlayer = player.GetModPlayer<CalamityPlayer>(mod);
			player.accRunSpeed = 9.25f;
			player.rocketBoots = 3;
			player.moveSpeed += 0.24f;
			player.iceSkate = true;
			player.waterWalk = true;
			player.fireWalk = true;
			player.lavaMax += 240;
			player.wingTimeMax = 120;
			modPlayer.IBoots = !hideVisual;
			modPlayer.sTracers = true;
		}

		public override void VerticalWingSpeeds(Player player, ref float ascentWhenFalling, ref float ascentWhenRising, ref float maxCanAscendMultiplier, ref float maxAscentMultiplier, ref float constantAscend)
		{
			ascentWhenFalling = 0.75f;
			ascentWhenRising = 0.15f;
			maxCanAscendMultiplier = 1f;
			maxAscentMultiplier = 2.5f;
			constantAscend = 0.125f;
		}

		public override void HorizontalWingSpeeds(Player player, ref float speed, ref float acceleration)
		{
			speed = 9f;
			acceleration *= 2.5f;
		}

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddRecipeGroup("WingsGroup");
			recipe.AddIngredient(null, "AngelTreads");
			recipe.AddIngredient(null, "CoreofCalamity", 3);
			recipe.AddIngredient(null, "BarofLife", 5);
			recipe.AddIngredient(ItemID.LunarBar, 5);
			recipe.AddTile(TileID.LunarCraftingStation);
			recipe.SetResult(this);
			recipe.AddRecipe();
		}
	}
}
