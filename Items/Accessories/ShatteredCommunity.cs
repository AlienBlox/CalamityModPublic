using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace CalamityMod.Items.Accessories
{
	public class ShatteredCommunity : ModItem
	{
		// The percentage of a full Rage bar that is gained every second with the Shattered Community equipped.
		public const float RagePerSecond = 0.02f;
		private static readonly Color rarityColorOne = new Color(128, 62, 128);
		private static readonly Color rarityColorTwo = new Color(245, 105, 245);

		// Base level cost is 400,000 damage dealt while Rage is active.
		// Each successive level costs (400,000 * level) MORE damage, so the total required goes up quadratically.
		// Total required to reach level 60 is 732,000,000 damage dealt.
		// This is within one order of magnitude of the integer limit, so relevant values are stored as longs.
		private const long BaseLevelCost = 400000L;
		private static long LevelCost(int level) => BaseLevelCost * level;
		private static long CumulativeLevelCost(int level) => (BaseLevelCost / 2L) * level * (level + 1);
		private const int MaxLevel = 60;

		internal int level = 0;
		internal long totalRageDamage = 0L;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Shattered Community");
			Tooltip.SetDefault("Ruined by unknowable hatred, it still contains (most of) the power of The Community...\n" +
				"Legendary Accessory\n" +
				"You generate rage over time and rage does not fade away out of combat\n" +
				"Taking damage gives rage, this effect is not hindered by your defensive stats\n" +
				"While Rage Mode is active, taking damage gives only half as much rage\n" +
				"Deal damage with Rage Mode to further empower your wrath\n");
			Main.RegisterItemAnimation(item.type, new DrawAnimationVertical(7, 5));
		}

		public override void SetDefaults()
		{
			item.width = 20;
			item.height = 20;
			item.accessory = true;
			item.value = CalamityGlobalItem.RarityHotPinkBuyPrice;
			item.rare = ItemRarityID.Red;
			item.Calamity().customRarity = CalamityRarity.HotPink;
			item.Calamity().devItem = true;
		}

		// Not overriding these Clones makes tooltips fail to function correctly due to HoverItem spaghetti.
		public override ModItem Clone()
		{
			var clone = (ShatteredCommunity)base.Clone();
			clone.level = level;
			clone.totalRageDamage = totalRageDamage;
			return clone;
		}
		public override ModItem Clone(Item item)
		{
			var clone = (ShatteredCommunity)base.Clone();
			clone.level = level;
			clone.totalRageDamage = totalRageDamage;
			return clone;
		}

		internal static Color GetRarityColor() => CalamityUtils.ColorSwap(rarityColorOne, rarityColorTwo, 3f);

		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			CalamityPlayer modPlayer = player.Calamity();
			modPlayer.shatteredCommunity = true;

			// Shattered Community gives (mostly) the same boosts as normal Community
			// It does not give melee speed or minion knockback, but always gives life regen (instead of only conditionally)
			player.allDamage += 0.1f;
			modPlayer.AllCritBoost(5);
			player.statDefense += 10;
			player.endurance += 0.05f;
			player.lifeRegen += 2;
			player.moveSpeed += 0.2f;

			// Shattered Community provides a stacking +1% Rage Mode damage per level.
			modPlayer.RageDamageBoost += level * 0.01;
		}

		// Community and Shattered Community are mutually exclusive
		public override bool CanEquipAccessory(Player player, int slot) => !player.Calamity().community;
		public override bool CanUseItem(Player player) => false;

		// Produces purple light while in the world.
		public override void PostUpdate()
		{
			float brightness = Main.essScale;
			Lighting.AddLight(item.Center, 0.92f * brightness, 0.42f * brightness, 0.92f * brightness);
		}

		internal static void AccumulateRageDamage(Player player, CalamityPlayer mp, long damage)
		{
			if (!mp.shatteredCommunity)
				return;
			// Static context, so the item ID has to be retrieved from TML.
			int thisItemID = ModContent.ItemType<ShatteredCommunity>();
			ShatteredCommunity sc = null;

			// First, find the Shattered Community in the player's inventory.
			// slots 3, 4, 5, 6, 7, 8, 9 are valid non-vanity accessories
			for (int i = 3; i <= 9; ++i)
			{
				Item acc = player.armor[i];
				if (acc.type == thisItemID)
					sc = acc.modItem as ShatteredCommunity;
			}

			// Safety check in case for some reason the equipped Shattered Community isn't found.
			if (sc is null)
				return;

			// Actually accumulate the damage.
			sc.totalRageDamage += damage;

			// Level up if applicable.
			if (sc.level < MaxLevel && sc.totalRageDamage > CumulativeLevelCost(sc.level + 1))
			{
				++sc.level;
				sc.LevelUpEffects(player);
			}
		}

		private void LevelUpEffects(Player player)
		{
			// Spawn the purple laser beam from failing the Dungeon Defenders event.
			int projID = ProjectileID.DD2ElderWins;
			Vector2 offset = new Vector2(0f, 800f); // The effect is extremely tall, so start it very low down
			Projectile fx = Projectile.NewProjectileDirect(player.Center + offset, Vector2.Zero, projID, 0, 0f, player.whoAmI);
			fx.friendly = false;
			fx.hostile = false;
			// On the 108th update, crystal debris is spawned, so we avoid that.
			fx.timeLeft = 107;
			fx.MaxUpdates = 2; // Make the animation play at double speed.

			// Play a weird dimensional lightning sound simultaneously.
			var extraSound = SoundID.DD2_EtherianPortalDryadTouch.WithVolume(1.4f);
			Main.PlaySound(extraSound, player.Center);

			// Display a level up text notification.
			Rectangle textArea = new Rectangle((int)player.Center.X, (int)player.Center.Y, 1, 1);
			Color textColor = new Color(236, 209, 236);
			CombatText.NewText(textArea, textColor, "The Community cracks...", false, false);
		}

		public override void ModifyTooltips(List<TooltipLine> tooltips)
		{
			// Stat tooltips are added dynamically.
			StringBuilder sb = new StringBuilder(256);

			// Line 6: Current level
			sb.Append("Current level: ");
			sb.Append(level);
			sb.Append(" (+");
			sb.Append(level);
			sb.Append("% Rage Mode damage)");
			tooltips.Add(new TooltipLine(mod, "Tooltip6", sb.ToString()));
			sb.Clear();

			if (level < MaxLevel)
			{
				long progressToNextLevel = totalRageDamage - CumulativeLevelCost(level);
				long totalToNextLevel = LevelCost(level + 1);
				double ratio = (double)progressToNextLevel / totalToNextLevel;
				string percent = (100D * ratio).ToString("0.00");

				// Line 7: Progress to next level
				sb.Append("Progress to next level: ");
				sb.Append(percent);
				sb.Append('%');
				tooltips.Add(new TooltipLine(mod, "Tooltip7", sb.ToString()));
				sb.Clear();
			}

			// Line 8: Total damage dealt
			sb.Append("Total Rage Mode damage: ");
			sb.Append(totalRageDamage);
			tooltips.Add(new TooltipLine(mod, "Tooltip8", sb.ToString()));
		}

		public override TagCompound Save()
		{
			TagCompound tag = new TagCompound
			{
				{ "level", level },
				{ "totalDamage", totalRageDamage }
			};
			return tag;
		}

		public override void Load(TagCompound tag)
		{
			level = tag.GetInt("level");
			totalRageDamage = tag.GetLong("totalDamage");
		}

		public override void NetSend(BinaryWriter writer)
		{
			writer.Write(level);
			writer.Write(totalRageDamage);
		}

		public override void NetRecieve(BinaryReader reader)
		{
			level = reader.ReadInt32();
			totalRageDamage = reader.ReadInt64();
		}
	}
}
