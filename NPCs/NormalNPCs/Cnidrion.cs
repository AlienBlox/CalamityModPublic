﻿using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
namespace CalamityMod.NPCs
{
    public class Cnidrion : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cnidrion");
            Main.npcFrameCount[npc.type] = 10;
        }

        public override void SetDefaults()
        {
            npc.npcSlots = 3f;
            npc.aiStyle = -1;
            npc.damage = 20;
            npc.width = 160;
            npc.height = 80;
            npc.defense = 6;
            npc.Calamity().RevPlusDR(0.05f);
            npc.lifeMax = 400;
            npc.knockBackResist = 0.05f;
            aiType = -1;
            npc.value = Item.buyPrice(0, 0, 25, 0);
            npc.HitSound = SoundID.NPCHit12;
            npc.DeathSound = SoundID.NPCDeath18;
            npc.rarity = 2;
            banner = npc.type;
            bannerItem = ModContent.ItemType<CnidrionBanner>();
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.playerSafe || spawnInfo.player.Calamity().ZoneSunkenSea)
            {
                return 0f;
            }
            return SpawnCondition.DesertCave.Chance * 0.0175f;
        }

        public override void FindFrame(int frameHeight)
        {
            npc.frameCounter += 0.1f;
            npc.frameCounter %= Main.npcFrameCount[npc.type];
            int frame = (int)npc.frameCounter;
            npc.frame.Y = frame * frameHeight;
        }

        public override void AI()
        {
            Player player = Main.player[npc.target];
            bool expertMode = Main.expertMode;
            npc.noGravity = true;
            npc.noTileCollide = true;
            npc.spriteDirection = (npc.direction > 0) ? 1 : -1;
            float num823 = 1f;
            npc.TargetClosest(true);
            bool flag51 = false;
            int offsetX = 80;
            if ((double)npc.life < (double)npc.lifeMax * 0.33)
            {
                num823 = 2f;
            }
            if ((double)npc.life < (double)npc.lifeMax * 0.1)
            {
                num823 = 4f;
            }
            if (npc.ai[0] == 0f)
            {
                npc.ai[1] += 1f;
                if ((double)npc.life < (double)npc.lifeMax * 0.33)
                {
                    npc.ai[1] += 1f;
                }
                if ((double)npc.life < (double)npc.lifeMax * 0.1)
                {
                    npc.ai[1] += 1f;
                }
                if (npc.ai[1] >= 300f && Main.netMode != NetmodeID.MultiplayerClient)
                {
                    npc.ai[1] = 0f;
                    if ((double)npc.life < (double)npc.lifeMax * 0.25)
                    {
                        npc.ai[0] = (float)Main.rand.Next(3, 5);
                    }
                    else
                    {
                        npc.ai[0] = (float)Main.rand.Next(1, 3);
                    }
                    npc.netUpdate = true;
                }
            }
            else if (npc.ai[0] == 1f)
            {
                flag51 = true;
                npc.ai[1] += 1f;
                if (npc.ai[1] % 30f == 0f)
                {
                    Vector2 vector18 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + 20f);
                    vector18.X += (float)(offsetX * npc.direction);
                    float num829 = Main.player[npc.target].position.X + (float)Main.player[npc.target].width * 0.5f - vector18.X;
                    float num830 = Main.player[npc.target].position.Y - vector18.Y;
                    float num831 = (float)Math.Sqrt((double)(num829 * num829 + num830 * num830));
                    float num832 = 6f;
                    num831 = num832 / num831;
                    num829 *= num831;
                    num830 *= num831;
                    num829 *= 1f + (float)Main.rand.Next(-10, 11) * 0.01f;
                    num830 *= 1f + (float)Main.rand.Next(-10, 11) * 0.01f;
                    Projectile.NewProjectile(vector18.X, vector18.Y, num829, num830, ModContent.ProjectileType<HorsWaterBlast>(), expertMode ? 8 : 11, 0f, Main.myPlayer, 0f, 0f);
                }
                if (npc.ai[1] >= 120f)
                {
                    npc.ai[1] = 0f;
                    npc.ai[0] = 0f;
                }
            }
            else if (npc.ai[0] == 2f)
            {
                flag51 = true;
                npc.ai[1] += 1f;
                if (npc.ai[1] > 60f && npc.ai[1] < 240f && npc.ai[1] % 16f == 0f)
                {
                    Vector2 vector18 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + 20f);
                    vector18.X += (float)(offsetX * npc.direction);
                    float num829 = Main.player[npc.target].position.X + (float)Main.player[npc.target].width * 0.5f - vector18.X;
                    float num830 = Main.player[npc.target].position.Y - vector18.Y;
                    float num831 = (float)Math.Sqrt((double)(num829 * num829 + num830 * num830));
                    float num832 = 8f;
                    num831 = num832 / num831;
                    num829 *= num831;
                    num830 *= num831;
                    num829 *= 1f + (float)Main.rand.Next(-15, 16) * 0.01f;
                    num830 *= 1f + (float)Main.rand.Next(-15, 16) * 0.01f;
                    Projectile.NewProjectile(vector18.X, vector18.Y, num829, num830, ModContent.ProjectileType<HorsWaterBlast>(), expertMode ? 9 : 12, 0f, Main.myPlayer, 0f, 0f);
                }
                if (npc.ai[1] >= 300f)
                {
                    npc.ai[1] = 0f;
                    npc.ai[0] = 0f;
                }
            }
            else if (npc.ai[0] == 3f)
            {
                num823 = 4f;
                npc.ai[1] += 1f;
                if (npc.ai[1] % 30f == 0f)
                {
                    Vector2 vector18 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + 20f);
                    vector18.X += (float)(offsetX * npc.direction);
                    float num844 = Main.player[npc.target].position.X + (float)Main.player[npc.target].width * 0.5f - vector18.X;
                    float num845 = Main.player[npc.target].position.Y - vector18.Y;
                    float num846 = (float)Math.Sqrt((double)(num844 * num844 + num845 * num845));
                    float num847 = 10f;
                    num846 = num847 / num846;
                    num844 *= num846;
                    num845 *= num846;
                    num844 *= 1f + (float)Main.rand.Next(-10, 11) * 0.001f;
                    num845 *= 1f + (float)Main.rand.Next(-10, 11) * 0.001f;
                    int num848 = Projectile.NewProjectile(vector18.X, vector18.Y, num844, num845, ModContent.ProjectileType<HorsWaterBlast>(), expertMode ? 11 : 14, 0f, Main.myPlayer, 0f, 0f);
                }
                if (npc.ai[1] >= 120f)
                {
                    npc.ai[1] = 0f;
                    npc.ai[0] = 0f;
                }
            }
            else if (npc.ai[0] == 4f)
            {
                num823 = 4f;
                npc.ai[1] += 1f;
                if (npc.ai[1] % 20f == 0f)
                {
                    Vector2 vector18 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + 20f);
                    vector18.X += (float)(offsetX * npc.direction);
                    float num829 = Main.player[npc.target].position.X + (float)Main.player[npc.target].width * 0.5f - vector18.X;
                    float num830 = Main.player[npc.target].position.Y - vector18.Y;
                    float num831 = (float)Math.Sqrt((double)(num829 * num829 + num830 * num830));
                    float num832 = 11f;
                    num831 = num832 / num831;
                    num829 *= num831;
                    num830 *= num831;
                    num829 *= 1f + (float)Main.rand.Next(-5, 6) * 0.01f;
                    num830 *= 1f + (float)Main.rand.Next(-5, 6) * 0.01f;
                    int num833 = Projectile.NewProjectile(vector18.X, vector18.Y, num829, num830, ModContent.ProjectileType<HorsWaterBlast>(), expertMode ? 13 : 17, 0f, Main.myPlayer, 0f, 0f);
                }
                if (npc.ai[1] >= 240f)
                {
                    npc.ai[1] = 0f;
                    npc.ai[0] = 0f;
                }
            }
            if (Math.Abs(npc.Center.X - Main.player[npc.target].Center.X) < 50f)
            {
                flag51 = true;
            }
            if (flag51)
            {
                npc.velocity.X = npc.velocity.X * 0.9f;
                if ((double)npc.velocity.X > -0.1 && (double)npc.velocity.X < 0.1)
                {
                    npc.velocity.X = 0f;
                }
            }
            else
            {
                if (npc.direction > 0)
                {
                    npc.velocity.X = (npc.velocity.X * 20f + num823) / 21f;
                }
                if (npc.direction < 0)
                {
                    npc.velocity.X = (npc.velocity.X * 20f - num823) / 21f;
                }
            }
            int num854 = 80;
            int num855 = 20;
            Vector2 position2 = new Vector2(npc.Center.X - (float)(num854 / 2), npc.position.Y + (float)npc.height - (float)num855);
            bool flag52 = false;
            if (npc.position.X < Main.player[npc.target].position.X && npc.position.X + (float)npc.width > Main.player[npc.target].position.X + (float)Main.player[npc.target].width && npc.position.Y + (float)npc.height < Main.player[npc.target].position.Y + (float)Main.player[npc.target].height - 16f)
            {
                flag52 = true;
            }
            if (flag52)
            {
                npc.velocity.Y = npc.velocity.Y + 0.5f;
            }
            else if (Collision.SolidCollision(position2, num854, num855))
            {
                if (npc.velocity.Y > 0f)
                {
                    npc.velocity.Y = 0f;
                }
                if ((double)npc.velocity.Y > -0.2)
                {
                    npc.velocity.Y = npc.velocity.Y - 0.025f;
                }
                else
                {
                    npc.velocity.Y = npc.velocity.Y - 0.2f;
                }
                if (npc.velocity.Y < -4f)
                {
                    npc.velocity.Y = -4f;
                }
            }
            else
            {
                if (npc.velocity.Y < 0f)
                {
                    npc.velocity.Y = 0f;
                }
                if ((double)npc.velocity.Y < 0.1)
                {
                    npc.velocity.Y = npc.velocity.Y + 0.025f;
                }
                else
                {
                    npc.velocity.Y = npc.velocity.Y + 0.5f;
                }
            }
            if (npc.velocity.Y > 10f)
            {
                npc.velocity.Y = 10f;
            }
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(npc.position, npc.width, npc.height, 5, hitDirection, -1f, 0, default, 1f);
            }
            if (npc.life <= 0)
            {
                for (int k = 0; k < 40; k++)
                {
                    Dust.NewDust(npc.position, npc.width, npc.height, 5, hitDirection, -1f, 0, default, 2f);
                }
            }
        }

        public override void NPCLoot()
        {
            Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ItemID.Coral, Main.rand.Next(1, 4));
            Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ItemID.Starfish, Main.rand.Next(1, 4));
            Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ItemID.Seashell, Main.rand.Next(1, 4));
            Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ModContent.ItemType<VictoryShard>(), Main.rand.Next(1, 4));
            if (Main.rand.NextBool(4))
            {
                if (Main.rand.NextBool(25))
                    Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ModContent.ItemType<TheTransformer>());
                else
                    Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ModContent.ItemType<AmidiasSpark>());
            }
        }
    }
}
