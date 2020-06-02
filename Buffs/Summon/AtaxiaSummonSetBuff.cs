using CalamityMod.CalPlayer;
using CalamityMod.Projectiles.Summon;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.Summon
{
    public class AtaxiaSummonSetBuff : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Hydrothermic Vent");
            Description.SetDefault("The hydrothermic vent will protect you");
            Main.buffNoTimeDisplay[Type] = true;
            Main.buffNoSave[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            CalamityPlayer modPlayer = player.Calamity();
            if (player.ownedProjectileCounts[ModContent.ProjectileType<ChaosSpirit>()] > 0)
            {
                modPlayer.cSpirit = true;
            }
            if (!modPlayer.cSpirit)
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
