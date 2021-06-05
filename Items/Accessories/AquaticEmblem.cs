using CalamityMod.CalPlayer;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    public class AquaticEmblem : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Aquatic Emblem");
            Tooltip.SetDefault("Most ocean enemies become friendly and provides waterbreathing\n" +
                "Being underwater slowly boosts your defense over time but also slows movement speed\n" +
                "The defense boost and movement speed reduction slowly vanish while outside of water\n" +
                "Maximum defense boost is 50, maximum movement speed reduction is 10%\n" +
                "Provides a small amount of light in the abyss\n" +
                "Moderately reduces breath loss in the abyss");
        }

        public override void SetDefaults()
        {
            item.width = 18;
            item.height = 26;
            item.value = CalamityGlobalItem.Rarity5BuyPrice;
            item.rare = ItemRarityID.Pink;
            item.accessory = true;
            item.expert = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.aquaticEmblem = true;
            player.npcTypeNoAggro[NPCID.Shark] = true;
            player.npcTypeNoAggro[NPCID.SeaSnail] = true;
            player.npcTypeNoAggro[NPCID.PinkJellyfish] = true;
            player.npcTypeNoAggro[NPCID.Crab] = true;
            player.npcTypeNoAggro[NPCID.Squid] = true;
            player.gills = true;
        }
    }
}
