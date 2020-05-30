using CalamityMod.NPCs;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class GrandDad : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Grand Dad");
            Tooltip.SetDefault("Lowers enemy defense to 0 when they are struck\n" +
                        "Yeets enemies across space and time\n" +
                        "7");
        }

        public override void SetDefaults()
        {
            item.width = 124;
            item.damage = 777;
            item.melee = true;
            item.useAnimation = 25;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.useTime = 25;
            item.useTurn = true;
            item.knockBack = 77f;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.height = 124;
            item.value = Item.buyPrice(1, 0, 0, 0);
            item.rare = 10;
            item.Calamity().customRarity = CalamityRarity.RareVariant;
        }

        public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit)
        {
            if (CalamityGlobalNPC.ShouldAffectNPC(target))
            {
                target.knockBackResist = 7f;
                target.defense = 0;
            }
        }
    }
}
