﻿using CalamityMod.Dusts.Furniture;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.ObjectInteractions;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.Tiles.FurnitureOtherworldly
{
    [LegacyName("OccultBed")]
    public class OtherworldlyBed : ModTile
    {
        public override void SetStaticDefaults()
        {
            this.SetUpBed(true);
            LocalizedText name = CreateMapEntryName();
            // name.SetDefault("Bed");
            AddMapEntry(new Color(191, 142, 111), name);
            AdjTiles = new int[] { TileID.Beds };
        }

        public override bool CreateDust(int i, int j, ref int type)
        {
            Dust.NewDust(new Vector2(i, j) * 16f, 16, 16, 1, 0f, 0f, 1, new Color(125, 94, 128), 1f);
            Dust.NewDust(new Vector2(i, j) * 16f, 16, 16, ModContent.DustType<OtherworldlyTileCloth>(), 0f, 0f, 1, new Color(255, 255, 255), 1f);
            return false;
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }

        public override bool HasSmartInteract(int i, int j, SmartInteractScanSettings settings) => true;

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            Item.NewItem(new EntitySource_TileBreak(i, j), i * 16, j * 16, 64, 32, ModContent.ItemType<Items.Placeables.FurnitureOtherworldly.OtherworldlyBed>());
        }

        public override bool RightClick(int i, int j) => CalamityUtils.BedRightClick(i, j);

        public override void MouseOver(int i, int j) => CalamityUtils.MouseOver(i, j, ModContent.ItemType<Items.Placeables.FurnitureOtherworldly.OtherworldlyBed>());
    }
}
