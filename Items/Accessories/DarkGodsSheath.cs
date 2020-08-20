using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    public class DarkGodsSheath : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Dark God's Sheath");
            Tooltip.SetDefault("+20 maximum stealth\n" +
                "Mobile stealth generation accelerates while not attacking\n" +
                "Stealth strikes have a 100% critical hit chance\n" +
                "Stealth strikes only expend 50% of your max stealth\n" +
                "6% increased rogue damage, and 6% increased rogue crit chance");
        }

        public override void SetDefaults()
        {
            item.width = 48;
            item.height = 62;
            item.value = CalamityGlobalItem.Rarity9BuyPrice;
            item.rare = 9;
            item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.stealthStrikeAlwaysCrits = true;
            modPlayer.stealthStrikeHalfCost = true;
            modPlayer.rogueStealthMax += 0.2f;
            modPlayer.darkGodSheath = true;
            modPlayer.throwingCrit += 6;
            modPlayer.throwingDamage += 0.06f;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<SilencingSheath>());
            recipe.AddIngredient(ModContent.ItemType<RuinMedallion>());
            recipe.AddIngredient(ModContent.ItemType<MeldiateBar>(), 5);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
