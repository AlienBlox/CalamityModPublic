using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Hybrid
{
    public class CorpusAvertorClone : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Corpus Avertor");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            projectile.width = 24;
            projectile.height = 24;
            projectile.friendly = true;
            projectile.penetrate = 1;
            projectile.timeLeft = 180;
            projectile.melee = true;
            projectile.tileCollide = false;
        }

        public override void AI()
        {
            if (projectile.ai[1] == 1f)
            {
                projectile.melee = false;
                projectile.Calamity().rogue = true;
            }

            projectile.rotation += (Math.Abs(projectile.velocity.X) + Math.Abs(projectile.velocity.Y)) * 0.04f;

            projectile.velocity.X *= 1.005f;
            projectile.velocity.Y *= 1.005f;

            switch ((int)projectile.ai[0])
            {
                case 20:
                    projectile.scale = 0.7f;
                    break;
                case 40:
                    projectile.scale = 0.8f;
                    break;
                case 60:
                    projectile.scale = 0.9f;
                    break;
                default:
                    break;
            }
            projectile.width = projectile.height = (int)(24f * projectile.scale);

			CalamityGlobalProjectile.HomeInOnNPC(projectile, true, 300f, 16f, 20f);
        }

        public override Color? GetAlpha(Color lightColor)
        {
            if (projectile.timeLeft < 85)
            {
                return new Color((int)(150f * ((float)projectile.timeLeft / 85f)), 0, 0, projectile.timeLeft / 5 * 3);
            }
            return new Color(150, 0, 0, 50);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityGlobalProjectile.DrawCenteredAndAfterimage(projectile, lightColor, ProjectileID.Sets.TrailingMode[projectile.type], 1);
            return false;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            if (target.type == NPCID.TargetDummy)
                return;

            float heal = (float)damage * 0.05f;
            if ((int)heal == 0)
                return;
            if (Main.player[Main.myPlayer].lifeSteal <= 0f)
                return;

			CalamityGlobalProjectile.SpawnLifeStealProjectile(projectile, Main.player[projectile.owner], heal, ProjectileID.VampireHeal, 1200f, 1.5f);
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            float heal = (float)damage * 0.05f;
            if ((int)heal == 0)
                return;
            if (Main.player[Main.myPlayer].lifeSteal <= 0f)
                return;

			CalamityGlobalProjectile.SpawnLifeStealProjectile(projectile, Main.player[projectile.owner], heal, ProjectileID.VampireHeal, 1200f, 1.5f);
        }
    }
}
