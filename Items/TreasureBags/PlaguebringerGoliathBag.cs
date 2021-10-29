using CalamityMod.Items.Accessories;
using CalamityMod.Items.Armor.Vanity;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Pets;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.NPCs.PlaguebringerGoliath;
using CalamityMod.World;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.TreasureBags
{
    public class PlaguebringerGoliathBag : ModItem
    {
        public override int BossBagNPC => ModContent.NPCType<PlaguebringerGoliath>();

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

        public override bool CanRightClick() => true;

        public override void OpenBossBag(Player player)
        {
            player.TryGettingDevArmor();

            // Materials
            DropHelper.DropItem(player, ModContent.ItemType<PlagueCellCluster>(), 13, 17);
            DropHelper.DropItem(player, ModContent.ItemType<InfectedArmorPlating>(), 16, 20);
            DropHelper.DropItem(player, ItemID.Stinger, 4, 8);

            // Weapons
            float w = DropHelper.BagWeaponDropRateFloat;
            DropHelper.DropEntireWeightedSet(player,
                DropHelper.WeightStack<VirulentKatana>(w), // Virulence
                DropHelper.WeightStack<DiseasedPike>(w),
                DropHelper.WeightStack<ThePlaguebringer>(w), // Pandemic
                DropHelper.WeightStack<Malevolence>(w),
                DropHelper.WeightStack<PestilentDefiler>(w),
                DropHelper.WeightStack<TheHive>(w),
                DropHelper.WeightStack<MepheticSprayer>(w), // Blight Spewer
                DropHelper.WeightStack<PlagueStaff>(w),
                DropHelper.WeightStack<FuelCellBundle>(w),
                DropHelper.WeightStack<InfectedRemote>(w),
                DropHelper.WeightStack<TheSyringe>(w)
            );

            // Equipment
            DropHelper.DropItem(player, ModContent.ItemType<ToxicHeart>());

            // Vanity
            DropHelper.DropItemChance(player, ModContent.ItemType<PlaguebringerGoliathMask>(), 7);
            DropHelper.DropItemChance(player, ModContent.ItemType<PlagueCaller>(), 10);
        }
    }
}
