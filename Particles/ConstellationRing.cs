﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Particles
{
    public class ConstellationRingVFX : Particle //Also check out Split mod!
    {
        public override string Texture => "CalamityMod/Particles/HollowCircleSoftEdge";
        public override bool UseAdditiveBlend => true;
        public override bool UseCustomDraw => true;
        public override bool SetLifetime => true;
        public override bool Important => NeededVisual;

        public Vector2 Squish;
        public int StarAmount;
        public float StarScale;
        public float SpinSpeed;
        public bool NeededVisual;
        public float Offset;
        public float Opacity;


        public ConstellationRingVFX(Vector2 position, Color color, float rotation, float scale, Vector2 squish, float opacity = 1f, int starAmount = 3, float starScale = 2f, float spinSpeed = 0.05f, bool important = false)
        {
            Position = position;
            Velocity = Vector2.Zero;
            Rotation = rotation;
            Color = color;
            Scale = scale;
            Squish = squish;
            Opacity = opacity;
            StarAmount = starAmount;
            StarScale = starScale;
            SpinSpeed = spinSpeed;
            NeededVisual = important;
            Lifetime = 2;
            Offset = Main.rand.NextFloat(0f, MathHelper.TwoPi);
        }

        public override void CustomDraw(SpriteBatch spriteBatch)
        {
            Texture2D ringTexture = GeneralParticleHandler.GetTexture(Type);
            Texture2D starTexture = ModContent.GetTexture("CalamityMod/Particles/Sparkle");
            Texture2D bloomTexture = ModContent.GetTexture("CalamityMod/Particles/BloomCircle");

            spriteBatch.Draw(ringTexture, Position - Main.screenPosition, null, Color * Opacity, Rotation, ringTexture.Size() / 2f, Squish * Scale, SpriteEffects.None, 0);

            for (int i = 0; i < StarAmount; i++)
            {

                float starHeight = (float)Math.Sin(Offset + Main.GlobalTime * SpinSpeed + i * MathHelper.TwoPi / (float)StarAmount);
                float starWidth = (float)Math.Cos(Offset + Main.GlobalTime * SpinSpeed + i * MathHelper.TwoPi / (float)StarAmount);

                Vector2 starPos = Position + Rotation.ToRotationVector2() * starWidth * (Squish.X * Scale * ringTexture.Width * 0.45f) + (Rotation + MathHelper.PiOver2).ToRotationVector2() * starHeight * (Squish.Y * Scale * ringTexture.Height * 0.45f);

                //Ajust the bloom's texture to be the same size as the star's
                float properBloomSize = (float)starTexture.Height / (float)bloomTexture.Height;
                spriteBatch.Draw(bloomTexture, starPos - Main.screenPosition, null, Color * Opacity * 0.5f, 0, bloomTexture.Size() / 2f, Scale * properBloomSize, SpriteEffects.None, 0);
                spriteBatch.Draw(starTexture, starPos - Main.screenPosition, null, Color * Opacity * 0.5f, Rotation + MathHelper.PiOver4 + MathHelper.PiOver4 * i, starTexture.Size() / 2f, Scale * 0.75f, SpriteEffects.None, 0);
                spriteBatch.Draw(starTexture, starPos - Main.screenPosition, null, Color.White * Opacity, Rotation + MathHelper.PiOver4 * i, starTexture.Size() / 2f, Scale, SpriteEffects.None, 0);
            }

        }
    }
}
