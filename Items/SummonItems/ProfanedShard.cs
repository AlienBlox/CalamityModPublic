﻿using System.Collections.Generic;
using System.Linq;
using CalamityMod.Events;
using CalamityMod.Items.Materials;
using CalamityMod.NPCs.ProfanedGuardians;
using CalamityMod.World;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.SummonItems
{
    public class ProfanedShard : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Profaned Shard");
            /* Tooltip.SetDefault("A shard of the unholy flame\n" +
                "Summons the Profaned Guardians when used in the Hallow or Underworld during daytime\n" +
                "Enrage when not in the Hallow or Underworld\n" +
                "Not consumable"); */
			ItemID.Sets.SortingPriorityBossSpawns[Type] = 17; // Celestial Sigil
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.useAnimation = 10;
            Item.useTime = 10;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.consumable = false;
            Item.rare = ItemRarityID.Purple;
        }

		public override void ModifyResearchSorting(ref ContentSamples.CreativeHelper.ItemGroup itemGroup)
		{
			itemGroup = ContentSamples.CreativeHelper.ItemGroup.BossItem;
		}

        public override bool CanUseItem(Player player)
        {
            return !NPC.AnyNPCs(ModContent.NPCType<ProfanedGuardianCommander>()) && (Main.dayTime || CalamityWorld.getFixedBoi) && (player.ZoneHallow || player.ZoneUnderworldHeight) && !BossRushEvent.BossRushActive;
        }

        public override bool? UseItem(Player player)
        {
            SoundEngine.PlaySound(SoundID.Roar, player.Center);
            if (Main.netMode != NetmodeID.MultiplayerClient)
                NPC.SpawnOnPlayer(player.whoAmI, ModContent.NPCType<ProfanedGuardianCommander>());
            else
                NetMessage.SendData(MessageID.SpawnBossUseLicenseStartEvent, -1, -1, null, player.whoAmI, ModContent.NPCType<ProfanedGuardianCommander>());

            return true;
        }
        public override void ModifyTooltips(List<TooltipLine> list)
        {
            Player player = Main.LocalPlayer;
            TooltipLine line1 = list.FirstOrDefault(x => x.Mod == "Terraria" && x.Name == "Tooltip1");

            if (CalamityWorld.getFixedBoi)
            {
                line1.Text = "Summons the Profaned Guardians when used in the Hallow or Underworld";
            }
            else
            {
                line1.Text = "Summons the Profaned Guardians when used in the Hallow or Underworld during daytime";
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.LunarBar, 5).
                AddIngredient<UnholyEssence>(25).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}
