﻿using Terraria.Graphics.Shaders;
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
using Terraria.Audio;

namespace CalamityMod.Projectiles.Melee
{
    public class SanguineFury : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/Melee/TrueBiomeBlade_SanguineFury";
        private bool initialized = false;
        Vector2 direction = Vector2.Zero;
        public ref float Shred => ref projectile.ai[0]; //How much the attack is, attacking
        public float ShredRatio => MathHelper.Clamp(Shred / (maxShred * 0.5f), 0f, 1f);
        public ref float PogoCooldown => ref projectile.ai[1]; //Cooldown for the pogo
        public ref float BounceTime => ref projectile.localAI[0];
        public Player Owner => Main.player[projectile.owner];
        public bool CanPogo => Owner.velocity.Y != 0 && PogoCooldown <= 0; //Only pogo when in the air and if the cooldown is zero
        private bool OwnerCanShoot => Owner.channel && !Owner.noItems && !Owner.CCed;

        public const float pogoStrenght = 16f; //How much the player gets pogoed up
        public const float maxShred = 500; //How much shred you get

        public Projectile Wheel;
        public bool Dashing;
        public Vector2 DashStart;


        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sanguine Fury");
        }
        public override void SetDefaults()
        {
            projectile.melee = true;
            projectile.width = projectile.height = 70;
            projectile.tileCollide = false;
            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.extraUpdates = 1;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = OmegaBiomeBlade.SuperPogoAttunement_LocalIFrames;
            projectile.timeLeft = OmegaBiomeBlade.SuperPogoAttunement_LocalIFrames;
        }

        public override bool CanDamage()
        {
            return projectile.timeLeft <= 2; //Prevent spam click abuse
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float collisionPoint = 0f;
            float bladeLenght = 130 * projectile.scale;
            float bladeWidth = 86 * projectile.scale;

            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Owner.Center, Owner.Center + (direction * bladeLenght), bladeWidth, ref collisionPoint);
        }

        public void Pogo()
        {
            if (CanPogo && Main.myPlayer == Owner.whoAmI)
            {
                Owner.velocity = -direction.SafeNormalize(Vector2.Zero) * pogoStrenght; //Bounce
                Owner.fallStart = (int)(Owner.position.Y / 16f);
                PogoCooldown = 30; //Cooldown
                Main.PlaySound(SoundID.DD2_MonkStaffGroundImpact, projectile.position);

                Vector2 hitPosition = Owner.Center + (direction * 100 * projectile.scale);
                BounceTime = 20f; //Used only for animation

                for (int i = 0; i < 8; i++)
                {
                    Vector2 hitPositionDisplace = direction.RotatedBy(MathHelper.PiOver2) * Main.rand.NextFloat(-10f, 10f);
                    Vector2 flyDirection = -direction.RotatedBy(Main.rand.NextFloat(-MathHelper.PiOver4, MathHelper.PiOver4));
                    Particle smoke = new SmallSmokeParticle(hitPosition + hitPositionDisplace, flyDirection * 9f, Color.Crimson, new Color(130, 130, 130), Main.rand.NextFloat(1.8f, 2.6f), 155 - Main.rand.Next(30));
                    GeneralParticleHandler.SpawnParticle(smoke);

                    Particle Glow = new StrongBloom(hitPosition - hitPositionDisplace * 3, -direction * 6 * Main.rand.NextFloat(0.5f, 1f), Color.Crimson * 0.5f, 0.01f + Main.rand.NextFloat(0f, 0.2f), 20 + Main.rand.Next(40));
                    GeneralParticleHandler.SpawnParticle(Glow);
                }
                for (int i = 0; i < 3; i++)
                {
                    Vector2 hitPositionDisplace = direction.RotatedBy(MathHelper.PiOver2) * Main.rand.NextFloat(-10f, 10f);
                    Vector2 flyDirection = -direction.RotatedBy(Main.rand.NextFloat(-MathHelper.PiOver4, MathHelper.PiOver4));

                    Particle Rock = new StoneDebrisParticle(hitPosition - hitPositionDisplace * 3, flyDirection * Main.rand.NextFloat(3f, 6f), Color.Beige, 1f + Main.rand.NextFloat(0f, 0.4f), 30 + Main.rand.Next(50), 0.1f);
                    GeneralParticleHandler.SpawnParticle(Rock);
                }
            }
        }

        public override void AI()
        {
            if (!initialized) //Initialization. Here its litterally just playing a sound tho lmfao
            {
                Main.PlaySound(SoundID.Item90, projectile.Center);
                initialized = true;

                foreach (Projectile proj in Main.projectile)
                {
                    if (proj.active && proj.type == ProjectileType<SanguineFuryWheel>() && proj.owner == Owner.whoAmI)
                    {
                        if (CalamityUtils.AngleBetween(Owner.Center - Main.MouseWorld, Owner.Center - proj.Center) > MathHelper.PiOver4)
                        {
                            proj.Kill();
                            break;
                        }

                        Wheel = proj;
                        Dashing = true;
                        DashStart = Owner.Center;
                        Wheel.timeLeft = 60;
                        Owner.GiveIFrames(OmegaBiomeBlade.SuperPogoAttunement_SlashIFrames);
                        break;
                    }
                }
            }

            if (!OwnerCanShoot)
            {
                projectile.Kill();
                return;
            }

            if (Shred >= maxShred)
                Shred = maxShred;
            if (Shred < 0)
                Shred = 0;

            Lighting.AddLight(projectile.Center, new Vector3(1f, 0.56f, 0.56f) * ShredRatio);

            //Manage position and rotation
            direction = Owner.DirectionTo(Main.MouseWorld);
            direction.Normalize();
            projectile.rotation = direction.ToRotation();
            projectile.Center = Owner.Center + (direction * 60);

            //Scaling based on shred
            projectile.localNPCHitCooldown = OmegaBiomeBlade.SuperPogoAttunement_LocalIFrames - (int)(MathHelper.Lerp(0, OmegaBiomeBlade.SuperPogoAttunement_LocalIFrames - OmegaBiomeBlade.SuperPogoAttunement_LocalIFramesCharged, ShredRatio)); //Increase the hit frequency
            projectile.scale = 1f + (ShredRatio * 1f); //SWAGGER


            if ((Wheel == null || !Wheel.active) && Dashing)
            {
                Dashing = false;
                Owner.velocity *= 0.1f; //Abrupt stop

                Main.PlaySound(mod.GetLegacySoundSlot(Terraria.ModLoader.SoundType.Custom, "Sounds/Custom/MeatySlash"), projectile.Center);
                if (Owner.whoAmI == Main.myPlayer)
                {
                    Projectile proj = Projectile.NewProjectileDirect(Owner.Center - DashStart / 2f, Vector2.Zero, ProjectileType<SanguineFuryDash>(), (int)(projectile.damage * OmegaBiomeBlade.SuperPogoAttunement_SlashDamageBoost), 0, Owner.whoAmI);
                    if (proj.modProjectile is SanguineFuryDash dash)
                    {
                        dash.DashStart = DashStart;
                        dash.DashEnd = Owner.Center;
                    }
                }
            }

            Owner.Calamity().LungingDown = false;

            if (Dashing)
            {
                Owner.Calamity().LungingDown = true;
                Owner.fallStart = (int)(Owner.position.Y / 16f);
                Owner.velocity = Owner.DirectionTo(Wheel.Center) * 60f;

                if (Owner.Distance(Wheel.Center) < 60f)
                    Wheel.active = false;
            }


            if (Collision.SolidCollision(Owner.Center + (direction * 100 * projectile.scale) - Vector2.One * 5f, 10, 10) && !Dashing)
            {
                Pogo();
                projectile.netUpdate = true;
                projectile.netSpam = 0;
            }

            //Make the owner look like theyre holding the sword bla bla
            Owner.heldProj = projectile.whoAmI;
            Owner.direction = Math.Sign(direction.X);
            Owner.itemRotation = direction.ToRotation();
            if (Owner.direction != 1)
            {
                Owner.itemRotation -= 3.14f;
            }
            Owner.itemRotation = MathHelper.WrapAngle(Owner.itemRotation);
            Owner.itemTime = 2;
            Owner.itemAnimation = 2;

            Shred--;
            PogoCooldown--;
            BounceTime--;
            if (projectile.timeLeft <= 2)
                projectile.timeLeft = 2;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit) => ShredTarget();
        public override void OnHitPvp(Player target, int damage, bool crit) => ShredTarget();

        private void ShredTarget()
        {
            if (Main.myPlayer != Owner.whoAmI)
                return;

            Owner.fallStart = (int)(Owner.position.Y / 16f);
            // get lifted up
            if (PogoCooldown <= 0)
            {
                Main.PlaySound(SoundID.NPCHit30, projectile.Center); //Sizzle
                Shred += 62; //Augment the shredspeed
                if (Owner.velocity.Y > 0)
                    Owner.velocity.Y = -2f; //Get "stuck" into the enemy partly
                Owner.GiveIFrames(OmegaBiomeBlade.SuperPogoAttunement_ShredIFrames); // i framez.
                PogoCooldown = 20;
            }
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.NPCHit43, projectile.Center);
            if (ShredRatio > 0.25 && Owner.whoAmI == Main.myPlayer)
            {
                Projectile.NewProjectile(projectile.Center, direction * 16f, ProjectileType<SanguineFuryWheel>(), (int)(projectile.damage * OmegaBiomeBlade.SuperPogoAttunement_ShotDamageBoost), projectile.knockBack, Owner.whoAmI, Shred);
            }
            Owner.Calamity().LungingDown = false;
        }

        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            if (Owner.HeldItem.modItem is OmegaBiomeBlade sword && Main.rand.NextFloat() <= OmegaBiomeBlade.SuperPogoAttunement_ShredderProc)
                sword.OnHitProc = true;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D handle = GetTexture("CalamityMod/Items/Weapons/Melee/OmegaBiomeBlade");
            Texture2D blade = GetTexture("CalamityMod/Projectiles/Melee/TrueBiomeBlade_SanguineFury");

            int bladeAmount = 4;

            float drawAngle = direction.ToRotation();
            float drawRotation = drawAngle + MathHelper.PiOver4;

            Vector2 drawOrigin = new Vector2(0f, handle.Height);
            Vector2 drawOffset = Owner.Center + direction * 10f - Main.screenPosition;

            spriteBatch.Draw(handle, drawOffset, null, lightColor, drawRotation, drawOrigin, projectile.scale, 0f, 0f);

            //Turn on additive blending
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, Main.instance.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

            GameShaders.Misc["CalamityMod:BasicTint"].UseOpacity(MathHelper.Clamp(BounceTime, 0f, 20f) / 20f);
            GameShaders.Misc["CalamityMod:BasicTint"].UseColor(new Color(207, 248, 255));
            GameShaders.Misc["CalamityMod:BasicTint"].Apply();

            //Update the parameters
            drawOrigin = new Vector2(0f, blade.Height);

            spriteBatch.Draw(blade, drawOffset, null, Color.Lerp(Color.White, lightColor, 0.5f) * 0.9f, drawRotation, drawOrigin, projectile.scale, 0f, 0f);


            for (int i = 0; i < bladeAmount; i++) //Draw extra copies
            {
                blade = GetTexture("CalamityMod/Projectiles/Melee/TrueBiomeBlade_SanguineFuryExtra");

                drawAngle = direction.ToRotation();

                float circleCompletion = (float)Math.Sin(Main.GlobalTime * 5 + i * MathHelper.PiOver2);
                drawRotation = drawAngle + MathHelper.PiOver4 + (circleCompletion * MathHelper.Pi / 10f) - (circleCompletion * (MathHelper.Pi / 9f) * ShredRatio);

                drawOrigin = new Vector2(0f, blade.Height);

                Vector2 drawOffsetStraight = Owner.Center + direction * (float)Math.Sin(Main.GlobalTime * 7) * 10 - Main.screenPosition; //How far from the player
                Vector2 drawDisplacementAngle = direction.RotatedBy(MathHelper.PiOver2) * circleCompletion.ToRotationVector2().Y * (20 + 40 * ShredRatio); //How far perpendicularly
                Vector2 drawOffsetFromBounce = direction * MathHelper.Clamp(BounceTime, 0f, 20f) / 20f * 20f;

                spriteBatch.Draw(blade, drawOffsetStraight + drawDisplacementAngle + drawOffsetFromBounce, null, Color.Lerp(Color.White, lightColor, 0.5f) * 0.8f, drawRotation, drawOrigin, projectile.scale, 0f, 0f);
            }

            //Back to normal
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.instance.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

            return false;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(initialized);
            writer.WriteVector2(direction);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            initialized = reader.ReadBoolean();
            direction = reader.ReadVector2();
        }
    }
}