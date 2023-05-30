using CalamityMod.Items.Materials;
using CalamityMod.Rarities;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Placeables.Furniture.CraftingStations
{
    public class DraedonsForge : ModItem, ILocalizedModType
    {
        public string LocalizationCategory => "Items.Placeables";
        public override void SetDefaults()
        {
            Item.width = 58;
            Item.height = 32;
            Item.maxStack = 9999;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.consumable = true;
            Item.createTile = ModContent.TileType<Tiles.Furniture.CraftingStations.DraedonsForge>();

            Item.rare = ModContent.RarityType<Violet>();
            Item.value = Item.sellPrice(platinum: 27, gold: 50);
        }

		public override void ModifyResearchSorting(ref ContentSamples.CreativeHelper.ItemGroup itemGroup)
		{
			itemGroup = ContentSamples.CreativeHelper.ItemGroup.CraftingObjects;
		}

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<CosmicAnvilItem>().
                AddRecipeGroup("HardmodeForge").
                AddIngredient(ItemID.TinkerersWorkshop).
                AddIngredient(ItemID.LunarCraftingStation).
                AddIngredient<AuricBar>(15).
                AddIngredient<ExoPrism>(12).
                AddIngredient<AscendantSpiritEssence>(25).
                Register();
        }
    }
}
