using CalamityMod.Items.Materials;
using CalamityMod.CalPlayer;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    public class FilthyGlove : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Filthy Glove");
            Tooltip.SetDefault("Stealth strikes have +10 armor penetration and deal 10% more damage");
        }

        public override void SetDefaults()
        {
            item.width = 24;
            item.height = 38;
            item.value = Item.buyPrice(0, 4, 0, 0);
            item.accessory = true;
            item.rare = 3;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.filthyGlove = true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.RottenChunk, 4);
            recipe.AddIngredient(ItemID.DemoniteBar, 4);
            recipe.AddIngredient(ModContent.ItemType<TrueShadowScale>(), 5);
            recipe.AddTile(TileID.DemonAltar);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
