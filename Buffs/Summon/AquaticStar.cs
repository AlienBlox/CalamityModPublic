using CalamityMod.CalPlayer;
using CalamityMod.Projectiles.Summon;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.Summon
{
    public class AquaticStar : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Aquatic Star");
            Description.SetDefault("The aquatic star will protect you");
            Main.buffNoTimeDisplay[Type] = true;
            Main.buffNoSave[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            CalamityPlayer modPlayer = player.Calamity();
            if (player.ownedProjectileCounts[ModContent.ProjectileType<AquaticStarMinion>()] > 0)
            {
                modPlayer.aStar = true;
            }
            if (!modPlayer.aStar)
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
