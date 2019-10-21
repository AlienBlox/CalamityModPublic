﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Typeless
{
    public class KelvinCatalystBoomerang : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Kelvin Catalyst");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 12;
            ProjectileID.Sets.TrailingMode[projectile.type] = 1;
        }

        public override void SetDefaults()
        {
            projectile.width = 30;
            projectile.height = 30;
            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.extraUpdates = 1;
            projectile.melee = true;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(projectile.localAI[0]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            projectile.localAI[0] = reader.ReadSingle();
        }

        public override void AI()
        {
            Lighting.AddLight(projectile.Center, Main.DiscoR * 0.3f / 255f, Main.DiscoR * 0.4f / 255f, Main.DiscoR * 0.5f / 255f);

            if (projectile.ai[1] == 1f)
            {
                projectile.melee = false;
                projectile.Calamity().rogue = true;
            }

            if (projectile.soundDelay == 0)
            {
                projectile.soundDelay = 8;
                Main.PlaySound(SoundID.Item7, projectile.position);
            }

            if (projectile.ai[0] == 0f)
            {
                projectile.localAI[0] += 1f;
                if (projectile.localAI[0] >= 75f)
                {
                    projectile.ai[0] = 1f;
                    projectile.localAI[0] = 0f;
                    projectile.width = projectile.height = 60;
                    projectile.tileCollide = false;
                    projectile.netUpdate = true;
                }
            }
            else
            {
                float num42 = 20f;
                float num43 = 2f;
                Vector2 vector2 = new Vector2(projectile.position.X + (float)projectile.width * 0.5f, projectile.position.Y + (float)projectile.height * 0.5f);
                float num44 = Main.player[projectile.owner].position.X + (float)(Main.player[projectile.owner].width / 2) - vector2.X;
                float num45 = Main.player[projectile.owner].position.Y + (float)(Main.player[projectile.owner].height / 2) - vector2.Y;
                float num46 = (float)Math.Sqrt((double)(num44 * num44 + num45 * num45));

                if (num46 > 3000f)
                    projectile.Kill();

                num46 = num42 / num46;
                num44 *= num46;
                num45 *= num46;

                if (projectile.velocity.X < num44)
                {
                    projectile.velocity.X = projectile.velocity.X + num43;
                    if (projectile.velocity.X < 0f && num44 > 0f)
                        projectile.velocity.X = projectile.velocity.X + num43;
                }
                else if (projectile.velocity.X > num44)
                {
                    projectile.velocity.X = projectile.velocity.X - num43;
                    if (projectile.velocity.X > 0f && num44 < 0f)
                        projectile.velocity.X = projectile.velocity.X - num43;
                }
                if (projectile.velocity.Y < num45)
                {
                    projectile.velocity.Y = projectile.velocity.Y + num43;
                    if (projectile.velocity.Y < 0f && num45 > 0f)
                        projectile.velocity.Y = projectile.velocity.Y + num43;
                }
                else if (projectile.velocity.Y > num45)
                {
                    projectile.velocity.Y = projectile.velocity.Y - num43;
                    if (projectile.velocity.Y > 0f && num45 < 0f)
                        projectile.velocity.Y = projectile.velocity.Y - num43;
                }
                if (Main.myPlayer == projectile.owner)
                {
                    Rectangle rectangle = new Rectangle((int)projectile.position.X, (int)projectile.position.Y, projectile.width, projectile.height);
                    Rectangle value2 = new Rectangle((int)Main.player[projectile.owner].position.X, (int)Main.player[projectile.owner].position.Y, Main.player[projectile.owner].width, Main.player[projectile.owner].height);
                    if (rectangle.Intersects(value2))
                        projectile.Kill();
                }
            }

            int dust = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 67, 0f, 0f, 100, default, 1f);
            Main.dust[dust].noGravity = true;
            Main.dust[dust].velocity *= 0f;

            projectile.rotation += 0.25f;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            projectile.ai[0] = 1f;
            projectile.localAI[0] = 0f;
            projectile.width = projectile.height = 60;
            projectile.tileCollide = false;
            projectile.netUpdate = true;
            if (projectile.velocity.X != oldVelocity.X)
            {
                projectile.velocity.X = -oldVelocity.X;
            }
            if (projectile.velocity.Y != oldVelocity.Y)
            {
                projectile.velocity.Y = -oldVelocity.Y;
            }
            return false;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.Frostburn, 240);
            if (projectile.owner == Main.myPlayer)
            {
                float spread = 45f * 0.0174f;
                double startAngle = Math.Atan2(projectile.velocity.X, projectile.velocity.Y) - spread / 2;
                double deltaAngle = spread / 8f;
                double offsetAngle;
                int i;
                for (i = 0; i < 4; i++)
                {
                    offsetAngle = startAngle + deltaAngle * (i + i * i) / 2f + 32f * i;

                    Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, (float)(Math.Sin(offsetAngle) * 4f), (float)(Math.Cos(offsetAngle) * 4f),
                        ModContent.ProjectileType<KelvinCatalystStar>(), projectile.damage / 6, projectile.knockBack * 0.5f, projectile.owner, 0f, 0f);

                    Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, (float)(-Math.Sin(offsetAngle) * 4f), (float)(-Math.Cos(offsetAngle) * 4f),
                        ModContent.ProjectileType<KelvinCatalystStar>(), projectile.damage / 6, projectile.knockBack * 0.5f, projectile.owner, 0f, 0f);
                }
            }
            Main.PlaySound(2, (int)projectile.position.X, (int)projectile.position.Y, 30);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityGlobalProjectile.DrawCenteredAndAfterimage(projectile, lightColor, ProjectileID.Sets.TrailingMode[projectile.type], 2);
            return false;
        }
    }
}
