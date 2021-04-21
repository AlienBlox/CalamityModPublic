using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class Arbalest : ModItem
    {
		private int totalProjectiles = 1;
		private float arrowScale = 0.5f;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Arbalest");
            Tooltip.SetDefault("Fires a volley of 10 high-speed arrows\n" +
				"Arrows start off small and grow in size with continuous fire\n" +
				"Arrow damage and knockback scale with arrow size");
        }

        public override void SetDefaults()
        {
            item.damage = 32;
            item.ranged = true;
            item.width = 58;
            item.height = 22;
            item.useTime = 7;
			item.reuseDelay = 30;
            item.useAnimation = 28;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 4f;
            item.value = Item.buyPrice(0, 36, 0, 0);
            item.rare = ItemRarityID.Pink;
            item.UseSound = null;
            item.autoReuse = true;
            item.shoot = ProjectileID.PurificationPowder;
            item.shootSpeed = 12f;
            item.useAmmo = AmmoID.Arrow;
			item.Calamity().challengeDrop = true;
		}

		// Terraria seems to really dislike high crit values in SetDefaults
		public override void GetWeaponCrit(Player player, ref int crit) => crit += 20;

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
			Main.PlaySound(SoundID.Item5, (int)player.Center.X, (int)player.Center.Y);

			if (totalProjectiles > 4)
			{
				totalProjectiles = 1;

				if (arrowScale < 1.5f)
					arrowScale += 0.05f;
			}

			for (int i = 0; i < totalProjectiles; i++)
			{
				float SpeedX = speedX + Main.rand.Next(-30, 31) * 0.05f;
				float SpeedY = speedY + Main.rand.Next(-30, 31) * 0.05f;
				int proj = Projectile.NewProjectile(position.X, position.Y, SpeedX, SpeedY, type, (int)(damage * arrowScale), knockBack * arrowScale, player.whoAmI);
				Main.projectile[proj].scale = arrowScale;
				Main.projectile[proj].extraUpdates += 2;
				Main.projectile[proj].noDropItem = true;
			}

			totalProjectiles++;

			if (arrowScale >= 1.5f)
				arrowScale = 0.5f;

			return false;
        }
    }
}
