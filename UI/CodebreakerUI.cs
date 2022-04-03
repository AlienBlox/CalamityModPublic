using CalamityMod.Items.DraedonMisc;
using CalamityMod.TileEntities;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Linq;
using System.Text;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI.Chat;
using Terraria.Audio;

namespace CalamityMod.UI
{
    public static class CodebreakerUI
    {
        public static int ViewedTileEntityID = -1;
        public static bool AwaitingCloseConfirmation = false;
        public static bool AwaitingDecryptionTextClose = false;
        public static float VerificationButtonScale = 1f;
        public static float CancelButtonScale = 0.75f;
        public static float ContactButtonScale = 1f;
        public static float MechIconScale = 1f;
        public static Vector2 BackgroundCenter => new Vector2(500f, Main.screenHeight * 0.5f + 115f);
        public static float GeneralScale => MathHelper.Lerp(1f, 0.7f, Utils.InverseLerp(1325f, 750f, Main.screenWidth, true)) * Main.UIScale;

        public static Rectangle MouseScreenArea => Utils.CenteredRectangle(Main.MouseScreen, Vector2.One * 2f);
        public static void Draw(SpriteBatch spriteBatch)
        {
            // If not viewing the specific tile entity's interface anymore, if the ID is for some reason invalid, or if the player is not equipped to continue viewing the UI
            // don't do anything other than resetting necessary data.
            if (!TileEntity.ByID.ContainsKey(ViewedTileEntityID) || !(TileEntity.ByID[ViewedTileEntityID] is TECodebreaker codebreakerTileEntity) || !Main.LocalPlayer.WithinRange(codebreakerTileEntity.Center, 270f) || !Main.playerInventory)
            {
                VerificationButtonScale = 1f;
                CancelButtonScale = 0.75f;
                ContactButtonScale = 1f;
                ViewedTileEntityID = -1;
                AwaitingCloseConfirmation = false;
                MechIconScale = 1f;
                return;
            }

            // Draw the background.
            Texture2D backgroundTexture = ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/UI/DraedonDecrypterBackground");
            spriteBatch.Draw(backgroundTexture, BackgroundCenter, null, Color.White, 0f, backgroundTexture.Size() * 0.5f, GeneralScale, SpriteEffects.None, 0f);

            Rectangle backgroundArea = Utils.CenteredRectangle(BackgroundCenter, backgroundTexture.Size());
            if (MouseScreenArea.Intersects(backgroundArea))
                Main.blockMouse = Main.LocalPlayer.mouseInterface = true;

            bool canSummonDraedon = codebreakerTileEntity.ReadyToSummonDraedon && CalamityWorld.AbleToSummonDraedon;
            Vector2 backgroundTopLeft = BackgroundCenter - backgroundTexture.Size() * GeneralScale * 0.5f;

            // Draw the cell payment slot icon.
            Texture2D emptyCellIconTexture = ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/UI/PowerCellSlot_Empty");
            Texture2D occupiedCellIconTexture = ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/UI/PowerCellSlot_Filled");
            Texture2D textPanelTexture = ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/UI/DraedonDecrypterScreen");
            Texture2D cellTexture = codebreakerTileEntity.InputtedCellCount > 0 ? occupiedCellIconTexture : emptyCellIconTexture;
            Vector2 cellDrawCenter = backgroundTopLeft + Vector2.One * GeneralScale * 60f;

            Vector2 schematicSlotDrawCenter = cellDrawCenter + Vector2.UnitY * GeneralScale * 70f;
            Vector2 costDisplayLocation = schematicSlotDrawCenter + Vector2.UnitY * GeneralScale * 20f;
            Vector2 costVerificationLocation = costDisplayLocation + Vector2.UnitY * GeneralScale * 60f;
            Vector2 textPanelCenter = backgroundTopLeft + Vector2.UnitX * backgroundTexture.Width + textPanelTexture.Size() * GeneralScale * new Vector2(-0.5f, 0.5f);
            Vector2 summonButtonCenter = backgroundTopLeft + new Vector2(58f, backgroundTexture.Height - 48f) * GeneralScale;

            // Display some error text if the codebreaker isn't strong enough to decrypt the schematic.
            if (codebreakerTileEntity.HeldSchematicID != 0 && !codebreakerTileEntity.CanDecryptHeldSchematic)
                DisplayNotStrongEnoughErrorText(schematicSlotDrawCenter + new Vector2(-24f, 30f));

            // Handle decryption costs.
            else if (codebreakerTileEntity.HeldSchematicID != 0 && codebreakerTileEntity.DecryptionCountdown == 0)
            {
                int cost = codebreakerTileEntity.DecryptionCellCost;
                DisplayCostText(costDisplayLocation, cost);

                if (codebreakerTileEntity.InputtedCellCount >= cost)
                {
                    if (canSummonDraedon)
                    {
                        costVerificationLocation.X -= GeneralScale * 15f;
                        summonButtonCenter.X += GeneralScale * 15f;
                    }
                    DrawCostVerificationButton(codebreakerTileEntity, costVerificationLocation);
                }
            }
            else if (codebreakerTileEntity.DecryptionCountdown > 0)
            {
                if (!AwaitingCloseConfirmation)
                    DisplayDecryptCancelButton(codebreakerTileEntity, costVerificationLocation - Vector2.UnitY * GeneralScale * 30f);
                else
                    DisplayDecryptCancelButton(codebreakerTileEntity, textPanelCenter + Vector2.UnitY * GeneralScale * 110f);
            }

            if (canSummonDraedon)
                HandleDraedonSummonButton(codebreakerTileEntity, summonButtonCenter);

            if (codebreakerTileEntity.DecryptionCountdown > 0 || AwaitingDecryptionTextClose)
                HandleDecryptionStuff(codebreakerTileEntity, backgroundTexture, backgroundTopLeft, schematicSlotDrawCenter + Vector2.UnitY * GeneralScale * 80f);
            if (codebreakerTileEntity.DecryptionCountdown > 0 && AwaitingCloseConfirmation)
                DrawDecryptCancelConfirmationText(textPanelCenter);

            // Draw the schematic icon.
            Texture2D schematicIconBG = ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/UI/EncryptedSchematicSlotBackground");
            Texture2D schematicIconTexture = schematicIconBG;
            int schematicType = 0;

            if (codebreakerTileEntity.HeldSchematicID > 0)
                schematicType = CalamityLists.EncryptedSchematicIDRelationship[codebreakerTileEntity.HeldSchematicID];

            if (schematicType == ModContent.ItemType<EncryptedSchematicPlanetoid>())
                schematicIconTexture = ModContent.Request<Texture2D>("CalamityMod/Items/DraedonMisc/EncryptedSchematicPlanetoid");
            if (schematicType == ModContent.ItemType<EncryptedSchematicJungle>())
                schematicIconTexture = ModContent.Request<Texture2D>("CalamityMod/Items/DraedonMisc/EncryptedSchematicJungle");
            if (schematicType == ModContent.ItemType<EncryptedSchematicHell>())
                schematicIconTexture = ModContent.Request<Texture2D>("CalamityMod/Items/DraedonMisc/EncryptedSchematicHell");
            if (schematicType == ModContent.ItemType<EncryptedSchematicIce>())
                schematicIconTexture = ModContent.Request<Texture2D>("CalamityMod/Items/DraedonMisc/EncryptedSchematicIce");

            spriteBatch.Draw(schematicIconBG, schematicSlotDrawCenter, null, Color.White, 0f, schematicIconBG.Size() * 0.5f, GeneralScale, SpriteEffects.None, 0f);
            if (codebreakerTileEntity.HeldSchematicID != 0)
                spriteBatch.Draw(schematicIconTexture, schematicSlotDrawCenter, null, Color.White, 0f, schematicIconTexture.Size() * 0.5f, GeneralScale, SpriteEffects.None, 0f);
            HandleSchematicSlotInteractions(codebreakerTileEntity, schematicSlotDrawCenter, cellTexture.Size() * GeneralScale);

            // Create a temporary item for drawing purposes.
            // If cells are present, make the item reflect that.
            Item temporaryPowerCell = new Item();
            temporaryPowerCell.SetDefaults(ModContent.ItemType<PowerCell>());
            temporaryPowerCell.stack = codebreakerTileEntity.InputtedCellCount;

            CalamityUtils.DrawPowercellSlot(spriteBatch, temporaryPowerCell, cellDrawCenter, GeneralScale);
            HandleCellSlotInteractions(codebreakerTileEntity, temporaryPowerCell, cellDrawCenter, cellTexture.Size());
        }

        public static void HandleCellSlotInteractions(TECodebreaker codebreakerTileEntity, Item temporaryItem, Vector2 cellIconCenter, Vector2 area)
        {
            Rectangle clickArea = Utils.CenteredRectangle(cellIconCenter, area);

            // If the mouse is not in the specific area don't do anything else.
            if (!MouseScreenArea.Intersects(clickArea))
                return;

            ref Item playerHandItem = ref Main.mouseItem;

            if (!temporaryItem.IsAir)
                Main.HoverItem = temporaryItem;

            if (Main.mouseLeft && Main.mouseLeftRelease && codebreakerTileEntity.DecryptionCountdown <= 0)
            {
                int powerCellID = ModContent.ItemType<PowerCell>();
                short cellStackDiff = 0;
                bool shouldPlaySound = true;

                // If the player is holding shift and has space for the power cells, just spawn all of them on his or her face, up to the max-stack limit.
                if (Main.keyState.PressingShift() && Main.LocalPlayer.ItemSpace(temporaryItem))
                {
                    cellStackDiff = (short)-Math.Min(temporaryItem.stack, temporaryItem.maxStack);
                    DropHelper.DropItem(Main.LocalPlayer, powerCellID, -cellStackDiff);

                    // Do not play a sound in this situation. The player is going to pick up the dropped cells in a few frames, which will make sound.
                    shouldPlaySound = false;
                }

                // If the slot is normally clicked, behavior depends on whether the player is holding power cells.
                else
                {
                    bool holdingPowercell = playerHandItem.type == powerCellID;

                    // If the player's held power cells can be stacked on top of what's already in the codeberaker, then stack them.
                    if (holdingPowercell && temporaryItem.stack < TECodebreaker.MaxCellCapacity)
                    {
                        int spaceLeft = TECodebreaker.MaxCellCapacity - temporaryItem.stack;

                        // If the player has more cells than there is space left, insert as many as possible. Otherwise insert all the cells.
                        int cellsToInsert = Math.Min(playerHandItem.stack, spaceLeft);
                        cellStackDiff = (short)cellsToInsert;
                        playerHandItem.stack -= cellsToInsert;
                        if (playerHandItem.stack == 0)
                            playerHandItem.TurnToAir();
                        AwaitingDecryptionTextClose = false;
                    }

                    // If the player is holding nothing, then pick up all the power cells (if any exist), up to the max-stack limit.
                    else if (playerHandItem.IsAir && temporaryItem.stack > 0)
                    {
                        cellStackDiff = (short)-temporaryItem.stack;
                        if (cellStackDiff < -temporaryItem.maxStack)
                            cellStackDiff = (short)-temporaryItem.maxStack;

                        playerHandItem.SetDefaults(temporaryItem.type);
                        playerHandItem.stack = -cellStackDiff;
                        temporaryItem.TurnToAir();
                        AwaitingDecryptionTextClose = false;
                    }
                }

                if (cellStackDiff != 0)
                {
                    if (shouldPlaySound)
                        SoundEngine.PlaySound(SoundID.Grab);
                    AwaitingDecryptionTextClose = false;
                    codebreakerTileEntity.InputtedCellCount += cellStackDiff;
                    codebreakerTileEntity.SyncContainedStuff();
                }
            }

            // Force the hover item to be drawn.
            // Since HoverItem is active, we don't need to input anything into this method.
            if (temporaryItem.stack > 0)
                Main.instance.MouseTextHackZoom(string.Empty);
        }

        public static void HandleSchematicSlotInteractions(TECodebreaker codebreakerTileEntity, Vector2 schematicIconCenter, Vector2 area)
        {
            Rectangle clickArea = Utils.CenteredRectangle(schematicIconCenter, area);

            // If the mouse is not in the specific area don't do anything else.
            if (!MouseScreenArea.Intersects(clickArea))
                return;

            ref Item playerHandItem = ref Main.mouseItem;

            // Handle mouse click interactions.
            if (Main.mouseLeft && Main.mouseLeftRelease && codebreakerTileEntity.DecryptionCountdown <= 0)
            {
                // If the player's hand item is empty and the codebreaker has a schematic, grab it.
                // This doesn't work if the Codebreaker is busy decrypting the schematic in question.
                if (playerHandItem.IsAir && CalamityLists.EncryptedSchematicIDRelationship.ContainsKey(codebreakerTileEntity.HeldSchematicID) && codebreakerTileEntity.DecryptionCountdown <= 0)
                {
                    playerHandItem.SetDefaults(CalamityLists.EncryptedSchematicIDRelationship[codebreakerTileEntity.HeldSchematicID]);
                    codebreakerTileEntity.HeldSchematicID = 0;
                    codebreakerTileEntity.DecryptionCountdown = 0;
                    codebreakerTileEntity.SyncContainedStuff();
                    SoundEngine.PlaySound(SoundID.Grab);

                    AwaitingDecryptionTextClose = false;
                }

                // Otherwise, if the player has an encrypted schematic and the Codebreaker doesn't, insert it into the machine.
                else if (CalamityLists.EncryptedSchematicIDRelationship.ContainsValue(playerHandItem.type) && codebreakerTileEntity.HeldSchematicID == 0)
                {
                    codebreakerTileEntity.HeldSchematicID = CalamityLists.EncryptedSchematicIDRelationship.First(i => i.Value == Main.mouseItem.type).Key;
                    playerHandItem.TurnToAir();
                    codebreakerTileEntity.SyncContainedStuff();
                    SoundEngine.PlaySound(SoundID.Grab);

                    AwaitingDecryptionTextClose = false;
                }

                // Lastly, if the player has an encrypted schematic but so does the Codebreaker, swap the two.
                else if (CalamityLists.EncryptedSchematicIDRelationship.ContainsValue(playerHandItem.type) && codebreakerTileEntity.HeldSchematicID != 0)
                {
                    int previouslyHeldSchematic = CalamityLists.EncryptedSchematicIDRelationship[codebreakerTileEntity.HeldSchematicID];

                    // If the schematics are the same, don't actually do anything, just play the sound as an illusion, to prevent having to send a packet.
                    SoundEngine.PlaySound(SoundID.Grab);
                    if (playerHandItem.type != previouslyHeldSchematic)
                    {
                        codebreakerTileEntity.HeldSchematicID = CalamityLists.EncryptedSchematicIDRelationship.First(i => i.Value == Main.mouseItem.type).Key;
                        playerHandItem.SetDefaults(previouslyHeldSchematic);
                        codebreakerTileEntity.SyncContainedStuff();
                        AwaitingDecryptionTextClose = false;
                    }
                }
            }
        }

        public static void DisplayCostText(Vector2 drawPosition, int totalCellsCost)
        {
            // Display the cost text.
            string text = "Cost: ";
            drawPosition.X -= GeneralScale * 30f;
            Utils.DrawBorderStringFourWay(Main.spriteBatch, Main.fontMouseText, text, drawPosition.X, drawPosition.Y + GeneralScale * 20f, Color.White * (Main.mouseTextColor / 255f), Color.Black, Vector2.Zero, GeneralScale);

            // And draw the cells to the right of the text.
            Texture2D cellTexture = ModContent.Request<Texture2D>("CalamityMod/Items/DraedonMisc/PowerCell");
            Vector2 offsetDrawPosition = new Vector2(drawPosition.X + ChatManager.GetStringSize(Main.fontMouseText, text, Vector2.One, -1f).X * GeneralScale + GeneralScale * 15f, drawPosition.Y + GeneralScale * 30f);
            Main.spriteBatch.Draw(cellTexture, offsetDrawPosition, null, Color.White, 0f, cellTexture.Size() * 0.5f, GeneralScale, SpriteEffects.None, 0f);

            // Display the cell quantity numerically below the drawn cells.
            Utils.DrawBorderStringFourWay(Main.spriteBatch, Main.fontItemStack, totalCellsCost.ToString(), offsetDrawPosition.X - GeneralScale * 11f, offsetDrawPosition.Y, Color.White, Color.Black, new Vector2(0.3f), GeneralScale * 0.75f);
        }

        public static void DrawCostVerificationButton(TECodebreaker codebreakerTileEntity, Vector2 drawPosition)
        {
            Texture2D confirmationTexture = ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/UI/DecryptIcon");
            Rectangle clickArea = Utils.CenteredRectangle(drawPosition, confirmationTexture.Size() * VerificationButtonScale);

            // Check if the mouse is hovering over the cost button area.
            if (MouseScreenArea.Intersects(clickArea))
            {
                // If so, cause the button to inflate a little bit.
                VerificationButtonScale = MathHelper.Clamp(VerificationButtonScale + 0.035f, 1f, 1.35f);

                // If a click is done, begin the decryption process.
                // This will "lock" various things and make the Codebreaker unbreakable, to prevent complications with lost items.
                // Also play a cool sound.
                if (Main.mouseLeft && Main.mouseLeftRelease)
                {
                    SoundEngine.PlaySound(SoundID.Zombie, Main.LocalPlayer.Center, 67);
                    AwaitingDecryptionTextClose = true;
                    codebreakerTileEntity.InitialCellCountBeforeDecrypting = codebreakerTileEntity.InputtedCellCount;
                    codebreakerTileEntity.DecryptionCountdown = codebreakerTileEntity.DecryptionTotalTime;
                    codebreakerTileEntity.SyncContainedStuff();
                    codebreakerTileEntity.SyncDecryptCountdown();
                }
            }

            // Otherwise, if not hovering, cause the button to deflate back to its normal size.
            else
                VerificationButtonScale = MathHelper.Clamp(VerificationButtonScale - 0.05f, 1f, 1.35f);

            // Draw the confirmation icon.
            Main.spriteBatch.Draw(confirmationTexture, drawPosition, null, Color.White, 0f, confirmationTexture.Size() * 0.5f, VerificationButtonScale * GeneralScale, SpriteEffects.None, 0f);
        }

        public static void DisplayDecryptCancelButton(TECodebreaker codebreakerTileEntity, Vector2 drawPosition)
        {
            bool clickingMouse = Main.mouseLeft && Main.mouseLeftRelease;
            Texture2D cancelTexture = ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/UI/DecryptCancelIcon");
            Rectangle clickArea = Utils.CenteredRectangle(drawPosition, cancelTexture.Size() * CancelButtonScale * 1.2f);

            // Check if the mouse is hovering over the decrypt button area.
            if (MouseScreenArea.Intersects(clickArea))
            {
                // If so, cause the button to inflate a little bit.
                CancelButtonScale = MathHelper.Clamp(CancelButtonScale + 0.035f, 0.9f, 1.2f);

                // If a click is done, cancel the decryption process.
                // This will cause already consumed cells to be lost.
                if (clickingMouse)
                {
                    if (AwaitingCloseConfirmation)
                    {
                        SoundEngine.PlaySound(SoundID.Item94, Main.LocalPlayer.Center);

                        AwaitingDecryptionTextClose = false;
                        codebreakerTileEntity.InitialCellCountBeforeDecrypting = 0;
                        codebreakerTileEntity.DecryptionCountdown = 0;
                        codebreakerTileEntity.SyncContainedStuff();
                        codebreakerTileEntity.SyncDecryptCountdown();
                        AwaitingCloseConfirmation = false;
                    }
                    else
                        AwaitingCloseConfirmation = true;
                }
            }

            // Otherwise, if not hovering, cause the button to deflate back to its normal size.
            else
            {
                CancelButtonScale = MathHelper.Clamp(CancelButtonScale - 0.05f, 0.9f, 1.2f);
                if (clickingMouse)
                    AwaitingCloseConfirmation = false;
            }

            // Draw the cancel icon.
            Main.spriteBatch.Draw(cancelTexture, drawPosition, null, Color.White, 0f, cancelTexture.Size() * 0.5f, CancelButtonScale * GeneralScale, SpriteEffects.None, 0f);
        }

        public static void DrawDecryptCancelConfirmationText(Vector2 drawPosition)
        {
            Texture2D textPanelTexture = ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/UI/DraedonDecrypterScreen");
            Vector2 scale = new Vector2(1f, 0.3f) * GeneralScale;
            Main.spriteBatch.Draw(textPanelTexture, drawPosition, null, Color.White, 0f, textPanelTexture.Size() * 0.5f, scale, SpriteEffects.None, 0);

            string confirmationText = "Are you sure?";
            Vector2 confirmationTextPosition = drawPosition - Main.fontMouseText.MeasureString(confirmationText) * GeneralScale * 0.5f + Vector2.UnitY * GeneralScale * 4f;
            ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, Main.fontMouseText, confirmationText, confirmationTextPosition, Color.Red, 0f, Vector2.Zero, Vector2.One * GeneralScale);
        }

        public static void HandleDecryptionStuff(TECodebreaker codebreakerTileEntity, Texture2D backgroundTexture, Vector2 backgroundTopLeft, Vector2 barCenter)
        {
            Texture2D textPanelTexture = ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/UI/DraedonDecrypterScreen");
            Vector2 textPanelCenter = backgroundTopLeft + Vector2.UnitX * backgroundTexture.Width * GeneralScale + textPanelTexture.Size() * new Vector2(-0.5f, 0.5f) * GeneralScale;
            Main.spriteBatch.Draw(textPanelTexture, textPanelCenter, null, Color.White, 0f, textPanelTexture.Size() * 0.5f, GeneralScale, SpriteEffects.None, 0f);

            // Generate gibberish and use slowly insert the real text.
            // When decryption is done the gibberish will go away and only the underlying text will remain.
            int textPadding = 6;
            string trueMessage = codebreakerTileEntity.UnderlyingSchematicText;
            StringBuilder text = new StringBuilder(codebreakerTileEntity.DecryptionCountdown == 0 ? trueMessage : CalamityUtils.GenerateRandomAlphanumericString(500));

            // Messing with whitespace characters so can cause the word-wrap to "jump" around.
            // As a result, changes to whitespace characters in the true text do not stay.
            for (int i = 0; i < trueMessage.Length; i++)
            {
                if (char.IsWhiteSpace(trueMessage[i]))
                    text[i] = trueMessage[i];
            }

            // Insert the necessary amount of true text.
            for (int i = 0; i < (int)(trueMessage.Length * codebreakerTileEntity.DecryptionCompletion); i++)
                text[i] = trueMessage[i];

            // Define the initial text draw position.
            Vector2 currentTextDrawPosition = backgroundTopLeft + new Vector2(backgroundTexture.Width - textPanelTexture.Width + textPadding, 6f) * GeneralScale;

            // Draw the lines of text. A maximum of 10 may be drawn and the vertical offset per line is 16 pixels.
            foreach (string line in Utils.WordwrapString(text.ToString(), Main.fontMouseText, (int)(textPanelTexture.Width * 1.5 - textPadding * 2), 10, out _))
            {
                // If a line is null or empty for some reason, don't attempt to draw it or move to the next line position.
                if (string.IsNullOrEmpty(line))
                    continue;

                ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, Main.fontMouseText, line, currentTextDrawPosition, Color.Cyan, 0f, Vector2.Zero, new Vector2(0.6f) * GeneralScale);
                currentTextDrawPosition.Y += GeneralScale * 16f;
            }

            // Handle special drawing when decryption is ongoing.
            // If it isn't, return; the below logic is unnecessary.
            if (codebreakerTileEntity.DecryptionCountdown <= 0)
                return;

            // Draw a small bar at the bottom to indicate how much work is left.
            Texture2D borderTexture = ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/UI/CodebreakerDecyptionBar");
            Texture2D barTexture = ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/UI/CodebreakerDecyptionBarCharge");
            Main.spriteBatch.Draw(borderTexture, barCenter, null, Color.White, 0f, borderTexture.Size() * 0.5f, GeneralScale, SpriteEffects.None, 0);
            Rectangle barRectangle = new Rectangle(0, 0, (int)(barTexture.Width * codebreakerTileEntity.DecryptionCompletion), barTexture.Width);
            Main.spriteBatch.Draw(barTexture, barCenter, barRectangle, Color.White, 0f, barTexture.Size() * 0.5f, GeneralScale, SpriteEffects.None, 0);

            // Display a completion percentage below the bar as a more precise indicator.
            string completionText = $"{codebreakerTileEntity.DecryptionCompletion * 100f:n2}%";
            Vector2 textDrawPosition = barCenter + new Vector2(-Main.fontMouseText.MeasureString(completionText).X * 0.5f, 10f) * GeneralScale;
            Utils.DrawBorderStringFourWay(Main.spriteBatch, Main.fontMouseText, completionText, textDrawPosition.X, textDrawPosition.Y, Color.Cyan * 1.2f, Color.Black, Vector2.Zero, GeneralScale);
        }

        public static void HandleDraedonSummonButton(TECodebreaker codebreakerTileEntity, Vector2 drawPosition)
        {
            Texture2D contactButton = ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/UI/ContactIcon_THanos");

            Rectangle clickArea = Utils.CenteredRectangle(drawPosition, contactButton.Size() * VerificationButtonScale);

            // Check if the mouse is hovering over the contact button area.
            if (MouseScreenArea.Intersects(clickArea))
            {
                // If so, cause the button to inflate a little bit.
                ContactButtonScale = MathHelper.Clamp(ContactButtonScale + 0.035f, 1f, 1.35f);

                // If a click is done, do a check.
                // Prepare the summoning process by defining the countdown and current summon position. The mech will be summoned by the Draedon NPC.
                // Also play a cool sound.
                if (Main.mouseLeft && Main.mouseLeftRelease)
                {
                    CalamityWorld.DraedonSummonCountdown = CalamityWorld.DraedonSummonCountdownMax;
                    CalamityWorld.DraedonSummonPosition = codebreakerTileEntity.Center + new Vector2(-8f, -100f);
                    SoundEngine.PlaySound(CalamityMod.Instance.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/CodebreakerBeam"), CalamityWorld.DraedonSummonPosition);

                    if (Main.netMode != NetmodeID.SinglePlayer)
                    {
                        var netMessage = CalamityMod.Instance.GetPacket();
                        netMessage.Write((byte)CalamityModMessageType.CodebreakerSummonStuff);
                        netMessage.Write(CalamityWorld.DraedonSummonCountdown);
                        netMessage.WriteVector2(CalamityWorld.DraedonSummonPosition);
                        netMessage.Send();
                    }
                }
            }

            // Otherwise, if not hovering, cause the button to deflate back to its normal size.
            else
                ContactButtonScale = MathHelper.Clamp(ContactButtonScale - 0.05f, 1f, 1.35f);

            // Draw the contact button.
            Main.spriteBatch.Draw(contactButton, drawPosition, null, Color.White, 0f, contactButton.Size() * 0.5f, ContactButtonScale * GeneralScale, SpriteEffects.None, 0f);

            // And display a text indicator that describes the function of the button.
            // The color of the text cycles through the exo mech crystal palette.
            string contactText = "Contact";
            Color contactTextColor = CalamityUtils.MulticolorLerp((float)Math.Cos(Main.GlobalTime * 0.7f) * 0.5f + 0.5f, CalamityUtils.ExoPalette);

            // Center the draw position.
            drawPosition.X -= Main.fontMouseText.MeasureString(contactText).X * GeneralScale * 0.5f;
            drawPosition.Y += GeneralScale * 20f;
            Utils.DrawBorderStringFourWay(Main.spriteBatch, Main.fontMouseText, contactText, drawPosition.X, drawPosition.Y, contactTextColor, Color.Black, Vector2.Zero, GeneralScale);
        }

        public static void DisplayNotStrongEnoughErrorText(Vector2 drawPosition)
        {
            string text = "Encryption unsolveable: Upgrades required.";
            Utils.DrawBorderStringFourWay(Main.spriteBatch, Main.fontMouseText, text, drawPosition.X, drawPosition.Y, Color.IndianRed * (Main.mouseTextColor / 255f), Color.Black, Vector2.Zero, GeneralScale);
        }
    }
}
