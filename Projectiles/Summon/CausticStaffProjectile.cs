using CalamityMod.Buffs.StatDebuffs;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Summon
{
    public class CausticStaffProjectile : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Thorn");
            ProjectileID.Sets.MinionShot[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.width = 10;
            projectile.height = 10;
            projectile.friendly = true;
            projectile.minion = true;
            projectile.minionSlots = 0f;
            projectile.ignoreWater = true;
            projectile.penetrate = 3;
            projectile.timeLeft = 360;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 10;
			projectile.extraUpdates = 1;
			projectile.tileCollide = false;
        }
        public override void AI()
        {
            int fire = Dust.NewDust(projectile.position, projectile.width, projectile.height, DustID.Fire, 0f, 0f, 0, default, 0.5f);
            Dust dust = Main.dust[fire];
            dust.velocity *= 0.1f;
            dust.scale = 1.3f;
            dust.noGravity = true;

            projectile.rotation = projectile.velocity.ToRotation() + MathHelper.PiOver2;
            projectile.velocity.X *= 0.99f;
            if (projectile.velocity.Y < 9f)
                projectile.velocity.Y += 0.085f;
        }
        public override void Kill(int timeLeft)
        {
            for (int i = 0; i < 5; i++)
            {
                Dust dust = Dust.NewDustPerfect(projectile.Center, DustID.Fire);
                dust.noGravity = true;
                dust.velocity = Vector2.One.RotatedByRandom(MathHelper.TwoPi) * Main.rand.NextFloat(2);
            }
        }
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
			Player player = Main.player[projectile.owner];
			if ((player.ActiveItem().summon &&
				!player.ActiveItem().melee &&
				!player.ActiveItem().ranged &&
				!player.ActiveItem().magic &&
				!player.ActiveItem().Calamity().rogue) ||
				player.ActiveItem().hammer > 0 ||
				player.ActiveItem().pick > 0 ||
				player.ActiveItem().axe > 0)
			{
				int duration = Main.rand.Next(60, 181); // Anywhere between 1 and 3 seconds
				switch ((int)projectile.ai[0])
				{
					case 0:
						target.AddBuff(ModContent.BuffType<MarkedforDeath>(), duration);
						break;
					case 1:
						target.AddBuff(BuffID.Ichor, duration);
						break;
					case 2:
						target.AddBuff(BuffID.Venom, duration);
						break;
					case 3:
						target.AddBuff(BuffID.CursedInferno, duration);
						break;
					case 4:
						target.AddBuff(BuffID.OnFire, duration);
						break;
					default:
						break;
				}
			}
        }
    }
}
