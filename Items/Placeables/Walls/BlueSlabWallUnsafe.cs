﻿using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Items.Placeables.Walls
{
    public class BlueSlabWallUnsafe : ModItem
    {
        public override string Texture => $"Terraria/Images/Item_{ItemID.BlueSlabWall}";

        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 400;
        }

        public override void SetDefaults()
        {
            Item.width = 12;
            Item.height = 12;
            Item.maxStack = 9999;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.useAnimation = 15;
            Item.useTime = 7;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.consumable = true;
            Item.createWall = WallID.BlueDungeonSlabUnsafe;
        }
    }
}
