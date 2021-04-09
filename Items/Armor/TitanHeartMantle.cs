using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Armor
{
	[AutoloadEquip(EquipType.Body)]
	public class TitanHeartMantle : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Titan Heart Mantle");
			Tooltip.SetDefault("45% chance to not consume rogue items\n" +
			"5% boosted rogue knockback but 15% lowered rogue shoot speed");
		}

		public override void SetDefaults()
		{
			item.width = 18;
			item.height = 18;
			item.value = Item.buyPrice(0, 12, 0, 0);
			item.rare = ItemRarityID.Pink;
			item.defense = 17;
		}

		public override void UpdateEquip(Player player)
		{
			player.Calamity().titanHeartMantle = true;
			player.Calamity().throwingAmmoCost *= 0.55f;
		}

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ModContent.ItemType<AstralMonolith>(), 20);
			recipe.AddIngredient(ModContent.ItemType<TitanHeart>());
			recipe.AddTile(TileID.MythrilAnvil);
			recipe.SetResult(this);
			recipe.AddRecipe();
		}
	}
}
