using CalamityMod.DataStructures;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Magic
{
	public class RedirectingSoul : ModProjectile
    {
        public ref float BurstIntensity => ref projectile.ai[0];
        public ref float Time => ref projectile.ai[1];
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Soul");
            Main.projFrames[projectile.type] = 3;
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            projectile.width = projectile.height = 18;
            projectile.friendly = true;
            projectile.penetrate = 1;
            projectile.tileCollide = false;
            projectile.magic = true;
            projectile.ignoreWater = true;
        }

        public override void AI()
        {
            if (Time >= 20f)
            {
                NPC potentialTarget = projectile.Center.ClosestNPCAt(1250f, false);
                if (potentialTarget != null)
                    HomeInOnTarget(potentialTarget);

                float accelerationFactor = MathHelper.SmoothStep(1.03f, 1.015f, Utils.InverseLerp(6f, 24f, projectile.velocity.Length(), true));
                if (projectile.velocity.Length() < 21f)
                    projectile.velocity *= accelerationFactor;
            }
            projectile.frameCounter++;
            if (projectile.frameCounter % 4f == 3f)
                projectile.frame = (projectile.frame + 1) % Main.projFrames[projectile.type];
            projectile.rotation = projectile.velocity.ToRotation() - MathHelper.PiOver2;
            Time++;
        }

        public void HomeInOnTarget(NPC target)
        {
            float oldSpeed = projectile.velocity.Length();
            float delayFactor = Utils.InverseLerp(20f, 35f, Time, true);
            float homeSpeed = MathHelper.Lerp(0f, 0.075f, delayFactor);

            projectile.velocity = Vector2.Lerp(projectile.velocity, projectile.DirectionTo(target.Center) * 16f, homeSpeed);
            projectile.velocity = projectile.velocity.SafeNormalize(Vector2.UnitY) * oldSpeed;
            projectile.position += (target.Center - projectile.Center).SafeNormalize(Vector2.Zero) * MathHelper.Lerp(25f, 1f, Utils.InverseLerp(100f, 360f, projectile.Distance(target.Center), true)) * delayFactor;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            Color baseColor = Color.Lerp(Color.IndianRed, Color.DarkViolet, projectile.identity % 5f / 5f);
            Color color = Color.Lerp(baseColor, Color.Black, BurstIntensity * 0.5f + (float)Math.Cos(Main.GlobalTime * 2.7f) * 0.04f);
            color.A = 0;
            return color * projectile.Opacity;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture = Main.projectileTexture[projectile.type];
            Rectangle frame = texture.Frame(1, Main.projFrames[projectile.type], 0, projectile.frame);
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < projectile.oldPos.Length; j++)
                {
                    Color drawColor = projectile.GetAlpha(lightColor) * ((projectile.oldPos.Length - j) / (float)projectile.oldPos.Length);
                    Vector2 drawPosition = projectile.oldPos[j] + projectile.Size * 0.5f + (MathHelper.TwoPi * i / 4f).ToRotationVector2() * 3.5f - Main.screenPosition;
                    float rotation = projectile.oldRot[j];

                    spriteBatch.Draw(texture, drawPosition, frame, drawColor, rotation, frame.Size() * 0.5f, projectile.scale, SpriteEffects.None, 0f);
                }
            }

            return false;
        }

        public override void Kill(int timeLeft)
        {
            // Play a wraith death sound at max intensity and a dungeon spirit hit sound otherwise.
            Main.PlaySound(BurstIntensity >= 1f ? SoundID.NPCDeath52 : SoundID.NPCHit35, projectile.Center);

            // Make a ghost sound and explode into ectoplasmic energy.
            if (Main.dedServ)
                return;

            for (int i = 0; i < 45; i++)
            {
                Dust ectoplasm = Dust.NewDustPerfect(projectile.Center + Main.rand.NextVector2Circular(50f, 50f) * BurstIntensity, 264);
                ectoplasm.velocity = Main.rand.NextVector2Circular(2f, 2f);
                ectoplasm.color = projectile.GetAlpha(Color.White);
                ectoplasm.scale = MathHelper.Lerp(1f, 1.6f, BurstIntensity);
                ectoplasm.noGravity = true;
                ectoplasm.noLight = true;
            }
        }
    }
}
