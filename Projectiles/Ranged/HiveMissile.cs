﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
namespace CalamityMod.Projectiles.Ranged
{
    public class HiveMissile : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Ranged";
        private ref float RocketType => ref Projectile.ai[0];

        public override void SetDefaults()
        {
            Projectile.width = 14;
            Projectile.height = 14;
            Projectile.scale = 0.75f;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 95;
            Projectile.DamageType = DamageClass.Ranged;
        }

        public override void AI()
        {
            if (Math.Abs(Projectile.velocity.X) >= 8f || Math.Abs(Projectile.velocity.Y) >= 8f)
            {
                for (int i = 0; i < 2; i++)
                {
                    float halfX = 0f;
                    float halfY = 0f;
                    if (i == 1)
                    {
                        halfX = Projectile.velocity.X * 0.5f;
                        halfY = Projectile.velocity.Y * 0.5f;
                    }
                    int dust = Dust.NewDust(new Vector2(Projectile.position.X + 3f + halfX, Projectile.position.Y + 3f + halfY) - Projectile.velocity * 0.5f, Projectile.width - 8, Projectile.height - 8, 6, 0f, 0f, 100, default, 1f);
                    Main.dust[dust].scale *= 2f + (float)Main.rand.Next(10) * 0.1f;
                    Main.dust[dust].velocity *= 0.2f;
                    Main.dust[dust].noGravity = true;
                    dust = Dust.NewDust(new Vector2(Projectile.position.X + 3f + halfX, Projectile.position.Y + 3f + halfY) - Projectile.velocity * 0.5f, Projectile.width - 8, Projectile.height - 8, 31, 0f, 0f, 100, default, 0.5f);
                    Main.dust[dust].fadeIn = 1f + (float)Main.rand.Next(5) * 0.1f;
                    Main.dust[dust].velocity *= 0.05f;
                }
            }
            if (Math.Abs(Projectile.velocity.X) < 15f && Math.Abs(Projectile.velocity.Y) < 15f)
            {
                Projectile.velocity *= 1.5f;
            }
            else if (Main.rand.NextBool())
            {
                int dust2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 31, 0f, 0f, 100, default, 1f);
                Main.dust[dust2].scale = 0.1f + (float)Main.rand.Next(5) * 0.1f;
                Main.dust[dust2].fadeIn = 1.5f + (float)Main.rand.Next(5) * 0.1f;
                Main.dust[dust2].noGravity = true;
                Main.dust[dust2].position = Projectile.Center + new Vector2(0f, (float)(-(float)Projectile.height / 2)).RotatedBy((double)Projectile.rotation, default) * 1.1f;
                Main.rand.Next(2);
                dust2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 6, 0f, 0f, 100, default, 1f);
                Main.dust[dust2].scale = 1f + (float)Main.rand.Next(5) * 0.1f;
                Main.dust[dust2].noGravity = true;
                Main.dust[dust2].position = Projectile.Center + new Vector2(0f, (float)(-(float)Projectile.height / 2 - 6)).RotatedBy((double)Projectile.rotation, default) * 1.1f;
            }
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.ToRadians(90);

            if (RocketType == ItemID.DryRocket || RocketType == ItemID.WetRocket || RocketType == ItemID.LavaRocket || RocketType == ItemID.HoneyRocket)
            {
                if (Projectile.wet)
                    Projectile.timeLeft = 1;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(lightColor), Projectile.rotation, tex.Size() / 2f, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }

        public override void OnKill(int timeLeft)
        {
            Projectile.ExpandHitboxBy(80);
            Projectile.maxPenetrate = -1;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
            Projectile.Damage();
            if (Projectile.owner == Main.myPlayer)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (j % 2 != 1 || Main.rand.NextBool(3))
                    {
                        Vector2 projPos = Projectile.position;
                        Vector2 projVel = Projectile.oldVelocity;
                        projVel.Normalize();
                        projVel *= 8f;
                        float beeVelX = (float)Main.rand.Next(-35, 36) * 0.01f;
                        float beeVelY = (float)Main.rand.Next(-35, 36) * 0.01f;
                        projPos -= projVel * (float)j;
                        beeVelX += Projectile.oldVelocity.X / 6f;
                        beeVelY += Projectile.oldVelocity.Y / 6f;
                        int bee = Projectile.NewProjectile(Projectile.GetSource_FromThis(), projPos.X, projPos.Y, beeVelX, beeVelY, Main.player[Projectile.owner].beeType(), Main.player[Projectile.owner].beeDamage(Projectile.damage / 2), Main.player[Projectile.owner].beeKB(0f), Main.myPlayer);
                        if (bee.WithinBounds(Main.maxProjectiles))
                        {
                            Main.projectile[bee].penetrate = 2;
                            Main.projectile[bee].DamageType = DamageClass.Ranged;
                        }
                    }
                }
            }
            SoundEngine.PlaySound(SoundID.Item14, Projectile.position);
            for (int i = 0; i < 20; i++)
            {
                int dusty = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 90, 0f, 0f, 100, default, 2f);
                Main.dust[dusty].velocity *= 3f;
                if (Main.rand.NextBool())
                {
                    Main.dust[dusty].scale = 0.5f;
                    Main.dust[dusty].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                }
            }
            for (int j = 0; j < 30; j++)
            {
                int dusty2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 90, 0f, 0f, 100, default, 3f);
                Main.dust[dusty2].noGravity = true;
                Main.dust[dusty2].velocity *= 5f;
                dusty2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 90, 0f, 0f, 100, default, 2f);
                Main.dust[dusty2].velocity *= 2f;
            }

            if (Main.netMode != NetmodeID.Server)
            {
                Vector2 goreSource = Projectile.Center;
                int goreAmt = 3;
                Vector2 source = new Vector2(goreSource.X - 24f, goreSource.Y - 24f);
                for (int goreIndex = 0; goreIndex < goreAmt; goreIndex++)
                {
                    float velocityMult = 0.33f;
                    if (goreIndex < (goreAmt / 3))
                    {
                        velocityMult = 0.66f;
                    }
                    if (goreIndex >= (2 * goreAmt / 3))
                    {
                        velocityMult = 1f;
                    }
                    Mod mod = ModContent.GetInstance<CalamityMod>();
                    int type = Main.rand.Next(61, 64);
                    int smoke = Gore.NewGore(Projectile.GetSource_Death(), source, default, type, 1f);
                    Gore gore = Main.gore[smoke];
                    gore.velocity *= velocityMult;
                    gore.velocity.X += 1f;
                    gore.velocity.Y += 1f;
                    type = Main.rand.Next(61, 64);
                    smoke = Gore.NewGore(Projectile.GetSource_Death(), source, default, type, 1f);
                    gore = Main.gore[smoke];
                    gore.velocity *= velocityMult;
                    gore.velocity.X -= 1f;
                    gore.velocity.Y += 1f;
                    type = Main.rand.Next(61, 64);
                    smoke = Gore.NewGore(Projectile.GetSource_Death(), source, default, type, 1f);
                    gore = Main.gore[smoke];
                    gore.velocity *= velocityMult;
                    gore.velocity.X += 1f;
                    gore.velocity.Y -= 1f;
                    type = Main.rand.Next(61, 64);
                    smoke = Gore.NewGore(Projectile.GetSource_Death(), source, default, type, 1f);
                    gore = Main.gore[smoke];
                    gore.velocity *= velocityMult;
                    gore.velocity.X -= 1f;
                    gore.velocity.Y -= 1f;
                }
            }

            // Only do rocket effects for the owner client side
            if (Projectile.owner != Main.myPlayer)
                return;

            int blastRadius = 0;
            if (RocketType == ItemID.RocketII)
                blastRadius = 5;
            else if (RocketType == ItemID.RocketIV)
                blastRadius = 8;
            else if (RocketType == ItemID.MiniNukeII)
                blastRadius = 11;

            Projectile.ExpandHitboxBy(14);

            if (blastRadius > 0)
                Projectile.ExplodeTiles(blastRadius);

            Point center = Projectile.Center.ToTileCoordinates();
            DelegateMethods.v2_1 = center.ToVector2();
            DelegateMethods.f_1 = 3f;
            if (RocketType == ItemID.DryRocket)
            {
                DelegateMethods.f_1 = 3.5f;
                Utils.PlotTileArea(center.X, center.Y, DelegateMethods.SpreadDry);
            }
            else if (RocketType == ItemID.WetRocket)
            {
                Utils.PlotTileArea(center.X, center.Y, DelegateMethods.SpreadWater);
            }
            else if (RocketType == ItemID.LavaRocket)
            {
                Utils.PlotTileArea(center.X, center.Y, DelegateMethods.SpreadLava);
            }
            else if (RocketType == ItemID.HoneyRocket)
            {
                Utils.PlotTileArea(center.X, center.Y, DelegateMethods.SpreadHoney);
            }
        }
    }
}
