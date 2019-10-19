using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using CalamityMod.Projectiles.Magic;

namespace CalamityMod.Items.Weapons.Magic
{
    public class FrigidflashBolt : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Frigidflash Bolt");
            Tooltip.SetDefault("Casts a slow-moving ball of flash-freezing magma");
        }

        public override void SetDefaults()
        {
            item.damage = 45;
            item.magic = true;
            item.mana = 13;
            item.width = 28;
            item.height = 30;
            item.useTime = 15;
            item.useAnimation = 15;
            item.useStyle = 5;
            item.noMelee = true;
            item.knockBack = 5.5f;
            item.value = Item.buyPrice(0, 36, 0, 0);
            item.rare = 5;
            item.UseSound = SoundID.Item21;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<FrigidflashBoltProjectile>();
            item.shootSpeed = 6.5f;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<FrostBolt>());
            recipe.AddIngredient(ModContent.ItemType<FlareBolt>());
            recipe.AddIngredient(ModContent.ItemType<EssenceofEleum>(), 2);
            recipe.AddIngredient(ModContent.ItemType<EssenceofChaos>(), 2);
            recipe.AddTile(TileID.Bookcases);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
