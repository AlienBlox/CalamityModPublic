﻿using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles; using CalamityMod.Tiles; using CalamityMod.Walls;

namespace CalamityMod.NPCs
{
    public class SeaSerpent4 : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sea Serpent");
        }

        public override void SetDefaults()
        {
            npc.damage = 30;
            npc.width = 28; //42
            npc.height = 24; //34
            npc.defense = 20;
            npc.lifeMax = 3000;
            npc.aiStyle = -1; //new
            aiType = -1; //new
            npc.knockBackResist = 0f;
            npc.alpha = 255;
            for (int k = 0; k < npc.buffImmune.Length; k++)
            {
                npc.buffImmune[k] = true;
            }
            npc.behindTiles = true;
            npc.noGravity = true;
            npc.noTileCollide = true;
            npc.HitSound = SoundID.NPCHit1;
            npc.DeathSound = SoundID.NPCDeath1;
            npc.netAlways = true;
            npc.dontCountMe = true;
            npc.chaseable = false;
            banner = ModContent.NPCType<SeaSerpent1>();
            bannerItem = ModContent.ItemType<SeaSerpentBanner>();
        }

        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
        {
            return false;
        }

        public override void AI()
        {
            Lighting.AddLight(npc.Center, (255 - npc.alpha) * 0f / 255f, (255 - npc.alpha) * 0.30f / 255f, (255 - npc.alpha) * 0.30f / 255f);
            if (npc.ai[3] > 0f)
            {
                npc.realLife = (int)npc.ai[3];
            }
            if (npc.target < 0 || npc.target == 255 || Main.player[npc.target].dead)
            {
                npc.TargetClosest(true);
            }
            npc.velocity.Length();
            if (npc.velocity.X < 0f)
            {
                npc.spriteDirection = -1;
            }
            else if (npc.velocity.X > 0f)
            {
                npc.spriteDirection = 1;
            }
            bool flag = false;
            if (npc.ai[1] <= 0f)
            {
                flag = true;
            }
            else if (Main.npc[(int)npc.ai[1]].life <= 0)
            {
                flag = true;
            }
            if (flag)
            {
                npc.life = 0;
                npc.HitEffect(0, 10.0);
                npc.checkDead();
            }
            if (!NPC.AnyNPCs(ModContent.NPCType<SeaSerpent1>()))
            {
                npc.active = false;
            }
            if (Main.npc[(int)npc.ai[1]].alpha < 128)
            {
                npc.alpha -= 42;
                if (npc.alpha < 0)
                {
                    npc.alpha = 0;
                }
            }
            if (Main.player[npc.target].dead)
            {
                npc.TargetClosest(false);
            }
            Vector2 vector18 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
            float num191 = Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2);
            float num192 = Main.player[npc.target].position.Y + (float)(Main.player[npc.target].height / 2);
            num191 = (float)((int)(num191 / 16f) * 16);
            num192 = (float)((int)(num192 / 16f) * 16);
            vector18.X = (float)((int)(vector18.X / 16f) * 16);
            vector18.Y = (float)((int)(vector18.Y / 16f) * 16);
            num191 -= vector18.X;
            num192 -= vector18.Y;
            float num193 = (float)System.Math.Sqrt((double)(num191 * num191 + num192 * num192));
            if (npc.ai[1] > 0f && npc.ai[1] < (float)Main.npc.Length)
            {
                try
                {
                    vector18 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
                    num191 = Main.npc[(int)npc.ai[1]].position.X + (float)(Main.npc[(int)npc.ai[1]].width / 2) - vector18.X;
                    num192 = Main.npc[(int)npc.ai[1]].position.Y + (float)(Main.npc[(int)npc.ai[1]].height / 2) - vector18.Y;
                } catch
                {
                }
                npc.rotation = (float)System.Math.Atan2((double)num192, (double)num191) + 1.57f;
                num193 = (float)System.Math.Sqrt((double)(num191 * num191 + num192 * num192));
                int num194 = npc.width;
                num193 = (num193 - (float)num194) / num193;
                num191 *= num193;
                num192 *= num193;
                npc.velocity = Vector2.Zero;
                npc.position.X = npc.position.X + num191;
                npc.position.Y = npc.position.Y + num192;
                if (num191 < 0f)
                {
                    npc.spriteDirection = -1;
                }
                else if (num191 > 0f)
                {
                    npc.spriteDirection = 1;
                }
            }
        }

        public override bool CheckActive()
        {
            return false;
        }

        public override bool PreNPCLoot()
        {
            return false;
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            for (int k = 0; k < 3; k++)
            {
                Dust.NewDust(npc.position, npc.width, npc.height, 37, hitDirection, -1f, 0, default, 1f);
            }
            if (npc.life <= 0)
            {
                for (int k = 0; k < 10; k++)
                {
                    Dust.NewDust(npc.position, npc.width, npc.height, 37, hitDirection, -1f, 0, default, 1f);
                }
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/SeaSerpent/SeaSerpentGore4"), 1f);
            }
        }
    }
}
