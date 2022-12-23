﻿using CalamityMod.Events;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Boss
{
    public class MushBombFall : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Falling Mushroom");
            Main.projFrames[Projectile.type] = 4;
        }

        public override void SetDefaults()
        {
            Projectile.width = 14;
            Projectile.height = 14;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 600;
        }

        public override void AI()
        {
            Projectile.frameCounter++;
            if (Projectile.frameCounter > 4)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
            }
            if (Projectile.frame > 3)
                Projectile.frame = 0;

            if (Projectile.position.Y > Projectile.ai[1])
                Projectile.tileCollide = true;

            Projectile.rotation = (float)Math.Atan2(Projectile.velocity.Y, Projectile.velocity.X) + MathHelper.PiOver2;

            Lighting.AddLight(Projectile.Center, 0f, 0.15f, 0.3f);

            float velocityYLimit = BossRushEvent.BossRushActive ? 12f : CalamityWorld.death ? 6f : 5f;
            float velocityYIncrement = BossRushEvent.BossRushActive ? 0.24f : CalamityWorld.death ? 0.12f : 0.1f;
            if (Projectile.velocity.Y < velocityYLimit)
                Projectile.velocity.Y += velocityYIncrement;

            if (Math.Abs(Projectile.velocity.X) > 2f)
                Projectile.velocity.X *= 0.995f;
        }

        public override bool CanHitPlayer(Player target) => Projectile.velocity.Y >= 0f;

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            int height = texture.Height / Main.projFrames[Projectile.type];
            int drawStart = height * Projectile.frame;
            Vector2 origin = Projectile.Size / 2;
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (Projectile.spriteDirection == -1)
                spriteEffects = SpriteEffects.FlipHorizontally;

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, new Microsoft.Xna.Framework.Rectangle?(new Rectangle(0, drawStart, texture.Width, height)), Color.White, Projectile.rotation, origin, Projectile.scale, spriteEffects, 0);
            return false;
        }

        public override void PostDraw(Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            int height = texture.Height / Main.projFrames[Projectile.type];
            int drawStart = height * Projectile.frame;
            Vector2 origin = Projectile.Size / 2;
            Main.EntitySpriteDraw(ModContent.Request<Texture2D>("CalamityMod/Projectiles/Boss/MushBombGlow").Value, Projectile.Center - Main.screenPosition, new Microsoft.Xna.Framework.Rectangle?(new Rectangle(0, drawStart, texture.Width, height)), Color.White, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);
        }

        public override void Kill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.NPCDeath1, Projectile.position);
            Projectile.position.X = Projectile.position.X + (float)(Projectile.width / 2);
            Projectile.position.Y = Projectile.position.Y + (float)(Projectile.height / 2);
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.position.X = Projectile.position.X - (float)(Projectile.width / 2);
            Projectile.position.Y = Projectile.position.Y - (float)(Projectile.height / 2);
            for (int num621 = 0; num621 < 4; num621++)
            {
                int num622 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 56, 0f, 0f, 100, default, 2f);
                Main.dust[num622].velocity *= 1.5f;
                if (Main.rand.NextBool(2))
                {
                    Main.dust[num622].scale = 0.5f;
                    Main.dust[num622].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                }
            }
            for (int num623 = 0; num623 < 12; num623++)
            {
                int num624 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 56, 0f, 0f, 100, default, 3f);
                Main.dust[num624].noGravity = true;
                Main.dust[num624].velocity *= 2f;
                Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 56, 0f, 0f, 100, default, 2f);
            }

            if (Main.getGoodWorld && NPC.CountNPCS(NPCID.Crab) < 20 && Main.netMode != NetmodeID.MultiplayerClient) // move to zenith seed later
            {
                int crab = NPC.NewNPC(Projectile.GetSource_FromAI(), (int)Projectile.Center.X, (int)Projectile.Center.Y, NPCID.Crab);
                if (crab.WithinBounds(Main.maxNPCs))
                {
                    Main.projectile[crab].timeLeft = 1200;
                }
            }
        }
    }
}
