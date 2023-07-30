﻿using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.NPCs;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Buffs
{
    public class CalamityGlobalBuff : GlobalBuff
    {
        public const byte ModdedFlaskEnchant = 99;

        public override void Update(int type, Player player, ref int buffIndex)
        {
            if (type == BuffID.Archery)
            {
                player.arrowDamage *= 0.875f;
            }
            else if (type == BuffID.Ironskin)
            {
                player.statDefense += CalamityUtils.GetScalingDefense(-1) - 8;
            }
            else if (type == BuffID.MagicPower)
            {
                player.GetDamage<MagicDamageClass>() -= 0.1f;
            }
            else if (type == BuffID.Panic)
            {
                player.moveSpeed -= 0.6f;
            }
            else if (type == BuffID.SugarRush)
            {
                player.moveSpeed -= 0.1f;
                player.pickSpeed += 0.1f;
            }
            else if (type == BuffID.Mining)
            {
                player.pickSpeed += 0.1f;
            }
            else if (type == BuffID.Swiftness)
            {
                player.moveSpeed -= 0.1f;
            }
            else if (type == BuffID.WellFed)
            {
                player.moveSpeed -= 0.15f;
                player.GetAttackSpeed<MeleeDamageClass>() -= 0.05f;
            }
            else if (type == BuffID.WellFed2)
            {
                player.moveSpeed -= 0.225f;
                player.pickSpeed += 0.025f;
                player.GetAttackSpeed<MeleeDamageClass>() -= 0.075f;
            }
            else if (type == BuffID.WellFed3)
            {
                player.moveSpeed -= 0.3f;
                player.pickSpeed += 0.05f;
                player.GetAttackSpeed<MeleeDamageClass>() -= 0.1f;
            }
            else if (type == BuffID.Shine)
            {
                player.Calamity().shine = true;
            }
            else if (type == BuffID.IceBarrier)
            {
                player.endurance -= 0.1f;
            }
            else if (type >= BuffID.NebulaUpDmg1 && type <= BuffID.NebulaUpDmg3)
            {
                float nebulaDamage = 0.075f * player.nebulaLevelDamage; // 15% to 45% changed to 7.5% to 22.5%
                player.GetDamage<GenericDamageClass>() -= nebulaDamage;
            }
            else if (type == BuffID.Warmth)
            {
                player.buffImmune[ModContent.BuffType<GlacialState>()] = true;
                player.buffImmune[BuffID.Frozen] = true;
                player.buffImmune[BuffID.Chilled] = true;
            }
            else if (type == BuffID.Rabies)
            {
                player.GetDamage<GenericDamageClass>() -= 0.2f;
            }
            else if (type == BuffID.BeetleMight1)
            {
                player.GetAttackSpeed<MeleeDamageClass>() -= 0.1f;
            }
            else if (type == BuffID.BeetleMight2)
            {
                player.GetAttackSpeed<MeleeDamageClass>() -= 0.2f;
            }
            else if (type == BuffID.BeetleMight3)
            {
                player.GetAttackSpeed<MeleeDamageClass>() -= 0.3f;
            }
        }

        public override void Update(int type, NPC npc, ref int buffIndex)
        {
            if (type == BuffID.Webbed)
            {
                if (npc.Calamity().webbed < npc.buffTime[buffIndex])
                    npc.Calamity().webbed = npc.buffTime[buffIndex];
                if ((CalamityLists.enemyImmunityList.Contains(npc.type) || npc.boss) && npc.Calamity().debuffResistanceTimer <= 0)
                    npc.Calamity().debuffResistanceTimer = CalamityGlobalNPC.slowingDebuffResistanceMin + npc.Calamity().webbed;
                npc.DelBuff(buffIndex);
                buffIndex--;
            }
            else if (type == BuffID.Slow)
            {
                if (npc.Calamity().slowed < npc.buffTime[buffIndex])
                    npc.Calamity().slowed = npc.buffTime[buffIndex];
                if ((CalamityLists.enemyImmunityList.Contains(npc.type) || npc.boss) && npc.Calamity().debuffResistanceTimer <= 0)
                    npc.Calamity().debuffResistanceTimer = CalamityGlobalNPC.slowingDebuffResistanceMin + npc.Calamity().slowed;
                npc.DelBuff(buffIndex);
                buffIndex--;
            }
            if (type == BuffID.Electrified)
            {
                if (npc.Calamity().electrified < npc.buffTime[buffIndex])
                    npc.Calamity().electrified = npc.buffTime[buffIndex];
                npc.DelBuff(buffIndex);
                buffIndex--;
            }
        }

        public override void ModifyBuffText(int type, ref string buffName, ref string tip, ref int rare)
        {
            // Vanilla buffs
            switch (type)
            {
                case BuffID.Ironskin:
                    tip = "Increase defense by " + CalamityUtils.GetScalingDefense(-1);
                    break;

                case BuffID.MagicPower:
                    tip = "10% increased magic damage";
                    break;

                case BuffID.Archery:
                    tip = "20% increased arrow speed and 5% increased arrow damage";
                    break;

                case BuffID.Swiftness:
                    tip = "15% increased movement speed";
                    break;

                case BuffID.SugarRush:
                    tip = "10% increased movement and mining speed";
                    break;

                case BuffID.Mining:
                    tip = "15% increased mining speed";
                    break;

                case BuffID.LeafCrystal:
                    tip = "Emits powerful life pulses at nearby enemies";
                    break;

                case BuffID.NebulaUpDmg1:
                    tip = "7.5% increased damage";
                    break;

                case BuffID.NebulaUpDmg2:
                    tip = "15% increased damage";
                    break;

                case BuffID.NebulaUpDmg3:
                    tip = "22.5% increased damage";
                    break;

                case BuffID.BeetleMight1:
                    tip = "Melee damage increased by 10%";
                    break;

                case BuffID.BeetleMight2:
                    tip = "Melee damage increased by 20%";
                    break;

                case BuffID.BeetleMight3:
                    tip = "Melee damage increased by 30%";
                    break;

                case BuffID.BeetleEndurance1:
                    tip = "Damage taken reduced by 10%";
                    break;

                case BuffID.BeetleEndurance2:
                    tip = "Damage taken reduced by 20%";
                    break;

                case BuffID.BeetleEndurance3:
                    tip = "Damage taken reduced by 30%";
                    break;

                case BuffID.WeaponImbueVenom:
                case BuffID.WeaponImbueCursedFlames:
                case BuffID.WeaponImbueFire:
                case BuffID.WeaponImbueGold:
                case BuffID.WeaponImbueIchor:
                case BuffID.WeaponImbueNanites:
                case BuffID.WeaponImbuePoison:
                    tip = tip.Insert(tip.IndexOf("Melee") + 5, ", Whip, and Rogue");
                    break;

                case BuffID.WeaponImbueConfetti:
                    tip = "All attacks cause confetti to appear";
                    break;

                case BuffID.IceBarrier:
                    tip = "Damage taken is reduced by 15%";
                    break;

                case BuffID.ChaosState:
                    tip = "Rod of Discord teleports are disabled";
                    break;

                case BuffID.Warmth:
                    tip += ". Grants immunity to Chilled, Frozen and Glacial State";
                    break;

                case BuffID.WellFed:
                    tip = "Minor improvements to all stats";
                    break;

                case BuffID.WellFed2:
                    tip = "Medium improvements to all stats";
                    break;

                case BuffID.WellFed3:
                    tip = "Major improvements to all stats";
                    break;
            }
        }
    }
}
