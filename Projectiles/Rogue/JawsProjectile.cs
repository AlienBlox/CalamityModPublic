using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Buffs.StatDebuffs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public class JawsProjectile : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Reaper Tooth");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 8;
            ProjectileID.Sets.TrailingMode[projectile.type] = 1;
        }

        public override void SetDefaults()
        {
            projectile.width = 10;
            projectile.height = 10;
            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.timeLeft = 600;
            projectile.Calamity().rogue = true;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 10;
        }

        public override void AI()
        {
            if (projectile.ai[0] == 0f)
            {
                projectile.rotation = (float)Math.Atan2((double)projectile.velocity.Y, (double)projectile.velocity.X) + 1.57f;
                if (projectile.Calamity().stealthStrike)
                {
                    int dustToUse = Main.rand.Next(0, 4);
                    int dustType = 0;
                    switch (dustToUse)
                    {
                        case 0:
                            dustType = 33;
                            break;
                        case 1:
                            dustType = 101;
                            break;
                        case 2:
                            dustType = 111;
                            break;
                        case 3:
                            dustType = 180;
                            break;
                    }

                    int dust = Dust.NewDust(projectile.Center, 1, 1, dustType, projectile.velocity.X, projectile.velocity.Y, 0, default, 1.5f);
                    Main.dust[dust].noGravity = true;
                }
            }
            //Sticky Behaviour
            CalamityUtils.StickyProjAI(projectile);
        }

        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            CalamityUtils.ModifyHitNPCSticky(projectile, 6, false);
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
			Player player = Main.player[projectile.owner];
            target.AddBuff(BuffID.Venom, 240);
            target.AddBuff(ModContent.BuffType<ArmorCrunch>(), 240);
            target.AddBuff(ModContent.BuffType<MarkedforDeath>(), 240);
            if (projectile.Calamity().stealthStrike)
            {
                target.AddBuff(ModContent.BuffType<CrushDepth>(), 240);
                Projectile.NewProjectile(projectile.Center, Vector2.Zero, ModContent.ProjectileType<JawsShockwave>(), (int)(100f * (player.allDamage + player.Calamity().throwingDamage - 1f)), 10f, projectile.owner, 0, 0);
            }
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
			Player player = Main.player[projectile.owner];
            target.AddBuff(BuffID.Venom, 240);
            target.AddBuff(ModContent.BuffType<ArmorCrunch>(), 240);
            target.AddBuff(ModContent.BuffType<MarkedforDeath>(), 240);
            if (projectile.Calamity().stealthStrike)
            {
                target.AddBuff(ModContent.BuffType<CrushDepth>(), 240);
                Projectile.NewProjectile(projectile.Center, Vector2.Zero, ModContent.ProjectileType<JawsShockwave>(), (int)(100f * (player.allDamage + player.Calamity().throwingDamage - 1f)), 10f, projectile.owner, 0, 0);
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Collision.HitTiles(projectile.position + projectile.velocity, projectile.velocity, projectile.width, projectile.height);
            Main.PlaySound(0, projectile.position);
            projectile.Kill();
            return false;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityGlobalProjectile.DrawCenteredAndAfterimage(projectile, lightColor, ProjectileID.Sets.TrailingMode[projectile.type], 1);
            return false;
        }

        public override void Kill(int timeLeft)
        {
            for (int i = 0; i < 5; i++)
            {
                int dustToUse = Main.rand.Next(0, 4);
                int dustType = 0;
                switch (dustToUse)
                {
                    case 0:
                        dustType = 33;
                        break;
                    case 1:
                        dustType = 101;
                        break;
                    case 2:
                        dustType = 111;
                        break;
                    case 3:
                        dustType = 180;
                        break;
                }

                int dust = Dust.NewDust(projectile.Center, 1, 1, dustType, 0, 0, 0, default, 1.5f);
                Main.dust[dust].noGravity = true;
            }
        }
    }
}
