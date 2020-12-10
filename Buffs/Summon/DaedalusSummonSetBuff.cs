using CalamityMod.CalPlayer;
using CalamityMod.Projectiles.Summon;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.Summon
{
    public class DaedalusSummonSetBuff : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Daedalus Crystal");
            Description.SetDefault("The daedalus crystal will protect you");
            Main.buffNoTimeDisplay[Type] = true;
            Main.buffNoSave[Type] = true;
			//Main.persistentBuff[Type] = true;
		}

        public override void Update(Player player, ref int buffIndex)
        {
            CalamityPlayer modPlayer = player.Calamity();
            if (player.ownedProjectileCounts[ModContent.ProjectileType<DaedalusCrystal>()] > 0)
            {
                modPlayer.dCrystal = true;
            }
            if (!modPlayer.dCrystal)
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
