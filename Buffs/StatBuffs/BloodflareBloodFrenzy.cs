﻿using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.StatBuffs
{
    public class BloodflareBloodFrenzy : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Blood Frenzy");
            Description.SetDefault("Contact damage is reduced and melee stats are greatly increased");
            Main.debuff[Type] = false;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = false;
            longerExpertDebuff = false;
            canBeCleared = false;
        }

		public override void Update(Player player, ref int buffIndex)
		{
			player.Calamity().bloodflareFrenzy = true;
		}
	}
}
