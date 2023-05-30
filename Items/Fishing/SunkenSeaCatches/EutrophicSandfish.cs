﻿using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Fishing.SunkenSeaCatches
{
    public class EutrophicSandfish : ModItem, ILocalizedModType
    {
        public string LocalizationCategory => "Items.Fishing";
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 2;
            ItemID.Sets.CanBePlacedOnWeaponRacks[Item.type] = true;
        }

        public override void SetDefaults()
        {
            Item.questItem = true;
            Item.maxStack = 1;
            Item.width = 40;
            Item.height = 28;
            Item.uniqueStack = true;
            Item.rare = ItemRarityID.Quest;
        }

        public override bool IsQuestFish()
        {
            return true;
        }

        public override bool IsAnglerQuestAvailable()
        {
            return DownedBossSystem.downedDesertScourge;
        }

        public override void AnglerQuestChat(ref string description, ref string catchLocation)
        {
            description = "You ever get to see what would happen if a lizard that lived in the desert scurried too deep underground? I did, and they sure are cool! But it's way too slippery for me to get my hands on it now. You go and get it so I can keep it as a pet!";
            catchLocation = "Caught in the Sunken Sea.";
        }
    }
}
