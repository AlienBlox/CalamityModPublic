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
    public class SacrilegiousDoorOpen : ModTile
    {
        public override void SetStaticDefaults()
        {
            this.SetUpDoorOpen(true);
            AddMapEntry(new Color(43, 19, 42), Language.GetText("MapObject.Door"));
            TileID.Sets.DisableSmartCursor[Type] = true;
            AdjTiles = new int[] { TileID.OpenDoor };
            TileID.Sets.CloseDoorID[Type] = ModContent.TileType<SacrilegiousDoorClosed>();
        }

        public override bool CreateDust(int i, int j, ref int type)
        {
            Dust.NewDust(new Vector2(i, j) * 16f, 16, 16, 8, 0f, 0f, 1, new Color(255, 255, 255), 1f);
            return false;
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }

        public override bool HasSmartInteract(int i, int j, SmartInteractScanSettings settings) => true;

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            Item.NewItem(new EntitySource_TileBreak(i, j), i * 16, j * 16, 32, 48, ModContent.ItemType<SacrilegiousDoor>());
        }

        public override void MouseOver(int i, int j)
        {
            Player player = Main.LocalPlayer;
            player.noThrow = 2;
            player.cursorItemIconEnabled = true;
            player.cursorItemIconID = ModContent.ItemType<SacrilegiousDoor>();
        }
    }
}
