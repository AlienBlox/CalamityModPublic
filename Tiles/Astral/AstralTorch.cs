﻿using CalamityMod.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.Tiles.Astral
{
    public class AstralTorch : ModTile
    {
        public override void SetStaticDefaults()
        {
            this.SetUpTorch();
            LocalizedText name = CreateMapEntryName();
            // name.SetDefault("Torch");
            AddMapEntry(new Color(253, 221, 3), name);
            TileID.Sets.DisableSmartCursor[Type] = true;
            ItemDrop = ModContent.ItemType<Items.Placeables.Furniture.AstralTorch>();
            AdjTiles = new int[] { TileID.Torches };
            TileID.Sets.Torch[Type] = true;
            TileID.Sets.FramesOnKillWall[Type] = true;
        }

        // This is required for torches to break underwater
        public override bool CanPlace(int i, int j) => Main.tile[i, j].LiquidAmount <= 0;

        public override bool CreateDust(int i, int j, ref int type)
        {
            Dust.NewDust(new Vector2(i, j) * 16f, 16, 16, 1, 0f, 0f, 1, new Color(255, 95, 48), 1f);
            return false;
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }

        public override void MouseOver(int i, int j)
        {
            Player player = Main.LocalPlayer;
            player.noThrow = 2;
            player.cursorItemIconEnabled = true;
            player.cursorItemIconID = ModContent.ItemType<Items.Placeables.Furniture.AstralTorch>();
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            Tile tile = Main.tile[i, j];

            if (tile.TileFrameX < 66)
            {
                r = 1.6f;
                g = 0.6f;
                b = 0.3f;
            }
        }

        public override void SetDrawPositions(int i, int j, ref int width, ref int offsetY, ref int height, ref short tileFrameX, ref short tileFrameY)
        {
            offsetY = 0;
            if (WorldGen.SolidTile(i, j - 1))
            {
                offsetY = 2;
                if (WorldGen.SolidTile(i - 1, j + 1) || WorldGen.SolidTile(i + 1, j + 1))
                {
                    offsetY = 4;
                }
            }
        }

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            CalamityUtils.DrawFlameEffect(ModContent.Request<Texture2D>("CalamityMod/Tiles/Astral/AstralTorchFlame").Value, i, j, 2);
        }

        public override void DrawEffects(int i, int j, SpriteBatch spriteBatch, ref TileDrawInfo drawData)
        {
            if (Main.tile[i, j].TileFrameX < 66)
                CalamityUtils.DrawFlameSparks(ModContent.DustType<AstralOrange>(), 5, i, j);
        }

        public override bool RightClick(int i, int j)
        {
            CalamityUtils.RightClickBreak(i, j);
            return true;
        }

		public override float GetTorchLuck(Player player)
		{
			// Note: Total Torch luck never goes below zero
			return player.Calamity().ZoneAstral ? 1f : -1f;
		}
    }
}