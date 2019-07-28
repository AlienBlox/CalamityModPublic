using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Permafrost
{
    public class IcicleTrident : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Icicle Trident");
			Tooltip.SetDefault("Shoots piercing icicles");
            Item.staff[item.type] = true;
		}
		public override void SetDefaults()
		{
			item.damage = 69;
			item.magic = true;
            item.mana = 21;
			item.width = 64;
			item.height = 64;
			item.useTime = 25;
            item.useAnimation = 25;
			item.useStyle = 5;
			item.useTurn = false;
			item.noMelee = true;
			item.knockBack = 7f;
            item.value = Item.buyPrice(0, 36, 0, 0);
            item.rare = 5;
			item.UseSound = SoundID.Item8;
			item.autoReuse = true;
			item.shoot = mod.ProjectileType("TridentIcicle");
            item.shootSpeed = 12f;
		}
        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Vector2 speed = new Vector2(speedX, speedY);
            Projectile.NewProjectile(position, speed, type, damage, knockBack, player.whoAmI);
            Projectile.NewProjectile(position, speed.RotatedBy(MathHelper.ToRadians(5)), type, damage, knockBack, player.whoAmI);
            Projectile.NewProjectile(position, speed.RotatedBy(MathHelper.ToRadians(-5)), type, damage, knockBack, player.whoAmI);
            return false;
        }
    }
}
