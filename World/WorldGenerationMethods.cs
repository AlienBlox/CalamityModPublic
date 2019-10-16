﻿
using CalamityMod.World.Planets;
using Microsoft.Xna.Framework;
using System;
using System.Reflection;
using Terraria; using CalamityMod.Projectiles; using Terraria.ModLoader;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles; using CalamityMod.Tiles; using CalamityMod.Walls;
using Terraria.Utilities;
using Terraria.World.Generation;

namespace CalamityMod.World
{
    public class WorldGenerationMethods : ModWorld
    {
        #region BiomeChests
        public static void GenerateBiomeChests(GenerationProgress progress)
        {
            Mod mod = ModLoader.GetMod("CalamityMod");
            // Get dungeon size field infos. These fields are private for some reason
            int MinX = (int)typeof(WorldGen).GetField("dMinX", BindingFlags.Static | BindingFlags.NonPublic).GetValue(null) + 25;
            int MaxX = (int)typeof(WorldGen).GetField("dMaxX", BindingFlags.Static | BindingFlags.NonPublic).GetValue(null) - 25;
            int MaxY = (int)typeof(WorldGen).GetField("dMaxY", BindingFlags.Static | BindingFlags.NonPublic).GetValue(null) - 25;
            int[] ChestTypes = new int[] { ModContent.TileType<AstralChestLocked>() };
            int[] ItemTypes = new int[] { ModContent.ItemType<HeavenfallenStardisk>() };

            progress.Message = "Calamity Mod: Biome Chests";

            int rounds = 1;
            for (int i = 0; i < ChestTypes.Length * rounds; i++)
            {
                Chest chest = null;
                int attempts = 0;
                while (chest == null && attempts < 1000)
                {
                    attempts++;
                    int x = WorldGen.genRand.Next(MinX, MaxX);
                    int y = WorldGen.genRand.Next((int)Main.worldSurface, MaxY);
                    if (Main.wallDungeon[Main.tile[x, y].wall] && !Main.tile[x, y].active())
                    {
                        chest = AddChestWithDefaultLoot(x, y, (ushort)ChestTypes[i % ChestTypes.Length], 1, 1);
                    }
                }
                if (chest != null)
                {
                    chest.item[0].SetDefaults(ItemTypes[i % ChestTypes.Length]);
                    chest.item[0].Prefix(-1);
                }
            }
        }

        internal static Chest AddChestWithDefaultLoot(int i, int j, ushort type = TileID.Containers, uint emptySlots = 1, int Style = 0)
        {
            Chest chest = null;
            while (j < Main.maxTilesY - 210)
            {
                if (!WorldGen.SolidTile(i, j))
                {
                    j++;
                    continue;
                }
                int chestIndex = WorldGen.PlaceChest(i - 1, j - 1, type, false, Style);
                if (chestIndex < 0)
                {
                    break;
                }
                chest = Main.chest[chestIndex];
                uint itemIndex = emptySlots;
                while (itemIndex == emptySlots)
                {
                    Mod mod = ModLoader.GetMod("CalamityMod");
                    bool AstralChest = type == ModContent.TileType<AstralChestLocked>();
                    int cItem;
                    cItem = WorldGen.genRand.NextBool() ? WorldGen.goldBar : WorldGen.silverBar;
                    int addAmount = 0;
                    if (AstralChest)
                        addAmount = 4;
                    chest.item[itemIndex].SetDefaults(cItem, false);
                    chest.item[itemIndex].stack = WorldGen.genRand.Next(3 + addAmount, 11 + addAmount * 2);
                    itemIndex++;
                    if (WorldGen.genRand.NextBool())
                    {
                        cItem = ItemID.HolyArrow;
                        int addAmount2 = 0;
                        if (AstralChest)
                            addAmount2 = 25;
                        chest.item[itemIndex].SetDefaults(cItem, false);
                        chest.item[itemIndex].stack = WorldGen.genRand.Next(25 + addAmount2, 51 + addAmount2 * 2);
                        itemIndex++;
                    }
                    if (AstralChest)
                    {
                        chest.item[itemIndex].SetDefaults(ModContent.ItemType<AstralJelly>(), false);
                        chest.item[itemIndex].stack = WorldGen.genRand.Next(3, 6);
                        itemIndex++;
                    }
                    if (WorldGen.genRand.NextBool())
                    {
                        int[] items = new int[] {
                            ItemID.SpelunkerPotion, ItemID.FeatherfallPotion, ItemID.NightOwlPotion,
                            ItemID.WaterWalkingPotion, ItemID.ArcheryPotion, ItemID.GravitationPotion
                        };
                        if (AstralChest)
                        {
                            items[1] = ModContent.ItemType<RevivifyPotion>();
                            items[4] = ItemID.ShinePotion;
                            items[5] = ItemID.HunterPotion;
                        }
                        chest.item[itemIndex].SetDefaults(WorldGen.genRand.Next(items), false);
                        chest.item[itemIndex].stack = WorldGen.genRand.Next(1, 3);
                        itemIndex++;
                    }
                    if (WorldGen.genRand.NextBool())
                    {
                        int[] items = new int[] {
                            ItemID.ThornsPotion, ItemID.WaterWalkingPotion, ItemID.InvisibilityPotion,
                            ItemID.ManaRegenerationPotion, ItemID.TeleportationPotion, ItemID.TrapsightPotion, ItemID.TrapsightPotion // yes, dangersense potions have double the chance as other potions in vanilla for some reason
                        };
                        if (AstralChest)
                        {
                            items[1] = ItemID.MagicPowerPotion;
                            items[2] = ModContent.ItemType<ZenPotion>();
                            items[5] = ModContent.ItemType<CadencePotion>();
                        }
                        chest.item[itemIndex].SetDefaults(WorldGen.genRand.Next(items), false);
                        chest.item[itemIndex].stack = WorldGen.genRand.Next(1, 3);
                        itemIndex++;
                    }
                    if (WorldGen.genRand.NextBool())
                    {
                        cItem = ItemID.RecallPotion;
                        int addAmount2 = 0;
                        if (AstralChest)
                        {
                            cItem = ItemID.BouncyDynamite;
                            addAmount2 = 2;
                        }
                        chest.item[itemIndex].SetDefaults(cItem, false);
                        chest.item[itemIndex].stack = WorldGen.genRand.Next(1, 3) * addAmount2;
                        itemIndex++;
                    }
                    if (AstralChest)
                        addAmount = 2;
                    chest.item[itemIndex].SetDefaults(ItemID.GoldCoin, false);
                    chest.item[itemIndex].stack = WorldGen.genRand.Next(1, 3) * addAmount;
                    itemIndex++;
                }
            }
            return chest;
        }
        #endregion

        #region Place Rox Shrine
        public static void PlaceRoxShrine()
        {
            Mod mod = ModLoader.GetMod("CalamityMod");
            while (!CalamityWorld.roxShrinePlaced)
            {
                CalamityWorld.roxShrinePlaced = true;
                for (int x = 0; x < Main.maxTilesX; x++)
                {
                    for (int y = 0; y < Main.maxTilesY; y++)
                    {
                        if (Main.tile[x, y] != null && Main.tile[x, y].type == TileID.LargePiles)
                        {
                            if ((Main.tile[x, y].frameX == 18 && Main.tile[x, y].frameY == 0) || (Main.tile[x, y].frameX == 45 && Main.tile[x, y].frameY == 0))
                            {
                                if (WorldGen.genRand.Next(3) == 0)
                                {
                                    for (int dx = -1; dx < 2; dx++)
                                    {
                                        for (int dy = -1; dy < 2; dy++)
                                            Main.tile[x + dx, y + dy].active(false);
                                    }

                                    WorldGen.PlaceTile(x, y + 1, ModContent.TileType<RoxTile>());
                                    return;
                                }
                                else
                                    CalamityWorld.roxShrinePlaced = false;
                            }
                        }
                    }
                }
            }
        }
        #endregion

        #region SafeTileFrame
        public static void SafeSquareTileFrame(int i, int j, bool resetFrame = true)
        {
            if (Main.tile[i, j] != null)
            {
                for (int x = i - 1; x <= i + 1; x++)
                {
                    for (int y = j - 1; y <= j + 1; y++)
                    {
                        if (x < 0 || y < 0 || x >= Main.maxTilesX || y >= Main.maxTilesY)
                            continue;
                        if (x == i && y == j)
                        {
                            WorldGen.TileFrame(i, j, resetFrame, false);
                        }
                        else
                        {
                            WorldGen.TileFrame(x, y, false, false);
                        }
                    }
                }
            }
        }
        #endregion

        #region ShuffleArray
        public static int[] ShuffleArray(int[] array)
        {
            Random random = new Random();
            for (int index = array.Length; index > 0; index--)
            {
                int j = random.Next(index);
                int k = array[j];
                array[j] = array[index - 1];
                array[index - 1] = k;
            }
            return array;
        }
        #endregion

        #region OreSpawn
        public static void SpawnOre(int type, double frequency, float depth, float depthLimit)
        {
            Mod mod = ModLoader.GetMod("CalamityMod");
            int x = Main.maxTilesX;
            int y = Main.maxTilesY;
            if (type == ModContent.TileType<ExodiumOre>())
            {
                depthLimit = 0.14f;
                if (y > 1500)
                { depthLimit = 0.1f; if (y > 2100) { depthLimit = 0.07f; } }
            }
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                for (int k = 0; k < (int)((double)(x * y) * frequency); k++)
                {
                    int tilesX = WorldGen.genRand.Next(0, x);
                    int tilesY = WorldGen.genRand.Next((int)(y * depth), (int)(y * depthLimit));
                    if (type == ModContent.TileType<AuricOre>())
                    {
                        WorldGen.OreRunner(tilesX, tilesY, (double)WorldGen.genRand.Next(12, 18), WorldGen.genRand.Next(12, 18), (ushort)type);
                    }
                    else if (type == ModContent.TileType<UelibloomOre>())
                    {
                        if (Main.tile[tilesX, tilesY].type == 59)
                        {
                            WorldGen.OreRunner(tilesX, tilesY, (double)WorldGen.genRand.Next(3, 8), WorldGen.genRand.Next(3, 8), (ushort)type);
                        }
                    }
                    else if (type == ModContent.TileType<PerennialOre>())
                    {
                        if (Main.tile[tilesX, tilesY].type == 0 || Main.tile[tilesX, tilesY].type == 1)
                        {
                            WorldGen.OreRunner(tilesX, tilesY, (double)WorldGen.genRand.Next(3, 8), WorldGen.genRand.Next(3, 8), (ushort)type);
                        }
                    }
                    else if (type == ModContent.TileType<CryonicOre>())
                    {
                        if (Main.tile[tilesX, tilesY].type == 147 || Main.tile[tilesX, tilesY].type == 161 || Main.tile[tilesX, tilesY].type == 163 || Main.tile[tilesX, tilesY].type == 164 || Main.tile[tilesX, tilesY].type == 200)
                        {
                            WorldGen.OreRunner(tilesX, tilesY, (double)WorldGen.genRand.Next(3, 8), WorldGen.genRand.Next(3, 8), (ushort)type);
                        }
                    }
                    else
                    {
                        WorldGen.OreRunner(tilesX, tilesY, (double)WorldGen.genRand.Next(3, 8), WorldGen.genRand.Next(3, 8), (ushort)type);
                    }
                }
            }
        }
        #endregion

        #region AstralMeteor
        public static bool CanAstralMeteorSpawn()
        {
            Mod mod = ModLoader.GetMod("CalamityMod");
            int astralOreCount = 0;
            float worldSizeFactor = Main.maxTilesX / 4200f; // Small = 4200, Medium = 6400, Large = 8400
            int astralOreAllowed = (int)(200f * worldSizeFactor); // Small = 201 Medium = 305 Large = 401
            for (int x = 5; x < Main.maxTilesX - 5; x++)
            {
                int y = 5;
                while (y < Main.worldSurface)
                {
                    if (Main.tile[x, y].active() && Main.tile[x, y].type == ModContent.TileType<AstralOre>())
                    {
                        astralOreCount++;
                        if (astralOreCount > astralOreAllowed)
                            return false;
                    }
                    y++;
                }
            }
            return true;
        }

        public static bool CanAstralBiomeSpawn()
        {
            Mod mod = ModLoader.GetMod("CalamityMod");
            int astralTileCount = 0;
            float worldSizeFactor = Main.maxTilesX / 4200f; // Small = 4200, Medium = 6400, Large = 8400
            int astralTilesAllowed = (int)(400f * worldSizeFactor); // Small = 401 Medium = 605 Large = 801
            for (int x = 5; x < Main.maxTilesX - 5; x++)
            {
                int y = 5;
                while (y < Main.worldSurface)
                {
                    if (Main.tile[x, y].active() &&
                        (Main.tile[x, y].type == ModContent.TileType<AstralSand>() || Main.tile[x, y].type == ModContent.TileType<AstralSandstone>() ||
                        Main.tile[x, y].type == ModContent.TileType<HardenedAstralSand>() || Main.tile[x, y].type == ModContent.TileType<AstralIce>() ||
                        Main.tile[x, y].type == ModContent.TileType<AstralDirt>() || Main.tile[x, y].type == ModContent.TileType<AstralStone>() ||
                        Main.tile[x, y].type == ModContent.TileType<AstralGrass>()))
                    {
                        astralTileCount++;
                        if (astralTileCount > astralTilesAllowed)
                            return false;
                    }
                    y++;
                }
            }
            return true;
        }

        public static void AstralMeteorThreadWrapper(object context)
        {
            PlaceAstralMeteor();
        }

        public static void PlaceAstralMeteor()
        {
            Mod mod = ModLoader.GetMod("CalamityMod");
            Mod ancientsAwakened = ModLoader.GetMod("AAMod");

            // This flag is also used to determine whether players are nearby.
            bool meteorDropped = true;

            // Clients in multiplayer don't drop meteors.
            if (Main.netMode == NetmodeID.MultiplayerClient)
                return;
            for (int i = 0; i < 255; i++)
            {
                if (Main.player[i].active)
                {
                    meteorDropped = false;
                    break;
                }
            }

            // Check whether there is already too much ore.
            if (!CanAstralMeteorSpawn())
                return;

            UnifiedRandom rand = WorldGen.genRand;
            float solidTileRequirement = 600f;
            bool localAbyssSide = WorldGen.dungeonX < Main.maxTilesX / 2;
            while (!meteorDropped)
            {
                float worldEdgeMargin = (float)Main.maxTilesX * 0.08f;
                int xLimit = Main.maxTilesX / 2;
                int x = CalamityWorld.abyssSide ? rand.Next(400, xLimit) : rand.Next(xLimit, Main.maxTilesX - 400);
                while ((float)x > (float)Main.spawnTileX - worldEdgeMargin && (float)x < (float)Main.spawnTileX + worldEdgeMargin)
                {
                    x = CalamityWorld.abyssSide ? rand.Next(400, xLimit) : rand.Next(xLimit, Main.maxTilesX - 400);
                }
                //world surface = 920 large 740 medium 560 small
                int y = (int)(Main.worldSurface * 0.5); //Large = 522, Medium = 444, Small = 336
                while (y < Main.maxTilesY)
                {
                    if (Main.tile[x, y].active() && Main.tileSolid[(int)Main.tile[x, y].type])
                    {
                        int suitableTiles = 0;
                        int checkRadius = 15;
                        for (int l = x - checkRadius; l < x + checkRadius; l++)
                        {
                            for (int m = y - checkRadius; m < y + checkRadius; m++)
                            {
                                if (WorldGen.SolidTile(l, m))
                                {
                                    suitableTiles++;

                                    // Avoid floating islands: Clouds and Sunplate both harshly punish attempted meteor spawns
                                    if (Main.tile[l, m].type == TileID.Cloud || Main.tile[l, m].type == TileID.Sunplate)
                                    {
                                        suitableTiles -= 100;
                                    }

                                    // Prevent the Astral biome from overriding or interfering with an AA biome
                                    else if (ancientsAwakened != null)
                                    {
                                        if (Main.tile[l, m].type == ancientsAwakened.TileType("InfernoGrass") || Main.tile[l, m].type == ancientsAwakened.TileType("Torchstone") ||
                                            Main.tile[l, m].type == ancientsAwakened.TileType("Torchsand") || Main.tile[l, m].type == ancientsAwakened.TileType("Torchsandstone") ||
                                            Main.tile[l, m].type == ancientsAwakened.TileType("Torchsandhardened") || Main.tile[l, m].type == ancientsAwakened.TileType("Torchice") ||
                                            Main.tile[l, m].type == ancientsAwakened.TileType("Depthstone") || Main.tile[l, m].type == ancientsAwakened.TileType("Depthsand") ||
                                            Main.tile[l, m].type == ancientsAwakened.TileType("Depthsandstone") || Main.tile[l, m].type == ancientsAwakened.TileType("Depthsandhardened") ||
                                            Main.tile[l, m].type == ancientsAwakened.TileType("Depthice"))
                                        {
                                            suitableTiles -= 100;
                                        }
                                    }
                                }

                                // Liquid aversion makes meteors less likely to fall in lakes
                                else if (Main.tile[l, m].liquid > 0)
                                {
                                    suitableTiles--;
                                }
                            }
                        }

                        if ((float)suitableTiles < solidTileRequirement)
                        {
                            solidTileRequirement -= 0.5f;
                            break;
                        }
                        meteorDropped = GenerateAstralMeteor(x, y);

                        // If the meteor actually dropped, post the message stating as such.
                        if (meteorDropped)
                        {
                            string key = "Mods.CalamityMod.AstralText";
                            Color messageColor = Color.Gold;

                            if (Main.netMode == NetmodeID.SinglePlayer)
                                Main.NewText(Language.GetTextValue(key), messageColor);
                            else if (Main.netMode == NetmodeID.Server)
                                NetMessage.BroadcastChatMessage(NetworkText.FromKey(key), messageColor);
                            break;
                        }
                        break;
                    }
                    else
                    {
                        y++;
                    }
                }
                if (solidTileRequirement < 100f)
                {
                    return;
                }
            }
        }

        public static bool GenerateAstralMeteor(int i, int j)
        {
            UnifiedRandom rand = WorldGen.genRand;
            Mod mod = ModLoader.GetMod("CalamityMod");
            if (i < 50 || i > Main.maxTilesX - 50)
            {
                return false;
            }
            if (j < 50 || j > Main.maxTilesY - 50)
            {
                return false;
            }
            int num = 35;
            Rectangle rectangle = new Rectangle((i - num) * 16, (j - num) * 16, num * 2 * 16, num * 2 * 16);
            for (int k = 0; k < 255; k++)
            {
                if (Main.player[k].active)
                {
                    Rectangle value = new Rectangle((int)(Main.player[k].position.X + (float)(Main.player[k].width / 2) - (float)(NPC.sWidth / 2) - (float)NPC.safeRangeX), (int)(Main.player[k].position.Y + (float)(Main.player[k].height / 2) - (float)(NPC.sHeight / 2) - (float)NPC.safeRangeY), NPC.sWidth + NPC.safeRangeX * 2, NPC.sHeight + NPC.safeRangeY * 2);
                    if (rectangle.Intersects(value))
                    {
                        return false;
                    }
                }
            }
            for (int l = 0; l < 200; l++)
            {
                if (Main.npc[l].active)
                {
                    Rectangle value2 = new Rectangle((int)Main.npc[l].position.X, (int)Main.npc[l].position.Y, Main.npc[l].width, Main.npc[l].height);
                    if (rectangle.Intersects(value2))
                    {
                        return false;
                    }
                }
            }
            for (int m = i - num; m < i + num; m++)
            {
                for (int n = j - num; n < j + num; n++)
                {
                    if (Main.tile[m, n].active() && Main.tile[m, n].type == 21)
                    {
                        return false;
                    }
                }
            }
            num = rand.Next(17, 23);
            for (int num2 = i - num; num2 < i + num; num2++)
            {
                for (int num3 = j - num; num3 < j + num; num3++)
                {
                    if (num3 > j + rand.Next(-2, 3) - 5)
                    {
                        float num4 = (float)Math.Abs(i - num2);
                        float num5 = (float)Math.Abs(j - num3);
                        float num6 = (float)Math.Sqrt((double)(num4 * num4 + num5 * num5));
                        if ((double)num6 < (double)num * 0.9 + (double)rand.Next(-4, 5))
                        {
                            if (Main.tile[num2, num3] != null)
                            {
                                if (!Main.tileSolid[(int)Main.tile[num2, num3].type])
                                {
                                    Main.tile[num2, num3].active(false);
                                }
                                Main.tile[num2, num3].type = (ushort)ModContent.TileType<AstralOre>();
                            }
                        }
                    }
                }
            }
            num = WorldGen.genRand.Next(8, 14);
            for (int num7 = i - num; num7 < i + num; num7++)
            {
                for (int num8 = j - num; num8 < j + num; num8++)
                {
                    if (num8 > j + rand.Next(-2, 3) - 4)
                    {
                        float num9 = (float)Math.Abs(i - num7);
                        float num10 = (float)Math.Abs(j - num8);
                        float num11 = (float)Math.Sqrt((double)(num9 * num9 + num10 * num10));
                        if ((double)num11 < (double)num * 0.8 + (double)rand.Next(-3, 4))
                        {
                            if (Main.tile[num7, num8] != null)
                                Main.tile[num7, num8].active(false);
                        }
                    }
                }
            }
            num = WorldGen.genRand.Next(25, 35);
            for (int num12 = i - num; num12 < i + num; num12++)
            {
                for (int num13 = j - num; num13 < j + num; num13++)
                {
                    float num14 = (float)Math.Abs(i - num12);
                    float num15 = (float)Math.Abs(j - num13);
                    float num16 = (float)Math.Sqrt((double)(num14 * num14 + num15 * num15));
                    if (Main.tile[num12, num13] != null)
                    {
                        if ((double)num16 < (double)num * 0.7)
                        {
                            if (Main.tile[num12, num13].type == 5 || Main.tile[num12, num13].type == 32 || Main.tile[num12, num13].type == 352)
                            {
                                WorldGen.KillTile(num12, num13, false, false, false);
                            }
                            Main.tile[num12, num13].liquid = 0;
                        }
                        if (Main.tile[num12, num13].type == (ushort)ModContent.TileType<AstralOre>())
                        {
                            if (!WorldGen.SolidTile(num12 - 1, num13) && !WorldGen.SolidTile(num12 + 1, num13) && !WorldGen.SolidTile(num12, num13 - 1) && !WorldGen.SolidTile(num12, num13 + 1))
                            {
                                Main.tile[num12, num13].active(false);
                            }
                            else if ((Main.tile[num12, num13].halfBrick() || Main.tile[num12 - 1, num13].topSlope()) && !WorldGen.SolidTile(num12, num13 + 1))
                            {
                                Main.tile[num12, num13].active(false);
                            }
                        }
                        WorldGen.SquareTileFrame(num12, num13, true);
                        WorldGen.SquareWallFrame(num12, num13, true);
                    }
                }
            }
            num = WorldGen.genRand.Next(23, 32);
            for (int num17 = i - num; num17 < i + num; num17++)
            {
                for (int num18 = j - num; num18 < j + num; num18++)
                {
                    if (num18 > j + WorldGen.genRand.Next(-3, 4) - 3 && Main.tile[num17, num18].active() && rand.NextBool(10))
                    {
                        float num19 = (float)Math.Abs(i - num17);
                        float num20 = (float)Math.Abs(j - num18);
                        float num21 = (float)Math.Sqrt((double)(num19 * num19 + num20 * num20));
                        if ((double)num21 < (double)num * 0.8)
                        {
                            if (Main.tile[num17, num18] != null)
                            {
                                if (Main.tile[num17, num18].type == 5 || Main.tile[num17, num18].type == 32 || Main.tile[num17, num18].type == 352)
                                {
                                    WorldGen.KillTile(num17, num18, false, false, false);
                                }
                                Main.tile[num17, num18].type = (ushort)ModContent.TileType<AstralOre>();
                                WorldGen.SquareTileFrame(num17, num18, true);
                            }
                        }
                    }
                }
            }
            num = WorldGen.genRand.Next(30, 38);
            for (int num22 = i - num; num22 < i + num; num22++)
            {
                for (int num23 = j - num; num23 < j + num; num23++)
                {
                    if (num23 > j + WorldGen.genRand.Next(-2, 3) && Main.tile[num22, num23].active() && rand.NextBool(20))
                    {
                        float num24 = (float)Math.Abs(i - num22);
                        float num25 = (float)Math.Abs(j - num23);
                        float num26 = (float)Math.Sqrt((double)(num24 * num24 + num25 * num25));
                        if ((double)num26 < (double)num * 0.85)
                        {
                            if (Main.tile[num22, num23] != null)
                            {
                                if (Main.tile[num22, num23].type == 5 || Main.tile[num22, num23].type == 32 || Main.tile[num22, num23].type == 352)
                                {
                                    WorldGen.KillTile(num22, num23, false, false, false);
                                }
                                Main.tile[num22, num23].type = (ushort)ModContent.TileType<AstralOre>();
                                WorldGen.SquareTileFrame(num22, num23, true);
                            }
                        }
                    }
                }
            }
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                NetMessage.SendTileSquare(-1, i, j, 40, TileChangeType.None);
                if (CanAstralBiomeSpawn())
                    DoAstralConversion(new Point(i, j));
            }
            return true;
        }

        public static void DoAstralConversion(object obj)
        {
            //Pre-calculate all variables necessary for elliptical area checking
            Point origin = (Point)obj;
            Vector2 center = origin.ToVector2() * 16f + new Vector2(8f);

            float angle = MathHelper.Pi * 0.15f;
            float otherAngle = MathHelper.PiOver2 - angle;

            int distanceInTiles = 150 + (Main.maxTilesX - 4200) / 4200 * 200;
            float distance = distanceInTiles * 16f;
            float constant = distance * 2f / (float)Math.Sin(angle);

            float fociSpacing = distance * (float)Math.Sin(otherAngle) / (float)Math.Sin(angle);
            int verticalRadius = (int)(constant / 16f);

            Vector2 fociOffset = Vector2.UnitY * fociSpacing;
            Vector2 topFoci = center - fociOffset;
            Vector2 bottomFoci = center + fociOffset;

            UnifiedRandom rand = WorldGen.genRand;
            for (int x = origin.X - distanceInTiles - 2; x <= origin.X + distanceInTiles + 2; x++)
            {
                for (int y = (int)(origin.Y - verticalRadius * 0.4f) - 3; y <= origin.Y + verticalRadius + 3; y++)
                {
                    if (CheckInEllipse(new Point(x, y), topFoci, bottomFoci, constant, center, out float dist, y < origin.Y))
                    {
                        //If we're in the outer blurPercent% of the ellipse
                        float percent = dist / constant;
                        float blurPercent = 0.98f;
                        if (percent > blurPercent)
                        {
                            float outerEdgePercent = (percent - blurPercent) / (1f - blurPercent);
                            if (rand.NextFloat(1f) > outerEdgePercent)
                            {
                                ConvertToAstral(x, y);
                            }
                        }
                        else
                        {
                            ConvertToAstral(x, y);
                        }
                    }
                }
            }
        }

        public static void ConvertToAstral(int startX, int endX, int startY, int endY)
        {
            for (int x = startX; x <= endX; x++)
            {
                for (int y = startY; y <= endY; y++)
                {
                    ConvertToAstral(x, y);
                }
            }
        }

        public static void ConvertToAstral(int x, int y, bool tileframe = true)
        {
            Mod mod = ModLoader.GetMod("CalamityMod");
            if (WorldGen.InWorld(x, y, 1))
            {
                int type = Main.tile[x, y].type;
                int wallType = Main.tile[x, y].wall;

                if (Main.tile[x, y] != null)
                {
                    if (WallID.Sets.Conversion.Grass[wallType])
                    {
                        Main.tile[x, y].wall = (ushort)ModContent.WallType<AstralGrassWallUnsafe>();
                    }
                    else if (WallID.Sets.Conversion.HardenedSand[wallType])
                    {
                        Main.tile[x, y].wall = (ushort)ModContent.WallType<HardenedAstralSandWallUnsafe>();
                    }
                    else if (WallID.Sets.Conversion.Sandstone[wallType])
                    {
                        Main.tile[x, y].wall = (ushort)ModContent.WallType<AstralSandstoneWallUnsafe>();
                    }
                    else if (WallID.Sets.Conversion.Stone[wallType])
                    {
                        Main.tile[x, y].wall = (ushort)ModContent.WallType<AstralStoneWallUnsafe>();
                    }
                    else
                    {
                        switch (wallType)
                        {
                            case WallID.DirtUnsafe:
                            case WallID.DirtUnsafe1:
                            case WallID.DirtUnsafe2:
                            case WallID.DirtUnsafe3:
                            case WallID.DirtUnsafe4:
                            case WallID.Cave6Unsafe:
                            case WallID.Dirt:
                                Main.tile[x, y].wall = (ushort)ModContent.WallType<AstralDirtWallUnsafe>();
                                break;
                            case WallID.IceUnsafe:
                                Main.tile[x, y].wall = (ushort)ModContent.WallType<AstralIceWallUnsafe>();
                                break;
                        }
                    }
                    if (TileID.Sets.Conversion.Grass[type] && !TileID.Sets.GrassSpecial[type])
                    {
                        Main.tile[x, y].type = (ushort)ModContent.TileType<AstralGrass>();
                    }
                    else if (TileID.Sets.Conversion.Stone[type] || Main.tileMoss[type])
                    {
                        Main.tile[x, y].type = (ushort)ModContent.TileType<AstralStone>();
                    }
                    else if (TileID.Sets.Conversion.Sand[type])
                    {
                        Main.tile[x, y].type = (ushort)ModContent.TileType<AstralSand>();
                    }
                    else if (TileID.Sets.Conversion.HardenedSand[type])
                    {
                        Main.tile[x, y].type = (ushort)ModContent.TileType<HardenedAstralSand>();
                    }
                    else if (TileID.Sets.Conversion.Sandstone[type])
                    {
                        Main.tile[x, y].type = (ushort)ModContent.TileType<AstralSandstone>();
                    }
                    else if (TileID.Sets.Conversion.Ice[type])
                    {
                        Main.tile[x, y].type = (ushort)ModContent.TileType<AstralIce>();
                    }
                    else
                    {
                        Tile tile = Main.tile[x, y];
                        switch (type)
                        {
                            case TileID.Dirt:
                                Main.tile[x, y].type = (ushort)ModContent.TileType<AstralDirt>();
                                break;
                            case TileID.Vines:
                                Main.tile[x, y].type = (ushort)ModContent.TileType<AstralVines>();
                                break;
                            case TileID.LargePiles:
                                if (tile.frameX <= 1170)
                                {
                                    RecursiveReplaceToAstral(TileID.LargePiles, (ushort)ModContent.TileType<AstralNormalLargePiles>(), x, y, 324, 0, 1170, 0, 18);
                                }
                                if (tile.frameX >= 1728)
                                {
                                    RecursiveReplaceToAstral(TileID.LargePiles, (ushort)ModContent.TileType<AstralNormalLargePiles>(), x, y, 324, 1728, 1872, 0, 18);
                                }
                                if (tile.frameX >= 1404 && tile.frameX <= 1710)
                                {
                                    RecursiveReplaceToAstral(TileID.LargePiles, (ushort)ModContent.TileType<AstralIceLargePiles>(), x, y, 324, 1404, 1710, 0, 18);
                                }
                                break;
                            case TileID.LargePiles2:
                                if (tile.frameX >= 1566 && tile.frameY < 36)
                                {
                                    RecursiveReplaceToAstral(TileID.LargePiles2, (ushort)ModContent.TileType<AstralDesertLargePiles>(), x, y, 324, 1566, 1872, 0, 18);
                                }
                                if (tile.frameX >= 756 && tile.frameX <= 900)
                                {
                                    RecursiveReplaceToAstral(TileID.LargePiles2, (ushort)ModContent.TileType<AstralNormalLargePiles>(), x, y, 324, 756, 900, 0, 18);
                                }
                                break;
                            case TileID.SmallPiles:
                                if (tile.frameY == 18)
                                {
                                    ushort newType;
                                    if (tile.frameX >= 1476 && tile.frameX <= 1674)
                                    {
                                        newType = (ushort)ModContent.TileType<AstralDesertMediumPiles>();
                                    }
                                    else if (tile.frameX <= 558 || (tile.frameX >= 1368 && tile.frameX <= 1458))
                                    {
                                        newType = (ushort)ModContent.TileType<AstralNormalMediumPiles>();
                                    }
                                    else if (tile.frameX >= 900 && tile.frameX <= 1098)
                                    {
                                        newType = (ushort)ModContent.TileType<AstralIceMediumPiles>();
                                    }
                                    else
                                    {
                                        break;
                                    }
                                    int leftMost = x;
                                    if (tile.frameX % 36 != 0) //this means it's the right tile of the two
                                    {
                                        leftMost--;
                                    }
                                    if (Main.tile[leftMost, y] != null)
                                        Main.tile[leftMost, y].type = newType;
                                    if (Main.tile[leftMost + 1, y] != null)
                                        Main.tile[leftMost + 1, y].type = newType;
                                    while (Main.tile[leftMost, y].frameX >= 216)
                                    {
                                        if (Main.tile[leftMost, y] != null)
                                            Main.tile[leftMost, y].frameX -= 216;
                                        if (Main.tile[leftMost + 1, y] != null)
                                            Main.tile[leftMost + 1, y].frameX -= 216;
                                    }
                                }
                                else if (tile.frameY == 0)
                                {
                                    ushort newType3;
                                    if (tile.frameX >= 972 && tile.frameX <= 1062)
                                    {
                                        newType3 = (ushort)ModContent.TileType<AstralDesertSmallPiles>();
                                    }
                                    else if (tile.frameX <= 486)
                                    {
                                        newType3 = (ushort)ModContent.TileType<AstralNormalSmallPiles>();
                                    }
                                    else if (tile.frameX >= 648 && tile.frameX <= 846)
                                    {
                                        newType3 = (ushort)ModContent.TileType<AstralIceSmallPiles>();
                                    }
                                    else
                                    {
                                        break;
                                    }
                                    Main.tile[x, y].type = newType3;
                                    while (Main.tile[x, y].frameX >= 108) //REFRAME IT
                                    {
                                        Main.tile[x, y].frameX -= 108;
                                    }
                                }
                                break;
                            case TileID.Stalactite:
                                int topMost = tile.frameY <= 54 ? (tile.frameY % 36 == 0 ? y : y - 1) : y;
                                bool twoTall = tile.frameY <= 54;
                                bool hanging = tile.frameY <= 18 || tile.frameY == 72;
                                ushort newType2;
                                if (tile.frameX >= 378 && tile.frameX <= 414) //DESERT
                                {
                                    newType2 = (ushort)ModContent.TileType<AstralDesertStalactite>();
                                }
                                else if ((tile.frameX >= 54 && tile.frameX <= 90) || (tile.frameX >= 216 && tile.frameX <= 360))
                                {
                                    newType2 = (ushort)ModContent.TileType<AstralNormalStalactite>();
                                }
                                else if (tile.frameX <= 36)
                                {
                                    newType2 = (ushort)ModContent.TileType<AstralIceStalactite>();
                                }
                                else
                                {
                                    break;
                                }

                                //Set types
                                if (Main.tile[x, topMost] != null)
                                    Main.tile[x, topMost].type = newType2;
                                if (twoTall)
                                {
                                    if (Main.tile[x, topMost + 1] != null)
                                        Main.tile[x, topMost + 1].type = newType2;
                                }

                                //Fix frames
                                while (Main.tile[x, topMost].frameX >= 54)
                                {
                                    if (Main.tile[x, topMost] != null)
                                        Main.tile[x, topMost].frameX -= 54;
                                    if (twoTall)
                                    {
                                        if (Main.tile[x, topMost + 1] != null)
                                            Main.tile[x, topMost + 1].frameX -= 54;
                                    }
                                }

                                if (hanging)
                                {
                                    ConvertToAstral(x, topMost - 1);
                                    break;
                                }
                                else
                                {
                                    if (twoTall)
                                    {
                                        ConvertToAstral(x, topMost + 2);
                                        break;
                                    }
                                    ConvertToAstral(x, topMost + 1);
                                    break;
                                }
                        }
                    }
                    if (tileframe)
                    {
                        if (Main.netMode == NetmodeID.SinglePlayer)
                        {
                            WorldGen.SquareTileFrame(x, y, true);
                        }
                        else if (Main.netMode == NetmodeID.Server)
                        {
                            NetMessage.SendTileSquare(-1, x, y, 1);
                        }
                    }
                }
            }
        }

        public static void ConvertFromAstral(int x, int y, ConvertType convert)
        {
            Tile tile = Main.tile[x, y];
            int type = tile.type;
            int wallType = tile.wall;
            Mod mod = ModContent.GetInstance<CalamityMod>();

            if (WorldGen.InWorld(x, y, 1))
            {
                #region WALL
                if (Main.tile[x, y] != null)
                {
                    if (wallType == ModContent.WallType<AstralDirtWall>())
                    {
                        Main.tile[x, y].wall = WallID.DirtUnsafe;
                    }
                    else if (wallType == ModContent.WallType<AstralGrassWall>())
                    {
                        switch (convert)
                        {
                            case ConvertType.Corrupt:
                                Main.tile[x, y].wall = WallID.CorruptGrassUnsafe;
                                break;
                            case ConvertType.Crimson:
                                Main.tile[x, y].wall = WallID.CrimsonGrassUnsafe;
                                break;
                            case ConvertType.Hallow:
                                Main.tile[x, y].wall = WallID.HallowedGrassUnsafe;
                                break;
                            case ConvertType.Pure:
                                Main.tile[x, y].wall = WallID.GrassUnsafe;
                                break;
                        }
                    }
                    else if (wallType == ModContent.WallType<AstralIceWall>())
                    {
                        Main.tile[x, y].wall = WallID.IceUnsafe;
                    }
                    else if (wallType == ModContent.WallType<AstralStoneWall>())
                    {
                        switch (convert)
                        {
                            case ConvertType.Corrupt:
                                Main.tile[x, y].wall = WallID.EbonstoneUnsafe;
                                break;
                            case ConvertType.Crimson:
                                Main.tile[x, y].wall = WallID.CrimstoneUnsafe;
                                break;
                            case ConvertType.Hallow:
                                Main.tile[x, y].wall = WallID.PearlstoneBrickUnsafe;
                                break;
                            case ConvertType.Pure:
                                Main.tile[x, y].wall = WallID.Stone;
                                break;
                        }
                    }
                }
                #endregion

                #region TILE
                if (Main.tile[x, y] != null)
                {
                    if (type == ModContent.TileType<AstralDirt>())
                    {
                        tile.type = TileID.Dirt;
                    }
                    else if (type == ModContent.TileType<AstralGrass>())
                    {
                        SetTileFromConvert(x, y, convert, TileID.CorruptGrass, TileID.FleshGrass, TileID.HallowedGrass, TileID.Grass);
                    }
                    else if (type == ModContent.TileType<AstralStone>())
                    {
                        SetTileFromConvert(x, y, convert, TileID.Ebonstone, TileID.Crimstone, TileID.Pearlstone, TileID.Stone);
                    }
                    else if (type == ModContent.TileType<AstralSand>())
                    {
                        SetTileFromConvert(x, y, convert, TileID.Ebonsand, TileID.Crimsand, TileID.Pearlsand, TileID.Sand);
                    }
                    else if (type == ModContent.TileType<AstralSandstone>())
                    {
                        SetTileFromConvert(x, y, convert, TileID.CorruptSandstone, TileID.CrimsonSandstone, TileID.HallowSandstone, TileID.Sandstone);
                    }
                    else if (type == ModContent.TileType<HardenedAstralSand>())
                    {
                        SetTileFromConvert(x, y, convert, TileID.CorruptHardenedSand, TileID.CrimsonHardenedSand, TileID.HallowHardenedSand, TileID.HardenedSand);
                    }
                    else if (type == ModContent.TileType<AstralIce>())
                    {
                        SetTileFromConvert(x, y, convert, TileID.CorruptIce, TileID.FleshIce, TileID.HallowedIce, TileID.IceBlock);
                    }
                    else if (type == ModContent.TileType<AstralVines>())
                    {
                        SetTileFromConvert(x, y, convert, ushort.MaxValue, TileID.CrimsonVines, TileID.HallowedVines, TileID.Vines);
                    }
                    else if (type == ModContent.TileType<AstralShortPlants>())
                    {
                        SetTileFromConvert(x, y, convert, TileID.CorruptPlants, ushort.MaxValue, TileID.HallowedPlants, TileID.Plants);
                    }
                    else if (type == ModContent.TileType<AstralTallPlants>())
                    {
                        SetTileFromConvert(x, y, convert, ushort.MaxValue, ushort.MaxValue, TileID.HallowedPlants2, TileID.Plants2);
                    }
                    else if (type == ModContent.TileType<AstralNormalLargePiles>())
                    {
                        RecursiveReplaceFromAstral((ushort)type, TileID.LargePiles, x, y, 378, 0);
                    }
                    else if (type == ModContent.TileType<AstralNormalMediumPiles>())
                    {
                        RecursiveReplaceFromAstral((ushort)type, TileID.SmallPiles, x, y, 0, 18);
                    }
                    else if (type == ModContent.TileType<AstralNormalSmallPiles>())
                    {
                        RecursiveReplaceFromAstral((ushort)type, TileID.SmallPiles, x, y, 0, 0);
                    }
                    else if (type == ModContent.TileType<AstralDesertLargePiles>())
                    {
                        RecursiveReplaceFromAstral((ushort)type, TileID.LargePiles2, x, y, 1566, 0);
                    }
                    else if (type == ModContent.TileType<AstralDesertMediumPiles>())
                    {
                        RecursiveReplaceFromAstral((ushort)type, TileID.SmallPiles, x, y, 1476, 18);
                    }
                    else if (type == ModContent.TileType<AstralDesertSmallPiles>())
                    {
                        RecursiveReplaceFromAstral((ushort)type, TileID.SmallPiles, x, y, 972, 0);
                    }
                    else if (type == ModContent.TileType<AstralIceLargePiles>())
                    {
                        RecursiveReplaceFromAstral((ushort)type, TileID.LargePiles, x, y, 1404, 0);
                    }
                    else if (type == ModContent.TileType<AstralIceMediumPiles>())
                    {
                        RecursiveReplaceFromAstral((ushort)type, TileID.SmallPiles, x, y, 900, 18);
                    }
                    else if (type == ModContent.TileType<AstralIceSmallPiles>())
                    {
                        RecursiveReplaceFromAstral((ushort)type, TileID.SmallPiles, x, y, 648, 0);
                    }
                    else if (type == ModContent.TileType<AstralNormalStalactite>())
                    {
                        ushort originType = TileID.Stone;
                        int frameXAdd = 54;
                        switch (convert)
                        {
                            case ConvertType.Corrupt:
                                originType = TileID.Ebonstone;
                                frameXAdd = 324;
                                break;
                            case ConvertType.Crimson:
                                originType = TileID.Crimstone;
                                frameXAdd = 270;
                                break;
                            case ConvertType.Hallow:
                                originType = TileID.Pearlstone;
                                frameXAdd = 216;
                                break;
                        }
                        ReplaceAstralStalactite((ushort)type, TileID.Stalactite, originType, x, y, frameXAdd, 0);
                    }
                    else if (type == ModContent.TileType<AstralDesertStalactite>())
                    {
                        ushort originType = TileID.Sandstone;
                        int frameXAdd = 378;
                        switch (convert)
                        {
                            case ConvertType.Corrupt:
                                originType = TileID.CorruptSandstone;
                                frameXAdd = 324;
                                break;
                            case ConvertType.Crimson:
                                originType = TileID.CrimsonSandstone;
                                frameXAdd = 270;
                                break;
                            case ConvertType.Hallow:
                                originType = TileID.HallowSandstone;
                                frameXAdd = 216;
                                break;
                        }
                        ReplaceAstralStalactite((ushort)type, TileID.Stalactite, originType, x, y, frameXAdd, 0);
                    }
                    else if (type == ModContent.TileType<AstralIceStalactite>())
                    {
                        ReplaceAstralStalactite((ushort)type, TileID.Stalactite, TileID.IceBlock, x, y, 0, 0);
                    }
                    if (TileID.Sets.Conversion.Grass[type] || type == TileID.Dirt)
                    {
                        WorldGen.SquareTileFrame(x, y);
                    }
                }
                #endregion
            }
        }

        public static void SetTileFromConvert(int x, int y, ConvertType convert, ushort corrupt, ushort crimson, ushort hallow, ushort pure)
        {
            switch (convert)
            {
                case ConvertType.Corrupt:
                    if (corrupt != ushort.MaxValue)
                    {
                        Main.tile[x, y].type = corrupt;
                        WorldGen.SquareTileFrame(x, y);
                    }
                    break;
                case ConvertType.Crimson:
                    if (crimson != ushort.MaxValue)
                    {
                        Main.tile[x, y].type = crimson;
                        WorldGen.SquareTileFrame(x, y);
                    }
                    break;
                case ConvertType.Hallow:
                    if (hallow != ushort.MaxValue)
                    {
                        Main.tile[x, y].type = hallow;
                        WorldGen.SquareTileFrame(x, y);
                    }
                    break;
                case ConvertType.Pure:
                    if (pure != ushort.MaxValue)
                    {
                        Main.tile[x, y].type = pure;
                        WorldGen.SquareTileFrame(x, y);
                    }
                    break;
            }
        }

        public static void RecursiveReplaceToAstral(ushort checkType, ushort replaceType, int x, int y, int replaceTextureWidth, int minFrameX = 0, int maxFrameX = int.MaxValue, int minFrameY = 0, int maxFrameY = int.MaxValue)
        {
            Tile tile = Main.tile[x, y];
            if (tile == null || !tile.active() || tile.type != checkType || tile.frameX < minFrameX || tile.frameX > maxFrameX || tile.frameY < minFrameY || tile.frameY > maxFrameY)
                return;

            Main.tile[x, y].type = replaceType;
            while (Main.tile[x, y].frameX >= replaceTextureWidth)
            {
                Main.tile[x, y].frameX -= (short)replaceTextureWidth;
            }

            if (Main.tile[x - 1, y] != null)
                RecursiveReplaceToAstral(checkType, replaceType, x - 1, y, replaceTextureWidth, minFrameX, maxFrameX, minFrameY, maxFrameY);
            if (Main.tile[x + 1, y] != null)
                RecursiveReplaceToAstral(checkType, replaceType, x + 1, y, replaceTextureWidth, minFrameX, maxFrameX, minFrameY, maxFrameY);
            if (Main.tile[x, y - 1] != null)
                RecursiveReplaceToAstral(checkType, replaceType, x, y - 1, replaceTextureWidth, minFrameX, maxFrameX, minFrameY, maxFrameY);
            if (Main.tile[x, y + 1] != null)
                RecursiveReplaceToAstral(checkType, replaceType, x, y + 1, replaceTextureWidth, minFrameX, maxFrameX, minFrameY, maxFrameY);
        }

        public static void RecursiveReplaceFromAstral(ushort checkType, ushort replaceType, int x, int y, int addFrameX, int addFrameY)
        {
            Tile tile = Main.tile[x, y];
            if (tile == null || !tile.active() || tile.type != checkType)
                return;

            Main.tile[x, y].type = replaceType;
            Main.tile[x, y].frameX += (short)addFrameX;
            Main.tile[x, y].frameY += (short)addFrameY;

            if (Main.tile[x - 1, y] != null)
                RecursiveReplaceFromAstral(checkType, replaceType, x - 1, y, addFrameX, addFrameY);
            if (Main.tile[x + 1, y] != null)
                RecursiveReplaceFromAstral(checkType, replaceType, x + 1, y, addFrameX, addFrameY);
            if (Main.tile[x, y - 1] != null)
                RecursiveReplaceFromAstral(checkType, replaceType, x, y - 1, addFrameX, addFrameY);
            if (Main.tile[x, y + 1] != null)
                RecursiveReplaceFromAstral(checkType, replaceType, x, y + 1, addFrameX, addFrameY);
        }

        public static void ReplaceAstralStalactite(ushort checkType, ushort replaceType, ushort replaceOriginTile, int x, int y, int addFrameX, int addFrameY)
        {
            Tile tile = Main.tile[x, y];

            int topMost = tile.frameY <= 54 ? (tile.frameY % 36 == 0 ? y : y - 1) : y;
            bool twoTall = tile.frameY <= 54;
            bool hanging = tile.frameY <= 18 || tile.frameY == 72;

            int yOriginTile = hanging ? topMost - 1 : (twoTall ? topMost + 2 : y + 1);

            if (Main.tile[x, topMost++] != null)
                Main.tile[x, topMost++].type = replaceType;
            if (twoTall)
            {
                if (Main.tile[x, topMost] != null)
                    Main.tile[x, topMost].type = replaceType;
            }
            if (Main.tile[x, yOriginTile] != null)
                Main.tile[x, yOriginTile].type = replaceOriginTile;
        }

        public static bool CheckInEllipse(Point tile, Vector2 focus1, Vector2 focus2, float distanceConstant, Vector2 center, out float distance, bool collapse = false)
        {
            Vector2 point = tile.ToVector2() * 16f + new Vector2(8f);
            if (collapse) //Collapse ensures the ellipse is shrunk down a lot in terms of distance.
            {
                float distY = center.Y - point.Y;
                point.Y -= distY * 8f;
            }
            float distance1 = Vector2.Distance(point, focus1);
            float distance2 = Vector2.Distance(point, focus2);
            distance = distance1 + distance2;
            return distance <= distanceConstant;
        }
        #endregion

        #region EvilIsland
        public static void EvilIsland(int i, int j)
        {
            double num = (double)WorldGen.genRand.Next(100, 150); //100 150
            float num2 = (float)WorldGen.genRand.Next(20, 30); //20 30
            int num3 = i;
            int num4 = i;
            int num5 = i;
            int num6 = j;
            Vector2 vector;
            vector.X = (float)i;
            vector.Y = (float)j;
            Vector2 vector2;
            vector2.X = (float)WorldGen.genRand.Next(-20, 21) * 0.2f;
            while (vector2.X > -2f && vector2.X < 2f)
            {
                vector2.X = (float)WorldGen.genRand.Next(-20, 21) * 0.2f;
            }
            vector2.Y = (float)WorldGen.genRand.Next(-20, -10) * 0.02f;
            while (num > 0.0 && num2 > 0f)
            {
                num -= (double)WorldGen.genRand.Next(4);
                num2 -= 1f;
                int num7 = (int)((double)vector.X - num * 0.5);
                int num8 = (int)((double)vector.X + num * 0.5);
                int num9 = (int)((double)vector.Y - num * 0.5);
                int num10 = (int)((double)vector.Y + num * 0.5);
                if (num7 < 0)
                {
                    num7 = 0;
                }
                if (num8 > Main.maxTilesX)
                {
                    num8 = Main.maxTilesX;
                }
                if (num9 < 0)
                {
                    num9 = 0;
                }
                if (num10 > Main.maxTilesY)
                {
                    num10 = Main.maxTilesY;
                }
                double num11 = num * (double)WorldGen.genRand.Next(80, 120) * 0.01; //80 120
                float num12 = vector.Y + 1f;
                for (int k = num7; k < num8; k++)
                {
                    if (WorldGen.genRand.Next(2) == 0)
                    {
                        num12 += (float)WorldGen.genRand.Next(-1, 2);
                    }
                    if (num12 < vector.Y)
                    {
                        num12 = vector.Y;
                    }
                    if (num12 > vector.Y + 2f)
                    {
                        num12 = vector.Y + 2f;
                    }
                    for (int l = num9; l < num10; l++)
                    {
                        if ((float)l > num12)
                        {
                            float arg_218_0 = Math.Abs((float)k - vector.X);
                            float num13 = Math.Abs((float)l - vector.Y) * 3f;
                            if (Math.Sqrt((double)(arg_218_0 * arg_218_0 + num13 * num13)) < num11 * 0.4)
                            {
                                if (k < num3)
                                {
                                    num3 = k;
                                }
                                if (k > num4)
                                {
                                    num4 = k;
                                }
                                if (l < num5)
                                {
                                    num5 = l;
                                }
                                if (l > num6)
                                {
                                    num6 = l;
                                }
                                Main.tile[k, l].active(true);
                                Main.tile[k, l].type = (ushort)(WorldGen.crimson ? 400 : 401); //ebonstone or crimstone
                                WorldGen.SquareTileFrame(k, l, true);
                            }
                        }
                    }
                }
                vector += vector2;
                vector2.X += (float)WorldGen.genRand.Next(-20, 21) * 0.05f;
                if (vector2.X > 1f)
                {
                    vector2.X = 1f;
                }
                if (vector2.X < -1f)
                {
                    vector2.X = -1f;
                }
                if ((double)vector2.Y > 0.2)
                {
                    vector2.Y = -0.2f;
                }
                if ((double)vector2.Y < -0.2)
                {
                    vector2.Y = -0.2f;
                }
            }
            int m = num3;
            int num15;
            for (m += WorldGen.genRand.Next(5); m < num4; m += WorldGen.genRand.Next(num15, (int)((double)num15 * 1.5)))
            {
                int num14 = num6;
                while (!Main.tile[m, num14].active())
                {
                    num14--;
                }
                num14 += WorldGen.genRand.Next(-3, 4);
                num15 = WorldGen.genRand.Next(4, 8);
                int num16 = WorldGen.crimson ? 400 : 401;
                if (WorldGen.genRand.Next(4) == 0)
                {
                    num16 = WorldGen.crimson ? 398 : 399;
                }
                for (int n = m - num15; n <= m + num15; n++)
                {
                    for (int num17 = num14 - num15; num17 <= num14 + num15; num17++)
                    {
                        if (num17 > num5)
                        {
                            float arg_409_0 = (float)Math.Abs(n - m);
                            float num18 = (float)(Math.Abs(num17 - num14) * 2);
                            if (Math.Sqrt((double)(arg_409_0 * arg_409_0 + num18 * num18)) < (double)(num15 + WorldGen.genRand.Next(2)))
                            {
                                Main.tile[n, num17].active(true);
                                Main.tile[n, num17].type = (ushort)num16;
                                WorldGen.SquareTileFrame(n, num17, true);
                            }
                        }
                    }
                }
            }
            num = (double)WorldGen.genRand.Next(80, 95);
            num2 = (float)WorldGen.genRand.Next(10, 15);
            vector.X = (float)i;
            vector.Y = (float)num5;
            vector2.X = (float)WorldGen.genRand.Next(-20, 21) * 0.2f;
            while (vector2.X > -2f && vector2.X < 2f)
            {
                vector2.X = (float)WorldGen.genRand.Next(-20, 21) * 0.2f;
            }
            vector2.Y = (float)WorldGen.genRand.Next(-20, -10) * 0.02f;
            while (num > 0.0 && num2 > 0f)
            {
                num -= (double)WorldGen.genRand.Next(4);
                num2 -= 1f;
                int num7 = (int)((double)vector.X - num * 0.5);
                int num8 = (int)((double)vector.X + num * 0.5);
                int num9 = num5 - 1;
                int num10 = (int)((double)vector.Y + num * 0.5);
                if (num7 < 0)
                {
                    num7 = 0;
                }
                if (num8 > Main.maxTilesX)
                {
                    num8 = Main.maxTilesX;
                }
                if (num9 < 0)
                {
                    num9 = 0;
                }
                if (num10 > Main.maxTilesY)
                {
                    num10 = Main.maxTilesY;
                }
                double num11 = num * (double)WorldGen.genRand.Next(80, 120) * 0.01;
                float num19 = vector.Y + 1f;
                for (int num20 = num7; num20 < num8; num20++)
                {
                    if (WorldGen.genRand.Next(2) == 0)
                    {
                        num19 += (float)WorldGen.genRand.Next(-1, 2);
                    }
                    if (num19 < vector.Y)
                    {
                        num19 = vector.Y;
                    }
                    if (num19 > vector.Y + 2f)
                    {
                        num19 = vector.Y + 2f;
                    }
                    for (int num21 = num9; num21 < num10; num21++)
                    {
                        if ((float)num21 > num19)
                        {
                            float arg_69E_0 = Math.Abs((float)num20 - vector.X);
                            float num22 = Math.Abs((float)num21 - vector.Y) * 3f;
                            if (Math.Sqrt((double)(arg_69E_0 * arg_69E_0 + num22 * num22)) < num11 * 0.4 &&
                                Main.tile[num20, num21].type == (WorldGen.crimson ? 400 : 401))
                            {
                                Main.tile[num20, num21].type = (ushort)(WorldGen.crimson ? 22 : 204); //ore
                                WorldGen.SquareTileFrame(num20, num21, true);
                            }
                        }
                    }
                }
                vector += vector2;
                vector2.X += (float)WorldGen.genRand.Next(-20, 21) * 0.05f;
                if (vector2.X > 1f)
                {
                    vector2.X = 1f;
                }
                if (vector2.X < -1f)
                {
                    vector2.X = -1f;
                }
                if ((double)vector2.Y > 0.2)
                {
                    vector2.Y = -0.2f;
                }
                if ((double)vector2.Y < -0.2)
                {
                    vector2.Y = -0.2f;
                }
            }
            int num23 = num3;
            num23 += WorldGen.genRand.Next(5);
            while (num23 < num4)
            {
                int num24 = num6;
                while ((!Main.tile[num23, num24].active() || Main.tile[num23, num24].type != 0) && num23 < num4)
                {
                    num24--;
                    if (num24 < num5)
                    {
                        num24 = num6;
                        num23 += WorldGen.genRand.Next(1, 4);
                    }
                }
                if (num23 < num4)
                {
                    num24 += WorldGen.genRand.Next(0, 4);
                    int num25 = WorldGen.genRand.Next(2, 5);
                    int num26 = WorldGen.crimson ? 400 : 401;
                    for (int num27 = num23 - num25; num27 <= num23 + num25; num27++)
                    {
                        for (int num28 = num24 - num25; num28 <= num24 + num25; num28++)
                        {
                            if (num28 > num5)
                            {
                                float arg_890_0 = (float)Math.Abs(num27 - num23);
                                float num29 = (float)(Math.Abs(num28 - num24) * 2);
                                if (Math.Sqrt((double)(arg_890_0 * arg_890_0 + num29 * num29)) < (double)num25)
                                {
                                    Main.tile[num27, num28].type = (ushort)num26;
                                    WorldGen.SquareTileFrame(num27, num28, true);
                                }
                            }
                        }
                    }
                    num23 += WorldGen.genRand.Next(num25, (int)((double)num25 * 1.5));
                }
            }
            for (int num30 = num3 - 20; num30 <= num4 + 20; num30++)
            {
                for (int num31 = num5 - 20; num31 <= num6 + 20; num31++)
                {
                    bool flag = true;
                    for (int num32 = num30 - 1; num32 <= num30 + 1; num32++)
                    {
                        for (int num33 = num31 - 1; num33 <= num31 + 1; num33++)
                        {
                            if (!Main.tile[num32, num33].active())
                            {
                                flag = false;
                            }
                        }
                    }
                    if (flag)
                    {
                        Main.tile[num30, num31].wall = (ushort)(WorldGen.crimson ? 220 : 221);
                        WorldGen.SquareWallFrame(num30, num31, true);
                    }
                }
            }
            for (int num34 = num3; num34 <= num4; num34++)
            {
                int num35 = num5 - 10;
                while (!Main.tile[num34, num35 + 1].active())
                {
                    num35++;
                }
                if (num35 < num6 && Main.tile[num34, num35 + 1].type == (WorldGen.crimson ? 400 : 401))
                {
                    if (WorldGen.genRand.Next(10) == 0)
                    {
                        int num36 = WorldGen.genRand.Next(1, 3);
                        for (int num37 = num34 - num36; num37 <= num34 + num36; num37++)
                        {
                            if (Main.tile[num37, num35].type == (WorldGen.crimson ? 400 : 401))
                            {
                                Main.tile[num37, num35].active(false);
                                Main.tile[num37, num35].liquid = 255;
                                Main.tile[num37, num35].lava(false);
                                WorldGen.SquareTileFrame(num34, num35, true);
                            }
                            if (Main.tile[num37, num35 + 1].type == (WorldGen.crimson ? 400 : 401))
                            {
                                Main.tile[num37, num35 + 1].active(false);
                                Main.tile[num37, num35 + 1].liquid = 255;
                                Main.tile[num37, num35 + 1].lava(false);
                                WorldGen.SquareTileFrame(num34, num35 + 1, true);
                            }
                            if (num37 > num34 - num36 && num37 < num34 + 2 && Main.tile[num37, num35 + 2].type == (WorldGen.crimson ? 400 : 401))
                            {
                                Main.tile[num37, num35 + 2].active(false);
                                Main.tile[num37, num35 + 2].liquid = 255;
                                Main.tile[num37, num35 + 2].lava(false);
                                WorldGen.SquareTileFrame(num34, num35 + 2, true);
                            }
                        }
                    }
                    if (WorldGen.genRand.Next(5) == 0)
                    {
                        Main.tile[num34, num35].liquid = 255;
                    }
                    Main.tile[num34, num35].lava(false);
                    WorldGen.SquareTileFrame(num34, num35, true);
                }
            }
            int num38 = WorldGen.genRand.Next(4);
            for (int num39 = 0; num39 <= num38; num39++)
            {
                int num40 = WorldGen.genRand.Next(num3 - 5, num4 + 5);
                int num41 = num5 - WorldGen.genRand.Next(20, 40);
                int num42 = WorldGen.genRand.Next(4, 8);
                int num43 = WorldGen.crimson ? 400 : 401;
                if (WorldGen.genRand.Next(2) == 0)
                {
                    num43 = WorldGen.crimson ? 398 : 399;
                }
                for (int num44 = num40 - num42; num44 <= num40 + num42; num44++)
                {
                    for (int num45 = num41 - num42; num45 <= num41 + num42; num45++)
                    {
                        float arg_C74_0 = (float)Math.Abs(num44 - num40);
                        float num46 = (float)(Math.Abs(num45 - num41) * 2);
                        if (Math.Sqrt((double)(arg_C74_0 * arg_C74_0 + num46 * num46)) < (double)(num42 + WorldGen.genRand.Next(-1, 2)))
                        {
                            Main.tile[num44, num45].active(true);
                            Main.tile[num44, num45].type = (ushort)num43;
                            WorldGen.SquareTileFrame(num44, num45, true);
                        }
                    }
                }
                for (int num47 = num40 - num42 + 2; num47 <= num40 + num42 - 2; num47++)
                {
                    int num48 = num41 - num42;
                    while (!Main.tile[num47, num48].active())
                    {
                        num48++;
                    }
                    Main.tile[num47, num48].active(false);
                    Main.tile[num47, num48].liquid = 255;
                    WorldGen.SquareTileFrame(num47, num48, true);
                }
            }
        }
        #endregion

        #region EvilIslandHouse
        public static void EvilIslandHouse(int i, int j)
        {
            ushort type = (ushort)(WorldGen.crimson ? 152 : 347); //tile
            byte wall = (byte)(WorldGen.crimson ? 35 : 174); //wall
            Vector2 vector = new Vector2((float)i, (float)j);
            int num = 1;
            if (WorldGen.genRand.Next(2) == 0)
            {
                num = -1;
            }
            int num2 = WorldGen.genRand.Next(7, 12);
            int num3 = WorldGen.genRand.Next(5, 7);
            vector.X = (float)(i + (num2 + 2) * num);
            for (int k = j - 15; k < j + 30; k++)
            {
                if (Main.tile[(int)vector.X, k].active())
                {
                    vector.Y = (float)(k - 1);
                    break;
                }
            }
            vector.X = (float)i;
            int num4 = (int)(vector.X - (float)num2 - 1f);
            int num5 = (int)(vector.X + (float)num2 + 1f);
            int num6 = (int)(vector.Y - (float)num3 - 1f);
            int num7 = (int)(vector.Y + 2f);
            if (num4 < 0)
            {
                num4 = 0;
            }
            if (num5 > Main.maxTilesX)
            {
                num5 = Main.maxTilesX;
            }
            if (num6 < 0)
            {
                num6 = 0;
            }
            if (num7 > Main.maxTilesY)
            {
                num7 = Main.maxTilesY;
            }
            for (int l = num4; l <= num5; l++)
            {
                for (int m = num6 - 1; m < num7 + 1; m++)
                {
                    if (m != num6 - 1 || (l != num4 && l != num5))
                    {
                        Main.tile[l, m].active(true);
                        Main.tile[l, m].liquid = 0;
                        Main.tile[l, m].type = type;
                        Main.tile[l, m].wall = 0;
                        Main.tile[l, m].halfBrick(false);
                        Main.tile[l, m].slope(0);
                    }
                }
            }
            num4 = (int)(vector.X - (float)num2);
            num5 = (int)(vector.X + (float)num2);
            num6 = (int)(vector.Y - (float)num3);
            num7 = (int)(vector.Y + 1f);
            if (num4 < 0)
            {
                num4 = 0;
            }
            if (num5 > Main.maxTilesX)
            {
                num5 = Main.maxTilesX;
            }
            if (num6 < 0)
            {
                num6 = 0;
            }
            if (num7 > Main.maxTilesY)
            {
                num7 = Main.maxTilesY;
            }
            for (int n = num4; n <= num5; n++)
            {
                for (int num8 = num6; num8 < num7; num8++)
                {
                    if ((num8 != num6 || (n != num4 && n != num5)) && Main.tile[n, num8].wall == 0)
                    {
                        Main.tile[n, num8].active(false);
                        Main.tile[n, num8].wall = wall;
                    }
                }
            }
            int num9 = i + (num2 + 1) * num;
            int num10 = (int)vector.Y;
            for (int num11 = num9 - 2; num11 <= num9 + 2; num11++)
            {
                Main.tile[num11, num10].active(false);
                Main.tile[num11, num10 - 1].active(false);
                Main.tile[num11, num10 - 2].active(false);
            }
            WorldGen.PlaceTile(num9, num10, 10, true, false, -1, WorldGen.crimson ? 1 : 10); //door
            num9 = i + (num2 + 1) * -num - num;
            for (int num12 = num6; num12 <= num7 + 1; num12++)
            {
                Main.tile[num9, num12].active(true);
                Main.tile[num9, num12].liquid = 0;
                Main.tile[num9, num12].type = type;
                Main.tile[num9, num12].wall = 0;
                Main.tile[num9, num12].halfBrick(false);
                Main.tile[num9, num12].slope(0);
            }
            int contain;
            if (WorldGen.crimson)
            {
                contain = 1571; //scourge
            }
            else
            {
                contain = 1569; //vampire
            }
            WorldGen.AddBuriedChest(i, num10 - 3, contain, false, WorldGen.crimson ? 19 : 20); //chest
            int num14 = i - num2 / 2 + 1;
            int num15 = i + num2 / 2 - 1;
            int num16 = 1;
            if (num2 > 10)
            {
                num16 = 2;
            }
            int num17 = (num6 + num7) / 2 - 1;
            for (int num18 = num14 - num16; num18 <= num14 + num16; num18++)
            {
                for (int num19 = num17 - 1; num19 <= num17 + 1; num19++)
                {
                    Main.tile[num18, num19].wall = 21; //glass
                }
            }
            for (int num20 = num15 - num16; num20 <= num15 + num16; num20++)
            {
                for (int num21 = num17 - 1; num21 <= num17 + 1; num21++)
                {
                    Main.tile[num20, num21].wall = 21; //glass
                }
            }
            int num22 = i + (num2 / 2 + 1) * -num;
            WorldGen.PlaceTile(num22, num7 - 1, 14, true, false, -1, WorldGen.crimson ? 1 : 8); //table
            WorldGen.PlaceTile(num22 - 2, num7 - 1, 15, true, false, 0, WorldGen.crimson ? 2 : 11); //chair
            Tile expr_510 = Main.tile[num22 - 2, num7 - 1];
            expr_510.frameX += 18;
            Tile expr_531 = Main.tile[num22 - 2, num7 - 2];
            expr_531.frameX += 18;
            WorldGen.PlaceTile(num22 + 2, num7 - 1, 15, true, false, 0, WorldGen.crimson ? 2 : 11); //chair
        }
        #endregion

        #region UnderworldIsland
        public static void UnderworldIsland(int i, int j, int sizeMin, int sizeMax, int sizeMin2, int sizeMax2)
        {
            Mod mod = ModLoader.GetMod("CalamityMod");
            double num = (double)WorldGen.genRand.Next(sizeMin, sizeMax); //100 150
            float num2 = (float)WorldGen.genRand.Next(sizeMin / 5, sizeMax / 5); //20 30
            int num3 = i;
            int num4 = i;
            int num5 = i;
            int num6 = j;
            Vector2 vector;
            vector.X = (float)i;
            vector.Y = (float)j;
            Vector2 vector2;
            vector2.X = (float)WorldGen.genRand.Next(-20, 21) * 0.2f;
            while (vector2.X > -2f && vector2.X < 2f)
            {
                vector2.X = (float)WorldGen.genRand.Next(-20, 21) * 0.2f;
            }
            vector2.Y = (float)WorldGen.genRand.Next(-20, -10) * 0.02f;
            while (num > 0.0 && num2 > 0f)
            {
                num -= (double)WorldGen.genRand.Next(4);
                num2 -= 1f;
                int num7 = (int)((double)vector.X - num * 0.5);
                int num8 = (int)((double)vector.X + num * 0.5);
                int num9 = (int)((double)vector.Y - num * 0.5);
                int num10 = (int)((double)vector.Y + num * 0.5);
                if (num7 < 0)
                {
                    num7 = 0;
                }
                if (num8 > Main.maxTilesX)
                {
                    num8 = Main.maxTilesX;
                }
                if (num9 < 0)
                {
                    num9 = 0;
                }
                if (num10 > Main.maxTilesY)
                {
                    num10 = Main.maxTilesY;
                }
                double num11 = num * (double)WorldGen.genRand.Next(sizeMin, sizeMax) * 0.01; //80 120
                float num12 = vector.Y + 1f;
                for (int k = num7; k < num8; k++)
                {
                    if (WorldGen.genRand.Next(2) == 0)
                    {
                        num12 += (float)WorldGen.genRand.Next(-1, 2);
                    }
                    if (num12 < vector.Y)
                    {
                        num12 = vector.Y;
                    }
                    if (num12 > vector.Y + 2f)
                    {
                        num12 = vector.Y + 2f;
                    }
                    for (int l = num9; l < num10; l++)
                    {
                        if ((float)l > num12)
                        {
                            float arg_218_0 = Math.Abs((float)k - vector.X);
                            float num13 = Math.Abs((float)l - vector.Y) * 3f;
                            if (Math.Sqrt((double)(arg_218_0 * arg_218_0 + num13 * num13)) < num11 * 0.4)
                            {
                                if (k < num3)
                                {
                                    num3 = k;
                                }
                                if (k > num4)
                                {
                                    num4 = k;
                                }
                                if (l < num5)
                                {
                                    num5 = l;
                                }
                                if (l > num6)
                                {
                                    num6 = l;
                                }
                                Main.tile[k, l].active(true);
                                Main.tile[k, l].type = (ushort)ModContent.TileType<BrimstoneSlag>();
                                WorldGen.SquareTileFrame(k, l, true);
                            }
                        }
                    }
                }
                vector += vector2;
                vector2.X += (float)WorldGen.genRand.Next(-20, 21) * 0.05f;
                if (vector2.X > 1f)
                {
                    vector2.X = 1f;
                }
                if (vector2.X < -1f)
                {
                    vector2.X = -1f;
                }
                if ((double)vector2.Y > 0.2)
                {
                    vector2.Y = -0.2f;
                }
                if ((double)vector2.Y < -0.2)
                {
                    vector2.Y = -0.2f;
                }
            }
            int m = num3;
            int num15;
            for (m += WorldGen.genRand.Next(5); m < num4; m += WorldGen.genRand.Next(num15, (int)((double)num15 * 1.5)))
            {
                int num14 = num6;
                while (!Main.tile[m, num14].active())
                {
                    num14--;
                }
                num14 += WorldGen.genRand.Next(-3, 4);
                num15 = WorldGen.genRand.Next(4, 8);
                int num16 = ModContent.TileType<BrimstoneSlag>();
                if (WorldGen.genRand.Next(4) == 0)
                {
                    num16 = ModContent.TileType<CharredOre>();
                }
                for (int n = m - num15; n <= m + num15; n++)
                {
                    for (int num17 = num14 - num15; num17 <= num14 + num15; num17++)
                    {
                        if (num17 > num5)
                        {
                            float arg_409_0 = (float)Math.Abs(n - m);
                            float num18 = (float)(Math.Abs(num17 - num14) * 2);
                            if (Math.Sqrt((double)(arg_409_0 * arg_409_0 + num18 * num18)) < (double)(num15 + WorldGen.genRand.Next(2)))
                            {
                                Main.tile[n, num17].active(true);
                                Main.tile[n, num17].type = (ushort)num16;
                                WorldGen.SquareTileFrame(n, num17, true);
                            }
                        }
                    }
                }
            }
            num = (double)WorldGen.genRand.Next(sizeMin2, sizeMax2);
            num2 = (float)WorldGen.genRand.Next(sizeMin2 / 8, sizeMax2 / 8);
            vector.X = (float)i;
            vector.Y = (float)num5;
            vector2.X = (float)WorldGen.genRand.Next(-20, 21) * 0.2f;
            while (vector2.X > -2f && vector2.X < 2f)
            {
                vector2.X = (float)WorldGen.genRand.Next(-20, 21) * 0.2f;
            }
            vector2.Y = (float)WorldGen.genRand.Next(-20, -10) * 0.02f;
            while (num > 0.0 && num2 > 0f)
            {
                num -= (double)WorldGen.genRand.Next(4);
                num2 -= 1f;
                vector += vector2;
                vector2.X += (float)WorldGen.genRand.Next(-20, 21) * 0.05f;
                if (vector2.X > 1f)
                {
                    vector2.X = 1f;
                }
                if (vector2.X < -1f)
                {
                    vector2.X = -1f;
                }
                if ((double)vector2.Y > 0.2)
                {
                    vector2.Y = -0.2f;
                }
                if ((double)vector2.Y < -0.2)
                {
                    vector2.Y = -0.2f;
                }
            }
            int num23 = num3;
            num23 += WorldGen.genRand.Next(5);
            while (num23 < num4)
            {
                int num24 = num6;
                while ((!Main.tile[num23, num24].active() || Main.tile[num23, num24].type != 0) && num23 < num4)
                {
                    num24--;
                    if (num24 < num5)
                    {
                        num24 = num6;
                        num23 += WorldGen.genRand.Next(1, 4);
                    }
                }
                if (num23 < num4)
                {
                    num24 += WorldGen.genRand.Next(0, 4);
                    int num25 = WorldGen.genRand.Next(2, 5);
                    int num26 = ModContent.TileType<BrimstoneSlag>();
                    for (int num27 = num23 - num25; num27 <= num23 + num25; num27++)
                    {
                        for (int num28 = num24 - num25; num28 <= num24 + num25; num28++)
                        {
                            if (num28 > num5)
                            {
                                float arg_890_0 = (float)Math.Abs(num27 - num23);
                                float num29 = (float)(Math.Abs(num28 - num24) * 2);
                                if (Math.Sqrt((double)(arg_890_0 * arg_890_0 + num29 * num29)) < (double)num25)
                                {
                                    Main.tile[num27, num28].type = (ushort)num26;
                                    WorldGen.SquareTileFrame(num27, num28, true);
                                }
                            }
                        }
                    }
                    num23 += WorldGen.genRand.Next(num25, (int)((double)num25 * 1.5));
                }
            }
            for (int num34 = num3; num34 <= num4; num34++)
            {
                int num35 = num5 - 10;
                while (!Main.tile[num34, num35 + 1].active())
                {
                    num35++;
                }
                if (num35 < num6 && Main.tile[num34, num35 + 1].type == (ushort)ModContent.TileType<BrimstoneSlag>())
                {
                    if (WorldGen.genRand.Next(10) == 0)
                    {
                        int num36 = WorldGen.genRand.Next(1, 3);
                        for (int num37 = num34 - num36; num37 <= num34 + num36; num37++)
                        {
                            if (Main.tile[num37, num35].type == (ushort)ModContent.TileType<BrimstoneSlag>())
                            {
                                Main.tile[num37, num35].active(false);
                                Main.tile[num37, num35].liquid = 255;
                                Main.tile[num37, num35].lava(false);
                                WorldGen.SquareTileFrame(num34, num35, true);
                            }
                            if (Main.tile[num37, num35 + 1].type == (ushort)ModContent.TileType<BrimstoneSlag>())
                            {
                                Main.tile[num37, num35 + 1].active(false);
                                Main.tile[num37, num35 + 1].liquid = 255;
                                Main.tile[num37, num35 + 1].lava(false);
                                WorldGen.SquareTileFrame(num34, num35 + 1, true);
                            }
                            if (num37 > num34 - num36 && num37 < num34 + 2 && Main.tile[num37, num35 + 2].type == (ushort)ModContent.TileType<BrimstoneSlag>())
                            {
                                Main.tile[num37, num35 + 2].active(false);
                                Main.tile[num37, num35 + 2].liquid = 255;
                                Main.tile[num37, num35 + 2].lava(false);
                                WorldGen.SquareTileFrame(num34, num35 + 2, true);
                            }
                        }
                    }
                    if (WorldGen.genRand.Next(5) == 0)
                    {
                        Main.tile[num34, num35].liquid = 255;
                    }
                    Main.tile[num34, num35].lava(false);
                    WorldGen.SquareTileFrame(num34, num35, true);
                }
            }
        }
        #endregion

        #region UnderworldIslandHouse
        public static void UnderworldIslandHouse(int i, int j, int item)
        {
            Mod mod = ModLoader.GetMod("CalamityMod");
            ushort type = (ushort)ModContent.TileType<BrimstoneSlag>(); //tile
            byte wall = (byte)14; //wall
            Vector2 vector = new Vector2((float)i, (float)j);
            int num = 1;
            if (WorldGen.genRand.Next(2) == 0)
            {
                num = -1;
            }
            int num2 = WorldGen.genRand.Next(7, 12);
            int num3 = WorldGen.genRand.Next(5, 7);
            vector.X = (float)(i + (num2 + 2) * num);
            for (int k = j - 15; k < j + 30; k++)
            {
                if (Main.tile[(int)vector.X, k].active())
                {
                    vector.Y = (float)(k - 1);
                    break;
                }
            }
            vector.X = (float)i;
            int num4 = (int)(vector.X - (float)num2 - 1f);
            int num5 = (int)(vector.X + (float)num2 + 1f);
            int num6 = (int)(vector.Y - (float)num3 - 1f);
            int num7 = (int)(vector.Y + 2f);
            if (num4 < 0)
            {
                num4 = 0;
            }
            if (num5 > Main.maxTilesX)
            {
                num5 = Main.maxTilesX;
            }
            if (num6 < 0)
            {
                num6 = 0;
            }
            if (num7 > Main.maxTilesY)
            {
                num7 = Main.maxTilesY;
            }
            for (int l = num4; l <= num5; l++)
            {
                for (int m = num6 - 1; m < num7 + 1; m++)
                {
                    if (m != num6 - 1 || (l != num4 && l != num5))
                    {
                        Main.tile[l, m].active(true);
                        Main.tile[l, m].liquid = 0;
                        Main.tile[l, m].type = type;
                        Main.tile[l, m].wall = 0;
                        Main.tile[l, m].halfBrick(false);
                        Main.tile[l, m].slope(0);
                    }
                }
            }
            num4 = (int)(vector.X - (float)num2);
            num5 = (int)(vector.X + (float)num2);
            num6 = (int)(vector.Y - (float)num3);
            num7 = (int)(vector.Y + 1f);
            if (num4 < 0)
            {
                num4 = 0;
            }
            if (num5 > Main.maxTilesX)
            {
                num5 = Main.maxTilesX;
            }
            if (num6 < 0)
            {
                num6 = 0;
            }
            if (num7 > Main.maxTilesY)
            {
                num7 = Main.maxTilesY;
            }
            for (int n = num4; n <= num5; n++)
            {
                for (int num8 = num6; num8 < num7; num8++)
                {
                    if ((num8 != num6 || (n != num4 && n != num5)) && Main.tile[n, num8].wall == 0)
                    {
                        Main.tile[n, num8].active(false);
                        Main.tile[n, num8].wall = wall;
                    }
                }
            }
            int num9 = i + (num2 + 1) * num;
            int num10 = (int)vector.Y;
            for (int num11 = num9 - 2; num11 <= num9 + 2; num11++)
            {
                Main.tile[num11, num10].active(false);
                Main.tile[num11, num10 - 1].active(false);
                Main.tile[num11, num10 - 2].active(false);
            }
            WorldGen.PlaceTile(num9, num10, 10, true, false, -1, 19); //door
            num9 = i + (num2 + 1) * -num - num;
            for (int num12 = num6; num12 <= num7 + 1; num12++)
            {
                Main.tile[num9, num12].active(true);
                Main.tile[num9, num12].liquid = 0;
                Main.tile[num9, num12].type = type;
                Main.tile[num9, num12].wall = 0;
                Main.tile[num9, num12].halfBrick(false);
                Main.tile[num9, num12].slope(0);
            }
            WorldGen.AddBuriedChest(i, num10 - 3, item, false, 4); //chest
            int num22 = i + (num2 / 2 + 1) * -num;
            WorldGen.PlaceTile(num22, num7 - 1, 14, true, false, -1, 13); //table
            WorldGen.PlaceTile(num22 - 2, num7 - 1, 15, true, false, 0, 16); //chair
            Tile expr_510 = Main.tile[num22 - 2, num7 - 1];
            expr_510.frameX += 18;
            Tile expr_531 = Main.tile[num22 - 2, num7 - 2];
            expr_531.frameX += 18;
            WorldGen.PlaceTile(num22 + 2, num7 - 1, 15, true, false, 0, 16); //chair
        }
        #endregion

        #region AbyssIsland
        public static void AbyssIsland(int i, int j, int sizeMin, int sizeMax, int sizeMin2, int sizeMax2, bool hasChest, bool hasTenebris, bool isVoid)
        {
            Mod mod = ModLoader.GetMod("CalamityMod");
            int sizeMinSmall = sizeMin / 5;
            int sizeMaxSmall = sizeMax / 5;
            double num = (double)WorldGen.genRand.Next(sizeMin, sizeMax); //100 150
            float num2 = (float)WorldGen.genRand.Next(sizeMinSmall, sizeMaxSmall); //20 30
            int num3 = i;
            int num4 = i;
            int num5 = i;
            int num6 = j;
            Vector2 vector;
            vector.X = (float)i;
            vector.Y = (float)j;
            Vector2 vector2;
            vector2.X = (float)WorldGen.genRand.Next(-20, 21) * 0.2f;
            while (vector2.X > -2f && vector2.X < 2f)
            {
                vector2.X = (float)WorldGen.genRand.Next(-20, 21) * 0.2f;
            }
            vector2.Y = (float)WorldGen.genRand.Next(-20, -10) * 0.02f;
            while (num > 0.0 && num2 > 0f)
            {
                num -= (double)WorldGen.genRand.Next(4);
                num2 -= 1f;
                int num7 = (int)((double)vector.X - num * 0.5);
                int num8 = (int)((double)vector.X + num * 0.5);
                int num9 = (int)((double)vector.Y - num * 0.5);
                int num10 = (int)((double)vector.Y + num * 0.5);
                if (num7 < 0)
                {
                    num7 = 0;
                }
                if (num8 > Main.maxTilesX)
                {
                    num8 = Main.maxTilesX;
                }
                if (num9 < 0)
                {
                    num9 = 0;
                }
                if (num10 > Main.maxTilesY)
                {
                    num10 = Main.maxTilesY;
                }
                double num11 = num * (double)WorldGen.genRand.Next(sizeMin, sizeMax) * 0.01; //80 120
                float num12 = vector.Y + 1f;
                for (int k = num7; k < num8; k++)
                {
                    if (WorldGen.genRand.Next(2) == 0)
                    {
                        num12 += (float)WorldGen.genRand.Next(-1, 2);
                    }
                    if (num12 < vector.Y)
                    {
                        num12 = vector.Y;
                    }
                    if (num12 > vector.Y + 2f)
                    {
                        num12 = vector.Y + 2f;
                    }
                    for (int l = num9; l < num10; l++)
                    {
                        if ((float)l > num12)
                        {
                            float arg_218_0 = Math.Abs((float)k - vector.X);
                            float num13 = Math.Abs((float)l - vector.Y) * 3f;
                            if (Math.Sqrt((double)(arg_218_0 * arg_218_0 + num13 * num13)) < num11 * 0.4)
                            {
                                if (k < num3)
                                {
                                    num3 = k;
                                }
                                if (k > num4)
                                {
                                    num4 = k;
                                }
                                if (l < num5)
                                {
                                    num5 = l;
                                }
                                if (l > num6)
                                {
                                    num6 = l;
                                }
                                Main.tile[k, l].active(true);
                                Main.tile[k, l].type = (ushort)(isVoid ? ModContent.TileType<Voidstone>() : ModContent.TileType<AbyssGravel>());
                                SafeSquareTileFrame(k, l, true);
                            }
                        }
                    }
                }
                vector += vector2;
                vector2.X += (float)WorldGen.genRand.Next(-20, 21) * 0.05f;
                if (vector2.X > 1f)
                {
                    vector2.X = 1f;
                }
                if (vector2.X < -1f)
                {
                    vector2.X = -1f;
                }
                if ((double)vector2.Y > 0.2)
                {
                    vector2.Y = -0.2f;
                }
                if ((double)vector2.Y < -0.2)
                {
                    vector2.Y = -0.2f;
                }
            }
            int m = num3;
            int num15;
            for (m += WorldGen.genRand.Next(5); m < num4; m += WorldGen.genRand.Next(num15, (int)((double)num15 * 1.5)))
            {
                int num14 = num6;
                while (!Main.tile[m, num14].active())
                {
                    num14--;
                }
                num14 += WorldGen.genRand.Next(-3, 4);
                num15 = WorldGen.genRand.Next(4, 8);
                int num16 = isVoid ? ModContent.TileType<Voidstone>() : ModContent.TileType<AbyssGravel>();
                if (WorldGen.genRand.Next(4) == 0)
                {
                    num16 = hasChest ? ModContent.TileType<ChaoticOre>() : ModContent.TileType<PlantyMush>();
                }
                for (int n = m - num15; n <= m + num15; n++)
                {
                    for (int num17 = num14 - num15; num17 <= num14 + num15; num17++)
                    {
                        if (num17 > num5)
                        {
                            float arg_409_0 = (float)Math.Abs(n - m);
                            float num18 = (float)(Math.Abs(num17 - num14) * 2);
                            if (Math.Sqrt((double)(arg_409_0 * arg_409_0 + num18 * num18)) < (double)(num15 + WorldGen.genRand.Next(2)))
                            {
                                Main.tile[n, num17].active(true);
                                Main.tile[n, num17].type = (ushort)num16;
                                SafeSquareTileFrame(n, num17, true);
                            }
                        }
                    }
                }
            }
            if (hasTenebris)
            {
                int p = num3;
                int num150;
                for (p += WorldGen.genRand.Next(5); p < num4; p += WorldGen.genRand.Next(num150, (int)((double)num150 * 1.5)))
                {
                    int num14 = num6;
                    while (!Main.tile[p, num14].active())
                    {
                        num14--;
                    }
                    num14 += WorldGen.genRand.Next(-3, 4); //-3 4
                    num150 = 1; //4 8
                    int num16 = ModContent.TileType<Tenebris>();
                    for (int n = p - num150; n <= p + num150; n++)
                    {
                        for (int num17 = num14 - num150; num17 <= num14 + num150; num17++)
                        {
                            if (num17 > num5)
                            {
                                float arg_409_0 = (float)Math.Abs(n - p);
                                float num18 = (float)(Math.Abs(num17 - num14) * 2);
                                if (Math.Sqrt((double)(arg_409_0 * arg_409_0 + num18 * num18)) < (double)(num150 + WorldGen.genRand.Next(2)))
                                {
                                    Main.tile[n, num17].active(true);
                                    Main.tile[n, num17].type = (ushort)num16;
                                    SafeSquareTileFrame(n, num17, true);
                                }
                            }
                        }
                    }
                }
            }
            int sizeMinSmall2 = sizeMin2 / 8;
            int sizeMaxSmall2 = sizeMax2 / 8;
            num = (double)WorldGen.genRand.Next(sizeMin2, sizeMax2);
            num2 = (float)WorldGen.genRand.Next(sizeMinSmall2, sizeMaxSmall2);
            vector.X = (float)i;
            vector.Y = (float)num5;
            vector2.X = (float)WorldGen.genRand.Next(-20, 21) * 0.2f;
            while (vector2.X > -2f && vector2.X < 2f)
            {
                vector2.X = (float)WorldGen.genRand.Next(-20, 21) * 0.2f;
            }
            vector2.Y = (float)WorldGen.genRand.Next(-20, -10) * 0.02f;
            while (num > 0.0 && num2 > 0f)
            {
                num -= (double)WorldGen.genRand.Next(4);
                num2 -= 1f;
                vector += vector2;
                vector2.X += (float)WorldGen.genRand.Next(-20, 21) * 0.05f;
                if (vector2.X > 1f)
                {
                    vector2.X = 1f;
                }
                if (vector2.X < -1f)
                {
                    vector2.X = -1f;
                }
                if ((double)vector2.Y > 0.2)
                {
                    vector2.Y = -0.2f;
                }
                if ((double)vector2.Y < -0.2)
                {
                    vector2.Y = -0.2f;
                }
            }
            int num23 = num3;
            num23 += WorldGen.genRand.Next(5);
            while (num23 < num4)
            {
                int num24 = num6;
                while ((!Main.tile[num23, num24].active() || Main.tile[num23, num24].type != 0) && num23 < num4)
                {
                    num24--;
                    if (num24 < num5)
                    {
                        num24 = num6;
                        num23 += WorldGen.genRand.Next(1, 4);
                    }
                }
                if (num23 < num4)
                {
                    num24 += WorldGen.genRand.Next(0, 4);
                    int num25 = WorldGen.genRand.Next(2, 5);
                    int num26 = isVoid ? ModContent.TileType<Voidstone>() : ModContent.TileType<AbyssGravel>();
                    for (int num27 = num23 - num25; num27 <= num23 + num25; num27++)
                    {
                        for (int num28 = num24 - num25; num28 <= num24 + num25; num28++)
                        {
                            if (num28 > num5)
                            {
                                float arg_890_0 = (float)Math.Abs(num27 - num23);
                                float num29 = (float)(Math.Abs(num28 - num24) * 2);
                                if (Math.Sqrt((double)(arg_890_0 * arg_890_0 + num29 * num29)) < (double)num25)
                                {
                                    Main.tile[num27, num28].type = (ushort)num26;
                                    SafeSquareTileFrame(num27, num28, true);
                                }
                            }
                        }
                    }
                    num23 += WorldGen.genRand.Next(num25, (int)((double)num25 * 1.5));
                }
            }
            for (int num30 = num3 - 20; num30 <= num4 + 20; num30++)
            {
                for (int num31 = num5 - 20; num31 <= num6 + 20; num31++)
                {
                    bool flag = true;
                    for (int num32 = num30 - 1; num32 <= num30 + 1; num32++)
                    {
                        for (int num33 = num31 - 1; num33 <= num31 + 1; num33++)
                        {
                            if (!Main.tile[num32, num33].active())
                            {
                                flag = false;
                            }
                        }
                    }
                    if (flag)
                    {
                        Main.tile[num30, num31].wall = (ushort)(isVoid ? ModContent.WallType<VoidstoneWallUnsafe>() : ModContent.WallType<AbyssGravelWall>());
                        WorldGen.SquareWallFrame(num30, num31, true);
                    }
                }
            }
            for (int num34 = num3; num34 <= num4; num34++)
            {
                int num35 = num5 - 10;
                while (!Main.tile[num34, num35 + 1].active())
                {
                    num35++;
                }
                if (num35 < num6 && Main.tile[num34, num35 + 1].type == (ushort)(isVoid ? ModContent.TileType<Voidstone>() : ModContent.TileType<AbyssGravel>()))
                {
                    if (WorldGen.genRand.Next(10) == 0)
                    {
                        int num36 = WorldGen.genRand.Next(1, 3);
                        for (int num37 = num34 - num36; num37 <= num34 + num36; num37++)
                        {
                            if (Main.tile[num37, num35].type == (ushort)(isVoid ? ModContent.TileType<Voidstone>() : ModContent.TileType<AbyssGravel>()))
                            {
                                Main.tile[num37, num35].active(false);
                                Main.tile[num37, num35].liquid = 255;
                                Main.tile[num37, num35].lava(false);
                                SafeSquareTileFrame(num34, num35, true);
                            }
                            if (Main.tile[num37, num35 + 1].type == (ushort)(isVoid ? ModContent.TileType<Voidstone>() : ModContent.TileType<AbyssGravel>()))
                            {
                                Main.tile[num37, num35 + 1].active(false);
                                Main.tile[num37, num35 + 1].liquid = 255;
                                Main.tile[num37, num35 + 1].lava(false);
                                SafeSquareTileFrame(num34, num35 + 1, true);
                            }
                            if (num37 > num34 - num36 && num37 < num34 + 2 && Main.tile[num37, num35 + 2].type == (ushort)(isVoid ? ModContent.TileType<Voidstone>() : ModContent.TileType<AbyssGravel>()))
                            {
                                Main.tile[num37, num35 + 2].active(false);
                                Main.tile[num37, num35 + 2].liquid = 255;
                                Main.tile[num37, num35 + 2].lava(false);
                                SafeSquareTileFrame(num34, num35 + 2, true);
                            }
                        }
                    }
                    if (WorldGen.genRand.Next(5) == 0)
                    {
                        Main.tile[num34, num35].liquid = 255;
                    }
                    Main.tile[num34, num35].lava(false);
                    SafeSquareTileFrame(num34, num35, true);
                }
            }
        }
        #endregion

        #region AbyssIslandHouse
        public static void AbyssIslandHouse(int i, int j, int itemChoice, bool isVoid)
        {
            Mod mod = ModLoader.GetMod("CalamityMod");
            ushort type = (ushort)(isVoid ? ModContent.TileType<Voidstone>() : ModContent.TileType<AbyssGravel>()); //tile
            ushort wall = (ushort)(isVoid ? ModContent.WallType<VoidstoneWallUnsafe>() : ModContent.WallType<AbyssGravelWall>()); //wall
            Vector2 vector = new Vector2((float)i, (float)j);
            int num = 1;
            if (WorldGen.genRand.Next(2) == 0)
            {
                num = -1;
            }
            int num2 = WorldGen.genRand.Next(5, 9);
            int num3 = 3;
            vector.X = (float)(i + (num2 + 2) * num);
            for (int k = j - 15; k < j + 30; k++)
            {
                if (Main.tile[(int)vector.X, k].active())
                {
                    vector.Y = (float)(k - 1);
                    break;
                }
            }
            vector.X = (float)i;
            int num4 = (int)(vector.X - (float)num2 - 1f);
            int num5 = (int)(vector.X + (float)num2 + 1f);
            int num6 = (int)(vector.Y - (float)num3 - 1f);
            int num7 = (int)(vector.Y + 2f);
            if (num4 < 0)
            {
                num4 = 0;
            }
            if (num5 > Main.maxTilesX)
            {
                num5 = Main.maxTilesX;
            }
            if (num6 < 0)
            {
                num6 = 0;
            }
            if (num7 > Main.maxTilesY)
            {
                num7 = Main.maxTilesY;
            }
            for (int l = num4; l <= num5; l++)
            {
                for (int m = num6 - 1; m < num7 + 1; m++)
                {
                    if (m != num6 - 1 || (l != num4 && l != num5))
                    {
                        Main.tile[l, m].active(true);
                        Main.tile[l, m].type = type;
                        Main.tile[l, m].wall = wall;
                        Main.tile[l, m].halfBrick(false);
                        Main.tile[l, m].slope(0);
                    }
                }
            }
            num4 = (int)(vector.X - (float)num2);
            num5 = (int)(vector.X + (float)num2);
            num6 = (int)(vector.Y - (float)num3);
            num7 = (int)(vector.Y + 1f);
            if (num4 < 0)
            {
                num4 = 0;
            }
            if (num5 > Main.maxTilesX)
            {
                num5 = Main.maxTilesX;
            }
            if (num6 < 0)
            {
                num6 = 0;
            }
            if (num7 > Main.maxTilesY)
            {
                num7 = Main.maxTilesY;
            }
            for (int n = num4; n <= num5; n++)
            {
                for (int num8 = num6; num8 < num7; num8++)
                {
                    if ((num8 != num6 || (n != num4 && n != num5)) && Main.tile[n, num8].wall == wall)
                    {
                        Main.tile[n, num8].active(false);
                    }
                }
            }
            int num9 = i + (num2 + 1) * num;
            int num10 = (int)vector.Y;
            for (int num11 = num9 - 2; num11 <= num9 + 2; num11++)
            {
                Main.tile[num11, num10].active(false);
                Main.tile[num11, num10 - 1].active(false);
                Main.tile[num11, num10 - 2].active(false);
            }
            switch (itemChoice)
            {
                case 0:
                    itemChoice = ModContent.ItemType<TorrentialTear>();
                    break; //rain item
                case 1:
                    itemChoice = ModContent.ItemType<IronBoots>();
                    break; //movement acc
                case 2:
                    itemChoice = ModContent.ItemType<DepthCharm>();
                    break; //regen acc
                case 3:
                    itemChoice = ModContent.ItemType<Archerfish>();
                    break; //ranged
                case 4:
                    itemChoice = ModContent.ItemType<AnechoicPlating>();
                    break; //defense acc
                case 5:
                    itemChoice = ModContent.ItemType<BallOFugu>();
                    break; //melee
                case 6:
                    itemChoice = ModContent.ItemType<StrangeOrb>();
                    break; //light pet
                case 7:
                    itemChoice = ModContent.ItemType<HerringStaff>();
                    break; //summon
                case 8:
                    itemChoice = ModContent.ItemType<BlackAnurian>();
                    break; //magic
                case 9:
                    itemChoice = ModContent.ItemType<Lionfish>();
                    break; //throwing
                default:
                    itemChoice = 497;
                    break;
            }
            WorldGen.AddBuriedChest(i, num10 - 3, itemChoice, false, 4); //chest
        }
        #endregion

        #region IceTomb
        public static bool IceTomb(int i, int j)
        {
            ushort num = 161;
            int arg_36_0 = j - WorldGen.genRand.Next(0, 7);
            int num2 = WorldGen.genRand.Next(9, 13);
            int num3 = 1;
            int num4 = j + WorldGen.genRand.Next(75, 125); //75 125
            for (int k = arg_36_0; k < num4; k++)
            {
                for (int l = i - num3; l < i + num3 - 1; l++)
                {
                    Main.tile[l, k].type = num;
                    Main.tile[l, k].active(true);
                    Main.tile[l, k].halfBrick(false);
                    Main.tile[l, k].slope(0);
                }
                num3++;
            }
            for (int m = i - num3 - 5; m <= i + num3 + 5; m++)
            {
                for (int n = j - 1; n <= num4 + 1; n++)
                {
                    bool flag = true;
                    for (int num5 = m - 1; num5 <= m + 1; num5++)
                    {
                        for (int num6 = n - 1; num6 <= n + 1; num6++)
                        {
                            if (Main.tile[num5, num6].type != num)
                            {
                                flag = false;
                            }
                        }
                    }
                    if (flag)
                    {
                        Main.tile[m, n].wall = 71;
                        WorldGen.SquareWallFrame(m, n, true);
                    }
                }
            }
            int num7 = 1;
            if (WorldGen.genRand.Next(2) == 0)
            {
                num7 = -1;
            }
            int num8 = i - num2 * num7;
            int num9 = j + num2;
            int num10 = WorldGen.genRand.Next(5, 8);
            bool flag2 = true;
            int num11 = WorldGen.genRand.Next(20, 30);
            while (flag2)
            {
                flag2 = false;
                bool flag3 = false;
                for (int num12 = num9; num12 <= num9 + num10; num12++)
                {
                    int num13 = num8;
                    if (Main.tile[num13, num12 - 1].type == 161)
                    {
                        flag3 = true;
                    }
                    if (Main.tile[num13, num12].type == num)
                    {
                        Main.tile[num13, num12 + 1].wall = 71;
                        Main.tile[num13 + num7, num12].wall = 71;
                        Main.tile[num13, num12].active(false);
                        flag2 = true;
                    }
                    if (flag3)
                    {
                        Main.tile[num13, num12].type = 161;
                        Main.tile[num13, num12].active(true);
                        Main.tile[num13, num12].halfBrick(false);
                        Main.tile[num13, num12].slope(0);
                    }
                }
                num8 -= num7;
            }
            num8 = i - num2 * num7;
            bool flag4 = true;
            bool flag5 = false;
            flag2 = true;
            while (flag2)
            {
                for (int num14 = num9; num14 <= num9 + num10; num14++)
                {
                    int num15 = num8;
                    Main.tile[num15, num14].active(false);
                }
                num8 += num7;
                num9++;
                num11--;
                if (num9 >= num4 - num10 * 2)
                {
                    num11 = 10;
                }
                if (num11 <= 0)
                {
                    bool flag6 = false;
                    if (!flag4 && !flag5)
                    {
                        flag5 = true;
                        flag6 = true;
                        int num16 = WorldGen.genRand.Next(7, 13);
                        int num17 = WorldGen.genRand.Next(23, 28);
                        int num18 = num17;
                        int num19 = num8;
                        while (num17 > 0)
                        {
                            for (int num20 = num9 - num16 + num10; num20 <= num9 + num10; num20++)
                            {
                                if (num17 == num18 || num17 == 1)
                                {
                                    if (num20 >= num9 - num16 + num10 + 2)
                                    {
                                        Main.tile[num8, num20].active(false);
                                    }
                                }
                                else if (num17 == num18 - 1 || num17 == 2 || num17 == num18 - 2 || num17 == 3)
                                {
                                    if (num20 >= num9 - num16 + num10 + 1)
                                    {
                                        Main.tile[num8, num20].active(false);
                                    }
                                }
                                else
                                {
                                    Main.tile[num8, num20].active(false);
                                }
                            }
                            num17--;
                            num8 += num7;
                        }
                        int num21 = num8 - num7;
                        int num22 = num21;
                        int num23 = num19;
                        if (num21 > num19)
                        {
                            num22 = num19;
                            num23 = num21;
                        }
                        int num24 = WorldGen.genRand.Next(3);
                        if (num24 == 0)
                        {
                            num24 = 1861; //diving gear
                        }
                        else if (num24 == 1)
                        {
                            num24 = 1163; //balloon
                        }
                        else if (num24 == 2)
                        {
                            num24 = 1253; //shell
                        }
                        WorldGen.AddBuriedChest((num22 + num23) / 2, num9, num24, false, 22);
                        int num25 = WorldGen.genRand.Next(1, 10);
                        for (int num26 = 0; num26 < num25; num26++)
                        {
                            int arg_49B_0 = WorldGen.genRand.Next(num22, num23);
                            int j2 = num9 + num10;
                            WorldGen.PlaceSmallPile(arg_49B_0, j2, WorldGen.genRand.Next(16, 19), 1, 185);
                        }
                        for (int num27 = num22; num27 <= num23; num27++)
                        {
                            WorldGen.PlacePot(num27, num9 + num10, 28, WorldGen.genRand.Next(4, 7));
                        }
                    }
                    if (flag4)
                    {
                        flag4 = false;
                        num7 *= -1;
                        num11 = WorldGen.genRand.Next(15, 20);
                    }
                    else if (flag6)
                    {
                        num11 = WorldGen.genRand.Next(10, 15);
                    }
                    else
                    {
                        num7 *= -1;
                        num11 = WorldGen.genRand.Next(20, 40);
                    }
                }
                if (num9 >= num4 - num10)
                {
                    flag2 = false;
                }
            }
            int num28 = WorldGen.genRand.Next(20, 40); //100 200
            int num29 = WorldGen.genRand.Next(100, 160); //500 800
            flag2 = true;
            int num30 = num10;
            num11 = WorldGen.genRand.Next(10, 50);
            if (num7 == 1)
            {
                num8 -= num30;
            }
            int num31 = WorldGen.genRand.Next(5, 10);
            while (flag2)
            {
                num28--;
                num29--;
                num11--;
                for (int num32 = num8 - num31 - WorldGen.genRand.Next(0, 2); num32 <= num8 + num30 + num31 + WorldGen.genRand.Next(0, 2); num32++)
                {
                    int num33 = num9;
                    if (num32 >= num8 && num32 <= num8 + num30)
                    {
                        Main.tile[num32, num33].active(false);
                    }
                    else
                    {
                        Main.tile[num32, num33].type = num;
                        Main.tile[num32, num33].active(true);
                        Main.tile[num32, num33].halfBrick(false);
                        Main.tile[num32, num33].slope(0);
                    }
                    if (num32 >= num8 - 1 && num32 <= num8 + 1 + num30)
                    {
                        Main.tile[num32, num33].wall = 71;
                    }
                }
                num9++;
                num8 += num7;
                if (num28 <= 0)
                {
                    flag2 = false;
                    for (int num34 = num8 + 1; num34 <= num8 + num30 - 1; num34++)
                    {
                        if (Main.tile[num34, num9].active())
                        {
                            flag2 = true;
                        }
                    }
                }
                if (num11 < 0)
                {
                    num11 = WorldGen.genRand.Next(10, 50);
                    num7 *= -1;
                }
                if (num29 <= 0)
                {
                    flag2 = false;
                }
            }
            return true;
        }
        #endregion

        #region SpecialHut
        // Special Hut: Takes arguments of tile type 1, tile type 2, wall type, hut type (useful if you use this method to generate different huts), and location of the shrine (x and y)
        public static void SpecialHut(ushort tile, ushort tile2, ushort wall, int hutType, int shrineLocationX, int shrineLocationY)
        {
            // Random variables for shrine size
            int randomX = WorldGen.genRand.Next(2, 4);
            int randomY = WorldGen.genRand.Next(2, 4);

            // Replace tiles in shrine area with shrine tile type 1
            for (int n = shrineLocationX - randomX - 1; n <= shrineLocationX + randomX + 1; n++)
            {
                for (int num8 = shrineLocationY - randomY - 1; num8 <= shrineLocationY + randomY + 1; num8++)
                {
                    Main.tile[n, num8].active(true);
                    Main.tile[n, num8].type = tile;
                    Main.tile[n, num8].slope(0);
                    Main.tile[n, num8].liquid = 0;
                    Main.tile[n, num8].lava(false);
                }
            }

            // Replace walls in shrine area with shrine wall type
            for (int num9 = shrineLocationX - randomX; num9 <= shrineLocationX + randomX; num9++)
            {
                for (int num10 = shrineLocationY - randomY; num10 <= shrineLocationY + randomY; num10++)
                {
                    Main.tile[num9, num10].active(false);
                    Main.tile[num9, num10].wall = wall;
                }
            }

            // Remove tiles from the inner part of the shrine area
            for (int num14 = shrineLocationX - randomX - 1; num14 <= shrineLocationX + randomX + 1; num14++)
            {
                for (int num15 = shrineLocationY + randomY - 2; num15 <= shrineLocationY + randomY; num15++)
                    Main.tile[num14, num15].active(false);
            }
            for (int num16 = shrineLocationX - randomX - 1; num16 <= shrineLocationX + randomX + 1; num16++)
            {
                for (int num17 = shrineLocationY + randomY - 2; num17 <= shrineLocationY + randomY - 1; num17++)
                    Main.tile[num16, num17].active(false);
            }

            // Replace tiles from bottom of shrine area with shrine tile type 2
            for (int num18 = shrineLocationX - randomX - 1; num18 <= shrineLocationX + randomX + 1; num18++)
            {
                int num19 = 4;
                int num20 = shrineLocationY + randomY + 2;
                while (!Main.tile[num18, num20].active() && num20 < Main.maxTilesY && num19 > 0)
                {
                    Main.tile[num18, num20].active(true);
                    Main.tile[num18, num20].type = tile2;
                    Main.tile[num18, num20].slope(0);
                    num20++;
                    num19--;
                }
            }

            // Replace tiles from top of shrine with shrine tile type 1
            randomX -= WorldGen.genRand.Next(1, 3);
            int num21 = shrineLocationY - randomY - 2;
            while (randomX > -1)
            {
                for (int num22 = shrineLocationX - randomX - 1; num22 <= shrineLocationX + randomX + 1; num22++)
                {
                    Main.tile[num22, num21].active(true);
                    Main.tile[num22, num21].type = tile;
                }
                randomX -= WorldGen.genRand.Next(1, 3);
                num21--;
            }

            // Place shrine chest
            CalamityWorld.SChestX[hutType] = shrineLocationX;
            CalamityWorld.SChestY[hutType] = shrineLocationY;
            SpecialChest(hutType);
        }
        #endregion

        #region SpecialChest
        // Special Chest: Used for placing shrine chests, takes argument of item choice which dictates what item will spawn in the first slot of this chest
        public static void SpecialChest(int itemChoice)
        {
            Mod mod = ModLoader.GetMod("CalamityMod");
            int item = 0;
            int chestType = 0;

            switch (itemChoice) //0 to 9
            {
                case 0:
                    item = ModContent.ItemType<TrinketofChi>();
                    break;
                case 1:
                    item = WorldGen.crimson ? ModContent.ItemType<CrimsonEffigy>() : ModContent.ItemType<CorruptionEffigy>();
                    chestType = WorldGen.crimson ? 43 : 3;
                    break;
                case 2:
                    item = ModContent.ItemType<OnyxExcavatorKey>();
                    chestType = 44;
                    break;
                case 3:
                    item = ModContent.ItemType<TundraLeash>();
                    chestType = 47;
                    break;
                case 4:
                    item = ModContent.ItemType<LuxorsGift>();
                    chestType = 30;
                    break;
                case 5:
                    item = ModContent.ItemType<FungalSymbiote>();
                    chestType = 32;
                    break;
                case 6:
                    item = ModContent.ItemType<UnstablePrism>();
                    chestType = 50;
                    break;
                case 7:
                    item = ModContent.ItemType<GladiatorsLocket>();
                    chestType = 51;
                    break;
                case 8:
                    item = ModContent.ItemType<Murasama>();
                    chestType = 44;
                    break;
                case 9:
                    item = ModContent.ItemType<BossRush>();
                    chestType = 4;
                    break;
            }

            // Destroy tiles in chest spawn location
            for (int j = CalamityWorld.SChestX[itemChoice] - 1; j <= CalamityWorld.SChestX[itemChoice] + 1; j++)
            {
                for (int k = CalamityWorld.SChestY[itemChoice]; k <= CalamityWorld.SChestY[itemChoice] + 2; k++)
                    WorldGen.KillTile(j, k, false, false, false);
            }

            // Attempt to fix sloped tiles under the chest to prevent the chest from killing itself (literally)
            for (int l = CalamityWorld.SChestX[itemChoice] - 1; l <= CalamityWorld.SChestX[itemChoice] + 1; l++)
            {
                for (int m = CalamityWorld.SChestY[itemChoice]; m <= CalamityWorld.SChestY[itemChoice] + 3; m++)
                {
                    if (m < Main.maxTilesY)
                    {
                        Main.tile[l, m].slope(0);
                        Main.tile[l, m].halfBrick(false);
                    }
                }
            }

            // Place the chest, finally
            WorldGen.AddBuriedChest(CalamityWorld.SChestX[itemChoice], CalamityWorld.SChestY[itemChoice], item, false, chestType);
        }
        #endregion

        #region ChasmGenerator
        public static void ChasmGenerator(int i, int j, int steps, bool ocean = false)
        {
            Mod mod = ModLoader.GetMod("CalamityMod");
            float num = (float)steps; //850 small 1450 medium 2050 large
            if (ocean)
            {
                int tileYLookup = j;
                if (CalamityWorld.abyssSide)
                {
                    while (!Main.tile[i + 125, tileYLookup].active())
                    {
                        tileYLookup++;
                    }
                }
                else
                {
                    while (!Main.tile[i - 125, tileYLookup].active())
                    {
                        tileYLookup++;
                    }
                }
                j = tileYLookup;
            }
            Vector2 vector;
            vector.X = (float)i;
            vector.Y = (float)j;
            Vector2 vector2;
            vector2.X = (float)WorldGen.genRand.Next(-1, 2) * 0.1f;
            vector2.Y = (float)WorldGen.genRand.Next(3, 8) * 0.2f + 0.5f;
            int num2 = 5;
            double num3 = (double)(WorldGen.genRand.Next(5, 7) + 40); //start width
            while (num3 > 0.0)
            {
                if (num > 0f)
                {
                    num3 += (double)WorldGen.genRand.Next(2);
                    num3 -= (double)WorldGen.genRand.Next(2);
                    float smallHoleLimit = 790f; //small
                    if (Main.maxTilesY > 1500)
                    { smallHoleLimit = 1360f; if (Main.maxTilesY > 2100) { smallHoleLimit = 1950f; } }
                    if (ocean && num > smallHoleLimit)
                    {
                        if (num3 < 4.0) //min width
                        {
                            num3 = 4.0; //min width
                        }
                        if (num3 > 6.0) //max width
                        {
                            num3 = 6.0; //max width
                        }
                    }
                    else //dig large hole
                    {
                        if (num3 < (ocean ? 42.0 : 14.0)) //min width
                        {
                            num3 = ocean ? 42.0 : 14.0; //min width
                        }
                        if (num3 > (ocean ? 60.0 : 40.0)) //max width
                        {
                            num3 = ocean ? 60.0 : 40.0; //max width
                        }
                        if (num == 1f && num3 < (ocean ? 50.0 : 20.0))
                        {
                            num3 = ocean ? 50.0 : 20.0;
                        }
                    }
                }
                else
                {
                    if ((double)vector.Y > CalamityWorld.abyssChasmBottom)
                    {
                        num3 -= (double)(WorldGen.genRand.Next(5) + 8);
                    }
                }
                if (Main.maxTilesY > 2100)
                {
                    if (((double)vector.Y > CalamityWorld.abyssChasmBottom && num > 0f && ocean) ||
                        ((double)vector.Y >= (double)Main.maxTilesY && num > 0f && !ocean))
                    {
                        num = 0f;
                    }
                }
                else if (Main.maxTilesY > 1500)
                {
                    if (((double)vector.Y > CalamityWorld.abyssChasmBottom && num > 0f && ocean) ||
                        ((double)vector.Y > (double)Main.maxTilesY && num > 0f && !ocean))
                    {
                        num = 0f;
                    }
                }
                else
                {
                    if (((double)vector.Y > CalamityWorld.abyssChasmBottom && num > 0f && ocean) ||
                        ((double)vector.Y > (double)Main.maxTilesY && num > 0f && !ocean))
                    {
                        num = 0f;
                    }
                }
                num -= 1f;
                int num4;
                int num5;
                int num6;
                int num7;
                if (num > (float)num2)
                {
                    num4 = (int)((double)vector.X - num3 * 0.5);
                    num5 = (int)((double)vector.X + num3 * 0.5);
                    num6 = (int)((double)vector.Y - num3 * 0.5);
                    num7 = (int)((double)vector.Y + num3 * 0.5);
                    if (num4 < 0)
                    {
                        num4 = 0;
                    }
                    if (num5 > Main.maxTilesX - 1)
                    {
                        num5 = Main.maxTilesX - 1;
                    }
                    if (num6 < 0)
                    {
                        num6 = 0;
                    }
                    if (num7 > Main.maxTilesY)
                    {
                        num7 = Main.maxTilesY;
                    }
                    for (int k = num4; k < num5; k++)
                    {
                        for (int l = num6; l < num7; l++)
                        {
                            if ((double)(Math.Abs((float)k - vector.X) + Math.Abs((float)l - vector.Y)) < num3 * 0.5 * (1.0 + (double)WorldGen.genRand.Next(-5, 6) * 0.015))
                            {
                                if (ocean)
                                {
                                    Main.tile[k, l].active(false);
                                    Main.tile[k, l].liquid = 255;
                                    Main.tile[k, l].lava(false);
                                }
                                else
                                {
                                    Main.tile[k, l].active(false);
                                    Main.tile[k, l].liquid = 255;
                                    Main.tile[k, l].lava(true);
                                }
                            }
                        }
                    }
                }
                if (num <= 2f && (double)vector.Y < (Main.rockLayer + (double)Main.maxTilesY * 0.3))
                {
                    num = 2f;
                }
                vector += vector2;
                vector2.X += (float)WorldGen.genRand.Next(-1, 2) * 0.01f;
                if ((double)vector2.X > 0.02)
                {
                    vector2.X = 0.02f;
                }
                if ((double)vector2.X < -0.02)
                {
                    vector2.X = -0.02f;
                }
                num4 = (int)((double)vector.X - num3 * 1.1);
                num5 = (int)((double)vector.X + num3 * 1.1);
                num6 = (int)((double)vector.Y - num3 * 1.1);
                num7 = (int)((double)vector.Y + num3 * 1.1);
                if (num4 < 1)
                {
                    num4 = 1;
                }
                if (num5 > Main.maxTilesX - 1)
                {
                    num5 = Main.maxTilesX - 1;
                }
                if (num6 < 0)
                {
                    num6 = 0;
                }
                if (num7 > Main.maxTilesY)
                {
                    num7 = Main.maxTilesY;
                }
                for (int m = num4; m < num5; m++)
                {
                    for (int n = num6; n < num7; n++)
                    {
                        if ((double)(Math.Abs((float)m - vector.X) + Math.Abs((float)n - vector.Y)) < num3 * 1.1 * (1.0 + (double)WorldGen.genRand.Next(-5, 6) * 0.015))
                        {
                            if (n > j + WorldGen.genRand.Next(7, 16))
                            {
                                Main.tile[m, n].active(false);
                            }
                            if (steps <= num2)
                            {
                                Main.tile[m, n].active(false);
                            }
                            if (ocean)
                            {
                                Main.tile[m, n].liquid = 255;
                                Main.tile[m, n].lava(false);
                            }
                            else
                            {
                                Main.tile[m, n].liquid = 255;
                                Main.tile[m, n].lava(true);
                            }
                        }
                    }
                }
                for (int num11 = num4; num11 < num5; num11++)
                {
                    for (int num12 = num6; num12 < num7; num12++)
                    {
                        if ((double)(Math.Abs((float)num11 - vector.X) + Math.Abs((float)num12 - vector.Y)) < num3 * 1.1 * (1.0 + (double)WorldGen.genRand.Next(-5, 6) * 0.015))
                        {
                            if (ocean)
                            {
                                Main.tile[num11, num12].liquid = 255;
                                Main.tile[num11, num12].lava(false);
                            }
                            else
                            {
                                Main.tile[num11, num12].liquid = 255;
                                Main.tile[num11, num12].lava(true);
                            }
                            if (steps <= num2)
                            {
                                Main.tile[num11, num12].active(false);
                            }
                        }
                    }
                }
            }
        }
        #endregion

        #region PlaceTits
        public static void PlaceTit(int x, int y, ushort type = 165)
        {
            Mod mod = ModLoader.GetMod("CalamityMod");
            if (Main.tile[x, y - 1] == null)
            {
                Main.tile[x, y - 1] = new Tile();
            }
            if (Main.tile[x, y] == null)
            {
                Main.tile[x, y] = new Tile();
            }
            if (Main.tile[x, y + 1] == null)
            {
                Main.tile[x, y + 1] = new Tile();
            }
            if (WorldGen.SolidTile(x, y - 1) && !Main.tile[x, y].active() && !Main.tile[x, y + 1].active())
            {
                if (Main.tile[x, y - 1].type == (ushort)ModContent.TileType<Navystone>())
                {
                    if (WorldGen.genRand.Next(2) == 0 || Main.tile[x, y + 2].active())
                    {
                        int num2 = WorldGen.genRand.Next(3) * 18;
                        Main.tile[x, y].type = type;
                        Main.tile[x, y].active(true);
                        Main.tile[x, y].frameX = (short)num2;
                        Main.tile[x, y].frameY = 72;
                    }
                    else
                    {
                        int num3 = WorldGen.genRand.Next(3) * 18;
                        Main.tile[x, y].type = type;
                        Main.tile[x, y].active(true);
                        Main.tile[x, y].frameX = (short)num3;
                        Main.tile[x, y].frameY = 0;
                        Main.tile[x, y + 1].type = type;
                        Main.tile[x, y + 1].active(true);
                        Main.tile[x, y + 1].frameX = (short)num3;
                        Main.tile[x, y + 1].frameY = 18;
                    }
                }
            }
            else
            {
                if (WorldGen.SolidTile(x, y + 1) && !Main.tile[x, y].active() && !Main.tile[x, y - 1].active())
                {
                    if (Main.tile[x, y + 1].type == (ushort)ModContent.TileType<Navystone>())
                    {
                        if (WorldGen.genRand.Next(2) == 0 || Main.tile[x, y - 2].active())
                        {
                            int num13 = WorldGen.genRand.Next(3) * 18;
                            Main.tile[x, y].type = type;
                            Main.tile[x, y].active(true);
                            Main.tile[x, y].frameX = (short)num13;
                            Main.tile[x, y].frameY = 90;
                        }
                        else
                        {
                            int num14 = WorldGen.genRand.Next(3) * 18;
                            Main.tile[x, y - 1].type = type;
                            Main.tile[x, y - 1].active(true);
                            Main.tile[x, y - 1].frameX = (short)num14;
                            Main.tile[x, y - 1].frameY = 36;
                            Main.tile[x, y].type = type;
                            Main.tile[x, y].active(true);
                            Main.tile[x, y].frameX = (short)num14;
                            Main.tile[x, y].frameY = 54;
                        }
                    }
                }
            }
        }
        #endregion

        #region Planetoids
        public static void Planetoids(GenerationProgress progress)
        {
            progress.Message = "Generating Planetoids...";

            int GrassPlanetoidCount = Main.maxTilesX / 1100;
            int LCPlanetoidCount = Main.maxTilesX / 800;
            int MudPlanetoidCount = Main.maxTilesX / 1100;

            while (true)
            {
                if (Biomes<MainPlanet>.Place(new Point(WorldGen.genRand.Next(Main.maxTilesX / 2 - 300, Main.maxTilesX / 2 + 300), WorldGen.genRand.Next(128, 134)), WorldGen.structures))
                {
                    break;
                }
            }
            while (LCPlanetoidCount > 0)
            {
                int x = WorldGen.genRand.Next((int)(Main.maxTilesX * 0.2), (int)(Main.maxTilesX * 0.8));
                int y = WorldGen.genRand.Next(70, 101);

                bool placed = Biomes<HeartPlanet>.Place(x, y, WorldGen.structures);

                if (placed)
                    LCPlanetoidCount--;
            }
            while (GrassPlanetoidCount > 0)
            {
                int x = WorldGen.genRand.Next((int)(Main.maxTilesX * 0.333), (int)(Main.maxTilesX * 0.666));
                int y = WorldGen.genRand.Next(100, 131);


                bool placed = Biomes<GrassPlanet>.Place(x, y, WorldGen.structures);

                if (placed)
                    GrassPlanetoidCount--;
            }
            while (MudPlanetoidCount > 0)
            {
                int x = WorldGen.genRand.Next((int)(Main.maxTilesX * 0.3f), (int)(Main.maxTilesX * 0.7f));
                int y = WorldGen.genRand.Next(100, 131);

                bool placed = Biomes<MudPlanet>.Place(x, y, WorldGen.structures);

                if (placed)
                    MudPlanetoidCount--;
            }
        }
        #endregion
    }
}
