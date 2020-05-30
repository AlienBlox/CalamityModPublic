using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Projectiles.Rogue
{
    public class DeepSeaDumbbell1 : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Deep Sea Dumbbell");
        }

        public override void SetDefaults()
        {
            projectile.width = 26;
            projectile.height = 26;
            projectile.friendly = true;
            projectile.Calamity().rogue = true;
            projectile.ignoreWater = true;
            projectile.penetrate = 1;
        }

        public override void AI()
        {
            projectile.rotation += Math.Abs(projectile.velocity.X) * 0.01f * (float)projectile.direction;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Main.PlaySound(SoundID.NPCKilled, (int)projectile.position.X, (int)projectile.position.Y, 43, 0.35f, 0f);

            if (projectile.velocity.X != oldVelocity.X)
                projectile.velocity.X = -oldVelocity.X;
            if (projectile.velocity.Y != oldVelocity.Y)
                projectile.velocity.Y = -oldVelocity.Y;

            if (projectile.owner == Main.myPlayer)
            {
                Projectile.NewProjectile(projectile.position.X, projectile.position.Y, projectile.velocity.X, projectile.velocity.Y, ModContent.ProjectileType<DeepSeaDumbbell2>(),
                        (int)((double)projectile.damage * 0.75), projectile.knockBack * 0.75f, Main.myPlayer, 0f, 0f);

                float num628 = (float)Main.rand.Next(-35, 36) * 0.01f;
                float num629 = (float)Main.rand.Next(-35, 36) * 0.01f;
                int num3;
                for (int num627 = 0; num627 < 2; num627 = num3 + 1)
                {
                    if (num627 == 1)
                    {
                        num628 *= 10f;
                        num629 *= 10f;
                    }
                    else
                    {
                        num628 *= -10f;
                        num629 *= -10f;
                    }

                    Projectile.NewProjectile(projectile.position.X, projectile.position.Y, num628, num629, ModContent.ProjectileType<DeepSeaDumbbellWeight>(),
                        (int)((double)projectile.damage * 0.25), projectile.knockBack * 0.25f, Main.myPlayer, 0f, 0f);

                    num3 = num627;
                }
            }

            projectile.Kill();

            return false;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            if (target.defense > 0)
                target.defense -= 15;

            target.AddBuff(ModContent.BuffType<CrushDepth>(), 600);

            Main.PlaySound(SoundID.NPCKilled, (int)projectile.position.X, (int)projectile.position.Y, 43, 0.35f, 0f);

            projectile.velocity.X = -projectile.velocity.X;
            projectile.velocity.Y = -projectile.velocity.Y;

            if (projectile.owner == Main.myPlayer)
            {
                Projectile.NewProjectile(projectile.position.X, projectile.position.Y, projectile.velocity.X, projectile.velocity.Y, ModContent.ProjectileType<DeepSeaDumbbell2>(),
                        (int)((double)projectile.damage * 0.75), projectile.knockBack * 0.75f, Main.myPlayer, 0f, 0f);

                float num628 = (float)Main.rand.Next(-35, 36) * 0.01f;
                float num629 = (float)Main.rand.Next(-35, 36) * 0.01f;
                int num3;
                for (int num627 = 0; num627 < 2; num627 = num3 + 1)
                {
                    if (num627 == 1)
                    {
                        num628 *= 10f;
                        num629 *= 10f;
                    }
                    else
                    {
                        num628 *= -10f;
                        num629 *= -10f;
                    }

                    Projectile.NewProjectile(projectile.position.X, projectile.position.Y, num628, num629, ModContent.ProjectileType<DeepSeaDumbbellWeight>(),
                        (int)((double)projectile.damage * 0.25), projectile.knockBack * 0.25f, Main.myPlayer, 0f, 0f);

                    num3 = num627;
                }
            }

            projectile.Kill();
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            target.AddBuff(ModContent.BuffType<CrushDepth>(), 600);

            Main.PlaySound(SoundID.NPCKilled, (int)projectile.position.X, (int)projectile.position.Y, 43, 0.35f, 0f);

            projectile.velocity.X = -projectile.velocity.X;
            projectile.velocity.Y = -projectile.velocity.Y;

            if (projectile.owner == Main.myPlayer)
            {
                Projectile.NewProjectile(projectile.position.X, projectile.position.Y, projectile.velocity.X, projectile.velocity.Y, ModContent.ProjectileType<DeepSeaDumbbell2>(),
                        (int)((double)projectile.damage * 0.75), projectile.knockBack * 0.75f, Main.myPlayer, 0f, 0f);

                float num628 = (float)Main.rand.Next(-35, 36) * 0.01f;
                float num629 = (float)Main.rand.Next(-35, 36) * 0.01f;
                int num3;
                for (int num627 = 0; num627 < 2; num627 = num3 + 1)
                {
                    if (num627 == 1)
                    {
                        num628 *= 10f;
                        num629 *= 10f;
                    }
                    else
                    {
                        num628 *= -10f;
                        num629 *= -10f;
                    }

                    Projectile.NewProjectile(projectile.position.X, projectile.position.Y, num628, num629, ModContent.ProjectileType<DeepSeaDumbbellWeight>(),
                        (int)((double)projectile.damage * 0.25), projectile.knockBack * 0.25f, Main.myPlayer, 0f, 0f);

                    num3 = num627;
                }
            }

            projectile.Kill();
        }
    }
}
