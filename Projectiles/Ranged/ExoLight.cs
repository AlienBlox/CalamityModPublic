﻿using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Buffs.StatDebuffs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;   
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Ranged
{
    public class ExoLight : ModProjectile
    {
        public float OffsetSpeed
        {
            get => projectile.ai[0];
            set => projectile.ai[0] = value;
        }
        public float OffsetRotation
        {
            get => projectile.ai[1];
            set => projectile.ai[1] = value;
        }
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Light");
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 20;
        }

        public override void SetDefaults()
        {
            projectile.width = projectile.height = 16;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.ranged = true;
            projectile.penetrate = -1;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 5;
            projectile.timeLeft = 180;
        }
        public override void AI()
        {
            if (projectile.localAI[0] == 0f)
            {
                OffsetRotation = Main.rand.NextFloat(MathHelper.TwoPi);
                OffsetSpeed = Main.rand.NextFloat(MathHelper.ToRadians(2.5f), MathHelper.ToRadians(4f)) * Main.rand.NextBool(2).ToDirectionInt();
                projectile.localAI[0] = 1f;
            }
            Projectile owner = Main.projectile[(int)projectile.localAI[1]];

            // Ensure the owner is the correct projectile.
            if (owner.type != ModContent.ProjectileType<ExoLightBurst>())
                projectile.Kill();

            // Movement around the owner.
            projectile.Center = owner.Center + OffsetRotation.ToRotationVector2() * (float)Math.Cos(OffsetRotation * 0.3f) * owner.Size * 0.5f;
            projectile.rotation = (projectile.position - projectile.oldPos[1]).ToRotation();
            OffsetRotation += OffsetSpeed;
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D lightTexture = ModContent.GetTexture("CalamityMod/ExtraTextures/PhotovisceratorLight");
            for (int i = 0; i < projectile.oldPos.Length; i++)
            {
                float colorInterpolation = (float)Math.Cos(projectile.timeLeft / 16f + Main.GlobalTime / 20f + i / (float)projectile.oldPos.Length * MathHelper.Pi) * 0.5f + 0.5f;
                Color color = Color.Lerp(Color.LightGreen, Color.LightPink, colorInterpolation) * 0.4f;
                color.A = 0;
                Vector2 drawPosition = projectile.oldPos[i] + lightTexture.Size() * 0.5f - Main.screenPosition + new Vector2(0f, projectile.gfxOffY);
                Color outerColor = color;
                Color innerColor = color * 0.5f;
                float intensity = 0.9f + 0.15f * (float)Math.Cos(Main.GlobalTime % 60f * MathHelper.TwoPi);

                // Become smaller the futher along the old positions we are.
                intensity *= MathHelper.Lerp(0.15f, 1f, 1f - i / (float)projectile.oldPos.Length);

                Vector2 outerScale = new Vector2(1.65f) * intensity;
                Vector2 innerScale = new Vector2(1.65f) * intensity * 0.7f;
                outerColor *= intensity;
                innerColor *= intensity;
                spriteBatch.Draw(lightTexture, drawPosition, null, outerColor, 0f, lightTexture.Size() * 0.5f, outerScale * 0.6f, SpriteEffects.None, 0);
                spriteBatch.Draw(lightTexture, drawPosition, null, innerColor, 0f, lightTexture.Size() * 0.5f, innerScale * 0.6f, SpriteEffects.None, 0);
            }
            return false;
        }
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<ExoFreeze>(), 60);
            target.AddBuff(ModContent.BuffType<BrimstoneFlames>(), 180);
            target.AddBuff(ModContent.BuffType<GlacialState>(), 180);
            target.AddBuff(ModContent.BuffType<Plague>(), 180);
            target.AddBuff(ModContent.BuffType<HolyFlames>(), 180);
            target.AddBuff(BuffID.CursedInferno, 180);
            target.AddBuff(BuffID.Frostburn, 180);
            target.AddBuff(BuffID.OnFire, 180);
            target.AddBuff(BuffID.Ichor, 180);
        }
    }
}
