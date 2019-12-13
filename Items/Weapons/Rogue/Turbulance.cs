using CalamityMod.Projectiles.Rogue;
using CalamityMod.Items.Materials;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class Turbulance : RogueWeapon
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Turbulance");
            Tooltip.SetDefault(@"Fires a cloudy javelin that bursts into wind slashes on hit
Wind slashes home if the javelin crits
Stealth strikes are trailed by homing wind slashes");
        }

        public override void SafeSetDefaults()
        {
            item.width = 14;
            item.damage = 20;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.useAnimation = 18;
            item.useStyle = 1;
            item.useTime = 18;
            item.knockBack = 5f;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.height = 14;
            item.value = Item.buyPrice(0, 2, 0, 0);
            item.rare = 3;
            item.shoot = ModContent.ProjectileType<TurbulanceProjectile>();
            item.shootSpeed = 12f;
            item.Calamity().rogue = true;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (player.Calamity().StealthStrikeAvailable()) //setting the stealth strike
            {
                int stealth = Projectile.NewProjectile(position, new Vector2(speedX, speedY), type, damage, knockBack, player.whoAmI, 0f, 0f);
                Main.projectile[stealth].Calamity().stealthStrike = true;
                return false;
            }
            return true;
        }

        /*public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<AerialiteBar>(), 5);
            recipe.AddIngredient(ItemID.SunplateBlock, 3);
            recipe.AddIngredient(ItemID.Cloud, 4);
            recipe.AddTile(TileID.SkyMill);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }*/
    }
}
