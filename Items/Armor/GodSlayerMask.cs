﻿using CalamityMod.CalPlayer;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Items
{
    [AutoloadEquip(EquipType.Head)]
    public class GodSlayerMask : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("God Slayer Mask");
            Tooltip.SetDefault("14% increased rogue damage and critical strike chance, 18% increased movement speed");
        }

        public override void SetDefaults()
        {
            item.width = 18;
            item.height = 18;
            item.value = Item.buyPrice(0, 75, 0, 0);
            item.defense = 29; //96
            item.Calamity().postMoonLordRarity = 14;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<GodSlayerChestplate>() && legs.type == ModContent.ItemType<GodSlayerLeggings>();
        }

        public override void ArmorSetShadows(Player player)
        {
            player.armorEffectDrawShadow = true;
        }

        public override void UpdateArmorSet(Player player)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.godSlayer = true;
            modPlayer.godSlayerThrowing = true;
            modPlayer.rogueStealthMax = 1.4f;
            modPlayer.wearingRogueArmor = true;
            player.setBonus = "You will survive fatal damage and will be healed 150 HP if an attack would have killed you\n" +
                "This effect can only occur once every 45 seconds\n" +
                "While the cooldown for this effect is active you gain a 10% increase to all damage\n" +
                "While at full HP all of your rogue stats are boosted by 10%\n" +
                "If you take over 80 damage in one hit you will be given extra immunity frames\n" +
                "Rogue stealth builds while not attacking and not moving, up to a max of 140\n" +
                "Rogue stealth only reduces when you attack, it does not reduce while moving\n" +
                "The higher your rogue stealth the higher your rogue damage, crit, and movement speed";
        }

        public override void UpdateEquip(Player player)
        {
            player.Calamity().throwingDamage += 0.14f;
            player.Calamity().throwingCrit += 14;
            player.moveSpeed += 0.18f;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<CosmiliteBar>(), 14);
            recipe.AddIngredient(ModContent.ItemType<NightmareFuel>(), 8);
            recipe.AddIngredient(ModContent.ItemType<EndothermicEnergy>(), 8);
            recipe.AddTile(null, "DraedonsForge");
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
