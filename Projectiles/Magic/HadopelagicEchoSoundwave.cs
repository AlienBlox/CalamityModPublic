﻿using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Magic
{
    public class HadopelagicEchoSoundwave : ModProjectile
    {
		private int echoCooldown = 0;
		private bool playedSound = false;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Echo");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            projectile.width = 36;
            projectile.height = 36;
            projectile.scale = 0.005f;
            projectile.alpha = 100;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.penetrate = -1;
            projectile.magic = true;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 0;
            projectile.timeLeft = 450;
        }

        public override void AI()
        {
			if (projectile.ai[0] == 0f || projectile.ai[0] == 4f)
			{
				if (!playedSound)
				{
					Main.PlaySound(Main.rand.NextBool(100) ? mod.GetLegacySoundSlot(SoundType.NPCKilled, "Sounds/NPCKilled/Sunskater") : mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/WyrmScream"), (int)projectile.Center.X, (int)projectile.Center.Y);
					playedSound = true;
				}
			}
            if (projectile.localAI[0] < 1f)
            {
                projectile.localAI[0] += 0.05f;
                projectile.scale += 0.05f;
                projectile.width = (int)(36f * projectile.scale);
                projectile.height = (int)(36f * projectile.scale);
            }
            else
            {
                projectile.width = 36;
                projectile.height = 36;
            }

			if (echoCooldown > 0)
				echoCooldown--;

            projectile.rotation = (float)Math.Atan2((double)projectile.velocity.Y, (double)projectile.velocity.X);
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (projectile.velocity.X != oldVelocity.X)
            {
                projectile.velocity.X = -oldVelocity.X;
            }
            if (projectile.velocity.Y != oldVelocity.Y)
            {
                projectile.velocity.Y = -oldVelocity.Y;
            }
            return false;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<CrushDepth>(), 600);
            target.AddBuff(BuffID.Ichor, 600);
            projectile.velocity *= 0.85f;

			if (echoCooldown == 0 && projectile.ai[0] == 4f)
			{
				echoCooldown = 60;
				int echoID = ModContent.ProjectileType<HadopelagicEcho2>();
				int echoDamage = (int)(0.2f * projectile.damage);
				float echoKB = projectile.knockBack / 3;
				int echos = 5;
				for (int i = 0; i < echos; ++i)
				{
					float startDist = Main.rand.NextFloat(260f, 270f);
					Vector2 startDir = Main.rand.NextVector2Unit();
					Vector2 startPoint = target.Center + (startDir * startDist);

					float echoSpeed = Main.rand.NextFloat(15f, 18f);
					Vector2 velocity = startDir * (-echoSpeed);

					if (projectile.owner == Main.myPlayer)
					{
						Projectile.NewProjectile(startPoint, velocity, echoID, echoDamage, echoKB, projectile.owner, 0f, 0f);
					}
				}
			}
        }

        public override Color? GetAlpha(Color lightColor)
        {
            if (projectile.timeLeft < 85)
            {
                byte b2 = (byte)(projectile.timeLeft * 3);
                byte a2 = (byte)(100f * ((float)b2 / 255f));
                return new Color((int)b2, (int)b2, (int)b2, (int)a2);
            }
            return new Color(255, 255, 255, 100);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityGlobalProjectile.DrawCenteredAndAfterimage(projectile, lightColor, ProjectileID.Sets.TrailingMode[projectile.type], 1);
            return false;
        }
    }
}
