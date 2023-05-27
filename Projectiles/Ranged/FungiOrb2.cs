using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Ranged
{
    public class FungiOrb2 : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/Ranged/FungiOrb";

        public override void SetDefaults()
        {
            Projectile.width = 12;
            Projectile.height = 12;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = 1;
            Projectile.aiStyle = ProjAIStyleID.Arrow;
            Projectile.timeLeft = 180;
        }

        public override bool? CanHitNPC(NPC target) => Projectile.timeLeft < 150 && target.CanBeChasedBy(Projectile);

        public override void AI()
        {
            //Rotation
            Projectile.spriteDirection = Projectile.direction = (Projectile.velocity.X > 0).ToDirectionInt();
            Projectile.rotation = Projectile.velocity.ToRotation() + (Projectile.spriteDirection == 1 ? 0f : MathHelper.Pi) + MathHelper.ToRadians(90) * Projectile.direction;

            Lighting.AddLight(Projectile.Center, new Vector3(0, 244, 252) * (1.2f / 255));

            Projectile.velocity.Y += 0.1f;
            Projectile.velocity.X *= 0.95f;

            Projectile.localAI[0] += 1f;
            if (Projectile.localAI[0] > 4f)
            {
                Vector2 dspeed = -Projectile.velocity * 0.5f;
                int num469 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 56, 0f, 0f, 100, default, 1f);
                Main.dust[num469].noGravity = true;
                Main.dust[num469].velocity = dspeed;
            }

            if (Projectile.timeLeft < 150)
                CalamityUtils.HomeInOnNPC(Projectile, !Projectile.tileCollide, 450f, 6f, 20f);
        }

        public override void Kill(int timeLeft)
        {
            Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, 56, Projectile.oldVelocity.X * 0.5f, Projectile.oldVelocity.Y * 0.5f);
        }
    }
}
