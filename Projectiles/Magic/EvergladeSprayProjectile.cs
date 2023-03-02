using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Magic
{
    public class EvergladeSprayProjectile : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Spray");
        }

        public override void SetDefaults()
        {
            Projectile.width = 4;
            Projectile.height = 4;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = 6;
            Projectile.MaxUpdates = 3;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 6 * Projectile.MaxUpdates; // 6 effective, 18 total
        }

        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, (255 - Projectile.alpha) * 0.01f / 255f, (255 - Projectile.alpha) * 0.15f / 255f, (255 - Projectile.alpha) * 0.05f / 255f);
            Projectile.scale -= 0.002f;
            if (Projectile.scale <= 0f)
            {
                Projectile.Kill();
            }
            if (Projectile.ai[0] <= 3f)
            {
                Projectile.ai[0] += 1f;
                return;
            }
            Projectile.velocity.Y = Projectile.velocity.Y + 0.075f;
            for (int num151 = 0; num151 < 3; num151++)
            {
                float num152 = Projectile.velocity.X / 3f * (float)num151;
                float num153 = Projectile.velocity.Y / 3f * (float)num151;
                int num155 = Dust.NewDust(Projectile.Center, Projectile.width, Projectile.height, 157, 0f, 0f, 100, default, 1f);
                Dust dust = Main.dust[num155];
                dust.noGravity = true;
                dust.velocity *= 0.1f;
                dust.velocity += Projectile.velocity * 0.5f;
                dust.position.X -= num152;
                dust.position.Y -= num153;
            }
            if (Main.rand.NextBool(8))
            {
                int num157 = Dust.NewDust(Projectile.Center, Projectile.width, Projectile.height, 157, 0f, 0f, 100, default, 0.5f);
                Main.dust[num157].velocity *= 0.25f;
                Main.dust[num157].velocity += Projectile.velocity * 0.5f;
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.Ichor, 600);
            target.AddBuff(BuffID.CursedInferno, 300);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(lightColor), Projectile.rotation, tex.Size() / 2f, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }
    }
}
