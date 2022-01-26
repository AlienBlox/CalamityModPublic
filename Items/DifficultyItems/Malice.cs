using CalamityMod.CalPlayer;
using CalamityMod.Events;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.DifficultyItems
{
    public class Malice : ModItem
    {
        public int frameCounter = 0;
        public int frame = 0;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Malice");
            Tooltip.SetDefault("Enables/disables Malice Mode, can only be used in Revengeance Mode.\n" +
				// Overall description and warning lines
				"[c/c01818:This mode is subjective, play how you want, don't expect to live.]\n" +

                // Misc lines
                "Greatly nerfs the effectiveness of life steal.\n" +
                "Nerfs the effectiveness of the Titanium Armor set bonus, doesn't stack with Revengeance Mode.\n" +
                "The Nurse no longer heals you while a boss is alive.\n" +
                "Defense damage is 5% higher than Death Mode.\n" +
                "Increases damage done by 50% for several debuffs and all alcohols that reduce life regen.\n" +

                // Boss lines
                "All boss minions no longer drop hearts.\n" +
                "Enrages all bosses and gives them new AI.\n" +
                "Bosses and their projectiles deal 35% more damage.\n" +
                "Increases the velocity of most boss projectiles by 25%, this is increased to 35% during Boss Rush.\n" +
                "Boss reactive DR is always active outside of Boss Rush and is increased by 50%.");
        }

        public override void SetDefaults()
        {
            item.rare = ItemRarityID.Purple;
            item.width = 82;
            item.height = 66;
            item.useAnimation = 45;
            item.useTime = 45;
            item.useStyle = ItemUseStyleID.HoldingUp;
            item.UseSound = SoundID.Item119;
            item.consumable = false;
        }

        public override void UseStyle(Player player)
        {
            player.itemLocation += new Vector2(-32f * player.direction, player.gravDir).RotatedBy(player.itemRotation);
        }

        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frameI, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            Texture2D texture = ModContent.GetTexture("CalamityMod/Items/DifficultyItems/Malice_Animated");
            spriteBatch.Draw(texture, position, item.GetCurrentFrame(ref frame, ref frameCounter, 8, 8), Color.White, 0f, origin, scale, SpriteEffects.None, 0);
            return false;
        }

        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            Texture2D texture = ModContent.GetTexture("CalamityMod/Items/DifficultyItems/Malice_Animated");
            spriteBatch.Draw(texture, item.position - Main.screenPosition, item.GetCurrentFrame(ref frame, ref frameCounter, 8, 8), lightColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0);
            return false;
        }

		public override bool UseItem(Player player)
		{
			// This world syncing code should only be run by one entity- the server, to prevent a race condition
			// with the packets.
			if (Main.netMode == NetmodeID.MultiplayerClient)
				return true;

			if (CalamityPlayer.areThereAnyDamnBosses || CalamityWorld.DoGSecondStageCountdown > 0 || BossRushEvent.BossRushActive)
			{
				string key = "Mods.CalamityMod.ChangingTheRules";
				Color messageColor = Color.Crimson;
				CalamityUtils.DisplayLocalizedText(key, messageColor);
				return true;
			}
			if (!CalamityWorld.malice)
			{
				CalamityWorld.malice = true;
				string key = "Mods.CalamityMod.MaliceText";
				Color messageColor = Color.Crimson;
				CalamityUtils.DisplayLocalizedText(key, messageColor);
			}
			else
			{
				CalamityWorld.malice = false;
				string key = "Mods.CalamityMod.MaliceText2";
				Color messageColor = Color.Crimson;
				CalamityUtils.DisplayLocalizedText(key, messageColor);
			}
			CalamityWorld.DoGSecondStageCountdown = 0;
			CalamityNetcode.SyncWorld();

			return true;
		}

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddTile(TileID.DemonAltar);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
