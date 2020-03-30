using CalamityMod.CalPlayer;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    public class ScuttlersJewel : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Scuttler's Jewel");
            Tooltip.SetDefault("Rogue javelin projectiles have a chance to spawn a jewel spike when destroyed");
        }

        public override void SetDefaults()
        {
            item.width = 26;
            item.height = 26;
            item.value = Item.buyPrice(0, 3, 0, 0);
            item.rare = 1;
            item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.scuttlersJewel = true;
        }
    }
}
