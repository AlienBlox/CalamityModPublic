﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Projectiles.Melee;
using CalamityMod.Buffs.DamageOverTime;

namespace CalamityMod.Projectiles.Rogue
{
    public class EpidemicShredderProjectile : ModProjectile
    {
		// This is never used
        bool justhit = false;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Epidemic Shredder");
        }

        public override void SetDefaults()
        {
            projectile.width = 34;
            projectile.height = 34;
            projectile.friendly = true;
            projectile.penetrate = 6;
            projectile.timeLeft = 600;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 40;
            projectile.Calamity().rogue = true;
            projectile.ignoreWater = true;
        }

        public override void AI()
        {
            projectile.rotation += Math.Sign(projectile.velocity.X) * MathHelper.ToRadians(10f);
            if (projectile.ai[0] > 0f)
            {
                projectile.ai[0] -= 1f;
            }
            if (projectile.timeLeft < 580f)
            {
                projectile.velocity = (projectile.velocity * 18f + projectile.DirectionTo(Main.player[projectile.owner].Center) * 18f) / 19f;
                if (Main.player[projectile.owner].Hitbox.Intersects(projectile.Hitbox))
                {
                    projectile.Kill();
                }
            }
            if (projectile.timeLeft % 5 == 0 && projectile.Calamity().stealthStrike)
            {
                int projIndex2 = Projectile.NewProjectile(projectile.Center, (projectile.velocity * -1f).RotatedByRandom(MathHelper.ToRadians(15f)), ModContent.ProjectileType<PlagueSeeker>(), (int)(projectile.damage * 0.25f), 2f, projectile.owner);
                Main.projectile[projIndex2].Calamity().forceRogue = true;
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (projectile.penetrate > 1)
            {
                if (projectile.velocity.X != oldVelocity.X)
                {
                    projectile.velocity.X = -oldVelocity.X;
                }
                if (projectile.velocity.Y != oldVelocity.Y)
                {
                    projectile.velocity.Y = -oldVelocity.Y;
                }
                if (projectile.ai[0] == 0f)
                {
                    int projIndex1 = Projectile.NewProjectile(projectile.Center, projectile.velocity, ModContent.ProjectileType<PlagueSeeker>(), (int)(projectile.damage * 0.25f), 2f, projectile.owner);
                    Main.projectile[projIndex1].Calamity().forceRogue = true;
                    projectile.ai[0] = 12f; //0.2th of a second cooldown
                }
                projectile.penetrate--;
            }
            else
                projectile.tileCollide = false;

            return false;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {        
            if (projectile.ai[0] == 0f)
            {
                int projectileIndex = Projectile.NewProjectile(projectile.Center, projectile.velocity, ModContent.ProjectileType<PlagueSeeker>(), (int)(projectile.damage * 0.25f), 2f, projectile.owner);
                Main.projectile[projectileIndex].Calamity().forceRogue = true;
                projectile.ai[0] = 12f; //0.2th of a second cooldown
            }
            target.AddBuff(ModContent.BuffType<Plague>(), 300);
        }
    }
}