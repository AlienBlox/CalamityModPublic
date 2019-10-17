using Terraria;
using CalamityMod.Projectiles;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Items
{
    public class MineralMortar : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Mineral Mortar");
            Tooltip.SetDefault("Shoots an onyx bomb that explodes into sand sharks on death");
        }

        public override void SetDefaults()
        {
            item.damage = 30;
            item.ranged = true;
            item.width = 58;
            item.height = 26;
            item.useTime = 33;
            item.useAnimation = 33;
            item.useStyle = 5;
            item.noMelee = true;
            item.knockBack = 5f;
            item.value = Item.buyPrice(0, 36, 0, 0);
            item.rare = 5;
            item.UseSound = SoundID.Item11;
            item.autoReuse = true;
            item.shootSpeed = 14f;
            item.shoot = ModContent.ProjectileType<OnyxSharkBomb>();
            item.useAmmo = 771;
        }

        public override bool Shoot(Player player, ref Microsoft.Xna.Framework.Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Projectile.NewProjectile(position.X, position.Y, speedX, speedY, ModContent.ProjectileType<OnyxSharkBomb>(), damage, knockBack, player.whoAmI, 0f, 0f);
            return false;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.AncientBattleArmorMaterial, 2);
            recipe.AddIngredient(ItemID.AdamantiteBar, 13);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
            recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.AncientBattleArmorMaterial, 2);
            recipe.AddIngredient(ItemID.TitaniumBar, 13);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
