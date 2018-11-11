﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;

namespace CalamityMod.NPCs.AstralBiomeNPCs
{
    public class Nova : ModNPC
    {
        private static Texture2D glowmask;

        private const float travelAcceleration = 0.2f;
        private const float targetTime = 120f;
        private const float waitBeforeTravel = 20f;
        private const float maxTravelTime = 300f;
        private const float slowdown = 0.84f;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Nova");
            Main.npcFrameCount[npc.type] = 8;

            glowmask = mod.GetTexture("NPCs/AstralBiomeNPCs/NovaGlow");
        }

        public override void SetDefaults()
        {
            npc.width = 78;
            npc.height = 50;
            npc.damage = 75;
            npc.defense = 25;
            npc.lifeMax = 350;
            npc.HitSound = mod.GetLegacySoundSlot(SoundType.NPCHit, "Sounds/NPCHit/AstralEnemyHit");
            npc.DeathSound = mod.GetLegacySoundSlot(SoundType.NPCHit, "Sounds/NPCHit/AstralEnemyDeath");
            npc.noGravity = true;
            npc.knockBackResist = 0.4f;
            npc.value = 1100;
            npc.aiStyle = -1;
        }

        public override void FindFrame(int frameHeight)
        {
            npc.frameCounter++;
            if (npc.ai[3] >= 0f)
            {
                if (npc.frameCounter >= 8)
                {
                    npc.frameCounter = 0;
                    npc.frame.Y += frameHeight;
                    if (npc.frame.Y >= frameHeight * 4)
                    {
                        npc.frame.Y = 0;
                    }
                }
            }
            else
            {
                if (npc.frameCounter >= 7)
                {
                    npc.frameCounter = 0;
                    npc.frame.Y += frameHeight;
                    if (npc.frame.Y >= frameHeight * 8)
                    {
                        npc.frame.Y = frameHeight * 4;
                    }
                }
            }
            
            //DO DUST
            Dust d = CalamityGlobalNPC.SpawnDustOnNPC(npc, 114, frameHeight, mod.DustType("AstralOrange"), new Rectangle(78, 34, 36, 18), Vector2.Zero, 0.45f, true);
            if (d != null)
            {
                d.customData = 0.04f;
            }
        }

        public override void AI()
        {
            Player target = Main.player[npc.target];
            if (npc.ai[3] >= 0)
            {
                CalamityGlobalNPC.DoFlyingAI(npc, 5.5f, 0.035f, 400f, 150, false);

                if (Collision.CanHit(npc.position, npc.width, npc.height, target.position, target.width, target.height))
                {
                    npc.ai[3]++;
                }
                else
                {
                    npc.ai[3] = 0f;
                }

                Vector2 between = target.Center - npc.Center;

                //after locking target for x amount of time and being far enough away
                if (between.Length() > 150 && npc.ai[3] >= targetTime && Main.rand.Next(180) == 0) 
                {
                    //set ai mode to target and travel
                    npc.ai[3] = -1f;
                }
                return;
            }
            else
            {
                npc.ai[3]--;
                Vector2 between = target.Center - npc.Center;

                if (npc.ai[3] < -waitBeforeTravel)
                {
                    if (npc.collideX || npc.collideY || npc.ai[3] < -maxTravelTime)
                    {
                        Explode();
                    }
                    
                    npc.velocity += new Vector2(npc.ai[1], npc.ai[2]) * travelAcceleration; //acceleration per frame
                    
                    //rotation
                    npc.rotation = npc.velocity.ToRotation();
                }
                else if (npc.ai[3] == -waitBeforeTravel)
                {
                    between.Normalize();
                    npc.ai[1] = between.X;
                    npc.ai[2] = between.Y;

                    //rotation
                    npc.rotation = between.ToRotation();
                    npc.velocity = Vector2.Zero;
                }
                else
                {
                    //slowdown
                    npc.velocity *= slowdown;

                    //rotation
                    npc.rotation = between.ToRotation();
                }
                npc.rotation += MathHelper.Pi;
            }
        }

        private void Explode()
        {
            //kill NPC
            Main.PlaySound(SoundID.Item14, npc.Center);

            //change stuffs
            Vector2 center = npc.Center;
            npc.width = 200;
            npc.height = 200;
            npc.Center = center;

            Rectangle myRect = npc.getRect();

            if (Main.netMode != 1)
            {
                for (int i = 0; i < 200; i++)
                {
                    if (Main.player[i].getRect().Intersects(myRect))
                    {
                        int direction = npc.Center.X - Main.player[i].Center.X < 0 ? -1 : 1;
                        Main.player[i].Hurt(PlayerDeathReason.ByNPC(npc.whoAmI), 100, direction);
                    }
                }
            }

            //other things
            npc.ai[3] = -20000f;
            npc.value = 0f;
            npc.extraValue = 0f;
            npc.StrikeNPCNoInteraction(9999, 1f, 1);

            int size = 30;
            Vector2 off = new Vector2(size / -2f);

            for (int i = 0; i < 45; i++)
            {
                int dust = Dust.NewDust(npc.Center - off, size, size, mod.DustType("AstralEnemy"), Main.rand.NextFloat(-3f, 3f), Main.rand.NextFloat(-3f, 3f), 0, default(Color), Main.rand.NextFloat(1f, 2f));
                Main.dust[dust].velocity *= 1.4f;
            }
            for (int i = 0; i < 15; i++)
            {
                int dust = Dust.NewDust(npc.Center - off, size, size, 31, 0f, 0f, 100, default(Color), 1.7f);
                Main.dust[dust].velocity *= 1.4f;
            }
            for (int i = 0; i < 27; i++)
            {
                int dust = Dust.NewDust(npc.Center - off, size, size, 6, 0f, 0f, 100, default(Color), 2.4f);
                Main.dust[dust].noGravity = true;
                Main.dust[dust].velocity *= 5f;
                dust = Dust.NewDust(npc.Center - off, size, size, 6, 0f, 0f, 100, default(Color), 1.6f);
                Main.dust[dust].velocity *= 3f;
            }
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            CalamityGlobalNPC.DoHitDust(npc, hitDirection, (Main.rand.Next(0, Math.Max(0, npc.life)) == 0) ? 5 : mod.DustType("AstralEnemy"), 1f, 3, 40);

            //if dead do gores
            if (npc.life <= 0)
            {
                for (int i = 0; i < 7; i++)
                {
                    Gore.NewGore(npc.Center, npc.velocity * 0.3f, mod.GetGoreSlot("Gores/Nova/NovaGore" + i));
                }
            }
        }

        public override bool PreNPCLoot()
        {
            return npc.ai[3] > -10000;
        }

        public override void PostDraw(SpriteBatch spriteBatch, Color drawColor)
        {
            spriteBatch.Draw(glowmask, npc.Center - Main.screenPosition - new Vector2(0, 4f), npc.frame, Color.White * 0.75f, npc.rotation, new Vector2(57f, 37f), npc.scale, npc.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0f);
        }
        
        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.player.GetModPlayer<CalamityPlayer>().ZoneAstral && spawnInfo.player.ZoneOverworldHeight)
            {
                return 0.19f;
            }
            return 0f;
        }
    }
}
