using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Rogue;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Rogue
{
	public class SearedPan : RogueWeapon
	{
		// Attacks must be within 48 frames of each other to count as "consecutive" hits
		// This is a little less than double the use time
        public static int ConsecutiveHitOpening = 48;
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Seared Pan");
			Tooltip.SetDefault("dAMaGe iS rAthEr cOnSisTeNT\n" +
				"Fires a frying pan at high velocity\n" +
				"Enemy hits summon fireballs that linger around the target\n" +
				"Landing three consecutive hits grants full stealth\n" +
				"Stealth strikes explode into golden sparks, and after hitting an enemy,\n" +
				"they cause all fireballs to aggressively home in on their target");
		}

		public override void SafeSetDefaults()
		{
			item.width = 60;
			item.height = 36;
			item.damage = 666;
			item.Calamity().rogue = true;
			item.knockBack = 10f;
			item.noMelee = true;
			item.noUseGraphic = true;
			item.useStyle = ItemUseStyleID.SwingThrow;
			item.useTime = item.useAnimation = 28;
			item.reuseDelay = 1;
			item.UseSound = SoundID.Item1;
			item.autoReuse = true;
			item.value = CalamityGlobalItem.Rarity15BuyPrice;
            item.Calamity().customRarity = CalamityRarity.Violet;
			item.shoot = ModContent.ProjectileType<SearedPanProjectile>();
			item.shootSpeed = 15f;
            item.Calamity().donorItem = true;
		}

		public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
		{
			int pan = Projectile.NewProjectile(position, new Vector2(speedX, speedY), type, damage, knockBack, player.whoAmI);
			if (player.Calamity().StealthStrikeAvailable() && pan.WithinBounds(Main.maxProjectiles))
			{
				Main.projectile[pan].Calamity().stealthStrike = true;
				Main.projectile[pan].extraUpdates++;
			}
			return false;
		}

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<UtensilPoker>());
            recipe.AddIngredient(ModContent.ItemType<AuricBar>(), 5);
            recipe.AddIngredient(ItemID.Bacon, 4);
            recipe.AddIngredient(ItemID.LifeCrystal);
            recipe.AddIngredient(ItemID.ManaCrystal);
            recipe.AddIngredient(ItemID.Bone, 92);
            recipe.AddTile(ModContent.TileType<CosmicAnvil>());
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
	}
}
