using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using CalamityMod.Projectiles.Ranged;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class TheBallista : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("The Ballista");
            Tooltip.SetDefault("Shoots greatarrows that crush enemy armor and break into shards on death");
        }

        public override void SetDefaults()
        {
            item.damage = 125;
            item.ranged = true;
            item.width = 40;
            item.height = 70;
            item.useTime = 20;
            item.useAnimation = 20;
            item.useStyle = 5;
            item.noMelee = true;
            item.knockBack = 8f;
            item.value = Item.buyPrice(0, 80, 0, 0);
            item.rare = 8;
            item.UseSound = SoundID.Item5;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<BallistaGreatArrow>();
            item.shootSpeed = 20f;
            item.useAmmo = 40;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Projectile.NewProjectile(position.X, position.Y, speedX, speedY, ModContent.ProjectileType<BallistaGreatArrow>(), damage, knockBack, player.whoAmI, 0f, 0f);
            return false;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.Marrow);
            recipe.AddIngredient(ItemID.AncientBattleArmorMaterial);
            recipe.AddIngredient(ItemID.Ectoplasm, 10);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
