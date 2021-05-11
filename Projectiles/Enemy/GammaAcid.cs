using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Enemy
{
    public class GammaAcid : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/Enemy/FlakAcid";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Acid");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 2;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            projectile.width = 30;
            projectile.height = 50;
            projectile.hostile = true;
            projectile.timeLeft = 480;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
        }
        public override void AI()
        {
            if (projectile.ai[0]++ <= 30f)
            {
                projectile.alpha = (int)MathHelper.Lerp(255f, 127f, projectile.ai[0] / 30f);
            }
            projectile.rotation = projectile.velocity.ToRotation() + MathHelper.PiOver2;
        }
        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.AddBuff(ModContent.BuffType<Irradiated>(), 180);
        }
        public override void Kill(int timeLeft)
        {
			CalamityGlobalProjectile.ExpandHitboxBy(projectile, 80);
            projectile.Damage();
            for (int i = 0; i <= 40; i++)
            {
                int idx = Dust.NewDust(projectile.position, 100, 100, (int)CalamityDusts.SulfurousSeaAcid, 0, 0, 0, default, 0.75f);
                Main.dust[idx].noGravity = true;
                Main.dust[idx].velocity = Vector2.One.RotatedByRandom(MathHelper.TwoPi) * (Main.dust[idx].position - projectile.Center).Length() / 30f;
                Main.dust[idx].scale = 2.5f;
            }
            for (int i = 0; i <= 90; i++)
            {
                int idx = Dust.NewDust(projectile.Center, 0, 0, (int)CalamityDusts.SulfurousSeaAcid);
                Main.dust[idx].velocity = Vector2.One.RotatedByRandom(MathHelper.TwoPi) * 8f;
                Main.dust[idx].scale = 3f;
                Main.dust[idx].noGravity = true;
            }
        }

        public override Color? GetAlpha(Color lightColor) => new Color(200, 200, 200, 200);

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(projectile, ProjectileID.Sets.TrailingMode[projectile.type], lightColor, 1);
            return false;
        }
    }
}
