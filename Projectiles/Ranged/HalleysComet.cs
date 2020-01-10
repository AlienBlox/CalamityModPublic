using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Ranged
{
    public class HalleysComet : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Halley's Comet");
        }

        public override void SetDefaults()
        {
            projectile.width = 12;
            projectile.height = 12;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.ranged = true;
            projectile.penetrate = 10;
            projectile.extraUpdates = 10;
            projectile.timeLeft = 600;
        }

        public override void AI()
        {
            if (projectile.scale <= 1.5f)
            {
                projectile.scale *= 1.01f;
            }
            Lighting.AddLight(projectile.Center, (255 - projectile.alpha) * 0.35f / 255f, (255 - projectile.alpha) * 0f / 255f, (255 - projectile.alpha) * 0.45f / 255f);
            if (projectile.timeLeft > 600)
            {
                projectile.timeLeft = 600;
            }
            if (projectile.ai[0] > 5f)
            {
                float num296 = 1f;
                if (projectile.ai[0] == 6f)
                {
                    num296 = 0.25f;
                }
                else if (projectile.ai[0] == 7f)
                {
                    num296 = 0.5f;
                }
                else if (projectile.ai[0] == 8f)
                {
                    num296 = 0.75f;
                }
                projectile.ai[0] += 1f;
                int num297 = 176;
                for (int num298 = 0; num298 < 3; num298++)
                {
                    int num299 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, num297, projectile.velocity.X * 0.2f, projectile.velocity.Y * 0.2f, 1, default, 1f);
					Dust dust = Main.dust[num299];
					if (Main.rand.NextBool(3))
					{
						dust.noGravity = true;
						dust.scale *= 1.75f;
						dust.velocity.X *= 2f;
						dust.velocity.Y *= 2f;
					}
					else
					{
						dust.scale *= 0.5f;
					}
					dust.velocity.X *= 1.2f;
					dust.velocity.Y *= 1.2f;
					dust.scale *= num296;
					dust.velocity += projectile.velocity;
					if (!dust.noGravity)
					{
						dust.velocity *= 0.5f;
					}
                }
            }
            else
            {
                projectile.ai[0] += 1f;
            }
            projectile.rotation += 0.3f * (float)projectile.direction;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<Nightwither>(), 300);
        }
    }
}
