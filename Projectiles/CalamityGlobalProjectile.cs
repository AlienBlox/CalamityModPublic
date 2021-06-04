using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.CalPlayer;
using CalamityMod.Events;
using CalamityMod.NPCs.NormalNPCs;
using CalamityMod.Projectiles.Boss;
using CalamityMod.Projectiles.Melee;
using CalamityMod.Projectiles.Melee.Yoyos;
using CalamityMod.Projectiles.Rogue;
using CalamityMod.Projectiles.Summon;
using CalamityMod.Projectiles.Typeless;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace CalamityMod.Projectiles
{
	public class CalamityGlobalProjectile : GlobalProjectile
    {
        public override bool InstancePerEntity
        {
            get
            {
                return true;
            }
        }

        // Class Types
        public bool rogue = false;
        public bool trueMelee = false;

        // Force Class Types
        public bool forceMelee = false;
        public bool forceRanged = false;
        public bool forceMagic = false;
        public bool forceMinion = false;
        public bool forceRogue = false;
        public bool forceTypeless = false;
        public bool forceHostile = false;

        // Damage Adjusters
        private bool setDamageValues = true;
        public float spawnedPlayerMinionDamageValue = 1f;
        public float ResistDamagePenaltyHarshness = 1f;
        public float ResistDamagePenaltyMinCapFactor = 0.5f;
        public int spawnedPlayerMinionProjectileDamageValue = 0;
        public int defDamage = 0;

		// Amount of extra updates that are set in SetDefaults.
		public int defExtraUpdates = -1;

		/// <summary>
		/// Allows hostile Projectiles to deal damage to the player's defense stat, used mostly for hard-hitting bosses.
		/// </summary>
		public bool canBreakPlayerDefense = false;

		// Rogue Stuff
		public bool stealthStrike = false; // Update all existing rogue weapons with this
        public bool momentumCapacitatorBoost = false; // Constant acceleration

        // Iron Heart
        public int ironHeartDamage = 0;

        // Counters and Timers
        public int stealthStrikeHitCount = 0;

        public int lineColor = 0; // Note: Although this was intended for fishing line colors, I use this as an AI variable a lot because vanilla only has 4 that sometimes are already in use.  ~Ben
        public bool extorterBoost = false;

        // Organic/Inorganic Boosts
        public bool hasOrganicEnemyHitBoost = false;
        public bool hasInorganicEnemyHitBoost = false;
        public float organicEnemyHitBoost = 0f;
        public float inorganicEnemyHitBoost = 0f;
        public Action<NPC> organicEnemyHitEffect = null;
        public Action<NPC> inorganicEnemyHitEffect = null;

        // Dogshit, hacky workarounds for the summon respawning system
        public bool RequiresManualResurrection = false;

        public bool overridesMinionDamagePrevention = false;

		public static List<int> MechBossProjectileIDs = new List<int>
		{
			ProjectileID.DeathLaser,
			ProjectileID.PinkLaser,
			ProjectileID.BombSkeletronPrime,
			ProjectileID.CursedFlameHostile,
			ProjectileID.EyeFire,
			ProjectileID.EyeLaser,
			ProjectileID.Skull,
			ProjectileID.SaucerMissile,
			ProjectileID.RocketSkeleton,
			ProjectileType<DestroyerCursedLaser>(),
			ProjectileType<DestroyerElectricLaser>(),
			ProjectileType<ShadowflameFireball>(),
			ProjectileType<Shadowflamethrower>(),
			ProjectileType<ScavengerLaser>()
		};

		// Boss projectile velocity multiplier in Malice Mode
		public bool affectedByMaliceModeVelocityMultiplier = false;
		public const float MaliceModeProjectileVelocityMultiplier = 1.25f;

        // Enchantment variables.
        public int ExplosiveEnchantCountdown = 0;
        public const int ExplosiveEnchantTime = 600;

        #region SetDefaults
        public override void SetDefaults(Projectile projectile)
        {
            if (CalamityLists.trueMeleeProjectileList.Contains(projectile.type))
                trueMelee = true;

            switch (projectile.type)
            {
				case ProjectileID.FlamingJack:
					projectile.extraUpdates = 1;
					break;

                case ProjectileID.ShadowBeamHostile:
                    projectile.timeLeft = 60;
                    break;

                case ProjectileID.LostSoulHostile:
                    projectile.tileCollide = true;
                    break;

                case ProjectileID.NebulaLaser:
                    projectile.extraUpdates = 1;
                    break;

                case ProjectileID.StarWrath:
                    projectile.penetrate = projectile.maxPenetrate = 1;
                    break;

                case ProjectileID.Retanimini:
                case ProjectileID.MiniRetinaLaser:
				case ProjectileID.FlowerPetal:
                    projectile.localNPCHitCooldown = 10;
                    projectile.usesLocalNPCImmunity = true;
                    projectile.usesIDStaticNPCImmunity = false;
                    break;

                case ProjectileID.Spazmamini:
                    projectile.usesIDStaticNPCImmunity = true;
                    projectile.idStaticNPCHitCooldown = 12;
                    break;

				case ProjectileID.DD2BetsyFireball:
				case ProjectileID.DD2BetsyFlameBreath:
				case ProjectileID.CultistBossLightningOrbArc:
				case ProjectileID.InfernoHostileBlast:
				case ProjectileID.RocketSkeleton:
				case ProjectileID.SniperBullet:
				case ProjectileID.RuneBlast:
				case ProjectileID.UnholyTridentHostile:
				case ProjectileID.JavelinHostile:
				case ProjectileID.FrostWave:
				case ProjectileID.Present:
				case ProjectileID.FlamingScythe:
				case ProjectileID.SaucerDeathray:
				case ProjectileID.CannonballHostile:
				case ProjectileID.PaladinsHammerHostile:
				case ProjectileID.Spike:
				case ProjectileID.Sharknado:
				case ProjectileID.Cthulunado:
				case ProjectileID.PhantasmalSphere:
				case ProjectileID.PhantasmalDeathray:
					canBreakPlayerDefense = true;
					break;

				case ProjectileID.Stinger:
				case ProjectileID.Shadowflames:
				case ProjectileID.DeathLaser:
				case ProjectileID.PinkLaser:
				case ProjectileID.CursedFlameHostile:
				case ProjectileID.EyeFire:
				case ProjectileID.EyeLaser:
				case ProjectileID.PoisonSeedPlantera:
				case ProjectileID.SeedPlantera:
				case ProjectileID.Fireball:
				case ProjectileID.EyeBeam:
				case ProjectileID.InfernoHostileBolt:
				case ProjectileID.CultistBossLightningOrb:
				case ProjectileID.AncientDoomProjectile:
				case ProjectileID.PhantasmalBolt:
				case ProjectileID.PhantasmalEye:
					affectedByMaliceModeVelocityMultiplier = true;
					break;

				case ProjectileID.DemonSickle:
				case ProjectileID.BombSkeletronPrime:
				case ProjectileID.Skull:
				case ProjectileID.SaucerMissile:
				case ProjectileID.ThornBall:
				case ProjectileID.CultistBossFireBall:
				case ProjectileID.CultistBossFireBallClone:
				case ProjectileID.CultistBossIceMist:
					canBreakPlayerDefense = affectedByMaliceModeVelocityMultiplier = true;
					break;

				default:
                    break;
            }

			if (projectile.type >= ProjectileID.BlackCounterweight && projectile.type <= ProjectileID.YellowCounterweight)
			{
				projectile.MaxUpdates = 2;
				projectile.usesIDStaticNPCImmunity = true;
				projectile.idStaticNPCHitCooldown = 10;
			}

			// Disable Lunatic Cultist's homing resistance globally
			ProjectileID.Sets.Homing[projectile.type] = false;
        }
        #endregion

        #region PreAI
        public override bool PreAI(Projectile projectile)
        {
            if (projectile.minion && ExplosiveEnchantCountdown > 0)
			{
                ExplosiveEnchantCountdown--;

                // Make fizzle sounds and fire dust to indicate the impending explosion.
                if (ExplosiveEnchantCountdown <= 300)
				{
                    if (Main.rand.NextBool(24))
                        Main.PlaySound(SoundID.DD2_BetsyFireballShot, projectile.Center);

                    Dust fire = Dust.NewDustPerfect(projectile.Center + Main.rand.NextVector2Circular(projectile.width, projectile.height) * 0.42f, 267);
                    fire.color = Color.Lerp(Color.Orange, Color.Red, Main.rand.NextFloat(0.45f, 1f));
                    fire.scale = Main.rand.NextFloat(1.4f, 1.65f);
                    fire.fadeIn = 0.5f;
                    fire.noGravity = true;
				}

                if (ExplosiveEnchantCountdown <= 0)
				{
                    Main.PlaySound(SoundID.DD2_KoboldExplosion, projectile.Center);
                    if (Main.myPlayer == projectile.owner)
                    {
                        if (projectile.minionSlots > 0f)
                        {
                            int damage = (int)(Main.player[projectile.owner].MinionDamage() * 6000);
                            Projectile.NewProjectile(projectile.Center, Vector2.Zero, ProjectileType<SummonBrimstoneExplosion>(), damage, 0f, projectile.owner);
                        }
                        projectile.Kill();
                    }
				}
            }

            if (RequiresManualResurrection)
            {
                // Reactivate the projectile the instant it's created. This is dirty as fuck, but
                // I can't find the offending Kill call in the frankly enormous codebase that causes this unusual instant-death behavior.
                projectile.active = true;
                projectile.timeLeft = 90000;
                RequiresManualResurrection = false;
            }

            if (projectile.type == ProjectileID.Starfury)
            {
                if (projectile.timeLeft > 45)
                    projectile.timeLeft = 45;

                if (projectile.ai[1] == 0f && !Collision.SolidCollision(projectile.position, projectile.width, projectile.height))
                {
                    projectile.ai[1] = 1f;
                    projectile.netUpdate = true;
                }

                if (projectile.soundDelay == 0)
                {
                    projectile.soundDelay = 20 + Main.rand.Next(40);
                    Main.PlaySound(SoundID.Item9, projectile.position);
                }

                if (projectile.localAI[0] == 0f)
                    projectile.localAI[0] = 1f;

                projectile.alpha += (int)(25f * projectile.localAI[0]);
                if (projectile.alpha > 200)
                {
                    projectile.alpha = 200;
                    projectile.localAI[0] = -1f;
                }
                if (projectile.alpha < 0)
                {
                    projectile.alpha = 0;
                    projectile.localAI[0] = 1f;
                }

                projectile.rotation += (Math.Abs(projectile.velocity.X) + Math.Abs(projectile.velocity.Y)) * 0.01f * projectile.direction;

                if (projectile.ai[1] == 1f)
                {
                    projectile.light = 0.9f;

                    if (Main.rand.NextBool(10))
                        Dust.NewDust(projectile.position, projectile.width, projectile.height, 58, projectile.velocity.X * 0.5f, projectile.velocity.Y * 0.5f, 150, default, 1.2f);

                    if (Main.rand.NextBool(20))
                        Gore.NewGore(projectile.position, new Vector2(projectile.velocity.X * 0.2f, projectile.velocity.Y * 0.2f), Main.rand.Next(16, 18), 1f);
                }

                return false;
            }

            if (projectile.type == ProjectileID.NurseSyringeHeal)
            {
                ref float initialSpeed = ref projectile.localAI[1];
                if (initialSpeed == 0f)
                    initialSpeed = projectile.velocity.Length();

                bool invalidHealTarget = !Main.npc.IndexInRange((int)projectile.ai[0]) || !Main.npc[(int)projectile.ai[0]].active || !Main.npc[(int)projectile.ai[0]].townNPC;
                if (invalidHealTarget)
                {
                    projectile.Kill();
                    return false;
                }

                NPC npcToHeal = Main.npc[(int)projectile.ai[0]];

                // If the needle is not colliding with the target, attempt to move towards it while falling.
                if (!projectile.WithinRange(npcToHeal.Center, initialSpeed) && !projectile.Hitbox.Intersects(npcToHeal.Hitbox))
                {
                    Vector2 flySpeed = projectile.SafeDirectionTo(npcToHeal.Center) * initialSpeed;

                    // Prevent the needle from ever violating its gravity.
                    if (flySpeed.Y < projectile.velocity.Y)
                        flySpeed.Y = projectile.velocity.Y;

                    flySpeed.Y++;

                    projectile.velocity = Vector2.Lerp(projectile.velocity, flySpeed, 0.04f);
                    projectile.rotation += projectile.velocity.X * 0.05f;
                    return false;
                }

                // Otherwise, die immediately and heal the target.
                projectile.Kill();

                int healAmount = npcToHeal.lifeMax - npcToHeal.life;
                int maxHealAmount = 20;

                // If the target has more than 250 max life, incorporate their total life into the max amount to heal.
                // This is done so that more powerful NPCs, such as Cirrus, do not take an eternity to receive meaningful healing benefits
                // from the Nurse.
                if (npcToHeal.lifeMax > 250)
                    maxHealAmount = (int)Math.Max(maxHealAmount, npcToHeal.lifeMax * 0.05f);

                if (healAmount > maxHealAmount)
                    healAmount = maxHealAmount;

                if (healAmount > 0)
                {
                    npcToHeal.life += healAmount;
                    npcToHeal.HealEffect(healAmount, true);
                    return false;
                }

                return false;
			}

            if (CalamityWorld.revenge || BossRushEvent.BossRushActive || CalamityWorld.malice)
            {
				if (projectile.type == ProjectileID.DemonSickle && CalamityPlayer.areThereAnyDamnBosses)
				{
					if (projectile.ai[0] == 0f)
						Main.PlaySound(SoundID.Item8, projectile.position);

					projectile.rotation += projectile.direction * 0.8f;

					projectile.ai[0] += 1f;
					if (projectile.velocity.Length() < projectile.ai[1])
					{
						if (!(projectile.ai[0] < 30f))
						{
							if (projectile.ai[0] < 100f)
								projectile.velocity *= 1.06f;
							else
								projectile.ai[0] = 200f;
						}
					}

					for (int i = 0; i < 2; i++)
					{
						int dust = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 27, 0f, 0f, 100);
						Main.dust[dust].noGravity = true;
					}

					return false;
				}

                else if (projectile.type == ProjectileID.EyeLaser && projectile.ai[0] == 1f)
                {
                    projectile.rotation = (float)Math.Atan2(projectile.velocity.Y, projectile.velocity.X) + MathHelper.PiOver2;

                    Lighting.AddLight(projectile.Center, (255 - projectile.alpha) * 0.3f / 255f, 0f, (255 - projectile.alpha) * 0.3f / 255f);

                    if (projectile.alpha > 0)
                        projectile.alpha -= 125;
                    if (projectile.alpha < 0)
                        projectile.alpha = 0;

                    if (projectile.localAI[1] == 0f)
                    {
                        Main.PlaySound(SoundID.Item33, (int)projectile.position.X, (int)projectile.position.Y);
                        projectile.localAI[1] = 1f;
                    }

                    if (projectile.velocity.Length() < 18f)
                        projectile.velocity *= 1.0025f;

                    return false;
                }

                else if (projectile.type == ProjectileID.DeathLaser && projectile.ai[0] == 1f)
                {
					if (defDamage == 0)
					{
						// Reduce mech boss projectile damage depending on the new ore progression changes
						if (CalamityConfig.Instance.EarlyHardmodeProgressionRework)
						{
							if (!NPC.downedMechBossAny)
								projectile.damage = (int)(projectile.damage * 0.8);
							else if ((!NPC.downedMechBoss1 && !NPC.downedMechBoss2) || (!NPC.downedMechBoss2 && !NPC.downedMechBoss3) || (!NPC.downedMechBoss3 && !NPC.downedMechBoss1))
								projectile.damage = (int)(projectile.damage * 0.9);
						}

						defDamage = projectile.damage;
					}

					projectile.rotation = (float)Math.Atan2(projectile.velocity.Y, projectile.velocity.X) + MathHelper.PiOver2;

                    Lighting.AddLight(projectile.Center, (255 - projectile.alpha) * 0.75f / 255f, 0f, 0f);

                    if (projectile.alpha > 0)
                        projectile.alpha -= 125;
                    if (projectile.alpha < 0)
                        projectile.alpha = 0;

                    if (projectile.localAI[1] == 0f)
                    {
                        Main.PlaySound(SoundID.Item33, (int)projectile.position.X, (int)projectile.position.Y);
                        projectile.localAI[1] = 1f;
                    }

                    if (projectile.velocity.Length() < 12f)
                        projectile.velocity *= 1.0025f;

                    return false;
                }

                else if (projectile.type == ProjectileID.PoisonSeedPlantera)
                {
                    projectile.frameCounter++;
                    if (projectile.frameCounter > 1)
                    {
                        projectile.frameCounter = 0;
                        projectile.frame++;

                        if (projectile.frame > 1)
                            projectile.frame = 0;
                    }

                    if (projectile.ai[1] == 0f)
                    {
                        projectile.ai[1] = 1f;
                        Main.PlaySound(SoundID.Item17, projectile.position);
                    }

                    if (projectile.alpha > 0)
                        projectile.alpha -= 30;
                    if (projectile.alpha < 0)
                        projectile.alpha = 0;

                    projectile.ai[0] += 1f;
                    if (projectile.ai[0] >= 120f)
                    {
                        projectile.ai[0] = 120f;

                        if (projectile.velocity.Length() < 18f)
                            projectile.velocity *= 1.01f;
                    }

                    projectile.tileCollide = false;

                    if (projectile.timeLeft > 600)
                        projectile.timeLeft = 600;

                    projectile.rotation = (float)Math.Atan2(projectile.velocity.Y, projectile.velocity.X) + 1.57f;

                    return false;
                }

                else if (projectile.type == ProjectileID.ThornBall)
                {
                    if (projectile.alpha > 0)
                    {
                        projectile.alpha -= 30;
                        if (projectile.alpha < 0)
                            projectile.alpha = 0;
                    }

                    int num147 = Player.FindClosest(projectile.Center, 1, 1);
                    float num146 = 7.5f * projectile.ai[1] + Vector2.Distance(Main.player[num147].Center, projectile.Center) * 0.01f;
                    Vector2 vector12 = Main.player[num147].Center - projectile.Center;
                    vector12.Normalize();
                    vector12 *= num146;
                    int num148 = 200;
                    projectile.velocity.X = (projectile.velocity.X * (num148 - 1) + vector12.X) / num148;

                    if (projectile.velocity.Length() > 16f)
                    {
                        projectile.velocity.Normalize();
                        projectile.velocity *= 16f;
                    }

                    projectile.ai[0] += 1f;
                    if (projectile.ai[0] > 15f)
                    {
                        if (projectile.velocity.Y == 0f && projectile.velocity.X != 0f)
                        {
                            projectile.velocity.X *= 0.97f;
                            if (projectile.velocity.X > -0.01f && projectile.velocity.X < 0.01f)
                                projectile.Kill();
                        }
                        projectile.velocity.Y += 0.1f;
                    }

                    projectile.rotation += projectile.velocity.X * 0.05f;

                    if (projectile.velocity.Y > 16f)
                        projectile.velocity.Y = 16f;

                    return false;
                }

                // Phase 1 sharknado
                else if (projectile.type == ProjectileID.SharknadoBolt)
                {
                    if (projectile.ai[1] < 0f)
                    {
                        float num623 = 0.209439516f;
                        float num624 = -2f;
                        float num625 = (float)(Math.Cos(num623 * projectile.ai[0]) - 0.5) * num624;

                        projectile.velocity.Y -= num625;

                        projectile.ai[0] += 1f;

                        num625 = (float)(Math.Cos(num623 * projectile.ai[0]) - 0.5) * num624;

                        projectile.velocity.Y += num625;

                        projectile.localAI[0] += 1f;
                        if (projectile.localAI[0] > 10f)
                        {
                            projectile.alpha -= 5;
                            if (projectile.alpha < 100)
                                projectile.alpha = 100;

                            projectile.rotation += projectile.velocity.X * 0.1f;
                            projectile.frame = (int)(projectile.localAI[0] / 3f) % 3;
                        }

                        return false;
                    }
                }

                else if (projectile.type == ProjectileID.Sharknado)
                {
                    projectile.damage = projectile.GetProjectileDamage(NPCID.DukeFishron);
                }

                // Larger cthulhunadoes
                else if (projectile.type == ProjectileID.Cthulunado)
                {
                    projectile.damage = projectile.GetProjectileDamage(NPCID.DukeFishron);

                    int num606 = 16;
                    int num607 = 16;
                    float num608 = 2f;
                    int num609 = 150;
                    int num610 = 42;

                    if (projectile.velocity.X != 0f)
                        projectile.direction = projectile.spriteDirection = -Math.Sign(projectile.velocity.X);

                    int num3 = projectile.frameCounter;
                    projectile.frameCounter = num3 + 1;
                    if (projectile.frameCounter > 2)
                    {
                        num3 = projectile.frame;
                        projectile.frame = num3 + 1;
                        projectile.frameCounter = 0;
                    }
                    if (projectile.frame >= 6)
                        projectile.frame = 0;

                    if (projectile.localAI[0] == 0f && Main.myPlayer == projectile.owner)
                    {
                        projectile.localAI[0] = 1f;
                        projectile.position.X += projectile.width / 2;
                        projectile.position.Y += projectile.height / 2;
                        projectile.scale = (num606 + num607 - projectile.ai[1]) * num608 / (num607 + num606);
                        projectile.width = (int)(num609 * projectile.scale);
                        projectile.height = (int)(num610 * projectile.scale);
                        projectile.position.X -= projectile.width / 2;
                        projectile.position.Y -= projectile.height / 2;
                        projectile.netUpdate = true;
                    }

                    if (projectile.ai[1] != -1f)
                    {
                        projectile.scale = (num606 + num607 - projectile.ai[1]) * num608 / (num607 + num606);
                        projectile.width = (int)(num609 * projectile.scale);
                        projectile.height = (int)(num610 * projectile.scale);
                    }

                    if (!Collision.SolidCollision(projectile.position, projectile.width, projectile.height))
                    {
                        projectile.alpha -= 30;
                        if (projectile.alpha < 60)
                            projectile.alpha = 60;
                        if (projectile.alpha < 100)
                            projectile.alpha = 100;
                    }
                    else
                    {
                        projectile.alpha += 30;
                        if (projectile.alpha > 150)
                            projectile.alpha = 150;
                    }

                    if (projectile.ai[0] > 0f)
                        projectile.ai[0] -= 1f;

                    if (projectile.ai[0] == 1f && projectile.ai[1] > 0f && projectile.owner == Main.myPlayer)
                    {
                        projectile.netUpdate = true;

                        Vector2 center = projectile.Center;
                        center.Y -= num610 * projectile.scale / 2f;

                        float num611 = (num606 + num607 - projectile.ai[1] + 1f) * num608 / (num607 + num606);
                        center.Y -= num610 * num611 / 2f;
                        center.Y += 2f;

                        Projectile.NewProjectile(center.X, center.Y, projectile.velocity.X, projectile.velocity.Y, projectile.type, projectile.damage, projectile.knockBack, projectile.owner, 10f, projectile.ai[1] - 1f);

                        if ((int)projectile.ai[1] % 3 == 0 && projectile.ai[1] != 0f)
                        {
                            int num614 = NPC.NewNPC((int)center.X, (int)center.Y, NPCID.Sharkron2);
                            Main.npc[num614].velocity = projectile.velocity;
                            Main.npc[num614].scale = 1.5f;
                            Main.npc[num614].netUpdate = true;
                            Main.npc[num614].ai[2] = projectile.width;
                            Main.npc[num614].ai[3] = -1.5f;
                        }
                    }

                    if (projectile.ai[0] <= 0f)
                    {
                        float num615 = 0.104719758f;
                        float num616 = projectile.width / 5f * 2.5f;
                        float num617 = (float)(Math.Cos(num615 * -(double)projectile.ai[0]) - 0.5) * num616;

                        projectile.position.X -= num617 * -projectile.direction;

                        projectile.ai[0] -= 1f;

                        num617 = (float)(Math.Cos(num615 * -(double)projectile.ai[0]) - 0.5) * num616;
                        projectile.position.X += num617 * -projectile.direction;
                    }

                    return false;
                }

				else if (projectile.type == ProjectileID.CultistBossLightningOrb)
				{
					if (NPC.AnyNPCs(NPCID.CultistBoss))
					{
						if (projectile.localAI[1] == 0f)
						{
							Main.PlaySound(SoundID.Item121, projectile.position);
							projectile.localAI[1] = 1f;
						}

						if (projectile.ai[0] < 180f)
						{
							projectile.alpha -= 5;
							if (projectile.alpha < 0)
								projectile.alpha = 0;
						}
						else
						{
							projectile.alpha += 5;
							if (projectile.alpha > 255)
							{
								projectile.alpha = 255;
								projectile.Kill();
								return false;
							}
						}

						ref float reference = ref projectile.ai[0];
						ref float reference46 = ref reference;
						float num15 = reference;
						reference46 = num15 + 1f;

						if (projectile.ai[0] % 30f == 0f && projectile.ai[0] < 180f && Main.netMode != NetmodeID.MultiplayerClient)
						{
							int[] array6 = new int[5];
							Vector2[] array7 = new Vector2[5];
							int num731 = 0;
							float num732 = 2000f;

							for (int num733 = 0; num733 < 255; num733++)
							{
								if (!Main.player[num733].active || Main.player[num733].dead)
									continue;

								Vector2 center9 = Main.player[num733].Center;
								float num734 = Vector2.Distance(center9, projectile.Center);
								if (num734 < num732 && Collision.CanHit(projectile.Center, 1, 1, center9, 1, 1))
								{
									array6[num731] = num733;
									array7[num731] = center9;
									int num34 = num731 + 1;
									num731 = num34;
									if (num34 >= array7.Length)
										break;
								}
							}

							for (int num735 = 0; num735 < num731; num735++)
							{
								Vector2 vector52 = array7[num735] + Main.player[array6[num735]].velocity * 40f - projectile.Center;
								float ai = Main.rand.Next(100);
								Vector2 vector53 = Vector2.Normalize(vector52.RotatedByRandom(MathHelper.PiOver4)) * 7f;
								Projectile.NewProjectile(projectile.Center, vector53, 466, projectile.damage, 0f, Main.myPlayer, vector52.ToRotation(), ai);
							}
						}

						Lighting.AddLight(projectile.Center, 0.4f, 0.85f, 0.9f);

						if (++projectile.frameCounter >= 4)
						{
							projectile.frameCounter = 0;
							if (++projectile.frame >= Main.projFrames[projectile.type])
								projectile.frame = 0;
						}

						if (projectile.alpha >= 150 || !(projectile.ai[0] < 180f))
							return false;

						for (int num736 = 0; num736 < 1; num736++)
						{
							float num737 = (float)Main.rand.NextDouble() * 1f - 0.5f;
							if (num737 < -0.5f)
								num737 = -0.5f;
							if (num737 > 0.5f)
								num737 = 0.5f;

							Vector2 value40 = new Vector2(-projectile.width * 0.2f * projectile.scale, 0f).RotatedBy(num737 * ((float)Math.PI * 2f)).RotatedBy(projectile.velocity.ToRotation());
							int num738 = Dust.NewDust(projectile.Center - Vector2.One * 5f, 10, 10, 226, (0f - projectile.velocity.X) / 3f, (0f - projectile.velocity.Y) / 3f, 150, Color.Transparent, 0.7f);
							Main.dust[num738].position = projectile.Center + value40;
							Main.dust[num738].velocity = Vector2.Normalize(Main.dust[num738].position - projectile.Center) * 2f;
							Main.dust[num738].noGravity = true;
						}

						for (int num739 = 0; num739 < 1; num739++)
						{
							float num740 = (float)Main.rand.NextDouble() * 1f - 0.5f;
							if (num740 < -0.5f)
								num740 = -0.5f;
							if (num740 > 0.5f)
								num740 = 0.5f;

							Vector2 value41 = new Vector2(-projectile.width * 0.6f * projectile.scale, 0f).RotatedBy(num740 * ((float)Math.PI * 2f)).RotatedBy(projectile.velocity.ToRotation());
							int num741 = Dust.NewDust(projectile.Center - Vector2.One * 5f, 10, 10, 226, (0f - projectile.velocity.X) / 3f, (0f - projectile.velocity.Y) / 3f, 150, Color.Transparent, 0.7f);
							Main.dust[num741].velocity = Vector2.Zero;
							Main.dust[num741].position = projectile.Center + value41;
							Main.dust[num741].noGravity = true;
						}

						return false;
					}
				}

				else if (projectile.type == ProjectileID.CultistBossIceMist)
				{
					if (NPC.AnyNPCs(NPCID.CultistBoss))
					{
						if (projectile.localAI[1] == 0f)
						{
							projectile.localAI[1] = 1f;
							Main.PlaySound(SoundID.Item120, projectile.position);
						}

						projectile.ai[0] += 1f;

						// Main projectile
						float duration = 300f;
						if (projectile.ai[1] == 1f)
						{
							if (projectile.ai[0] >= duration - 20f)
								projectile.alpha += 10;
							else
								projectile.alpha -= 10;

							if (projectile.alpha < 0)
								projectile.alpha = 0;
							if (projectile.alpha > 255)
								projectile.alpha = 255;

							if (projectile.ai[0] >= duration)
							{
								projectile.Kill();
								return false;
							}

							int num103 = Player.FindClosest(projectile.Center, 1, 1);
							Vector2 vector11 = Main.player[num103].Center - projectile.Center;
							float scaleFactor2 = projectile.velocity.Length();
							vector11.Normalize();
							vector11 *= scaleFactor2;
							projectile.velocity = (projectile.velocity * 15f + vector11) / 16f;
							projectile.velocity.Normalize();
							projectile.velocity *= scaleFactor2;

							if (projectile.ai[0] % 60f == 0f && Main.netMode != NetmodeID.MultiplayerClient)
							{
								Vector2 vector50 = projectile.rotation.ToRotationVector2();
								Projectile.NewProjectile(projectile.Center, vector50, projectile.type, projectile.damage, projectile.knockBack, projectile.owner);
							}

							projectile.rotation += (float)Math.PI / 30f;

							Lighting.AddLight(projectile.Center, 0.3f, 0.75f, 0.9f);

							return false;
						}

						// Split projectiles
						projectile.position -= projectile.velocity;

						if (projectile.ai[0] >= duration - 260f)
							projectile.alpha += 3;
						else
							projectile.alpha -= 40;

						if (projectile.alpha < 0)
							projectile.alpha = 0;
						if (projectile.alpha > 255)
							projectile.alpha = 255;

						if (projectile.ai[0] >= duration - 255f)
						{
							projectile.Kill();
							return false;
						}

						Vector2 value39 = new Vector2(0f, -720f).RotatedBy(projectile.velocity.ToRotation());
						float scaleFactor3 = projectile.ai[0] % (duration - 255f) / (duration - 255f);
						Vector2 spinningpoint13 = value39 * scaleFactor3;

						for (int num724 = 0; num724 < 6; num724++)
						{
							Vector2 vector51 = projectile.Center + spinningpoint13.RotatedBy(num724 * ((float)Math.PI * 2f) / 6f);

							Lighting.AddLight(vector51, 0.3f, 0.75f, 0.9f);

							for (int num725 = 0; num725 < 2; num725++)
							{
								int num726 = Dust.NewDust(vector51 + Utils.RandomVector2(Main.rand, -8f, 8f) / 2f, 8, 8, 197, 0f, 0f, 100, Color.Transparent);
								Main.dust[num726].noGravity = true;
							}
						}

						return false;
					}
				}

                // Change the stupid homing eyes
                else if (projectile.type == ProjectileID.PhantasmalEye)
                {
                    projectile.alpha -= 40;
                    if (projectile.alpha < 0)
                        projectile.alpha = 0;

                    if (projectile.ai[0] == 0f)
                    {
                        projectile.localAI[0] += 1f;
                        if (projectile.localAI[0] >= 45f)
                        {
                            projectile.localAI[0] = 0f;
                            projectile.ai[0] = 1f;
                            projectile.ai[1] = 0f - projectile.ai[1];
                            projectile.netUpdate = true;
                        }

                        projectile.velocity.X = projectile.velocity.RotatedBy(projectile.ai[1]).X;
                        projectile.velocity.X = MathHelper.Clamp(projectile.velocity.X, -6f, 6f);
                        projectile.velocity.Y -= 0.08f;

                        if (projectile.velocity.Y > 0f)
                            projectile.velocity.Y -= 0.2f;
                        if (projectile.velocity.Y < -7f)
                            projectile.velocity.Y = -7f;
                    }
                    else if (projectile.ai[0] == 1f)
                    {
                        projectile.localAI[0] += 1f;
                        if (projectile.localAI[0] >= 90f)
                        {
                            projectile.localAI[0] = 0f;
                            projectile.ai[0] = 2f;
                            projectile.ai[1] = Player.FindClosest(projectile.position, projectile.width, projectile.height);
                            projectile.netUpdate = true;
                        }

                        projectile.velocity.X = projectile.velocity.RotatedBy(projectile.ai[1]).X;
                        projectile.velocity.X = MathHelper.Clamp(projectile.velocity.X, -6f, 6f);
                        projectile.velocity.Y -= 0.08f;

                        if (projectile.velocity.Y > 0f)
                            projectile.velocity.Y -= 0.2f;
                        if (projectile.velocity.Y < -7f)
                            projectile.velocity.Y = -7f;
                    }
                    else if (projectile.ai[0] == 2f)
                    {
                        projectile.localAI[0] += 1f;
                        if (projectile.localAI[0] >= 45f)
                        {
                            projectile.localAI[0] = 0f;
                            projectile.ai[0] = 3f;
                            projectile.netUpdate = true;
                        }

                        Vector2 value23 = Main.player[(int)projectile.ai[1]].Center - projectile.Center;
                        value23.Normalize();
                        value23 *= 12f;
                        value23 = Vector2.Lerp(projectile.velocity, value23, 0.6f);

                        float num675 = 0.4f;
                        if (projectile.velocity.X < value23.X)
                        {
                            projectile.velocity.X += num675;
                            if (projectile.velocity.X < 0f && value23.X > 0f)
                                projectile.velocity.X += num675;
                        }
                        else if (projectile.velocity.X > value23.X)
                        {
                            projectile.velocity.X -= num675;
                            if (projectile.velocity.X > 0f && value23.X < 0f)
                                projectile.velocity.X -= num675;
                        }
                        if (projectile.velocity.Y < value23.Y)
                        {
                            projectile.velocity.Y += num675;
                            if (projectile.velocity.Y < 0f && value23.Y > 0f)
                                projectile.velocity.Y += num675;
                        }
                        else if (projectile.velocity.Y > value23.Y)
                        {
                            projectile.velocity.Y -= num675;
                            if (projectile.velocity.Y > 0f && value23.Y < 0f)
                                projectile.velocity.Y -= num675;
                        }
                    }
                    else if (projectile.ai[0] == 3f)
                    {
                        Vector2 value23 = Main.player[(int)projectile.ai[1]].Center - projectile.Center;
                        if (value23.Length() < 30f)
                        {
                            projectile.Kill();
                            return false;
                        }

                        float velocityLimit = ((CalamityWorld.death || BossRushEvent.BossRushActive || CalamityWorld.malice) ? 28f : 24f) / MathHelper.Clamp(lineColor * 0.75f, 1f, 3f);
                        if (projectile.velocity.Length() < velocityLimit)
                            projectile.velocity *= 1.01f;
                    }

                    if (projectile.alpha < 40)
                    {
                        int num676 = Dust.NewDust(projectile.Center - Vector2.One * 5f, 10, 10, 229, (0f - projectile.velocity.X) / 3f, (0f - projectile.velocity.Y) / 3f, 150, Color.Transparent, 1.2f);
                        Main.dust[num676].noGravity = true;
                    }

                    projectile.rotation = projectile.velocity.ToRotation() + MathHelper.PiOver2;

                    return false;
                }

                // Moon Lord big eye spheres
                else if (projectile.type == ProjectileID.PhantasmalSphere && Main.npc[(int)projectile.ai[1]].type == NPCID.MoonLordHand)
                {
                    float velocityLimit = (CalamityWorld.death || BossRushEvent.BossRushActive || CalamityWorld.malice) ? 14f : 12f;
                    if (projectile.velocity.Length() < velocityLimit)
                        projectile.velocity *= 1.0075f;

                    return true;
                }

                // Moon Lord Deathray
                else if (projectile.type == ProjectileID.PhantasmalDeathray)
                {
                    if (Main.npc[(int)projectile.ai[1]].type == NPCID.MoonLordHead)
                    {
                        Vector2? vector78 = null;

                        if (projectile.velocity.HasNaNs() || projectile.velocity == Vector2.Zero)
                            projectile.velocity = -Vector2.UnitY;

                        if (Main.npc[(int)projectile.ai[1]].active)
                        {
                            Vector2 value21 = new Vector2(27f, 59f);
                            Vector2 value22 = Utils.Vector2FromElipse(Main.npc[(int)projectile.ai[1]].localAI[0].ToRotationVector2(), value21 * Main.npc[(int)projectile.ai[1]].localAI[1]);
                            projectile.position = Main.npc[(int)projectile.ai[1]].Center + value22 - new Vector2(projectile.width, projectile.height) / 2f;
                        }

                        if (projectile.velocity.HasNaNs() || projectile.velocity == Vector2.Zero)
                            projectile.velocity = -Vector2.UnitY;

                        if (projectile.localAI[0] == 0f)
                            Main.PlaySound(SoundID.Zombie, (int)projectile.position.X, (int)projectile.position.Y, 104, 1f, 0f);

                        float num801 = 1f;
                        projectile.localAI[0] += 1f;
                        if (projectile.localAI[0] >= 180f)
                        {
                            projectile.Kill();
                            return false;
                        }

                        projectile.scale = (float)Math.Sin(projectile.localAI[0] * MathHelper.Pi / 180f) * 10f * num801;
                        if (projectile.scale > num801)
                            projectile.scale = num801;

                        float num804 = projectile.velocity.ToRotation();
                        num804 += projectile.ai[0];
                        projectile.rotation = num804 - MathHelper.PiOver2;
                        projectile.velocity = num804.ToRotationVector2();

                        float num805 = 3f;
                        float num806 = projectile.width;

                        Vector2 samplingPoint = projectile.Center;
                        if (vector78.HasValue)
                        {
                            samplingPoint = vector78.Value;
                        }

                        float[] array3 = new float[(int)num805];
                        Collision.LaserScan(samplingPoint, projectile.velocity, num806 * projectile.scale, 2400f, array3);
                        float num807 = 0f;
                        int num3;
                        for (int num808 = 0; num808 < array3.Length; num808 = num3 + 1)
                        {
                            num807 += array3[num808];
                            num3 = num808;
                        }
                        num807 /= num805;

                        // Fire laser through walls at max length if target cannot be seen
                        if (!Collision.CanHitLine(Main.npc[(int)projectile.ai[1]].Center, 1, 1, Main.player[Main.npc[(int)projectile.ai[1]].target].Center, 1, 1) &&
                            Main.npc[(int)projectile.ai[1]].Calamity().newAI[0] == 1f)
                        {
                            num807 = 2400f;
                        }

                        float amount = 0.5f;
                        projectile.localAI[1] = MathHelper.Lerp(projectile.localAI[1], num807, amount);

                        Vector2 vector79 = projectile.Center + projectile.velocity * (projectile.localAI[1] - 14f);
                        for (int num809 = 0; num809 < 2; num809 = num3 + 1)
                        {
                            float num810 = projectile.velocity.ToRotation() + ((Main.rand.Next(2) == 1) ? -1f : 1f) * MathHelper.PiOver2;
                            float num811 = (float)Main.rand.NextDouble() * 2f + 2f;
                            Vector2 vector80 = new Vector2((float)Math.Cos(num810) * num811, (float)Math.Sin(num810) * num811);
                            int num812 = Dust.NewDust(vector79, 0, 0, 229, vector80.X, vector80.Y, 0, default, 1f);
                            Main.dust[num812].noGravity = true;
                            Main.dust[num812].scale = 1.7f;
                            num3 = num809;
                        }

                        if (Main.rand.Next(5) == 0)
                        {
                            Vector2 value29 = projectile.velocity.RotatedBy(MathHelper.PiOver2) * ((float)Main.rand.NextDouble() - 0.5f) * projectile.width;
                            int num813 = Dust.NewDust(vector79 + value29 - Vector2.One * 4f, 8, 8, 31, 0f, 0f, 100, default, 1.5f);
                            Dust dust = Main.dust[num813];
                            dust.velocity *= 0.5f;
                            Main.dust[num813].velocity.Y = -Math.Abs(Main.dust[num813].velocity.Y);
                        }

                        DelegateMethods.v3_1 = new Vector3(0.3f, 0.65f, 0.7f);
                        Utils.PlotTileLine(projectile.Center, projectile.Center + projectile.velocity * projectile.localAI[1], projectile.width * projectile.scale, new Utils.PerLinePoint(DelegateMethods.CastLight));

                        return false;
                    }
                }
            }

            if (CalamityWorld.death && !CalamityPlayer.areThereAnyDamnBosses)
            {
                if (projectile.type == ProjectileID.Sharknado || projectile.type == ProjectileID.Cthulunado)
                {
                    int num520 = 10;
                    int num521 = 15;
                    float num522 = 1f;
                    int num523 = 150;
                    int num524 = 42;
                    if (projectile.type == ProjectileID.Cthulunado)
                    {
                        num520 = 16;
                        num521 = 16;
                        num522 = 1.5f;
                    }
                    if (projectile.velocity.X != 0f)
                    {
                        projectile.direction = projectile.spriteDirection = -Math.Sign(projectile.velocity.X);
                    }
                    projectile.frameCounter++;
                    if (projectile.frameCounter > 2)
                    {
                        projectile.frame++;
                        projectile.frameCounter = 0;
                    }
                    if (projectile.frame >= 6)
                    {
                        projectile.frame = 0;
                    }
                    if (projectile.localAI[0] == 0f && Main.myPlayer == projectile.owner)
                    {
                        projectile.localAI[0] = 1f;
                        projectile.position.X += projectile.width / 2;
                        projectile.position.Y += projectile.height / 2;
                        projectile.scale = (num520 + num521 - projectile.ai[1]) * num522 / (num521 + num520);
                        projectile.width = (int)(num523 * projectile.scale);
                        projectile.height = (int)(num524 * projectile.scale);
                        projectile.position.X -= projectile.width / 2;
                        projectile.position.Y -= projectile.height / 2;
                        projectile.netUpdate = true;
                    }
                    if (projectile.ai[1] != -1f)
                    {
                        projectile.scale = (num520 + num521 - projectile.ai[1]) * num522 / (num521 + num520);
                        projectile.width = (int)(num523 * projectile.scale);
                        projectile.height = (int)(num524 * projectile.scale);
                    }
                    if (!Collision.SolidCollision(projectile.position, projectile.width, projectile.height))
                    {
                        projectile.alpha -= 30;
                        if (projectile.alpha < 60)
                        {
                            projectile.alpha = 60;
                        }
                        if (projectile.type == ProjectileID.Cthulunado && projectile.alpha < 100)
                        {
                            projectile.alpha = 100;
                        }
                    }
                    else
                    {
                        projectile.alpha += 30;
                        if (projectile.alpha > 150)
                        {
                            projectile.alpha = 150;
                        }
                    }
                    if (projectile.ai[0] > 0f)
                    {
                        projectile.ai[0] -= 1f;
                    }
                    if (projectile.ai[0] == 1f && projectile.ai[1] > 0f && projectile.owner == Main.myPlayer)
                    {
                        projectile.netUpdate = true;
                        Vector2 center2 = projectile.Center;
                        center2.Y -= num524 * projectile.scale / 2f;
                        float num525 = (num520 + num521 - projectile.ai[1] + 1f) * num522 / (num521 + num520);
                        center2.Y -= num524 * num525 / 2f;
                        center2.Y += 2f;
                        Projectile.NewProjectile(center2, projectile.velocity, projectile.type, projectile.damage, projectile.knockBack, projectile.owner, 10f, projectile.ai[1] - 1f);
                    }
                    if (projectile.ai[0] <= 0f)
                    {
                        float num529 = (float)Math.PI / 30f;
                        float num530 = projectile.width / 5f;
                        if (projectile.type == ProjectileID.Cthulunado)
                        {
                            num530 *= 2f;
                        }
                        float num531 = (float)(Math.Cos(num529 * (-projectile.ai[0])) - 0.5) * num530;
                        projectile.position.X -= num531 * (-projectile.direction);
                        projectile.ai[0] -= 1f;
                        num531 = (float)(Math.Cos(num529 * (-projectile.ai[0])) - 0.5) * num530;
                        projectile.position.X += num531 * (-projectile.direction);
                    }

                    return false;
                }
            }

            return true;
        }
        #endregion

        #region AI
        public override void AI(Projectile projectile)
        {
            Player player = Main.player[projectile.owner];
            CalamityPlayer modPlayer = player.Calamity();

			if (defDamage == 0)
			{
				if (projectile.hostile)
				{
					if (CalamityPlayer.areThereAnyDamnBosses && affectedByMaliceModeVelocityMultiplier)
						projectile.velocity *= MaliceModeProjectileVelocityMultiplier;

					// Reduce Nail damage from Nailheads because they're stupid
					if (projectile.type == ProjectileID.Nail && Main.expertMode)
						projectile.damage /= 2;

					if ((CalamityLists.hardModeNerfList.Contains(projectile.type) && Main.hardMode && !CalamityPlayer.areThereAnyDamnBosses && !Main.snowMoon) || projectile.type == ProjectileID.JavelinHostile)
					{
						projectile.damage = (int)(projectile.damage * 0.65);
					}

					// Reduce mech boss projectile damage depending on the new ore progression changes
					if (CalamityConfig.Instance.EarlyHardmodeProgressionRework)
					{
						if (!NPC.downedMechBossAny)
						{
							if (MechBossProjectileIDs.Contains(projectile.type))
							{
								if (CalamityUtils.AnyBossNPCS(true))
									projectile.damage = (int)(projectile.damage * 0.8);
							}
						}
						else if ((!NPC.downedMechBoss1 && !NPC.downedMechBoss2) || (!NPC.downedMechBoss2 && !NPC.downedMechBoss3) || (!NPC.downedMechBoss3 && !NPC.downedMechBoss1))
						{
							if (MechBossProjectileIDs.Contains(projectile.type))
							{
								if (CalamityUtils.AnyBossNPCS(true))
									projectile.damage = (int)(projectile.damage * 0.9);
							}
						}
					}
				}
				else
				{
					if (modPlayer.camper && !player.StandingStill())
						projectile.damage = (int)(projectile.damage * 0.1);
				}

				defDamage = projectile.damage;
			}

			// Setting this in SetDefaults didn't work
			switch (projectile.type)
			{
				case ProjectileID.Bee:
				case ProjectileID.Wasp:
				case ProjectileID.TinyEater:
				case ProjectileID.GiantBee:
				case ProjectileID.Bat:
					projectile.extraUpdates = 1;
					break;
			}

			if (NPC.downedMoonlord)
            {
                if (CalamityLists.dungeonProjectileBuffList.Contains(projectile.type))
                {
                    //Prevents them being buffed in Skeletron, Skeletron Prime, or Golem fights
                    if (((projectile.type == ProjectileID.RocketSkeleton || projectile.type == ProjectileID.Shadowflames) && projectile.ai[1] == 1f) ||
                        (NPC.golemBoss > 0 && (projectile.type == ProjectileID.InfernoHostileBolt || projectile.type == ProjectileID.InfernoHostileBlast)))
                    {
                        projectile.damage = defDamage;
                    }
                    else
                        projectile.damage = defDamage + 30;
                }
            }
			
            if (CalamityWorld.downedDoG && (Main.pumpkinMoon || Main.snowMoon || Main.eclipse))
            {
                if (CalamityLists.eventProjectileBuffList.Contains(projectile.type))
                    projectile.damage = defDamage + 15;
            }

            // Iron Heart damage variable will scale with projectile.damage
            if (CalamityWorld.ironHeart)
            {
                ironHeartDamage = 0;
            }

            if (projectile.modProjectile != null && projectile.modProjectile.mod.Name.Equals("CalamityMod"))
                goto SKIP_CALAMITY;

            if ((projectile.minion || projectile.sentry) && !ProjectileID.Sets.StardustDragon[projectile.type]) //For all other mods and vanilla, exclude dragon due to bugs
            {
                if (setDamageValues)
                {
                    spawnedPlayerMinionDamageValue = player.MinionDamage();
                    spawnedPlayerMinionProjectileDamageValue = projectile.damage;
                    setDamageValues = false;
                }
                if (player.MinionDamage() != spawnedPlayerMinionDamageValue)
                {
                    int damage2 = (int)(spawnedPlayerMinionProjectileDamageValue / spawnedPlayerMinionDamageValue * player.MinionDamage());
                    projectile.damage = damage2;
                }
            }
            SKIP_CALAMITY:

            // If rogue projectiles are not internally throwing while in-flight, they can never critically strike.
            if (rogue)
                projectile.thrown = true;

            if (forceMelee)
            {
                projectile.hostile = false;
                projectile.friendly = true;
                projectile.melee = true;
                projectile.ranged = false;
                projectile.magic = false;
                projectile.minion = false;
                projectile.thrown = false;
                rogue = false;
            }
            else if (forceRanged)
            {
                projectile.hostile = false;
                projectile.friendly = true;
                projectile.melee = false;
                projectile.ranged = true;
                projectile.magic = false;
                projectile.minion = false;
                projectile.thrown = false;
                rogue = false;
            }
            else if (forceMagic)
            {
                projectile.hostile = false;
                projectile.friendly = true;
                projectile.melee = false;
                projectile.ranged = false;
                projectile.magic = true;
                projectile.minion = false;
                projectile.thrown = false;
                rogue = false;
            }
            else if (forceMinion)
            {
                projectile.hostile = false;
                projectile.friendly = true;
                projectile.melee = false;
                projectile.ranged = false;
                projectile.magic = false;
                projectile.minion = true;
                projectile.thrown = false;
                rogue = false;
            }
            else if (forceRogue)
            {
                projectile.hostile = false;
                projectile.friendly = true;
                projectile.melee = false;
                projectile.ranged = false;
                projectile.magic = false;
                projectile.minion = false;
                projectile.thrown = true;
                rogue = true;
            }
            else if (forceTypeless)
            {
                projectile.hostile = false;
                projectile.friendly = true;
                projectile.melee = false;
                projectile.ranged = false;
                projectile.magic = false;
                projectile.minion = false;
                projectile.thrown = false;
                rogue = false;
            }
            else if (forceHostile)
            {
                projectile.hostile = true;
                projectile.friendly = false;
                projectile.melee = false;
                projectile.ranged = false;
                projectile.magic = false;
                projectile.minion = false;
                projectile.thrown = false;
                rogue = false;
            }

            if (projectile.type == ProjectileID.GiantBee || projectile.type == ProjectileID.Bee)
            {
                if (projectile.timeLeft > 570) //all of these have a time left of 600 or 660
                {
                    if (player.ActiveItem().type == ItemID.BeesKnees)
                    {
                        projectile.magic = false;
                        projectile.ranged = true;
                    }
                }
            }
            else if (projectile.type == ProjectileID.SoulDrain)
                projectile.magic = true;

            if (projectile.type == ProjectileID.OrnamentFriendly && lineColor == 1) //spawned by Festive Wings
            {
                Vector2 center = projectile.Center;
                float maxDistance = 460f;
                bool homeIn = false;

                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    if (Main.npc[i].CanBeChasedBy(projectile, false))
                    {
                        float extraDistance = (float)(Main.npc[i].width / 2) + (Main.npc[i].height / 2);

                        bool canHit = Collision.CanHit(projectile.Center, 1, 1, Main.npc[i].Center, 1, 1);

                        if (Vector2.Distance(Main.npc[i].Center, projectile.Center) < (maxDistance + extraDistance) && canHit)
                        {
                            center = Main.npc[i].Center;
                            homeIn = true;
                            break;
                        }
                    }
                }

                if (homeIn)
                {
                    Vector2 moveDirection = projectile.SafeDirectionTo(center, Vector2.UnitY);
                    projectile.velocity = (projectile.velocity * 20f + moveDirection * 15f) / 21f;
                }
            }

            if (!projectile.npcProj && !projectile.trap && projectile.friendly && projectile.damage > 0)
            {
                if (modPlayer.fungalSymbiote && trueMelee)
                {
                    if (Main.player[projectile.owner].miscCounter % 6 == 0 && projectile.FinalExtraUpdate())
                    {
                        if (projectile.owner == Main.myPlayer && player.ownedProjectileCounts[ProjectileID.Mushroom] < 15)
                        {
                            //Note: these don't count as true melee anymore but its useful code to keep around
                            if (projectile.type == ProjectileType<NebulashFlail>() || projectile.type == ProjectileType<CosmicDischargeFlail>() ||
                                projectile.type == ProjectileType<MourningstarFlail>() || projectile.type == ProjectileID.SolarWhipSword)
                            {
                                Vector2 vector24 = Main.OffsetsPlayerOnhand[Main.player[projectile.owner].bodyFrame.Y / 56] * 2f;
                                if (player.direction != 1)
                                {
                                    vector24.X = player.bodyFrame.Width - vector24.X;
                                }
                                if (player.gravDir != 1f)
                                {
                                    vector24.Y = player.bodyFrame.Height - vector24.Y;
                                }
                                vector24 -= new Vector2(player.bodyFrame.Width - player.width, player.bodyFrame.Height - 42) / 2f;
                                Vector2 newCenter = player.RotatedRelativePoint(player.position + vector24, true) + projectile.velocity;
                                Projectile.NewProjectile(newCenter, Vector2.Zero, ProjectileID.Mushroom, (int)(projectile.damage * 0.15), 0f, projectile.owner);
                            }
                            else
                            {
                                Projectile.NewProjectile(projectile.Center, Vector2.Zero, ProjectileID.Mushroom, (int)(projectile.damage * 0.15), 0f, projectile.owner);
                            }
                        }
                    }
                }

                if (rogue)
                {
                    if (modPlayer.nanotech)
                    {
                        if (Main.player[projectile.owner].miscCounter % 30 == 0 && projectile.FinalExtraUpdate())
                        {
                            if (projectile.owner == Main.myPlayer && player.ownedProjectileCounts[ProjectileType<Nanotech>()] < 5)
                            {
                                Projectile.NewProjectile(projectile.Center, Vector2.Zero, ProjectileType<Nanotech>(), (int)(60 * player.RogueDamage()), 0f, projectile.owner);
                            }
                        }
                    }
                    else if (modPlayer.moonCrown)
                    {
						if (Main.player[projectile.owner].miscCounter % 120 == 0 && projectile.FinalExtraUpdate())
						{
							if (projectile.owner == Main.myPlayer && player.ownedProjectileCounts[ProjectileType<MoonSigil>()] < 5)
							{
								int proj = Projectile.NewProjectile(projectile.Center, Vector2.Zero, ProjectileType<MoonSigil>(), (int)(45 * player.RogueDamage()), 0f, projectile.owner);
								if (proj.WithinBounds(Main.maxProjectiles))
									Main.projectile[proj].Calamity().forceTypeless = true;
							}
						}
                    }

                    if (modPlayer.dragonScales)
                    {
                        if (Main.player[projectile.owner].miscCounter % 50 == 0 && projectile.FinalExtraUpdate())
                        {
                            if (projectile.owner == Main.myPlayer && player.ownedProjectileCounts[ProjectileType<DragonShit>()] < 5)
                            {
                                int proj = Projectile.NewProjectile(projectile.Center, Vector2.One.RotatedByRandom(MathHelper.TwoPi), ProjectileType<DragonShit>(),
									(int)(80 * player.RogueDamage()), 0f, projectile.owner);
								if (proj.WithinBounds(Main.maxProjectiles))
									Main.projectile[proj].Calamity().forceTypeless = true;
							}
                        }
                    }

                    if (modPlayer.daedalusSplit)
                    {
                        if (Main.player[projectile.owner].miscCounter % 30 == 0 && projectile.FinalExtraUpdate())
                        {
                            if (projectile.owner == Main.myPlayer && player.ownedProjectileCounts[ProjectileID.CrystalShard] < 30)
                            {
                                for (int i = 0; i < 2; i++)
                                {
                                    Vector2 velocity = CalamityUtils.RandomVelocity(100f, 70f, 100f);
                                    int shard = Projectile.NewProjectile(projectile.Center, velocity, ProjectileID.CrystalShard, CalamityUtils.DamageSoftCap(projectile.damage * 0.25, 30), 0f, projectile.owner);
									if (shard.WithinBounds(Main.maxProjectiles))
										Main.projectile[shard].Calamity().forceTypeless = true;
                                }
                            }
                        }
                    }

                    // Will always be friendly and rogue if it has this boost
                    if (modPlayer.momentumCapacitor && momentumCapacitatorBoost)
                    {
                        if (projectile.velocity.Length() < 26f)
                            projectile.velocity *= 1.05f;
                    }

                    if (player.meleeEnchant > 0 && !projectile.noEnchantments)
                    {
                        switch (player.meleeEnchant)
                        {
                            case 1:
                                if (Main.rand.NextBool(3))
                                {
                                    int index = Dust.NewDust(projectile.position, projectile.width, projectile.height, 171, 0.0f, 0.0f, 100, new Color(), 1f);
                                    Main.dust[index].noGravity = true;
                                    Main.dust[index].fadeIn = 1.5f;
                                    Main.dust[index].velocity *= 0.25f;
                                }
                                if (Main.rand.NextBool(3))
                                {
                                    int index = Dust.NewDust(projectile.position, projectile.width, projectile.height, 171, 0.0f, 0.0f, 100, new Color(), 1f);
                                    Main.dust[index].noGravity = true;
                                    Main.dust[index].fadeIn = 1.5f;
                                    Main.dust[index].velocity *= 0.25f;
                                }
                                break;
                            case 2:
                                if (Main.rand.NextBool(2))
                                {
                                    int index = Dust.NewDust(projectile.position, projectile.width, projectile.height, 75, projectile.velocity.X * 0.2f + (projectile.direction * 3), projectile.velocity.Y * 0.2f, 100, new Color(), 2.5f);
                                    Main.dust[index].noGravity = true;
                                    Main.dust[index].velocity *= 0.7f;
                                    Main.dust[index].velocity.Y -= 0.5f;
                                }
                                break;
                            case 3:
                                if (Main.rand.NextBool(2))
                                {
                                    int index = Dust.NewDust(projectile.position, projectile.width, projectile.height, 6, projectile.velocity.X * 0.2f + (projectile.direction * 3), projectile.velocity.Y * 0.2f, 100, new Color(), 2.5f);
                                    Main.dust[index].noGravity = true;
                                    Main.dust[index].velocity *= 0.7f;
                                    Main.dust[index].velocity.Y -= 0.5f;
                                }
                                break;
                            case 4:
                                if (Main.rand.NextBool(2))
                                {
                                    int index = Dust.NewDust(projectile.position, projectile.width, projectile.height, 57, projectile.velocity.X * 0.2f + (projectile.direction * 3), projectile.velocity.Y * 0.2f, 100, new Color(), 1.1f);
                                    Main.dust[index].noGravity = true;
                                    Main.dust[index].velocity.X /= 2f;
                                    Main.dust[index].velocity.Y /= 2f;
                                }
                                break;
                            case 5:
                                if (Main.rand.NextBool(2))
                                {
                                    int index = Dust.NewDust(projectile.position, projectile.width, projectile.height, 169, 0.0f, 0.0f, 100, new Color(), 1f);
                                    Main.dust[index].velocity.X += projectile.direction;
                                    Main.dust[index].velocity.Y += 0.2f;
                                    Main.dust[index].noGravity = true;
                                }
                                break;
                            case 6:
                                if (Main.rand.NextBool(2))
                                {
                                    int index = Dust.NewDust(projectile.position, projectile.width, projectile.height, 135, 0.0f, 0.0f, 100, new Color(), 1f);
                                    Main.dust[index].velocity.X += projectile.direction;
                                    Main.dust[index].velocity.Y += 0.2f;
                                    Main.dust[index].noGravity = true;
                                }
                                break;
                            case 8:
                                if (Main.rand.NextBool(4))
                                {
                                    int index = Dust.NewDust(projectile.position, projectile.width, projectile.height, 46, 0.0f, 0.0f, 100, new Color(), 1f);
                                    Main.dust[index].noGravity = true;
                                    Main.dust[index].fadeIn = 1.5f;
                                    Main.dust[index].velocity *= 0.25f;
                                }
                                break;
                            default:
                                break;
                        }
                    }
                }

                if (modPlayer.theBee && projectile.owner == Main.myPlayer && projectile.damage > 0 && player.statLife >= player.statLifeMax2)
                {
                    if (Main.rand.NextBool(5))
                    {
                        int dust = Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 91, projectile.oldVelocity.X * 0.5f, projectile.oldVelocity.Y * 0.5f, 0, default, 0.5f);
                        Main.dust[dust].noGravity = true;
                    }
                }

                if (!projectile.melee && player.meleeEnchant > 0 && !projectile.noEnchantments)
                {
                    if (player.meleeEnchant == 7) //flask of party affects all types of weapons
                    {
                        Vector2 velocity = projectile.velocity;
                        if (velocity.Length() > 4.0)
                            velocity *= 4f / velocity.Length();
                        if (Main.rand.NextBool(20))
                        {
                            int index = Dust.NewDust(projectile.position, projectile.width, projectile.height, Main.rand.Next(139, 143), velocity.X, velocity.Y, 0, new Color(), 1.2f);
                            Main.dust[index].velocity.X *= (float)(1.0 + Main.rand.Next(-50, 51) * 0.01);
                            Main.dust[index].velocity.Y *= (float)(1.0 + Main.rand.Next(-50, 51) * 0.01);
                            Main.dust[index].velocity.X += Main.rand.Next(-50, 51) * 0.05f;
                            Main.dust[index].velocity.Y += Main.rand.Next(-50, 51) * 0.05f;
                            Main.dust[index].scale *= (float)(1.0 + Main.rand.Next(-30, 31) * 0.01);
                        }
                        if (Main.rand.NextBool(40))
                        {
                            int Type = Main.rand.Next(276, 283);
                            int index = Gore.NewGore(projectile.position, velocity, Type, 1f);
                            Main.gore[index].velocity.X *= (float)(1.0 + Main.rand.Next(-50, 51) * 0.01);
                            Main.gore[index].velocity.Y *= (float)(1.0 + Main.rand.Next(-50, 51) * 0.01);
                            Main.gore[index].scale *= (float)(1.0 + Main.rand.Next(-20, 21) * 0.01);
                            Main.gore[index].velocity.X += Main.rand.Next(-50, 51) * 0.05f;
                            Main.gore[index].velocity.Y += Main.rand.Next(-50, 51) * 0.05f;
                        }
                    }
                }
            }
        }
        #endregion

        #region PostAI
        public override void PostAI(Projectile projectile)
        {
            Player player = Main.player[projectile.owner];
            CalamityPlayer modPlayer = player.Calamity();

            // optimization to remove conversion X/Y loop for irrelevant projectiles
            bool isConversionProjectile = projectile.type == ProjectileID.PurificationPowder
                || projectile.type == ProjectileID.PureSpray
                || projectile.type == ProjectileID.CorruptSpray
                || projectile.type == ProjectileID.CrimsonSpray
                || projectile.type == ProjectileID.HallowSpray;
            if (!isConversionProjectile)
                return;

            if (projectile.owner == Main.myPlayer/* && Main.netMode != NetmodeID.MultiplayerClient*/)
            {
                int x = (int)(projectile.Center.X / 16f);
                int y = (int)(projectile.Center.Y / 16f);

                bool isPowder = projectile.type == ProjectileID.PurificationPowder;
                /* || projectile.type == ProjectileID.VilePowder || projectile.type == ProjectileID.ViciousPowder */

                for (int i = x - 1; i <= x + 1; i++)
                {
                    for (int j = y - 1; j <= y + 1; j++)
                    {
                        if (projectile.type == ProjectileID.PureSpray || projectile.type == ProjectileID.PurificationPowder)
                        {
                            WorldGenerationMethods.ConvertFromAstral(i, j, ConvertType.Pure, !isPowder);
                        }
                        //commented out for Terraria 1.4 when vile/vicious powder spread corruption/crimson
                        if (projectile.type == ProjectileID.CorruptSpray)// || projectile.type == ProjectileID.VilePowder)
                        {
                            WorldGenerationMethods.ConvertFromAstral(i, j, ConvertType.Corrupt, !isPowder);
                        }
                        if (projectile.type == ProjectileID.CrimsonSpray)// || projectile.type == ProjectileID.ViciousPowder)
                        {
                            WorldGenerationMethods.ConvertFromAstral(i, j, ConvertType.Crimson, !isPowder);
                        }
                        if (projectile.type == ProjectileID.HallowSpray)
                        {
                            WorldGenerationMethods.ConvertFromAstral(i, j, ConvertType.Hallow);
                        }
                        NetMessage.SendTileRange(-1, i, j, 1, 1);
                    }
                }
            }
        }
        #endregion

        #region Grappling Hooks
        public override void GrapplePullSpeed(Projectile projectile, Player player, ref float speed) 
        {
            if (player.Calamity().reaverSpeed)
				speed *= 1.1f;
        }

        public override void GrappleRetreatSpeed(Projectile projectile, Player player, ref float speed) 
        {
            if (player.Calamity().reaverSpeed)
				speed *= 1.1f;
        }
        #endregion

        #region ModifyHitNPC
        public override void ModifyHitNPC(Projectile projectile, NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            Player player = Main.player[projectile.owner];
            CalamityPlayer modPlayer = player.Calamity();

            // Super dummies have nearly 10 million max HP (which is used in damage calculations).
            // This can very easily cause damage numbers that are unrealistic for the weapon.
            // As a result, they are omitted in this code.

            List<int> ignoreTheseBitches = new List<int>()
            {
                NPCType<SuperDummyNPC>(),
                NPCID.TheDestroyerBody, //why aren't these bosses
                NPCID.TheDestroyerTail
            };
            if (!target.boss && ignoreTheseBitches.TrueForAll(x => target.type != x))
            {
                if (target.Inorganic() && hasInorganicEnemyHitBoost)
                {
                    damage += (int)(target.lifeMax * inorganicEnemyHitBoost);
                    inorganicEnemyHitEffect?.Invoke(target);
                }
                if (target.Organic() && hasOrganicEnemyHitBoost)
                {
                    damage += (int)(target.lifeMax * organicEnemyHitBoost);
                    organicEnemyHitEffect?.Invoke(target);
                }
            }
            
            if (modPlayer.flamingItemEnchant && !projectile.minion)
                target.AddBuff(BuffType<VulnerabilityHex>(), 420);

            if (modPlayer.farProximityRewardEnchant)
			{
                float proximityDamageInterpolant = Utils.InverseLerp(250f, 2400f, target.Distance(player.Center), true);
                float proximityDamageFactor = MathHelper.SmoothStep(0.7f, 1.45f, proximityDamageInterpolant);
                damage = (int)Math.Ceiling(damage * proximityDamageFactor);
            }

            if (modPlayer.closeProximityRewardEnchant)
            {
                float proximityDamageInterpolant = Utils.InverseLerp(400f, 175f, target.Distance(player.Center), true);
                float proximityDamageFactor = MathHelper.SmoothStep(0.75f, 1.75f, proximityDamageInterpolant);
                damage = (int)Math.Ceiling(damage * proximityDamageFactor);
            }

            if (!projectile.npcProj && !projectile.trap && rogue && stealthStrike && modPlayer.stealthStrikeAlwaysCrits)
                crit = true;
        }
        #endregion

        #region CanDamage
        public override bool CanDamage(Projectile projectile)
        {
            switch (projectile.type)
            {
                case ProjectileID.Sharknado:
                    if (projectile.timeLeft > 420)
                        return false;
                    break;

                case ProjectileID.Cthulunado:
                    if (projectile.timeLeft > 720)
                        return false;
                    break;

                default:
                    break;
            }
            return true;
        }
        #endregion

        #region Drawing
        public override Color? GetAlpha(Projectile projectile, Color lightColor)
        {
            if (Main.player[Main.myPlayer].Calamity().trippy)
                return new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB, projectile.alpha);

            if (Main.LocalPlayer.Calamity().omniscience && projectile.hostile && projectile.damage > 0 && projectile.alpha < 255)
            {
                if (projectile.modProjectile is null || projectile.modProjectile != null && projectile.modProjectile.CanHitPlayer(Main.LocalPlayer) && projectile.modProjectile.CanDamage())
                {
                    return Color.Coral;
                }
            }

            if (projectile.type == ProjectileID.PinkLaser)
            {
                if (projectile.alpha < 200)
                    return new Color(255 - projectile.alpha, 255 - projectile.alpha, 255 - projectile.alpha, 0);

                return Color.Transparent;
            }

            if (projectile.type == ProjectileID.SeedPlantera || projectile.type == ProjectileID.PoisonSeedPlantera ||
				projectile.type == ProjectileID.ThornBall || projectile.type == ProjectileID.CultistBossFireBallClone ||
				projectile.type == ProjectileID.AncientDoomProjectile)
            {
                if (projectile.timeLeft < 85)
                {
                    byte b2 = (byte)(projectile.timeLeft * 3);
                    byte a2 = (byte)(projectile.alpha * (b2 / 255f));
                    return new Color(b2, b2, b2, a2);
                }
                return new Color(255, 255, 255, projectile.alpha);
            }

            return null;
        }

        public override bool PreDraw(Projectile projectile, SpriteBatch spriteBatch, Color lightColor)
        {
			if (Main.player[Main.myPlayer].Calamity().trippy)
            {
                Texture2D texture = Main.projectileTexture[projectile.type];

                SpriteEffects spriteEffects = SpriteEffects.None;
                if (projectile.spriteDirection == -1)
                    spriteEffects = SpriteEffects.FlipHorizontally;

                Vector2 vector11 = new Vector2(texture.Width / 2, texture.Height / Main.projFrames[projectile.type] / 2);
                Color color9 = new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB, projectile.alpha);
                Color alpha15 = projectile.GetAlpha(color9);

                for (int num213 = 0; num213 < 4; num213++)
                {
                    Vector2 position9 = projectile.position;
                    float num214 = Math.Abs(projectile.Center.X - Main.player[Main.myPlayer].Center.X);
                    float num215 = Math.Abs(projectile.Center.Y - Main.player[Main.myPlayer].Center.Y);

                    if (num213 == 0 || num213 == 2)
                        position9.X = Main.player[Main.myPlayer].Center.X + num214;
                    else
                        position9.X = Main.player[Main.myPlayer].Center.X - num214;

                    position9.X -= projectile.width / 2;

                    if (num213 == 0 || num213 == 1)
                        position9.Y = Main.player[Main.myPlayer].Center.Y + num215;
                    else
                        position9.Y = Main.player[Main.myPlayer].Center.Y - num215;

                    int frames = texture.Height / Main.projFrames[projectile.type];
                    int y = frames * projectile.frame;
                    position9.Y -= projectile.height / 2;

                    Main.spriteBatch.Draw(texture,
                        new Vector2(position9.X - Main.screenPosition.X + (projectile.width / 2) - texture.Width * projectile.scale / 2f + vector11.X * projectile.scale, position9.Y - Main.screenPosition.Y + projectile.height - texture.Height * projectile.scale / Main.projFrames[projectile.type] + 4f + vector11.Y * projectile.scale + projectile.gfxOffY),
                        new Microsoft.Xna.Framework.Rectangle?(new Rectangle(0, y, texture.Width, frames)), alpha15, projectile.rotation, vector11, projectile.scale, spriteEffects, 0f);
                }
            }

            return true;
        }
        #endregion

        #region Kill
        public override void Kill(Projectile projectile, int timeLeft)
        {
			Player player = Main.player[projectile.owner];
            CalamityPlayer modPlayer = player.Calamity();
            if (projectile.owner == Main.myPlayer && !projectile.npcProj && !projectile.trap)
            {
                if (rogue)
                {
                    if (modPlayer.etherealExtorter && Main.player[projectile.owner].ownedProjectileCounts[ProjectileType<LostSoulFriendly>()] < 5)
                    {
                        for (int i = 0; i < 2; i++)
                        {
                            Vector2 velocity = CalamityUtils.RandomVelocity(100f, 70f, 100f);
                            int soul = Projectile.NewProjectile(projectile.Center, velocity, ProjectileType<LostSoulFriendly>(), (int)(25 * player.RogueDamage()), 0f, projectile.owner);
                            Main.projectile[soul].tileCollide = false;
							if (soul.WithinBounds(Main.maxProjectiles))
								Main.projectile[soul].Calamity().forceTypeless = true;
						}
                    }

                    if (modPlayer.scuttlersJewel && CalamityLists.javelinProjList.Contains(projectile.type) && Main.rand.NextBool(3))
                    {
                        int spike = Projectile.NewProjectile(projectile.Center, Vector2.Zero, ProjectileType<JewelSpike>(), (int)(15 * player.RogueDamage()), projectile.knockBack, projectile.owner);
                        Main.projectile[spike].frame = 4;
						if (spike.WithinBounds(Main.maxProjectiles))
							Main.projectile[spike].Calamity().forceTypeless = true;
					}
                }

                if (projectile.type == ProjectileID.UnholyWater)
                    Projectile.NewProjectile(projectile.Center, Vector2.Zero, ProjectileType<WaterConvertor>(), 0, 0f, projectile.owner, 1f);

                if (projectile.type == ProjectileID.BloodWater)
                    Projectile.NewProjectile(projectile.Center, Vector2.Zero, ProjectileType<WaterConvertor>(), 0, 0f, projectile.owner, 2f);

                if (projectile.type == ProjectileID.HolyWater)
                    Projectile.NewProjectile(projectile.Center, Vector2.Zero, ProjectileType<WaterConvertor>(), 0, 0f, projectile.owner, 3f);
            }
        }
        #endregion

        #region CanHit
        public override bool CanHitPlayer(Projectile projectile, Player target)
        {
            if (projectile.type == ProjectileID.CultistBossLightningOrb)
            {
                return false;
            }
            return true;
        }
		#endregion

		#region LifeSteal
		public static bool CanSpawnLifeStealProjectile(float healMultiplier, float healAmount)
        {
            if (healMultiplier <= 0f || (int)healAmount <= 0)
                return false;

            return true;
        }

        public static void SpawnLifeStealProjectile(Projectile projectile, Player player, float healAmount, int healProjectileType, float distanceRequired, float cooldownMultiplier)
        {
            if (Main.player[Main.myPlayer].moonLeech)
                return;

            Main.player[Main.myPlayer].lifeSteal -= healAmount * cooldownMultiplier;

            float lowestHealthCheck = 0f;
            int healTarget = projectile.owner;
            for (int i = 0; i < Main.maxPlayers; i++)
            {
                Player otherPlayer = Main.player[i];
                if (otherPlayer.active && !otherPlayer.dead && ((!player.hostile && !otherPlayer.hostile) || player.team == otherPlayer.team))
                {
                    float playerDist = Vector2.Distance(projectile.Center, otherPlayer.Center);
                    if (playerDist < distanceRequired && (otherPlayer.statLifeMax2 - otherPlayer.statLife) > lowestHealthCheck)
                    {
                        lowestHealthCheck = otherPlayer.statLifeMax2 - otherPlayer.statLife;
                        healTarget = i;
                    }
                }
            }

            Projectile.NewProjectile(projectile.Center, Vector2.Zero, healProjectileType, 0, 0f, projectile.owner, healTarget, healAmount);
        }
        #endregion

        // TODO -- this entire region needs to go to Projectile Utilities
        #region AI Shortcuts
        public static Projectile SpawnOrb(Projectile projectile, int damage, int projType, float distanceRequired, float speedMult, bool gsPhantom = false)
        {
            Player player = Main.player[projectile.owner];
            CalamityPlayer modPlayer = player.Calamity();

            float ai1 = Main.rand.NextFloat() + 0.5f;
            int[] array = new int[Main.maxNPCs];
            int targetArrayA = 0;
            int targetArrayB = 0;
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (npc.CanBeChasedBy(projectile, false))
                {
                    float enemyDist = Vector2.Distance(projectile.Center, npc.Center);
                    if (enemyDist < distanceRequired)
                    {
                        if (Collision.CanHit(projectile.position, 1, 1, npc.position, npc.width, npc.height) && enemyDist > 50f)
                        {
                            array[targetArrayB] = i;
                            targetArrayB++;
                        }
                        else if (targetArrayB == 0)
                        {
                            array[targetArrayA] = i;
                            targetArrayA++;
                        }
                    }
                }
            }
            if (targetArrayA == 0 && targetArrayB == 0)
            {
                return Projectile.NewProjectileDirect(projectile.Center, Vector2.Zero, ProjectileType<NobodyKnows>(), 0, 0f, projectile.owner);
            }
            int target = targetArrayB <= 0 ? array[Main.rand.Next(targetArrayA)] : array[Main.rand.Next(targetArrayB)];
            Vector2 velocity = CalamityUtils.RandomVelocity(100f, speedMult, speedMult, 1f);
            Projectile orb = Projectile.NewProjectileDirect(projectile.Center, velocity, projType, damage, 0f, projectile.owner, gsPhantom ? 0f : target, gsPhantom ? ai1 : 0f);
            return orb;
        }

        public static void HomeInOnNPC(Projectile projectile, bool ignoreTiles, float distanceRequired, float homingVelocity, float N)
        {
            if (!projectile.friendly)
                return;

			// Set amount of extra updates.
			if (projectile.Calamity().defExtraUpdates == -1)
				projectile.Calamity().defExtraUpdates = projectile.extraUpdates;

			Vector2 destination = projectile.Center;
            float maxDistance = distanceRequired;
            bool locatedTarget = false;

			// Find a target.
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                float extraDistance = (Main.npc[i].width / 2) + (Main.npc[i].height / 2);
                if (!Main.npc[i].CanBeChasedBy(projectile, false) || !projectile.WithinRange(Main.npc[i].Center, maxDistance + extraDistance))
                    continue;

                if (ignoreTiles || Collision.CanHit(projectile.Center, 1, 1, Main.npc[i].Center, 1, 1))
                {
                    destination = Main.npc[i].Center;
                    locatedTarget = true;
                    break;
                }
            }

            if (locatedTarget)
            {
				// Increase amount of extra updates to greatly increase homing velocity.
				projectile.extraUpdates = projectile.Calamity().defExtraUpdates + 1;

				// Home in on the target.
                Vector2 homeDirection = (destination - projectile.Center).SafeNormalize(Vector2.UnitY);
                projectile.velocity = (projectile.velocity * N + homeDirection * homingVelocity) / (N + 1f);
            }
			else
			{
				// Set amount of extra updates to default amount.
				projectile.extraUpdates = projectile.Calamity().defExtraUpdates;
			}
        }

        public static void MagnetSphereHitscan(Projectile projectile, float distanceRequired, float homingVelocity, float projectileTimer, int maxTargets, int spawnedProjectile, double damageMult = 1D, bool attackMultiple = false)
        {
            float maxDistance = distanceRequired;
            bool homeIn = false;
            int[] targetArray = new int[maxTargets];
            int targetArrayIndex = 0;

            for (int i = 0; i < Main.maxNPCs; i++)
            {
                if (Main.npc[i].CanBeChasedBy(projectile, false))
                {
                    float extraDistance = (Main.npc[i].width / 2) + (Main.npc[i].height / 2);

                    bool canHit = true;
                    if (extraDistance < maxDistance)
                        canHit = Collision.CanHit(projectile.Center, 1, 1, Main.npc[i].Center, 1, 1);

                    if (projectile.WithinRange(Main.npc[i].Center, maxDistance + extraDistance) && canHit)
                    {
                        if (targetArrayIndex < maxTargets)
                        {
                            targetArray[targetArrayIndex] = i;
                            targetArrayIndex++;
                            homeIn = true;
                        }
                        else
                            break;
                    }
                }
            }

            if (homeIn)
            {
                int randomTarget = Main.rand.Next(targetArrayIndex);
                randomTarget = targetArray[randomTarget];

                projectile.localAI[0] += 1f;
                if (projectile.localAI[0] > projectileTimer)
                {
                    projectile.localAI[0] = 0f;
                    Vector2 spawnPos = projectile.Center + projectile.velocity * 4f;
                    Vector2 velocity = Vector2.Normalize(Main.npc[randomTarget].Center - spawnPos) * homingVelocity;

                    if (attackMultiple)
                    {
                        for (int i = 0; i < targetArrayIndex; i++)
                        {
                            velocity = Vector2.Normalize(Main.npc[targetArray[i]].Center - spawnPos) * homingVelocity;

                            if (projectile.owner == Main.myPlayer)
                            {
                                int projectile2 = Projectile.NewProjectile(spawnPos, velocity, spawnedProjectile, (int)(projectile.damage * damageMult), projectile.knockBack, projectile.owner, 0f, 0f);

                                if (projectile.type == ProjectileType<EradicatorProjectile>())
									if (projectile2.WithinBounds(Main.maxProjectiles))
										Main.projectile[projectile2].Calamity().forceRogue = true;
                            }
                        }

                        return;
                    }

                    if (projectile.type == ProjectileType<GodsGambitYoyo>())
                    {
                        velocity.Y += Main.rand.Next(-30, 31) * 0.05f;
                        velocity.X += Main.rand.Next(-30, 31) * 0.05f;
                    }

                    if (projectile.owner == Main.myPlayer)
                    {
                        int projectile2 = Projectile.NewProjectile(spawnPos, velocity, spawnedProjectile, (int)(projectile.damage * damageMult), projectile.knockBack, projectile.owner, 0f, 0f);

                        if (projectile.type == ProjectileType<CnidarianYoyo>() || projectile.type == ProjectileType<GodsGambitYoyo>() ||
                            projectile.type == ProjectileType<ShimmersparkYoyo>() || projectile.type == ProjectileType<VerdantYoyo>())
							if (projectile2.WithinBounds(Main.maxProjectiles))
								Main.projectile[projectile2].Calamity().forceMelee = true;

						if (projectile.type == ProjectileType<SeashellBoomerangProjectile>())
							if (projectile2.WithinBounds(Main.maxProjectiles))
								Main.projectile[projectile2].Calamity().forceRogue = true;
                    }
                }
            }
        }

        public static void ExpandHitboxBy(Projectile projectile, int width, int height)
        {
            projectile.position = projectile.Center;
            projectile.width = width;
            projectile.height = height;
            projectile.position -= projectile.Size * 0.5f;
        }

        public static void ExpandHitboxBy(Projectile projectile, int newSize)
        {
            ExpandHitboxBy(projectile, newSize, newSize);
        }

        public static void ExpandHitboxBy(Projectile projectile, float expandRatio)
        {
            ExpandHitboxBy(projectile, (int)(projectile.width * expandRatio), (int)(projectile.height * expandRatio));
        }
        #endregion
    }
}
