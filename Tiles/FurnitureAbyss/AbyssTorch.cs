using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Tiles.FurnitureAbyss
{
    public class AbyssTorch : ModTile
    {
        public override void SetDefaults()
        {
            this.SetUpTorch(waterImmune: true);
            ModTranslation name = CreateMapEntryName();
            name.SetDefault("Abyss Torch");
            AddMapEntry(new Color(253, 221, 3), name);
            disableSmartCursor = true;
            drop = ModContent.ItemType<Items.Placeables.FurnitureAbyss.AbyssTorch>();
            adjTiles = new int[] { TileID.Torches };
            torch = true;
        }

        public override bool CreateDust(int i, int j, ref int type)
        {
            Dust.NewDust(new Vector2(i, j) * 16f, 16, 16, 1, 0f, 0f, 1, new Color(100, 130, 150), 1f);
            return false;
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }

        public override void MouseOver(int i, int j)
        {
            Player player = Main.LocalPlayer;
            player.noThrow = 2;
            player.showItemIcon = true;
            player.showItemIcon2 = ModContent.ItemType<Items.Placeables.FurnitureAbyss.AbyssTorch>();
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            Tile tile = Main.tile[i, j];
            if (tile.frameX < 66)
            {
                r = 0.9f;
                g = 0.9f;
                b = 0.9f;
            }
        }

        public override void SetDrawPositions(int i, int j, ref int width, ref int offsetY, ref int height)
        {
            offsetY = 0;
            if (WorldGen.SolidTile(i, j - 1))
            {
                offsetY = 2;
                if (WorldGen.SolidTile(i - 1, j + 1) || WorldGen.SolidTile(i + 1, j + 1))
                {
                    offsetY = 4;
                }
            }
        }

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            CalamityUtils.DrawFlameEffect(ModContent.GetTexture("CalamityMod/Tiles/FurnitureAbyss/AbyssTorchFlame"), i, j);
        }
        public override void DrawEffects(int i, int j, SpriteBatch spriteBatch, ref Color drawColor, ref int nextSpecialDrawIndex)
        {
            CalamityUtils.DrawFlameSparks(187, 5, i, j);
        }

        public override bool NewRightClick(int i, int j)
        {
            CalamityUtils.RightClickBreak(i, j);
            return true;
        }
    }
}
