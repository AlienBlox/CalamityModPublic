﻿using CalamityMod.CalPlayer;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Items.Accessories
{
    [AutoloadEquip(new EquipType[] { EquipType.HandsOn, EquipType.HandsOff } )]
    public class GloveOfRecklessness : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
            // DisplayName.SetDefault("Glove of Recklessness");
            /* Tooltip.SetDefault("Increases rogue attack speed by 15% but decreases damage by 13%\n" +
                               "15% increased stealth regeneration\n" +
                               "Adds inaccuracy to rogue weapons"); */
        }

        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 36;
            Item.value = CalamityGlobalItem.Rarity7BuyPrice;
            Item.accessory = true;
            Item.rare = ItemRarityID.Lime;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.gloveOfRecklessness = true;
            modPlayer.stealthGenStandstill += 0.15f;
            modPlayer.stealthGenMoving += 0.15f;
            player.GetDamage<RogueDamageClass>() -= 0.13f;
            player.GetAttackSpeed<RogueDamageClass>() += 0.15f;
        }
    }
}
