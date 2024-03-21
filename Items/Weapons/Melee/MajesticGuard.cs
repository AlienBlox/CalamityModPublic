﻿using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class MajesticGuard : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Melee";
        public override void SetDefaults()
        {
            Item.width = 100;
            Item.height = 100;
            Item.damage = 70;
            Item.DamageType = DamageClass.Melee;
            Item.useAnimation = 22;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = 22;
            Item.useTurn = true;
            Item.knockBack = 7.5f;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.value = CalamityGlobalItem.Rarity5BuyPrice;
            Item.rare = ItemRarityID.Pink;
        }

        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (target.Calamity().miscDefenseLoss < target.defense)
                target.Calamity().miscDefenseLoss += 1;

            if (target.Calamity().miscDefenseLoss >= target.defense)
            {
                if (player.moonLeech || player.lifeSteal <= 0f)
                    return;

                int heal = 3;
                player.lifeSteal -= heal;
                player.statLife += heal;
                player.HealEffect(heal);
                if (player.statLife > player.statLifeMax2)
                    player.statLife = player.statLifeMax2;
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.AdamantiteSword).
                AddIngredient(ItemID.SoulofMight, 15).
                AddTile(TileID.MythrilAnvil).
                Register();
            CreateRecipe().
                AddIngredient(ItemID.TitaniumSword).
                AddIngredient(ItemID.SoulofMight, 15).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
