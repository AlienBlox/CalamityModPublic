﻿using CalamityMod.BiomeManagers;
using CalamityMod.Items.Placeables.Banners;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;
using CalamityMod.Items.Weapons.Magic;
using Terraria.GameContent.ItemDropRules;

namespace CalamityMod.NPCs.Abyss
{
    public class BoxJellyfish : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Box Jellyfish");
            Main.npcFrameCount[NPC.type] = 4;
        }

        public override void SetDefaults()
        {
            NPC.noGravity = true;
            NPC.damage = 44;
            NPC.width = 30;
            NPC.height = 33;
            NPC.defense = 5;
            NPC.lifeMax = 90;
            NPC.alpha = 20;
            NPC.aiStyle = -1;
            AIType = -1;
            NPC.value = Item.buyPrice(0, 0, 0, 80);
            NPC.HitSound = SoundID.NPCHit25;
            NPC.DeathSound = SoundID.NPCDeath28;
            Banner = NPC.type;
            BannerItem = ModContent.ItemType<BoxJellyfishBanner>();
            NPC.Calamity().VulnerableToHeat = false;
            NPC.Calamity().VulnerableToSickness = false;
            NPC.Calamity().VulnerableToElectricity = true;
            NPC.Calamity().VulnerableToWater = false;
            SpawnModBiomes = new int[1] { ModContent.GetInstance<AbyssLayer1Biome>().Type };
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Ocean,

				// Will move to localization whenever that is cleaned up.
				new FlavorTextBestiaryInfoElement("These seemingly inept masses of gelatin are anything but harmless. Let even one of their trailing tentacles brush you, and you will be heavily envenomed.")
            });
        }

        public override void AI()
        {
            if (NPC.direction == 0)
            {
                NPC.TargetClosest(true);
            }
            if (!NPC.wet)
            {
                NPC.rotation += NPC.velocity.X * 0.1f;
                if (NPC.velocity.Y == 0f)
                {
                    NPC.velocity.X = NPC.velocity.X * 0.98f;
                    if (NPC.velocity.X > -0.01 && NPC.velocity.X < 0.01)
                    {
                        NPC.velocity.X = 0f;
                    }
                }
                NPC.velocity.Y = NPC.velocity.Y + 0.2f;
                if (NPC.velocity.Y > 10f)
                {
                    NPC.velocity.Y = 10f;
                }
                NPC.ai[0] = 1f;
                return;
            }
            if (NPC.collideX)
            {
                NPC.velocity.X = NPC.velocity.X * -1f;
                NPC.direction *= -1;
            }
            if (NPC.collideY)
            {
                if (NPC.velocity.Y > 0f)
                {
                    NPC.velocity.Y = Math.Abs(NPC.velocity.Y) * -1f;
                    NPC.directionY = -1;
                    NPC.ai[0] = -1f;
                }
                else if (NPC.velocity.Y < 0f)
                {
                    NPC.velocity.Y = Math.Abs(NPC.velocity.Y);
                    NPC.directionY = 1;
                    NPC.ai[0] = 1f;
                }
            }
            bool flag16 = false;
            NPC.TargetClosest(false);
            if (Main.player[NPC.target].wet && !Main.player[NPC.target].dead && Collision.CanHit(NPC.position, NPC.width, NPC.height, Main.player[NPC.target].position, Main.player[NPC.target].width, Main.player[NPC.target].height))
            {
                flag16 = true;
            }
            if (flag16)
            {
                NPC.localAI[2] = 1f;
                NPC.rotation = (float)Math.Atan2(NPC.velocity.Y, NPC.velocity.X) + 1.57f;
                NPC.velocity *= 0.975f;
                float num263 = 0.8f;
                if (NPC.velocity.X > -num263 && NPC.velocity.X < num263 && NPC.velocity.Y > -num263 && NPC.velocity.Y < num263)
                {
                    NPC.TargetClosest(true);
                    float num264 = CalamityWorld.death ? 12f : 8f;
                    Vector2 vector31 = new Vector2(NPC.position.X + NPC.width * 0.5f, NPC.position.Y + NPC.height * 0.5f);
                    float num265 = Main.player[NPC.target].position.X + (Main.player[NPC.target].width / 2) - vector31.X;
                    float num266 = Main.player[NPC.target].position.Y + (Main.player[NPC.target].height / 2) - vector31.Y;
                    float num267 = (float)Math.Sqrt(num265 * num265 + num266 * num266);
                    num267 = num264 / num267;
                    num265 *= num267;
                    num266 *= num267;
                    NPC.velocity.X = num265;
                    NPC.velocity.Y = num266;
                }
            }
            else
            {
                NPC.localAI[2] = 0f;
                NPC.velocity.X = NPC.velocity.X + NPC.direction * 0.02f;
                NPC.rotation = NPC.velocity.X * 0.4f;
                if (NPC.velocity.X < -1f || NPC.velocity.X > 1f)
                {
                    NPC.velocity.X = NPC.velocity.X * 0.95f;
                }
                if (NPC.ai[0] == -1f)
                {
                    NPC.velocity.Y = NPC.velocity.Y - 0.01f;
                    if (NPC.velocity.Y < -1f)
                    {
                        NPC.ai[0] = 1f;
                    }
                }
                else
                {
                    NPC.velocity.Y = NPC.velocity.Y + 0.01f;
                    if (NPC.velocity.Y > 1f)
                    {
                        NPC.ai[0] = -1f;
                    }
                }
                int num268 = (int)(NPC.position.X + (NPC.width / 2)) / 16;
                int num269 = (int)(NPC.position.Y + (NPC.height / 2)) / 16;
                if (Main.tile[num268, num269 - 1].LiquidAmount > 128)
                {
                    if (Main.tile[num268, num269 + 1].HasTile)
                    {
                        NPC.ai[0] = -1f;
                    }
                    else if (Main.tile[num268, num269 + 2].HasTile)
                    {
                        NPC.ai[0] = -1f;
                    }
                }
                else
                {
                    NPC.ai[0] = 1f;
                }
                if (NPC.velocity.Y > 1.2 || NPC.velocity.Y < -1.2)
                {
                    NPC.velocity.Y = NPC.velocity.Y * 0.99f;
                }
            }
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter += 0.15f;
            NPC.frameCounter %= Main.npcFrameCount[NPC.type];
            int frame = (int)NPC.frameCounter;
            NPC.frame.Y = frame * frameHeight;
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.PlayerSafe || spawnInfo.Player.Calamity().ZoneSulphur)
            {
                return 0f;
            }
            if (spawnInfo.Player.Calamity().ZoneAbyssLayer1 && spawnInfo.Water)
            {
                return SpawnCondition.CaveJellyfish.Chance * 1.2f;
            }
            return SpawnCondition.OceanMonster.Chance * 0.1f;
        }

        public override void OnHitPlayer(Player player, int damage, bool crit)
        {
            player.AddBuff(BuffID.Venom, 120, true);
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            for (int k = 0; k < 3; k++)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood, hitDirection, -1f, 0, default, 1f);
            }
            if (NPC.life <= 0)
            {
                for (int k = 0; k < 15; k++)
                {
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood, hitDirection, -1f, 0, default, 1f);
                }
            }
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemID.JellyfishNecklace, 100);
            var postSkeletron = npcLoot.DefineConditionalDropSet(() => NPC.downedBoss3);
            postSkeletron.Add(ItemDropRule.NormalvsExpert(ModContent.ItemType<AbyssShocker>(), 50, 40));
        }
    }
}
