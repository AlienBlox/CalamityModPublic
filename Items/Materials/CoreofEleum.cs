using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Materials
{
    public class CoreofEleum : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Core of Eleum");
        }

        public override void SetDefaults()
        {
            item.width = 34;
            item.height = 34;
            item.maxStack = 999;
            item.value = Item.sellPrice(silver: 40);
            item.rare = 8;
        }

		public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
		{
			Texture2D texture = ModContent.GetTexture("CalamityMod/Items/Materials/CoreofEleumGlow");
			Vector2 origin = new Vector2(texture.Width / 2f, texture.Height / 2f - 2f);
			spriteBatch.Draw(texture, item.Center - Main.screenPosition, null, Color.White, rotation, origin, 1f, SpriteEffects.None, 0f);
		}

        public override void Update(ref float gravity, ref float maxFallSpeed)
        {
            float brightness = Main.essScale * Main.rand.NextFloat(0.9f, 1.1f);
            Lighting.AddLight(item.Center, 0.15f * brightness, 0.05f * brightness, 0.5f * brightness);
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<EssenceofEleum>());
            recipe.AddIngredient(ItemID.Ectoplasm);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this, 3);
            recipe.AddRecipe();
        }
    }
}
