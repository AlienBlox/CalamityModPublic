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
	public class EarthenPike : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Earthen Pike");
			Tooltip.SetDefault("Crushes enemy defenses\n" +
				"Sprays fossil shards on use");
		}

		public override void SetDefaults()
		{
			item.width = 60;
			item.damage = 50;
			item.melee = true;
			item.noMelee = true;
			item.useTurn = true;
			item.noUseGraphic = true;
			item.useAnimation = 25;
			item.useStyle = 5;
			item.useTime = 25;
			item.knockBack = 7f;
			item.UseSound = SoundID.Item1;
			item.autoReuse = false;
			item.height = 60;
            item.value = Item.buyPrice(0, 36, 0, 0);
            item.rare = 5;
			item.shoot = mod.ProjectileType("EarthenPike");
			item.shootSpeed = 6f;
		}
	}
}
