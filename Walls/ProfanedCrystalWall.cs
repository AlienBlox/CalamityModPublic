using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Walls
{
	public class ProfanedCrystalWall : ModWall
	{
		public override void SetDefaults()
		{
			Main.wallHouse[Type] = true;
            Main.wallLargeFrames[Type] = 2;
			dustType = mod.DustType("Sparkle");
            soundType = 13;
			drop = mod.ItemType("ProfanedCrystalWall");
			AddMapEntry(new Color(125, 97, 123));
        }

        public override bool CreateDust(int i, int j, ref int type)
        {
            Dust.NewDust(new Vector2(i, j) * 16f, 16, 16, 205, 0f, 0f, 1, new Color(255, 255, 255), 1f);
            return false;
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
		{
			num = fail ? 1 : 3;
		}
	}
}