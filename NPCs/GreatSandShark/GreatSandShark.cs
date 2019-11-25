﻿using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Banners;
using CalamityMod.Projectiles.Boss;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.NPCs.GreatSandShark
{
    public class GreatSandShark : ModNPC
    {
        private bool resetAI = false;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Great Sand Shark");
            Main.npcFrameCount[npc.type] = 8;
        }

        public override void SetDefaults()
        {
            npc.noGravity = true;
            npc.noTileCollide = true;
            npc.npcSlots = 15f;
            npc.damage = 100;
            npc.width = 300;
            npc.height = 120;
            npc.defense = 60;
            npc.Calamity().RevPlusDR(0.25f);
            npc.lifeMax = CalamityWorld.revenge ? 11000 : 8000;
            if (CalamityWorld.death)
            {
                npc.lifeMax = 16000;
            }
            npc.aiStyle = -1;
            aiType = -1;
            npc.knockBackResist = 0f;
            npc.value = Item.buyPrice(0, 10, 0, 0);
            NPCID.Sets.TrailCacheLength[npc.type] = 8;
            NPCID.Sets.TrailingMode[npc.type] = 1;
            for (int k = 0; k < npc.buffImmune.Length; k++)
            {
                npc.buffImmune[k] = true;
            }
            npc.buffImmune[BuffID.Ichor] = false;
            npc.buffImmune[ModContent.BuffType<MarkedforDeath>()] = false;
			npc.buffImmune[BuffID.Frostburn] = false;
			npc.buffImmune[BuffID.CursedInferno] = false;
            npc.buffImmune[BuffID.Daybreak] = false;
            npc.buffImmune[ModContent.BuffType<AbyssalFlames>()] = false;
            npc.buffImmune[ModContent.BuffType<ArmorCrunch>()] = false;
            npc.buffImmune[ModContent.BuffType<DemonFlames>()] = false;
            npc.buffImmune[ModContent.BuffType<HolyFlames>()] = false;
            npc.buffImmune[ModContent.BuffType<Nightwither>()] = false;
            npc.buffImmune[ModContent.BuffType<Plague>()] = false;
            npc.buffImmune[ModContent.BuffType<Shred>()] = false;
            npc.buffImmune[ModContent.BuffType<WhisperingDeath>()] = false;
            npc.buffImmune[ModContent.BuffType<SilvaStun>()] = false;
            npc.behindTiles = true;
            npc.netAlways = true;
            npc.HitSound = SoundID.NPCHit1;
            npc.DeathSound = SoundID.NPCDeath1;
            npc.timeLeft = NPC.activeTime * 30;
            banner = npc.type;
            bannerItem = ModContent.ItemType<GreatSandSharkBanner>();
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(resetAI);
            writer.Write(npc.localAI[0]);
            writer.Write(npc.localAI[1]);
            writer.Write(npc.localAI[2]);
            writer.Write(npc.localAI[3]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            resetAI = reader.ReadBoolean();
            npc.localAI[0] = reader.ReadSingle();
            npc.localAI[1] = reader.ReadSingle();
            npc.localAI[2] = reader.ReadSingle();
            npc.localAI[3] = reader.ReadSingle();
        }

        public override void AI()
        {
            bool expertMode = Main.expertMode;
            bool revenge = CalamityWorld.revenge;
            bool death = CalamityWorld.death;
            bool lowLife = (double)npc.life <= (double)npc.lifeMax * (expertMode ? 0.75 : 0.5);
            bool lowerLife = (double)npc.life <= (double)npc.lifeMax * (expertMode ? 0.35 : 0.2);
            bool youMustDie = !Main.player[npc.target].ZoneDesert;
            CalamityUtils.StartSandstorm();
            CalamityMod.UpdateServerBoolean();
            if (npc.soundDelay <= 0)
            {
                npc.soundDelay = 480;
                Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/GreatSandSharkRoar"), (int)npc.position.X, (int)npc.position.Y);
            }
            if (npc.localAI[3] >= 1f || Vector2.Distance(Main.player[npc.target].Center, npc.Center) > 1000f)
            {
                if (!resetAI)
                {
                    npc.localAI[0] = 0f;
                    npc.ai[0] = 0f;
                    npc.ai[1] = 0f;
                    npc.ai[2] = 0f;
                    resetAI = true;
                    npc.netUpdate = true;
                }
                int num2 = expertMode ? 35 : 50;
                float num3 = expertMode ? 0.5f : 0.42f;
                float scaleFactor = expertMode ? 7.5f : 6.7f;
                int num4 = expertMode ? 28 : 30;
                float num5 = expertMode ? 15.5f : 14f;
                if (revenge || lowerLife)
                {
                    num3 *= 1.1f;
                    scaleFactor *= 1.1f;
                    num5 *= 1.1f;
                }
                if (death)
                {
                    num3 *= 1.1f;
                    scaleFactor *= 1.1f;
                    num5 *= 1.1f;
                    num4 = 25;
                }
                if (youMustDie)
                {
                    num3 *= 1.5f;
                    scaleFactor *= 1.5f;
                    num5 *= 1.5f;
                    num4 = 20;
                }
                Vector2 vector = npc.Center;
                Player player = Main.player[npc.target];
                if (npc.target < 0 || npc.target == 255 || player.dead || !player.active)
                {
                    npc.TargetClosest(true);
                    player = Main.player[npc.target];
                    npc.netUpdate = true;
                }
                if (player.dead || Vector2.Distance(player.Center, vector) > 5600f)
                {
                    npc.velocity.Y = npc.velocity.Y + 0.4f;
                    if (npc.timeLeft > 10)
                    {
                        npc.timeLeft = 10;
                    }
                    npc.ai[0] = 0f;
                    npc.ai[2] = 0f;
                }
                float num17 = (float)Math.Atan2((double)(player.Center.Y - vector.Y), (double)(player.Center.X - vector.X));
                if (npc.spriteDirection == 1)
                {
                    num17 += 3.14159274f;
                }
                if (num17 < 0f)
                {
                    num17 += 6.28318548f;
                }
                if (num17 > 6.28318548f)
                {
                    num17 -= 6.28318548f;
                }
                float num18 = 0.04f;
                if (npc.ai[0] == 1f)
                {
                    num18 = 0f;
                }
                if (npc.rotation < num17)
                {
                    if ((double)(num17 - npc.rotation) > 3.1415926535897931)
                    {
                        npc.rotation -= num18;
                    }
                    else
                    {
                        npc.rotation += num18;
                    }
                }
                if (npc.rotation > num17)
                {
                    if ((double)(npc.rotation - num17) > 3.1415926535897931)
                    {
                        npc.rotation += num18;
                    }
                    else
                    {
                        npc.rotation -= num18;
                    }
                }
                if (npc.rotation > num17 - num18 && npc.rotation < num17 + num18)
                {
                    npc.rotation = num17;
                }
                if (npc.rotation < 0f)
                {
                    npc.rotation += 6.28318548f;
                }
                if (npc.rotation > 6.28318548f)
                {
                    npc.rotation -= 6.28318548f;
                }
                if (npc.rotation > num17 - num18 && npc.rotation < num17 + num18)
                {
                    npc.rotation = num17;
                }
                if (npc.ai[0] == 0f && !player.dead)
                {
                    if (npc.ai[1] == 0f)
                    {
                        npc.ai[1] = (float)(300 * Math.Sign((vector - player.Center).X));
                    }
                    Vector2 vector3 = Vector2.Normalize(player.Center + new Vector2(npc.ai[1], -200f) - vector - npc.velocity) * scaleFactor;
                    if (npc.velocity.X < vector3.X)
                    {
                        npc.velocity.X = npc.velocity.X + num3;
                        if (npc.velocity.X < 0f && vector3.X > 0f)
                        {
                            npc.velocity.X = npc.velocity.X + num3;
                        }
                    }
                    else if (npc.velocity.X > vector3.X)
                    {
                        npc.velocity.X = npc.velocity.X - num3;
                        if (npc.velocity.X > 0f && vector3.X < 0f)
                        {
                            npc.velocity.X = npc.velocity.X - num3;
                        }
                    }
                    if (npc.velocity.Y < vector3.Y)
                    {
                        npc.velocity.Y = npc.velocity.Y + num3;
                        if (npc.velocity.Y < 0f && vector3.Y > 0f)
                        {
                            npc.velocity.Y = npc.velocity.Y + num3;
                        }
                    }
                    else if (npc.velocity.Y > vector3.Y)
                    {
                        npc.velocity.Y = npc.velocity.Y - num3;
                        if (npc.velocity.Y > 0f && vector3.Y < 0f)
                        {
                            npc.velocity.Y = npc.velocity.Y - num3;
                        }
                    }
                    int num22 = Math.Sign(player.Center.X - vector.X);
                    if (num22 != 0)
                    {
                        if (npc.ai[2] == 0f && num22 != npc.direction)
                        {
                            npc.rotation += 3.14159274f;
                        }
                        npc.direction = num22;
                        if (npc.spriteDirection != -npc.direction)
                        {
                            npc.rotation += 3.14159274f;
                        }
                        npc.spriteDirection = -npc.direction;
                    }
                    npc.ai[2] += 1f;
                    if (npc.ai[2] >= (float)num2)
                    {
                        npc.ai[0] = 1f;
                        npc.ai[1] = 0f;
                        npc.ai[2] = 0f;
                        npc.velocity = Vector2.Normalize(player.Center - vector) * num5;
                        npc.rotation = (float)Math.Atan2((double)npc.velocity.Y, (double)npc.velocity.X);
                        if (num22 != 0)
                        {
                            npc.direction = num22;
                            if (npc.spriteDirection == 1)
                            {
                                npc.rotation += 3.14159274f;
                            }
                            npc.spriteDirection = -npc.direction;
                        }
                        npc.netUpdate = true;
                        return;
                    }
                }
                else if (npc.ai[0] == 1f)
                {
                    npc.ai[2] += 1f;
                    if (npc.ai[2] >= (float)num4)
                    {
                        npc.localAI[3] += 1f;
                        if (npc.localAI[3] >= 2f)
                        {
                            npc.localAI[3] = 0f;
                        }
                        npc.ai[0] = 0f;
                        npc.ai[1] = 0f;
                        npc.ai[2] = 0f;
                        npc.netUpdate = true;
                        return;
                    }
                }
            }
            else
            {
                resetAI = false;
                if (npc.direction == 0)
                {
                    npc.TargetClosest(true);
                }
                Point point15 = npc.Center.ToTileCoordinates();
                Tile tileSafely = Framing.GetTileSafely(point15);
                bool flag121 = tileSafely.nactive() || tileSafely.liquid > 0;
                bool flag122 = false;
                npc.TargetClosest(false);
                Vector2 vector260 = npc.targetRect.Center.ToVector2();
                if (Main.player[npc.target].velocity.Y > -0.1f && !Main.player[npc.target].dead && npc.Distance(vector260) > 150f)
                {
                    flag122 = true;
                }
                npc.localAI[1] += 1f;
                if (lowLife)
                {
                    bool spawnFlag = npc.localAI[1] == 150f;
                    if (NPC.CountNPCS(NPCID.SandShark) > 2)
                    {
                        spawnFlag = false;
                    }
                    if (spawnFlag && Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        NPC.NewNPC((int)npc.Center.X, (int)npc.Center.Y + 50, NPCID.SandShark, 0, 0f, 0f, 0f, 0f, 255);
                        Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/GreatSandSharkRoar"), (int)npc.position.X, (int)npc.position.Y);
                    }
                }
                if (npc.localAI[1] >= 300f)
                {
                    npc.localAI[1] = 0f;
                    if (npc.localAI[2] > 0f)
                    {
                        npc.localAI[2] = 0f;
                    }
                    switch (Main.rand.Next(3))
                    {
                        case 0:
                            npc.ai[3] = 0f;
                            break;
                        case 1:
                            npc.ai[3] = 1f;
                            break;
                        case 2:
                            npc.ai[3] = 2f;
                            break;
                    }
                    int random = lowerLife ? 5 : 9;
                    if (lowLife && Main.rand.Next(random) == 0)
                    {
                        npc.localAI[3] = 1f;
                    }
                    npc.netUpdate = true;
                }
                if (npc.localAI[0] == -1f && !flag121)
                {
                    npc.localAI[0] = 20f;
                }
                if (npc.localAI[0] > 0f)
                {
                    npc.localAI[0] -= 1f;
                }
                if (flag121)
                {
                    float num1534 = npc.ai[1];
                    bool flag123 = false;
                    point15 = (npc.Center + new Vector2(0f, 24f)).ToTileCoordinates();
                    tileSafely = Framing.GetTileSafely(point15.X, point15.Y - 2);
                    if (tileSafely.nactive())
                    {
                        flag123 = true;
                    }
                    npc.ai[1] = (float)flag123.ToInt();
                    if (npc.ai[2] < 30f)
                    {
                        npc.ai[2] += 1f;
                    }
                    if (flag122)
                    {
                        npc.TargetClosest(true);
                        npc.velocity.X = npc.velocity.X + (float)npc.direction * 0.15f;
                        npc.velocity.Y = npc.velocity.Y + (float)npc.directionY * 0.15f;
                        float velocityX = 8f;
                        float velocityY = 6f;
                        switch ((int)npc.ai[3])
                        {
                            case 0:
                                velocityX = 10f;
                                velocityY = 9f;
                                break;
                            case 1:
                                velocityX = 14f;
                                velocityY = 7f;
                                break;
                            case 2:
                                velocityX = 8f;
                                velocityY = 11f;
                                break;
                        }
                        if (revenge || lowerLife)
                        {
                            velocityX *= 1.1f;
                            velocityY *= 1.1f;
                        }
                        if (youMustDie)
                        {
                            velocityX *= 1.5f;
                            velocityY *= 1.5f;
                        }
                        if (npc.velocity.X > velocityX)
                        {
                            npc.velocity.X = velocityX;
                        }
                        if (npc.velocity.X < -velocityX)
                        {
                            npc.velocity.X = -velocityX;
                        }
                        if (npc.velocity.Y > velocityY)
                        {
                            npc.velocity.Y = velocityY;
                        }
                        if (npc.velocity.Y < -velocityY)
                        {
                            npc.velocity.Y = -velocityY;
                        }
                        Vector2 vec4 = npc.Center + npc.velocity.SafeNormalize(Vector2.Zero) * npc.Size.Length() / 2f + npc.velocity;
                        point15 = vec4.ToTileCoordinates();
                        tileSafely = Framing.GetTileSafely(point15);
                        bool flag124 = tileSafely.nactive();
                        if (!flag124 && Math.Sign(npc.velocity.X) == npc.direction && (npc.Distance(vector260) < 600f || youMustDie) &&
                            (npc.ai[2] >= 30f || npc.ai[2] < 0f))
                        {
                            if (npc.localAI[0] == 0f)
                            {
                                Main.PlaySound(4, (int)npc.position.X, (int)npc.position.Y, 15);
                                npc.localAI[0] = -1f;
                                for (int num621 = 0; num621 < 25; num621++)
                                {
                                    int num622 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 32, 0f, 0f, 100, default, 2f);
                                    Main.dust[num622].velocity.Y *= 6f;
                                    Main.dust[num622].velocity.X *= 3f;
                                    if (Main.rand.NextBool(2))
                                    {
                                        Main.dust[num622].scale = 0.5f;
                                        Main.dust[num622].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                                    }
                                }
                                for (int num623 = 0; num623 < 50; num623++)
                                {
                                    int num624 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 85, 0f, 0f, 100, default, 3f);
                                    Main.dust[num624].noGravity = true;
                                    Main.dust[num624].velocity.Y *= 10f;
                                    num624 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 268, 0f, 0f, 100, default, 2f);
                                    Main.dust[num624].velocity.X *= 2f;
                                }
                                int spawnX = (int)(npc.width / 2);
                                for (int sand = 0; sand < 5; sand++)
                                    Projectile.NewProjectile(npc.Center.X + (float)Main.rand.Next(-spawnX, spawnX), npc.Center.Y,
                                        (float)Main.rand.Next(-3, 4), (float)Main.rand.Next(-12, -6), ModContent.ProjectileType<GreatSandBlast>(), 40, 0f, Main.myPlayer, 0f, 0f);
                            }
                            npc.ai[2] = -30f;
                            Vector2 vector261 = npc.DirectionTo(vector260 + new Vector2(0f, -80f));
                            npc.velocity = vector261 * 18f; //12
                        }
                    }
                    else
                    {
                        float num1535 = 6f;
                        npc.velocity.X = npc.velocity.X + (float)npc.direction * 0.1f;
                        if (npc.velocity.X < -num1535 || npc.velocity.X > num1535)
                        {
                            npc.velocity.X = npc.velocity.X * 0.95f; //.95
                        }
                        if (flag123)
                        {
                            npc.ai[0] = -1f;
                        }
                        else
                        {
                            npc.ai[0] = 1f;
                        }
                        float num1536 = 0.06f;
                        float num1537 = 0.01f;
                        if (npc.ai[0] == -1f)
                        {
                            npc.velocity.Y = npc.velocity.Y - num1537;
                            if (npc.velocity.Y < -num1536)
                            {
                                npc.ai[0] = 1f;
                            }
                        }
                        else
                        {
                            npc.velocity.Y = npc.velocity.Y + num1537;
                            if (npc.velocity.Y > num1536)
                            {
                                npc.ai[0] = -1f;
                            }
                        }
                        if (npc.velocity.Y > 0.4f || npc.velocity.Y < -0.4f)
                        {
                            npc.velocity.Y = npc.velocity.Y * 0.95f;
                        }
                    }
                }
                else
                {
                    if (npc.velocity.Y == 0f)
                    {
                        if (flag122)
                        {
                            npc.TargetClosest(true);
                        }
                        float num1538 = 1f;
                        npc.velocity.X = npc.velocity.X + (float)npc.direction * 0.1f;
                        if (npc.velocity.X < -num1538 || npc.velocity.X > num1538)
                        {
                            npc.velocity.X = npc.velocity.X * 0.95f; //.95
                        }
                    }
                    if (npc.localAI[2] == 0f)
                    {
                        npc.localAI[2] = 1f;
                        float velocityX = 12f;
                        float velocityY = 12f;
                        switch ((int)npc.ai[3])
                        {
                            case 0:
                                velocityX = 12f;
                                velocityY = 12f;
                                break;
                            case 1:
                                velocityX = 14f;
                                velocityY = 14f;
                                break;
                            case 2:
                                velocityX = 16f;
                                velocityY = 16f;
                                break;
                        }
                        if (revenge || lowerLife)
                        {
                            velocityX *= 1.1f;
                            velocityY *= 1.1f;
                        }
                        if (youMustDie)
                        {
                            velocityX *= 1.5f;
                            velocityY *= 1.5f;
                        }
                        npc.velocity.Y = -velocityY;
                        npc.velocity.X = velocityX * (float)npc.direction;
                        npc.netUpdate = true;
                    }
                    npc.velocity.Y = npc.velocity.Y + 0.4f; //0.3
                    if (npc.velocity.Y > 10f)
                    {
                        npc.velocity.Y = 10f;
                    }
                    npc.ai[0] = 1f;
                }
                npc.rotation = npc.velocity.Y * (float)npc.direction * 0.1f;
                if (npc.rotation < -0.1f)
                {
                    npc.rotation = -0.1f;
                }
                if (npc.rotation > 0.1f)
                {
                    npc.rotation = 0.1f;
                }
            }
        }

        public override void FindFrame(int frameHeight)
        {
            if (npc.localAI[3] == 0f)
            {
                npc.spriteDirection = -npc.direction;
            }
            npc.frameCounter += 0.15f;
            npc.frameCounter %= Main.npcFrameCount[npc.type];
            int frame = (int)npc.frameCounter;
            npc.frame.Y = frame * frameHeight;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color drawColor)
        {
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (npc.spriteDirection == 1)
            {
                spriteEffects = SpriteEffects.FlipHorizontally;
            }
            Color color24 = npc.GetAlpha(drawColor);
            Color color25 = Lighting.GetColor((int)((double)npc.position.X + (double)npc.width * 0.5) / 16, (int)(((double)npc.position.Y + (double)npc.height * 0.5) / 16.0));
            Texture2D texture2D3 = Main.npcTexture[npc.type];
            int num156 = Main.npcTexture[npc.type].Height / Main.npcFrameCount[npc.type];
            int y3 = num156 * (int)npc.frameCounter;
            Rectangle rectangle = new Rectangle(0, y3, texture2D3.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;
            int num157 = 8;
            int num158 = 2;
            int num159 = 1;
            float num160 = 0f;
            int num161 = num159;
            while (((num158 > 0 && num161 < num157) || (num158 < 0 && num161 > num157)) && Lighting.NotRetro)
            {
                Color color26 = npc.GetAlpha(color25);
                {
                    goto IL_6899;
                }
                IL_6881:
                num161 += num158;
                continue;
                IL_6899:
                float num164 = (float)(num157 - num161);
                if (num158 < 0)
                {
                    num164 = (float)(num159 - num161);
                }
                color26 *= num164 / ((float)NPCID.Sets.TrailCacheLength[npc.type] * 1.5f);
                Vector2 value4 = npc.oldPos[num161];
                float num165 = npc.rotation;
                Main.spriteBatch.Draw(texture2D3, value4 + npc.Size / 2f - Main.screenPosition + new Vector2(0, npc.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color26, num165 + npc.rotation * num160 * (float)(num161 - 1) * -(float)spriteEffects.HasFlag(SpriteEffects.FlipHorizontally).ToDirectionInt(), origin2, npc.scale, spriteEffects, 0f);
                goto IL_6881;
            }
            var something = npc.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            spriteBatch.Draw(texture2D3, npc.Center - Main.screenPosition + new Vector2(0, npc.gfxOffY), npc.frame, color24, npc.rotation, npc.frame.Size() / 2, npc.scale, something, 0);
            return false;
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(npc.position, npc.width, npc.height, 5, hitDirection, -1f, 0, default, 1f);
            }
            if (npc.life <= 0)
            {
                for (int num621 = 0; num621 < 50; num621++)
                {
                    int num622 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 5, 0f, 0f, 100, default, 2f);
                    Main.dust[num622].velocity *= 3f;
                    if (Main.rand.NextBool(2))
                    {
                        Main.dust[num622].scale = 0.5f;
                        Main.dust[num622].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                    }
                }
                for (int num623 = 0; num623 < 100; num623++)
                {
                    int num624 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 5, 0f, 0f, 100, default, 3f);
                    Main.dust[num624].noGravity = true;
                    Main.dust[num624].velocity *= 5f;
                    num624 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 5, 0f, 0f, 100, default, 2f);
                    Main.dust[num624].velocity *= 2f;
                }
            }
        }

        public override void NPCLoot()
        {
            Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ModContent.ItemType<GrandScale>());
            if (Main.expertMode && Main.rand.NextBool(3))
            {
                Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ModContent.ItemType<GrandScale>());
            }
        }

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            npc.lifeMax = (int)(npc.lifeMax * 0.8f * bossLifeScale);
            npc.damage = (int)(npc.damage * 0.8f);
        }

        public override void OnHitPlayer(Player player, int damage, bool crit)
        {
            player.AddBuff(BuffID.Rabies, 240, true);
            player.AddBuff(BuffID.Bleeding, 240, true);
        }
    }
}
