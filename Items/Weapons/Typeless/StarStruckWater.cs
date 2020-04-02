using CalamityMod.Projectiles.Typeless;
using CalamityMod.Items.Placeables;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Typeless
{
    public class StarStruckWater : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Star Struck Water");
            Tooltip.SetDefault("Spreads the astral infection to some blocks");
        }

        public override void SetDefaults()
        {
			item.useStyle = 1;
			item.shootSpeed = 14f;
			item.rare = 3;
			item.damage = 20;
			item.shoot = ModContent.ProjectileType<StarStruckWaterBottle>();
			item.width = 18;
			item.height = 20;
			item.maxStack = 999;
			item.consumable = true;
			item.knockBack = 3f;
			item.UseSound = SoundID.Item1;
			item.useAnimation = 15;
			item.useTime = 15;
			item.noUseGraphic = true;
			item.noMelee = true;
			item.value = 200;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.BottledWater, 10);
            recipe.AddIngredient(ModContent.ItemType<AstralSand>());
            recipe.AddIngredient(ModContent.ItemType<AstralMonolith>());
            recipe.SetResult(this, 10);
            recipe.AddRecipe();
        }
    }
}
