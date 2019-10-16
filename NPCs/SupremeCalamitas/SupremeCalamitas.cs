﻿
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria; using CalamityMod.Projectiles; using Terraria.ModLoader;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles; using CalamityMod.Tiles; using CalamityMod.Walls;

namespace CalamityMod.NPCs
{
    [AutoloadBossHead]
    public class SupremeCalamitas : ModNPC
    {
        private float bossLife;
        private float uDieLul = 1f;
        private float passedVar = 0f;

        private bool protectionBoost = false;
        private bool canDespawn = false;
        private bool despawnProj = false;
        private bool startText = false;
        private bool startBattle = false; //100%
        private bool startSecondAttack = false; //80%
        private bool startThirdAttack = false; //60%
        private bool halfLife = false; //40%
        private bool startFourthAttack = false; //30%
        private bool secondStage = false; //20%
        private bool startFifthAttack = false; //10%
        private bool gettingTired = false; //8%
        private bool gettingTired2 = false; //6%
        private bool gettingTired3 = false; //4%
        private bool gettingTired4 = false; //2%
        private bool gettingTired5 = false; //1%
        private bool willCharge = false;
        private bool canFireSplitingFireball = true;
        private bool spawnArena = false;

        private int giveUpCounter = 1200;
        private int lootTimer = 0; //900 * 5 = 4500
        private int phaseChange = 0;
        private int spawnX = 0;
        private int spawnX2 = 0;
        private int spawnXReset = 0;
        private int spawnXReset2 = 0;
        private int spawnXAdd = 200;
        private int spawnY = 0;
        private int spawnYReset = 0;
        private int spawnYAdd = 0;
        private int bulletHellCounter = 0;
        private int bulletHellCounter2 = 0;

        private Rectangle safeBox = default;

        public static float normalDR = 0.7f;
        public static float deathDR = 0.75f;
        public static float bossRushDR = 0.6f;
        public static float enragedDR = 0.99f;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Supreme Calamitas");
            Main.npcFrameCount[npc.type] = 6;
        }

        public override void SetDefaults()
        {
            npc.damage = 350;
            npc.npcSlots = 50f;
            npc.width = 120;
            npc.height = 120;
            npc.defense = 120;
            CalamityGlobalNPC global = npc.Calamity();
            global.DR = CalamityWorld.bossRushActive ? bossRushDR : CalamityWorld.death ? deathDR : normalDR;
            global.customDR = true;
            global.multDRReductions.Add(BuffID.Ichor, 0.9f);
            global.multDRReductions.Add(BuffID.CursedInferno, 0.91f);
            npc.value = Item.buyPrice(10, 0, 0, 0);
            npc.lifeMax = CalamityWorld.revenge ? 5500000 : 5000000;
            if (CalamityWorld.death)
            {
                npc.lifeMax = 6250000;
            }
            if (CalamityWorld.bossRushActive)
            {
                npc.lifeMax = CalamityWorld.death ? 2300000 : 2100000;
            }
            double HPBoost = (double)Config.BossHealthPercentageBoost * 0.01;
            npc.lifeMax += (int)((double)npc.lifeMax * HPBoost);
            npc.aiStyle = -1;
            aiType = -1;
            npc.knockBackResist = 0f;
            NPCID.Sets.TrailCacheLength[npc.type] = 8;
            NPCID.Sets.TrailingMode[npc.type] = 1;
            for (int k = 0; k < npc.buffImmune.Length; k++)
            {
                npc.buffImmune[k] = true;
            }
            npc.buffImmune[BuffID.Ichor] = false;
            npc.buffImmune[BuffID.CursedInferno] = false;
            npc.dontTakeDamage = false;
            npc.chaseable = true;
            npc.boss = true;
            npc.canGhostHeal = false;
            npc.noGravity = true;
            npc.noTileCollide = true;
            npc.HitSound = SoundID.NPCHit4;
            Mod calamityModMusic = ModLoader.GetMod("CalamityModMusic");
            if (calamityModMusic != null)
                music = calamityModMusic.GetSoundSlot(SoundType.Music, "Sounds/Music/SCG");
            else
                music = MusicID.Boss2;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(protectionBoost);
            writer.Write(canDespawn);
            writer.Write(despawnProj);
            writer.Write(startText);
            writer.Write(startBattle);
            writer.Write(startSecondAttack);
            writer.Write(startThirdAttack);
            writer.Write(startFourthAttack);
            writer.Write(startFifthAttack);
            writer.Write(halfLife);
            writer.Write(secondStage);
            writer.Write(gettingTired);
            writer.Write(gettingTired2);
            writer.Write(gettingTired3);
            writer.Write(gettingTired4);
            writer.Write(gettingTired5);
            writer.Write(willCharge);
            writer.Write(canFireSplitingFireball);
            writer.Write(spawnArena);
            writer.Write(npc.dontTakeDamage);
            writer.Write(npc.chaseable);

            writer.Write(giveUpCounter);
            writer.Write(lootTimer);
            writer.Write(phaseChange);
            writer.Write(spawnX);
            writer.Write(spawnX2);
            writer.Write(spawnXReset);
            writer.Write(spawnXReset2);
            writer.Write(spawnXAdd);
            writer.Write(spawnY);
            writer.Write(spawnYReset);
            writer.Write(spawnYAdd);
            writer.Write(bulletHellCounter);
            writer.Write(bulletHellCounter2);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            protectionBoost = reader.ReadBoolean();
            canDespawn = reader.ReadBoolean();
            despawnProj = reader.ReadBoolean();
            startText = reader.ReadBoolean();
            startBattle = reader.ReadBoolean();
            startSecondAttack = reader.ReadBoolean();
            startThirdAttack = reader.ReadBoolean();
            startFourthAttack = reader.ReadBoolean();
            startFifthAttack = reader.ReadBoolean();
            halfLife = reader.ReadBoolean();
            secondStage = reader.ReadBoolean();
            gettingTired = reader.ReadBoolean();
            gettingTired2 = reader.ReadBoolean();
            gettingTired3 = reader.ReadBoolean();
            gettingTired4 = reader.ReadBoolean();
            gettingTired5 = reader.ReadBoolean();
            willCharge = reader.ReadBoolean();
            canFireSplitingFireball = reader.ReadBoolean();
            spawnArena = reader.ReadBoolean();
            npc.dontTakeDamage = reader.ReadBoolean();
            npc.chaseable = reader.ReadBoolean();

            giveUpCounter = reader.ReadInt32();
            lootTimer = reader.ReadInt32();
            phaseChange = reader.ReadInt32();
            spawnX = reader.ReadInt32();
            spawnX2 = reader.ReadInt32();
            spawnXReset = reader.ReadInt32();
            spawnXReset2 = reader.ReadInt32();
            spawnXAdd = reader.ReadInt32();
            spawnY = reader.ReadInt32();
            spawnYReset = reader.ReadInt32();
            spawnYAdd = reader.ReadInt32();
            bulletHellCounter = reader.ReadInt32();
            bulletHellCounter2 = reader.ReadInt32();
        }

        public override void AI()
        {
            #region StartUp
            CalamityGlobalNPC.SCal = npc.whoAmI;
            lootTimer++;

            bool wormAlive = false;
            if (CalamityGlobalNPC.SCalWorm != -1)
            {
                wormAlive = Main.npc[CalamityGlobalNPC.SCalWorm].active;
            }
            bool cataclysmAlive = false;
            if (CalamityGlobalNPC.SCalCataclysm != -1)
            {
                cataclysmAlive = Main.npc[CalamityGlobalNPC.SCalCataclysm].active;
            }
            bool catastropheAlive = false;
            if (CalamityGlobalNPC.SCalCatastrophe != -1)
            {
                catastropheAlive = Main.npc[CalamityGlobalNPC.SCalCatastrophe].active;
            }

            if (Main.slimeRain)
            {
                Main.StopSlimeRain(true);
                CalamityMod.UpdateServerBoolean();
            }
            CalamityMod.StopRain();

            bool expertMode = Main.expertMode || CalamityWorld.bossRushActive;
            Player player = Main.player[npc.target];

            if (!startText)
            {
                if (Main.LocalPlayer.Calamity().sCalKillCount == 4)
                {
                    string key = "Mods.CalamityMod.SupremeBossText12"; //kill SCal 4 times
                    Color messageColor = Color.Orange;
                    if (Main.netMode == NetmodeID.SinglePlayer)
                    {
                        Main.NewText(Language.GetTextValue(key), messageColor);
                    }
                    else if (Main.netMode == NetmodeID.Server)
                    {
                        NetMessage.BroadcastChatMessage(NetworkText.FromKey(key), messageColor);
                    }
                }
                else if (Main.LocalPlayer.Calamity().sCalKillCount == 1)
                {
                    string key = "Mods.CalamityMod.SupremeBossText11"; //kill SCal once
                    Color messageColor = Color.Orange;
                    if (Main.netMode == NetmodeID.SinglePlayer)
                    {
                        Main.NewText(Language.GetTextValue(key), messageColor);
                    }
                    else if (Main.netMode == NetmodeID.Server)
                    {
                        NetMessage.BroadcastChatMessage(NetworkText.FromKey(key), messageColor);
                    }
                }
                if (Main.LocalPlayer.Calamity().sCalDeathCount < 51)
                {
                    if (Main.LocalPlayer.Calamity().sCalDeathCount == 50)
                    {
                        string key = "Mods.CalamityMod.SupremeBossText15"; //die 50 or more times
                        Color messageColor = Color.Orange;
                        if (Main.netMode == NetmodeID.SinglePlayer)
                        {
                            Main.NewText(Language.GetTextValue(key), messageColor);
                        }
                        else if (Main.netMode == NetmodeID.Server)
                        {
                            NetMessage.BroadcastChatMessage(NetworkText.FromKey(key), messageColor);
                        }
                    }
                    else if (Main.LocalPlayer.Calamity().sCalDeathCount > 19)
                    {
                        string key = "Mods.CalamityMod.SupremeBossText14"; //die 20 or more times
                        Color messageColor = Color.Orange;
                        if (Main.netMode == NetmodeID.SinglePlayer)
                        {
                            Main.NewText(Language.GetTextValue(key), messageColor);
                        }
                        else if (Main.netMode == NetmodeID.Server)
                        {
                            NetMessage.BroadcastChatMessage(NetworkText.FromKey(key), messageColor);
                        }
                    }
                    else if (Main.LocalPlayer.Calamity().sCalDeathCount > 4)
                    {
                        string key = "Mods.CalamityMod.SupremeBossText13"; //die 5 or more times
                        Color messageColor = Color.Orange;
                        if (Main.netMode == NetmodeID.SinglePlayer)
                        {
                            Main.NewText(Language.GetTextValue(key), messageColor);
                        }
                        else if (Main.netMode == NetmodeID.Server)
                        {
                            NetMessage.BroadcastChatMessage(NetworkText.FromKey(key), messageColor);
                        }
                    }
                }
                startText = true;
            }
            #endregion
            #region ArenaCreation
            if (!spawnArena)
            {
                spawnArena = true;
                Vector2 vectorPlayer = new Vector2(player.position.X, player.position.Y);
                if (CalamityWorld.death || CalamityWorld.bossRushActive)
                {
                    safeBox.X = spawnX = spawnXReset = (int)(vectorPlayer.X - 1000f);
                    spawnX2 = spawnXReset2 = (int)(vectorPlayer.X + 1000f);
                    safeBox.Y = spawnY = spawnYReset = (int)(vectorPlayer.Y - 1000f);
                    safeBox.Width = 2000;
                    safeBox.Height = 2000;
                    spawnYAdd = 100;
                }
                else
                {
                    safeBox.X = spawnX = spawnXReset = (int)(vectorPlayer.X - 1250f);
                    spawnX2 = spawnXReset2 = (int)(vectorPlayer.X + 1250f);
                    safeBox.Y = spawnY = spawnYReset = (int)(vectorPlayer.Y - 1250f);
                    safeBox.Width = 2500;
                    safeBox.Height = 2500;
                    spawnYAdd = 125;
                }
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    int num52 = (int)(safeBox.X + (float)(safeBox.Width / 2)) / 16;
                    int num53 = (int)(safeBox.Y + (float)(safeBox.Height / 2)) / 16;
                    int num54 = safeBox.Width / 2 / 16 + 1;
                    for (int num55 = num52 - num54; num55 <= num52 + num54; num55++)
                    {
                        for (int num56 = num53 - num54; num56 <= num53 + num54; num56++)
                        {
                            if ((num55 == num52 - num54 || num55 == num52 + num54 || num56 == num53 - num54 || num56 == num53 + num54) && !Main.tile[num55, num56].active())
                            {
                                Main.tile[num55, num56].type = (ushort)ModContent.TileType<ArenaTile>();
                                Main.tile[num55, num56].active(true);
                            }
                            if (Main.netMode == NetmodeID.Server)
                            {
                                NetMessage.SendTileSquare(-1, num55, num56, 1, TileChangeType.None);
                            }
                            else
                            {
                                WorldGen.SquareTileFrame(num55, num56, true);
                            }
                        }
                    }
                }
            }
            #endregion
            #region Enrage and DR
            if (!player.Hitbox.Intersects(safeBox))
            {
                if (uDieLul < 3f)
                {
                    uDieLul *= 1.01f;
                }
                else if (uDieLul > 3f)
                {
                    uDieLul = 3f;
                }
                protectionBoost = true;
            }
            else
            {
                if (uDieLul > 1f)
                {
                    uDieLul *= 0.99f;
                }
                else if (uDieLul < 1f)
                {
                    uDieLul = 1f;
                }
                protectionBoost = false;
            }

            // Set DR to be 99% and unbreakable if enraged. Boost DR during the 5th attack.
            CalamityGlobalNPC global = npc.Calamity();
            if (protectionBoost)
            {
                global.DR = enragedDR;
                global.unbreakableDR = true;
            }
            else
            {
                global.DR = CalamityWorld.bossRushActive ? bossRushDR : CalamityWorld.death ? deathDR : normalDR;
                global.unbreakableDR = false;
                if (startFifthAttack)
                    global.DR *= 1.2f;
            }
            #endregion
            #region Despawn
            if (!player.active || player.dead)
            {
                npc.TargetClosest(false);
                player = Main.player[npc.target];
                if (!player.active || player.dead)
                {
                    npc.velocity = new Vector2(0f, -50f);
                    canDespawn = true;
                    if (npc.timeLeft > 150)
                    {
                        npc.timeLeft = 150;
                    }
                    return;
                }
            }
            else
            {
                canDespawn = false;
            }
            #endregion
            #region FirstAttack
            if (bulletHellCounter2 < 900)
            {
                despawnProj = true;
                bulletHellCounter2 += 1;
                npc.damage = 0;
                npc.chaseable = false;
                npc.dontTakeDamage = true;
                npc.TargetClosest(true);
                npc.velocity *= 0.95f;
                Vector2 vector92 = new Vector2(npc.Center.X, npc.Center.Y);
                float num740 = player.Center.X - vector92.X;
                float num741 = player.Center.Y - vector92.Y;
                npc.rotation = (float)Math.Atan2((double)num741, (double)num740) - 1.57f;
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    bulletHellCounter += 1;
                    if (bulletHellCounter > ((npc.Calamity().enraged || (Config.BossRushXerocCurse && CalamityWorld.bossRushActive)) ? 4 : 6))
                    {
                        bulletHellCounter = 0;
                        int damage = expertMode ? 200 : 250; //800 500
                        if (bulletHellCounter2 < 300) //blasts from above
                        {
                            Projectile.NewProjectile(player.position.X + (float)Main.rand.Next(-1000, 1000), player.position.Y - 1000f, 0f, 4f * uDieLul, ModContent.ProjectileType<BrimstoneHellblast2>(), damage, 0f, Main.myPlayer, 0f, 0f);
                        }
                        else if (bulletHellCounter2 < 600) //blasts from left and right
                        {
                            Projectile.NewProjectile(player.position.X + 1000f, player.position.Y + (float)Main.rand.Next(-1000, 1000), -3.5f * uDieLul, 0f, ModContent.ProjectileType<BrimstoneHellblast2>(), damage, 0f, Main.myPlayer, 0f, 0f);
                            Projectile.NewProjectile(player.position.X - 1000f, player.position.Y + (float)Main.rand.Next(-1000, 1000), 3.5f * uDieLul, 0f, ModContent.ProjectileType<BrimstoneHellblast2>(), damage, 0f, Main.myPlayer, 0f, 0f);
                        }
                        else //blasts from above, left, and right
                        {
                            Projectile.NewProjectile(player.position.X + (float)Main.rand.Next(-1000, 1000), player.position.Y - 1000f, 0f, 3f * uDieLul, ModContent.ProjectileType<BrimstoneHellblast2>(), damage, 0f, Main.myPlayer, 0f, 0f);
                            Projectile.NewProjectile(player.position.X + 1000f, player.position.Y + (float)Main.rand.Next(-1000, 1000), -3f * uDieLul, 0f, ModContent.ProjectileType<BrimstoneHellblast2>(), damage, 0f, Main.myPlayer, 0f, 0f);
                            Projectile.NewProjectile(player.position.X + 1000f, player.position.Y + (float)Main.rand.Next(-1000, 1000), 3f * uDieLul, 0f, ModContent.ProjectileType<BrimstoneHellblast2>(), damage, 0f, Main.myPlayer, 0f, 0f);
                        }
                        return;
                    }
                }
                return;
            }
            else if (!startBattle)
            {
                string key = "Mods.CalamityMod.SupremeBossText3";
                Color messageColor = Color.Orange;
                if (Main.netMode == NetmodeID.SinglePlayer)
                {
                    Main.NewText(Language.GetTextValue(key), messageColor);
                }
                else if (Main.netMode == NetmodeID.Server)
                {
                    NetMessage.BroadcastChatMessage(NetworkText.FromKey(key), messageColor);
                }
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    spawnY += 250;
                    if (CalamityWorld.death || CalamityWorld.bossRushActive)
                    {
                        spawnY -= 50;
                    }
                    for (int x = 0; x < 5; x++)
                    {
                        NPC.NewNPC(spawnX + 50, spawnY, ModContent.NPCType<SCalWormHeart>(), 0, 0f, 0f, 0f, 0f, 255);
                        spawnX += spawnXAdd;
                        NPC.NewNPC(spawnX2 - 50, spawnY, ModContent.NPCType<SCalWormHeart>(), 0, 0f, 0f, 0f, 0f, 255);
                        spawnX2 -= spawnXAdd;
                        spawnY += spawnYAdd;
                    }
                    spawnX = spawnXReset;
                    spawnX2 = spawnXReset2;
                    spawnY = spawnYReset;
                    NPC.SpawnOnPlayer(npc.FindClosestPlayer(), ModContent.NPCType<SCalWormHead>());
                }
                startBattle = true;
            }
            #endregion
            #region SecondAttack
            if (bulletHellCounter2 < 1800 && startSecondAttack)
            {
                despawnProj = true;
                bulletHellCounter2 += 1;
                npc.damage = 0;
                npc.chaseable = false;
                npc.dontTakeDamage = true;
                npc.TargetClosest(true);
                npc.velocity *= 0.95f;
                Vector2 vector92 = new Vector2(npc.Center.X, npc.Center.Y);
                float num740 = player.Center.X - vector92.X;
                float num741 = player.Center.Y - vector92.Y;
                npc.rotation = (float)Math.Atan2((double)num741, (double)num740) - 1.57f;
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    int damage = expertMode ? 150 : 200; //600 400
                    if (bulletHellCounter2 < 1200)
                    {
                        if (bulletHellCounter2 % 180 == 0) //blasts from top
                        {
                            Projectile.NewProjectile(player.position.X + (float)Main.rand.Next(-1000, 1000), player.position.Y - 1000f, 0f, 5f * uDieLul, ModContent.ProjectileType<BrimstoneGigaBlast>(), damage, 0f, Main.myPlayer, 0f, 0f);
                        }
                    }
                    else if (bulletHellCounter2 < 1500 && bulletHellCounter2 > 1200)
                    {
                        if (bulletHellCounter2 % 180 == 0) //blasts from right
                        {
                            Projectile.NewProjectile(player.position.X + 1000f, player.position.Y + (float)Main.rand.Next(-1000, 1000), -5f * uDieLul, 0f, ModContent.ProjectileType<BrimstoneGigaBlast>(), damage, 0f, Main.myPlayer, 0f, 0f);
                        }
                    }
                    else if (bulletHellCounter2 > 1500)
                    {
                        if (bulletHellCounter2 % 180 == 0) //blasts from top
                        {
                            Projectile.NewProjectile(player.position.X + (float)Main.rand.Next(-1000, 1000), player.position.Y - 1000f, 0f, 5f * uDieLul, ModContent.ProjectileType<BrimstoneGigaBlast>(), damage, 0f, Main.myPlayer, 0f, 0f);
                        }
                    }
                    bulletHellCounter += 1;
                    if (bulletHellCounter > ((npc.Calamity().enraged || (Config.BossRushXerocCurse && CalamityWorld.bossRushActive)) ? 7 : 9))
                    {
                        bulletHellCounter = 0;
                        if (bulletHellCounter2 < 1200) //blasts from below
                        {
                            Projectile.NewProjectile(player.position.X + (float)Main.rand.Next(-1000, 1000), player.position.Y + 1000f, 0f, -4f * uDieLul, ModContent.ProjectileType<BrimstoneHellblast2>(), damage, 0f, Main.myPlayer, 0f, 0f);
                        }
                        else if (bulletHellCounter2 < 1500) //blasts from left
                        {
                            Projectile.NewProjectile(player.position.X - 1000f, player.position.Y + (float)Main.rand.Next(-1000, 1000), 3.5f * uDieLul, 0f, ModContent.ProjectileType<BrimstoneHellblast2>(), damage, 0f, Main.myPlayer, 0f, 0f);
                        }
                        else //blasts from left and right
                        {
                            Projectile.NewProjectile(player.position.X - 1000f, player.position.Y + (float)Main.rand.Next(-1000, 1000), 3f * uDieLul, 0f, ModContent.ProjectileType<BrimstoneHellblast2>(), damage, 0f, Main.myPlayer, 0f, 0f);
                            Projectile.NewProjectile(player.position.X + 1000f, player.position.Y + (float)Main.rand.Next(-1000, 1000), -3f * uDieLul, 0f, ModContent.ProjectileType<BrimstoneHellblast2>(), damage, 0f, Main.myPlayer, 0f, 0f);
                        }
                        return;
                    }
                }
                return;
            }
            if (!startSecondAttack && ((double)npc.life <= (double)npc.lifeMax * 0.75))
            {
                string key = "Mods.CalamityMod.SupremeBossText4";
                Color messageColor = Color.Orange;
                if (Main.netMode == NetmodeID.SinglePlayer)
                {
                    Main.NewText(Language.GetTextValue(key), messageColor);
                }
                else if (Main.netMode == NetmodeID.Server)
                {
                    NetMessage.BroadcastChatMessage(NetworkText.FromKey(key), messageColor);
                }
                startSecondAttack = true;
                return;
            }
            #endregion
            #region ThirdAttack
            if (bulletHellCounter2 < 2700 && startThirdAttack)
            {
                despawnProj = true;
                bulletHellCounter2 += 1;
                npc.damage = 0;
                npc.chaseable = false;
                npc.dontTakeDamage = true;
                npc.TargetClosest(true);
                npc.velocity *= 0.95f;
                Vector2 vector92 = new Vector2(npc.Center.X, npc.Center.Y);
                float num740 = player.Center.X - vector92.X;
                float num741 = player.Center.Y - vector92.Y;
                npc.rotation = (float)Math.Atan2((double)num741, (double)num740) - 1.57f;
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    int damage = expertMode ? 150 : 200;
                    if (bulletHellCounter2 % 180 == 0) //blasts from top
                    {
                        Projectile.NewProjectile(player.position.X + (float)Main.rand.Next(-1000, 1000), player.position.Y - 1000f, 0f, 5f * uDieLul, ModContent.ProjectileType<BrimstoneGigaBlast>(), damage, 0f, Main.myPlayer, 0f, 0f);
                    }
                    if (bulletHellCounter2 % 240 == 0) //fireblasts from above
                    {
                        Projectile.NewProjectile(player.position.X + (float)Main.rand.Next(-1000, 1000), player.position.Y - 1000f, 0f, 10f * uDieLul, ModContent.ProjectileType<BrimstoneFireblast>(), damage, 0f, Main.myPlayer, 1f, 0f);
                    }
                    bulletHellCounter += 1;
                    if (bulletHellCounter > ((npc.Calamity().enraged || (Config.BossRushXerocCurse && CalamityWorld.bossRushActive)) ? 9 : 11))
                    {
                        bulletHellCounter = 0;
                        if (bulletHellCounter2 < 2100) //blasts from above
                        {
                            Projectile.NewProjectile(player.position.X + (float)Main.rand.Next(-1000, 1000), player.position.Y - 1000f, 0f, 4f * uDieLul, ModContent.ProjectileType<BrimstoneHellblast2>(), damage, 0f, Main.myPlayer, 0f, 0f);
                        }
                        else if (bulletHellCounter2 < 2400) //blasts from right
                        {
                            Projectile.NewProjectile(player.position.X + 1000f, player.position.Y + (float)Main.rand.Next(-1000, 1000), -3.5f * uDieLul, 0f, ModContent.ProjectileType<BrimstoneHellblast2>(), damage, 0f, Main.myPlayer, 0f, 0f);
                        }
                        else //blasts from left and right
                        {
                            Projectile.NewProjectile(player.position.X + 1000f, player.position.Y + (float)Main.rand.Next(-1000, 1000), -3.5f * uDieLul, 0f, ModContent.ProjectileType<BrimstoneHellblast2>(), damage, 0f, Main.myPlayer, 0f, 0f);
                            Projectile.NewProjectile(player.position.X - 1000f, player.position.Y + (float)Main.rand.Next(-1000, 1000), 3.5f * uDieLul, 0f, ModContent.ProjectileType<BrimstoneHellblast2>(), damage, 0f, Main.myPlayer, 0f, 0f);
                        }
                        return;
                    }
                }
                return;
            }
            if (!startThirdAttack && ((double)npc.life <= (double)npc.lifeMax * 0.5))
            {
                Mod calamityModMusic = ModLoader.GetMod("CalamityModMusic");
                if (calamityModMusic != null)
                    music = calamityModMusic.GetSoundSlot(SoundType.Music, "Sounds/Music/SCL");
                else
                    music = MusicID.Boss3;
                string key = "Mods.CalamityMod.SupremeBossText5";
                Color messageColor = Color.Orange;
                if (Main.netMode == NetmodeID.SinglePlayer)
                {
                    Main.NewText(Language.GetTextValue(key), messageColor);
                }
                else if (Main.netMode == NetmodeID.Server)
                {
                    NetMessage.BroadcastChatMessage(NetworkText.FromKey(key), messageColor);
                }
                startThirdAttack = true;
                return;
            }
            #endregion
            #region FourthAttack
            if (bulletHellCounter2 < 3600 && startFourthAttack)
            {
                despawnProj = true;
                bulletHellCounter2 += 1;
                npc.damage = 0;
                npc.chaseable = false;
                npc.dontTakeDamage = true;
                npc.TargetClosest(true);
                npc.velocity *= 0.95f;
                Vector2 vector92 = new Vector2(npc.Center.X, npc.Center.Y);
                float num740 = player.Center.X - vector92.X;
                float num741 = player.Center.Y - vector92.Y;
                npc.rotation = (float)Math.Atan2((double)num741, (double)num740) - 1.57f;
                if (Main.netMode != NetmodeID.MultiplayerClient) //more clustered attack
                {
                    int damage = expertMode ? 150 : 200;
                    if (bulletHellCounter2 % 180 == 0) //blasts from top
                    {
                        Projectile.NewProjectile(player.position.X + (float)Main.rand.Next(-1000, 1000), player.position.Y - 1000f, 0f, 5f * uDieLul, ModContent.ProjectileType<BrimstoneGigaBlast>(), damage, 0f, Main.myPlayer, 0f, 0f);
                    }
                    if (bulletHellCounter2 % 240 == 0) //fireblasts from above
                    {
                        Projectile.NewProjectile(player.position.X + (float)Main.rand.Next(-1000, 1000), player.position.Y - 1000f, 0f, 10f * uDieLul, ModContent.ProjectileType<BrimstoneFireblast>(), damage, 0f, Main.myPlayer, 1f, 0f);
                    }
                    if (bulletHellCounter2 % 450 == 0) //giant homing fireballs
                    {
                        Projectile.NewProjectile(player.position.X + (float)Main.rand.Next(-1000, 1000), player.position.Y - 1000f, 0f, 1f * uDieLul, ModContent.ProjectileType<BrimstoneMonster>(), damage, 0f, Main.myPlayer, 0f, passedVar);
                        passedVar += 1f;
                    }
                    bulletHellCounter += 1;
                    if (bulletHellCounter > ((npc.Calamity().enraged || (Config.BossRushXerocCurse && CalamityWorld.bossRushActive)) ? 13 : 15))
                    {
                        bulletHellCounter = 0;
                        if (bulletHellCounter2 < 3000) //blasts from below
                        {
                            Projectile.NewProjectile(player.position.X + (float)Main.rand.Next(-1000, 1000), player.position.Y + 1000f, 0f, -4f * uDieLul, ModContent.ProjectileType<BrimstoneHellblast2>(), damage, 0f, Main.myPlayer, 0f, 0f);
                        }
                        else if (bulletHellCounter2 < 3300) //blasts from left
                        {
                            Projectile.NewProjectile(player.position.X - 1000f, player.position.Y + (float)Main.rand.Next(-1000, 1000), 3.5f * uDieLul, 0f, ModContent.ProjectileType<BrimstoneHellblast2>(), damage, 0f, Main.myPlayer, 0f, 0f);
                        }
                        else //blasts from left and right
                        {
                            Projectile.NewProjectile(player.position.X + 1000f, player.position.Y + (float)Main.rand.Next(-1000, 1000), -3.5f * uDieLul, 0f, ModContent.ProjectileType<BrimstoneHellblast2>(), damage, 0f, Main.myPlayer, 0f, 0f);
                            Projectile.NewProjectile(player.position.X - 1000f, player.position.Y + (float)Main.rand.Next(-1000, 1000), 3.5f * uDieLul, 0f, ModContent.ProjectileType<BrimstoneHellblast2>(), damage, 0f, Main.myPlayer, 0f, 0f);
                        }
                        return;
                    }
                }
                return;
            }
            if (!startFourthAttack && ((double)npc.life <= (double)npc.lifeMax * 0.3))
            {
                Mod calamityModMusic = ModLoader.GetMod("CalamityModMusic");
                if (calamityModMusic != null)
                    music = calamityModMusic.GetSoundSlot(SoundType.Music, "Sounds/Music/SCE");
                else
                    music = MusicID.LunarBoss;
                string key = "Mods.CalamityMod.SupremeBossText7";
                Color messageColor = Color.Orange;
                if (Main.netMode == NetmodeID.SinglePlayer)
                {
                    Main.NewText(Language.GetTextValue(key), messageColor);
                }
                else if (Main.netMode == NetmodeID.Server)
                {
                    NetMessage.BroadcastChatMessage(NetworkText.FromKey(key), messageColor);
                }
                startFourthAttack = true;
                return;
            }
            #endregion
            #region FifthAttack
            if (bulletHellCounter2 < 4500 && startFifthAttack)
            {
                despawnProj = true;
                bulletHellCounter2 += 1;
                npc.damage = 0;
                npc.chaseable = false;
                npc.dontTakeDamage = true;
                npc.TargetClosest(true);
                npc.velocity *= 0.95f;
                Vector2 vector92 = new Vector2(npc.Center.X, npc.Center.Y);
                float num740 = player.Center.X - vector92.X;
                float num741 = player.Center.Y - vector92.Y;
                npc.rotation = (float)Math.Atan2((double)num741, (double)num740) - 1.57f;
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    int damage = expertMode ? 150 : 200;
                    if (bulletHellCounter2 % 240 == 0) //blasts from top
                    {
                        Projectile.NewProjectile(player.position.X + (float)Main.rand.Next(-1000, 1000), player.position.Y - 1000f, 0f, 5f * uDieLul, ModContent.ProjectileType<BrimstoneGigaBlast>(), damage, 0f, Main.myPlayer, 0f, 0f);
                    }
                    if (bulletHellCounter2 % 360 == 0) //fireblasts from above
                    {
                        Projectile.NewProjectile(player.position.X + (float)Main.rand.Next(-1000, 1000), player.position.Y - 1000f, 0f, 10f * uDieLul, ModContent.ProjectileType<BrimstoneFireblast>(), damage, 0f, Main.myPlayer, 1f, 0f);
                    }
                    if (bulletHellCounter2 % 450 == 0) //giant homing fireballs
                    {
                        Projectile.NewProjectile(player.position.X + (float)Main.rand.Next(-1000, 1000), player.position.Y - 1000f, 0f, 1f * uDieLul, ModContent.ProjectileType<BrimstoneMonster>(), damage, 0f, Main.myPlayer, 0f, passedVar);
                        passedVar += 1f;
                    }
                    if (bulletHellCounter2 % 30 == 0) //projectiles that move in wave pattern
                    {
                        Projectile.NewProjectile(player.position.X + 1000f, player.position.Y + (float)Main.rand.Next(-500, 500), -10f * uDieLul, 0f, ModContent.ProjectileType<BrimstoneWave>(), damage, 0f, Main.myPlayer, 0f, 0f);
                    }
                    bulletHellCounter += 1;
                    if (bulletHellCounter > ((npc.Calamity().enraged || (Config.BossRushXerocCurse && CalamityWorld.bossRushActive)) ? 16 : 18))
                    {
                        bulletHellCounter = 0;
                        if (bulletHellCounter2 < 3900) //blasts from above
                        {
                            Projectile.NewProjectile(player.position.X + (float)Main.rand.Next(-1000, 1000), player.position.Y - 1000f, 0f, 4f * uDieLul, ModContent.ProjectileType<BrimstoneHellblast2>(), damage, 0f, Main.myPlayer, 0f, 0f);
                        }
                        else if (bulletHellCounter2 < 4200) //blasts from left and right
                        {
                            Projectile.NewProjectile(player.position.X + 1000f, player.position.Y + (float)Main.rand.Next(-1000, 1000), -3.5f * uDieLul, 0f, ModContent.ProjectileType<BrimstoneHellblast2>(), damage, 0f, Main.myPlayer, 0f, 0f);
                            Projectile.NewProjectile(player.position.X - 1000f, player.position.Y + (float)Main.rand.Next(-1000, 1000), 3.5f * uDieLul, 0f, ModContent.ProjectileType<BrimstoneHellblast2>(), damage, 0f, Main.myPlayer, 0f, 0f);
                        }
                        else //blasts from above, left, and right
                        {
                            Projectile.NewProjectile(player.position.X + (float)Main.rand.Next(-1000, 1000), player.position.Y - 1000f, 0f, 3f * uDieLul, ModContent.ProjectileType<BrimstoneHellblast2>(), damage, 0f, Main.myPlayer, 0f, 0f);
                            Projectile.NewProjectile(player.position.X + 1000f, player.position.Y + (float)Main.rand.Next(-1000, 1000), -3f * uDieLul, 0f, ModContent.ProjectileType<BrimstoneHellblast2>(), damage, 0f, Main.myPlayer, 0f, 0f);
                            Projectile.NewProjectile(player.position.X + 1000f, player.position.Y + (float)Main.rand.Next(-1000, 1000), 3f * uDieLul, 0f, ModContent.ProjectileType<BrimstoneHellblast2>(), damage, 0f, Main.myPlayer, 0f, 0f);
                        }
                        return;
                    }
                }
                return;
            }
            if (!startFifthAttack && ((double)npc.life <= (double)npc.lifeMax * 0.1))
            {
                string key = "Mods.CalamityMod.SupremeBossText9";
                Color messageColor = Color.Orange;
                if (Main.netMode == NetmodeID.SinglePlayer)
                {
                    Main.NewText(Language.GetTextValue(key), messageColor);
                }
                else if (Main.netMode == NetmodeID.Server)
                {
                    NetMessage.BroadcastChatMessage(NetworkText.FromKey(key), messageColor);
                }
                startFifthAttack = true;
                return;
            }
            #endregion
            #region EndSections
            if (startFifthAttack)
            {
                if (gettingTired5)
                {
                    Mod calamityModMusic = ModLoader.GetMod("CalamityModMusic");
                    if (calamityModMusic != null)
                        music = calamityModMusic.GetSoundSlot(SoundType.Music, "Sounds/Music/SCA");
                    else
                        music = MusicID.Eerie;
                    npc.noGravity = false;
                    npc.noTileCollide = false;
                    npc.damage = 0;
                    npc.velocity.X *= 0.98f;
                    Vector2 vector2 = new Vector2(npc.Center.X, npc.Center.Y);
                    float num = player.Center.X - vector2.X;
                    float num1 = player.Center.Y - vector2.Y;
                    npc.rotation = (float)Math.Atan2((double)num1, (double)num) - 1.57f;
                    if (CalamityWorld.downedSCal) //after first time you kill her
                    {
                        if (giveUpCounter == 900)
                        {
                            string key = "Mods.CalamityMod.SupremeBossText27";
                            Color messageColor = Color.Orange;
                            if (Main.netMode == NetmodeID.SinglePlayer)
                            {
                                Main.NewText(Language.GetTextValue(key), messageColor);
                            }
                            else if (Main.netMode == NetmodeID.Server)
                            {
                                NetMessage.BroadcastChatMessage(NetworkText.FromKey(key), messageColor);
                            }
                        }
                        giveUpCounter--;
                        npc.chaseable = (giveUpCounter < 900) ? true : false;
                        npc.dontTakeDamage = (giveUpCounter < 900) ? false : true;
                        return;
                    }
                    if (giveUpCounter == 600)
                    {
                        string key = "Mods.CalamityMod.SupremeBossText25";
                        Color messageColor = Color.Orange;
                        if (Main.netMode == NetmodeID.SinglePlayer)
                        {
                            Main.NewText(Language.GetTextValue(key), messageColor);
                        }
                        else if (Main.netMode == NetmodeID.Server)
                        {
                            NetMessage.BroadcastChatMessage(NetworkText.FromKey(key), messageColor);
                        }
                    }
                    if (giveUpCounter == 300)
                    {
                        string key = "Mods.CalamityMod.SupremeBossText26";
                        Color messageColor = Color.Orange;
                        if (Main.netMode == NetmodeID.SinglePlayer)
                        {
                            Main.NewText(Language.GetTextValue(key), messageColor);
                        }
                        else if (Main.netMode == NetmodeID.Server)
                        {
                            NetMessage.BroadcastChatMessage(NetworkText.FromKey(key), messageColor);
                        }
                    }
                    if (giveUpCounter <= 0)
                    {
                        npc.chaseable = true;
                        npc.dontTakeDamage = false;
                        return;
                    }
                    giveUpCounter--;
                    npc.chaseable = false;
                    npc.dontTakeDamage = true;
                    return;
                }
                if (!gettingTired5 && ((double)npc.life <= (double)npc.lifeMax * 0.01))
                {
                    string key = "Mods.CalamityMod.SupremeBossText24";
                    Color messageColor = Color.Orange;
                    if (Main.netMode == NetmodeID.SinglePlayer)
                    {
                        Main.NewText(Language.GetTextValue(key), messageColor);
                    }
                    else if (Main.netMode == NetmodeID.Server)
                    {
                        NetMessage.BroadcastChatMessage(NetworkText.FromKey(key), messageColor);
                    }
                    gettingTired5 = true;
                    return;
                }
                else if (!gettingTired4 && ((double)npc.life <= (double)npc.lifeMax * 0.02))
                {
                    string key = "Mods.CalamityMod.SupremeBossText23";
                    Color messageColor = Color.Orange;
                    if (Main.netMode == NetmodeID.SinglePlayer)
                    {
                        Main.NewText(Language.GetTextValue(key), messageColor);
                    }
                    else if (Main.netMode == NetmodeID.Server)
                    {
                        NetMessage.BroadcastChatMessage(NetworkText.FromKey(key), messageColor);
                    }
                    gettingTired4 = true;
                    return;
                }
                else if (!gettingTired3 && ((double)npc.life <= (double)npc.lifeMax * 0.04))
                {
                    string key = "Mods.CalamityMod.SupremeBossText22";
                    Color messageColor = Color.Orange;
                    if (Main.netMode == NetmodeID.SinglePlayer)
                    {
                        Main.NewText(Language.GetTextValue(key), messageColor);
                    }
                    else if (Main.netMode == NetmodeID.Server)
                    {
                        NetMessage.BroadcastChatMessage(NetworkText.FromKey(key), messageColor);
                    }
                    gettingTired3 = true;
                    return;
                }
                else if (!gettingTired2 && ((double)npc.life <= (double)npc.lifeMax * 0.06))
                {
                    string key = "Mods.CalamityMod.SupremeBossText21";
                    Color messageColor = Color.Orange;
                    if (Main.netMode == NetmodeID.SinglePlayer)
                    {
                        Main.NewText(Language.GetTextValue(key), messageColor);
                    }
                    else if (Main.netMode == NetmodeID.Server)
                    {
                        NetMessage.BroadcastChatMessage(NetworkText.FromKey(key), messageColor);
                    }
                    gettingTired2 = true;
                    return;
                }
                else if (!gettingTired && ((double)npc.life <= (double)npc.lifeMax * 0.08))
                {
                    string key = "Mods.CalamityMod.SupremeBossText20";
                    Color messageColor = Color.Orange;
                    if (Main.netMode == NetmodeID.SinglePlayer)
                    {
                        Main.NewText(Language.GetTextValue(key), messageColor);
                    }
                    else if (Main.netMode == NetmodeID.Server)
                    {
                        NetMessage.BroadcastChatMessage(NetworkText.FromKey(key), messageColor);
                    }
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        spawnY += 250;
                        if (CalamityWorld.death || CalamityWorld.bossRushActive)
                        {
                            spawnY -= 50;
                        }
                        for (int x = 0; x < 5; x++)
                        {
                            NPC.NewNPC(spawnX + 50, spawnY, ModContent.NPCType<SCalWormHeart>(), 0, 0f, 0f, 0f, 0f, 255);
                            spawnX += spawnXAdd;
                            NPC.NewNPC(spawnX2 - 50, spawnY, ModContent.NPCType<SCalWormHeart>(), 0, 0f, 0f, 0f, 0f, 255);
                            spawnX2 -= spawnXAdd;
                            spawnY += spawnYAdd;
                        }
                        spawnX = spawnXReset;
                        spawnX2 = spawnXReset2;
                        spawnY = spawnYReset;
                        NPC.SpawnOnPlayer(npc.FindClosestPlayer(), ModContent.NPCType<SCalWormHead>());
                    }
                    gettingTired = true;
                    return;
                }
            }
            #endregion
            #region DespawnProjectiles
            if (bulletHellCounter2 % 900 == 0 && despawnProj)
            {
                int proj;
                for (int x = 0; x < 1000; x = proj + 1)
                {
                    Projectile projectile = Main.projectile[x];
                    if (projectile.active)
                    {
                        if (projectile.type == ModContent.ProjectileType<BrimstoneHellblast2>() ||
                            projectile.type == ModContent.ProjectileType<BrimstoneBarrage>() ||
                            projectile.type == ModContent.ProjectileType<BrimstoneWave>())
                        {
                            projectile.Kill();
                        }
                        else if (projectile.type == ModContent.ProjectileType<BrimstoneGigaBlast>() ||
                            projectile.type == ModContent.ProjectileType<BrimstoneFireblast>())
                        {
                            projectile.active = false;
                        }
                    }
                    proj = x;
                }
                despawnProj = false;
            }
            #endregion
            #region TransformSeekerandBrotherTriggers
            if (!halfLife && ((double)npc.life <= (double)npc.lifeMax * 0.4))
            {
                string key = "Mods.CalamityMod.SupremeBossText";
                Color messageColor = Color.Orange;
                if (Main.netMode == NetmodeID.SinglePlayer)
                {
                    Main.NewText(Language.GetTextValue(key), messageColor);
                }
                else if (Main.netMode == NetmodeID.Server)
                {
                    NetMessage.BroadcastChatMessage(NetworkText.FromKey(key), messageColor);
                }
                halfLife = true;
            }
            if ((double)npc.life <= (double)npc.lifeMax * 0.2)
            {
                if (secondStage == false)
                {
                    string key = "Mods.CalamityMod.SupremeBossText8";
                    Color messageColor = Color.Orange;
                    if (Main.netMode == NetmodeID.SinglePlayer)
                    {
                        Main.NewText(Language.GetTextValue(key), messageColor);
                    }
                    else if (Main.netMode == NetmodeID.Server)
                    {
                        NetMessage.BroadcastChatMessage(NetworkText.FromKey(key), messageColor);
                    }
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Main.PlaySound(2, (int)npc.position.X, (int)npc.position.Y, 74);
                        for (int I = 0; I < 20; I++)
                        {
                            int FireEye = NPC.NewNPC((int)(npc.Center.X + (Math.Sin(I * 18) * 300)), (int)(npc.Center.Y + (Math.Cos(I * 18) * 300)), ModContent.NPCType<SoulSeekerSupreme>(), npc.whoAmI, 0, 0, 0, -1);
                            NPC Eye = Main.npc[FireEye];
                            Eye.ai[0] = I * 18;
                            Eye.ai[3] = I * 18;
                        }
                    }
                    secondStage = true;
                }
            }
            if (bossLife == 0f && npc.life > 0)
            {
                bossLife = (float)npc.lifeMax;
            }
            if (npc.life > 0)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    int num660 = (int)((double)npc.lifeMax * 0.55);
                    if ((float)(npc.life + num660) < bossLife)
                    {
                        bossLife = (float)npc.life;
                        NPC.SpawnOnPlayer(npc.FindClosestPlayer(), ModContent.NPCType<SupremeCataclysm>());
                        NPC.SpawnOnPlayer(npc.FindClosestPlayer(), ModContent.NPCType<SupremeCatastrophe>());
                        string key = "Mods.CalamityMod.SupremeBossText6";
                        Color messageColor = Color.Orange;
                        if (Main.netMode == NetmodeID.SinglePlayer)
                        {
                            Main.NewText(Language.GetTextValue(key), messageColor);
                        }
                        else if (Main.netMode == NetmodeID.Server)
                        {
                            NetMessage.BroadcastChatMessage(NetworkText.FromKey(key), messageColor);
                        }
                        return;
                    }
                }
            }
            #endregion
            #region TargetandRotation
            if (npc.target < 0 || npc.target == 255 || player.dead || !player.active)
            {
                npc.TargetClosest(true);
            }
            float num801 = npc.position.X + (float)(npc.width / 2) - player.position.X - (float)(player.width / 2);
            float num802 = npc.position.Y + (float)npc.height - 59f - player.position.Y - (float)(player.height / 2);
            float num803 = (float)Math.Atan2((double)num802, (double)num801) + 1.57f;
            if (num803 < 0f)
            {
                num803 += 6.283f;
            }
            else if ((double)num803 > 6.283)
            {
                num803 -= 6.283f;
            }
            float num804 = 0.1f;
            if (npc.rotation < num803)
            {
                if ((double)(num803 - npc.rotation) > 3.1415)
                {
                    npc.rotation -= num804;
                }
                else
                {
                    npc.rotation += num804;
                }
            }
            else if (npc.rotation > num803)
            {
                if ((double)(npc.rotation - num803) > 3.1415)
                {
                    npc.rotation += num804;
                }
                else
                {
                    npc.rotation -= num804;
                }
            }
            if (npc.rotation > num803 - num804 && npc.rotation < num803 + num804)
            {
                npc.rotation = num803;
            }
            if (npc.rotation < 0f)
            {
                npc.rotation += 6.283f;
            }
            else if ((double)npc.rotation > 6.283)
            {
                npc.rotation -= 6.283f;
            }
            if (npc.rotation > num803 - num804 && npc.rotation < num803 + num804)
            {
                npc.rotation = num803;
            }
            #endregion
            #region FirstStage
            if (npc.ai[0] == 0f)
            {
                npc.damage = npc.defDamage;
                if (wormAlive)
                {
                    npc.dontTakeDamage = true;
                    npc.chaseable = false;
                }
                else
                {
                    if (cataclysmAlive || catastropheAlive)
                    {
                        npc.dontTakeDamage = true;
                        npc.chaseable = false;
                        npc.damage = 0;
                        npc.TargetClosest(true);
                        npc.velocity *= 0.95f;
                        Vector2 vector92 = new Vector2(npc.Center.X, npc.Center.Y);
                        float num740 = player.Center.X - vector92.X;
                        float num741 = player.Center.Y - vector92.Y;
                        npc.rotation = (float)Math.Atan2((double)num741, (double)num740) - 1.57f;
                        return;
                    }
                    else
                    {
                        npc.dontTakeDamage = false;
                        npc.chaseable = true;
                    }
                }
                if (npc.ai[1] == 0f)
                {
                    float num823 = 12f;
                    float num824 = 0.12f;
                    Vector2 vector82 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
                    float num825 = player.position.X + (float)(player.width / 2) - vector82.X;
                    float num826 = player.position.Y + (float)(player.height / 2) - 550f - vector82.Y;
                    float num827 = (float)Math.Sqrt((double)(num825 * num825 + num826 * num826));
                    num827 = num823 / num827;
                    num825 *= num827;
                    num826 *= num827;
                    if (npc.velocity.X < num825)
                    {
                        npc.velocity.X = npc.velocity.X + num824;
                        if (npc.velocity.X < 0f && num825 > 0f)
                        {
                            npc.velocity.X = npc.velocity.X + num824;
                        }
                    }
                    else if (npc.velocity.X > num825)
                    {
                        npc.velocity.X = npc.velocity.X - num824;
                        if (npc.velocity.X > 0f && num825 < 0f)
                        {
                            npc.velocity.X = npc.velocity.X - num824;
                        }
                    }
                    if (npc.velocity.Y < num826)
                    {
                        npc.velocity.Y = npc.velocity.Y + num824;
                        if (npc.velocity.Y < 0f && num826 > 0f)
                        {
                            npc.velocity.Y = npc.velocity.Y + num824;
                        }
                    }
                    else if (npc.velocity.Y > num826)
                    {
                        npc.velocity.Y = npc.velocity.Y - num824;
                        if (npc.velocity.Y > 0f && num826 < 0f)
                        {
                            npc.velocity.Y = npc.velocity.Y - num824;
                        }
                    }
                    npc.ai[2] += 1f;
                    if (npc.ai[2] >= 300f)
                    {
                        npc.ai[1] = -1f;
                        npc.TargetClosest(true);
                        npc.netUpdate = true;
                    }
                    vector82 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
                    num825 = player.position.X + (float)(player.width / 2) - vector82.X;
                    num826 = player.position.Y + (float)(player.height / 2) - vector82.Y;
                    npc.rotation = (float)Math.Atan2((double)num826, (double)num825) - 1.57f;
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        npc.localAI[1] += wormAlive ? 0.5f : 1f;
                        if (npc.localAI[1] > 90f)
                        {
                            npc.localAI[1] = 0f;
                            float num828 = 10f * uDieLul;
                            int num829 = expertMode ? 150 : 200; //600 400
                            Vector2 value9 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
                            float num180 = player.position.X + (float)player.width * 0.5f - value9.X;
                            float num181 = Math.Abs(num180) * 0.1f;
                            float num182 = player.position.Y + (float)player.height * 0.5f - value9.Y - num181;
                            float num183 = (float)Math.Sqrt((double)(num180 * num180 + num182 * num182));
                            num183 = num828 / num183;
                            num180 *= num183;
                            num182 *= num183;
                            value9.X += num180;
                            value9.Y += num182;
                            int randomShot = Main.rand.Next(6); //0 to 5
                            if (randomShot == 0 && canFireSplitingFireball)
                            {
                                canFireSplitingFireball = false;
                                randomShot = ModContent.ProjectileType<BrimstoneFireblast>();
                                num827 = (float)Math.Sqrt((double)(num825 * num825 + num826 * num826));
                                num827 = num828 / num827;
                                num825 *= num827;
                                num826 *= num827;
                                vector82.X += num825 * 15f;
                                vector82.Y += num826 * 15f;
                                Projectile.NewProjectile(vector82.X, vector82.Y, num825, num826, randomShot, num829, 0f, Main.myPlayer, 0f, 0f);
                            }
                            else if (randomShot == 1 && canFireSplitingFireball)
                            {
                                canFireSplitingFireball = false;
                                randomShot = ModContent.ProjectileType<BrimstoneGigaBlast>();
                                num827 = (float)Math.Sqrt((double)(num825 * num825 + num826 * num826));
                                num827 = num828 / num827;
                                num825 *= num827;
                                num826 *= num827;
                                vector82.X += num825 * 15f;
                                vector82.Y += num826 * 15f;
                                Projectile.NewProjectile(vector82.X, vector82.Y, num825, num826, randomShot, num829, 0f, Main.myPlayer, 0f, 0f);
                            }
                            else
                            {
                                canFireSplitingFireball = true;
                                randomShot = ModContent.ProjectileType<BrimstoneBarrage>();
                                for (int num186 = 1; num186 <= 8; num186++)
                                {
                                    num180 = player.position.X + (float)player.width * 0.5f - value9.X;
                                    num182 = player.position.Y + (float)player.height * 0.5f - value9.Y;
                                    num183 = (float)Math.Sqrt((double)(num180 * num180 + num182 * num182));
                                    float speedBoost = (float)(num186 > 4 ? -(num186 - 4) : num186);
                                    num183 = (8f + speedBoost) / num183;
                                    num180 *= num183;
                                    num182 *= num183;
                                    Projectile.NewProjectile(value9.X, value9.Y, num180 + speedBoost, num182 + speedBoost, randomShot, num829, 0f, Main.myPlayer, 0f, 0f);
                                }
                            }
                            return;
                        }
                    }
                }
                else if (npc.ai[1] == 1f)
                {
                    npc.rotation = num803; //change
                    float num383 = wormAlive ? 26f : 30f;
                    if ((double)npc.life < (double)npc.lifeMax * 0.95)
                    {
                        num383 += 1f;
                    }
                    if ((double)npc.life < (double)npc.lifeMax * 0.85)
                    {
                        num383 += 1f;
                    }
                    if ((double)npc.life < (double)npc.lifeMax * 0.7)
                    {
                        num383 += 1f;
                    }
                    if ((double)npc.life < (double)npc.lifeMax * 0.6)
                    {
                        num383 += 1f;
                    }
                    if ((double)npc.life < (double)npc.lifeMax * 0.5)
                    {
                        num383 += 1f;
                    }
                    Vector2 vector37 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
                    float num384 = player.position.X + (float)(player.width / 2) - vector37.X;
                    float num385 = player.position.Y + (float)(player.height / 2) - vector37.Y;
                    float num386 = (float)Math.Sqrt((double)(num384 * num384 + num385 * num385));
                    num386 = num383 / num386;
                    npc.velocity.X = num384 * num386;
                    npc.velocity.Y = num385 * num386;
                    npc.ai[1] = 2f;
                }
                else if (npc.ai[1] == 2f)
                {
                    npc.ai[2] += 1f;
                    if (npc.ai[2] >= 25f)
                    {
                        npc.velocity.X = npc.velocity.X * 0.96f;
                        npc.velocity.Y = npc.velocity.Y * 0.96f;
                        if ((double)npc.velocity.X > -0.1 && (double)npc.velocity.X < 0.1)
                        {
                            npc.velocity.X = 0f;
                        }
                        if ((double)npc.velocity.Y > -0.1 && (double)npc.velocity.Y < 0.1)
                        {
                            npc.velocity.Y = 0f;
                        }
                    }
                    else
                    {
                        npc.rotation = (float)Math.Atan2((double)npc.velocity.Y, (double)npc.velocity.X) - 1.57f;
                    }
                    if (npc.ai[2] >= 70f)
                    {
                        npc.ai[3] += 1f;
                        npc.ai[2] = 0f;
                        npc.target = 255;
                        npc.rotation = num803;
                        if (npc.ai[3] >= 2f)
                        {
                            npc.ai[1] = -1f;
                        }
                        else
                        {
                            npc.ai[1] = 1f;
                        }
                    }
                }
                else if (npc.ai[1] == 3f)
                {
                    npc.TargetClosest(true);
                    float num412 = 32f;
                    float num413 = 1.2f;
                    int num414 = 1;
                    if (npc.position.X + (float)(npc.width / 2) < player.position.X + (float)player.width)
                    {
                        num414 = -1;
                    }
                    Vector2 vector40 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
                    float num415 = player.position.X + (float)(player.width / 2) + (float)(num414 * 600) - vector40.X;
                    float num416 = player.position.Y + (float)(player.height / 2) - vector40.Y;
                    float num417 = (float)Math.Sqrt((double)(num415 * num415 + num416 * num416));
                    num417 = num412 / num417;
                    num415 *= num417;
                    num416 *= num417;
                    if (npc.velocity.X < num415)
                    {
                        npc.velocity.X = npc.velocity.X + num413;
                        if (npc.velocity.X < 0f && num415 > 0f)
                        {
                            npc.velocity.X = npc.velocity.X + num413;
                        }
                    }
                    else if (npc.velocity.X > num415)
                    {
                        npc.velocity.X = npc.velocity.X - num413;
                        if (npc.velocity.X > 0f && num415 < 0f)
                        {
                            npc.velocity.X = npc.velocity.X - num413;
                        }
                    }
                    if (npc.velocity.Y < num416)
                    {
                        npc.velocity.Y = npc.velocity.Y + num413;
                        if (npc.velocity.Y < 0f && num416 > 0f)
                        {
                            npc.velocity.Y = npc.velocity.Y + num413;
                        }
                    }
                    else if (npc.velocity.Y > num416)
                    {
                        npc.velocity.Y = npc.velocity.Y - num413;
                        if (npc.velocity.Y > 0f && num416 < 0f)
                        {
                            npc.velocity.Y = npc.velocity.Y - num413;
                        }
                    }
                    npc.ai[2] += 1f;
                    if (npc.ai[2] >= 480f)
                    {
                        npc.ai[1] = -1f;
                        npc.target = 255;
                        npc.netUpdate = true;
                    }
                    else
                    {
                        if (!player.dead)
                        {
                            npc.ai[3] += wormAlive ? 0.5f : 1f;
                        }
                        if (npc.ai[3] >= 20f)
                        {
                            npc.ai[3] = 0f;
                            vector40 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
                            num415 = player.position.X + (float)(player.width / 2) - vector40.X;
                            num416 = player.position.Y + (float)(player.height / 2) - vector40.Y;
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                float num418 = 10f * uDieLul;
                                int num419 = expertMode ? 150 : 200; //600 500
                                int num420 = ModContent.ProjectileType<BrimstoneHellblast>();
                                num417 = (float)Math.Sqrt((double)(num415 * num415 + num416 * num416));
                                num417 = num418 / num417;
                                num415 *= num417;
                                num416 *= num417;
                                vector40.X += num415 * 4f;
                                vector40.Y += num416 * 4f;
                                Projectile.NewProjectile(vector40.X, vector40.Y, num415, num416, num420, num419, 0f, Main.myPlayer, 0f, 0f);
                            }
                        }
                    }
                }
                else if (npc.ai[1] == 4f)
                {
                    int num831 = 1;
                    if (npc.position.X + (float)(npc.width / 2) < player.position.X + (float)player.width)
                    {
                        num831 = -1;
                    }
                    float num832 = 32f;
                    float num833 = 1.2f;
                    Vector2 vector83 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
                    float num834 = player.position.X + (float)(player.width / 2) + (float)(num831 * 750) - vector83.X; //600
                    float num835 = player.position.Y + (float)(player.height / 2) - vector83.Y;
                    float num836 = (float)Math.Sqrt((double)(num834 * num834 + num835 * num835));
                    num836 = num832 / num836;
                    num834 *= num836;
                    num835 *= num836;
                    if (npc.velocity.X < num834)
                    {
                        npc.velocity.X = npc.velocity.X + num833;
                        if (npc.velocity.X < 0f && num834 > 0f)
                        {
                            npc.velocity.X = npc.velocity.X + num833;
                        }
                    }
                    else if (npc.velocity.X > num834)
                    {
                        npc.velocity.X = npc.velocity.X - num833;
                        if (npc.velocity.X > 0f && num834 < 0f)
                        {
                            npc.velocity.X = npc.velocity.X - num833;
                        }
                    }
                    if (npc.velocity.Y < num835)
                    {
                        npc.velocity.Y = npc.velocity.Y + num833;
                        if (npc.velocity.Y < 0f && num835 > 0f)
                        {
                            npc.velocity.Y = npc.velocity.Y + num833;
                        }
                    }
                    else if (npc.velocity.Y > num835)
                    {
                        npc.velocity.Y = npc.velocity.Y - num833;
                        if (npc.velocity.Y > 0f && num835 < 0f)
                        {
                            npc.velocity.Y = npc.velocity.Y - num833;
                        }
                    }
                    vector83 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
                    num834 = player.position.X + (float)(player.width / 2) - vector83.X;
                    num835 = player.position.Y + (float)(player.height / 2) - vector83.Y;
                    npc.rotation = (float)Math.Atan2((double)num835, (double)num834) - 1.57f;
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        npc.localAI[1] += wormAlive ? 0.5f : 1f;
                        if (npc.localAI[1] > 140f)
                        {
                            npc.localAI[1] = 0f;
                            float num837 = 5f * uDieLul;
                            int num838 = expertMode ? 150 : 200; //600 500
                            int num839 = ModContent.ProjectileType<BrimstoneFireblast>();
                            num836 = (float)Math.Sqrt((double)(num834 * num834 + num835 * num835));
                            num836 = num837 / num836;
                            num834 *= num836;
                            num835 *= num836;
                            vector83.X += num834 * 15f;
                            vector83.Y += num835 * 15f;
                            Projectile.NewProjectile(vector83.X, vector83.Y, num834, num835, num839, num838, 0f, Main.myPlayer, 0f, 0f);
                        }
                    }
                    npc.ai[2] += 1f;
                    if (npc.ai[2] >= 300f)
                    {
                        npc.ai[1] = -1f;
                        npc.TargetClosest(true);
                        npc.netUpdate = true;
                        return;
                    }
                }
                if (npc.ai[1] == -1f)
                {
                    phaseChange++;
                    if (phaseChange > 23)
                    {
                        phaseChange = 0;
                    }
                    int phase = 0; //0 = shots above 1 = charge 2 = nothing 3 = hellblasts 4 = fireblasts
                    switch (phaseChange)
                    {
                        case 0:
                            phase = 0;
                            willCharge = false;
                            break; //0341
                        case 1:
                            phase = 3;
                            break;
                        case 2:
                            phase = 4;
                            willCharge = true;
                            break;
                        case 3:
                            phase = 1;
                            break;
                        case 4:
                            phase = 1;
                            break; //1430
                        case 5:
                            phase = 4;
                            willCharge = false;
                            break;
                        case 6:
                            phase = 3;
                            break;
                        case 7:
                            phase = 0;
                            willCharge = true;
                            break;
                        case 8:
                            phase = 1;
                            break; //1034
                        case 9:
                            phase = 0;
                            willCharge = false;
                            break;
                        case 10:
                            phase = 3;
                            break;
                        case 11:
                            phase = 4;
                            break;
                        case 12:
                            phase = 4;
                            break; //4310
                        case 13:
                            phase = 3;
                            willCharge = true;
                            break;
                        case 14:
                            phase = 1;
                            break;
                        case 15:
                            phase = 0;
                            willCharge = false;
                            break;
                        case 16:
                            phase = 4;
                            break; //4411
                        case 17:
                            phase = 4;
                            willCharge = true;
                            break;
                        case 18:
                            phase = 1;
                            break;
                        case 19:
                            phase = 1;
                            break;
                        case 20:
                            phase = 0;
                            break; //0101
                        case 21:
                            phase = 1;
                            break;
                        case 22:
                            phase = 0;
                            break;
                        case 23:
                            phase = 1;
                            break;
                    }
                    npc.ai[1] = (float)phase;
                    npc.ai[2] = 0f;
                    npc.ai[3] = 0f;
                    return;
                }
                if ((double)npc.life < (double)npc.lifeMax * 0.4)
                {
                    npc.ai[0] = 1f;
                    npc.ai[1] = 0f;
                    npc.ai[2] = 0f;
                    npc.ai[3] = 0f;
                    npc.netUpdate = true;
                    return;
                }
            }
            #endregion
            #region Transition
            else if (npc.ai[0] == 1f || npc.ai[0] == 2f)
            {
                npc.dontTakeDamage = true;
                npc.chaseable = false;
                if (npc.ai[0] == 1f)
                {
                    npc.ai[2] += 0.005f;
                    if ((double)npc.ai[2] > 0.5)
                    {
                        npc.ai[2] = 0.5f;
                    }
                }
                else
                {
                    npc.ai[2] -= 0.005f;
                    if (npc.ai[2] < 0f)
                    {
                        npc.ai[2] = 0f;
                    }
                }
                npc.rotation += npc.ai[2];
                npc.ai[1] += 1f;
                if (npc.ai[1] == 100f)
                {
                    npc.ai[0] += 1f;
                    npc.ai[1] = 0f;
                    if (npc.ai[0] == 3f)
                    {
                        npc.ai[2] = 0f;
                    }
                    else
                    {
                        for (int num388 = 0; num388 < 50; num388++)
                        {
                            Dust.NewDust(npc.position, npc.width, npc.height, 235, (float)Main.rand.Next(-30, 31) * 0.2f, (float)Main.rand.Next(-30, 31) * 0.2f, 0, default, 1f);
                        }
                        Main.PlaySound(15, (int)npc.position.X, (int)npc.position.Y, 0, 1f, 0f);
                    }
                }
                Dust.NewDust(npc.position, npc.width, npc.height, 235, (float)Main.rand.Next(-30, 31) * 0.2f, (float)Main.rand.Next(-30, 31) * 0.2f, 0, default, 1f);
                npc.velocity.X = npc.velocity.X * 0.98f;
                npc.velocity.Y = npc.velocity.Y * 0.98f;
                if ((double)npc.velocity.X > -0.1 && (double)npc.velocity.X < 0.1)
                {
                    npc.velocity.X = 0f;
                }
                if ((double)npc.velocity.Y > -0.1 && (double)npc.velocity.Y < 0.1)
                {
                    npc.velocity.Y = 0f;
                    return;
                }
            }
            #endregion
            #region LastStage
            else
            {
                npc.damage = npc.defDamage;
                if (wormAlive)
                {
                    npc.dontTakeDamage = true;
                    npc.chaseable = false;
                }
                else
                {
                    if (NPC.AnyNPCs(ModContent.NPCType<SoulSeekerSupreme>()))
                    {
                        npc.dontTakeDamage = true;
                        npc.chaseable = false;
                    }
                    else
                    {
                        npc.dontTakeDamage = false;
                        npc.chaseable = true;
                    }
                }
                if (npc.ai[1] == 0f)
                {
                    float num823 = 12f;
                    float num824 = 0.12f;
                    Vector2 vector82 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
                    float num825 = player.position.X + (float)(player.width / 2) - vector82.X;
                    float num826 = player.position.Y + (float)(player.height / 2) - 550f - vector82.Y;
                    float num827 = (float)Math.Sqrt((double)(num825 * num825 + num826 * num826));
                    num827 = num823 / num827;
                    num825 *= num827;
                    num826 *= num827;
                    if (npc.velocity.X < num825)
                    {
                        npc.velocity.X = npc.velocity.X + num824;
                        if (npc.velocity.X < 0f && num825 > 0f)
                        {
                            npc.velocity.X = npc.velocity.X + num824;
                        }
                    }
                    else if (npc.velocity.X > num825)
                    {
                        npc.velocity.X = npc.velocity.X - num824;
                        if (npc.velocity.X > 0f && num825 < 0f)
                        {
                            npc.velocity.X = npc.velocity.X - num824;
                        }
                    }
                    if (npc.velocity.Y < num826)
                    {
                        npc.velocity.Y = npc.velocity.Y + num824;
                        if (npc.velocity.Y < 0f && num826 > 0f)
                        {
                            npc.velocity.Y = npc.velocity.Y + num824;
                        }
                    }
                    else if (npc.velocity.Y > num826)
                    {
                        npc.velocity.Y = npc.velocity.Y - num824;
                        if (npc.velocity.Y > 0f && num826 < 0f)
                        {
                            npc.velocity.Y = npc.velocity.Y - num824;
                        }
                    }
                    npc.ai[2] += 1f;
                    if (npc.ai[2] >= 240f)
                    {
                        npc.ai[1] = -1f;
                        npc.TargetClosest(true);
                        npc.netUpdate = true;
                    }
                    vector82 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
                    num825 = player.position.X + (float)(player.width / 2) - vector82.X;
                    num826 = player.position.Y + (float)(player.height / 2) - vector82.Y;
                    npc.rotation = (float)Math.Atan2((double)num826, (double)num825) - 1.57f;
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        npc.localAI[1] += wormAlive ? 0.5f : 1f;
                        if (npc.localAI[1] > 60f)
                        {
                            npc.localAI[1] = 0f;
                            float num828 = 10f * uDieLul;
                            int num829 = expertMode ? 150 : 200; //600 500
                            Vector2 value9 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
                            float num180 = player.position.X + (float)player.width * 0.5f - value9.X;
                            float num181 = Math.Abs(num180) * 0.1f;
                            float num182 = player.position.Y + (float)player.height * 0.5f - value9.Y - num181;
                            float num183 = (float)Math.Sqrt((double)(num180 * num180 + num182 * num182));
                            num183 = num828 / num183;
                            num180 *= num183;
                            num182 *= num183;
                            value9.X += num180;
                            value9.Y += num182;
                            int randomShot = Main.rand.Next(6);
                            if (randomShot == 0 && canFireSplitingFireball)
                            {
                                canFireSplitingFireball = false;
                                randomShot = ModContent.ProjectileType<BrimstoneFireblast>();
                                num827 = (float)Math.Sqrt((double)(num825 * num825 + num826 * num826));
                                num827 = num828 / num827;
                                num825 *= num827;
                                num826 *= num827;
                                vector82.X += num825 * 15f;
                                vector82.Y += num826 * 15f;
                                Projectile.NewProjectile(vector82.X, vector82.Y, num825, num826, randomShot, num829, 0f, Main.myPlayer, 0f, 0f);
                            }
                            else if (randomShot == 1 && canFireSplitingFireball)
                            {
                                canFireSplitingFireball = false;
                                randomShot = ModContent.ProjectileType<BrimstoneGigaBlast>();
                                num827 = (float)Math.Sqrt((double)(num825 * num825 + num826 * num826));
                                num827 = num828 / num827;
                                num825 *= num827;
                                num826 *= num827;
                                vector82.X += num825 * 15f;
                                vector82.Y += num826 * 15f;
                                Projectile.NewProjectile(vector82.X, vector82.Y, num825, num826, randomShot, num829, 0f, Main.myPlayer, 0f, 0f);
                            }
                            else
                            {
                                canFireSplitingFireball = true;
                                randomShot = ModContent.ProjectileType<BrimstoneBarrage>();
                                for (int num186 = 1; num186 <= 8; num186++)
                                {
                                    num180 = player.position.X + (float)player.width * 0.5f - value9.X;
                                    num182 = player.position.Y + (float)player.height * 0.5f - value9.Y;
                                    num183 = (float)Math.Sqrt((double)(num180 * num180 + num182 * num182));
                                    float speedBoost = (float)(num186 > 4 ? -(num186 - 4) : num186);
                                    num183 = (8f + speedBoost) / num183;
                                    num180 *= num183;
                                    num182 *= num183;
                                    Projectile.NewProjectile(value9.X, value9.Y, num180 + speedBoost, num182 + speedBoost, randomShot, num829, 0f, Main.myPlayer, 0f, 0f);
                                }
                            }
                            return;
                        }
                    }
                }
                else if (npc.ai[1] == 1f)
                {
                    npc.rotation = num803; //change
                    float num383 = wormAlive ? 31f : 35f;
                    if ((double)npc.life < (double)npc.lifeMax * 0.3)
                    {
                        num383 += 1f;
                    }
                    if ((double)npc.life < (double)npc.lifeMax * 0.2)
                    {
                        num383 += 1f;
                    }
                    if ((double)npc.life < (double)npc.lifeMax * 0.1)
                    {
                        num383 += 1f;
                    }
                    Vector2 vector37 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
                    float num384 = player.position.X + (float)(player.width / 2) - vector37.X;
                    float num385 = player.position.Y + (float)(player.height / 2) - vector37.Y;
                    float num386 = (float)Math.Sqrt((double)(num384 * num384 + num385 * num385));
                    num386 = num383 / num386;
                    npc.velocity.X = num384 * num386;
                    npc.velocity.Y = num385 * num386;
                    npc.ai[1] = 2f;
                }
                else if (npc.ai[1] == 2f)
                {
                    npc.ai[2] += 1f;
                    if (npc.ai[2] >= 25f)
                    {
                        npc.velocity.X = npc.velocity.X * 0.96f;
                        npc.velocity.Y = npc.velocity.Y * 0.96f;
                        if ((double)npc.velocity.X > -0.1 && (double)npc.velocity.X < 0.1)
                        {
                            npc.velocity.X = 0f;
                        }
                        if ((double)npc.velocity.Y > -0.1 && (double)npc.velocity.Y < 0.1)
                        {
                            npc.velocity.Y = 0f;
                        }
                    }
                    else
                    {
                        npc.rotation = (float)Math.Atan2((double)npc.velocity.Y, (double)npc.velocity.X) - 1.57f;
                    }
                    if (npc.ai[2] >= 70f)
                    {
                        npc.ai[3] += 1f;
                        npc.ai[2] = 0f;
                        npc.target = 255;
                        npc.rotation = num803; //change
                        if (npc.ai[3] >= 1f)
                        {
                            npc.ai[1] = -1f;
                        }
                        else
                        {
                            npc.ai[1] = 1f;
                        }
                    }
                }
                else if (npc.ai[1] == 3f)
                {
                    npc.TargetClosest(true);
                    float num412 = 32f;
                    float num413 = 1.2f;
                    int num414 = 1;
                    if (npc.position.X + (float)(npc.width / 2) < player.position.X + (float)player.width)
                    {
                        num414 = -1;
                    }
                    Vector2 vector40 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
                    float num415 = player.position.X + (float)(player.width / 2) + (float)(num414 * 600) - vector40.X;
                    float num416 = player.position.Y + (float)(player.height / 2) - vector40.Y;
                    float num417 = (float)Math.Sqrt((double)(num415 * num415 + num416 * num416));
                    num417 = num412 / num417;
                    num415 *= num417;
                    num416 *= num417;
                    if (npc.velocity.X < num415)
                    {
                        npc.velocity.X = npc.velocity.X + num413;
                        if (npc.velocity.X < 0f && num415 > 0f)
                        {
                            npc.velocity.X = npc.velocity.X + num413;
                        }
                    }
                    else if (npc.velocity.X > num415)
                    {
                        npc.velocity.X = npc.velocity.X - num413;
                        if (npc.velocity.X > 0f && num415 < 0f)
                        {
                            npc.velocity.X = npc.velocity.X - num413;
                        }
                    }
                    if (npc.velocity.Y < num416)
                    {
                        npc.velocity.Y = npc.velocity.Y + num413;
                        if (npc.velocity.Y < 0f && num416 > 0f)
                        {
                            npc.velocity.Y = npc.velocity.Y + num413;
                        }
                    }
                    else if (npc.velocity.Y > num416)
                    {
                        npc.velocity.Y = npc.velocity.Y - num413;
                        if (npc.velocity.Y > 0f && num416 < 0f)
                        {
                            npc.velocity.Y = npc.velocity.Y - num413;
                        }
                    }
                    npc.ai[2] += 1f;
                    if (npc.ai[2] >= 300f)
                    {
                        npc.ai[1] = -1f;
                        npc.target = 255;
                        npc.netUpdate = true;
                    }
                    else
                    {
                        if (!player.dead)
                        {
                            npc.ai[3] += wormAlive ? 0.5f : 1f;
                        }
                        if (npc.ai[3] >= 24f)
                        {
                            npc.ai[3] = 0f;
                            vector40 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
                            num415 = player.position.X + (float)(player.width / 2) - vector40.X;
                            num416 = player.position.Y + (float)(player.height / 2) - vector40.Y;
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                float num418 = 10f * uDieLul;
                                int num419 = expertMode ? 150 : 200; //600 500
                                int num420 = ModContent.ProjectileType<BrimstoneHellblast>();
                                num417 = (float)Math.Sqrt((double)(num415 * num415 + num416 * num416));
                                num417 = num418 / num417;
                                num415 *= num417;
                                num416 *= num417;
                                vector40.X += num415 * 4f;
                                vector40.Y += num416 * 4f;
                                Projectile.NewProjectile(vector40.X, vector40.Y, num415, num416, num420, num419, 0f, Main.myPlayer, 0f, 0f);
                            }
                        }
                    }
                }
                else if (npc.ai[1] == 4f)
                {
                    int num831 = 1;
                    if (npc.position.X + (float)(npc.width / 2) < player.position.X + (float)player.width)
                    {
                        num831 = -1;
                    }
                    float num832 = 32f;
                    float num833 = 1.2f;
                    Vector2 vector83 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
                    float num834 = player.position.X + (float)(player.width / 2) + (float)(num831 * 750) - vector83.X; //600
                    float num835 = player.position.Y + (float)(player.height / 2) - vector83.Y;
                    float num836 = (float)Math.Sqrt((double)(num834 * num834 + num835 * num835));
                    num836 = num832 / num836;
                    num834 *= num836;
                    num835 *= num836;
                    if (npc.velocity.X < num834)
                    {
                        npc.velocity.X = npc.velocity.X + num833;
                        if (npc.velocity.X < 0f && num834 > 0f)
                        {
                            npc.velocity.X = npc.velocity.X + num833;
                        }
                    }
                    else if (npc.velocity.X > num834)
                    {
                        npc.velocity.X = npc.velocity.X - num833;
                        if (npc.velocity.X > 0f && num834 < 0f)
                        {
                            npc.velocity.X = npc.velocity.X - num833;
                        }
                    }
                    if (npc.velocity.Y < num835)
                    {
                        npc.velocity.Y = npc.velocity.Y + num833;
                        if (npc.velocity.Y < 0f && num835 > 0f)
                        {
                            npc.velocity.Y = npc.velocity.Y + num833;
                        }
                    }
                    else if (npc.velocity.Y > num835)
                    {
                        npc.velocity.Y = npc.velocity.Y - num833;
                        if (npc.velocity.Y > 0f && num835 < 0f)
                        {
                            npc.velocity.Y = npc.velocity.Y - num833;
                        }
                    }
                    vector83 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
                    num834 = player.position.X + (float)(player.width / 2) - vector83.X;
                    num835 = player.position.Y + (float)(player.height / 2) - vector83.Y;
                    npc.rotation = (float)Math.Atan2((double)num835, (double)num834) - 1.57f;
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        npc.localAI[1] += wormAlive ? 0.5f : 1f;
                        if (npc.localAI[1] > 100f)
                        {
                            npc.localAI[1] = 0f;
                            float num837 = 5f * uDieLul;
                            int num838 = expertMode ? 150 : 200; //600 500
                            int num839 = ModContent.ProjectileType<BrimstoneFireblast>();
                            num836 = (float)Math.Sqrt((double)(num834 * num834 + num835 * num835));
                            num836 = num837 / num836;
                            num834 *= num836;
                            num835 *= num836;
                            vector83.X += num834 * 15f;
                            vector83.Y += num835 * 15f;
                            int shot = Projectile.NewProjectile(vector83.X, vector83.Y, num834, num835, num839, num838, 0f, Main.myPlayer, 0f, 0f);
                        }
                    }
                    npc.ai[2] += 1f;
                    if (npc.ai[2] >= 240f)
                    {
                        npc.ai[1] = -1f;
                        npc.TargetClosest(true);
                        npc.netUpdate = true;
                        return;
                    }
                }
                if (npc.ai[1] == -1f)
                {
                    phaseChange++;
                    if (phaseChange > 23)
                    {
                        phaseChange = 0;
                    }
                    int phase = 0; //0 = shots above 1 = charge 2 = nothing 3 = hellblasts 4 = fireblasts
                    switch (phaseChange)
                    {
                        case 0:
                            phase = 0;
                            willCharge = false;
                            break; //0341
                        case 1:
                            phase = 3;
                            break;
                        case 2:
                            phase = 4;
                            willCharge = true;
                            break;
                        case 3:
                            phase = 1;
                            break;
                        case 4:
                            phase = 1;
                            break; //1430
                        case 5:
                            phase = 4;
                            willCharge = false;
                            break;
                        case 6:
                            phase = 3;
                            break;
                        case 7:
                            phase = 0;
                            willCharge = true;
                            break;
                        case 8:
                            phase = 1;
                            break; //1034
                        case 9:
                            phase = 0;
                            willCharge = false;
                            break;
                        case 10:
                            phase = 3;
                            break;
                        case 11:
                            phase = 4;
                            break;
                        case 12:
                            phase = 4;
                            break; //4310
                        case 13:
                            phase = 3;
                            willCharge = true;
                            break;
                        case 14:
                            phase = 1;
                            break;
                        case 15:
                            phase = 0;
                            willCharge = false;
                            break;
                        case 16:
                            phase = 4;
                            break; //4411
                        case 17:
                            phase = 4;
                            willCharge = true;
                            break;
                        case 18:
                            phase = 1;
                            break;
                        case 19:
                            phase = 1;
                            break;
                        case 20:
                            phase = 0;
                            break; //0101
                        case 21:
                            phase = 1;
                            break;
                        case 22:
                            phase = 0;
                            break;
                        case 23:
                            phase = 1;
                            break;
                    }
                    npc.ai[1] = (float)phase;
                    npc.ai[2] = 0f;
                    npc.ai[3] = 0f;
                    return;
                }
            }
            #endregion
        }

        #region Loot
        public override void BossLoot(ref string name, ref int potionType)
        {
            potionType = ModContent.ItemType<OmegaHealingPotion>();
        }

        // If SCal is killed too quickly, cancel all drops and chastise the player
        public override bool SpecialNPCLoot()
        {
            if (lootTimer < 6000) //75 seconds for bullet hells + 25 seconds for normal phases
            {
                string key = "Mods.CalamityMod.SupremeBossText2";
                Color messageColor = Color.Orange;
                if (Main.netMode == NetmodeID.SinglePlayer)
                {
                    Main.NewText(Language.GetTextValue(key), messageColor);
                }
                else if (Main.netMode == NetmodeID.Server)
                {
                    NetMessage.BroadcastChatMessage(NetworkText.FromKey(key), messageColor);
                }
                return true;
            }

            return false;
        }

        public override void NPCLoot()
        {
            DeathMessage();

            // Incrase the player's SCal kill count
            if (Main.player[npc.target].Calamity().sCalKillCount < 5)
                Main.player[npc.target].Calamity().sCalKillCount++;

            // Materials
            int essenceMin = Main.expertMode ? 30 : 20;
            int essenceMax = Main.expertMode ? 40 : 30;
            DropHelper.DropItem(npc, ModContent.ItemType<CalamitousEssence>(), true, essenceMin, essenceMax);

            // Weapons
            // All non-hybrid weapons are listed twice so that the drop rates are actually equal between each unique weapon
            DropHelper.DropItemFromSetCondition(npc, Main.expertMode,
                ModContent.ItemType<Animus>(), ModContent.ItemType<Animus>(),
                ModContent.ItemType<Azathoth>(), ModContent.ItemType<Azathoth>(),
                ModContent.ItemType<Contagion>(), ModContent.ItemType<Contagion>(),
                ModContent.ItemType<CrystylCrusher>(), ModContent.ItemType<CrystylCrusher>(),
                ModContent.ItemType<DraconicDestruction>(), ModContent.ItemType<DraconicDestruction>(),
                ModContent.ItemType<Earth>(), ModContent.ItemType<Earth>(),
                ModContent.ItemType<Fabstaff>(), ModContent.ItemType<Fabstaff>(),
                ModContent.ItemType<RoyalKnivesMelee>(), ModContent.ItemType<RoyalKnives>(), // Illustrious Knives
                ModContent.ItemType<NanoblackReaperMelee>(), ModContent.ItemType<NanoblackReaperRogue>(),
                ModContent.ItemType<RedSun>(), ModContent.ItemType<RedSun>(),
                ModContent.ItemType<ScarletDevil>(), ModContent.ItemType<ScarletDevil>(),
                ModContent.ItemType<SomaPrime>(), ModContent.ItemType<SomaPrime>(),
                ModContent.ItemType<BlushieStaff>(), ModContent.ItemType<BlushieStaff>(), // Staff of Blushie
                ModContent.ItemType<Svantechnical>(), ModContent.ItemType<Svantechnical>(),
                ModContent.ItemType<Judgement>(), ModContent.ItemType<Judgement>(), // The Dance of Light
                ModContent.ItemType<TriactisTruePaladinianMageHammerofMightMelee>(), ModContent.ItemType<TriactisTruePaladinianMageHammerofMight>(),
                ModContent.ItemType<Megafleet>(), ModContent.ItemType<Megafleet>() // Voidragon
            );
            DropHelper.DropItemCondition(npc, ModContent.ItemType<Vehemenc>(), CalamityWorld.revenge);

            // Vanity
            DropHelper.DropItemCondition(npc, ModContent.ItemType<Levi>(), CalamityWorld.death);

            // Other
            DropHelper.DropItemCondition(npc, ModContent.ItemType<KnowledgeCalamitas>(), true, !CalamityWorld.downedSCal);
            DropHelper.DropResidentEvilAmmo(npc, CalamityWorld.downedSCal, 6, 3, 2);

            // Mark Supreme Calamitas as dead
            CalamityWorld.downedSCal = true;
            CalamityMod.UpdateServerBoolean();
        }
        #endregion

        private void DeathMessage()
        {
            Color messageColor = Color.Orange;
            string key;

            // If the player has never killed SCal before, comment on how many attempts it took
            if (Main.player[npc.target].Calamity().sCalKillCount == 0)
            {
                switch (Main.LocalPlayer.Calamity().sCalDeathCount)
                {
                    case 0:
                        key = "Mods.CalamityMod.SupremeBossText16";
                        break;
                    case 1:
                        key = "Mods.CalamityMod.SupremeBossText17";
                        break;
                    case 2:
                        key = "Mods.CalamityMod.SupremeBossText18";
                        break;
                    case 3: // Three deaths exactly rewards Lul
                        key = "Mods.CalamityMod.SupremeBossText19";
                        DropHelper.DropItem(npc, ModContent.ItemType<CheatTestThing>());
                        break;
                    default: // Four or more deaths: Lul is permanently missed
                        key = "Mods.CalamityMod.SupremeBossText10";
                        break;
                }
            }
            else
            {
                // If SCal has been killed before, instead comment on her respawning
                key = "Mods.CalamityMod.SupremeBossText10";
            }

            if (Main.netMode == NetmodeID.SinglePlayer)
                Main.NewText(Language.GetTextValue(key), messageColor);
            else if (Main.netMode == NetmodeID.Server)
                NetMessage.BroadcastChatMessage(NetworkText.FromKey(key), messageColor);
        }

        public override void ModifyHitByProjectile(Projectile projectile, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            if (projectile.type == ModContent.ProjectileType<SonOfYharon>())
            {
                damage /= 2;
            }
        }

        public override bool StrikeNPC(ref double damage, int defense, ref float knockback, int hitDirection, ref bool crit)
        {
            return !CalamityUtils.AntiButcher(npc, ref damage, 0.1f);
        }

        public override bool CheckActive()
        {
            return canDespawn;
        }

        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
        {
            scale = 1.5f;
            return null;
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            cooldownSlot = 1;
            return true;
        }

        public override void FindFrame(int frameHeight)
        {
            npc.frameCounter += 0.15f;
            npc.frameCounter %= Main.npcFrameCount[npc.type];
            int frame = (int)npc.frameCounter;
            npc.frame.Y = frame * frameHeight;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color drawColor)
        {
            Mod mod = ModLoader.GetMod("CalamityMod");
            Texture2D texture = Main.npcTexture[npc.type];
            if (npc.ai[0] > 1f)
            {
                texture = mod.GetTexture("NPCs/SupremeCalamitas/SupremeCalamitas2");
            }
            else
            {
                texture = Main.npcTexture[npc.type];
            }
            Color newColor = willCharge ? new Color(100, 0, 0, 0) : drawColor;
            SpriteEffects spriteEffects = SpriteEffects.None;
            Microsoft.Xna.Framework.Color color24 = npc.GetAlpha(newColor);
            Microsoft.Xna.Framework.Color color25 = Lighting.GetColor((int)((double)npc.position.X + (double)npc.width * 0.5) / 16, (int)(((double)npc.position.Y + (double)npc.height * 0.5) / 16.0));
            int num156 = Main.npcTexture[npc.type].Height / Main.npcFrameCount[npc.type];
            int y3 = num156 * (int)npc.frameCounter;
            Microsoft.Xna.Framework.Rectangle rectangle = new Microsoft.Xna.Framework.Rectangle(0, y3, texture.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;
            int num157 = 8;
            int num158 = 2;
            int num159 = 1;
            float num160 = 0f;
            int num161 = num159;
            while (((num158 > 0 && num161 < num157) || (num158 < 0 && num161 > num157)) && Lighting.NotRetro)
            {
                Microsoft.Xna.Framework.Color color26 = npc.GetAlpha(color25);
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
                Main.spriteBatch.Draw(texture, value4 + npc.Size / 2f - Main.screenPosition + new Vector2(0, npc.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color26, num165 + npc.rotation * num160 * (float)(num161 - 1) * -(float)spriteEffects.HasFlag(SpriteEffects.FlipHorizontally).ToDirectionInt(), origin2, npc.scale, spriteEffects, 0f);
                goto IL_6881;
            }
            var something = npc.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            spriteBatch.Draw(texture, npc.Center - Main.screenPosition + new Vector2(0, npc.gfxOffY), npc.frame, color24, npc.rotation, npc.frame.Size() / 2, npc.scale, something, 0);
            return false;
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(npc.position, npc.width, npc.height, 235, hitDirection, -1f, 0, default, 1f);
            }
            if (npc.life <= 0)
            {
                npc.position.X = npc.position.X + (float)(npc.width / 2);
                npc.position.Y = npc.position.Y + (float)(npc.height / 2);
                npc.width = 100;
                npc.height = 100;
                npc.position.X = npc.position.X - (float)(npc.width / 2);
                npc.position.Y = npc.position.Y - (float)(npc.height / 2);
                for (int num621 = 0; num621 < 40; num621++)
                {
                    int num622 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 235, 0f, 0f, 100, default, 2f);
                    Main.dust[num622].velocity *= 3f;
                    if (Main.rand.NextBool(2))
                    {
                        Main.dust[num622].scale = 0.5f;
                        Main.dust[num622].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                    }
                }
                for (int num623 = 0; num623 < 70; num623++)
                {
                    int num624 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 235, 0f, 0f, 100, default, 3f);
                    Main.dust[num624].noGravity = true;
                    Main.dust[num624].velocity *= 5f;
                    num624 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 235, 0f, 0f, 100, default, 2f);
                    Main.dust[num624].velocity *= 2f;
                }
            }
        }

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            npc.lifeMax = (int)(npc.lifeMax * 0.8f * bossLifeScale);
            npc.damage = (int)(npc.damage * 0.8f);
        }

        public override void OnHitPlayer(Player player, int damage, bool crit)
        {
            player.AddBuff(ModContent.BuffType<VulnerabilityHex>(), 600, true);
            if (CalamityWorld.revenge)
            {
                player.AddBuff(ModContent.BuffType<Horror>(), 600, true);
            }
        }
    }
}
