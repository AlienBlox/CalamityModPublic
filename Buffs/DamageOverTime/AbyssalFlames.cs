using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.DamageOverTime
{
    public class AbyssalFlames : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Abyssal Flames");
            Description.SetDefault("Your soul is being consumed");
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
            longerExpertDebuff = false;
            canBeCleared = false;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.Calamity().aFlames = true;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
			if (npc.Calamity().aFlames < npc.buffTime[buffIndex])
				npc.Calamity().aFlames = npc.buffTime[buffIndex];
			npc.DelBuff(buffIndex);
			buffIndex--;
        }
    }
}
