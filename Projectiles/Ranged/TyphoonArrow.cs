﻿using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Ranged
{
    public class TyphoonArrow : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Arrow");
        }

        public override void SetDefaults()
        {
            Projectile.width = 14;
            Projectile.height = 14;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.arrow = true;
            Projectile.penetrate = 1;
            Projectile.aiStyle = 1;
            Projectile.timeLeft = 600;
            AIType = ProjectileID.WoodenArrowFriendly;
            Projectile.Calamity().pointBlankShotDuration = CalamityGlobalProjectile.DefaultPointBlankDuration;
        }

        public override void Kill(int timeLeft)
        {
            if (Projectile.owner == Main.myPlayer)
            {
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center.X, Projectile.Center.Y, 0f, 0f, ModContent.ProjectileType<TyphoonBubble>(), Projectile.damage, Projectile.knockBack, Projectile.owner, 0f, 0f);
            }
        }
    }
}
