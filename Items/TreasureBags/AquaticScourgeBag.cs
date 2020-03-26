using CalamityMod.World;
using CalamityMod.Items.Accessories;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.NPCs.AquaticScourge;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items.Armor.Vanity;

namespace CalamityMod.Items.TreasureBags
{
    public class AquaticScourgeBag : ModItem
    {
        public override int BossBagNPC => ModContent.NPCType<AquaticScourgeHead>();

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

        public override void PostUpdate() => CalamityUtils.ForceItemIntoWorld(item);

        public override void OpenBossBag(Player player)
        {
            // AS is available PHM, so this check is necessary to keep vanilla consistency
            if (Main.hardMode)
                player.TryGettingDevArmor();

            // Materials
            DropHelper.DropItem(player, ItemID.SoulofSight, 25, 40);
            DropHelper.DropItem(player, ModContent.ItemType<VictoryShard>(), 15, 25);
            DropHelper.DropItem(player, ItemID.Coral, 7, 11);
            DropHelper.DropItem(player, ItemID.Seashell, 7, 11);
            DropHelper.DropItem(player, ItemID.Starfish, 7, 11);

            // Weapons
            DropHelper.DropItemChance(player, ModContent.ItemType<SubmarineShocker>(), 3);
            DropHelper.DropItemChance(player, ModContent.ItemType<Barinautical>(), 3);
            DropHelper.DropItemChance(player, ModContent.ItemType<Downpour>(), 3);
            DropHelper.DropItemChance(player, ModContent.ItemType<DeepseaStaff>(), 3);
            DropHelper.DropItemChance(player, ModContent.ItemType<ScourgeoftheSeas>(), 3);
            float searingChance = DropHelper.LegendaryDropRateFloat;
            DropHelper.DropItemCondition(player, ModContent.ItemType<SeasSearing>(), CalamityWorld.revenge, searingChance);

            // Equipment
            DropHelper.DropItem(player, ModContent.ItemType<AquaticEmblem>());
            DropHelper.DropItemChance(player, ModContent.ItemType<AeroStone>(), 8);
            DropHelper.DropItemCondition(player, ModContent.ItemType<CorrosiveSpine>(), CalamityWorld.revenge, 0.25f);

            // Vanity
            DropHelper.DropItemChance(player, ModContent.ItemType<AquaticScourgeMask>(), 7);

            // Fishing
            DropHelper.DropItemChance(player, ItemID.HighTestFishingLine, 10);
            DropHelper.DropItemChance(player, ItemID.AnglerTackleBag, 10);
            DropHelper.DropItemChance(player, ItemID.TackleBox, 10);
            DropHelper.DropItemChance(player, ItemID.AnglerEarring, 8);
            DropHelper.DropItemChance(player, ItemID.FishermansGuide, 8);
            DropHelper.DropItemChance(player, ItemID.WeatherRadio, 8);
            DropHelper.DropItemChance(player, ItemID.Sextant, 8);
            DropHelper.DropItemChance(player, ItemID.AnglerHat, 3);
            DropHelper.DropItemChance(player, ItemID.AnglerVest, 3);
            DropHelper.DropItemChance(player, ItemID.AnglerPants, 3);
            DropHelper.DropItemChance(player, ItemID.FishingPotion, 3, 2, 3);
            DropHelper.DropItemChance(player, ItemID.SonarPotion, 3, 2, 3);
            DropHelper.DropItemChance(player, ItemID.CratePotion, 3, 2, 3);
            DropHelper.DropItemChance(player, ItemID.GoldenBugNet, 12);
        }
    }
}
