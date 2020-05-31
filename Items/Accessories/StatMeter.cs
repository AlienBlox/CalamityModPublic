using CalamityMod.CalPlayer;
using CalamityMod.World;
using System.Collections.Generic;
using System.Text;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
	public class StatMeter : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Stat Meter");
			Tooltip.SetDefault("Displays almost all player stats");
		}

		public override void SetDefaults()
		{
			item.width = 26;
			item.height = 26;
			item.value = Item.buyPrice(0, 6, 0, 0);
			item.rare = 1;
		}

		public override void ModifyTooltips(List<TooltipLine> list)
		{
			Player player = Main.player[Main.myPlayer];
			if (player is null)
				return;
			CalamityPlayer modPlayer = player.Calamity();

			Item heldItem = null;
			if (player.selectedItem >= 0 && player.selectedItem < Main.maxInventory)
				heldItem = player.ActiveItem();

			// Replace the vanilla tooltip with a full stat readout
			foreach (TooltipLine l in list)
			{
				if (l.mod == "Terraria" && l.Name == "Tooltip0")
					l.text = CreateStatMeterTooltip(player, modPlayer, heldItem);
			}

			// To save screen space, favorited tooltips do not exist for the Stat Meter
			list.RemoveAll(l => l.mod == "Terraria" && (l.Name == "Favorite" || l.Name == "FavoriteDesc"));
		}

		private string CreateStatMeterTooltip(Player player, CalamityPlayer modPlayer, Item heldItem)
		{
			int defense = modPlayer.defenseStat;
			int DR = modPlayer.DRStat;
			int meleeSpeed = modPlayer.meleeSpeedStat;
			int manaCost = modPlayer.manaCostStat;
			int rogueVelocity = modPlayer.rogueVelocityStat;
			int minionSlots = modPlayer.minionSlotStat;
			int ammoConsumption = modPlayer.ammoReductionRanged;
			int rogueConsumption = modPlayer.ammoReductionRogue;
			int rogueStealth = modPlayer.stealthStat;
			// Format the stealth regen statistics to be accurate to 2 decimal places.
			string standingRegen = modPlayer.standingRegenStat.ToString("n2");
			string movingRegen = modPlayer.movingRegenStat.ToString("n2");
			int lifeRegen = modPlayer.lifeRegenStat;
			int manaRegen = modPlayer.manaRegenStat;
			int armorPenetration = modPlayer.armorPenetrationStat;
			int wingFlightTime = modPlayer.wingFlightTimeStat;
			int moveSpeed = modPlayer.moveSpeedStat;
			int lightLevel = modPlayer.abyssLightLevelStat;
			int breathLoss = modPlayer.abyssBreathLossStat;
			int breathLossRate = modPlayer.abyssBreathLossRateStat;
			int lifeLostAtZeroBreath = modPlayer.abyssLifeLostAtZeroBreathStat;
			int defenseLoss = modPlayer.abyssDefenseLossStat;

			// The notice about held item mattering is always displayed first.
			StringBuilder sb = new StringBuilder("Displays almost all player stats\nOffensive stats displayed vary with held item\n\n", 1024);

			// Only append rippers stats in Rev+, if rippers are enabled.
			if (CalamityWorld.revenge && CalamityConfig.Instance.Rippers)
			{
				sb.Append("Adrenaline Charge Time: ").Append(modPlayer.adrenalineChargeStat)
					.Append(" seconds | Rage Damage Boost: ").Append(modPlayer.rageDamageStat)
					.Append("%\n\n");
			}

			// Append item stats only if the held item isn't null, and base it off of the item's damage type.
			if (heldItem != null && !heldItem.IsAir)
			{
				if (heldItem.melee)
				{
					sb.Append("Melee Damage: ").Append(modPlayer.damageStats[0])
						.Append("% | Melee Crit Chance: ").Append(modPlayer.critStats[0])
						.Append("%\nMelee Speed Boost: ").Append(meleeSpeed).Append("%\n\n");
				}
				else if (heldItem.ranged)
				{
					sb.Append("Ranged Damage: ").Append(modPlayer.damageStats[1])
						.Append("% | Ranged Crit Chance: ").Append(modPlayer.critStats[1])
						.Append("%\nAmmo Consumption Chance: ").Append(ammoConsumption).Append("%\n\n");
				}
				else if (heldItem.magic)
				{
					sb.Append("Magic Damage: ").Append(modPlayer.damageStats[2])
						.Append("% | Magic Crit Chance: ").Append(modPlayer.critStats[2])
						.Append("%\nMana Usage: ").Append(manaCost)
						.Append("% | Mana Regen: ").Append(manaRegen).Append("\n\n");
				}
				else if (heldItem.summon)
				{
					sb.Append("Minion Damage: ").Append(modPlayer.damageStats[3])
						.Append("% | Minion Slots: ").Append(minionSlots).Append("\n\n");
				}
				else if (heldItem.Calamity()?.rogue ?? false)
				{
					sb.Append("Rogue Damage: ").Append(modPlayer.damageStats[4])
						.Append("% | Rogue Crit Chance: ").Append(modPlayer.critStats[3])
						.Append("%\nRogue Velocity Boost: ").Append(rogueVelocity)
						.Append("% | Rogue Weapon Consumption Chance: ").Append(rogueConsumption)
						.Append("%\nMax Stealth: ").Append(rogueStealth)
						.Append(" | Standing Regen: ").Append(standingRegen).Append(" / sec")
						.Append(" | Moving Regen: ").Append(movingRegen).Append(" / sec")
						.Append("\n\n");
				}
			}

			// Generic stats always render.
			sb.Append("Defense: ").Append(defense);
			sb.Append(" | DR: ").Append(DR).Append("%\n");
			sb.Append("Life Regen: ").Append(lifeRegen);
			sb.Append(" | Armor Penetration: ").Append(armorPenetration).Append("\n");
			sb.Append("Wing Flight Time: ").Append(wingFlightTime);
			sb.Append(" | Movement Speed Boost: ").Append(moveSpeed).Append("%\n\n");
			sb.Append(CalamityWorld.death ? "Abyss/Cave Light Strength: " : "Abyss Light Strength: ").Append(lightLevel).Append("\n\n");

			// Abyss stats only render if the player is in the Abyss.
			if (modPlayer.ZoneAbyss)
			{
				sb.Append("Other Abyss Stats:").Append("\n");
				sb.Append("Breath Lost Per Tick: ").Append(breathLoss);
				sb.Append(" | Breath Loss Rate: ").Append(breathLossRate).Append("\n");
				sb.Append("Life Lost Per Tick at Zero Breath: ").Append(lifeLostAtZeroBreath).Append("\n");
				sb.Append("Defense Lost: ").Append(defenseLoss);
			}
			else
				sb.Append("Abyss stats only displayed while in the Abyss");

			return sb.ToString();
		}
	}
}
