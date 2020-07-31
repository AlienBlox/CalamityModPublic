using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.DraedonsArsenal;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.DraedonsArsenal
{
    public class GaussPistol : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Gauss Pistol");
			Tooltip.SetDefault("A simple pistol that utilizes magic power, a weapon for the more magically adapt.\n" +
			"Fires a devastating high velocity blast with extreme knockback");
		}

		public override void SetDefaults()
		{
			item.width = 40;
			item.height = 22;
			item.magic = true;
			item.mana = 6;
			item.damage = 130;
			item.knockBack = 11f;
			item.useTime = item.useAnimation = 20;
			item.autoReuse = true;

			item.useStyle = ItemUseStyleID.HoldingOut;
			item.UseSound = mod.GetLegacySoundSlot(SoundType.Item, "Sounds/Item/GaussWeaponFire");
			item.noMelee = true;

			item.value = CalamityGlobalItem.Rarity5BuyPrice;
			item.rare = ItemRarityID.Red;
			item.Calamity().customRarity = CalamityRarity.DraedonRust;

			item.shoot = ModContent.ProjectileType<GaussPistolShot>();
			item.shootSpeed = 14f;

			item.Calamity().ChargeMax = 85;
			item.Calamity().Chargeable = true;
		}

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ModContent.ItemType<MysteriousCircuitry>(), 12);
			recipe.AddIngredient(ModContent.ItemType<DubiousPlating>(), 8);
			recipe.AddIngredient(ItemID.HallowedBar, 10);
			recipe.AddIngredient(ItemID.SoulofMight, 20);
			recipe.AddTile(TileID.MythrilAnvil);
			recipe.SetResult(this);
			recipe.AddRecipe();
		}
	}
}
