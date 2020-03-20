﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Buffs.StatDebuffs;

namespace CalamityMod.Projectiles.Boss
{
    public class OldDukeSummonDrop : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Acid");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 5;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            projectile.width = 14;
            projectile.height = 20;
            projectile.hostile = true;
            projectile.tileCollide = true;
            projectile.ignoreWater = false;
            projectile.timeLeft = 400;
        }
        public override void AI()
        {
            projectile.rotation = projectile.velocity.ToRotation() - MathHelper.PiOver2;
            if (projectile.velocity.Y <= 8f)
            {
                projectile.velocity.Y += 0.15f;
            }
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            // Water drip
            for (int i = 0; i < 4; i++)
            {
                int idx = Dust.NewDust(projectile.position - projectile.velocity, 2, 2, 154, 0f, 0f, 0, new Color(112, 150, 42, 127), 1f);
                Dust dust = Main.dust[idx];
                dust.position.X -= 2f;
                Main.dust[idx].alpha = 38;
                Main.dust[idx].velocity *= 0.1f;
                Main.dust[idx].velocity -= projectile.velocity * 0.025f;
                Main.dust[idx].scale = 0.75f;
            }
            return true;
        }

		public override void OnHitPlayer(Player target, int damage, bool crit)
        {
			if (Main.rand.NextBool(2))
			{
				// 1 to 3 seconds of poisoned
				target.AddBuff(BuffID.Poisoned, 60 * Main.rand.Next(1, 4));
			}
			else if (Main.rand.NextBool(4))
			{
				// 1 to 2 second of Sulphuric Poisoning
				target.AddBuff(ModContent.BuffType<SulphuricPoisoning>(), 60 * Main.rand.Next(1, 3));
			}
			else
			{
				// 3 to 5 seconds of Irradiated
				target.AddBuff(ModContent.BuffType<Irradiated>(), 60 * Main.rand.Next(3, 6));
			}
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityGlobalProjectile.DrawCenteredAndAfterimage(projectile, new Color(255, 255, 255, 127), ProjectileID.Sets.TrailingMode[projectile.type], 2);
            return false;
        }
    }
}
