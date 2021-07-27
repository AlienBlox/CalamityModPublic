using CalamityMod.Items.Accessories;
using CalamityMod.Items.Mounts;
using CalamityMod.Items.Placeables.Furniture;
using CalamityMod.Items.SummonItems;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.World
{
    public class UndergroundShrines : ModWorld
    {
        #region Enumeration
        public enum UndergroundShrineType
        {
            Surface,
            Cavern,
            WorldEvil,
            Ice,
            Desert,
            Mushroom,
            Granite,
            Marble,
            Abyss
        }
        #endregion

        #region Hut Outline Creation

        // Special Hut: Takes arguments of tile type 1, tile type 2, wall type, hut type (useful if you use this method to generate different huts), and location of the shrine (x and y)
        public static void SpecialHut(ushort tile, ushort tile2, ushort wall, UndergroundShrineType hutType, int shrineLocationX, int shrineLocationY)
        {
            // Random variables for shrine size
            int randomX = WorldGen.genRand.Next(2, 4);
            int randomY = WorldGen.genRand.Next(2, 4);

            // Replace tiles in shrine area with shrine tile type 1
            for (int x = shrineLocationX - randomX - 1; x <= shrineLocationX + randomX + 1; x++)
            {
                for (int y = shrineLocationY - randomY - 1; y <= shrineLocationY + randomY + 1; y++)
                {
                    Main.tile[x, y].active(true);
                    Main.tile[x, y].type = tile;
                    Main.tile[x, y].slope(0);
                    Main.tile[x, y].liquid = 0;
                    Main.tile[x, y].lava(false);
                }
            }

            // Replace walls in shrine area with shrine wall type
            for (int x = shrineLocationX - randomX; x <= shrineLocationX + randomX; x++)
            {
                for (int y = shrineLocationY - randomY; y <= shrineLocationY + randomY; y++)
                {
                    Main.tile[x, y].active(false);
                    Main.tile[x, y].wall = wall;
                }
            }

            // Remove tiles from the inner part of the shrine area
            for (int x = shrineLocationX - randomX - 1; x <= shrineLocationX + randomX + 1; x++)
            {
                for (int y = shrineLocationY + randomY - 2; y <= shrineLocationY + randomY; y++)
                    Main.tile[x, y].active(false);
            }
            for (int x = shrineLocationX - randomX - 1; x <= shrineLocationX + randomX + 1; x++)
            {
                for (int y = shrineLocationY + randomY - 2; y <= shrineLocationY + randomY - 1; y++)
                    Main.tile[x, y].active(false);
            }

            // Replace tiles from bottom of shrine area with shrine tile type 2
            for (int x = shrineLocationX - randomX - 1; x <= shrineLocationX + randomX + 1; x++)
            {
                int verticalOffset = 4;
                int y = shrineLocationY + randomY + 2;
                while (!Main.tile[x, y].active() && y < Main.maxTilesY && verticalOffset > 0)
                {
                    Main.tile[x, y].active(true);
                    Main.tile[x, y].type = tile2;
                    Main.tile[x, y].slope(0);
                    y++;
                    verticalOffset--;
                }
            }

            // Replace tiles from top of shrine with shrine tile type 1
            randomX -= WorldGen.genRand.Next(1, 3);
            int num21 = shrineLocationY - randomY - 2;
            while (randomX > -1)
            {
                for (int x = shrineLocationX - randomX - 1; x <= shrineLocationX + randomX + 1; x++)
                {
                    Main.tile[x, num21].active(true);
                    Main.tile[x, num21].type = tile;
                }
                randomX -= WorldGen.genRand.Next(1, 3);
                num21--;
            }

            // Place shrine chest
            CalamityWorld.SChestX[(int)hutType] = shrineLocationX;
            CalamityWorld.SChestY[(int)hutType] = shrineLocationY;
            SpecialChest(hutType);
        }
        #endregion

        #region Chest Creation
        // Special Chest: Used for placing shrine chests, takes argument of the shrine type which dictates what item will spawn in the first slot of this chest
        public static void SpecialChest(UndergroundShrineType shrineType)
        {
            int item = 0;
            int chestType = 0;

            switch (shrineType)
            {
                case UndergroundShrineType.Surface:
                    item = ModContent.ItemType<TrinketofChi>();
                    break;
                case UndergroundShrineType.Cavern:
                    item = ModContent.ItemType<OnyxExcavatorKey>();
                    chestType = 44;
                    break;
                case UndergroundShrineType.WorldEvil:
                    item = WorldGen.crimson ? ModContent.ItemType<CrimsonEffigy>() : ModContent.ItemType<CorruptionEffigy>();
                    chestType = WorldGen.crimson ? 43 : 3;
                    break;
                case UndergroundShrineType.Ice:
                    item = ModContent.ItemType<TundraLeash>();
                    chestType = 47;
                    break;
                case UndergroundShrineType.Desert:
                    item = ModContent.ItemType<LuxorsGift>();
                    chestType = 30;
                    break;
                case UndergroundShrineType.Mushroom:
                    item = ModContent.ItemType<FungalSymbiote>();
                    chestType = 32;
                    break;
                case UndergroundShrineType.Granite:
                    item = ModContent.ItemType<UnstablePrism>();
                    chestType = 50;
                    break;
                case UndergroundShrineType.Marble:
                    item = ModContent.ItemType<GladiatorsLocket>();
                    chestType = 51;
                    break;
                case UndergroundShrineType.Abyss:
                    item = ModContent.ItemType<BossRush>();
                    chestType = 4;
                    break;
            }

            // Destroy tiles in chest spawn location
            for (int j = CalamityWorld.SChestX[(int)shrineType] - 1; j <= CalamityWorld.SChestX[(int)shrineType] + 1; j++)
            {
                for (int k = CalamityWorld.SChestY[(int)shrineType]; k <= CalamityWorld.SChestY[(int)shrineType] + 2; k++)
                    WorldGen.KillTile(j, k, false, false, false);
            }

            // Attempt to fix sloped tiles under the chest to prevent the chest from killing itself (literally)
            for (int l = CalamityWorld.SChestX[(int)shrineType] - 1; l <= CalamityWorld.SChestX[(int)shrineType] + 1; l++)
            {
                for (int m = CalamityWorld.SChestY[(int)shrineType]; m <= CalamityWorld.SChestY[(int)shrineType] + 3; m++)
                {
                    if (m < Main.maxTilesY)
                    {
                        Main.tile[l, m].slope(0);
                        Main.tile[l, m].halfBrick(false);
                    }
                }
            }

            // Place the chest, finally
            WorldGen.AddBuriedChest(CalamityWorld.SChestX[(int)shrineType], CalamityWorld.SChestY[(int)shrineType], item, false, chestType);
        }
        #endregion

        #region Direct Gen
        public static void PlaceShrines()
        {
            int x = Main.maxTilesX;
            int y = Main.maxTilesY;
            int genLimit = x / 2;
            int generateBack = genLimit - 80; //Small = 2020
            int generateForward = genLimit + 80; //Small = 2180
			double shrineChance = 100E-05;

			for (int k = 0; k < (int)(x * y * shrineChance); k++) //Surface Shrine
            {
                int tilesX = WorldGen.genRand.Next((int)(x * 0.35), generateBack);
                int tilesX2 = WorldGen.genRand.Next(generateForward, (int)(x * 0.65));
                int tilesY = WorldGen.genRand.Next((int)(y * 0.3f), (int)(y * 0.35f));

                if (Main.tile[tilesX, tilesY].type == TileID.Dirt || Main.tile[tilesX, tilesY].type == TileID.Stone)
                {
                    SpecialHut(TileID.RedBrick, TileID.Dirt, WallID.RedBrick, 0, tilesX, tilesY);
                    break;
                }
                if (Main.tile[tilesX2, tilesY].type == TileID.Dirt || Main.tile[tilesX2, tilesY].type == TileID.Stone)
                {
                    SpecialHut(TileID.RedBrick, TileID.Dirt, WallID.RedBrick, 0, tilesX2, tilesY);
                    break;
                }
            }

            for (int k = 0; k < (int)(x * y * shrineChance); k++) //Evil Shrine
            {
                int tilesX = WorldGen.genRand.Next(0, x);
                int tilesY = WorldGen.genRand.Next((int)(y * 0.3f), (int)(y * 0.35f));

                if (Main.tile[tilesX, tilesY].type == (WorldGen.crimson ? TileID.Crimstone : TileID.Ebonstone))
                {
                    SpecialHut(WorldGen.crimson ? TileID.CrimtaneBrick : TileID.DemoniteBrick,
                        WorldGen.crimson ? TileID.Crimstone : TileID.Ebonstone,
                        WorldGen.crimson ? WallID.CrimtaneBrick : WallID.DemoniteBrick, UndergroundShrineType.WorldEvil, tilesX, tilesY);
                    break;
                }
            }

            for (int k = 0; k < (int)(x * y * shrineChance); k++) //Cavern Shrine
            {
                int tilesX = WorldGen.genRand.Next((int)(x * 0.3), generateBack);
                int tilesX2 = WorldGen.genRand.Next(generateForward, (int)(x * 0.7));
                int tilesY = WorldGen.genRand.Next((int)(y * 0.55f), (int)(y * 0.8f));

                if (Main.tile[tilesX, tilesY].type == TileID.Stone)
                {
                    SpecialHut(TileID.ObsidianBrick, TileID.Obsidian, WallID.ObsidianBrick, UndergroundShrineType.Cavern, tilesX, tilesY);
                    break;
                }
                if (Main.tile[tilesX2, tilesY].type == TileID.Stone)
                {
                    SpecialHut(TileID.ObsidianBrick, TileID.Obsidian, WallID.ObsidianBrick, UndergroundShrineType.Cavern, tilesX2, tilesY);
                    break;
                }
            }

            for (int k = 0; k < (int)(x * y * shrineChance); k++) //Ice Shrine
            {
                int tilesX = WorldGen.genRand.Next(0, x);
                int tilesY = WorldGen.genRand.Next((int)(y * 0.35f), (int)(y * 0.5f));

                if (Main.tile[tilesX, tilesY].type == TileID.IceBlock)
                {
                    SpecialHut(TileID.IceBrick, TileID.IceBlock, WallID.IceBrick, UndergroundShrineType.Ice, tilesX, tilesY);
                    break;
                }
            }

            for (int k = 0; k < (int)(x * y * shrineChance); k++) //Desert Shrine
            {
                int tilesX = WorldGen.genRand.Next(0, x);
                int tilesY = WorldGen.genRand.Next((int)(y * 0.3f), (int)(y * 0.5f));

                if (Main.tile[tilesX, tilesY].type == TileID.DesertFossil)
                {
                    SpecialHut(TileID.DesertFossil, TileID.Sandstone, WallID.DesertFossil, UndergroundShrineType.Desert, tilesX, tilesY);
                    break;
                }
            }

            for (int k = 0; k < (int)(x * y * shrineChance); k++) //Mushroom Shrine
            {
                int tilesX = WorldGen.genRand.Next(0, x);
                int tilesY = WorldGen.genRand.Next((int)(y * 0.35f), (int)(y * 0.5f));

                if (Main.tile[tilesX, tilesY].type == TileID.MushroomGrass)
                {
                    SpecialHut(TileID.MushroomBlock, TileID.Mud, WallID.MushroomUnsafe, UndergroundShrineType.Mushroom, tilesX, tilesY);
                    break;
                }
            }

            for (int k = 0; k < (int)(x * y * shrineChance); k++) //Granite Shrine
            {
                int tilesX = WorldGen.genRand.Next(0, x);
                int tilesY = WorldGen.genRand.Next((int)(y * 0.35f), (int)(y * 0.5f));

                if (Main.tile[tilesX, tilesY].type == TileID.Granite)
                {
                    SpecialHut(TileID.GraniteBlock, TileID.Granite, WallID.GraniteUnsafe, UndergroundShrineType.Granite, tilesX, tilesY);
                    break;
                }
            }

            for (int k = 0; k < (int)(x * y * shrineChance); k++) //Marble Shrine
            {
                int tilesX = WorldGen.genRand.Next(0, x);
                int tilesY = WorldGen.genRand.Next((int)(y * 0.35f), (int)(y * 0.5f));

                if (Main.tile[tilesX, tilesY].type == TileID.Marble)
                {
                    SpecialHut(TileID.MarbleBlock, TileID.Marble, WallID.MarbleUnsafe, UndergroundShrineType.Marble, tilesX, tilesY);
                    break;
                }
            }
        }
        #endregion
    }
}
