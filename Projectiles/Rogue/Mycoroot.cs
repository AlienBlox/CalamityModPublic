﻿using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public class Mycoroot : ModProjectile
    {
    	public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Root");
		}
    	
        public override void SetDefaults()
        {
            projectile.width = 16;
            projectile.height = 16;
            projectile.friendly = true;
            projectile.penetrate = 1;
			projectile.GetGlobalProjectile<CalamityGlobalProjectile>(mod).rogue = true;
		}
        
        public override void AI()
        {
        	if (Main.rand.Next(4) == 0)
        	{
        		Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 56, 0f, 0f);
        	}
        	projectile.rotation = (float)Math.Atan2((double)projectile.velocity.Y, (double)projectile.velocity.X) + 0.785f;
        	projectile.alpha += 20;
        	if (projectile.alpha >= 255)
        	{
        		projectile.Kill();
        	}
        	Lighting.AddLight(projectile.Center, ((255 - projectile.alpha) * 0f) / 255f, ((255 - projectile.alpha) * 0.35f) / 255f, ((255 - projectile.alpha) * 0.5f) / 255f);
        }
        
        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D tex = Main.projectileTexture[projectile.type];
            spriteBatch.Draw(tex, projectile.Center - Main.screenPosition, null, projectile.GetAlpha(lightColor), projectile.rotation, tex.Size() / 2f, projectile.scale, SpriteEffects.None, 0f);
            return false;
        }
    }
}