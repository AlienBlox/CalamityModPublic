using CalamityMod.Items.Materials;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Projectiles.DraedonsArsenal;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.DraedonsArsenal
{
	public class GaussRifle : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Gauss Rifle");
			Tooltip.SetDefault("Fires a devastating high velocity blast with absurd knockback");
		}

		public override void SetDefaults()
		{
			item.width = 112;
			item.height = 36;
			item.ranged = true;
			item.damage = 170;
			item.knockBack = 30f;
			item.useTime = item.useAnimation = 28;
			item.autoReuse = true;

			item.useStyle = ItemUseStyleID.HoldingOut;
			item.UseSound = mod.GetLegacySoundSlot(SoundType.Item, "Sounds/Item/GaussWeaponFire");
			item.noMelee = true;

			item.value = CalamityGlobalItem.Rarity8BuyPrice;
			item.rare = ItemRarityID.Red;
			item.Calamity().customRarity = CalamityRarity.DraedonRust;

			item.shoot = ModContent.ProjectileType<GaussRifleBlast>();
			item.shootSpeed = 27f;

			item.Calamity().Chargeable = true;
			item.Calamity().ChargeMax = 135;
		}

		public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
		{
			Projectile.NewProjectile(position, new Vector2(speedX, speedY), ModContent.ProjectileType<GaussRifleBlast>(), damage, knockBack, player.whoAmI, 0f, 0f);
			return false;
		}

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ModContent.ItemType<MysteriousCircuitry>(), 18);
			recipe.AddIngredient(ModContent.ItemType<DubiousPlating>(), 12);
			recipe.AddIngredient(ModContent.ItemType<BarofLife>(), 5);
			recipe.AddIngredient(ModContent.ItemType<InfectedArmorPlating>(), 5);
			recipe.AddIngredient(ModContent.ItemType<SpectreRifle>());
			recipe.AddTile(TileID.MythrilAnvil);
			recipe.SetResult(this);
			recipe.AddRecipe();
		}
	}
}
