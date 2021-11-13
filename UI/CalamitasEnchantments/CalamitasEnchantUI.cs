using CalamityMod.Items;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria.UI.Chat;

namespace CalamityMod.UI.CalamitasEnchants
{
	public class CalamitasEnchantUI
	{
		public static int NPCIndex = -1;
		public static int EnchantIndex = 0;
		public static Enchantment? SelectedEnchantment = null;
		public static Item CurrentlyHeldItem = new Item();
		public static float TopButtonClickCountdown = 0f;
		public static float BottomButtonClickCountdown = 0f;
		public static float ReforgeButtonClickCountdown = 0f;

		public static bool CurrentlyViewing = false;

		public static readonly Vector2 ReforgeUITopLeft = new Vector2(68f, 320f);
		public static readonly float ResolutionRatio = Main.screenHeight / 1440f;

		public static Rectangle MouseScreenArea => Utils.CenteredRectangle(Main.MouseScreen, Vector2.One * 2f);

		public static bool InRangeOfNPC()
		{
			// Don't bother trying if no valid NPC has been selected yet.
			if (!Main.npc.IndexInRange(NPCIndex) || !Main.npc[NPCIndex].active)
				return false;

			Rectangle validTalkArea = Utils.CenteredRectangle(Main.LocalPlayer.Center, new Vector2(Player.tileRangeX * 3f, Player.tileRangeY * 2f) * 16f);
			return validTalkArea.Intersects(Main.npc[NPCIndex].Hitbox);
		}

		public static void Draw(SpriteBatch spriteBatch)
		{
			// Decrement click cooldowns.
			// This is done so that click textures do not instantly disappear.
			if (TopButtonClickCountdown > 0f)
				TopButtonClickCountdown--;
			if (BottomButtonClickCountdown > 0f)
				BottomButtonClickCountdown--;
			if (ReforgeButtonClickCountdown > 0f)
				ReforgeButtonClickCountdown--;

			// Don't bother doing anything except resetting if not looking at the UI.
			if (!CurrentlyViewing)
			{
				// If an item was stored, release it back into the world.
				if (!CurrentlyHeldItem.IsAir)
				{
					Main.LocalPlayer.QuickSpawnClonedItem(CurrentlyHeldItem, CurrentlyHeldItem.stack);
					CurrentlyHeldItem.TurnToAir();
				}

				EnchantIndex = 0;
				NPCIndex = -1;
				return;
			}

			// Check if the player can still be in the UI.
			if (Main.LocalPlayer.chest != -1 || Main.LocalPlayer.sign != -1 || Main.LocalPlayer.talkNPC == -1 || !Main.playerInventory || !InRangeOfNPC() || Main.InGuideCraftMenu)
			{
				CurrentlyViewing = false;

				// Check if the player has any items being held.
				// If they do, drop it onto the ground.
				Main.LocalPlayer.dropItemCheck();

				// Reload visible recipes in case the dropped item was an ingredient.
				Recipe.FindRecipes();
				return;
			}

			// Open the inventory and stop talking to any NPCs by default while the UI is open, similar to the goblin tinkerer.
			Main.playerInventory = true;
			Main.npcChatText = string.Empty;

			Texture2D backgroundTexture = ModContent.GetTexture("CalamityMod/ExtraTextures/UI/CalamitasCurseBackground");
			Vector2 backgroundScale = Vector2.One * Main.UIScale;

			// Draw the background.
			spriteBatch.Draw(backgroundTexture, ReforgeUITopLeft, null, Color.White, 0f, Vector2.Zero, backgroundScale, SpriteEffects.None, 0f);

			// Prevent the player from say, firing a weapon while the mouse is hovering over the UI.
			DisableMouseWhenOverUI(backgroundTexture, backgroundScale);

			// Select the enchantment to use.
			IEnumerable<Enchantment> possibleEnchantments = SelectEnchantment();

			// Draw the cost and description.
			int cost = 0;
			if (SelectedEnchantment.HasValue)
			{
				Point costDrawPositionTopLeft = (ReforgeUITopLeft + new Vector2(50f, 78f) * backgroundScale).ToPoint();
				cost = DrawEnchantmentCost(spriteBatch, costDrawPositionTopLeft);
				Point descriptionDrawPositionTopLeft = costDrawPositionTopLeft;
				descriptionDrawPositionTopLeft.Y += (int)(Main.UIScale * 70f);
				Vector2 iconDrawPositionTopLeft = costDrawPositionTopLeft.ToVector2() + new Vector2(270f, -24f) * Main.UIScale;

				DrawEnchantmentDescription(spriteBatch, descriptionDrawPositionTopLeft);
				if (!string.IsNullOrEmpty(SelectedEnchantment.Value.IconTexturePath))
                {
					Texture2D iconTexture = ModContent.GetTexture(SelectedEnchantment.Value.IconTexturePath);
					DrawIcon(spriteBatch, iconDrawPositionTopLeft, iconTexture);
                }
			}

			// Draw the item icon.
			Vector2 itemSlotDrawPosition = ReforgeUITopLeft + new Vector2(30f, 50f) * backgroundScale;
			Vector2 reforgeIconDrawPosition = ReforgeUITopLeft + new Vector2(84f, 60f) * backgroundScale;
			DrawItemIcon(spriteBatch, itemSlotDrawPosition, reforgeIconDrawPosition, backgroundScale, out bool isHoveringOverItemIcon, out bool isHoveringOverReforgeIcon);
			if (isHoveringOverItemIcon)
				InteractWithItemSlot();

			// Draw the buttons.
			DrawAndInteractWithButtons(spriteBatch, possibleEnchantments, ReforgeUITopLeft + new Vector2(210f, 50f) * backgroundScale, ReforgeUITopLeft + new Vector2(210f, 90f) * backgroundScale, backgroundScale);

			// Draw the enchantment name.
			if (SelectedEnchantment.HasValue)
				DrawEnchantmentName(spriteBatch, ReforgeUITopLeft + new Vector2(216f, 66f) * backgroundScale);

			// Handle spending logic.
			if (isHoveringOverReforgeIcon)
			{
				if (Main.mouseLeft && Main.mouseLeftRelease)
				{
					InteractWithEnchantIcon(cost);
					ReforgeButtonClickCountdown = 15f;
				}
			}
		}

		public static void DisableMouseWhenOverUI(Texture2D backgroundTexture, Vector2 backgroundScale)
		{
			Rectangle backgroundArea = new Rectangle((int)ReforgeUITopLeft.X, (int)ReforgeUITopLeft.Y, (int)(backgroundTexture.Width * backgroundScale.X), (int)(backgroundTexture.Width * backgroundScale.Y));

			if (MouseScreenArea.Intersects(backgroundArea))
			{
				Main.LocalPlayer.mouseInterface = false;
				Main.blockMouse = true;
			}
		}

		public static IEnumerable<Enchantment> SelectEnchantment()
		{
			IEnumerable<Enchantment> possibleEnchantments = EnchantmentManager.GetValidEnchantmentsForItem(CurrentlyHeldItem);

			// Select the enchantment based on an index in the possible enchantments.
			// If no possible enchantments were found, default to null.
			SelectedEnchantment = null;

			if (possibleEnchantments.Count() > 0)
				SelectedEnchantment = possibleEnchantments.ElementAt(EnchantIndex);

			return possibleEnchantments;
		}

		public static int DrawEnchantmentCost(SpriteBatch spriteBatch, Point costDrawPositionTopLeft)
		{
			if (CurrentlyHeldItem.IsAir)
				return 0;

			int cost = CurrentlyHeldItem.value * 4;

			// Increase the cost of enchanting significantly if doing so would upgrade the item directly.
			if (SelectedEnchantment.HasValue && SelectedEnchantment.Value.Name == EnchantmentManager.UpgradeEnchantName)
				cost = (int)MathHelper.Min(cost, Item.buyPrice(5)) * 5;

			// Draw the coin costs.
			string costText = "Cost: ";
			Utils.DrawBorderStringFourWay(spriteBatch, Main.fontMouseText, costText, costDrawPositionTopLeft.X, costDrawPositionTopLeft.Y + 45f * Main.UIScale, Color.White * (Main.mouseTextColor / 255f), Color.Black, Vector2.Zero, Main.UIScale);
			costDrawPositionTopLeft.X += (int)((Main.fontMouseText.MeasureString(costText).X * 0.5f + 12f) * Main.UIScale);

			int[] coinsArray = Utils.CoinsSplit(cost);
			for (int i = 0; i < 4; i++)
			{
				Vector2 drawPosition = new Vector2(costDrawPositionTopLeft.X + (ChatManager.GetStringSize(Main.fontMouseText, costText, Vector2.One, -1f).X + ((24 * i) - 24f)) * Main.UIScale, costDrawPositionTopLeft.Y + 54f * Main.UIScale);
				spriteBatch.Draw(Main.itemTexture[ItemID.PlatinumCoin - i], drawPosition, null, Color.White, 0f, Main.itemTexture[ItemID.PlatinumCoin - i].Size() * 0.5f, Main.UIScale, SpriteEffects.None, 0f);
				Utils.DrawBorderStringFourWay(spriteBatch, Main.fontItemStack, coinsArray[3 - i].ToString(), drawPosition.X - 11f, drawPosition.Y, Color.White, Color.Black, new Vector2(0.3f), 0.75f * Main.UIScale);
			}

			return cost;
		}

		public static void DrawEnchantmentDescription(SpriteBatch spriteBatch, Point descriptionDrawPositionTopLeft)
		{
			Vector2 vectorDrawPosition = descriptionDrawPositionTopLeft.ToVector2();
			Vector2 scale = new Vector2(0.8f, 0.825f) * MathHelper.Clamp(ResolutionRatio, 0.825f, 1f) * Main.UIScale;
			foreach (string line in Utils.WordwrapString(SelectedEnchantment.Value.Description, Main.fontMouseText, 400, 16, out _))
			{
				if (string.IsNullOrEmpty(line))
					continue;

				ChatManager.DrawColorCodedStringWithShadow(spriteBatch, Main.fontMouseText, line, vectorDrawPosition, Color.Orange, 0f, Vector2.Zero, scale);
				vectorDrawPosition.Y += Main.UIScale * 16f;
			}
		}

		public static void DrawIcon(SpriteBatch spriteBatch, Vector2 drawPositionTopLeft, Texture2D texture)
        {
			spriteBatch.Draw(texture, drawPositionTopLeft, null, Color.White, 0f, Vector2.Zero, Main.UIScale, SpriteEffects.None, 0f);
		}

		public static void DrawItemIcon(SpriteBatch spriteBatch, Vector2 itemSlotDrawPosition, Vector2 reforgeIconDrawPosition, Vector2 scale, out bool isHoveringOverItemIcon, out bool isHoveringOverReforgeIcon)
		{
			isHoveringOverReforgeIcon = false;
			Texture2D itemSlotTexture = ModContent.GetTexture("CalamityMod/ExtraTextures/UI/CalamitasCurseItemSlot");

			Texture2D reforgeIconTexture = ModContent.GetTexture("CalamityMod/ExtraTextures/UI/CalamitasCurseUI_Button");
			Rectangle reforgeIconArea = new Rectangle((int)reforgeIconDrawPosition.X, (int)reforgeIconDrawPosition.Y, (int)(reforgeIconTexture.Width * scale.X), (int)(reforgeIconTexture.Height * scale.Y));

			// Have the reforge icon light up if the mouse is hovering over it.
			if (MouseScreenArea.Intersects(reforgeIconArea))
			{
				reforgeIconTexture = ModContent.GetTexture("CalamityMod/ExtraTextures/UI/CalamitasCurseUI_ButtonHovered");
				isHoveringOverReforgeIcon = true;
			}

			if (ReforgeButtonClickCountdown > 0f)
				reforgeIconTexture = ModContent.GetTexture("CalamityMod/ExtraTextures/UI/CalamitasCurseUI_ButtonClicked");

			// This will be used for item deposit/withdrawal logic.
			isHoveringOverItemIcon = MouseScreenArea.Intersects(new Rectangle((int)itemSlotDrawPosition.X, (int)itemSlotDrawPosition.Y, (int)(itemSlotTexture.Width * scale.X), (int)(itemSlotTexture.Height * scale.Y)));

			spriteBatch.Draw(itemSlotTexture, itemSlotDrawPosition, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);

			// Draw the draw the item within the slot, if it exists.
			if (!CurrentlyHeldItem.IsAir)
				AttemptToDrawItemInIcon(spriteBatch, itemSlotDrawPosition);

			spriteBatch.Draw(reforgeIconTexture, reforgeIconDrawPosition, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
		}

		public static void AttemptToDrawItemInIcon(SpriteBatch spriteBatch, Vector2 drawPosition)
		{
			float inventoryScale = Main.inventoryScale;
			Texture2D itemTexture = Main.itemTexture[CurrentlyHeldItem.type];
			Rectangle itemFrame = itemTexture.Frame(1, 1, 0, 0);
			bool hasMultipleFrames = Main.itemAnimations[CurrentlyHeldItem.type] != null;
			if (hasMultipleFrames)
				itemFrame = Main.itemAnimations[CurrentlyHeldItem.type].GetFrame(itemTexture);

			float baseScale = Main.UIScale;
			Color _ = Color.White;
			ItemSlot.GetItemLight(ref _, ref baseScale, CurrentlyHeldItem, false);

			float itemScale = 1f;

			// Ensure that the item being drawn does not exceed a certain size.
			// If it does, constrict its scale to prevent it from going beyond the maximum.
			if (itemFrame.Width > 36 || itemFrame.Height > 36)
				itemScale = 36f / MathHelper.Max(itemFrame.Width, itemFrame.Height);

			itemScale *= inventoryScale * baseScale;
			drawPosition += Vector2.One * 24f * baseScale;

			// Draw the item.
			spriteBatch.Draw(itemTexture, drawPosition, itemFrame, CurrentlyHeldItem.GetAlpha(Color.White), 0f, itemFrame.Size() * 0.5f, itemScale, SpriteEffects.None, 0f);
			spriteBatch.Draw(itemTexture, drawPosition, itemFrame, CurrentlyHeldItem.GetColor(Color.White), 0f, itemFrame.Size() * 0.5f, itemScale, SpriteEffects.None, 0f);
		}

		public static void InteractWithItemSlot()
		{
			if (!CurrentlyHeldItem.IsAir)
			{
				// Display item stats.
				Main.HoverItem = CurrentlyHeldItem.Clone();

				// Force the HoverItem to be displayed.
				Main.instance.MouseTextHackZoom(string.Empty);
			}

			// Attempt to exchange if the slot is clicked.
			if (Main.mouseLeftRelease && Main.mouseLeft && (Main.mouseItem.CanBeEnchantedBySomething() || Main.mouseItem.IsAir))
			{
				EnchantIndex = 0;
				Utils.Swap(ref Main.mouseItem, ref CurrentlyHeldItem);
				Main.PlaySound(SoundID.Grab);
			}
		}

		public static void DrawAndInteractWithButtons(SpriteBatch spriteBatch, IEnumerable<Enchantment> possibleEnchantments, Vector2 topButtonTopLeft, Vector2 bottomButtonTopLeft, Vector2 scale)
		{
			// Leave the arrows blank if no possible enchants exist.
			if (possibleEnchantments.Count() == 0)
				return;

			// Decide textures.
			Texture2D topArrowTexture = ModContent.GetTexture("CalamityMod/ExtraTextures/UI/CalamitasCurseUI_ArrowUp");
			Texture2D bottomArrowTexture = ModContent.GetTexture("CalamityMod/ExtraTextures/UI/CalamitasCurseUI_ArrowDown");
			if (TopButtonClickCountdown > 0f)
				topArrowTexture = ModContent.GetTexture("CalamityMod/ExtraTextures/UI/CalamitasCurseUI_ArrowUpClicked");
			if (BottomButtonClickCountdown > 0f)
				bottomArrowTexture = ModContent.GetTexture("CalamityMod/ExtraTextures/UI/CalamitasCurseUI_ArrowDownClicked");

			Rectangle topButtonArea = new Rectangle((int)topButtonTopLeft.X, (int)topButtonTopLeft.Y, (int)(topArrowTexture.Width * scale.X), (int)(topArrowTexture.Height * scale.Y));
			Rectangle bottomButtonArea = new Rectangle((int)bottomButtonTopLeft.X, (int)bottomButtonTopLeft.Y, (int)(bottomArrowTexture.Width * scale.X), (int)(bottomArrowTexture.Height * scale.Y));
			bool hoveringOverTopArrow = MouseScreenArea.Intersects(topButtonArea);
			bool hoveringOverBottomArrow = MouseScreenArea.Intersects(bottomButtonArea);
			if (hoveringOverTopArrow)
				topArrowTexture = ModContent.GetTexture("CalamityMod/ExtraTextures/UI/CalamitasCurseUI_ArrowUpHovered");

			if (hoveringOverBottomArrow)
				bottomArrowTexture = ModContent.GetTexture("CalamityMod/ExtraTextures/UI/CalamitasCurseUI_ArrowDownHovered");

			// Draw the arrows.
			if (EnchantIndex > 0)
				spriteBatch.Draw(topArrowTexture, topButtonTopLeft, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
			if (EnchantIndex < possibleEnchantments.Count() - 1)
				spriteBatch.Draw(bottomArrowTexture, bottomButtonTopLeft, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);

			if (Main.mouseLeft && Main.mouseLeftRelease)
			{
				// Decement the enchantment index if the top button is pressed.
				if (hoveringOverTopArrow && EnchantIndex > 0)
				{
					EnchantIndex--;
					TopButtonClickCountdown = 15f;
					Main.PlaySound(SoundID.MenuTick);
				}

				// Increment the enchantment index if the bottom button is pressed.
				if (hoveringOverBottomArrow && EnchantIndex < possibleEnchantments.Count() - 1)
				{
					EnchantIndex++;
					BottomButtonClickCountdown = 15f;
					Main.PlaySound(SoundID.MenuTick);
				}
			}
		}

		public static void DrawEnchantmentName(SpriteBatch spriteBatch, Vector2 nameDrawCenter)
		{
			Vector2 scale = new Vector2(0.8f, 0.745f) * Main.UIScale;
			float textWidth = Main.fontMouseText.MeasureString(SelectedEnchantment.Value.Name).X * scale.X;
			Color drawColor = SelectedEnchantment.Value.Equals(EnchantmentManager.ClearEnchantment) ? Color.White : Color.Orange;
			nameDrawCenter.X -= textWidth * 0.5f;
			ChatManager.DrawColorCodedStringWithShadow(spriteBatch, Main.fontMouseText, SelectedEnchantment.Value.Name, nameDrawCenter, drawColor, 0f, Vector2.Zero, scale);
		}

		public static void InteractWithEnchantIcon(int cost)
		{
			// If there's no valid item in the slot, do nothing.
			if (CurrentlyHeldItem.IsAir)
				return;

			// If no enchantment has been selected, do nothing.
			if (!SelectedEnchantment.HasValue)
				return;

			// If there is no cost or the player cannot afford it, do nothing.
			if (cost <= 0 || !Main.LocalPlayer.CanBuyItem(cost))
				return;

			Item originalItem = CurrentlyHeldItem.Clone();
			byte oldPrefix = CurrentlyHeldItem.prefix;
			CurrentlyHeldItem.SetDefaults(CurrentlyHeldItem.type);
			CurrentlyHeldItem.Prefix(oldPrefix);
			CurrentlyHeldItem = CurrentlyHeldItem.CloneWithModdedDataFrom(originalItem);

			if (SelectedEnchantment.Value.Name == EnchantmentManager.UpgradeEnchantName)
			{
				CurrentlyHeldItem.SetDefaults(EnchantmentManager.ItemUpgradeRelationship[CurrentlyHeldItem.type]);
				CurrentlyHeldItem.Prefix(oldPrefix);
			}
			else
			{
				CurrentlyHeldItem.Calamity().AppliedEnchantment = SelectedEnchantment.Value;
				SelectedEnchantment.Value.CreationEffect?.Invoke(CurrentlyHeldItem);
			}

			// Update the compare item. This is used check comparisons when showing reforge tooltip bonuses.
			// Updating it with the same bonuses as what was applied to the real item will negate the incorrect numbers,
			// such as absurd damage boosts.
			if (Main.cpItem is null)
				Main.cpItem = new Item();
			Main.cpItem.SetDefaults(Main.cpItem.type);

			if (SelectedEnchantment.Value.Name != EnchantmentManager.UpgradeEnchantName)
			{
				Main.cpItem.Calamity().AppliedEnchantment = SelectedEnchantment.Value;
				SelectedEnchantment.Value.CreationEffect?.Invoke(Main.cpItem);
			}

			// Take away the money for the cost.
			Main.LocalPlayer.BuyItem(cost);

			// Reset the enchantment index to prevent index problems on a different item.
			EnchantIndex = 0;

			Main.PlaySound(SoundID.DD2_BetsyFlameBreath, Main.LocalPlayer.Center);

			if (SelectedEnchantment.Value.Name == EnchantmentManager.UpgradeEnchantName)
				Main.PlaySound(SoundID.DD2_DarkMageHealImpact, Main.LocalPlayer.Center);
		}
	}
}
