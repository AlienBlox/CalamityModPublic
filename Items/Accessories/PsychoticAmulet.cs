using CalamityMod.CalPlayer;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    public class PsychoticAmulet : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Psychotic Amulet");
            Tooltip.SetDefault("Boosts rogue and ranged damage and critical strike chance by 5%\n" +
                               "Grants a massive boost to these stats if you aren't moving");
        }

        public override void SetDefaults()
        {
            item.width = 26;
            item.height = 26;
            item.value = Item.buyPrice(0, 15, 0, 0);
            item.rare = 6;
            item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.pAmulet = true;
            player.shroomiteStealth = true;
            modPlayer.throwingDamage += 0.05f;
            modPlayer.throwingCrit += 5;
            player.rangedDamage += 0.05f;
            player.rangedCrit += 5;
        }
    }
}
