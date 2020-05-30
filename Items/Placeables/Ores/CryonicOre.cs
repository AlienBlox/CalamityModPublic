using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Items.Placeables.Ores
{
    public class CryonicOre : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cryonic Ore");
        }

        public override void SetDefaults()
        {
            item.createTile = ModContent.TileType<Tiles.Ores.CryonicOre>();
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.useTurn = true;
            item.useAnimation = 15;
            item.useTime = 10;
            item.autoReuse = true;
            item.consumable = true;
            item.width = 13;
            item.height = 10;
            item.maxStack = 999;
            item.value = Item.sellPrice(silver: 12);
            item.rare = 5;
        }
    }
}
