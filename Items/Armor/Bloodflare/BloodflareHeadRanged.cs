﻿using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
using CalamityMod.Rarities;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Armor.Bloodflare
{
    [AutoloadEquip(EquipType.Head)]
    [LegacyName("BloodflareHornedHelm")]
    public class BloodflareHeadRanged : ModItem, ILocalizedModType
    {
        public string LocalizationCategory => "Items.Armor.PostMoonLord";
        public static readonly SoundStyle ActivationSound = new("CalamityMod/Sounds/Custom/AbilitySounds/BloodflareRangerActivation");

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.value = CalamityGlobalItem.Rarity13BuyPrice;
            Item.defense = 34; //85
            Item.rare = ModContent.RarityType<PureGreen>();
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<BloodflareBodyArmor>() && legs.type == ModContent.ItemType<BloodflareCuisses>();
        }

        public override void ArmorSetShadows(Player player)
        {
            player.armorEffectDrawShadowSubtle = true;
        }

        public override void UpdateArmorSet(Player player)
        {
            var modPlayer = player.Calamity();
            modPlayer.bloodflareSet = true;
            modPlayer.bloodflareRanged = true;
            var hotkey = CalamityKeybinds.SetBonusHotKey.TooltipHotkeyString();
            player.setBonus = "Greatly increases life regen\n" +
                "Enemies below 50% life drop a heart when struck\n" +
                "This effect has a 5 second cooldown\n" +
                "Press " + hotkey + " to unleash the lost souls of polterghast to destroy your enemies\n" +
                "This effect has a 30 second cooldown\n" +
                "Ranged weapons fire bloodsplosion orbs every 2.5 seconds";
            player.crimsonRegen = true;
        }

        public override void UpdateEquip(Player player)
        {
            player.GetDamage<RangedDamageClass>() += 0.1f;
            player.GetCritChance<RangedDamageClass>() += 10;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<BloodstoneCore>(11).
                AddIngredient<RuinousSoul>(2).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}
