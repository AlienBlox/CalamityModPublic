using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using CalamityMod.Items.Materials;
using Terraria.ID;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class ClaretCannon : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Claret Cannon");
            Tooltip.SetDefault("Fires a string of bloodfire bullets that drain enemy health");
        }

        public override void SetDefaults()
        {
            item.damage = 215;
            item.ranged = true;
            item.width = 48;
            item.height = 30;
            item.useTime = 3;
            item.reuseDelay = 10;
            item.useAnimation = 9;
            item.useStyle = 5;
            item.noMelee = true;
            item.knockBack = 5.5f;
            item.value = Item.buyPrice(1, 40, 0, 0);
            item.rare = 10;
            item.UseSound = SoundID.Item40;
            item.autoReuse = true;
            item.shootSpeed = 24f;
            item.shoot = ModContent.ProjectileType<BloodfireBullet>();
            item.useAmmo = 97;
            item.Calamity().postMoonLordRarity = 13;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(0, -5);
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<BloodstoneCore>(), 4);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Projectile.NewProjectile(position.X, position.Y, speedX, speedY, ModContent.ProjectileType<BloodfireBullet>(), damage, knockBack, player.whoAmI, 0.0f, 0.0f);
            return false;
        }
    }
}
