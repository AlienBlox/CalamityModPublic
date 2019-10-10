using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons
{
    public class ElysianArrow : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Elysian Arrow");
			Tooltip.SetDefault("Summons meteors from the sky on death");
		}

		public override void SetDefaults()
		{
			item.damage = 20;
			item.ranged = true;
			item.width = 22;
			item.height = 36;
			item.maxStack = 999;
			item.consumable = true;
			item.knockBack = 3f;
			item.value = 2000;
			item.shoot = mod.ProjectileType("ElysianArrow");
			item.shootSpeed = 10f;
			item.ammo = 40;
			item.Calamity().postMoonLordRarity = 12;
		}

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(null, "UnholyEssence");
			recipe.AddIngredient(ItemID.HolyArrow, 150);
			recipe.AddTile(TileID.LunarCraftingStation);
			recipe.SetResult(this, 150);
			recipe.AddRecipe();
		}
	}
}
