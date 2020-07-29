using CalamityMod.Items.Placeables.DraedonStructures;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace CalamityMod.Tiles.DraedonStructures
{
    public class LaboratoryDoorClosed : ModTile
    {
        public override void SetDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileSolid[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileLavaDeath[Type] = true;
            Main.tileWaterDeath[Type] = false;
            TileID.Sets.NotReallySolid[Type] = true;
            TileID.Sets.DrawsWalls[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style1xX);
            TileObjectData.newTile.Height = 4;
            TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16, 16, 16 };
            TileObjectData.newTile.Origin = new Point16(0, 3);
            TileObjectData.newTile.AnchorTop = new AnchorData(AnchorType.SolidTile, TileObjectData.newTile.Width, 0);
            TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile, TileObjectData.newTile.Width, 0);
            TileObjectData.newTile.UsesCustomCanPlace = true;
            TileObjectData.addTile(Type);
            AddToArray(ref TileID.Sets.RoomNeeds.CountsAsDoor);

            ModTranslation name = CreateMapEntryName();
            name.SetDefault("Laboratory Door");
            AddMapEntry(Color.DarkSlateGray, name);
            disableSmartCursor = true;
            adjTiles = new int[] { TileID.ClosedDoor };
            dustType = 8;
        }

        public override bool HasSmartInteract() => true;

        public override bool NewRightClick(int i, int j)
        {
            ushort type = (ushort)ModContent.TileType<LaboratoryDoorOpen>();
            short frameY = 0;
            for (int dy = -4; dy < 4; dy++)
            {
                if (Main.tile[i, j + dy].type == ModContent.TileType<LaboratoryDoorClosed>())
                {
                    if (Main.tile[i, j + dy] == null)
                    {
                        Main.tile[i, j + dy] = new Tile();
                    }
                    Main.tile[i, j + dy].type = type;
                    Main.tile[i, j + dy].frameY = frameY;
                    frameY += 16;
                }
            }

            Main.PlaySound(SoundID.DoorOpen, i * 16, j * 16, 1, 1f, 0f);
            return true;
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            Item.NewItem(i * 16, j * 16, 16, 48, ModContent.ItemType<LaboratoryDoorItem>());
        }

        public override void MouseOver(int i, int j)
        {
            Player player = Main.LocalPlayer;
            player.noThrow = 2;
            player.showItemIcon = true;
            player.showItemIcon2 = ModContent.ItemType<LaboratoryDoorItem>();
        }
    }
}
