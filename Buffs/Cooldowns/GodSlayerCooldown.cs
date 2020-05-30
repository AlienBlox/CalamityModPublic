using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.Cooldowns
{
    public class GodSlayerCooldown : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("God Slayer Cooldown");
            Description.SetDefault("10% increase to all damage; godslayer effect is recharging");
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
            longerExpertDebuff = false;
            canBeCleared = false;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.Calamity().godSlayerCooldown = true;
        }
    }
}
