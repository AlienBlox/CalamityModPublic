using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.StatDebuffs
{
    public class WhisperingDeath : ModBuff
    {
        public static int DefenseReduction = 20;

        public override void SetDefaults()
        {
            DisplayName.SetDefault("Whispering Death");
            Description.SetDefault("Death approaches; defense, attack power, and life regen reduced");
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
            longerExpertDebuff = false;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.Calamity().wDeath = true;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
			if (npc.Calamity().wDeath < npc.buffTime[buffIndex])
				npc.Calamity().wDeath = npc.buffTime[buffIndex];
			npc.DelBuff(buffIndex);
			buffIndex--;
        }
    }
}
