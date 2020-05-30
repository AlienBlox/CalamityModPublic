using CalamityMod.Buffs.Alcohol;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Potions.Alcohol
{
    public class BloodyMary : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Bloody Mary");
            Tooltip.SetDefault(@"Extra spicy and bloody!
Boosts damage, movement speed, and melee speed by 15% and crit chance by 7% during a Blood Moon
Reduces life regen by 2 and defense by 6");
        }

        public override void SetDefaults()
        {
            item.width = 28;
            item.height = 18;
            item.useTurn = true;
            item.maxStack = 30;
            item.rare = 3;
            item.useAnimation = 17;
            item.useTime = 17;
            item.useStyle = ItemUseStyleID.EatingUsing;
            item.UseSound = SoundID.Item3;
            item.consumable = true;
            item.buffType = ModContent.BuffType<BloodyMaryBuff>();
            item.buffTime = 18000; //5 minutes
            item.value = Item.buyPrice(0, 10, 0, 0);
        }
    }
}
