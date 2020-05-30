using CalamityMod.Tiles.Furniture.CraftingStations;
using CalamityMod.Items.Placeables.Walls;
using CalamityMod.Items.Placeables.FurnitureAstral;
using Terraria.ID;
using Terraria.ModLoader; // If you are using c# 6, you can use: "using static Terraria.Localization.GameCulture;" which would mean you could just write "DisplayName.AddTranslation(German, "");"

namespace CalamityMod.Items.Placeables
{
    public class AstralMonolith : ModItem
    {
        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            item.width = 12;
            item.height = 12;
            item.maxStack = 999;
            item.useTurn = true;
            item.autoReuse = true;
            item.useAnimation = 15;
            item.useTime = 10;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.consumable = true;
            item.createTile = ModContent.TileType<Tiles.Astral.AstralMonolith>();
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<AstralMonolithWall>(), 4);
            recipe.SetResult(this, 1);
            recipe.AddTile(ModContent.TileType<MonolithCrafting>());
            recipe.AddRecipe();
            recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<MonolithPlatform>(), 2);
            recipe.SetResult(this, 1);
            recipe.AddRecipe();
        }
    }
}
