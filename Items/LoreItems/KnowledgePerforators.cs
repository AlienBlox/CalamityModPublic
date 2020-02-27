﻿using CalamityMod.CalPlayer;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Items.LoreItems
{
    public class KnowledgePerforators : LoreItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("The Perforators and their Hive");
            Tooltip.SetDefault("An abomination of comingled flesh, bone, and organ, infested primarily by blood-slurping worms.\n" +
                "Place in your inventory for all of your projectiles to inflict ichor when in the crimson.\n" +
				"However, enemy spawn rates will be greatly increased while in the crimson due to your body excreting a sweet-smelling pus.");
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.rare = 3;
            item.consumable = false;
        }

        public override bool CanUseItem(Player player)
        {
            return false;
        }

        public override void UpdateInventory(Player player)
        {
            if (player.ZoneCrimson)
            {
                CalamityPlayer modPlayer = player.Calamity();
                modPlayer.perforatorLore = true;
            }
        }
    }
}
