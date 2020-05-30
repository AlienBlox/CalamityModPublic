using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    public class EtherealTalisman : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ethereal Talisman");
            Tooltip.SetDefault("15% increased magic damage, 5% increased magic critical strike chance, and 10% decreased mana usage\n" +
                "+150 max mana and reveals treasure locations if visibility is on\n" +
                "Increases pickup range for mana stars and you restore mana when damaged\n" +
                "You automatically use mana potions when needed if visibility is on\n" +
                "Magic attacks have a chance to instantly kill normal enemies");
        }

        public override void SetDefaults()
        {
            item.width = 28;
            item.height = 32;
            item.value = Item.buyPrice(0, 90, 0, 0);
            item.accessory = true;
            item.Calamity().customRarity = CalamityRarity.DarkBlue;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.eTalisman = true;
            if (!hideVisual)
            {
                player.findTreasure = true;
                player.manaFlower = true;
            }
            player.magicCuffs = true;
            player.manaMagnet = true;
            player.statManaMax2 += 150;
            player.magicDamage += 0.15f;
            player.manaCost *= 0.9f;
            player.magicCrit += 5;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<SigilofCalamitas>());
            recipe.AddIngredient(ItemID.ManaFlower);
            recipe.AddIngredient(ModContent.ItemType<Phantoplasm>(), 20);
            recipe.AddIngredient(ModContent.ItemType<NightmareFuel>(), 20);
            recipe.AddIngredient(ModContent.ItemType<EndothermicEnergy>(), 20);
            recipe.AddTile(ModContent.TileType<DraedonsForge>());
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
