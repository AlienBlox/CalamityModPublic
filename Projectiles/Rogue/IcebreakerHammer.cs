﻿using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Projectiles.Melee;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public class IcebreakerHammer : ModProjectile
    {
		private int explosionCount = 0;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Icebreaker");
        }

        public override void SetDefaults()
        {
            projectile.width = 30;
            projectile.height = 30;
            projectile.friendly = true;
            projectile.Calamity().rogue = true;
            projectile.penetrate = -1;
            projectile.aiStyle = 3;
            projectile.timeLeft = 120;
            aiType = ProjectileID.WoodenBoomerang;
			projectile.coldDamage = true;
        }

        public override void AI()
        {
            if (Main.rand.NextBool(3))
            {
                Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 67, projectile.velocity.X * 0.5f, projectile.velocity.Y * 0.5f);
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D tex = Main.projectileTexture[projectile.type];
            spriteBatch.Draw(tex, projectile.Center - Main.screenPosition, null, projectile.GetAlpha(lightColor), projectile.rotation, tex.Size() / 2f, projectile.scale, SpriteEffects.None, 0f);
            return false;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
			if (projectile.owner == Main.myPlayer)
			{
				target.AddBuff(BuffID.Frostburn, 180);

				if (projectile.Calamity().stealthStrike)
				{
					if (explosionCount < 3) //max amount of explosions to prevent worm memes
					{
						int ice = Projectile.NewProjectile(projectile.Center, Vector2.Zero, ModContent.ProjectileType<CosmicIceBurst>(), projectile.damage, projectile.knockBack, projectile.owner, 0f, 0.85f + Main.rand.NextFloat() * 1.15f);
						Main.projectile[ice].Calamity().forceRogue = true;
						explosionCount++;
					}

					int buffType = ModContent.BuffType<GlacialState>();
					float radius = 112f; // 7 blocks

					for (int i = 0; i < Main.maxNPCs; i++)
					{
						NPC nPC = Main.npc[i];
						if (nPC.active && !nPC.dontTakeDamage && !nPC.buffImmune[buffType] && Vector2.Distance(projectile.Center, nPC.Center) <= radius)
						{
							if (nPC.FindBuffIndex(buffType) == -1)
								nPC.AddBuff(buffType, 180, false);
						}
					}
				}
			}
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
			if (projectile.owner == Main.myPlayer)
			{
				target.AddBuff(BuffID.Frostburn, 180);

				if (projectile.Calamity().stealthStrike)
				{
					//no explosion count cap in pvp
					int ice = Projectile.NewProjectile(projectile.Center, Vector2.Zero, ModContent.ProjectileType<CosmicIceBurst>(), projectile.damage, projectile.knockBack, projectile.owner, 0f, 0.85f + Main.rand.NextFloat() * 1.15f);
					Main.projectile[ice].Calamity().forceRogue = true;

					int buffType = ModContent.BuffType<GlacialState>();
					float radius = 112f; // 7 blocks

					for (int i = 0; i < Main.maxPlayers; i++)
					{
						Player player = Main.player[i];
						if ((Main.player[projectile.owner].team != player.team || player.team == 0) && !player.dead && !player.buffImmune[buffType] && Vector2.Distance(projectile.Center, player.Center) <= radius)
						{
							if (player.FindBuffIndex(buffType) == -1)
								player.AddBuff(buffType, 180, false);
						}
					}
				}
			}
        }
    }
}
