﻿using CalamityMod.Items;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Projectiles
{
    public class VoidEssence : ModProjectile
    {
        private const int NumAnimationFrames = 4;
        private const int AnimationFrameTime = 12;
        private const float TentacleRange = 140f;
        private const float TentacleCooldown = 25f;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Void Essence");
            Main.projFrames[projectile.type] = NumAnimationFrames;
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            projectile.height = 24;
            projectile.width = 24;
            projectile.melee = true;
            projectile.timeLeft = 100;
            projectile.friendly = true;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.alpha = 80;

            projectile.penetrate = 8;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 4;
            projectile.extraUpdates = 1;
        }

        public override void AI()
        {
            projectile.rotation = projectile.velocity.ToRotation() + MathHelper.PiOver2;
            drawOffsetX = 1;
            drawOriginOffsetY = 4;

            // Update animation
            projectile.frameCounter++;
            if (projectile.frameCounter > AnimationFrameTime)
            {
                projectile.frame++;
                projectile.frameCounter = 0;
            }
            if (projectile.frame >= NumAnimationFrames)
                projectile.frame = 0;

            // Produce light
            Lighting.AddLight(projectile.Center, 0.9f, 0.9f, 1.0f);

            // Continuously trail dust
            int trailDust = 1;
            for (int i = 0; i < trailDust; ++i)
            {
                int dustID = Main.rand.NextBool(8) ? 66 : 143;

                int idx = Dust.NewDust(projectile.position - projectile.velocity, projectile.width, projectile.height, dustID);
                Main.dust[idx].noGravity = true;
                Main.dust[idx].velocity += projectile.velocity * 0.8f;
            }

            // If tentacle is currently on cooldown, reduce the cooldown.
            if (projectile.ai[0] > 0f)
                projectile.ai[0] -= 1f;

            // Home in on nearby enemies if homing is enabled
            if (projectile.ai[1] == 0f)
                HomingAI();

            // Fade out if about to despawn
            if (projectile.timeLeft <= 40 && projectile.timeLeft % 5 == 0)
            {
                projectile.alpha += 20;
                if (projectile.alpha > 255)
                    projectile.alpha = 255;
            }
        }

        private void HomingAI()
        {
            // Find the closest NPC within range.
            int targetIdx = -1;
            float maxHomingRange = 400f;
            bool hasHomingTarget = false;
            for (int i = 0; i < Main.npc.Length; ++i)
            {
                NPC npc = Main.npc[i];
                if (npc == null || !npc.active)
                    continue;

                // Won't home in through walls and won't chase invulnerable targets.
                if (npc.CanBeChasedBy(projectile, false) && Collision.CanHit(projectile.Center, 1, 1, npc.Center, 1, 1))
                {
                    float dist = (projectile.Center - npc.Center).Length();
                    if (dist < maxHomingRange)
                    {
                        targetIdx = i;
                        maxHomingRange = dist;
                        hasHomingTarget = true;
                    }
                }
            }

            // Home in on said closest NPC.
            if (hasHomingTarget)
            {
                NPC target = Main.npc[targetIdx];
                Vector2 homingVector = (target.Center - projectile.Center).SafeNormalize(Vector2.Zero) * Nadir.ShootSpeed;
                float homingRatio = 35f;
                projectile.velocity = (projectile.velocity * homingRatio + homingVector) / (homingRatio + 1f);

                // If the target is close enough and tentacle is off cooldown, summon one.
                // maxHomingRange doubles as the distance to the target.
                if (projectile.ai[0] <= 0f && maxHomingRange <= TentacleRange)
                {
                    Vector2 projVel = (target.Center - projectile.Center).SafeNormalize(Vector2.Zero);
                    projVel *= 6f;
                    SpawnTentacle(projVel);
                    projectile.ai[0] = TentacleCooldown;
                }
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityGlobalProjectile.DrawCenteredAndAfterimage(projectile, lightColor, ProjectileID.Sets.TrailingMode[projectile.type], 1);
            return false;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            // Rapidly screech to a halt upon touching an enemy and disable homing.
            projectile.velocity *= 0.4f;
            projectile.ai[1] = 1f;

            // Fade out a bit with every hit
            projectile.alpha += 20;
            if (projectile.alpha > 255)
                projectile.alpha = 255;

            // Explode into dust (as if being shredded apart on contact)
            int onHitDust = Main.rand.Next(6, 11);
            for (int i = 0; i < onHitDust; ++i)
            {
                int dustID = Main.rand.NextBool() ? 198 : 199;
                int idx = Dust.NewDust(projectile.position, projectile.width, projectile.height, dustID, 0f, 0f);

                Main.dust[idx].noGravity = true;
                float speed = Main.rand.NextFloat(1.4f, 2.6f);
                Main.dust[idx].velocity *= speed;
                float scale = Main.rand.NextFloat(1.0f, 1.8f);
                Main.dust[idx].scale = scale;
            }
        }

        public override void Kill(int timeLeft)
        {
            // Create a burst of dust
            int killDust = Main.rand.Next(30, 41);
            for (int i = 0; i < killDust; ++i)
            {
                int dustID = Main.rand.NextBool() ? 198 : 199;
                int idx = Dust.NewDust(projectile.position, projectile.width, projectile.height, dustID, 0f, 0f);

                Main.dust[idx].noGravity = true;
                float speed = Main.rand.NextFloat(2.0f, 3.1f);
                Main.dust[idx].velocity *= speed;
                float scale = Main.rand.NextFloat(1.0f, 1.8f);
                Main.dust[idx].scale = scale;
            }

            // Spawn three tentacles pointing in random directions
            for (int i = 0; i < 3; ++i)
            {
                Vector2 projVel = Vector2.One.RotatedByRandom(MathHelper.TwoPi);
                projVel *= 4f;
                SpawnTentacle(projVel);
            }
        }

        private void SpawnTentacle(Vector2 tentacleVelocity)
        {
            int damage = Nadir.BaseDamage;
            float kb = 7f;

            // Randomize tentacle behavior variables
            float ai0 = Main.rand.NextFloat(0.01f, 0.08f);
            ai0 *= Main.rand.NextBool() ? -1f : 1f;
            float ai1 = Main.rand.NextFloat(0.01f, 0.08f);
            ai1 *= Main.rand.NextBool() ? -1f : 1f;

            if (projectile.owner == Main.myPlayer)
                Projectile.NewProjectile(projectile.Center, tentacleVelocity, ModContent.ProjectileType<VoidTentacle>(), damage, kb, projectile.owner, ai0, ai1);
        }
    }
}
