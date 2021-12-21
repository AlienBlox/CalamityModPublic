using CalamityMod.Items.Materials;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Tiles
{
    public class CryonicBar : ModTile
    {
        public override void SetDefaults()
        {
			this.SetUpBar(new Color(138, 43, 226));
            dustType = 44;
            drop = ModContent.ItemType<VerstaltiteBar>();
        }

        public override bool CreateDust(int i, int j, ref int type)
        {
			type = Main.rand.NextBool() ? 56 : 73;
            return true;
        }
    }
}
