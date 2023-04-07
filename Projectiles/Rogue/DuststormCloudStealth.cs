﻿using Terraria;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Rogue
{
    public class DuststormCloudStealth : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/Rogue/DuststormCloud";

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Dust Cloud");
            Main.projFrames[Projectile.type] = 4;
        }

        public override void SetDefaults()
        {
            Projectile.width = 38;
            Projectile.height = 14;
            Projectile.friendly = true;
            Projectile.alpha = 255;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 3600;
            Projectile.DamageType = RogueDamageClass.Instance;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 5;
        }

        public override void AI()
        {
            Projectile.frameCounter++;
            if (Projectile.frameCounter > 6)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
            }
            if (Projectile.frame >= Main.projFrames[Projectile.type])
            {
                Projectile.frame = 0;
            }
            Projectile.velocity *= 0.998f;
            Projectile.ai[1] += 1f;
            if (Projectile.ai[1] >= 120f)
            {
                if (Projectile.alpha < 255)
                {
                    Projectile.alpha += 3;
                    if (Projectile.alpha > 255)
                    {
                        Projectile.alpha = 255;
                    }
                }
                else if (Projectile.owner == Main.myPlayer)
                {
                    Projectile.Kill();
                }
            }
            else if (Projectile.alpha > 80)
            {
                Projectile.alpha -= 30;
                if (Projectile.alpha < 80)
                {
                    Projectile.alpha = 80;
                }
            }
        }
    }
}
