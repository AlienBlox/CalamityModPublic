﻿using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Armor
{
    [AutoloadEquip(EquipType.Head)]
    public class AtaxiaHood : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ataxia Hood");
            Tooltip.SetDefault("12% increased rogue damage and 10% increased rogue critical strike chance\n" +
                "50% chance to not consume rogue items and 15% increased movement speed\n" +
                "Temporary immunity to lava and immunity to fire damage");
        }

        public override void SetDefaults()
        {
            item.width = 18;
            item.height = 18;
            item.value = Item.buyPrice(0, 30, 0, 0);
            item.rare = 8;
            item.defense = 12; //49
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<AtaxiaArmor>() && legs.type == ModContent.ItemType<AtaxiaSubligar>();
        }

        public override void ArmorSetShadows(Player player)
        {
            player.armorEffectDrawOutlines = true;
        }

        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = "5% increased rogue damage\n" +
                "Inferno effect when below 50% life\n" +
                "Rogue weapons have a 10% chance to unleash a volley of chaos flames around the player that chase enemies when used\n" +
                "You have a 20% chance to emit a blazing explosion when you are hit\n" +
                "Rogue stealth builds while not attacking and not moving, up to a max of 110\n" +
                "Once you have built max stealth, you will be able to perform a Stealth Strike\n" +
                "Rogue stealth only reduces when you attack, it does not reduce while moving\n" +
                "The higher your rogue stealth the higher your rogue damage, crit, and movement speed";
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.ataxiaBlaze = true;
            modPlayer.ataxiaVolley = true;
            modPlayer.rogueStealthMax += 1.1f;
            player.Calamity().throwingDamage += 0.05f;
            player.Calamity().wearingRogueArmor = true;
        }

        public override void UpdateEquip(Player player)
        {
            player.Calamity().throwingAmmoCost50 = true;
            player.Calamity().throwingDamage += 0.12f;
            player.Calamity().throwingCrit += 10;
            player.moveSpeed += 0.15f;
            player.lavaMax += 240;
            player.buffImmune[BuffID.OnFire] = true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<CruptixBar>(), 7);
			recipe.AddIngredient(ItemID.HellstoneBar, 4);
			recipe.AddIngredient(ModContent.ItemType<CoreofChaos>());
			recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
