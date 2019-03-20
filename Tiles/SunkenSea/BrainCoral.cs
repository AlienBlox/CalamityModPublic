﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Terraria;
using Terraria.ObjectData;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace CalamityMod.Tiles.SunkenSea
{
    public class BrainCoral : ModTile
    {
        public override void SetDefaults()
        {
            Main.tileFrameImportant[Type] = true;
			Main.tileNoAttach[Type] = true;
			TileObjectData.newTile.CopyFrom(TileObjectData.Style3x2);
			TileObjectData.addTile(Type);
			dustType = 253;
			ModTranslation name = CreateMapEntryName();
			name.SetDefault("Brain Coral");
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
				if (Main.rand.Next(300) == 0)
				{
					int tileLocationY = j - 1;
					if (Main.tile[i, tileLocationY] != null)
					{
						if (!Main.tile[i, tileLocationY].active())
						{
							if (Main.tile[i, tileLocationY].liquid == 255 && Main.tile[i, tileLocationY - 1].liquid == 255 && Main.tile[i, tileLocationY - 2].liquid == 255)
							{
								if (Main.netMode != 1)
									Projectile.NewProjectile((float)(i * 16 + 16), (float)(tileLocationY * 16 + 16), 0f, -0.1f, mod.ProjectileType("CoralBubble"), 0, 0f, Main.myPlayer, 0f, 0f);
							}
						}
					}
				}
			}
		}
	}
}
