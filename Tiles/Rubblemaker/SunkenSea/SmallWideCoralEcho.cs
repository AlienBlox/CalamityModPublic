﻿using CalamityMod.Projectiles.Environment;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent;
using Terraria.ModLoader;
using Terraria.ObjectData;
using System;

namespace CalamityMod.Tiles.Rubblemaker.SunkenSea
{
    public class SmallWideCoralEcho : ModTile
    {
        public override string Texture => "CalamityMod/Tiles/SunkenSea/SmallWideCoral";
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileLavaDeath[Type] = true;
            Main.tileWaterDeath[Type] = false;
            Main.tileNoAttach[Type] = true;
            TileObjectData.newTile.DrawYOffset = 2;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style2x1);
            TileObjectData.addTile(Type);
            AddMapEntry(new Color(137, 154, 71));
            DustType = DustID.Coralstone;
            RegisterItemDrop(ItemID.CoralstoneBlock);
            FlexibleTileWand.RubblePlacementMedium.AddVariations(ItemID.CoralstoneBlock, Type, 0);

            base.SetStaticDefaults();
        }
    }
    public class SmallWideCoral2Echo : SmallWideCoralEcho
    {
        public override string Texture => "CalamityMod/Tiles/SunkenSea/SmallWideCoral2";
    }
}
