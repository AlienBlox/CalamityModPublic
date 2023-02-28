﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Terraria.DataStructures;
using Terraria.Enums;

namespace CalamityMod.Tiles.Crags
{
    public class GloomTorch : ModTile
    {
        public override void SetStaticDefaults()
        {
            this.SetUpTorch();
            ModTranslation name = CreateMapEntryName();
            name.SetDefault("Torch");
            AddMapEntry(new Color(253, 221, 3), name);
            TileID.Sets.DisableSmartCursor[Type] = true;
            TileID.Sets.Torch[Type] = true;
            TileID.Sets.FramesOnKillWall[Type] = true;
            ItemDrop = ModContent.ItemType<Items.Placeables.Furniture.GloomTorch>();
            AdjTiles = new int[] { TileID.Torches };
        }

        // This is required for torches to break underwater
        public override bool CanPlace(int i, int j) => Main.tile[i, j].LiquidAmount <= 0;

        public override bool CreateDust(int i, int j, ref int type)
        {
            Dust.NewDust(new Vector2(i, j) * 16f, 16, 16, 1, 0f, 0f, 1, new Color(190, 255, 60), 1f);
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
            player.cursorItemIconID = ModContent.ItemType<Items.Placeables.Furniture.GloomTorch>();
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            Tile tile = Main.tile[i, j];

            if (tile.TileFrameX < 66)
            {
                r = 0.5f;
                g = 0.75f;
                b = 1.2f;
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
            CalamityUtils.DrawFlameEffect(ModContent.Request<Texture2D>("CalamityMod/Tiles/Crags/GloomTorchFlame").Value, i, j, 2);
        }

        public override void DrawEffects(int i, int j, SpriteBatch spriteBatch, ref TileDrawInfo drawData)
        {
            if (Main.tile[i, j].TileFrameX < 66)
                CalamityUtils.DrawFlameSparks(Main.rand.NextBool() ? 61 : 64, 5, i, j);
        }

        public override bool RightClick(int i, int j)
        {
            CalamityUtils.RightClickBreak(i, j);
            return true;
        }

		public override float GetTorchLuck(Player player)
		{
			// Note: Total Torch luck never goes below zero
			return player.Calamity().ZoneCalamity ? 1f : -1f;
		}
    }
}
