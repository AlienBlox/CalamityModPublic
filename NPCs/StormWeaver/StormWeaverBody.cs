﻿using CalamityMod.Dusts;
using CalamityMod.Events;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.StormWeaver
{
    public class StormWeaverBody : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Storm Weaver");
            NPCID.Sets.TrailingMode[NPC.type] = 1;
        }

        public override void SetDefaults()
        {
            NPC.GetNPCDamage();
            NPC.npcSlots = 5f;
            NPC.width = 40;
            NPC.height = 40;

            // 10% of HP is phase one
            bool notDoGFight = CalamityWorld.DoGSecondStageCountdown <= 0 || !DownedBossSystem.downedStormWeaver;
            NPC.lifeMax = notDoGFight ? 825500 : 139750;
            NPC.LifeMaxNERB(NPC.lifeMax, NPC.lifeMax, 475000);

            // If fought alone, Storm Weaver plays its own theme
            if (notDoGFight)
                Music = CalamityMod.Instance.GetMusicFromMusicMod("Weaver") ?? MusicID.Boss3;
            // If fought as a DoG interlude, keep the DoG music playing
            else
                Music = CalamityMod.Instance.GetMusicFromMusicMod("ScourgeofTheUniverse") ?? MusicID.Boss3;

            // Phase one settings
            CalamityGlobalNPC global = NPC.Calamity();
            NPC.defense = 150;
            global.DR = 0.999999f;
            global.unbreakableDR = true;
            NPC.chaseable = false;
            NPC.HitSound = SoundID.NPCHit4;
            NPC.DeathSound = SoundID.NPCDeath14;

            double HPBoost = CalamityConfig.Instance.BossHealthBoost * 0.01;
            NPC.lifeMax += (int)(NPC.lifeMax * HPBoost);
            NPC.aiStyle = -1;
            AIType = -1;
            NPC.knockBackResist = 0f;
            NPC.alpha = 255;
            NPC.behindTiles = true;
            NPC.noGravity = true;
            NPC.boss = true;
            NPC.noTileCollide = true;
            NPC.canGhostHeal = false;
            NPC.netAlways = true;
            NPC.dontCountMe = true;

            if (CalamityWorld.malice || BossRushEvent.BossRushActive)
                NPC.scale = 1.25f;
            else if (CalamityWorld.death)
                NPC.scale = 1.2f;
            else if (CalamityWorld.revenge)
                NPC.scale = 1.15f;
            else if (Main.expertMode)
                NPC.scale = 1.1f;

            NPC.Calamity().VulnerableToElectricity = false;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(NPC.chaseable);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            NPC.chaseable = reader.ReadBoolean();
        }

        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
        {
            return false;
        }

        public override void AI()
        {
            if (NPC.ai[2] > 0f)
                NPC.realLife = (int)NPC.ai[2];

            if (NPC.life > Main.npc[(int)NPC.ai[1]].life)
                NPC.life = Main.npc[(int)NPC.ai[1]].life;

            // Shed armor
            bool shedArmor = NPC.life / (float)NPC.lifeMax < 0.9f;

            // Update armored settings to naked settings
            if (shedArmor)
            {
                // Spawn armor gore and set other crucial variables
                if (!NPC.chaseable)
                {
                    NPC.Calamity().VulnerableToHeat = true;
                    NPC.Calamity().VulnerableToCold = true;
                    NPC.Calamity().VulnerableToSickness = true;

                    if (Main.netMode != NetmodeID.Server)
                    {
                        Gore.NewGore(NPC.position, NPC.velocity, Mod.Find<ModGore>("Gores/SWArmorBody1").Type, NPC.scale);
                        Gore.NewGore(NPC.position, NPC.velocity, Mod.Find<ModGore>("Gores/SWArmorBody2").Type, NPC.scale);
                        Gore.NewGore(NPC.position, NPC.velocity, Mod.Find<ModGore>("Gores/SWArmorBody3").Type, NPC.scale);
                    }

                    CalamityGlobalNPC global = NPC.Calamity();
                    NPC.defense = 30;
                    global.DR = 0.2f;
                    global.unbreakableDR = false;
                    NPC.chaseable = true;
                    NPC.HitSound = SoundID.NPCHit13;
                    NPC.DeathSound = SoundID.NPCDeath13;
                    NPC.frame = new Rectangle(0, 0, 54, 52);
                }
            }

            Lighting.AddLight((int)((NPC.position.X + (NPC.width / 2)) / 16f), (int)((NPC.position.Y + (NPC.height / 2)) / 16f), 0.2f, 0.05f, 0.2f);

            // Check if other segments are still alive, if not, die
            bool shouldDespawn = true;
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                if (Main.npc[i].active && Main.npc[i].type == ModContent.NPCType<StormWeaverHead>())
                {
                    shouldDespawn = false;
                    break;
                }
            }
            if (!shouldDespawn)
            {
                if (NPC.ai[1] <= 0f)
                    shouldDespawn = true;
                else if (Main.npc[(int)NPC.ai[1]].life <= 0)
                    shouldDespawn = true;
            }
            if (shouldDespawn)
            {
                NPC.life = 0;
                NPC.HitEffect(0, 10.0);
                NPC.checkDead();
                NPC.active = false;
            }

            if (Main.npc[(int)NPC.ai[1]].alpha < 128)
            {
                if (NPC.alpha != 0)
                {
                    for (int num934 = 0; num934 < 2; num934++)
                    {
                        int num935 = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, 182, 0f, 0f, 100, default, 2f);
                        Main.dust[num935].noGravity = true;
                        Main.dust[num935].noLight = true;
                    }
                }

                NPC.alpha -= 42;
                if (NPC.alpha < 0)
                    NPC.alpha = 0;
            }

            Vector2 vector18 = new Vector2(NPC.position.X + NPC.width * 0.5f, NPC.position.Y + NPC.height * 0.5f);
            float num191 = Main.player[NPC.target].position.X + (Main.player[NPC.target].width / 2);
            float num192 = Main.player[NPC.target].position.Y + (Main.player[NPC.target].height / 2);
            num191 = (int)(num191 / 16f) * 16;
            num192 = (int)(num192 / 16f) * 16;
            vector18.X = (int)(vector18.X / 16f) * 16;
            vector18.Y = (int)(vector18.Y / 16f) * 16;
            num191 -= vector18.X;
            num192 -= vector18.Y;

            float num193 = (float)System.Math.Sqrt(num191 * num191 + num192 * num192);
            if (NPC.ai[1] > 0f && NPC.ai[1] < Main.npc.Length)
            {
                try
                {
                    vector18 = new Vector2(NPC.position.X + NPC.width * 0.5f, NPC.position.Y + NPC.height * 0.5f);
                    num191 = Main.npc[(int)NPC.ai[1]].position.X + (Main.npc[(int)NPC.ai[1]].width / 2) - vector18.X;
                    num192 = Main.npc[(int)NPC.ai[1]].position.Y + (Main.npc[(int)NPC.ai[1]].height / 2) - vector18.Y;
                } catch
                {
                }

                NPC.rotation = (float)System.Math.Atan2(num192, num191) + MathHelper.PiOver2;
                num193 = (float)System.Math.Sqrt(num191 * num191 + num192 * num192);
                int num194 = NPC.width;
                num193 = (num193 - num194) / num193;
                num191 *= num193;
                num192 *= num193;
                NPC.velocity = Vector2.Zero;
                NPC.position.X = NPC.position.X + num191;
                NPC.position.Y = NPC.position.Y + num192;

                if (num191 < 0f)
                    NPC.spriteDirection = -1;
                else if (num191 > 0f)
                    NPC.spriteDirection = 1;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (NPC.spriteDirection == 1)
                spriteEffects = SpriteEffects.FlipHorizontally;

            bool shedArmor = NPC.life / (float)NPC.lifeMax < 0.9f;
            Texture2D texture2D15 = shedArmor ? ModContent.Request<Texture2D>("CalamityMod/NPCs/StormWeaver/StormWeaverBodyNaked").Value : TextureAssets.Npc[NPC.type].Value;
            Vector2 vector11 = new Vector2(texture2D15.Width / 2, texture2D15.Height / 2);
            Color color36 = Color.White;
            float amount9 = 0.5f;
            int num153 = 5;

            if (CalamityConfig.Instance.Afterimages)
            {
                for (int num155 = 1; num155 < num153; num155 += 2)
                {
                    Color color38 = drawColor;

                    if (CalamityWorld.revenge || BossRushEvent.BossRushActive)
                    {
                        if (Main.npc[(int)NPC.ai[2]].Calamity().newAI[0] > 280f)
                            color38 = Color.Lerp(color38, Color.Cyan, MathHelper.Clamp((Main.npc[(int)NPC.ai[2]].Calamity().newAI[0] - 280f) / 120f, 0f, 1f));
                        else if (Main.npc[(int)NPC.ai[2]].localAI[3] > 0f)
                            color38 = Color.Lerp(color38, Color.Cyan, MathHelper.Clamp(Main.npc[(int)NPC.ai[2]].localAI[3] / 60f, 0f, 1f));
                    }

                    color38 = Color.Lerp(color38, color36, amount9);
                    color38 = NPC.GetAlpha(color38);
                    color38 *= (num153 - num155) / 15f;
                    Vector2 vector41 = NPC.oldPos[num155] + new Vector2(NPC.width, NPC.height) / 2f - screenPos;
                    vector41 -= new Vector2(texture2D15.Width, texture2D15.Height) * NPC.scale / 2f;
                    vector41 += vector11 * NPC.scale + new Vector2(0f, NPC.gfxOffY);
                    spriteBatch.Draw(texture2D15, vector41, NPC.frame, color38, NPC.rotation, vector11, NPC.scale, spriteEffects, 0f);
                }
            }

            Vector2 vector43 = NPC.Center - screenPos;
            vector43 -= new Vector2(texture2D15.Width, texture2D15.Height) * NPC.scale / 2f;
            vector43 += vector11 * NPC.scale + new Vector2(0f, NPC.gfxOffY);
            Color color = NPC.GetAlpha(drawColor);

            if (CalamityWorld.revenge || BossRushEvent.BossRushActive)
            {
                if (Main.npc[(int)NPC.ai[2]].Calamity().newAI[0] > 280f)
                    color = Color.Lerp(color, Color.Cyan, MathHelper.Clamp((Main.npc[(int)NPC.ai[2]].Calamity().newAI[0] - 280f) / 120f, 0f, 1f));
                else if (Main.npc[(int)NPC.ai[2]].localAI[3] > 0f)
                    color = Color.Lerp(color, Color.Cyan, MathHelper.Clamp(Main.npc[(int)NPC.ai[2]].localAI[3] / 60f, 0f, 1f));
            }

            spriteBatch.Draw(texture2D15, vector43, NPC.frame, color, NPC.rotation, vector11, NPC.scale, spriteEffects, 0f);

            return false;
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            cooldownSlot = 1;
            return true;
        }

        public override void OnHitPlayer(Player player, int damage, bool crit)
        {
            int buffDuration = Main.npc[(int)NPC.ai[2]].Calamity().newAI[0] >= 400f ? 180 : 90;
            player.AddBuff(BuffID.Electrified, buffDuration, true);
        }

        public override bool CheckActive()
        {
            return false;
        }

        public override bool PreNPCLoot()
        {
            return false;
        }

        public override void ModifyHitByProjectile(Projectile projectile, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            if (CalamityLists.projectileDestroyExceptionList.TrueForAll(x => projectile.type != x) && NPC.life / (float)NPC.lifeMax >= 0.9f)
            {
                if (projectile.penetrate == -1 && !projectile.minion)
                    projectile.penetrate = 1;
                else if (projectile.penetrate >= 1)
                    projectile.penetrate = 1;
            }
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            for (int k = 0; k < 3; k++)
                Dust.NewDust(NPC.position, NPC.width, NPC.height, (int)CalamityDusts.PurpleCosmilite, hitDirection, -1f, 0, default, 1f);

            if (NPC.life <= 0)
            {
                if (Main.netMode != NetmodeID.Server)
                {
                    Gore.NewGore(NPC.position, NPC.velocity, Mod.Find<ModGore>("Gores/SWNudeBody1").Type, NPC.scale);
                    Gore.NewGore(NPC.position, NPC.velocity, Mod.Find<ModGore>("Gores/SWNudeBody2").Type, NPC.scale);
                    Gore.NewGore(NPC.position, NPC.velocity, Mod.Find<ModGore>("Gores/SWNudeBody3").Type, NPC.scale);
                }

                NPC.position.X = NPC.position.X + (NPC.width / 2);
                NPC.position.Y = NPC.position.Y + (NPC.height / 2);
                NPC.width = 30;
                NPC.height = 30;
                NPC.position.X = NPC.position.X - (NPC.width / 2);
                NPC.position.Y = NPC.position.Y - (NPC.height / 2);

                for (int num621 = 0; num621 < 20; num621++)
                {
                    int num622 = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, (int)CalamityDusts.PurpleCosmilite, 0f, 0f, 100, default, 2f);
                    Main.dust[num622].velocity *= 3f;
                    if (Main.rand.NextBool(2))
                    {
                        Main.dust[num622].scale = 0.5f;
                        Main.dust[num622].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                    }
                }

                for (int num623 = 0; num623 < 40; num623++)
                {
                    int num624 = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, (int)CalamityDusts.PurpleCosmilite, 0f, 0f, 100, default, 3f);
                    Main.dust[num624].noGravity = true;
                    Main.dust[num624].velocity *= 5f;
                    num624 = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, (int)CalamityDusts.PurpleCosmilite, 0f, 0f, 100, default, 2f);
                    Main.dust[num624].velocity *= 2f;
                }
            }
        }

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            NPC.lifeMax = (int)(NPC.lifeMax * 0.8f * bossLifeScale);
            NPC.damage = (int)(NPC.damage * NPC.GetExpertDamageMultiplier());
        }

    }
}
