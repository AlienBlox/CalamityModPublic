using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public class GhoulishGougerBoomerang : ModProjectile
    {
        public override string Texture => "CalamityMod/Items/Weapons/Rogue/GhoulishGouger";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Scythe");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            projectile.width = 74;
            projectile.height = 68;
            projectile.friendly = true;
            projectile.tileCollide = false;
            projectile.penetrate = -1;
            projectile.extraUpdates = 4;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 10;
            projectile.Calamity().rogue = true;
        }

        public override void AI()
        {
            Lighting.AddLight(projectile.Center, 0.7f, 0f, 0.15f);
            if (projectile.soundDelay == 0)
            {
                projectile.soundDelay = 8;
                Main.PlaySound(SoundID.Item7, projectile.position);
            }
            if (projectile.ai[0] == 0f)
            {
                projectile.ai[1] += 1f;
                if (projectile.ai[1] >= 45f)
                {
                    projectile.ai[0] = 1f;
                    projectile.ai[1] = 0f;
                    projectile.netUpdate = true;
                }
            }
            else
            {
                projectile.tileCollide = false;
                float num42 = 28f;
                float num43 = 5f;
                Vector2 vector2 = new Vector2(projectile.position.X + (float)projectile.width * 0.5f, projectile.position.Y + (float)projectile.height * 0.5f);
                float num44 = Main.player[projectile.owner].position.X + (float)(Main.player[projectile.owner].width / 2) - vector2.X;
                float num45 = Main.player[projectile.owner].position.Y + (float)(Main.player[projectile.owner].height / 2) - vector2.Y;
                float num46 = (float)Math.Sqrt((double)(num44 * num44 + num45 * num45));
                if (num46 > 3000f)
                {
                    projectile.Kill();
                }
                num46 = num42 / num46;
                num44 *= num46;
                num45 *= num46;
                if (projectile.velocity.X < num44)
                {
                    projectile.velocity.X = projectile.velocity.X + num43;
                    if (projectile.velocity.X < 0f && num44 > 0f)
                    {
                        projectile.velocity.X = projectile.velocity.X + num43;
                    }
                }
                else if (projectile.velocity.X > num44)
                {
                    projectile.velocity.X = projectile.velocity.X - num43;
                    if (projectile.velocity.X > 0f && num44 < 0f)
                    {
                        projectile.velocity.X = projectile.velocity.X - num43;
                    }
                }
                if (projectile.velocity.Y < num45)
                {
                    projectile.velocity.Y = projectile.velocity.Y + num43;
                    if (projectile.velocity.Y < 0f && num45 > 0f)
                    {
                        projectile.velocity.Y = projectile.velocity.Y + num43;
                    }
                }
                else if (projectile.velocity.Y > num45)
                {
                    projectile.velocity.Y = projectile.velocity.Y - num43;
                    if (projectile.velocity.Y > 0f && num45 < 0f)
                    {
                        projectile.velocity.Y = projectile.velocity.Y - num43;
                    }
                }
                if (Main.myPlayer == projectile.owner)
                {
                    Rectangle rectangle = new Rectangle((int)projectile.position.X, (int)projectile.position.Y, projectile.width, projectile.height);
                    Rectangle value2 = new Rectangle((int)Main.player[projectile.owner].position.X, (int)Main.player[projectile.owner].position.Y, Main.player[projectile.owner].width, Main.player[projectile.owner].height);
                    if (rectangle.Intersects(value2))
                    {
                        projectile.Kill();
                    }
                }
            }
            projectile.rotation += 0.5f;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(250, 250, 250, 50);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityGlobalProjectile.DrawCenteredAndAfterimage(projectile, lightColor, ProjectileID.Sets.TrailingMode[projectile.type], 1);
            return false;
        }

		public override void PostDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			Vector2 origin = new Vector2(37f, 34f);
			spriteBatch.Draw(ModContent.GetTexture("CalamityMod/Items/Weapons/Rogue/GhoulishGougerGlow"), projectile.Center - Main.screenPosition, null, Color.White, projectile.rotation, origin, 1f, SpriteEffects.None, 0f);
		}

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
			float spread = 45f * 0.0174f;
			double startAngle = Math.Atan2(projectile.velocity.X, projectile.velocity.Y) - spread / 2;
			double deltaAngle = spread / 8f;
			double offsetAngle;
			int i;
			if (projectile.owner == Main.myPlayer && projectile.Calamity().stealthStrike && Main.player[projectile.owner].ownedProjectileCounts[ModContent.ProjectileType<PhantasmalSoul>()] < 8)
			{
				for (i = 0; i < 8; i++)
				{
					float ai1 = Main.rand.NextFloat() + 0.5f;
					float randomSpeed = (float)Main.rand.Next(1, 7);
					float randomSpeed2 = (float)Main.rand.Next(1, 7);
					offsetAngle = startAngle + deltaAngle * (i + i * i) / 2f + 32f * i;
					int num23 = Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, (float)(Math.Sin(offsetAngle) * 5f), (float)(Math.Cos(offsetAngle) * 5f) + randomSpeed, ModContent.ProjectileType<PhantasmalSoul>(), (int)((double)projectile.damage * 0.2), 0f, projectile.owner, 1f, ai1);
					int num24 = Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, (float)(-Math.Sin(offsetAngle) * 5f), (float)(-Math.Cos(offsetAngle) * 5f) + randomSpeed2, ModContent.ProjectileType<PhantasmalSoul>(), (int)((double)projectile.damage * 0.2), 0f, projectile.owner, 1f, ai1);
				}
			}
		}
	}
}
