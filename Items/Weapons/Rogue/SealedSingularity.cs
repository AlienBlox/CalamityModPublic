using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Rogue;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Rogue
{
	public class SealedSingularity : RogueWeapon
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Sealed Singularity");
			Tooltip.SetDefault("Shatters on impact, summoning a black hole that sucks in nearby enemies\n" +
			"Stealth strikes summon a black hole that lasts longer and sucks enemies with stronger force");
		}

		public override void SafeSetDefaults()
		{
			item.damage = 110;
			item.knockBack = 5f;
			item.useAnimation = item.useTime = 25;
			item.useStyle = 1;
			item.Calamity().rogue = true;
			item.autoReuse = true;
			item.shoot = ModContent.ProjectileType<SealedSingularityProj>();
			item.shootSpeed = 12f;

			item.noMelee = item.noUseGraphic = true;
			item.height = item.width = 34;
			item.UseSound = SoundID.Item106;
			item.value = CalamityGlobalItem.Rarity12BuyPrice;
			item.rare = ItemRarityID.Purple;
			item.Calamity().customRarity = CalamityRarity.Dedicated;
		}

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (player.Calamity().StealthStrikeAvailable()) //setting the stealth strike
            {
                int stealth = Projectile.NewProjectile(position, new Vector2(speedX, speedY), type, damage, knockBack, player.whoAmI);
				if (stealth.WithinBounds(Main.maxProjectiles))
					Main.projectile[stealth].Calamity().stealthStrike = true;
                return false;
            }
            return true;
        }

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ModContent.ItemType<DuststormInABottle>());
			recipe.AddIngredient(ModContent.ItemType<DarkPlasma>(), 3);
			recipe.AddTile(TileID.LunarCraftingStation);
			recipe.SetResult(this);
			recipe.AddRecipe();
		}
    }
}
