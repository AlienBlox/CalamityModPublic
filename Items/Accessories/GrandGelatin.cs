using CalamityMod.Items.Materials;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    public class GrandGelatin : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Grand Gelatin");
            Tooltip.SetDefault("10% increased movement speed\n" +
                "200% increased jump speed\n" +
                "+40 max life and mana\n" +
                "Standing still boosts life and mana regen");
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 24;
            item.value = CalamityGlobalItem.Rarity5BuyPrice;
            item.rare = 5;
            item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.moveSpeed += 0.1f;
            player.jumpSpeedBoost += player.autoJump ? 0.5f : 2.0f;
            player.statLifeMax2 += 40;
            player.statManaMax2 += 40;
            if (Math.Abs(player.velocity.X) < 0.05f && Math.Abs(player.velocity.Y) < 0.05f && player.itemAnimation == 0)
            {
                player.lifeRegen += 2;
                player.manaRegenBonus += 2;
            }
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<ManaJelly>());
            recipe.AddIngredient(ModContent.ItemType<LifeJelly>());
            recipe.AddIngredient(ModContent.ItemType<VitalJelly>());
            recipe.AddIngredient(ModContent.ItemType<VerstaltiteBar>(), 3);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
