﻿using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Graphics.Metaballs;
using CalamityMod.Items.Weapons.Ranged;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Ranged
{
    // Photoviscerator right click main projectile (invisible flare cluster bomb)
    public class ExoFlareCluster : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Ranged";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public int Time = 0;
        public Color sparkColor;
        public bool PostTileHit = false;
        public ref int audioCooldown => ref Main.player[Projectile.owner].Calamity().PhotoAudioCooldown;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.NeedsUUID[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 50;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = 20;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 6;
            Projectile.extraUpdates = 1;
            Projectile.timeLeft = 300;
        }

        public override void AI()
        {
            Time++;
            sparkColor = Main.rand.Next(4) switch
            {
                0 => Color.Red,
                1 => Color.MediumTurquoise,
                2 => Color.Orange,
                _ => Color.LawnGreen,
            };
            PhotoMetaball.SpawnParticle(Projectile.Center, 90);
            PhotoMetaball2.SpawnParticle(Projectile.Center, 85);
            CalamityUtils.HomeInOnNPC(Projectile, false, 600f, 12f, 20f);
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<MiracleBlight>(), 600);
            float numberOflines = 2;
            float rotFactorlines = 360f / numberOflines;
            for (int i = 0; i < numberOflines; i++)
            {
                float rot = MathHelper.ToRadians(i * rotFactorlines);
                Vector2 offset = (Vector2.UnitX * Main.rand.NextFloat(0.2f, 3.1f)).RotatedBy(rot + Main.rand.NextFloat(0.1f, 5.1f));
                Vector2 velOffset = (Vector2.UnitX * Main.rand.NextFloat(0.2f, 3.1f)).RotatedBy(rot + Main.rand.NextFloat(0.1f, 5.1f));
                SparkParticle spark = new SparkParticle(Projectile.Center + offset, velOffset * Main.rand.NextFloat(3.5f, 6.5f), true, 95, Main.rand.NextFloat(0.3f, 0.8f), Color.White);
                GeneralParticleHandler.SpawnParticle(spark);

                float rot2 = MathHelper.ToRadians(i * rotFactorlines);
                Vector2 offset2 = (Vector2.UnitX * Main.rand.NextFloat(0.2f, 3.1f)).RotatedBy(rot2 + Main.rand.NextFloat(0.1f, 5.1f));
                Vector2 velOffset2 = (Vector2.UnitX * Main.rand.NextFloat(0.2f, 3.1f)).RotatedBy(rot2 + Main.rand.NextFloat(0.1f, 5.1f));

                SquishyLightParticle exoEnergy = new(Projectile.Center + offset2, velOffset2 * Main.rand.NextFloat(0.5f, 2.5f), 0.5f, sparkColor, 35);
                GeneralParticleHandler.SpawnParticle(exoEnergy);
            }
            if (audioCooldown == 0)
            {
                SoundEngine.PlaySound(Photoviscerator.HitSound, target.Center);
                audioCooldown = 6;
            }
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            SoundEngine.PlaySound(DeadSunsWind.Ricochet with { Volume = 1.2f }, Projectile.Center);
            if (!PostTileHit)
            {
                float numberOflines = 25;
                float rotFactorlines = 360f / numberOflines;
                for (int i = 0; i < numberOflines; i++)
                {
                    sparkColor = Main.rand.Next(4) switch
                    {
                        0 => Color.Red,
                        1 => Color.MediumTurquoise,
                        2 => Color.Orange,
                        _ => Color.LawnGreen,
                    };

                    float rot2 = MathHelper.ToRadians(i * rotFactorlines);
                    Vector2 offset2 = (Vector2.UnitX * Main.rand.NextFloat(0.2f, 3.1f)).RotatedBy(rot2 * Main.rand.NextFloat(1.1f, 9.1f));
                    Vector2 velOffset2 = (Vector2.UnitX * Main.rand.NextFloat(0.2f, 3.1f)).RotatedBy(rot2 * Main.rand.NextFloat(1.1f, 9.1f));

                    SquishyLightParticle exoEnergy = new(Projectile.Center + offset2, velOffset2 * Main.rand.NextFloat(0.2f, 1.9f), 0.5f, sparkColor, 40);
                    GeneralParticleHandler.SpawnParticle(exoEnergy);
                }
                PostTileHit = true;
            }

            if (Projectile.velocity.X != oldVelocity.X)
            {
                Projectile.velocity.X = -oldVelocity.X;
            }
            if (Projectile.velocity.Y != oldVelocity.Y)
            {
                Projectile.velocity.Y = -oldVelocity.Y;
            }
            return false;
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(ModContent.BuffType<MiracleBlight>(), 600);
        }
    }
}
