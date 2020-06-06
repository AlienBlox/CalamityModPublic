using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Projectiles.Typeless;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Rogue
{
    public class XerocPitchforkProjectile : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Pitchfork");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 5;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            projectile.width = 12;
            projectile.height = 12;
            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.extraUpdates = 1;
            projectile.aiStyle = 113;
            projectile.timeLeft = 600;
            aiType = ProjectileID.BoneJavelin;
            projectile.Calamity().rogue = true;
        }

        public override void AI()
        {
            projectile.rotation = (float)Math.Atan2((double)projectile.velocity.Y, (double)projectile.velocity.X) + 2.355f;
            if (Main.rand.NextBool(3))
            {
                Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 58, projectile.velocity.X * 0.5f, projectile.velocity.Y * 0.5f);
            }
            if (projectile.spriteDirection == -1)
            {
                projectile.rotation -= 1.57f;
            }
			if (projectile.timeLeft % 12 == 0)
			{
				if (projectile.owner == Main.myPlayer)
				{
					if (projectile.Calamity().stealthStrike)
					{
						int star = Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, projectile.velocity.X * 0.25f, projectile.velocity.Y * 0.25f, ModContent.ProjectileType<XerocStar>(), (int)(projectile.damage * 0.5), projectile.knockBack, projectile.owner, 0f, 0f);
						Main.projectile[star].Calamity().rogue = true;
					}
				}
			}
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityGlobalProjectile.DrawCenteredAndAfterimage(projectile, lightColor, ProjectileID.Sets.TrailingMode[projectile.type], 1);
            return false;
        }

        public override void Kill(int timeLeft)
        {
            if (Main.rand.NextBool(2))
            {
                Item.NewItem((int)projectile.position.X, (int)projectile.position.Y, projectile.width, projectile.height, ModContent.ItemType<XerocPitchfork>());
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            projectile.damage /= 2;
            if (projectile.damage < 1)
            {
                projectile.damage = 1;
            }
            target.immune[projectile.owner] = 2;
            target.AddBuff(BuffID.CursedInferno, 300);
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            projectile.damage /= 2;
            if (projectile.damage < 1)
            {
                projectile.damage = 1;
            }
            target.AddBuff(BuffID.CursedInferno, 300);
        }
    }
}
