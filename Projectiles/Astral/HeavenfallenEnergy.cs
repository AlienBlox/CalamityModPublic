﻿using Microsoft.Xna.Framework;
using Terraria; using CalamityMod.Projectiles; using Terraria.ModLoader;
using Terraria.ModLoader; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles; using CalamityMod.Tiles; using CalamityMod.Walls;

namespace CalamityMod.Projectiles
{
    public class HeavenfallenEnergy : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Heavenfallen Energy");
        }

        public override void SetDefaults()
        {
            projectile.width = 8;
            projectile.height = 8;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.penetrate = 1;
            projectile.timeLeft = 300;
            projectile.Calamity().rogue = true;
        }

        public override void AI()
        {
            int num154 = 14;
            int coolDust;
            projectile.ai[0] += 1;
            if (projectile.ai[0] % 2 == 0)
            {
                if (projectile.ai[0] % 4 == 0)
                {
                    coolDust = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width - num154 * 2, projectile.height - num154 * 2, ModContent.DustType<AstralBlue>(), 0f, 0f, 100, default, 1.5f);
                }
                else
                {
                    coolDust = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width - num154 * 2, projectile.height - num154 * 2, ModContent.DustType<AstralOrange>(), 0f, 0f, 100, default, 1.5f);
                }
                Main.dust[coolDust].noGravity = true;
                Main.dust[coolDust].velocity *= 0.1f;
                Main.dust[coolDust].velocity += projectile.velocity * 0.5f;
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<AstralInfectionDebuff>(), 180);
        }
    }
}
