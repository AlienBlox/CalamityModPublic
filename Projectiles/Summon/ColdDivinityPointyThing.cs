﻿using CalamityMod.CalPlayer;
using Microsoft.Xna.Framework;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Summon
{
    public class ColdDivinityPointyThing : ModProjectile
    {
        public int recharging = -1;
        public bool circling = true;
        public bool circlingPlayer = true;
        public float floatyDistance = 90f;
        public NPC target = null;

        private void homingAi()
        {
            if (Projectile.timeLeft <= 240)
            {
                if (target != null)
                {
                    target.checkDead();
                    if (target.life <= 0 || !target.active || !target.CanBeChasedBy(this, false))
                        target = null;
                }
                if (target == null)
                    target = CalamityUtils.MinionHoming(Projectile.Center, 1000f, Main.player[Projectile.owner]);
                if (target != null) //target found
                {
                    float num550 = 40f;
                    Vector2 vector43 = new Vector2(Projectile.position.X + (float)Projectile.width * 0.5f, Projectile.position.Y + (float)Projectile.height * 0.5f);
                    float num551 = target.Center.X - vector43.X;
                    float num552 = target.Center.Y - vector43.Y;
                    float num553 = (float)Math.Sqrt((double)(num551 * num551 + num552 * num552));
                    if (num553 < 100f)
                    {
                        num550 = 28f; //14
                    }
                    num553 = num550 / num553;
                    num551 *= num553;
                    num552 *= num553;
                    Projectile.velocity.X = (Projectile.velocity.X * 20f + num551) / 21f;
                    Projectile.velocity.Y = (Projectile.velocity.Y * 20f + num552) / 21f;
                }
            }
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ice Castle Shard");
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 7;
        }

        public override void SetDefaults()
        {
            Projectile.width = 28;
            Projectile.height = 60;
            Projectile.netImportant = true;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.minionSlots = 0f;
            Projectile.timeLeft = 18000;
            Projectile.penetrate = 1;
            Projectile.tileCollide = false;
            Projectile.timeLeft *= 5;
            Projectile.minion = true;
            Projectile.DamageType = DamageClass.Summon;
        }
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(recharging);
            writer.Write(circling);
            writer.Write(circlingPlayer);
            writer.Write((double)floatyDistance);
            writer.Write(target is null ? -1 : target.whoAmI);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            recharging = reader.ReadInt32();
            circling = reader.ReadBoolean();
            circlingPlayer = reader.ReadBoolean();
            floatyDistance = (float)reader.ReadDouble();
            int targ = reader.ReadInt32();
            target = targ == -1 ? null : Main.npc[targ];
        }

        private void dust(int dustAmt)
        {
            for (int i = 0; i < dustAmt; i++)
            {
                Dust.NewDust(Projectile.Center, Projectile.width, Projectile.height, DustID.Ice, Main.rand.NextFloat(1, 3), Main.rand.NextFloat(1, 3), 0, Color.Cyan, Main.rand.NextFloat(0.5f, 1.5f));
            }
        }

        public override bool PreAI()
        {
            if (recharging == -1)
            {
                recharging = Projectile.ai[1] == 0f ? 210 : 0;
                dust(30);
            }
            if (Projectile.ai[1] == 1f && Projectile.timeLeft > 1000)
            {
                Projectile.ai[1] = 0f;
                Projectile.timeLeft = 250;
                circling = circlingPlayer = false;
                Projectile.netUpdate = true;
            }
            else if (Projectile.ai[1] >= 2f && Projectile.timeLeft > 900)
            {
                target = CalamityUtils.MinionHoming(Projectile.Center, 1000f, Main.player[Projectile.owner]);
                if (target != null)
                {
                    Projectile.timeLeft = 669;
                    Projectile.ai[1]++;
                    circlingPlayer = false;
                    float height = target.getRect().Height;
                    float width = target.getRect().Width;
                    floatyDistance = MathHelper.Min((height > width ? height : width) * 3f, (Main.LogicCheckScreenWidth * Main.LogicCheckScreenHeight) / 2);
                    if (floatyDistance > Main.LogicCheckScreenWidth / 3)
                        floatyDistance = Main.LogicCheckScreenWidth / 3;
                    Projectile.penetrate = -1;
                    Projectile.usesIDStaticNPCImmunity = true;
                    Projectile.idStaticNPCHitCooldown = 4;
                    Projectile.netUpdate = true;
                }
            }
            if (circlingPlayer)
            {
                ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
                if (Projectile.penetrate == 1)
                {
                    Projectile.penetrate++;
                }
            }
            return true;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            CalamityPlayer modPlayer = player.Calamity();
            if (player.dead)
            {
                modPlayer.coldDivinity = false;
            }
            if (circlingPlayer)
            {
                Projectile.minionSlots = 1f;
                Projectile.timeLeft = 2;
                if (!modPlayer.coldDivinity && recharging > 0)
                    Projectile.Kill();

            }
            if (circling && !circlingPlayer)
            {
                if (target != null && (!target.active || target.life <= 0))
                {
                    Projectile.Kill();
                }
            }
            if (recharging > 0)
            {
                recharging--;
                if (recharging == 0)
                {
                    dust(15);
                    SoundEngine.PlaySound(SoundID.Item30 with { Pitch = 0.2f }, Projectile.position);
                    Projectile.netUpdate = true;
                }
            }
            if (circling)
            {
                if (circling && !circlingPlayer && Projectile.timeLeft < 120)
                {
                    recharging = 0;
                    Projectile.usesIDStaticNPCImmunity = false;
                    Projectile.penetrate = 1;
                    float applicableDist = target.getRect().Width > target.getRect().Height ? target.getRect().Width : target.getRect().Height;
                    if (Projectile.timeLeft > 60)
                        floatyDistance += 5;
                    else
                        floatyDistance -= 10;
                }
                if (circlingPlayer)
                {
                    float math = recharging == 0 ? 90f :(300 - recharging) / 3;
                    float regularDistance = math > 90f ? 90f : math;
                    Projectile.Center = player.Center + Projectile.ai[0].ToRotationVector2() * regularDistance;
                    Projectile.rotation = Projectile.ai[0] + (float)Math.Atan(90);
                    Projectile.ai[0] -= MathHelper.ToRadians(4f);
                    NPC target = recharging > 0 ? null : CalamityUtils.MinionHoming(Projectile.Center, 800f, player);
                    if (target != null && Projectile.owner == Main.myPlayer)
                    {
                        recharging = 180;
                        Vector2 velocity = Projectile.ai[0].ToRotationVector2().RotatedBy(Math.Atan(0));
                        velocity.Normalize();
                        velocity *= 20f;
                        int shard = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position, velocity, Projectile.type, (int)(Projectile.damage * 1.05f), Projectile.knockBack, Projectile.owner, Projectile.ai[0], 1f);
                        if (Main.projectile.IndexInRange(shard))
                            Main.projectile[shard].originalDamage = (int)(Projectile.originalDamage * 1.05f);
                    }
                    Projectile.netUpdate = Projectile.owner == Main.myPlayer;
                }
                else
                {
                    Projectile.Center = target.Center + Projectile.ai[0].ToRotationVector2() * floatyDistance;
                    Projectile.rotation = Projectile.ai[0] + (float)Math.Atan(90);
                    Vector2 vec = Projectile.rotation.ToRotationVector2() - target.Center;
                    vec.Normalize();
                    if (Projectile.timeLeft <= 120)
                        Projectile.rotation = Projectile.timeLeft <= 60 ? Projectile.ai[0] - (float)Math.Atan(90) : Projectile.rotation - (MathHelper.Distance(Projectile.rotation, -Projectile.rotation) / (120 - 60));
                    Projectile.ai[0] -= MathHelper.ToRadians(4f);
                }
            }
            else
            {
                Projectile.rotation = Projectile.velocity.ToRotation() + (float)Math.Atan(90);
                homingAi();
            }
        }

        public override bool? CanDamage()
        {
            return recharging <= 0 && (circlingPlayer || (circling && (Projectile.timeLeft >= 120 || Projectile.timeLeft <= 45)) || !circling) && !Projectile.hide ? null : false;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.Frostburn2, 300);
            int circlers = 0;
            for (int i = 0; i < Main.projectile.Length; i++)
            {
                if (Main.projectile[i].active && Main.projectile[i].owner == Projectile.owner && Main.projectile[i].type == Projectile.type)
                {
                    ColdDivinityPointyThing pointy = (ColdDivinityPointyThing)Main.projectile[i].ModProjectile;
                    if (Main.projectile[i].ai[1] > 2f)
                        circlers += Main.rand.Next(1, 4);
                }
            }
            circlers = (int)MathHelper.Min(Main.rand.Next(15, 21), circlers);
            if (Projectile.ai[1] > 2f)
                Projectile.ai[1]++;
            if (Projectile.ai[1] >= (30f - circlers) && Projectile.timeLeft >= 120)
                recharging = Projectile.timeLeft > 121 ? Projectile.timeLeft - 121 : 0;

            if (circling && target == this.target && Projectile.timeLeft < 60)
            {
                if (Projectile.timeLeft < 60)
                    Projectile.Kill();
            }
            else if (circlingPlayer)
            {
                recharging = 300;
                dust(20);
            }
        }

        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            if (circling && target == this.target && Projectile.timeLeft < 60)
            {
                dust(30);
                SoundEngine.PlaySound(SoundID.NPCHit5, Projectile.position);
                damage = (int)(damage * 1.1f);
            }
            else if (circling && target == this.target && Projectile.timeLeft > 60)
            {
                dust(5);
                damage = (int)(damage * 0.2f); //nerfffffff the nerf because nerf? nerf.
            }
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            target.AddBuff(BuffID.Frostburn2, 300);
            if (circlingPlayer)
            {
                recharging = 300;
                dust(20);
            }
        }

        public override void Kill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.NPCHit5, Projectile.position);
            dust(20);
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(recharging > 0 ? lightColor.R : 53, recharging > 0 ? lightColor.G : Main.DiscoG, recharging > 0 ? lightColor.B : 255, recharging > 200 ? 255 : 255 - recharging);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (!circling || (!circlingPlayer && recharging == 0))
            {
                CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, !circlingPlayer ? 1 : 3);
            }
            return true;
        }
    }
}
