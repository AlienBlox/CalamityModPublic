﻿using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Rogue
{
    public class BrackishWaterBlast : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Rogue";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            Projectile.width = 130;
            Projectile.height = 130;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 60;
            Projectile.DamageType = RogueDamageClass.Instance;
        }

        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, (255 - Projectile.alpha) * 0f / 255f, (255 - Projectile.alpha) * 0.5f / 255f, (255 - Projectile.alpha) * 0.75f / 255f);
            int randomDust = Utils.SelectRandom(Main.rand, new int[]
            {
                33,
                89
            });
            if (Projectile.localAI[0] == 0f)
            {
                SoundEngine.PlaySound(SoundID.Item21, Projectile.position);
                Projectile.localAI[0] += 1f;
            }
            bool flag15 = false;
            bool flag16 = false;
            if (Projectile.velocity.X < 0f && Projectile.position.X < Projectile.ai[0])
            {
                flag15 = true;
            }
            if (Projectile.velocity.X > 0f && Projectile.position.X > Projectile.ai[0])
            {
                flag15 = true;
            }
            if (Projectile.velocity.Y < 0f && Projectile.position.Y < Projectile.ai[1])
            {
                flag16 = true;
            }
            if (Projectile.velocity.Y > 0f && Projectile.position.Y > Projectile.ai[1])
            {
                flag16 = true;
            }
            if (flag15 && flag16)
            {
                Projectile.Kill();
            }
            float num461 = 25f;
            if (Projectile.ai[0] > 180f)
            {
                num461 -= (Projectile.ai[0] - 180f) / 2f;
            }
            if (num461 <= 0f)
            {
                num461 = 0f;
                Projectile.Kill();
            }
            num461 *= 0.7f;
            Projectile.ai[0] += 4f;
            float num462 = 0f;
            while (num462 < num461)
            {
                float num463 = (float)Main.rand.Next(-10, 11);
                float num464 = (float)Main.rand.Next(-10, 11);
                float num465 = (float)Main.rand.Next(3, 9);
                float num466 = (float)Math.Sqrt((double)(num463 * num463 + num464 * num464));
                num466 = num465 / num466;
                num463 *= num466;
                num464 *= num466;
                int num467 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, randomDust, 0f, 0f, 100, default, 0.5f);
                Dust dust = Main.dust[num467];
                dust.noGravity = true;
                dust.position.X = Projectile.Center.X;
                dust.position.Y = Projectile.Center.Y;
                dust.position.X += (float)Main.rand.Next(-10, 11);
                dust.position.Y += (float)Main.rand.Next(-10, 11);
                dust.velocity.X = num463;
                dust.velocity.Y = num464;
                num462++;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.immune[Projectile.owner] = 6;
            target.AddBuff(BuffID.Venom, 90);
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(BuffID.Venom, 90);
        }
    }
}
