﻿
using Terraria;
using Terraria.ModLoader;
using Terraria.Graphics.Effects;
using Terraria.GameContent.Shaders;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CalamityMod.Gores
{
    public class AstralWaterDroplet : ModGore
    {
        public override void OnSpawn(Gore gore)
        {
            gore.numFrames = 15;
            gore.behindTiles = true;
            gore.timeLeft = Gore.goreTime * 3;
        }
        public override bool DrawBehind(Gore gore)
        {
            Main.instance.LoadGore(gore.type);

            int num = Main.goreTexture[gore.type].Height / (int)gore.numFrames;

            Color alpha = gore.GetAlpha(Lighting.GetColor((int)((double)gore.position.X + (double)Main.goreTexture[gore.type].Width * 0.5) / 16, (int)(((double)gore.position.Y + (double)num * 0.5) / 16.0)));

            Main.spriteBatch.Draw(
                Main.goreTexture[gore.type],
                new Vector2(gore.position.X - Main.screenPosition.X + (float)(Main.goreTexture[gore.type].Width / 2), gore.position.Y - Main.screenPosition.Y + (float)(num / 2) - 2f),
                new Rectangle(0, num * (int)gore.frame, Main.goreTexture[gore.type].Width, num),
                alpha, gore.rotation,
                new Vector2((float)(Main.goreTexture[gore.type].Width / 2), (float)(num / 2)),
                gore.scale, SpriteEffects.None, 0f);

            return false;
        }
        public override bool Update(Gore gore)
        {
            if ((double)gore.position.Y < Main.worldSurface * 16.0 + 8.0)
            {
                gore.alpha = 0;
            }
            else
            {
                gore.alpha = 100;
            }
            int num = 4;
            gore.frameCounter++;
            if (gore.frame <= 4)
            {
                int num2 = (int)(gore.position.X / 16f);
                int num3 = (int)(gore.position.Y / 16f) - 1;
                if (WorldGen.InWorld(num2, num3, 0) && !Main.tile[num2, num3].active())
                {
                    gore.active = false;
                }
                if (gore.frame == 0)
                {
                    num = 24 + Main.rand.Next(256);
                }
                if (gore.frame == 1)
                {
                    num = 24 + Main.rand.Next(256);
                }
                if (gore.frame == 2)
                {
                    num = 24 + Main.rand.Next(256);
                }
                if (gore.frame == 3)
                {
                    num = 24 + Main.rand.Next(96);
                }
                if (gore.frame == 5)
                {
                    num = 16 + Main.rand.Next(64);
                }
                if ((int)gore.frameCounter >= num)
                {
                    gore.frameCounter = 0;
                    gore.frame += 1;
                    if (gore.frame == 5)
                    {
                        int num4 = Gore.NewGore(gore.position, gore.velocity, gore.type, 1f);
                        Main.gore[num4].frame = 9;
                        Main.gore[num4].velocity *= 0f;
                    }
                }
            }
            else if (gore.frame <= 6)
            {
                num = 8;
                if ((int)gore.frameCounter >= num)
                {
                    gore.frameCounter = 0;
                    gore.frame += 1;
                    if (gore.frame == 7)
                    {
                        gore.active = false;
                    }
                }
            }
            else if (gore.frame <= 9)
            {
                num = 6;
                gore.velocity.Y = gore.velocity.Y + 0.2f;
                if ((double)gore.velocity.Y < 0.5)
                {
                    gore.velocity.Y = 0.5f;
                }
                if (gore.velocity.Y > 12f)
                {
                    gore.velocity.Y = 12f;
                }
                if ((int)gore.frameCounter >= num)
                {
                    gore.frameCounter = 0;
                    gore.frame += 1;
                }
                if (gore.frame > 9)
                {
                    gore.frame = 7;
                }
            }
            else
            {
                gore.velocity.Y = gore.velocity.Y + 0.1f;
                if ((int)gore.frameCounter >= num)
                {
                    gore.frameCounter = 0;
                    gore.frame += 1;
                }
                gore.velocity *= 0f;
                if (gore.frame > 14)
                {
                    gore.active = false;
                }
            }

            Vector2 value3 = gore.velocity;
            gore.velocity = Collision.TileCollision(gore.position, gore.velocity, 16, 14, false, false, 1);
            if (gore.velocity != value3)
            {
                if (gore.frame < 10)
                {
                    gore.frame = 10;
                    gore.frameCounter = 0;
                    if (gore.type != 716 && gore.type != 717 && gore.type != 943)
                    {
                        Main.PlaySound(39, (int)gore.position.X + 8, (int)gore.position.Y + 8, Main.rand.Next(2), 1f, 0f);
                    }
                }
            }
            else if (Collision.WetCollision(gore.position + gore.velocity, 16, 14))
            {
                if (gore.frame < 10)
                {
                    gore.frame = 10;
                    gore.frameCounter = 0;
                    if (gore.type != 716 && gore.type != 717 && gore.type != 943)
                    {
                        Main.PlaySound(39, (int)gore.position.X + 8, (int)gore.position.Y + 8, 2, 1f, 0f);
                    }
                    ((WaterShaderData)Filters.Scene["WaterDistortion"].GetShader()).QueueRipple(gore.position + new Vector2(8f, 8f), 1f, RippleShape.Square, 0f);
                }
                int num21 = (int)(gore.position.X + 8f) / 16;
                int num22 = (int)(gore.position.Y + 14f) / 16;
                if (Main.tile[num21, num22] != null && Main.tile[num21, num22].liquid > 0)
                {
                    gore.velocity *= 0f;
                    gore.position.Y = (float)(num22 * 16 - (int)(Main.tile[num21, num22].liquid / 16));
                }
            }

            return true;
        }
    }
}
