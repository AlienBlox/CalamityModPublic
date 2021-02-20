using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Ranged
{
    public class CardSpade : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Spade");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 5;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            projectile.width = 14;
            projectile.height = 14;
            projectile.friendly = true;
            projectile.alpha = 255;
            projectile.penetrate = -1;
            projectile.timeLeft = 600;
            projectile.ranged = true;
            projectile.extraUpdates = 1;
            projectile.aiStyle = 1;
            aiType = ProjectileID.Bullet;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 8;
        }

        public override void AI()
        {
            Lighting.AddLight(projectile.Center, (255 - projectile.alpha) * 0.5f / 255f, (255 - projectile.alpha) * 0f / 255f, (255 - projectile.alpha) * 0f / 255f);
			projectile.rotation -= (MathHelper.ToRadians(90) * projectile.direction);
			projectile.spriteDirection = projectile.direction;
            if (Main.rand.NextBool(2))
            {
                int num137 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), 1, 1, 30, 0f, 0f, 0, default, 0.5f);
                Main.dust[num137].velocity *= 0f;
                Main.dust[num137].noGravity = true;
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Collision.HitTiles(projectile.position, projectile.velocity, projectile.width, projectile.height);
            Main.PlaySound(SoundID.Dig, (int)projectile.position.X, (int)projectile.position.Y, 1, 1f, 0f);
            return true;
        }

        public override void Kill(int timeLeft)
        {
            for (int k = 0; k < 2; k++)
            {
                Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 30, projectile.oldVelocity.X * 0.15f, projectile.oldVelocity.Y * 0.15f);
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<BrimstoneFlames>(), 360);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityGlobalProjectile.DrawCenteredAndAfterimage(projectile, lightColor, ProjectileID.Sets.TrailingMode[projectile.type], 1);
            return false;
        }
    }
}
