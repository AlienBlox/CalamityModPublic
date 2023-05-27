using CalamityMod.Tiles.Furniture.CraftingStations;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Items.Placeables.FurnitureSilva
{
    public class SilvaDresser : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 20;
            Item.maxStack = 9999;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.consumable = true;
            Item.createTile = ModContent.TileType<Tiles.FurnitureSilva.SilvaDresser>();
        }

        public override void AddRecipes()
        {
            CreateRecipe(1).AddIngredient(ModContent.ItemType<SilvaCrystal>(), 16).AddTile(ModContent.TileType<SilvaBasin>()).Register();
        }
    }
}
