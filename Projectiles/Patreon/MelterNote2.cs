﻿using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Patreon
{
    public class MelterNote2 : ModProjectile
    {
    	public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Song");
		}
    	
        public override void SetDefaults()
        {
            projectile.width = 16;
            projectile.height = 24;
            projectile.friendly = true;
            projectile.magic = true;
            projectile.penetrate = 10;
            projectile.timeLeft = 600;
			projectile.usesLocalNPCImmunity = true;
			projectile.localNPCHitCooldown = 7;
			projectile.tileCollide = false;
			projectile.ignoreWater = true;
        }

        public override void AI()
        {
        	projectile.velocity.X *= 0.99f;
        	projectile.velocity.Y *= 0.99f;
        	if (projectile.localAI[0] == 0f)
			{
				projectile.scale += 0.02f;
				if (projectile.scale >= 1.25f)
				{
					projectile.localAI[0] = 1f;
				}
			}
			else if (projectile.localAI[0] == 1f)
			{
				projectile.scale -= 0.02f;
				if (projectile.scale <= 0.75f)
				{
					projectile.localAI[0] = 0f;
				}
			}
        }
		
		public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255, 255, 255);
        }
    }
}