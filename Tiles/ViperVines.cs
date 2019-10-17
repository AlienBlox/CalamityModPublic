using Microsoft.Xna.Framework;
using Terraria; using CalamityMod.Projectiles; using Terraria.ModLoader; using CalamityMod.Dusts;
using Terraria.ID;
using Terraria.ModLoader; using CalamityMod.Dusts; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles; using CalamityMod.Tiles; using CalamityMod.Walls;

namespace CalamityMod.Tiles
{
    public class ViperVines : ModTile
    {
        public override void SetDefaults()
        {
            Main.tileCut[Type] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileLavaDeath[Type] = true;
            Main.tileNoFail[Type] = true;
            ModTranslation name = CreateMapEntryName();
            name.SetDefault("Viper Vines");
            AddMapEntry(new Color(0, 50, 0), name);
            soundType = 6;
            dustType = 2;
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }

        public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem)
        {
            //GIVE VINE ROPE IF SPECIAL VINE BOOK
            if (WorldGen.genRand.Next(2) == 0 && Main.player[(int)Player.FindClosest(new Vector2((float)(i * 16), (float)(j * 16)), 16, 16)].cordage)
            {
                Item.NewItem(new Vector2(i * 16 + 8f, j * 16 + 8f), ItemID.VineRope);
            }
            if (Main.tile[i, j + 1] != null)
            {
                if (Main.tile[i, j + 1].active())
                {
                    if (Main.tile[i, j + 1].type == ModContent.TileType<ViperVines>())
                    {
                        WorldGen.KillTile(i, j + 1, false, false, false);
                        if (!Main.tile[i, j + 1].active() && Main.netMode != NetmodeID.SinglePlayer)
                        {
                            NetMessage.SendData(17, -1, -1, null, 0, (float)i, (float)j + 1, 0f, 0, 0, 0);
                        }
                    }
                }
            }
        }

        public override void RandomUpdate(int i, int j)
        {
            if (Main.tile[i, j + 1] != null)
            {
                if (!Main.tile[i, j + 1].active() && Main.tile[i, j + 1].type != (ushort)ModContent.TileType<ViperVines>())
                {
                    if (Main.tile[i, j + 1].liquid >= 128 && !Main.tile[i, j + 1].lava())
                    {
                        bool flag13 = false;
                        for (int num52 = j; num52 > j - 10; j--)
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
                            Main.tile[num53, num54].type = (ushort)ModContent.TileType<ViperVines>();
                            Main.tile[num53, num54].frameX = (short)(WorldGen.genRand.Next(8) * 18);
                            Main.tile[num53, num54].frameY = (short)(4 * 18);
                            Main.tile[num53, num54 - 1].frameX = (short)(WorldGen.genRand.Next(12) * 18);
                            Main.tile[num53, num54 - 1].frameY = (short)(WorldGen.genRand.Next(4) * 18);
                            Main.tile[num53, num54].active(true);
                            WorldGen.SquareTileFrame(num53, num54, true);
                            WorldGen.SquareTileFrame(num53, num54 - 1, true);
                            if (Main.netMode == NetmodeID.Server)
                            {
                                NetMessage.SendTileSquare(-1, num53, num54, 3, TileChangeType.None);
                                NetMessage.SendTileSquare(-1, num53, num54 - 1, 3, TileChangeType.None);
                            }
                        }
                    }
                }
            }
        }
    }
}
