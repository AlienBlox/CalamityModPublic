using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Amidias
{
    public class MagicalConch : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Magical Conch");
            Tooltip.SetDefault("Summons a hermit crab to fight for you");
        }

        public override void SetDefaults()
        {
            item.damage = 20;
            item.mana = 10;
            item.width = 34;
            item.height = 24;
            item.useTime = 36;
            item.useAnimation = 36;
            item.useStyle = 1;
            item.noMelee = true;
            item.knockBack = 2f;
            item.value = Item.buyPrice(0, 2, 0, 0);
            item.rare = 2;
            item.UseSound = SoundID.Item44;
            item.autoReuse = true;
            item.shoot = mod.ProjectileType("HermitCrab");
            item.shootSpeed = 10f;
            item.summon = true;
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
            if ((float.IsNaN(num78) && float.IsNaN(num79)) || (num78 == 0f && num79 == 0f))
            {
                num78 = (float)player.direction;
            }
            else
            {
                num80 = num72 / num80;
            }
            num78 = 0f;
            num79 = 0f;
            vector2.X = (float)Main.mouseX + Main.screenPosition.X;
            vector2.Y = (float)Main.mouseY + Main.screenPosition.Y;
            Projectile.NewProjectile(vector2.X, vector2.Y, num78, num79, mod.ProjectileType("HermitCrab"), num73, num74, i, 0f, 0f);
            return false;
        }
    }
}
