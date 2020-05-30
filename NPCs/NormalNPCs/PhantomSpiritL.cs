using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Banners;
using CalamityMod.Projectiles.Boss;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.NPCs.NormalNPCs
{
    public class PhantomSpiritL : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Phantom Spirit");
            Main.npcFrameCount[npc.type] = 4;
        }

        public override void SetDefaults()
        {
            npc.aiStyle = -1;
            npc.damage = 100;
            npc.width = 32;
            npc.height = 32;
            npc.scale = 1.2f;
            npc.defense = 40;
            npc.lifeMax = 9000;
            npc.knockBackResist = 0f;
            aiType = -1;
            npc.value = Item.buyPrice(0, 0, 60, 0);
            npc.HitSound = SoundID.NPCHit36;
            npc.DeathSound = SoundID.NPCDeath39;
            npc.noGravity = true;
            npc.noTileCollide = true;
            banner = ModContent.NPCType<PhantomSpirit>();
            bannerItem = ModContent.ItemType<PhantomSpiritBanner>();
        }

        public override void FindFrame(int frameHeight)
        {
            npc.frameCounter += 0.15f;
            npc.frameCounter %= Main.npcFrameCount[npc.type];
            int frame = (int)npc.frameCounter;
            npc.frame.Y = frame * frameHeight;
        }

        public override void AI()
        {
            float speed = CalamityWorld.death ? 16f : 12f;
            CalamityAI.DungeonSpiritAI(npc, mod, speed, -MathHelper.PiOver2);
            int num822 = Dust.NewDust(npc.position, npc.width, npc.height, 60, 0f, 0f, 0, default, 1f);
            Dust dust = Main.dust[num822];
            dust.velocity *= 0.1f;
            dust.scale = 1.3f;
            dust.noGravity = true;
            Vector2 vector17 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
            float num147 = Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2) - vector17.X;
            float num148 = Main.player[npc.target].position.Y + (float)(Main.player[npc.target].height / 2) - vector17.Y;
            float num149 = (float)Math.Sqrt((double)(num147 * num147 + num148 * num148));
            if (num149 > 800f)
            {
                npc.ai[2] = 0f;
                npc.ai[3] = 0f;
                return;
            }
            npc.ai[2] += 1f;
            if (npc.ai[3] == 0f)
            {
                if (npc.ai[2] > 120f)
                {
                    npc.ai[2] = 0f;
                    npc.ai[3] = 1f;
                    npc.netUpdate = true;
                    return;
                }
            }
            else
            {
                if (npc.ai[2] > 40f)
                {
                    npc.ai[3] = 0f;
                }
                if (Main.netMode != NetmodeID.MultiplayerClient && npc.ai[2] == 20f)
                {
                    float num151 = 5f;
                    int num152 = Main.expertMode ? 53 : 65;
                    int num153 = ModContent.ProjectileType<PhantomGhostShot>();
                    num149 = num151 / num149;
                    num147 *= num149;
                    num148 *= num149;
                    int num154 = Projectile.NewProjectile(vector17.X, vector17.Y, num147, num148, num153, num152, 0f, Main.myPlayer, 0f, 0f);
                }
            }
        }

        public override void OnHitPlayer(Player player, int damage, bool crit)
        {
            if (CalamityWorld.revenge)
            {
                player.AddBuff(ModContent.BuffType<MarkedforDeath>(), 180);
            }
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(npc.position, npc.width, npc.height, 60, hitDirection, -1f, 0, default, 1f);
            }
            if (npc.life <= 0)
            {
                for (int num288 = 0; num288 < 50; num288++)
                {
                    int num289 = Dust.NewDust(npc.position, npc.width, npc.height, 60, npc.velocity.X, npc.velocity.Y, 0, default, 1f);
                    Dust dust = Main.dust[num289];
                    dust.velocity *= 2f;
                    dust.noGravity = true;
                    dust.scale = 1.4f;
                }
            }
        }

        public override Color? GetAlpha(Color drawColor)
        {
            return new Color(200, 200, 200, 0);
        }

        public override void NPCLoot()
        {
            Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ModContent.ItemType<Phantoplasm>(), Main.rand.Next(2, 5));
        }
    }
}
