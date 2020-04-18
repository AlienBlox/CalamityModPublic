﻿using CalamityMod.Items.Weapons.Rogue;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Rogue
{
    public class CrystalPiercerProjectile : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Crystal Piercer");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            projectile.width = 12;
            projectile.height = 12;
            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.aiStyle = 113;
            projectile.timeLeft = 600;
            aiType = ProjectileID.BoneJavelin;
            projectile.Calamity().rogue = true;
            projectile.localNPCHitCooldown = 10;
        }

        public override void AI()
        {
            if (Main.rand.NextBool(3))
            {
                Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 173, projectile.velocity.X * 0.1f, projectile.velocity.Y * 0.1f);
            }
            projectile.rotation = (float)Math.Atan2((double)projectile.velocity.Y, (double)projectile.velocity.X) + 2.355f;
            if (projectile.spriteDirection == -1)
            {
                projectile.rotation -= 1.57f;
            }

        	if (projectile.timeLeft % 4 == 0)
			{
        		if (projectile.owner == Main.myPlayer)
        		{
					if (projectile.Calamity().stealthStrike)
					{
						Projectile.NewProjectile(projectile.Center.X + Main.rand.NextFloat(-15f, 15f), projectile.Center.Y + Main.rand.NextFloat(-15f, 15f), projectile.velocity.X, projectile.velocity.Y, ModContent.ProjectileType<CrystalPiercerShard>(), (int)(projectile.damage * 0.4), projectile.knockBack * 0.4f, projectile.owner, 0f, 0f);
					}
                }
			}
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityGlobalProjectile.DrawCenteredAndAfterimage(projectile, lightColor, ProjectileID.Sets.TrailingMode[projectile.type], 1);
            return false;
        }

		//glowmask effect if stealth strike
        public override Color? GetAlpha(Color lightColor)
		{
			if (projectile.Calamity().stealthStrike)
				return new Color(200, 200, 200, 200);
			else
				return null;
		}

        public override void Kill(int timeLeft)
        {
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 173, projectile.oldVelocity.X * 0.5f, projectile.oldVelocity.Y * 0.5f);
            }
            if (Main.rand.NextBool(2))
            {
                Item.NewItem((int)projectile.position.X, (int)projectile.position.Y, projectile.width, projectile.height, ModContent.ItemType<CrystalPiercer>());
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.Frostburn, 120);
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            target.AddBuff(BuffID.Frostburn, 120);
        }
    }
}
