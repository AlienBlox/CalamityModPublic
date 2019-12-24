using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Ranged
{
    public class SicknessRound2 : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Round");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 3;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            projectile.width = 8;
            projectile.height = 8;
            projectile.aiStyle = 1;
            projectile.friendly = true;
            projectile.ranged = true;
            projectile.penetrate = 1;
            projectile.timeLeft = 180;
            projectile.light = 0.15f;
            projectile.extraUpdates = 1;
            aiType = ProjectileID.WoodenArrowFriendly;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityGlobalProjectile.DrawCenteredAndAfterimage(projectile, lightColor, ProjectileID.Sets.TrailingMode[projectile.type], 1);
            return false;
        }

        public override bool PreAI()
        {
            Vector2 dspeed = -projectile.velocity * 0.5f;
            float x2 = projectile.Center.X - projectile.velocity.X / 10f;
            float y2 = projectile.Center.Y - projectile.velocity.Y / 10f;
            int num137 = Dust.NewDust(new Vector2(x2, y2), 1, 1, 107, 0f, 0f, 0, default, 1f);
            Main.dust[num137].alpha = projectile.alpha;
            Main.dust[num137].position.X = x2;
            Main.dust[num137].position.Y = y2;
            Main.dust[num137].velocity = dspeed;
            Main.dust[num137].noGravity = true;
            
            float num138 = (float)Math.Sqrt((double)(projectile.velocity.X * projectile.velocity.X + projectile.velocity.Y * projectile.velocity.Y));
            float num139 = projectile.localAI[0];
            if (num139 == 0f)
            {
                projectile.localAI[0] = num138;
            }
            //Rotation
            projectile.spriteDirection = projectile.direction = (projectile.velocity.X > 0).ToDirectionInt();
            projectile.rotation = (projectile.velocity.ToRotation() + (projectile.spriteDirection == 1 ? 0f : MathHelper.Pi)) + MathHelper.ToRadians(90) * projectile.direction;

            return false;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.Poisoned, 180);
            target.AddBuff(BuffID.Venom, 180);
            target.AddBuff(ModContent.BuffType<Plague>(), 180);
        }

        public override void Kill(int timeLeft)
        {
            if (projectile.owner == Main.myPlayer)
            {
                Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, 0f, 0f, ModContent.ProjectileType<Sickness>(), projectile.damage, projectile.knockBack, projectile.owner, 0f, 0f);
            }
        }
    }
}
