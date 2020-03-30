using CalamityMod.Dusts;
using CalamityMod.Items.Placeables.Banners;
using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Enemy;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Buffs.StatDebuffs;
namespace CalamityMod.NPCs.AcidRain
{
    public class Trilobite : ModNPC
    {
        // When the abs(velocity) is less than this, lunge in the water
        public const float MinSpeedLungePrompt = 0.5f;
        public const float MinYDriftSpeed = 0.9f;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Trilobite");
            Main.npcFrameCount[npc.type] = 8;
        }

        public override void SetDefaults()
        {
            npc.width = 36;
            npc.height = 38;
            npc.aiStyle = aiType = -1;

            npc.damage = 40;
            npc.lifeMax = 360;
            npc.defense = 15;

            if (CalamityWorld.downedPolterghast)
            {
                npc.damage = 160;
                npc.lifeMax = 7200;
                npc.defense = 78;
                npc.Calamity().DR = 0.4f;
            }
            else if (CalamityWorld.downedAquaticScourge)
            {
                npc.damage = 66;
                npc.lifeMax = 850;
                npc.Calamity().DR = 0.25f;
            }

            npc.knockBackResist = 0f;
            npc.value = Item.buyPrice(0, 0, 4, 0);
            for (int k = 0; k < npc.buffImmune.Length; k++)
            {
                npc.buffImmune[k] = true;
            }
            npc.lavaImmune = false;
            npc.noGravity = true;
            npc.noTileCollide = false;
            npc.HitSound = SoundID.NPCHit42;
            npc.DeathSound = SoundID.NPCDeath27;
            banner = npc.type;
            bannerItem = ModContent.ItemType<TrilobiteBanner>();
        }
        public override void AI()
        {
            npc.TargetClosest(false);
            if (!npc.wet)
            {
                if (npc.ai[0] > 0)
                    npc.ai[0]--;
                npc.rotation += npc.velocity.X * 0.1f;
                if (npc.velocity.Y == 0f)
                {
                    npc.velocity.X *= 0.99f;
                    if (Math.Abs(npc.velocity.X) < 0.01f)
                    {
                        npc.velocity.X = 0f;
                    }
                    npc.netUpdate = true;
                }
                npc.velocity.Y += 0.3f;
                if (npc.velocity.Y > 13f)
                {
                    npc.velocity.Y = 13f;
                    npc.netUpdate = true;
                }
            }
            else
            {
                Player player = Main.player[npc.target];
                if (npc.velocity.Length() < MinSpeedLungePrompt)
                {
                    npc.TargetClosest(true);
                    float speed = 15f;
                    if (CalamityWorld.downedPolterghast)
                    {
                        speed = 28.5f;
                    }

                    npc.velocity = npc.DirectionTo(player.Center) * speed;
                    npc.velocity.X *= 1.6f;
                    npc.rotation = npc.velocity.ToRotation() + MathHelper.PiOver2;
                    npc.netUpdate = true;
                }
                else
                {
                    if (Math.Abs(npc.velocity.X) < 20f)
                    {
                        npc.velocity.X += npc.direction * 0.02f;
                    }
                    npc.rotation = npc.velocity.X * 0.4f;
                    if (Math.Abs(npc.velocity.Y) < MinYDriftSpeed)
                    {
                        npc.velocity.X *= 0.96f;
                    }
                    else if (Math.Abs(npc.velocity.X) < 3.5f)
                    {
                        float speedX = 18f;
                        float speedY = 9f;
                        if (CalamityWorld.downedPolterghast)
                        {
                            speedX = 27f;
                            speedY = 25f;
                        }
                        npc.velocity = npc.DirectionTo(player.Center) * new Vector2(speedX, speedY);
                        npc.netUpdate = true;
                    }
                    npc.velocity.Y *= 0.98f;
                }
            }
        }
        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            npc.lifeMax = (int)(npc.lifeMax * 0.8f * bossLifeScale);
            npc.damage = (int)(npc.damage * 0.85f);
        }
        public override void NPCLoot()
        {
            DropHelper.DropItemCondition(npc, ModContent.ItemType<CorrodedFossil>(), CalamityWorld.downedAquaticScourge && Main.rand.NextBool(3 * (CalamityWorld.downedPolterghast ? 5 : 1)), 1, 3);
        }
        public override void ModifyHitByProjectile(Projectile projectile, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                if (npc.ai[0] <= 0f)
                {
                    Main.PlaySound(SoundID.NPCDeath11, npc.Center);
                    int projDamage = CalamityWorld.downedPolterghast ? 35 : CalamityWorld.downedAquaticScourge ? 29 : 21;
                    Projectile.NewProjectile(npc.Center + Utils.NextVector2Unit(Main.rand) * npc.Size * 0.7f,
                        -npc.velocity.RotatedByRandom(MathHelper.ToRadians(10f)), ModContent.ProjectileType<TrilobiteSpike>(),
                        projDamage, 3f);
                    npc.ai[0] = Main.rand.Next(50, 65);
                    npc.netUpdate = true;
                }
            }
        }
        public override void FindFrame(int frameHeight)
        {
            npc.frameCounter++;
            if (npc.frameCounter >= 4)
            {
                npc.frameCounter = 0;
                npc.frame.Y += frameHeight;
                if (npc.frame.Y >= Main.npcFrameCount[npc.type] * frameHeight)
                {
                    npc.frame.Y = 0;
                }
            }
        }
        public override void HitEffect(int hitDirection, double damage)
        {
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(npc.position, npc.width, npc.height, (int)CalamityDusts.SulfurousSeaAcid, hitDirection, -1f, 0, default, 1f);
            }
            if (npc.life <= 0)
            {
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/AcidRain/TrilobiteGore"), npc.scale);
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/AcidRain/TrilobiteGore2"), npc.scale);
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/AcidRain/TrilobiteGore3"), npc.scale);
                for (int k = 0; k < 30; k++)
                {
                    Dust.NewDust(npc.position, npc.width, npc.height, (int)CalamityDusts.SulfurousSeaAcid, hitDirection, -1f, 0, default, 1f);
                }
            }
        }
        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.AddBuff(ModContent.BuffType<Irradiated>(), 180);
        }
    }
}
