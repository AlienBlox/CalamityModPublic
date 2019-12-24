using CalamityMod.Projectiles.Ranged;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class DaemonsFlame : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Daemon's Flame");
            Tooltip.SetDefault("Shoots daemon flame fireballs as well as regular arrows");
        }

        public override void SetDefaults()
        {
            item.damage = 160;
            item.width = 62;
            item.height = 128;
            item.useTime = 12;
            item.useAnimation = 12;
            item.useStyle = 5;
            item.knockBack = 4f;
            item.value = Item.buyPrice(1, 40, 0, 0);
            item.rare = 10;
            item.UseSound = SoundID.Item5;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.ranged = true;
            item.channel = true;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<DaemonsFlameBow>();
            item.shootSpeed = 20f;
            item.useAmmo = 40;
            item.Calamity().postMoonLordRarity = 13;
        }

		public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
		{
			Vector2 origin = new Vector2(31f, 62f);
			spriteBatch.Draw(ModContent.GetTexture("CalamityMod/Items/Weapons/Ranged/DaemonsFlameGlow"), item.Center - Main.screenPosition, null, Color.White, rotation, origin, 1f, SpriteEffects.None, 0f);
		}

		public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Projectile.NewProjectile(position.X, position.Y, speedX, speedY, ModContent.ProjectileType<DaemonsFlameBow>(), damage, knockBack, player.whoAmI, 0f, 0f);
            return false;
        }
    }
}
