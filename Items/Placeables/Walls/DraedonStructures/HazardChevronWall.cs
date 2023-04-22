using Terraria.ModLoader;
using WallTiles = CalamityMod.Walls.DraedonStructures;
using TileItems = CalamityMod.Items.Placeables.DraedonStructures;
using Terraria.ID;

namespace CalamityMod.Items.Placeables.Walls.DraedonStructures
{
    public class HazardChevronWall : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 400;
        }

        public override void SetDefaults()
        {
            Item.width = 12;
            Item.height = 12;
            Item.maxStack = Item.CommonMaxStack;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.useAnimation = 15;
            Item.useTime = 7;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.consumable = true;
            Item.createWall = ModContent.WallType<WallTiles.HazardChevronWall>();
        }

        public override void AddRecipes()
        {
            CreateRecipe(4).AddIngredient(ModContent.ItemType<TileItems.HazardChevronPanels>()).AddTile(TileID.WorkBenches).Register();
        }
    }
}
