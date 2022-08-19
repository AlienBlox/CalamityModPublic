﻿using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using CalamityMod.Items.Weapons.Melee;
using ReLogic.Utilities;

namespace CalamityMod.Projectiles.Melee.Yoyos
{
    public class MicrowaveYoyo : ModProjectile
    {
        private const float Radius = 100f;
        private SlotId mmmmmm;
        private bool spawnedAura = false;
        public int soundCooldown = 0;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("The Microwave");
            ProjectileID.Sets.YoyosLifeTimeMultiplier[Projectile.type] = -1f;
            ProjectileID.Sets.YoyosMaximumRange[Projectile.type] = 450f;
            ProjectileID.Sets.YoyosTopSpeed[Projectile.type] = 14f;

            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.aiStyle = 99;
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.scale = 1f;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.MeleeNoSpeed;
            Projectile.penetrate = -1;
            Projectile.MaxUpdates = 2;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20;
        }

        public override void SendExtraAI(BinaryWriter writer) => writer.Write(soundCooldown);
        public override void ReceiveExtraAI(BinaryReader reader) => soundCooldown = reader.ReadInt32();

        public override void AI()
        {
            // Sound is done manually, so that it can loop correctly.
            ActiveSound MMMMMMMMMMMMMMM;

            bool mmmIsThere = SoundEngine.TryGetActiveSound(mmmmmm, out MMMMMMMMMMMMMMM);

            if (!mmmIsThere)
            {
                mmmmmm = SoundEngine.PlaySound(TheMicrowave.MMMSound, Projectile.Center);
            }

            else if (mmmIsThere)
            {
                if (MMMMMMMMMMMMMMM.IsPlaying)
                    MMMMMMMMMMMMMMM.Position = Projectile.Center;

                else
                    MMMMMMMMMMMMMMM.Resume();
            }

            // Spawn invisible but damaging aura projectile
            if (Projectile.owner == Main.myPlayer && !spawnedAura)
            {
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<MicrowaveAura>(), (int)(Projectile.damage * 0.35), Projectile.knockBack, Projectile.owner, Projectile.identity, 0f);
                spawnedAura = true;
            }

            // Decrement sound cooldown
            if (soundCooldown > 0 && Projectile.FinalExtraUpdate())
                soundCooldown--;

            // Dust circle appears for all players, even though the aura projectile is only spawned by the owner
            int numDust = (int)(0.2f * MathHelper.TwoPi * Radius);
            float angleIncrement = MathHelper.TwoPi / (float)numDust;
            Vector2 dustOffset = new Vector2(Radius, 0f);
            dustOffset = dustOffset.RotatedByRandom(MathHelper.TwoPi);
            for (int i = 0; i < numDust; i++)
            {
                dustOffset = dustOffset.RotatedBy(angleIncrement);
                int dustType = Utils.SelectRandom(Main.rand, new int[]
                {
                        ModContent.DustType<AstralOrange>(),
                        ModContent.DustType<AstralBlue>()
                });
                int dust = Dust.NewDust(Projectile.Center, 1, 1, dustType);
                Main.dust[dust].position = Projectile.Center + dustOffset;
                Main.dust[dust].fadeIn = 1f;
                Main.dust[dust].velocity *= 0.2f;
                Main.dust[dust].scale = 0.1599999999f;
            }

            // Delete the projectile if it is farther than 200 blocks away from the player
            if ((Projectile.position - Main.player[Projectile.owner].position).Length() > 3200f)
                Projectile.Kill();
        }

        public override void Kill(int timeLeft)
        {
            ActiveSound MMMMMMMMMMMMMMM;
            if (SoundEngine.TryGetActiveSound(mmmmmm, out MMMMMMMMMMMMMMM))
            {
                MMMMMMMMMMMMMMM.Stop();
                //No more dispose function?
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1);
            return false;
        }

        public override void PostDraw(Color lightColor)
        {
            Rectangle frame = new Rectangle(0, 0, 20, 16);
            Main.EntitySpriteDraw(ModContent.Request<Texture2D>("CalamityMod/Projectiles/Melee/Yoyos/MicrowaveYoyoGlow").Value, Projectile.Center - Main.screenPosition, frame, Color.White, Projectile.rotation, Projectile.Size / 2, 1f, SpriteEffects.None, 0);
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<AstralInfectionDebuff>(), 240);
            if (target.life <= 0 && soundCooldown <= 0)
            {
                SoundEngine.PlaySound(TheMicrowave.BeepSound, Projectile.Center);
                soundCooldown = 45;
            }
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            target.AddBuff(ModContent.BuffType<AstralInfectionDebuff>(), 240);
            if (target.statLife <= 0 && soundCooldown <= 0)
            {
                SoundEngine.PlaySound(TheMicrowave.BeepSound, Projectile.Center);
                soundCooldown = 45;
            }
        }
    }
}
