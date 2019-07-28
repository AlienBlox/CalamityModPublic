using System;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Magic
{
    public class ApothMark : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Jaws of Annihilation");
		}

		public override void SetDefaults()
		{
			projectile.width = 100;
            projectile.height = 100;
            projectile.alpha = 70;
			projectile.timeLeft = 240;
			projectile.penetrate = -1;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 1;
			projectile.friendly = true;
			projectile.magic = true;
			projectile.tileCollide = false;
			projectile.ignoreWater = true;
			projectile.light = 1.5f;
		}

		public override void AI()
        {
            if (projectile.timeLeft < 30)
                projectile.alpha = projectile.alpha + 6;
            else if (projectile.timeLeft < 210)
            {
                projectile.velocity.X *= 0.9f;
                projectile.velocity.Y *= 0.9f;
            }
            else if (projectile.timeLeft == 240)
            {
                projectile.rotation = (float)Math.Atan2((double)projectile.velocity.Y, (double)projectile.velocity.X);
                double offsetHyp = Math.Sqrt(48*48+38*38);
                double offsetRotation = Math.Atan2(-38, 48) + (double)projectile.rotation;
                double offsetX = offsetHyp * Math.Cos(offsetRotation);
                double offsetY = offsetHyp * Math.Sin(offsetRotation);
                Projectile.NewProjectile(projectile.Center.X + (float)offsetX, projectile.Center.Y + (float)offsetY, projectile.velocity.X, projectile.velocity.Y, mod.ProjectileType("ApothJaws"), projectile.damage, projectile.knockBack, Main.myPlayer, projectile.rotation, 0f);
                offsetRotation = Math.Atan2(38, 48) + (double)projectile.rotation;
                offsetX = offsetHyp * Math.Cos(offsetRotation);
                offsetY = offsetHyp * Math.Sin(offsetRotation);
                Projectile.NewProjectile(projectile.position.X + projectile.width/2 + (float)offsetX, projectile.position.Y + projectile.height/2 + (float)offsetY, projectile.velocity.X, projectile.velocity.Y, mod.ProjectileType("ApothJaws"), projectile.damage, projectile.knockBack, Main.myPlayer, projectile.rotation, 1f);
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(mod.BuffType("GodSlayerInferno"), 600, true);
            target.AddBuff(mod.BuffType("DemonFlames"), 600, true);
            target.AddBuff(mod.BuffType("ArmorCrunch"), 600, true);
            if (Main.rand.Next(30) == 0)
            {
                target.AddBuff(mod.BuffType("ExoFreeze"), 120, true);
            }
        }
	}
}
