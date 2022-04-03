﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Summon
{
    public class LostSoulGold : ModProjectile
    {
        public ref float Time => ref Projectile.ai[0];
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Gold Soul");
            Main.projFrames[Projectile.type] = 4;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 38;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.ignoreWater = true;
        }

        public override void AI() => LostSoulGiant.DoSoulAI(Projectile, ref Time);

        public override Color? GetAlpha(Color lightColor)
        {
            Color baseColor = Color.Lerp(Color.DarkGoldenrod, Color.Gold, Projectile.identity % 5f / 5f);
            Color color = Color.Lerp(baseColor, Color.White, (float)Math.Cos(Main.GlobalTimeWrappedHourly * 2.7f) * 0.04f + 0.5f);
            color.A = 127;
            return color * Projectile.Opacity;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            SpriteEffects direction = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Rectangle frame = texture.Frame(1, Main.projFrames[Projectile.type], 0, Projectile.frame);
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < Projectile.oldPos.Length / 2; j++)
                {
                    float fade = (float)Math.Pow(1f - Utils.GetLerpValue(0f, Projectile.oldPos.Length / 2, j, true), 2D);
                    Color drawColor = Color.Lerp(Projectile.GetAlpha(lightColor), Color.White * Projectile.Opacity, j / Projectile.oldPos.Length) * fade;
                    Vector2 drawPosition = Projectile.oldPos[j] + Projectile.Size * 0.5f + (MathHelper.TwoPi * i / 4f).ToRotationVector2() * 0.8f - Main.screenPosition;
                    float rotation = Projectile.oldRot[j];

                    Main.spriteBatch.Draw(texture, drawPosition, frame, drawColor, rotation, frame.Size() * 0.5f, Projectile.scale, direction, 0f);
                }
            }

            return false;
        }

        public override void Kill(int timeLeft)
        {
            // Play a wraith death sound.
            SoundEngine.PlaySound(SoundID.NPCDeath52, Projectile.Center);

            if (Main.dedServ)
                return;

            for (int i = 0; i < 45; i++)
            {
                Dust ectoplasm = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(42f, 42f), 264);
                ectoplasm.velocity = Main.rand.NextVector2Circular(1.75f, 1.75f);
                ectoplasm.color = Projectile.GetAlpha(Color.White);
                ectoplasm.scale = 1.45f;
                ectoplasm.noGravity = true;
                ectoplasm.noLight = true;
            }
        }
    }
}
