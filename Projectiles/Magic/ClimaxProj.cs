﻿using CalamityMod.Items.Weapons.Magic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Magic
{
    public class ClimaxProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Voltaic Orb");
            Main.projFrames[Projectile.type] = 5;
        }

        public override void SetDefaults()
        {
            Projectile.width = 38;
            Projectile.height = 38;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 48;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.ignoreWater = true;
        }

        public override void AI()
        {
            // Rapidly screech to a halt once spawned.
            Projectile.velocity *= 0.86f;

            // Spin chaotically given a pre-defined spin direction. Choose one initially at random.
            float spinTheta = 0.11f;
            if (Projectile.localAI[0] == 0f)
                Projectile.localAI[0] = Main.rand.NextBool() ? -spinTheta : spinTheta;
            Projectile.rotation += Projectile.localAI[0];

            // Animate the lightning orb.
            Projectile.frameCounter++;
            if (Projectile.frameCounter > 6)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
                if (Projectile.frame > 4)
                {
                    Projectile.frame = 0;
                }
            }

            // Initial stagger in frames needs to be skipped over before it starts shooting,
            // but once it's past that then it can fire constantly
            --Projectile.ai[0];
            if (Projectile.ai[0] < 0f)
                CalamityUtils.MagnetSphereHitscan(Projectile, 400f, 8f, VoltaicClimax.OrbFireRate, 2, ModContent.ProjectileType<ClimaxBeam>(), 1D, true);
        }

        public override Color? GetAlpha(Color lightColor)
        {
            if (Projectile.timeLeft < 30)
            {
                float num7 = Projectile.timeLeft / 30f;
                Projectile.alpha = (int)(255f - 255f * num7);
            }
            return new Color(255 - Projectile.alpha, 255 - Projectile.alpha, 255 - Projectile.alpha, 0);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture2D13 = ModContent.Request<Texture2D>(Texture).Value;
            int num214 = ModContent.Request<Texture2D>(Texture).Value.Height / Main.projFrames[Projectile.type];
            int y6 = num214 * Projectile.frame;
            Main.spriteBatch.Draw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(new Rectangle(0, y6, texture2D13.Width, num214)), Projectile.GetAlpha(lightColor), Projectile.rotation, new Vector2(texture2D13.Width / 2f, num214 / 2f), Projectile.scale, SpriteEffects.None, 0);
            return false;
        }

        public override bool? CanDamage() => false;
    }
}
