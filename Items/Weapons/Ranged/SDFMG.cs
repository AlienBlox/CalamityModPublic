using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables;
using CalamityMod.Projectiles.Ranged;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class SDFMG : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("SDFMG");
            Tooltip.SetDefault("It came from the edge of Terraria\n" +
                "50% chance to not consume ammo");
        }

        public override void SetDefaults()
        {
            item.damage = 200;
            item.ranged = true;
            item.width = 74;
            item.height = 34;
            item.crit += 15;
            item.useTime = 2;
            item.useAnimation = 2;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 2.75f;
            item.value = Item.buyPrice(1, 80, 0, 0);
            item.rare = 10;
            item.UseSound = SoundID.Item11;
            item.autoReuse = true;
            item.shoot = ProjectileID.PurificationPowder;
            item.shootSpeed = 16f;
            item.useAmmo = 97;
            item.Calamity().customRarity = CalamityRarity.DarkBlue;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-10, 0);
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            float SpeedX = speedX + (float)Main.rand.Next(-5, 6) * 0.05f;
            float SpeedY = speedY + (float)Main.rand.Next(-5, 6) * 0.05f;
            if (Main.rand.NextBool(5))
            {
                Projectile.NewProjectile(position.X, position.Y, SpeedX, SpeedY, ModContent.ProjectileType<FishronRPG>(), damage, knockBack, player.whoAmI, 0f, 0f);
            }
            Projectile.NewProjectile(position.X, position.Y, SpeedX, SpeedY, type, damage, knockBack, player.whoAmI, 0f, 0f);
            return false;
        }

        public override bool ConsumeAmmo(Player player)
        {
            if (Main.rand.Next(0, 100) < 50)
                return false;
            return true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.SDMG);
            recipe.AddIngredient(ModContent.ItemType<FetidEmesis>());
            recipe.AddIngredient(ModContent.ItemType<CosmiliteBar>(), 5);
            recipe.AddIngredient(ModContent.ItemType<AscendantSpiritEssence>());
            recipe.AddIngredient(ModContent.ItemType<DepthCells>(), 10);
            recipe.AddIngredient(ModContent.ItemType<Lumenite>(), 10);
            recipe.AddIngredient(ModContent.ItemType<Tenebris>(), 5);
            recipe.AddTile(ModContent.TileType<DraedonsForge>());
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
