using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Items.Placeables.Banners;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Projectiles.Enemy;
using Microsoft.Xna.Framework;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
namespace CalamityMod.NPCs.NormalNPCs
{
    public class Horse : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Earth Elemental");
            Main.npcFrameCount[NPC.type] = 6;
        }

        public override void SetDefaults()
        {
            NPC.Calamity().canBreakPlayerDefense = true;
            NPC.npcSlots = 3f;
            NPC.damage = 50;
            NPC.width = 230;
            NPC.height = 230;
            NPC.defense = 20;
            NPC.DR_NERD(0.1f);
            NPC.lifeMax = 3800;
            NPC.aiStyle = -1;
            AIType = -1;
            NPC.knockBackResist = 0f;
            NPC.value = Item.buyPrice(0, 1, 50, 0);
            NPC.dontTakeDamage = true;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.HitSound = SoundID.NPCHit4;
            NPC.rarity = 2;
            Banner = NPC.type;
            BannerItem = ModContent.ItemType<EarthElementalBanner>();
            NPC.Calamity().VulnerableToSickness = false;
            NPC.Calamity().VulnerableToWater = true;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(NPC.dontTakeDamage);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            NPC.dontTakeDamage = reader.ReadBoolean();
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.playerSafe || !Main.hardMode || spawnInfo.Player.Calamity().ZoneAbyss ||
                spawnInfo.Player.Calamity().ZoneSunkenSea || NPC.AnyNPCs(NPC.type))
            {
                return 0f;
            }
            return SpawnCondition.Cavern.Chance * 0.005f;
        }

        public override void FindFrame(int frameHeight)
        {
            if (NPC.ai[0] == 0f)
                return;

            NPC.frameCounter++;
            if (NPC.frameCounter >= 8)
            {
                NPC.frame.Y = (NPC.frame.Y + frameHeight) % (Main.npcFrameCount[NPC.type] * frameHeight);
                NPC.frameCounter = 0;
            }
        }

        public override void NPCLoot()
        {
            // Weapons
            float w = DropHelper.BagWeaponDropRateFloat;
            DropHelper.DropEntireWeightedSet(NPC,
                DropHelper.WeightStack<SlagMagnum>(w),
                DropHelper.WeightStack<Aftershock>(w),
                DropHelper.WeightStack<EarthenPike>(w)
            );
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, 31, hitDirection, -1f, 0, default, 1f);
            }
            if (NPC.life <= 0)
            {
                SoundEngine.PlaySound(SoundID.Item14, NPC.position);
                NPC.position.X = NPC.position.X + (NPC.width / 2);
                NPC.position.Y = NPC.position.Y + (NPC.height / 2);
                NPC.width = 160;
                NPC.height = 160;
                NPC.position.X = NPC.position.X - (NPC.width / 2);
                NPC.position.Y = NPC.position.Y - (NPC.height / 2);
                for (int num621 = 0; num621 < 40; num621++)
                {
                    int num622 = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, 31, 0f, 0f, 100, default, 2f);
                    Main.dust[num622].velocity *= 3f;
                    if (Main.rand.NextBool(2))
                    {
                        Main.dust[num622].scale = 0.5f;
                        Main.dust[num622].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                    }
                }
                for (int num623 = 0; num623 < 70; num623++)
                {
                    int num624 = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, DustID.Fire, 0f, 0f, 100, default, 3f);
                    Main.dust[num624].noGravity = true;
                    Main.dust[num624].velocity *= 5f;
                    num624 = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, DustID.Fire, 0f, 0f, 100, default, 2f);
                    Main.dust[num624].velocity *= 2f;
                }
                CalamityUtils.ExplosionGores(NPC.Center, 3);
            }
        }

        public override bool PreAI()
        {
            // Get a target
            if (NPC.target < 0 || NPC.target == Main.maxPlayers || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
                NPC.TargetClosest();

            if (Vector2.Distance(NPC.Center, Main.player[NPC.target].Center) < 480f)
            {
                if (NPC.ai[0] == 0f)
                {
                    NPC.ai[0] = 1f;
                    NPC.dontTakeDamage = false;
                }
            }
            else
                NPC.TargetClosest();

            if (NPC.ai[0] == 0f)
                return false;

            if (Main.player[NPC.target].dead || !Main.player[NPC.target].active)
            {
                if (NPC.velocity.Y < -2f)
                    NPC.velocity.Y = -2f;
                NPC.velocity.Y += 0.1f;
                if (NPC.velocity.Y > 12f)
                    NPC.velocity.Y = 12f;

                if (NPC.timeLeft > 60)
                    NPC.timeLeft = 60;
            }

            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                NPC.localAI[0] += 1f;
                if (NPC.localAI[0] >= 300f)
                {
                    NPC.localAI[0] = 0f;
                    SoundEngine.PlaySound(SoundID.NPCHit43, NPC.Center);
                    NPC.TargetClosest();
                    if (Collision.CanHit(NPC.position, NPC.width, NPC.height, Main.player[NPC.target].position, Main.player[NPC.target].width, Main.player[NPC.target].height))
                    {
                        float num179 = 4f;
                        Vector2 value9 = new Vector2(NPC.position.X + NPC.width * 0.5f, NPC.position.Y + NPC.height * 0.5f);
                        float num180 = Main.player[NPC.target].position.X + Main.player[NPC.target].width * 0.5f - value9.X;
                        float num181 = Math.Abs(num180) * 0.1f;
                        float num182 = Main.player[NPC.target].position.Y + Main.player[NPC.target].height * 0.5f - value9.Y - num181;
                        float num183 = (float)Math.Sqrt(num180 * num180 + num182 * num182);
                        num183 = num179 / num183;
                        num180 *= num183;
                        num182 *= num183;
                        int num184 = 30;
                        int num185 = ModContent.ProjectileType<EarthRockSmall>();
                        value9.X += num180;
                        value9.Y += num182;
                        for (int num186 = 0; num186 < 4; num186++)
                        {
                            num185 = Main.rand.NextBool(4) ? ModContent.ProjectileType<EarthRockBig>() : ModContent.ProjectileType<EarthRockSmall>();
                            num180 = Main.player[NPC.target].position.X + Main.player[NPC.target].width * 0.5f - value9.X;
                            num182 = Main.player[NPC.target].position.Y + Main.player[NPC.target].height * 0.5f - value9.Y;
                            num183 = (float)Math.Sqrt(num180 * num180 + num182 * num182);
                            num183 = num179 / num183;
                            num180 += Main.rand.Next(-40, 41);
                            num182 += Main.rand.Next(-40, 41);
                            num180 *= num183;
                            num182 *= num183;
                            Projectile.NewProjectile(value9.X, value9.Y, num180, num182, num185, num184, 0f, Main.myPlayer, 0f, 0f);
                        }
                    }
                }
            }

            float playerLocation = NPC.Center.X - Main.player[NPC.target].Center.X;
            NPC.direction = playerLocation < 0 ? 1 : -1;
            NPC.spriteDirection = NPC.direction;

            Vector2 direction = Main.player[NPC.target].Center - NPC.Center;
            direction.Normalize();
            NPC.ai[1] += Main.expertMode ? 2f : 1f;
            if (NPC.ai[1] >= 600f)
            {
                direction *= 6f;
                NPC.velocity = direction;
                NPC.ai[1] = 0f;
            }

            if (Math.Sqrt((NPC.velocity.X * NPC.velocity.X) + (NPC.velocity.Y * NPC.velocity.Y)) > 1)
                NPC.velocity *= 0.985f;

            if (Math.Sqrt((NPC.velocity.X * NPC.velocity.X) + (NPC.velocity.Y * NPC.velocity.Y)) <= 1 * 1.15)
                NPC.velocity = direction * 1;

            return false;
        }

        public override void OnHitPlayer(Player player, int damage, bool crit)
        {
            player.AddBuff(ModContent.BuffType<ArmorCrunch>(), 180);
        }
    }
}
