﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using CalamityMod.Sounds;

namespace CalamityMod.Projectiles.Ranged
{
    public class ImpactRound : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/Ranged/AMRShot";

        private bool initialized = false;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Impact Round");
        }

        public override void SetDefaults()
        {
            Projectile.width = 4;
            Projectile.height = 4;
            Projectile.light = 0.5f;
            Projectile.alpha = 255;
            Projectile.extraUpdates = 7;
            Projectile.scale = 1.18f;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.ignoreWater = true;
            Projectile.aiStyle = 1;
            AIType = ProjectileID.BulletHighVelocity;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 600;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
            Projectile.Calamity().pointBlankShotDuration = CalamityGlobalProjectile.basePointBlankShotDuration;
        }

        public override void AI()
        {
            if (!initialized && Projectile.CountsAsClass<RangedDamageClass>()) //Ranged check prevents quiver splits triggering the sound
            {
                initialized = true;

                if (Main.netMode != NetmodeID.Server)
                {
                    SoundEngine.PlaySound(CommonCalamitySounds.LargeWeaponFireSound with { Volume = CommonCalamitySounds.LargeWeaponFireSound.Volume * 0.45f}, Projectile.Center);
                }
            }
        }

        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            double damageMult = 1D;
            if (crit)
                damageMult += 0.25;
            damage = (int)(damage * damageMult);
        }

        public override bool PreDraw(ref Color lightColor) => Projectile.timeLeft < 600;
    }
}
