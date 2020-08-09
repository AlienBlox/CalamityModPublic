using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.World.Generation;

namespace CalamityMod.World
{
    public class CustomTemple
    {
		public static void NewJungleTemple()
		{
			bool success = false;
			while (!success)
			{
				int x;
				if (WorldGen.dungeonX < Main.maxTilesX / 2)
					x = WorldGen.genRand.Next((int)(Main.maxTilesX * 0.6), (int)(Main.maxTilesX * 0.85));
				else
					x = WorldGen.genRand.Next((int)(Main.maxTilesX * 0.15), (int)(Main.maxTilesX * 0.4));

				int y = WorldGen.genRand.Next((int)Main.rockLayer, Main.maxTilesY - 500);

				if (Main.tile[x, y].active() && Main.tile[x, y].type == 60)
				{
					success = true;
					GenNewTemple(x, y);
				}
			}
		}

		public static void GenNewTemple(int x, int y)
		{
			Rectangle[] roomBounds = new Rectangle[200];
			float worldScale = Main.maxTilesX / 4200f;
			int totalRooms = (int)(worldScale * WorldGen.genRand.Next(12, 16));
			int xDirection = WorldGen.genRand.NextBool(2).ToDirectionInt();
			int initalDirection = xDirection;
			int pathPositionX = x;
			int pathPositionY = y;
			int currentRoomPositionX = x;
			int currentRoomPositionY = y;
			int xDirectionChangePromptThreshold = WorldGen.genRand.Next(1, 3);
			int totalAttempts = 0;

			// Temple room sizes.
			for (int i = 0; i < totalRooms; i++)
			{
				totalAttempts++;
				int currentXDirection = xDirection;
				int roomPositionX = currentRoomPositionX;
				int roomPositionY = currentRoomPositionY;
				bool isValidRoom = false;
				int width = 0;
				int height = 0;
				int xOffset = -10;
				Rectangle rectangle = new Rectangle(roomPositionX - width / 2, roomPositionY - height / 2, width, height);
				while (!isValidRoom)
				{
					roomPositionX = currentRoomPositionX;
					roomPositionY = currentRoomPositionY;

					// Typical temple room size.
					width = WorldGen.genRand.Next(30, 46);
					height = WorldGen.genRand.Next(25, 31);

					// Final temple room size.
					if (i == totalRooms - 1)
					{
						width = WorldGen.genRand.Next(105, 116);
						height = WorldGen.genRand.Next(90, 106);
						roomPositionY += WorldGen.genRand.Next(6, 9);
					}

					if (totalAttempts > xDirectionChangePromptThreshold)
					{
						roomPositionY += WorldGen.genRand.Next(height + 1, height + 3) + xOffset;
						roomPositionX += WorldGen.genRand.Next(-2, 3);
						currentXDirection = xDirection * -1;
					}
					else
					{
						roomPositionX += (WorldGen.genRand.Next(width + 1, width + 3) + xOffset) * currentXDirection;
						roomPositionY += WorldGen.genRand.Next(-2, 3);
					}

					// Ensure that the new room doesn't intersect any other existing rooms.
					isValidRoom = true;
					rectangle = new Rectangle(roomPositionX - width / 2, roomPositionY - height / 2, width, height);
					for (int j = 0; j < i; j++)
					{
						if (rectangle.Intersects(roomBounds[j]))
							isValidRoom = false;
						if (WorldGen.genRand.Next(100) == 0)
						{
							xOffset++;
						}
					}
				}
				if (totalAttempts > xDirectionChangePromptThreshold)
				{
					xDirectionChangePromptThreshold++;
					totalAttempts = 1;
				}

				// Update the old values after valid room bounds have been determined.
				roomBounds[i] = rectangle;
				xDirection = currentXDirection;
				currentRoomPositionX = roomPositionX;
				currentRoomPositionY = roomPositionY;
			}

			for (int i = 0; i < totalRooms; i++)
			{
				for (int j = 0; j < 2; j++)
				{
					for (int k = 0; k < totalRooms; k++)
					{
						for (int l = 0; l < 2; l++)
						{
							int roomPositionX = roomBounds[i].X;
							if (j == 1)
								roomPositionX += roomBounds[i].Width - 1;

							int roomTop = roomBounds[i].Y;
							int roomBottom = roomTop + roomBounds[i].Height;
							int checkRoomPositionX = roomBounds[k].X;

							if (l == 1)
								checkRoomPositionX += roomBounds[k].Width - 1;

							int checkRoomTop = roomBounds[k].Y;
							int checkRoomBottom = checkRoomTop + roomBounds[k].Height;
							while (roomPositionX != checkRoomPositionX || roomTop != checkRoomTop || roomBottom != checkRoomBottom)
							{
								if (roomPositionX < checkRoomPositionX)
								{
									roomPositionX++;
								}
								if (roomPositionX > checkRoomPositionX)
								{
									roomPositionX--;
								}
								if (roomTop < checkRoomTop)
								{
									roomTop++;
								}
								if (roomTop > checkRoomTop)
								{
									roomTop--;
								}
								if (roomBottom < checkRoomBottom)
								{
									roomBottom++;
								}
								if (roomBottom > checkRoomBottom)
								{
									roomBottom--;
								}
								for (int roomPositionY = roomTop; roomPositionY < roomBottom; roomPositionY++)
								{
									Main.tile[roomPositionX, roomPositionY].active(true);
									Main.tile[roomPositionX, roomPositionY].type = TileID.LihzahrdBrick;
									Main.tile[roomPositionX, roomPositionY].liquid = 0;
									Main.tile[roomPositionX, roomPositionY].slope(0);
									Main.tile[roomPositionX, roomPositionY].halfBrick(false);
								}
							}
						}
					}
				}
			}

			// Cut out rooms and replace the empty space with walls.
			for (int i = 0; i < totalRooms; i++)
			{
				bool isLastRoom = i == totalRooms - 1;
				for (int roomPositionX = roomBounds[i].X; roomPositionX < roomBounds[i].X + roomBounds[i].Width; roomPositionX++)
				{
					for (int roomPositionY = roomBounds[i].Y; roomPositionY < roomBounds[i].Y + roomBounds[i].Height; roomPositionY++)
					{
						Main.tile[roomPositionX, roomPositionY].active(true);
						Main.tile[roomPositionX, roomPositionY].type = TileID.LihzahrdBrick;
						Main.tile[roomPositionX, roomPositionY].liquid = 0;
						Main.tile[roomPositionX, roomPositionY].slope(0);
						Main.tile[roomPositionX, roomPositionY].halfBrick(false);
					}
				}

				if (!isLastRoom)
					GenerateGenericRoom(roomBounds[i]);
				else
					GenerateShrineRoom(roomBounds[i]);
			}

			Vector2 pathPosition = new Vector2(pathPositionX, pathPositionY);

			// Cut out areas between rooms, creating corridors.
			for (int i = 0; i < totalRooms; i++)
			{
				Rectangle destinationArea = roomBounds[i];
				destinationArea.X += 8;
				destinationArea.Y += 8;
				destinationArea.Width -= 16;
				destinationArea.Height -= 16;

				// Recursively attempt to reach the next room and empty anything in said path.
				bool reachedDestination = false;
				while (!reachedDestination)
				{
					int destinationX = WorldGen.genRand.Next(destinationArea.X, destinationArea.X + destinationArea.Width);
					int destinationY = WorldGen.genRand.Next(destinationArea.Y, destinationArea.Y + destinationArea.Height);
					pathPosition = WorldGen.templePather(pathPosition, destinationX, destinationY);

					if (pathPosition.X == destinationX && pathPosition.Y == destinationY)
						reachedDestination = true;
				}
				if (i < totalRooms - 1)
				{
					reachedDestination = false;
					if (WorldGen.genRand.Next(3) != 0)
					{
						Rectangle nextRoom = roomBounds[i + 1];
						if (nextRoom.Y >= roomBounds[i].Y + roomBounds[i].Height)
						{
							destinationArea.X = nextRoom.X;

							if (nextRoom.X < roomBounds[i].X)
								destinationArea.X += (int)(nextRoom.Width * 0.2);
							else destinationArea.X += (int)(nextRoom.Width * 0.8f);

							destinationArea.Y = nextRoom.Y;
						}
						else
						{
							destinationArea.X = (roomBounds[i].X + roomBounds[i].Width / 2 + nextRoom.X + nextRoom.Width / 2) / 2;
							destinationArea.Y = (int)(nextRoom.Y + nextRoom.Height * 0.8);
						}
						while (!reachedDestination)
						{
							int destinationX = WorldGen.genRand.Next(destinationArea.X - 4, destinationArea.X + 5);
							int destinationY = WorldGen.genRand.Next(destinationArea.Y - 4, destinationArea.Y + 5);
							pathPosition = WorldGen.templePather(pathPosition, destinationX, destinationY);
							if (pathPosition.X == destinationX && pathPosition.Y == destinationY)
								reachedDestination = true;
						}
					}
					else
					{
						Rectangle nextRoom = roomBounds[i + 1];
						int roomCenterMidpointX = (roomBounds[i].X + roomBounds[i].Width / 2 + nextRoom.X + nextRoom.Width / 2) / 2;
						int roomCenterMidpointY = (roomBounds[i].Y + roomBounds[i].Height / 2 + nextRoom.Y + nextRoom.Height / 2) / 2;
						while (!reachedDestination)
						{
							int destinationX = WorldGen.genRand.Next(roomCenterMidpointX - 4, roomCenterMidpointX + 5);
							int destinationY = WorldGen.genRand.Next(roomCenterMidpointY - 4, roomCenterMidpointY + 5);
							pathPosition = WorldGen.templePather(pathPosition, destinationX, destinationY);
							if (pathPosition.X == destinationX && pathPosition.Y == destinationY)
								reachedDestination = true;
						}
					}
				}
			}

			int farthestRoomLeft = Main.maxTilesX - 20;
			int farthestRoomRight = 20;
			int farthestRoomTop = Main.maxTilesY - 20;
			int farthestRoomBottom = 20;
			for (int i = 0; i < totalRooms; i++)
			{
				if (roomBounds[i].X < farthestRoomLeft)
				{
					farthestRoomLeft = roomBounds[i].X;
				}
				if (roomBounds[i].X + roomBounds[i].Width > farthestRoomRight)
				{
					farthestRoomRight = roomBounds[i].X + roomBounds[i].Width;
				}
				if (roomBounds[i].Y < farthestRoomTop)
				{
					farthestRoomTop = roomBounds[i].Y;
				}
				if (roomBounds[i].Y + roomBounds[i].Height > farthestRoomBottom)
				{
					farthestRoomBottom = roomBounds[i].Y + roomBounds[i].Height;
				}
			}

			// Create outer temple shape.
			farthestRoomLeft -= 10;
			farthestRoomRight += 10;
			farthestRoomTop -= 10;
			farthestRoomBottom += 10;

			for (int xRoomPosition = farthestRoomLeft; xRoomPosition < farthestRoomRight; xRoomPosition++)
			{
				for (int roomPositionY = farthestRoomTop; roomPositionY < farthestRoomBottom; roomPositionY++)
				{
					WorldGen.outerTempled(xRoomPosition, roomPositionY);
				}
			}
			for (int xRoomPosition = farthestRoomRight; xRoomPosition >= farthestRoomLeft; xRoomPosition--)
			{
				for (int yRoomPosition = farthestRoomTop; yRoomPosition < farthestRoomBottom / 2; yRoomPosition++)
				{
					WorldGen.outerTempled(xRoomPosition, yRoomPosition);
				}
			}
			for (int yRoomPosition = farthestRoomTop; yRoomPosition < farthestRoomBottom; yRoomPosition++)
			{
				for (int xRoomPosition = farthestRoomLeft; xRoomPosition < farthestRoomRight; xRoomPosition++)
				{
					WorldGen.outerTempled(xRoomPosition, yRoomPosition);
				}
			}
			for (int yRoomPosition = farthestRoomBottom; yRoomPosition >= farthestRoomTop; yRoomPosition--)
			{
				for (int xRoomPosition = farthestRoomLeft; xRoomPosition < farthestRoomRight; xRoomPosition++)
				{
					WorldGen.outerTempled(xRoomPosition, yRoomPosition);
				}
			}

			// Move downward and prepare to have the tunnel go in the opposite direction(?)
			xDirection = -initalDirection;
			Vector2 endPathPosition = new Vector2(pathPositionX, pathPositionY);
			int yArea = 4;
			bool success = false;
			int totralDescentTries = 0;
			int endPathRisePrompt = WorldGen.genRand.Next(12, 14);
			while (!success)
			{
				totralDescentTries++;
				if (totralDescentTries >= endPathRisePrompt)
				{
					totralDescentTries = 0;
					endPathPosition.Y--;
				}
				endPathPosition.X += xDirection;
				success = true;
				int dy = (int)endPathPosition.Y - yArea;
				while (dy < endPathPosition.Y + yArea)
				{
					if (Main.tile[(int)endPathPosition.X, dy].wall == WallID.LihzahrdBrickUnsafe || (Main.tile[(int)endPathPosition.X, dy].active() && Main.tile[(int)endPathPosition.X, dy].type == TileID.LihzahrdBrick))
					{
						success = false;
					}
					if (Main.tile[(int)endPathPosition.X, dy].active() && Main.tile[(int)endPathPosition.X, dy].type == TileID.LihzahrdBrick)
					{
						Main.tile[(int)endPathPosition.X, dy].active(false);
						Main.tile[(int)endPathPosition.X, dy].wall = WallID.LihzahrdBrickUnsafe;
					}
					dy++;
				}
			}

			// Place bricks and walls around temple door.
			int doorPositionX = pathPositionX;
			int doorPositionY = pathPositionY;
			while (!Main.tile[doorPositionX, doorPositionY].active())
			{
				doorPositionY++;
			}
			doorPositionY -= 4;
			int doorTop = doorPositionY;
			while ((Main.tile[doorPositionX, doorTop].active() && Main.tile[doorPositionX, doorTop].type == TileID.LihzahrdBrick) || Main.tile[doorPositionX, doorTop].wall == WallID.LihzahrdBrickUnsafe)
			{
				doorTop--;
			}
			doorTop += 2;
			for (int dx = doorPositionX - 1; dx <= doorPositionX + 1; dx++)
			{
				for (int dy = doorTop; dy <= doorPositionY; dy++)
				{
					Main.tile[dx, dy].active(true);
					Main.tile[dx, dy].type = TileID.LihzahrdBrick;
					Main.tile[dx, dy].liquid = 0;
					Main.tile[dx, dy].slope(0);
					Main.tile[dx, dy].halfBrick(false);
				}
			}
			for (int dx = doorPositionX - 4; dx <= doorPositionX + 4; dx++)
			{
				for (int dy = doorPositionY - 1; dy < doorPositionY + 3; dy++)
				{
					Main.tile[dx, dy].active(false);
					Main.tile[dx, dy].wall = WallID.LihzahrdBrickUnsafe;
				}
			}
			for (int dx = doorPositionX - 1; dx <= doorPositionX + 1; dx++)
			{
				for (int dy = doorPositionY - 5; dy <= doorPositionY + 8; dy++)
				{
					Main.tile[dx, dy].active(true);
					Main.tile[dx, dy].type = TileID.LihzahrdBrick;
					Main.tile[dx, dy].liquid = 0;
					Main.tile[dx, dy].slope(0);
					Main.tile[dx, dy].halfBrick(false);
				}
			}
			for (int dx = doorPositionX - 1; dx <= doorPositionX + 1; dx++)
			{
				for (int dy = doorPositionY; dy < doorPositionY + 3; dy++)
				{
					Main.tile[dx, dy].active(false);
					Main.tile[dx, dy].wall = WallID.LihzahrdBrickUnsafe;
				}
			}

			// Temple Door
			WorldGen.PlaceTile(doorPositionX, doorPositionY, 10, true, false, -1, 11);

			// Clear space in temple
			for (int dx = farthestRoomLeft; dx < farthestRoomRight; dx++)
			{
				for (int dy = farthestRoomTop; dy < farthestRoomBottom; dy++)
				{
					WorldGen.templeCleaner(dx, dy);
				}
			}
			for (int dx = farthestRoomBottom; dx >= farthestRoomTop; dx--)
			{
				for (int dy = farthestRoomRight; dy >= farthestRoomLeft; dy--)
				{
					WorldGen.templeCleaner(dy, dx);
				}
			}

			// Place walls
			for (int dx = farthestRoomLeft; dx < farthestRoomRight; dx++)
			{
				for (int dy = farthestRoomTop; dy < farthestRoomBottom; dy++)
				{
					bool shouldPlaceWalls = true;
					for (int dx2 = dx - 1; dx2 <= dx + 1; dx2++)
					{
						for (int dy2 = dy - 1; dy2 <= dy + 1; dy2++)
						{
							if ((!Main.tile[dx2, dy2].active() || Main.tile[dx2, dy2].type != TileID.LihzahrdBrick) && Main.tile[dx2, dy2].wall != WallID.LihzahrdBrickUnsafe)
							{
								shouldPlaceWalls = false;
								break;
							}
						}
					}
					if (shouldPlaceWalls)
					{
						Main.tile[dx, dy].wall = WallID.LihzahrdBrickUnsafe;
					}
				}
			}

			// Create temple spikes
			float totalSpikeAreasToSawpn = totalRooms * 1.3f;
			int spikeAreaSpawnAttempts = 0;
			while (totalSpikeAreasToSawpn > 0f)
			{
				spikeAreaSpawnAttempts++;
				int roomToFillIndex = WorldGen.genRand.Next(totalRooms);
				int randomPointInRoomX = WorldGen.genRand.Next(roomBounds[roomToFillIndex].X, roomBounds[roomToFillIndex].X + roomBounds[roomToFillIndex].Width);
				int randomPointInRoomY = WorldGen.genRand.Next(roomBounds[roomToFillIndex].Y, roomBounds[roomToFillIndex].Y + roomBounds[roomToFillIndex].Height);
				if (Main.tile[randomPointInRoomX, randomPointInRoomY].wall == 87 && !Main.tile[randomPointInRoomX, randomPointInRoomY].active())
				{
					bool successPlacingSpikes = false;
					if (WorldGen.genRand.Next(2) == 0)
					{
						int xMoveDirection = WorldGen.genRand.NextBool(2).ToDirectionInt();
						while (!Main.tile[randomPointInRoomX, randomPointInRoomY].active())
						{
							randomPointInRoomY += xMoveDirection;
						}
						randomPointInRoomY -= xMoveDirection;
						int yMoveDirection = WorldGen.genRand.Next(2);
						int areaToCheck = WorldGen.genRand.Next(8, 10);
						bool noDoorInWay = true;
						for (int dx = randomPointInRoomX - areaToCheck; dx < randomPointInRoomX + areaToCheck; dx++)
						{
							for (int dy = randomPointInRoomY - areaToCheck; dy < randomPointInRoomY + areaToCheck; dy++)
							{
								if (Main.tile[dx, dy].active() && Main.tile[dx, dy].type == 10)
								{
									noDoorInWay = false;
									break;
								}
							}
						}
						if (noDoorInWay)
						{
							for (int dx = randomPointInRoomX - areaToCheck; dx < randomPointInRoomX + areaToCheck; dx++)
							{
								for (int dy = randomPointInRoomY - areaToCheck; dy < randomPointInRoomY + areaToCheck; dy++)
								{
									if (WorldGen.SolidTile(dx, dy) && Main.tile[dx, dy].type != TileID.WoodenSpikes && !WorldGen.SolidTile(dx, dy - xMoveDirection))
									{
										Main.tile[dx, dy].type = TileID.WoodenSpikes;
										successPlacingSpikes = true;
										if (yMoveDirection == 0)
										{
											Main.tile[dx, dy - 1].type = TileID.WoodenSpikes;
											Main.tile[dx, dy - 1].active(true);
										}
										else
										{
											Main.tile[dx, dy + 1].type = TileID.WoodenSpikes;
											Main.tile[dx, dy + 1].active(true);
										}
										yMoveDirection++;
										if (yMoveDirection > 1)
										{
											yMoveDirection = 0;
										}
									}
								}
							}
						}
						if (successPlacingSpikes)
						{
							spikeAreaSpawnAttempts = 0;
							totalSpikeAreasToSawpn--;
						}
					}
					else
					{
						int xMoveDirection = WorldGen.genRand.NextBool(2).ToDirectionInt();
						while (!Main.tile[randomPointInRoomX, randomPointInRoomY].active())
						{
							randomPointInRoomX += xMoveDirection;
						}
						randomPointInRoomX -= xMoveDirection;
						int yMoveDirection = WorldGen.genRand.Next(2);
						int areaToCheck = WorldGen.genRand.Next(8, 10);
						bool noDoorInWay = true;
						for (int dx = randomPointInRoomX - areaToCheck; dx < randomPointInRoomX + areaToCheck; dx++)
						{
							for (int dy = randomPointInRoomY - areaToCheck; dy < randomPointInRoomY + areaToCheck; dy++)
							{
								if (Main.tile[dx, dy].active() && Main.tile[dx, dy].type == 10)
								{
									noDoorInWay = false;
									break;
								}
							}
						}
						if (noDoorInWay)
						{
							for (int dx = randomPointInRoomX - areaToCheck; dx < randomPointInRoomX + areaToCheck; dx++)
							{
								for (int dy = randomPointInRoomY - areaToCheck; dy < randomPointInRoomY + areaToCheck; dy++)
								{
									if (WorldGen.SolidTile(dx, dy) && Main.tile[dx, dy].type != TileID.WoodenSpikes && !WorldGen.SolidTile(dx - xMoveDirection, dy))
									{
										Main.tile[dx, dy].type = TileID.WoodenSpikes;
										successPlacingSpikes = true;
										if (yMoveDirection == 0)
										{
											Main.tile[dx - 1, dy].type = TileID.WoodenSpikes;
											Main.tile[dx - 1, dy].active(true);
										}
										else
										{
											Main.tile[dx + 1, dy].type = TileID.WoodenSpikes;
											Main.tile[dx + 1, dy].active(true);
										}
										yMoveDirection++;
										if (yMoveDirection > 1)
										{
											yMoveDirection = 0;
										}
									}
								}
							}
						}
						if (successPlacingSpikes)
						{
							spikeAreaSpawnAttempts = 0;
							totalSpikeAreasToSawpn--;
						}
					}
				}
				if (spikeAreaSpawnAttempts > 1000)
				{
					spikeAreaSpawnAttempts = 0;
					totalSpikeAreasToSawpn--;
				}
			}

			// Set variables for the bounds and room count of the temple.
			WorldGen.tLeft = farthestRoomLeft;
			WorldGen.tRight = farthestRoomRight;
			WorldGen.tTop = farthestRoomTop;
			WorldGen.tBottom = farthestRoomBottom;
			WorldGen.tRooms = totalRooms;
		}

		public static void GenerateGenericRoom(Rectangle roomBounds)
		{
			int roomLeft = roomBounds.X;
			int roomRight = roomLeft + roomBounds.Width;
			int roomTop = roomBounds.Y;
			int roomBottom = roomTop + roomBounds.Height;

			roomLeft += WorldGen.genRand.Next(4, 7);
			roomRight -= WorldGen.genRand.Next(4, 7);
			roomTop += WorldGen.genRand.Next(4, 7);
			roomBottom -= WorldGen.genRand.Next(4, 7);

			int offsettedRoomLeft = roomLeft;
			int offsettedRoomRight = roomRight;
			int offsettedRoomTop = roomTop;
			int offsettedRoomBottom = roomBottom;

			int roomCenterX = (roomLeft + roomRight) / 2;
			int roomCenterY = (roomTop + roomBottom) / 2;

			// Cut out the rooms with a bit of randomness incorporated, so that the room is not entirely square.
			for (int dx = roomLeft; dx < roomRight; dx++)
			{
				for (int dy = roomTop; dy < roomBottom; dy++)
				{
					if (WorldGen.genRand.NextBool(20))
						offsettedRoomTop += WorldGen.genRand.NextBool(2).ToDirectionInt();

					if (WorldGen.genRand.NextBool(20))
						offsettedRoomBottom += WorldGen.genRand.NextBool(2).ToDirectionInt();

					if (WorldGen.genRand.NextBool(20))
						offsettedRoomLeft += WorldGen.genRand.NextBool(2).ToDirectionInt();

					if (WorldGen.genRand.NextBool(20))
						offsettedRoomRight += WorldGen.genRand.NextBool(2).ToDirectionInt();

					offsettedRoomLeft = Utils.Clamp(offsettedRoomLeft, roomLeft, roomCenterX);
					offsettedRoomRight = Utils.Clamp(offsettedRoomRight, roomCenterX, roomRight);
					offsettedRoomTop = Utils.Clamp(offsettedRoomTop, roomTop, roomCenterY);
					offsettedRoomBottom = Utils.Clamp(offsettedRoomBottom, roomCenterY, roomBottom);

					if (dx >= offsettedRoomLeft && (dx < offsettedRoomRight & dy >= offsettedRoomTop) && dy <= offsettedRoomBottom)
					{
						Main.tile[dx, dy].active(false);
						Main.tile[dx, dy].liquid = 0; // Apparently this can indeed happen, and it can interfere with furniture placement.
						Main.tile[dx, dy].wall = WallID.LihzahrdBrickUnsafe;
					}
				}
			}
			for (int dy = roomBottom; dy > roomTop; dy--)
			{
				for (int dx = roomRight; dx > roomLeft; dx--)
				{
					if (WorldGen.genRand.NextBool(20))
						offsettedRoomTop += WorldGen.genRand.NextBool(2).ToDirectionInt();

					if (WorldGen.genRand.NextBool(20))
						offsettedRoomBottom += WorldGen.genRand.NextBool(2).ToDirectionInt();

					if (WorldGen.genRand.NextBool(20))
						offsettedRoomLeft += WorldGen.genRand.NextBool(2).ToDirectionInt();

					if (WorldGen.genRand.NextBool(20))
						offsettedRoomRight += WorldGen.genRand.NextBool(2).ToDirectionInt();

					offsettedRoomLeft = Utils.Clamp(offsettedRoomLeft, roomLeft, roomCenterX);
					offsettedRoomRight = Utils.Clamp(offsettedRoomRight, roomCenterX, roomRight);
					offsettedRoomTop = Utils.Clamp(offsettedRoomTop, roomTop, roomCenterY);
					offsettedRoomBottom = Utils.Clamp(offsettedRoomBottom, roomCenterY, roomBottom);

					if (dx >= offsettedRoomLeft && (dx < offsettedRoomRight & dy >= offsettedRoomTop) && dy <= offsettedRoomBottom)
					{
						Main.tile[dx, dy].active(false);
						Main.tile[dx, dy].liquid = 0; // Apparently this can indeed happen, and it can interfere with furniture placement.
						Main.tile[dx, dy].wall = WallID.LihzahrdBrickUnsafe;
					}
				}
			}
		}

		public static void GenerateShrineRoom(Rectangle roomBounds)
		{
			// Cut out the room.
			for (int dx = roomBounds.Left; dx < roomBounds.Right; dx++)
			{
				for (int dy = roomBounds.Top; dy < roomBounds.Bottom; dy++)
				{
					Main.tile[dx, dy].active(false);
					Main.tile[dx, dy].liquid = 0; // Apparently this can indeed happen, and it can interfere with furniture placement.
					Main.tile[dx, dy].wall = WallID.LihzahrdBrickUnsafe;
				}
			}

			// Place the shrine atop a pedestal.
			int roomCenterX = roomBounds.Center.X;
			int roomBottom = roomBounds.Bottom;
			int shrinePedestalWidth = roomBounds.Width / 6 + WorldGen.genRand.Next(6);

			// Ensure that the pedestal is always an even number, so that the shrine can be placed atop symmetrically.
			if (shrinePedestalWidth % 2 != 0)
				shrinePedestalWidth++;

			int calculateHeightFromOutwardness(int outwardness)
			{
				int height = Math.Max(outwardness, 4); // 8 tile wide area on top.

				return (shrinePedestalWidth / 2) - height; // Invert height so that it ascends instead of descends.
			}

			// Place the pedestal.
			for (int dx = 0; dx < shrinePedestalWidth / 2; dx++)
			{
				int height = calculateHeightFromOutwardness(dx);

				for (int y = roomBottom - height; y <= roomBottom; y++)
				{
					Main.tile[roomCenterX - dx, y].active(true);
					Main.tile[roomCenterX - dx, y].type = TileID.LihzahrdBrick;

					Main.tile[roomCenterX + dx, y].active(true);
					Main.tile[roomCenterX + dx, y].type = TileID.LihzahrdBrick;
				}
			}

			// And place the shrine.
			CalamityWorld.newAltarX = roomCenterX - 1;
			CalamityWorld.newAltarY = roomBottom - calculateHeightFromOutwardness(0) - 2;
		}

		public static void NewJungleTemplePart2()
		{
			int templeLeft = WorldGen.tLeft;
			int templeRight = WorldGen.tRight;
			int templeTop = WorldGen.tTop;
			int templeBottom = WorldGen.tBottom;
			int totalRooms = WorldGen.tRooms;
			float totalTrapsToPlace = totalRooms * 2.1f;
			float totalStatuesToPlace = totalRooms * 1.6f;
			float totalChestsToPlace = totalRooms * 0.4f;

			int placementAttempts = 0;
			while (totalTrapsToPlace > 0f)
			{
				int randomPointInRoomX = WorldGen.genRand.Next(templeLeft, templeRight);
				int randomPointInRoomY = WorldGen.genRand.Next(templeTop, templeBottom);
				if (Main.tile[randomPointInRoomX, randomPointInRoomY].wall == WallID.LihzahrdBrickUnsafe && !Main.tile[randomPointInRoomX, randomPointInRoomY].active())
				{
					if (WorldGen.mayanTrap(randomPointInRoomX, randomPointInRoomY))
					{
						totalTrapsToPlace--;
						placementAttempts = 0;
					}
					else
					{
						placementAttempts++;
					}
				}
				else
				{
					placementAttempts++;
				}
				if (placementAttempts > 100)
				{
					placementAttempts = 0;
					totalTrapsToPlace--;
				}
			}

			// Bit of a weird solution that makes wooden spikes non-solid so that nothing is placed on top of them.
			Main.tileSolid[TileID.WoodenSpikes] = false;
			
			placementAttempts = 0;
			while (totalChestsToPlace > 0f)
			{
				int randomPointInRoomX = WorldGen.genRand.Next(templeLeft, templeRight);
				int randomPointInRoomY = WorldGen.genRand.Next(templeTop, templeBottom);
				if (Main.tile[randomPointInRoomX, randomPointInRoomY].wall == WallID.LihzahrdBrickUnsafe &&
					!Main.tile[randomPointInRoomX, randomPointInRoomY].active() && 
					WorldGen.AddBuriedChest(randomPointInRoomX, randomPointInRoomY, ItemID.LihzahrdPowerCell, true, 16))
				{
					totalChestsToPlace--;
					placementAttempts = 0;
				}
				placementAttempts++;
				if (placementAttempts > 10000)
				{
					break;
				}
			}
			placementAttempts = 0;
			while (totalStatuesToPlace > 0f)
			{
				placementAttempts++;
				int randomPointInRoomX = WorldGen.genRand.Next(templeLeft, templeRight);
				int randomPointInRoomY = WorldGen.genRand.Next(templeTop, templeBottom);
				if (Main.tile[randomPointInRoomX, randomPointInRoomY].wall == WallID.LihzahrdBrickUnsafe && !Main.tile[randomPointInRoomX, randomPointInRoomY].active())
				{
					int statuePlacementPositionY = randomPointInRoomY;
					while (!Main.tile[randomPointInRoomX, statuePlacementPositionY].active())
					{
						statuePlacementPositionY++;
						if (statuePlacementPositionY > templeBottom)
						{
							break;
						}
					}
					statuePlacementPositionY--;
					if (statuePlacementPositionY <= templeBottom)
					{
						WorldGen.PlaceTile(randomPointInRoomX, statuePlacementPositionY, TileID.Statues, true, false, -1, WorldGen.genRand.Next(43, 46));
						if (Main.tile[randomPointInRoomX, statuePlacementPositionY].type == TileID.Statues)
						{
							totalStatuesToPlace--;
						}
					}
				}
			}

			float totalFurnitureElementsToPlace = totalRooms * 1.6f;

			placementAttempts = 0;
			while (totalFurnitureElementsToPlace > 0f)
			{
				placementAttempts++;
				int randomPointInRoomX = WorldGen.genRand.Next(templeLeft, templeRight);
				int randomPointInRoomY = WorldGen.genRand.Next(templeTop, templeBottom);
				if (Main.tile[randomPointInRoomX, randomPointInRoomY].wall == WallID.LihzahrdBrickUnsafe && !Main.tile[randomPointInRoomX, randomPointInRoomY].active())
				{
					int furniturePlacementPositionY = randomPointInRoomY;
					while (!Main.tile[randomPointInRoomX, furniturePlacementPositionY].active())
					{
						furniturePlacementPositionY++;
						if (furniturePlacementPositionY > templeBottom)
						{
							break;
						}
					}
					furniturePlacementPositionY--;
					if (furniturePlacementPositionY <= templeBottom)
					{
						int totalRooms0 = WorldGen.genRand.Next(3);
						if (totalRooms0 == 0)
						{
							WorldGen.PlaceTile(randomPointInRoomX, furniturePlacementPositionY, TileID.WorkBenches, true, false, -1, 10);
							if (Main.tile[randomPointInRoomX, furniturePlacementPositionY].type == TileID.WorkBenches)
							{
								totalFurnitureElementsToPlace--;
							}
						}
						else if (totalRooms0 == 1)
						{
							WorldGen.PlaceTile(randomPointInRoomX, furniturePlacementPositionY, TileID.Tables, true, false, -1, 9);
							if (Main.tile[randomPointInRoomX, furniturePlacementPositionY].type == TileID.Tables)
							{
								totalFurnitureElementsToPlace--;
							}
						}
						else if (totalRooms0 == 2)
						{
							WorldGen.PlaceTile(randomPointInRoomX, furniturePlacementPositionY, TileID.Chairs, true, false, -1, 12);
							if (Main.tile[randomPointInRoomX, furniturePlacementPositionY].type == TileID.Chairs)
							{
								totalFurnitureElementsToPlace--;
							}
						}
					}
				}
				if (placementAttempts > 10000)
				{
					break;
				}
			}

			// Return wood spikes back to normal after the above placements are done.
			Main.tileSolid[TileID.WoodenSpikes] = true;
		}

		public static void NewJungleTempleLihzahrdAltar()
		{
			int alterPositionX = CalamityWorld.newAltarX;
			int alterPositionY = CalamityWorld.newAltarY;
			for (int i = 0; i <= 2; i++)
			{
				for (int j = 0; j < 2; j++)
				{
					int framedAlterPositionX = alterPositionX + i;
					int framedAlterPositionY = alterPositionY + j;
					Main.tile[framedAlterPositionX, framedAlterPositionY].active(true);
					Main.tile[framedAlterPositionX, framedAlterPositionY].type = TileID.LihzahrdAltar;
					Main.tile[framedAlterPositionX, framedAlterPositionY].frameX = (short)(i * 18);
					Main.tile[framedAlterPositionX, framedAlterPositionY].frameY = (short)(j * 18);
				}

				// Ensure that the tiles below the altar are active and valid.
				Main.tile[i, alterPositionY + 2].active(true);
				Main.tile[i, alterPositionY + 2].slope(0);
				Main.tile[i, alterPositionY + 2].halfBrick(false);
				Main.tile[i, alterPositionY + 2].type = TileID.LihzahrdBrick;
			}
		}
	}
}
