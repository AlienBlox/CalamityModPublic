using CalamityMod.Projectiles.Magic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Magic
{
    public class IonBlaster : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ion Blaster");
            Tooltip.SetDefault("Fires ion blasts that speed up and then explode\n" +
                "The higher your mana the more damage they will do");
        }

        public override void SetDefaults()
        {
            item.width = 44;
            item.damage = 30;
            item.magic = true;
            item.mana = 6;
            item.useAnimation = 10;
            item.useTime = 10;
            item.useStyle = 5;
            item.knockBack = 5.5f;
            item.UseSound = SoundID.Item91;
            item.autoReuse = true;
            item.noMelee = true;
            item.height = 28;
            item.value = Item.buyPrice(0, 36, 0, 0);
            item.rare = 5;
            item.shoot = ModContent.ProjectileType<IonBlast>();
            item.shootSpeed = 3f;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-5, 0);
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            float manaAmount = (float)player.statMana * 0.01f;
            float damageMult = manaAmount * 0.75f;
            int projectile = Projectile.NewProjectile(position.X, position.Y, speedX, speedY, type, (int)((double)damage * damageMult), knockBack, player.whoAmI, 0.0f, 0.0f);
            Main.projectile[projectile].scale = manaAmount;
            return false;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.SoulofFright, 10);
            recipe.AddRecipeGroup("AnyAdamantiteBar", 7);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
