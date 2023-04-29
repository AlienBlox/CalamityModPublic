﻿using CalamityMod.CalPlayer;
using CalamityMod.Cooldowns;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables;
using CalamityMod.Rarities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Armor.OmegaBlue
{
    [AutoloadEquip(EquipType.Head)]
    public class OmegaBlueHelmet : ModItem
    {

        public static readonly SoundStyle ActivationSound = new("CalamityMod/Sounds/Custom/AbilitySounds/OmegaBlueAbility");

        public override void Load()
        {
            if (Main.netMode != NetmodeID.Server)
            {
                EquipLoader.AddEquipTexture(Mod, "CalamityMod/Items/Armor/OmegaBlue/OmegaBlueHelmet_HeadMadness", EquipType.Head, name: "OmegaBlueTransformation");
            }
        }

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Omega Blue Helmet");
            // Tooltip.SetDefault(@"You can move freely through liquids
//12% increased damage and 8% increased critical strike chance");

            if (Main.netMode == NetmodeID.Server)
                return;
            var equipSlotHead = EquipLoader.GetEquipSlot(Mod, "OmegaBlueTransformation", EquipType.Head);
            ArmorIDs.Head.Sets.DrawHead[equipSlotHead] = false;
        }

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.value = CalamityGlobalItem.Rarity13BuyPrice;
            Item.defense = 19;
            Item.rare = ModContent.RarityType<PureGreen>();
        }

        public override void UpdateEquip(Player player)
        {
            player.ignoreWater = true;

            player.GetDamage<GenericDamageClass>() += 0.12f;
            player.GetCritChance<GenericDamageClass>() += 8;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<OmegaBlueChestplate>() && legs.type == ModContent.ItemType<OmegaBlueTentacles>();
        }

        public override void ArmorSetShadows(Player player)
        {
            player.armorEffectDrawOutlines = true;
            player.Calamity().omegaBlueTransformation = true;
            player.Calamity().omegaBlueTransformationForce = true;
        }

        public override void UpdateArmorSet(Player player)
        {
            var hotkey = CalamityKeybinds.SetBonusHotKey.TooltipHotkeyString();
            player.setBonus = "Increases armor penetration by 15\n" +
                "10% increased damage and critical strike chance and +2 max minions\n" +
                "Short-ranged tentacles heal you by sucking enemy life\n" +
                "Press " + hotkey + " to activate abyssal madness for 5 seconds\n" +
                "Abyssal madness increases damage, critical strike chance, and tentacle aggression/range\n" +
                "This effect has a 25 second cooldown";

            var mp = player.Calamity();
            player.GetArmorPenetration<GenericDamageClass>() += 15;
            player.maxMinions += 2;
            mp.wearingRogueArmor = true;
            mp.omegaBlueSet = true;
            mp.WearingPostMLSummonerSet = true;

            var hasOmegaBlueCooldown = mp.cooldowns.TryGetValue(Cooldowns.OmegaBlue.ID, out var cd);
            if (hasOmegaBlueCooldown && cd.timeLeft > 1500)
            {
                var d = Dust.NewDust(player.position, player.width, player.height, 20, 0, 0, 100, Color.Transparent, 1.6f);
                Main.dust[d].noGravity = true;
                Main.dust[d].noLight = true;
                Main.dust[d].fadeIn = 1f;
                Main.dust[d].velocity *= 3f;
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<ReaperTooth>(8).
                AddIngredient<DepthCells>(12).
                AddIngredient<RuinousSoul>(1).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}
