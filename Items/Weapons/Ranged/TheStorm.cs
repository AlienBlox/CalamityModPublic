using CalamityMod.Projectiles.Ranged;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class TheStorm : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("The Storm");
            Tooltip.SetDefault("Fires a spread of arrows from the sky\n" +
                "Wooden arrows are converted to lightning bolts");
        }

        public override void SetDefaults()
        {
            item.damage = 44;
            item.ranged = true;
            item.width = 34;
            item.height = 50;
            item.useTime = 7;
            item.useAnimation = 14;
            item.useStyle = 5;
            item.noMelee = true;
            item.knockBack = 3.5f;
            item.value = Item.buyPrice(1, 40, 0, 0);
            item.rare = 10;
            item.UseSound = SoundID.Item122;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<Bolt>();
            item.shootSpeed = 28f;
            item.useAmmo = 40;
            item.Calamity().customRarity = CalamityRarity.PureGreen;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            int i = Main.myPlayer;
            float num72 = Main.rand.Next(25, 30);
            int num73 = damage;
            float num74 = knockBack;
            num74 = player.GetWeaponKnockback(item, num74);
            player.itemTime = item.useTime;
            Vector2 vector2 = player.RotatedRelativePoint(player.MountedCenter, true);
            float num78 = (float)Main.mouseX + Main.screenPosition.X - vector2.X;
            float num79 = (float)Main.mouseY + Main.screenPosition.Y - vector2.Y;
            if (player.gravDir == -1f)
            {
                num79 = Main.screenPosition.Y + (float)Main.screenHeight - (float)Main.mouseY - vector2.Y;
            }
            float num80 = (float)Math.Sqrt((double)(num78 * num78 + num79 * num79));
            if ((float.IsNaN(num78) && float.IsNaN(num79)) || (num78 == 0f && num79 == 0f))
            {
                num78 = (float)player.direction;
                num79 = 0f;
                num80 = num72;
            }
            else
            {
                num80 = num72 / num80;
            }

            int num107 = 3;
            for (int num108 = 0; num108 < num107; num108++)
            {
                vector2 = new Vector2(player.position.X + (float)player.width * 0.5f + (float)(Main.rand.Next(201) * -(float)player.direction) + ((float)Main.mouseX + Main.screenPosition.X - player.position.X), player.MountedCenter.Y - 600f);
                vector2.X = (vector2.X + player.Center.X) / 2f + (float)Main.rand.Next(-200, 201);
                vector2.Y -= (float)(100 * num108);
                num78 = (float)Main.mouseX + Main.screenPosition.X - vector2.X;
                num79 = (float)Main.mouseY + Main.screenPosition.Y - vector2.Y;
                if (num79 < 0f)
                {
                    num79 *= -1f;
                }
                if (num79 < 20f)
                {
                    num79 = 20f;
                }
                num80 = (float)Math.Sqrt((double)(num78 * num78 + num79 * num79));
                num80 = num72 / num80;
                num78 *= num80;
                num79 *= num80;
                float speedX4 = num78 + (float)Main.rand.Next(-120, 121) * 0.02f;
                float speedY5 = num79 + (float)Main.rand.Next(-120, 121) * 0.02f;
                if (type == ProjectileID.WoodenArrowFriendly)
                {
                    Projectile.NewProjectile(vector2.X, vector2.Y, speedX4, speedY5 * 0.9f, ModContent.ProjectileType<Bolt>(), num73, num74, i, 0f, 0f);
                    Projectile.NewProjectile(vector2.X, vector2.Y, speedX4, speedY5 * 0.8f, ModContent.ProjectileType<Bolt>(), num73, num74, i, 0f, 0f);
                    Projectile.NewProjectile(vector2.X, vector2.Y, speedX4, speedY5 * 0.7f, ModContent.ProjectileType<Bolt>(), num73, num74, i, 0f, 0f);
                }
                else
                {
                    int num121 = Projectile.NewProjectile(vector2.X, vector2.Y, speedX4, speedY5 * 0.9f, type, num73, num74, i, 0f, 0f);
                    Main.projectile[num121].noDropItem = true;
                    int num122 = Projectile.NewProjectile(vector2.X, vector2.Y, speedX4, speedY5 * 0.8f, type, num73, num74, i, 0f, 0f);
                    Main.projectile[num122].noDropItem = true;
                    int num123 = Projectile.NewProjectile(vector2.X, vector2.Y, speedX4, speedY5 * 0.7f, type, num73, num74, i, 0f, 0f);
                    Main.projectile[num123].noDropItem = true;
                }
            }
            return false;
        }
    }
}
