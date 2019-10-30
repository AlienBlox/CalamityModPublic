﻿using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Walls
{
    public class AstralDirtWallSafe : ModWall
    {

        public override void SetDefaults()
        {
            // TODO -- Change this dust to be one more befitting Astral Dirt.
            dustType = DustID.Shadowflame;
            Main.wallHouse[Type] = true;
            drop = ModContent.ItemType<Items.Placeables.Walls.AstralDirtWall>();
            AddMapEntry(new Color(26, 22, 32));
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }
    }
}
