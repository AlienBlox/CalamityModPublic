﻿using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.CalPlayer;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Items.Accessories
{
    [LegacyName("FabledTortoiseShell")]
    public class FlameLickedShell : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
            // DisplayName.SetDefault("Flame-Licked Shell");
            /* Tooltip.SetDefault("35% decreased movement speed\n" +
                                "Enemies take damage when they hit you\n" +
                                "You move faster and lose 18 defense for 3 seconds if you take damage\n" +
                                "Grants immunity to lava and Armor Crunch"); */
        }

        public override void SetDefaults()
        {
            Item.defense = 36;
            Item.width = 36;
            Item.height = 42;
            Item.value = CalamityGlobalItem.Rarity5BuyPrice;
            Item.rare = ItemRarityID.Pink;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.flameLickedShell = true;
            player.buffImmune[ModContent.BuffType<ArmorCrunch>()] = true;
            player.lavaImmune = true;
            float moveSpeedDecrease = modPlayer.shellBoost ? 0.15f : 0.35f;
            player.moveSpeed -= moveSpeedDecrease;
            player.thorns += 0.25f;
            if (modPlayer.shellBoost)
                player.statDefense -= 18;
        }
    }
}
