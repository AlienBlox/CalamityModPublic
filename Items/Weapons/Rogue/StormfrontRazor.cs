﻿using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Rogue;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class StormfrontRazor : RogueWeapon
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Stormfront Razor");
            Tooltip.SetDefault("Throws a throwing knife that leaves sparks as it travels.\n" +
                               "Stealth strike causes the knife to be faster and leave a huge shower of sparks as it travels");
        }

        public override void SafeSetDefaults()
        {
            item.width = 40;
            item.height = 42;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.useStyle = 1;
            item.value = Item.buyPrice(0, 30, 0, 0);
            item.rare = 5;

            item.useAnimation = 15;
            item.useTime = 15;
            item.damage = 50;
            item.crit += 8;
            item.knockBack = 7f;
            item.shoot = ModContent.ProjectileType<StormfrontRazorProjectile>();
            item.shootSpeed = 7f;
            item.Calamity().rogue = true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<Cinquedea>());
            recipe.AddIngredient(ItemID.HallowedBar, 6);
            recipe.AddIngredient(ModContent.ItemType<EssenceofCinder>(), 4);
            recipe.AddIngredient(ItemID.Feather, 8);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (player.Calamity().StealthStrikeAvailable())
            {
                int p = Projectile.NewProjectile(position, new Vector2(speedX * 1.6f, speedY * 1.6f), ModContent.ProjectileType<StormfrontRazorProjectile>(), damage, knockBack, player.whoAmI, 0, 40f);
                Main.projectile[p].Calamity().stealthStrike = true;
                return false;
            }
            else
            {
                Projectile.NewProjectile(position, new Vector2(speedX, speedY), ModContent.ProjectileType<StormfrontRazorProjectile>(), damage, knockBack, player.whoAmI, 0, 1);
                return false;
            }
        }
    }
}
