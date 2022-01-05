using CalamityMod.CalPlayer;
using CalamityMod.World;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Items.Accessories
{
    public class FabledTortoiseShell : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Flame-Licked Shell");
            Tooltip.SetDefault("35% decreased movement speed\n" +
                                "Enemies take damage when they hit you\n" +
                                "You move faster and lose 18 defense for 3 seconds if you take damage\n" +
								"Temporary immunity to lava");
        }

        public override void SetDefaults()
        {
            item.defense = 36;
            item.width = 36;
            item.height = 42;
            item.value = CalamityGlobalItem.Rarity5BuyPrice;
            item.rare = ItemRarityID.Pink;
            item.accessory = true;
			item.Calamity().challengeDrop = true;
		}

		public override void ModifyTooltips(List<TooltipLine> list)
		{
			if (CalamityWorld.death)
			{
				foreach (TooltipLine line2 in list)
				{
					if (line2.mod == "Terraria" && line2.Name == "Tooltip3")
					{
						line2.text = "Temporary immunity to lava\n" +
						"Provides heat protection in Death Mode";
					}
				}
			}
		}

		public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.fabledTortoise = true;
			player.lavaMax += 240;
			float moveSpeedDecrease = modPlayer.shellBoost ? 0.15f : 0.35f;
            player.moveSpeed -= moveSpeedDecrease;
            player.thorns += 0.25f;
			if (modPlayer.shellBoost)
				player.statDefense -= 18;
        }
    }
}
