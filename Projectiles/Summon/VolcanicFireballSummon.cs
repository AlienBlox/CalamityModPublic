﻿using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon
{
    public class VolcanicFireballSummon : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/Melee/VolcanicFireball";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Volcanic Fireball");
            Main.projFrames[Projectile.type] = 4;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = 12;
            Projectile.height = 12;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 300;
            Projectile.penetrate = 1;
            Projectile.minionSlots = 0f;
            Projectile.minion = true;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            if (Projectile.frameCounter++ % 4 == 0)
            {
                Projectile.frame++;
            }
            if (Projectile.frame >= Main.projFrames[Projectile.type])
            {
                Projectile.frame = 0;
            }

            Lighting.AddLight(Projectile.Center, (255 - Projectile.alpha) * 0.5f / 255f, (255 - Projectile.alpha) * 0.25f / 255f, (255 - Projectile.alpha) * 0f / 255f);
            if (Projectile.localAI[0] == 0f)
            {
                Projectile.scale = 1f - Main.rand.NextFloat() * 0.5f;
                Projectile.localAI[0] += 1f;
            }
            if (Main.rand.NextBool(4))
            {
                int flame = Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, Main.rand.NextBool(3) ? 16 : 174, 0f, 0f);
                Main.dust[flame].noGravity = true;
                Main.dust[flame].velocity *= 0f;
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.OnFire, 180);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (Projectile.timeLeft == 300)
                return false;

            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 2);
            return false;
        }
    }
}
