using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Melee;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class Mourningstar : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Mourningstar");
            Tooltip.SetDefault("Launches two solar whip swords that explode on hit");
        }

        public override void SetDefaults()
        {
            item.width = 16;
            item.height = 16;
            item.damage = 230;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.channel = true;
            item.autoReuse = true;
            item.melee = true;
            item.useAnimation = 10;
            item.useTime = 10;
            item.useStyle = 5;
            item.knockBack = 2.5f;
            item.UseSound = SoundID.Item116;
            item.value = Item.buyPrice(1, 20, 0, 0);
            item.rare = 10;
            item.shootSpeed = 24f;
            item.shoot = ModContent.ProjectileType<MourningstarFlail>();
            item.Calamity().customRarity = CalamityRarity.Turquoise;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<CoreofChaos>(), 6);
            recipe.AddIngredient(ModContent.ItemType<CoreofCinder>(), 6);
            recipe.AddIngredient(ModContent.ItemType<DivineGeode>(), 6);//Quantities may or may not be intentional hue
            recipe.AddIngredient(ItemID.SolarEruption);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            float ai3 = (Main.rand.NextFloat() - 0.75f) * 0.7853982f; //0.5
            float ai3X = (Main.rand.NextFloat() - 0.25f) * 0.7853982f; //0.5
            Projectile.NewProjectile(position.X, position.Y, speedX, speedY, type, damage, knockBack, player.whoAmI, 0f, ai3);
            Projectile.NewProjectile(position.X, position.Y, speedX, speedY, type, damage, knockBack, player.whoAmI, 0f, ai3X);
            return false;
        }
    }
}
