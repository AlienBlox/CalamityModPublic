﻿using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public class BarrelShrapnel : ModProjectile
    {
        public bool hitTile = false;
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 3;
        }

        public override void SetDefaults()
        {
            Projectile.width = 12;
            Projectile.height = 14;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 160;
            Projectile.tileCollide = true;
            Projectile.DamageType = RogueDamageClass.Instance;
            Projectile.ignoreWater = true;
        }
        public override void AI()
        {
            Projectile.ai[0] += 1f;
            if (hitTile)
            {
                Projectile.velocity.X = 0f;
                Projectile.rotation = MathHelper.Pi;
            }
            else
            {
                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            }
            Projectile.velocity.Y += 0.2f;
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            hitTile = true;
            return false;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.OnFire3, 60 * 4);
            Projectile.Kill();
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(BuffID.OnFire3, 60 * 4);
            Projectile.Kill();
        }
    }
}
