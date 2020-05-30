using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.Alcohol
{
    public class EvergreenGinBuff : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Evergreen Gin");
            Description.SetDefault("Nature-based weapon damage and damage reduction boosted, life regen reduced");
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = false;
            longerExpertDebuff = false;
            canBeCleared = false;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.Calamity().evergreenGin = true;
        }
    }
}
