﻿using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Projectiles.BaseProjectiles;
using CalamityMod.Projectiles.Ranged;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Melee.Spears
{
    public class EarthenPikeSpear : BaseSpearProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Pike");
        }

        public override void SetDefaults()
        {
            Projectile.width = 40;  //The width of the .png file in pixels divided by 2.
            Projectile.aiStyle = ProjAIStyleID.Spear;
            Projectile.DamageType = DamageClass.Melee;  //Dictates whether projectile is a melee-class weapon.
            Projectile.timeLeft = 90;
            Projectile.height = 40;  //The height of the .png file in pixels divided by 2.
            Projectile.scale = 1.5f;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.ownerHitCheck = true;
        }

        public override float InitialSpeed => 3f;
        public override float ReelbackSpeed => 2.4f;
        public override float ForwardSpeed => 0.95f;
        public override void ExtraBehavior()
        {
            if (Main.rand.NextBool(4))
                Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, 32, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f);

            Projectile.localAI[0] += 1f;
            if (Projectile.localAI[0] >= 6f)
            {
                Projectile.localAI[0] = 0f;
                if (Main.myPlayer == Projectile.owner)
                {
                    float velocityY = Projectile.velocity.Y * 1.25f;
                    if (velocityY < 0.1f)
                        velocityY = 0.1f;
                    int proj = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center.X + Projectile.velocity.X, Projectile.Center.Y + Projectile.velocity.Y,
                        Projectile.velocity.X * 1.25f, velocityY, ModContent.ProjectileType<FossilShard>(), (int)(Projectile.damage * 0.5), 0f, Projectile.owner, 0f, 0f);
                    if (proj.WithinBounds(Main.maxProjectiles))
                        Main.projectile[proj].DamageType = DamageClass.Melee;
                }
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.immune[Projectile.owner] = 8;
            target.AddBuff(ModContent.BuffType<ArmorCrunch>(), 300);
        }
    }
}
