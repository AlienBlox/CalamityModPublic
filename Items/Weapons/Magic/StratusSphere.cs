using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Projectiles.Magic;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Ores;

namespace CalamityMod.Items.Weapons.Magic
{
	public class StratusSphere : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Stratus Sphere");
			Tooltip.SetDefault("Fires an energy orb containing the essence of our stratosphere\n" +
			"Up to six of these can be active at a time");
		}
		public override void SetDefaults()
		{
			item.damage = 500;
			item.noMelee = true;
			item.magic = true;
			item.width = 50;
			item.height = 50;
			item.useTime = 45;
			item.useAnimation = 45;
			item.shoot = ModContent.ProjectileType<StratusSphereProj>();
			item.shootSpeed = 7f;
			item.useStyle = 5;
			item.mana = 30;
			item.knockBack = 2;
			item.UseSound = SoundID.Item20;
			item.rare = 4;
			item.autoReuse = true;
			item.useTurn = true;
			item.holdStyle = 3;
            item.value = Item.buyPrice(1, 40, 0, 0);
            item.rare = 10;
            item.Calamity().postMoonLordRarity = 13;
		}

        public override bool CanUseItem(Player player)
        {
			if (player.ownedProjectileCounts[item.shoot] >= 6)
			{
				return false;
			}
			else
			{
				return true;
			}
        }

        public override void AddRecipes()
		{
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.NebulaArcanum);
            recipe.AddIngredient(ModContent.ItemType<RuinousSoul>(), 5);
            recipe.AddIngredient(ModContent.ItemType<ExodiumClusterOre>(), 12);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
		}
	}
}

