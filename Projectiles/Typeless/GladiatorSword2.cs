﻿using CalamityMod.CalPlayer;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Typeless
{
    public class GladiatorSword2 : ModProjectile
    {
        private double rotation = 0;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Gladiator Sword");
            ProjectileID.Sets.MinionSacrificable[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.width = 22;
            projectile.height = 22;
            projectile.ignoreWater = true;
            projectile.timeLeft = 18000;
            projectile.tileCollide = false;
            projectile.friendly = true;
            projectile.timeLeft *= 5;
            projectile.penetrate = -1;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 10 -
                (Main.hardMode ? 2 : 0) -
                (NPC.downedPlantBoss ? 2 : 0) -
                (NPC.downedMoonlord ? 2 : 0) -
                (CalamityWorld.downedDoG ? 2 : 0);
        }

        public override void AI()
        {
            bool flag64 = projectile.type == ModContent.ProjectileType<GladiatorSword2>();
            Player player = Main.player[projectile.owner];
            CalamityPlayer modPlayer = player.Calamity();
            if (projectile.localAI[0] == 0f)
            {
                projectile.Calamity().spawnedPlayerMinionDamageValue = player.AverageDamage();
                projectile.Calamity().spawnedPlayerMinionProjectileDamageValue = projectile.damage;
                projectile.localAI[0] += 1f;
            }
            if (player.AverageDamage() != projectile.Calamity().spawnedPlayerMinionDamageValue)
            {
                int damage2 = (int)((float)projectile.Calamity().spawnedPlayerMinionProjectileDamageValue /
                    projectile.Calamity().spawnedPlayerMinionDamageValue *
                    player.AverageDamage());
                projectile.damage = damage2;
            }
            if (!modPlayer.gladiatorSword)
            {
                projectile.active = false;
                return;
            }
            if (flag64)
            {
                if (player.dead)
                {
                    modPlayer.glSword = false;
                }
                if (modPlayer.glSword)
                {
                    projectile.timeLeft = 2;
                }
            }
            Lighting.AddLight(projectile.Center, (255 - projectile.alpha) * 0.15f / 255f, (255 - projectile.alpha) * 0.15f / 255f, (255 - projectile.alpha) * 0.01f / 255f);
            Vector2 vector = player.Center - projectile.Center;
            projectile.rotation = vector.ToRotation() - 1.57f;
            projectile.Center = player.Center + new Vector2(80, 0).RotatedBy(rotation);
			double rotateAmt = CalamityWorld.downedDoG ? 0.09 : NPC.downedMoonlord ? 0.07 : NPC.downedPlantBoss ? 0.04 : Main.hardMode ? 0.03 : 0.02;
			//values are slightly different from the other sword to make this sword marginally slower so the intersection point isn't always at the same spot
            rotation -= rotateAmt;
            if (rotation <= 0)
            {
                rotation = 360;
            }
            projectile.velocity.X = (vector.X > 0f) ? -0.000001f : 0f;
        }
    }
}
