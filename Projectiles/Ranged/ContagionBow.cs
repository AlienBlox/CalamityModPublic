﻿using CalamityMod.Items.Weapons.Ranged;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Ranged
{
    public class ContagionBow : ModProjectile
    {
        public override LocalizedText DisplayName => CalamityUtils.GetItemName<Contagion>();
        public override string Texture => "CalamityMod/Items/Weapons/Ranged/Contagion";

        public override void SetDefaults()
        {
            Projectile.width = 42;
            Projectile.height = 84;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.DamageType = DamageClass.Ranged;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            Vector2 vector = player.RotatedRelativePoint(player.MountedCenter, true);
            if (Projectile.type == ModContent.ProjectileType<ContagionBow>())
            {
                Projectile.ai[0] += 1f;
                int num2 = 0;
                if (Projectile.ai[0] >= 40f)
                {
                    num2++;
                }
                if (Projectile.ai[0] >= 80f)
                {
                    num2++;
                }
                if (Projectile.ai[0] >= 120f)
                {
                    num2++;
                }
                int num3 = 24;
                int num4 = 6;
                Projectile.ai[1] += 1f;
                bool flag = false;
                if (Projectile.ai[1] >= (float)(num3 - num4 * num2))
                {
                    Projectile.ai[1] = 0f;
                    flag = true;
                }
                Projectile.frameCounter += 1 + num2;
                if (Projectile.frameCounter >= 4)
                {
                    Projectile.frameCounter = 0;
                    Projectile.frame++;
                    if (Projectile.frame >= 3)
                    {
                        Projectile.frame = 0;
                    }
                }
                if (Projectile.soundDelay <= 0)
                {
                    Projectile.soundDelay = num3 - num4 * num2;
                    if (Projectile.ai[0] != 1f)
                    {
                        SoundEngine.PlaySound(SoundID.Item5, Projectile.Center);
                    }
                }
                if (Projectile.ai[1] == 1f && Projectile.ai[0] != 1f)
                {
                    Vector2 vector2 = Vector2.UnitX * 24f;
                    vector2 = vector2.RotatedBy((double)(Projectile.rotation - MathHelper.PiOver2), default);
                    Vector2 value = Projectile.Center + vector2;
                    for (int i = 0; i < 2; i++)
                    {
                        int num5 = Dust.NewDust(value - Vector2.One * 8f, 16, 16, 44, Projectile.velocity.X / 2f, Projectile.velocity.Y / 2f, 100, default, 0.25f);
                        Main.dust[num5].velocity *= 0.66f;
                        Main.dust[num5].noGravity = true;
                        Main.dust[num5].scale = 1.4f;
                    }
                }
                if (flag && Main.myPlayer == Projectile.owner)
                {
                    bool flag2 = player.channel && !player.noItems && !player.CCed;
                    if (flag2)
                    {
                        float speed = player.ActiveItem().shootSpeed * Projectile.scale;
                        Vector2 spawnPos = vector;
                        Vector2 direction = Main.screenPosition + new Vector2((float)Main.mouseX, (float)Main.mouseY) - spawnPos;
                        if (player.gravDir == -1f)
                        {
                            direction.Y = (float)(Main.screenHeight - Main.mouseY) + Main.screenPosition.Y - spawnPos.Y;
                        }
                        Vector2 velocity = Vector2.Normalize(direction);
                        if (float.IsNaN(velocity.X) || float.IsNaN(velocity.Y))
                        {
                            velocity = -Vector2.UnitY;
                        }
                        velocity *= speed;
                        if (velocity.X != Projectile.velocity.X || velocity.Y != Projectile.velocity.Y)
                        {
                            Projectile.netUpdate = true;
                        }
                        Projectile.velocity = velocity;
                        int projType = ModContent.ProjectileType<ContagionArrow>();
                        float velocityMult = 14f;
                        float randNum = 7f;
                        spawnPos += new Vector2(Main.rand.NextFloat(-randNum, randNum), Main.rand.NextFloat(-randNum, randNum));
                        Vector2 spinningpoint = Vector2.Normalize(Projectile.velocity) * velocityMult;
                        spinningpoint = spinningpoint.RotatedBy(Main.rand.NextDouble() * 0.2 - 0.1, default);
                        if (float.IsNaN(spinningpoint.X) || float.IsNaN(spinningpoint.Y))
                        {
                            spinningpoint = -Vector2.UnitY;
                        }
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), spawnPos, spinningpoint, projType, Projectile.damage, Projectile.knockBack, Projectile.owner, 0f, 0f);
                    }
                    else
                    {
                        Projectile.Kill();
                    }
                }
            }
            Projectile.rotation = Projectile.velocity.ToRotation();
            Vector2 displayOffset = new Vector2(5f, 0f).RotatedBy(Projectile.rotation);
            Projectile.Center = player.RotatedRelativePoint(player.MountedCenter, true) + displayOffset;
            if (Projectile.spriteDirection == -1)
                Projectile.rotation += MathHelper.Pi;
            Projectile.spriteDirection = Projectile.direction;
            Projectile.timeLeft = 2;
            player.ChangeDir(Projectile.direction);
            player.heldProj = Projectile.whoAmI;
            player.itemTime = 2;
            player.itemAnimation = 2;
            player.itemRotation = (float)Math.Atan2((double)(Projectile.velocity.Y * (float)Projectile.direction), (double)(Projectile.velocity.X * (float)Projectile.direction));
        }

        public override bool? CanDamage() => false;

        public override bool PreDraw(ref Color lightColor) => Projectile.ai[0] > 0f;
    }
}
