﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Hybrid
{
    public class NanoblackSplit : ModProjectile
    {
        private static int SpriteWidth = 52;
        private static int Lifetime = 90;
        private static float MaxRotationSpeed = 0.25f;
        private static float MaxSpeed = 22f;

        private static float HomingStartRange = 600f;
        private static float HomingBreakRange = 1000f;
        private static float HomingBonusRangeCap = 200f;
        private static float BaseHomingFactor = 1.6f;
        private static float MaxHomingFactor = 6.6f;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Nanoblack Blade");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 3;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            projectile.width = 32;
            projectile.height = 32;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.penetrate = 4;
            projectile.extraUpdates = 1;
            projectile.timeLeft = Lifetime;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 8;
        }

        // ai[0] = Index of current NPC target. If 0 or negative, the projectile has no target
        // ai[1] = Current spin speed. Negative speeds are also allowed.
        public override void AI()
        {
            drawOffsetX = -10;
            drawOriginOffsetY = 0;
            drawOriginOffsetX = 0;

            // On the very first frame, clear any invalid starting target variable and create some dust.
            // Also grab the damage type based on ai[0].
            if (projectile.timeLeft == Lifetime)
                SpawnDust();

            // Spin in the specified starting direction and slow down spin over time
            // Loses 1.66% of current speed every frame
            // Also update current orientation to reflect current spin direction
            float currentSpin = projectile.ai[1];
            projectile.direction = (currentSpin <= 0f) ? 1 : -1;
            projectile.spriteDirection = projectile.direction;
            projectile.rotation += currentSpin * MaxRotationSpeed;
            float spinReduction = 0.0166f * currentSpin;
            projectile.ai[1] -= spinReduction;

            // If about to disappear, shrink by 8% every frame
            if (projectile.timeLeft < 15)
                projectile.scale *= 0.92f;

            // Search for and home in on nearby targets
            HomingAI();
        }

        private void HomingAI()
        {
            // If we don't currently have a target, go try and get one!
            int targetID = (int)projectile.ai[0] - 1;
            if (targetID < 0)
                targetID = AcquireTarget();

            // Save the target, whether we have one or not.
            projectile.ai[0] = targetID + 1f;

            // If we don't have a target, then just slow down a bit.
            if (targetID < 0)
            {
                projectile.velocity *= 0.94f;
                return;
            }

            // Homing behavior depends on how far the blade is from its target.
            NPC target = Main.npc[targetID];
            float xDist = projectile.Center.X - target.Center.X;
            float yDist = projectile.Center.Y - target.Center.Y;
            float dist = (float)Math.Sqrt(xDist * xDist + yDist * yDist);

            // If the target is too far away, stop homing in on it.
            if (dist > HomingBreakRange)
            {
                projectile.ai[0] = 0f;
                return;
            }

            // Adds a multiple of the towards-target vector to its velocity every frame.
            float homingFactor = CalcHomingFactor(dist);
            Vector2 posDiff = target.Center - projectile.Center;
            posDiff = posDiff.SafeNormalize(Vector2.Zero);
            posDiff *= homingFactor;
            Vector2 newVelocity = projectile.velocity += posDiff;

            // Caps speed to make sure it doesn't go too fast.
            if (newVelocity.Length() >= MaxSpeed)
            {
                newVelocity = newVelocity.SafeNormalize(Vector2.Zero);
                newVelocity *= MaxSpeed;
            }

            projectile.velocity = newVelocity;
        }

        // Returns the ID of the NPC to be targeted by this energy blade.
        // It chooses the closest target which can be chased, ignoring invulnerable NPCs.
        // Nanoblack Blades prefer to target bosses whenever possible.
        private int AcquireTarget()
        {
            bool bossFound = false;
            int target = -1;
            float minDist = HomingStartRange;
            for (int i = 0; i < 200; ++i)
            {
                NPC npc = Main.npc[i];
                if (!npc.active || npc.type == NPCID.TargetDummy)
                    continue;

                // If we've found a valid boss target, ignore ALL targets which aren't bosses.
                if (bossFound && !npc.boss)
                    continue;

                if (npc.CanBeChasedBy(projectile, false))
                {
                    float xDist = projectile.Center.X - npc.Center.X;
                    float yDist = projectile.Center.Y - npc.Center.Y;
                    float distToNPC = (float)Math.Sqrt(xDist * xDist + yDist * yDist);
                    if (distToNPC < minDist)
                    {
                        // If this target within range is a boss, set the boss found flag.
                        if (npc.boss)
                            bossFound = true;
                        minDist = distToNPC;
                        target = i;
                    }
                }
            }
            return target;
        }

        // Energy blades home even more aggressively if they are very close to their target.
        private float CalcHomingFactor(float dist)
        {
            float baseFactor = BaseHomingFactor;
            float bonus = (MaxHomingFactor - BaseHomingFactor) * (1f - dist / HomingBonusRangeCap);
            if (bonus < 0f)
                bonus = 0f;
            return baseFactor + bonus;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityGlobalProjectile.DrawCenteredAndAfterimage(projectile, lightColor, ProjectileID.Sets.TrailingMode[projectile.type], 1);
            return false;
        }

        // Draws the energy blade's glowmask.
        public override void PostDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            float fWidthOverTwo = SpriteWidth / 2f;
            float fHeightOverTwo = projectile.height / 2f;

            // Make sure the glowmask matches the blade's own orientation
            SpriteEffects eff = SpriteEffects.None;
            if (projectile.spriteDirection == -1)
                eff = SpriteEffects.FlipHorizontally;
            Vector2 origin = new Vector2(fWidthOverTwo, fHeightOverTwo);
            spriteBatch.Draw(ModContent.GetTexture("CalamityMod/Projectiles/Hybrid/NanoblackSplitGlow"),
                projectile.Center - Main.screenPosition, null, Color.White, projectile.rotation,
                origin, projectile.scale, eff, 0f);
        }

        // Spawns a tiny bit of dust when the energy blade vanishes.
        public override void Kill(int timeLeft)
        {
            SpawnDust();
        }

        // Spawns a small bit of Luminite themed dust.
        private void SpawnDust()
        {
            int dustCount = Main.rand.Next(3, 6);
            Vector2 corner = projectile.position;
            for (int i = 0; i < dustCount; ++i)
            {
                int dustType = 229;
                float scale = 0.6f + Main.rand.NextFloat(0.4f);
                int idx = Dust.NewDust(corner, projectile.width, projectile.height, dustType);
                Main.dust[idx].noGravity = true;
                Main.dust[idx].velocity *= 3f;
                Main.dust[idx].scale = scale;
            }
        }
    }
}
