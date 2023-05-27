﻿using CalamityMod.Events;
using CalamityMod.Items.Materials;
using CalamityMod.NPCs.Yharon;
using CalamityMod.Rarities;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.SummonItems
{
    [LegacyName("ChickenEgg", "JungleDragonEgg")]
    public class YharonEgg : ModItem
    {
        public override void SetStaticDefaults()
        {
                       ItemID.Sets.SortingPriorityBossSpawns[Type] = 17; // Celestial Sigil
        }

        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 18;
            Item.useAnimation = 10;
            Item.useTime = 10;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.consumable = false;
            Item.rare = ModContent.RarityType<DarkBlue>();
        }

        public override void ModifyResearchSorting(ref ContentSamples.CreativeHelper.ItemGroup itemGroup)
        {
            itemGroup = ContentSamples.CreativeHelper.ItemGroup.BossItem;
        }

        public override bool CanUseItem(Player player)
        {
            return !NPC.AnyNPCs(ModContent.NPCType<Yharon>()) && !BossRushEvent.BossRushActive;
        }

        public override bool? UseItem(Player player)
        {
            SoundEngine.PlaySound(Yharon.FireSound, player.Center);
            if (Main.netMode != NetmodeID.MultiplayerClient)
                NPC.SpawnOnPlayer(player.whoAmI, ModContent.NPCType<Yharon>());
            else
                NetMessage.SendData(MessageID.SpawnBossUseLicenseStartEvent, -1, -1, null, player.whoAmI, ModContent.NPCType<Yharon>());

            return true;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<EffulgentFeather>(15).
                AddIngredient<LifeAlloy>(10).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}
