﻿using CalamityMod.World;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.StatBuffs
{
    public class AdrenalineMode : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Adrenaline Mode");
            Description.SetDefault("150% damage boost.");
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = false;
            longerExpertDebuff = false;
            canBeCleared = false;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.Calamity().adrenalineModeActive = true;
        }
    }
}
