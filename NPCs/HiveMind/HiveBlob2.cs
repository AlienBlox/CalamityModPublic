using CalamityMod.Events;
using CalamityMod.Projectiles.Boss;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.HiveMind
{
    public class HiveBlob2 : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Hive Blob");
        }

        public override void SetDefaults()
        {
            NPC.npcSlots = 0.1f;
            NPC.aiStyle = -1;
            NPC.damage = 0;
            NPC.width = 25;
            NPC.height = 25;
            NPC.lifeMax = 150;
            if (BossRushEvent.BossRushActive)
            {
                NPC.lifeMax = 1300;
            }
            NPC.knockBackResist = 0f;
            aiType = -1;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.canGhostHeal = false;
            NPC.chaseable = false;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.Calamity().VulnerableToHeat = true;
            NPC.Calamity().VulnerableToCold = true;
            NPC.Calamity().VulnerableToSickness = true;
        }

        public override void AI()
        {
            bool expertMode = Main.expertMode || BossRushEvent.BossRushActive;
            bool revenge = CalamityWorld.revenge || BossRushEvent.BossRushActive;
            bool death = CalamityWorld.death || BossRushEvent.BossRushActive;

            int num750 = CalamityGlobalNPC.hiveMind;
            if (num750 < 0 || !Main.npc[num750].active)
            {
                NPC.active = false;
                NPC.netUpdate = true;
                return;
            }

            if (NPC.ai[3] > 0f)
                num750 = (int)NPC.ai[3] - 1;

            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                NPC.localAI[0] -= 1f;
                if (NPC.localAI[0] <= 0f)
                {
                    NPC.localAI[0] = Main.rand.Next(180, 361);
                    NPC.ai[0] = Main.rand.Next(-100, 101);
                    NPC.ai[1] = Main.rand.Next(-100, 101);
                    NPC.netUpdate = true;
                }
            }

            NPC.TargetClosest(true);

            float num751 = death ? 0.8f : revenge ? 0.7f : expertMode ? 0.6f : 0.5f;
            float num752 = 96f;
            Vector2 vector22 = new Vector2(NPC.ai[0] * 16f + 8f, NPC.ai[1] * 16f + 8f);
            float num189 = Main.player[NPC.target].position.X + (Main.player[NPC.target].width / 2) - (NPC.width / 2) - vector22.X;
            float num190 = Main.player[NPC.target].position.Y + (Main.player[NPC.target].height / 2) - (NPC.height / 2) - vector22.Y;
            float num191 = (float)Math.Sqrt(num189 * num189 + num190 * num190);
            float num754 = Main.npc[num750].position.X + (Main.npc[num750].width / 2);
            float num755 = Main.npc[num750].position.Y + (Main.npc[num750].height / 2);
            Vector2 vector93 = new Vector2(num754, num755);
            float num756 = num754 + NPC.ai[0];
            float num757 = num755 + NPC.ai[1];
            float num758 = num756 - vector93.X;
            float num759 = num757 - vector93.Y;
            float num760 = (float)Math.Sqrt(num758 * num758 + num759 * num759);
            num760 = num752 / num760;
            num758 *= num760;
            num759 *= num760;
            if (NPC.position.X < num754 + num758)
            {
                NPC.velocity.X = NPC.velocity.X + num751;
                if (NPC.velocity.X < 0f && num758 > 0f)
                    NPC.velocity.X = NPC.velocity.X * 0.8f;
            }
            else if (NPC.position.X > num754 + num758)
            {
                NPC.velocity.X = NPC.velocity.X - num751;
                if (NPC.velocity.X > 0f && num758 < 0f)
                    NPC.velocity.X = NPC.velocity.X * 0.8f;
            }
            if (NPC.position.Y < num755 + num759)
            {
                NPC.velocity.Y = NPC.velocity.Y + num751;
                if (NPC.velocity.Y < 0f && num759 > 0f)
                    NPC.velocity.Y = NPC.velocity.Y * 0.8f;
            }
            else if (NPC.position.Y > num755 + num759)
            {
                NPC.velocity.Y = NPC.velocity.Y - num751;
                if (NPC.velocity.Y > 0f && num759 < 0f)
                    NPC.velocity.Y = NPC.velocity.Y * 0.8f;
            }

            float velocityLimit = 8f;
            if (NPC.velocity.X > velocityLimit)
                NPC.velocity.X = velocityLimit;
            if (NPC.velocity.X < -velocityLimit)
                NPC.velocity.X = -velocityLimit;
            if (NPC.velocity.Y > velocityLimit)
                NPC.velocity.Y = velocityLimit;
            if (NPC.velocity.Y < -velocityLimit)
                NPC.velocity.Y = -velocityLimit;

            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                if (!Collision.CanHit(NPC.position, NPC.width, NPC.height, Main.player[NPC.target].position, Main.player[NPC.target].width, Main.player[NPC.target].height))
                    NPC.localAI[1] = 180f;

                NPC.localAI[1] += 1f;
                if (NPC.localAI[1] >= 360f && Vector2.Distance(Main.player[NPC.target].Center, NPC.Center) > 80f)
                {
                    NPC.localAI[1] = 0f;
                    NPC.TargetClosest(true);
                    if (Collision.CanHit(NPC.position, NPC.width, NPC.height, Main.player[NPC.target].position, Main.player[NPC.target].width, Main.player[NPC.target].height))
                    {
                        float num941 = death ? 5f : revenge ? 4.5f : expertMode ? 4f : 3.5f;
                        Vector2 vector104 = new Vector2(NPC.position.X + NPC.width * 0.5f, NPC.position.Y + (NPC.height / 2));
                        float num942 = Main.player[NPC.target].position.X + Main.player[NPC.target].width * 0.5f - vector104.X;
                        float num943 = Main.player[NPC.target].position.Y + Main.player[NPC.target].height * 0.5f - vector104.Y;
                        float num944 = (float)Math.Sqrt(num942 * num942 + num943 * num943);
                        num944 = num941 / num944;
                        num942 *= num944;
                        num943 *= num944;
                        int type = ModContent.ProjectileType<VileClot>();
                        int damage = NPC.GetProjectileDamage(type);
                        Projectile.NewProjectile(vector104.X, vector104.Y, num942, num943, type, damage, 0f, Main.myPlayer, 0f, 0f);
                        NPC.netUpdate = true;
                    }
                }
            }
        }

        public override void NPCLoot()
        {
            if (!CalamityWorld.revenge)
            {
                int closestPlayer = Player.FindClosest(NPC.Center, 1, 1);
                if (Main.rand.Next(8) == 0 && Main.player[closestPlayer].statLife < Main.player[closestPlayer].statLifeMax2)
                    Item.NewItem((int)NPC.position.X, (int)NPC.position.Y, NPC.width, NPC.height, ItemID.Heart);
            }
        }

        public override bool CheckActive()
        {
            return false;
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, 14, hitDirection, -1f, 0, default, 1f);
            }
            if (NPC.life <= 0)
            {
                for (int k = 0; k < 10; k++)
                {
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, 14, hitDirection, -1f, 0, default, 1f);
                }
            }
        }
    }
}
