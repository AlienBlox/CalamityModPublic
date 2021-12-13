using CalamityMod.Projectiles.BaseProjectiles;
using Microsoft.Xna.Framework;
using Terraria;

namespace CalamityMod.Projectiles.Boss
{
	public class DoGDeathBoom : BaseMassiveExplosionProjectile
	{
		public override int Lifetime => 180;
		public override bool UsesScreenshake => true;
		public override float GetScreenshakePower(float pulseCompletionRatio) => CalamityUtils.Convert01To010(pulseCompletionRatio) * 28f;
		public override Color GetCurrentExplosionColor(float pulseCompletionRatio)
		{
			return Color.Lerp(Color.Cyan, Color.Fuchsia, MathHelper.Clamp(pulseCompletionRatio * 1.75f, 0f, 1f));
		}
		public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

		public override void SetStaticDefaults() => DisplayName.SetDefault("Cosmic Explosion");

		public override void SetDefaults()
		{
			projectile.width = projectile.height = 2;
			projectile.ignoreWater = true;
			projectile.tileCollide = false;
			projectile.penetrate = -1;
			projectile.timeLeft = Lifetime;
			cooldownSlot = 1;
		}

		public override void PostAI()
        {
			MaxRadius = 4200f;
			Lighting.AddLight(projectile.Center, 0.1f, 0.1f, 0.1f);
		}
	}
}
