﻿using System;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
namespace CalamityMod.NPCs
{
    public class HeatSpirit : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Heat Spirit");
            Main.npcFrameCount[npc.type] = 4;
        }

        public override void SetDefaults()
        {
            npc.aiStyle = 86;
            npc.damage = 33;
            npc.width = 40;
            npc.height = 40;
            npc.defense = 8;
            npc.lifeMax = 50;
            npc.knockBackResist = 0f;
            npc.value = Item.buyPrice(0, 0, 1, 0);
            npc.HitSound = SoundID.NPCHit52;
            npc.DeathSound = SoundID.NPCDeath55;
            banner = npc.type;
            bannerItem = ModContent.ItemType<HeatSpiritBanner>();
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.playerSafe || !Main.hardMode)
            {
                return 0f;
            }
            return SpawnCondition.Underworld.Chance * 0.065f;
        }

        public override void FindFrame(int frameHeight)
        {
            if (npc.velocity.X < 0f)
            {
                npc.direction = -1;
            }
            else
            {
                npc.direction = 1;
            }
            if (npc.direction == 1)
            {
                npc.spriteDirection = 1;
            }
            if (npc.direction == -1)
            {
                npc.spriteDirection = -1;
            }
            npc.rotation = (float)Math.Atan2((double)(npc.velocity.Y * (float)npc.direction), (double)(npc.velocity.X * (float)npc.direction));
            npc.frameCounter += 0.15f;
            npc.frameCounter %= Main.npcFrameCount[npc.type];
            int frame = (int)npc.frameCounter;
            npc.frame.Y = frame * frameHeight;
        }

        public override void AI()
        {
            Lighting.AddLight((int)((npc.position.X + (float)(npc.width / 2)) / 16f), (int)((npc.position.Y + (float)(npc.height / 2)) / 16f), 0.5f, 0f, 0.05f);
        }

        public override void OnHitPlayer(Player player, int damage, bool crit)
        {
            player.AddBuff(BuffID.OnFire, 120, true);
            player.AddBuff(ModContent.BuffType<BrimstoneFlames>(), 120, true);
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            for (int k = 0; k < 3; k++)
            {
                Dust.NewDust(npc.position, npc.width, npc.height, 235, hitDirection, -1f, 0, default, 1f);
            }
            if (npc.life <= 0)
            {
                for (int k = 0; k < 20; k++)
                {
                    Dust.NewDust(npc.position, npc.width, npc.height, 235, hitDirection, -1f, 0, default, 1f);
                }
            }
        }

        public override void NPCLoot()
        {
            if (Main.rand.NextBool(4))
            {
                Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ModContent.ItemType<EssenceofChaos>());
            }
        }
    }
}
