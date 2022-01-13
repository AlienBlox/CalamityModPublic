using CalamityMod.Items.Placeables.FurnitureAcidwood;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Tiles.FurnitureAcidwood
{
    public class AcidwoodClockTile : ModTile
    {
        public override void SetDefaults()
        {
            this.SetUpClock();
            ModTranslation name = CreateMapEntryName();
            name.SetDefault("Grandfather Clock");
            AddMapEntry(new Color(191, 142, 111), name);
            adjTiles = new int[] { TileID.GrandfatherClocks };
        }

        public override bool HasSmartInteract() => true;

        public override bool CreateDust(int i, int j, ref int type)
        {
            Dust.NewDust(new Vector2(i, j) * 16f, 16, 16, 7, 0f, 0f, 1, new Color(255, 255, 255), 1f);
            return false;
        }

        public override bool NewRightClick(int x, int y) => CalamityUtils.ClockRightClick();

        public override void NearbyEffects(int i, int j, bool closer)
        {
            if (closer)
            {
                Main.clock = true;
            }
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            Item.NewItem(i * 16, j * 16, 48, 32, ModContent.ItemType<AcidwoodClock>());
        }
    }
}
