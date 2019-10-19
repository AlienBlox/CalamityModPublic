using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class Karasawa : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Karasawa");
            Tooltip.SetDefault("...This is heavy...too heavy.");
        }

        public override void SetDefaults()
        {
            item.width = 94;
            item.height = 44;
            item.ranged = true;
            item.damage = 1400;
            item.knockBack = 12f;
            item.useTime = 52;
            item.useAnimation = 52;
            item.autoReuse = true;

            item.useStyle = 5;
            item.UseSound = mod.GetLegacySoundSlot(SoundType.Item, "Sounds/Item/MechGaussRifle");
            item.noMelee = true;

            item.value = Item.buyPrice(1, 80, 0, 0);
            item.rare = 10;
            item.Calamity().postMoonLordRarity = 21;

            item.shoot = ModContent.ProjectileType<KarasawaShot>();
            item.shootSpeed = 1f;
            item.useAmmo = AmmoID.Bullet;
        }

        public override bool CanUseItem(Player player)
        {
            return CalamityGlobalItem.HasEnoughAmmo(player, item, 5);
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Vector2 velocity = new Vector2(speedX, speedY);
            if (velocity.Length() > 5f)
            {
                velocity.Normalize();
                velocity *= 5f;
            }
            Projectile.NewProjectile(position.X, position.Y, velocity.X, velocity.Y, ModContent.ProjectileType<KarasawaShot>(), damage, knockBack, player.whoAmI, 0f, 0f);

            // Consume 5 ammo per shot
            CalamityGlobalItem.ConsumeAdditionalAmmo(player, item, 5);

            return false;
        }

        // Disable vanilla ammo consumption
        public override bool ConsumeAmmo(Player player)
        {
            return false;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-20, 0);
        }

        public override void AddRecipes()
        {
            ModRecipe r = new ModRecipe(mod);
            r.AddIngredient(ModContent.ItemType<CrownJewel>());
            r.AddIngredient(ModContent.ItemType<GalacticaSingularity>(), 5);
            r.AddIngredient(ModContent.ItemType<BarofLife>(), 10);
            r.AddIngredient(ModContent.ItemType<CosmiliteBar>(), 15);
            r.AddTile(ModContent.TileType<DraedonsForge>());
            r.SetResult(this);
            r.AddRecipe();
        }
    }
}
