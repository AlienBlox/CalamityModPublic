using CalamityMod.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Tiles.FurnitureAstral
{
    public class MonolithChest : ModTile
    {
        public override void SetDefaults()
        {
            this.SetUpChest();
            ModTranslation name = CreateMapEntryName();
            name.SetDefault("Monolith Chest");
            AddMapEntry(new Color(191, 142, 111), name, MapChestName);
            disableSmartCursor = true;
            adjTiles = new int[] { TileID.Containers };
            chest = "Monolith Chest";
            chestDrop = ModContent.ItemType<Items.Placeables.FurnitureAstral.MonolithChest>();
        }

        public override bool CreateDust(int i, int j, ref int type)
        {
            Dust.NewDust(new Vector2(i, j) * 16f, 16, 16, ModContent.DustType<AstralBasic>(), 0f, 0f, 1, new Color(255, 255, 255), 1f);
            return false;
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            int xPos = Main.tile[i, j].frameX;
            int yPos = Main.tile[i, j].frameY;
            Texture2D glowmask = ModContent.GetTexture("CalamityMod/Tiles/FurnitureAstral/MonolithChestGlow");
            Color drawColour = GetDrawColour(i, j, new Color(100, 100, 100, 100));
            Vector2 zero = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange);
            Vector2 drawOffset = new Vector2(i * 16 - Main.screenPosition.X, j * 16 - Main.screenPosition.Y) + zero;
            Main.spriteBatch.Draw(glowmask, drawOffset, new Rectangle?(new Rectangle(xPos, yPos, 18, 18)), drawColour, 0.0f, Vector2.Zero, 1f, SpriteEffects.None, 0.0f);
        }

        private Color GetDrawColour(int i, int j, Color colour)
        {
            int colType = Main.tile[i, j].color();
            Color paintCol = WorldGen.paintColor(colType);
            if (colType >= 13 && colType <= 24)
            {
                colour.R = (byte)(paintCol.R / 255f * colour.R);
                colour.G = (byte)(paintCol.G / 255f * colour.G);
                colour.B = (byte)(paintCol.B / 255f * colour.B);
            }
            return colour;
        }

        public override bool HasSmartInteract()
        {
            return true;
        }

        public string MapChestName(string name, int i, int j) => CalamityUtils.GetMapChestName(name, i, j);

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            Item.NewItem(i * 16, j * 16, 32, 32, chestDrop);
            Chest.DestroyChest(i, j);
        }

        public override bool NewRightClick(int i, int j)
        {
            // Glowmask animation & custom sound
            Player player = Main.LocalPlayer;
            Tile tile = Main.tile[i, j];
            Main.mouseRightRelease = false;
            int left = i;
            int top = j;
            if (tile.frameX % 36 != 0)
            {
                left--;
            }
            if (tile.frameY != 0)
            {
                top--;
            }
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                int chest = Chest.FindChest(left, top);
                if (chest >= 0)
                {
                    if (player.chest < 0)
                    {
                        Main.PlaySound(SoundID.NPCKilled, Style: 22, volumeScale: 0.5f);
                    }
                }
            }

            return CalamityUtils.ChestRightClick(i, j);
        }

        public override void MouseOver(int i, int j)
        {
            CalamityUtils.ChestMouseOver<Items.Placeables.FurnitureAstral.MonolithChest>("Monolith Chest", i, j);
        }

        public override void MouseOverFar(int i, int j)
        {
            CalamityUtils.ChestMouseFar<Items.Placeables.FurnitureAstral.MonolithChest>("Monolith Chest", i, j);
        }
    }
}
