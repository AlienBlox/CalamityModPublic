﻿using CalamityMod.CalPlayer;
using CalamityMod.NPCs;
using CalamityMod.World;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    public class GoldBurdenBreaker : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Burden Breaker");
            Tooltip.SetDefault("The good time\n" +
				"Go fast\n" +
				"WARNING: May have disastrous results\n" +
				"Increases horizontal movement speed beyond comprehension");
        }

        public override void SetDefaults()
        {
            item.width = 28;
            item.height = 28;
            item.value = Item.buyPrice(0, 21, 0, 0);
            item.rare = 10;
            item.accessory = true;
            item.Calamity().customRarity = CalamityRarity.Turquoise;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            if (CalamityPlayer.areThereAnyDamnBosses)
            { return; }
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.dashMod = modPlayer.dashMod == 7 ? 0 : modPlayer.dashMod; //statis belt memes for projectile spam :feelsgreat:

            if (player.velocity.X > 5f)
            {
                player.velocity.X *= 1.025f;
                if (player.velocity.X >= 500f)
                {
                    player.velocity.X = 0f;
                }
            }
            else if (player.velocity.X < -5f)
            {
                player.velocity.X *= 1.025f;
                if (player.velocity.X <= -500f)
                {
                    player.velocity.X = 0f;
                }
            }
        }
    }
}
