﻿using CalamityMod.CalPlayer;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    public class PlaguedFuelPack : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Plagued Fuel Pack");
            Tooltip.SetDefault("*5 increased rogue damage\n" +
                "15% increased rogue projectile velocity\n" +
                "Press [Hotkey] to consume 25% of your maximum stealth to perform a swift upwards/diagonal dash which leaves a trail of plagued clouds\n" + 
                "This effect has a 3 second cooldown before it can be used again");
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 36;
            item.value = Item.buyPrice(0, 24, 0, 0);
            item.rare = 8;
            item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.Calamity().throwingDamage += 0.08f;
            player.Calamity().throwingVelocity += 0.15f;
            player.Calamity().plaguedFuelPack = true;
        }
    }
}
