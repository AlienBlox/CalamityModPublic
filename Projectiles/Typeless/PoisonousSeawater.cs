﻿using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Typeless
{
    public class PoisonousSeawater : ModProjectile
    {
    	public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Seawater");
		}

        public override void SetDefaults()
        {
            projectile.width = 30;
            projectile.height = 30;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.penetrate = -1;
			projectile.timeLeft = 6;
        }

        public override void AI()
        {
            Lighting.AddLight(projectile.Center, ((255 - projectile.alpha) * 0f) / 255f, ((255 - projectile.alpha) * 0.15f) / 255f, ((255 - projectile.alpha) * 0.4f) / 255f);
            int randomDust = Main.rand.Next(4);
            if (randomDust == 0)
            {
                randomDust = 33;
            }
            else if (randomDust == 1)
            {
                randomDust = 33;
            }
            else if (randomDust == 2)
            {
                randomDust = 33;
            }
            else
            {
                randomDust = 89;
            }
            for (int num468 = 0; num468 < 2; num468++)
            {
                int num469 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, randomDust, 0f, 0f, 100, default(Color), 1f);
                if (randomDust == 89)
                {
                    Main.dust[num469].scale *= 0.35f;
                }
                Main.dust[num469].velocity *= 0f;
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
        	target.AddBuff(BuffID.Venom, 120);
        	target.AddBuff(BuffID.Poisoned, 120);
        }
    }
}
