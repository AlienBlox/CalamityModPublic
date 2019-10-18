﻿using CalamityMod.Projectiles.Magic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    public class AmidiasPendant : ModItem
    {
        public const int ShardProjectiles = 2;
        public const float ShardAngleSpread = 120;
        public int ShardCountdown = 0;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Amidias' Pendant");
            Tooltip.SetDefault("Periodically rains down prism shards");
        }

        public override void SetDefaults()
        {
            item.width = 26;
            item.height = 46;
            item.value = Item.buyPrice(0, 3, 0, 0);
            item.rare = 2;
            item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            if (ShardCountdown == 0)
            {
                ShardCountdown = 120;
            }
            if (ShardCountdown > 0)
            {
                ShardCountdown--;
                if (ShardCountdown == 0)
                {
                    if (player.whoAmI == Main.myPlayer)
                    {
                        int speed2 = 25;
                        float spawnX = Main.rand.Next(1000) - 500 + player.Center.X;
                        float spawnY = -1000 + player.Center.Y;
                        Vector2 baseSpawn = new Vector2(spawnX, spawnY);
                        Vector2 baseVelocity = player.Center - baseSpawn;
                        baseVelocity.Normalize();
                        baseVelocity *= speed2;
                        for (int i = 0; i < ShardProjectiles; i++)
                        {
                            Vector2 spawn = baseSpawn;
                            spawn.X = spawn.X + i * 30 - (ShardProjectiles * 15);
                            Vector2 velocity = baseVelocity.RotatedBy(MathHelper.ToRadians(-ShardAngleSpread / 2 + (ShardAngleSpread * i / (float)ShardProjectiles)));
                            velocity.X = velocity.X + 3 * Main.rand.NextFloat() - 1.5f;
                            int type = 0;
                            int damage = 0;
                            switch (Main.rand.Next(3))
                            {
                                case 0:
                                    type = ModContent.ProjectileType<PendantProjectile1>();
                                    damage = 15;
                                    break;
                                case 1:
                                    type = ModContent.ProjectileType<PendantProjectile2>();
                                    damage = 15;
                                    break;
                                case 2:
                                    type = ModContent.ProjectileType<PendantProjectile3>();
                                    damage = 30;
                                    break;
                            }
                            int projectile = Projectile.NewProjectile(spawn.X, spawn.Y, velocity.X / 3, velocity.Y / 2, type, damage, 5f, Main.myPlayer, 0f, 0f);
                            Main.projectile[projectile].tileCollide = false;
                            Main.projectile[projectile].timeLeft = 220;
                        }
                    }
                }
            }
        }
    }
}
