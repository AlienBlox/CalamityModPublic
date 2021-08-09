using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Boss
{
    public class AresGaussNukeProjectileSpark : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Gauss Spark");
            Main.projFrames[projectile.type] = 4;
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            projectile.width = 14;
            projectile.height = 14;
            projectile.hostile = true;
            projectile.ignoreWater = true;
			projectile.tileCollide = false;
			projectile.Opacity = 0f;
			cooldownSlot = 1;
			projectile.penetrate = -1;
            projectile.timeLeft = 600;
			projectile.Calamity().affectedByMaliceModeVelocityMultiplier = true;
		}

		public override void SendExtraAI(BinaryWriter writer)
		{
			writer.Write(projectile.localAI[0]);
		}

		public override void ReceiveExtraAI(BinaryReader reader)
		{
			projectile.localAI[0] = reader.ReadSingle();
		}

		public override void AI()
        {
			projectile.localAI[0] += 1f;
			if (projectile.localAI[0] >= 15f)
			{
				projectile.localAI[0] = 15f;
				projectile.velocity.Y += 0.1f;
			}

			if (projectile.velocity.Y > 16f)
				projectile.velocity.Y = 16f;

			if (projectile.timeLeft < 15)
				projectile.Opacity = MathHelper.Clamp(projectile.timeLeft / 15f, 0f, 1f);
			else
				projectile.Opacity = MathHelper.Clamp(1f - ((projectile.timeLeft - 597) / 3f), 0f, 1f);

			Lighting.AddLight(projectile.Center, 0.1f * projectile.Opacity, 0.125f * projectile.Opacity, 0.025f * projectile.Opacity);

			projectile.frameCounter++;
            if (projectile.frameCounter > 4)
            {
                projectile.frame++;
                projectile.frameCounter = 0;
            }
            if (projectile.frame > 3)
                projectile.frame = 0;

            projectile.rotation = (float)Math.Atan2(projectile.velocity.Y, projectile.velocity.X) - MathHelper.PiOver2;
        }

		public override bool CanHitPlayer(Player target) => projectile.Opacity == 1f;

		public override void OnHitPlayer(Player target, int damage, bool crit)
		{
			if (projectile.Opacity != 1f)
				return;

			target.AddBuff(BuffID.OnFire, 180);
		}

		public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255 * projectile.Opacity, 255 * projectile.Opacity, 255 * projectile.Opacity, projectile.alpha);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(projectile, ProjectileID.Sets.TrailingMode[projectile.type], lightColor, 1);
            return false;
        }

		public override void ModifyHitPlayer(Player target, ref int damage, ref bool crit)
		{
			target.Calamity().lastProjectileHit = projectile;
		}
	}
}
