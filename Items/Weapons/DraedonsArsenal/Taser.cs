using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.DraedonsArsenal;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.DraedonsArsenal
{
    public class Taser : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Taser");
			Tooltip.SetDefault("Shoots a hook that attaches to enemies and electrocutes them before returning");
		}

		public override void SetDefaults()
		{
			item.width = 50;
			item.height = 26;
			item.ranged = true;
			item.damage = 28;
			item.knockBack = 0f;
			item.useTime = item.useAnimation = 28;
			item.autoReuse = true;

			item.useStyle = ItemUseStyleID.HoldingOut;
			item.UseSound = mod.GetLegacySoundSlot(SoundType.Item, "Sounds/Item/PlasmaBolt");
			item.noMelee = true;

			item.value = CalamityGlobalItem.Rarity3BuyPrice;
			item.rare = ItemRarityID.Red;
			item.Calamity().customRarity = CalamityRarity.DraedonRust;

			item.shoot = ModContent.ProjectileType<TaserHook>();
			item.shootSpeed = 15f;

			item.Calamity().Chargeable = true;
			item.Calamity().ChargeMax = 50;
		}

		public override bool CanUseItem(Player player) => player.ownedProjectileCounts[item.shoot] <= 0;

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ModContent.ItemType<MysteriousCircuitry>(), 7);
			recipe.AddIngredient(ModContent.ItemType<DubiousPlating>(), 5);
			recipe.AddIngredient(ModContent.ItemType<AerialiteBar>(), 4);
			recipe.AddIngredient(ItemID.MeteoriteBar, 4);
			recipe.AddIngredient(ItemID.Harpoon);
			recipe.AddTile(TileID.Anvils);
			recipe.SetResult(this);
			recipe.AddRecipe();
		}
	}
}
