﻿using CalamityMod.CalPlayer;
using CalamityMod.Rarities;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    public class TheEvolution : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
            // DisplayName.SetDefault("The Evolution");
            /* Tooltip.SetDefault("You reflect projectiles when they hit you\n" +
                                "Reflected projectiles deal no damage to you\n" +
                                "This reflect has a 90 second cooldown which is shared with all other dodges and reflects\n" +
                                "If this effect triggers you get a health regeneration boost for 5 seconds\n" +
                                "If the same enemy projectile type hits you again you will resist its damage by 15%"); */
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(6, 10));
            ItemID.Sets.AnimatesAsSoul[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 58;
            Item.height = 44;
            Item.value = CalamityGlobalItem.Rarity12BuyPrice;
            Item.rare = ModContent.RarityType<Turquoise>();
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.evolution = true;
        }
    }
}
