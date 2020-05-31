using CalamityMod.Dusts;
using CalamityMod.Projectiles.Boss;
using CalamityMod.Projectiles.Summon;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.SupremeCalamitas
{
	[AutoloadBossHead]
    public class SupremeCatastrophe : ModNPC
    {
        private int distanceY = 375;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Catastrophe");
            Main.npcFrameCount[npc.type] = 6;
			NPCID.Sets.TrailingMode[npc.type] = 1;
		}

        public override void SetDefaults()
        {
            npc.damage = 0;
            npc.npcSlots = 5f;
            npc.width = 120;
            npc.height = 120;
            npc.defense = 100;
            CalamityGlobalNPC global = npc.Calamity();
            global.DR = CalamityWorld.bossRushActive ? 0.6f : CalamityWorld.death ? 0.75f : 0.7f;
            global.customDR = true;
            global.multDRReductions.Add(BuffID.Ichor, 0.9f);
            global.multDRReductions.Add(BuffID.CursedInferno, 0.91f);
			npc.LifeMaxNERB(1200000, 1500000);
            double HPBoost = (double)CalamityConfig.Instance.BossHealthPercentageBoost * 0.01;
            npc.lifeMax += (int)((double)npc.lifeMax * HPBoost);
            npc.aiStyle = -1;
            aiType = -1;
            npc.knockBackResist = 0f;
            npc.noGravity = true;
            npc.noTileCollide = true;
            for (int k = 0; k < npc.buffImmune.Length; k++)
            {
                npc.buffImmune[k] = true;
            }
            npc.buffImmune[BuffID.Ichor] = false;
            npc.buffImmune[BuffID.CursedInferno] = false;
            npc.HitSound = SoundID.NPCHit4;
            npc.DeathSound = SoundID.NPCDeath14;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(distanceY);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            distanceY = reader.ReadInt32();
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
            CalamityGlobalNPC.SCalCatastrophe = npc.whoAmI;
            bool expertMode = Main.expertMode;
            if (CalamityGlobalNPC.SCal < 0 || !Main.npc[CalamityGlobalNPC.SCal].active)
            {
                npc.active = false;
                npc.netUpdate = true;
                return;
            }
            npc.TargetClosest(true);
            float num676 = 60f;
            float num677 = 1.5f;
            float distanceX = 750f;
            if (npc.ai[3] < 750f)
            {
                npc.ai[3] += 1f;
                distanceY -= 1;
            }
            else if (npc.ai[3] < 1500f)
            {
                npc.ai[3] += 1f;
                distanceY += 1;
            }
            if (npc.ai[3] >= 1500f)
            {
                npc.ai[3] = 0f;
            }
            Vector2 vector83 = new Vector2(npc.Center.X, npc.Center.Y);
            float num678 = Main.player[npc.target].Center.X - vector83.X - distanceX;
            float num679 = Main.player[npc.target].Center.Y - vector83.Y + (float)distanceY;
            npc.rotation = 4.71f;
            float num680 = (float)Math.Sqrt((double)(num678 * num678 + num679 * num679));
            num680 = num676 / num680;
            num678 *= num680;
            num679 *= num680;
            if (npc.velocity.X < num678)
            {
                npc.velocity.X = npc.velocity.X + num677;
                if (npc.velocity.X < 0f && num678 > 0f)
                {
                    npc.velocity.X = npc.velocity.X + num677;
                }
            }
            else if (npc.velocity.X > num678)
            {
                npc.velocity.X = npc.velocity.X - num677;
                if (npc.velocity.X > 0f && num678 < 0f)
                {
                    npc.velocity.X = npc.velocity.X - num677;
                }
            }
            if (npc.velocity.Y < num679)
            {
                npc.velocity.Y = npc.velocity.Y + num677;
                if (npc.velocity.Y < 0f && num679 > 0f)
                {
                    npc.velocity.Y = npc.velocity.Y + num677;
                }
            }
            else if (npc.velocity.Y > num679)
            {
                npc.velocity.Y = npc.velocity.Y - num677;
                if (npc.velocity.Y > 0f && num679 < 0f)
                {
                    npc.velocity.Y = npc.velocity.Y - num677;
                }
            }
            if (npc.localAI[0] < 120f)
            {
                npc.localAI[0] += 1f;
            }
            if (npc.localAI[0] >= 120f)
            {
                npc.ai[1] += 1f;
                if (npc.ai[1] >= 30f)
                {
                    npc.ai[1] = 0f;
                    Vector2 vector85 = new Vector2(npc.Center.X, npc.Center.Y);
                    float num689 = 4f;
                    int num690 = expertMode ? 150 : 200; //600 500
                    int num691 = ModContent.ProjectileType<BrimstoneHellblast2>();
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        int num695 = Projectile.NewProjectile(vector85.X, vector85.Y, num689, 0f, num691, num690, 0f, Main.myPlayer, 0f, 0f);
                    }
                }
                npc.ai[2] += 1f;
                if (!NPC.AnyNPCs(ModContent.NPCType<SupremeCataclysm>()))
                {
                    npc.ai[2] += 2f;
                }
                if (npc.ai[2] >= 300f)
                {
                    npc.ai[2] = 0f;
                    float num689 = 7f;
                    int num690 = expertMode ? 150 : 200; //600 500
                    Main.PlaySound(SoundID.Item20, npc.position);
                    float spread = 45f * 0.0174f;
                    double startAngle = Math.Atan2(npc.velocity.X, npc.velocity.Y) - spread / 2;
                    double deltaAngle = spread / 8f;
                    double offsetAngle;
                    int i;
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        for (i = 0; i < 8; i++)
                        {
                            offsetAngle = startAngle + deltaAngle * (i + i * i) / 2f + 32f * i;
                            Projectile.NewProjectile(npc.Center.X, npc.Center.Y, (float)(Math.Sin(offsetAngle) * num689), (float)(Math.Cos(offsetAngle) * num689), ModContent.ProjectileType<BrimstoneBarrage>(), num690, 0f, Main.myPlayer, 0f, 1f);
                            Projectile.NewProjectile(npc.Center.X, npc.Center.Y, (float)(-Math.Sin(offsetAngle) * num689), (float)(-Math.Cos(offsetAngle) * num689), ModContent.ProjectileType<BrimstoneBarrage>(), num690, 0f, Main.myPlayer, 0f, 1f);
                        }
                    }
                    for (int dust = 0; dust <= 5; dust++)
                    {
                        Dust.NewDust(npc.position + npc.velocity, npc.width, npc.height, (int)CalamityDusts.Brimstone, 0f, 0f);
                    }
                }
            }
        }

        public override void ModifyHitByProjectile(Projectile projectile, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            if (projectile.type == ModContent.ProjectileType<SonOfYharon>())
            {
                damage /= 2;
            }
        }

        public override bool StrikeNPC(ref double damage, int defense, ref float knockback, int hitDirection, ref bool crit)
        {
            return !CalamityUtils.AntiButcher(npc, ref damage, 0.5f);
        }

		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			SpriteEffects spriteEffects = SpriteEffects.None;
			if (npc.spriteDirection == 1)
				spriteEffects = SpriteEffects.FlipHorizontally;

			Texture2D texture2D15 = Main.npcTexture[npc.type];
			Vector2 vector11 = new Vector2((float)(Main.npcTexture[npc.type].Width / 2), (float)(Main.npcTexture[npc.type].Height / Main.npcFrameCount[npc.type] / 2));
			Color color36 = Color.White;
			float amount9 = 0.5f;
			int num153 = 7;

			if (CalamityConfig.Instance.Afterimages)
			{
				for (int num155 = 1; num155 < num153; num155 += 2)
				{
					Color color38 = lightColor;
					color38 = Color.Lerp(color38, color36, amount9);
					color38 = npc.GetAlpha(color38);
					color38 *= (float)(num153 - num155) / 15f;
					Vector2 vector41 = npc.oldPos[num155] + new Vector2((float)npc.width, (float)npc.height) / 2f - Main.screenPosition;
					vector41 -= new Vector2((float)texture2D15.Width, (float)(texture2D15.Height / Main.npcFrameCount[npc.type])) * npc.scale / 2f;
					vector41 += vector11 * npc.scale + new Vector2(0f, 4f + npc.gfxOffY);
					spriteBatch.Draw(texture2D15, vector41, npc.frame, color38, npc.rotation, vector11, npc.scale, spriteEffects, 0f);
				}
			}

			Vector2 vector43 = npc.Center - Main.screenPosition;
			vector43 -= new Vector2((float)texture2D15.Width, (float)(texture2D15.Height / Main.npcFrameCount[npc.type])) * npc.scale / 2f;
			vector43 += vector11 * npc.scale + new Vector2(0f, 4f + npc.gfxOffY);
			spriteBatch.Draw(texture2D15, vector43, npc.frame, npc.GetAlpha(lightColor), npc.rotation, vector11, npc.scale, spriteEffects, 0f);

			texture2D15 = ModContent.GetTexture("CalamityMod/NPCs/SupremeCalamitas/SupremeCatastropheGlow");
			Color color37 = Color.Lerp(Color.White, Color.Red, 0.5f);

			if (CalamityConfig.Instance.Afterimages)
			{
				for (int num163 = 1; num163 < num153; num163++)
				{
					Color color41 = color37;
					color41 = Color.Lerp(color41, color36, amount9);
					color41 *= (float)(num153 - num163) / 15f;
					Vector2 vector44 = npc.oldPos[num163] + new Vector2((float)npc.width, (float)npc.height) / 2f - Main.screenPosition;
					vector44 -= new Vector2((float)texture2D15.Width, (float)(texture2D15.Height / Main.npcFrameCount[npc.type])) * npc.scale / 2f;
					vector44 += vector11 * npc.scale + new Vector2(0f, 4f + npc.gfxOffY);
					spriteBatch.Draw(texture2D15, vector44, npc.frame, color41, npc.rotation, vector11, npc.scale, spriteEffects, 0f);
				}
			}

			spriteBatch.Draw(texture2D15, vector43, npc.frame, color37, npc.rotation, vector11, npc.scale, spriteEffects, 0f);

			return false;
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
            if (npc.life <= 0)
            {
                npc.position.X = npc.position.X + (float)(npc.width / 2);
                npc.position.Y = npc.position.Y + (float)(npc.height / 2);
                npc.width = 100;
                npc.height = 100;
                npc.position.X = npc.position.X - (float)(npc.width / 2);
                npc.position.Y = npc.position.Y - (float)(npc.height / 2);
                for (int num621 = 0; num621 < 40; num621++)
                {
                    int num622 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, (int)CalamityDusts.Brimstone, 0f, 0f, 100, default, 2f);
                    Main.dust[num622].velocity *= 3f;
                    if (Main.rand.NextBool(2))
                    {
                        Main.dust[num622].scale = 0.5f;
                        Main.dust[num622].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                    }
                }
                for (int num623 = 0; num623 < 70; num623++)
                {
                    int num624 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, (int)CalamityDusts.Brimstone, 0f, 0f, 100, default, 3f);
                    Main.dust[num624].noGravity = true;
                    Main.dust[num624].velocity *= 5f;
                    num624 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, (int)CalamityDusts.Brimstone, 0f, 0f, 100, default, 2f);
                    Main.dust[num624].velocity *= 2f;
                }
            }
        }
    }
}
