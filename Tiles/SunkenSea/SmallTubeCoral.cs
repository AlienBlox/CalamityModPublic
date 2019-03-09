﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Terraria;
using Terraria.Enums;
using Terraria.ObjectData;
using Terraria.DataStructures;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace CalamityMod.Tiles.SunkenSea
{
    public class SmallTubeCoral : ModTile
    {
        public override void SetDefaults()
        {
            Main.tileFrameImportant[Type] = true;
			Main.tileNoAttach[Type] = true;
            Main.tileBlockLight[Type] = true;
			TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
			TileObjectData.addTile(Type);
			dustType = 253;
			ModTranslation name = CreateMapEntryName();
			name.SetDefault("Small Tube Coral");
			AddMapEntry(new Color(0, 0, 80));
			mineResist = 3f;

			base.SetDefaults();
        }

		public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }
    }
}
