﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.GameContent;

namespace CalamityMod.Items.Materials
{
    [LegacyName("EssenceofCinder")]
    public class EssenceofSunlight : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 25;
            ItemID.Sets.ItemNoGravity[Item.type] = true;

            // DisplayName.SetDefault("Essence of Sunlight");
            // Tooltip.SetDefault("The essence of sky, light, and storm creatures");
			ItemID.Sets.SortingPriorityMaterials[Type] = 71; // Soul of Light
        }

        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 26;
            Item.maxStack = 999;
            Item.value = Item.sellPrice(silver: 40);
            Item.rare = ItemRarityID.LightRed;
        }
        public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
        {
            Item.DrawItemGlowmaskSingleFrame(spriteBatch, rotation, TextureAssets.Item[Item.type].Value);
        }

        public override void Update(ref float gravity, ref float maxFallSpeed)
        {
            float brightness = Main.essScale * Main.rand.NextFloat(0.9f, 1.1f);
            Lighting.AddLight(Item.Center, 0.3f * brightness, 0.3f * brightness, 0.05f * brightness);
        }
    }
}
