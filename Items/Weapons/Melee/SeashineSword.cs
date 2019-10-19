using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using CalamityMod.Projectiles.Melee;

namespace CalamityMod.Items.Weapons.Melee
{
    public class SeashineSword : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Seashine Sword");
            Tooltip.SetDefault("Shoots an aqua sword beam");
        }

        public override void SetDefaults()
        {
            item.CloneDefaults(ItemID.EnchantedSword);
            item.damage = 26;
            item.melee = true;
            item.value = Item.buyPrice(0, 2, 0, 0);
            item.width = 38;
            item.height = 38;
            item.knockBack = 2;
            item.shootSpeed = 11;
            item.rare = 2;
            item.UseSound = SoundID.Item1;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            type = ModContent.ProjectileType<SeashineSwordProj>();
            return base.Shoot(player, ref position, ref speedX, ref speedY, ref type, ref damage, ref knockBack);
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "SeaPrism", 7);
            recipe.AddIngredient(null, "Navystone", 10);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
