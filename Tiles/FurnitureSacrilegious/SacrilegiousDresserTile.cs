﻿using CalamityMod.Items.Placeables.FurnitureSacrilegious;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.ObjectInteractions;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.Tiles.FurnitureSacrilegious
{
    public class SacrilegiousDresserTile : ModTile
    {
        public override void SetStaticDefaults()
        {
            this.SetUpDresser();
            LocalizedText name = CreateMapEntryName();
            AddMapEntry(new Color(43, 19, 42), name);
            TileID.Sets.DisableSmartCursor[Type] = true;
            AdjTiles = new int[] { TileID.Dressers };
        }

        public override LocalizedText DefaultContainerName(int frameX, int frameY) => CreateMapEntryName();

        public override bool CreateDust(int i, int j, ref int type)
        {
            Dust.NewDust(new Vector2(i, j) * 16f, 16, 16, 8, 0f, 0f, 1, new Color(255, 255, 255), 1f);
            return false;
        }

        public override bool HasSmartInteract(int i, int j, SmartInteractScanSettings settings) => true;

        public override bool RightClick(int i, int j) => CalamityUtils.DresserRightClick();

        public override void MouseOverFar(int i, int j) => CalamityUtils.DresserMouseFar<SacrilegiousDresser>();

        public override void MouseOver(int i, int j) => CalamityUtils.DresserMouseOver<SacrilegiousDresser>();

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            Chest.DestroyChest(i, j);
        }
    }
}
