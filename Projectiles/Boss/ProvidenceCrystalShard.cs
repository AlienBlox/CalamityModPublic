﻿using CalamityMod.NPCs;
using CalamityMod.NPCs.Providence;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Boss
{
    public class ProvidenceCrystalShard : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Holy Crystal Shard");
        }

        public override void SetDefaults()
        {
            Projectile.Calamity().DealsDefenseDamage = true;
            Projectile.width = 34;
            Projectile.height = 34;
            Projectile.hostile = true;
            Projectile.Opacity = 0f;
            Projectile.penetrate = 1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 600;
            CooldownSlot = ImmunityCooldownID.Bosses;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(Projectile.localAI[0]);
            writer.Write(Projectile.localAI[1]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            Projectile.localAI[0] = reader.ReadSingle();
            Projectile.localAI[1] = reader.ReadSingle();
        }

        public override void AI()
        {
            bool healerGuardianAlive = true;
            if (CalamityGlobalNPC.doughnutBossHealer < 0 || !Main.npc[CalamityGlobalNPC.doughnutBossHealer].active)
                healerGuardianAlive = false;

            Lighting.AddLight(Projectile.Center, 0.3f * Projectile.Opacity, 0.3f * Projectile.Opacity, 0.3f * Projectile.Opacity);

            // Day mode by default but syncs with the boss
            if (CalamityGlobalNPC.holyBoss != -1)
            {
                if (Main.npc[CalamityGlobalNPC.holyBoss].active)
                    Projectile.maxPenetrate = (int)Main.npc[CalamityGlobalNPC.holyBoss].localAI[1];
            }
            else
                Projectile.maxPenetrate = (int)Providence.BossMode.Day;
            
            // Night AI or Guardian Healer
            if (Projectile.maxPenetrate != (int)Providence.BossMode.Day || healerGuardianAlive)
                Projectile.extraUpdates = 1;

            if (Projectile.timeLeft < 300)
                Projectile.tileCollide = true;

            Color newColor2 = Main.hslToRgb(Projectile.ai[0], 1f, 0.5f);

            if (Projectile.Opacity < 1f)
                Projectile.Opacity += 0.03f;

            if (Projectile.Opacity > 1f)
                Projectile.Opacity = 1f;

            if (Projectile.Opacity == 1f)
                Lighting.AddLight(Projectile.Center, newColor2.ToVector3() * 0.5f);

            Projectile.velocity.X *= 0.995f;
            if (Projectile.velocity.Y < 0f)
            {
                Projectile.velocity.Y *= 0.98f;
            }
            else
            {
                Projectile.velocity.Y *= 1.06f;
                float fallSpeed = (CalamityWorld.revenge || (Projectile.maxPenetrate != (int)Providence.BossMode.Day)) ? 3.5f : 3f;
                if (Projectile.velocity.Y > fallSpeed)
                    Projectile.velocity.Y = fallSpeed;
            }

            if (Projectile.velocity.Y > -0.5f && Projectile.localAI[1] == 0f)
            {
                Projectile.localAI[1] = 1f;
                Projectile.velocity.Y = 0.5f;
            }

            Projectile.rotation = (float)Math.Atan2(Projectile.velocity.Y, Projectile.velocity.X) - MathHelper.PiOver2;

            for (int num979 = 0; num979 < 2; num979++)
            {
                if (Main.rand.NextBool(10))
                {
                    Vector2 value55 = Vector2.UnitY.RotatedBy(num979 * MathHelper.Pi).RotatedBy(Projectile.rotation);
                    Dust dust24 = Main.dust[Dust.NewDust(Projectile.Center, 0, 0, 267, 0f, 0f, 225, newColor2, 1.5f)];
                    dust24.noGravity = true;
                    dust24.noLight = true;
                    dust24.scale = Projectile.Opacity * Projectile.localAI[0];
                    dust24.position = Projectile.Center;
                    dust24.velocity = value55;
                }
            }

            for (int num980 = 0; num980 < 2; num980++)
            {
                if (Main.rand.NextBool(10))
                {
                    Vector2 value56 = Vector2.UnitY.RotatedBy(num980 * MathHelper.Pi);
                    Dust dust25 = Main.dust[Dust.NewDust(Projectile.Center, 0, 0, 267, 0f, 0f, 225, newColor2, 1.5f)];
                    dust25.noGravity = true;
                    dust25.noLight = true;
                    dust25.scale = Projectile.Opacity * Projectile.localAI[0];
                    dust25.position = Projectile.Center;
                    dust25.velocity = value56;
                }
            }

            if (Main.rand.NextBool(10))
            {
                float scaleFactor13 = 1f + Main.rand.NextFloat() * 2f;
                float fadeIn = 1f + Main.rand.NextFloat();
                float num981 = 1f + Main.rand.NextFloat();
                Vector2 vector136 = Utils.RandomVector2(Main.rand, -1f, 1f);
                if (vector136 != Vector2.Zero)
                {
                    vector136.Normalize();
                }
                vector136 *= 16f + Main.rand.NextFloat() * 16f;
                Vector2 vector137 = Projectile.Center + vector136;
                Point point3 = vector137.ToTileCoordinates();
                bool flag52 = true;
                if (!WorldGen.InWorld(point3.X, point3.Y, 0))
                {
                    flag52 = false;
                }
                if (flag52 && WorldGen.SolidTile(point3.X, point3.Y))
                {
                    flag52 = false;
                }
                if (flag52)
                {
                    Dust dust26 = Main.dust[Dust.NewDust(vector137, 0, 0, 267, 0f, 0f, 127, newColor2, 1f)];
                    dust26.noGravity = true;
                    dust26.position = vector137;
                    dust26.velocity = -Vector2.UnitY * scaleFactor13 * (Main.rand.NextFloat() * 0.9f + 1.6f);
                    dust26.fadeIn = fadeIn;
                    dust26.scale = num981;
                    dust26.noLight = true;
                    Dust dust27 = Dust.CloneDust(dust26);
                    Dust dust = dust27;
                    dust.scale *= 0.65f;
                    dust = dust27;
                    dust.fadeIn *= 0.65f;
                    dust27.color = new Color(255, 255, 255, 255);
                }
            }
        }

        public override void Kill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item27, Projectile.Center);
            Vector2 spinningpoint = new Vector2(0f, -3f).RotatedByRandom(MathHelper.Pi);
            float num69 = Main.rand.Next(7, 13);
            Vector2 value5 = new Vector2(1.6f, 1.5f);
            Color newColor = Main.hslToRgb(Projectile.ai[0], 1f, 0.5f);
            newColor.A = 255;
            for (float num70 = 0f; num70 < num69; num70++)
            {
                int num71 = Dust.NewDust(Projectile.Center, 0, 0, 267, 0f, 0f, 0, newColor, 1f);
                Main.dust[num71].position = Projectile.Center;
                Main.dust[num71].velocity = spinningpoint.RotatedBy(MathHelper.TwoPi * num70 / num69) * value5 * (0.8f + Main.rand.NextFloat() * 0.4f);
                Main.dust[num71].noGravity = true;
                Main.dust[num71].scale = 2f;
                Main.dust[num71].fadeIn = Main.rand.NextFloat() * 2f;
                Dust dust11 = Dust.CloneDust(num71);
                Dust dust = dust11;
                dust.scale /= 2f;
                dust = dust11;
                dust.fadeIn /= 2f;
                dust11.color = new Color(255, 255, 255, 255);
            }
            for (float num73 = 0f; num73 < num69; num73++)
            {
                int num74 = Dust.NewDust(Projectile.Center, 0, 0, 267, 0f, 0f, 0, newColor, 1f);
                Main.dust[num74].position = Projectile.Center;
                Main.dust[num74].velocity = spinningpoint.RotatedBy(MathHelper.TwoPi * num73 / num69) * value5 * (0.8f + Main.rand.NextFloat() * 0.4f);
                Dust dust = Main.dust[num74];
                dust.velocity *= Main.rand.NextFloat() * 0.8f;
                dust.noGravity = true;
                dust.scale = Main.rand.NextFloat() * 1f;
                dust.fadeIn = Main.rand.NextFloat() * 2f;
                Dust dust12 = Dust.CloneDust(num74);
                dust = dust12;
                dust.scale /= 2f;
                dust = dust12;
                dust.fadeIn /= 2f;
                dust12.color = new Color(255, 255, 255, 255);
            }
        }

        public override Color? GetAlpha(Color lightColor) => new Color(255 * Projectile.Opacity, 255 * Projectile.Opacity, 255 * Projectile.Opacity, 0);

        public override void ModifyHitPlayer(Player target, ref Player.HurtModifiers modifiers)
        {
            // In GFB, "real damage" is replaced with negative healing
            if (Projectile.maxPenetrate >= (int)Providence.BossMode.Red)
                damage = 0;

            // If the player is dodging, don't apply debuffs
            if (damage <= 0 && Projectile.maxPenetrate < (int)Providence.BossMode.Red || target.creativeGodMode)
                return;

            ProvUtils.ApplyHitEffects(target, Projectile.maxPenetrate, 0, 10);
        }
    }
}
