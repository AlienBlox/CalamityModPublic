﻿using CalamityMod.Dusts;
using CalamityMod.Events;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Banners;
using CalamityMod.Projectiles.Boss;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.NormalNPCs
{
    public class PhantomSpiritL : ModNPC
    {
        public override LocalizedText DisplayName => CalamityUtils.GetText("NPCs.PhantomSpirit.DisplayName");
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 5;
        }

        public override void SetDefaults()
        {
            NPC.aiStyle = -1;
            NPC.damage = 100;
            NPC.width = 32;
            NPC.height = 80;
            NPC.scale *= 1.2f;
            NPC.defense = 30;
            NPC.lifeMax = 3000;
            NPC.knockBackResist = 0f;
            AIType = -1;
            NPC.value = Item.buyPrice(0, 0, 60, 0);
            NPC.HitSound = SoundID.NPCHit36;
            NPC.DeathSound = SoundID.NPCDeath39;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            Banner = ModContent.NPCType<PhantomSpirit>();
            BannerItem = ModContent.ItemType<PhantomSpiritBanner>();
            NPC.Calamity().VulnerableToSickness = false;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.TheDungeon,

				// Will move to localization whenever that is cleaned up.
				new FlavorTextBestiaryInfoElement("Spirits which turned upon one another, cannibalizing others in their mindless frustrated frenzies. There is precedent in the past, for spirits which have devoured enough of others, to grow astoundingly in power.")
            });
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter += 0.15f;
            NPC.frameCounter %= Main.npcFrameCount[NPC.type];
            int frame = (int)NPC.frameCounter;
            NPC.frame.Y = frame * frameHeight;
        }

        public override void AI()
        {
            float speed = (CalamityWorld.death || BossRushEvent.BossRushActive) ? 16f : CalamityWorld.revenge ? 14f : 12f;
            CalamityAI.DungeonSpiritAI(NPC, Mod, speed, -MathHelper.PiOver2);
            int num822 = Dust.NewDust(NPC.position, NPC.width, NPC.height, (int)CalamityDusts.Phantoplasm, 0f, 0f, 0, default, 1f);
            Dust dust = Main.dust[num822];
            dust.velocity *= 0.1f;
            dust.scale = 1.3f;
            dust.noGravity = true;
            Vector2 vector17 = new Vector2(NPC.position.X + (float)NPC.width * 0.5f, NPC.position.Y + (float)NPC.height * 0.5f);
            float num147 = Main.player[NPC.target].position.X + (float)(Main.player[NPC.target].width / 2) - vector17.X;
            float num148 = Main.player[NPC.target].position.Y + (float)(Main.player[NPC.target].height / 2) - vector17.Y;
            float num149 = (float)Math.Sqrt((double)(num147 * num147 + num148 * num148));

            if (NPC.justHit)
                NPC.ai[2] = 0f;

            NPC.ai[2] += 1f;
            if (Main.netMode != NetmodeID.MultiplayerClient && NPC.ai[2] >= 150f)
            {
                NPC.ai[2] = 0f;
                float num151 = 10f;
                int type = ModContent.ProjectileType<PhantomGhostShot>();
                int damage = NPC.GetProjectileDamage(type);
                num149 = num151 / num149;
                num147 *= num149;
                num148 *= num149;
                Projectile.NewProjectile(NPC.GetSource_FromAI(), vector17.X, vector17.Y, num147, num148, type, damage, 0f, Main.myPlayer, 0f, 0f);
            }
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, (int)CalamityDusts.Phantoplasm, hit.HitDirection, -1f, 0, default, 1f);
            }
            if (NPC.life <= 0)
            {
                for (int num288 = 0; num288 < 50; num288++)
                {
                    int num289 = Dust.NewDust(NPC.position, NPC.width, NPC.height, (int)CalamityDusts.Phantoplasm, NPC.velocity.X, NPC.velocity.Y, 0, default, 1f);
                    Dust dust = Main.dust[num289];
                    dust.velocity *= 2f;
                    dust.noGravity = true;
                    dust.scale = 1.4f;
                }
            }
        }

        public override Color? GetAlpha(Color drawColor) => new Color(200, 200, 200, 0);

        public override void ModifyNPCLoot(NPCLoot npcLoot) => npcLoot.Add(ModContent.ItemType<Phantoplasm>(), 1, 2, 4);
    }
}
