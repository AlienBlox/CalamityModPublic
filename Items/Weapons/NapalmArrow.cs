using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons
{
    public class NapalmArrow : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Napalm Arrow");
            Tooltip.SetDefault("Explodes into fire shards");
        }

        public override void SetDefaults()
        {
            item.damage = 13;
            item.ranged = true;
            item.width = 22;
            item.height = 36;
            item.maxStack = 999;
            item.consumable = true;
            item.knockBack = 1.5f;
            item.value = 1000;
            item.rare = 3;
            item.shoot = mod.ProjectileType("NapalmArrow");
            item.shootSpeed = 13f;
            item.ammo = 40;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "EssenceofChaos");
            recipe.AddIngredient(ItemID.Torch);
            recipe.AddIngredient(ItemID.WoodenArrow, 250);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this, 250);
            recipe.AddRecipe();
        }
    }
}
