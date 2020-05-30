using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables;
using CalamityMod.Projectiles.Rogue;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Rogue
{
	public class Apoctolith : RogueWeapon
	{
		public override void SetStaticDefaults()
		{
			Tooltip.SetDefault("Maybe catching bricks with your face isn't such a hot idea...\n" +
				"Critical hits tear away enemy defense\n" +
				"Stealth strikes shatter and briefly stun enemies");
			DisplayName.SetDefault("Apoctolith");
		}

		public override void SafeSetDefaults()
		{
			item.damage = 108;
			item.shootSpeed = 15f;
			item.shoot = ModContent.ProjectileType<ApoctolithProj>();
			item.width = 66;
			item.height = 64;
			item.useTime = 25;
			item.useAnimation = 25;
			item.useStyle = 1;
			item.knockBack = 10f;
			item.crit = 20;
			item.value = CalamityGlobalItem.Rarity7BuyPrice;
			item.rare = 7;
			item.UseSound = SoundID.Item1;
			item.noMelee = true;
			item.noUseGraphic = true;
			item.Calamity().rogue = true;
			item.autoReuse = true;
		}

		public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
		{
			//Check if stealth is full
			if (player.Calamity().StealthStrikeAvailable())
			{
				int p = Projectile.NewProjectile(position.X, position.Y, speedX, speedY, type, damage, knockBack, player.whoAmI);
				Main.projectile[p].Calamity().stealthStrike = true;
				return false;
			}
			return true;
		}

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ModContent.ItemType<ThrowingBrick>(), 100);
			recipe.AddIngredient(ModContent.ItemType<Voidstone>(), 20);
			recipe.AddIngredient(ModContent.ItemType<Lumenite>(), 8);
			recipe.AddTile(TileID.MythrilAnvil);
			recipe.SetResult(this);
			recipe.AddRecipe();
		}

		public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
		{
			Vector2 origin = new Vector2(item.width / 2f, item.height / 2f - 2f);
			spriteBatch.Draw(ModContent.GetTexture("CalamityMod/Items/Weapons/Rogue/ApoctolithGlow"), item.Center - Main.screenPosition, null, Color.White, rotation, origin, 1f, SpriteEffects.None, 0f);
		}
	}
}
