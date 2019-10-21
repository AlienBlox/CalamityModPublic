﻿using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
namespace CalamityMod.Items.SummonItems
{
    public class SandstormsCore : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sandstorm's Core");
            Tooltip.SetDefault("Summons the Great Sand Shark");
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.maxStack = 20;
            item.rare = 7;
            item.useAnimation = 45;
            item.useTime = 45;
            item.useStyle = 4;
            item.consumable = true;
        }

        public override bool CanUseItem(Player player)
        {
            return player.ZoneDesert && !NPC.AnyNPCs(ModContent.NPCType<GreatSandShark>());
        }

        public override bool UseItem(Player player)
        {
            NPC.SpawnOnPlayer(player.whoAmI, ModContent.NPCType<GreatSandShark>());
            Main.PlaySound(SoundID.Roar, player.position, 0);
            return true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.AncientBattleArmorMaterial, 2);
            recipe.AddIngredient(ItemID.Ectoplasm, 5);
            recipe.AddIngredient(ItemID.HallowedBar, 5);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
