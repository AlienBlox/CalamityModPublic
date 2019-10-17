﻿using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Buffs
{
    public class AbyssalWeapon : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Abyssal Weapon");
            Description.SetDefault("Melee weapons inflict abyssal flames, 15% increased movement speed");
            Main.debuff[Type] = false;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = false;
            longerExpertDebuff = false;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.Calamity().aWeapon = true;
        }
    }
}
