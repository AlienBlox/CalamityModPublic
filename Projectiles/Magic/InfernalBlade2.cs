﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
namespace CalamityMod.Projectiles.Magic
{
    public class InfernalBlade2 : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Blade");
        }

        public override void SetDefaults()
        {
            projectile.width = 10;
            projectile.height = 10;
            projectile.friendly = true;
            projectile.alpha = 255;
            projectile.penetrate = 2;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.timeLeft = 180;
            projectile.magic = true;
        }

        public override void AI()
        {
            DelegateMethods.v3_1 = new Vector3(0.6f, 1f, 1f) * 0.2f;
            Utils.PlotTileLine(projectile.Center, projectile.Center + projectile.velocity * 10f, 8f, new Utils.PerLinePoint(DelegateMethods.CastLightOpen));
            if (projectile.alpha > 0)
            {
                Main.PlaySound(2, projectile.Center, 9);
                projectile.alpha = 0;
                projectile.scale = 1.1f;
                projectile.frame = Main.rand.Next(14);
                float num98 = 16f;
                int num99 = 0;
                while ((float)num99 < num98)
                {
                    Vector2 vector11 = Vector2.UnitX * 0f;
                    vector11 += -Vector2.UnitY.RotatedBy((double)((float)num99 * (6.28318548f / num98)), default) * new Vector2(1f, 4f);
                    vector11 = vector11.RotatedBy((double)projectile.velocity.ToRotation(), default);
                    int num100 = Dust.NewDust(projectile.Center, 0, 0, 182, 0f, 0f, 0, default, 1f);
                    Main.dust[num100].scale = 1.5f;
                    Main.dust[num100].noGravity = true;
                    Main.dust[num100].position = projectile.Center + vector11;
                    Main.dust[num100].velocity = projectile.velocity * 0f + vector11.SafeNormalize(Vector2.UnitY) * 1f;
                    num99++;
                }
            }
            projectile.rotation = projectile.velocity.ToRotation() + 0.7853982f;
        }

        public override void Kill(int timeLeft)
        {
            Collision.HitTiles(projectile.position, projectile.velocity, projectile.width, projectile.height);
            Main.PlaySound(2, (int)projectile.position.X, (int)projectile.position.Y, 10);
            int num487 = Main.rand.Next(4, 10);
            for (int num488 = 0; num488 < num487; num488++)
            {
                int num489 = Dust.NewDust(projectile.Center, 0, 0, 182, 0f, 0f, 100, default, 1f);
                Main.dust[num489].velocity *= 1.6f;
                Dust expr_FEDF_cp_0 = Main.dust[num489];
                expr_FEDF_cp_0.velocity.Y -= 1f;
                Main.dust[num489].velocity += -projectile.velocity * (Main.rand.NextFloat() * 2f - 1f) * 0.5f;
                Main.dust[num489].scale = 2f;
                Main.dust[num489].fadeIn = 0.5f;
                Main.dust[num489].noGravity = true;
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255, 50, 50, 0);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (projectile.spriteDirection == -1)
            {
                spriteEffects = SpriteEffects.FlipHorizontally;
            }
            Color color25 = Lighting.GetColor((int)((double)projectile.position.X + (double)projectile.width * 0.5) / 16, (int)(((double)projectile.position.Y + (double)projectile.height * 0.5) / 16.0));
            Texture2D texture2D3 = Main.projectileTexture[projectile.type];
            int num155 = Main.projectileTexture[projectile.type].Height / Main.projFrames[projectile.type];
            int y3 = num155 * projectile.frame;
            Rectangle rectangle = new Rectangle(0, y3, texture2D3.Width, num155);
            Vector2 origin2 = rectangle.Size() / 2f;
            float num158 = 0f;
            int num156 = 3;
            int num157 = 1;
            float value4 = 8f;
            rectangle = new Rectangle(38 * projectile.frame, 0, 38, 38);
            origin2 = rectangle.Size() / 2f;
            for (int num159 = 1; num159 < num156; num159 += num157)
            {
                Color color26 = color25;
                color26 = projectile.GetAlpha(color26);
                color26 *= (float)(num156 - num159) / ((float)ProjectileID.Sets.TrailCacheLength[projectile.type] * 1.5f);
                Vector2 value5 = projectile.oldPos[num159];
                float num160 = projectile.rotation;
                SpriteEffects effects = spriteEffects;
                if (ProjectileID.Sets.TrailingMode[projectile.type] == 2)
                {
                    num160 = projectile.oldRot[num159];
                    effects = (projectile.oldSpriteDirection[num159] == -1) ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
                }
                Main.spriteBatch.Draw(texture2D3, value5 + projectile.Size / 2f - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color26, num160 + projectile.rotation * num158 * (float)(num159 - 1) * (float)-(float)spriteEffects.HasFlag(SpriteEffects.FlipHorizontally).ToDirectionInt(), origin2, MathHelper.Lerp(projectile.scale, value4, (float)num159 / 15f), effects, 0f);
            }
            Color color28 = projectile.GetAlpha(color25);
            Main.spriteBatch.Draw(texture2D3, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color28, projectile.rotation, origin2, projectile.scale, spriteEffects, 0f);
            return false;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<BrimstoneFlames>(), 240);
        }
    }
}
