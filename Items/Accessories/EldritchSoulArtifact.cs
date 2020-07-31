using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Plates;
using CalamityMod.Items.Placeables.Ores;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    public class EldritchSoulArtifact : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Eldritch Soul Artifact");
            Tooltip.SetDefault("Knowledge\n" +
                "Boosts melee speed by 10%, shoot speed by 25%, rogue damage by 15%, max minions by 2, and reduces mana cost by 15%");
        }

        public override void SetDefaults()
        {
            item.width = 28;
            item.height = 28;
            item.value = CalamityGlobalItem.Rarity12BuyPrice;
            item.accessory = true;
            item.rare = 10;
            item.Calamity().postMoonLordRarity = 12;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.eArtifact = true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<Cinderplate>(), 5);
            recipe.AddIngredient(ModContent.ItemType<EssenceofChaos>(), 10);
            recipe.AddIngredient(ModContent.ItemType<Phantoplasm>(), 10);
            recipe.AddIngredient(ModContent.ItemType<ExodiumClusterOre>(), 15);
            recipe.AddTile(TileID.DemonAltar);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
