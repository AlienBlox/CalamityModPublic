using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Tiles.FurnitureSilva
{
    public class SilvaClock : ModTile
    {
        public override void SetDefaults()
        {
            CalamityUtils.SetUpClock(Type);
            ModTranslation name = CreateMapEntryName();
            name.SetDefault("Silva Clock");
            AddMapEntry(new Color(191, 142, 111), name);
            adjTiles = new int[] { TileID.GrandfatherClocks };
        }

        public override bool HasSmartInteract()
        {
            return true;
        }

        public override bool CreateDust(int i, int j, ref int type)
        {
            Dust.NewDust(new Vector2(i, j) * 16f, 16, 16, ModContent.DustType<SilvaTileGold>(), 0f, 0f, 1, new Color(255, 255, 255), 1f);
            Dust.NewDust(new Vector2(i, j) * 16f, 16, 16, 157, 0f, 0f, 1, new Color(255, 255, 255), 1f);
            return false;
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }

        public override bool NewRightClick(int x, int y)
        {
            return CalamityUtils.ClockRightClick();
        }

        public override void NearbyEffects(int i, int j, bool closer)
        {
            if (closer)
            {
                Main.clock = true;
            }
        }

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            Item.NewItem(i * 16, j * 16, 48, 32, ModContent.ItemType<Items.SilvaClock>());
        }
    }
}
