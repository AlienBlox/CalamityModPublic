﻿using CalamityMod.Buffs.Mounts;
using CalamityMod.CalPlayer;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Mounts
{
    class AlicornMount : ModMountData
    {
        public override void SetDefaults()
        {
            mountData.spawnDust = 234;
            mountData.spawnDustNoGravity = true;
            mountData.buff = ModContent.BuffType<AlicornBuff>();
            mountData.heightBoost = 34;
            mountData.fallDamage = 0f; //0.5
            mountData.runSpeed = 7f; //12
            mountData.dashSpeed = 21f; //8
            mountData.flightTimeMax = 500;
            mountData.fatigueMax = 0;
            mountData.jumpHeight = 12;
            mountData.acceleration = 0.45f;
            mountData.jumpSpeed = 8f; //10
            mountData.swimSpeed = 4f;
            mountData.blockExtraJumps = false;
            mountData.totalFrames = 15;
            mountData.constantJump = false;
            int[] array = new int[mountData.totalFrames];
            for (int l = 0; l < array.Length; l++)
            {
                array[l] = 30;
            }
            array[1] = 28;
            array[3] = 28;
            array[5] = 28;
            array[7] = 28;
            array[12] = 28;
            mountData.playerYOffsets = array;
            mountData.xOffset = 0;
            mountData.bodyFrame = 3;
            mountData.yOffset = 7; //-8
            mountData.playerHeadOffset = 36; //30
            mountData.standingFrameCount = 1;
            mountData.standingFrameDelay = 12;
            mountData.standingFrameStart = 0;
            mountData.runningFrameCount = 8; //7
            mountData.runningFrameDelay = 36; //36
            mountData.runningFrameStart = 1; //9
            mountData.flyingFrameCount = 6; //0
            mountData.flyingFrameDelay = 4; //0
            mountData.flyingFrameStart = 9; //0
            mountData.inAirFrameCount = 1; //1
            mountData.inAirFrameDelay = 12; //12
            mountData.inAirFrameStart = 10; //10
            mountData.idleFrameCount = 5; //4
            mountData.idleFrameDelay = 12; //12
            mountData.idleFrameStart = 0;
            mountData.idleFrameLoop = true;
            mountData.swimFrameCount = mountData.inAirFrameCount;
            mountData.swimFrameDelay = mountData.inAirFrameDelay;
            mountData.swimFrameStart = mountData.inAirFrameStart;
            if (Main.netMode != NetmodeID.Server)
            {
                mountData.frontTextureExtra = ModContent.GetTexture("CalamityMod/Items/Mounts/AlicornMountExtra");
                mountData.textureWidth = mountData.backTexture.Width;
                mountData.textureHeight = mountData.backTexture.Height;
            }
        }

        public override void UpdateEffects(Player player)
        {
            CalamityPlayer modPlayer = player.Calamity();
            if (modPlayer.fabsolVodka)
            {
                player.allDamage += 0.1f;
            }
            if (Math.Abs(player.velocity.X) > 12f || Math.Abs(player.velocity.Y) > 12f)
            {
                int rand = Main.rand.Next(2);
                bool momo = false;
                if (rand == 1)
                {
                    momo = true;
                }
                Color meme;
                if (momo)
                {
                    meme = new Color(255, 68, 242);
                }
                else
                {
                    meme = new Color(25, 105, 255);
                }
                Rectangle rect = player.getRect();
                int dust = Dust.NewDust(new Vector2(rect.X, rect.Y), rect.Width, rect.Height, 234, 0, 0, 0, meme);
                Main.dust[dust].noGravity = true;
            }
            if (player.velocity.Y != 0f)
            {
                if (player.mount.PlayerOffset == 28)
                {
                    if (!player.flapSound)
                        Main.PlaySound(SoundID.Item32, player.position);
                    player.flapSound = true;
                }
                else
                    player.flapSound = false;
            }
        }
    }
}
