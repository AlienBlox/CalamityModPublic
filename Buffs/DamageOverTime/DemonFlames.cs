﻿using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.DamageOverTime
{
    public class DemonFlames : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Demon Flames");
            Description.SetDefault("Another burning debuff");
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
            longerExpertDebuff = true;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.Calamity().dFlames = true;
        }
    }
}
