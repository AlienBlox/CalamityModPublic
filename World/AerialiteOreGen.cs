﻿using System;
using System.Collections.Generic;
using System.Linq;
using CalamityMod.DataStructures;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Generation;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.WorldBuilding;

namespace CalamityMod.World
{
    public class AerialiteOreGen
    {
        public static void Generate(bool Convert)
        {
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                for (int x = 5; x < Main.maxTilesX - 5; x++)
                {
                    for (int y = 5; y < Main.worldSurface; y++)
                    {
                        if (!Convert)
                        {
                            Tile tile = Main.tile[x, y];

                            if (WorldGen.genRand.NextBool(365)&& tile.TileType == TileID.Cloud && tile.HasTile)
                            {
                                ShapeData circle = new ShapeData();
                                ShapeData biggerCircle = new ShapeData();
                                GenAction blotchMod = new Modifiers.Blotches(2, 0.4);

                                int radius = (int)(WorldGen.genRand.Next(3, 5) * WorldGen.genRand.NextFloat(0.74f, 0.82f));

                                //big cloud circle
                                WorldUtils.Gen(new Point(x, y), new Shapes.Circle(radius + 1), Actions.Chain(new GenAction[]
                                {
                                    blotchMod.Output(biggerCircle)
                                }));

                                WorldUtils.Gen(new Point(x, y), new ModShapes.All(biggerCircle), Actions.Chain(new GenAction[]
                                {
                                    new Actions.ClearTile(),
                                    new Actions.PlaceTile((ushort)TileID.Cloud)
                                }));

                                //circle of ore
                                WorldUtils.Gen(new Point(x, y), new Shapes.Circle(radius), Actions.Chain(new GenAction[]
                                {
                                    blotchMod.Output(circle)
                                }));

                                WorldUtils.Gen(new Point(x, y), new ModShapes.All(circle), Actions.Chain(new GenAction[]
                                {
                                    new Actions.ClearTile(),
                                    new Actions.PlaceTile((ushort)ModContent.TileType<Tiles.Ores.AerialiteOreDisenchanted>())
                                }));
                            }
                        }

                        if (Convert)
                        {
                            if (Main.tile[x, y].TileType == ModContent.TileType<Tiles.Ores.AerialiteOreDisenchanted>())
                            {
                                WorldGen.SquareTileFrame(x, y, true);
                                Main.tile[x, y].TileType = (ushort)ModContent.TileType<Tiles.Ores.AerialiteOre>();
                            }
                        }
                    }
                }
            }
        }
    }
}
