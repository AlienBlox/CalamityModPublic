using CalamityMod.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Tiles
{
    public class AshenChest : ModTile
    {
        public override void SetDefaults()
        {
            CalamityUtils.SetUpChest(Type);
            ModTranslation name = CreateMapEntryName();
            name.SetDefault("Ashen Chest");
            AddMapEntry(new Color(191, 142, 111), name, MapChestName);
            name = CreateMapEntryName(Name + "_Locked");
            name.SetDefault("Locked Ashen Chest");
            AddMapEntry(new Color(174, 129, 92), name, MapChestName);
            disableSmartCursor = true;
            adjTiles = new int[] { TileID.Containers };
            chest = "Ashen Chest";
            chestDrop = ModContent.ItemType<Items.AshenChest>();
        }

        public override bool CreateDust(int i, int j, ref int type)
        {
            Dust.NewDust(new Vector2(i, j) * 16f, 16, 16, 60, 0f, 0f, 1, new Color(255, 255, 255), 1f);
            Dust.NewDust(new Vector2(i, j) * 16f, 16, 16, 1, 0f, 0f, 1, new Color(100, 100, 100), 1f);
            return false;
        }

        public override ushort GetMapOption(int i, int j) => (ushort)(Main.tile[i, j].frameX / 36);

        public override bool HasSmartInteract() => true;

        public override bool IsLockedChest(int i, int j) => Main.tile[i, j].frameX / 36 == 1;

        public override bool UnlockChest(int i, int j, ref short frameXAdjustment, ref int dustType, ref bool manual)
        {
            dustType = this.dustType;
            return true;
        }

        public string MapChestName(string name, int i, int j)
        {
            int left = i;
            int top = j;
            Tile tile = Main.tile[i, j];
            if (tile.frameX % 36 != 0)
            {
                left--;
            }
            if (tile.frameY != 0)
            {
                top--;
            }
            int chest = Chest.FindChest(left, top);
            if (Main.chest[chest].name == "")
            {
                return name;
            }
            else
            {
                return name + ": " + Main.chest[chest].name;
            }
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = 1;
        }

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            Item.NewItem(i * 16, j * 16, 32, 32, chestDrop);
            Chest.DestroyChest(i, j);
        }

        public override bool NewRightClick(int i, int j)
        {
            Tile tile = Main.tile[i, j];

            int left = i;
            int top = j;

            if (tile.frameX % 36 != 0)
            {
                left--;
            }
            if (tile.frameY != 0)
            {
                top--;
            }
            return CalamityUtils.LockedChestRightClick(IsLockedChest(left, top), left, top, i, j);
        }

        public override void MouseOver(int i, int j)
        {
            CalamityUtils.LockedChestMouseOver("BrimstoneKey", "AshenChest", "Ashen Chest", i, j);
        }

        public override void MouseOverFar(int i, int j)
        {
            CalamityUtils.LockedChestMouseOverFar("BrimstoneKey", "AshenChest", "Ashen Chest", i, j);
        }
    }
}
