using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Graphics;
using System;
using System.Reflection;
using System.Text;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod
{
	public static partial class CalamityUtils
	{
		public static readonly Color[] ExoPalette = new Color[]
        {
            new Color(250, 255, 112),
            new Color(211, 235, 108),
            new Color(166, 240, 105),
            new Color(105, 240, 220),
            new Color(64, 130, 145),
            new Color(145, 96, 145),
            new Color(242, 112, 73),
            new Color(199, 62, 62),
        };

		#region Projectile Afterimages
		/// <summary>
		/// Draws a projectile as a series of afterimages. The first of these afterimages is centered on the center of the projectile's hitbox.<br />
		/// This function is guaranteed to draw the projectile itself, even if it has no afterimages and/or the Afterimages config option is turned off.
		/// </summary>
		/// <param name="proj">The projectile to be drawn.</param>
		/// <param name="mode">The type of afterimage drawing code to use. Vanilla Terraria has three options: 0, 1, and 2.</param>
		/// <param name="lightColor">The light color to use for the afterimages.</param>
		/// <param name="typeOneIncrement">If mode 1 is used, this controls the loop increment. Set it to more than 1 to skip afterimages.</param>
		/// <param name="texture">The texture to draw. Set to <b>null</b> to draw the projectile's own loaded texture.</param>
		/// <param name="drawCentered">If <b>false</b>, the afterimages will be centered on the projectile's position instead of its own center.</param>
		public static void DrawAfterimagesCentered(Projectile proj, int mode, Color lightColor, int typeOneIncrement = 1, Texture2D texture = null, bool drawCentered = true)
		{
			if (texture is null)
				texture = Main.projectileTexture[proj.type];

			int frameHeight = texture.Height / Main.projFrames[proj.type];
			int frameY = frameHeight * proj.frame;
			float scale = proj.scale;
			float rotation = proj.rotation;

			Rectangle rectangle = new Rectangle(0, frameY, texture.Width, frameHeight);
			Vector2 origin = rectangle.Size() / 2f;

			SpriteEffects spriteEffects = SpriteEffects.None;
			if (proj.spriteDirection == -1)
				spriteEffects = SpriteEffects.FlipHorizontally;

			// If no afterimages are drawn due to an invalid mode being specified, ensure the projectile itself is drawn anyway.
			bool failedToDrawAfterimages = false;

			if (CalamityConfig.Instance.Afterimages)
			{
				Vector2 centerOffset = drawCentered ? proj.Size / 2f : Vector2.Zero;
				switch (mode)
				{
					// Standard afterimages. No customizable features other than total afterimage count.
					// Type 0 afterimages linearly scale down from 100% to 0% opacity. Their color and lighting is equal to the main projectile's.
					case 0:
						for (int i = 0; i < proj.oldPos.Length; ++i)
						{
							Vector2 drawPos = proj.oldPos[i] + centerOffset - Main.screenPosition + new Vector2(0f, proj.gfxOffY);
							// DO NOT REMOVE THESE "UNNECESSARY" FLOAT CASTS. THIS WILL BREAK THE AFTERIMAGES.
							Color color = proj.GetAlpha(lightColor) * ((float)(proj.oldPos.Length - i) / (float)proj.oldPos.Length);
							Main.spriteBatch.Draw(texture, drawPos, new Rectangle?(rectangle), color, rotation, origin, scale, spriteEffects, 0f);
						}
						break;

					// Paladin's Hammer style afterimages. Can be optionally spaced out further by using the typeOneDistanceMultiplier variable.
					// Type 1 afterimages linearly scale down from 66% to 0% opacity. They otherwise do not differ from type 0.
					case 1:
						// Safety check: the loop must increment
						int increment = Math.Max(1, typeOneIncrement);
						Color drawColor = proj.GetAlpha(lightColor);
						int afterimageCount = ProjectileID.Sets.TrailCacheLength[proj.type];
						int k = 0;
						while (k < afterimageCount)
						{
							Vector2 drawPos = proj.oldPos[k] + centerOffset - Main.screenPosition + new Vector2(0f, proj.gfxOffY);
							// DO NOT REMOVE THESE "UNNECESSARY" FLOAT CASTS EITHER.
							if (k > 0)
							{
								float colorMult = (float)(afterimageCount - k);
								drawColor *= colorMult / ((float)afterimageCount * 1.5f);
							}
							Main.spriteBatch.Draw(texture, drawPos, new Rectangle?(rectangle), drawColor, rotation, origin, scale, spriteEffects, 0f);
							k += increment;
						}
						break;

					// Standard afterimages with rotation. No customizable features other than total afterimage count.
					// Type 2 afterimages linearly scale down from 100% to 0% opacity. Their color and lighting is equal to the main projectile's.
					case 2:
						for (int i = 0; i < proj.oldPos.Length; ++i)
						{
							float afterimageRot = proj.oldRot[i];
							SpriteEffects sfxForThisAfterimage = proj.oldSpriteDirection[i] == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

							Vector2 drawPos = proj.oldPos[i] + centerOffset - Main.screenPosition + new Vector2(0f, proj.gfxOffY);
							// DO NOT REMOVE THESE "UNNECESSARY" FLOAT CASTS. THIS WILL BREAK THE AFTERIMAGES.
							Color color = proj.GetAlpha(lightColor) * ((float)(proj.oldPos.Length - i) / (float)proj.oldPos.Length);
							Main.spriteBatch.Draw(texture, drawPos, new Rectangle?(rectangle), color, afterimageRot, origin, scale, sfxForThisAfterimage, 0f);
						}
						break;

					default:
						failedToDrawAfterimages = true;
						break;
				}
			}

			// Draw the projectile itself. Only do this if no afterimages are drawn because afterimage 0 is the projectile itself.
			if (!CalamityConfig.Instance.Afterimages || ProjectileID.Sets.TrailCacheLength[proj.type] <= 0 || failedToDrawAfterimages)
			{
				Vector2 startPos = drawCentered ? proj.Center : proj.position;
				Main.spriteBatch.Draw(texture, startPos - Main.screenPosition + new Vector2(0f, proj.gfxOffY), rectangle, proj.GetAlpha(lightColor), rotation, origin, scale, spriteEffects, 0f);
			}
		}

		// Used for bullets. This lets you draw afterimages while keeping the hitbox at the front of the projectile.
		// This supports type 0 and type 2 afterimages. Vanilla bullets never have type 2 afterimages.
		public static void DrawAfterimagesFromEdge(Projectile proj, int mode, Color lightColor, Texture2D texture = null)
		{
			if (texture is null)
				texture = Main.projectileTexture[proj.type];

			int frameHeight = texture.Height / Main.projFrames[proj.type];
			int frameY = frameHeight * proj.frame;
			float scale = proj.scale;
			float rotation = proj.rotation;

			Rectangle rectangle = new Rectangle(0, frameY, texture.Width, frameHeight);

			SpriteEffects spriteEffects = SpriteEffects.None;
			if (proj.spriteDirection == -1)
				spriteEffects = SpriteEffects.FlipHorizontally;

			Vector2 drawOrigin = new Vector2(texture.Width * 0.5f, proj.height * 0.5f);

			switch (mode)
			{
				default: // If you specify an afterimage mode other than 0 or 2, you get nothing.
					return;

				// Standard afterimages. No customizable features other than total afterimage count.
				// Type 0 afterimages linearly scale down from 100% to 0% opacity. Their color and lighting is equal to the main projectile's.
				case 0:
					for (int i = 0; i < proj.oldPos.Length; ++i)
					{
						Vector2 drawPos = proj.oldPos[i] + drawOrigin - Main.screenPosition + new Vector2(0f, proj.gfxOffY);
						// DO NOT REMOVE THESE "UNNECESSARY" FLOAT CASTS. THIS WILL BREAK THE AFTERIMAGES.
						Color color = proj.GetAlpha(lightColor) * ((float)(proj.oldPos.Length - i) / (float)proj.oldPos.Length);
						Main.spriteBatch.Draw(texture, drawPos, new Rectangle?(rectangle), color, rotation, drawOrigin, scale, spriteEffects, 0f);
					}
					return;

				// Standard afterimages with rotation. No customizable features other than total afterimage count.
				// Type 2 afterimages linearly scale down from 100% to 0% opacity. Their color and lighting is equal to the main projectile's.
				case 2:
					for (int i = 0; i < proj.oldPos.Length; ++i)
					{
						float afterimageRot = proj.oldRot[i];
						SpriteEffects sfxForThisAfterimage = proj.oldSpriteDirection[i] == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

						Vector2 drawPos = proj.oldPos[i] + drawOrigin - Main.screenPosition + new Vector2(0f, proj.gfxOffY);
						// DO NOT REMOVE THESE "UNNECESSARY" FLOAT CASTS. THIS WILL BREAK THE AFTERIMAGES.
						Color color = proj.GetAlpha(lightColor) * ((float)(proj.oldPos.Length - i) / (float)proj.oldPos.Length);
						Main.spriteBatch.Draw(texture, drawPos, new Rectangle?(rectangle), color, afterimageRot, drawOrigin, scale, sfxForThisAfterimage, 0f);
					}
					return;
			}
		}
		#endregion

		/// <summary>
		/// Sets a <see cref="SpriteBatch"/>'s <see cref="BlendState"/> arbitrarily.
		/// </summary>
		/// <param name="spriteBatch">The sprite batch.</param>
		/// <param name="blendState">The blend state to use.</param>
		public static void SetBlendState(this SpriteBatch spriteBatch, BlendState blendState)
		{
			spriteBatch.End();
			spriteBatch.Begin(SpriteSortMode.Immediate, blendState, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
		}

		// Cached for efficiency purposes.
		internal static readonly FieldInfo BeginEndPairField = typeof(SpriteBatch).GetField("inBeginEndPair", BindingFlags.NonPublic | BindingFlags.Instance);

		/// <summary>
		/// Determines if a <see cref="SpriteBatch"/> is in a lock due to a <see cref="SpriteBatch.Begin"/> call.
		/// </summary>
		/// <param name="spriteBatch">The sprite batch to check.</param>
		public static bool HasBeginBeenCalled(this SpriteBatch spriteBatch)
		{
			return (bool)BeginEndPairField.GetValue(spriteBatch);
		}

		/// <summary>
		/// Draws text with an eight-way one-pixel offset border.
		/// </summary>
		/// <param name="sb"></param>
		/// <param name="font"></param>
		/// <param name="text"></param>
		/// <param name="baseDrawPosition"></param>
		/// <param name="main"></param>
		/// <param name="border"></param>
		/// <param name="scale"></param>
		public static void DrawBorderStringEightWay(SpriteBatch sb, DynamicSpriteFont font, string text, Vector2 baseDrawPosition, Color main, Color border, float scale = 1f)
		{
			for (int x = -1; x <= 1; x++)
			{
				for (int y = -1; y <= 1; y++)
				{
					Vector2 drawPosition = baseDrawPosition + new Vector2(x, y);
					if (x == 0 && y == 0)
						continue;

					DynamicSpriteFontExtensionMethods.DrawString(sb, font, text, drawPosition, border, 0f, default, scale, SpriteEffects.None, 0f);
				}
			}
			DynamicSpriteFontExtensionMethods.DrawString(sb, font, text, baseDrawPosition, main, 0f, default, scale, SpriteEffects.None, 0f);
		}

		public static void DrawItemGlowmaskSingleFrame(this Item item, SpriteBatch spriteBatch, float rotation, Texture2D glowmaskTexture)
		{
			Vector2 origin = new Vector2(glowmaskTexture.Width / 2f, glowmaskTexture.Height / 2f - 2f);
			spriteBatch.Draw(glowmaskTexture, item.Center - Main.screenPosition, null, Color.White, rotation, origin, 1f, SpriteEffects.None, 0f);
		}

		public static Rectangle GetCurrentFrame(this Item item, ref int frame, ref int frameCounter, int frameDelay, int frameAmt, bool frameCounterUp = true)
		{
			if (frameCounter >= frameDelay)
			{
				frameCounter = -1;
				frame = frame == frameAmt - 1 ? 0 : frame + 1;
			}
			if (frameCounterUp)
				frameCounter++;
			return new Rectangle(0, item.height * frame, item.width, item.height);
		}

		public static bool DrawFishingLine(this Projectile projectile, int fishingRodType, Color poleColor, int xPositionAdditive = 45, float yPositionAdditive = 35f)
		{
			Player player = Main.player[projectile.owner];
			Item item = player.HeldItem;
			if (!projectile.bobber || item.holdStyle <= 0)
				return false;

			float originX = player.MountedCenter.X;
			float originY = player.MountedCenter.Y;
			originY += player.gfxOffY;
			//This variable is used to account for Gravitation Potions
			float gravity = player.gravDir;

			if (item.type == fishingRodType)
			{
				originX += (float)(xPositionAdditive * player.direction);
				if (player.direction < 0)
				{
					originX -= 13f;
				}
				originY -= yPositionAdditive * gravity;
			}

			if (gravity == -1f)
			{
				originY -= 12f;
			}
			Vector2 mountedCenter = new Vector2(originX, originY);
			mountedCenter = player.RotatedRelativePoint(mountedCenter + new Vector2(8f), true) - new Vector2(8f);
			Vector2 lineOrigin = projectile.Center - mountedCenter;
			bool canDraw = true;
			if (lineOrigin.X == 0f && lineOrigin.Y == 0f)
				return false;

			float projPosMagnitude = lineOrigin.Length();
			projPosMagnitude = 12f / projPosMagnitude;
			lineOrigin.X *= projPosMagnitude;
			lineOrigin.Y *= projPosMagnitude;
			mountedCenter -= lineOrigin;
			lineOrigin = projectile.Center - mountedCenter;

			while (canDraw)
			{
				float height = 12f;
				float positionMagnitude = lineOrigin.Length();
				if (float.IsNaN(positionMagnitude) || float.IsNaN(positionMagnitude))
					break;

				if (positionMagnitude < 20f)
				{
					height = positionMagnitude - 8f;
					canDraw = false;
				}
				positionMagnitude = 12f / positionMagnitude;
				lineOrigin.X *= positionMagnitude;
				lineOrigin.Y *= positionMagnitude;
				mountedCenter += lineOrigin;
				lineOrigin = projectile.Center - mountedCenter;
				if (positionMagnitude > 12f)
				{
					float positionInverseMultiplier = 0.3f;
					float absVelocitySum = Math.Abs(projectile.velocity.X) + Math.Abs(projectile.velocity.Y);
					if (absVelocitySum > 16f)
					{
						absVelocitySum = 16f;
					}
					absVelocitySum = 1f - absVelocitySum / 16f;
					positionInverseMultiplier *= absVelocitySum;
					absVelocitySum = positionMagnitude / 80f;
					if (absVelocitySum > 1f)
					{
						absVelocitySum = 1f;
					}
					positionInverseMultiplier *= absVelocitySum;
					if (positionInverseMultiplier < 0f)
					{
						positionInverseMultiplier = 0f;
					}
					absVelocitySum = 1f - projectile.localAI[0] / 100f;
					positionInverseMultiplier *= absVelocitySum;
					if (lineOrigin.Y > 0f)
					{
						lineOrigin.Y *= 1f + positionInverseMultiplier;
						lineOrigin.X *= 1f - positionInverseMultiplier;
					}
					else
					{
						absVelocitySum = Math.Abs(projectile.velocity.X) / 3f;
						if (absVelocitySum > 1f)
						{
							absVelocitySum = 1f;
						}
						absVelocitySum -= 0.5f;
						positionInverseMultiplier *= absVelocitySum;
						if (positionInverseMultiplier > 0f)
						{
							positionInverseMultiplier *= 2f;
						}
						lineOrigin.Y *= 1f + positionInverseMultiplier;
						lineOrigin.X *= 1f - positionInverseMultiplier;
					}
				}
				//This color decides the color of the fishing line.
				Color lineColor = Lighting.GetColor((int)mountedCenter.X / 16, (int)mountedCenter.Y / 16, poleColor);
				float rotation = lineOrigin.ToRotation() - MathHelper.PiOver2;

				Main.spriteBatch.Draw(Main.fishingLineTexture, new Vector2(mountedCenter.X - Main.screenPosition.X + Main.fishingLineTexture.Width * 0.5f, mountedCenter.Y - Main.screenPosition.Y + Main.fishingLineTexture.Height * 0.5f), new Rectangle?(new Rectangle(0, 0, Main.fishingLineTexture.Width, (int)height)), lineColor, rotation, new Vector2(Main.fishingLineTexture.Width * 0.5f, 0f), 1f, SpriteEffects.None, 0f);
			}
			return false;
		}

		public static void DrawHook(this Projectile projectile, Texture2D hookTexture, float angleAdditive = 0f)
		{
			Player player = Main.player[projectile.owner];
			Vector2 center = projectile.Center;
			float angleToMountedCenter = projectile.AngleTo(player.MountedCenter) - MathHelper.PiOver2;
			bool canShowHook = true;
			while (canShowHook)
			{
				float distanceMagnitude = (player.MountedCenter - center).Length(); //Exact same as using a Sqrt
				if (distanceMagnitude < hookTexture.Height + 1f)
				{
					canShowHook = false;
				}
				else if (float.IsNaN(distanceMagnitude))
				{
					canShowHook = false;
				}
				else
				{
					center += projectile.SafeDirectionTo(player.MountedCenter) * hookTexture.Height;
					Color tileAtCenterColor = Lighting.GetColor((int)center.X / 16, (int)(center.Y / 16f));
					Main.spriteBatch.Draw(hookTexture, center - Main.screenPosition,
						new Rectangle?(new Rectangle(0, 0, hookTexture.Width, hookTexture.Height)),
						tileAtCenterColor, angleToMountedCenter + angleAdditive,
						hookTexture.Size() / 2, 1f, SpriteEffects.None, 0f);
				}
			}
		}

		internal static void IterateDisco(ref Color c, ref float aiParam, in byte discoIter = 7)
		{
			switch (aiParam)
			{
				case 0f:
					c.G += discoIter;
					if (c.G >= 255)
					{
						c.G = 255;
						aiParam = 1f;
					}
					break;
				case 1f:
					c.R -= discoIter;
					if (c.R <= 0)
					{
						c.R = 0;
						aiParam = 2f;
					}
					break;
				case 2f:
					c.B += discoIter;
					if (c.B >= 255)
					{
						c.B = 255;
						aiParam = 3f;
					}
					break;
				case 3f:
					c.G -= discoIter;
					if (c.G <= 0)
					{
						c.G = 0;
						aiParam = 4f;
					}
					break;
				case 4f:
					c.R += discoIter;
					if (c.R >= 255)
					{
						c.R = 255;
						aiParam = 5f;
					}
					break;
				case 5f:
					c.B -= discoIter;
					if (c.B <= 0)
					{
						c.B = 0;
						aiParam = 0f;
					}
					break;
				default:
					aiParam = 0f;
					c = Color.Red;
					break;
			}
		}

		public static string ColorMessage(string msg, Color color)
		{
			StringBuilder sb;
			if (!msg.Contains("\n"))
			{
				sb = new StringBuilder(msg.Length + 12);
				sb.Append("[c/").Append(color.Hex3()).Append(':').Append(msg).Append(']');
			}
			else
			{
				sb = new StringBuilder();
				foreach (string newlineSlice in msg.Split('\n'))
					sb.Append("[c/").Append(color.Hex3()).Append(':').Append(newlineSlice).Append(']').Append('\n');
			}
			return sb.ToString();
		}

		/// <summary>
		/// Returns a color lerp that allows for smooth transitioning between two given colors
		/// </summary>
		/// <param name="firstColor">The first color you want it to switch between</param>
		/// <param name="secondColor">The second color you want it to switch between</param>
		/// <param name="seconds">How long you want it to take to swap between colors</param>
		public static Color ColorSwap(Color firstColor, Color secondColor, float seconds)
		{
			double timeMult = (double)(MathHelper.TwoPi / seconds);
			float colorMePurple = (float)((Math.Sin(timeMult * Main.GlobalTime) + 1) * 0.5f);
			return Color.Lerp(firstColor, secondColor, colorMePurple);
		}
		/// <summary>
		/// Returns a color lerp that supports multiple colors.
		/// </summary>
		/// <param name="increment">The 0-1 incremental value used when interpolating.</param>
		/// <param name="colors">The various colors to interpolate across.</param>
		/// <returns></returns>
		public static Color MulticolorLerp(float increment, params Color[] colors)
		{
			increment %= 0.999f;
			int currentColorIndex = (int)(increment * colors.Length);
			Color currentColor = colors[currentColorIndex];
			Color nextColor = colors[(currentColorIndex + 1) % colors.Length];
			return Color.Lerp(currentColor, nextColor, increment * colors.Length % 1f);
		}

		// Cached for efficiency purposes.
		internal static readonly FieldInfo UImageField = typeof(MiscShaderData).GetField("_uImage", BindingFlags.NonPublic | BindingFlags.Instance);

		/// <summary>
		/// Manually sets the texture of a <see cref="MiscShaderData"/> instance, since vanilla's implementation only supports strings that access vanilla textures.
		/// </summary>
		/// <param name="shader">The shader to bind the texture to.</param>
		/// <param name="texture">The texture to bind.</param>
		public static void SetShaderTexture(this MiscShaderData shader, Texture2D texture) => UImageField.SetValue(shader, new Ref<Texture2D>(texture));

		public static void EnterShaderRegion(this SpriteBatch spriteBatch)
		{
			spriteBatch.End();
			spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.instance.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
		}

		public static void ExitShaderRegion(this SpriteBatch spriteBatch)
		{
			spriteBatch.End();
			spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.instance.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
		}

		public static void DrawAuroras(Player player, float auroraCount, float opacity, Color color)
		{
            float time = Main.GlobalTime % 3f / 3f;
            Texture2D auroraTexture = ModContent.GetTexture("CalamityMod/ExtraTextures/AuroraTexture");
            for (int i = 0; i < auroraCount; i++)
            {
                float incrementOffsetAngle = MathHelper.TwoPi * i / auroraCount;
                float xOffset = (float)Math.Sin(time * MathHelper.TwoPi + incrementOffsetAngle * 2f) * 20f;
                float yOffset = (float)Math.Sin(time * MathHelper.TwoPi + incrementOffsetAngle * 2f + MathHelper.ToRadians(60f)) * 6f;
                float rotation = (float)Math.Sin(incrementOffsetAngle) * MathHelper.Pi / 12f;
                Vector2 offset = new Vector2(xOffset, yOffset - 14f);
                DrawData drawData = new DrawData(auroraTexture,
                                 player.Top + offset - Main.screenPosition,
                                 null,
                                 color * opacity,
                                 rotation + MathHelper.PiOver2,
                                 auroraTexture.Size() * 0.5f,
                                 0.135f,
                                 SpriteEffects.None,
                                 1);
                Main.playerDrawData.Add(drawData);
            }
		}
	}
}
