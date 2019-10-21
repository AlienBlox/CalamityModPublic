using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Typeless;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class GalaxySmasherMelee : ModItem
    {
        public static int BaseDamage = 855;
        public static float Speed = 18f;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Galaxy Smasher");
            Tooltip.SetDefault("Explodes and summons death lasers on enemy hits");
        }

        public override void SetDefaults()
        {
            item.width = 86;
            item.height = 72;
            item.melee = true;
            item.damage = BaseDamage;
            item.knockBack = 9f;
            item.useAnimation = 13;
            item.useTime = 13;
            item.autoReuse = true;
            item.noMelee = true;
            item.noUseGraphic = true;

            item.useStyle = 1;
            item.UseSound = SoundID.Item1;

            item.rare = 10;
            item.Calamity().postMoonLordRarity = 14;
            item.value = Item.buyPrice(1, 80, 0, 0);

            item.shoot = ModContent.ProjectileType<GalaxySmasherHammer>();
            item.shootSpeed = Speed;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            int proj = Projectile.NewProjectile(position, new Vector2(speedX, speedY), type, damage, knockBack, player.whoAmI);
            Main.projectile[proj].Calamity().forceMelee = true;
            return false;
        }

        public override void AddRecipes()
        {
            ModRecipe r = new ModRecipe(mod);
            r.SetResult(this);
            r.AddTile(ModContent.TileType<DraedonsForge>());
            r.AddIngredient(ModContent.ItemType<StellarContemptMelee>());
            r.AddIngredient(ModContent.ItemType<CosmiliteBar>(), 10);
            r.AddIngredient(ModContent.ItemType<NightmareFuel>(), 10);
            r.AddIngredient(ModContent.ItemType<EndothermicEnergy>(), 10);
            r.AddRecipe();
        }
    }
}
