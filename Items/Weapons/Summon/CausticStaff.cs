using CalamityMod.Projectiles.Summon;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Items.Weapons.Summon
{
    public class CausticStaff : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Caustic Staff");
            Tooltip.SetDefault("Summons a mini dragon to fight for you\n" +
                               "The dragon inflicts several debilitating debuffs");
        }

        public override void SetDefaults()
        {
			item.mana = 10;
			item.damage = 15;
			item.useStyle = 1;
			item.shootSpeed = 10f;
			item.shoot = ModContent.ProjectileType<CausticStaffSummon>();
			item.width = 26;
			item.height = 28;
			item.UseSound = SoundID.Item77;
			item.useAnimation = item.useTime = 25;
			item.rare = 4;
			item.noMelee = true;
			item.knockBack = 2f;
			item.summon = true;
            item.value = Item.buyPrice(0, 12, 0, 0);
            item.Calamity().customRarity = CalamityRarity.Dedicated;
			item.autoReuse = true;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (player.altFunctionUse != 2)
            {
                Projectile.NewProjectile(Main.MouseWorld, Vector2.Zero, type, damage, knockBack, player.whoAmI);
            }
            return false;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddRecipeGroup("AnyEvilFlask", 5);
            recipe.AddIngredient(ItemID.Deathweed, 2);
            recipe.AddIngredient(ItemID.SoulofNight, 10);
            recipe.AddRecipeGroup("AnyEvilBar", 10);
            recipe.AddTile(TileID.DemonAltar);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
