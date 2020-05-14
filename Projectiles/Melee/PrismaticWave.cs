using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Buffs.StatDebuffs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Melee
{
    public class PrismaticWave : ModProjectile
    {
        private int alpha = 50;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Wave");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[projectile.type] = 1;
        }

        public override void SetDefaults()
        {
            projectile.width = 36;
            projectile.height = 36;
            aiType = ProjectileID.FrostWave;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.ranged = true;
			projectile.melee = true;
            projectile.penetrate = 3;
            projectile.timeLeft = 200;
			projectile.tileCollide = false;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 10;
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
            if (projectile.localAI[0] == 0f)
            {
                projectile.scale -= 0.02f;
                projectile.alpha += 30;
                if (projectile.alpha >= 250)
                {
                    projectile.alpha = 255;
                    projectile.localAI[0] = 1f;
                }
            }
            else if (projectile.localAI[0] == 1f)
            {
                projectile.scale += 0.02f;
                projectile.alpha -= 30;
                if (projectile.alpha <= 0)
                {
                    projectile.alpha = 0;
                    projectile.localAI[0] = 0f;
                }
            }
            Lighting.AddLight(projectile.Center, Main.DiscoR * 0.5f / 255f, Main.DiscoG * 0.5f / 255f, Main.DiscoB * 0.5f / 255f);
            projectile.rotation = (float)Math.Atan2((double)projectile.velocity.Y, (double)projectile.velocity.X);
            if (Main.rand.NextBool(2))
            {
				Color color = Utils.SelectRandom(Main.rand, new Color[]
				{
					new Color(255, 0, 0, alpha), //Red
					new Color(255, 128, 0, alpha), //Orange
					new Color(255, 255, 0, alpha), //Yellow
					new Color(128, 255, 0, alpha), //Lime
					new Color(0, 255, 0, alpha), //Green
					new Color(0, 255, 128, alpha), //Turquoise
					new Color(0, 255, 255, alpha), //Cyan
					new Color(0, 128, 255, alpha), //Light Blue
					new Color(0, 0, 255, alpha), //Blue
					new Color(128, 0, 255, alpha), //Purple
					new Color(255, 0, 255, alpha), //Fuschia
					new Color(255, 0, 128, alpha) //Hot Pink
				});
                int rainbow = Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 267, projectile.velocity.X * 0.5f, projectile.velocity.Y * 0.5f, alpha, color);
				Main.dust[rainbow].noGravity = true;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
			if (projectile.timeLeft > 195)
				return false;

			CalamityGlobalProjectile.DrawCenteredAndAfterimage(projectile, lightColor, ProjectileID.Sets.TrailingMode[projectile.type], 2);
            return false;
        }

        public override Color? GetAlpha(Color lightColor) => new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB, alpha);

        public override void Kill(int timeLeft)
        {
            for (int k = 0; k < 3; k++)
            {
				Color color = Utils.SelectRandom(Main.rand, new Color[]
				{
					new Color(255, 0, 0, alpha), //Red
					new Color(255, 128, 0, alpha), //Orange
					new Color(255, 255, 0, alpha), //Yellow
					new Color(128, 255, 0, alpha), //Lime
					new Color(0, 255, 0, alpha), //Green
					new Color(0, 255, 128, alpha), //Turquoise
					new Color(0, 255, 255, alpha), //Cyan
					new Color(0, 128, 255, alpha), //Light Blue
					new Color(0, 0, 255, alpha), //Blue
					new Color(128, 0, 255, alpha), //Purple
					new Color(255, 0, 255, alpha), //Fuschia
					new Color(255, 0, 128, alpha) //Hot Pink
				});
                int rainbow = Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 267, projectile.oldVelocity.X * 0.5f, projectile.oldVelocity.Y * 0.5f, alpha, color);
				Main.dust[rainbow].noGravity = true;
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<BrimstoneFlames>(), 120);
            target.AddBuff(ModContent.BuffType<GlacialState>(), 120);
            target.AddBuff(ModContent.BuffType<Plague>(), 120);
            target.AddBuff(ModContent.BuffType<HolyFlames>(), 120);
            target.AddBuff(BuffID.CursedInferno, 120);
            target.AddBuff(BuffID.Frostburn, 120);
            target.AddBuff(BuffID.OnFire, 120);
            target.AddBuff(BuffID.Ichor, 120);
            projectile.velocity *= 0.75f;
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            target.AddBuff(ModContent.BuffType<BrimstoneFlames>(), 120);
            target.AddBuff(ModContent.BuffType<GlacialState>(), 120);
            target.AddBuff(ModContent.BuffType<Plague>(), 120);
            target.AddBuff(ModContent.BuffType<HolyFlames>(), 120);
            target.AddBuff(BuffID.CursedInferno, 120);
            target.AddBuff(BuffID.Frostburn, 120);
            target.AddBuff(BuffID.OnFire, 120);
            target.AddBuff(BuffID.Ichor, 120);
            projectile.velocity *= 0.75f;
        }
    }
}
