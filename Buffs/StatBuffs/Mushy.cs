﻿using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Buffs
{
    public class Mushy : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Mushy");
            Description.SetDefault("Increased defense and life regen");
            Main.buffNoTimeDisplay[Type] = true;
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
            longerExpertDebuff = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.Calamity().mushy = true;
        }
    }
}
