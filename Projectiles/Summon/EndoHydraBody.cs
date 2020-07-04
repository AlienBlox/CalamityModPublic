using CalamityMod.Buffs.Summon;
using CalamityMod.CalPlayer;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon
{
    public class EndoHydraBody : ModProjectile
    {
        public const float DistanceToCheck = 1600f;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Hydra Body");
            Main.projFrames[projectile.type] = 5;
            ProjectileID.Sets.MinionTargettingFeature[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.width = 52;
            projectile.height = 86;
            projectile.netImportant = true;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.minionSlots = 0f;
            projectile.timeLeft = 18000;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
            projectile.timeLeft *= 5;
            projectile.minion = true;
			projectile.coldDamage = true;
        }

        public override void AI()
        {
            Player player = Main.player[projectile.owner];
            CalamityPlayer modPlayer = player.Calamity();
            if (projectile.localAI[0] == 0f)
            {
                projectile.Calamity().spawnedPlayerMinionDamageValue = player.MinionDamage();
                projectile.Calamity().spawnedPlayerMinionProjectileDamageValue = projectile.damage;
                projectile.ai[0] = -1f;
                projectile.localAI[0] = 1f;
			}
            if (player.MinionDamage() != projectile.Calamity().spawnedPlayerMinionDamageValue)
            {
                int trueDamage = (int)(projectile.Calamity().spawnedPlayerMinionProjectileDamageValue /
                    projectile.Calamity().spawnedPlayerMinionDamageValue *
                    player.MinionDamage());
                projectile.damage = trueDamage;
            }
            bool isProperProjectile = projectile.type == ModContent.ProjectileType<EndoHydraBody>();
            player.AddBuff(ModContent.BuffType<EndoHydraBuff>(), 3600);
            if (isProperProjectile)
            {
                if (player.dead)
                {
                    modPlayer.endoHydra = false;
                }
                if (modPlayer.endoHydra)
                {
                    projectile.timeLeft = 2;
                }
            }

            Vector2 returnLocation = player.Center;
            returnLocation.X -= (18 + player.width / 2) * player.direction;
            returnLocation.Y -= 25f;

            NPC potentialTarget = projectile.Center.MinionHoming(DistanceToCheck, player);
            if (potentialTarget != null)
            {
                if (projectile.ai[0] != potentialTarget.whoAmI)
                {
                    projectile.ai[0] = potentialTarget.whoAmI;
                    Main.PlaySound(SoundID.Zombie, projectile.Center, 53); // Ethereal whisper indicating a new target has been spotted.
                    projectile.netUpdate = true;
                }
            }

            projectile.frameCounter++;
            if (projectile.frameCounter > (projectile.ai[0] == -1 ? 8 : 6))
            {
                projectile.frame++;
                projectile.frameCounter = 0;
            }
            if (projectile.frame >= Main.projFrames[projectile.type])
            {
                projectile.frame = 0;
            }

            projectile.Center = Vector2.Lerp(projectile.Center, returnLocation, 0.25f);
            projectile.direction = projectile.spriteDirection = player.direction;
            Lighting.AddLight(projectile.Center - Vector2.UnitY * 21f, 0.25f, 0.865f, 0.825f);
        }

        public override bool CanDamage() => false;
    }
}
