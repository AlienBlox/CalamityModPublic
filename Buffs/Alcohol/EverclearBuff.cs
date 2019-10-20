﻿using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.Alcohol
{
    public class EverclearBuff : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Everclear");
            Description.SetDefault("25% increased damage, -10 life regen and -40 defense");
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = false;
            longerExpertDebuff = false;
            canBeCleared = false;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.Calamity().everclear = true;
        }
    }
}
