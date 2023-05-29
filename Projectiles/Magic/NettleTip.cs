﻿using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Magic
{
    public class NettleTip : ModProjectile, ILocalizedModType
    {
        public string LocalizationCategory => "Projectiles.Magic";
        public override void SetDefaults()
        {
            Projectile.width = 28;
            Projectile.height = 28;
            Projectile.aiStyle = ProjAIStyleID.Vilethorn;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.tileCollide = false;
            Projectile.alpha = 255;
            Projectile.ignoreWater = true;
            Projectile.DamageType = DamageClass.Magic;
            AIType = ProjectileID.NettleBurstEnd;
        }
    }
}
