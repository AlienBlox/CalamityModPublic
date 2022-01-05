using CalamityMod.CalPlayer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Melee
{
	public class TaintedBladeSlasher : ModProjectile
    {
        internal PrimitiveTrail TrailDrawer = null;
        public ref float SwordItemID => ref projectile.ai[1];
        public ref float VerticalOffset => ref projectile.localAI[0];
        public ref float Time => ref projectile.localAI[1];
        public float AttackCompletionRatio
        {
            get
            {
                float completionRatio = 1f - Owner.itemAnimation / (float)Owner.itemAnimationMax;
                if (float.IsNaN(completionRatio) || float.IsInfinity(completionRatio))
                    completionRatio = 0f;
                return completionRatio;
            }
        }
        public Player Owner => Main.player[projectile.owner];
        public int Variant => (int)projectile.ai[0] % 2;

        public const float ForearmLength = 80f;
        public const float ArmLength = 108f;

        public Rectangle BladeFrame
        {
            get
            {
                Texture2D bladeTexture = Main.itemTexture[(int)SwordItemID];
                Rectangle bladeFrame = bladeTexture.Frame(1, 1, 0, 0);
                bool hasMultipleFrames = Main.itemAnimations[(int)SwordItemID] != null;
                if (hasMultipleFrames)
                    bladeFrame = Main.itemAnimations[(int)SwordItemID].GetFrame(bladeTexture);
                return bladeFrame;
            }
        }

        public Vector2 BackArmAimPosition
        {
            get
            {
                Vector2 baseAimPosition = Owner.Top + new Vector2(-Owner.direction * (ForearmLength + Math.Abs(VerticalOffset) * 0.55f), -27f);
                if (Variant == 1)
                    baseAimPosition.Y += 30f;
                if (Owner.itemAnimation == 0)
                    return baseAimPosition;

                Vector2 endSwingPosition = Owner.Center + Vector2.UnitX * Owner.direction * 510f;
                return Vector2.SmoothStep(baseAimPosition, endSwingPosition, Utils.InverseLerp(0f, 0.67f, AttackCompletionRatio, true) * Utils.InverseLerp(1f, 0.67f, AttackCompletionRatio, true));
            }
        }
        public Vector2 IdleMoveOffset
        {
            get
            {
                Vector2 offset = Owner.itemAnimation == 0 ? (Time / 27f).ToRotationVector2() * new Vector2(5f, 4f) : Vector2.Zero;
                if (Variant == 1)
                    offset += Vector2.UnitX * Owner.direction * (1f - AttackCompletionRatio) * 40f;
                return offset;
            }
        }
        public Vector2 FrontArmEnd
        {
            get
            {
                float backArmRotation = Owner.AngleTo(BackArmAimPosition);
                Vector2 fromArmDrawPosition = Owner.Center + backArmRotation.ToRotationVector2() * ForearmLength;
                Vector2 backOffset = (projectile.Center - fromArmDrawPosition).SafeNormalize(Vector2.Zero) * ArmLength + IdleMoveOffset;
                return fromArmDrawPosition + backOffset;
            }
        }
        public Vector2 BladeOffsetDirection
        {
            get
            {
                float backArmRotation = Owner.AngleTo(BackArmAimPosition);
                Vector2 fromArmDrawPosition = Owner.Center + backArmRotation.ToRotationVector2() * ForearmLength;
                Vector2 backOffset = (projectile.Center - fromArmDrawPosition).SafeNormalize(Vector2.Zero) * ArmLength + IdleMoveOffset;
                float bladeRotation = backOffset.ToRotation() - MathHelper.PiOver4 + MathHelper.Pi;
                return -(bladeRotation + MathHelper.PiOver4 + BladeRotationOffset).ToRotationVector2();
            }
        }
        public Vector2 BladeCenterPosition
        {
            get
            {
                float offsetFactor = BladeFrame.Height / 4f + 31f;
                return FrontArmEnd + BladeOffsetDirection * offsetFactor;
            }
        }
        public float BladeRotationOffset => MathHelper.Lerp(MathHelper.PiOver2, 0f, Utils.InverseLerp(0f, 0.17f, 1f - AttackCompletionRatio, true)) * -Owner.direction;
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Blade");
            ProjectileID.Sets.TrailingMode[projectile.type] = 2;
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 70;
        }

        public override void SetDefaults()
        {
            projectile.width = projectile.height = 4;
            projectile.friendly = true;
            projectile.melee = true;
            projectile.tileCollide = false;
            projectile.penetrate = -1;
            projectile.timeLeft = 90000;
            projectile.usesLocalNPCImmunity = true;
            projectile.noEnchantments = true;
			projectile.Calamity().trueMelee = true;
        }

        public override void SendExtraAI(BinaryWriter writer) => writer.Write(VerticalOffset);

        public override void ReceiveExtraAI(BinaryReader reader) => VerticalOffset = reader.ReadSingle();

        public override void AI()
        {
            CalamityPlayer.EnchantHeldItemEffects(Owner, Owner.Calamity(), Owner.ActiveItem());
            if (!Owner.Calamity().bladeArmEnchant || Owner.ActiveItem().type != SwordItemID || Owner.CCed || !Owner.active || Owner.dead)
            {
                projectile.Kill();
                return;
            }

            if (Owner.itemAnimationMax == 0)
                Owner.itemAnimationMax = (int)(Owner.ActiveItem().useAnimation * Owner.meleeSpeed);

            float swingOffsetAngle = MathHelper.SmoothStep(-1.87f, 3.79f, AttackCompletionRatio);

            // Offset a little bit angularly if the secondary variant to prevent complete overlap.
            if (Variant == 1)
                swingOffsetAngle -= 0.7f;
            swingOffsetAngle *= Owner.direction;
            swingOffsetAngle = MathHelper.Lerp(0f, swingOffsetAngle, Utils.InverseLerp(0f, 0.16f, AttackCompletionRatio, true));

            if (Owner.itemAnimation == 0)
                swingOffsetAngle = 0f;

            Vector2 destination = Owner.Center;

            if (Owner.itemAnimation == 0)
            {
                destination.X += Owner.direction * (VerticalOffset * 0.6f + 180f);
                destination.Y += 330f + VerticalOffset;
                if (Variant == 1)
                    destination.Y += 540f;

                // The blade can be kept in a static location when not being swung.
                // This can result in insane damage coming from "poking" enemies.
                // To accomodate for this, the hit countdown is increased when not attacking.
                projectile.localNPCHitCooldown = 24;
            }
            else
            {
                // Swing around in an arc as needed.
                // One arm extends a little bit further than the other.
                destination -= Vector2.UnitY.RotatedBy(swingOffsetAngle) * 600f;

                // Make a fire/swing sound.
                if (Owner.itemAnimation == 21)
                    Main.PlaySound(SoundID.DD2_FlameburstTowerShot, Owner.Center);
                projectile.localNPCHitCooldown = 3;
            }

            if (AttackCompletionRatio < 0.16f)
                projectile.oldPos = new Vector2[projectile.oldPos.Length];

            // Very, very quickly approach the swing destination while attacking.
            // Also gain a temporary extra update.
            if (Owner.itemAnimation > 0)
            {
                projectile.Center = Vector2.Lerp(projectile.Center, destination, 0.19f);
                projectile.extraUpdates = 1;
            }

            // Idly move towards the destination.
            if (projectile.Center != destination)
                projectile.Center += (destination - projectile.Center).SafeNormalize(Vector2.Zero) * MathHelper.Min(projectile.Distance(destination), 12f + Owner.velocity.Length());

            // Ensure that the position is never too far from the destination.
            if (!projectile.WithinRange(destination, 300f))
                projectile.Center = destination;

            Time++;
        }

        internal float PrimitiveWidthFunction(float completionRatio) => BladeFrame.Height * 0.47f;

        internal Color PrimitiveColorFunction(float completionRatio)
        {
            float opacity = Utils.InverseLerp(0.8f, 0.52f, completionRatio, true) * Utils.InverseLerp(1f, 0.81f, AttackCompletionRatio, true);
            Color startingColor = Color.Lerp(Color.Red, Color.DarkRed, 0.4f);
            Color endingColor = Color.Lerp(Color.DarkRed, Color.Purple, 0.77f);
            return Color.Lerp(startingColor, endingColor, (float)Math.Pow(completionRatio, 0.37f)) * opacity;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            if (TrailDrawer is null)
                TrailDrawer = new PrimitiveTrail(PrimitiveWidthFunction, PrimitiveColorFunction, specialShader: GameShaders.Misc["CalamityMod:FadingSolidTrail"]);

            Texture2D forearmTexture = ModContent.GetTexture("CalamityMod/ExtraTextures/TaintedForearm");
            Texture2D armTexture = ModContent.GetTexture("CalamityMod/ExtraTextures/TaintedArm");
            Texture2D handTexture = ModContent.GetTexture("CalamityMod/ExtraTextures/TaintedHand");

            // Reflect the 1st hand variant horizontally.
            SpriteEffects handDirection = SpriteEffects.None;
            if (Variant == (Owner.direction == -1).ToInt())
            {
                handTexture = ModContent.GetTexture("CalamityMod/ExtraTextures/TaintedHand2");
                handDirection = SpriteEffects.FlipHorizontally;
            }
            Texture2D bladeTexture = Main.itemTexture[(int)SwordItemID];

            // Draw the arms.
            float backArmRotation = Owner.AngleTo(BackArmAimPosition);
            spriteBatch.Draw(forearmTexture, Owner.Center - Main.screenPosition, null, Color.White, backArmRotation - MathHelper.PiOver2, Vector2.UnitX * forearmTexture.Size() * 0.5f, projectile.scale, SpriteEffects.None, 0f);

            Vector2 frontArmDrawPosition = Owner.Center + backArmRotation.ToRotationVector2() * ForearmLength + IdleMoveOffset;
            float frontArmRotation = projectile.AngleFrom(frontArmDrawPosition);
            spriteBatch.Draw(armTexture, frontArmDrawPosition - Main.screenPosition, null, Color.White, frontArmRotation - MathHelper.PiOver2, Vector2.UnitX * armTexture.Size() * 0.5f, projectile.scale, SpriteEffects.None, 0f);

            float handRotation = frontArmRotation + MathHelper.PiOver2 + MathHelper.Pi;
            float bladeRotation = handRotation - MathHelper.PiOver4 + MathHelper.Pi + BladeRotationOffset;
            if (Owner.direction == -1)
                bladeRotation += MathHelper.PiOver2;

            // Draw the trail behind the blade.
            if (Owner.itemAnimation > 0)
            {
                GameShaders.Misc["CalamityMod:FadingSolidTrail"].SetShaderTexture(ModContent.GetTexture("CalamityMod/ExtraTextures/BladeTrailUVMap"));
                GameShaders.Misc["CalamityMod:FadingSolidTrail"].Shader.Parameters["shouldFlip"].SetValue((float)(Owner.direction == -1).ToInt());

                Vector2 bottom = BladeCenterPosition - BladeOffsetDirection * BladeFrame.Height * 0.5f - Main.screenPosition;
                Vector2 top = BladeCenterPosition + BladeOffsetDirection * BladeFrame.Height * 0.5f - Main.screenPosition;
                Vector2 offsetToBlade = (top - bottom).SafeNormalize(Vector2.Zero).RotatedBy(MathHelper.PiOver2) * 5f;

                TrailDrawer.OverridingStickPointStart = BladeCenterPosition - BladeOffsetDirection * BladeFrame.Height * 0.5f + offsetToBlade - Main.screenPosition;
                TrailDrawer.OverridingStickPointEnd = BladeCenterPosition + BladeOffsetDirection * BladeFrame.Height * 0.5f + offsetToBlade - Main.screenPosition;

                // Swap the ends if needed so that the trail faces the right direction.
                if (Owner.direction == -1)
                    Utils.Swap(ref TrailDrawer.OverridingStickPointStart, ref TrailDrawer.OverridingStickPointEnd);

                Vector2[] drawPoints = new Vector2[projectile.oldPos.Length];
                Vector2 perpendicularDirection = BladeOffsetDirection.SafeNormalize(Vector2.UnitY).RotatedBy(MathHelper.PiOver2);
                for (int i = 1; i < drawPoints.Length; i++)
                {
                    if (projectile.oldPos[i] == Vector2.Zero)
                        continue;

                    drawPoints[i] = projectile.Center + perpendicularDirection.RotatedBy(i * -0.014f * Owner.direction) * i * -Owner.direction * 6f;
                }

                TrailDrawer.Draw(drawPoints, BladeCenterPosition - projectile.position - Main.screenPosition, 67);
            }

            // Draw the blade.
            Vector2 bladeDrawPosition = BladeCenterPosition - Main.screenPosition;
            SpriteEffects bladeDirection = Owner.direction == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            spriteBatch.Draw(bladeTexture, bladeDrawPosition, BladeFrame, Color.White, bladeRotation, BladeFrame.Size() * 0.5f, projectile.scale, bladeDirection, 0f);

            // Draw the hand.
            spriteBatch.Draw(handTexture, FrontArmEnd - Main.screenPosition, null, projectile.GetAlpha(Color.White), handRotation, handTexture.Size() * 0.5f, projectile.scale, handDirection, 0f);
            return false;
        }

        // Register collision at the blade's position.
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            Vector2 start = BladeCenterPosition - BladeOffsetDirection * BladeFrame.Height * 0.5f;
            Vector2 end = BladeCenterPosition + BladeOffsetDirection * BladeFrame.Height * 0.5f;
            float width = 60f;
            float _ = 0f;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), start, end, width, ref _);
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            ItemLoader.OnHitNPC(Owner.ActiveItem(), Owner, target, damage, knockback, crit);
            NPCLoader.OnHitByItem(target, Owner, Owner.ActiveItem(), damage, knockback, crit);
            PlayerHooks.OnHitNPC(Owner, Owner.ActiveItem(), target, damage, knockback, crit);
        }
    }
}
