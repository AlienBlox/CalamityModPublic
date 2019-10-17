﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria; using CalamityMod.Projectiles; using Terraria.ModLoader; using CalamityMod.Dusts;
using Terraria.ID;
using Terraria.ModLoader; using CalamityMod.Dusts; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles; using CalamityMod.Tiles; using CalamityMod.Walls;

namespace CalamityMod.Projectiles
{
    public class CorpusAvertor : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Corpus Avertor");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            projectile.width = 24;
            projectile.height = 24;
            projectile.friendly = true;
            projectile.penetrate = 1;
            projectile.timeLeft = 300;
            projectile.melee = true;
        }

        public override void AI()
        {
            if (projectile.ai[1] == 1f)
            {
                projectile.melee = false;
                projectile.Calamity().rogue = true;
            }

            projectile.rotation += (Math.Abs(projectile.velocity.X) + Math.Abs(projectile.velocity.Y)) * 0.02f;

            if (projectile.ai[0] < 120f)
                projectile.ai[0] += 1f;

            if (projectile.ai[0] < 61f)
            {
                if (projectile.ai[0] % 20f == 0f)
                {
                    Vector2 velocity = new Vector2(projectile.velocity.X, projectile.velocity.Y);
                    float mult = projectile.ai[0] / 80f; // Ranges from 0.25 to 0.5 to 0.75
                    velocity *= mult;

                    Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, velocity.X, velocity.Y, ModContent.ProjectileType<CorpusAvertorClone>(),
                        (int)((float)projectile.damage * mult), projectile.knockBack * mult, projectile.owner, projectile.ai[0], projectile.melee ? 0f : 1f);
                }
            }
            else
            {
                projectile.velocity.X *= 1.01f;
                projectile.velocity.Y *= 1.01f;

                int scale = (int)((projectile.ai[0] - 60f) * 4.25f);
                int dust = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 5, 0f, 0f, 100, new Color(scale, 0, 0, 50), 2f);
                Main.dust[dust].velocity *= 0f;
                Main.dust[dust].noGravity = true;
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
            if (projectile.ai[0] >= 61f)
            {
                int scale = (int)((projectile.ai[0] - 60f) * 4.25f);
                return new Color(scale, 0, 0, 50);
            }
            return new Color(0, 0, 0, 50);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityGlobalProjectile.DrawCenteredAndAfterimage(projectile, lightColor, ProjectileID.Sets.TrailingMode[projectile.type], 1);
            return false;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            if (target.type == NPCID.TargetDummy)
                return;

            float heal = (float)damage * 0.05f;
            if ((int)heal == 0)
                return;
            if (Main.player[Main.myPlayer].lifeSteal <= 0f)
                return;

            Main.player[Main.myPlayer].lifeSteal -= heal * 1.5f;
            int owner = projectile.owner;
            Projectile.NewProjectile(target.position.X, target.position.Y, 0f, 0f, ProjectileID.VampireHeal, 0, 0f, projectile.owner, (float)owner, heal);
        }
    }
}
