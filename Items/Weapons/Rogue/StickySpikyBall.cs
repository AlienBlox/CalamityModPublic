using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Rogue;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class StickySpikyBall : RogueWeapon
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sticky Spiky Ball");
            Tooltip.SetDefault(@"Throws a spiky ball that sticks to everything
Stealth strikes throw seven at once and last a lot longer");
        }

        public override void SafeSetDefaults()
        {
            item.width = 14;
            item.damage = 12;
            item.noMelee = true;
            item.noUseGraphic = true;
			item.maxStack = 999;
			item.consumable = true;
            item.useAnimation = 20;
            item.useStyle = 1;
            item.useTime = 20;
            item.knockBack = 3f;
            item.UseSound = SoundID.Item1;
            item.height = 14;
            item.value = Item.buyPrice(0, 0, 1, 0);
            item.rare = 1;
            item.shoot = ModContent.ProjectileType<StickyBol>();
            item.shootSpeed = 8f;
            item.Calamity().rogue = true;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (player.Calamity().StealthStrikeAvailable()) //setting the stealth strike
            {
                int spread = 3;
                for (int i = 0; i < 7; i++)
                {
                    Vector2 perturbedspeed = new Vector2(speedX + Main.rand.Next(-3,4), speedY + Main.rand.Next(-3,4)).RotatedBy(MathHelper.ToRadians(spread));
                    int proj = Projectile.NewProjectile(position.X, position.Y, perturbedspeed.X, perturbedspeed.Y, type, item.damage, item.knockBack, player.whoAmI, 0f, 0f);
                    Main.projectile[proj].Calamity().stealthStrike = true;
                    Main.projectile[proj].timeLeft *= 4;
                    spread -= Main.rand.Next(1,4);
                }
                return false;
            }
            return true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.SpikyBall, 3);
            recipe.AddIngredient(ItemID.Gel);
            recipe.AddTile(TileID.WorkBenches);
            recipe.SetResult(this, 3);
            recipe.AddRecipe();
        }
    }
}
