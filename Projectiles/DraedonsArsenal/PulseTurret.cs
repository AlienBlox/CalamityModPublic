﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.DraedonsArsenal
{
    public class PulseTurret : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Pulse Turret");
            ProjectileID.Sets.MinionSacrificable[projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.width = 50;
            projectile.height = 24;
            projectile.ignoreWater = true;
            projectile.tileCollide = true;
            projectile.sentry = true;
            projectile.timeLeft = Projectile.SentryLifeTime;
            projectile.timeLeft *= 5;
            projectile.penetrate = -1;
        }

        public override void AI()
        {
            Player player = Main.player[projectile.owner];
            if (projectile.localAI[0] == 0f)
            {
                projectile.Calamity().spawnedPlayerMinionDamageValue = player.MinionDamage();
                projectile.Calamity().spawnedPlayerMinionProjectileDamageValue = projectile.damage;
                projectile.localAI[0] = 1;
            }
            if (player.MinionDamage() != projectile.Calamity().spawnedPlayerMinionDamageValue)
            {
                int trueDamage = (int)(projectile.Calamity().spawnedPlayerMinionProjectileDamageValue /
                    projectile.Calamity().spawnedPlayerMinionDamageValue *
                    player.MinionDamage());
                projectile.damage = trueDamage;
            }

            if (projectile.velocity.Y < 12f)
            {
                projectile.velocity.Y += 0.5f;
            }

            NPC potentialTarget = projectile.Center.MinionHoming(850f, player);

            if (potentialTarget != null && Collision.CanHit(projectile.position, projectile.width, projectile.height, potentialTarget.position, potentialTarget.width, potentialTarget.height))
            {
                projectile.spriteDirection = (potentialTarget.Center.X - projectile.Center.X > 0).ToDirectionInt();
                projectile.ai[0]++;
                if (projectile.ai[0] % 40f < 30f)
                {
                    float idealAngle = projectile.AngleTo(potentialTarget.Center) + (projectile.spriteDirection == -1).ToInt() * MathHelper.Pi;
                    if (projectile.ai[1] <= 0f)
                    {
                        projectile.rotation = projectile.rotation.AngleLerp(idealAngle, MathHelper.TwoPi / 25f);
                    }
                    // Recoil back after firing
                    else
                    {
                        if (projectile.ai[1] > 13f)
                        {
                            projectile.rotation -= MathHelper.ToRadians(10f) * projectile.localAI[0];
                        }
                        projectile.ai[1]--;
                    }
                }
                if (projectile.ai[0] % 40f == 39f && Main.myPlayer == projectile.owner)
                {
                    Texture2D standTexture = ModContent.GetTexture("CalamityMod/Projectiles/DraedonsArsenal/PulseTurretStand");
                    Vector2 shootPosition = projectile.Center - ((standTexture.Height / 2 + 6f) * Vector2.UnitY);
                    shootPosition += (projectile.Size * 0.5f).RotatedBy(projectile.rotation - MathHelper.ToRadians(18f) - (projectile.spriteDirection == -1).ToInt() * MathHelper.Pi);

                    if (Math.Abs(Vector2.Normalize(potentialTarget.Center - shootPosition).ToRotation() - projectile.rotation) < MathHelper.ToRadians(32f) + (projectile.spriteDirection == -1).ToInt() * MathHelper.Pi)
                    {
                        Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Item, "Sounds/Item/PulseRifleFire"), shootPosition);
                        Projectile.NewProjectile(shootPosition,
                                                 Vector2.Normalize(potentialTarget.Center - shootPosition) * 24f,
                                                 ModContent.ProjectileType<PulseTurretShot>(),
                                                 projectile.damage,
                                                 projectile.knockBack,
                                                 projectile.owner);
                        if (projectile.ai[0] % 120f == 119f)
                        {
                            for (int i = -1; i <= 1; i += 2)
                            {
                                Projectile.NewProjectile(shootPosition,
                                                         Vector2.Normalize(potentialTarget.Center - shootPosition).RotatedBy(i * MathHelper.ToRadians(28f)) * 14f,
                                                         ModContent.ProjectileType<PulseTurretShot>(),
                                                         projectile.damage,
                                                         projectile.knockBack,
                                                         projectile.owner,
                                                         0f,
                                                         1f);
                            }
                        }
                        if (!Main.dedServ)
                        {
                            for (int i = 0; i < 12; i++)
                            {
                                Dust.NewDustPerfect(shootPosition, 173).scale = Main.rand.NextFloat(1.4f, 1.8f);
                            }
                        }
                        projectile.ai[1] = 15f;
                        projectile.localAI[0] = Math.Sign(projectile.DirectionTo(potentialTarget.Center).X);
                    }
                }
            }
            else
            {
                projectile.rotation = projectile.rotation.AngleLerp(0f, MathHelper.TwoPi / 50f);
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D standTexture = ModContent.GetTexture("CalamityMod/Projectiles/DraedonsArsenal/PulseTurretStand");
            spriteBatch.Draw(ModContent.GetTexture(Texture),
                             projectile.Center - ((standTexture.Height / 2 + 6f) * Vector2.UnitY) - Main.screenPosition,
                             null,
                             lightColor, 
                             projectile.rotation,
                             projectile.Size * 0.5f,
                             projectile.scale,
                             projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally,
                             0f);
            spriteBatch.Draw(standTexture,
                             projectile.Center - Main.screenPosition,
                             null,
                             lightColor,
                             0f,
                             standTexture.Size() * 0.5f,
                             projectile.scale,
                             SpriteEffects.None,
                             0f);
            return false;
        }

        public override bool CanDamage() => false;
        public override bool OnTileCollide(Vector2 oldVelocity) => false;
    }
}
