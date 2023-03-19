﻿using CalamityMod.Dusts;
using CalamityMod.Projectiles.Boss;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.Polterghast
{
    public class PhantomFuckYou : ModNPC
    {
        private bool start = true;

        public override void SetStaticDefaults()
        {
            this.HideFromBestiary();
            DisplayName.SetDefault("Phantom");
            NPCID.Sets.TrailingMode[NPC.type] = 1;
        }

        public override void SetDefaults()
        {
            NPC.aiStyle = -1;
            AIType = -1;
            NPC.width = 30;
            NPC.height = 30;
            NPC.defense = 45;
            NPC.DR_NERD(0.1f);
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.canGhostHeal = false;
            NPC.damage = 0;
            NPC.LifeMaxNERB(Main.expertMode ? 22500 : 17500, 37500, 30000);
            NPC.HitSound = SoundID.NPCHit36;
            NPC.DeathSound = SoundID.NPCDeath39;
            NPC.Calamity().VulnerableToSickness = false;
            if (CalamityWorld.getFixedBoi)
            {
                NPC.dontTakeDamage = true;
            }
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(start);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            start = reader.ReadBoolean();
        }

        public override bool PreAI()
        {
            if (start)
            {
                start = false;

                for (int i = 0; i < 10; i++)
                {
                    int dust = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, (int)CalamityDusts.Ectoplasm, 0f, 0f, 100, default, 2f);
                    Main.dust[dust].noGravity = true;
                }

                NPC.ai[1] = NPC.ai[0];
            }

            if (CalamityGlobalNPC.ghostBoss < 0 || !Main.npc[CalamityGlobalNPC.ghostBoss].active)
            {
                NPC.life = 0;
                NPC.HitEffect();
                NPC.active = false;
                NPC.netUpdate = true;
                return false;
            }

            float chargePhaseGateValue = 480f;
            if (Main.getGoodWorld)
                chargePhaseGateValue *= 0.5f;

            bool chargePhase = Main.npc[CalamityGlobalNPC.ghostBoss].Calamity().newAI[0] >= chargePhaseGateValue - 60f;

            // Percent life remaining, Polter
            float lifeRatio = Main.npc[CalamityGlobalNPC.ghostBoss].life / Main.npc[CalamityGlobalNPC.ghostBoss].lifeMax;

            // Scale multiplier based on nearby active tiles
            float tileEnrageMult = Main.npc[CalamityGlobalNPC.ghostBoss].ai[3];

            NPC.TargetClosest(true);

            Vector2 direction = Main.player[NPC.target].Center - NPC.Center;
            direction.Normalize();
            direction *= 0.5f;
            NPC.rotation = direction.ToRotation();

            if (!chargePhase)
            {
                NPC.ai[2] += 1f;
                float shootMineGateValue = 150f;
                if (Main.getGoodWorld)
                    shootMineGateValue *= 0.5f;

                if (NPC.ai[2] >= shootMineGateValue)
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        int type = ModContent.ProjectileType<PhantomMine>();
                        int damage = NPC.GetProjectileDamage(type);
                        float maxVelocity = 8f * tileEnrageMult;
                        float acceleration = 1.15f + (tileEnrageMult - 1f) * 0.15f;
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, direction, type, damage, 1f, NPC.target, maxVelocity, acceleration);
                    }
                    NPC.ai[2] = 0f;
                }
            }

            NPC parent = Main.npc[NPC.FindFirstNPC(ModContent.NPCType<Polterghast>())];
            double deg = NPC.ai[1];
            double rad = deg * (Math.PI / 180);
            double dist = 500;
            NPC.position.X = parent.Center.X - (int)(Math.Cos(rad) * dist) - NPC.width / 2;
            NPC.position.Y = parent.Center.Y - (int)(Math.Sin(rad) * dist) - NPC.height / 2;
            float SPEEN = 1f - lifeRatio * 2f;
            if (SPEEN < 0f)
                SPEEN = 0f;
            NPC.ai[1] += (Main.getGoodWorld ? 1.5f : 0.5f) + SPEEN;
            return false;
        }

        public override Color? GetAlpha(Color drawColor) => new Color(200, 200, 200, 0);

        public override bool CheckActive()
        {
            return false;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (NPC.spriteDirection == 1)
                spriteEffects = SpriteEffects.FlipHorizontally;

            Texture2D texture2D15 = TextureAssets.Npc[NPC.type].Value;
            Vector2 vector11 = new Vector2(TextureAssets.Npc[NPC.type].Value.Width / 2, TextureAssets.Npc[NPC.type].Value.Height / 2);
            Color color36 = Color.White;
            float amount9 = 0.5f;
            int num153 = 5;

            if (CalamityConfig.Instance.Afterimages)
            {
                for (int num155 = 1; num155 < num153; num155 += 2)
                {
                    Color color38 = drawColor;
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
            spriteBatch.Draw(texture2D15, vector43, NPC.frame, NPC.GetAlpha(drawColor), NPC.rotation, vector11, NPC.scale, spriteEffects, 0f);

            return false;
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            Dust.NewDust(NPC.position, NPC.width, NPC.height, 180, hitDirection, -1f, 0, default, 1f);
            if (NPC.life <= 0)
            {
                NPC.position.X = NPC.position.X + (NPC.width / 2);
                NPC.position.Y = NPC.position.Y + (NPC.height / 2);
                NPC.width = 45;
                NPC.height = 45;
                NPC.position.X = NPC.position.X - (NPC.width / 2);
                NPC.position.Y = NPC.position.Y - (NPC.height / 2);
                for (int num621 = 0; num621 < 2; num621++)
                {
                    int num622 = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, (int)CalamityDusts.Phantoplasm, 0f, 0f, 100, default, 2f);
                    Main.dust[num622].velocity *= 3f;
                    if (Main.rand.NextBool(2))
                    {
                        Main.dust[num622].scale = 0.5f;
                        Main.dust[num622].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                    }
                }
                for (int num623 = 0; num623 < 10; num623++)
                {
                    int num624 = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, 180, 0f, 0f, 100, default, 3f);
                    Main.dust[num624].noGravity = true;
                    Main.dust[num624].velocity *= 5f;
                    num624 = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, 180, 0f, 0f, 100, default, 2f);
                    Main.dust[num624].velocity *= 2f;
                }
            }
        }
    }
}
