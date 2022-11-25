﻿using System;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Dusts;
using CalamityMod.Enums;
using CalamityMod.Items.Accessories;
using CalamityMod.Projectiles.Typeless;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;

namespace CalamityMod.CalPlayer.Dashes
{
    public class AsgardianAegisDash : PlayerDashEffect
    {
        public static new string ID => "Asgardian Aegis";

        public override DashCollisionType CollisionType => DashCollisionType.ShieldSlam;
        public override bool IsOmnidirectional => false;

        public override float CalculateDashSpeed(Player player) => 23.3f;

        public override void OnDashEffects(Player player)
        {
            // Spawn fire dust around the player's body.
            for (int d = 0; d < 60; d++)
            {
                Dust holyFireDashDust = Dust.NewDustDirect(player.position, player.width, player.height, 246, 0f, 0f, 100, default, 3f);
                holyFireDashDust.position += Main.rand.NextVector2Square(-5f, 5f);
                holyFireDashDust.velocity += Main.rand.NextVector2Circular(5f, 5f);
                holyFireDashDust.velocity *= 0.75f;
                holyFireDashDust.scale *= Main.rand.NextFloat(1f, 1.2f);
                holyFireDashDust.shader = GameShaders.Armor.GetSecondaryShader(player.ArmorSetDye(), player);
                holyFireDashDust.noGravity = true;
                holyFireDashDust.fadeIn = 0.5f;
            }
        }

        public override void MidDashEffects(Player player, ref float dashSpeed, ref float dashSpeedDecelerationFactor, ref float runSpeedDecelerationFactor)
        {
            // Spawn fire dust around the player's body.
            for (int d = 0; d < 8; d++)
            {
                int dashDustID = Main.rand.Next(new int[]
                {
                    (int)CalamityDusts.BlueCosmilite,
                    (int)CalamityDusts.PurpleCosmilite,
                    (int)CalamityDusts.ProfanedFire
                });
                Dust fireDashDust = Dust.NewDustDirect(player.position + Vector2.UnitY * 4f, player.width, player.height - 8, dashDustID, 0f, 0f, 100, default, 2f);
                fireDashDust.velocity *= 0.1f;
                fireDashDust.scale *= Main.rand.NextFloat(1f, 1.2f);
                fireDashDust.shader = GameShaders.Armor.GetSecondaryShader(player.ArmorSetDye(), player);
                fireDashDust.noGravity = true;
                if (Main.rand.NextBool(2))
                    fireDashDust.fadeIn = 0.5f;
            }

            // Dash at a faster speed than the default value.
            dashSpeed = 16f;
        }

        public override void OnHitEffects(Player player, NPC npc, IEntitySource source, ref DashHitContext hitContext)
        {
            float kbFactor = 15f;
            bool crit = Main.rand.Next(100) < player.GetCritChance<MeleeDamageClass>();
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
            hitContext.PlayerImmunityFrames = AsgardianAegis.ShieldSlamIFrames;
            hitContext.Damage = (int)player.GetTotalDamage<MeleeDamageClass>().ApplyTo(300f);

            int supremeExplosionDamage = (int)player.GetBestClassDamage().ApplyTo(135);
            Projectile.NewProjectile(source, player.Center, Vector2.Zero, ModContent.ProjectileType<HolyExplosionSupreme>(), supremeExplosionDamage, 20f, Main.myPlayer, 0f, 0f);
            npc.AddBuff(ModContent.BuffType<GodSlayerInferno>(), 300);
        }
    }
}
