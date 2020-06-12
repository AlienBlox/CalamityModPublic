using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Boss
{
    public class FlareDust : ModProjectile
    {
		private bool start = true;
		private float startingPosX = 0f;
		private float startingPosY = 0f;
		private double distance = 0D;

		public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Flare Dust");
            Main.projFrames[projectile.type] = 4;
        }

        public override void SetDefaults()
        {
            projectile.width = 30;
            projectile.height = 30;
            projectile.scale = 1.5f;
            projectile.hostile = true;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.penetrate = -1;
            projectile.timeLeft = 560;
            cooldownSlot = 1;
        }

		public override void SendExtraAI(BinaryWriter writer)
		{
			writer.Write(projectile.localAI[0]);
			writer.Write(start);
			writer.Write(startingPosX);
			writer.Write(startingPosY);
			writer.Write(distance);
		}

		public override void ReceiveExtraAI(BinaryReader reader)
		{
			projectile.localAI[0] = reader.ReadSingle();
			start = reader.ReadBoolean();
			startingPosX = reader.ReadSingle();
			startingPosY = reader.ReadSingle();
			distance = reader.ReadDouble();
		}

		public override void AI()
        {
            projectile.frameCounter++;
            if (projectile.frameCounter > 4)
            {
                projectile.frame++;
                projectile.frameCounter = 0;
            }
            if (projectile.frame > 3)
            {
                projectile.frame = 0;
            }

            Lighting.AddLight(projectile.Center, 0.5f, 0.25f, 0f);

			if (projectile.ai[0] == 2f)
				return;

			if (start)
			{
				startingPosX = projectile.Center.X;
				startingPosY = projectile.Center.Y;
				start = false;
			}

			double rad = MathHelper.ToRadians(projectile.ai[1]);

			float amount = projectile.localAI[0] / 120f;
			if (amount > 1f)
				amount = 1f;

			distance += MathHelper.Lerp(1f, 9f, amount);

			int timeGateValue = 180;
			if (projectile.timeLeft < timeGateValue)
				distance += 6f;

			if (projectile.ai[0] == 0f)
			{
				projectile.position.X = startingPosX - (int)(Math.Sin(rad) * distance) - projectile.width / 2;
				projectile.position.Y = startingPosY - (int)(Math.Cos(rad) * distance) - projectile.height / 2;
			}
			else
			{
				projectile.position.X = startingPosX - (int)(Math.Cos(rad) * distance) - projectile.width / 2;
				projectile.position.Y = startingPosY - (int)(Math.Sin(rad) * distance) - projectile.height / 2;
			}

			projectile.ai[1] += (1.1f - amount) * 0.5f;
			projectile.localAI[0] += 1f;
		}

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255, Main.DiscoG, 53, projectile.alpha);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture2D13 = Main.projectileTexture[projectile.type];
            int num214 = Main.projectileTexture[projectile.type].Height / Main.projFrames[projectile.type];
            int y6 = num214 * projectile.frame;
            Main.spriteBatch.Draw(texture2D13, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(new Rectangle(0, y6, texture2D13.Width, num214)), projectile.GetAlpha(lightColor), projectile.rotation, new Vector2((float)texture2D13.Width / 2f, (float)num214 / 2f), projectile.scale, SpriteEffects.None, 0f);
            return false;
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.Item14, projectile.position);
            projectile.position = projectile.Center;
            projectile.width = projectile.height = 48;
            projectile.position.X = projectile.position.X - (float)(projectile.width / 2);
            projectile.position.Y = projectile.position.Y - (float)(projectile.height / 2);
            for (int num621 = 0; num621 < 2; num621++)
            {
                int num622 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 244, 0f, 0f, 100, default, 1f);
                Main.dust[num622].velocity *= 3f;
                if (Main.rand.NextBool(2))
                {
                    Main.dust[num622].scale = 0.5f;
                    Main.dust[num622].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                }
            }
            for (int num623 = 0; num623 < 4; num623++)
            {
                int num624 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 244, 0f, 0f, 100, default, 2f);
                Main.dust[num624].noGravity = true;
                Main.dust[num624].velocity *= 5f;
                num624 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 244, 0f, 0f, 100, default, 1f);
                Main.dust[num624].velocity *= 2f;
            }
			CalamityUtils.ExplosionGores(projectile, 3);
            projectile.damage = Main.expertMode ? 75 : 90;
            projectile.Damage();
        }

        public override void ModifyHitPlayer(Player target, ref int damage, ref bool crit)	
        {
			target.Calamity().lastProjectileHit = projectile;
		}
    }
}
