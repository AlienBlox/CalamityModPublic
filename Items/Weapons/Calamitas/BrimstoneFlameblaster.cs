using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Calamitas
{
    public class BrimstoneFlameblaster : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Brimstone Flameblaster");
			Tooltip.SetDefault("Fires bouncing brimstone fireballs");
		}

	    public override void SetDefaults()
	    {
			item.damage = 64;
			item.ranged = true;
			item.width = 50;
			item.height = 18;
			item.useTime = 20;
			item.useAnimation = 20;
			item.useStyle = 5;
			item.noMelee = true;
			item.knockBack = 3.5f;
			item.UseSound = SoundID.Item34;
            item.value = Item.buyPrice(0, 60, 0, 0);
            item.rare = 7;
			item.autoReuse = true;
			item.shoot = mod.ProjectileType("BrimstoneBallFriendly");
			item.shootSpeed = 10f;
			item.useAmmo = 23;
		}
	}
}
