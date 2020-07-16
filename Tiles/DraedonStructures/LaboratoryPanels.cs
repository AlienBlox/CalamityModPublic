using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Tiles.DraedonStructures
{
    public class LaboratoryPanels : ModTile
    {
        public override void SetDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileMergeDirt[Type] = true;

            CalamityUtils.MergeWithGeneral(Type);

            soundType = SoundID.Tink;
            dustType = 109;
            minPick = 30;
            drop = ModContent.ItemType<Items.Placeables.DraedonStructures.LaboratoryPanels>();
            AddMapEntry(new Color(36, 35, 37));
        }
    }
}
