﻿using System;
using CalamityMod.Items.Accessories;
using CalamityMod.Projectiles.Typeless;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;

namespace CalamityMod.CalPlayer.Dashes
{
    public class ElysianAegisDash : PlayerDashEffect
    {
        public static new string ID => "Elysian Aegis";

        public override bool IsOmnidirectional => false;

        public override float CalculateDashSpeed(Player player) => 21.5f;

        public override void OnDashEffects(Player player)
        {
            // Spawn fire dust around the player's body.
            for (int d = 0; d < 20; d++)
            {
                Dust holyFireDashDust = Dust.NewDustDirect(player.position, player.width, player.height, 246, 0f, 0f, 100, default, 3f);
                holyFireDashDust.position += Main.rand.NextVector2Square(-5f, 5f);
                holyFireDashDust.velocity *= 0.2f;
                holyFireDashDust.scale *= Main.rand.NextFloat(1f, 1.2f);
                holyFireDashDust.shader = GameShaders.Armor.GetSecondaryShader(player.ArmorSetDye(), player);
                holyFireDashDust.noGravity = true;
                holyFireDashDust.fadeIn = 0.5f;
            }
        }

        public override void MidDashEffects(Player player, ref float dashSpeed, ref float dashSpeedDecelerationFactor, ref float runSpeedDecelerationFactor)
        {
            // Spawn fire dust around the player's body.
            for (int d = 0; d < 4; d++)
            {
                Dust holyFireDashDust = Dust.NewDustDirect(player.position + Vector2.UnitY * 4f, player.width, player.height - 8, 246, 0f, 0f, 100, default, 2.75f);
                holyFireDashDust.velocity *= 0.1f;
                holyFireDashDust.scale *= Main.rand.NextFloat(1f, 1.2f);
                holyFireDashDust.shader = GameShaders.Armor.GetSecondaryShader(player.ArmorSetDye(), player);
                holyFireDashDust.noGravity = true;
                if (Main.rand.NextBool(2))
                    holyFireDashDust.fadeIn = 0.5f;
            }

            // Dash at a faster speed than the default value.
            dashSpeed = 14f;
        }

        public override void OnHitEffects(Player player, NPC npc, IEntitySource source, ref DashHitContext hitContext)
        {
            float kbFactor = 12f;
            bool crit = Main.rand.Next(100) < player.GetCritChance(DamageClass.Melee);
            if (player.kbGlove)
                kbFactor *= 2f;
            if (player.kbBuff)
                kbFactor *= 1.5f;

            int hitDirection = player.direction;
            if (player.velocity.X != 0f)
                hitDirection = Math.Sign(player.velocity.X);

            // Define hit context variables.
            hitContext.CriticalHit = crit;
            hitContext.HitDirection = hitDirection;
            hitContext.KnockbackFactor = kbFactor;
            hitContext.PlayerImmunityFrames = ElysianAegis.ShieldSlamIFrames;
            hitContext.Damage = (int)player.GetDamage<MeleeDamageClass>().ApplyTo(250f);

            int supremeExplosionDamage = (int)player.GetBestClassDamage().ApplyTo(120);
            int holyEruptionDamage = (int)player.GetBestClassDamage().ApplyTo(80);
            Projectile.NewProjectile(source, player.Center, Vector2.Zero, ModContent.ProjectileType<HolyExplosionSupreme>(), supremeExplosionDamage, 15f, Main.myPlayer, 0f, 0f);
            Projectile.NewProjectile(source, player.Center, Vector2.Zero, ModContent.ProjectileType<HolyEruption>(), holyEruptionDamage, 5f, Main.myPlayer, 0f, 0f);
        }
    }
}
