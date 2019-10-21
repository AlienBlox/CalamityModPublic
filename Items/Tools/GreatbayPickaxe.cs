using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Tools
{
    public class GreatbayPickaxe : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Greatbay Pickaxe");
            Tooltip.SetDefault("Can mine Meteorite");
        }

        public override void SetDefaults()
        {
            item.damage = 9;
            item.melee = true;
            item.width = 44;
            item.height = 44;
            item.useTime = 16;
            item.useAnimation = 16;
            item.useTurn = true;
            item.pick = 60;
            item.useStyle = 1;
            item.knockBack = 2f;
            item.value = Item.buyPrice(0, 2, 0, 0);
            item.rare = 2;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<VictideBar>(), 3);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
