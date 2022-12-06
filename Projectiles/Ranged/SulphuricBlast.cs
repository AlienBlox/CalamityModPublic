﻿using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Ranged
{
    public class SulphuricBlast : ModProjectile
    {
        public const int TotalSecondsToStick = 8;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sulphuric Blast");
            Main.projFrames[Projectile.type] = 20;
        }

        public override void SetDefaults()
        {
            Projectile.width = 36;
            Projectile.height = 36;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = -1;
            Projectile.alpha = 255;
            Projectile.MaxUpdates = 3;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = Projectile.MaxUpdates * 12;
        }

        public override void AI()
        {
            if (Projectile.FinalExtraUpdate())
                Projectile.frameCounter++;
            Projectile.frame = Projectile.frameCounter / 4 % Main.projFrames[Projectile.type];

            Projectile.StickyProjAI(8);
            Projectile.Opacity = Utils.GetLerpValue(CalamityUtils.SecondsToFrames(TotalSecondsToStick), CalamityUtils.SecondsToFrames(TotalSecondsToStick * 0.5f), Projectile.localAI[0], true);

            // Emit sulpuric gas.
            if (Main.netMode != NetmodeID.Server && Projectile.FinalExtraUpdate() && Projectile.velocity.Length() > 3f)
            {
                Color color = new Color(136, 211, 113, 127);
                Color fadeColor = new Color(165, 165, 86);
                Vector2 gasSpawnPosition = Projectile.Center + Main.rand.NextVector2Circular(8f, 8f);
                Vector2 gasVelocity = Projectile.velocity * 1.2f + Projectile.velocity.RotatedBy(0.75f) * 0.3f;
                gasVelocity *= Main.rand.NextFloat(0.24f, 0.6f);

                Particle gas = new MediumMistParticle(gasSpawnPosition, gasVelocity, color, fadeColor, Main.rand.NextFloat(0.5f, 1f), 205 - Main.rand.Next(50), 0.02f);
                GeneralParticleHandler.SpawnParticle(gas);
            }

            // Home in on enemies if not sticking to anything.
            if (Projectile.ai[0] != 1f)
            {
                CalamityUtils.HomeInOnNPC(Projectile, true, 720f, 16f, Projectile.MaxUpdates * 20f);
                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit) => target.AddBuff(ModContent.BuffType<SulphuricPoisoning>(), 180);

        public override void OnHitPvp(Player target, int damage, bool crit) => target.AddBuff(ModContent.BuffType<SulphuricPoisoning>(), 180);

        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            Projectile.ModifyHitNPCSticky(2, true);
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (targetHitbox.Width > 8 && targetHitbox.Height > 8)
            {
                targetHitbox.Inflate(-targetHitbox.Width / 8, -targetHitbox.Height / 8);
            }
            return null;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture2D13 = ModContent.Request<Texture2D>(Texture).Value;
            int num214 = ModContent.Request<Texture2D>(Texture).Value.Height / Main.projFrames[Projectile.type];
            int y6 = num214 * Projectile.frame;
            Main.spriteBatch.Draw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(new Rectangle(0, y6, texture2D13.Width, num214)), Projectile.GetAlpha(lightColor), Projectile.rotation, new Vector2((float)texture2D13.Width / 2f, (float)num214 / 2f), Projectile.scale, SpriteEffects.None, 0);
            return false;
        }
    }
}
