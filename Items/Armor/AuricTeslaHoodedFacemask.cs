﻿using CalamityMod.CalPlayer;
using Terraria;
using Terraria.ModLoader; using CalamityMod.Items.Materials;

namespace CalamityMod.Items.Armor
{
    [AutoloadEquip(EquipType.Head)]
    public class AuricTeslaHoodedFacemask : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Auric Tesla Hooded Facemask");
            Tooltip.SetDefault("30% increased ranged damage and critical strike chance\n" +
                               "Not moving boosts all damage and critical strike chance");
        }

        public override void SetDefaults()
        {
            item.width = 18;
            item.height = 18;
            item.value = Item.buyPrice(1, 80, 0, 0);
            item.defense = 40; //132
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
            player.setBonus = "Ranged Tarragon, Bloodflare, God Slayer, and Silva armor effects\n" +
                "All projectiles spawn healing auric orbs on enemy hits\n" +
                "Max run speed and acceleration boosted by 10%";
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.tarraSet = true;
            modPlayer.tarraRanged = true;
            modPlayer.bloodflareSet = true;
            modPlayer.bloodflareRanged = true;
            modPlayer.godSlayer = true;
            modPlayer.godSlayerRanged = true;
            modPlayer.silvaSet = true;
            modPlayer.silvaRanged = true;
            modPlayer.auricSet = true;
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
            player.rangedDamage += 0.3f;
            player.rangedCrit += 30;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<SilvaHornedHelm>());
            recipe.AddIngredient(ModContent.ItemType<GodSlayerHelmet>());
            recipe.AddIngredient(ModContent.ItemType<BloodflareHornedHelm>());
            recipe.AddIngredient(ModContent.ItemType<TarragonVisage>());
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
