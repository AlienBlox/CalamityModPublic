﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Particles
{
    public static class GeneralParticleHandler
    {
        private static List<Particle> particles;
        //List containing the particles to delete
        private static List<Particle> particlesToKill;
        //Static list for details concerning every particle type
        private static Dictionary<Type, int> particleTypes;
        private static Dictionary<int, Texture2D> particleTextures;
        private static List<Particle> particleInstances;
        //Lists used when drawing particles batched
        private static List<Particle> batchedAlphaBlendParticles;
        private static List<Particle> batchedNonPremultipliedParticles;
        private static List<Particle> batchedAdditiveBlendParticles;


        internal static void Load()
        {
            particles = new List<Particle>();
            particlesToKill = new List<Particle>();
            particleTypes = new Dictionary<Type, int>();
            particleTextures = new Dictionary<int, Texture2D>();
            particleInstances = new List<Particle>();

            batchedAlphaBlendParticles = new List<Particle>();
            batchedNonPremultipliedParticles = new List<Particle>();
            batchedAdditiveBlendParticles = new List<Particle>();

            Type baseParticleType = typeof(Particle);
            CalamityMod calamity = ModContent.GetInstance<CalamityMod>();

            foreach (Type type in calamity.Code.GetTypes())
            {
                if (type.IsSubclassOf(baseParticleType) && !type.IsAbstract && type != baseParticleType)
                {
                    int ID = particleTypes.Count; //Get the ID of the particle
                    particleTypes[type] = ID;

                    Particle instance = (Particle)FormatterServices.GetUninitializedObject(type);
                    particleInstances.Add(instance);

                    string texturePath = type.Namespace.Replace('.', '/') + "/" + type.Name;
                    if (instance.Texture != "")
                        texturePath = instance.Texture;
                    particleTextures[ID] = ModContent.GetTexture(texturePath);
                }
            }
        }

        internal static void Unload()
        {
            particles = null;
            particlesToKill = null;
            particleTypes = null;
            particleTextures = null;
            particleInstances = null;
            batchedAlphaBlendParticles = null;
            batchedNonPremultipliedParticles = null;
            batchedAdditiveBlendParticles = null;
        }

        /// <summary>
		/// Spawns the particle instance provided into the world. If the particle limit is reached but the particle is marked as important, it will try to replace a non important particle.
		/// </summary>
		public static void SpawnParticle(Particle particle)
        {
            if (particles.Count >= CalamityConfig.Instance.ParticleLimit && !particle.Important)
                return;

            particles.Add(particle);
            particle.Type = particleTypes[particle.GetType()];
        }

        public static void Update()
        {
            foreach (Particle particle in particles)
            {
                if (particle == null)
                    continue;
                particle.Position += particle.Velocity;
                particle.Time++;
                particle.Update();
            }
            //Clear out particles whose time is up
            particles.RemoveAll(particle => (particle.Time >= particle.Lifetime && particle.SetLifetime) || particlesToKill.Contains(particle));
            particlesToKill.Clear();
        }

        public static void RemoveParticle(Particle particle)
        {
            particlesToKill.Add(particle);
        }

        public static void DrawAllParticles(SpriteBatch sb)
        {
            //Batch the particles to avoid constant restarting of the spritebatch
            foreach (Particle particle in particles)
            {
                if (particle == null)
                    continue;

                if (particle.UseAdditiveBlend)
                    batchedAdditiveBlendParticles.Add(particle);
                else if (particle.UseHalfTransparency)
                    batchedNonPremultipliedParticles.Add(particle);
                else
                    batchedAlphaBlendParticles.Add(particle);
            }

            foreach (Particle particle in batchedAlphaBlendParticles)
            {
                if (particle.UseCustomDraw)
                    particle.CustomDraw(sb);
                else
                {
                    Rectangle frame = particleTextures[particle.Type].Frame(1, particle.FrameVariants, 0, particle.Variant);
                    sb.Draw(particleTextures[particle.Type], particle.Position - Main.screenPosition, frame, particle.Color, particle.Rotation, frame.Size() * 0.5f, particle.Scale, SpriteEffects.None, 0f);
                }
            }

            if (batchedNonPremultipliedParticles.Count > 0)
            {
                sb.End();
                sb.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointClamp, DepthStencilState.Default, null, null, Main.GameViewMatrix.ZoomMatrix);

                foreach (Particle particle in batchedNonPremultipliedParticles)
                {
                    if (particle.UseCustomDraw)
                        particle.CustomDraw(sb);
                    else
                    {
                        Rectangle frame = particleTextures[particle.Type].Frame(1, particle.FrameVariants, 0, particle.Variant);
                        sb.Draw(particleTextures[particle.Type], particle.Position - Main.screenPosition, frame, particle.Color, particle.Rotation, frame.Size() * 0.5f, particle.Scale, SpriteEffects.None, 0f);
                    }
                }
            }

            if (batchedAdditiveBlendParticles.Count > 0)
            {
                sb.End();
                sb.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.PointClamp, DepthStencilState.Default, null, null, Main.GameViewMatrix.ZoomMatrix);

                foreach (Particle particle in batchedAdditiveBlendParticles)
                {
                    if (particle.UseCustomDraw)
                        particle.CustomDraw(sb);
                    else
                    {
                        Rectangle frame = particleTextures[particle.Type].Frame(1, particle.FrameVariants, 0, particle.Variant);
                        sb.Draw(particleTextures[particle.Type], particle.Position - Main.screenPosition, frame, particle.Color, particle.Rotation, frame.Size() * 0.5f, particle.Scale, SpriteEffects.None, 0f);
                    }
                }
            }

            //Return to normal if we swapped the blend state before
            if (batchedAdditiveBlendParticles.Count > 0 || batchedNonPremultipliedParticles.Count > 0)
            { 
                sb.End();
                sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
            }

            batchedAlphaBlendParticles.Clear();
            batchedNonPremultipliedParticles.Clear();
            batchedAdditiveBlendParticles.Clear();
        }

        /// <summary>
        /// Gives you the amount of particle slots that are available. Useful when you need multiple particles at once to make an effect and dont want it to be only halfway drawn due to a lack of particle slots
        /// </summary>
        /// <returns></returns>
        public static int FreeSpacesAvailable() => CalamityConfig.Instance.ParticleLimit - particles.Count();

        /// <summary>
        /// Gives you the texture of the particle type. Useful for custom drawing
        /// </summary>
        public static Texture2D GetTexture(int type) => particleTextures[type];

        private static string noteToEveryone = "This particle system was inspired by spirit mod's own particle system, with permission granted by Yuyutsu. Love you spirit mod! -Iban";
    }
}
