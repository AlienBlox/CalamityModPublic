﻿using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Mounts
{
    [LegacyName("BirdSeed")]
    public class FollyFeed : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 34;
            Item.height = 36;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Swing;

            Item.value = Item.sellPrice(gold: 20);
            Item.rare = ItemRarityID.Purple;
            Item.Calamity().donorItem = true;

            Item.UseSound = SoundID.NPCHit51;
            Item.noMelee = true;
            Item.mountType = ModContent.MountType<BUMBLEDOGE>();
        }
    }
}
