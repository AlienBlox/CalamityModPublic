﻿using CalamityMod.Buffs.StatDebuffs;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Enemy
{
    public class MaulerAcidDrop : ModProjectile, ILocalizedModType
    {
        public string LocalizationCategory => "Projectiles.Enemy";
        public override string Texture => "CalamityMod/Projectiles/Environment/AcidDrop";
        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.penetrate = -1;
            Projectile.hostile = true;
            Projectile.timeLeft = 240;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = false;
        }
        public override void AI()
        {
            float homingSpeed = 16f;
            Player target = Main.player[Player.FindClosest(Projectile.Center, 1, 1)];
            if (Projectile.WithinRange(target.Center, 1200f) && Projectile.timeLeft < 210)
                Projectile.velocity = (Projectile.velocity * 59f + Projectile.SafeDirectionTo(target.Center) * homingSpeed) / 60f;
            Projectile.Opacity = Utils.GetLerpValue(0f, 25f, Projectile.timeLeft, true);
            Projectile.rotation = Projectile.velocity.ToRotation() - MathHelper.PiOver2;
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            if (info.Damage <= 0)
                return;

            target.AddBuff(ModContent.BuffType<Irradiated>(), 60);
        }

        public override Color? GetAlpha(Color lightColor)
        {
            lightColor = Color.White;
            lightColor.A = 64;
            return lightColor * Projectile.Opacity;
        }
    }
}
