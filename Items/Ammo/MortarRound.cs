using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Items
{
    public class MortarRound : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Mortar Round");
            Tooltip.SetDefault("Large blast radius. Will destroy tiles\n" +
                "Used by normal guns");
        }

        public override void SetDefaults()
        {
            item.damage = 20;
            item.ranged = true;
            item.width = 20;
            item.height = 14;
            item.maxStack = 999;
            item.consumable = true;
            item.knockBack = 7.5f;
            item.value = 500;
            item.rare = 3;
            item.ammo = 97;
            item.shoot = ModContent.ProjectileType<Projectiles.MortarRound>();
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.RocketIV, 100);
            recipe.AddIngredient(null, "UeliaceBar");
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this, 100);
            recipe.AddRecipe();
        }
    }
}
