using Microsoft.Xna.Framework;
using System;
using Terraria; using CalamityMod.Projectiles; using Terraria.ModLoader;
using Terraria.ID;
using Terraria.ModLoader; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles; using CalamityMod.Tiles; using CalamityMod.Walls;

namespace CalamityMod.Items
{
    public class Aftershock : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Aftershock");
            Tooltip.SetDefault("Summons boulders from the sky on enemy hits");
        }

        public override void SetDefaults()
        {
            item.damage = 60;
            item.melee = true;
            item.width = 54;
            item.height = 58;
            item.useTime = 28;
            item.useAnimation = 28;
            item.useTurn = true;
            item.useStyle = 1;
            item.knockBack = 7.5f;
            item.value = Item.buyPrice(0, 36, 0, 0);
            item.rare = 5;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.shootSpeed = 12f;
        }

        public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<ArmorCrunch>(), 120);
            float num72 = item.shootSpeed;
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

            vector2 = new Vector2(player.position.X + (float)player.width * 0.5f + (float)(Main.rand.Next(201) * -(float)player.direction) + ((float)Main.mouseX + Main.screenPosition.X - player.position.X), player.MountedCenter.Y - 600f);
            vector2.X = (vector2.X + player.Center.X) / 2f + (float)Main.rand.Next(-200, 201);
            vector2.Y -= (float)100;
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
            float speedY5 = num79 + (float)Main.rand.Next(-10, 11) * 0.02f;
            Projectile.NewProjectile(vector2.X, vector2.Y, speedX4, speedY5, ModContent.ProjectileType<Aftershock>(), (int)((float)item.damage * player.meleeDamage), knockback, player.whoAmI, 0f, (float)Main.rand.Next(10));
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            if (Main.rand.NextBool(5))
            {
                int dust = Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, 32);
            }
        }
    }
}
