﻿using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.StatBuffs
{
    public class HallowedRuneRegenBuff : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Hallowed Regen");
            Description.SetDefault("Regenerating life");
            Main.debuff[Type] = false;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
			Main.buffNoTimeDisplay[Type] = true;
            longerExpertDebuff = false;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.Calamity().hallowedRegen = true;
        }
    }
}
