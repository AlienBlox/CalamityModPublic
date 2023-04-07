using CalamityMod.CalPlayer;
using CalamityMod.Projectiles.Summon;
using Terraria;
using Terraria.ModLoader;
namespace CalamityMod.Buffs.Summon
{
    public class Calamari : ModBuff
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Calamari");
            // Description.SetDefault("The squid will protect you");
            Main.buffNoTimeDisplay[Type] = true;
            Main.buffNoSave[Type] = true;
            //Main.persistentBuff[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            CalamityPlayer modPlayer = player.Calamity();
            if (player.ownedProjectileCounts[ModContent.ProjectileType<CalamariMinion>()] > 0)
            {
                modPlayer.calamari = true;
            }
            if (!modPlayer.calamari)
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
