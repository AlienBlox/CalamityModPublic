using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Projectiles.Ranged;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Rogue
{
    public class PrismallineProj : ModProjectile
    {
        public bool hitEnemy = false;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Prismalline");
        }

        public override void SetDefaults()
        {
            projectile.width = 20;
            projectile.height = 20;
            projectile.friendly = true;
            projectile.penetrate = 2;
            projectile.aiStyle = 113;
            projectile.timeLeft = 180;
            aiType = ProjectileID.BoneJavelin;
            projectile.Calamity().rogue = true;
        }

        public override void AI()
        {
            projectile.rotation = (float)Math.Atan2((double)projectile.velocity.Y, (double)projectile.velocity.X) + 2.355f;
            if (projectile.spriteDirection == -1)
            {
                projectile.rotation -= 1.57f;
            }
            projectile.ai[1] += 1f;
            if (projectile.ai[1] == 40f)
            {
                int numProj = 4;
                int numSpecProj = 0;
                float rotation = MathHelper.ToRadians(50);
                if (projectile.owner == Main.myPlayer)
                {
					if (!projectile.Calamity().stealthStrike)
					{
						for (int i = 0; i < numProj + 1; i++)
						{
							Vector2 speed = new Vector2((float)Main.rand.Next(-50, 51), (float)Main.rand.Next(-50, 51));
							while (speed.X == 0f && speed.Y == 0f)
							{
								speed = new Vector2((float)Main.rand.Next(-50, 51), (float)Main.rand.Next(-50, 51));
							}
							speed.Normalize();
							speed *= (float)Main.rand.Next(30, 61) * 0.1f * 2f;
							if (numSpecProj < 2 && !hitEnemy)
							{
								Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, speed.X, speed.Y, ModContent.ProjectileType<Prismalline3>(), (int)(projectile.damage * 1.25), projectile.knockBack, projectile.owner, 0f, 0f);
								++numSpecProj;
							}
							else
							{
								Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, speed.X, speed.Y, ModContent.ProjectileType<Prismalline2>(), (int)(projectile.damage * 0.75), projectile.knockBack, projectile.owner, 0f, 0f);
							}
						}
					}
					else //stealth strike
					{
						int shardCount = Main.rand.Next(2,5);
						for (int num252 = 0; num252 < shardCount; num252++)
						{
							Vector2 value15 = new Vector2((float)Main.rand.Next(-100, 101), (float)Main.rand.Next(-100, 101));
							while (value15.X == 0f && value15.Y == 0f)
							{
								value15 = new Vector2((float)Main.rand.Next(-100, 101), (float)Main.rand.Next(-100, 101));
							}
							value15.Normalize();
							value15 *= (float)Main.rand.Next(70, 101) * 0.1f;
							int shard = Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, value15.X, value15.Y, ModContent.ProjectileType<AquashardSplit>(), projectile.damage / 2, 0f, projectile.owner, 0f, 0f);
							Main.projectile[shard].Calamity().forceRogue = true;
							Main.projectile[shard].usesLocalNPCImmunity = true;
							Main.projectile[shard].localNPCHitCooldown = 10;
						}
						for (int i = 0; i < numProj + 1; i++)
						{
							Vector2 speed = new Vector2((float)Main.rand.Next(-50, 51), (float)Main.rand.Next(-50, 51));
							while (speed.X == 0f && speed.Y == 0f)
							{
								speed = new Vector2((float)Main.rand.Next(-50, 51), (float)Main.rand.Next(-50, 51));
							}
							speed.Normalize();
							speed *= (float)Main.rand.Next(30, 61) * 0.1f * 2f;
							Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, speed.X, speed.Y, ModContent.ProjectileType<Prismalline3>(), (int)(projectile.damage * 1.25), projectile.knockBack, projectile.owner, 1f, 0f);
						}
					}
                }
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D tex = Main.projectileTexture[projectile.type];
            spriteBatch.Draw(tex, projectile.Center - Main.screenPosition, null, projectile.GetAlpha(lightColor), projectile.rotation, tex.Size() / 2f, projectile.scale, SpriteEffects.None, 0f);
            return false;
        }

        public override void Kill(int timeLeft)
		{
			Main.PlaySound(SoundID.Item27, projectile.position);
			for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 154, projectile.oldVelocity.X * 0.5f, projectile.oldVelocity.Y * 0.5f);
            }
			if (projectile.Calamity().stealthStrike)
			{
				int shardCount = Main.rand.Next(1,4);
				for (int num252 = 0; num252 < shardCount; num252++)
				{
					Vector2 value15 = new Vector2((float)Main.rand.Next(-100, 101), (float)Main.rand.Next(-100, 101));
					while (value15.X == 0f && value15.Y == 0f)
					{
						value15 = new Vector2((float)Main.rand.Next(-100, 101), (float)Main.rand.Next(-100, 101));
					}
					value15.Normalize();
					value15 *= (float)Main.rand.Next(70, 101) * 0.1f;
					int shard = Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, value15.X, value15.Y, ModContent.ProjectileType<AquashardSplit>(), projectile.damage / 2, 0f, projectile.owner, 0f, 0f);
					Main.projectile[shard].Calamity().forceRogue = true;
					Main.projectile[shard].penetrate = 1;
				}
			}
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            hitEnemy = true;
			if (projectile.Calamity().stealthStrike)
				target.AddBuff(ModContent.BuffType<Eutrophication>(), 15);
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            hitEnemy = true;
			if (projectile.Calamity().stealthStrike)
				target.AddBuff(ModContent.BuffType<Eutrophication>(), 15);
        }
    }
}
