﻿using CalamityMod.Projectiles.Typeless;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace CalamityMod.Tiles.SunkenSea
{
    public class SmallBrainCoral : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
            TileObjectData.addTile(Type);
            DustType = 253;
            ModTranslation name = CreateMapEntryName();
            name.SetDefault("Small Brain Coral");
            AddMapEntry(new Color(36, 61, 111));
            MineResist = 3f;

            base.SetStaticDefaults();
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }

        public override void NearbyEffects(int i, int j, bool closer)
        {
            if (Main.gamePaused)
            {
                return;
            }
            if (closer)
            {
                if (Main.rand.NextBool(300))
                {
                    int tileLocationY = j - 1;
                    if (Main.tile[i, tileLocationY] != null)
                    {
                        if (!Main.tile[i, tileLocationY].HasTile)
                        {
                            if (Main.tile[i, tileLocationY].LiquidAmount == 255 && Main.tile[i, tileLocationY - 1].LiquidAmount == 255 && Main.tile[i, tileLocationY - 2].LiquidAmount == 255)
                            {
                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                    Projectile.NewProjectile(new EntitySource_WorldEvent(), i * 16 + 16, tileLocationY * 16 + 16, 0f, -0.1f, ModContent.ProjectileType<CoralBubbleSmall>(), 0, 1f, Main.myPlayer);
                            }
                        }
                    }
                }
            }
        }
    }
}
