using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Tiles.FurnitureAshen
{
    public class AshenMonolith : ModTile
    {
        public override void SetDefaults()
        {
            this.SetUpClock(true);
            ModTranslation name = CreateMapEntryName();
            AddMapEntry(new Color(191, 142, 111), name);
            name.SetDefault("Ashen Monolith");
            adjTiles = new int[] { TileID.GrandfatherClocks };
        }
        int animationFrameWidth = 36;

        public override bool HasSmartInteract()
        {
            return true;
        }

        public override bool CreateDust(int i, int j, ref int type)
        {
            Dust.NewDust(new Vector2(i, j) * 16f, 16, 16, 60, 0f, 0f, 1, new Color(255, 255, 255), 1f);
            Dust.NewDust(new Vector2(i, j) * 16f, 16, 16, 1, 0f, 0f, 1, new Color(100, 100, 100), 1f);
            return false;
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            r = 1f;
            g = 0.5f;
            b = 0.5f;
        }

        public override bool NewRightClick(int x, int y)
        {
            return CalamityUtils.ClockRightClick();
        }

        public override void NearbyEffects(int i, int j, bool closer)
        {
            if (closer)
            {
                Main.clock = true;
            }
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            Item.NewItem(i * 16, j * 16, 48, 32, ModContent.ItemType<Items.Placeables.FurnitureAshen.AshenMonolith>());
        }

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            //This is used to draw the eye, where the frame is changed depending on the player's position relative to the eye's centre.
            Tile currentTile = Main.tile[i, j];
            Vector2 eyeCentre = new Vector2(i * 16, j * 16);
            if (currentTile.frameX == 0)
            { eyeCentre += new Vector2(16f, 0f); }
            if (currentTile.frameY == 0)
            { eyeCentre += new Vector2(0f, 16f); }
            //The eye should track the closest player
            Vector2 playerPos = eyeCentre;
            float distanceToTarget = 9999f;
            for (int x = 0; x < Main.player.Length; x++)
            {
                if ((Main.player[x].position - eyeCentre).Length() < distanceToTarget)
                {
                    playerPos = Main.player[x].position;
                    distanceToTarget = (Main.player[x].position - eyeCentre).Length();
                }
            }
            //Use this to determine which eye frame to draw
            //Default positions for the eye (This is for the pupil being centred)
            int frameX = 5;
            int frameY = 2;
            //horizontal and vertical range for the eye before it reaches maximum value (in tiles). Used to get the various 'magnitudes' of the eye to work
            int xRange = 0;
            if (distanceToTarget > 250)
            {
                xRange = 5;
            }
            else if (distanceToTarget > 170)
            {
                xRange = 4;
            }
            else if (distanceToTarget > 100)
            {
                xRange = 3;
            }
            else if (distanceToTarget > 40)
            {
                xRange = 2;
            }
            else if (distanceToTarget > 10)
            {
                xRange = 1;
            }
            int yRange = 0;
            if (distanceToTarget > 170)
            {
                yRange = 2;
            }
            else if (distanceToTarget > 10)
            {
                yRange = 1;
            }

            //Attempt to use this to get the eye to look in the direction of the player
            Vector2 eyeToPlayer = (playerPos - eyeCentre) / 16;
            eyeToPlayer.X = (int)eyeToPlayer.X;
            eyeToPlayer.Y = (int)eyeToPlayer.Y;

            float factor = Math.Abs((Math.Abs(eyeToPlayer.X) >= 2 * Math.Abs(eyeToPlayer.Y)) ? eyeToPlayer.X / xRange : eyeToPlayer.Y / yRange);
            if (factor != 0)
            { eyeToPlayer /= factor; }
            frameX += (int)eyeToPlayer.X;
            frameY += (int)eyeToPlayer.Y;
            frameX *= animationFrameWidth;
            frameY *= 90;
            Vector2 zero = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange);
            Vector2 drawOffset = new Vector2(i * 16 - Main.screenPosition.X, j * 16 - Main.screenPosition.Y) + zero;
            Texture2D eyeSheet = ModContent.GetTexture("CalamityMod/Tiles/FurnitureAshen/AshenMonolith_Eye");
            spriteBatch.Draw
            (
                eyeSheet,
                drawOffset,
                new Rectangle(frameX + currentTile.frameX, frameY + currentTile.frameY, 16, 16),
                new Color(255, 255, 255, 255),
                0,
                new Vector2(0f, 0f),
                1,
                SpriteEffects.None,
                0f
            );
        }
    }
}
