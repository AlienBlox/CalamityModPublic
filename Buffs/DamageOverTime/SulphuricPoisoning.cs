using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.DamageOverTime
{
    public class SulphuricPoisoning : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Sulphuric Poisoning");
            Description.SetDefault("The illness has spread"); //nope, not an AHiT reference, definitely not
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
            longerExpertDebuff = false;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.Calamity().sulphurPoison = true;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
			if (npc.Calamity().sulphurPoison < npc.buffTime[buffIndex])
				npc.Calamity().sulphurPoison = npc.buffTime[buffIndex];
			npc.DelBuff(buffIndex);
			buffIndex--;
        }
    }
}
