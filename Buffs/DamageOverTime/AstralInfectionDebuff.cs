﻿using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.DamageOverTime
{
    public class AstralInfectionDebuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
            BuffID.Sets.LongerExpertDebuff[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.Calamity().astralInfection = true;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            if (npc.Calamity().astralInfection < npc.buffTime[buffIndex])
                npc.Calamity().astralInfection = npc.buffTime[buffIndex];
            npc.DelBuff(buffIndex);
            buffIndex--;
        }

        internal static void DrawEffects(Player player)
        {

        }

        internal static void DrawEffects(NPC npc, ref Color drawColor)
        {
            if (Main.rand.NextBool(5))
            {
                Vector2 npcSize = npc.Center + new Vector2(Main.rand.NextFloat(-npc.width / 2, npc.width / 2), Main.rand.NextFloat(-npc.height / 2, npc.height / 2));
                Vector2 npcSize2 = npc.Center + new Vector2(Main.rand.NextFloat(-npc.width / 2, npc.width / 2), Main.rand.NextFloat(-npc.height / 2, npc.height / 2));
                DirectionalPulseRing pulse = new DirectionalPulseRing(npcSize, Vector2.Zero, Main.rand.NextBool(2) ? Color.DarkTurquoise : Color.Coral, new Vector2(1, 1), 0, 0.08f + (0.000003f * npc.width * npc.height), 0f, 20);
                GeneralParticleHandler.SpawnParticle(pulse);
                Particle orb = new GenericBloom(npcSize2, Vector2.Zero, Main.rand.NextBool(2) ? Color.DarkTurquoise : Color.Coral, 0.055f + (0.000003f * npc.width * npc.height), 8);
                GeneralParticleHandler.SpawnParticle(orb);
            }
        }
    }
}
