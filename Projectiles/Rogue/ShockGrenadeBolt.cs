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
    public class ShockGrenadeBolt : ModProjectile
    {
        public static int frameWidth = 12;
        public static int frameHeight = 26;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Bolt");
            Main.projFrames[projectile.type] = 4;
        }

        public override void SetDefaults()
        {
            projectile.width = 10;
            projectile.height = 10;
            projectile.friendly = true;
            projectile.tileCollide = false;
            projectile.penetrate = 3;
            projectile.timeLeft = 120;
            projectile.Calamity().rogue = true;
        }

        public override void AI()
        {
            projectile.frameCounter++;
            if (projectile.frameCounter > 6)
            {
                projectile.frame++;
                projectile.frameCounter = 0;
            }
            if (projectile.frame >= Main.projFrames[projectile.type])
            {
                projectile.frame = 0;
            }

            projectile.rotation = (float)Math.Atan2((double)projectile.velocity.Y, (double)projectile.velocity.X) + 1.57f;

            if (projectile.timeLeft < 55)
            {
                projectile.tileCollide = true;
            }

            if (projectile.ai[1] == 1f)
            {
                float minDist = 999f;
                int index = 0;
                for (int i = 0; i < Main.npc.Length; i++)
                {
                    NPC npc = Main.npc[i];
                    if (!npc.friendly && !npc.townNPC && npc.active && !npc.dontTakeDamage && npc.chaseable)
                    {
                        float dist = (projectile.Center - npc.Center).Length();
                        if (dist < minDist)
                        {
                            minDist = dist;
                            index = i;
                        }
                    }
                }

                Vector2 velocityNew;
                if (minDist < 999f)
                {
                    velocityNew = Main.npc[index].Center - projectile.Center;
                    velocityNew.Normalize();
                    velocityNew *= 2f;
                    projectile.velocity += velocityNew;
                    if (projectile.velocity.Length() > 10f)
                    {
                        projectile.velocity.Normalize();
                        projectile.velocity *= 10f;
                    }
                }
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.Electrified, 180);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D sprite;
            if (projectile.ai[0] == 0f)
                sprite = ModContent.GetTexture("CalamityMod/Projectiles/Rogue/ShockGrenadeBolt");
            else
                sprite = ModContent.GetTexture("CalamityMod/Projectiles/Rogue/ShockGrenadeBolt2");
            Color drawColour = Color.White;

            Vector2 origin = new Vector2(frameWidth / 2, frameHeight / 2);
            spriteBatch.Draw(sprite, projectile.Center - Main.screenPosition, new Rectangle(0, frameHeight * projectile.frame, frameWidth, frameHeight), drawColour, projectile.rotation, origin, 1f, SpriteEffects.None, 0f);
            return false;
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(2, (int)projectile.position.X, (int)projectile.position.Y, 93, 0.25f, 0f);

            for (int i = 0; i < 5; i++)
            {
                int dustType = 132;
                int dust = Dust.NewDust(projectile.Center, 1, 1, dustType, projectile.velocity.X, projectile.velocity.Y, 0, default, 0.5f);
                Main.dust[dust].noGravity = true;
            }
        }
    }
}
