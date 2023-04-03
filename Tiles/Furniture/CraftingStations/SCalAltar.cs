﻿using CalamityMod.Events;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Furniture.CraftingStations;
using CalamityMod.Items.SummonItems;
using CalamityMod.NPCs.SupremeCalamitas;
using CalamityMod.Projectiles.Boss;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Terraria.Audio;

namespace CalamityMod.Tiles.Furniture.CraftingStations
{
    public class SCalAltar : ModTile
    {
        public static readonly SoundStyle SummonSound = new("CalamityMod/Sounds/Custom/SCalAltarSummon");

        public const int Width = 4;
        public const int Height = 3;
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileLavaDeath[Type] = false;

            // Various data sets to protect this tile from unintentional death
            TileID.Sets.PreventsTileRemovalIfOnTopOfIt[Type] = true;
            //TileID.Sets.PreventsTileReplaceIfOnTopOfIt[Type] = true; Since this is a furniture item this may be unnecessary?
            TileID.Sets.PreventsSandfall[Type] = true;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
            TileObjectData.newTile.Width = 4;
            TileObjectData.newTile.Height = 3;
            TileObjectData.newTile.Origin = new Point16(1, 2);
            TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop | AnchorType.SolidSide, TileObjectData.newTile.Width, 0);
            TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16, 16 };
            TileObjectData.newTile.DrawYOffset = 2;
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.LavaDeath = false;
            TileObjectData.addTile(Type);
            ModTranslation name = CreateMapEntryName();
            name.SetDefault("Altar");
            AddMapEntry(new Color(43, 19, 42), name);
            TileID.Sets.DisableSmartCursor[Type] = true;
        }

        public override bool CanExplode(int i, int j) => false;

        public override bool CreateDust(int i, int j, ref int type)
        {
            // Red torch dust.
            type = 60;
            return true;
        }

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            Item.NewItem(new EntitySource_TileBreak(i, j), i * 16, j * 16, Width * 16, Height * 16, ModContent.ItemType<AltarOfTheAccursedItem>());
        }

        public override bool RightClick(int i, int j) => AttemptToSummonSCal(i, j);
        public override void MouseOver(int i, int j) => HoverItemIcon();
        public override void MouseOverFar(int i, int j) => HoverItemIcon();

        public static void HoverItemIcon()
        {
            if (Main.LocalPlayer.HasItem(ModContent.ItemType<CeremonialUrn>()))
                Main.LocalPlayer.cursorItemIconID = ModContent.ItemType<CeremonialUrn>();
            else
                Main.LocalPlayer.cursorItemIconID = ModContent.ItemType<AshesofCalamity>();

            Main.LocalPlayer.noThrow = 2;
            Main.LocalPlayer.cursorItemIconEnabled = true;
        }

        public static bool AttemptToSummonSCal(int i, int j)
        {
            Tile tile = Main.tile[i, j];

            int left = i - tile.TileFrameX / 18;
            int top = j - tile.TileFrameY / 18;

            if (!Main.LocalPlayer.HasItem(ModContent.ItemType<AshesofCalamity>()) &&
                !Main.LocalPlayer.HasItem(ModContent.ItemType<CeremonialUrn>()))
            {
                return true;
            }

            if (NPC.AnyNPCs(ModContent.NPCType<SupremeCalamitas>()) || BossRushEvent.BossRushActive)
                return true;

            if (CalamityUtils.CountProjectiles(ModContent.ProjectileType<SCalRitualDrama>()) > 0)
                return true;

            bool usingSpecialItem = Main.LocalPlayer.HasItem(ModContent.ItemType<CeremonialUrn>());

            Vector2 ritualSpawnPosition = new Vector2(left + Width / 2, top).ToWorldCoordinates();
            ritualSpawnPosition += new Vector2(-10f, -24f);

            SoundEngine.PlaySound(SummonSound, ritualSpawnPosition);
            Projectile.NewProjectile(new EntitySource_WorldEvent(), ritualSpawnPosition, Vector2.Zero, ModContent.ProjectileType<SCalRitualDrama>(), 0, 0f, Main.myPlayer);

            if (!usingSpecialItem)
                Main.LocalPlayer.ConsumeItem(ModContent.ItemType<AshesofCalamity>(), true);
            return true;
        }
    }

    public class SCalAltarLarge : ModTile
    {
        public const int Width = 5;
        public const int Height = 3;
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileLavaDeath[Type] = false;

            // Various data sets to protect this tile from unintentional death
            TileID.Sets.PreventsTileRemovalIfOnTopOfIt[Type] = true;
            //TileID.Sets.PreventsTileReplaceIfOnTopOfIt[Type] = true; Since this is a furniture item this may be unnecessary?
            TileID.Sets.PreventsSandfall[Type] = true;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
            TileObjectData.newTile.Width = 5;
            TileObjectData.newTile.Height = 3;
            TileObjectData.newTile.Origin = new Point16(1, 2);
            TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop | AnchorType.SolidSide, TileObjectData.newTile.Width, 0);
            TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16, 16 };
            TileObjectData.newTile.DrawYOffset = 2;
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.LavaDeath = false;
            TileObjectData.addTile(Type);
            ModTranslation name = CreateMapEntryName();
            name.SetDefault("Altar");
            AddMapEntry(new Color(43, 19, 42), name);
            TileID.Sets.DisableSmartCursor[Type] = true;
        }

        public override bool CanExplode(int i, int j) => false;

        public override bool CreateDust(int i, int j, ref int type)
        {
            // Red torch dust.
            type = 60;
            return true;
        }

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            Item.NewItem(new EntitySource_TileBreak(i, j), i * 16, j * 16, Width * 16, Height * 16, ModContent.ItemType<AltarOfTheAccursedItem>());
        }

        public override bool RightClick(int i, int j) => SCalAltar.AttemptToSummonSCal(i, j);
        public override void MouseOver(int i, int j) => SCalAltar.HoverItemIcon();
        public override void MouseOverFar(int i, int j) => SCalAltar.HoverItemIcon();
    }
}
