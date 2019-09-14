﻿using Terraria;
using Terraria.ModLoader;
using CalamityMod.CalPlayer;

namespace CalamityMod.Buffs.StatDebuffs
{
    public class ExtremeGravity : ModBuff
	{
		public override void SetDefaults()
		{
			DisplayName.SetDefault("Icarus' Folly");
			Description.SetDefault("Your wing time is reduced by 75%, infinite flight is disabled");
			Main.debuff[Type] = true;
			Main.pvpBuff[Type] = true;
			Main.buffNoSave[Type] = true;
			longerExpertDebuff = false;
			canBeCleared = false;
		}

		public override void Update(Player player, ref int buffIndex)
		{
			player.GetCalamityPlayer().eGravity = true;
		}
	}
}
