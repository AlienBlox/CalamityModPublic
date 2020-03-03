using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Ranged;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Items.Ammo
{
    public class HyperiusBullet : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Hyperius Bullet");
            Tooltip.SetDefault("Your enemies might have a bad time\n" +
                "Spawns additional bullets on enemy hits");
        }

        public override void SetDefaults()
        {
            item.damage = 18;
            item.ranged = true;
            item.width = 8;
            item.height = 8;
            item.maxStack = 999;
            item.consumable = true;
            item.knockBack = 1.5f;
            item.value = 2000;
            item.rare = 9;
            item.shoot = ModContent.ProjectileType<HyperiusBulletProj>();
            item.shootSpeed = 16f;
            item.ammo = 97;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.MusketBall, 150);
            recipe.AddIngredient(ModContent.ItemType<BarofLife>());
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this, 150);
            recipe.AddRecipe();
        }
    }
}
