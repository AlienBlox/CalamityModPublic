using Microsoft.Xna.Framework; using CalamityMod.Dusts;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Walls
{
    public class AerialiteBrickWall : ModWall
    {
        public override void SetDefaults()
        {
            Main.wallHouse[Type] = true;
            drop = ModContent.ItemType<Items.AerialiteBrickWall>();
            AddMapEntry(new Color(51, 45, 97));
        }

        public override bool CreateDust(int i, int j, ref int type)
        {
            Dust.NewDust(new Vector2(i, j) * 16f, 16, 16, 1, 0f, 0f, 1, new Color(119, 102, 255), 1f);
            Dust.NewDust(new Vector2(i, j) * 16f, 16, 16, 10, 0f, 0f, 1, new Color(255, 255, 255), 1f);
            return false;
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }
    }
}
