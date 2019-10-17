using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Items
{
    public class TerraBullet : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Terra Bullet");
            Tooltip.SetDefault("Explodes and splits into homing terra shards on death");
        }

        public override void SetDefaults()
        {
            item.damage = 9;
            item.ranged = true;
            item.width = 8;
            item.height = 8;
            item.maxStack = 999;
            item.consumable = true;
            item.knockBack = 1.25f;
            item.value = 2000;
            item.rare = 7;
            item.shoot = ModContent.ProjectileType<Projectiles.TerraBullet>();
            item.shootSpeed = 10f;
            item.ammo = 97;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.CrystalBullet, 100);
            recipe.AddIngredient(null, "LivingShard");
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this, 100);
            recipe.AddRecipe();
        }
    }
}
