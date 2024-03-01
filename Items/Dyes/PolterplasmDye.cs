﻿using CalamityMod.Items.Materials;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Dyes
{
    [LegacyName("PhantoplasmDye")]
    public class PolterplasmDye : BaseDye
    {
        public override ArmorShaderData ShaderDataToBind => new ArmorShaderData(new Ref<Effect>(Mod.Assets.Request<Effect>("Effects/Dyes/PolterplasmDyeShader", AssetRequestMode.ImmediateLoad).Value), "DyePass").
            UseColor(new Color(245, 143, 182)).UseSecondaryColor(new Color(119, 238, 255)).UseImage("Images/Misc/Perlin");

        public override void SafeSetStaticDefaults()
        {
            Item.ResearchUnlockCount = 3;
        }

        public override void SafeSetDefaults()
        {
            Item.rare = ItemRarityID.Purple;
            Item.value = Item.sellPrice(0, 2, 50, 0);
        }

        public override void AddRecipes()
        {
            CreateRecipe(2).
                AddIngredient(ItemID.BottledWater, 2).
                AddIngredient<Polterplasm>(3).
                AddTile(TileID.DyeVat).
                Register();
        }
    }
}
