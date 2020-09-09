using CalamityMod.Projectiles.Ranged;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class ProporsePistol : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Proporse Pistol");
            Tooltip.SetDefault("Fires a blue energy blast that bounces");
        }

        public override void SetDefaults()
        {
            item.damage = 90;
            item.ranged = true;
            item.width = 36;
            item.height = 20;
            item.useTime = 25;
            item.useAnimation = 25;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 3.5f;
            item.value = Item.buyPrice(0, 36, 0, 0);
            item.rare = 5;
            item.UseSound = SoundID.Item33;
            item.autoReuse = true;
            item.shootSpeed = 20f;
            item.shoot = ModContent.ProjectileType<ProBolt>();
            item.useAmmo = AmmoID.Bullet;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Projectile.NewProjectile(position.X, position.Y, speedX, speedY, ModContent.ProjectileType<ProBolt>(), damage, knockBack, player.whoAmI, 0f, 0f);
            return false;
        }
    }
}
