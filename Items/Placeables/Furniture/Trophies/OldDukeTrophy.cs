using CalamityMod.Tiles.Furniture.BossTrophies;
using Terraria.ModLoader;
using Terraria.ID;
namespace CalamityMod.Items.Placeables.Furniture.Trophies
{
    public class OldDukeTrophy : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
            // DisplayName.SetDefault("Old Duke Trophy");
        }

        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 30;
            Item.maxStack = Item.CommonMaxStack;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.consumable = true;
            Item.value = 50000;
            Item.rare = ItemRarityID.Blue;
            Item.createTile = ModContent.TileType<OldDukeTrophyTile>();
        }
    }
}
