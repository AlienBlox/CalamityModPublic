﻿using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Particles;
using CalamityMod.Items.Weapons.Melee;
using Terraria.Graphics.Shaders;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;
using static CalamityMod.CalamityUtils;

namespace CalamityMod.Projectiles.Melee
{
    public class AndromedasStrideBoltSpawner : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";
        public Player Owner => Main.player[projectile.owner];
        public ref float Size => ref projectile.ai[1]; //Yes

        public float WaitTimer; //How long until the monoliths appears
        public Vector2 OriginDirection; //The direction of the original strike
        public float Facing; //The direction of the original strike

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Andromeda Shock");
        }
        public override void SetDefaults()
        {
            projectile.melee = true;
            projectile.width = projectile.height = 70;
            projectile.tileCollide = false;
            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.timeLeft = 30;
            projectile.hide = true;
        }

        public override bool CanDamage() => false;

        public override void AI()
        {
            if (projectile.velocity != Vector2.Zero)
            {
                SurfaceUp();
                projectile.rotation = projectile.velocity.ToRotation();
                projectile.velocity = Vector2.Zero;
            }

            if (WaitTimer > 0)
            {
                projectile.timeLeft = 30;
                WaitTimer--;
            }

            if (projectile.timeLeft == 29)
            {
                if (Size * 0.8 > 0.4 && Facing != 0)
                    SideSprouts(Facing, 150f, Size * 0.8f);
            }

            if (projectile.timeLeft < 29)
            {
                if (Main.rand.Next(3) == 0)
                {
                    Vector2 particleDirection = (projectile.rotation - MathHelper.PiOver4).ToRotationVector2();
                    Vector2 flyDirection = particleDirection.RotatedBy(Main.rand.NextFloat(-MathHelper.PiOver4 / 2f, MathHelper.PiOver4 / 2f)) * Main.rand.NextFloat(15f, 35f);

                    Particle smoke = new HeavySmokeParticle(projectile.Center, flyDirection, Color.Lerp(Color.MidnightBlue, Color.Indigo, (float)Math.Sin(Main.GlobalTime * 6f)), 30, Main.rand.NextFloat(0.4f, 1.3f) * projectile.scale, 0.8f, 0, false, 0, true);
                    GeneralParticleHandler.SpawnParticle(smoke);

                    if (Main.rand.Next(3) == 0)
                    {
                        Particle smokeGlow = new HeavySmokeParticle(projectile.Center, flyDirection, Color.Red, 20, Main.rand.NextFloat(0.1f, 0.7f) * projectile.scale, 0.8f, 0, true, 0.01f, true);
                        GeneralParticleHandler.SpawnParticle(smokeGlow);
                    }

                }
            }

            if (projectile.timeLeft == 2)
            {
                //New bolt
                if (Owner.whoAmI == Main.myPlayer)
                {
                    Projectile proj = Projectile.NewProjectileDirect(projectile.position, projectile.rotation.ToRotationVector2() * 18f, ProjectileType<GalaxiaBolt>(), projectile.damage, 10f, Owner.whoAmI, 0.75f, MathHelper.Pi / 25f);
                    proj.scale = Size * 3f;
                    proj.timeLeft = 50;
                }

                Vector2 particleDirection = (projectile.rotation - MathHelper.PiOver2).ToRotationVector2();
                Main.PlaySound(SoundID.DD2_EtherianPortalDryadTouch, projectile.Center);

                for (int i = 0; i < 8; i++)
                {
                    Vector2 hitPositionDisplace = particleDirection.RotatedBy(MathHelper.PiOver2) * Main.rand.NextFloat(0f, 10f);
                    Vector2 flyDirection = particleDirection.RotatedBy(Main.rand.NextFloat(-MathHelper.PiOver2, MathHelper.PiOver2)) * Main.rand.NextFloat(5f, 15f);

                    Particle smoke = new HeavySmokeParticle(projectile.Center + hitPositionDisplace, flyDirection, Color.Lerp(Color.MidnightBlue, Color.Indigo, (float)Math.Sin(Main.GlobalTime * 6f)), 30, Main.rand.NextFloat(1.6f, 2.2f) * projectile.scale, 0.8f, 0, false, 0, true);
                    GeneralParticleHandler.SpawnParticle(smoke);

                    if (Main.rand.Next(3) == 0)
                    {
                        Particle smokeGlow = new HeavySmokeParticle(projectile.Center + hitPositionDisplace, flyDirection, Color.Red, 20, Main.rand.NextFloat(1.4f, 1.7f) * projectile.scale, 0.8f, 0, true, 0.005f, true);
                        GeneralParticleHandler.SpawnParticle(smokeGlow);
                    }
                }
            }
        }

        //Go up to the "surface" so you're not stuck in the middle of the ground like a complete moron.
        public void SurfaceUp()
        {
            for (float i = 0; i < 40; i += 0.5f)
            {
                Vector2 positionToCheck = projectile.Center + projectile.velocity * i;
                if (!Main.tile[(int)(positionToCheck.X / 16), (int)(positionToCheck.Y / 16)].IsTileSolidGround())
                {
                    projectile.Center = projectile.Center + projectile.velocity * i;
                    return;
                }
            }
            projectile.Center = projectile.Center + projectile.velocity * 40f;
        }

        public bool SideSprouts(float facing, float distance, float projSize)
        {
            float widestAngle = 0f;
            float widestSurfaceAngle = 0f;
            bool validPositionFound = false;
            for (float i = 0f; i < 1; i += 1 / distance)
            {
                Vector2 positionToCheck = projectile.Center + OriginDirection.RotatedBy((i * MathHelper.PiOver2 + MathHelper.PiOver4) * facing) * distance;

                if (Main.tile[(int)(positionToCheck.X / 16), (int)(positionToCheck.Y / 16)].IsTileSolidGround())
                    widestAngle = i;

                else if (widestAngle != 0)
                {
                    validPositionFound = true;
                    widestSurfaceAngle = widestAngle;
                }
            }

            if (validPositionFound)
            {
                Vector2 projPosition = projectile.Center + OriginDirection.RotatedBy((widestSurfaceAngle * MathHelper.PiOver2 + MathHelper.PiOver4) * facing) * distance;
                Vector2 monolithRotation = OriginDirection.RotatedBy(Utils.AngleLerp(widestSurfaceAngle * -facing, 0f, projSize));
                Projectile proj = Projectile.NewProjectileDirect(projPosition, -monolithRotation, ProjectileType<AndromedasStrideBoltSpawner>(), projectile.damage, 10f, Owner.whoAmI, Main.rand.Next(4), projSize);
                if (proj.modProjectile is AndromedasStrideBoltSpawner spawner)
                {
                    spawner.WaitTimer = (float)Math.Sqrt(1.0 - Math.Pow(projSize - 1.0, 2)) * 3f;
                    spawner.OriginDirection = OriginDirection;
                    spawner.Facing = facing;
                }
            }

            return validPositionFound;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(WaitTimer);
            writer.Write(Facing);
            writer.WriteVector2(OriginDirection);

        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            WaitTimer = reader.ReadSingle();
            Facing = reader.ReadSingle();
            OriginDirection = reader.ReadVector2();
        }
    }
}