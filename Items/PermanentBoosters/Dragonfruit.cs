﻿using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.PermanentBoosters
{
    public class Dragonfruit : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Dragonfruit");
            Tooltip.SetDefault("Though somewhat bland, what taste can be described is unlike any other experienced\n" +
                               "Permanently increases maximum life by 25\n" +
                               "Can only be used if the max amount of life fruit has been consumed");
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.useAnimation = 30;
            item.useTime = 30;
            item.useStyle = 4;
            item.UseSound = SoundID.Item4;
            item.consumable = true;
            item.rare = 10;
            item.Calamity().customRarity = CalamityRarity.Violet;
        }

        public override bool CanUseItem(Player player)
        {
            CalamityPlayer modPlayer = player.Calamity();
            if (modPlayer.dFruit || player.statLifeMax < 500)
            {
                return false;
            }
            return true;
        }

        public override bool UseItem(Player player)
        {
            if (player.itemAnimation > 0 && player.itemTime == 0)
            {
                player.itemTime = item.useTime;
                if (Main.myPlayer == player.whoAmI)
                {
                    player.HealEffect(25);
                }
                CalamityPlayer modPlayer = player.Calamity();
                modPlayer.dFruit = true;
            }
            return true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.LifeFruit, 5);
            recipe.AddIngredient(ModContent.ItemType<Phantoplasm>(), 5);
            recipe.AddIngredient(ModContent.ItemType<CosmiliteBar>(), 5);
            recipe.AddIngredient(ModContent.ItemType<DarksunFragment>(), 10);
            recipe.AddIngredient(ItemID.FragmentSolar, 15);
            recipe.AddIngredient(ModContent.ItemType<NightmareFuel>(), 5);
            recipe.AddIngredient(ModContent.ItemType<EndothermicEnergy>(), 5);
            recipe.AddTile(ModContent.TileType<DraedonsForge>());
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
