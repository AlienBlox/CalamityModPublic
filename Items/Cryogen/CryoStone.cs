﻿using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace CalamityMod.Items.Cryogen
{
    public class CryoStone : ModItem
    {
    	public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Cryo Stone");
			Tooltip.SetDefault("One of the ancient relics\n" +
            	"Increases damage reduction by 5% and all damage by 3%");
			Main.RegisterItemAnimation(item.type, new DrawAnimationVertical(6, 5));
		}

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.value = Item.buyPrice(0, 15, 0, 0);
            item.rare = 5;
            item.defense = 6;
			item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
		{
        	Lighting.AddLight((int)player.Center.X / 16, (int)player.Center.Y / 16, 0f, 0.25f, 0.6f);
			player.endurance += 0.05f;
			player.allDamage += 0.03f;
		}
    }
}
