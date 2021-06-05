using CalamityMod.CalPlayer;
using CalamityMod.Projectiles.Summon;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.Summon
{
    public class DormantBrimseekerBuff : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Brimseeker");
            Description.SetDefault("Does it want something from you?");
            Main.buffNoTimeDisplay[Type] = true;
            Main.buffNoSave[Type] = true;
			//Main.persistentBuff[Type] = true;
		}

        public override void Update(Player player, ref int buffIndex)
        {
            CalamityPlayer modPlayer = player.Calamity();
            if (player.ownedProjectileCounts[ModContent.ProjectileType<DormantBrimseekerBab>()] > 0)
            {
                modPlayer.brimseeker = true;
            }
            if (!modPlayer.brimseeker)
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
