﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Terraria;
using Terraria.Enums;
using Terraria.ObjectData;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;

namespace CalamityMod.Tiles.SunkenSea
{
    public class TableCoral : ModTile
    {
        public override void SetDefaults()
        {
            Main.tileFrameImportant[Type] = true;
			Main.tileNoAttach[Type] = true;
            Main.tileBlockLight[Type] = true;
			Main.tileSolidTop[Type] = true;
			TileObjectData.newTile.CopyFrom(TileObjectData.Style3x2);
			TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.None, 0, 0);
			TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
			TileObjectData.newAlternate.AnchorLeft = new AnchorData(AnchorType.SolidTile, 2, 0);
			TileObjectData.addAlternate(1);
			TileObjectData.newTile.AnchorRight = new AnchorData(AnchorType.SolidTile, 2, 0);
			TileObjectData.addTile(Type);
			dustType = 253;
			ModTranslation name = CreateMapEntryName();
			name.SetDefault("Table Coral");
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
