using Microsoft.Xna.Framework;
using Terraria;
using CalamityMod.Projectiles;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Items.Weapons.Typeless
{
    public class LunicEye : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Lunic Eye");
            Tooltip.SetDefault("Fires lunic beams that reduce enemy protection\n" +
                "This weapon scales with all your damage stats at once");
        }

        public override void SetDefaults()
        {
            item.width = 80;
            item.damage = 9;
            item.rare = 5;
            item.useAnimation = 15;
            item.useTime = 15;
            item.useStyle = 5;
            item.knockBack = 4.5f;
            item.UseSound = mod.GetLegacySoundSlot(SoundType.Item, "Sounds/Item/LaserCannon");
            item.autoReuse = true;
            item.noMelee = true;
            item.height = 50;
            item.value = Item.buyPrice(0, 36, 0, 0);
            item.shoot = ModContent.ProjectileType<LunicBeam>();
            item.shootSpeed = 13f;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-15, 0);
        }

        // Lunic Eye scales off of all damage types simultaneously (meaning it scales 5x from universal damage boosts).
        public override void ModifyWeaponDamage(Player player, ref float add, ref float mult, ref float flat)
        {
            float formula = 5f * (player.allDamage - 1f);
            formula += player.meleeDamage - 1f;
            formula += player.rangedDamage - 1f;
            formula += player.magicDamage - 1f;
            formula += player.minionDamage - 1f;
            formula += player.Calamity().throwingDamage - 1f;
            add += formula;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Projectile.NewProjectile(position.X, position.Y, speedX, speedY, type, damage, knockBack, player.whoAmI, 0f, 0f);
            return false;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "Stardust", 20);
            recipe.AddIngredient(null, "AerialiteBar", 15);
            recipe.AddIngredient(ItemID.SunplateBlock, 15);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
