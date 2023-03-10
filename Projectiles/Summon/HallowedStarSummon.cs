﻿using CalamityMod.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Summon
{
    public class HallowedStarSummon : ModProjectile
    {
        private int noTileHitCounter = 120;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Star");
            ProjectileID.Sets.MinionShot[Projectile.type] = true;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = 24;
            Projectile.height = 24;
            Projectile.friendly = true;
            Projectile.alpha = 50;
            Projectile.penetrate = 1;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Summon;
        }

        public override void AI()
        {
            int randomToSubtract = Main.rand.Next(1, 3);
            noTileHitCounter -= randomToSubtract;
            if (noTileHitCounter == 0)
            {
                Projectile.tileCollide = true;
            }
            if (Projectile.soundDelay == 0)
            {
                Projectile.soundDelay = 20 + Main.rand.Next(40);
                if (Main.rand.NextBool(5))
                {
                    SoundEngine.PlaySound(SoundID.Item9, Projectile.position);
                }
            }
            Projectile.alpha -= 15;
            int alphaMin = 150;
            if (Projectile.Center.Y >= Projectile.ai[1])
            {
                alphaMin = 0;
            }
            if (Projectile.alpha < alphaMin)
            {
                Projectile.alpha = alphaMin;
            }
            Projectile.rotation += (Math.Abs(Projectile.velocity.X) + Math.Abs(Projectile.velocity.Y)) * 0.01f * Projectile.direction;
            if (Main.rand.NextBool(48) && Main.netMode != NetmodeID.Server)
            {
                int idx = Gore.NewGore(Projectile.GetSource_FromAI(), Projectile.Center, Projectile.velocity * 0.2f, 16, 1f);
                Main.gore[idx].velocity *= 0.66f;
                Main.gore[idx].velocity += Projectile.velocity * 0.3f;
            }
            if (Projectile.ai[1] == 1f)
            {
                Projectile.light = 0.9f;
                if (Main.rand.NextBool(10))
                {
                    Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<AstralOrange>(), Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f, 150, default, 1.2f);
                }
                if (Main.rand.NextBool(20) && Main.netMode != NetmodeID.Server)
                {
                    Gore.NewGore(Projectile.GetSource_FromAI(), Projectile.position, new Vector2(Projectile.velocity.X * 0.2f, Projectile.velocity.Y * 0.2f), Main.rand.Next(16, 18), 1f);
                }
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(200, 45, 250, Projectile.alpha);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 offsets = new Vector2(0f, Projectile.gfxOffY) - Main.screenPosition;
            Color alpha = Projectile.GetAlpha(lightColor);
            Rectangle spriteRec = new Microsoft.Xna.Framework.Rectangle(0, 0, tex.Width, tex.Height);
            Vector2 spriteOrigin = spriteRec.Size() / 2f;
            SpriteEffects spriteEffects = Projectile.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            Texture2D aura = ModContent.Request<Texture2D>("CalamityMod/Projectiles/StarTrail").Value;
            Vector2 drawStart = Projectile.Center + Projectile.velocity;
            Vector2 drawStart2 = Projectile.Center - Projectile.velocity * 0.5f;
            Vector2 spinPoint = new Vector2(0f, -10f);
            float time = Main.player[Projectile.owner].miscCounter % 216000f / 60f;
            Rectangle auraRec = aura.Frame();
            Color purple = Color.Purple * 0.2f;
            Color white = Color.White * 0.5f;
            white.A = 0;
            purple.A = 0;
            Vector2 auraOrigin = new Vector2(auraRec.Width / 2f, 10f);

            //Draw the aura
            Main.EntitySpriteDraw(aura, drawStart + offsets + spinPoint.RotatedBy(MathHelper.TwoPi * time), auraRec, purple, Projectile.velocity.ToRotation() + MathHelper.PiOver2, auraOrigin, 1.5f, SpriteEffects.None, 0);
            Main.EntitySpriteDraw(aura, drawStart + offsets + spinPoint.RotatedBy(MathHelper.TwoPi * time + MathHelper.TwoPi / 3f), auraRec, purple, Projectile.velocity.ToRotation() + MathHelper.PiOver2, auraOrigin, 1.1f, SpriteEffects.None, 0);
            Main.EntitySpriteDraw(aura, drawStart + offsets + spinPoint.RotatedBy(MathHelper.TwoPi * time + MathHelper.Pi * 4f / 3f), auraRec, purple, Projectile.velocity.ToRotation() + MathHelper.PiOver2, auraOrigin, 1.3f, SpriteEffects.None, 0);
            for (float d = 0f; d < 1f; d += 0.5f)
            {
                float scaleMult = time % 0.5f / 0.5f;
                scaleMult = (scaleMult + d) % 1f;
                float colorMult = scaleMult * 2f;
                if (colorMult > 1f)
                {
                    colorMult = 2f - colorMult;
                }
                Main.EntitySpriteDraw(aura, drawStart2 + offsets, auraRec, white * colorMult, Projectile.velocity.ToRotation() + MathHelper.PiOver2, auraOrigin, 0.3f + scaleMult * 0.5f, SpriteEffects.None, 0);
            }

            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1);
            return false;
        }

        public override void Kill(int timeLeft)
        {
            Projectile.position += Projectile.Size;
            Projectile.width = 50;
            Projectile.height = 50;
            Projectile.position -= Projectile.Size;
            for (int i = 0; i < 5; i++)
            {
                int idx = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 16, 0f, 0f, 100, default, 1.2f);
                Main.dust[idx].velocity *= 3f;
                if (Main.rand.NextBool(2))
                {
                    Main.dust[idx].scale = 0.5f;
                    Main.dust[idx].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                }
            }
            if (Main.netMode != NetmodeID.Server)
            {
                for (int i = 0; i < 3; i++)
                {
                    Gore.NewGore(Projectile.GetSource_Death(), Projectile.position, Projectile.velocity * 0.05f, Main.rand.Next(16, 18), 1f);
                }
            }
        }
    }
}
