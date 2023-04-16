﻿using CalamityMod.Buffs.Placeables;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace CalamityMod.Tiles.Furniture
{
    public class YellowCandle : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileLighted[Type] = true;
            Main.tileFrameImportant[Type] = true;
            Main.tileLavaDeath[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style1x2);
            TileObjectData.addTile(Type);
            LocalizedText name = CreateMapEntryName();
            // name.SetDefault("Spiteful Candle");
            AdjTiles = new int[] { TileID.Candles };
            AddToArray(ref TileID.Sets.RoomNeeds.CountsAsTorch);
            AddMapEntry(new Color(238, 145, 105), name);
            AnimationFrameHeight = 34;
        }

        public override void AnimateTile(ref int frame, ref int frameCounter)
        {
            frameCounter++;
            if (frameCounter >= 6)
            {
                frame = (frame + 1) % 6;
                frameCounter = 0;
            }
        }

        public override void NearbyEffects(int i, int j, bool closer)
        {
            Player player = Main.LocalPlayer;
            if (player == null || !player.active || player.dead)
                return;
            player.AddBuff(ModContent.BuffType<CirrusYellowCandleBuff>(), 20);
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                for (int m = 0; m < Main.maxNPCs; m++)
                {
                    if (Main.npc[m].active && !Main.npc[m].friendly)
                    {
                        Main.npc[m].buffImmune[ModContent.BuffType<CirrusYellowCandleBuff>()] = false;
                        if (Main.npc[m].Calamity().DR >= 0.99f)
                        {
                            Main.npc[m].buffImmune[ModContent.BuffType<CirrusYellowCandleBuff>()] = true;
                        }
                        Main.npc[m].AddBuff(ModContent.BuffType<CirrusYellowCandleBuff>(), 20, false);
                    }
                }
            }
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            r = 0.75f;
            g = 0.75f;
            b = 0.35f;
        }
    }
}
