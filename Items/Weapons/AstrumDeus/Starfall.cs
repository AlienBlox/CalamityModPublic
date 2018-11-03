using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items;

namespace CalamityMod.Items.Weapons.AstrumDeus
{
	public class Starfall : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Starfall");
		}

	    public override void SetDefaults()
	    {
	        item.damage = 45;
            item.crit += 25;
            item.magic = true;
	        item.mana = 15;
            item.rare = 7;
	        item.width = 28;
	        item.height = 30;
	        item.useTime = 14;
	        item.useAnimation = 14;
	        item.useStyle = 5;
	        item.noMelee = true;
	        item.knockBack = 3.25f;
	        item.value = 500000;
	        item.UseSound = SoundID.Item105;
	        item.autoReuse = true;
	        item.shoot = mod.ProjectileType("AstralStar");
	        item.shootSpeed = 12f;
	    }
	    
	    public override bool Shoot(Player player, ref Microsoft.Xna.Framework.Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
	    {
            float num72 = item.shootSpeed;
            Vector2 vector2 = player.RotatedRelativePoint(player.MountedCenter, true);
            float num78 = (float)Main.mouseX + Main.screenPosition.X - vector2.X;
            float num79 = (float)Main.mouseY + Main.screenPosition.Y - vector2.Y;
            if (player.gravDir == -1f)
            {
                num79 = Main.screenPosition.Y + (float)Main.screenHeight - (float)Main.mouseY - vector2.Y;
            }
            if ((float.IsNaN(num78) && float.IsNaN(num79)) || (num78 == 0f && num79 == 0f))
            {
                num78 = (float)player.direction;
                num79 = 0f;
            }
            float num208 = num78;
            float num209 = num79;
            num208 += (float)Main.rand.Next(-1, 2) * 0.5f;
            num209 += (float)Main.rand.Next(-1, 2) * 0.5f;
            vector2 += new Vector2(num208, num209);
            for (int num108 = 0; num108 < 5; num108++)
            {
                float speedX4 = 2f + (float)Main.rand.Next(-8, 5);
                float speedY5 = 15f + (float)Main.rand.Next(1, 6);
                int star = Projectile.NewProjectile(vector2.X, vector2.Y, speedX4, speedY5, type, damage, knockBack, player.whoAmI, 0f, 0f);
                Main.projectile[star].magic = true;
            }
            return false;
	    }
	}
}