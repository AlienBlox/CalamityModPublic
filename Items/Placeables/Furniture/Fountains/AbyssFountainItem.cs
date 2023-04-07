using Terraria;
using Terraria.ModLoader;
using CalamityMod.Tiles.Furniture.Fountains;
using Terraria.ID;

namespace CalamityMod.Items.Placeables.Furniture.Fountains
{
    public class AbyssFountainItem : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
            // DisplayName.SetDefault("Abyss Water Fountain");
        }

        public override void SetDefaults()
        {
            Item.width = 22;
            Item.height = 42;
            Item.maxStack = 999;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.consumable = true;
            Item.value = Item.buyPrice(0, 4, 0, 0);
            Item.rare = ItemRarityID.White;
            Item.createTile = ModContent.TileType<AbyssFountainTile>();
        }
    }
}
