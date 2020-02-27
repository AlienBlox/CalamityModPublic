﻿using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Rogue
{
    public class CosmicKunaiProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Kunai");
        }

        public override void SetDefaults()
        {
            projectile.width = 24;
            projectile.height = 24;
            projectile.friendly = true;
            projectile.tileCollide = false;
            projectile.penetrate = 1;
            projectile.Calamity().rogue = true;
        }

        public override void AI()
        {
            projectile.rotation = (float)Math.Atan2((double)projectile.velocity.Y, (double)projectile.velocity.X) + 1.57f;
            projectile.alpha += 17;
            if (projectile.alpha >= 255)
            {
                projectile.Kill();
            }
            Lighting.AddLight(projectile.Center, (255 - projectile.alpha) * 0.5f / 255f, (255 - projectile.alpha) * 0f / 255f, (255 - projectile.alpha) * 0.5f / 255f);
			CalamityGlobalProjectile.HomeInOnNPC(projectile, false, 300f, 28f, 20f);
        }
    }
}
