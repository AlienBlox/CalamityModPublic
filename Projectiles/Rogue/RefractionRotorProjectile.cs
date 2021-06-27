using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public class RefractionRotorProjectile : ModProjectile
    {
        public const int EnergyShotCount = 6;
        public override string Texture => "CalamityMod/Items/Weapons/Rogue/RefractionRotor";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Prism Shuriken");
            ProjectileID.Sets.TrailingMode[projectile.type] = 2;
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 16;
        }

        public override void SetDefaults()
        {
            projectile.width = 142;
            projectile.height = 126;
            projectile.friendly = true;
            projectile.penetrate = 10;
            projectile.timeLeft = 300;
            projectile.alpha = 255;
            projectile.tileCollide = false;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 10;
            projectile.Calamity().rogue = true;
        }

        public override void AI()
        {
            projectile.alpha = Utils.Clamp(projectile.alpha - 18, 0, 255);
            projectile.rotation += projectile.velocity.Length() * Math.Sign(projectile.velocity.X) * 0.036f;
        }

        public override bool CanDamage() => projectile.alpha <= 128;

		public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
		{
            if (projectile.timeLeft > 20)
                projectile.timeLeft = 20;
		}

		public override void Kill(int timeLeft)
        {
            // Release a puff of rainbow dust and some blades.
            // If this projectile is a stealth strike, don't create the blades as a gore-- create them as a projectile instead.
            if (!Main.dedServ)
			{
                for (int i = 0; i < 80; i++)
                {
                    Dust rainbowBurst = Dust.NewDustPerfect(projectile.Center, 267);
                    rainbowBurst.color = Main.hslToRgb(i / 80f, 0.9f, 0.6f);
                    rainbowBurst.velocity = Main.rand.NextVector2Unit() * Main.rand.NextFloat(3f, 5.5f);
                    rainbowBurst.scale = Main.rand.NextFloat(1.4f, 2.4f);
                    rainbowBurst.fadeIn = Main.rand.NextFloat(0.8f, 1.6f);
                    rainbowBurst.noGravity = true;
                }

                if (!projectile.Calamity().stealthStrike)
                {
                    int goreType = mod.GetGoreSlot("Gores/PrismShurikenBlade");
                    for (int i = 0; i < 6; i++)
                    {
                        Vector2 shootDirection = (MathHelper.TwoPi * i / 6f + projectile.rotation + MathHelper.PiOver2).ToRotationVector2();
                        Vector2 spawnPosition = projectile.Center + projectile.Size * 0.5f * projectile.scale * shootDirection * 0.85f;
                        if (!WorldGen.SolidTile((int)spawnPosition.X / 16, (int)spawnPosition.Y / 16))
                            Gore.NewGorePerfect(spawnPosition, projectile.velocity * 0.5f + shootDirection * 7f, goreType, projectile.scale);
                    }
                }
			}

            int shootType = ModContent.ProjectileType<PrismRocket>();
            if (Main.myPlayer != projectile.owner)
                return;

            // Release a circle of damaging blades if this projectile is a stealth strike.
            if (projectile.Calamity().stealthStrike)
            {
                int bladeType = ModContent.ProjectileType<PrismShurikenBlade>();
                for (int i = 0; i < 6; i++)
                {
                    Vector2 shootDirection = (MathHelper.TwoPi * i / 6f + projectile.rotation + MathHelper.PiOver2).ToRotationVector2();
                    Vector2 spawnPosition = projectile.Center + projectile.Size * 0.5f * projectile.scale * shootDirection * 0.85f;
                    Projectile.NewProjectile(spawnPosition, projectile.velocity * 0.5f + shootDirection * 7f, bladeType, projectile.damage, projectile.knockBack, projectile.owner);
                }
            }

            if (CalamityUtils.CountProjectiles(shootType) > 24)
                return;

            int energyDamage = (int)(projectile.damage * 0.66);
            float baseDirectionRotation = Main.rand.NextFloat(MathHelper.TwoPi);
            for (int i = 0; i < EnergyShotCount; i++)
            {
                Vector2 shootVelocity = (MathHelper.TwoPi * i / EnergyShotCount + baseDirectionRotation).ToRotationVector2() * 9f;
                Projectile.NewProjectile(projectile.Center + shootVelocity, shootVelocity, shootType, energyDamage, projectile.knockBack, projectile.owner);
            }
		}

		public override void PostDraw(SpriteBatch spriteBatch, Color lightColor)
		{
            Texture2D glowmask = ModContent.GetTexture("CalamityMod/Projectiles/Rogue/RefractionRotorGlowmask");
            Vector2 drawPosition = projectile.Center - Main.screenPosition + Vector2.UnitY * projectile.gfxOffY;
            Vector2 origin = glowmask.Size() * 0.5f;
            spriteBatch.Draw(glowmask, drawPosition, null, projectile.GetAlpha(Color.White), projectile.rotation, origin, projectile.scale, SpriteEffects.None, 0f);
        }
	}
}
