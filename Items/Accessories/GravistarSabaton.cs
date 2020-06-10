using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
	public class GravistarSabaton : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Gravistar Sabaton");
            Tooltip.SetDefault("Tap the DOWN key to increase your fall speed for 5 seconds\n" +
                               "This has a 8 second cooldown\n" +
                               "Striking the ground with increased fall speed will cause an astral explosion");
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 22;
            item.value = CalamityGlobalItem.Rarity7BuyPrice;
            item.accessory = true;
            item.expert = true;
            item.rare = 9;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.Calamity().gSabaton = true;
        }
    }
}
