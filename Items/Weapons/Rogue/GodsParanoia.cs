using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Rogue;
using CalamityMod.Tiles.Furniture.CraftingStations;
using CalamityMod.CalPlayer;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class GodsParanoia : RogueWeapon
    {
        private static int damage = 125;
        private static int knockBack = 5;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("God's Paranoia");
            Tooltip.SetDefault(@"Shoots a speedy homing spiky ball. Stacks up to 10.
Attaches to enemies and summons localized storm of god slayer kunai
Stealth strikes home in faster and summon kunai at a faster rate
Right click to delete all existing spiky balls");
        }

        public override void SafeSetDefaults()
        {
            item.damage = damage;
            item.Calamity().rogue = true;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.width = 1;
            item.height = 1;
            item.useTime = 15;
            item.useAnimation = 15;
            item.useStyle = 1;
            item.knockBack = knockBack;
            item.value = Item.buyPrice(0, 18, 0, 0);
            item.rare = 10;
            item.Calamity().postMoonLordRarity = 14;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.maxStack = 10;

            item.shootSpeed = 5f;
            item.shoot = ModContent.ProjectileType<GodsParanoiaProj>();

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
				int UseMax = item.stack - 1;

				if (player.ownedProjectileCounts[item.shoot] > UseMax)
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
                int stealth = Projectile.NewProjectile(position, new Vector2(speedX, speedY), type, damage, knockBack, player.whoAmI, 0f, 0f);
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

            recipe.AddIngredient(ItemID.SpikyBall, 200);
            recipe.AddIngredient(ModContent.ItemType<CosmiliteBar>(), 2);
            recipe.AddIngredient(ModContent.ItemType<NightmareFuel>(), 2);
            recipe.AddTile(ModContent.TileType<DraedonsForge>());
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
