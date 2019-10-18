﻿using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Projectiles.Rogue
{
    public class KylieBoomerang : ModProjectile
    {
        //This variable will be used for the stealth strike
        public float ReboundTime = 0f;
        public float timer = 0f;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Kylie");
        }

        public override void SetDefaults()
        {
            projectile.friendly = true;
            projectile.width = 40;
            projectile.height = 40;
            projectile.penetrate = -1;
            projectile.timeLeft = 360;
            projectile.tileCollide = false;
            
            projectile.Calamity().rogue = true;
        }

        public override void AI()
        {

            //Constant rotation
            projectile.rotation += 0.2f;

            timer++;
            //Dust trail
            if (Main.rand.Next(15) == 0)
            {
                int d = Dust.NewDust(projectile.position, projectile.width, projectile.height, 7, projectile.velocity.X * 0.25f, projectile.velocity.Y * 0.25f, 100, default, 0f);
                Main.dust[d].position = projectile.Center;
            }
            //Constant sound effects
            if (projectile.soundDelay == 0)
            {
                projectile.soundDelay = 15;
                Main.PlaySound(SoundID.Item7, projectile.position);
            }
            //Slopes REEEEEEEEEEEE
            if (timer == 3f)
                projectile.tileCollide = true;
            //Decide the range of the boomerang depending on stealth
            if (projectile.Calamity().stealthStrike)
                ReboundTime = 25f;
            else
                ReboundTime = 50f;
            
            // ai[0] stores whether the boomerang is returning. If 0, it isn't. If 1, it is.
            if (projectile.ai[0] == 0f)
            {
                projectile.ai[1] += 1f;
                if (projectile.ai[1] >= ReboundTime)
                {
                    projectile.ai[0] = 1f;
                    projectile.ai[1] = 0f;
                    projectile.netUpdate = true;
                }
            }
            else
            {
                projectile.tileCollide = false;
                float returnSpeed = Kylie.Speed * 1.5f;
                float acceleration = 3.2f;
                Player owner = Main.player[projectile.owner];

                // Delete the boomerang if it's excessively far away.
                Vector2 playerCenter = owner.Center;
                float xDist = playerCenter.X - projectile.Center.X;
                float yDist = playerCenter.Y - projectile.Center.Y;
                float dist = (float)Math.Sqrt((double)(xDist * xDist + yDist * yDist));
                if (dist > 3000f)
                    projectile.Kill();

                dist = returnSpeed / dist;
                xDist *= dist;
                yDist *= dist;

                // Home back in on the player.
                if (projectile.velocity.X < xDist)
                {
                    projectile.velocity.X = projectile.velocity.X + acceleration;
                    if (projectile.velocity.X < 0f && xDist > 0f)
                        projectile.velocity.X += acceleration;
                }
                else if (projectile.velocity.X > xDist)
                {
                    projectile.velocity.X = projectile.velocity.X - acceleration;
                    if (projectile.velocity.X > 0f && xDist < 0f)
                        projectile.velocity.X -= acceleration;
                }
                if (projectile.velocity.Y < yDist)
                {
                    projectile.velocity.Y = projectile.velocity.Y + acceleration;
                    if (projectile.velocity.Y < 0f && yDist > 0f)
                        projectile.velocity.Y += acceleration;
                }
                else if (projectile.velocity.Y > yDist)
                {
                    projectile.velocity.Y = projectile.velocity.Y - acceleration;
                    if (projectile.velocity.Y > 0f && yDist < 0f)
                        projectile.velocity.Y -= acceleration;
                }


                // Delete the projectile if it touches its owner.
                if (Main.myPlayer == projectile.owner)
                    if (projectile.Hitbox.Intersects(owner.Hitbox))
                        projectile.Kill();
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            //Start homing at player if you hit an enemy
            projectile.ai[0] = 1;
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            //Bounce off tiles and start homing on player if it hits a tile
            Collision.HitTiles(projectile.position + projectile.velocity, projectile.velocity, projectile.width, projectile.height);
            Main.PlaySound(0, projectile.position);
            if (projectile.velocity.X != oldVelocity.X)
            {
                projectile.velocity.X = -oldVelocity.X;
            }
            if (projectile.velocity.Y != oldVelocity.Y)
            {
                projectile.velocity.Y = -oldVelocity.Y;
            }
            projectile.ai[0] = 1;
            return false;

        }
    }
}
