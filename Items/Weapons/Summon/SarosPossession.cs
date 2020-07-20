using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Summon;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Summon
{
    public class SarosPossession : ModItem
    {
		int radianceSlots;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Saros Possession");
            Tooltip.SetDefault("Gain absolute control over light itself\n" +
							   "Summons a radiant aura\n" +
                               "Consumes all of the remaining minion slots on use\n" +
							   "Must be used from the hotbar\n" +
                               "Increased power based on the number of minion slots used");
        }

        public override void SetDefaults()
        {
            item.width = 56;
            item.height = 56;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.noMelee = true;
            item.UseSound = SoundID.DD2_BetsyFlameBreath;

            item.summon = true;
            item.mana = 100;
            item.damage = 426;
            item.knockBack = 4f;
            item.useTime = item.useAnimation = 10;
            item.shoot = ModContent.ProjectileType<RadiantResolutionAura>();
            item.shootSpeed = 10f;

            item.value = Item.buyPrice(2, 50, 0, 0);
            item.rare = 10;
            item.Calamity().customRarity = CalamityRarity.Violet;
        }

		public override void HoldItem(Player player)
        {
			double minionCount = 0;
			for (int j = 0; j < Main.projectile.Length; j++)
			{
                Projectile proj = Main.projectile[j];
				if (proj.active && proj.owner == player.whoAmI && proj.minion && proj.type != item.shoot)
				{
					minionCount += proj.minionSlots;
				}
			}
            radianceSlots = (int)(player.maxMinions - minionCount);
		}

        public override bool CanUseItem(Player player)
		{
			return radianceSlots >= 1;
		}

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            for (int x = 0; x < Main.projectile.Length; x++)
            {
                Projectile proj = Main.projectile[x];
                if (proj.active && proj.owner == player.whoAmI && proj.type == type)
                {
                    proj.Kill();
					break;
                }
            }
            Projectile.NewProjectile(position, Vector2.Zero, type, damage, knockBack, player.whoAmI, radianceSlots);
            return false;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<Sirius>());
            recipe.AddIngredient(ModContent.ItemType<DarksunFragment>(), 50);
            recipe.AddTile(ModContent.TileType<DraedonsForge>());
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
