using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.CalPlayer;

namespace CalamityMod.Items.Weapons.FiniteUse
{
    public class LightningHawk : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Lightning Hawk");
			Tooltip.SetDefault("Uses Magnum Rounds\n" +
                "Does more damage to organic enemies\n" +
				"Can be used thrice per boss battle");
		}

	    public override void SetDefaults()
	    {
	        item.damage = 400;
            item.crit += 56;
	        item.width = 50;
	        item.height = 28;
	        item.useTime = 21;
	        item.useAnimation = 21;
	        item.useStyle = 5;
	        item.noMelee = true;
	        item.knockBack = 8f;
            item.value = Item.buyPrice(0, 48, 0, 0);
            item.rare = 6;
	        item.UseSound = mod.GetLegacySoundSlot(SoundType.Item, "Sounds/Item/Magnum");
	        item.autoReuse = true;
	        item.shootSpeed = 12f;
	        item.shoot = mod.ProjectileType("MagnumRound");
	        item.useAmmo = mod.ItemType("MagnumRounds");
			if (CalamityPlayer.areThereAnyDamnBosses)
			{
				item.GetGlobalItem<CalamityGlobalItem>(mod).timesUsed = 3;
			}
		}

		public override bool OnPickup(Player player)
		{
			if (CalamityPlayer.areThereAnyDamnBosses)
			{
				item.GetGlobalItem<CalamityGlobalItem>(mod).timesUsed = 3;
			}
			return true;
		}

		public override bool CanUseItem(Player player)
		{
			return item.GetGlobalItem<CalamityGlobalItem>(mod).timesUsed < 3;
		}

		public override Vector2? HoldoutOffset()
		{
			return new Vector2(-5, 0);
		}

		public override void UpdateInventory(Player player)
		{
			if (!CalamityPlayer.areThereAnyDamnBosses)
			{
				item.GetGlobalItem<CalamityGlobalItem>(mod).timesUsed = 0;
			}
		}

		public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
		{
			if (CalamityPlayer.areThereAnyDamnBosses)
			{
				for (int i = 0; i < 58; i++)
				{
					if (player.inventory[i].type == item.type)
					{
						player.inventory[i].GetGlobalItem<CalamityGlobalItem>(mod).timesUsed++;
					}
				}
			}
			return true;
		}

		public override void AddRecipes()
	    {
	        ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "Magnum");
            recipe.AddIngredient(ItemID.SoulofMight, 20);
            recipe.AddIngredient(ItemID.SoulofSight, 20);
            recipe.AddIngredient(ItemID.SoulofFright, 20);
            recipe.AddIngredient(ItemID.IllegalGunParts);
            recipe.AddIngredient(ItemID.HallowedBar, 10);
            recipe.AddTile(TileID.MythrilAnvil);
	        recipe.SetResult(this);
	        recipe.AddRecipe();
	    }
	}
}
