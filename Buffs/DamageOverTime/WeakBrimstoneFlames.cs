using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.DamageOverTime
{
    public class WeakBrimstoneFlames : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Weak Brimstone Flames");
            Description.SetDefault("Health loss");
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
            longerExpertDebuff = false;
        }

        public override void Update(Player player, ref int buffIndex) => player.Calamity().weakBrimstoneFlames = true;
    }
}
