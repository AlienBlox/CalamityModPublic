﻿using System;
using System.IO;
using CalamityMod.BiomeManagers;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables;
using CalamityMod.Items.Placeables.Banners;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.NPCs.AdultEidolonWyrm;
using CalamityMod.NPCs.NormalNPCs;
using CalamityMod.Sounds;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;

namespace CalamityMod.NPCs.Abyss
{
    public class EidolonWyrmHead : ModNPC
    {
        private Vector2 patrolSpot = Vector2.Zero;
        public bool detectsPlayer = false;
        public const int minLength = 40;
        public const int maxLength = 41;
        public float speed = 5f; //10
        public float turnSpeed = 0.1f; //0.15
        bool TailSpawned = false;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Eidolon Wyrm");
            NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers(0)
            {
                CustomTexturePath = "CalamityMod/ExtraTextures/Bestiary/EidolonWyrm_Bestiary",
                PortraitPositionXOverride = 40
            };
            value.Position.X += 40;
            NPCID.Sets.NPCBestiaryDrawOffset[Type] = value;
        }

        public override void SetDefaults()
        {
            NPC.Calamity().canBreakPlayerDefense = true;
            NPC.npcSlots = 8f;
            NPC.damage = 170;
            NPC.width = 126; //36
            NPC.height = 76; //20
            NPC.defense = 70;
            NPC.DR_NERD(0.15f);
            NPC.lifeMax = 160000;
            NPC.aiStyle = -1;
            AIType = -1;
            NPC.knockBackResist = 0f;
            NPC.value = Item.buyPrice(0, 25, 0, 0);
            NPC.behindTiles = true;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath6;
            NPC.netAlways = true;
            NPC.rarity = 2;
            Banner = NPC.type;
            BannerItem = ModContent.ItemType<EidolonWyrmJuvenileBanner>();
            NPC.Calamity().VulnerableToHeat = false;
            NPC.Calamity().VulnerableToSickness = true;
            NPC.Calamity().VulnerableToElectricity = true;
            NPC.Calamity().VulnerableToWater = false;
            SpawnModBiomes = new int[2] { ModContent.GetInstance<AbyssLayer3Biome>().Type, ModContent.GetInstance<AbyssLayer4Biome>().Type };
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {

				// Will move to localization whenever that is cleaned up.
				new FlavorTextBestiaryInfoElement("Among any creature ever witnessed, it is these that stray furthest from our understanding. Such grand might, such mystery…")
            });
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.WriteVector2(patrolSpot);
            writer.Write(detectsPlayer);
            writer.Write(NPC.chaseable);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            patrolSpot = reader.ReadVector2();
            detectsPlayer = reader.ReadBoolean();
            NPC.chaseable = reader.ReadBoolean();
        }

        public override void AI()
        {
            bool adultWyrmAlive = false;
            SoundStyle roar = CalamityWorld.getFixedBoi ? Sunskater.DeathSound with { Pitch = Sunskater.DeathSound.Pitch - 0.5f } : CommonCalamitySounds.WyrmScreamSound;
            if (CalamityGlobalNPC.adultEidolonWyrmHead != -1)
            {
                if (Main.npc[CalamityGlobalNPC.adultEidolonWyrmHead].active)
                    adultWyrmAlive = true;
            }

            if (NPC.target < 0 || NPC.target == Main.maxPlayers || Main.player[NPC.target].dead)
            {
                NPC.TargetClosest(true);
            }
            if (NPC.justHit || detectsPlayer || Main.player[NPC.target].chaosState || adultWyrmAlive || (Main.player[NPC.target].Center - NPC.Center).Length() < Main.player[NPC.target].Calamity().GetAbyssAggro(160f))
            {
                detectsPlayer = true;
                NPC.damage = Main.expertMode ? 340 : 170;
            }
            else
            {
                NPC.damage = 0;
            }
            NPC.chaseable = detectsPlayer;
            if (detectsPlayer)
            {
                if (NPC.soundDelay <= 0)
                {
                    NPC.soundDelay = 420;
                    SoundEngine.PlaySound(roar, NPC.Center);
                }
            }
            else
            {
                if (Main.rand.NextBool(900))
                {
                    SoundEngine.PlaySound(roar, NPC.Center);
                }
            }
            if (NPC.ai[2] > 0f)
            {
                NPC.realLife = (int)NPC.ai[2];
            }
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                if (!TailSpawned && NPC.ai[0] == 0f)
                {
                    int Previous = NPC.whoAmI;
                    for (int num36 = 0; num36 < maxLength; num36++)
                    {
                        int lol;
                        if (num36 >= 0 && num36 < minLength)
                        {
                            if (num36 % 2 == 0)
                            {
                                lol = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.position.X + (NPC.width / 2), (int)NPC.position.Y + (NPC.height / 2), ModContent.NPCType<EidolonWyrmBody>(), NPC.whoAmI);
                            }
                            else
                            {
                                lol = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.position.X + (NPC.width / 2), (int)NPC.position.Y + (NPC.height / 2), ModContent.NPCType<EidolonWyrmBodyAlt>(), NPC.whoAmI);
                            }
                        }
                        else
                        {
                            lol = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.position.X + (NPC.width / 2), (int)NPC.position.Y + (NPC.height / 2), ModContent.NPCType<EidolonWyrmTail>(), NPC.whoAmI);
                        }
                        Main.npc[lol].realLife = NPC.whoAmI;
                        Main.npc[lol].ai[2] = (float)NPC.whoAmI;
                        Main.npc[lol].ai[1] = (float)Previous;
                        Main.npc[Previous].ai[0] = (float)lol;
                        NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, lol, 0f, 0f, 0f, 0);
                        Previous = lol;
                    }
                    TailSpawned = true;
                }
            }
            if (NPC.velocity.X < 0f)
            {
                NPC.spriteDirection = -1;
            }
            else if (NPC.velocity.X > 0f)
            {
                NPC.spriteDirection = 1;
            }
            if (Main.player[NPC.target].dead)
            {
                NPC.TargetClosest(false);

                NPC.velocity.Y += 3f;
                if (NPC.position.Y > Main.worldSurface * 16.0)
                    NPC.velocity.Y += 3f;

                if (NPC.position.Y > (Main.maxTilesY - 200) * 16.0)
                {
                    for (int a = 0; a < Main.maxNPCs; a++)
                    {
                        if (Main.npc[a].type == NPC.type || Main.npc[a].type == ModContent.NPCType<EidolonWyrmBodyAlt>() || Main.npc[a].type == ModContent.NPCType<EidolonWyrmBody>() || Main.npc[a].type == ModContent.NPCType<EidolonWyrmTail>())
                            Main.npc[a].active = false;
                    }
                }
            }
            NPC.alpha -= 42;
            if (NPC.alpha < 0)
            {
                NPC.alpha = 0;
            }
            if (Vector2.Distance(Main.player[NPC.target].Center, NPC.Center) > 6400f)
            {
                NPC.active = false;
            }

            float num188 = speed;
            float num189 = turnSpeed;
            Vector2 vector18 = new Vector2(NPC.position.X + NPC.width * 0.5f, NPC.position.Y + NPC.height * 0.5f);

            if (patrolSpot == Vector2.Zero)
                patrolSpot = Main.player[NPC.target].Center;

            float num191 = detectsPlayer ? Main.player[NPC.target].Center.X : patrolSpot.X;
            float num192 = detectsPlayer ? Main.player[NPC.target].Center.Y : patrolSpot.Y;

            if (!detectsPlayer)
            {
                num192 += 800;
                if (Math.Abs(NPC.Center.X - num191) < 400f) //500
                {
                    if (NPC.velocity.X > 0f)
                    {
                        num191 += 500f;
                    }
                    else
                    {
                        num191 -= 500f;
                    }
                }
            }
            else
            {
                num188 = 7.5f;
                num189 = 0.125f;
                if (!Main.player[NPC.target].wet)
                {
                    num188 = 15f;
                    num189 = 0.25f;
                }
                if (adultWyrmAlive)
                {
                    if (!Main.player[NPC.target].Calamity().ZoneAbyssLayer4)
                    {
                        num188 = 22.5f;
                        num189 = 0.375f;
                    }
                }
            }
            float num48 = num188 * 1.3f;
            float num49 = num188 * 0.7f;
            float num50 = NPC.velocity.Length();
            if (num50 > 0f)
            {
                if (num50 > num48)
                {
                    NPC.velocity.Normalize();
                    NPC.velocity *= num48;
                }
                else if (num50 < num49)
                {
                    NPC.velocity.Normalize();
                    NPC.velocity *= num49;
                }
            }
            num191 = (float)((int)(num191 / 16f) * 16);
            num192 = (float)((int)(num192 / 16f) * 16);
            vector18.X = (float)((int)(vector18.X / 16f) * 16);
            vector18.Y = (float)((int)(vector18.Y / 16f) * 16);
            num191 -= vector18.X;
            num192 -= vector18.Y;
            float num193 = (float)System.Math.Sqrt((double)(num191 * num191 + num192 * num192));
            float num196 = System.Math.Abs(num191);
            float num197 = System.Math.Abs(num192);
            float num198 = num188 / num193;
            num191 *= num198;
            num192 *= num198;
            if ((NPC.velocity.X > 0f && num191 > 0f) || (NPC.velocity.X < 0f && num191 < 0f) || (NPC.velocity.Y > 0f && num192 > 0f) || (NPC.velocity.Y < 0f && num192 < 0f))
            {
                if (NPC.velocity.X < num191)
                {
                    NPC.velocity.X = NPC.velocity.X + num189;
                }
                else
                {
                    if (NPC.velocity.X > num191)
                    {
                        NPC.velocity.X = NPC.velocity.X - num189;
                    }
                }
                if (NPC.velocity.Y < num192)
                {
                    NPC.velocity.Y = NPC.velocity.Y + num189;
                }
                else
                {
                    if (NPC.velocity.Y > num192)
                    {
                        NPC.velocity.Y = NPC.velocity.Y - num189;
                    }
                }
                if ((double)System.Math.Abs(num192) < (double)num188 * 0.2 && ((NPC.velocity.X > 0f && num191 < 0f) || (NPC.velocity.X < 0f && num191 > 0f)))
                {
                    if (NPC.velocity.Y > 0f)
                    {
                        NPC.velocity.Y = NPC.velocity.Y + num189 * 2f;
                    }
                    else
                    {
                        NPC.velocity.Y = NPC.velocity.Y - num189 * 2f;
                    }
                }
                if ((double)System.Math.Abs(num191) < (double)num188 * 0.2 && ((NPC.velocity.Y > 0f && num192 < 0f) || (NPC.velocity.Y < 0f && num192 > 0f)))
                {
                    if (NPC.velocity.X > 0f)
                    {
                        NPC.velocity.X = NPC.velocity.X + num189 * 2f; //changed from 2
                    }
                    else
                    {
                        NPC.velocity.X = NPC.velocity.X - num189 * 2f; //changed from 2
                    }
                }
            }
            else
            {
                if (num196 > num197)
                {
                    if (NPC.velocity.X < num191)
                    {
                        NPC.velocity.X = NPC.velocity.X + num189 * 1.1f; //changed from 1.1
                    }
                    else if (NPC.velocity.X > num191)
                    {
                        NPC.velocity.X = NPC.velocity.X - num189 * 1.1f; //changed from 1.1
                    }
                    if ((double)(System.Math.Abs(NPC.velocity.X) + System.Math.Abs(NPC.velocity.Y)) < (double)num188 * 0.5)
                    {
                        if (NPC.velocity.Y > 0f)
                        {
                            NPC.velocity.Y = NPC.velocity.Y + num189;
                        }
                        else
                        {
                            NPC.velocity.Y = NPC.velocity.Y - num189;
                        }
                    }
                }
                else
                {
                    if (NPC.velocity.Y < num192)
                    {
                        NPC.velocity.Y = NPC.velocity.Y + num189 * 1.1f;
                    }
                    else if (NPC.velocity.Y > num192)
                    {
                        NPC.velocity.Y = NPC.velocity.Y - num189 * 1.1f;
                    }
                    if ((double)(System.Math.Abs(NPC.velocity.X) + System.Math.Abs(NPC.velocity.Y)) < (double)num188 * 0.5)
                    {
                        if (NPC.velocity.X > 0f)
                        {
                            NPC.velocity.X = NPC.velocity.X + num189;
                        }
                        else
                        {
                            NPC.velocity.X = NPC.velocity.X - num189;
                        }
                    }
                }
            }
            NPC.rotation = (float)System.Math.Atan2((double)NPC.velocity.Y, (double)NPC.velocity.X) + 1.57f;
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            bool adultWyrmAlive = false;
            if (CalamityGlobalNPC.adultEidolonWyrmHead != -1)
            {
                if (Main.npc[CalamityGlobalNPC.adultEidolonWyrmHead].active)
                    adultWyrmAlive = true;
            }

            if (adultWyrmAlive)
                cooldownSlot = ImmunityCooldownID.Bosses;

            Rectangle targetHitbox = target.Hitbox;

            float dist1 = Vector2.Distance(NPC.Center, targetHitbox.TopLeft());
            float dist2 = Vector2.Distance(NPC.Center, targetHitbox.TopRight());
            float dist3 = Vector2.Distance(NPC.Center, targetHitbox.BottomLeft());
            float dist4 = Vector2.Distance(NPC.Center, targetHitbox.BottomRight());

            float minDist = dist1;
            if (dist2 < minDist)
                minDist = dist2;
            if (dist3 < minDist)
                minDist = dist3;
            if (dist4 < minDist)
                minDist = dist4;

            return minDist <= 40f;
        }

        public override bool? CanBeHitByProjectile(Projectile projectile)
        {
            if (projectile.minion && !projectile.Calamity().overridesMinionDamagePrevention)
            {
                return detectsPlayer;
            }
            return null;
        }

        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (NPC.spriteDirection == 1)
            {
                spriteEffects = SpriteEffects.FlipHorizontally;
            }
            Vector2 center = new Vector2(NPC.Center.X, NPC.Center.Y);
            Vector2 vector11 = new Vector2((float)(TextureAssets.Npc[NPC.type].Value.Width / 2), (float)(TextureAssets.Npc[NPC.type].Value.Height / Main.npcFrameCount[NPC.type] / 2));
            Vector2 vector = center - screenPos;
            vector -= new Vector2((float)ModContent.Request<Texture2D>("CalamityMod/NPCs/Abyss/EidolonWyrmHeadGlow").Value.Width, (float)(ModContent.Request<Texture2D>("CalamityMod/NPCs/Abyss/EidolonWyrmHeadGlow").Value.Height / Main.npcFrameCount[NPC.type])) * 1f / 2f;
            vector += vector11 * 1f + new Vector2(0f, NPC.gfxOffY);
            Color color = new Color(127 - NPC.alpha, 127 - NPC.alpha, 127 - NPC.alpha, 0).MultiplyRGBA(Color.White);
            Main.spriteBatch.Draw(ModContent.Request<Texture2D>("CalamityMod/NPCs/Abyss/EidolonWyrmHeadGlow").Value, vector,
                new Microsoft.Xna.Framework.Rectangle?(NPC.frame), color, NPC.rotation, vector11, 1f, spriteEffects, 0f);
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.Player.Calamity().ZoneAbyssLayer3 && spawnInfo.Water && !NPC.AnyNPCs(ModContent.NPCType<EidolonWyrmHead>()) &&
                !NPC.AnyNPCs(ModContent.NPCType<ReaperShark>()) && !NPC.AnyNPCs(ModContent.NPCType<ColossalSquid>()))
            {
                return SpawnCondition.CaveJellyfish.Chance * 0.3f;
            }
            if (spawnInfo.Player.Calamity().ZoneAbyssLayer4 && spawnInfo.Water && !NPC.AnyNPCs(ModContent.NPCType<EidolonWyrmHead>()))
            {
                return SpawnCondition.CaveJellyfish.Chance * 0.6f;
            }
            return 0f;
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            // Never drop anything if this Eidolon Wyrm is a minion during an AEW fight.
            var aewMinionCondition = npcLoot.DefineConditionalDropSet(AdultEidolonWyrmHead.CanMinionsDropThings);

            // 30-40 Voidstone
            aewMinionCondition.Add(ModContent.ItemType<Voidstone>(), 1, 30, 40);

            // Post-Polterghast: Soul Edge, Eidolic Wail, Stardust Staff
            LeadingConditionRule postPolter = npcLoot.DefineConditionalDropSet(DropHelper.PostPolter());
            aewMinionCondition.Add(postPolter);
            postPolter.Add(ModContent.ItemType<SoulEdge>(), 3);
            postPolter.Add(ModContent.ItemType<EidolicWail>(), 3);
            postPolter.Add(ModContent.ItemType<EidolonStaff>(), 3);

            // Post-Clone: 6-8 Lumenyl (8-11 on Expert)
            LeadingConditionRule postClone = npcLoot.DefineConditionalDropSet(DropHelper.PostCal());
            aewMinionCondition.Add(postClone);
            postClone.Add(DropHelper.NormalVsExpertQuantity(ModContent.ItemType<Lumenyl>(), 1, 6, 8, 8, 11));

            // Post-Plantera: 8-12 Ectoplasm
            aewMinionCondition.Add(ItemDropRule.ByCondition(new Conditions.DownedPlantera(), ItemID.Ectoplasm, 1, 8, 12));
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, 4, hitDirection, -1f, 0, default, 1f);
            }
            if (NPC.life <= 0)
            {
                for (int k = 0; k < 15; k++)
                {
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, 4, hitDirection, -1f, 0, default, 1f);
                }
                if (Main.netMode != NetmodeID.Server)
                {
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("Wyrm").Type, 1f);
                }
            }
        }

        public override bool CheckActive()
        {
            if (detectsPlayer && !Main.player[NPC.target].dead)
            {
                return false;
            }
            return true;
        }

        public override void OnHitPlayer(Player player, int damage, bool crit)
        {
            if (damage > 0)
                player.AddBuff(ModContent.BuffType<CrushDepth>(), 300, true);
        }
    }
}
