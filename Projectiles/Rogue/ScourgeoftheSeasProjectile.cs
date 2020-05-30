using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Buffs.DamageOverTime;

namespace CalamityMod.Projectiles.Rogue
{
    public class ScourgeoftheSeasProjectile : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Moist Scourge");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            projectile.width = 26;
            projectile.height = 26;
            projectile.friendly = true;
            projectile.aiStyle = 113;
            aiType = ProjectileID.BoneJavelin;
            projectile.penetrate = 1;
            projectile.extraUpdates = 1;
            projectile.timeLeft = 1200;
            projectile.Calamity().rogue = true;
        }

        public override void AI()
        {
            if (Main.rand.NextBool(5))
            {
                Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 85, projectile.velocity.X * 0.5f, projectile.velocity.Y * 0.5f);
            }
            projectile.rotation = (float)Math.Atan2((double)projectile.velocity.Y, (double)projectile.velocity.X) + 0.785f;
            projectile.velocity.X *= 1.015f;
            projectile.velocity.Y *= 1.015f;
			projectile.velocity.X = Math.Min(16f, projectile.velocity.X);
			projectile.velocity.Y = Math.Min(16f, projectile.velocity.Y);
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.Venom, 600);
            if (projectile.Calamity().stealthStrike) //stealth strike attack
            {
				target.AddBuff(ModContent.BuffType<SulphuricPoisoning>(), 600);
			}
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            target.AddBuff(BuffID.Venom, 600);
            if (projectile.Calamity().stealthStrike) //stealth strike attack
            {
				target.AddBuff(ModContent.BuffType<SulphuricPoisoning>(), 600);
			}
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.Item14, projectile.position);
            for (int dustIndex = 0; dustIndex < 8; dustIndex++)
            {
                int dusty = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 85, 0f, 0f, 100, default, 1f);
                Main.dust[dusty].velocity *= 1f;
            }
            if (projectile.owner == Main.myPlayer)
            {
				int cloudNumber = Main.rand.Next(2, 6);
				for (int cloudIndex = 0; cloudIndex < cloudNumber; cloudIndex++)
				{
					Vector2 velocity = new Vector2((float)Main.rand.Next(-100, 101), (float)Main.rand.Next(-100, 101));
					velocity.Normalize();
					velocity *= (float)Main.rand.Next(10, 201) * 0.01f;
					Projectile.NewProjectile(projectile.Center, velocity, ModContent.ProjectileType<ScourgeVenomCloud>(), (int)(projectile.damage * 0.25), 1f, projectile.owner, 0f, projectile.Calamity().stealthStrike ? 1f : 0f);
				}
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityGlobalProjectile.DrawCenteredAndAfterimage(projectile, lightColor, ProjectileID.Sets.TrailingMode[projectile.type], 1);
            return false;
        }
    }
}
