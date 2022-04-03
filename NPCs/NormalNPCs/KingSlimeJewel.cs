using CalamityMod.Events;
using CalamityMod.Projectiles.Boss;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.NPCs.NormalNPCs
{
    public class KingSlimeJewel : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Crown Jewel");
        }

        public override void SetDefaults()
        {
            NPC.aiStyle = -1;
            AIType = -1;
            NPC.damage = 0;
            NPC.width = 22;
            NPC.height = 22;
            NPC.defense = 10;
            NPC.DR_NERD(0.1f);
            NPC.lifeMax = 280;
            NPC.knockBackResist = 0f;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.canGhostHeal = false;
            NPC.HitSound = SoundID.NPCHit5;
            NPC.DeathSound = SoundID.NPCDeath15;
            NPC.Calamity().VulnerableToSickness = false;
        }

        public override void AI()
        {
            // Red light
            Lighting.AddLight((int)((NPC.position.X + (float)(NPC.width / 2)) / 16f), (int)((NPC.position.Y + (float)(NPC.height / 2)) / 16f), 1f, 0f, 0f);

            // Despawn
            if (!NPC.AnyNPCs(NPCID.KingSlime))
            {
                NPC.active = false;
                NPC.netUpdate = true;
                return;
            }

            // Float around the player
            NPC.rotation = NPC.velocity.X / 15f;

            NPC.TargetClosest(true);

            float velocity = 2f;
            float acceleration = 0.1f;

            if (NPC.position.Y > Main.player[NPC.target].position.Y - 350f)
            {
                if (NPC.velocity.Y > 0f)
                    NPC.velocity.Y *= 0.98f;

                NPC.velocity.Y -= acceleration;

                if (NPC.velocity.Y > velocity)
                    NPC.velocity.Y = velocity;
            }
            else if (NPC.position.Y < Main.player[NPC.target].position.Y - 400f)
            {
                if (NPC.velocity.Y < 0f)
                    NPC.velocity.Y *= 0.98f;

                NPC.velocity.Y += acceleration;

                if (NPC.velocity.Y < -velocity)
                    NPC.velocity.Y = -velocity;
            }

            if (NPC.Center.X > Main.player[NPC.target].Center.X + 100f)
            {
                if (NPC.velocity.X > 0f)
                    NPC.velocity.X *= 0.98f;

                NPC.velocity.X -= acceleration;

                if (NPC.velocity.X > 8f)
                    NPC.velocity.X = 8f;
            }
            if (NPC.Center.X < Main.player[NPC.target].Center.X - 100f)
            {
                if (NPC.velocity.X < 0f)
                    NPC.velocity.X *= 0.98f;

                NPC.velocity.X += acceleration;

                if (NPC.velocity.X < -8f)
                    NPC.velocity.X = -8f;
            }

            // Fire projectiles
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                // Fire bolt every 1.5 seconds
                NPC.localAI[0] += 1f;
                if (NPC.localAI[0] >= ((CalamityWorld.malice || BossRushEvent.BossRushActive) ? 45f : CalamityWorld.death ? 60f : 75f))
                {
                    NPC.localAI[0] = 0f;

                    Vector2 npcPos = new Vector2(NPC.Center.X, NPC.Center.Y);
                    float xDist = Main.player[NPC.target].Center.X - npcPos.X;
                    float yDist = Main.player[NPC.target].Center.Y - npcPos.Y;
                    Vector2 projVector = new Vector2(xDist, yDist);
                    float projLength = projVector.Length();

                    float speed = 9f;
                    int type = ModContent.ProjectileType<JewelProjectile>();

                    projLength = speed / projLength;
                    projVector.X *= projLength;
                    projVector.Y *= projLength;
                    npcPos.X += projVector.X * 2f;
                    npcPos.Y += projVector.Y * 2f;

                    for (int dusty = 0; dusty < 10; dusty++)
                    {
                        Vector2 dustVel = projVector;
                        dustVel.Normalize();
                        int ruby = Dust.NewDust(NPC.Center, NPC.width, NPC.height, 90, dustVel.X, dustVel.Y, 100, default, 2f);
                        Main.dust[ruby].velocity *= 1.5f;
                        Main.dust[ruby].noGravity = true;
                        if (Main.rand.NextBool(2))
                        {
                            Main.dust[ruby].scale = 0.5f;
                            Main.dust[ruby].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                        }
                    }

                    SoundEngine.PlaySound(SoundID.Item8, NPC.position);
                    int damage = NPC.GetProjectileDamage(type);
                    if (CalamityWorld.death || BossRushEvent.BossRushActive)
                    {
                        int numProj = 2;
                        float rotation = MathHelper.ToRadians(9);
                        for (int i = 0; i < numProj + 1; i++)
                        {
                            Vector2 perturbedSpeed = projVector.RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (numProj - 1)));
                            Projectile.NewProjectile(npcPos, perturbedSpeed, type, damage, 0f, Main.myPlayer, 0f, 0f);
                        }
                    }
                    else
                        Projectile.NewProjectile(npcPos, projVector, type, damage, 0f, Main.myPlayer, 0f, 0f);
                }
            }
        }

        public override Color? GetAlpha(Color drawColor)
        {
            return new Color(255, 50, 50, 0);
        }

        public override bool CheckActive()
        {
            return false;
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            Dust.NewDust(NPC.position, NPC.width, NPC.height, 90, hitDirection, -1f, 0, default, 1f);
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
                    int num622 = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, 90, 0f, 0f, 100, default, 2f);
                    Main.dust[num622].velocity *= 3f;
                    if (Main.rand.NextBool(2))
                    {
                        Main.dust[num622].scale = 0.5f;
                        Main.dust[num622].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                    }
                }
                for (int num623 = 0; num623 < 10; num623++)
                {
                    int num624 = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, 90, 0f, 0f, 100, default, 3f);
                    Main.dust[num624].noGravity = true;
                    Main.dust[num624].velocity *= 5f;
                    num624 = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, 90, 0f, 0f, 100, default, 2f);
                    Main.dust[num624].velocity *= 2f;
                }
            }
        }
    }
}
