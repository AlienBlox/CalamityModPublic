﻿using CalamityMod.Dusts;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon
{
    public class EyeOfNightCell : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cell");
            ProjectileID.Sets.MinionShot[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.width = Projectile.height = 10;
            Projectile.minion = true;
            Projectile.minionSlots = 0f;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = 2;
            Projectile.timeLeft = 180;
            Projectile.extraUpdates = 1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 90;
            Projectile.DamageType = DamageClass.Summon;
        }

        public override void AI()
        {
            if (!Main.dedServ && Projectile.velocity.Length() > 5f)
                Dust.NewDustPerfect(Projectile.Center, (int)CalamityDusts.SulfurousSeaAcid).noGravity = true;

            Projectile.StickyProjAI(3);
        }
        public override void Kill(int timeLeft)
        {
            if (!Main.dedServ)
            {
                for (int i = 0; i < 10; i++)
                    Dust.NewDustDirect(Projectile.position, 36, 36, (int)CalamityDusts.SulfurousSeaAcid).noGravity = true;
            }
        }

        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection) => Projectile.ModifyHitNPCSticky(4, true);
    }
}
