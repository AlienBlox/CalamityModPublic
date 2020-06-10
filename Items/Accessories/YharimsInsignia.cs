using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    public class YharimsInsignia : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Yharim's Insignia");
            Tooltip.SetDefault("10% increased damage when under 50% life\n" +
                "10% increased melee speed\n" +
                "5% increased melee damage\n" +
                "Melee attacks and melee projectiles inflict holy fire\n" +
                "Increased invincibility after taking damage\n" +
                "Temporary immunity to lava\n" +
                "Increased melee knockback\n" +
				"Provides heat protection in Death Mode");
        }

        public override void SetDefaults()
        {
            item.width = 22;
            item.height = 38;
            item.value = CalamityGlobalItem.Rarity12BuyPrice;
            item.accessory = true;
            item.Calamity().customRarity = CalamityRarity.Turquoise;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.yInsignia = true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.WarriorEmblem);
            recipe.AddIngredient(ModContent.ItemType<NecklaceofVexation>());
            recipe.AddIngredient(ModContent.ItemType<CoreofCinder>(), 5);
            recipe.AddIngredient(ItemID.CrossNecklace);
            recipe.AddIngredient(ModContent.ItemType<BadgeofBravery>());
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
