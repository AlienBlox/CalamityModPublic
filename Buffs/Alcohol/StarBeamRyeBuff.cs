using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.Alcohol
{
    public class StarBeamRyeBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Star Beam Rye");
            // Description.SetDefault("Max mana and magic damage increased. Defense, mana usage, and life regen reduced");
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = false;
            BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.Calamity().starBeamRye = true;
        }
    }
}
