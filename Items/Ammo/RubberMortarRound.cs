using CalamityMod.Projectiles.Ranged;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Items.Ammo
{
    public class RubberMortarRound : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Rubber Mortar Round");
            Tooltip.SetDefault("Large blast radius\n" +
                "Will destroy tiles on each bounce\n" +
                "Used by normal guns");
        }

        public override void SetDefaults()
        {
            item.damage = 25;
            item.ranged = true;
            item.width = 20;
            item.height = 14;
            item.maxStack = 999;
            item.consumable = true;
            item.knockBack = 7.5f;
            item.value = 1000;
            item.rare = 5;
            item.ammo = 97;
            item.shoot = ModContent.ProjectileType<RubberMortarRoundProj>();
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "MortarRound", 100);
            recipe.AddIngredient(ItemID.PinkGel, 5);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this, 100);
            recipe.AddRecipe();
        }
    }
}
