﻿using CalamityMod.CalPlayer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria; using CalamityMod.Projectiles; using Terraria.ModLoader; using CalamityMod.Dusts;
using Terraria.ID;
using Terraria.ModLoader; using CalamityMod.Dusts; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles; using CalamityMod.Tiles; using CalamityMod.Walls;

namespace CalamityMod.Projectiles
{
    public class Calamari : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Calamari");
            Main.projFrames[projectile.type] = 5;
            ProjectileID.Sets.MinionSacrificable[projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.width = 40;
            projectile.height = 40;
            projectile.netImportant = true;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.minionSlots = 1f;
            projectile.timeLeft = 18000;
            projectile.penetrate = -1;
            projectile.timeLeft *= 5;
            projectile.minion = true;
            projectile.tileCollide = false;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 30;
        }

        public override void AI()
        {
            if (projectile.localAI[1] == 0f)
            {
                projectile.Calamity().spawnedPlayerMinionDamageValue = Main.player[projectile.owner].minionDamage; //66% = 1.66
                projectile.Calamity().spawnedPlayerMinionProjectileDamageValue = projectile.damage; //300 * 1.66 = 498 (new value)
                int num226 = 36;
                for (int num227 = 0; num227 < num226; num227++)
                {
                    Vector2 vector6 = Vector2.Normalize(projectile.velocity) * new Vector2((float)projectile.width / 2f, (float)projectile.height) * 0.75f;
                    vector6 = vector6.RotatedBy((double)((float)(num227 - (num226 / 2 - 1)) * 6.28318548f / (float)num226), default) + projectile.Center;
                    Vector2 vector7 = vector6 - projectile.Center;
                    int num228 = Dust.NewDust(vector6 + vector7, 0, 0, 109, vector7.X * 1.5f, vector7.Y * 1.5f, 100, default, 1.4f);
                    Main.dust[num228].noGravity = true;
                    Main.dust[num228].noLight = true;
                    Main.dust[num228].velocity = vector7;
                }
                projectile.localAI[1] += 1f;
            }
            if (Main.player[projectile.owner].minionDamage != projectile.Calamity().spawnedPlayerMinionDamageValue) //15% = 1.15 != 1.66
            {
                int damage2 = (int)((float)projectile.Calamity().spawnedPlayerMinionProjectileDamageValue / //498
                    projectile.Calamity().spawnedPlayerMinionDamageValue * //1.66 498 / 1.66 = 300 (original value)
                    Main.player[projectile.owner].minionDamage); //300 * 1.15 = 345 (new value)
                projectile.damage = damage2;
            }
            projectile.frameCounter++;
            if (projectile.frameCounter > 8)
            {
                projectile.frame++;
                projectile.frameCounter = 0;
            }
            if (projectile.frame > 4)
            {
                projectile.frame = 0;
            }
            bool flag64 = projectile.type == ModContent.ProjectileType<Calamari>();
            Player player = Main.player[projectile.owner];
            CalamityPlayer modPlayer = player.Calamity();
            player.AddBuff(ModContent.BuffType<Calamari>(), 3600);
            if (flag64)
            {
                if (player.dead)
                {
                    modPlayer.calamari = false;
                }
                if (modPlayer.calamari)
                {
                    projectile.timeLeft = 2;
                }
            }
            if (Main.rand.NextBool(600))
            {
                Main.PlaySound(29, (int)projectile.position.X, (int)projectile.position.Y, 35);
            }
            if (projectile.ai[0] == 2f)
            {
                projectile.ai[1] -= 1f;
                if (projectile.ai[1] > 3f)
                {
                    Main.PlaySound(29, (int)projectile.position.X, (int)projectile.position.Y, 34);
                    int num = Dust.NewDust(projectile.Center, 0, 0, 109, projectile.velocity.X, projectile.velocity.Y, 100, default, 1.4f);
                    Main.dust[num].scale = 0.5f + (float)Main.rand.NextDouble() * 0.3f;
                    Main.dust[num].velocity /= 2.5f;
                    Main.dust[num].noGravity = true;
                    Main.dust[num].noLight = true;
                }
                if (projectile.ai[1] != 0f)
                {
                    return;
                }
                projectile.ai[1] = 15f;
                projectile.ai[0] = 0f;
                projectile.velocity /= 5f;
                projectile.velocity.Y = 0f;
                projectile.extraUpdates = 0;
                projectile.numUpdates = 0;
                projectile.netUpdate = true;
                projectile.extraUpdates = 0;
                projectile.numUpdates = 0;
            }
            if (projectile.extraUpdates > 1)
            {
                projectile.extraUpdates = 0;
            }
            if (projectile.numUpdates > 1)
            {
                projectile.numUpdates = 0;
            }
            if (projectile.localAI[0] > 0f)
            {
                projectile.localAI[0] -= 1f;
            }
            int num3;
            for (int num534 = 0; num534 < 1000; num534 = num3 + 1)
            {
                if (num534 != projectile.whoAmI && Main.projectile[num534].active && Main.projectile[num534].owner == projectile.owner &&
                    Main.projectile[num534].type == ModContent.ProjectileType<Calamari>() &&
                    Math.Abs(projectile.position.X - Main.projectile[num534].position.X) + Math.Abs(projectile.position.Y - Main.projectile[num534].position.Y) < (float)projectile.width)
                {
                    if (projectile.position.X < Main.projectile[num534].position.X)
                    {
                        projectile.velocity.X = projectile.velocity.X - 0.05f;
                    }
                    else
                    {
                        projectile.velocity.X = projectile.velocity.X + 0.05f;
                    }
                    if (projectile.position.Y < Main.projectile[num534].position.Y)
                    {
                        projectile.velocity.Y = projectile.velocity.Y - 0.05f;
                    }
                    else
                    {
                        projectile.velocity.Y = projectile.velocity.Y + 0.05f;
                    }
                }
                num3 = num534;
            }
            Vector2 vector = projectile.position;
            float num10 = 300f; //300
            bool flag = false;
            Vector2 center = Main.player[projectile.owner].Center;
            Vector2 value = new Vector2(0.5f);
            value.Y = 0f;
            if (player.HasMinionAttackTargetNPC)
            {
                NPC npc = Main.npc[player.MinionAttackTargetNPC];
                if (npc.CanBeChasedBy(projectile, false))
                {
                    Vector2 vector2 = npc.position + npc.Size * value;
                    float num12 = Vector2.Distance(vector2, center);
                    if (((Vector2.Distance(center, vector) > num12 && num12 < num10) || !flag) && Collision.CanHitLine(projectile.position, projectile.width, projectile.height, npc.position, npc.width, npc.height))
                    {
                        vector = vector2;
                        flag = true;
                        int num11 = npc.whoAmI;
                    }
                }
            }
            else
            {
                for (int k = 0; k < 200; k++)
                {
                    NPC nPC = Main.npc[k];
                    if (nPC.CanBeChasedBy(projectile, false))
                    {
                        Vector2 vector3 = nPC.position + nPC.Size * value;
                        float num13 = Vector2.Distance(vector3, center);
                        if (((Vector2.Distance(center, vector) > num13 && num13 < num10) || !flag) && Collision.CanHitLine(projectile.position, projectile.width, projectile.height, nPC.position, nPC.width, nPC.height))
                        {
                            num10 = num13;
                            vector = vector3;
                            flag = true;
                        }
                    }
                }
            }
            int num16 = 500;
            if (flag)
            {
                num16 = 2000;
            }
            if (Vector2.Distance(player.Center, projectile.Center) > (float)num16)
            {
                projectile.ai[0] = 1f;
                projectile.netUpdate = true;
            }
            if (flag && projectile.ai[0] == 0f)
            {
                Vector2 vector4 = vector - projectile.Center;
                float num17 = vector4.Length();
                vector4.Normalize();
                vector4 = vector - Vector2.UnitY * 80f;
                int num18 = (int)vector4.Y / 16;
                if (num18 < 0)
                {
                    num18 = 0;
                }
                Tile tile = Main.tile[(int)vector4.X / 16, num18];
                if (tile != null && tile.active() && Main.tileSolid[(int)tile.type] && !Main.tileSolidTop[(int)tile.type])
                {
                    vector4 += Vector2.UnitY * 16f;
                    tile = Main.tile[(int)vector4.X / 16, (int)vector4.Y / 16];
                    if (tile != null && tile.active() && Main.tileSolid[(int)tile.type] && !Main.tileSolidTop[(int)tile.type])
                    {
                        vector4 += Vector2.UnitY * 16f;
                    }
                }
                vector4 -= projectile.Center;
                num17 = vector4.Length();
                vector4.Normalize();
                if (num17 > 300f && num17 <= 800f && projectile.localAI[0] == 0f)
                {
                    projectile.ai[0] = 2f;
                    projectile.ai[1] = (float)(int)(num17 / 5f); //10
                    projectile.extraUpdates = (int)(projectile.ai[1] * 2f);
                    projectile.velocity = vector4 * 5f; //10
                    projectile.localAI[0] = 60f;
                    return;
                }
                if (num17 > 200f)
                {
                    float scaleFactor2 = 9f; //6
                    vector4 *= scaleFactor2;
                    projectile.velocity.X = (projectile.velocity.X * 40f + vector4.X) / 41f;
                    projectile.velocity.Y = (projectile.velocity.Y * 40f + vector4.Y) / 41f;
                }
                if (num17 > 70f && num17 < 130f)
                {
                    float scaleFactor3 = 14f; //7
                    if (num17 < 100f)
                    {
                        scaleFactor3 = -6f; //-3
                    }
                    vector4 *= scaleFactor3;
                    projectile.velocity = (projectile.velocity * 20f + vector4) / 21f;
                    if (Math.Abs(vector4.X) > Math.Abs(vector4.Y))
                    {
                        projectile.velocity.X = (projectile.velocity.X * 10f + vector4.X) / 11f;
                    }
                }
                else
                {
                    projectile.velocity *= 0.97f;
                }
            }
            else
            {
                if (!Collision.CanHitLine(projectile.Center, 1, 1, Main.player[projectile.owner].Center, 1, 1))
                {
                    projectile.ai[0] = 1f;
                }
                float num21 = 6f; //6
                if (projectile.ai[0] == 1f)
                {
                    num21 = 22.5f; //15
                }
                Vector2 center2 = projectile.Center;
                Vector2 vector6 = player.Center - center2 + new Vector2(0f, -60f);
                float num23 = vector6.Length();
                if (num23 > 200f && num21 < 13.5f)
                {
                    num21 = 13.5f; //9
                }
                if (num23 < 400f && projectile.ai[0] == 1f)
                {
                    projectile.ai[0] = 0f;
                    projectile.netUpdate = true;
                }
                if (num23 > 3000f)
                {
                    projectile.position.X = Main.player[projectile.owner].Center.X - (float)(projectile.width / 2);
                    projectile.position.Y = Main.player[projectile.owner].Center.Y - (float)(projectile.width / 2);
                }
                if (num23 > 70f)
                {
                    vector6.Normalize();
                    vector6 *= num21;
                    projectile.velocity = (projectile.velocity * 20f + vector6) / 21f;
                }
                else
                {
                    if (projectile.velocity.X == 0f && projectile.velocity.Y == 0f)
                    {
                        projectile.velocity.X = -0.15f;
                        projectile.velocity.Y = -0.05f;
                    }
                    projectile.velocity *= 1.01f;
                }
            }
            projectile.rotation = projectile.velocity.X * 0.025f;
            if (projectile.ai[1] > 0f)
            {
                projectile.ai[1] += 1f;
                if (Main.rand.Next(3) != 0)
                {
                    projectile.ai[1] += 1f;
                }
            }
            if (projectile.ai[1] > 15f)
            {
                projectile.ai[1] = 0f;
                projectile.netUpdate = true;
            }
            if (projectile.ai[0] == 0f)
            {
                float scaleFactor4 = 9f;
                int num28 = ModContent.ProjectileType<CalamariInk>();
                if (flag)
                {
                    if (Math.Abs((vector - projectile.Center).ToRotation() - 1.57079637f) > 0.7853982f)
                    {
                        projectile.velocity += Vector2.Normalize(vector - projectile.Center - Vector2.UnitY * 80f); //80
                        return;
                    }
                    if ((vector - projectile.Center).Length() <= 400f && projectile.ai[1] == 0f) //400
                    {
                        projectile.ai[1] += 1f;
                        if (Main.myPlayer == projectile.owner)
                        {
                            Main.PlaySound(2, (int)projectile.position.X, (int)projectile.position.Y, 111);
                            Vector2 vector7 = vector - projectile.Center;
                            vector7.Normalize();
                            vector7 *= scaleFactor4;
                            Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y + 20, vector7.X, vector7.Y, num28, projectile.damage, 0f, Main.myPlayer, 0f, 0f);
                            projectile.netUpdate = true;
                        }
                    }
                }
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture2D13 = Main.projectileTexture[projectile.type];
            int num214 = Main.projectileTexture[projectile.type].Height / Main.projFrames[projectile.type];
            int y6 = num214 * projectile.frame;
            Main.spriteBatch.Draw(texture2D13, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(0, y6, texture2D13.Width, num214)), projectile.GetAlpha(lightColor), projectile.rotation, new Vector2((float)texture2D13.Width / 2f, (float)num214 / 2f), projectile.scale, SpriteEffects.None, 0f);
            return false;
        }

        public override bool CanDamage()
        {
            return false;
        }
    }
}
