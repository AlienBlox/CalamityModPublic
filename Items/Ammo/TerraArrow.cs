using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using CalamityMod.Projectiles.Ranged;

namespace CalamityMod.Items.Ammo
{
    public class TerraArrow : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Terra Arrow");
            Tooltip.SetDefault("Travels incredibly quickly and explodes into more arrows when it hits a certain velocity");
        }

        public override void SetDefaults()
        {
            item.damage = 9;
            item.ranged = true;
            item.width = 22;
            item.height = 36;
            item.maxStack = 999;
            item.consumable = true;
            item.knockBack = 1.5f;
            item.value = Item.sellPrice(0, 0, 0, 40);
            item.rare = 7;
            item.shoot = ModContent.ProjectileType<TerraArrowMain>();
            item.shootSpeed = 15f;
            item.ammo = 40;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<LivingShard>());
            recipe.AddIngredient(ItemID.WoodenArrow, 250);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this, 250);
            recipe.AddRecipe();
        }
    }
}
