using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Banners;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.NPCs.NormalNPCs
{
	public class Cryon : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cryon");
            Main.npcFrameCount[npc.type] = 6;
        }

        public override void SetDefaults()
        {
            npc.aiStyle = -1;
            aiType = -1;
            npc.damage = 42;
            npc.width = 50;
            npc.height = 64;
            npc.defense = 10;
			npc.DR_NERD(0.1f);
            npc.lifeMax = 300;
            npc.knockBackResist = 0f;
            npc.value = Item.buyPrice(0, 0, 5, 0);
            npc.HitSound = SoundID.NPCHit5;
            npc.DeathSound = SoundID.NPCDeath7;
            banner = npc.type;
            bannerItem = ModContent.ItemType<CryonBanner>();
			npc.coldDamage = true;
        }

        public override void AI()
        {
            CalamityAI.UnicornAI(npc, mod, false, CalamityWorld.death ? 6f : 4f, 5f, CalamityWorld.death ? 0.15f : 0.1f);
        }

        public override void FindFrame(int frameHeight)
        {
            if (npc.velocity.Y > 0f || npc.velocity.Y < 0f)
            {
                npc.spriteDirection = npc.direction;
                npc.frame.Y = frameHeight * 5;
                npc.frameCounter = 0.0;
            }
            else
            {
                npc.spriteDirection = npc.direction;
                npc.frameCounter += (double)(npc.velocity.Length() / 2f);
                if (npc.frameCounter > 12.0)
                {
                    npc.frame.Y = npc.frame.Y + frameHeight;
                    npc.frameCounter = 0.0;
                }
                if (npc.frame.Y >= frameHeight * 4)
                {
                    npc.frame.Y = 0;
                }
            }
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            return spawnInfo.player.ZoneSnow &&
                !spawnInfo.player.PillarZone() &&
                !spawnInfo.player.ZoneDungeon &&
                !spawnInfo.player.InSunkenSea() &&
                Main.hardMode && !spawnInfo.playerInTown && !spawnInfo.player.ZoneOldOneArmy && !Main.snowMoon && !Main.pumpkinMoon ? 0.015f : 0f;
        }

        public override void OnHitPlayer(Player player, int damage, bool crit)
        {
            player.AddBuff(BuffID.Frostburn, 300, true);
            if (Main.rand.NextBool(3))
            {
                player.AddBuff(ModContent.BuffType<GlacialState>(), 30, true);
            }
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            for (int k = 0; k < 3; k++)
            {
                Dust.NewDust(npc.position, npc.width, npc.height, 92, hitDirection, -1f, 0, default, 1f);
            }
            if (npc.life <= 0)
            {
                for (int k = 0; k < 15; k++)
                {
                    Dust.NewDust(npc.position, npc.width, npc.height, 92, hitDirection, -1f, 0, default, 1f);
                }
            }
        }

        public override void NPCLoot()
        {
            if (Main.rand.NextBool(2))
            {
                Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ModContent.ItemType<EssenceofEleum>());
            }
        }
    }
}
