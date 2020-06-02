using CalamityMod.Projectiles.Typeless;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Melee.Yoyos
{
	public class CnidarianYoyo : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cnidarian");
            ProjectileID.Sets.YoyosLifeTimeMultiplier[projectile.type] = 7f;
            ProjectileID.Sets.YoyosMaximumRange[projectile.type] = 190f;
            ProjectileID.Sets.YoyosTopSpeed[projectile.type] = 11f;
        }

        public override void SetDefaults()
        {
            projectile.aiStyle = 99;
            projectile.width = 16;
            projectile.height = 16;
            projectile.scale = 1.15f;
            projectile.friendly = true;
            projectile.melee = true;
            projectile.penetrate = -1;
        }

        public override void AI()
        {
			CalamityGlobalProjectile.MagnetSphereHitscan(projectile, 300f, 6f, 60f, 5, ModContent.ProjectileType<Seashell>(), 0.5);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D tex = Main.projectileTexture[projectile.type];
            spriteBatch.Draw(tex, projectile.Center - Main.screenPosition, null, projectile.GetAlpha(lightColor), projectile.rotation, tex.Size() / 2f, projectile.scale, SpriteEffects.None, 0f);
            return false;
        }
    }
}
