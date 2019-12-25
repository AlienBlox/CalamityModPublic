﻿using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Ranged
{
    public class AquaBlast : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Aqua Blast");
            Main.projFrames[projectile.type] = 4;
        }

        public override void SetDefaults()
        {
            projectile.width = 12;
            projectile.height = 12;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.penetrate = 1;
            projectile.timeLeft = 600;
            projectile.ranged = true;
        }

        public override void AI()
        {
            //Animation
            projectile.frameCounter++;
            if (projectile.frameCounter > 5)
            {
                projectile.frame++;
                projectile.frameCounter = 0;
            }
            if (projectile.frame > 3)
            {
                projectile.frame = 0;
            }

            //Rotation
            projectile.spriteDirection = projectile.direction = (projectile.velocity.X > 0).ToDirectionInt();
            projectile.rotation = projectile.velocity.ToRotation() + (projectile.spriteDirection == 1 ? 0f : MathHelper.Pi) + MathHelper.ToRadians(90) * projectile.direction;


            Lighting.AddLight(projectile.Center, (255 - projectile.alpha) * 0f / 255f, (255 - projectile.alpha) * 0f / 255f, (255 - projectile.alpha) * 0.75f / 255f);
            for (int num457 = 0; num457 < 2; num457++)
            {
                Vector2 dspeed = -projectile.velocity * Main.rand.NextFloat(0.5f * 0.8f);
                int num458 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 33, 0f, 0f, 100, default, 1f);
                Main.dust[num458].noGravity = true;
                Main.dust[num458].velocity = dspeed;
            }
        }

        public override void Kill(int timeLeft)
        {
            for (int lol = 0; lol < 10; lol++)
            {
                Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 33, 0f, 0f, 100, default, 1f);
            }
        }
    }
}
