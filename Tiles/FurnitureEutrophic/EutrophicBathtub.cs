﻿using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace CalamityMod.Tiles.FurnitureEutrophic
{
    public class EutrophicBathtub : ModTile
    {
        public override void SetStaticDefaults()
        {
            this.SetUpBathtub();
            TileObjectData.newTile.CopyFrom(TileObjectData.Style4x2);
            TileObjectData.addTile(Type);
            LocalizedText name = CreateMapEntryName();
            // name.SetDefault("Bathtub");
            AddMapEntry(new Color(191, 142, 111), name);
            AnimationFrameHeight = 54;
        }

        public override bool CreateDust(int i, int j, ref int type)
        {
            Dust.NewDust(new Vector2(i, j) * 16f, 16, 16, 51, 0f, 0f, 1, new Color(54, 69, 72), 1f);
            return false;
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            Item.NewItem(new EntitySource_TileBreak(i, j), i * 16, j * 16, 16, 32, ModContent.ItemType<Items.Placeables.FurnitureEutrophic.EutrophicBathtub>());
        }
    }
}
