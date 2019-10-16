﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria; using CalamityMod.Projectiles; using Terraria.ModLoader;
using Terraria.ID;
using Terraria.ModLoader; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles; using CalamityMod.Tiles; using CalamityMod.Walls;

namespace CalamityMod.Projectiles
{
    public class IceCluster : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ice");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            projectile.width = 90;
            projectile.height = 90;
            projectile.friendly = true;
            projectile.alpha = 255;
            projectile.penetrate = -1;
            projectile.timeLeft = 100;
            projectile.tileCollide = false;
            projectile.magic = true;
        }

        public override void AI()
        {
            projectile.rotation += 0.5f;
            if (projectile.localAI[1] == 0f)
            {
                projectile.localAI[1] = 1f;
                Main.PlaySound(2, (int)projectile.position.X, (int)projectile.position.Y, 120);
            }
            projectile.ai[0] += 1f;
            if (projectile.ai[1] == 1f)
            {
                if (projectile.ai[0] >= 130f)
                {
                    projectile.alpha += 10;
                }
                else
                {
                    projectile.alpha -= 10;
                }
                if (projectile.alpha < 0)
                {
                    projectile.alpha = 0;
                }
                if (projectile.alpha > 255)
                {
                    projectile.alpha = 255;
                }
                if (projectile.ai[0] >= 150f)
                {
                    return;
                }
                if (projectile.ai[0] % 30f == 0f && Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Vector2 vector80 = projectile.rotation.ToRotationVector2();
                    Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, vector80.X, vector80.Y, ModContent.ProjectileType<IceCluster>(), projectile.damage, projectile.knockBack, projectile.owner, 0f, 0f);
                }
                Lighting.AddLight(projectile.Center, 0.3f, 0.75f, 0.9f);
            }
            else
            {
                if (projectile.ai[0] >= 40f)
                {
                    projectile.alpha += 3;
                }
                else
                {
                    projectile.alpha -= 40;
                }
                if (projectile.alpha < 0)
                {
                    projectile.alpha = 0;
                }
                if (projectile.alpha > 255)
                {
                    projectile.alpha = 255;
                }
                if (projectile.ai[0] >= 45f)
                {
                    return;
                }
                Vector2 value47 = new Vector2(0f, -720f).RotatedBy((double)projectile.velocity.ToRotation(), default);
                float scaleFactor8 = projectile.ai[0] % 45f / 45f;
                Vector2 spinningpoint = value47 * scaleFactor8;
                for (int num844 = 0; num844 < 6; num844++)
                {
                    Vector2 vector81 = projectile.Center + spinningpoint.RotatedBy((double)((float)num844 * 6.28318548f / 6f), default);
                    Lighting.AddLight(vector81, 0.3f, 0.75f, 0.9f);
                    for (int num845 = 0; num845 < 2; num845++)
                    {
                        int num846 = Dust.NewDust(vector81 + Utils.RandomVector2(Main.rand, -8f, 8f) / 2f, 8, 8, 197, 0f, 0f, 100, Color.Transparent, 1f);
                        Main.dust[num846].noGravity = true;
                    }
                }
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityGlobalProjectile.DrawCenteredAndAfterimage(projectile, lightColor, ProjectileID.Sets.TrailingMode[projectile.type], 1);
            return false;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.immune[projectile.owner] = 5;
            target.AddBuff(ModContent.BuffType<GlacialState>(), 120);
            Vector2 vector80 = projectile.rotation.ToRotationVector2();
            if (projectile.owner == Main.myPlayer)
            {
                Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, vector80.X, vector80.Y, ModContent.ProjectileType<IceCluster>(), projectile.damage, projectile.knockBack, projectile.owner, 0f, 0f);
            }
        }
    }
}
