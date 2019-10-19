using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using CalamityMod.Projectiles.Melee;

namespace CalamityMod.Items.Weapons.Melee
{
    public class GoldplumeSpear : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Goldplume Spear");
            Tooltip.SetDefault("Shoots falling feathers");
        }

        public override void SetDefaults()
        {
            item.width = 54;
            item.damage = 23;
            item.melee = true;
            item.noMelee = true;
            item.useTurn = true;
            item.noUseGraphic = true;
            item.useAnimation = 23;
            item.useStyle = 5;
            item.useTime = 23;
            item.knockBack = 5.75f;
            item.UseSound = SoundID.Item1;
            item.autoReuse = false;
            item.height = 54;
            item.value = Item.buyPrice(0, 4, 0, 0);
            item.rare = 3;
            item.shoot = ModContent.ProjectileType<GoldplumeSpearProjectile>();
            item.shootSpeed = 5f;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<AerialiteBar>(), 10);
            recipe.AddIngredient(ItemID.SunplateBlock, 4);
            recipe.AddTile(TileID.SkyMill);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
