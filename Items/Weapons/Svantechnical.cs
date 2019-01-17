using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items;

namespace CalamityMod.Items.Weapons
{
    public class Svantechnical : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Svantechnical");
        }

        public override void SetDefaults()
        {
            item.damage = 350;
            item.ranged = true;
            item.width = 60;
            item.height = 26;
            item.useAnimation = 24;
            item.reuseDelay = 10;
            item.useTime = 2;
            item.useStyle = 5;
            item.noMelee = true;
            item.knockBack = 3.5f;
            item.value = Item.buyPrice(5, 0, 0, 0);
            item.rare = 10;
            item.UseSound = SoundID.Item31;
            item.autoReuse = true;
            item.shootSpeed = 12f;
            item.shoot = 10;
            item.useAmmo = 97;
        }

        public override void ModifyTooltips(List<TooltipLine> list)
        {
            foreach (TooltipLine line2 in list)
            {
                if (line2.mod == "Terraria" && line2.Name == "ItemName")
                {
                    line2.overrideColor = new Color(255, 0, 255);
                }
            }
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-5, 0);
        }

        public override bool ConsumeAmmo(Player player)
        {
            if (Main.rand.Next(0, 100) <= 70)
                return false;
            return true;
        }

        public override bool Shoot(Player player, ref Microsoft.Xna.Framework.Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            int i = Main.myPlayer;
            float num72 = item.shootSpeed;
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
            float num81 = num80;
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
            float num208 = num78;
            float num209 = num79;
            num208 += (float)Main.rand.Next(-1, 2) * 0.5f;
            num209 += (float)Main.rand.Next(-1, 2) * 0.5f;
            if (Collision.CanHitLine(player.Center, 0, 0, vector2 + new Vector2(num208, num209) * 2f, 0, 0))
            {
                vector2 += new Vector2(num208, num209);
            }
            Projectile.NewProjectile(position.X, position.Y - player.gravDir * 4f, num208, num209, type, num73, num74, i, 0f, (float)Main.rand.Next(12) / 6f);
            int num6 = Main.rand.Next(2, 4);
            for (int index = 0; index < num6; ++index)
            {
                float SpeedX = speedX + (float)Main.rand.Next(-60, 61) * 0.05f;
                float SpeedY = speedY + (float)Main.rand.Next(-60, 61) * 0.05f;
                Projectile.NewProjectile(vector2.X, vector2.Y, SpeedX, SpeedY, type, damage, knockBack, player.whoAmI, 0.0f, 0.0f);
            }
            return false;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.SDMG);
            recipe.AddIngredient(null, "ShadowspecBar", 5);
            recipe.AddIngredient(ItemID.SoulofMight, 10);
            recipe.AddIngredient(ItemID.SoulofSight, 10);
            recipe.AddIngredient(ItemID.SoulofFright, 10);
            recipe.AddTile(null, "DraedonsForge");
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}