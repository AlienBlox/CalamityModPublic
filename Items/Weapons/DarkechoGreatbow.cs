using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons
{
    public class DarkechoGreatbow : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Darkecho Greatbow");
            Tooltip.SetDefault("Fires two arrows at once\n" +
                "Fires an additional crystal dart");
        }

        public override void SetDefaults()
        {
            item.damage = 34;
            item.ranged = true;
            item.width = 34;
            item.height = 62;
            item.useTime = 22;
            item.useAnimation = 22;
            item.useStyle = 5;
            item.noMelee = true;
            item.knockBack = 4;
            item.value = Item.buyPrice(0, 36, 0, 0);
            item.rare = 5;
            item.UseSound = SoundID.Item5;
            item.autoReuse = true;
            item.shoot = 10;
            item.shootSpeed = 12f;
            item.useAmmo = 40;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            for (int i = 0; i < 2; i++)
            {
                float SpeedX = speedX + (float)Main.rand.Next(-30, 31) * 0.05f;
                float SpeedY = speedY + (float)Main.rand.Next(-30, 31) * 0.05f;
                int index = Projectile.NewProjectile(position.X, position.Y, SpeedX, SpeedY, type, damage, knockBack, player.whoAmI, 0f, 0f);
                Main.projectile[index].noDropItem = true;
            }
            int projectile = Projectile.NewProjectile(position.X, position.Y, speedX, speedY, ProjectileID.CrystalDart, damage, knockBack, player.whoAmI, 0f, 0f);
            Main.projectile[projectile].penetrate = 3;
            return false;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "VerstaltiteBar", 8);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
