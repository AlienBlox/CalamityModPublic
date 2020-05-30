using CalamityMod.CalPlayer;
using CalamityMod.Projectiles.Healing;
using CalamityMod.Projectiles.Typeless;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon
{
    public class FungalClumpMinion : ModProjectile
    {
		private bool returnToPlayer = false;
		private bool amalgam = false;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Fungal Clump");
            ProjectileID.Sets.MinionSacrificable[projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.width = 40;
            projectile.height = 40;
            projectile.netImportant = true;
            projectile.friendly = true;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 10;
            projectile.minionSlots = 0f;
            projectile.timeLeft = 18000;
            projectile.penetrate = -1;
            projectile.timeLeft *= 5;
            projectile.minion = true;
            projectile.tileCollide = false;
        }

        public override void AI()
        {
            Player player = Main.player[projectile.owner];
            CalamityPlayer modPlayer = player.Calamity();

            bool correctMinion = projectile.type == ModContent.ProjectileType<FungalClumpMinion>();
            if (!modPlayer.fungalClump)
            {
                projectile.active = false;
                return;
            }
            if (correctMinion)
            {
                if (player.dead)
                {
                    modPlayer.fClump = false;
                }
                if (modPlayer.fClump)
                {
                    projectile.timeLeft = 2;
                }
            }
			amalgam = projectile.ai[0] == 1f;

			//Initializing dust and damage
            if (projectile.localAI[0] == 0f)
            {
                projectile.Calamity().spawnedPlayerMinionDamageValue = player.MinionDamage();
                projectile.Calamity().spawnedPlayerMinionProjectileDamageValue = projectile.damage;
                int num226 = 36;
                for (int num227 = 0; num227 < num226; num227++)
                {
                    Vector2 vector6 = Vector2.Normalize(projectile.velocity) * new Vector2((float)projectile.width / 2f, (float)projectile.height) * 0.75f;
                    vector6 = vector6.RotatedBy((double)((float)(num227 - (num226 / 2 - 1)) * 6.28318548f / (float)num226), default) + projectile.Center;
                    Vector2 vector7 = vector6 - projectile.Center;
                    int num228 = Dust.NewDust(vector6 + vector7, 0, 0, 56, vector7.X * 1.5f, vector7.Y * 1.5f, 100, default, 1.4f);
                    Main.dust[num228].noGravity = true;
                    Main.dust[num228].noLight = true;
                    Main.dust[num228].velocity = vector7;
                }
                projectile.localAI[0] += 1f;
            }

			//Flexible damage correction
            if (player.MinionDamage() != projectile.Calamity().spawnedPlayerMinionDamageValue)
            {
                int damage2 = (int)((float)projectile.Calamity().spawnedPlayerMinionProjectileDamageValue /
                    projectile.Calamity().spawnedPlayerMinionDamageValue *
                    player.MinionDamage());
                projectile.damage = damage2;
            }

			//Periodically create dust
            if (Main.rand.NextBool(16))
            {
                Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 56, projectile.velocity.X * 0.05f, projectile.velocity.Y * 0.05f);
            }

			//Anti-sticky movement failsafe
            float SAIMovement = 0.05f;
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
				Projectile proj = Main.projectile[i];
                bool typeCheck = proj.type == ModContent.ProjectileType<FungalClumpMinion>();
                if (i != projectile.whoAmI && proj.active && proj.owner == projectile.owner && typeCheck && Math.Abs(projectile.position.X - proj.position.X) + Math.Abs(projectile.position.Y - proj.position.Y) < (float)projectile.width)
                {
                    if (projectile.position.X < proj.position.X)
                    {
                        projectile.velocity.X -= SAIMovement;
                    }
                    else
                    {
                        projectile.velocity.X += SAIMovement;
                    }
                    if (projectile.position.Y < proj.position.Y)
                    {
                        projectile.velocity.Y -= SAIMovement;
                    }
                    else
                    {
                        projectile.velocity.Y += SAIMovement;
                    }
                }
            }

			//If summoned by Amalgam, trail poisonous seawater
			if (Math.Abs(projectile.velocity.X) > 0.1f || Math.Abs(projectile.velocity.Y) > 0.1f)
            {
                if (projectile.owner == Main.myPlayer && amalgam)
                {
                    int water = Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, 0f, 0f, ModContent.ProjectileType<PoisonousSeawater>(), projectile.damage, 0f, projectile.owner, 1f, 0f);
					Main.projectile[water].usesIDStaticNPCImmunity = true;
					Main.projectile[water].usesLocalNPCImmunity = false;
                }
            }

			//If too far from player, increase speed to chase after player
			float playerRange = 500f;
			//Range is boosted if chasing after an enemy
			if (projectile.ai[1] != 0f || projectile.friendly)
				playerRange = amalgam ? 2000f : 1400f;
			if (Math.Abs(projectile.Center.X - player.Center.X) + Math.Abs(projectile.Center.Y - player.Center.Y) > playerRange)
				returnToPlayer = true;

			//Find an npc to target, or if minion targetting is used, choose that npc
			Vector2 targetVec = projectile.Center;
			float range = 900f;
			bool npcFound = false;
            Vector2 half = new Vector2(0.5f);
			if (!returnToPlayer)
			{
				if (player.HasMinionAttackTargetNPC)
				{
					NPC npc = Main.npc[player.MinionAttackTargetNPC];
					if (npc.CanBeChasedBy(projectile, false))
					{
						//Check the size of the target to make it easier to hit fat targets like Levi
						Vector2 sizeCheck = npc.position + npc.Size * half;
						float targetDist = Vector2.Distance(npc.Center, projectile.Center);
						//Some minions will ignore tiles when choosing a target like Ice Claspers, others will not
						bool canHit = Collision.CanHitLine(projectile.position, projectile.width, projectile.height, npc.position, npc.width, npc.height);
						if (!npcFound && targetDist < range && canHit)
						{
							range = targetDist;
							targetVec = sizeCheck;
							npcFound = true;
						}
					}
				}
				//If no npc is specifically targetted, check through the entire array
				else
				{
					for (int npcIndex = 0; npcIndex < Main.maxNPCs; npcIndex++)
					{
						NPC npc = Main.npc[npcIndex];
						if (npc.CanBeChasedBy(projectile, false))
						{
							Vector2 sizeCheck = npc.position + npc.Size * half;
							float targetDist = Vector2.Distance(npc.Center, projectile.Center);
							bool canHit = Collision.CanHitLine(projectile.position, projectile.width, projectile.height, npc.position, npc.width, npc.height);
							if (!npcFound && targetDist < range && canHit)
							{
								range = targetDist;
								targetVec = sizeCheck;
								npcFound = true;
							}
						}
					}
				}
			}

			//Tile collision depends on if returning to the player or not
			projectile.tileCollide = !returnToPlayer;

			if (!npcFound)
			{
				projectile.friendly = true;
				float homingSpeed = amalgam ? 12f : 8f;
				float turnSpeed = 20f;
				if (returnToPlayer) //move faster if returning to the player
					homingSpeed = amalgam ? 30f : 12f;
				Vector2 playerVector = player.Center - projectile.Center;
				playerVector.Y -= 60f;
				float playerDist = playerVector.Length();
				if (playerDist < 100f && returnToPlayer && !Collision.SolidCollision(projectile.position, projectile.width, projectile.height))
					returnToPlayer = false;
				if (playerDist > 2000f)
				{
					projectile.position.X = player.Center.X - projectile.width / 2;
					projectile.position.Y = player.Center.Y - projectile.width / 2;
				}
				//If more than 70 pixels away, move toward the player
                if (playerDist > 70f)
                {
                    playerVector.Normalize();
                    playerVector *= homingSpeed;
                    projectile.velocity = (projectile.velocity * turnSpeed + playerVector) / (turnSpeed + 1f);
                }
				//Minions never stay still
                else
                {
					if (projectile.velocity.X == 0f && projectile.velocity.Y == 0f)
					{
						projectile.velocity.X = -0.15f;
						projectile.velocity.Y = -0.05f;
					}
					projectile.velocity *= 1.01f;
                }
				projectile.friendly = false;
				projectile.rotation = projectile.velocity.X * 0.05f;
				if (Math.Abs(projectile.velocity.X) <= 0f)
					return;
				projectile.spriteDirection = -projectile.direction;
			}
			else
			{
				if (projectile.ai[1] == -1f)
					projectile.ai[1] = 17f;
				if (projectile.ai[1] > 0f)
				{
					projectile.ai[1] -= 1f;
				}
				if (projectile.ai[1] == 0f)
				{
					projectile.friendly = true;
					float minionSpeed = amalgam ? 20f : 8f;
					float turnSpeed = 14f;
					Vector2 targetLocation = targetVec - projectile.Center;
					float targetDist = targetLocation.Length();
					if (targetDist < 100f)
						minionSpeed = amalgam ? 25f : 10f;

					Vector2 homeInVector = projectile.DirectionTo(targetLocation);
					if (homeInVector.HasNaNs())
						homeInVector = Vector2.UnitY;

                    targetLocation.Normalize();
                    targetLocation *= minionSpeed;
                    projectile.velocity = (projectile.velocity * turnSpeed + targetLocation) / (turnSpeed + 1f);
				}
				else
				{
					projectile.friendly = false;
					if (Math.Abs(projectile.velocity.X) + Math.Abs(projectile.velocity.Y) < 10f)
						projectile.velocity *= 1.05f;
				}
				projectile.rotation = projectile.velocity.X * 0.05f;
				if (Math.Abs(projectile.velocity.X) <= 0.2f)
					return;
				projectile.spriteDirection = -projectile.direction;
			}
		}

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            if (target.type == NPCID.TargetDummy || !target.canGhostHeal)
            {
                return;
            }
            float healAmt = damage * 0.25f;
            if ((int)healAmt == 0)
            {
                return;
            }
            if (Main.player[Main.myPlayer].lifeSteal <= 0f)
            {
                return;
            }
			CalamityGlobalProjectile.SpawnLifeStealProjectile(projectile, Main.player[projectile.owner], healAmt, ModContent.ProjectileType<FungalHeal>(), 1200f, 1f);
        }

        public override bool OnTileCollide(Vector2 oldVelocity) => false;
    }
}
