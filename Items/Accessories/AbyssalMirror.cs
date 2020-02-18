﻿using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    public class AbyssalMirror : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Abyssal Mirror");
            Tooltip.SetDefault("Light does not reach the depths of the ocean\n" +
                "Significantly reduces enemy aggression, even in the abyss\n" +
                "Stealth generates 30% faster when standing still and 20% faster while moving\n" +
                "Grants a slight chance to evade attacks, releasing a cloud of lumenyl fluid which damages and stuns nearby enemies\n" +
                "Evading an attack grants a lot of stealth\n" +
                "This evade has a 20s cooldown before it can occur again");
        }

        public override void SetDefaults()
        {
            item.width = 30;
            item.height = 38;
            item.value = Item.buyPrice(0, 6, 0, 0);
            item.rare = 8;
            item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.stealthGenStandstill += 0.3f;
            modPlayer.stealthGenMoving += 0.2f;
            modPlayer.abyssalMirror = true;
            player.aggro -= 450;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<MirageMirror>());
            recipe.AddIngredient(ModContent.ItemType<InkBomb>());
            recipe.AddIngredient(ItemID.SpectreBar, 8);
            recipe.AddIngredient(ModContent.ItemType<DepthCells>(), 5);
            recipe.AddIngredient(ModContent.ItemType<Lumenite>(), 5);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
