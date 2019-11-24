﻿using Terraria;
using Microsoft.Xna.Framework;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Walls
{
    public class AstralSnowWallSafe : ModWall
    {

        public override void SetDefaults()
        {
            dustType = ModContent.DustType<Dusts.AstralBasic>();
            drop = ModContent.ItemType<Items.Placeables.Walls.AstralSnowWall>();
            Main.wallHouse[Type] = true;

            AddMapEntry(new Color(135, 145, 149));
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }
    }
}
