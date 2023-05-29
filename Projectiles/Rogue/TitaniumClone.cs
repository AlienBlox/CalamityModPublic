﻿using Microsoft.Xna.Framework;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public class TitaniumClone : ModProjectile, ILocalizedModType
    {
        public string LocalizationCategory => "Projectiles.Rogue";
        public override string Texture => "CalamityMod/Items/Weapons/Rogue/TitaniumShuriken";

        private static float RotationIncrement = 0.22f;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = 34;
            Projectile.height = 34;
            Projectile.alpha = 150;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 200;
            Projectile.DamageType = RogueDamageClass.Instance;
            Projectile.extraUpdates = 1;
        }

        public override void AI()
        {
            Projectile.rotation += RotationIncrement;
            Projectile.ai[0] += 1f;
            if (Projectile.ai[0] > 30f)
            {
                CalamityUtils.HomeInOnNPC(Projectile, true, 200f, 12f, 20f);
            }
        }

        public override bool? CanDamage() => Projectile.ai[0] >= 30f ? null : false;

        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1);
            return false;
        }
    }
}
