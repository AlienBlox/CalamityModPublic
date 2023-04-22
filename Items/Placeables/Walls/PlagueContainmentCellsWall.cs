using Terraria.ModLoader;
using WallTiles = CalamityMod.Walls;
using Terraria.ID;

namespace CalamityMod.Items.Placeables.Walls
{
    public class PlagueContainmentCellsWall : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 400;
            // DisplayName.SetDefault("Plagueplate Wall");
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
            Item.createWall = ModContent.WallType<WallTiles.PlagueContainmentCellsWall>();
        }

        public override void AddRecipes()
        {
            CreateRecipe(4).AddIngredient(ModContent.ItemType<Plates.Plagueplate>()).AddTile(TileID.WorkBenches).Register();
        }
    }
}
