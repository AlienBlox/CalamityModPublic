﻿using CalamityMod.Dusts;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader;
namespace CalamityMod.Tiles.Astral
{
    public class AstralTree : ModTree
    {
        public override Texture2D GetTopTextures(int i, int j, ref int frame, ref int frameWidth, ref int frameHeight, ref int xOffsetLeft, ref int yOffset)
        {
            frame = (i + j * j) % 3;
            return ModContent.GetTexture("CalamityMod/Tiles/Astral/AstralTree_Tops");
        }

        public override Texture2D GetBranchTextures(int i, int j, int trunkOffset, ref int frame) => ModContent.GetTexture("CalamityMod/Tiles/Astral/AstralTree_Branches");
        public override Texture2D GetTexture() => ModContent.GetTexture("CalamityMod/Tiles/Astral/AstralTree");
        public override int DropWood() => ModContent.ItemType<Items.Placeables.AstralMonolith>();
        public override int CreateDust() => ModContent.DustType<AstralBasic>();
        public override int GrowthFXGore() => -1;
    }
}
