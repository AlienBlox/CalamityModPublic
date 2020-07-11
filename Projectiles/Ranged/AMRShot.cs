using CalamityMod.Buffs.StatDebuffs;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Ranged
{
    public class AMRShot : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("AMR");
        }

        public override void SetDefaults()
        {
            projectile.width = 4;
            projectile.height = 4;
            projectile.light = 0.5f;
            projectile.alpha = 255;
            projectile.extraUpdates = 10;
            projectile.scale = 1.18f;
            projectile.friendly = true;
            projectile.ranged = true;
            projectile.ignoreWater = true;
            projectile.aiStyle = 1;
            aiType = ProjectileID.BulletHighVelocity;
            projectile.penetrate = 1;
            projectile.timeLeft = 600;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Collision.HitTiles(projectile.position, projectile.velocity, projectile.width, projectile.height);
            Main.PlaySound(SoundID.Dig, (int)projectile.position.X, (int)projectile.position.Y, 1, 1f, 0f);
            return true;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            if (crit)
            {
                for (int x = 0; x < 8; x++)
                {
                    float xPos = x > 4 ? projectile.Center.X + 500 : projectile.Center.X - 500;
					float yPos = projectile.Center.Y + Main.rand.Next(-500, 501);
                    Vector2 spawnPosition = new Vector2(xPos, yPos);
					Vector2 velocity = target.Center - spawnPosition;
                    float dir = velocity.Length();
                    dir = 10 / spawnPosition.X;
					velocity.X *= dir * 150;
					velocity.Y *= dir * 150;
                    if (projectile.owner == Main.myPlayer)
                    {
                        Projectile.NewProjectile(spawnPosition, velocity, ModContent.ProjectileType<AMR2>(), (int)(projectile.damage * 0.1), 1f, projectile.owner);
                    }
                }
            }
            target.AddBuff(ModContent.BuffType<MarkedforDeath>(), 600);
            if (target.defense > 50)
            {
                target.defense -= 50;
            }
        }
    }
}
