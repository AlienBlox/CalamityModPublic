using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons
{
    public class BladecrestOathsword : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Bladecrest Oathsword");
			Tooltip.SetDefault("Sword of an ancient demon lord\n" +
				"Fires a blood scythe");
		}

		public override void SetDefaults()
		{
			item.width = 58;
			item.damage = 25;
			item.melee = true;
			item.useAnimation = 25;
			item.useStyle = 1;
			item.useTime = 25;
			item.knockBack = 4f;
			item.UseSound = SoundID.Item1;
			item.autoReuse = false;
			item.height = 58;
            item.value = Item.buyPrice(0, 4, 0, 0);
            item.rare = 3;
			item.shoot = mod.ProjectileType("BloodScythe");
			item.shootSpeed = 6f;
		}

	    public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit)
	    {
			target.AddBuff(BuffID.OnFire, 240);
		}
	}
}
