using Microsoft.Xna.Framework;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Magic
{
    public class MelterNote1 : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Song");
        }

        public override void SetDefaults()
        {
            projectile.width = 24;
            projectile.height = 24;
            projectile.friendly = true;
            projectile.magic = true;
            projectile.penetrate = 10;
            projectile.timeLeft = 600;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 5;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
        }

        public override void AI()
        {
            projectile.velocity.X *= 0.985f;
            projectile.velocity.Y *= 0.985f;
            if (projectile.localAI[0] == 0f)
            {
                projectile.scale += 0.02f;
                if (projectile.scale >= 1.25f)
                {
                    projectile.localAI[0] = 1f;
                }
            }
            else if (projectile.localAI[0] == 1f)
            {
                projectile.scale -= 0.02f;
                if (projectile.scale <= 0.75f)
                {
                    projectile.localAI[0] = 0f;
                }
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255, 255, 255);
        }
    }
}
