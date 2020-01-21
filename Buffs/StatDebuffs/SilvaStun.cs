﻿using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.StatDebuffs
{
    public class SilvaStun : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Silva Stun");
            Description.SetDefault("Can't move");
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
            longerExpertDebuff = true;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
			if (npc.Calamity().silvaStun < npc.buffTime[buffIndex])
				npc.Calamity().silvaStun = npc.buffTime[buffIndex];
			npc.DelBuff(buffIndex);
			buffIndex--;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.Calamity().silvaStun = true;
        }
    }
}
