using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Summon;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Summon
{
    public class CausticCroakerStaff : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Caustic Croaker Staff");
            Tooltip.SetDefault("Summons a toad that explodes if enemies are nearby");
        }

        public override void SetDefaults()
        {
            item.damage = 38;
            item.mana = 10;
            item.width = 10;
            item.height = 32;
            item.useTime = item.useAnimation = 33;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.noMelee = true;
            item.knockBack = 0f;
            item.value = Item.buyPrice(0, 2, 0, 0);
            item.rare = 1;
            item.UseSound = SoundID.Item44;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<EXPLODINGFROG>();
            item.shootSpeed = 10f;
            item.summon = true;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (player.altFunctionUse != 2)
            {
                position = Main.MouseWorld;
                Point mouseTileCoords = position.ToTileCoordinates();
                if (WorldGen.SolidTile(mouseTileCoords.X, mouseTileCoords.Y + 1))
                    return false;
                Projectile.NewProjectile(position, Vector2.Zero, type, damage, knockBack, player.whoAmI);
                player.UpdateMaxTurrets();
            }
            return false;
        }
        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<SulfuricScale>(), 20);
            recipe.AddIngredient(ModContent.ItemType<Acidwood>(), 15);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
        }
    }
}
