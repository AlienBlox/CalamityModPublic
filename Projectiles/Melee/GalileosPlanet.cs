using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Graphics.Shaders;
using CalamityMod.Buffs.DamageOverTime;

namespace CalamityMod.Projectiles.Melee
{
    public class GalileosPlanet : ModProjectile
    {
		private int radius = 47;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Planet");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            projectile.width = 42;
            projectile.height = 42;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.extraUpdates = 1;
            projectile.penetrate = 1;
        }

        public override void AI()
        {
            projectile.rotation = (float)Math.Atan2((double)projectile.velocity.Y, (double)projectile.velocity.X) + 1.57f;
            for (float i = -5; i <= 0; i++)
            {
                if (Main.rand.NextFloat(1f) < 0.4f)
                {
                    // You need to set position depending on what you are doing. You may need to subtract width/2 and height/2 as well to center the spawn rectangle.
                    Vector2 pos = projectile.position + new Vector2((projectile.width / 2) - 8, projectile.height + 16);
                    Dust dust1 = Dust.NewDustPerfect(pos + new Vector2(-20 + i, -2), 180, new Vector2(i * 0.7f, -1.1f));
                    dust1.noGravity = true;
                }
            }
            for (float i = 0; i <= 5; i++)
            {
                if (Main.rand.NextFloat(1f) < 0.4f)
                {
                    // You need to set position depending on what you are doing. You may need to subtract width/2 and height/2 as well to center the spawn rectangle.
                    Vector2 pos = projectile.position + new Vector2((projectile.width / 2) - 8, projectile.height + 16);
                    Dust dust1 = Dust.NewDustPerfect(pos + new Vector2(20 + i, -2), 180, new Vector2(i * 0.7f, -1.1f));
                    dust1.noGravity = true;
                }
            }

            Dust dust;
            // You need to set position depending on what you are doing. You may need to subtract width/2 and height/2 as well to center the spawn rectangle.
            Vector2 pos2 = projectile.Center - new Vector2(20, 20);
            dust = Main.dust[Dust.NewDust(pos2, 40, 40, 180, 0f, 0f, 0, new Color(255, 255, 255), 1f)];

            Lighting.AddLight(projectile.Center, 0.3f, 0.3f, 0.7f);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityGlobalProjectile.DrawCenteredAndAfterimage(projectile, lightColor, ProjectileID.Sets.TrailingMode[projectile.type], 1);
            return false;
        }

        public override void Kill(int timeLeft)
        {
            projectile.position = projectile.Center;
            projectile.width = projectile.height = 32;
            projectile.position.X = projectile.position.X - (float)(projectile.width / 2);
            projectile.position.Y = projectile.position.Y - (float)(projectile.height / 2);
            radius = 100;
            projectile.maxPenetrate = -1;
            projectile.penetrate = -1;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 10;
            projectile.damage /= 2;
            projectile.Damage();
            for (int i = 0; i < 72; i++)
            {
                Dust dust;
                // You need to set position depending on what you are doing. You may need to subtract width/2 and height/2 as well to center the spawn rectangle.
                Vector2 position = projectile.Center - new Vector2(30, 30);
                dust = Main.dust[Terraria.Dust.NewDust(position, 60, 60, 187, Main.rand.Next(-5, 5), Main.rand.Next(-5, 5), 0, new Color(0, 167, 255), 5f)];
                dust.noGravity = true;
                dust.shader = GameShaders.Armor.GetSecondaryShader(47, Main.LocalPlayer);
            }

            Main.PlaySound(4, (int)projectile.position.X, (int)projectile.position.Y, 43);

            for (int i = 0; i < 155; i++)
            {
                Dust dust;
                // You need to set position depending on what you are doing. You may need to subtract width/2 and height/2 as well to center the spawn rectangle.
                Vector2 position = projectile.position;
                dust = Main.dust[Dust.NewDust(position, 94, 94, 180, Main.rand.Next(-5, 5), Main.rand.Next(-5, 5), 0, new Color(255, 255, 255), Main.rand.NextFloat(1.2f, 2.2f))];
                dust.noGravity = true;
            }

            for (int x = 1; x <= 3; x++)
            {
                int num20 = 36;
                for (int i = 0; i < num20; i++)
                {
                    Vector2 spinningpoint = Vector2.Normalize(projectile.velocity) * new Vector2((float)projectile.width / 2f, (float)projectile.height) * 0.75f * 0.5f;
                    spinningpoint = spinningpoint.RotatedBy((double)((float)(i - (num20 / 2 - 1)) * 6.28318548f / (float)num20), default(Vector2)) + projectile.Center;
                    Vector2 vector = spinningpoint - projectile.Center;
                    int num21 = Dust.NewDust(spinningpoint + vector, 0, 0, 180, vector.X * 2f, vector.Y * 2f, 0, new Color(255, 255, 255), 1f);
                    Main.dust[num21].noGravity = true;
                    Main.dust[num21].velocity = Vector2.Normalize(vector) * x;
                }
            }

            for (int x = 1; x <= 3; x++)
            {
                int num20 = 36;
                for (int i = 0; i < num20; i++)
                {
                    Vector2 spinningpoint = Vector2.Normalize(projectile.velocity) * new Vector2((float)projectile.width / 2f, (float)projectile.height) * 0.75f * 0.5f;
                    spinningpoint = spinningpoint.RotatedBy((double)((float)(i - (num20 / 2 - 1)) * 6.28318548f / (float)num20), default(Vector2)) + projectile.Center;
                    Vector2 vector = spinningpoint - projectile.Center;
                    int num21 = Dust.NewDust(spinningpoint + vector, 0, 0, 180, vector.X * 2f, vector.Y * 2f, 0, new Color(255, 255, 255), 1f);
                    Main.dust[num21].noGravity = true;
                    Main.dust[num21].velocity = Vector2.Normalize(vector) * x;
                    Main.dust[num21].velocity *= 2;
                }
            }
            for (int num625 = 0; num625 < 3; num625++)
            {
                float scaleFactor10 = 0.33f;
                if (num625 == 1)
                {
                    scaleFactor10 = 0.66f;
                }
                if (num625 == 2)
                {
                    scaleFactor10 = 1f;
                }
                int num626 = Gore.NewGore(new Vector2(projectile.position.X + (float)(projectile.width / 2) - 24f, projectile.position.Y + (float)(projectile.height / 2) - 24f), default, Main.rand.Next(61, 64), 1f);
                Gore gore = Main.gore[num626];
                gore.velocity *= scaleFactor10;
                gore.velocity.X += 1f;
                gore.velocity.Y += 1f;
                num626 = Gore.NewGore(new Vector2(projectile.position.X + (float)(projectile.width / 2) - 24f, projectile.position.Y + (float)(projectile.height / 2) - 24f), default, Main.rand.Next(61, 64), 1f);
                gore.velocity *= scaleFactor10;
                gore.velocity.X -= 1f;
                gore.velocity.Y += 1f;
                num626 = Gore.NewGore(new Vector2(projectile.position.X + (float)(projectile.width / 2) - 24f, projectile.position.Y + (float)(projectile.height / 2) - 24f), default, Main.rand.Next(61, 64), 1f);
                gore.velocity *= scaleFactor10;
                gore.velocity.X += 1f;
                gore.velocity.Y -= 1f;
                num626 = Gore.NewGore(new Vector2(projectile.position.X + (float)(projectile.width / 2) - 24f, projectile.position.Y + (float)(projectile.height / 2) - 24f), default, Main.rand.Next(61, 64), 1f);
                gore.velocity *= scaleFactor10;
                gore.velocity.X -= 1f;
                gore.velocity.Y -= 1f;
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<Nightwither>(), 300);
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float dist1 = Vector2.Distance(projectile.Center, targetHitbox.TopLeft());
            float dist2 = Vector2.Distance(projectile.Center, targetHitbox.TopRight());
            float dist3 = Vector2.Distance(projectile.Center, targetHitbox.BottomLeft());
            float dist4 = Vector2.Distance(projectile.Center, targetHitbox.BottomRight());

            float minDist = dist1;
            if (dist2 < minDist)
                minDist = dist2;
            if (dist3 < minDist)
                minDist = dist3;
            if (dist4 < minDist)
                minDist = dist4;

            return minDist <= radius;
        }
    }
}
