﻿using Microsoft.Xna.Framework;
using System;
using System.IO;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using CalamityMod.Items.Placeables.Banners;
using CalamityMod.Items.Tools.ClimateChange;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Ranged;
namespace CalamityMod.NPCs
{
    public class Horse : ModNPC
    {
        private int chargetimer = 0;
        private int basespeed = 1;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Earth Elemental");
            Main.npcFrameCount[npc.type] = 6;
        }

        public override void SetDefaults()
        {
            npc.npcSlots = 3f;
            npc.damage = 50;
            npc.width = 230;
            npc.height = 230;
            npc.defense = 20;
            npc.Calamity().RevPlusDR(0.1f);
            npc.lifeMax = 3800;
            npc.aiStyle = -1;
            aiType = -1;
            for (int k = 0; k < npc.buffImmune.Length; k++)
            {
                npc.buffImmune[k] = true;
            }
            npc.buffImmune[BuffID.Ichor] = false;
            npc.buffImmune[BuffID.CursedInferno] = false;
            npc.knockBackResist = 0f;
            npc.value = Item.buyPrice(0, 1, 50, 0);
            npc.noGravity = true;
            npc.noTileCollide = true;
            npc.HitSound = SoundID.NPCHit4;
            npc.rarity = 2;
            banner = npc.type;
            bannerItem = ModContent.ItemType<EarthElementalBanner>();
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(chargetimer);
            writer.Write(basespeed);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            chargetimer = reader.ReadInt32();
            basespeed = reader.ReadInt32();
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.playerSafe || !Main.hardMode || spawnInfo.player.Calamity().ZoneAbyss ||
                spawnInfo.player.Calamity().ZoneSunkenSea)
            {
                return 0f;
            }
            return SpawnCondition.Cavern.Chance * 0.005f;
        }

        public override void FindFrame(int frameHeight)
        {
            npc.frameCounter++;
            if (npc.frameCounter >= 8)
            {
                npc.frame.Y = (npc.frame.Y + frameHeight) % (Main.npcFrameCount[npc.type] * frameHeight);
                npc.frameCounter = 0;
            }
        }

        public override void NPCLoot()
        {
            if (Main.rand.NextBool(3))
            {
                Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ModContent.ItemType<AridArtifact>());
            }
            if (Main.rand.NextBool(4))
            {
                Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ModContent.ItemType<SlagMagnum>());
            }
            if (Main.rand.NextBool(4))
            {
                Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ModContent.ItemType<Items.Aftershock>());
            }
            if (Main.rand.NextBool(4))
            {
                Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ModContent.ItemType<Items.EarthenPike>());
            }
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(npc.position, npc.width, npc.height, 31, hitDirection, -1f, 0, default, 1f);
            }
            if (npc.life <= 0)
            {
                Main.PlaySound(2, (int)npc.position.X, (int)npc.position.Y, 14);
                npc.position.X = npc.position.X + (float)(npc.width / 2);
                npc.position.Y = npc.position.Y + (float)(npc.height / 2);
                npc.width = 160;
                npc.height = 160;
                npc.position.X = npc.position.X - (float)(npc.width / 2);
                npc.position.Y = npc.position.Y - (float)(npc.height / 2);
                for (int num621 = 0; num621 < 40; num621++)
                {
                    int num622 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 31, 0f, 0f, 100, default, 2f);
                    Main.dust[num622].velocity *= 3f;
                    if (Main.rand.NextBool(2))
                    {
                        Main.dust[num622].scale = 0.5f;
                        Main.dust[num622].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                    }
                }
                for (int num623 = 0; num623 < 70; num623++)
                {
                    int num624 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 6, 0f, 0f, 100, default, 3f);
                    Main.dust[num624].noGravity = true;
                    Main.dust[num624].velocity *= 5f;
                    num624 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 6, 0f, 0f, 100, default, 2f);
                    Main.dust[num624].velocity *= 2f;
                }
                for (int num625 = 0; num625 < 3; num625++)
                {
                    float scaleFactor10 = 0.33f;
                    if (num625 == 1)
                    {
                        scaleFactor10 = 0.66f;
                    }
                    if (num625 == 2)
                    {
                        scaleFactor10 = 1f;
                    }
                    int num626 = Gore.NewGore(new Vector2(npc.position.X + (float)(npc.width / 2) - 24f, npc.position.Y + (float)(npc.height / 2) - 24f), default, Main.rand.Next(61, 64), 1f);
                    Main.gore[num626].velocity *= scaleFactor10;
                    Gore expr_13AB6_cp_0 = Main.gore[num626];
                    expr_13AB6_cp_0.velocity.X += 1f;
                    Gore expr_13AD6_cp_0 = Main.gore[num626];
                    expr_13AD6_cp_0.velocity.Y += 1f;
                    num626 = Gore.NewGore(new Vector2(npc.position.X + (float)(npc.width / 2) - 24f, npc.position.Y + (float)(npc.height / 2) - 24f), default, Main.rand.Next(61, 64), 1f);
                    Main.gore[num626].velocity *= scaleFactor10;
                    Gore expr_13B79_cp_0 = Main.gore[num626];
                    expr_13B79_cp_0.velocity.X -= 1f;
                    Gore expr_13B99_cp_0 = Main.gore[num626];
                    expr_13B99_cp_0.velocity.Y += 1f;
                    num626 = Gore.NewGore(new Vector2(npc.position.X + (float)(npc.width / 2) - 24f, npc.position.Y + (float)(npc.height / 2) - 24f), default, Main.rand.Next(61, 64), 1f);
                    Main.gore[num626].velocity *= scaleFactor10;
                    Gore expr_13C3C_cp_0 = Main.gore[num626];
                    expr_13C3C_cp_0.velocity.X += 1f;
                    Gore expr_13C5C_cp_0 = Main.gore[num626];
                    expr_13C5C_cp_0.velocity.Y -= 1f;
                    num626 = Gore.NewGore(new Vector2(npc.position.X + (float)(npc.width / 2) - 24f, npc.position.Y + (float)(npc.height / 2) - 24f), default, Main.rand.Next(61, 64), 1f);
                    Main.gore[num626].velocity *= scaleFactor10;
                    Gore expr_13CFF_cp_0 = Main.gore[num626];
                    expr_13CFF_cp_0.velocity.X -= 1f;
                    Gore expr_13D1F_cp_0 = Main.gore[num626];
                    expr_13D1F_cp_0.velocity.Y -= 1f;
                }
            }
        }

        public override bool PreAI()
        {
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                npc.localAI[0] += 1f;
                if (npc.localAI[0] >= 300f)
                {
                    npc.localAI[0] = 0f;
                    Main.PlaySound(3, (int)(npc.position.X + (float)(npc.width / 2)), (int)(npc.position.Y + (float)(npc.height / 2)), 41);
                    npc.TargetClosest(true);
                    if (Collision.CanHit(npc.position, npc.width, npc.height, Main.player[npc.target].position, Main.player[npc.target].width, Main.player[npc.target].height))
                    {
                        float num179 = 4f;
                        Vector2 value9 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
                        float num180 = Main.player[npc.target].position.X + (float)Main.player[npc.target].width * 0.5f - value9.X;
                        float num181 = Math.Abs(num180) * 0.1f;
                        float num182 = Main.player[npc.target].position.Y + (float)Main.player[npc.target].height * 0.5f - value9.Y - num181;
                        float num183 = (float)Math.Sqrt((double)(num180 * num180 + num182 * num182));
                        npc.netUpdate = true;
                        num183 = num179 / num183;
                        num180 *= num183;
                        num182 *= num183;
                        int num184 = 30;
                        int num185 = ModContent.ProjectileType<EarthRockSmall>();
                        value9.X += num180;
                        value9.Y += num182;
                        for (int num186 = 0; num186 < 4; num186++)
                        {
                            num185 = Main.rand.NextBool(4) ? ModContent.ProjectileType<EarthRockBig>() : ModContent.ProjectileType<EarthRockSmall>();
                            num180 = Main.player[npc.target].position.X + (float)Main.player[npc.target].width * 0.5f - value9.X;
                            num182 = Main.player[npc.target].position.Y + (float)Main.player[npc.target].height * 0.5f - value9.Y;
                            num183 = (float)Math.Sqrt((double)(num180 * num180 + num182 * num182));
                            num183 = num179 / num183;
                            num180 += (float)Main.rand.Next(-40, 41);
                            num182 += (float)Main.rand.Next(-40, 41);
                            num180 *= num183;
                            num182 *= num183;
                            Projectile.NewProjectile(value9.X, value9.Y, num180, num182, num185, num184, 0f, Main.myPlayer, 0f, 0f);
                        }
                    }
                }
            }
            if ((double)Math.Abs(npc.velocity.X) > 0.2)
            {
                npc.spriteDirection = npc.direction;
            }
            bool expertMode = Main.expertMode;
            npc.TargetClosest(true);
            Vector2 direction = Main.player[npc.target].Center - npc.Center;
            direction.Normalize();
            chargetimer += expertMode ? 2 : 1;
            if (chargetimer >= Math.Sqrt(npc.life) * 14.0)
            {
                if (Main.rand.Next(25) == 1)
                {
                    direction.X *= 6f;
                    direction.Y *= 6f;
                    npc.velocity = direction;
                    chargetimer = 0;
                }
            }
            if (Math.Sqrt((npc.velocity.X * npc.velocity.X) + (npc.velocity.Y * npc.velocity.Y)) > basespeed)
            {
                npc.velocity *= 0.985f;
            }
            if (Math.Sqrt((npc.velocity.X * npc.velocity.X) + (npc.velocity.Y * npc.velocity.Y)) <= basespeed * 1.15)
            {
                npc.velocity = direction * basespeed;
            }
            return false;

        }
    }
}
