﻿using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Rogue
{
    public class EGloveSpark : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Spark");
        }

        public override void SetDefaults()
        {
            Projectile.width = 6;
            Projectile.height = 12;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 60;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 20;
            Projectile.DamageType = DamageClass.Generic; //No triggering nanoblades
        }

        public override void AI()
        {
            if (Projectile.velocity.X != Projectile.velocity.X)
            {
                Projectile.velocity.X = Projectile.velocity.X * -0.05f;
            }
            if (Projectile.velocity.X != Projectile.velocity.X)
            {
                Projectile.velocity.X = Projectile.velocity.X * -0.3f;
            }
            if (Projectile.velocity.Y != Projectile.velocity.Y && Projectile.velocity.Y > 0.5f)
            {
                Projectile.velocity.Y = Projectile.velocity.Y * -0.3f;
            }
            Projectile.ai[0] += 1f;
            if (Projectile.ai[0] > 5f)
            {
                Projectile.ai[0] = 5f;
                if (Projectile.velocity.Y == 0f && Projectile.velocity.X != 0f)
                {
                    Projectile.velocity.X = Projectile.velocity.X * 0.97f;
                    if ((double)Projectile.velocity.X > -0.01 && (double)Projectile.velocity.X < 0.01)
                    {
                        Projectile.velocity.X = 0f;
                        Projectile.netUpdate = true;
                    }
                }
                Projectile.velocity.Y = Projectile.velocity.Y + 0.2f;
            }
            Projectile.rotation += Projectile.velocity.X * 0.1f;
            int num199 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 206, 0f, 0f, 100, new Color(Main.rand.Next(20, 100), 204, 250), 1f);
            Dust expr_8976_cp_0 = Main.dust[num199];
            expr_8976_cp_0.position.X -= 2f;
            Dust expr_8994_cp_0 = Main.dust[num199];
            expr_8994_cp_0.position.Y += 2f;
            Main.dust[num199].scale += (float)Main.rand.Next(50) * 0.01f;
            Main.dust[num199].noGravity = true;
            Dust expr_89E7_cp_0 = Main.dust[num199];
            expr_89E7_cp_0.velocity.Y -= 2f;
            if (Main.rand.NextBool(2))
            {
                int num200 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 206, 0f, 0f, 100, new Color(Main.rand.Next(20, 100), 204, 250), 1f);
                Dust expr_8A4E_cp_0 = Main.dust[num200];
                expr_8A4E_cp_0.position.X -= 2f;
                Dust expr_8A6C_cp_0 = Main.dust[num200];
                expr_8A6C_cp_0.position.Y += 2f;
                Main.dust[num200].scale += 0.3f + (float)Main.rand.Next(50) * 0.01f;
                Main.dust[num200].noGravity = true;
                Main.dust[num200].velocity *= 0.1f;
            }
            if ((double)Projectile.velocity.Y < 0.25 && (double)Projectile.velocity.Y > 0.15)
            {
                Projectile.velocity.X = Projectile.velocity.X * 0.8f;
            }
            Projectile.rotation = -Projectile.velocity.X * 0.05f;
            if (Projectile.velocity.Y > 16f)
            {
                Projectile.velocity.Y = 16f;
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity) => false;
    }
}
