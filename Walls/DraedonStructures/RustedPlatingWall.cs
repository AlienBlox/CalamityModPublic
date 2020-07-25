using Terraria;
using Microsoft.Xna.Framework;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Walls.DraedonStructures
{
    public class RustedPlatingWall : ModWall
    {

        public override void SetDefaults()
        {
            dustType = 32;
            drop = ModContent.ItemType<Items.Placeables.Walls.DraedonStructures.RustedPlatingWall>();
            Main.wallHouse[Type] = true;

            AddMapEntry(new Color(83, 59, 50));
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }
    }
}
