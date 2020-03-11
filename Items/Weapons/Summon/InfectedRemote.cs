using CalamityMod.Projectiles.Summon;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Summon
{
    public class InfectedRemote : ModItem
    {
		int viruliSlots;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Infected Remote");
            Tooltip.SetDefault("There’s a faded note written on it in green\n" +
							   "Only the first line is readable: 'She won’t afflict you, I promise!'\n" +
							   "Summons the harbinger of the plague...?\n" +
                               "Consumes all of the remaining minion slots on use\n" +
							   "Must be used from the hotbar\n" +
                               "Increased power based on the number of minion slots used");
        }

        public override void SetDefaults()
        {
            item.damage = 80;
            item.mana = 10;
            item.width = 46;
            item.height = 28;
            item.useTime = item.useAnimation = 20;
            item.useStyle = 4;
            item.noMelee = true;
            item.knockBack = 5f;
            item.UseSound = SoundID.Item15; //phaseblade sound effect
            item.shoot = ModContent.ProjectileType<PlaguePrincess>();
            item.shootSpeed = 10f;
            item.summon = true;
            item.value = Item.buyPrice(0, 80, 0, 0);
            item.rare = 8;
            item.Calamity().customRarity = CalamityRarity.Dedicated;
        }

		public override void HoldItem(Player player)
        {
			double minionCount = 0;
			for (int j = 0; j < Main.projectile.Length; j++)
			{
                Projectile projectile = Main.projectile[j];
				if (projectile.active && projectile.owner == player.whoAmI && projectile.minion && projectile.type != ModContent.ProjectileType<PlaguePrincess>())
				{
					minionCount += projectile.minionSlots;
				}
			}
			viruliSlots = (int)((double)player.maxMinions - minionCount);
		}

        public override bool CanUseItem(Player player)
		{
			return viruliSlots >= 1;
		}

        public override bool Shoot(Player player, ref Microsoft.Xna.Framework.Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            for (int x = 0; x < Main.projectile.Length; x++)
            {
                Projectile projectile2 = Main.projectile[x];
                if (projectile2.active && projectile2.owner == player.whoAmI && projectile2.type == ModContent.ProjectileType<PlaguePrincess>())
                {
                    projectile2.Kill();
                }
            }
			float damageMult = ((float)Math.Log(viruliSlots, 8f)) + 1f;
            position = Main.MouseWorld;
            speedX = 0;
            speedY = 0;
            Projectile.NewProjectile(position.X, position.Y, speedX, speedY, type, (int)(damage * damageMult), knockBack, player.whoAmI, viruliSlots);
            return false;
        }
    }
}
