using CalamityMod.Buffs.Pets;
using CalamityMod.Projectiles.Pets;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Pets
{
    public class ChromaticOrb : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Chromatic Orb");
            Tooltip.SetDefault("It glows warmly in your hand\n" +
                "Summons an ancient dragon light pet\n" +
                "Provides a small amount of light in the abyss");
        }

        public override void SetDefaults()
        {
            item.CloneDefaults(ItemID.WispinaBottle);
            item.shoot = ModContent.ProjectileType<BendyPet>();
            item.buffType = ModContent.BuffType<BendyBuff>();
            item.Calamity().customRarity = CalamityRarity.Dedicated;
        }

        public override void UseStyle(Player player)
        {
            if (player.whoAmI == Main.myPlayer && player.itemTime == 0)
            {
                player.AddBuff(item.buffType, 3600, true);
            }
        }
    }
}
