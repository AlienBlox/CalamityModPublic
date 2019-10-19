using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Items.Weapons.Melee
{
    public class Greentide : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Greentide");
            Tooltip.SetDefault("Summons green blasts of water from the sky on hit");
        }

        public override void SetDefaults()
        {
            item.damage = 95;
            item.melee = true;
            item.width = 60;
            item.height = 60;
            item.useTime = 24;
            item.useAnimation = 24;
            item.useTurn = true;
            item.useStyle = 1;
            item.knockBack = 7;
            item.value = Item.buyPrice(0, 60, 0, 0);
            item.rare = 7;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.shootSpeed = 18f;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<IOU>());
            recipe.AddIngredient(ModContent.ItemType<LivingShard>());
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }

        public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit)
        {
            int i = Main.myPlayer;
            float num72 = item.shootSpeed;
            float num74 = knockback;
            num74 = player.GetWeaponKnockback(item, num74);
            player.itemTime = item.useTime;
            Vector2 vector2 = player.RotatedRelativePoint(player.MountedCenter, true);
            float num78 = (float)Main.mouseX - Main.screenPosition.X - vector2.X;
            float num79 = (float)Main.mouseY - Main.screenPosition.Y - vector2.Y;
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

            int num107 = 4;
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
                float speedX4 = num78;
                float speedY5 = num79 + (float)Main.rand.Next(-180, 181) * 0.02f;
                Projectile.NewProjectile(vector2.X, vector2.Y, speedX4, speedY5, ModContent.ProjectileType<GreenWater>(), (int)((float)item.damage * player.meleeDamage), num74, i, 0f, (float)Main.rand.Next(10));
            }
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            int randomDust = Main.rand.Next(2);
            if (randomDust == 0)
            {
                randomDust = 33;
            }
            else
            {
                randomDust = 89;
            }
            if (Main.rand.NextBool(4))
            {
                int dust = Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, randomDust);
            }
        }
    }
}
