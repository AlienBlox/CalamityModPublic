using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Walls
{
    public class OccultStoneWall : ModWall
    {
        public override void SetDefaults()
        {
            Main.wallHouse[Type] = true;

            drop = ModContent.ItemType<Items.Placeables.Walls.OccultStoneWall>();
            AddMapEntry(new Color(23, 23, 26));
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }

        public override bool CreateDust(int i, int j, ref int type)
        {
            Dust.NewDust(new Vector2(i, j) * 16f, 16, 16, 1, 0f, 0f, 1, new Color(125, 94, 128), 1f);
            return false;
        }
    }
}
