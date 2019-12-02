﻿using CalamityMod.Items.Weapons.Melee;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Melee
{
    public class GaelSkull2 : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ark of the Cosmos' Superior Cousin");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
            Main.projFrames[projectile.type] = 5;
        }

        public override void SetDefaults()
        {
            projectile.width = 16;
            projectile.height = 16;
            projectile.friendly = true;
            projectile.melee = true;
            projectile.ignoreWater = true;
            projectile.penetrate = 2;
            projectile.timeLeft = 600;
            projectile.light = 1f;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = GaelsGreatsword.ImmunityFrames;
        }

        public override void AI()
        {
            //Rotation

            if (projectile.velocity.X < 0f)
            {
                projectile.spriteDirection = -1;
                projectile.rotation = (-projectile.velocity).ToRotation();
            }
            else
            {
                projectile.spriteDirection = 1;
                projectile.rotation = projectile.velocity.ToRotation();
            }

            projectile.ai[0] += 1f;
            if (projectile.localAI[1] == 1f && projectile.ai[0] < 35f)
            {
                projectile.velocity.Y += 0.5f;
                return;
            }

            //Targeting

            NPC target = projectile.Center.ClosestNPCAt(GaelsGreatsword.SearchDistance);
            projectile.tileCollide = target != null; //Go through walls if we're hunting an NPC
            if (target != null)
            {
                float velocityMagnitude = projectile.velocity.Length();
                Vector2 distanceNorm = projectile.DirectionTo(target.Center) * velocityMagnitude * (projectile.Distance(target.Center) < 220f ? 1.3f : 1f);
                float inverseTurnSpeed = (projectile.Distance(target.Center) < 240f ? 4f : 13f);
                projectile.velocity = (projectile.velocity * inverseTurnSpeed + distanceNorm) / (inverseTurnSpeed + 0.4f);
                projectile.velocity.Normalize();
                projectile.velocity *= velocityMagnitude;
            }

            //Dust circle

            if (projectile.ai[0] % 20 == 0f)
            {
                for (int l = 0; l < 14; l++)
                {
                    Vector2 spawnPositionAdditive = Vector2.UnitX * (float)-(float)projectile.width / 2f;
                    spawnPositionAdditive += -Vector2.UnitY.RotatedBy((double)((float)l * MathHelper.TwoPi / 14f), default) * new Vector2(8f, 16f) * projectile.scale;
                    spawnPositionAdditive = spawnPositionAdditive.RotatedBy((double)(projectile.rotation), default);
                    int dustIndex = Dust.NewDust(projectile.Center, 0, 0, 218, 0f, 0f, 0, new Color(188, 126, 154), 1.5f);
                    Main.dust[dustIndex].noGravity = true;
                    Main.dust[dustIndex].position = projectile.Center + spawnPositionAdditive;
                    Main.dust[dustIndex].velocity = projectile.velocity * 0.1f;
                    Main.dust[dustIndex].velocity = Vector2.Normalize(projectile.Center - projectile.velocity * 3f - Main.dust[dustIndex].position) * 1.25f;
                }
            }

            //Frame logic

            projectile.frameCounter++;
            if (projectile.frameCounter % 5 == 0)
            {
                projectile.frame++;
            }
            if (projectile.frame >= Main.projFrames[projectile.type])
            {
                projectile.frame = 0;
            }
            if (projectile.alpha > 0)
            {
                projectile.alpha -= 15;
                if (projectile.alpha < 0)
                {
                    projectile.alpha = 0;
                }
            }
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
			CalamityGlobalProjectile.DrawCenteredAndAfterimage(projectile, lightColor, ProjectileID.Sets.TrailingMode[projectile.type], 1);
            return false;
        }

        public override void Kill(int timeLeft)
        {
            projectile.position = projectile.Center;
            projectile.width = projectile.height = 240;
            projectile.position.X = projectile.position.X - (float)(projectile.width / 2);
            projectile.position.Y = projectile.position.Y - (float)(projectile.height / 2);
            projectile.maxPenetrate = -1;
            projectile.penetrate = -1;
            projectile.Damage();
            Main.PlaySound((int)SoundType.NPCKilled, (int)projectile.Center.X, (int)projectile.Center.Y, 52, 0.4f);
            for (int i = 0; i < 3; i++)
            {
                Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 218, 0f, 0f, 100, default, 1.5f);
            }
            for (int i = 0; i < 30; i++)
            {
                float angle = MathHelper.TwoPi * i / 30f;
                int dustIndex = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 218, 0f, 0f, 0, default, 2.5f);
                Main.dust[dustIndex].noGravity = true;
                Main.dust[dustIndex].velocity *= 3f;
                dustIndex = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 218, 0f, 0f, 100, default, 1.5f);
                Main.dust[dustIndex].velocity *= 2f;
                Main.dust[dustIndex].noGravity = true;
                Dust.NewDust(projectile.Center + angle.ToRotationVector2() * 160f, 0, 0, 218, 0f, 0f, 100, default, 1.5f);
            }
        }
    }
}
