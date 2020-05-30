using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class MarniteRifleSpear : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Marnite Bayonet");
            Tooltip.SetDefault("The gun damages enemies that touch it");
        }

        public override void SetDefaults()
        {
            item.damage = 20;
            item.ranged = true;
            item.width = 72;
            item.height = 20;
            item.useTime = 28;
            item.useAnimation = 28;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.knockBack = 2.25f;
            item.value = Item.buyPrice(0, 2, 0, 0);
            item.rare = 2;
            item.UseSound = SoundID.Item41;
            item.autoReuse = true;
            item.shootSpeed = 22f;
            item.useAmmo = 97;
            item.shoot = ProjectileID.PurificationPowder;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddRecipeGroup("AnyGoldBar", 7);
            recipe.AddIngredient(ItemID.Granite, 5);
            recipe.AddIngredient(ItemID.Marble, 5);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
