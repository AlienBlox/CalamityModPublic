using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Potions
{
    public class SupremeHealingPotion : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Supreme Healing Potion");
        }

        public override void SetDefaults()
        {
            item.width = 28;
            item.height = 18;
            item.useTurn = true;
            item.maxStack = 999;
            item.healLife = 250;
            item.useAnimation = 17;
            item.useTime = 17;
            item.useStyle = ItemUseStyleID.EatingUsing;
            item.UseSound = SoundID.Item3;
            item.consumable = true;
            item.potion = true;
            item.rare = 10;
            item.value = Item.buyPrice(0, 6, 50, 0);
            item.Calamity().customRarity = CalamityRarity.Turquoise;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.SuperHealingPotion);
            recipe.AddIngredient(ModContent.ItemType<UnholyEssence>());
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
