﻿using CalamityMod.Items.Placeables.Pylons;
using CalamityMod.Systems;
using CalamityMod.Tiles.BaseTiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace CalamityMod.Tiles.Pylons
{
    public class AstralPylonTile : BasePylonTile
    {
        public override Color LightColor => new Color(0.8f, 0.5f, 0.8f);
        public override int AssociatedItem => ModContent.ItemType<AstralPylon>();
        public override Color PylonMapColor => Color.Coral;
        public override Color DustColor => Main.rand.NextBool() ? Color.Coral : Color.MediumTurquoise;


        public override NPCShop.Entry GetNPCShopEntry()/* tModPorter See ExamplePylonTile for an example. To register to specific NPC shops, use the new shop system directly in ModNPC.AddShop, GlobalNPC.ModifyShop or ModSystem.PostAddRecipes */
        {
            return isNPCHappyEnough && player.Calamity().ZoneAstral ? AssociatedItem : null;
        }


        public override bool ValidTeleportCheck_BiomeRequirements(TeleportPylonInfo pylonInfo, SceneMetrics sceneData) => BiomeTileCounterSystem.AstralTiles >= 100;
    }
}
