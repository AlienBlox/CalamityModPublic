﻿using CalamityMod.CalPlayer;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.StatBuffs
{
    public class AquaticHeartBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
            BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            CalamityPlayer modPlayer = player.Calamity();
            if (modPlayer.aquaticHeartPrevious)
            {
                modPlayer.aquaticHeartPower = true;
                player.ignoreWater = NPC.downedBoss3;
                player.accFlipper = true;
                if (player.breath <= player.breathMax + 2 && !modPlayer.ZoneAbyss && NPC.downedBoss3)
                {
                    player.breath = player.breathMax + 3;
                }
                if (Main.myPlayer == player.whoAmI && player.wet && NPC.downedBoss3)
                {
                    player.AddBuff(ModContent.BuffType<AquaticHeartWaterSpeed>(), 360);
                }
            }
            else
            {
                player.DelBuff(buffIndex);
                buffIndex--;
            }
        }
    }
}
