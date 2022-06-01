﻿using System.Text;
using Terraria;
using Terraria.ID;

namespace CalamityMod.Items.VanillaArmorChanges
{
    public class LeadArmorSetChange : VanillaArmorChange
    {
        public override int? HeadPieceID => ItemID.LeadHelmet;

        public override int? BodyPieceID => ItemID.LeadChainmail;

        public override int? LegPieceID => ItemID.LeadGreaves;

        public override string ArmorSetName => "Lead";

        public const float ArmorPieceDR = 0.03f;
        public const float SetBonusDR = 0.03f;
        public const int SetBonusLifeRegen = 1;
        public const int SetBonusMiningSpeedPercent = 25;

        public override void ApplyHeadPieceEffect(Player player) => player.endurance += ArmorPieceDR;

        public override void ApplyBodyPieceEffect(Player player) => player.endurance += ArmorPieceDR;

        public override void ApplyLegPieceEffect(Player player) => player.endurance += ArmorPieceDR;

        public override void UpdateSetBonusText(ref string setBonusText)
        {
            StringBuilder sb = new StringBuilder(256);
            sb.Append("\nReduces damage taken by 3%, +1 life regen\nGrants immunity to knockback");
            sb.Append(CalamityGlobalItem.MiningSpeedString(SetBonusMiningSpeedPercent));
            setBonusText += sb.ToString();
        }

        public override void ApplyArmorSetBonus(Player player)
        {
            player.endurance += SetBonusDR;
            player.lifeRegen += SetBonusLifeRegen;
            player.noKnockback = true;
            player.pickSpeed -= SetBonusMiningSpeedPercent * 0.01f;
        }
    }
}
