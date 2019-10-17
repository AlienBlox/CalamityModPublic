﻿
using CalamityMod.World;
using Microsoft.Xna.Framework;
using System.IO;
using System.Reflection;
using Terraria; using CalamityMod.Projectiles; using Terraria.ModLoader;
using Terraria.GameContent.Events;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles;  

namespace CalamityMod.NPCs
{
    [AutoloadBossHead]
    public class DesertScourgeHead : ModNPC
    {
        private bool flies = false;
        private float speed = 15f;
        private float turnSpeed = 0.15f;
        private const int minLength = 30;
        private const int maxLength = 31;
        private bool TailSpawned = false;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Desert Scourge");
        }

        public override void SetDefaults()
        {
            npc.damage = 30;
            npc.npcSlots = 12f;
            npc.width = 32;
            npc.height = 80;
			npc.LifeMaxNERD(2300, 2650, 5100, 16500000, 17500000);
            double HPBoost = Config.BossHealthPercentageBoost * 0.01;
            npc.lifeMax += (int)(npc.lifeMax * HPBoost);
            npc.aiStyle = 6;
            aiType = -1;
            npc.knockBackResist = 0f;
            npc.boss = true;
            npc.value = Item.buyPrice(0, 2, 0, 0);
            npc.alpha = 255;
            for (int k = 0; k < npc.buffImmune.Length; k++)
            {
                npc.buffImmune[k] = true;
            }
            npc.behindTiles = true;
            npc.noGravity = true;
            npc.noTileCollide = true;
            npc.HitSound = SoundID.NPCHit1;
            npc.DeathSound = SoundID.NPCDeath1;
            npc.netAlways = true;
            bossBag = ModContent.ItemType<Items.DesertScourgeBag>();
            Mod calamityModMusic = ModLoader.GetMod("CalamityModMusic");
            if (calamityModMusic != null)
                music = calamityModMusic.GetSoundSlot(SoundType.Music, "Sounds/Music/DesertScourge");
            else
                music = MusicID.Boss1;
            if (Main.expertMode)
            {
                npc.scale = 1.15f;
            }
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(npc.dontTakeDamage);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            npc.dontTakeDamage = reader.ReadBoolean();
        }

        public override void AI()
        {
            Player player = Main.player[npc.target];
            npc.dontTakeDamage = !player.ZoneDesert && !CalamityWorld.bossRushActive;
            bool expertMode = Main.expertMode || CalamityWorld.bossRushActive;
            float speedMult = expertMode ? 1.5f : 1.45f;
            if (CalamityWorld.death || CalamityWorld.bossRushActive)
                speedMult = 1.6f;
            if (npc.Calamity().enraged || (Config.BossRushXerocCurse && CalamityWorld.bossRushActive))
                speedMult = 2f;
            if (CalamityWorld.bossRushActive)
                speedMult *= 2f;

            float life = (float)npc.life;
            float totalLife = (float)npc.lifeMax;
            speed = 13f * (speedMult - (life / totalLife));
            turnSpeed = 0.13f * (speedMult - (life / totalLife));
            if (npc.ai[3] > 0f)
            {
                npc.realLife = (int)npc.ai[3];
            }
            if (npc.target < 0 || npc.target == 255 || player.dead)
            {
                npc.TargetClosest(true);
            }
            npc.velocity.Length();
            npc.alpha -= 42;
            if (npc.alpha < 0)
            {
                npc.alpha = 0;
            }
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                if (!TailSpawned && npc.ai[0] == 0f)
                {
                    int Previous = npc.whoAmI;
                    for (int num36 = 0; num36 < maxLength; num36++)
                    {
                        int lol;
                        if (num36 >= 0 && num36 < minLength)
                        {
                            lol = NPC.NewNPC((int)npc.position.X + (npc.width / 2), (int)npc.position.Y + (npc.height / 2), ModContent.NPCType<DesertScourgeBody>(), npc.whoAmI);
                        }
                        else
                        {
                            lol = NPC.NewNPC((int)npc.position.X + (npc.width / 2), (int)npc.position.Y + (npc.height / 2), ModContent.NPCType<DesertScourgeTail>(), npc.whoAmI);
                        }
                        Main.npc[lol].realLife = npc.whoAmI;
                        Main.npc[lol].ai[2] = (float)npc.whoAmI;
                        Main.npc[lol].ai[1] = (float)Previous;
                        Main.npc[Previous].ai[0] = (float)lol;
                        NetMessage.SendData(23, -1, -1, null, lol, 0f, 0f, 0f, 0);
                        Previous = lol;
                    }
                    TailSpawned = true;
                }
            }
            int num180 = (int)(npc.position.X / 16f) - 1;
            int num181 = (int)((npc.position.X + (float)npc.width) / 16f) + 2;
            int num182 = (int)(npc.position.Y / 16f) - 1;
            int num183 = (int)((npc.position.Y + (float)npc.height) / 16f) + 2;
            if (num180 < 0)
            {
                num180 = 0;
            }
            if (num181 > Main.maxTilesX)
            {
                num181 = Main.maxTilesX;
            }
            if (num182 < 0)
            {
                num182 = 0;
            }
            if (num183 > Main.maxTilesY)
            {
                num183 = Main.maxTilesY;
            }
            bool flag94 = flies;
            if (!flag94)
            {
                for (int num952 = num180; num952 < num181; num952++)
                {
                    for (int num953 = num182; num953 < num183; num953++)
                    {
                        if (Main.tile[num952, num953] != null && ((Main.tile[num952, num953].nactive() && (Main.tileSolid[(int)Main.tile[num952, num953].type] || (Main.tileSolidTop[(int)Main.tile[num952, num953].type] && Main.tile[num952, num953].frameY == 0))) || Main.tile[num952, num953].liquid > 64))
                        {
                            Vector2 vector105;
                            vector105.X = (float)(num952 * 16);
                            vector105.Y = (float)(num953 * 16);
                            if (npc.position.X + (float)npc.width > vector105.X && npc.position.X < vector105.X + 16f && npc.position.Y + (float)npc.height > vector105.Y && npc.position.Y < vector105.Y + 16f)
                            {
                                flag94 = true;
                                break;
                            }
                        }
                    }
                }
            }
            if (!flag94)
            {
                npc.localAI[1] = 1f;
                Rectangle rectangle12 = new Rectangle((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height);
                int num954 = (npc.Calamity().enraged || (Config.BossRushXerocCurse && CalamityWorld.bossRushActive)) ? 500 : 1000;
                if (CalamityWorld.bossRushActive)
                    num954 /= 2;

                bool flag95 = true;
                if (npc.position.Y > player.position.Y)
                {
                    for (int num955 = 0; num955 < 255; num955++)
                    {
                        if (Main.player[num955].active)
                        {
                            Rectangle rectangle13 = new Rectangle((int)Main.player[num955].position.X - num954, (int)Main.player[num955].position.Y - num954, num954 * 2, num954 * 2);
                            if (rectangle12.Intersects(rectangle13))
                            {
                                flag95 = false;
                                break;
                            }
                        }
                    }
                    if (flag95)
                    {
                        flag94 = true;
                    }
                }
            }
            else
            {
                npc.localAI[1] = 0f;
            }
            if (player.dead)
            {
                flag94 = false;
                npc.velocity.Y = npc.velocity.Y + 1f;
                if ((double)npc.position.Y > Main.worldSurface * 16.0)
                {
                    npc.velocity.Y = npc.velocity.Y + 1f;
                }
                if ((double)npc.position.Y > Main.rockLayer * 16.0)
                {
                    for (int num957 = 0; num957 < 200; num957++)
                    {
                        if (Main.npc[num957].aiStyle == npc.aiStyle)
                        {
                            Main.npc[num957].active = false;
                        }
                    }
                }
            }
            float num188 = speed;
            float num189 = turnSpeed;
            Vector2 vector18 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
            float num191 = player.position.X + (float)(player.width / 2);
            float num192 = player.position.Y + (float)(player.height / 2);
            num191 = (float)((int)(num191 / 16f) * 16);
            num192 = (float)((int)(num192 / 16f) * 16);
            vector18.X = (float)((int)(vector18.X / 16f) * 16);
            vector18.Y = (float)((int)(vector18.Y / 16f) * 16);
            num191 -= vector18.X;
            num192 -= vector18.Y;
            float num193 = (float)System.Math.Sqrt((double)(num191 * num191 + num192 * num192));
            if (!flag94)
            {
                npc.TargetClosest(true);
                npc.velocity.Y = npc.velocity.Y + (turnSpeed * 0.75f);
                if (npc.velocity.Y > num188)
                {
                    npc.velocity.Y = num188;
                }
                if ((double)(System.Math.Abs(npc.velocity.X) + System.Math.Abs(npc.velocity.Y)) < (double)num188 * 0.4)
                {
                    if (npc.velocity.X < 0f)
                    {
                        npc.velocity.X = npc.velocity.X - num189 * 1.1f;
                    }
                    else
                    {
                        npc.velocity.X = npc.velocity.X + num189 * 1.1f;
                    }
                }
                else if (npc.velocity.Y == num188)
                {
                    if (npc.velocity.X < num191)
                    {
                        npc.velocity.X = npc.velocity.X + num189;
                    }
                    else if (npc.velocity.X > num191)
                    {
                        npc.velocity.X = npc.velocity.X - num189;
                    }
                }
                else if (npc.velocity.Y > 4f)
                {
                    if (npc.velocity.X < 0f)
                    {
                        npc.velocity.X = npc.velocity.X + num189 * 0.9f;
                    }
                    else
                    {
                        npc.velocity.X = npc.velocity.X - num189 * 0.9f;
                    }
                }
            }
            else
            {
                if (!flies && npc.behindTiles && npc.soundDelay == 0)
                {
                    float num195 = num193 / 40f;
                    if (num195 < 10f)
                    {
                        num195 = 10f;
                    }
                    if (num195 > 20f)
                    {
                        num195 = 20f;
                    }
                    npc.soundDelay = (int)num195;
                    Main.PlaySound(15, (int)npc.position.X, (int)npc.position.Y, 1);
                }
                num193 = (float)System.Math.Sqrt((double)(num191 * num191 + num192 * num192));
                float num196 = System.Math.Abs(num191);
                float num197 = System.Math.Abs(num192);
                float num198 = num188 / num193;
                num191 *= num198;
                num192 *= num198;
                bool flag21 = false;
                if (!flag21)
                {
                    if ((npc.velocity.X > 0f && num191 > 0f) || (npc.velocity.X < 0f && num191 < 0f) || (npc.velocity.Y > 0f && num192 > 0f) || (npc.velocity.Y < 0f && num192 < 0f))
                    {
                        if (npc.velocity.X < num191)
                        {
                            npc.velocity.X = npc.velocity.X + num189;
                        }
                        else
                        {
                            if (npc.velocity.X > num191)
                            {
                                npc.velocity.X = npc.velocity.X - num189;
                            }
                        }
                        if (npc.velocity.Y < num192)
                        {
                            npc.velocity.Y = npc.velocity.Y + num189;
                        }
                        else
                        {
                            if (npc.velocity.Y > num192)
                            {
                                npc.velocity.Y = npc.velocity.Y - num189;
                            }
                        }
                        if ((double)System.Math.Abs(num192) < (double)num188 * 0.2 && ((npc.velocity.X > 0f && num191 < 0f) || (npc.velocity.X < 0f && num191 > 0f)))
                        {
                            if (npc.velocity.Y > 0f)
                            {
                                npc.velocity.Y = npc.velocity.Y + num189 * 2f;
                            }
                            else
                            {
                                npc.velocity.Y = npc.velocity.Y - num189 * 2f;
                            }
                        }
                        if ((double)System.Math.Abs(num191) < (double)num188 * 0.2 && ((npc.velocity.Y > 0f && num192 < 0f) || (npc.velocity.Y < 0f && num192 > 0f)))
                        {
                            if (npc.velocity.X > 0f)
                            {
                                npc.velocity.X = npc.velocity.X + num189 * 2f;
                            }
                            else
                            {
                                npc.velocity.X = npc.velocity.X - num189 * 2f;
                            }
                        }
                    }
                    else
                    {
                        if (num196 > num197)
                        {
                            if (npc.velocity.X < num191)
                            {
                                npc.velocity.X = npc.velocity.X + num189 * 1.1f;
                            }
                            else if (npc.velocity.X > num191)
                            {
                                npc.velocity.X = npc.velocity.X - num189 * 1.1f;
                            }
                            if ((double)(System.Math.Abs(npc.velocity.X) + System.Math.Abs(npc.velocity.Y)) < (double)num188 * 0.5)
                            {
                                if (npc.velocity.Y > 0f)
                                {
                                    npc.velocity.Y = npc.velocity.Y + num189;
                                }
                                else
                                {
                                    npc.velocity.Y = npc.velocity.Y - num189;
                                }
                            }
                        }
                        else
                        {
                            if (npc.velocity.Y < num192)
                            {
                                npc.velocity.Y = npc.velocity.Y + num189 * 1.1f;
                            }
                            else if (npc.velocity.Y > num192)
                            {
                                npc.velocity.Y = npc.velocity.Y - num189 * 1.1f;
                            }
                            if ((double)(System.Math.Abs(npc.velocity.X) + System.Math.Abs(npc.velocity.Y)) < (double)num188 * 0.5)
                            {
                                if (npc.velocity.X > 0f)
                                {
                                    npc.velocity.X = npc.velocity.X + num189;
                                }
                                else
                                {
                                    npc.velocity.X = npc.velocity.X - num189;
                                }
                            }
                        }
                    }
                }
                npc.rotation = (float)System.Math.Atan2((double)npc.velocity.Y, (double)npc.velocity.X) + 1.57f;
                if (flag94)
                {
                    if (npc.localAI[0] != 1f)
                    {
                        npc.netUpdate = true;
                    }
                    npc.localAI[0] = 1f;
                }
                else
                {
                    if (npc.localAI[0] != 0f)
                    {
                        npc.netUpdate = true;
                    }
                    npc.localAI[0] = 0f;
                }
                if (((npc.velocity.X > 0f && npc.oldVelocity.X < 0f) || (npc.velocity.X < 0f && npc.oldVelocity.X > 0f) || (npc.velocity.Y > 0f && npc.oldVelocity.Y < 0f) || (npc.velocity.Y < 0f && npc.oldVelocity.Y > 0f)) && !npc.justHit)
                {
                    npc.netUpdate = true;
                }
            }
        }

        #region Loot
        public override void BossLoot(ref string name, ref int potionType)
        {
            potionType = ItemID.SandBlock;
        }

        public override bool SpecialNPCLoot()
        {
            int closestSegmentID = DropHelper.FindClosestWormSegment(npc,
                ModContent.NPCType<DesertScourgeHead>(),
                ModContent.NPCType<DesertScourgeBody>(),
                ModContent.NPCType<DesertScourgeTail>());
            npc.position = Main.npc[closestSegmentID].position;
            return false;
        }

        public override void NPCLoot()
        {
            DropHelper.DropBags(npc);

            DropHelper.DropItem(npc, ItemID.LesserHealingPotion, 8, 14);
            DropHelper.DropItemChance(npc, ModContent.ItemType<Items.DesertScourgeTrophy>(), 10);
            DropHelper.DropItemCondition(npc, ModContent.ItemType<Items.KnowledgeDesertScourge>(), true, !CalamityWorld.downedDesertScourge);
            DropHelper.DropResidentEvilAmmo(npc, CalamityWorld.downedDesertScourge, 2, 0, 0);

            // All other drops are contained in the bag, so they only drop directly on Normal
            if (!Main.expertMode)
            {
                // Materials
                DropHelper.DropItem(npc, ModContent.ItemType<Items.VictoryShard>(), 7, 14);
                DropHelper.DropItem(npc, ItemID.Coral, 5, 9);
                DropHelper.DropItem(npc, ItemID.Seashell, 5, 9);
                DropHelper.DropItem(npc, ItemID.Starfish, 5, 9);

                // Weapons
                DropHelper.DropItemChance(npc, ModContent.ItemType<Items.AquaticDischarge>(), 4);
                DropHelper.DropItemChance(npc, ModContent.ItemType<Items.Barinade>(), 4);
                DropHelper.DropItemChance(npc, ModContent.ItemType<Items.StormSpray>(), 4);
                DropHelper.DropItemChance(npc, ModContent.ItemType<Items.SeaboundStaff>(), 4);
                float f = Main.rand.NextFloat();
                bool replaceWithRare = f <= DropHelper.RareVariantDropRateFloat; // 1/40 chance overall of getting Dune Hopper
                if (f < 0.25f) // 1/4 chance of getting Scourge of the Desert OR Dune Hopper replacing it
                {
                    DropHelper.DropItemCondition(npc, ModContent.ItemType<Items.ScourgeoftheDesert>(), !replaceWithRare);
                    DropHelper.DropItemCondition(npc, ModContent.ItemType<Items.DuneHopper>(), replaceWithRare);
                }

                // Equipment
                DropHelper.DropItemChance(npc, ModContent.ItemType<Items.AeroStone>(), 10);
                DropHelper.DropItemChance(npc, ModContent.ItemType<Items.DeepDiver>(), DropHelper.RareVariantDropRateInt);

                // Vanity
                DropHelper.DropItemChance(npc, ModContent.ItemType<Items.DesertScourgeMask>(), 7);

                // Fishing
                DropHelper.DropItemChance(npc, ItemID.HighTestFishingLine, 15);
                DropHelper.DropItemChance(npc, ItemID.AnglerTackleBag, 15);
                DropHelper.DropItemChance(npc, ItemID.TackleBox, 15);
                DropHelper.DropItemChance(npc, ItemID.AnglerEarring, 10);
                DropHelper.DropItemChance(npc, ItemID.FishermansGuide, 10);
                DropHelper.DropItemChance(npc, ItemID.WeatherRadio, 10);
                DropHelper.DropItemChance(npc, ItemID.Sextant, 10);
                DropHelper.DropItemChance(npc, ItemID.AnglerHat, 5);
                DropHelper.DropItemChance(npc, ItemID.AnglerVest, 5);
                DropHelper.DropItemChance(npc, ItemID.AnglerPants, 5);
                DropHelper.DropItemChance(npc, ItemID.FishingPotion, 5, 2, 3);
                DropHelper.DropItemChance(npc, ItemID.SonarPotion, 5, 2, 3);
                DropHelper.DropItemChance(npc, ItemID.CratePotion, 5, 2, 3);
                DropHelper.DropItemCondition(npc, ItemID.GoldenBugNet, NPC.downedBoss3, 20, 1, 1);
            }

            // If Desert Scourge has not been killed yet, notify players that the Sunken Sea is open and Sandstorms can happen
            if (!CalamityWorld.downedDesertScourge)
            {
                string key = "Mods.CalamityMod.OpenSunkenSea";
                Color messageColor = Color.Aquamarine;
                string key2 = "Mods.CalamityMod.SandstormTrigger";
                Color messageColor2 = Color.PaleGoldenrod;
                if (Main.netMode == NetmodeID.SinglePlayer)
                {
                    Main.NewText(Language.GetTextValue(key), messageColor);
                    Main.NewText(Language.GetTextValue(key2), messageColor2);
                }
                else if (Main.netMode == NetmodeID.Server)
                {
                    NetMessage.BroadcastChatMessage(NetworkText.FromKey(key), messageColor);
                    NetMessage.BroadcastChatMessage(NetworkText.FromKey(key2), messageColor2);
                }

                if (!Terraria.GameContent.Events.Sandstorm.Happening)
                    typeof(Terraria.GameContent.Events.Sandstorm).GetMethod("StartSandstorm", BindingFlags.Static | BindingFlags.NonPublic).Invoke(null, null);
            }

            // Mark Desert Scourge as dead
            CalamityWorld.downedDesertScourge = true;
            CalamityMod.UpdateServerBoolean();
        }
        #endregion

        public override void HitEffect(int hitDirection, double damage)
        {
            for (int k = 0; k < 3; k++)
            {
                Dust.NewDust(npc.position, npc.width, npc.height, 5, hitDirection, -1f, 0, default, 1f);
            }
            if (npc.life <= 0)
            {
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/ScourgeHead"), 1f);
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/ScourgeHead2"), 1f);
                for (int k = 0; k < 10; k++)
                {
                    Dust.NewDust(npc.position, npc.width, npc.height, 5, hitDirection, -1f, 0, default, 1f);
                }
            }
        }

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            npc.lifeMax = (int)(npc.lifeMax * 0.8f * bossLifeScale);
            npc.damage = (int)(npc.damage * 1.1f);
        }

        public override void OnHitPlayer(Player player, int damage, bool crit)
        {
            player.AddBuff(BuffID.Bleeding, 300, true);
        }
    }
}
