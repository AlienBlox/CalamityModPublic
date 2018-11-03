﻿using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles
{
    public class ExoTornado : ModProjectile
    {
    	public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Tornado");
		}
    	
        public override void SetDefaults()
        {
            projectile.width = 10;
            projectile.height = 10;
            projectile.friendly = true;
            projectile.tileCollide = false;
            projectile.ranged = true;
            projectile.penetrate = -1;
            projectile.timeLeft = 300; //1200
            projectile.usesLocalNPCImmunity = true;
			projectile.localNPCHitCooldown = 5;
        }
        
        public override void AI()
        {
			float num1125 = 225f; //900
			if (projectile.soundDelay == 0) 
			{
				projectile.soundDelay = -1;
				Main.PlaySound(2, projectile.Center, 122);
			}
			projectile.ai[0] += 1f;
			if (projectile.ai[0] >= num1125) 
			{
				projectile.Kill();
			}
			if (projectile.localAI[0] >= 30f) 
			{
				projectile.damage = 0;
				if (projectile.ai[0] < num1125 - 120f) 
				{
					float num1126 = projectile.ai[0] % 60f;
					projectile.ai[0] = num1125 - 120f + num1126;
					projectile.netUpdate = true;
				}
			}
			float num1127 = 30f; //15
			float num1128 = 30f; //15
			Point point8 = projectile.Center.ToTileCoordinates();
			int num1129;
			int num1130;
			Collision.ExpandVertically(point8.X, point8.Y, out num1129, out num1130, (int)num1127, (int)num1128);
			num1129++;
			num1130--;
			Vector2 value72 = new Vector2((float)point8.X, (float)num1129) * 16f + new Vector2(8f);
			Vector2 value73 = new Vector2((float)point8.X, (float)num1130) * 16f + new Vector2(8f);
			Vector2 vector146 = Vector2.Lerp(value72, value73, 0.5f);
			Vector2 value74 = new Vector2(0f, value73.Y - value72.Y);
			value74.X = value74.Y * 0.2f;
			projectile.width = (int)(value74.X * 0.65f);
			projectile.height = (int)value74.Y;
			projectile.Center = vector146;
			if (projectile.owner == Main.myPlayer) 
			{
				bool flag74 = false;
				Vector2 center16 = Main.player[projectile.owner].Center;
				Vector2 top = Main.player[projectile.owner].Top;
				for (float num1131 = 0f; num1131 < 1f; num1131 += 0.05f) 
				{
					Vector2 position2 = Vector2.Lerp(value72, value73, num1131);
					if (Collision.CanHitLine(position2, 0, 0, center16, 0, 0) || Collision.CanHitLine(position2, 0, 0, top, 0, 0)) 
					{
						flag74 = true;
						break;
					}
				}
				if (!flag74 && projectile.ai[0] < num1125 - 120f) 
				{
					float num1132 = projectile.ai[0] % 60f;
					projectile.ai[0] = num1125 - 120f + num1132;
					projectile.netUpdate = true;
				}
			}
			if (projectile.ai[0] < num1125 - 120f) 
			{
				return;
			}
        }
        
        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
        	float num226 = 150f; //600
			float num227 = 30f; //15
			float num228 = 30f; //15
			float num229 = projectile.ai[0];
			float scale5 = MathHelper.Clamp(num229 / 30f, 0f, 1f);
			if (num229 > num226 - 60f)
			{
				scale5 = MathHelper.Lerp(1f, 0f, (num229 - (num226 - 60f)) / 60f);
			}
			Microsoft.Xna.Framework.Point point5 = projectile.Center.ToTileCoordinates();
			int num230;
			int num231;
			Collision.ExpandVertically(point5.X, point5.Y, out num230, out num231, (int)num227, (int)num228);
			num230++;
			num231--;
			float num232 = 0.2f;
			Vector2 value32 = new Vector2((float)point5.X, (float)num230) * 16f + new Vector2(8f);
			Vector2 value33 = new Vector2((float)point5.X, (float)num231) * 16f + new Vector2(8f);
			Vector2.Lerp(value32, value33, 0.5f);
			Vector2 vector33 = new Vector2(0f, value33.Y - value32.Y);
			vector33.X = vector33.Y * num232;
			new Vector2(value32.X - vector33.X / 2f, value32.Y);
			Texture2D texture2D23 = Main.projectileTexture[projectile.type];
			Microsoft.Xna.Framework.Rectangle rectangle9 = texture2D23.Frame(1, 1, 0, 0);
			Vector2 origin3 = rectangle9.Size() / 2f;
			float num233 = -0.06283186f * num229;
			Vector2 spinningpoint2 = Vector2.UnitY.RotatedBy((double)(num229 * 0.1f), default(Vector2));
			float num234 = 0f;
			float num235 = 5.1f;
			Microsoft.Xna.Framework.Color value34 = new Microsoft.Xna.Framework.Color(0, 250, 0);
			for (float num236 = (float)((int)value33.Y); num236 > (float)((int)value32.Y); num236 -= num235)
			{
				num234 += num235;
				float num237 = num234 / vector33.Y;
				float num238 = num234 * 6.28318548f / -20f;
				float num239 = num237 - 0.15f;
				Vector2 vector34 = spinningpoint2.RotatedBy((double)num238, default(Vector2));
				Vector2 value35 = new Vector2(0f, num237 + 1f);
				value35.X = value35.Y * num232;
				Microsoft.Xna.Framework.Color color39 = Microsoft.Xna.Framework.Color.Lerp(Microsoft.Xna.Framework.Color.Transparent, value34, num237 * 2f);
				if (num237 > 0.5f)
				{
					color39 = Microsoft.Xna.Framework.Color.Lerp(Microsoft.Xna.Framework.Color.Transparent, value34, 2f - num237 * 2f);
				}
				color39.A = (byte)((float)color39.A * 0.5f);
				color39 *= scale5;
				vector34 *= value35 * 100f;
				vector34.Y = 0f;
				vector34.X = 0f;
				vector34 += new Vector2(value33.X, num236) - Main.screenPosition;
				Main.spriteBatch.Draw(texture2D23, vector34, new Microsoft.Xna.Framework.Rectangle?(rectangle9), color39, num233 + num238, origin3, 1f + num239, SpriteEffects.None, 0f);
			}
			return false;
        }
    }
}