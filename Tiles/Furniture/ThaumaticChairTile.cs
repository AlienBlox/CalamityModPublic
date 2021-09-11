using CalamityMod.Items.Placeables.Furniture;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Tiles.Furniture
{
    public class ThaumaticChairTile : ModTile
    {
        public override void SetDefaults()
        {
            this.SetUpChair(true);
            ModTranslation name = CreateMapEntryName();
            name.SetDefault("Thaumatic Chair");
            AddMapEntry(new Color(236, 123, 89), name);
            disableSmartCursor = true;
            adjTiles = new int[] { TileID.Chairs };
        }

        public override bool CreateDust(int i, int j, ref int type)
        {
            Dust.NewDust(new Vector2(i, j) * 16f, 16, 16, 7, 0f, 0f, 1, default, 1f);
            return false;
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            Item.NewItem(i * 16, j * 16, 16, 32, ModContent.ItemType<ThaumaticChair>());
        }
    }
}
