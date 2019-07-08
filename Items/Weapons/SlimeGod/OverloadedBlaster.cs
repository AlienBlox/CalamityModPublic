using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items;

namespace CalamityMod.Items.Weapons.SlimeGod
{
	public class OverloadedBlaster : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Overloaded Blaster");
			Tooltip.SetDefault("33% chance to not consume gel\n" +
				"Fires a large spread of bouncing slime");
		}

	    public override void SetDefaults()
	    {
			item.damage = 16;
			item.ranged = true;
			item.width = 42;
			item.height = 20;
			item.useTime = 17;
			item.useAnimation = 17;
			item.useStyle = 5;
			item.noMelee = true;
			item.knockBack = 1.5f;
            item.value = Item.buyPrice(0, 12, 0, 0);
            item.rare = 4;
			item.UseSound = SoundID.Item9;
			item.autoReuse = true;
			item.shootSpeed = 6.5f;
			item.shoot = mod.ProjectileType("SlimeBolt");
			item.useAmmo = 23;
		}
	    
	    public override bool ConsumeAmmo(Player player)
	    {
	    	if (Main.rand.Next(0, 100) < 33)
	    		return false;
	    	return true;
	    }
		
		public override bool Shoot(Player player, ref Microsoft.Xna.Framework.Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            for (int index = 0; index < 5; ++index)
            {
            	float num7 = speedX;
                float num8 = speedY;
                float SpeedX = speedX + (float) Main.rand.Next(-40, 41) * 0.05f;
                float SpeedY = speedY + (float) Main.rand.Next(-40, 41) * 0.05f;
                Projectile.NewProjectile(position.X, position.Y, SpeedX, SpeedY, type, damage, knockBack, player.whoAmI, 0f, 0f);
            }
            return false;
		}
	}
}