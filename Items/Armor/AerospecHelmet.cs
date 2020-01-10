﻿using CalamityMod.Buffs.Summon;
using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Summon;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Armor
{
    [AutoloadEquip(EquipType.Head)]
    public class AerospecHelmet : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Aerospec Helmet");
            Tooltip.SetDefault("5% increased movement speed and +1 max minion");
        }

        public override void SetDefaults()
        {
            item.width = 18;
            item.height = 18;
            item.value = Item.buyPrice(0, 5, 0, 0);
            item.rare = 3;
            item.defense = 2; //13
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<AerospecBreastplate>() && legs.type == ModContent.ItemType<AerospecLeggings>();
        }

        public override void ArmorSetShadows(Player player)
        {
            player.armorEffectDrawShadow = true;
        }

        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = "16% increased minion damage\n" +
                "Summons a valkyrie to protect you\n" +
                "Taking over 25 damage in one hit will cause a spread of homing feathers to fall\n" +
                "Allows you to fall more quickly and disables fall damage";
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.valkyrie = true;
            modPlayer.aeroSet = true;
            player.noFallDmg = true;
            if (player.whoAmI == Main.myPlayer)
            {
                if (player.FindBuffIndex(ModContent.BuffType<AerospecSummonSetBuff>()) == -1)
                {
                    player.AddBuff(ModContent.BuffType<AerospecSummonSetBuff>(), 3600, true);
                }
                if (player.ownedProjectileCounts[ModContent.ProjectileType<Valkyrie>()] < 1)
                {
                    Projectile.NewProjectile(player.Center.X, player.Center.Y, 0f, -1f, ModContent.ProjectileType<Valkyrie>(), (int)(25f * (player.allDamage + player.minionDamage - 1f)), 0f, Main.myPlayer, 0f, 0f);
                }
            }
            player.minionDamage += 0.16f;
        }

        public override void UpdateEquip(Player player)
        {
            player.moveSpeed += 0.05f;
            player.maxMinions++;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<AerialiteBar>(), 5);
            recipe.AddIngredient(ItemID.Cloud, 3);
            recipe.AddIngredient(ItemID.RainCloud);
            recipe.AddIngredient(ItemID.Feather);
            recipe.AddTile(TileID.SkyMill);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
