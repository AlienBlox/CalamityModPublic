﻿using CalamityMod.CalPlayer;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Rogue
{
    public class ShatteredSunKnife : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Rogue";
        public override string Texture => "CalamityMod/Items/Weapons/Rogue/ShatteredSun";

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = 56;
            Projectile.height = 56;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = 4;
            Projectile.timeLeft = 300;
            Projectile.alpha = 255;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 15;
            Projectile.DamageType = RogueDamageClass.Instance;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.ToRadians(45f);
            Projectile.ai[1] += 1f;
            if (Projectile.ai[1] < 5f)
            {
                Projectile.alpha -= 50;
            }
            if (Projectile.ai[1] == 5f)
            {
                Projectile.alpha = 0;
                Projectile.tileCollide = false;
            }

            if (Projectile.ai[1] == 20f)
            {
                int numProj = 5;
                if (Projectile.owner == Main.myPlayer)
                {
                    int spread = 6;
                    int projID = ModContent.ProjectileType<ShatteredSunScorchedBlade>();
                    int splitDamage = (int)(0.75f * Projectile.damage);
                    float splitKB = 1f;
                    for (int i = 0; i < numProj; i++)
                    {
                        Vector2 perturbedspeed = new Vector2(Projectile.velocity.X, Projectile.velocity.Y + Main.rand.Next(-3, 4)).RotatedBy(MathHelper.ToRadians(spread));
                        int proj = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, perturbedspeed * 0.2f, projID, splitDamage, splitKB, Projectile.owner, 0f, 0f);
                        Main.projectile[proj].Calamity().stealthStrike = Projectile.Calamity().stealthStrike;
                        spread -= Main.rand.Next(2, 6);
                    }
                    SoundEngine.PlaySound(SoundID.Item27, Projectile.position);
                    Projectile.active = false;
                    for (int num621 = 0; num621 < 8; num621++)
                    {
                        int num622 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 127, 0f, 0f, 100, default, 2f);
                        Main.dust[num622].velocity *= 3f;
                        if (Main.rand.NextBool())
                        {
                            Main.dust[num622].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                        }
                    }
                    for (int num623 = 0; num623 < 16; num623++)
                    {
                        int num624 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 127, 0f, 0f, 100, default, 3f);
                        Main.dust[num624].noGravity = true;
                        Main.dust[num624].velocity *= 5f;
                        num624 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 127, 0f, 0f, 100, default, 2f);
                        Main.dust[num624].velocity *= 2f;
                    }
                }
            }

            if (Projectile.Calamity().stealthStrike)
            {
                float num472 = Projectile.Center.X;
                float num473 = Projectile.Center.Y;
                float num474 = 600f;
                for (int num475 = 0; num475 < Main.maxNPCs; num475++)
                {
                    if (Main.npc[num475].CanBeChasedBy(Projectile, false) && Collision.CanHit(Projectile.Center, 1, 1, Main.npc[num475].Center, 1, 1) && !CalamityPlayer.areThereAnyDamnBosses)
                    {
                        float npcCenterX = Main.npc[num475].position.X + (float)(Main.npc[num475].width / 2);
                        float npcCenterY = Main.npc[num475].position.Y + (float)(Main.npc[num475].height / 2);
                        float num478 = Math.Abs(Projectile.position.X + (float)(Projectile.width / 2) - npcCenterX) + Math.Abs(Projectile.position.Y + (float)(Projectile.height / 2) - npcCenterY);
                        if (num478 < num474)
                        {
                            if (Main.npc[num475].position.X < num472)
                            {
                                Main.npc[num475].velocity.X += 0.25f;
                            }
                            else
                            {
                                Main.npc[num475].velocity.X -= 0.25f;
                            }
                            if (Main.npc[num475].position.Y < num473)
                            {
                                Main.npc[num475].velocity.Y += 0.25f;
                            }
                            else
                            {
                                Main.npc[num475].velocity.Y -= 0.25f;
                            }
                        }
                    }
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1);
            return false;
        }

        private void ShatteredExplosion()
        {
            int projID = ModContent.ProjectileType<ShatteredExplosion>();
            int explosionDamage = (int)(Projectile.damage * 0.45f);
            float explosionKB = 3f;
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, projID, explosionDamage, explosionKB, Projectile.owner, 0f, 0f);
            SoundEngine.PlaySound(SoundID.Item14, Projectile.position);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => ShatteredExplosion();

        public override void OnHitPlayer(Player target, Player.HurtInfo info) => ShatteredExplosion();

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            ShatteredExplosion();
            return true;
        }

        public override void Kill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item27, Projectile.position);
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, 246, Projectile.oldVelocity.X * 0.5f, Projectile.oldVelocity.Y * 0.5f, 0, default, 1f);
            }
        }
    }
}
