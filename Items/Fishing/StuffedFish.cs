using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Fishing
{
    public class StuffedFish : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Stuffed Fish");
            Tooltip.SetDefault("Right click to extract herbs and seeds");
        }

        public override void SetDefaults()
        {
            item.maxStack = 999;
            item.consumable = true;
            item.width = 34;
            item.height = 30;
            item.rare = 2;
            item.value = Item.sellPrice(silver: 50);
        }

        public override bool CanRightClick()
        {
            return true;
        }

        public override void RightClick(Player player)
        {
            int herbMin = 1;
            int herbMax = 3;
            int seedMin = 2;
            int seedMax = 5;
            DropHelper.DropItemChance(player, ItemID.Daybloom, 0.25f, herbMin, herbMax);
            DropHelper.DropItemChance(player, ItemID.Moonglow, 0.25f, herbMin, herbMax);
            DropHelper.DropItemChance(player, ItemID.Waterleaf, 0.25f, herbMin, herbMax);
            DropHelper.DropItemChance(player, ItemID.Deathweed, 0.25f, herbMin, herbMax);
            DropHelper.DropItemChance(player, ItemID.Shiverthorn, 0.25f, herbMin, herbMax);
            DropHelper.DropItemChance(player, ItemID.Fireblossom, 0.25f, herbMin, herbMax);
            DropHelper.DropItemChance(player, ItemID.Blinkroot, 0.25f, herbMin, herbMax);
            DropHelper.DropItemChance(player, ItemID.DaybloomSeeds, 0.20f, seedMin, seedMax);
            DropHelper.DropItemChance(player, ItemID.MoonglowSeeds, 0.20f, seedMin, seedMax);
            DropHelper.DropItemChance(player, ItemID.WaterleafSeeds, 0.20f, seedMin, seedMax);
            DropHelper.DropItemChance(player, ItemID.DeathweedSeeds, 0.20f, seedMin, seedMax);
            DropHelper.DropItemChance(player, ItemID.ShiverthornSeeds, 0.20f, seedMin, seedMax);
            DropHelper.DropItemChance(player, ItemID.FireblossomSeeds, 0.20f, seedMin, seedMax);
            DropHelper.DropItemChance(player, ItemID.BlinkrootSeeds, 0.20f, seedMin, seedMax);
            DropHelper.DropItemChance(player, ItemID.GrassSeeds, 0.1f, seedMin, seedMax);
            DropHelper.DropItemChance(player, ItemID.JungleGrassSeeds, 0.1f, seedMin, seedMax);
            DropHelper.DropItemChance(player, ItemID.MushroomGrassSeeds, 0.1f, seedMin, seedMax);
            DropHelper.DropItemChance(player, ItemID.PumpkinSeed, 0.05f, seedMin, seedMax);
            DropHelper.DropItemCondition(player, ItemID.CorruptSeeds, !WorldGen.crimson, 0.05f, seedMin, seedMax);
            DropHelper.DropItemCondition(player, ItemID.CrimsonSeeds, WorldGen.crimson, 0.05f, seedMin, seedMax);
            DropHelper.DropItemCondition(player, ItemID.HallowedSeeds, Main.hardMode, 0.05f, seedMin, seedMax);
            /*Mod thorium = ModLoader.GetMod("ThoriumMod");
            DropHelper.DropItemChance(player, thorium.ItemType("MarineKelp"), 0.25f, herbMin, herbMax);
            DropHelper.DropItemChance(player, thorium.ItemType("MarineKelpSeeds"), 0.1f, seedMin, seedMax);*/
        }
    }
}
