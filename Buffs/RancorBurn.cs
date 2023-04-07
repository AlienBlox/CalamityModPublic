using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Buffs
{
    public class RancorBurn : ModBuff
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Rancor Burn");
            // Description.SetDefault("Burning");
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
            BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            if (npc.Calamity().RancorBurnTime < npc.buffTime[buffIndex])
                npc.Calamity().RancorBurnTime = npc.buffTime[buffIndex];
            npc.DelBuff(buffIndex);
            buffIndex--;
        }
    }
}
