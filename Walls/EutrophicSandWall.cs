using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Walls
{
    public class EutrophicSandWall : ModWall
    {
        public override void SetDefaults()
        {
            dustType = 108;
        }

		public override void RandomUpdate(int i, int j)
		{
			if (Main.tile[i, j].liquid <= 0)
			{
				Main.tile[i, j].liquid = 255;
				Main.tile[i, j].lava(false);
			}
		}

		public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }
    }
}