using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ID;

namespace CalamityMod.Projectiles.Rogue
{
	public class PrismShurikenBlade : ModProjectile
    {
    	public override void SetStaticDefaults() => DisplayName.SetDefault("Blade");

		public override void SetDefaults()
        {
            projectile.width = projectile.height = 22;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.Calamity().rogue = true;
			projectile.extraUpdates = 1;
            projectile.timeLeft = 360;
        }

        public override void AI()
		{
			CalamityGlobalProjectile.HomeInOnNPC(projectile, false, 850f, 14.5f, 35f);
			projectile.rotation = projectile.velocity.ToRotation() + MathHelper.PiOver2;
		}

		public override void Kill(int timeLeft)
		{
			if (Main.dedServ)
				return;

			for (int i = 0; i < 25; i++)
			{
				Vector2 circularOffsetDirection = (MathHelper.TwoPi * i / 25f).ToRotationVector2().RotatedBy(projectile.velocity.ToRotation() - MathHelper.PiOver2);
				Vector2 spawnPosition = projectile.Center - (projectile.rotation - MathHelper.PiOver2).ToRotationVector2() * projectile.height * 1.25f;
				spawnPosition += circularOffsetDirection * new Vector2(12f, 7f);

				Dust energy = Dust.NewDustPerfect(spawnPosition, 261);
				energy.velocity = circularOffsetDirection * new Vector2(3f, 2f);
				energy.color = Color.Cyan;
				energy.scale = 0.7f;
				energy.noGravity = true;
			}
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			CalamityGlobalProjectile.DrawCenteredAndAfterimage(projectile, lightColor, ProjectileID.Sets.TrailingMode[projectile.type], 1);
			return false;
        }
    }
}
