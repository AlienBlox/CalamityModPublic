using CalamityMod.Projectiles.Magic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Magic
{
    public class YharimsCrystal : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Yharim's Crystal");
            Tooltip.SetDefault("Fires draconic beams of total annihilation");
        }

        public override void SetDefaults()
        {
            item.damage = 220;
            item.magic = true;
            item.mana = 15;
            item.width = 16;
            item.height = 16;
            item.useTime = 10;
            item.useAnimation = 10;
            item.reuseDelay = 5;
            item.useStyle = 5;
            item.UseSound = SoundID.Item13;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.channel = true;
            item.knockBack = 0f;
            item.value = Item.buyPrice(platinum: 1, gold: 80);
            item.rare = 10;
            item.shoot = ModContent.ProjectileType<YharimsCrystalPrism>();
            item.shootSpeed = 30f;
            item.Calamity().customRarity = CalamityRarity.ItemSpecific;
        }

        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[ModContent.ProjectileType<YharimsCrystalPrism>()] == 0;
    }
}
