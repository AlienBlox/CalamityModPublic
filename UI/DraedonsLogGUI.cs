using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.UI
{
	public abstract class DraedonsLogGUI : PopupGUI
    {
        public int Page = 0;
        public int ArrowClickCooldown;
        public bool HoveringOverBook = false;
        public int TotalLinesPerPage => 16;
        public abstract int TotalPages { get; }

        public const int TextStartOffsetX = 40;

        public override void Update()
        {
            if (Active)
            {
                if (FadeTime < FadeTimeMax)
                    FadeTime++;
            }
            else if (FadeTime > 0)
            {
                FadeTime--;
            }

            if (Main.mouseLeft && !HoveringOverBook && FadeTime >= 30)
            {
                Page = 0;
                Active = false;
            }

            if (ArrowClickCooldown > 0)
                ArrowClickCooldown--;
            HoveringOverBook = false;
        }

        public abstract string GetTextByPage();

        public abstract Texture2D GetTextureByPage();

        public override void Draw(SpriteBatch spriteBatch)
        {
            Texture2D pageTexture = ModContent.GetTexture("CalamityMod/ExtraTextures/UI/DraedonsLogPage");
            float xScale = MathHelper.Lerp(0.004f, 1f, FadeTime / (float)FadeTimeMax);
            Vector2 scale = new Vector2(xScale, 1f) * new Vector2(Main.screenWidth, Main.screenHeight) / pageTexture.Size();
            scale.Y *= 1.5f;
            scale *= 0.5f;

            float xResolutionScale = Main.screenWidth / 2560f;
            float yResolutionScale = Main.screenHeight / 1440f;
            float bookScale = 0.5f;
            scale *= bookScale;

            float yPageTop = MathHelper.Lerp(Main.screenHeight * 2, Main.screenHeight * 0.25f, FadeTime / (float)FadeTimeMax);

            Rectangle mouseRectangle = new Rectangle((int)Main.MouseScreen.X, (int)Main.MouseScreen.Y, 2, 2);
            for (int i = 0; i < 2; i++)
            {
                Vector2 drawPosition = new Vector2(Main.screenWidth * 0.5f, yPageTop);
                spriteBatch.Draw(pageTexture,
                                 drawPosition,
                                 null,
                                 Color.White,
                                 0f,
                                 new Vector2(i == 0f ? pageTexture.Width : 0f, 0f),
                                 scale,
                                 i == 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally,
                                 0f);
                if (!HoveringOverBook)
                {
                    Rectangle pageRectangle = new Rectangle((int)drawPosition.X - (int)(pageTexture.Width * scale.X), (int)yPageTop, (int)(pageTexture.Width * scale.X) * 2, (int)(pageTexture.Height * scale.Y));
                    HoveringOverBook = mouseRectangle.Intersects(pageRectangle);
                }
            }

            // Create text and arrows.
            if (FadeTime >= FadeTimeMax - 4 && Active)
            {
                int textWidth = (int)(xScale * pageTexture.Width) - TextStartOffsetX;
                textWidth = (int)(textWidth * xResolutionScale);
                List<string> dialogLines = Utils.WordwrapString(GetTextByPage(), Main.fontMouseText, (int)(textWidth / xResolutionScale), 250, out _).ToList();
                dialogLines.RemoveAll(text => string.IsNullOrEmpty(text));

                int trimmedTextCharacterCount = string.Concat(dialogLines).Length;
                float yOffsetPerLine = 28f;
                if (dialogLines.Count > TotalLinesPerPage)
                    yOffsetPerLine *= string.Concat(dialogLines).Length / GetTextByPage().Length;

                // Ensure the page number doesn't become nonsensical as a result of a resolution change (such as by resizing the game).
                if (Page < 0)
                    Page = 0;
                if (Page > TotalPages)
                    Page = TotalPages;

                DrawArrows(spriteBatch, xResolutionScale, yResolutionScale, yPageTop + 506f * yResolutionScale, mouseRectangle);
                for (int i = 0; i < dialogLines.Count; i++)
                {
                    if (dialogLines[i] != null)
                    {
                        int textDrawPositionX = (int)(pageTexture.Width * xResolutionScale + 350 * xResolutionScale);
                        int textDrawPositionY = (int)(42 * yResolutionScale) + i * (int)(yOffsetPerLine * yResolutionScale) + (int)yPageTop;

                        Utils.DrawBorderStringFourWay(spriteBatch,
                                                      Main.fontMouseText,
                                                      dialogLines[i],
                                                      textDrawPositionX,
                                                      textDrawPositionY,
                                                      Color.DarkCyan,
                                                      Color.Black,
                                                      Vector2.Zero,
                                                      xResolutionScale);
                    }
                }
                DrawSpecialImage(spriteBatch, xResolutionScale, yResolutionScale, yPageTop - 70f * yResolutionScale);
            }
        }

        public void DrawArrows(SpriteBatch spriteBatch, float xResolutionScale, float yResolutionScale, float yPageBottom, Rectangle mouseRectangle)
        {
            float arrowScale = 0.6f;
            Texture2D arrowTexture = ModContent.GetTexture("CalamityMod/ExtraTextures/UI/DraedonsLogArrow");
            if (Page > 0)
            {
                Vector2 drawPosition = new Vector2(Main.screenWidth / 2 - 80f, yPageBottom);
                Rectangle arrowRectangle = new Rectangle((int)drawPosition.X, (int)drawPosition.Y, arrowTexture.Width, arrowTexture.Height);
                arrowRectangle.Width = (int)(arrowRectangle.Width * xResolutionScale * arrowScale);
                arrowRectangle.Height = (int)(arrowRectangle.Height * yResolutionScale * arrowScale);

                if (mouseRectangle.Intersects(arrowRectangle))
                {
                    arrowTexture = ModContent.GetTexture("CalamityMod/ExtraTextures/UI/DraedonsLogArrowHover");
                    if (ArrowClickCooldown <= 0 && Main.mouseLeft)
                    {
                        Page--;
                        ArrowClickCooldown = 8;
                    }
                    Main.blockMouse = true;
                }

                spriteBatch.Draw(arrowTexture, drawPosition, null, Color.White, 0f, Vector2.Zero, new Vector2(xResolutionScale, yResolutionScale) * arrowScale, SpriteEffects.FlipHorizontally, 0f);
            }

            if (Page < TotalPages - 1)
            {
                Vector2 drawPosition = new Vector2(Main.screenWidth / 2 + 40f, yPageBottom);
                Rectangle arrowRectangle = new Rectangle((int)drawPosition.X, (int)drawPosition.Y, arrowTexture.Width, arrowTexture.Height);
                arrowRectangle.Width = (int)(arrowRectangle.Width * xResolutionScale);
                arrowRectangle.Height = (int)(arrowRectangle.Height * yResolutionScale);

                if (mouseRectangle.Intersects(arrowRectangle))
                {
                    arrowTexture = ModContent.GetTexture("CalamityMod/ExtraTextures/UI/DraedonsLogArrowHover");
                    if (ArrowClickCooldown <= 0 && Main.mouseLeft)
                    {
                        Page++;
                        ArrowClickCooldown = 8;
                    }
                    Main.blockMouse = true;
                }

                spriteBatch.Draw(arrowTexture, drawPosition, null, Color.White, 0f, Vector2.Zero, new Vector2(xResolutionScale, yResolutionScale) * arrowScale, SpriteEffects.None, 0f);
            }
        }

        public void DrawSpecialImage(SpriteBatch spriteBatch, float xResolutionScale, float yResolutionScale, float yPageTop)
        {
            Texture2D texture = GetTextureByPage();

            // Don't draw at all if the inputted texture is null.
            if (texture is null)
                return;
            float yAspectRatio = texture.Height / (float)texture.Width;
            Vector2 drawPosition;
            drawPosition.X = Main.screenWidth / 2 + (int)(285f * xResolutionScale);
            drawPosition.Y = yPageTop;
            Vector2 scale = new Vector2(360f * xResolutionScale, MathHelper.Clamp(360f * yAspectRatio * yResolutionScale, 330f, 400f));
            scale /= texture.Size();
            drawPosition.Y += texture.Height * 0.4f * scale.Y;
            spriteBatch.Draw(texture, drawPosition, null, Color.White, 0f, new Vector2(texture.Width * 0.5f, 0f), scale, SpriteEffects.None, 0f);
        }
    }
}
