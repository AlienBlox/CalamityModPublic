using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Tiles.FurnitureStratus
{
    public class StratusWorkbench : ModTile
    {
        public override void SetDefaults()
        {
            this.SetUpWorkBench();
            ModTranslation name = CreateMapEntryName();
            name.SetDefault("Stratus Work Bench");
            AddMapEntry(new Color(191, 142, 111), name);
            disableSmartCursor = true;
            adjTiles = new int[] { TileID.WorkBenches };
        }

        public override bool CreateDust(int i, int j, ref int type)
        {
            Dust.NewDust(new Vector2(i, j) * 16f, 16, 16, 1, 0f, 0f, 1, new Color(100, 130, 150), 1f);
            Dust.NewDust(new Vector2(i, j) * 16f, 16, 16, 132, 0f, 0f, 1, new Color(255, 255, 255), 1f);
            return false;
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            Item.NewItem(i * 16, j * 16, 32, 16, ModContent.ItemType<Items.Placeables.FurnitureStratus.StratusWorkbench>());
        }
    }
}
