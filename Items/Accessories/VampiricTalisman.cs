using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    public class VampiricTalisman : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Vampiric Talisman");
            Tooltip.SetDefault(@"Rogue projectiles give lifesteal on crits
12% increased rogue damage");
        }

        public override void SetDefaults()
        {
            item.width = 50;
            item.height = 26;
            item.value = Item.buyPrice(0, 40, 0, 0);
            item.accessory = true;
            item.rare = 6;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.vampiricTalisman = true;
            player.Calamity().throwingDamage += 0.12f;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<RogueEmblem>());
            recipe.AddIngredient(ModContent.ItemType<SolarVeil>(), 10);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
