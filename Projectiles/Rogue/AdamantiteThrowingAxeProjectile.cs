﻿using CalamityMod.Items.Weapons.Rogue;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Rogue
{
    public class AdamantiteThrowingAxeProjectile : ModProjectile
    {
        public override string Texture => "CalamityMod/Items/Weapons/Rogue/AdamantiteThrowingAxe";

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Adamantite Throwing Axe");
        }

        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.friendly = true;
            Projectile.penetrate = 4;
            Projectile.aiStyle = ProjAIStyleID.ThrownProjectile;
            Projectile.timeLeft = 600;
            AIType = ProjectileID.Shuriken;
            Projectile.DamageType = RogueDamageClass.Instance;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(lightColor), Projectile.rotation, tex.Size() / 2f, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            OnHitEffects();
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            OnHitEffects();
        }

        private void OnHitEffects()
        {
            if (Projectile.Calamity().stealthStrike && Main.myPlayer == Projectile.owner)
            {
                var source = Projectile.GetSource_FromThis();
                for (int n = 0; n < 3; n++)
                {
                    Projectile lightning = CalamityUtils.ProjectileRain(source, Projectile.Center, 400f, 100f, -800f, -500f, 8f, ModContent.ProjectileType<BlunderBoosterLightning>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                    lightning.ai[0] = Main.rand.Next(2);
                }
            }
        }
    }
}
