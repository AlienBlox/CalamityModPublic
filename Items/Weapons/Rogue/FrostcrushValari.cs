using CalamityMod.CalPlayer;
using CalamityMod.Projectiles.Rogue;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class FrostcrushValari : RogueWeapon
    {
        public static float Speed = 15f;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Frostcrush Valari");
            Tooltip.SetDefault(@"Fires a long ranged boomerang that explodes into icicles on hit
Stealth strikes throws three short ranged boomerangs along with a spread of icicles");
        }

        public override void SafeSetDefaults()
        {
            item.damage = 70;
            item.knockBack = 12;
            item.thrown = true;
            item.crit = 16;
            item.value = Item.buyPrice(0, 60, 0, 0);
            item.rare = 7;
            item.useTime = 19;
            item.useAnimation = 19;
            item.width = 32;
            item.height = 46;
            item.useStyle = 1;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.shootSpeed = Speed;
            item.shoot = ModContent.ProjectileType<ValariBoomerang>();
            item.noMelee = true;
            item.noUseGraphic = true;
            item.Calamity().rogue = true;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            CalamityPlayer modPlayer = Main.player[Main.myPlayer].Calamity();
            //If stealth is full, shoot a spread of 3 boomerangs with reduced range and 6 to 10 icicles
            if (modPlayer.StealthStrikeAvailable())
            {
                int spread = 10;
                for (int i = 0; i < 3; i++)
                {
                    Vector2 perturbedspeed = new Vector2(speedX, speedY).RotatedBy(MathHelper.ToRadians(spread));
                    int proj = Projectile.NewProjectile(position.X, position.Y, perturbedspeed.X, perturbedspeed.Y, type, damage, knockBack, player.whoAmI, 0f, 1f);
                    Main.projectile[proj].Calamity().stealthStrike = true;
                    spread -= 10;
                }
                int spread2 = 3;
                for (int i = 0; i < Main.rand.Next(6,11); i++)
                {
                    Vector2 perturbedspeed = new Vector2(speedX + Main.rand.Next(-3,4), speedY + Main.rand.Next(-3,4)).RotatedBy(MathHelper.ToRadians(spread2));
                    Projectile.NewProjectile(position.X, position.Y, perturbedspeed.X, perturbedspeed.Y, (Main.rand.NextBool(2) ? ModContent.ProjectileType<Valaricicle>() : ModContent.ProjectileType<Valaricicle2>()), damage / 2, 0f, player.whoAmI, 0f, 0f);
                    spread2 -= Main.rand.Next(1,4);
                }
                return false;
            }
            return true;
        }

        /*public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<Kylie>());
            recipe.AddIngredient(ModContent.ItemType<CryoBar>(), 6);
            recipe.AddIngredient(ModContent.ItemType<Voidstone>(), 40);
            recipe.AddIngredient(ModContent.ItemType<CoreofEleum>(), 5);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }*/
    }
}
