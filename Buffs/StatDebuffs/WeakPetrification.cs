﻿using Terraria;
using Terraria.ModLoader;
using CalamityMod.CalPlayer;

namespace CalamityMod.Buffs.StatDebuffs
{
    public class WeakPetrification : ModBuff
	{
		public override void SetDefaults()
		{
			DisplayName.SetDefault("Weak Petrification");
			Description.SetDefault("Your vertical movement is weakened");
			Main.buffNoTimeDisplay[Type] = true;
			Main.debuff[Type] = true;
			Main.pvpBuff[Type] = true;
			Main.buffNoSave[Type] = true;
			longerExpertDebuff = false;
			canBeCleared = false;
		}

		public override void Update(Player player, ref int buffIndex)
		{
			player.GetModPlayer<CalamityPlayer>(mod).weakPetrification = true;
		}
	}
}
