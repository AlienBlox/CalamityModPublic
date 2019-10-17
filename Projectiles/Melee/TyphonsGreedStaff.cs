﻿using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;
using Terraria.Enums;
using CalamityMod.Buffs;
namespace CalamityMod.Projectiles
{
    public class TyphonsGreedStaff : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Typhon's Greed");
        }

        public override void SetDefaults()
        {
            projectile.width = 110;
            projectile.height = 110;
            projectile.friendly = true;
            projectile.alpha = 255;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
            projectile.timeLeft = 300;
            projectile.melee = true;
            projectile.hide = true;
            projectile.ignoreWater = true;
            projectile.ownerHitCheck = true;
        }

        public override void AI()
        {
            float num = 50f;
            float num2 = 2f;
            float scaleFactor = 20f;
            Player player = Main.player[projectile.owner];
            float num3 = -0.7853982f;
            Vector2 value = player.RotatedRelativePoint(player.MountedCenter, true);
            Vector2 value2 = Vector2.Zero;
            if (player.dead)
            {
                projectile.Kill();
                return;
            }
            Lighting.AddLight(player.Center, 0f, 0.2f, 1.45f);
            int num9 = Math.Sign(projectile.velocity.X);
            projectile.velocity = new Vector2((float)num9, 0f);
            if (projectile.ai[0] == 0f)
            {
                projectile.rotation = new Vector2((float)num9, -player.gravDir).ToRotation() + num3 + 3.14159274f;
                if (projectile.velocity.X < 0f)
                {
                    projectile.rotation -= 1.57079637f;
                }
            }
            projectile.alpha -= 128;
            if (projectile.alpha < 0)
            {
                projectile.alpha = 0;
            }
            float arg_5DB_0 = projectile.ai[0] / num;
            float num10 = 1f;
            projectile.ai[0] += num10;
            projectile.rotation += 6.28318548f * num2 / num * (float)num9;
            bool flag2 = projectile.ai[0] == (float)(int)(num / 2f);
            if (projectile.ai[0] >= num || (flag2 && !player.controlUseItem))
            {
                projectile.Kill();
                player.reuseDelay = 2;
            }
            else if (flag2)
            {
                Vector2 mouseWorld2 = Main.MouseWorld;
                int num11 = (player.DirectionTo(mouseWorld2).X > 0f) ? 1 : -1;
                if ((float)num11 != projectile.velocity.X)
                {
                    player.ChangeDir(num11);
                    projectile.velocity = new Vector2((float)num11, 0f);
                    projectile.netUpdate = true;
                    projectile.rotation -= 3.14159274f;
                }
            }
            if ((projectile.ai[0] == num10 || (projectile.ai[0] == (float)(int)(num / 2f) && projectile.active)) && projectile.owner == Main.myPlayer)
            {
                Vector2 mouseWorld3 = Main.MouseWorld;
                Vector2 mouse = player.DirectionTo(mouseWorld3) * 0f;
                player.DirectionTo(mouse);
            }
            float num12 = projectile.rotation - 0.7853982f * (float)num9;
            value2 = (num12 + ((num9 == -1) ? 3.14159274f : 0f)).ToRotationVector2() * (projectile.ai[0] / num) * scaleFactor;
            Vector2 value3 = projectile.Center + (num12 + ((num9 == -1) ? 3.14159274f : 0f)).ToRotationVector2() * 30f;
            Vector2 vector2 = num12.ToRotationVector2();
            Vector2 value4 = vector2.RotatedBy((double)(1.57079637f * (float)projectile.spriteDirection), default);
            if (Main.rand.NextBool(2))
            {
                Dust dust3 = Dust.NewDustDirect(value3 - new Vector2(5f), 10, 10, 33, player.velocity.X, player.velocity.Y, 150, default, 1f);
                dust3.velocity = projectile.DirectionTo(dust3.position) * 0.1f + dust3.velocity * 0.1f;
            }
            for (int j = 0; j < 4; j++)
            {
                float scaleFactor2 = 1f;
                float scaleFactor3 = 1f;
                switch (j)
                {
                    case 1:
                        scaleFactor3 = -1f;
                        break;
                    case 2:
                        scaleFactor3 = 1.25f;
                        scaleFactor2 = 0.5f;
                        break;
                    case 3:
                        scaleFactor3 = -1.25f;
                        scaleFactor2 = 0.5f;
                        break;
                }
                if (Main.rand.Next(6) != 0)
                {
                    Dust dust4 = Dust.NewDustDirect(projectile.position, 0, 0, 186, 0f, 0f, 100, default, 1f);
                    dust4.position = projectile.Center + vector2 * (60f + Main.rand.NextFloat() * 20f) * scaleFactor3;
                    dust4.velocity = value4 * (4f + 4f * Main.rand.NextFloat()) * scaleFactor3 * scaleFactor2;
                    dust4.noGravity = true;
                    dust4.noLight = true;
                    dust4.scale = 0.5f;
                    if (Main.rand.NextBool(4))
                    {
                        dust4.noGravity = false;
                    }
                }
            }
            projectile.position = value - projectile.Size / 2f;
            projectile.position += value2;
            projectile.spriteDirection = projectile.direction;
            projectile.timeLeft = 2;
            player.ChangeDir(projectile.direction);
            player.heldProj = projectile.whoAmI;
            player.itemTime = 2;
            player.itemAnimation = 2;
            player.itemRotation = MathHelper.WrapAngle(projectile.rotation);
            projectile.localAI[0] += 1f;
            if (projectile.localAI[0] >= 12f)
            {
                projectile.localAI[0] = 0f;
                float xPos = Main.rand.NextBool(2) ? projectile.position.X + 800f : projectile.position.X - 800f;
                Vector2 vector20 = new Vector2(xPos, projectile.position.Y + (float)Main.rand.Next(-800, 801));
                float num80 = xPos;
                float speedX = (float)player.position.X - vector20.X;
                float speedY = (float)player.position.Y - vector20.Y;
                float dir = (float)Math.Sqrt((double)(speedX * speedX + speedY * speedY));
                dir = 10 / num80;
                speedX *= dir * 150;
                speedY *= dir * 150;
                if (speedX > 15f)
                {
                    speedX = 15f;
                }
                if (speedX < -15f)
                {
                    speedX = -15f;
                }
                if (speedY > 15f)
                {
                    speedY = 15f;
                }
                if (speedY < -15f)
                {
                    speedY = -15f;
                }
                if (projectile.owner == Main.myPlayer)
                {
                    float ai1 = Main.rand.NextFloat() + 0.5f;
                    Projectile.NewProjectile(vector20.X, vector20.Y, speedX, speedY, ModContent.ProjectileType<TyphonsGreed>(), projectile.damage, 2f, projectile.owner, 0.0f, ai1);
                }
            }
        }

        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            Rectangle myRect = new Rectangle((int)projectile.position.X, (int)projectile.position.Y, projectile.width, projectile.height);
            if (projectile.owner == Main.myPlayer)
            {
                for (int i = 0; i < 200; i++)
                {
                    if (Main.npc[i].active && !Main.npc[i].dontTakeDamage &&
                        ((projectile.friendly && (!Main.npc[i].friendly || projectile.type == 318 || (Main.npc[i].type == 22 && projectile.owner < 255 && Main.player[projectile.owner].killGuide) || (Main.npc[i].type == 54 && projectile.owner < 255 && Main.player[projectile.owner].killClothier))) ||
                        (projectile.hostile && Main.npc[i].friendly && !Main.npc[i].dontTakeDamageFromHostiles)) && (projectile.owner < 0 || Main.npc[i].immune[projectile.owner] == 0 || projectile.maxPenetrate == 1))
                    {
                        if (Main.npc[i].noTileCollide || !projectile.ownerHitCheck || projectile.CanHit(Main.npc[i]))
                        {
                            bool flag3;
                            if (Main.npc[i].type == 414)
                            {
                                Rectangle rect = Main.npc[i].getRect();
                                int num5 = 8;
                                rect.X -= num5;
                                rect.Y -= num5;
                                rect.Width += num5 * 2;
                                rect.Height += num5 * 2;
                                flag3 = projectile.Colliding(myRect, rect);
                            }
                            else
                            {
                                flag3 = projectile.Colliding(myRect, Main.npc[i].getRect());
                            }
                            if (flag3)
                            {
                                if (Main.npc[i].reflectingProjectiles && projectile.CanReflect())
                                {
                                    Main.npc[i].ReflectProjectile(projectile.whoAmI);
                                    return;
                                }
                                hitDirection = (Main.player[projectile.owner].Center.X < Main.npc[i].Center.X) ? 1 : -1;
                            }
                        }
                    }
                }
            }
        }

        public override void CutTiles()
        {
            float num5 = 60f;
            float f = projectile.rotation - 0.7853982f * (float)Math.Sign(projectile.velocity.X);
            DelegateMethods.tilecut_0 = TileCuttingContext.AttackProjectile;
            Utils.PlotTileLine(projectile.Center + f.ToRotationVector2() * -num5, projectile.Center + f.ToRotationVector2() * num5, (float)projectile.width * projectile.scale, new Utils.PerLinePoint(DelegateMethods.CutTiles));
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (projHitbox.Intersects(targetHitbox))
            {
                return true;
            }
            float f = projectile.rotation - 0.7853982f * (float)Math.Sign(projectile.velocity.X);
            float num2 = 0f;
            float num3 = 110f;
            if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), projectile.Center + f.ToRotationVector2() * -num3, projectile.Center + f.ToRotationVector2() * num3, 23f * projectile.scale, ref num2))
            {
                return true;
            }
            return false;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<CrushDepth>(), 240);
            target.immune[projectile.owner] = 6;
        }
    }
}
