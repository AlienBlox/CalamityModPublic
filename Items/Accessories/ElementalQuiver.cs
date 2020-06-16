using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    public class ElementalQuiver : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Elemental Quiver");
            Tooltip.SetDefault("Ranged projectiles have a chance to split\n" +
                "Ranged weapons have a chance to instantly kill normal enemies\n" +
                "10% increased ranged damage, 5% increased ranged critical strike chance, and 20% reduced ammo usage\n" +
                "5 increased defense, 2 increased life regen, and 15% increased pick speed\n" +
				"Greatly increases arrow speed and grants a 20% chance to not consume arrows");
        }

        public override void SetDefaults()
        {
            item.width = 28;
            item.height = 32;
            item.value = CalamityGlobalItem.Rarity14BuyPrice;
            item.accessory = true;
            item.Calamity().customRarity = CalamityRarity.DarkBlue;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.eQuiver = true;
            player.rangedDamage += 0.1f;
            player.rangedCrit += 5;
            player.ammoCost80 = true;
            player.lifeRegen += 2;
            player.statDefense += 5;
            player.pickSpeed -= 0.15f;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.MagicQuiver);
            recipe.AddIngredient(ModContent.ItemType<DaedalusEmblem>());
            recipe.AddIngredient(ModContent.ItemType<Phantoplasm>(), 20);
            recipe.AddIngredient(ModContent.ItemType<NightmareFuel>(), 20);
            recipe.AddIngredient(ModContent.ItemType<EndothermicEnergy>(), 20);
            recipe.AddTile(ModContent.TileType<DraedonsForge>());
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
