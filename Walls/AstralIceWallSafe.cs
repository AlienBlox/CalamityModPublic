﻿using Terraria;
using Microsoft.Xna.Framework;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Walls
{
    public class AstralIceWallSafe : ModWall
    {

        public override void SetDefaults()
        {
            // TODO -- Change this dust to be one more befitting Astral Ice.
            Main.wallHouse[Type] = true;
            dustType = DustID.Shadowflame;
            drop = ModContent.ItemType<Items.Placeables.Walls.AstralIceWall>();

            AddMapEntry(new Color(83, 76, 92));
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }
    }
}
