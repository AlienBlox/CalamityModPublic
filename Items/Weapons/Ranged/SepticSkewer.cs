﻿using CalamityMod.Projectiles.Ranged;
using CalamityMod.Rarities;
using Microsoft.Xna.Framework;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class SepticSkewer : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Septic Skewer");
            /* Tooltip.SetDefault("Launches a spiky harpoon infested with toxins\n" +
                "Releases bacteria when returning to the player"); */
        }

        public override void SetDefaults()
        {
            Item.damage = 272;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 46;
            Item.height = 24;
            Item.useTime = 12;
            Item.useAnimation = 12;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 7.5f;
            Item.UseSound = SoundID.Item10;
            Item.autoReuse = true;
            Item.shootSpeed = 20f;
            Item.shoot = ModContent.ProjectileType<SepticSkewerHarpoon>();

            Item.value = CalamityGlobalItem.Rarity13BuyPrice;
            Item.rare = ModContent.RarityType<PureGreen>();
            Item.Calamity().canFirePointBlankShots = true;
        }

        public override Vector2? HoldoutOffset() => new Vector2(-10, 0);
    }
}
