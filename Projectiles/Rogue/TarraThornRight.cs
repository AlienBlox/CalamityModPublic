﻿using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Rogue
{
    public class TarraThornRight : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/Magic/NettleRight";

        public override void SetDefaults()
        {
            Projectile.width = 28;
            Projectile.height = 28;
            Projectile.friendly = true;
            Projectile.penetrate = 5;
            Projectile.tileCollide = false;
            Projectile.alpha = 255;
            Projectile.ignoreWater = true;
            Projectile.DamageType = RogueDamageClass.Instance;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 10;
            Projectile.aiStyle = ProjAIStyleID.Vilethorn;
        }

        public override void AI()
        {
            Projectile.rotation = (float)Math.Atan2((double)Projectile.velocity.Y, (double)Projectile.velocity.X) + MathHelper.PiOver2;
            if (Projectile.ai[0] == 0f)
            {
                Projectile.alpha -= 50;
                if (Projectile.alpha > 0)
                    return;
                Projectile.alpha = 0;
                Projectile.ai[0] = 1f;
                if (Projectile.ai[1] == 0f)
                {
                    Projectile.ai[1] += 1f;
                    Projectile.position += Projectile.velocity * 1f;
                }
                if (Main.myPlayer == Projectile.owner)
                {
                    int type = Projectile.type;
                    if (Projectile.ai[1] >= 10f)
                        type = ModContent.ProjectileType<TarraThornTip>();
                    int number = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position.X + Projectile.velocity.X + (float)(Projectile.width / 2), Projectile.position.Y + Projectile.velocity.Y + (float)(Projectile.height / 2), Projectile.velocity.X, Projectile.velocity.Y, type, Projectile.damage, Projectile.knockBack, Projectile.owner, 0f, 0f);
                    Main.projectile[number].ai[1] = Projectile.ai[1] + 1f;
                }
            }
            else
            {
                if (Projectile.alpha < 170 && Projectile.alpha + 5 >= 170)
                {
                    for (int index1 = 0; index1 < 8; ++index1)
                    {
                        int index2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 7, Projectile.velocity.X * 0.025f, Projectile.velocity.Y * 0.025f, 200, new Color(), 1.3f);
                        Dust dust = Main.dust[index2];
                        dust.noGravity = true;
                        dust.velocity *= 0.5f;
                    }
                }
                Projectile.alpha += 3;
                if (Projectile.alpha < 255)
                    return;
                Projectile.Kill();
            }
        }
    }
}
