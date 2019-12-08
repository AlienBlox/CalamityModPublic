using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System;
namespace CalamityMod.Projectiles.Rogue
{
    public class SphereBlue : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Blue Thruster Sphere");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            projectile.width = 20;
            projectile.height = 20;
            projectile.friendly = true;
            projectile.penetrate = 1;
            projectile.timeLeft = 300;
            projectile.tileCollide = false;
            projectile.Calamity().rogue = true;
            projectile.extraUpdates = 1;
        }

        public override void AI()
        {
            Lighting.AddLight(projectile.Center, 0f, 0f, 1f);
            if (Main.rand.NextBool(5))
            {
                Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 229, projectile.velocity.X * 0.5f, projectile.velocity.Y * 0.5f, 100);
            }
            projectile.ai[0] += 1f;
            if (projectile.ai[0] >= 5f)
            {
                projectile.tileCollide = true;
            }
            projectile.rotation += projectile.velocity.X * 0.02f;
            projectile.velocity.Y = projectile.velocity.Y + 0.085f;
            projectile.velocity.X = projectile.velocity.X * 0.99f;
			float pcx = projectile.Center.X;
			float pcy = projectile.Center.Y;
			float var1 = 800f;
			bool flag = false;
			for (int npcvar = 0; npcvar < 200; npcvar++)
			{
				if (Main.npc[npcvar].CanBeChasedBy(projectile, false) && Collision.CanHit(projectile.Center, 1, 1, Main.npc[npcvar].Center, 1, 1))
				{
					float var2 = Main.npc[npcvar].position.X + (Main.npc[npcvar].width / 2);
					float var3 = Main.npc[npcvar].position.Y + (Main.npc[npcvar].height / 2);
					float var4 = Math.Abs(projectile.position.X + (projectile.width / 2) - var2) + Math.Abs(projectile.position.Y + (projectile.height / 2) - var3);
					if (var4 < var1)
					{
						var1 = var4;
						pcx = var2;
						pcy = var3;
						flag = true;
					}
				}
			}
			if (flag)
			{
				float homingstrenght = 16f;
				Vector2 vector1 = new Vector2(projectile.position.X + projectile.width * 0.5f, projectile.position.Y + projectile.height * 0.5f);
				float var6 = pcx - vector1.X;
				float var7 = pcy - vector1.Y;
				float var8 = (float)Math.Sqrt(var6 * var6 + var7 * var7);
				var8 = homingstrenght / var8;
				var6 *= var8;
				var7 *= var8;
				projectile.velocity.X = (projectile.velocity.X * 20f + var6) / 21f;
				projectile.velocity.Y = (projectile.velocity.Y * 20f + var7) / 21f;
			}
        }

        public override void Kill(int timeLeft)
        {
            projectile.position = projectile.Center;
            projectile.width = projectile.height = 192;
            projectile.position.X = projectile.position.X - (float)(projectile.width / 2);
            projectile.position.Y = projectile.position.Y - (float)(projectile.height / 2);
            projectile.maxPenetrate = -1;
            projectile.penetrate = -1;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 10;
            projectile.Damage();
            Main.PlaySound(4, (int)projectile.Center.X, (int)projectile.Center.Y, 37);
            for (int num625 = 0; num625 < 3; num625++)
            {
                float scaleFactor10 = 0.33f;
                if (num625 == 1)
                {
                    scaleFactor10 = 0.66f;
                }
                if (num625 == 2)
                {
                    scaleFactor10 = 1f;
                }
				int defectiveBruh = Gore.NewGore(new Vector2(projectile.position.X + (float)(projectile.width / 2) - 24f, projectile.position.Y + (float)(projectile.height / 2) - 24f), default, Main.rand.Next(660, 662), 1f);
				Main.gore[defectiveBruh].velocity *= scaleFactor10;
				Main.gore[defectiveBruh].velocity += projectile.velocity;
            }
            for (int num194 = 0; num194 < 25; num194++)
            {
				int dustType = Utils.SelectRandom(Main.rand, new int[]
				{
					226,
					229
				});
                int num195 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, dustType, 0f, 0f, 100, default, 1f);
                Main.dust[num195].noGravity = true;
                Main.dust[num195].velocity *= 0f;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityGlobalProjectile.DrawCenteredAndAfterimage(projectile, lightColor, ProjectileID.Sets.TrailingMode[projectile.type], 1);
            return false;
        }
    }
}
