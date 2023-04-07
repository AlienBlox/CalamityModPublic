﻿using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Placeables.Furniture
{
    public class RefractivePrismTorch : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 100;
            // Tooltip.SetDefault("Can be placed in water");
			ItemID.Sets.Torches[Item.type] = true;
			// Right now this causes some Cursed Inferno dust until tmod fixes AutoLightSelect, it's a small sacrifice
			ItemID.Sets.WaterTorches[Item.type] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 14;
            Item.height = 16;
            Item.maxStack = 99;
            Item.holdStyle = 1;
            Item.noWet = false;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.consumable = true;
            Item.createTile = ModContent.TileType<Tiles.SunkenSea.RefractivePrismTorch>();
            Item.flame = true;
            Item.value = 500;
        }

		public override void ModifyResearchSorting(ref ContentSamples.CreativeHelper.ItemGroup itemGroup)
		{
			// Vanilla usually matches sorting methods with the right type of item, but sometimes, like with torches, it doesn't. Make sure to set whichever items manually if need be.
			itemGroup = ContentSamples.CreativeHelper.ItemGroup.Torches;
		}

        public override void HoldItem(Player player)
        {
            if (Main.rand.NextBool(player.itemAnimation > 0 ? 10 : 20))
            {
                Dust.NewDust(new Vector2(player.itemLocation.X + 16f * player.direction, player.itemLocation.Y - 14f * player.gravDir), 4, 4, Main.rand.Next(68, 71));
            }
            Vector2 position = player.RotatedRelativePoint(new Vector2(player.itemLocation.X + 12f * player.direction + player.velocity.X, player.itemLocation.Y - 14f + player.velocity.Y), true);
            Lighting.AddLight(position, 1f, 0.9f, 1.2f);
        }

        public override void PostUpdate()
        {
            Lighting.AddLight((int)((Item.position.X + Item.width / 2) / 16f), (int)((Item.position.Y + Item.height / 2) / 16f), 1f, 0.9f, 1.2f);
        }

		// This function doesn't even work....
        public override void AutoLightSelect(ref bool dryTorch, ref bool wetTorch, ref bool glowstick)
        {
            wetTorch = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe(3).
                AddIngredient(ItemID.Torch, 3).
                AddIngredient(ModContent.ItemType<PrismShard>()).
                Register();
        }
    }
}
