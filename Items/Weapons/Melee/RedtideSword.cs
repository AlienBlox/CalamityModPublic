using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Items.Weapons.Melee
{
    public class RedtideSword : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Redtide Sword");
            Tooltip.SetDefault("Throws short-range whirlpools");
        }

        public override void SetDefaults()
        {
            item.width = 42;
            item.damage = 20;
            item.melee = true;
            item.useAnimation = 19;
            item.useStyle = 1;
            item.useTime = 19;
            item.useTurn = true;
            item.knockBack = 4;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.height = 42;
            item.value = Item.buyPrice(0, 2, 0, 0);
            item.rare = 2;
            item.shoot = ModContent.ProjectileType<Projectiles.Whirlpool>();
            item.shootSpeed = 6f;
        }

        public override bool Shoot(Player player, ref Microsoft.Xna.Framework.Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Projectile.NewProjectile(position.X, position.Y, speedX, speedY, type, damage / 2, knockBack, player.whoAmI, 0f, 0f);
            return false;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "VictideBar", 3);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
