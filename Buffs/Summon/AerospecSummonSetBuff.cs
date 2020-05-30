using CalamityMod.CalPlayer;
using CalamityMod.Projectiles.Summon;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.Summon
{
    public class AerospecSummonSetBuff : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Valkyrie");
            Description.SetDefault("The valkyrie will protect you");
            Main.buffNoTimeDisplay[Type] = true;
            Main.buffNoSave[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            CalamityPlayer modPlayer = player.Calamity();
            if (player.ownedProjectileCounts[ModContent.ProjectileType<Valkyrie>()] > 0)
            {
                modPlayer.aValkyrie = true;
            }
            if (!modPlayer.aValkyrie)
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
