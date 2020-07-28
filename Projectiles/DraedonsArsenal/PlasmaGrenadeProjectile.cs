using CalamityMod.Dusts;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.DraedonsArsenal
{
    public class PlasmaGrenadeProjectile : ModProjectile
    {
        public float Time
        {
            get => projectile.ai[0];
            set => projectile.ai[0] = value;
        }
        public const float FallAcceleration = 0.15f;
        public const float MaxFallSpeed = 12f;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Web Ball");
        }

        public override void SetDefaults()
        {
            projectile.width = 22;
            projectile.height = 28;
            projectile.friendly = true;
            projectile.penetrate = 1;
            projectile.timeLeft = 180;
            projectile.Calamity().rogue = true;
        }

        public override void AI()
        {
            projectile.rotation = projectile.velocity.ToRotation() + MathHelper.PiOver2;
            Vector2 projectileTop = projectile.Center + new Vector2(0f, projectile.height * -0.5f).RotatedBy(projectile.rotation);
            if (projectile.velocity.Y < MaxFallSpeed && Time > 30f)
            {
                projectile.velocity.Y += 0.5f;
            }
            if (!Main.dedServ)
            {
                Dust dust = Dust.NewDustPerfect(projectileTop, 107);
                dust.velocity = projectile.rotation.ToRotationVector2().RotatedByRandom(0.35f) * Main.rand.NextFloat(2f, 4f);
                dust.velocity += projectile.velocity;
                dust.scale = Main.rand.NextFloat(0.95f, 1.3f);
            }
            Time++;
        }

        public override void Kill(int timeLeft)
        {
            if (projectile.Calamity().stealthStrike)
            {
                if (Main.myPlayer == projectile.owner)
                {
                    Projectile.NewProjectile(projectile.Center, Vector2.Zero, ModContent.ProjectileType<MassivePlasmaExplosion>(), projectile.damage * 2, projectile.knockBack * 2f, projectile.owner);
                }
                if (!Main.dedServ)
                {
                    for (int i = 0; i < 220; i++)
                    {
                        int type = Main.rand.NextBool(2) ? 261 : 107;
                        Dust dust = Dust.NewDustPerfect(projectile.Center + Main.rand.NextVector2Circular(10f, 10f), type);
                        dust.scale = Main.rand.NextFloat(1.6f, 2.2f);
                        dust.velocity = Main.rand.NextVector2CircularEdge(75f, 75f);
                        dust.noGravity = true;
                        if (type == 261)
                        {
                            dust.velocity *= 1.5f;
                        }
                    }
                }
            }
            else
            {
                CalamityGlobalProjectile.ExpandHitboxBy(projectile, 360);
				projectile.usesLocalNPCImmunity = true;
				projectile.localNPCHitCooldown = 10;
                projectile.Damage();
                if (!Main.dedServ)
                {
                    for (int i = 0; i < 120; i++)
                    {
                        int type = Main.rand.NextBool(3) ? 261 : (int)CalamityDusts.SulfurousSeaAcid;
                        Dust dust = Dust.NewDustPerfect(projectile.Center + Main.rand.NextVector2Circular(10f, 10f), type);
                        dust.scale = Main.rand.NextFloat(1.3f, 1.5f);
                        dust.velocity = Main.rand.NextVector2CircularEdge(15f, 15f);
                        dust.noGravity = true;
                        if (type == 261)
                        {
                            dust.velocity *= 2f;
                            dust.scale *= 1.8f;
                        }
                    }
                }
            }
        }
    }
}

