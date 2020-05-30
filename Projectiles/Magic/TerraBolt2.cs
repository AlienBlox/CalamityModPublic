using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Magic
{
    public class TerraBolt2 : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Bolt");
        }

        public override void SetDefaults()
        {
            projectile.width = 4;
            projectile.height = 4;
            projectile.extraUpdates = 100;
            projectile.friendly = true;
            projectile.timeLeft = 15;
            projectile.magic = true;
        }

		public override void AI()
		{
			Vector2 vector33 = projectile.position;
			vector33 -= projectile.velocity * 0.25f;
			int num448 = Dust.NewDust(vector33, 1, 1, 107, 0f, 0f, 0, default, 1.25f);
			Main.dust[num448].position = vector33;
			Main.dust[num448].scale = (float)Main.rand.Next(70, 110) * 0.013f;
			Main.dust[num448].velocity *= 0.1f;
		}

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.immune[projectile.owner] = 8;
        }
    }
}
