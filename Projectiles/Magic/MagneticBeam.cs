using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Magic
{
    public class MagneticBeam : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Beam");
        }

        public override void SetDefaults()
        {
            Projectile.width = 4;
            Projectile.height = 4;
            Projectile.extraUpdates = 100;
            Projectile.friendly = true;
            Projectile.timeLeft = 90;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.ignoreWater = true;
        }

        public override void AI()
        {
            Vector2 vector33 = Projectile.position;
            vector33 -= Projectile.velocity * 0.25f;
            int num448 = Dust.NewDust(vector33, 1, 1, 173, 0f, 0f, 0, default, 1.25f);
            Main.dust[num448].position = vector33;
            Main.dust[num448].scale = (float)Main.rand.Next(70, 110) * 0.013f;
            Main.dust[num448].velocity *= 0.1f;
        }
    }
}
