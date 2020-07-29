using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Banners;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace CalamityMod.NPCs.NormalNPCs
{
    public class WulfrumPylon : ModNPC
    {
        public bool Charging
        {
            get => npc.ai[0] != 0f;
            set => npc.ai[0] = value.ToInt();
        }
        public float ChargeRadius
        {
            get => npc.ai[1];
            set => npc.ai[1] = value;
        }
        public static List<int> SuperchargableEnemies = new List<int>()
        {
            ModContent.NPCType<WulfrumDrone>(),
            ModContent.NPCType<WulfrumGyrator>(),
            ModContent.NPCType<WulfrumHovercraft>()
        };
        public const float ChargeRadiusMax = 200f;
        public const float SuperchargeTime = 600f;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Wulfrum Pylon");
            Main.npcFrameCount[npc.type] = 6;
        }

        public override void SetDefaults()
        {
            aiType = -1;
            npc.aiStyle = -1;
            npc.damage = 16;
            npc.width = 44;
            npc.height = 44;
            npc.defense = 4;
            npc.lifeMax = 49;
            npc.knockBackResist = 0f;
            npc.value = Item.buyPrice(0, 0, 1, 50);
            npc.noGravity = false;
            npc.noTileCollide = false;
            npc.HitSound = SoundID.NPCHit4;
            npc.DeathSound = SoundID.NPCDeath14;
            npc.buffImmune[BuffID.Confused] = false;
            banner = npc.type;
            bannerItem = ModContent.ItemType<WulfrumPylonBanner>();
        }

        public override void AI()
        {
            npc.TargetClosest(false);

            Player player = Main.player[npc.target];

            if (!Charging && npc.Distance(player.Center) < ChargeRadiusMax * 0.667f)
            {
                Charging = true;
                npc.netUpdate = true;
            }
            else if (Charging)
            {
                ChargeRadius = (int)MathHelper.Lerp(ChargeRadius, ChargeRadiusMax, 0.1f);

                if (Main.rand.NextBool(4))
                {
                    float dustCount = MathHelper.TwoPi * ChargeRadius / 5f;
                    for (int i = 0; i < dustCount; i++)
                    {
                        float angle = MathHelper.TwoPi * i / dustCount;
                        Dust dust = Dust.NewDustPerfect(npc.Center, 229);
                        dust.position = npc.Center + angle.ToRotationVector2() * ChargeRadius;
                        dust.noGravity = true;
                        dust.velocity = npc.velocity;
                    }
                }

                for (int i = 0; i < Main.npc.Length; i++)
                {
                    NPC npcAtIndex = Main.npc[i];
                    if (!npcAtIndex.active)
                        continue;

                    // For some strange reason, the Wulfrum Rover is not counted when it's added to a static list.
                    // What I assume is going on is that it hasn't been loaded yet since it's later alphabetically (Pylon is before Rover).
                    // As a result, they are checked separately.
                    if (!SuperchargableEnemies.Contains(npcAtIndex.type) &&
                        npcAtIndex.type != ModContent.NPCType<WulfrumRover>())
                        continue;
                    if (npcAtIndex.ai[3] > 0f)
                        continue;
                    if (npc.Distance(npcAtIndex.Center) > ChargeRadius)
                        continue;

                    npcAtIndex.ai[3] = SuperchargeTime; // Supercharge the npc for a while if isn't already supercharged.
                    npcAtIndex.netUpdate = true;

                    // And emit some dust.

                    // Dust doesn't need to be spawned for the server.
                    if (Main.dedServ)
                        continue;

                    for (int j = 0; j < 10; j++)
                    {
                        Dust.NewDust(npcAtIndex.position, npcAtIndex.width, npcAtIndex.height, 226);
                    }
                }
            }
        }

        public override void FindFrame(int frameHeight)
        {
            npc.frameCounter++;
            int frame = (int)(npc.frameCounter / 8) % Main.npcFrameCount[npc.type];

            npc.frame.Y = frame * frameHeight;
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.playerSafe || spawnInfo.player.Calamity().ZoneSulphur)
                return 0f;

            // Only spawn in the other thirds of the world bounds.
            if (spawnInfo.playerFloorX > Main.maxTilesX * 0.333f && spawnInfo.playerFloorX < Main.maxTilesX - Main.maxTilesX * 0.333f)
                return 0f;

            return SpawnCondition.OverworldDaySlime.Chance * (Main.hardMode ? 0.06f : 0.15f);
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            for (int k = 0; k < 3; k++)
            {
                Dust.NewDust(npc.position, npc.width, npc.height, DustID.GrassBlades, hitDirection, -1f, 0, default, 1f);
            }
            if (npc.life <= 0)
            {
                for (int k = 0; k < 15; k++)
                {
                    Dust.NewDust(npc.position, npc.width, npc.height, DustID.GrassBlades, hitDirection, -1f, 0, default, 1f);
                }
            }
        }

        public override void NPCLoot()
        {
            DropHelper.DropItem(npc, ModContent.ItemType<WulfrumShard>(), 2, 3);
            DropHelper.DropItem(npc, ModContent.ItemType<EnergyCore>());
        }
    }
}
