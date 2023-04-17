﻿using CalamityMod.Items.LoreItems;
using CalamityMod.Items.Pets;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Items.Weapons.Summon;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.TreasureBags.MiscGrabBags
{
    public class StarterBag : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 0;
        }

        public override void SetDefaults()
        {
            Item.consumable = true;
            Item.width = 24;
            Item.height = 24;
            Item.rare = ItemRarityID.Blue;
        }

        public override bool CanRightClick() => true;

        public override void ModifyItemLoot(ItemLoot itemLoot)
        {
            // Weapons
            // Tin and copper content is explicitly separated
            LeadingConditionRule tin = itemLoot.DefineConditionalDropSet(() => WorldGen.SavedOreTiers.Copper == TileID.Tin);
            tin.Add(ItemID.TinBroadsword);
            tin.Add(ItemID.TinBow);
            tin.Add(ItemID.TopazStaff);
            tin.OnFailedConditions(new CommonDrop(ItemID.CopperBroadsword, 1));
            tin.OnFailedConditions(new CommonDrop(ItemID.CopperBow, 1));
            tin.OnFailedConditions(new CommonDrop(ItemID.AmethystStaff, 1));
            itemLoot.Add(ItemID.WoodenArrow, 1, 100, 100); // You must specify 100 to 100.
            itemLoot.Add(ModContent.ItemType<SquirrelSquireStaff>());
            itemLoot.Add(ModContent.ItemType<ThrowingBrick>(), 1, 150, 150);

            // 1 Mana Crystal
            itemLoot.Add(ItemID.ManaCrystal);

            // Tools and Utility Items
            tin.Add(ItemID.TinHammer);
            tin.OnFailedConditions(new CommonDrop(ItemID.CopperHammer, 1));
            itemLoot.Add(ItemID.Bomb, 1, 10, 10);
            itemLoot.Add(ItemID.Rope, 1, 50, 50);

            // Potions
            itemLoot.Add(ItemID.MiningPotion);
            itemLoot.Add(ItemID.SpelunkerPotion, 1, 2, 2);
            itemLoot.Add(ItemID.SwiftnessPotion, 1, 3, 3);
            itemLoot.Add(ItemID.GillsPotion, 1, 2, 2);
            itemLoot.Add(ItemID.ShinePotion);
            itemLoot.Add(ItemID.RecallPotion, 1, 3, 3);

            // Tiles
            itemLoot.Add(ItemID.Torch, 1, 25, 25);
            itemLoot.Add(ItemID.Chest, 1, 3, 3);

            // Calamity title theme music box (if music mod is enabled)
            Mod musicMod = CalamityMod.Instance.musicMod;
            if (musicMod is not null)
                itemLoot.Add(musicMod.Find<ModItem>("CalamityMusicbox").Type);

            // Awakening lore item
            itemLoot.Add(ModContent.ItemType<LoreAwakening>());

            // Aleksh donator item
            // Name specific: "Aleksh" or "Shark Lad"
            static bool getsLadPet(DropAttemptInfo info)
            {
                string playerName = info.player.name;
                return playerName == "Aleksh" || playerName == "Shark Lad";
            };
            itemLoot.AddIf(getsLadPet, ModContent.ItemType<JoyfulHeart>());
        }
    }
}
