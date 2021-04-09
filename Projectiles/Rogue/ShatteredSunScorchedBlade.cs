using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public class ShatteredSunScorchedBlade : ModProjectile
    {
        int counter = 0;
        bool stealthOrigin = false;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Scorched Blade");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            projectile.width = 56;
            projectile.height = 56;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.extraUpdates = 1;
            projectile.penetrate = 1;
            projectile.Calamity().rogue = true;
            projectile.tileCollide = false;
            projectile.timeLeft = 500;
        }

        public override void AI()
        {
            counter++;
            if (counter == 1)
            {
                stealthOrigin = projectile.ai[0] == 1f;
                projectile.alpha += (int) projectile.ai[1];
                projectile.ai[0] = 0f;
            }
            if (counter == 20 && !projectile.Calamity().stealthStrike && !stealthOrigin)
            {
                projectile.tileCollide = true;
            }
            if (counter % 5 == 0)
            {
                projectile.velocity *= 1.15f;
            }
            if (counter % 10 == 0)
            {
                if (!stealthOrigin && projectile.alpha < 200)
                    projectile.alpha += 6;
            }
            if (counter % 9 == 0 || (counter % 5 == 0 && projectile.Calamity().stealthStrike))
            {
                int timesToSpawnDust = projectile.Calamity().stealthStrike  ? 2 : 1;
                for (int i = 0; i < timesToSpawnDust; i++)
                {
                    int num624 = Dust.NewDust(projectile.position, projectile.width, projectile.height, 127, 0f, 0f, 100, default, projectile.Calamity().stealthStrike ? 1.8f : 1.3f);
                    Main.dust[num624].noGravity = true;
                    Main.dust[num624].velocity *= 5f;
                    num624 = Dust.NewDust(projectile.position, projectile.width, projectile.height, 127, 0f, 0f, 100, default, projectile.Calamity().stealthStrike ? 1.8f : 1.3f);
                    Main.dust[num624].velocity *= 2f;
                }
            }

            projectile.rotation = (float)Math.Atan2((double)projectile.velocity.Y, (double)projectile.velocity.X) + 2.355f;
            if (projectile.spriteDirection == -1)
            {
                projectile.rotation -= 1.57f;
            }

            Lighting.AddLight(projectile.Center, 0.7f, 0.3f, 0f);
			CalamityGlobalProjectile.HomeInOnNPC(projectile, true, 200f, 12f, 20f);
            float num633;
            Vector2 vector46 = projectile.position;
            bool flag25 = false;
            for (int num645 = 0; num645 < 200; num645++)
            {
                NPC nPC2 = Main.npc[num645];
                if (nPC2.CanBeChasedBy(projectile, false))
                {
                    float num646 = Vector2.Distance(nPC2.Center, projectile.Center);
                    if (!flag25)
                    {
                        num633 = num646;
                        vector46 = nPC2.Center;
                        flag25 = true;
                    }
                }
            }
            if (flag25 && projectile.ai[0] == 0f)
            {
                Vector2 vector47 = vector46 - projectile.Center;
                float num648 = vector47.Length();
                vector47.Normalize();
                if (num648 > 200f)
                {
                    float scaleFactor2 = 8f;
                    vector47 *= scaleFactor2;
                    projectile.velocity = (projectile.velocity * 40f + vector47) / 41f;
                }
                else
                {
                    float num649 = 4f;
                    vector47 *= -num649;
                    projectile.velocity = (projectile.velocity * 40f + vector47) / 41f;
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
            if (projectile.Calamity().stealthStrike)
            {
                int numProj = 2;
                if (projectile.owner == Main.myPlayer)
                {
                    Player owner = Main.player[projectile.owner];
                    Vector2 correctedVelocity = target.Center - owner.Center;
                    correctedVelocity.Normalize();
                    correctedVelocity *= 10f;
                    int spread = 6;
                    for (int i = 0; i < numProj; i++)
                    {
                        Vector2 perturbedspeed = new Vector2(correctedVelocity.X, correctedVelocity.Y + Main.rand.Next(-3, 4)).RotatedBy(MathHelper.ToRadians(spread));
                        int projDamage = (int)(projectile.damage * 0.6f);
                        float kb = 1f;
                        int proj = Projectile.NewProjectile(owner.Center.X, owner.Center.Y - 10, perturbedspeed.X, perturbedspeed.Y, projectile.type, projDamage, kb, projectile.owner, 1f, projectile.alpha);
                        spread -= Main.rand.Next(2, 6);
                        Main.projectile[proj].ai[0] = 1f;
                    }
                    projectile.Kill();
                }
            }
            target.AddBuff(ModContent.BuffType<HolyFlames>(), 180);
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            if (projectile.Calamity().stealthStrike)
            {
                int numProj = 2;
                if (projectile.owner == Main.myPlayer)
                {
                    Player owner = Main.player[projectile.owner];
                    Vector2 correctedVelocity = target.Center - owner.Center;
                    correctedVelocity.Normalize();
                    correctedVelocity *= 10f;
                    int spread = 6;
                    for (int i = 0; i < numProj; i++)
                    {
                        Vector2 perturbedspeed = new Vector2(correctedVelocity.X, correctedVelocity.Y + Main.rand.Next(-3, 4)).RotatedBy(MathHelper.ToRadians(spread));
                        int projDamage = (int)(projectile.damage * 0.6f);
                        float kb = 1f;
                        int proj = Projectile.NewProjectile(owner.Center.X, owner.Center.Y - 10, perturbedspeed.X, perturbedspeed.Y, projectile.type, projDamage, kb, projectile.owner, 1f, projectile.alpha);
                        spread -= Main.rand.Next(2, 6);
                        Main.projectile[proj].ai[0] = 1f;
                    }
                    projectile.Kill();
                }
            }
            target.AddBuff(ModContent.BuffType<HolyFlames>(), 180);
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.Item14, projectile.position);
            projectile.position.X = projectile.position.X + (float)(projectile.width / 2);
            projectile.position.Y = projectile.position.Y + (float)(projectile.height / 2);
            projectile.width = projectile.height = 200;
            projectile.position.X = projectile.position.X - (float)(projectile.width / 2);
            projectile.position.Y = projectile.position.Y - (float)(projectile.height / 2);
            for (int num621 = 0; num621 < 4; num621++)
            {
                int num622 = Dust.NewDust(projectile.position, projectile.width, projectile.height, 244, 0f, 0f, 100, default, 2f);
                Main.dust[num622].velocity *= 3f;
                if (Main.rand.NextBool(2))
                {
                    Main.dust[num622].scale = 0.5f;
                    Main.dust[num622].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                }
            }
            for (int num623 = 0; num623 < 12; num623++)
            {
                int num624 = Dust.NewDust(projectile.position, projectile.width, projectile.height, 244, 0f, 0f, 100, default, 3f);
                Main.dust[num624].noGravity = true;
                Main.dust[num624].velocity *= 5f;
                num624 = Dust.NewDust(projectile.position, projectile.width, projectile.height, 244, 0f, 0f, 100, default, 2f);
                Main.dust[num624].velocity *= 2f;

            }
			CalamityUtils.ExplosionGores(projectile.Center, 3);
        }
    }
}
