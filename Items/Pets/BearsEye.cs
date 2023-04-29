﻿using CalamityMod.Buffs.Pets;
using CalamityMod.Projectiles.Pets;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Pets
{
    [LegacyName("BearEye")]
    public class BearsEye : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Bear's Eye");
            // Tooltip.SetDefault("Summons a pet guardian angel");
        }
        public override void SetDefaults()
        {
            Item.damage = 0;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useAnimation = 20;
            Item.useTime = 20;
            Item.noMelee = true;
            Item.width = 30;
            Item.height = 30;

            Item.value = Item.sellPrice(platinum: 1);
            Item.rare = ItemRarityID.Pink;
            Item.Calamity().devItem = true;

            Item.shoot = ModContent.ProjectileType<Bear>();
            Item.buffType = ModContent.BuffType<BearBuff>();
            Item.UseSound = SoundID.Meowmere;
        }

        public override void UseStyle(Player player, Rectangle heldItemFrame)
        {
            if (player.whoAmI == Main.myPlayer && player.itemTime == 0)
            {
                player.AddBuff(Item.buffType, 15, true);
            }
        }
    }
}
