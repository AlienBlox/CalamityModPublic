using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Tiles.Abyss
{
    public class HardenedSulphurousSandstone : ModTile
    {
        public override void SetDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileBlockLight[Type] = true;

            CalamityUtils.MergeWithGeneral(Type);
            CalamityUtils.MergeWithAbyss(Type);

            dustType = 32;
            drop = ModContent.ItemType<Items.Placeables.HardenedSulphurousSandstone>();
            AddMapEntry(new Color(76, 58, 59));
            mineResist = 1f;
            minPick = 55;
            soundType = 0;
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }
        public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem)
        {
            if (CalamityUtils.ParanoidTileRetrieval(i, j + 1).active() &&
                CalamityUtils.ParanoidTileRetrieval(i, j + 1).type == (ushort)ModContent.TileType<SulphurousVines>())
            {
                WorldGen.KillTile(i, j + 1);
            }
        }
        public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
        {
            TileFraming.CustomMergeFrame(i, j, Type, ModContent.TileType<AbyssGravel>(), false, false, false, false, resetFrame);
            return false;
        }
        public override void RandomUpdate(int i, int j)
        {
            int num8 = WorldGen.genRand.Next((int)Main.rockLayer, (int)(Main.rockLayer + (double)Main.maxTilesY * 0.143));
            int nearbyVineCount = 0;
            for (int x = i - 15; x <= i + 15; x++)
            {
                for (int y = j - 15; y <= j + 15; y++)
                {
                    if (WorldGen.InWorld(x, y))
                    {
                        if (CalamityUtils.ParanoidTileRetrieval(x, y).active() &&
                            CalamityUtils.ParanoidTileRetrieval(x, y).type == (ushort)ModContent.TileType<SulphurousVines>())
                        {
                            nearbyVineCount++;
                        }
                    }
                }
            }
            if (Main.tile[i, j + 1] != null && nearbyVineCount < 5)
            {
                if (!Main.tile[i, j + 1].active() && Main.tile[i, j + 1].type != (ushort)ModContent.TileType<SulphurousVines>())
                {
                    if (Main.tile[i, j + 1].liquid == 255 &&
                        !Main.tile[i, j + 1].lava())
                    {
                        bool flag13 = false;
                        for (int num52 = num8; num52 > num8 - 10; num52--)
                        {
                            if (Main.tile[i, num52].bottomSlope())
                            {
                                flag13 = false;
                                break;
                            }
                            if (Main.tile[i, num52].active() && !Main.tile[i, num52].bottomSlope())
                            {
                                flag13 = true;
                                break;
                            }
                        }
                        if (flag13)
                        {
                            int num53 = i;
                            int num54 = j + 1;
                            Main.tile[num53, num54].type = (ushort)ModContent.TileType<SulphurousVines>();
                            Main.tile[num53, num54].active(true);
                            WorldGen.SquareTileFrame(num53, num54, true);
                            if (Main.netMode == 2)
                            {
                                NetMessage.SendTileSquare(-1, num53, num54, 3, TileChangeType.None);
                            }
                        }
                    }
                    Main.tile[i, j].slope(0);
                    Main.tile[i, j].halfBrick(false);
                }
            }
        }
    }
}
