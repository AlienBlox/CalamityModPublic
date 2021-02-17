using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.CalPlayer;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    public class HideofAstrumDeus : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Hide of Astrum Deus");
            Tooltip.SetDefault("Taking damage, or inflicting a true melee strike, drops an immense amount of astral stars from the sky\n" +
								"Taking damage boosts true melee damage by 50%\n" +
                                "Boost duration is based on the amount of damage you took, the higher the damage the longer the boost\n" +
                                "Provides immunity to the astral infection, cursed inferno, on fire, and frostburn debuffs\n" +
                                "Enemies take damage when they hit you and are inflicted with the astral infection debuff");
        }

        public override void SetDefaults()
        {
            item.width = 26;
            item.height = 26;
            item.value = CalamityGlobalItem.Rarity9BuyPrice;
            item.rare = ItemRarityID.Cyan;
            item.accessory = true;
            item.Calamity().customRarity = CalamityRarity.RareVariant;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.aBulwark = true;
            player.buffImmune[ModContent.BuffType<AstralInfectionDebuff>()] = true;
            modPlayer.aBulwarkRare = true;
            player.buffImmune[BuffID.CursedInferno] = true;
            player.buffImmune[BuffID.OnFire] = true;
            player.buffImmune[BuffID.Frostburn] = true;
            player.thorns += 0.75f;
        }
    }
}
