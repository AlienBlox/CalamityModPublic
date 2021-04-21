using CalamityMod.Buffs.Potions;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Potions
{
	public class TeslaPotion : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Tesla Potion");
			Tooltip.SetDefault("Summons an aura of electricity that electrifies and slows enemies\n" +
				"Slowdown does not work on bosses and aura damage is reduced on bosses\n" +
				"Reduces the duration of the Electrified debuff");
		}

		public override void SetDefaults()
		{
			item.width = 24;
			item.height = 26;
			item.useTurn = true;
			item.maxStack = 999;
			item.rare = ItemRarityID.Orange;
			item.useAnimation = 17;
			item.useTime = 17;
			item.useStyle = ItemUseStyleID.EatingUsing;
			item.UseSound = SoundID.Item3;
			item.consumable = true;
			item.buffType = ModContent.BuffType<TeslaBuff>();
			item.buffTime = CalamityUtils.SecondsToFrames(480f);
			item.value = Item.buyPrice(0, 2, 0, 0);
		}

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.BottledWater);
			recipe.AddIngredient(ModContent.ItemType<SeaPrism>());
			recipe.AddRecipeGroup("AnyGoldOre", 2);
			recipe.AddTile(TileID.Bottles);
			recipe.SetResult(this);
			recipe.AddRecipe();
			recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.BottledWater);
			recipe.AddIngredient(ModContent.ItemType<BloodOrb>(), 10);
			recipe.AddTile(TileID.AlchemyTable);
			recipe.SetResult(this);
			recipe.AddRecipe();
		}
	}
}
