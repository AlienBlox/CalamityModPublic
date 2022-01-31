﻿using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.CalPlayer;
using CalamityMod.DataStructures;
using CalamityMod.Particles;
using CalamityMod.Items.Weapons.Melee;
using Terraria.Graphics.Shaders;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;
using static CalamityMod.CalamityUtils;


namespace CalamityMod.Projectiles.Melee
{
    public class AncientBeam : ModProjectile //The boring plain one
    {
        public override string Texture => "CalamityMod/Items/Weapons/Melee/ArkoftheAncientsGlow";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ancient Beam");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 5;
            ProjectileID.Sets.TrailingMode[projectile.type] = 2;
        }

        const float MaxTime = 40;
        public float Timer => MaxTime - projectile.timeLeft;

        public override void SetDefaults()
        {
            projectile.width = projectile.height = 32;
            projectile.aiStyle = 27;
            aiType = ProjectileID.LightBeam;
            projectile.friendly = true;
            projectile.penetrate = 1;
            projectile.timeLeft = (int)MaxTime;
            projectile.melee = true;
            projectile.tileCollide = false;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float collisionPoint = 0f;
            float bladeLenght = 46f * projectile.scale;
            Vector2 start = -projectile.rotation.ToRotationVector2() * 8.5f;

            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), projectile.Center + start, projectile.Center + start + projectile.rotation.ToRotationVector2() * bladeLenght, 24, ref collisionPoint);
        }

        public override void AI()
        {
            if (projectile.timeLeft < MaxTime - 5)
                projectile.tileCollide = true;

            projectile.scale = 2.4f;
            projectile.Opacity = 0.6f;
            Lighting.AddLight(projectile.Center, 0.75f, 1f, 0.24f);

            projectile.velocity *= (1 - (float)Math.Pow(Timer / MaxTime, 3));

            if (Main.rand.NextBool(3))
            {
                int dustTrail = Dust.NewDust(projectile.Center, 14, 14, 66, projectile.velocity.X * 0.05f, projectile.velocity.Y * 0.05f, 150, new Color(Main.DiscoR, 100, 255), 1.2f);
                Main.dust[dustTrail].noGravity = true;
            }

            if (Main.rand.NextBool(3))
            {
                int dustType = Main.rand.Next(3);
                dustType = dustType == 0 ? 15 : dustType == 1 ? 57 : 58;

                Dust.NewDust(projectile.Center, 14, 14, dustType, projectile.velocity.X * 0.1f, projectile.velocity.Y * 0.1f, 150, default, 1.3f);
            }



            if (projectile.velocity.Length() < 1.0f)
                projectile.Kill();
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            if (projectile.timeLeft > 35)
                return false;

            DrawAfterimagesCentered(projectile, ProjectileID.Sets.TrailingMode[projectile.type], lightColor, 1);
            return false;
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.DD2_WitherBeastDeath, projectile.Center);
            for (int i = 0; i < 10; i++)
            {
                Vector2 particleSpeed = Main.rand.NextVector2CircularEdge(1, 1) * Main.rand.NextFloat(1.2f, 2.3f);
                Particle energyLeak = new SquishyLightParticle(projectile.Center + Utils.SafeNormalize(projectile.velocity, Vector2.Zero) * 40f, particleSpeed, Main.rand.NextFloat(0.3f, 0.6f), Color.Cyan, 60, 1, 1.5f, hueShift: 0.02f);
                GeneralParticleHandler.SpawnParticle(energyLeak);
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            Vector2 particleOrigin = target.Hitbox.Size().Length() < 140 ? target.Center : projectile.Center + projectile.rotation.ToRotationVector2() * 60f;
            for (int i = 0; i < 10; i++)
            {
                Vector2 particleSpeed = Main.rand.NextVector2CircularEdge(1, 1) * Main.rand.NextFloat(2.6f, 4f);
                Particle energyLeak = new SquishyLightParticle(particleOrigin, particleSpeed, Main.rand.NextFloat(0.3f, 0.6f), Color.Cyan, 60, 1, 1.5f, hueShift: 0.02f);
                GeneralParticleHandler.SpawnParticle(energyLeak);
            }
        }
    }
}