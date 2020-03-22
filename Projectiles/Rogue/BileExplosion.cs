﻿using CalamityMod.Dusts;
using CalamityMod.Buffs.StatDebuffs;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public class BileExplosion : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Bile");
        }

        public override void SetDefaults()
        {
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.penetrate = -1;
            projectile.timeLeft = 60;
            projectile.Calamity().rogue = true;
        }

        public override void AI()
        {
            projectile.position = projectile.Center;
            if (projectile.Calamity().stealthStrike)
                projectile.width = projectile.height = 120;
            else
                projectile.width = projectile.height = 70;
            projectile.position -= projectile.Size / 2f;
            for (int i = 0; i < 15; i++)
            {
                Dust dust = Dust.NewDustPerfect(projectile.Center, (int)CalamityDusts.SulfurousSeaAcid);
                dust.velocity = projectile.width / 33.333f * Vector2.One.RotatedByRandom(MathHelper.TwoPi);
                dust.scale = projectile.width == 120 ? 3.1f : 2.2f;
                dust.noGravity = true;
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.immune[projectile.owner] = 9;
            target.AddBuff(ModContent.BuffType<Irradiated>(), 180);
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            target.AddBuff(ModContent.BuffType<Irradiated>(), 180);
        }
    }
}
