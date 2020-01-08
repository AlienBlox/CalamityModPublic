﻿using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Magic
{
    public class WaterStream : ModProjectile
    {
        public int addSprayTimer = 20;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Stream");
        }

        public override void SetDefaults()
        {
            projectile.width = 32;
            projectile.height = 32;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.penetrate = 3;
            projectile.extraUpdates = 2;
            projectile.magic = true;
            projectile.tileCollide = false;
        }

        public override void AI()
        {
            projectile.ai[1]++;
            if (projectile.ai[1] >= 4f)
            {
                projectile.tileCollide = true;
            }
            addSprayTimer--;
            if (addSprayTimer <= 0)
            {
                if (projectile.owner == Main.myPlayer)
                {
                    Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, 0f, 0f, ModContent.ProjectileType<WaterStream2>(), projectile.damage, projectile.knockBack, projectile.owner, 0f, 0f);
                }
                addSprayTimer = 20;
            }
            Lighting.AddLight(projectile.Center, (255 - projectile.alpha) * 0f / 255f, (255 - projectile.alpha) * 0.15f / 255f, (255 - projectile.alpha) * 0.25f / 255f);
            projectile.scale -= 0.002f;
            if (projectile.scale <= 0f)
            {
                projectile.Kill();
            }
            if (projectile.ai[0] <= 3f)
            {
                projectile.ai[0] += 1f;
                return;
            }
            projectile.velocity.Y = projectile.velocity.Y + 0.075f;
            for (int num151 = 0; num151 < 3; num151++)
            {
                float num152 = projectile.velocity.X / 3f * (float)num151;
                float num153 = projectile.velocity.Y / 3f * (float)num151;
                int num154 = 14;
                int num155 = Dust.NewDust(new Vector2(projectile.position.X + (float)num154, projectile.position.Y + (float)num154), projectile.width - num154 * 2, projectile.height - num154 * 2, 14, 0f, 0f, 100, new Color(0, 255, 255), 1f);
                Dust dust = Main.dust[num155];
                dust.noGravity = true;
                dust.velocity *= 0.1f;
                dust.velocity += projectile.velocity * 0.5f;
                dust.position.X -= num152;
                dust.position.Y -= num153;
            }
            if (Main.rand.NextBool(8))
            {
                int num156 = 16;
                int num157 = Dust.NewDust(new Vector2(projectile.position.X + (float)num156, projectile.position.Y + (float)num156), projectile.width - num156 * 2, projectile.height - num156 * 2, 14, 0f, 0f, 100, new Color(0, 255, 255), 0.5f);
                Main.dust[num157].velocity *= 0.25f;
                Main.dust[num157].velocity += projectile.velocity * 0.5f;
            }
        }
    }
}
