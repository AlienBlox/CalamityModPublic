using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Healing
{
    public class EarthHealOrb : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Healing";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            Projectile.width = 4;
            Projectile.height = 4;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 300;
            Projectile.extraUpdates = 3;
        }

        public override void AI()
        {
            Projectile.HealingProjectile((int)Projectile.ai[1], (int)Projectile.ai[0], 8f, 15f);
            int dusty = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 74, 0f, 0f, 100, default, 1f);
            Dust dust = Main.dust[dusty];
            dust.noGravity = true;
            dust.position.X -= Projectile.velocity.X * 0.2f;
            dust.position.Y += Projectile.velocity.Y * 0.2f;
        }
    }
}
