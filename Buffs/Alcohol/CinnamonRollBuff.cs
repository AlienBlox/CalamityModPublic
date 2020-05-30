using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.Alcohol
{
    public class CinnamonRollBuff : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Cinnamon Roll");
            Description.SetDefault("Mana regen rate and fire weapon damage boosted, defense reduced");
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = false;
            longerExpertDebuff = false;
            canBeCleared = false;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.Calamity().cinnamonRoll = true;
        }
    }
}
