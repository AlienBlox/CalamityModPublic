using CalamityMod.Rarities;
using CalamityMod.Tiles.DraedonStructures;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Placeables.DraedonStructures
{
    public class ChargingStationItem : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
            // DisplayName.SetDefault("Charging Station");
            /* Tooltip.SetDefault("Charges Draedon's Arsenal items using Power Cells\n" +
                "Place both an item and Power Cells into the Charging Station to charge the item"); */
        }

        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 26;
            Item.maxStack = 9999;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.useAnimation = Item.useTime = 15;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.consumable = true;
            Item.createTile = ModContent.TileType<ChargingStation>();

            Item.rare = ModContent.RarityType<DarkOrange>();
            Item.value = Item.buyPrice(gold: 50);
        }
    }
}
