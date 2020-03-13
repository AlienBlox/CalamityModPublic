﻿using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.StatBuffs
{
    public class HallowedRuneAtkBuff : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Hallowed Power");
            Description.SetDefault("Minion damage boosted by 15%");
            Main.debuff[Type] = false;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
			Main.buffNoTimeDisplay[Type] = true;
            longerExpertDebuff = false;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.Calamity().hallowedPower = true;
        }
    }
}
