using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.CalPlayer;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Items.Fishing
{
    public class UrsaSergeant : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ursa Sergeant");
            Tooltip.SetDefault("+20 defense but 35% reduced movement speed\n" +
                "Immune to Astral Infection and Feral Bite\n" +
                "Increased regeneration at lower health");
        }

        public override void SetDefaults()
        {
            item.width = 36;
            item.height = 26;
            item.value = Item.buyPrice(0, 24, 0, 0);
            item.rare = 8;
            item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.ursaSergeant = true;
            player.statDefense += 20;
            player.buffImmune[ModContent.BuffType<AstralInfectionDebuff>()] = true;
            player.buffImmune[148] = true; //Feral Bite
        }
    }
}
