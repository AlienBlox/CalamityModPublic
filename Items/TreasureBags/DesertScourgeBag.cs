using CalamityMod.Items.Accessories;
using CalamityMod.Items.Armor.Vanity;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.NPCs.DesertScourge;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.TreasureBags
{
    public class DesertScourgeBag : ModItem
    {
        public override int BossBagNPC => ModContent.NPCType<DesertScourgeHead>();

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
            item.rare = ItemRarityID.Cyan;
            item.expert = true;
        }

        public override bool CanRightClick()
        {
            return true;
        }

        public override void OpenBossBag(Player player)
        {
            // Materials
            DropHelper.DropItem(player, ModContent.ItemType<VictoryShard>(), 10, 16);
            DropHelper.DropItem(player, ItemID.Coral, 7, 11);
            DropHelper.DropItem(player, ItemID.Seashell, 7, 11);
            DropHelper.DropItem(player, ItemID.Starfish, 7, 11);

            // Weapons
            // Set up the base drop set, which includes Scourge of the Desert at its normal drop chance.
            float w = DropHelper.BagWeaponDropRateFloat;
            DropHelper.WeightedItemStack[] weapons =
            {
                DropHelper.WeightStack<AquaticDischarge>(w),
                DropHelper.WeightStack<Barinade>(w),
                DropHelper.WeightStack<StormSpray>(w),
                DropHelper.WeightStack<SeaboundStaff>(w),
                DropHelper.WeightStack<ScourgeoftheDesert>(w),
            };
            DropHelper.DropEntireWeightedSet(player, weapons);

            // Equipment
            DropHelper.DropItem(player, ModContent.ItemType<OceanCrest>());
            DropHelper.DropItemChance(player, ModContent.ItemType<AeroStone>(), 5);
            DropHelper.DropItemChance(player, ModContent.ItemType<SandCloak>(), 9);

            // Vanity
            DropHelper.DropItemChance(player, ModContent.ItemType<DesertScourgeMask>(), 7);

            // Fishing
            DropHelper.DropItem(player, ModContent.ItemType<SandyAnglingKit>());
        }
    }
}
