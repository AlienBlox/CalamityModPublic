using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Rogue
{
	public class CelestialReaperProjectile : ModProjectile
    {
        public int HomingCooldown = 0;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Celestial Reaper");
        }

        public override void SetDefaults()
        {
            projectile.width = 66;
            projectile.height = 76;
            projectile.friendly = true;
            projectile.penetrate = 6;
            projectile.tileCollide = false;
            projectile.Calamity().rogue = true;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 5;
        }
        public override void AI()
        {
            projectile.rotation += MathHelper.ToRadians(30f) / (float)Math.Log(6f - projectile.penetrate + 2f) / 1.4f; //slow down the more it's hit
            //log(1) is 0. Dividing by 0 would be bad, so add by 2 instead of 1
            if (HomingCooldown > 0)
            {
                HomingCooldown--;
            }
            else
            {
                NPC target = projectile.position.ClosestNPCAt(640f);
                if (target != null)
                {
                    projectile.velocity = (projectile.velocity * 20f + projectile.DirectionTo(target.Center) * 20f) / 21f;
                }
            }
            if (projectile.ai[0] == 1f)
            {
                //Damaging afterimages
                float framesNeeded = 60f;
                framesNeeded /= 6f - projectile.penetrate + 1f; //1 is to prevent division by zero. The more hits, the more afterimages
                if (projectile.timeLeft % (int)framesNeeded == 0f)
                {
                    Projectile.NewProjectile(projectile.Center, projectile.velocity, ModContent.ProjectileType<CelestialReaperAfterimage>(), projectile.damage, 2f, projectile.owner);
                }
            }
        }

        public override bool? CanHitNPC(NPC target)
		{
			if (HomingCooldown > 0)
			{
				return false;
			}
			return null;
		}

        public override bool CanHitPvp(Player target)
		{
			return HomingCooldown <= 0;
		}

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            HomingCooldown = 25;
            projectile.velocity *= -0.75f; //bounce off of enemy
        }
        public override void Kill(int timeLeft)
        {
            for (float i = 0f; i < 7f; i += 1f)
            {
                float angle = MathHelper.TwoPi * i / 7f;
                Projectile.NewProjectile(projectile.Center, angle.ToRotationVector2() * 12f, ModContent.ProjectileType<CelestialReaperAfterimage>(), projectile.damage, 2f, projectile.owner);
            }
        }
    }
}
