using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Armor
{
    [AutoloadEquip(EquipType.Legs)]
    public class TarragonLeggings : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Tarragon Leggings");
            Tooltip.SetDefault("Leggings of a fabled explorer\n" +
				"20% increased movement speed; 35% increase when below half health\n" +
                "6% increased damage and critical strike chance");
        }

        public override void SetDefaults()
        {
            item.width = 18;
            item.height = 18;
            item.value = Item.buyPrice(0, 30, 0, 0);
            item.defense = 32;
            item.Calamity().customRarity = CalamityRarity.Turquoise;
        }

        public override void UpdateEquip(Player player)
        {
            player.moveSpeed += 0.20f;
            if (player.statLife <= (int)((double)player.statLifeMax2 * 0.5))
            {
                player.moveSpeed += 0.15f;
            }
            player.allDamage += 0.06f;
            player.Calamity().AllCritBoost(6);
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<UeliaceBar>(), 11);
            recipe.AddIngredient(ModContent.ItemType<DivineGeode>(), 12);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
