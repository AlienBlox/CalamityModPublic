﻿using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.PotionBuffs
{
    public class BoundingBuff : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Bounding");
            Description.SetDefault("Increased jump height, jump speed, and fall damage resistance");
            Main.debuff[Type] = false;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = false;
            longerExpertDebuff = false;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.Calamity().bounding = true;
        }
    }
}
