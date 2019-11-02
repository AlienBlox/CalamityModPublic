using CalamityMod.Items.Accessories;
using CalamityMod.Items.Armor.Vanity;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.NPCs.Leviathan;
using CalamityMod.World;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.TreasureBags
{
    public class LeviathanBag : ModItem
    {
        public override int BossBagNPC => ModContent.NPCType<Siren>();

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Treasure Bag");
            Tooltip.SetDefault("{$CommonItemTooltip.RightClickToOpen}");
        }

        public override void SetDefaults()
        {
            item.maxStack = 999;
            item.consumable = true;
            item.width = 24;
            item.height = 24;
            item.rare = 9;
            item.expert = true;
        }

        public override bool CanRightClick()
        {
            return true;
        }

        public override void OpenBossBag(Player player)
        {
            // siren & levi are available PHM, so this check is necessary to keep vanilla consistency
            if (Main.hardMode)
                player.TryGettingDevArmor();

            // Weapons
            DropHelper.DropItemChance(player, ModContent.ItemType<Greentide>(), 3);
            DropHelper.DropItemChance(player, ModContent.ItemType<Leviatitan>(), 3);
            DropHelper.DropItemChance(player, ModContent.ItemType<SirensSong>(), 3);
            DropHelper.DropItemChance(player, ModContent.ItemType<Atlantis>(), 3);
            DropHelper.DropItemChance(player, ModContent.ItemType<BrackishFlask>(), 3);

            // Equipment
            DropHelper.DropItem(player, ModContent.ItemType<LeviathanAmbergris>());
            DropHelper.DropItemChance(player, ModContent.ItemType<LureofEnthrallment>(), 3);
            float communityChance = CalamityWorld.defiled ? DropHelper.DefiledDropRateFloat : DropHelper.LegendaryDropRateFloat;
            DropHelper.DropItemCondition(player, ModContent.ItemType<TheCommunity>(), CalamityWorld.revenge, communityChance);

            // Vanity
            DropHelper.DropItemChance(player, ModContent.ItemType<LeviathanMask>(), 7);

            // Fishing
            DropHelper.DropItemChance(player, ItemID.HotlineFishingHook, 10);
            DropHelper.DropItemChance(player, ItemID.BottomlessBucket, 10);
            DropHelper.DropItemChance(player, ItemID.SuperAbsorbantSponge, 10);
            DropHelper.DropItemChance(player, ItemID.FishingPotion, 5, 5, 8);
            DropHelper.DropItemChance(player, ItemID.SonarPotion, 5, 5, 8);
            DropHelper.DropItemChance(player, ItemID.CratePotion, 5, 5, 8);
        }
    }
}
