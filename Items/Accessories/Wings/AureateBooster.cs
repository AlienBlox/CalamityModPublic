﻿using CalamityMod.Items.Materials;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Terraria.DataStructures;

namespace CalamityMod.Items.Accessories.Wings
{
    [AutoloadEquip(EquipType.Wings)]
    [LegacyName("AureateWings")]
    public class AureateBooster : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Accessories.Wings";
        public override void SetStaticDefaults()
        {
            ArmorIDs.Wing.Sets.Stats[Item.wingSlot] = new WingStats(120, 8f, 1.5f);
        }

        public override void SetDefaults()
        {
            Item.width = 54;
            Item.height = 26;
            Item.value = CalamityGlobalItem.Rarity7BuyPrice;
            Item.rare = ItemRarityID.Lime;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            if (player.controlJump && player.wingTime > 0f && !player.canJumpAgain_Cloud && player.jump == 0 && player.velocity.Y != 0f && !hideVisual)
            {
                player.rocketDelay2--;
                if (player.rocketDelay2 <= 0)
                {
                    SoundEngine.PlaySound(SoundID.Item13, player.Center);
                    player.rocketDelay2 = 60;
                }
                int num66 = 2;
                if (player.controlUp)
                {
                    num66 = 4;
                }
                for (int num67 = 0; num67 < num66; num67++)
                {
                    int type = 6;
                    float scale = 1.75f;
                    int alpha = 100;
                    float x = player.position.X + (float)(player.width / 2) + 16f;
                    if (player.direction > 0)
                    {
                        x = player.position.X + (float)(player.width / 2) - 26f;
                    }
                    float num68 = player.position.Y + (float)player.height - 18f;
                    if (num67 == 1 || num67 == 3)
                    {
                        x = player.position.X + (float)(player.width / 2) + 8f;
                        if (player.direction > 0)
                        {
                            x = player.position.X + (float)(player.width / 2) - 20f;
                        }
                        num68 += 6f;
                    }
                    if (num67 > 1)
                    {
                        num68 += player.velocity.Y;
                    }
                    int num69 = Dust.NewDust(new Vector2(x, num68), 8, 8, type, 0f, 0f, alpha, default, scale);
                    Dust dust = Main.dust[num69];
                    dust.velocity.X *= 0.1f;
                    dust.velocity.Y = Main.dust[num69].velocity.Y * 1f + 2f * player.gravDir - player.velocity.Y * 0.3f;
                    dust.noGravity = true;
                    dust.shader = GameShaders.Armor.GetSecondaryShader(player.cWings, player);
                    if (num66 == 4)
                    {
                        dust.velocity.Y += 6f;
                    }
                }
            }
            player.noFallDmg = true;
        }

        public override void VerticalWingSpeeds(Player player, ref float ascentWhenFalling, ref float ascentWhenRising, ref float maxCanAscendMultiplier, ref float maxAscentMultiplier, ref float constantAscend)
        {
            ascentWhenFalling = 0.75f;
            ascentWhenRising = 0.15f;
            maxCanAscendMultiplier = 1f;
            maxAscentMultiplier = 2.5f;
            constantAscend = 0.125f;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.SoulofFlight, 20).
                AddIngredient<PerennialBar>(5).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
