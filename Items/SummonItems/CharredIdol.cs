﻿using CalamityMod.Events;
using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
using CalamityMod.NPCs.BrimstoneElemental;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Items.SummonItems
{
    public class CharredIdol : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
            // DisplayName.SetDefault("Charred Idol");
            /* Tooltip.SetDefault("Use at your own risk\n" +
               "Summons the Brimstone Elemental when used in the Brimstone Crags\n" +
               "Enrages outside the Brimstone Crags\n" +
               "Not consumable"); */
			ItemID.Sets.SortingPriorityBossSpawns[Type] = 9; // Mechanical Skull
        }

        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 18;
            Item.rare = ItemRarityID.Pink;
            Item.useAnimation = 10;
            Item.useTime = 10;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.consumable = false;
        }

		public override void ModifyResearchSorting(ref ContentSamples.CreativeHelper.ItemGroup itemGroup)
		{
			itemGroup = ContentSamples.CreativeHelper.ItemGroup.BossItem;
		}

        public override bool CanUseItem(Player player)
        {
            CalamityPlayer modPlayer = player.Calamity();
            return modPlayer.ZoneCalamity && !NPC.AnyNPCs(ModContent.NPCType<BrimstoneElemental>()) && !BossRushEvent.BossRushActive;
        }

        public override bool? UseItem(Player player)
        {
            SoundEngine.PlaySound(SoundID.Roar, player.Center);
            if (Main.netMode != NetmodeID.MultiplayerClient)
                NPC.SpawnOnPlayer(player.whoAmI, ModContent.NPCType<BrimstoneElemental>());
            else
                NetMessage.SendData(MessageID.SpawnBossUseLicenseStartEvent, -1, -1, null, player.whoAmI, ModContent.NPCType<BrimstoneElemental>());

            return true;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.SoulofNight, 5).
                AddIngredient<EssenceofHavoc>(7).
                AddIngredient<UnholyCore>(2).
                AddTile(TileID.Hellforge).
                Register();
        }
    }
}
