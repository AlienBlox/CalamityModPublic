using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.Potions
{
    public class Zen : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Zen");
            Description.SetDefault("Spawn rates are reduced");
            Main.debuff[Type] = false;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = false;
            longerExpertDebuff = false;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.Calamity().zen = true;
        }
    }
}
