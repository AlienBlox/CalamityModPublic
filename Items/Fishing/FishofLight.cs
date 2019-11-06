using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Fishing
{
    public class FishofLight : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Fish of Light");
            Tooltip.SetDefault("Right click to extract souls");
        }

        public override void SetDefaults()
        {
            item.maxStack = 999;
            item.consumable = true;
            item.width = 34;
            item.height = 34;
            item.rare = 3;
            item.value = Item.sellPrice(gold: 1);
        }

        public override bool CanRightClick()
        {
            return true;
        }

        public override void RightClick(Player player)
        {
            DropHelper.DropItem(player, ItemID.SoulofLight, 2, 5);
        }
    }
}
