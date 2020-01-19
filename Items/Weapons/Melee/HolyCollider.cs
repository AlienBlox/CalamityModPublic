using CalamityMod.Projectiles.Melee;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class HolyCollider : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Holy Collider");
            Tooltip.SetDefault("Striking enemies will cause them to explode into holy fire");
        }

        public override void SetDefaults()
        {
            item.width = 94;
            item.damage = 400;
            item.melee = true;
            item.useAnimation = 22;
            item.useStyle = 1;
            item.useTime = 22;
            item.useTurn = true;
            item.knockBack = 7.75f;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.height = 80;
            item.value = Item.buyPrice(1, 20, 0, 0);
            item.rare = 10;
            item.shootSpeed = 10f;
            item.Calamity().postMoonLordRarity = 12;
        }

        public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit)
        {
            Main.PlaySound(2, (int)player.position.X, (int)player.position.Y, 14);
            for (int num621 = 0; num621 < 30; num621++)
            {
                int num622 = Dust.NewDust(new Vector2(player.position.X, player.position.Y), player.width, player.height, 244, 0f, 0f, 100, default, 2f);
                Main.dust[num622].velocity *= 3f;
                if (Main.rand.NextBool(2))
                {
                    Main.dust[num622].scale = 0.5f;
                    Main.dust[num622].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                }
            }
            for (int num623 = 0; num623 < 50; num623++)
            {
                int num624 = Dust.NewDust(new Vector2(player.position.X, player.position.Y), player.width, player.height, 244, 0f, 0f, 100, default, 3f);
                Main.dust[num624].noGravity = true;
                Main.dust[num624].velocity *= 5f;
                num624 = Dust.NewDust(new Vector2(player.position.X, player.position.Y), player.width, player.height, 244, 0f, 0f, 100, default, 2f);
                Main.dust[num624].velocity *= 2f;
            }

            float spread = 45f * 0.0174f;
            double startAngle = Math.Atan2(item.shootSpeed, item.shootSpeed) - spread / 2;
            double deltaAngle = spread / 8f;
            double offsetAngle;
            int i;
            for (i = 0; i < 4; i++)
            {
                offsetAngle = startAngle + deltaAngle * (i + i * i) / 2f + 32f * i;
                Projectile.NewProjectile(target.Center.X, target.Center.Y, (float)(Math.Sin(offsetAngle) * 5f), (float)(Math.Cos(offsetAngle) * 5f), ModContent.ProjectileType<HolyColliderHolyFire>(), (int)(item.damage * (player.allDamage + player.meleeDamage - 1f) * 0.3), knockback, Main.myPlayer);
                Projectile.NewProjectile(target.Center.X, target.Center.Y, (float)(-Math.Sin(offsetAngle) * 5f), (float)(-Math.Cos(offsetAngle) * 5f), ModContent.ProjectileType<HolyColliderHolyFire>(), (int)(item.damage * (player.allDamage + player.meleeDamage - 1f) * 0.3), knockback, Main.myPlayer);
            }
        }

        public override void OnHitPvp(Player player, Player target, int damage, bool crit)
        {
            Main.PlaySound(2, (int)player.position.X, (int)player.position.Y, 14);
            for (int num621 = 0; num621 < 30; num621++)
            {
                int num622 = Dust.NewDust(new Vector2(player.position.X, player.position.Y), player.width, player.height, 244, 0f, 0f, 100, default, 2f);
                Main.dust[num622].velocity *= 3f;
                if (Main.rand.NextBool(2))
                {
                    Main.dust[num622].scale = 0.5f;
                    Main.dust[num622].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                }
            }
            for (int num623 = 0; num623 < 50; num623++)
            {
                int num624 = Dust.NewDust(new Vector2(player.position.X, player.position.Y), player.width, player.height, 244, 0f, 0f, 100, default, 3f);
                Main.dust[num624].noGravity = true;
                Main.dust[num624].velocity *= 5f;
                num624 = Dust.NewDust(new Vector2(player.position.X, player.position.Y), player.width, player.height, 244, 0f, 0f, 100, default, 2f);
                Main.dust[num624].velocity *= 2f;
            }

            float spread = 45f * 0.0174f;
            double startAngle = Math.Atan2(item.shootSpeed, item.shootSpeed) - spread / 2;
            double deltaAngle = spread / 8f;
            double offsetAngle;
            int i;
            for (i = 0; i < 4; i++)
            {
                offsetAngle = startAngle + deltaAngle * (i + i * i) / 2f + 32f * i;
                Projectile.NewProjectile(target.Center.X, target.Center.Y, (float)(Math.Sin(offsetAngle) * 5f), (float)(Math.Cos(offsetAngle) * 5f), ModContent.ProjectileType<HolyColliderHolyFire>(), (int)(item.damage * (player.allDamage + player.meleeDamage - 1f) * 0.3), item.knockBack, Main.myPlayer);
                Projectile.NewProjectile(target.Center.X, target.Center.Y, (float)(-Math.Sin(offsetAngle) * 5f), (float)(-Math.Cos(offsetAngle) * 5f), ModContent.ProjectileType<HolyColliderHolyFire>(), (int)(item.damage * (player.allDamage + player.meleeDamage - 1f) * 0.3), item.knockBack, Main.myPlayer);
            }
        }
    }
}
