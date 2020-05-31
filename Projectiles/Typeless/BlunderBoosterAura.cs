using CalamityMod.NPCs;
using CalamityMod.NPCs.AcidRain;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Typeless
{
	public class BlunderBoosterAura : ModProjectile
    {
        private const float radius = 98f;
        private const int framesX = 3;
        private const int framesY = 6;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Red Lightning Aura");
            ProjectileID.Sets.MinionSacrificable[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.width = 218;
            projectile.height = 218;
            projectile.ignoreWater = true;
            projectile.minionSlots = 0f;
            projectile.timeLeft = 18000;
            projectile.tileCollide = false;
            projectile.friendly = true;
            projectile.timeLeft *= 5;
            projectile.penetrate = -1;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 25;
        }

        public override void AI()
        {
            projectile.frameCounter++;
            if (projectile.frameCounter > 3)
            {
                projectile.localAI[0]++;
                projectile.frameCounter = 0;
            }
            if (projectile.localAI[0] >= framesY)
            {
                projectile.localAI[0] = 0;
                projectile.localAI[1]++;
            }
            if (projectile.localAI[1] >= framesX)
            {
                projectile.localAI[1] = 0;
            }
			if (Main.myPlayer != projectile.owner)
				projectile.Kill();
            Player player = Main.player[projectile.owner];
            Lighting.AddLight(projectile.Center, (255 - projectile.alpha) * 0.15f / 255f, (255 - projectile.alpha) * 0.15f / 255f, (255 - projectile.alpha) * 0.01f / 255f);
            projectile.Center = player.Center;
			if (player.dead || player is null)
			{
				player.Calamity().blunderBooster = false;
				projectile.Kill();
			}
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.Electrified, 300);

			if (target.knockBackResist <= 0f)
				return;

            if (CalamityGlobalNPC.ShouldAffectNPC(target))
            {
                float knockbackMultiplier = knockback - (1f - target.knockBackResist);
                if (knockbackMultiplier < 0)
                {
                    knockbackMultiplier = 0;
                }
                Vector2 trueKnockback = target.Center - projectile.Center;
                trueKnockback.Normalize();
                target.velocity = trueKnockback * knockbackMultiplier;
            }
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            target.AddBuff(BuffID.Electrified, 300);
		}

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D sprite = Main.projectileTexture[projectile.type];

            Color drawColour = Color.White;
            Rectangle sourceRect = new Rectangle(projectile.width * (int)projectile.localAI[1], projectile.height * (int)projectile.localAI[0], projectile.width, projectile.height);
            Vector2 origin = new Vector2(projectile.width / 2, projectile.height / 2);

            float opacity = 1f;
            int sparkCount = 0;
            int fadeTime = 20;

            if (projectile.timeLeft < fadeTime)
            {
                opacity = projectile.timeLeft * (1f / fadeTime);
                sparkCount = fadeTime - projectile.timeLeft;
            }

            for (int i = 0; i < sparkCount * 2; i++)
            {
                int dustType = 132;
                if (Main.rand.NextBool())
                {
                    dustType = 264;
                }
                float rangeDiff = 2f;

                Vector2 dustPos = new Vector2(Main.rand.NextFloat(-1, 1), Main.rand.NextFloat(-1, 1));
                dustPos.Normalize();
                dustPos *= radius + Main.rand.NextFloat(-rangeDiff, rangeDiff);

                int dust = Dust.NewDust(projectile.Center + dustPos, 1, 1, dustType, 0, 0, 0, default, 0.75f);
                Main.dust[dust].noGravity = true;
            }

            spriteBatch.Draw(sprite, projectile.Center - Main.screenPosition, sourceRect, drawColour * opacity, projectile.rotation, origin, 1f, SpriteEffects.None, 0f);
            return false;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float dist1 = Vector2.Distance(projectile.Center, targetHitbox.TopLeft());
            float dist2 = Vector2.Distance(projectile.Center, targetHitbox.TopRight());
            float dist3 = Vector2.Distance(projectile.Center, targetHitbox.BottomLeft());
            float dist4 = Vector2.Distance(projectile.Center, targetHitbox.BottomRight());

            float minDist = dist1;
            if (dist2 < minDist)
                minDist = dist2;
            if (dist3 < minDist)
                minDist = dist3;
            if (dist4 < minDist)
                minDist = dist4;

            return minDist <= radius;
        }

        public override bool? CanHitNPC(NPC target)
		{
			if (target.catchItem != 0 && target.type != ModContent.NPCType<Radiator>())
			{
				return false;
			}
			return null;
		}
    }
}
