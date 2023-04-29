﻿using CalamityMod.Projectiles.Typeless;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    public class Gehenna : ModItem
    {
        public const int FireProjectiles = 2;
        public const float FireAngleSpread = 120;
        public int FireCountdown = 0;

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Gehenna");
            // Tooltip.SetDefault("Drops brimstone fireballs from the sky occasionally");
        }

        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 26;
            Item.value = CalamityGlobalItem.Rarity5BuyPrice;
            Item.rare = ItemRarityID.Pink;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            if (FireCountdown == 0)
            {
                FireCountdown = 600;
            }
            if (FireCountdown > 0)
            {
                FireCountdown--;
                if (FireCountdown == 0)
                {
                    var source = player.GetSource_Accessory(Item);
                    if (player.whoAmI == Main.myPlayer)
                    {
                        int speed2 = 25;
                        float spawnX = Main.rand.Next(1000) - 500 + player.Center.X;
                        float spawnY = -1000 + player.Center.Y;
                        Vector2 baseSpawn = new Vector2(spawnX, spawnY);
                        Vector2 baseVelocity = player.Center - baseSpawn;
                        baseVelocity.Normalize();
                        baseVelocity *= speed2;
                        for (int i = 0; i < FireProjectiles; i++)
                        {
                            Vector2 spawn = baseSpawn;
                            spawn.X = spawn.X + i * 30 - (FireProjectiles * 15);
                            Vector2 velocity = baseVelocity.RotatedBy(MathHelper.ToRadians(-FireAngleSpread / 2 + (FireAngleSpread * i / FireProjectiles)));
                            velocity.X = velocity.X + 3 * Main.rand.NextFloat() - 1.5f;
                            int damage = (int)player.GetBestClassDamage().ApplyTo(54);
                            Projectile.NewProjectile(source, spawn.X, spawn.Y, velocity.X, velocity.Y, ModContent.ProjectileType<BrimstoneHellfireballFriendly2>(), damage, 5f, Main.myPlayer, 0f, 0f);
                        }
                    }
                }
            }
        }

        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            CalamityUtils.DrawInventoryCustomScale(
                spriteBatch,
                texture: TextureAssets.Item[Type].Value,
                position,
                frame,
                drawColor,
                itemColor,
                origin,
                scale,
                wantedScale: 0.8f,
                drawOffset: new(0f, 0f)
            );
            return false;
        }
    }
}
