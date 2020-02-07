using CalamityMod.Items.Materials;
using CalamityMod.Items.Tools;
using CalamityMod.Projectiles.Summon;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Summon
{
    public class ElementalAxe : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Elemental Axe");
            Tooltip.SetDefault("Summons an elemental axe to fight for you");
        }

        public override void SetDefaults()
        {
            item.damage = 140;
            item.summon = true;
            item.mana = 10;
            item.width = 36;
            item.height = 36;
            item.useTime = 36;
            item.useAnimation = 36;
            item.useStyle = 1;
            item.noMelee = true;
            item.knockBack = 5f;
            item.value = Item.buyPrice(1, 20, 0, 0);
            item.rare = 10;
            item.UseSound = SoundID.Item44;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<ElementalAxeMinion>();
            item.shootSpeed = 10f;
            item.Calamity().postMoonLordRarity = 12;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (player.altFunctionUse != 2)
            {
                position = Main.MouseWorld;
                speedX = 0;
                speedY = 0;
                Projectile.NewProjectile(position.X, position.Y, speedX, speedY, type, damage, knockBack, player.whoAmI);
            }
            return false;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<BarofLife>(), 5);
            recipe.AddIngredient(ItemID.LunarBar, 5);
            recipe.AddIngredient(ModContent.ItemType<GalacticaSingularity>(), 5);
            recipe.AddIngredient(ModContent.ItemType<PlantationStaff>());
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
