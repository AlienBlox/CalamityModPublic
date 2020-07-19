using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Tiles.DraedonStructures
{
    public class RustedPipes : ModTile
    {
        public override void SetDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileMerge[Type][ModContent.TileType<LaboratoryPipePlating>()] = true;

            soundType = SoundID.Item;
            soundStyle = 52;
            dustType = 32;
            minPick = 30;
            drop = ModContent.ItemType<Items.Placeables.DraedonStructures.RustedPipes>();
            AddMapEntry(new Color(128, 90, 77));
        }

        public override void PlaceInWorld(int i, int j, Item item)
        {
            Main.PlaySound(SoundID.Item,i * 16, j * 16, 52, 0.75f, -0.5f);
        }
    }
}
