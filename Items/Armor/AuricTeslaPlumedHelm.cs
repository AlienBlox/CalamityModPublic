﻿using CalamityMod.CalPlayer;
using CalamityMod.Items.Accessories;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Ores;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Items.Armor
{
    [AutoloadEquip(EquipType.Head)]
    public class AuricTeslaPlumedHelm : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Auric Tesla Plumed Helm");
            Tooltip.SetDefault("20% increased rogue damage and critical strike chance, 25% increased movement speed\n" +
                               "Not moving boosts all damage and critical strike chance");
        }

        public override void SetDefaults()
        {
            item.width = 18;
            item.height = 18;
            item.value = Item.buyPrice(1, 80, 0, 0);
            item.defense = 34; //132
            item.Calamity().postMoonLordRarity = 20;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<AuricTeslaBodyArmor>() && legs.type == ModContent.ItemType<AuricTeslaCuisses>();
        }

        public override void ArmorSetShadows(Player player)
        {
            player.armorEffectDrawShadow = true;
        }

        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = "Rogue Tarragon, Bloodflare, God Slayer, and Silva armor effects\n" +
                "All projectiles spawn healing auric orbs on enemy hits\n" +
                "Max run speed and acceleration boosted by 10%\n" +
                "Rogue weapon critical strikes will do 1.25 times damage while you are above 50% HP\n" +
                "Rogue stealth builds while not attacking and not moving, up to a max of 160\n" +
                "Rogue stealth only reduces when you attack, it does not reduce while moving\n" +
                "The higher your rogue stealth the higher your rogue damage, crit, and movement speed";
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.tarraSet = true;
            modPlayer.tarraThrowing = true;
            modPlayer.bloodflareSet = true;
            modPlayer.bloodflareThrowing = true;
            modPlayer.godSlayer = true;
            modPlayer.godSlayerThrowing = true;
            modPlayer.silvaSet = true;
            modPlayer.silvaThrowing = true;
            modPlayer.auricSet = true;
            modPlayer.rogueStealthMax = 1.6f;
            modPlayer.wearingRogueArmor = true;
            player.thorns += 3f;
            player.lavaMax += 240;
            player.ignoreWater = true;
            player.crimsonRegen = true;
            if (player.lavaWet)
            {
                player.statDefense += 30;
                player.lifeRegen += 10;
            }
        }

        public override void UpdateEquip(Player player)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.auricBoost = true;
            player.Calamity().throwingDamage += 0.2f;
            player.Calamity().throwingCrit += 20;
            player.moveSpeed += 0.25f;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<SilvaMask>());
            recipe.AddIngredient(ModContent.ItemType<GodSlayerMask>());
            recipe.AddIngredient(ModContent.ItemType<BloodflareHelm>());
            recipe.AddIngredient(ModContent.ItemType<TarragonHelmet>());
            recipe.AddIngredient(ModContent.ItemType<AuricOre>(), 60);
            recipe.AddIngredient(ModContent.ItemType<EndothermicEnergy>(), 10);
            recipe.AddIngredient(ModContent.ItemType<NightmareFuel>(), 10);
            recipe.AddIngredient(ModContent.ItemType<Phantoplasm>(), 8);
            recipe.AddIngredient(ModContent.ItemType<DarksunFragment>(), 6);
            recipe.AddIngredient(ModContent.ItemType<BarofLife>(), 5);
            recipe.AddIngredient(ModContent.ItemType<HellcasterFragment>(), 5);
            recipe.AddIngredient(ModContent.ItemType<CoreofCalamity>(), 2);
            recipe.AddIngredient(ModContent.ItemType<GalacticaSingularity>());
            recipe.AddIngredient(ModContent.ItemType<PsychoticAmulet>());
            recipe.AddTile(ModContent.TileType<DraedonsForge>());
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
