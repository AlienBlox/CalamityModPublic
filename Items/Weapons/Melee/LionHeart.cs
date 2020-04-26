using CalamityMod.Projectiles.Ranged;
using CalamityMod.Projectiles.Typeless;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class LionHeart : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Lion Heart");
            Tooltip.SetDefault("Summons an energy explosion on enemy hits\n" +
			"Right click to summon an energy shell for a few seconds that halves all damage sources\n" +
			"This has a 45 second cooldown");
        }

        public override void SetDefaults()
        {
            item.width = 60;
            item.height = 62;

            item.damage = 700;
            item.knockBack = 5.5f;
            item.useStyle = 1;
            item.useAnimation = 15;
            item.useTime = 15;
            item.shootSpeed = 0f;

            item.melee = true;
            item.useTurn = true;
            item.autoReuse = true;
            item.UseSound = SoundID.Item1;

            item.value = CalamityGlobalItem.Rarity13BuyPrice;
            item.rare = 10;
            item.Calamity().customRarity = CalamityRarity.Dedicated;
        }

		public override bool AltFunctionUse(Player player) => true;

		public override bool CanUseItem(Player player)
		{
			if (player.altFunctionUse == 2)
			{
				item.shoot = ModContent.ProjectileType<EnergyShell>();
			}
			else
			{
				item.shoot = 0;
			}
			return base.CanUseItem(player);
		}

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
			if (player.altFunctionUse == 2)
			{
				if (!player.Calamity().energyShellCooldown && player.ownedProjectileCounts[ModContent.ProjectileType<EnergyShell>()] <= 0)
				{
					Projectile.NewProjectile(position.X, position.Y, speedX, speedY, ModContent.ProjectileType<EnergyShell>(), 0, 0f, player.whoAmI, 0f, 0f);
				}
			}
            return false;
        }

		public override bool? CanHitNPC(Player player, NPC target)
		{
			if (player.altFunctionUse == 2)
			{
				return false;
			}
			return null;
		}

		public override bool CanHitPvp(Player player, Player target) => player.altFunctionUse != 2;

        public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit)
        {
			int explosion = Projectile.NewProjectile(target.Center.X, target.Center.Y, 0f, 0f, ModContent.ProjectileType<PlanarRipperExplosion>(), (int)(item.damage * player.MeleeDamage()), knockback, player.whoAmI, 0f, 0f);
			Main.projectile[explosion].Calamity().forceMelee = true;
        }

        public override void OnHitPvp(Player player, Player target, int damage, bool crit)
        {
			int explosion = Projectile.NewProjectile(target.Center.X, target.Center.Y, 0f, 0f, ModContent.ProjectileType<PlanarRipperExplosion>(), (int)(item.damage * player.MeleeDamage()), item.knockBack, player.whoAmI, 0f, 0f);
			Main.projectile[explosion].Calamity().forceMelee = true;
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            if (Main.rand.NextBool(3))
            {
                Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, 132);
            }
        }
    }
}
