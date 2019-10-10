﻿using Terraria;
using Terraria.ModLoader;
using CalamityMod.CalPlayer;

namespace CalamityMod.Buffs.StatDebuffs
{
    public class ExtremeGrav : ModBuff
	{
		public override void SetDefaults()
		{
			DisplayName.SetDefault("Extreme Gravity");
			Description.SetDefault("Your wing time is reduced by 50%, infinite flight is disabled");
			Main.buffNoTimeDisplay[Type] = true;
			Main.debuff[Type] = true;
			Main.pvpBuff[Type] = true;
			Main.buffNoSave[Type] = true;
			longerExpertDebuff = false;
			canBeCleared = false;
		}

		public override void Update(Player player, ref int buffIndex)
		{
			player.Calamity().eGrav = true;
		}
	}
}
