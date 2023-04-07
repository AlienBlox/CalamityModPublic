using CalamityMod.Rarities;
using CalamityMod.Tiles.Furniture.CraftingStations;
using CalamityMod.Tiles.FurnitureExo;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Items.Placeables.FurnitureExo
{
    public class ExoKeyboard : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.SetNameOverride("Exo Keyboard");
            Item.width = 28;
            Item.height = 20;
            Item.maxStack = 999;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.consumable = true;
            Item.createTile = ModContent.TileType<ExoKeyboardTile>();
            Item.rare = ModContent.RarityType<Violet>();
        }

        public override void AddRecipes()
        {
            CreateRecipe(1).AddIngredient(ItemID.Bone, 4).AddIngredient(ModContent.ItemType<ExoPlating>(), 15).AddIngredient(ItemID.Book).AddTile(ModContent.TileType<DraedonsForge>()).Register();
        }
    }
}
