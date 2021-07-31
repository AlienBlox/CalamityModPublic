using CalamityMod.Items;
using Microsoft.Xna.Framework;
using System;
using System.IO;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public class TheAtomSplitterProjectile : ModProjectile
    {
        // Atom splitting is cool and all, but actual thermonuclear meltdown levels of DPS is unacceptable.
        // DO NOT increase this unless you are ABSOLUTELY SURE you know what will happen.
        private const float StealthSplitMultiplier = 0.1666667f;
        
        public ref float HitTargetIndex => ref projectile.ai[0];
        public ref float Time => ref projectile.ai[1];

        public override string Texture => "CalamityMod/Items/Weapons/Rogue/TheAtomSplitter";

        public override void SetStaticDefaults() => DisplayName.SetDefault("Atom Splitter");

        public override void SetDefaults()
        {
            projectile.width = projectile.height = 124;
            projectile.friendly = true;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.penetrate = 2;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 10;
            projectile.timeLeft = 300;
            projectile.Calamity().rogue = true;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            // timeLeft is not synced by default. However, it is changed during an operation that is only
            // performed from one client's perspective, therefore it must be synced in this context.
            writer.Write(projectile.timeLeft);
            writer.Write((byte)projectile.alpha);
            writer.Write(projectile.scale);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            projectile.timeLeft = reader.ReadInt32();
            projectile.alpha = reader.ReadByte();
            projectile.scale = reader.ReadSingle();
        }

        public override void AI()
        {
            // Accelerate until at a certain speed.
            if (projectile.velocity.Length() < 20f)
                projectile.velocity *= 1.02f;

            if (projectile.alpha < 10)
                EmitDustFromTip();

            // Stick to the target for a while and release a bunch of duplicates.
            if (Main.npc.IndexInRange((int)HitTargetIndex) && Main.npc[(int)HitTargetIndex].active)
            {
                if (projectile.alpha < 255)
                    projectile.alpha = Utils.Clamp(projectile.alpha + 30, 0, 255);

                bool stealth = projectile.Calamity().stealthStrike;
                int shootRate = stealth ? 2 : 4;
                if (projectile.timeLeft % shootRate == shootRate - 1)
                {
                    NPC target = Main.npc[(int)HitTargetIndex];
                    FireDuplicateAtTarget(target, stealth ? 305f : 200f, stealth);
                    if (stealth)
                        FireExtraDuplicatesAtTarget(target);
                }
            }
            projectile.rotation = projectile.velocity.ToRotation() + MathHelper.PiOver4;
            Time++;
        }

        public void EmitDustFromTip()
        {
            if (Main.dedServ)
                return;

            float dustVelocityArcOffset = 1.5f + (float)Math.Sin(MathHelper.TwoPi * projectile.timeLeft / 35f) * 0.25f;

            float colorInterpolant = (float)Math.Cos(MathHelper.TwoPi * projectile.timeLeft / 75f) * 0.5f + 0.5f;
            Color dustColor = CalamityUtils.MulticolorLerp(colorInterpolant, CalamityGlobalItem.ExoPalette);
            Vector2 currentDirection = projectile.velocity.SafeNormalize(-Vector2.UnitY);
            Vector2 tipPosition = projectile.Center + currentDirection * (projectile.height * 0.67f - 3f);
            tipPosition += Main.rand.NextVector2CircularEdge(0.35f, 0.35f);

            for (float direction = -1f; direction <= 1f; direction += 2f)
            {
                Dust prismaticEnergy = Dust.NewDustPerfect(tipPosition, 267);
                prismaticEnergy.velocity = currentDirection.RotatedBy(direction * dustVelocityArcOffset) * -7f + projectile.velocity;
                prismaticEnergy.scale = 1.2f;
                prismaticEnergy.color = dustColor;
                prismaticEnergy.noGravity = true;

                prismaticEnergy = Dust.CloneDust(prismaticEnergy);
                prismaticEnergy.scale *= 0.6f;
            }
        }

        public void FireDuplicateAtTarget(NPC target, float baseOutwardness, bool stealth)
        {
            if (Main.myPlayer != projectile.owner)
                return;

            Vector2 spawnPosition = target.Center + Main.rand.NextVector2CircularEdge(baseOutwardness, baseOutwardness) * Main.rand.NextFloat(0.9f, 1.15f);
            Vector2 shootVelocity = (target.Center - spawnPosition).SafeNormalize(Vector2.UnitY) * Main.rand.NextFloat(12f, 14f);
            int damage = stealth ? (int)(projectile.damage * StealthSplitMultiplier) : projectile.damage;
            Projectile.NewProjectile(spawnPosition, shootVelocity, ModContent.ProjectileType<TheAtomSplitterDuplicate>(), damage, projectile.knockBack, projectile.owner, 0f, baseOutwardness / 9f);
        }

        public void FireExtraDuplicatesAtTarget(NPC target)
        {
            if (Main.myPlayer != projectile.owner)
                return;

            Vector2 spawnPosition = target.Center + Vector2.UnitY * Main.rand.NextBool(2).ToDirectionInt() * Main.rand.NextFloat(200f, 270f);
            spawnPosition.X += Main.rand.NextFloatDirection() * target.width * 0.45f;
            int damage = (int)(projectile.damage * StealthSplitMultiplier);
            Vector2 shootVelocity = Vector2.UnitY * (target.Center.Y - spawnPosition.Y > 0f).ToDirectionInt() * Main.rand.NextFloat(14f, 16.5f);
            Projectile.NewProjectile(spawnPosition, shootVelocity, ModContent.ProjectileType<TheAtomSplitterDuplicate>(), damage, projectile.knockBack, projectile.owner, 0f, 24f);
        }

        public void ReleaseHitDust(Vector2 spawnPosition)
        {
            if (Main.dedServ)
                return;

            int dustCount = projectile.Calamity().stealthStrike ? 60 : 40;
            Vector2 baseDustVelocity = -Vector2.UnitY.RotatedByRandom(MathHelper.Pi) * 1.4f;
            Vector2 outwardFireSpeedFactor = new Vector2(2.1f, 2f);

            for (float i = 0f; i < dustCount; i++)
            {
                Color dustColor = CalamityUtils.MulticolorLerp(Main.rand.NextFloat(), CalamityGlobalItem.ExoPalette);
                Dust explosionDust = Dust.NewDustDirect(spawnPosition, 0, 0, 267, 0f, 0f, 0, dustColor, 1f);
                explosionDust.position = spawnPosition;
                explosionDust.velocity = baseDustVelocity.RotatedBy(MathHelper.TwoPi * i / dustCount) * outwardFireSpeedFactor * Main.rand.NextFloat(0.8f, 1.2f);
                explosionDust.velocity += projectile.velocity * Main.rand.NextFloat(0.6f, 0.85f);
                if (projectile.Calamity().stealthStrike)
                    explosionDust.velocity *= 1.6f;

                explosionDust.noGravity = true;
                explosionDust.scale = 1.1f;
                explosionDust.fadeIn = Main.rand.NextFloat(1.4f, 2.4f);

                Dust.CloneDust(explosionDust).velocity *= Main.rand.NextFloat(0.8f);

                explosionDust = Dust.CloneDust(explosionDust);
                explosionDust.scale /= 2f;
                explosionDust.fadeIn /= 2f;
                explosionDust.color = new Color(255, 255, 255, 255);
            }
        }

        public override bool CanDamage() => projectile.alpha < 80;

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            ReleaseHitDust(target.Center - projectile.velocity * 3f);
            if (!Main.npc.IndexInRange((int)HitTargetIndex))
            {
                HitTargetIndex = target.whoAmI;
                projectile.timeLeft = projectile.Calamity().stealthStrike ? 100 : 60;
                projectile.netUpdate = true;
            }
        }
    }
}
