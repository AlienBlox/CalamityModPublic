using CalamityMod.NPCs.Providence;
using CalamityMod.World;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Items.SummonItems
{
    public class ProfanedCore : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Profaned Core");
            Tooltip.SetDefault("The core of the unholy flame\n" +
                "Summons Providence\n" +
                "Should be used during daytime");
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.maxStack = 20;
            item.useAnimation = 45;
            item.useTime = 45;
            item.useStyle = ItemUseStyleID.HoldingUp;
            item.consumable = true;
            item.rare = 10;
            item.Calamity().customRarity = CalamityRarity.Turquoise;
        }

        public override bool CanUseItem(Player player)
        {
            return !NPC.AnyNPCs(ModContent.NPCType<Providence>()) && (player.ZoneHoly || player.ZoneUnderworldHeight) && CalamityWorld.downedBossAny;
        }

        public override bool UseItem(Player player)
        {
			if (Main.netMode != NetmodeID.MultiplayerClient)
			{
				NPC.NewNPC((int)(player.position.X + Main.rand.Next(-500, 501)), (int)(player.position.Y - 250f), ModContent.NPCType<Providence>(), 0, 0f, 0f, 0f, 0f, 255);
				Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/ProvidenceSpawn"), (int)player.position.X, (int)player.position.Y);
			}
			return true;
        }
    }
}
