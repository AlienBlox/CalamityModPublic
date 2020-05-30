using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Ores;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Armor
{
    [AutoloadEquip(EquipType.Legs)]
    public class AuricTeslaCuisses : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Auric Tesla Cuisses");
            Tooltip.SetDefault("50% increased movement speed\n" +
                "12% increased damage and 5% increased critical strike chance\n" +
                "Magic carpet effect");
        }

        public override void SetDefaults()
        {
            item.width = 18;
            item.height = 18;
            item.value = Item.buyPrice(1, 8, 0, 0);
            item.defense = 44;
            item.Calamity().customRarity = CalamityRarity.Violet;
        }

        public override void UpdateEquip(Player player)
        {
            player.moveSpeed += 0.5f;
            player.carpet = true;
            player.allDamage += 0.12f;
            player.Calamity().AllCritBoost(5);
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<SilvaLeggings>());
            recipe.AddIngredient(ModContent.ItemType<GodSlayerLeggings>());
            recipe.AddIngredient(ModContent.ItemType<BloodflareCuisses>());
            recipe.AddIngredient(ModContent.ItemType<TarragonLeggings>());
			recipe.AddIngredient(ModContent.ItemType<AuricBar>(), 14);
			recipe.AddIngredient(ItemID.FlyingCarpet);
            recipe.AddTile(ModContent.TileType<DraedonsForge>());
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
