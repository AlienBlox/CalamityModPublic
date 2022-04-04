﻿using CalamityMod.Items.Placeables.Banners;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.NormalNPCs
{
    public class MantisShrimp : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Mantis Shrimp");
            Main.npcFrameCount[NPC.type] = 6;
        }

        public override void SetDefaults()
        {
            NPC.damage = 200;
            NPC.width = 40;
            NPC.height = 24;
            NPC.defense = 10;
            NPC.DR_NERD(0.1f);
            NPC.lifeMax = 30;
            NPC.aiStyle = 3;
            AIType = NPCID.Crab;
            NPC.value = Item.buyPrice(0, 0, 1, 0);
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            Banner = NPC.type;
            BannerItem = ModContent.ItemType<MantisShrimpBanner>();
            NPC.Calamity().VulnerableToHeat = false;
            NPC.Calamity().VulnerableToSickness = true;
            NPC.Calamity().VulnerableToElectricity = true;
            NPC.Calamity().VulnerableToWater = false;
        }

        public override void AI()
        {
            NPC.spriteDirection = (NPC.direction > 0) ? -1 : 1;
            float num79 = (Main.player[NPC.target].Center - NPC.Center).Length();
            num79 *= 0.0025f;
            if ((double)num79 > 1.5)
            {
                num79 = 1.5f;
            }
            float num78;
            if (Main.expertMode)
            {
                num78 = 3f - num79;
            }
            else
            {
                num78 = 2.5f - num79;
            }
            num78 *= (CalamityWorld.death ? 1.2f : 0.8f);
            if (NPC.velocity.X < -num78 || NPC.velocity.X > num78)
            {
                if (NPC.velocity.Y == 0f)
                {
                    NPC.velocity *= 0.8f;
                }
            }
            else if (NPC.velocity.X < num78 && NPC.direction == 1)
            {
                NPC.velocity.X = NPC.velocity.X + 1f;
                if (NPC.velocity.X > num78)
                {
                    NPC.velocity.X = num78;
                }
            }
            else if (NPC.velocity.X > -num78 && NPC.direction == -1)
            {
                NPC.velocity.X = NPC.velocity.X - 1f;
                if (NPC.velocity.X < -num78)
                {
                    NPC.velocity.X = -num78;
                }
            }
        }

        public override void OnHitPlayer(Player player, int damage, bool crit)
        {
            if (Main.netMode != NetmodeID.MultiplayerClient)
                Projectile.NewProjectile(NPC.GetSpawnSource_ForProjectile(), player.Center, Vector2.Zero, ProjectileID.SolarWhipSwordExplosion, 0, 0f, Main.myPlayer);
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
            if (spawnInfo.playerSafe || !Main.hardMode || spawnInfo.Player.Calamity().ZoneSulphur)
                return 0f;

            return SpawnCondition.OceanMonster.Chance * 0.2f;
        }

        public override void NPCLoot()
        {
            DropHelper.DropItemCondition(NPC, ModContent.ItemType<MantisClaws>(), NPC.downedPlantBoss, 0.2f);
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
    }
}
