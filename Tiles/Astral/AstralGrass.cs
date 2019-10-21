
using CalamityMod.Dusts;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Tiles.Astral
{
    public class AstralGrass : ModTile
    {
        public override void SetDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileBrick[Type] = true;

            TileMerge.MergeGeneralTiles(Type);
            TileMerge.MergeAstralTiles(Type);
            TileMerge.MergeOreTiles(Type);

            dustType = ModContent.DustType<AstralBasic>();
            drop = ModContent.ItemType<Items.Placeables.AstralDirt>();

            AddMapEntry(new Color(133, 109, 140));

            TileID.Sets.Grass[Type] = true;
            TileID.Sets.Conversion.Grass[Type] = true;

            //Grass framing (<3 terraria devs)
            TileID.Sets.NeedsGrassFraming[Type] = true;
            TileID.Sets.NeedsGrassFramingDirt[Type] = ModContent.TileType<AstralDirt>();

            SetModTree(new AstralTree());
        }

        public override void NumDust(int i, int j, bool fail, ref int Type)
        {
            Type = fail ? 1 : 3;
        }

        public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem)
        {
            if (fail && !effectOnly)
            {
                Main.tile[i, j].type = (ushort)ModContent.TileType<AstralDirt>();
            }
        }

        public override int SaplingGrowthType(ref int style)
        {
            style = 0;
            return ModContent.TileType<AstralTreeSapling>();
        }
    }
}
