﻿using Terraria.ModLoader;
using Microsoft.Xna.Framework.Graphics;

namespace CalamityMod.Tiles
{
    public class AstralSnowTree : ModTree
    {
        public override Texture2D GetTopTextures(int i, int j, ref int frame, ref int frameWidth, ref int frameHeight, ref int xOffsetLeft, ref int yOffset)
        {
            frame = (i + j * j) % 6;
            return ModContent.GetTexture("CalamityMod/Tiles/AstralSnow/AstralSnowTree_Tops");
        }

        public override Texture2D GetBranchTextures(int i, int j, int trunkOffset, ref int frame)
        {
            return ModContent.GetTexture("CalamityMod/Tiles/AstralSnow/AstralSnowTree_Branches");
        }

        public override Texture2D GetTexture()
        {
            return ModContent.GetTexture("CalamityMod/Tiles/AstralSnow/AstralSnowTree");
        }

        public override int DropWood()
        {
            return ModContent.ItemType<Items.Placeables.AstralMonolith>();
        }

        public override int CreateDust()
        {
            return ModContent.DustType<Dusts.AstralBasic>();
        }

        public override int GrowthFXGore()
        {
            return -1;
        }
    }
}
