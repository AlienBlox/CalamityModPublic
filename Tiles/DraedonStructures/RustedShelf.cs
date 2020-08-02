using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Tiles.DraedonStructures
{
    public class RustedShelf : ModTile
    {
        public override void SetDefaults()
        {
            this.SetUpPlatform(true);
            soundType = SoundID.Tink;
            dustType = 32;
            AddMapEntry(new Color(128, 90, 77));
            drop = ModContent.ItemType<Items.Placeables.DraedonStructures.RustedShelf>();
            disableSmartCursor = true;
            adjTiles = new int[] { TileID.Platforms };
        }

        public override void PostSetDefaults()
        {
            Main.tileNoSunLight[Type] = false;
        }
    }
}
