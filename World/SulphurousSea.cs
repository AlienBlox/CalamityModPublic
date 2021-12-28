using CalamityMod.Items;
using CalamityMod.Items.Accessories;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.Schematics;
using CalamityMod.Tiles.Abyss;
using CalamityMod.Walls;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.World.Generation;

namespace CalamityMod.World
{
    public class SulphurousSea
    {
        #region Fields and Properties
        public static int BiomeWidth
        {
            get
            {
                // Small world
                if (Main.maxTilesX == 4200)
                {
                    return 370;
                }
                // Medium world
                else if (Main.maxTilesX == 6400)
                {
                    return 445;
                }
                // Large world
                else
                {
                    return 500;
                }
            }
        }

        public static int BlockDepth
        {
            get
            {
                // Small world
                if (Main.maxTilesX == 4200)
                {
                    return (int)((Main.rockLayer + 20 - YStart) * 0.8);
                }
                // Medium world
                else if (Main.maxTilesX == 6400)
                {
                    return (int)((Main.rockLayer + 20 - YStart) * 0.85);
                }
                // Large world
                else
                {
                    return (int)((Main.rockLayer + 20 - YStart) * 0.925);
                }
            }
        }

        public static int YStart = 0;
        public static readonly List<int> SulphSeaTiles = new List<int>()
        {
            ModContent.TileType<SulphurousSand>(),
            ModContent.TileType<SulphurousSandstone>(),
            ModContent.TileType<HardenedSulphurousSandstone>()
        };
        #endregion

        #region Placement Methods
        public static void PlaceSulphurSea()
        {
            CalamityWorld.abyssSide = Main.dungeonX < Main.maxTilesX / 2;
            DetermineYStart();
            CreateStartingBlock();
            CreateWater();
            GenerateHardenedSandstone();
            RemoveStupidTilesAboveSea();
            GenerateLake();
            SettleWater(); // The island Y spawn position calculations are relative to water. Settling the water before doing these calculations is ideal.
            GenerateIslands();
            GenerateVentsAndFossils();
            SmoothenTheEntireSea();
        }

        public static void FinishGeneratingSulphurSea()
        {
            PlaceStalactites();
            PlaceColumns();
            PlaceRustyChests();
            CreateBeachNearSea();
            PlaceScrapPiles();
        }
        #endregion

        #region Perlin Noise
        public static float NoiseFunction(int seed)
        {
            seed = (seed << 13) ^ seed;
            return 1.0f - ((seed * (seed * seed * 15731) + 1376312589) & 0x7fffffff) / 1073741824.0f;
        }
        public static float PerlinNoise2D(float x, float y, int octave, int seed)
        {
            float frequency = (float)Math.Pow(2, octave);
            float xFrequency = x * frequency;
            float yFrequency = y * frequency;
            int flooredXFrequency = (int)xFrequency;
            int flooredYFrequency = (int)yFrequency;
            float fractionXFrequency = xFrequency - flooredXFrequency;
            float fractionYFrequency = yFrequency - flooredYFrequency;

            float noise1 = NoiseFunction(flooredXFrequency + flooredYFrequency * 54 + seed);
            float noise2 = NoiseFunction(flooredXFrequency + 1 + flooredYFrequency * 54 + seed);
            float noise3 = NoiseFunction(flooredXFrequency + (1 + flooredYFrequency) * 54 + seed);
            float noise4 = NoiseFunction(flooredXFrequency + 1 + (1 + flooredYFrequency) * 54 + seed);

            float lerpStart = MathHelper.Lerp(noise1, noise2, fractionXFrequency);
            float lerpEnd = MathHelper.Lerp(noise3, noise4, fractionXFrequency);
            return MathHelper.Lerp(lerpStart, lerpEnd, fractionYFrequency);
        }
        #endregion

        #region Generating Initial Block
        public static void CreateStartingBlock()
        {
            // Set above for general performance.
            ushort sandTileType = (ushort)ModContent.TileType<SulphurousSand>();
            ushort sandstoneTileType = (ushort)ModContent.TileType<SulphurousSandstone>();

            ushort sandWallType = (ushort)ModContent.WallType<SulphurousSandWall>();
            ushort sandstoneWallType = (ushort)ModContent.WallType<SulphurousSandstoneWall>();


            float randomValue1 = WorldGen.genRand.NextFloat(-0.4f, 0.4f);
            while (Math.Abs(randomValue1) < 0.22f)
            {
                randomValue1 = WorldGen.genRand.NextFloat(-0.4f, 0.4f);
            }
            float randomValue2 = WorldGen.genRand.NextFloat(-0.2f, 0.2f);
            while (Math.Abs(randomValue2) < 0.07f)
            {
                randomValue2 = WorldGen.genRand.NextFloat(-0.2f, 0.2f);
            }

            float randomValue3 = WorldGen.genRand.NextFloat(0.42f, 0.96f) * WorldGen.genRand.NextBool(2).ToDirectionInt();
            float wallBoundAtPosition(float xRatio) => (float)(Math.Sin(randomValue3 * MathHelper.Pi * xRatio) + Math.Cos(randomValue3 * Math.E / 2f * xRatio)) * 0.5f * 28f;

            for (int x = 1; x < BiomeWidth; x++)
            {
                float xRatio = x / (float)BiomeWidth;
                for (int y = YStart; y <= YStart + BlockDepth; y++)
                {
                    int trueX = CalamityWorld.abyssSide ? x : Main.maxTilesX - x;
                    float yRatio = (y - YStart) / (float)BlockDepth;
                    float xAngleWrap = x / (float)BiomeWidth * MathHelper.TwoPi - MathHelper.Pi;
                    bool generateSand = yRatio < SandstoneYMinimum(xAngleWrap, randomValue1, randomValue2);
                    if (WorldGen.InWorld(trueX, y))
                    {
                        if (y - YStart < BlockDepth - (int)(BlockDepth * 0.35f) + (int)(Math.Sin(xRatio * MathHelper.Pi) * (int)(BlockDepth * 0.35f)))
                        {
                            Main.tile[trueX, y] = new Tile()
                            {
                                type = generateSand ? sandTileType : sandstoneTileType,
                                wall = generateSand ? sandWallType : sandstoneWallType
                            };
                            if (y - YStart - 6 < wallBoundAtPosition(xRatio * MathHelper.TwoPi * 4f))
                                Main.tile[trueX, y].wall = WallID.None;

                            Main.tile[trueX, y].active(true);
                        }
                    }
                }
            }
        }
        public static float SandstoneYMinimum(float xAsAngle, float randomValue1, float randomValue2)
        {
            float sineSquaredRandom1 = randomValue1 * (float)Math.Cos(xAsAngle * randomValue1) * (float)Math.Cos(xAsAngle * randomValue1);
            float sineSquaredRandom2 = randomValue1 * (float)Math.Sin(xAsAngle * randomValue1) * (float)Math.Sin(xAsAngle * randomValue1);
            float sineSquaredRandom3 = randomValue2 * (float)Math.Sin(xAsAngle * randomValue2);
            float sineSquaredRandom4 = (float)Math.Sin(randomValue1 / randomValue2 * xAsAngle) * (float)Math.Cos(randomValue1 * MathHelper.Pi);
            return 0.25f * Math.Abs(sineSquaredRandom1 + 0.5f * sineSquaredRandom2 + sineSquaredRandom3 - sineSquaredRandom4) + 0.25f;
        }
        #endregion

        #region Generating Water
        public const int PerlinIterations = 1;
        public const int PerlinOctaves = 4;
        public const int PerlinXEdgeClamp = 20;
        public const float PerlinYDelta = 40;
        public const float PerlinNoiseMax = 0.05f;
        public const float PerlinThreshold = 0.1f; // Be careful with this number

        public static void GenerateLake()
        {
            int lakeDepthMax = WorldGen.genRand.Next(36, 52 + 1);
            float lakeSteepness = WorldGen.genRand.NextFloat(1.2f, 1.5f);
            for (int x = 1; x < BiomeWidth; x++)
            {
                int trueX = CalamityWorld.abyssSide ? x : Main.maxTilesX - x;
                float xRatio = x / (float)BiomeWidth;
                int lakeDepth = (int)(Math.Sin(MathHelper.Min((1f - xRatio) * MathHelper.PiOver2 * lakeSteepness * 1.2f, MathHelper.PiOver2)) * lakeDepthMax);
                for (int y = YStart; y <= YStart + lakeDepth; y++)
                {
                    ushort oldWall = Main.tile[trueX, y].wall;
                    Main.tile[trueX, y] = new Tile()
                    {
                        wall = oldWall,
                        liquid = 255
                    };
                }
            }
        }

        public static void CreateWater()
        {
            int[] seeds = new int[PerlinIterations];
            for (int i = 0; i < seeds.Length; i++)
            {
                seeds[i] = WorldGen.genRand.Next();
            }
            for (int x = 1; x < BiomeWidth; x++)
            {
                float xRatio = x / (float)BiomeWidth;
                for (int y = YStart; y <= YStart + BlockDepth; y++)
                {
                    float yRatio = (y - YStart) / (float)BlockDepth;
                    int trueX = CalamityWorld.abyssSide ? x : Main.maxTilesX - x;
                    if (y - YStart < BlockDepth - (int)(BlockDepth * 0.35f) + (int)(Math.Sin(xRatio * MathHelper.Pi) * (int)(BlockDepth * 0.35f)) - PerlinYDelta)
                    {
                        if (x < PerlinXEdgeClamp || x > BiomeWidth - PerlinXEdgeClamp)
                            continue;
                        float perlinAverage = 0f;
                        for (int k = 0; k < seeds.Length; k++)
                        {
                            perlinAverage += PerlinNoise2D(xRatio, yRatio, PerlinOctaves, seeds[k]) / PerlinIterations;
                        }
                        if (perlinAverage > PerlinThreshold * WorldGen.genRand.NextFloat(1f - PerlinNoiseMax, 1f + PerlinNoiseMax))
                        {
                            ushort oldWall = Main.tile[trueX, y].wall;
                            Main.tile[trueX, y] = new Tile()
                            {
                                wall = oldWall,
                                liquid = 255
                            };
                        }
                    }
                }
            }
        }
        #endregion

        #region Generating Edge Sandstone
        public const int EdgeCheckMinX = 6;
        public const int EdgeCheckMaxX = 8;
        public const int EdgeCheckMinY = 4;
        public const int EdgeCheckMaxY = 6;
        public const int MinimumEdgeScore = 4;
        public const int SpotWidth = 4;
        public static void GenerateHardenedSandstone()
        {
            for (int x = 1; x < BiomeWidth; x++)
            {
                for (int y = YStart; y <= YStart + BlockDepth; y++)
                {
                    int trueX = CalamityWorld.abyssSide ? x : Main.maxTilesX - x;
                    int edgeScore = GetEdgeScore(trueX, y);
                    
                    if (edgeScore >= MinimumEdgeScore)
                    {
                        for (int dx = -SpotWidth / 2; dx <= SpotWidth; dx++)
                        {
                            for (int dy = -SpotWidth / 2; dy <= SpotWidth / 2; dy++)
                            {
                                if (WorldGen.InWorld(trueX + dx, y + dy))
                                {
                                    if (CalamityUtils.ParanoidTileRetrieval(trueX + dx, y + dy).type != (ushort)ModContent.TileType<HardenedSulphurousSandstone>() &&
                                        SulphSeaTiles.Contains(CalamityUtils.ParanoidTileRetrieval(trueX + dx, y + dy).type))
                                    {
                                        Main.tile[trueX + dx, y + dy].type = (ushort)ModContent.TileType<HardenedSulphurousSandstone>();
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        public static int GetEdgeScore(int x, int y)
        {
            int edgeScore = 0;
            for (int dx = x - WorldGen.genRand.Next(EdgeCheckMinX, EdgeCheckMaxX + 1); dx <= x + WorldGen.genRand.Next(EdgeCheckMinX, EdgeCheckMaxX + 1); dx++)
            {
                if (dx == x)
                    continue;
                if (CalamityUtils.ParanoidTileRetrieval(dx, y).liquid == 255)
                    edgeScore++;
            }
            for (int dy = y - WorldGen.genRand.Next(EdgeCheckMinY, EdgeCheckMaxY + 1); dy <= y + WorldGen.genRand.Next(EdgeCheckMinY, EdgeCheckMaxY + 1); dy++)
            {
                if (dy == y)
                    continue;
                if (CalamityUtils.ParanoidTileRetrieval(x, dy).liquid == 255)
                    edgeScore++;
            }
            return edgeScore;
        }
        #endregion

        #region Generating Islands
        public const int IslandCount = 4;
        public const int IslandXPadding = 20;

        public const int IslandMinWidth = 15;
        public const int IslandMaxWidth = 22;

        public const int IslandMinDepth = 5;
        public const int IslandMaxDepth = 10;
        public static void SettleWater()
        {
            Liquid.QuickWater(3, -1, -1);
            WorldGen.WaterCheck();
            int counter = 0;
            Liquid.quickSettle = true;
            while (counter < 10)
            {
                counter++;
                while (Liquid.numLiquid > 0)
                {
                    Liquid.UpdateLiquid();
                }
                WorldGen.WaterCheck();
            }
            Liquid.quickSettle = false;

            // Remove liquid above the biome line, to prevent flooding.
            for (int x = 2; x < BiomeWidth - 2; x++)
            {
                int trueX = x;
                if (!CalamityWorld.abyssSide)
                    trueX = Main.maxTilesX - x;
                for (int y = YStart - 300; y <= YStart; y++)
                {
                    if (CalamityUtils.ParanoidTileRetrieval(trueX, y).liquid > 0)
                        Main.tile[trueX, y].liquid = 0;
                }
            }
        }
        public static void GenerateIslands()
        {
            for (int i = 0; i < IslandCount; i++)
            {
                int y = YStart - 240;
                int x = WorldGen.genRand.Next(IslandMaxWidth + IslandXPadding, BiomeWidth - IslandMaxWidth - IslandXPadding);
                if (!CalamityWorld.abyssSide)
                    x = Main.maxTilesX - x;
                while (CalamityUtils.ParanoidTileRetrieval(x, y).liquid == 0)
                {
                    y++;
                    if (y > Main.rockLayer - 35)
                    {
                        break;
                    }
                }
                if (y > Main.rockLayer - 35)
                {
                    i--;
                    continue; // Try again
                }
                int height = 2 * WorldGen.genRand.Next(IslandMinDepth, IslandMaxDepth + 1);
                int width = WorldGen.genRand.Next(IslandMinWidth, IslandMaxWidth + 1);
                int treePosition = WorldGen.genRand.Next(-width + 4, width - 4);

                for (float theta = 0f; theta <= MathHelper.TwoPi; theta += 0.05f)
                {
                    for (int dx = 0; dx != (int)(height * Math.Cos(theta)); dx += (height * Math.Cos(theta) > 0).ToDirectionInt())
                    {
                        for (int dy = 0; dy != (int)(width * Math.Sin(theta)); dy += (width * Math.Sin(theta) > 0).ToDirectionInt())
                        {
                            if (WorldGen.InWorld(x + dx, y + dy))
                            {
                                if (dy >= -2 - WorldGen.genRand.Next(-2, 3))
                                {
                                    int extraHeight = WorldGen.genRand.Next(0, 1 + 1);
                                    for (int y2 = 0; y2 <= extraHeight; y2++)
                                    {
                                        Main.tile[x + dx, y + dy - y2].type = (ushort)ModContent.TileType<SulphurousSandNoWater>();
                                        Main.tile[x + dx, y + dy - y2].slope(0);
                                        Main.tile[x + dx, y + dy - y2].halfBrick(false);
                                        Main.tile[x + dx, y + dy - y2].active(true);
                                    }
                                    if (dy == -2 && dx == treePosition)
                                    {
                                        WorldGen.PlaceTile(x + dx, y + dy - extraHeight - 1, ModContent.TileType<AcidWoodTreeSapling>());
                                        GrowSaplingImmediately(x + dx, y + dy - extraHeight - 1);
                                    }
                                }
                                else
                                {
                                    Main.tile[x + dx, y + dy].liquid = 0;
                                }
                            }
                        }
                    }
                }
            }
        }
        #endregion

        #region Smoothening Everything
        public static void SmoothenTheEntireSea()
        {
            for (int x = 2; x < BiomeWidth - 2; x++)
            {
                int trueX = x;
                if (!CalamityWorld.abyssSide)
                    trueX = Main.maxTilesX - x;
                for (int y = YStart; y <= YStart + BlockDepth; y++)
                {
                    Tile.SmoothSlope(trueX, y);
                }
            }
        }
        #endregion

        #region Generating Scenery
        public const int ColumnMinDistanceX = 8;
        public const int ColumnMinHeight = 10;
        public const int ColumnMaxHeight = 27;
        public const int StalactitePairMinDistance = 6;
        public const int StalactitePairMaxDistance = 44;
        public static void GenerateVentsAndFossils()
        {
            for (int x = 1; x < BiomeWidth; x++)
            {
                int trueX = CalamityWorld.abyssSide ? x : Main.maxTilesX - x;
                for (int y = YStart; y <= YStart + BlockDepth; y++)
                {
                    // Generate vents in the top 45%
                    if (y - YStart < BlockDepth * 0.45f)
                    {
                        if (WorldGen.SolidTile(trueX, y + 1) && WorldGen.genRand.NextBool(20))
                        {
                            PlaceMiscTile(trueX, y, 1, 2, (ushort)ModContent.TileType<SteamGeyser>());
                        }
                    }
                    // And fossils in the bottom 55%
                    else
                    {
                        if (WorldGen.SolidTile(trueX, y + 1))
                        {
                            // Generate ribs such that they're all nearby
                            if (WorldGen.genRand.NextBool(25))
                            {
                                ushort type = (ushort)Utils.SelectRandom(WorldGen.genRand,
                                    ModContent.TileType<SulphurousRib1>(),
                                    ModContent.TileType<SulphurousRib2>(),
                                    ModContent.TileType<SulphurousRib3>(),
                                    ModContent.TileType<SulphurousRib4>(),
                                    ModContent.TileType<SulphurousRib5>());
                                int height;
                                if (type == ModContent.TileType<SulphurousRib1>() || type == ModContent.TileType<SulphurousRib4>())
                                {
                                    height = 3;
                                }
                                else if (type == ModContent.TileType<SulphurousRib3>())
                                {
                                    height = 2;
                                }
                                else if (type == ModContent.TileType<SulphurousRib1>())
                                {
                                    height = 4;
                                }
                                else
                                {
                                    height = 1;
                                }
                                PlaceMiscTile(trueX, y, 1, height, type);
                            }
                            else if (WorldGen.genRand.NextBool(16))
                            {
                                PlaceMiscTile(trueX, y, 3, 2, (ushort)Utils.SelectRandom(WorldGen.genRand, ModContent.TileType<SulphuricFossil1>(), ModContent.TileType<SulphuricFossil2>(), ModContent.TileType<SulphuricFossil3>()));
                            }
                        }
                    }
                }
            }
        }
        public static void PlaceColumns()
        {
            int previousX = 0;
            int style;
            for (int x = 1; x < BiomeWidth; x++)
            {
                int trueX = CalamityWorld.abyssSide ? x : Main.maxTilesX - x;
                for (int y = YStart; y <= YStart + BlockDepth; y++)
                {
                    if (y - YStart > BlockDepth * 0.5f)
                    {
                        if (WorldGen.SolidTile(trueX, y + 1) &&
                            WorldGen.SolidTile(trueX + 1, y + 1) && 
                            SulphSeaTiles.Contains(CalamityUtils.ParanoidTileRetrieval(trueX, y + 1).type) &&
                            WorldGen.genRand.NextBool(3) &&
                            Math.Abs(trueX - previousX) >= ColumnMinDistanceX)
                        {
                            int top = y;
                            int height = 0;
                            while (!(WorldGen.SolidTile(trueX - 1, top) && WorldGen.SolidTile(trueX, top) && WorldGen.SolidTile(trueX + 1, top)))
                            {
                                top--;
                                height++;
                                if (height > ColumnMaxHeight + 1)
                                    break;
                            }
                            if (height <= ColumnMaxHeight && height >= ColumnMinHeight)
                            {
                                for (int dy = 0; dy < height; dy++)
                                {
                                    style = WorldGen.genRand.Next(3);
                                    short yFrame = 18;
                                    if (dy == 0)
                                        yFrame = 36;
                                    if (dy == height - 1)
                                        yFrame = 0;
                                    if (Main.tile[trueX, y - dy] is null)
                                        Main.tile[trueX, y - dy] = new Tile();
                                    ushort oldWall = Main.tile[trueX, y - dy].wall;
                                    Main.tile[trueX, y - dy] = new Tile
                                    {
                                        type = (ushort)ModContent.TileType<SulphurousColumn>(),
                                        frameX = (short)(style * 36),
                                        frameY = yFrame,
                                        liquid = 255,
                                        wall = oldWall
                                    };
                                    Main.tile[trueX, y - dy].active(true);

                                    if (Main.tile[trueX + 1, y - dy] is null)
                                        Main.tile[trueX + 1, y - dy] = new Tile();

                                    oldWall = Main.tile[trueX + 1, y - dy].wall;
                                    Main.tile[trueX + 1, y - dy] = new Tile
                                    {
                                        type = (ushort)ModContent.TileType<SulphurousColumn>(),
                                        frameX = (short)(style * 36 + 18),
                                        frameY = yFrame,
                                        liquid = 255,
                                        wall = oldWall
                                    };
                                    Main.tile[trueX + 1, y - dy].active(true);
                                }
                                previousX = trueX;
                            }
                        }
                    }
                }
            }
        }
        public static void PlaceStalactites()
        {
            int heightFromType(int type)
            {
                if (type <= 2)
                    return 2;
                else if (type <= 4)
                    return 3;
                else
                    return 4;
            };
            for (int x = 1; x < BiomeWidth; x++)
            {
                int trueX = CalamityWorld.abyssSide ? x : Main.maxTilesX - x;
                for (int y = YStart; y <= YStart + BlockDepth; y++)
                {
                    if (y - YStart > BlockDepth * 0.25f)
                    {
                        if (WorldGen.SolidTile(trueX, y - 1) && WorldGen.genRand.NextBool(10))
                        {
                            int dy = 1;
                            while (!CalamityUtils.ParanoidTileRetrieval(trueX, y + dy).active())
                            {
                                dy++;
                                if (dy > StalactitePairMaxDistance)
                                    break;
                            }
                            if (dy <= StalactitePairMaxDistance && dy >= StalactitePairMinDistance)
                            {
                                int type = WorldGen.genRand.Next(6);
                                type++;
                                int height = heightFromType(type);

                                PlaceStalactite(trueX, y, height, (ushort)CalamityMod.Instance.TileType($"SulphurousStalactite{type}"));
                                if (WorldGen.SolidTile(trueX, y + dy + 1))
                                    PlaceStalacmite(trueX, y + dy, height, (ushort)CalamityMod.Instance.TileType($"SulphurousStalacmite{type}"));
                            }
                        }
                    }
                }
            }
        }
        public static void PlaceMiscTile(int x, int y, int width, int height, ushort type, int style = 0)
        {
            try
            {
                bool canGenerate = true;
                for (int i = x; i < x + width; i++)
                {
                    for (int j = y - (int)Math.Ceiling(height / 2.0); j < y + (int)Math.Ceiling(height / 2.0); j++)
                    {
                        if (Main.tile[i, j] is null)
                        {
                            Main.tile[i, j] = new Tile();
                        }
                        if (Main.tile[i, j].active())
                        {
                            canGenerate = false;
                        }
                    }
                    if (Main.tile[i, y + 1] is null)
                    {
                        Main.tile[i, y + 1] = new Tile();
                    }
                    if (!Main.tile[i, y + 1].nactive() || Main.tile[i, y + 1].halfBrick() || Main.tile[i, y + 1].slope() != 0 || !Main.tileSolid[Main.tile[i, y + 1].type])
                    {
                        canGenerate = false;
                    }
                }
                if (canGenerate)
                {
                    WorldGen.PlaceTile(x, y, type, style: style);
                }
            }
            catch
            {
                return; // This should only ever happen from an index error, and there's really no point in continuing if that happens
            }
        }
        public static void PlaceStalactite(int x, int y, int height, ushort type)
        {
            for (int dy = 0; dy < height; dy++)
            {
                ushort oldWall = Main.tile[x, y + dy].wall;
                Main.tile[x, y + dy] = new Tile
                {
                    wall = oldWall,
                    type = type,
                    frameY = (short)(dy * 18)
                };
                Main.tile[x, y + dy].active(true);
            }
        }
        public static void PlaceStalacmite(int x, int y, int height, ushort type)
        {
            for (int dy = height - 1; dy > 0; dy--)
            {
                ushort oldWall = Main.tile[x, y + dy].wall;
                Main.tile[x, y - dy] = new Tile
                {
                    type = type,
                    frameY = (short)(height * 18 - dy * 18),
                    wall = oldWall
                };
                Main.tile[x, y - dy].active(true);
            }
        }
        #endregion

        #region Generating Chests
        public const int CheckCutoffDistance = 15;
        public static void PlaceRustyChests()
        {
            // Ambiguity bullshit.
            int[] ItemTypes = { ModContent.ItemType<Items.Placeables.Furniture.EffigyOfDecay>(),
                                ModContent.ItemType<BrokenWaterFilter>(),
                                ModContent.ItemType<RustyBeaconPrototype>(),
                                ModContent.ItemType<RustyMedallion>() };

            for (int i = 0; i < ItemTypes.Length; ++i)
            {
                Chest chest = null;
                int attempts = 0;

                while (chest is null)
                {
                    attempts++;
                    int x = WorldGen.genRand.Next(CheckCutoffDistance, BiomeWidth - CheckCutoffDistance);
                    if (!CalamityWorld.abyssSide)
                    {
                        x = Main.maxTilesX - x;
                    }
                    int y = WorldGen.genRand.Next(YStart + CheckCutoffDistance, YStart + BlockDepth - CheckCutoffDistance);
                    if (WorldGen.InWorld(x, y))
                    {
                        if (!CalamityUtils.ParanoidTileRetrieval(x, y).active() &&
                            SulphSeaTiles.Contains(CalamityUtils.ParanoidTileRetrieval(x, y + 1).type) &&
                            CalamityUtils.ParanoidTileRetrieval(x, y + 1).active())
                        {
                            chest = MiscWorldgenRoutines.AddChestWithLoot(x, y, (ushort)ModContent.TileType<RustyChestTile>());
                        }
                    }
                }

                // If a chest was placed, force its first item to be the unique item.
                if (chest != null)
                {
                    chest.item[0].SetDefaults(ItemTypes[i]);
                    chest.item[0].Prefix(-1);
                }
            }
        }
        #endregion

        #region Removal of stupid Tiles above the Sea

        public static List<int> OtherTilesForSulphSeaToDestroy = new List<int>()
        {
            TileID.PalmTree,
            TileID.Sunflower,
            TileID.CorruptThorns,
            TileID.CrimtaneThorns,
            TileID.CorruptGrass,
            TileID.CorruptPlants,
            TileID.Stalactite,
            TileID.ImmatureHerbs,
            TileID.MatureHerbs,
            TileID.Pots,
            TileID.Pumpkins, // Happens during Halloween
        };

        public static List<int> WallsForSulphSeaToDestroy = new List<int>()
        {
            WallID.Dirt,
            WallID.DirtUnsafe,
            WallID.DirtUnsafe1,
            WallID.DirtUnsafe2,
            WallID.DirtUnsafe3,
            WallID.DirtUnsafe4,
            WallID.Cave6Unsafe, // Rocky dirt wall
            WallID.Grass,
            WallID.GrassUnsafe,
            WallID.Flower,
            WallID.FlowerUnsafe,
            WallID.CorruptGrassUnsafe,
            WallID.EbonstoneUnsafe,
            WallID.CrimstoneUnsafe,
        };
        public static void RemoveStupidTilesAboveSea()
        {
            for (int x = 0; x < BiomeWidth; x++)
            {
                int trueX = CalamityWorld.abyssSide ? x : Main.maxTilesX - x;
                for (int y = YStart - 140; y < YStart + 80; y++)
                {
                    int type = CalamityUtils.ParanoidTileRetrieval(trueX, y).type;
                    if (YStartWhitelist.Contains(type) ||
                        OtherTilesForSulphSeaToDestroy.Contains(type))
                    {
                        if (Main.tile[trueX, y] is null)
                            Main.tile[trueX, y] = new Tile();
                        Main.tile[trueX, y].active(false);
                    }
                    if (WallsForSulphSeaToDestroy.Contains(Main.tile[trueX, y].wall))
                    {
                        Main.tile[trueX, y].wall = 0;
                    }
                }
            }
        }
        #endregion

        #region Nearby Beach
        public static readonly List<int> ValidBeachCovertTiles = new List<int>()
        {
            TileID.Dirt,
            TileID.Stone,
            TileID.Crimstone,
            TileID.Ebonstone,
            TileID.Sand,
            TileID.Ebonsand,
            TileID.Crimsand,
            TileID.Grass,
            TileID.CorruptGrass,
            TileID.FleshGrass,
            TileID.ClayBlock,
            TileID.Mud,
        };
        public static readonly List<int> ValidBeachDestroyTiles = new List<int>()
        {
            TileID.Coral,
            TileID.BeachPiles,
            TileID.Plants,
            TileID.Plants2,
            TileID.SmallPiles,
            TileID.LargePiles,
            TileID.LargePiles2,
            TileID.CorruptThorns,
            TileID.CrimtaneThorns,
            TileID.DyePlants,
            TileID.Trees,
            TileID.Sunflower,
        };
        public static void CreateBeachNearSea()
        {
            int beachWidth = WorldGen.genRand.Next(150, 190 + 1);

            var searchCondition = Searches.Chain(new Searches.Down(3000), new Conditions.IsSolid());
            WorldUtils.Find(new Point(BiomeWidth + 4, (int)WorldGen.worldSurfaceLow - 20), searchCondition, out Point determinedPoint);
            Tile tileAtEdge = CalamityUtils.ParanoidTileRetrieval(determinedPoint.X, determinedPoint.Y);

            // Extend outward to encompass some of the desert, if there is one.
            if (tileAtEdge.type == TileID.Sand ||
                tileAtEdge.type == TileID.Ebonsand ||
                tileAtEdge.type == TileID.Crimsand)
            {
                beachWidth += 85;
            }

            for (int x = BiomeWidth - 10; x <= BiomeWidth + beachWidth; x++)
            {
                float xRatio = Utils.InverseLerp(BiomeWidth - 10, BiomeWidth + beachWidth, x, true);
                int trueX = CalamityWorld.abyssSide ? x : Main.maxTilesX - x;
                int depth = (int)(Math.Sin((1f - xRatio) * MathHelper.PiOver2) * 45 + 1);
                for (int y = YStart - 40; y < YStart + depth; y++)
                {
                    Tile tileAtPosition = CalamityUtils.ParanoidTileRetrieval(trueX, y);
                    if (tileAtPosition.active() && ValidBeachDestroyTiles.Contains(tileAtPosition.type))
                    {
                        // Kill trees manually so that no leftover tiles are present.
                        if (Main.tile[trueX, y].type == TileID.Trees)
                            WorldGen.KillTile(trueX, y);
                        else
                            Main.tile[trueX, y].active(false);
                    }

                    else if (tileAtPosition.active() && ValidBeachCovertTiles.Contains(tileAtPosition.type))
                    {
                        Main.tile[trueX, y].type = (ushort)ModContent.TileType<SulphurousSand>();
                    }
                }
            }

            // Plant new trees.
            for (int x = BiomeWidth - 10; x <= BiomeWidth + beachWidth; x++)
            {
                int trueX = CalamityWorld.abyssSide ? x : Main.maxTilesX - x;
                if (!WorldGen.genRand.NextBool(10))
                    continue;

                int y = YStart - 30;
                if (!WorldUtils.Find(new Point(trueX, y), Searches.Chain(new Searches.Down(100), new Conditions.IsTile((ushort)ModContent.TileType<SulphurousSand>())), out Point treePlantPosition))
                    continue;

                WorldGen.PlaceTile(treePlantPosition.X, treePlantPosition.Y - 1, ModContent.TileType<AcidWoodTreeSapling>());
                GrowSaplingImmediately(treePlantPosition.X, treePlantPosition.Y - 1);
            }
        }
        #endregion

        #region Scrap Piles
        public static void PlaceScrapPiles()
        {
            int tries = 0;
            List<Vector2> pastPlacementPostiion = new List<Vector2>();
            for (int i = 0; i < 3; i++)
            {
                tries++;
                if (tries > 20000)
                    continue;

                int x = WorldGen.genRand.Next(75, BiomeWidth - 85);
                if (!CalamityWorld.abyssSide)
                    x = Main.maxTilesX - x;
                int y = WorldGen.genRand.Next(YStart + (int)(BlockDepth * 0.3f), YStart + (int)(BlockDepth * 0.8f));

                Point pilePlacementPosition = new Point(x, y);

                // If the selected position is sitting inside of a tile, try again.
                if (WorldGen.SolidTile(pilePlacementPosition.X, pilePlacementPosition.Y))
                {
                    i--;
                    continue;
                }

                // If the selected position is close to other piles, try again.
                if (pastPlacementPostiion.Any(p => Vector2.Distance(p, pilePlacementPosition.ToVector2()) < 85f))
                {
                    i--;
                    continue;
                }

                // Otherwise, decide which pile should be created.
                bool createLargePile = WorldGen.genRand.NextBool(3);
                int pileVariant = WorldGen.genRand.Next(4);
                string schematicName = $"{(createLargePile ? "Large " : string.Empty)}Sulphurous Scrap {pileVariant + 1}";
                Vector2? wrappedSchematicArea = SchematicManager.GetSchematicArea(schematicName);

                // Create a log message if for some reason the schematic in question doesn't exist.
                if (!wrappedSchematicArea.HasValue)
                {
                    CalamityMod.Instance.Logger.Warn($"Tried to place a schematic with name \"{schematicName}\". No matching schematic file found.");
                    continue;
                }

                Vector2 schematicArea = wrappedSchematicArea.Value;

                // Decide the placement position by searching downward and looking for the lowest point.
                // If the position is quite steep, try again.
                Vector2 left = pilePlacementPosition.ToVector2() - Vector2.UnitX * schematicArea.X * 0.5f;
                Vector2 right = pilePlacementPosition.ToVector2() + Vector2.UnitX * schematicArea.X * 0.5f;
                while (!WorldGen.SolidTile(CalamityUtils.ParanoidTileRetrieval((int)left.X, (int)left.Y)))
                    left.Y++;
                while (!WorldGen.SolidTile(CalamityUtils.ParanoidTileRetrieval((int)right.X, (int)right.Y)))
                    right.Y++;

                if (Math.Abs(left.Y - right.Y) >= 20f)
                {
                    i--;
                    continue;
                }

                // If the placement position ended up in the abyss, try again.
                if (left.Y >= YStart + BlockDepth + 5 || right.Y >= YStart + BlockDepth + 5)
                {
                    i--;
                    continue;
                }

                // Pick the lowest point vertically.
                Point bottomCenter = new Point(pilePlacementPosition.X, (int)Math.Max(left.Y, right.Y) + 6);
                bool _ = false;
                SchematicManager.PlaceSchematic<Action<Chest>>(schematicName, bottomCenter, SchematicAnchor.BottomCenter, ref _);

                pastPlacementPostiion.Add(bottomCenter.ToVector2());
                tries = 0;
            }
        }
        #endregion Scrap Piles

        #region Misc Functions
        public static List<int> YStartWhitelist = new List<int>()
        {
            TileID.Stone,
            TileID.Dirt,
            TileID.Sand,
            TileID.Ebonsand,
            TileID.Crimsand,
            TileID.Grass,
            TileID.CorruptGrass,
            TileID.FleshGrass,
            TileID.ClayBlock,
            TileID.Mud,
            TileID.Copper,
            TileID.Tin,
            TileID.Iron,
            TileID.Lead,
            TileID.Silver,
            TileID.Tungsten,
            TileID.Crimstone,
            TileID.Ebonstone,
            TileID.HardenedSand,
            TileID.CorruptHardenedSand,
            TileID.CrimsonHardenedSand,
            TileID.Coral,
            TileID.BeachPiles,
            TileID.Plants,
            TileID.Plants2,
            TileID.SmallPiles,
            TileID.LargePiles,
            TileID.LargePiles2,
            TileID.Trees,
            TileID.Vines,
            TileID.CorruptThorns,
            TileID.CrimtaneThorns,
            TileID.CrimsonVines,
            TileID.Containers,
            TileID.DyePlants,
            TileID.JungleGrass // Yes, this can happen on rare occasion
        };
        public static void DetermineYStart()
        {
            int xCheckPosition = CalamityWorld.abyssSide ? BiomeWidth + 1 : Main.maxTilesX - BiomeWidth - 1;
            var searchCondition = Searches.Chain(new Searches.Down(3000), new Conditions.IsSolid());
            Point determinedPoint;

            do
            {
                WorldUtils.Find(new Point(xCheckPosition, (int)WorldGen.worldSurfaceLow - 20), searchCondition, out determinedPoint);
                xCheckPosition += CalamityWorld.abyssSide.ToDirectionInt();
            }
            while (CalamityUtils.ParanoidTileRetrieval(determinedPoint.X, determinedPoint.Y).type == TileID.Ebonstone);
            YStart = determinedPoint.Y;
        }
        public static void GrowSaplingImmediately(int i, int j)
        {
            int trueStartingPositionY = j;
            while (TileLoader.IsSapling(Main.tile[i, trueStartingPositionY].type))
            {
                trueStartingPositionY++;
            }
            Tile tileAtPosition = Main.tile[i, trueStartingPositionY];
            Tile tileAbovePosition = Main.tile[i, trueStartingPositionY - 1];
            if (!tileAtPosition.active() || tileAtPosition.halfBrick() || tileAtPosition.slope() != 0)
            {
                return;
            }
            if (tileAbovePosition.wall != 0)
            {
                return;
            }
            if (!WorldGen.EmptyTileCheck(i - 1, i + 1, trueStartingPositionY - 30, trueStartingPositionY - 1, 20))
            {
                return;
            }
            int treeHeight = WorldGen.genRand.Next(10, 21);
            int frameYIdeal = WorldGen.genRand.Next(-8, 9);
            frameYIdeal *= 2;
            short frameY = 0;
            for (int k = 0; k < treeHeight; k++)
            {
                tileAtPosition = Main.tile[i, trueStartingPositionY - 1 - k];
                if (k == 0)
                {
                    tileAtPosition.active(true);
                    tileAtPosition.type = TileID.PalmTree;
                    tileAtPosition.frameX = 66;
                    tileAtPosition.frameY = 0;
                }
                else if (k == treeHeight - 1)
                {
                    tileAtPosition.active(true);
                    tileAtPosition.type = TileID.PalmTree;
                    tileAtPosition.frameX = (short)(22 * WorldGen.genRand.Next(4, 7));
                    tileAtPosition.frameY = frameY;
                }
                else
                {
                    if (frameY != frameYIdeal)
                    {
                        float heightRatio = k / (float)treeHeight;
                        bool increaseFrameY = heightRatio >= 0.25f && ((heightRatio < 0.5f && WorldGen.genRand.Next(13) == 0) || (heightRatio < 0.7f && WorldGen.genRand.Next(9) == 0) || heightRatio >= 0.95f || WorldGen.genRand.Next(5) != 0 || true);
                        if (increaseFrameY)
                        {
                            frameY += (short)(Math.Sign(frameYIdeal) * 2);
                        }
                    }
                    tileAtPosition.active(true);
                    tileAtPosition.type = TileID.PalmTree;
                    tileAtPosition.frameX = (short)(22 * WorldGen.genRand.Next(0, 3));
                    tileAtPosition.frameY = frameY;
                }
            }
            WorldGen.RangeFrame(i - 2, trueStartingPositionY - treeHeight - 1, i + 2, trueStartingPositionY + 1);
            if (Main.netMode == NetmodeID.Server)
            {
                NetMessage.SendTileSquare(-1, i, (int)(trueStartingPositionY - treeHeight * 0.5), treeHeight + 1, TileChangeType.None);
            }
        }
        #endregion
    }
}
