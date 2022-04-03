using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Melee
{
    public class StreamGougePortal : ModProjectile
    {
        public Vector2 SpearCenter => Vector2.Lerp(Projectile.Center + Projectile.velocity + Projectile.velocity.SafeNormalize(Vector2.Zero) * 42f, Projectile.Center, 1f - Projectile.scale);
        public ref float Time => ref Projectile.ai[0];

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Portal");
        }

        public override void SetDefaults()
        {
            Projectile.width = 14;
            Projectile.height = 14;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.tileCollide = false;
            Projectile.timeLeft = StreamGouge.PortalLifetime;
            Projectile.ignoreWater = true;
            Projectile.alpha = 255;
            Projectile.penetrate = -1;
            Projectile.MaxUpdates = 2;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override void AI()
        {
            // Play a slice sound when spawning.
            if (Projectile.localAI[0] == 0f)
            {
                Main.PlayTrackedSound(CalamityUtils.GetTrackableSound("Sounds/Custom/SwiftSlice"), Projectile.Center);
                Projectile.localAI[0] = 1f;
            }

            float spearOutwardness = MathHelper.Lerp(4f, 52f, Utils.InverseLerp(0f, 18f, Time, true));
            Projectile.velocity = Projectile.velocity.SafeNormalize(Vector2.UnitY) * spearOutwardness;
            Projectile.Opacity = Utils.InverseLerp(0f, 8f, Time, true) * Utils.InverseLerp(0f, 10f, Projectile.timeLeft, true);
            Projectile.scale = Projectile.Opacity;
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4;
            Time++;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>("CalamityMod/Items/Weapons/Melee/StreamGouge");
            Vector2 spearDrawPosition = SpearCenter - Main.screenPosition;
            Vector2 spearOrigin = texture.Size() * 0.5f;
            Color spearColor = Main.hslToRgb(Projectile.identity % 10f / 40f + 0.67f, 1f, 0.5f);
            spearColor.A = 0;

            for (int i = 0; i < 2; i++)
                spriteBatch.Draw(texture, spearDrawPosition, null, spearColor * Projectile.Opacity, Projectile.rotation, spearOrigin, Projectile.scale, 0, 0);

            Texture2D portalTexture = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Melee/StreamGougePortal");
            Vector2 origin = portalTexture.Size() * 0.5f;
            Color baseColor = Color.White;
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            float rotation = Main.GlobalTime * 6f + Projectile.identity * 1.45f;

            // Black portal.
            Color color = Color.Lerp(baseColor, Color.Black, 0.55f).MultiplyRGB(Color.DarkGray) * Projectile.Opacity;
            spriteBatch.Draw(portalTexture, drawPosition, null, color, rotation, origin, Projectile.scale * 1.2f, SpriteEffects.None, 0f);
            spriteBatch.Draw(portalTexture, drawPosition, null, color, -rotation, origin, Projectile.scale * 1.2f, SpriteEffects.None, 0f);

            spriteBatch.SetBlendState(BlendState.Additive);

            // Cyan portal.
            color = Color.Lerp(baseColor, Color.Cyan, 0.55f) * Projectile.Opacity * 1.4f;
            spriteBatch.Draw(portalTexture, drawPosition, null, color, rotation * 0.6f, origin, Projectile.scale * 1.2f, SpriteEffects.None, 0f);

            // Magenta portal.
            color = Color.Lerp(baseColor, Color.Fuchsia, 0.55f) * Projectile.Opacity * 1.4f;
            spriteBatch.Draw(portalTexture, drawPosition, null, color, rotation * -0.7f, origin, Projectile.scale * 1.2f, SpriteEffects.None, 0f);
            spriteBatch.SetBlendState(BlendState.AlphaBlend);
            return false;
        }

        // Create impact and cosmic parallax particles when hitting enemies.
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            SoundEngine.PlaySound(SoundID.Item74, target.Center);
            if (Main.netMode == NetmodeID.Server)
                return;

            Color impactColor = Color.Lerp(Color.Cyan, Color.White, Main.rand.NextFloat(0.3f, 0.64f));
            Vector2 impactPoint = Vector2.Lerp(Projectile.Center, target.Center, 0.65f);
            ImpactParticle impactParticle = new ImpactParticle(impactPoint, 0.1f, 20, Main.rand.NextFloat(0.4f, 0.5f), impactColor);
            GeneralParticleHandler.SpawnParticle(impactParticle);

            for (int i = 0; i < 20; i++)
            {
                Vector2 spawnPosition = target.Center + Main.rand.NextVector2Circular(30f, 30f);
                FusableParticleManager.GetParticleSetByType<StreamGougeParticleSet>()?.SpawnParticle(spawnPosition, 60f);

                float scale = MathHelper.Lerp(24f, 64f, CalamityUtils.Convert01To010(i / 19f));
                spawnPosition = target.Center + Projectile.velocity.SafeNormalize(Vector2.UnitY) * MathHelper.Lerp(-40f, 90f, i / 19f);
                FusableParticleManager.GetParticleSetByType<StreamGougeParticleSet>()?.SpawnParticle(spawnPosition, scale);
            }
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float _ = 0f;
            Vector2 start = SpearCenter - Projectile.velocity.SafeNormalize(Vector2.UnitY) * Projectile.scale * 50f;
            Vector2 end = SpearCenter + Projectile.velocity.SafeNormalize(Vector2.UnitY) * Projectile.scale * 50f;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), start, end, 15f, ref _);
        }

        public override bool ShouldUpdatePosition() => false;
    }
}
