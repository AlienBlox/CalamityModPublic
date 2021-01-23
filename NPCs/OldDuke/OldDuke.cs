using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Items.Accessories;
using CalamityMod.Items.Armor.Vanity;
using CalamityMod.Items.LoreItems;
using CalamityMod.Items.Placeables.Furniture.Trophies;
using CalamityMod.Items.Potions;
using CalamityMod.Items.TreasureBags;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.NPCs.TownNPCs;
using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Dusts;
using CalamityMod.World;

namespace CalamityMod.NPCs.OldDuke
{
	[AutoloadBossHead]
    public class OldDuke : ModNPC
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("The Old Duke");
            Main.npcFrameCount[npc.type] = 7;
			NPCID.Sets.TrailingMode[npc.type] = 1;
		}
		
		public override void SetDefaults()
		{
			npc.width = 150;
            npc.height = 100;
            npc.aiStyle = -1;
			aiType = -1;
			npc.GetNPCDamage();
			npc.defense = 100;
			npc.DR_NERD(0.5f, null, null, null, true);
			npc.LifeMaxNERB(562500, 750000, 4000000);
			double HPBoost = CalamityConfig.Instance.BossHealthBoost * 0.01;
			npc.lifeMax += (int)(npc.lifeMax * HPBoost);
			npc.knockBackResist = 0f;
            npc.noTileCollide = true;
            npc.noGravity = true;
            npc.npcSlots = 15f;
            npc.HitSound = SoundID.NPCHit14;
            npc.DeathSound = SoundID.NPCDeath20;
			npc.value = Item.buyPrice(0, 70, 0, 0);
			npc.boss = true;
            npc.netAlways = true;
            npc.timeLeft = NPC.activeTime * 30;
            Mod calamityModMusic = ModLoader.GetMod("CalamityModMusic");
            if (calamityModMusic != null)
                music = calamityModMusic.GetSoundSlot(SoundType.Music, "Sounds/Music/BoomerDuke");
            else
                music = MusicID.Boss1;
            bossBag = ModContent.ItemType<OldDukeBag>();
		}

		public override void SendExtraAI(BinaryWriter writer)
		{
			writer.Write(npc.dontTakeDamage);
			writer.Write(npc.localAI[0]);
			writer.Write(npc.rotation);
			writer.Write(npc.spriteDirection);
			for (int i = 0; i < 4; i++)
				writer.Write(npc.Calamity().newAI[i]);
		}

		public override void ReceiveExtraAI(BinaryReader reader)
		{
			npc.dontTakeDamage = reader.ReadBoolean();
			npc.localAI[0] = reader.ReadSingle();
			npc.rotation = reader.ReadSingle();
			npc.spriteDirection = reader.ReadInt32();
			for (int i = 0; i < 4; i++)
				npc.Calamity().newAI[i] = reader.ReadSingle();
		}

		public override void AI()
        {
			CalamityAI.OldDukeAI(npc, mod);
        }

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            npc.lifeMax = (int)(npc.lifeMax * 0.8f * bossLifeScale);
            npc.damage = (int)(npc.damage * npc.GetExpertDamageMultiplier());
        }

        public override void FindFrame(int frameHeight)
        {
			bool tired = npc.Calamity().newAI[1] == 1f;
			if (npc.ai[0] == 0f || npc.ai[0] == 5f || npc.ai[0] == 10f || npc.ai[0] == 12f)
            {
                int num114 = tired ? 14 : 7;
                if (npc.ai[0] == 5f || npc.ai[0] == 12f)
                {
                    num114 = tired ? 12 : 6;
                }
                npc.frameCounter += 1D;
                if (npc.frameCounter > num114)
                {
                    npc.frameCounter = 0D;
                    npc.frame.Y += frameHeight;
                }
                if (npc.frame.Y >= frameHeight * 6)
                {
                    npc.frame.Y = 0;
                }
            }
			if (npc.ai[0] == 1f || npc.ai[0] == 6f || npc.ai[0] == 11f)
			{
				npc.frame.Y = frameHeight * 2;
			}
			if (npc.ai[0] == 2f || npc.ai[0] == 7f || npc.ai[0] == 14f)
			{
				npc.frame.Y = frameHeight * 6;
			}
			if (npc.ai[0] == 3f || npc.ai[0] == 8f || npc.ai[0] == 13f || npc.ai[0] == -1f)
            {
                int num115 = 120;
                if (npc.ai[2] < (num115 - 50) || npc.ai[2] > (num115 - 10))
                {
                    npc.frameCounter += 1D;
                    if (npc.frameCounter > 7D)
                    {
                        npc.frameCounter = 0D;
                        npc.frame.Y += frameHeight;
                    }
                    if (npc.frame.Y >= frameHeight * 6)
                    {
                        npc.frame.Y = 0;
                    }
                }
                else
                {
                    npc.frame.Y = frameHeight * 5;
                    if (npc.ai[2] > (num115 - 40) && npc.ai[2] < (num115 - 15))
                    {
                        npc.frame.Y = frameHeight * 6;
                    }
                }
            }
            if (npc.ai[0] == 4f || npc.ai[0] == 9f)
            {
                int num116 = 180;
                if (npc.ai[2] < (num116 - 60) || npc.ai[2] > (num116 - 20))
                {
                    npc.frameCounter += 1D;
                    if (npc.frameCounter > 7D)
                    {
                        npc.frameCounter = 0D;
                        npc.frame.Y += frameHeight;
                    }
                    if (npc.frame.Y >= frameHeight * 6)
                    {
                        npc.frame.Y = 0;
                    }
                }
                else
                {
                    npc.frame.Y = frameHeight * 5;
                    if (npc.ai[2] > (num116 - 50) && npc.ai[2] < (num116 - 25))
                    {
                        npc.frame.Y = frameHeight * 6;
                    }
                }
            }
        }

		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			SpriteEffects spriteEffects = SpriteEffects.None;
			if (npc.spriteDirection == 1)
			{
				spriteEffects = SpriteEffects.FlipHorizontally;
			}
			Texture2D texture2D15 = Main.npcTexture[npc.type];
			Vector2 vector11 = new Vector2(texture2D15.Width / 2, texture2D15.Height / Main.npcFrameCount[npc.type] / 2);
			Color color = lightColor;
			Color color36 = Color.White;
			float amount9 = 0f;
			bool flag8 = npc.ai[0] > 4f;
			bool flag9 = npc.ai[0] > 9f && npc.ai[0] <= 12f;
			int num150 = 120;
			int num151 = 60;
			if (flag9)
			{
				color = CalamityGlobalNPC.buffColor(color, 0.4f, 0.8f, 0.4f, 1f);
			}
			else if (flag8)
			{
				color = CalamityGlobalNPC.buffColor(color, 0.5f, 0.7f, 0.5f, 1f);
			}
			else if (npc.ai[0] == 4f && npc.ai[2] > num150)
			{
				float num152 = npc.ai[2] - num150;
				num152 /= num151;
				color = CalamityGlobalNPC.buffColor(color, 1f - 0.5f * num152, 1f - 0.3f * num152, 1f - 0.5f * num152, 1f);
			}

			int num153 = 10;
			int num154 = 2;
			if (npc.ai[0] == -1f)
			{
				num153 = 0;
			}
			if (npc.ai[0] == 0f || npc.ai[0] == 5f || npc.ai[0] == 10f || npc.ai[0] == 12f)
			{
				num153 = 7;
			}
			if (npc.ai[0] == 1f || npc.ai[0] == 6f || npc.ai[0] > 9f)
			{
				color36 = Color.Lime;
				amount9 = 0.5f;
			}
			else
			{
				color = lightColor;
			}

			if (CalamityConfig.Instance.Afterimages)
			{
				for (int num155 = 1; num155 < num153; num155 += num154)
				{
					Color color38 = color;
					color38 = Color.Lerp(color38, color36, amount9);
					color38 = npc.GetAlpha(color38);
					color38 *= (num153 - num155) / 15f;
					Vector2 vector41 = npc.oldPos[num155] + new Vector2(npc.width, npc.height) / 2f - Main.screenPosition;
					vector41 -= new Vector2(texture2D15.Width, texture2D15.Height / Main.npcFrameCount[npc.type]) * npc.scale / 2f;
					vector41 += vector11 * npc.scale + new Vector2(0f, 4f + npc.gfxOffY);
					spriteBatch.Draw(texture2D15, vector41, npc.frame, color38, npc.rotation, vector11, npc.scale, spriteEffects, 0f);
				}
			}

			int num156 = 0;
			float num157 = 0f;
			float scaleFactor9 = 0f;

			if (npc.ai[0] == -1f)
			{
				num156 = 0;
			}

			if (npc.ai[0] == 3f || npc.ai[0] == 8f || npc.ai[0] == 13f)
			{
				int num158 = 60;
				int num159 = 30;
				if (npc.ai[2] > num158)
				{
					num156 = 6;
					num157 = 1f - (float)Math.Cos((npc.ai[2] - num158) / num159 * MathHelper.TwoPi);
					num157 /= 3f;
					scaleFactor9 = 40f;
				}
			}

			if ((npc.ai[0] == 4f || npc.ai[0] == 9f) && npc.ai[2] > num150)
			{
				num156 = 6;
				num157 = 1f - (float)Math.Cos((npc.ai[2] - num150) / num151 * MathHelper.TwoPi);
				num157 /= 3f;
				scaleFactor9 = 60f;
			}

			if (npc.ai[0] == 12f)
			{
				num156 = 6;
				num157 = 1f - (float)Math.Cos(npc.ai[2] / 30f * MathHelper.TwoPi);
				num157 /= 3f;
				scaleFactor9 = 20f;
			}

			if (CalamityConfig.Instance.Afterimages)
			{
				for (int num160 = 0; num160 < num156; num160++)
				{
					Color color39 = lightColor;
					color39 = Color.Lerp(color39, color36, amount9);
					color39 = npc.GetAlpha(color39);
					color39 *= 1f - num157;
					Vector2 vector42 = npc.Center + (num160 / (float)num156 * MathHelper.TwoPi + npc.rotation).ToRotationVector2() * scaleFactor9 * num157 - Main.screenPosition;
					vector42 -= new Vector2(texture2D15.Width, texture2D15.Height / Main.npcFrameCount[npc.type]) * npc.scale / 2f;
					vector42 += vector11 * npc.scale + new Vector2(0f, 4f + npc.gfxOffY);
					spriteBatch.Draw(texture2D15, vector42, npc.frame, color39, npc.rotation, vector11, npc.scale, spriteEffects, 0f);
				}
			}

			Color color2 = lightColor;
			color2 = Color.Lerp(color2, color36, amount9);
			color2 = npc.GetAlpha(color2);
			Vector2 vector43 = npc.Center - Main.screenPosition;
			vector43 -= new Vector2(texture2D15.Width, texture2D15.Height / Main.npcFrameCount[npc.type]) * npc.scale / 2f;
			vector43 += vector11 * npc.scale + new Vector2(0f, 4f + npc.gfxOffY);
			spriteBatch.Draw(texture2D15, vector43, npc.frame, (npc.ai[0] > 9f ? color2 : npc.GetAlpha(lightColor)), npc.rotation, vector11, npc.scale, spriteEffects, 0f);

			if (npc.ai[0] >= 4f && npc.Calamity().newAI[1] != 1f)
			{
				texture2D15 = ModContent.GetTexture("CalamityMod/NPCs/OldDuke/OldDukeGlow");
				Color color40 = Color.Lerp(Color.White, Color.Yellow, 0.5f);
				color36 = Color.Yellow;

				amount9 = 1f;
				num157 = 0.5f;
				scaleFactor9 = 10f;
				num154 = 1;

				if (npc.ai[0] == 4f || npc.ai[0] == 9f)
				{
					float num161 = npc.ai[2] - num150;
					num161 /= num151;
					color36 *= num161;
					color40 *= num161;
				}

				if (npc.ai[0] == 12f)
				{
					float num162 = npc.ai[2];
					num162 /= 30f;
					if (num162 > 0.5f)
					{
						num162 = 1f - num162;
					}
					num162 *= 2f;
					num162 = 1f - num162;
					color36 *= num162;
					color40 *= num162;
				}

				if (CalamityConfig.Instance.Afterimages)
				{
					for (int num163 = 1; num163 < num153; num163 += num154)
					{
						Color color41 = color40;
						color41 = Color.Lerp(color41, color36, amount9);
						color41 *= (num153 - num163) / 15f;
						Vector2 vector44 = npc.oldPos[num163] + new Vector2(npc.width, npc.height) / 2f - Main.screenPosition;
						vector44 -= new Vector2(texture2D15.Width, texture2D15.Height / Main.npcFrameCount[npc.type]) * npc.scale / 2f;
						vector44 += vector11 * npc.scale + new Vector2(0f, 4f + npc.gfxOffY);
						spriteBatch.Draw(texture2D15, vector44, npc.frame, color41, npc.rotation, vector11, npc.scale, spriteEffects, 0f);
					}

					for (int num164 = 1; num164 < num156; num164++)
					{
						Color color42 = color40;
						color42 = Color.Lerp(color42, color36, amount9);
						color42 = npc.GetAlpha(color42);
						color42 *= 1f - num157;
						Vector2 vector45 = npc.Center + (num164 / (float)num156 * MathHelper.TwoPi + npc.rotation).ToRotationVector2() * scaleFactor9 * num157 - Main.screenPosition;
						vector45 -= new Vector2(texture2D15.Width, texture2D15.Height / Main.npcFrameCount[npc.type]) * npc.scale / 2f;
						vector45 += vector11 * npc.scale + new Vector2(0f, 4f + npc.gfxOffY);
						spriteBatch.Draw(texture2D15, vector45, npc.frame, color42, npc.rotation, vector11, npc.scale, spriteEffects, 0f);
					}
				}

				spriteBatch.Draw(texture2D15, vector43, npc.frame, color40, npc.rotation, vector11, npc.scale, spriteEffects, 0f);
			}

			return false;
		}

		public override void BossLoot(ref string name, ref int potionType)
        {
            potionType = ModContent.ItemType<SupremeHealingPotion>();
        }

        public override void NPCLoot()
        {
            DropHelper.DropBags(npc);

            DropHelper.DropItemChance(npc, ModContent.ItemType<OldDukeTrophy>(), 10);
            DropHelper.DropItemCondition(npc, ModContent.ItemType<KnowledgeOldDuke>(), true, !CalamityWorld.downedBoomerDuke);
            DropHelper.DropResidentEvilAmmo(npc, CalamityWorld.downedBoomerDuke, 6, 3, 2);

			CalamityGlobalTownNPC.SetNewShopVariable(new int[] { ModContent.NPCType<SEAHOE>() }, CalamityWorld.downedBoomerDuke);

			// All other drops are contained in the bag, so they only drop directly on Normal
			if (!Main.expertMode)
            {
				// Weapons
				float w = DropHelper.DirectWeaponDropRateFloat;
				DropHelper.DropEntireWeightedSet(npc,
					DropHelper.WeightStack<InsidiousImpaler>(w),
					DropHelper.WeightStack<FetidEmesis>(w),
					DropHelper.WeightStack<SepticSkewer>(w),
					DropHelper.WeightStack<VitriolicViper>(w),
					DropHelper.WeightStack<CadaverousCarrion>(w),
					DropHelper.WeightStack<ToxicantTwister>(w)
				);

				//Equipment
				DropHelper.DropItemChance(npc, ModContent.ItemType<DukeScales>(), 10);

                // Vanity
                DropHelper.DropItemChance(npc, ModContent.ItemType<OldDukeMask>(), 7);
            }

            // Mark Old Duke as dead
            CalamityWorld.downedBoomerDuke = true;

			// Mark first acid rain encounter as true even if he wasn't fought in the acid rain, because it makes sense
			CalamityWorld.encounteredOldDuke = true;
            CalamityNetcode.SyncWorld();
        }

		public override bool CanHitPlayer(Player target, ref int cooldownSlot)
		{
			cooldownSlot = 1;
			return true;
		}

		public override void OnHitPlayer(Player player, int damage, bool crit)
		{
			player.AddBuff(BuffID.Venom, 300, true);
            player.AddBuff(BuffID.Rabies, 300, true);
            player.AddBuff(BuffID.Poisoned, 300, true);
			player.AddBuff(ModContent.BuffType<Irradiated>(), 300);
		}
		
		public override void HitEffect(int hitDirection, double damage)
		{
			if (npc.life > 0)
			{
				int num211 = 0;
				while (num211 < damage / npc.lifeMax * 100.0)
				{
					Dust.NewDust(npc.position, npc.width, npc.height, (int)CalamityDusts.SulfurousSeaAcid, hitDirection, -1f, 0, default, 1f);
					num211++;
				}
			}
			else
			{
				for (int num212 = 0; num212 < 150; num212++)
				{
					Dust.NewDust(npc.position, npc.width, npc.height, (int)CalamityDusts.SulfurousSeaAcid, 2 * hitDirection, -2f, 0, default, 1f);
				}

				Gore.NewGore(npc.Center + Vector2.UnitX * 20f * npc.direction, npc.velocity, mod.GetGoreSlot("Gores/OldDuke/OldDukeGore"), npc.scale);
				Gore.NewGore(npc.Center + Vector2.UnitX * 20f * npc.direction, npc.velocity, mod.GetGoreSlot("Gores/OldDuke/OldDukeGore2"), npc.scale);
				Gore.NewGore(npc.Center - Vector2.UnitX * 20f * npc.direction, npc.velocity, mod.GetGoreSlot("Gores/OldDuke/OldDukeGore3"), npc.scale);
				Gore.NewGore(npc.Center - Vector2.UnitX * 20f * npc.direction, npc.velocity, mod.GetGoreSlot("Gores/OldDuke/OldDukeGore4"), npc.scale);
			}
		}
	}
}
