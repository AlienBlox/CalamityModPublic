using CalamityMod.CalPlayer;
using CalamityMod.Projectiles.Summon;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.Summon
{
    public class BloodDragonsBuff : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Skeletal Dragons");
            Description.SetDefault("Big happy family");
            Main.buffNoTimeDisplay[Type] = true;
            Main.buffNoSave[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            CalamityPlayer modPlayer = player.Calamity();
            if (player.ownedProjectileCounts[ModContent.ProjectileType<SkeletalDragonMother>()] > 0)
            {
                modPlayer.dragonFamily = true;
            }
            if (!modPlayer.dragonFamily)
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
