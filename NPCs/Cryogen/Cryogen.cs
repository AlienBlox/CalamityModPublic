﻿using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Effects;
using CalamityMod.Events;
using CalamityMod.Items.Accessories;
using CalamityMod.Items.Accessories.Wings;
using CalamityMod.Items.Armor.Vanity;
using CalamityMod.Items.LoreItems;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Furniture.BossRelics;
using CalamityMod.Items.Placeables.Furniture.DevPaintings;
using CalamityMod.Items.Placeables.Furniture.Trophies;
using CalamityMod.Items.TreasureBags;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.NPCs.TownNPCs;
using CalamityMod.Particles;
using CalamityMod.Projectiles.Boss;
using CalamityMod.Tiles.AstralSnow;
using CalamityMod.Tiles.Ores;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Terraria.GameContent.ItemDropRules;
using Terraria.Graphics.Shaders;

namespace CalamityMod.NPCs.Cryogen
{
    [AutoloadBossHead]
    public class Cryogen : ModNPC
    {
        private int biomeEnrageTimer = CalamityGlobalNPC.biomeEnrageTimerMax;
        private int currentPhase = 1;
        private int teleportLocationX = 0;

        public static Color BackglowColor => new Color(24, 100, 255, 80) * 0.6f;

        public override string Texture => "CalamityMod/NPCs/Cryogen/Cryogen_Phase1";

        public static readonly SoundStyle HitSound = new("CalamityMod/Sounds/NPCHit/CryogenHit", 3);
        public static readonly SoundStyle TransitionSound = new("CalamityMod/Sounds/NPCHit/CryogenPhaseTransitionCrack");
        public static readonly SoundStyle ShieldRegenSound = new("CalamityMod/Sounds/Custom/CryogenShieldRegenerate");
        public static readonly SoundStyle DeathSound = new("CalamityMod/Sounds/NPCKilled/CryogenDeath");

        public FireParticleSet FireDrawer = null;

        public static int cryoIconIndex;
        public static int pyroIconIndex;

        internal static void LoadHeadIcons()
        {
            string cryoIconPath = "CalamityMod/NPCs/Cryogen/Cryogen_Phase1_Head_Boss";
            string pyroIconPath = "CalamityMod/NPCs/Cryogen/Pyrogen_Head_Boss";

            CalamityMod.Instance.AddBossHeadTexture(cryoIconPath, -1);
            cryoIconIndex = ModContent.GetModBossHeadSlot(cryoIconPath);

            CalamityMod.Instance.AddBossHeadTexture(pyroIconPath, -1);
            pyroIconIndex = ModContent.GetModBossHeadSlot(pyroIconPath);
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cryogen");
            NPCID.Sets.BossBestiaryPriority.Add(Type);
			NPCID.Sets.MPAllowedEnemies[Type] = true;
        }

        public override void SetDefaults()
        {
            NPC.Calamity().canBreakPlayerDefense = true;
            NPC.npcSlots = 24f;
            NPC.GetNPCDamage();
            NPC.width = 86;
            NPC.height = 88;
            NPC.defense = 15;
            NPC.DR_NERD(0.3f);
            NPC.LifeMaxNERB(30000, 36000, 300000);
            double HPBoost = CalamityConfig.Instance.BossHealthBoost * 0.01;
            NPC.lifeMax += (int)(NPC.lifeMax * HPBoost);
            NPC.aiStyle = -1;
            AIType = -1;
            NPC.knockBackResist = 0f;
            NPC.value = Item.buyPrice(0, 40, 0, 0);
            NPC.boss = true;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.coldDamage = true;
            NPC.HitSound = HitSound;
            NPC.DeathSound = DeathSound;

            if (Main.getGoodWorld)
                NPC.scale *= 0.8f;

            if (CalamityWorld.getFixedBoi)
            {
                NPC.Calamity().VulnerableToHeat = false;
                NPC.Calamity().VulnerableToCold = true;
                NPC.Calamity().VulnerableToWater = true;
            }
            else
            {
                NPC.Calamity().VulnerableToHeat = true;
                NPC.Calamity().VulnerableToCold = false;
                NPC.Calamity().VulnerableToSickness = false;
            }
        }

        public override void BossHeadSlot(ref int index)
        {
            if (CalamityWorld.getFixedBoi)
                index = pyroIconIndex;
            else
                index = cryoIconIndex;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Snow,

				// Will move to localization whenever that is cleaned up.
				new FlavorTextBestiaryInfoElement("A prismatic living ice crystal. Though typically glimpsed only through the harsh sleet of blizzards, on the rare days where it is seen during a sunny day, its body gleams a deadly, beautiful blue.")
            });
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(biomeEnrageTimer);
            writer.Write(teleportLocationX);
            writer.Write(NPC.dontTakeDamage);
            for (int i = 0; i < 4; i++)
                writer.Write(NPC.Calamity().newAI[i]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            biomeEnrageTimer = reader.ReadInt32();
            teleportLocationX = reader.ReadInt32();
            NPC.dontTakeDamage = reader.ReadBoolean();
            for (int i = 0; i < 4; i++)
                NPC.Calamity().newAI[i] = reader.ReadSingle();
        }

        public override void AI()
        {
            CalamityGlobalNPC calamityGlobalNPC = NPC.Calamity();

            Lighting.AddLight((int)((NPC.position.X + (NPC.width / 2)) / 16f), (int)((NPC.position.Y + (NPC.height / 2)) / 16f), 0f, 1f, 1f);

            if (FireDrawer != null)
                FireDrawer.Update();

            // Get a target
            if (NPC.target < 0 || NPC.target == Main.maxPlayers || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
                NPC.TargetClosest();

            // Despawn safety, make sure to target another player if the current player target is too far away
            if (Vector2.Distance(Main.player[NPC.target].Center, NPC.Center) > CalamityGlobalNPC.CatchUpDistance200Tiles)
                NPC.TargetClosest();

            Player player = Main.player[NPC.target];

            bool bossRush = BossRushEvent.BossRushActive;
            bool expertMode = Main.expertMode || bossRush;
            bool revenge = CalamityWorld.revenge || bossRush;
            bool death = CalamityWorld.death || bossRush;

            // Enrage
            if (!player.ZoneSnow && !bossRush)
            {
                if (biomeEnrageTimer > 0)
                    biomeEnrageTimer--;
            }
            else
                biomeEnrageTimer = CalamityGlobalNPC.biomeEnrageTimerMax;

            bool biomeEnraged = biomeEnrageTimer <= 0 || bossRush;

            float enrageScale = death ? 0.5f : 0f;
            if (biomeEnraged)
            {
                NPC.Calamity().CurrentlyEnraged = !bossRush;
                enrageScale += 2f;
            }

            if (enrageScale > 2f)
                enrageScale = 2f;

            if (bossRush)
                enrageScale = 3f;

            // Percent life remaining
            float lifeRatio = NPC.life / (float)NPC.lifeMax;

            // Phases
            bool phase2 = lifeRatio < (revenge ? 0.85f : 0.8f) || death;
            bool phase3 = lifeRatio < (death ? 0.8f : revenge ? 0.7f : 0.6f);
            bool phase4 = lifeRatio < (death ? 0.6f : revenge ? 0.55f : 0.4f);
            bool phase5 = lifeRatio < (death ? 0.5f : revenge ? 0.45f : 0.3f);
            bool phase6 = lifeRatio < (death ? 0.35f : 0.25f) && revenge;
            bool phase7 = lifeRatio < (death ? 0.25f : 0.15f) && revenge;

            // Projectile and sound variables
            int iceBlast = CalamityWorld.getFixedBoi ? ModContent.ProjectileType<BrimstoneBarrage>() :  ModContent.ProjectileType<IceBlast>();
            int iceBomb = CalamityWorld.getFixedBoi ? ModContent.ProjectileType<SCalBrimstoneFireblast>() : ModContent.ProjectileType<IceBomb>();
            int iceRain = CalamityWorld.getFixedBoi ? ModContent.ProjectileType<BrimstoneBarrage>() : ModContent.ProjectileType<IceRain>();
            int dustType = CalamityWorld.getFixedBoi ? 235 : 67;

            SoundStyle frostSound = CalamityWorld.getFixedBoi ? SoundID.Item20 : SoundID.Item28;
            NPC.HitSound = CalamityWorld.getFixedBoi ? SoundID.NPCHit41 : HitSound;
            NPC.DeathSound = CalamityWorld.getFixedBoi ? SoundID.NPCDeath14 : DeathSound;

            // Reset damage
            NPC.damage = NPC.defDamage;

            if ((int)NPC.ai[0] + 1 > currentPhase)
                HandlePhaseTransition((int)NPC.ai[0] + 1);

            if (NPC.ai[2] == 0f && NPC.localAI[1] == 0f && Main.netMode != NetmodeID.MultiplayerClient && (NPC.ai[0] < 3f || bossRush || (death && NPC.ai[0] > 3f))) //spawn shield for phase 0 1 2, not 3 4 5
            {
                SoundEngine.PlaySound(ShieldRegenSound, NPC.Center);
                int num6 = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<CryogenShield>(), NPC.whoAmI);
                NPC.ai[2] = num6 + 1;
                NPC.localAI[1] = -1f;
                NPC.netUpdate = true;
                Main.npc[num6].ai[0] = NPC.whoAmI;
                Main.npc[num6].netUpdate = true;
            }

            int num7 = (int)NPC.ai[2] - 1;
            if (num7 != -1 && Main.npc[num7].active && Main.npc[num7].type == ModContent.NPCType<CryogenShield>())
            {
                NPC.dontTakeDamage = true;
            }
            else
            {
                NPC.dontTakeDamage = false;
                NPC.ai[2] = 0f;

                if (NPC.localAI[1] == -1f)
                    NPC.localAI[1] = death ? 540f : expertMode ? 720f : 1080f;
                if (NPC.localAI[1] > 0f)
                    NPC.localAI[1] -= 1f;
            }

            if (CalamityConfig.Instance.BossesStopWeather)
                CalamityMod.StopRain();
            else if (!Main.raining)
                CalamityUtils.StartRain();

            if (!player.active || player.dead)
            {
                NPC.TargetClosest(false);
                player = Main.player[NPC.target];
                if (!player.active || player.dead)
                {
                    if (NPC.velocity.Y > 3f)
                        NPC.velocity.Y = 3f;
                    NPC.velocity.Y -= 0.1f;
                    if (NPC.velocity.Y < -12f)
                        NPC.velocity.Y = -12f;

                    if (NPC.timeLeft > 60)
                        NPC.timeLeft = 60;

                    if (NPC.ai[1] != 0f)
                    {
                        NPC.ai[1] = 0f;
                        teleportLocationX = 0;
                        calamityGlobalNPC.newAI[2] = 0f;
                        NPC.netUpdate = true;
                    }
                    return;
                }
            }
            else if (NPC.timeLeft < 1800)
                NPC.timeLeft = 1800;

            float chargePhaseGateValue = bossRush ? 240f : 360f;
            float chargeDuration = 60f;
            float chargeTelegraphTime = NPC.ai[0] == 2f ? 80f : 120f;
            float chargeTelegraphMaxRotationIncrement = 1f;
            float chargeTelegraphRotationIncrement = chargeTelegraphMaxRotationIncrement / chargeTelegraphTime;
            float chargeSlowDownTime = 15f;
            float chargeVelocityMin = 12f;
            float chargeVelocityMax = 30f;
            if (Main.getGoodWorld)
            {
                chargePhaseGateValue *= 0.7f;
                chargeDuration *= 0.8f;
            }
            float chargeGateValue = chargePhaseGateValue + chargeTelegraphTime;
            float chargeSlownDownPhaseGateValue = chargeGateValue + chargeSlowDownTime;
            bool chargePhase = NPC.ai[1] >= chargePhaseGateValue;

            if (Main.netMode != NetmodeID.MultiplayerClient && expertMode && (NPC.ai[0] < 5f || !phase6) && !chargePhase)
            {
                calamityGlobalNPC.newAI[3] += 1f;
                if (calamityGlobalNPC.newAI[3] >= (bossRush ? 660f : 900f))
                {
                    calamityGlobalNPC.newAI[3] = 0f;
                    SoundEngine.PlaySound(CalamityWorld.getFixedBoi ? SoundID.NPCHit41 : HitSound, NPC.Center);
                    int totalProjectiles = 3;
                    float radians = MathHelper.TwoPi / totalProjectiles;
                    int type = iceBomb;
                    int damage = NPC.GetProjectileDamage(type);
                    float velocity = 2f + NPC.ai[0];
                    double angleA = radians * 0.5;
                    double angleB = MathHelper.ToRadians(90f) - angleA;
                    float velocityX = (float)(velocity * Math.Sin(angleA) / Math.Sin(angleB));
                    Vector2 spinningPoint = Main.rand.NextBool() ? new Vector2(0f, -velocity) : new Vector2(-velocityX, -velocity);
                    for (int k = 0; k < totalProjectiles; k++)
                    {
                        Vector2 vector255 = spinningPoint.RotatedBy(radians * k);
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + Vector2.Normalize(vector255) * 30f, vector255, type, damage, 0f, Main.myPlayer);
                    }
                }
            }

            if (NPC.ai[0] == 0f)
            {
                NPC.rotation = NPC.velocity.X * 0.1f;

                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    NPC.localAI[0] += 1f;
                    if (NPC.localAI[0] >= 120f)
                    {
                        NPC.localAI[0] = 0f;
                        NPC.TargetClosest();
                        if (Collision.CanHit(NPC.position, NPC.width, NPC.height, player.position, player.width, player.height))
                        {
                            SoundEngine.PlaySound(CalamityWorld.getFixedBoi ? SoundID.NPCHit41 : HitSound, NPC.Center);
                            int totalProjectiles = bossRush ? 24 : 16;
                            float radians = MathHelper.TwoPi / totalProjectiles;
                            int type = iceBlast;
                            int damage = NPC.GetProjectileDamage(type);
                            float velocity = 9f + enrageScale;
                            Vector2 spinningPoint = new Vector2(0f, -velocity);
                            for (int k = 0; k < totalProjectiles; k++)
                            {
                                Vector2 vector255 = spinningPoint.RotatedBy(radians * k);
                                Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + Vector2.Normalize(vector255) * 30f, vector255, type, damage, 0f, Main.myPlayer);
                            }
                        }
                    }
                }

                Vector2 vector142 = new Vector2(NPC.Center.X, NPC.Center.Y);
                float num1243 = player.Center.X - vector142.X;
                float num1244 = player.Center.Y - vector142.Y;
                float num1245 = (float)Math.Sqrt(num1243 * num1243 + num1244 * num1244);

                float num1246 = revenge ? 5f : 4f;
                num1246 += 4f * enrageScale;

                num1245 = num1246 / num1245;
                num1243 *= num1245;
                num1244 *= num1245;

                float inertia = 50f;
                if (Main.getGoodWorld)
                    inertia *= 0.5f;

                NPC.velocity.X = (NPC.velocity.X * inertia + num1243) / (inertia + 1f);
                NPC.velocity.Y = (NPC.velocity.Y * inertia + num1244) / (inertia + 1f);

                if (phase2)
                {
                    NPC.TargetClosest();
                    NPC.ai[0] = 1f;
                    NPC.localAI[0] = 0f;
                    NPC.netUpdate = true;
                }
            }
            else if (NPC.ai[0] == 1f)
            {
                if (NPC.ai[1] < chargePhaseGateValue)
                {
                    NPC.ai[1] += 1f;

                    NPC.rotation = NPC.velocity.X * 0.1f;

                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        NPC.localAI[0] += 1f;
                        if (NPC.localAI[0] >= 120f)
                        {
                            NPC.localAI[0] = 0f;
                            NPC.TargetClosest();
                            if (Collision.CanHit(NPC.position, NPC.width, NPC.height, player.position, player.width, player.height))
                            {
                                SoundEngine.PlaySound(CalamityWorld.getFixedBoi ? SoundID.NPCHit41 : HitSound, NPC.Center);
                                int totalProjectiles = bossRush ? 18 : 12;
                                float radians = MathHelper.TwoPi / totalProjectiles;
                                int type = iceBlast;
                                int damage = NPC.GetProjectileDamage(type);
                                float velocity2 = 9f + enrageScale;
                                Vector2 spinningPoint = new Vector2(0f, -velocity2);
                                for (int k = 0; k < totalProjectiles; k++)
                                {
                                    Vector2 vector255 = spinningPoint.RotatedBy(radians * k);
                                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + Vector2.Normalize(vector255) * 30f, vector255, type, damage, 0f, Main.myPlayer);
                                }
                            }
                        }
                    }

                    float velocity = revenge ? 3.5f : 4f;
                    float acceleration = 0.15f;
                    velocity -= enrageScale * 0.8f;
                    acceleration += 0.07f * enrageScale;

                    if (NPC.position.Y > player.position.Y - 375f)
                    {
                        if (NPC.velocity.Y > 0f)
                            NPC.velocity.Y *= 0.98f;

                        NPC.velocity.Y -= acceleration;

                        if (NPC.velocity.Y > velocity)
                            NPC.velocity.Y = velocity;
                    }
                    else if (NPC.position.Y < player.position.Y - 425f)
                    {
                        if (NPC.velocity.Y < 0f)
                            NPC.velocity.Y *= 0.98f;

                        NPC.velocity.Y += acceleration;

                        if (NPC.velocity.Y < -velocity)
                            NPC.velocity.Y = -velocity;
                    }

                    if (NPC.position.X + (NPC.width / 2) > player.position.X + (player.width / 2) + 300f)
                    {
                        if (NPC.velocity.X > 0f)
                            NPC.velocity.X *= 0.98f;

                        NPC.velocity.X -= acceleration;

                        if (NPC.velocity.X > velocity)
                            NPC.velocity.X = velocity;
                    }
                    if (NPC.position.X + (NPC.width / 2) < player.position.X + (player.width / 2) - 300f)
                    {
                        if (NPC.velocity.X < 0f)
                            NPC.velocity.X *= 0.98f;

                        NPC.velocity.X += acceleration;

                        if (NPC.velocity.X < -velocity)
                            NPC.velocity.X = -velocity;
                    }
                }
                else if (NPC.ai[1] < chargeGateValue)
                {
                    NPC.ai[1] += 1f;

                    float totalSpreads = 3f;
                    if ((NPC.ai[1] - chargePhaseGateValue) % (chargeTelegraphTime / totalSpreads) == 0f)
                    {
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            if (Collision.CanHit(NPC.position, NPC.width, NPC.height, player.position, player.width, player.height))
                            {
                                SoundEngine.PlaySound(CalamityWorld.getFixedBoi ? SoundID.NPCHit41 : HitSound, NPC.Center);
                                int type = iceRain;
                                int damage = NPC.GetProjectileDamage(type);
                                float maxVelocity = 9f + enrageScale;
                                float velocity = maxVelocity - (calamityGlobalNPC.newAI[0] * maxVelocity * 0.5f);
                                int totalProjectiles = 10;
                                int maxTotalProjectileReductionBasedOnRotationSpeed = (int)(totalProjectiles * 0.7f);
                                int totalProjectilesShot = totalProjectiles - (int)Math.Round(calamityGlobalNPC.newAI[0] * maxTotalProjectileReductionBasedOnRotationSpeed);
                                for (int i = 0; i < 2; i++)
                                {
                                    float radians = MathHelper.TwoPi / totalProjectilesShot;
                                    float newVelocity = velocity - (velocity * 0.5f * i);
                                    double angleA = radians * 0.5;
                                    double angleB = MathHelper.ToRadians(90f) - angleA;
                                    float velocityX = (float)(newVelocity * Math.Sin(angleA) / Math.Sin(angleB));
                                    Vector2 spinningPoint = i == 0 ? new Vector2(0f, -newVelocity) : new Vector2(-velocityX, -newVelocity);
                                    for (int k = 0; k < totalProjectilesShot; k++)
                                    {
                                        Vector2 vector255 = spinningPoint.RotatedBy(radians * k);
                                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + Vector2.Normalize(vector255) * 30f, vector255, type, damage, 0f, Main.myPlayer, 0f, velocity);
                                    }
                                }
                            }
                        }
                    }

                    calamityGlobalNPC.newAI[0] += chargeTelegraphRotationIncrement;
                    NPC.rotation += calamityGlobalNPC.newAI[0];
                    NPC.velocity *= 0.98f;
                }
                else
                {
                    if (NPC.ai[1] == chargeGateValue)
                    {
                        float chargeVelocity = Vector2.Distance(NPC.Center, player.Center) / chargeDuration * 2f;
                        NPC.velocity = Vector2.Normalize(player.Center - NPC.Center) * (chargeVelocity + enrageScale * 2f);

                        if (NPC.velocity.Length() < chargeVelocityMin)
                        {
                            NPC.velocity.Normalize();
                            NPC.velocity *= chargeVelocityMin;
                        }

                        if (NPC.velocity.Length() > chargeVelocityMax)
                        {
                            NPC.velocity.Normalize();
                            NPC.velocity *= chargeVelocityMax;
                        }

                        NPC.ai[1] = chargeGateValue + chargeDuration;
                        calamityGlobalNPC.newAI[0] = 0f;
                    }

                    NPC.ai[1] -= 1f;
                    if (NPC.ai[1] == chargeGateValue)
                    {
                        NPC.TargetClosest();

                        NPC.ai[1] = 0f;
                        NPC.localAI[0] = 0f;

                        NPC.rotation = NPC.velocity.X * 0.1f;
                    }
                    else if (NPC.ai[1] <= chargeSlownDownPhaseGateValue)
                    {
                        NPC.velocity *= 0.95f;
                        NPC.rotation = NPC.velocity.X * 0.15f;
                    }
                    else
                        NPC.rotation += NPC.direction * 0.5f;
                }

                if (phase3)
                {
                    NPC.TargetClosest();
                    NPC.ai[0] = 2f;
                    NPC.ai[1] = 0f;
                    NPC.localAI[0] = 0f;
                    calamityGlobalNPC.newAI[0] = 0f;
                    calamityGlobalNPC.newAI[2] = 0f;
                    NPC.netUpdate = true;
                }
            }
            else if (NPC.ai[0] == 2f)
            {
                if (NPC.ai[1] < chargePhaseGateValue)
                {
                    NPC.ai[1] += 1f;

                    NPC.rotation = NPC.velocity.X * 0.1f;

                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        NPC.localAI[0] += 1f;
                        if (NPC.localAI[0] >= 120f)
                        {
                            NPC.localAI[0] = 0f;
                            NPC.TargetClosest();
                            if (Collision.CanHit(NPC.position, NPC.width, NPC.height, player.position, player.width, player.height))
                            {
                                SoundEngine.PlaySound(CalamityWorld.getFixedBoi ? SoundID.NPCHit41 : HitSound, NPC.Center);
                                int totalProjectiles = bossRush ? 18 : 12;
                                float radians = MathHelper.TwoPi / totalProjectiles;
                                int type = iceBlast;
                                int damage = NPC.GetProjectileDamage(type);
                                float velocity = 9f + enrageScale;
                                Vector2 spinningPoint = new Vector2(0f, -velocity);
                                for (int k = 0; k < totalProjectiles; k++)
                                {
                                    Vector2 vector255 = spinningPoint.RotatedBy(radians * k);
                                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + Vector2.Normalize(vector255) * 30f, vector255, type, damage, 0f, Main.myPlayer);
                                }
                            }
                        }
                    }

                    Vector2 vector142 = new Vector2(NPC.Center.X, NPC.Center.Y);
                    float num1243 = player.Center.X - vector142.X;
                    float num1244 = player.Center.Y - vector142.Y;
                    float num1245 = (float)Math.Sqrt(num1243 * num1243 + num1244 * num1244);

                    float num1246 = revenge ? 7f : 6f;
                    num1246 += 4f * enrageScale;

                    num1245 = num1246 / num1245;
                    num1243 *= num1245;
                    num1244 *= num1245;

                    float inertia = 50f;
                    if (Main.getGoodWorld)
                        inertia *= 0.5f;

                    NPC.velocity.X = (NPC.velocity.X * inertia + num1243) / (inertia + 1f);
                    NPC.velocity.Y = (NPC.velocity.Y * inertia + num1244) / (inertia + 1f);
                }
                else if (NPC.ai[1] < chargeGateValue)
                {
                    NPC.ai[1] += 1f;

                    float totalSpreads = 2f;
                    if ((NPC.ai[1] - chargePhaseGateValue) % (chargeTelegraphTime / totalSpreads) == 0f)
                    {
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            if (Collision.CanHit(NPC.position, NPC.width, NPC.height, player.position, player.width, player.height))
                            {
                                SoundEngine.PlaySound(CalamityWorld.getFixedBoi ? SoundID.NPCHit41 : HitSound, NPC.Center);
                                int type = iceRain;
                                int damage = NPC.GetProjectileDamage(type);
                                float maxVelocity = 9f + enrageScale;
                                float velocity = maxVelocity - (calamityGlobalNPC.newAI[0] * maxVelocity * 0.5f);
                                int totalProjectiles = calamityGlobalNPC.newAI[1] == 0f ? 8 : 4;
                                int maxTotalProjectileReductionBasedOnRotationSpeed = (int)(totalProjectiles * 0.4f);
                                int totalProjectilesShot = totalProjectiles - (int)Math.Round(calamityGlobalNPC.newAI[0] * maxTotalProjectileReductionBasedOnRotationSpeed);
                                for (int i = 0; i < 3; i++)
                                {
                                    float radians = MathHelper.TwoPi / totalProjectilesShot;
                                    float newVelocity = velocity - (velocity * 0.33f * i);
                                    double angleA = radians * 0.5;
                                    double angleB = MathHelper.ToRadians(90f) - angleA;
                                    float velocityX = (float)(newVelocity * Math.Sin(angleA) / Math.Sin(angleB));
                                    Vector2 spinningPoint = i == 1 ? new Vector2(0f, -newVelocity) : new Vector2(-velocityX, -newVelocity);
                                    for (int k = 0; k < totalProjectilesShot; k++)
                                    {
                                        Vector2 vector255 = spinningPoint.RotatedBy(radians * k);
                                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + Vector2.Normalize(vector255) * 30f, vector255, type, damage, 0f, Main.myPlayer, 0f, velocity);
                                    }
                                }
                            }
                        }
                    }

                    calamityGlobalNPC.newAI[0] += chargeTelegraphRotationIncrement;
                    NPC.rotation += calamityGlobalNPC.newAI[0];
                    NPC.velocity *= 0.98f;
                }
                else
                {
                    if (NPC.ai[1] == chargeGateValue)
                    {
                        float chargeVelocity = Vector2.Distance(NPC.Center, player.Center) / chargeDuration * 2f;
                        NPC.velocity = Vector2.Normalize(player.Center - NPC.Center) * (chargeVelocity + enrageScale * 2f);

                        if (NPC.velocity.Length() < chargeVelocityMin)
                        {
                            NPC.velocity.Normalize();
                            NPC.velocity *= chargeVelocityMin;
                        }

                        if (NPC.velocity.Length() > chargeVelocityMax)
                        {
                            NPC.velocity.Normalize();
                            NPC.velocity *= chargeVelocityMax;
                        }

                        NPC.ai[1] = chargeGateValue + chargeDuration;
                        calamityGlobalNPC.newAI[0] = 0f;
                    }

                    NPC.ai[1] -= 1f;
                    if (NPC.ai[1] == chargeGateValue)
                    {
                        NPC.TargetClosest();

                        calamityGlobalNPC.newAI[1] += 1f;
                        if (calamityGlobalNPC.newAI[1] > 1f)
                        {
                            NPC.ai[1] = 0f;
                            NPC.localAI[0] = 0f;
                            calamityGlobalNPC.newAI[1] = 0f;
                        }
                        else
                            NPC.ai[1] = chargePhaseGateValue;

                        NPC.rotation = NPC.velocity.X * 0.1f;
                    }
                    else if (NPC.ai[1] <= chargeSlownDownPhaseGateValue)
                    {
                        NPC.velocity *= 0.95f;
                        NPC.rotation = NPC.velocity.X * 0.15f;
                    }
                    else
                        NPC.rotation += NPC.direction * 0.5f;
                }

                if (phase4)
                {
                    NPC.TargetClosest();
                    NPC.ai[0] = 3f;
                    NPC.ai[1] = 0f;
                    NPC.localAI[0] = 0f;
                    calamityGlobalNPC.newAI[0] = 0f;
                    calamityGlobalNPC.newAI[1] = 0f;
                    NPC.netUpdate = true;
                }
            }
            else if (NPC.ai[0] == 3f)
            {
                NPC.rotation = NPC.velocity.X * 0.1f;

                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    NPC.localAI[0] += 1f;
                    if (NPC.localAI[0] >= 90f && NPC.Opacity == 1f)
                    {
                        NPC.localAI[0] = 0f;
                        if (Collision.CanHit(NPC.position, NPC.width, NPC.height, player.position, player.width, player.height))
                        {
                            SoundEngine.PlaySound(CalamityWorld.getFixedBoi ? SoundID.NPCHit41 : HitSound, NPC.Center);
                            int totalProjectiles = bossRush ? 18 : 12;
                            float radians = MathHelper.TwoPi / totalProjectiles;
                            int type = iceBlast;
                            int damage = NPC.GetProjectileDamage(type);
                            float velocity = 10f + enrageScale;
                            Vector2 spinningPoint = new Vector2(0f, -velocity);
                            for (int k = 0; k < totalProjectiles; k++)
                            {
                                Vector2 vector255 = spinningPoint.RotatedBy(radians * k);
                                Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + Vector2.Normalize(vector255) * 30f, vector255, type, damage, 0f, Main.myPlayer);
                            }
                        }
                    }
                }

                Vector2 vector142 = new Vector2(NPC.Center.X, NPC.Center.Y);
                float num1243 = player.Center.X - vector142.X;
                float num1244 = player.Center.Y - vector142.Y;
                float num1245 = (float)Math.Sqrt(num1243 * num1243 + num1244 * num1244);

                float speed = revenge ? 5.5f : 5f;
                speed += 3f * enrageScale;

                num1245 = speed / num1245;
                num1243 *= num1245;
                num1244 *= num1245;

                float inertia = 50f;
                if (Main.getGoodWorld)
                    inertia *= 0.5f;

                NPC.velocity.X = (NPC.velocity.X * inertia + num1243) / (inertia + 1f);
                NPC.velocity.Y = (NPC.velocity.Y * inertia + num1244) / (inertia + 1f);

                if (NPC.ai[1] == 0f)
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        NPC.localAI[2] += 1f;
                        if (NPC.localAI[2] >= 180f)
                        {
                            NPC.TargetClosest();
                            NPC.localAI[2] = 0f;
                            int num1249 = 0;
                            int num1250;
                            int num1251;
                            while (true)
                            {
                                num1249++;
                                num1250 = (int)player.Center.X / 16;
                                num1251 = (int)player.Center.Y / 16;

                                int min = 16;
                                int max = 20;

                                if (Main.rand.NextBool(2))
                                    num1250 += Main.rand.Next(min, max);
                                else
                                    num1250 -= Main.rand.Next(min, max);

                                if (Main.rand.NextBool(2))
                                    num1251 += Main.rand.Next(min, max);
                                else
                                    num1251 -= Main.rand.Next(min, max);

                                if (!WorldGen.SolidTile(num1250, num1251) && Collision.CanHit(new Vector2(num1250 * 16, num1251 * 16), 1, 1, player.position, player.width, player.height))
                                    break;

                                if (num1249 > 100)
                                    goto Block;
                            }
                            NPC.ai[1] = 1f;
                            teleportLocationX = num1250;
                            calamityGlobalNPC.newAI[2] = num1251;
                            NPC.netUpdate = true;
                            Block:
                            ;
                        }
                    }
                }
                else if (NPC.ai[1] == 1f)
                {
                    // Avoid cheap bullshit
                    NPC.damage = 0;

                    Vector2 position = new Vector2(teleportLocationX * 16f - (NPC.width / 2), calamityGlobalNPC.newAI[2] * 16f - (NPC.height / 2));
                    for (int m = 0; m < 5; m++)
                    {
                        int dust = Dust.NewDust(position, NPC.width, NPC.height, dustType, 0f, 0f, 100, default, 2f);
                        Main.dust[dust].noGravity = true;
                    }

                    NPC.Opacity -= 0.008f;
                    if (NPC.Opacity <= 0f)
                    {
                        NPC.Opacity = 0f;
                        NPC.position = position;

                        for (int n = 0; n < 15; n++)
                        {
                            int num39 = Dust.NewDust(NPC.position, NPC.width, NPC.height, dustType, 0f, 0f, 100, default, 3f);
                            Main.dust[num39].noGravity = true;
                        }

                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            if (Collision.CanHit(NPC.position, NPC.width, NPC.height, player.position, player.width, player.height))
                            {
                                NPC.localAI[0] = 0f;
                                SoundEngine.PlaySound(CalamityWorld.getFixedBoi ? SoundID.NPCHit41 : HitSound, NPC.Center);
                                int type = iceRain;
                                int damage = NPC.GetProjectileDamage(type);
                                float velocity = 9f + enrageScale;
                                for (int i = 0; i < 3; i++)
                                {
                                    int totalProjectiles = bossRush ? 9 : 6;
                                    float radians = MathHelper.TwoPi / totalProjectiles;
                                    float newVelocity = velocity - (velocity * 0.33f * i);
                                    float velocityX = 0f;
                                    if (i > 0)
                                    {
                                        double angleA = radians * 0.33 * (3 - i);
                                        double angleB = MathHelper.ToRadians(90f) - angleA;
                                        velocityX = (float)(newVelocity * Math.Sin(angleA) / Math.Sin(angleB));
                                    }
                                    Vector2 spinningPoint = i == 0 ? new Vector2(0f, -newVelocity) : new Vector2(-velocityX, -newVelocity);
                                    for (int k = 0; k < totalProjectiles; k++)
                                    {
                                        Vector2 vector255 = spinningPoint.RotatedBy(radians * k);
                                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + Vector2.Normalize(vector255) * 30f, vector255, type, damage, 0f, Main.myPlayer, 0f, velocity);
                                    }
                                }
                            }
                        }

                        NPC.ai[1] = 2f;
                        NPC.netUpdate = true;
                    }
                }
                else if (NPC.ai[1] == 2f)
                {
                    // Avoid cheap bullshit
                    NPC.damage = 0;

                    NPC.Opacity += 0.2f;
                    if (NPC.Opacity >= 1f)
                    {
                        NPC.Opacity = 1f;
                        NPC.ai[1] = 0f;
                        NPC.netUpdate = true;
                    }
                }

                if (phase5)
                {
                    NPC.TargetClosest();
                    NPC.ai[0] = 4f;
                    NPC.ai[1] = 0f;
                    NPC.ai[3] = 0f;
                    NPC.localAI[0] = 0f;
                    NPC.localAI[2] = 0f;
                    NPC.Opacity = 1f;
                    teleportLocationX = 0;
                    calamityGlobalNPC.newAI[2] = 0f;
                    NPC.netUpdate = true;

                    int chance = 100;
                    if (DateTime.Now.Month == 4 && DateTime.Now.Day == 1)
                        chance = 20;
                    if (CalamityWorld.getFixedBoi)
                        chance = 1;

                    if (Main.rand.NextBool(chance))
                    {
                        string key = CalamityWorld.getFixedBoi ? "Mods.CalamityMod.PyrogenBossText" : "Mods.CalamityMod.CryogenBossText";
                        Color messageColor = CalamityWorld.getFixedBoi ? Color.Orange : Color.Cyan;
                        CalamityUtils.DisplayLocalizedText(key, messageColor);
                    }
                }
            }
            else if (NPC.ai[0] == 4f)
            {
                if (phase6)
                {
                    if (NPC.ai[1] == 60f)
                    {
                        NPC.velocity = Vector2.Normalize(player.Center - NPC.Center) * (18f + enrageScale * 2f);

                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            if (Collision.CanHit(NPC.position, NPC.width, NPC.height, player.position, player.width, player.height))
                            {
                                SoundEngine.PlaySound(CalamityWorld.getFixedBoi ? SoundID.NPCHit41 : HitSound, NPC.Center);
                                int type = iceBlast;
                                int damage = NPC.GetProjectileDamage(type);
                                float velocity = 1.5f + enrageScale * 0.5f;
                                int totalSpreads = phase7 ? 3 : 2;
                                for (int i = 0; i < totalSpreads; i++)
                                {
                                    int totalProjectiles = bossRush ? 3 : 2;
                                    float radians = MathHelper.TwoPi / totalProjectiles;
                                    float newVelocity = velocity - (velocity * (phase7 ? 0.25f : 0.5f) * i);
                                    float velocityX = 0f;
                                    float ai = CalamityWorld.getFixedBoi ? 2f : NPC.target;
                                    if (i > 0)
                                    {
                                        double angleA = radians * (phase7 ? 0.25 : 0.5) * (totalSpreads - i);
                                        double angleB = MathHelper.ToRadians(90f) - angleA;
                                        velocityX = (float)(newVelocity * Math.Sin(angleA) / Math.Sin(angleB));
                                    }
                                    Vector2 spinningPoint = i == 0 ? new Vector2(0f, -newVelocity) : new Vector2(-velocityX, -newVelocity);
                                    for (int k = 0; k < totalProjectiles; k++)
                                    {
                                        Vector2 vector255 = spinningPoint.RotatedBy(radians * k);
                                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + Vector2.Normalize(vector255) * 30f, vector255, type, damage, 0f, Main.myPlayer, ai, 1f);
                                    }
                                }
                            }
                        }
                    }

                    NPC.ai[1] -= 1f;
                    if (NPC.ai[1] <= 0f)
                    {
                        NPC.ai[3] += 1f;
                        NPC.TargetClosest();
                        if (NPC.ai[3] > 2f)
                        {
                            NPC.ai[0] = 5f;
                            NPC.ai[1] = 0f;
                            NPC.ai[3] = 0f;
                            calamityGlobalNPC.newAI[3] = 0f;
                        }
                        else
                            NPC.ai[1] = 60f;

                        NPC.rotation = NPC.velocity.X * 0.1f;
                    }
                    else if (NPC.ai[1] <= 15f)
                    {
                        NPC.velocity *= 0.95f;
                        NPC.rotation = NPC.velocity.X * 0.15f;
                    }
                    else
                        NPC.rotation += NPC.direction * 0.5f;

                    return;
                }

                float num1372 = 18f + enrageScale * 2f;

                Vector2 vector167 = new Vector2(NPC.Center.X + (NPC.direction * 20), NPC.Center.Y + 6f);
                float num1373 = player.position.X + player.width * 0.5f - vector167.X;
                float num1374 = player.Center.Y - vector167.Y;
                float num1375 = (float)Math.Sqrt(num1373 * num1373 + num1374 * num1374);
                float num1376 = num1372 / num1375;
                num1373 *= num1376;
                num1374 *= num1376;
                calamityGlobalNPC.newAI[2] -= 1f;

                float chargeStartDistance = 300f;
                float chargeCooldown = 30f;

                if (num1375 < chargeStartDistance || calamityGlobalNPC.newAI[2] > 0f)
                {
                    if (num1375 < chargeStartDistance)
                        calamityGlobalNPC.newAI[2] = chargeCooldown;

                    if (NPC.velocity.Length() < num1372)
                    {
                        NPC.velocity.Normalize();
                        NPC.velocity *= num1372;
                    }

                    NPC.rotation += NPC.direction * 0.5f;

                    return;
                }

                float inertia = 30f;
                if (Main.getGoodWorld)
                    inertia *= 0.5f;

                NPC.velocity.X = (NPC.velocity.X * inertia + num1373) / (inertia + 1f);
                NPC.velocity.Y = (NPC.velocity.Y * inertia + num1374) / (inertia + 1f);
                if (num1375 < chargeStartDistance + 200f)
                {
                    NPC.velocity.X = (NPC.velocity.X * 9f + num1373) / 10f;
                    NPC.velocity.Y = (NPC.velocity.Y * 9f + num1374) / 10f;
                }
                if (num1375 < chargeStartDistance + 100f)
                {
                    NPC.velocity.X = (NPC.velocity.X * 4f + num1373) / 5f;
                    NPC.velocity.Y = (NPC.velocity.Y * 4f + num1374) / 5f;
                }

                NPC.rotation = NPC.velocity.X * 0.15f;
            }
            else
            {
                NPC.rotation = NPC.velocity.X * 0.1f;

                calamityGlobalNPC.newAI[3] += 1f;
                if (calamityGlobalNPC.newAI[3] >= (bossRush ? 50f : 75f))
                {
                    calamityGlobalNPC.newAI[3] = 0f;
                    SoundEngine.PlaySound(CalamityWorld.getFixedBoi ? SoundID.NPCHit41 : HitSound, NPC.Center);
                    int totalProjectiles = 2;
                    float radians = MathHelper.TwoPi / totalProjectiles;
                    int type = iceBomb;
                    int damage = NPC.GetProjectileDamage(type);
                    float velocity2 = 6f;
                    double angleA = radians * 0.5;
                    double angleB = MathHelper.ToRadians(90f) - angleA;
                    float velocityX = (float)(velocity2 * Math.Sin(angleA) / Math.Sin(angleB));
                    Vector2 spinningPoint = Main.rand.NextBool() ? new Vector2(0f, -velocity2) : new Vector2(velocityX, -velocity2);
                    for (int k = 0; k < totalProjectiles; k++)
                    {
                        Vector2 vector255 = spinningPoint.RotatedBy(radians * k);
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + Vector2.Normalize(vector255) * 30f, vector255, type, damage, 0f, Main.myPlayer);
                    }
                }

                NPC.ai[1] += 1f;
                if (NPC.ai[1] >= (bossRush ? 120f : 180f))
                {
                    NPC.TargetClosest();
                    NPC.ai[0] = 4f;
                    NPC.ai[1] = 60f;
                    calamityGlobalNPC.newAI[3] = 0f;
                    calamityGlobalNPC.newAI[2] = 0f;
                    NPC.netUpdate = true;
                }

                float velocity = revenge ? 5f : 6f;
                float acceleration = 0.2f;
                velocity -= enrageScale;
                acceleration += 0.07f * enrageScale;

                if (NPC.position.Y > player.position.Y - 375f)
                {
                    if (NPC.velocity.Y > 0f)
                        NPC.velocity.Y *= 0.98f;

                    NPC.velocity.Y -= acceleration;

                    if (NPC.velocity.Y > velocity)
                        NPC.velocity.Y = velocity;
                }
                else if (NPC.position.Y < player.position.Y - 400f)
                {
                    if (NPC.velocity.Y < 0f)
                        NPC.velocity.Y *= 0.98f;

                    NPC.velocity.Y += acceleration;

                    if (NPC.velocity.Y < -velocity)
                        NPC.velocity.Y = -velocity;
                }

                if (NPC.position.X + (NPC.width / 2) > player.position.X + (player.width / 2) + 350f)
                {
                    if (NPC.velocity.X > 0f)
                        NPC.velocity.X *= 0.98f;

                    NPC.velocity.X -= acceleration;

                    if (NPC.velocity.X > velocity)
                        NPC.velocity.X = velocity;
                }
                if (NPC.position.X + (NPC.width / 2) < player.position.X + (player.width / 2) - 350f)
                {
                    if (NPC.velocity.X < 0f)
                        NPC.velocity.X *= 0.98f;

                    NPC.velocity.X += acceleration;

                    if (NPC.velocity.X < -velocity)
                        NPC.velocity.X = -velocity;
                }
            }
        }

        private void HandlePhaseTransition(int newPhase)
        {
            SoundStyle sound = CalamityWorld.getFixedBoi ? SoundID.NPCDeath14 : TransitionSound;
            SoundEngine.PlaySound(sound, NPC.Center);
            if (Main.netMode != NetmodeID.Server && !CalamityWorld.getFixedBoi)
            {
                int chipGoreAmount = newPhase >= 5 ? 3 : newPhase >= 3 ? 2 : 1;
                for (int i = 1; i < chipGoreAmount; i++)
                    Gore.NewGore(NPC.GetSource_FromAI(), NPC.position, NPC.velocity, Mod.Find<ModGore>("CryoChipGore" + i).Type, NPC.scale);
            }

            currentPhase = newPhase;

            switch (currentPhase)
            {
                case 0:
                case 1:
                    break;
                case 2:
                    NPC.defense = 13;
                    NPC.Calamity().DR = 0.27f;
                    break;
                case 3:
                    NPC.defense = 10;
                    NPC.Calamity().DR = 0.21f;
                    break;
                case 4:
                    NPC.defense = 6;
                    NPC.Calamity().DR = 0.12f;
                    break;
                case 5:
                case 6:
                    NPC.defense = 0;
                    NPC.Calamity().DR = 0f;
                    break;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (CalamityWorld.getFixedBoi)
            {
                float compactness = NPC.width * 0.6f;
                if (compactness < 10f)
                    compactness = 10f;

                float power = NPC.height / 100f;
                if (power > 2.75f)
                    power = 2.75f;

                if (FireDrawer is null)
                {
                    FireDrawer = new FireParticleSet(int.MaxValue, 1, Color.Red * 1.25f, Color.Red, compactness, power);
                }
                else
                {
                    FireDrawer.DrawSet(NPC.Bottom - Vector2.UnitY * (12f - NPC.gfxOffY));
                }
            }
            else
                FireDrawer = null;

            string phase = "CalamityMod/NPCs/Cryogen/Cryogen_Phase" + currentPhase;
            Texture2D texture = ModContent.Request<Texture2D>(phase).Value;

            SpriteEffects spriteEffects = SpriteEffects.None;
            if (NPC.spriteDirection == 1)
                spriteEffects = SpriteEffects.FlipHorizontally;

            NPC.DrawBackglow(BackglowColor, 4f, spriteEffects, NPC.frame, screenPos);

            Vector2 origin = new Vector2(TextureAssets.Npc[NPC.type].Value.Width / 2, TextureAssets.Npc[NPC.type].Value.Height / Main.npcFrameCount[NPC.type] / 2);
            Vector2 drawPos = NPC.Center - screenPos;
            drawPos -= new Vector2(texture.Width, texture.Height / Main.npcFrameCount[NPC.type]) * NPC.scale / 2f;
            drawPos += origin * NPC.scale + new Vector2(0f, NPC.gfxOffY);
            Color overlay = CalamityWorld.getFixedBoi ? Color.Red : drawColor;
            spriteBatch.Draw(texture, drawPos, NPC.frame, NPC.GetAlpha(overlay), NPC.rotation, origin, NPC.scale, spriteEffects, 0f);

            return false;
        }

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            NPC.lifeMax = (int)(NPC.lifeMax * 0.8f * bossLifeScale);
            NPC.damage = (int)(NPC.damage * NPC.GetExpertDamageMultiplier());
        }

        public override void ModifyTypeName(ref string typeName)
        {
            if (CalamityWorld.getFixedBoi)
            {
                typeName = "Pyrogen";
            }
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            int dusttype = CalamityWorld.getFixedBoi ? 235 : 67;
            for (int k = 0; k < 3; k++)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, dusttype, hitDirection, -1f, 0, default, 1f);
            }
            if (NPC.life <= 0)
            {
                for (int num621 = 0; num621 < 40; num621++)
                {
                    int num622 = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, dusttype, 0f, 0f, 100, default, 2f);
                    Main.dust[num622].velocity *= 3f;
                    if (Main.rand.NextBool(2))
                    {
                        Main.dust[num622].scale = 0.5f;
                        Main.dust[num622].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                    }
                }
                for (int num623 = 0; num623 < 70; num623++)
                {
                    int num624 = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, dusttype, 0f, 0f, 100, default, 3f);
                    Main.dust[num624].noGravity = true;
                    Main.dust[num624].velocity *= 5f;
                    num624 = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, dusttype, 0f, 0f, 100, default, 2f);
                    Main.dust[num624].velocity *= 2f;
                }
                if (Main.netMode != NetmodeID.Server && !CalamityWorld.getFixedBoi)
                {
                    float randomSpread = Main.rand.Next(-200, 201) / 100f;
                    for (int i = 1; i < 4; i++)
                    {
                        Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity * randomSpread, Mod.Find<ModGore>("CryoDeathGore" + i).Type, NPC.scale);
                        Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity * randomSpread, Mod.Find<ModGore>("CryoChipGore" + i).Type, NPC.scale);
                    }
                }
            }
        }

        public override void BossLoot(ref string name, ref int potionType)
        {
            potionType = ItemID.GreaterHealingPotion;
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            // Boss bag
            npcLoot.Add(ItemDropRule.BossBag(ModContent.ItemType<CryogenBag>()));

            // Normal drops: Everything that would otherwise be in the bag
            var normalOnly = npcLoot.DefineNormalOnlyDropSet();
            {
                // Weapons
                int[] weapons = new int[]
                {
                    ModContent.ItemType<Avalanche>(),
                    ModContent.ItemType<HoarfrostBow>(),
                    ModContent.ItemType<SnowstormStaff>(),
                    ModContent.ItemType<Icebreaker>()
                };
                normalOnly.Add(DropHelper.CalamityStyle(DropHelper.NormalWeaponDropRateFraction, weapons));
                normalOnly.Add(ModContent.ItemType<ColdDivinity>(), 10);

                // Vanity
                normalOnly.Add(ModContent.ItemType<CryogenMask>(), 7);
                normalOnly.Add(ModContent.ItemType<ThankYouPainting>(), ThankYouPainting.DropInt);

                // Materials
                normalOnly.Add(ModContent.ItemType<EssenceofEleum>(), 1, 4, 8);

                // Equipment
                normalOnly.Add(DropHelper.PerPlayer(ModContent.ItemType<SoulofCryogen>()));
                normalOnly.Add(ModContent.ItemType<CryoStone>(), DropHelper.NormalWeaponDropRateFraction);
                normalOnly.Add(ModContent.ItemType<FrostFlare>(), DropHelper.NormalWeaponDropRateFraction);
            }

            npcLoot.Add(ItemID.FrozenKey, 3);

            // Trophy (always directly from boss, never in bag)
            npcLoot.Add(ModContent.ItemType<CryogenTrophy>(), 10);

            // Relic
            npcLoot.DefineConditionalDropSet(DropHelper.RevAndMaster).Add(ModContent.ItemType<CryogenRelic>());

            // Lore
            npcLoot.AddConditionalPerPlayer(() => !DownedBossSystem.downedCryogen, ModContent.ItemType<LoreArchmage>(), desc: DropHelper.FirstKillText);
        }

        public override void OnKill()
        {
            CalamityGlobalNPC.SetNewBossJustDowned(NPC);

            // Spawn Permafrost if he isn't in the world
            int permafrostNPC = NPC.FindFirstNPC(ModContent.NPCType<DILF>());
            if (permafrostNPC == -1 && !BossRushEvent.BossRushActive)
                NPC.NewNPC(NPC.GetSource_Death(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<DILF>(), 0, 0f, 0f, 0f, 0f, 255);

            // If Cryogen has not been killed, notify players about Cryonic Ore
            if (!DownedBossSystem.downedCryogen)
            {
                string key = "Mods.CalamityMod.IceOreText";
                Color messageColor = Color.LightSkyBlue;
                CalamityUtils.SpawnOre(ModContent.TileType<CryonicOre>(), 15E-05, 0.45f, 0.7f, 3, 8, TileID.SnowBlock, TileID.IceBlock, TileID.CorruptIce, TileID.FleshIce, TileID.HallowedIce, ModContent.TileType<AstralSnow>(), ModContent.TileType<AstralIce>());

                CalamityUtils.DisplayLocalizedText(key, messageColor);
            }

            // Mark Cryogen as dead
            DownedBossSystem.downedCryogen = true;
            CalamityNetcode.SyncWorld();
        }

        // Can only hit the target if within certain distance
        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
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

            return minDist <= 40f * NPC.scale;
        }

        public override void OnHitPlayer(Player player, int damage, bool crit)
        {
            if (damage > 0)
            {
                if (CalamityWorld.getFixedBoi)
                {
                    player.AddBuff(ModContent.BuffType<BrimstoneFlames>(), 240, true);
                    player.AddBuff(ModContent.BuffType<VulnerabilityHex>(), 120, true);
                }
                else
                {
                    player.AddBuff(BuffID.Frostburn, 240, true);
                    player.AddBuff(BuffID.Chilled, 120, true);
                }
            }
        }
    }
}
