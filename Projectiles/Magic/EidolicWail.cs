﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria; using CalamityMod.Projectiles; using Terraria.ModLoader; using CalamityMod.Dusts;
using Terraria.ID;
using Terraria.ModLoader; using CalamityMod.Dusts; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles; using CalamityMod.Tiles; using CalamityMod.Walls;

namespace CalamityMod.Projectiles
{
    public class EidolicWail : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Wail");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 8;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            projectile.width = 36;
            projectile.height = 36;
            projectile.scale = 0.005f;
            projectile.alpha = 100;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.penetrate = -1;
            projectile.magic = true;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 2;
            projectile.timeLeft = 450;
        }

        public override void AI()
        {
            if (projectile.localAI[0] < 1f)
            {
                projectile.localAI[0] += 0.005f; //200 to reach full size and max power
                projectile.scale += 0.005f;
                projectile.width = (int)(36f * projectile.scale);
                projectile.height = (int)(36f * projectile.scale);
            }
            else
            {
                projectile.width = 36;
                projectile.height = 36;
            }
            projectile.rotation = (float)Math.Atan2((double)projectile.velocity.Y, (double)projectile.velocity.X);
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (projectile.velocity.X != oldVelocity.X)
            {
                projectile.velocity.X = -oldVelocity.X;
            }
            if (projectile.velocity.Y != oldVelocity.Y)
            {
                projectile.velocity.Y = -oldVelocity.Y;
            }
            return false;
        }

        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            damage = (int)((double)damage * (double)projectile.localAI[0]);
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<Buffs.CrushDepth>(), 600);
            projectile.velocity *= 0.975f;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            if (projectile.timeLeft < 85)
            {
                byte b2 = (byte)(projectile.timeLeft * 3);
                byte a2 = (byte)(100f * ((float)b2 / 255f));
                return new Color((int)b2, (int)b2, (int)b2, (int)a2);
            }
            return new Color(255, 255, 255, 100);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityGlobalProjectile.DrawCenteredAndAfterimage(projectile, lightColor, ProjectileID.Sets.TrailingMode[projectile.type], 1);
            return false;
        }
    }
}
