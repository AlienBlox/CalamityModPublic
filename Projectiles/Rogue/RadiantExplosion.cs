﻿using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Dusts;
using CalamityMod.Projectiles.Typeless;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Rogue
{
    public class RadiantExplosion : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Explosion");
        }

        public override void SetDefaults()
        {
            projectile.width = 150;
            projectile.height = 150;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.penetrate = -1;
            projectile.timeLeft = 10;
            projectile.Calamity().rogue = true;
        }

        public override void AI()
        {
            projectile.localAI[0] += 1f;
            if (projectile.localAI[0] > 4f)
            {
                for (int i = 0; i < 5; i++)
                {
                    int num469 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, ModContent.DustType<AstralBlue>(), 0f, 0f, 100, default, 1.5f);
                    Main.dust[num469].noGravity = true;
                    Main.dust[num469].velocity *= 0f;
                }
                for (int i = 0; i < 5; i++)
                {
                    int num469 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, ModContent.DustType<AstralOrange>(), 0f, 0f, 100, default, 1.5f);
                    Main.dust[num469].noGravity = true;
                    Main.dust[num469].velocity *= 0f;
                }
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<AstralInfectionDebuff>(), 120);
            for (int n = 0; n < 3; n++)
            {
                float x = target.position.X + (float)Main.rand.Next(-400, 400);
                float y = target.position.Y - (float)Main.rand.Next(500, 800);
                Vector2 vector = new Vector2(x, y);
                float num13 = target.position.X + (float)(target.width / 2) - vector.X;
                float num14 = target.position.Y + (float)(target.height / 2) - vector.Y;
                num13 += (float)Main.rand.Next(-100, 101);
                int num15 = 25;
                int projectileType = Main.rand.Next(3);
                if (projectileType == 0)
                {
                    projectileType = ModContent.ProjectileType<AstralStar>();
                }
                else if (projectileType == 1)
                {
                    projectileType = ProjectileID.HallowStar;
                }
                else
                {
                    projectileType = ProjectileID.FallingStar;
                }
                float num16 = (float)Math.Sqrt((double)(num13 * num13 + num14 * num14));
                num16 = (float)num15 / num16;
                num13 *= num16;
                num14 *= num16;
                int num17 = Projectile.NewProjectile(x, y, num13, num14, projectileType, (int)((double)projectile.damage * 0.75), 5f, projectile.owner, 2f, 0f);
                Main.projectile[num17].ranged = false;
            }
        }
    }
}
