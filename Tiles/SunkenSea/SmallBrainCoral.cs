﻿using Microsoft.Xna.Framework;
using Terraria;
using CalamityMod.Projectiles;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.ObjectData;

namespace CalamityMod.Tiles
{
    public class SmallBrainCoral : ModTile
    {
        public override void SetDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
            TileObjectData.addTile(Type);
            dustType = 253;
            ModTranslation name = CreateMapEntryName();
            name.SetDefault("Small Brain Coral");
            AddMapEntry(new Color(0, 0, 80));
            mineResist = 3f;

            base.SetDefaults();
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }

        public override void NearbyEffects(int i, int j, bool closer)
        {
            if (closer)
            {
                if (Main.rand.NextBool(300))
                {
                    int tileLocationY = j - 1;
                    if (Main.tile[i, tileLocationY] != null)
                    {
                        if (!Main.tile[i, tileLocationY].active())
                        {
                            if (Main.tile[i, tileLocationY].liquid == 255 && Main.tile[i, tileLocationY - 1].liquid == 255 && Main.tile[i, tileLocationY - 2].liquid == 255)
                            {
                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                    Projectile.NewProjectile((float)(i * 16 + 16), (float)(tileLocationY * 16 + 16), 0f, -0.1f, ModContent.ProjectileType<CoralBubbleSmall>(), 0, 1f, Main.myPlayer, 0f, 0f);
                            }
                        }
                    }
                }
            }
        }
    }
}
