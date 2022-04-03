using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Magic
{
    public class TacticiansTrumpCardProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Beam");
        }

        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.aiStyle = 27;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = 3;
            Projectile.timeLeft = 300;
            aiType = ProjectileID.LightBeam;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
            Projectile.extraUpdates = 1;
        }

        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, (255 - Projectile.alpha) * 0.37f / 255f, (255 - Projectile.alpha) * 0.5f / 255f, (255 - Projectile.alpha) * 0.47f / 255f);
            if (Main.rand.NextBool(5))
            {
                Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, 132, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f);
            }
            Projectile.rotation = Projectile.velocity.ToRotation();
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (Projectile.timeLeft > 295)
                return false;

            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
            spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(lightColor), Projectile.rotation, tex.Size() / 2f, Projectile.scale, SpriteEffects.None, 0f);
            return false;
        }

        public override void Kill(int timeLeft)
        {
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, 132, Projectile.oldVelocity.X * 0.5f, Projectile.oldVelocity.Y * 0.5f);
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            Projectile.NewProjectile(Projectile.Center, Vector2.Zero, ModContent.ProjectileType<TacticiansElectricBoom>(), Projectile.damage / 2, 0f, Projectile.owner);
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            Projectile.NewProjectile(Projectile.Center, Vector2.Zero, ModContent.ProjectileType<TacticiansElectricBoom>(), Projectile.damage / 2, 0f, Projectile.owner);
        }
    }
}
