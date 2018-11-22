﻿using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles
{
    public class WulfrumBolt : ModProjectile
    {
    	public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Bolt");
		}
    	
        public override void SetDefaults()
        {
            projectile.width = 8;
            projectile.height = 8;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.penetrate = 2;
            projectile.magic = true;
        }

        public override void AI()
        {
        	Lighting.AddLight(projectile.Center, ((255 - projectile.alpha) * 0f) / 255f, ((255 - projectile.alpha) * 0.15f) / 255f, ((255 - projectile.alpha) * 0.25f) / 255f);
			for (int num151 = 0; num151 < 3; num151++)
			{
				int num154 = 14;
				int num155 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width - num154 * 2, projectile.height - num154 * 2, 61, 0f, 0f, 100, default(Color), 3f);
				Main.dust[num155].noGravity = true;
				Main.dust[num155].velocity *= 0.1f;
				Main.dust[num155].velocity += projectile.velocity * 0.5f;
			}
			if (Main.rand.Next(8) == 0)
			{
				int num156 = 16;
				int num157 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width - num156 * 2, projectile.height - num156 * 2, 61, 0f, 0f, 100, default(Color), 2.25f);
				Main.dust[num157].velocity *= 0.25f;
				Main.dust[num157].velocity += projectile.velocity * 0.5f;
				return;
			}
        }
        
        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D tex = Main.projectileTexture[projectile.type];
            spriteBatch.Draw(tex, projectile.Center - Main.screenPosition, null, projectile.GetAlpha(lightColor), projectile.rotation, tex.Size() / 2f, projectile.scale, SpriteEffects.None, 0f);
            return false;
        }
    }
}