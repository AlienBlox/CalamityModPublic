﻿using CalamityMod.CalPlayer;
using CalamityMod.Projectiles.Typeless;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.Summon
{
    public class GladiatorSwords : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Gladiator Swords");
            Description.SetDefault("The gladiator swords will protect you");
            Main.buffNoTimeDisplay[Type] = true;
            Main.buffNoSave[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            CalamityPlayer modPlayer = player.Calamity();
            if (player.ownedProjectileCounts[ModContent.ProjectileType<GladiatorSword>()] > 0)
            {
                modPlayer.glSword = true;
            }
            if (!modPlayer.glSword)
            {
                player.DelBuff(buffIndex);
                buffIndex--;
            }
            else
            {
                player.buffTime[buffIndex] = 18000;
            }
        }
    }
}
