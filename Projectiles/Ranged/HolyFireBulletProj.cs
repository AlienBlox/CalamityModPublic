﻿using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Projectiles.Typeless;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Ranged
{
    public class HolyFireBulletProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Holy Fire Bullet");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 12;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            projectile.width = 12;
            projectile.height = 12;
            projectile.friendly = true;
            projectile.extraUpdates = 5;
            projectile.penetrate = 1;
            projectile.timeLeft = 600;
            projectile.ranged = true;
        }

        public override void AI()
        {
            projectile.rotation = projectile.velocity.ToRotation() + MathHelper.ToRadians(90f);
            projectile.spriteDirection = projectile.direction;

            // Flaking dust
            if (Main.rand.NextBool())
            {
                float scale = Main.rand.NextFloat(0.6f, 1.6f);
                int dustID = Dust.NewDust(projectile.Center, 1, 1, 244);
                Main.dust[dustID].position = projectile.Center;
                Main.dust[dustID].noGravity = true;
                Main.dust[dustID].scale = scale;
                float angleDeviation = 0.17f;
                float angle = Main.rand.NextFloat(-angleDeviation, angleDeviation);
                Vector2 sprayVelocity = projectile.velocity.RotatedBy(angle) * 0.6f;
                Main.dust[dustID].velocity = sprayVelocity;
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(1f, 1f, 1f, 0f);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityGlobalProjectile.DrawCenteredAndAfterimage(projectile, lightColor, ProjectileID.Sets.TrailingMode[projectile.type], 1);
            return false;
        }

        public override void Kill(int timeLeft)
        {
            if (projectile.owner == Main.myPlayer)
            {
                int blastDamage = (int)(projectile.damage * 0.85f);
                float scale = 0.85f + Main.rand.NextFloat() * 1.15f;
                int boom = Projectile.NewProjectile(projectile.Center, Vector2.Zero, ModContent.ProjectileType<FuckYou>(), blastDamage, projectile.knockBack, projectile.owner, 0f, scale);
                Main.projectile[boom].Calamity().forceRanged = true;
            }
            for (int k = 0; k < 4; k++)
            {
                float scale = Main.rand.NextFloat(1.4f, 1.8f);
                int dustID = Dust.NewDust(projectile.position, projectile.width, projectile.height, 244);
                Main.dust[dustID].noGravity = false;
                Main.dust[dustID].scale = scale;
                float angleDeviation = 0.25f;
                float angle = Main.rand.NextFloat(-angleDeviation, angleDeviation);
                float velMult = Main.rand.NextFloat(0.08f, 0.14f);
                Vector2 shrapnelVelocity = projectile.oldVelocity.RotatedBy(angle) * velMult;
                Main.dust[dustID].velocity = shrapnelVelocity;
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<HolyFlames>(), 300);
        }
    }
}
