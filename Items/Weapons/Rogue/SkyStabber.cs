using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Rogue;
using CalamityMod.CalPlayer;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class SkyStabber : RogueWeapon
    {
        private static int damage = 50;
        private static int knockBack = 2;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sky Stabber");
            Tooltip.SetDefault("Shoots a gravity-defying spiky ball. Stacks up to 4.\n" +
                "Stealth strikes makes the balls rain feathers onto enemies when they hit\n" +
				"Right click to delete all existing spiky balls");
        }

        public override void SafeSetDefaults()
        {
            item.damage = damage;
            item.crit = 4;
            item.Calamity().rogue = true;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.width = 1;
            item.height = 1;
            item.useTime = 15;
            item.useAnimation = 15;
            item.useStyle = 1;
            item.knockBack = knockBack;
            item.value = Item.buyPrice(0, 1, 0, 0);
            item.rare = 3;
            item.UseSound = SoundID.Item1;
            item.maxStack = 4;

            item.shootSpeed = 2f;
            item.shoot = ModContent.ProjectileType<SkyStabberProj>();

        }

        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                if (player.ownedProjectileCounts[item.shoot] > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
			else
			{
				int UseMax = item.stack;

				if (player.ownedProjectileCounts[item.shoot] >= UseMax)
				{
					return false;
				}
				else
				{
					return true;
				}
			}
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            CalamityPlayer modPlayer = player.Calamity();
			modPlayer.killSpikyBalls = false;
            if (player.altFunctionUse == 2)
			{
				modPlayer.killSpikyBalls = true;
				return false;
			}
            if (player.Calamity().StealthStrikeAvailable()) //setting the stealth strike
            {
                int stealth = Projectile.NewProjectile(position, new Vector2(speedX, speedY), ModContent.ProjectileType<SkyStabberProj>(), damage, knockBack, player.whoAmI, 0f, 0f);
                Main.projectile[stealth].Calamity().stealthStrike = true;
                return false;
            }
            return true;
        }

        public override bool AltFunctionUse(Player player)
        {
            return true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);

            recipe.AddIngredient(ItemID.SpikyBall, 100);
            recipe.AddIngredient(ItemID.Cloud, 10);
            recipe.AddIngredient(ModContent.ItemType<AerialiteBar>(), 4);
            recipe.AddTile(TileID.SkyMill);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }

    }
}
