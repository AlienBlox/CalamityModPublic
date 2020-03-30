﻿using CalamityMod.Projectiles.Typeless;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Hybrid
{
    public class FlameScytheProjectile : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Scythe");
        }

        public override void SetDefaults()
        {
            projectile.width = 30;
            projectile.height = 30;
            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.aiStyle = 3;
            projectile.extraUpdates = 1;
            projectile.timeLeft = 600;
            projectile.alpha = 55;
            aiType = ProjectileID.WoodenBoomerang;
            projectile.Calamity().rogue = true;
        }

        public override void AI()
        {
            Lighting.AddLight(projectile.Center, 0.25f, 0.15f, 0f);
            if (Main.rand.NextBool(5))
            {
                Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, Main.rand.NextBool(3) ? 16 : 127, projectile.velocity.X * 0.5f, projectile.velocity.Y * 0.5f);
            }
            Vector2 goreVec = new Vector2(projectile.position.X + (float)(projectile.width / 2) + projectile.velocity.X, projectile.position.Y + (float)(projectile.height / 2) + projectile.velocity.Y);
			if (Main.rand.NextBool(8))
			{
				int smoke = Gore.NewGore(goreVec, default, Main.rand.Next(375, 378), 0.75f);
				Main.gore[smoke].behindTiles = true;
			}
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D tex = Main.projectileTexture[projectile.type];
            spriteBatch.Draw(tex, projectile.Center - Main.screenPosition, null, projectile.GetAlpha(lightColor), projectile.rotation, tex.Size() / 2f, projectile.scale, SpriteEffects.None, 0f);
            return false;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.immune[projectile.owner] = 6;
            target.AddBuff(BuffID.OnFire, 300);
            if (projectile.owner == Main.myPlayer)
            {
                int proj = Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, 0f, 0f, ModContent.ProjectileType<FuckYou>(), projectile.damage, projectile.knockBack, projectile.owner, 0f, 0.85f + Main.rand.NextFloat() * 1.15f);
                Main.projectile[proj].Calamity().forceRogue = true;
            }
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            target.AddBuff(BuffID.OnFire, 300);
            if (projectile.owner == Main.myPlayer)
            {
                int proj = Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, 0f, 0f, ModContent.ProjectileType<FuckYou>(), projectile.damage, projectile.knockBack, projectile.owner, 0f, 0.85f + Main.rand.NextFloat() * 1.15f);
                Main.projectile[proj].Calamity().forceRogue = true;
            }
        }
    }
}
