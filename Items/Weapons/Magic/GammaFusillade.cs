using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Magic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Magic
{
    public class GammaFusillade : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Biofusillade");
            Tooltip.SetDefault("Unleashes a concentrated beam of life energy");
        }

        public override void SetDefaults()
        {
            item.damage = 110;
            item.magic = true;
            item.mana = 4;
            item.width = 28;
            item.height = 30;
            item.useTime = 3;
            item.useAnimation = 3;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 3f;
            item.value = Item.buyPrice(1, 20, 0, 0);
            item.rare = 10;
            item.UseSound = SoundID.Item33;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<GammaLaser>();
            item.shootSpeed = 20f;
            item.Calamity().customRarity = CalamityRarity.Turquoise;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<UeliaceBar>(), 8);
            recipe.AddIngredient(ItemID.SpellTome);
            recipe.AddTile(TileID.Bookcases);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
