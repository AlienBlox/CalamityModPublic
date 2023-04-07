﻿using CalamityMod.CustomRecipes;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Projectiles.DraedonsArsenal;
using CalamityMod.Rarities;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.DraedonsArsenal
{
    public class TrackingDisk : RogueWeapon
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
            // DisplayName.SetDefault("Tracking Disk");
            /* Tooltip.SetDefault("A weapon that, as it flies, processes calculations and fires lasers\n" +
                               "Releases a flying disk that fires lasers at nearby enemies\n" +
                               "Stealth strikes allow the disk to fire multiple larger lasers at different targets"); */
        }
        public override void SetDefaults()
        {
            CalamityGlobalItem modItem = Item.Calamity();

            Item.damage = 16;
            Item.DamageType = RogueDamageClass.Instance;

            Item.width = 30;
            Item.height = 34;
            Item.useTime = 45;
            Item.useAnimation = 45;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTurn = false;
            Item.knockBack = 3f;

            Item.value = CalamityGlobalItem.Rarity3BuyPrice;
            Item.rare = ModContent.RarityType<DarkOrange>();

            Item.noUseGraphic = true;

            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;

            Item.shoot = ModContent.ProjectileType<TrackingDiskProjectile>();
            Item.shootSpeed = 10f;

            modItem.UsesCharge = true;
            modItem.MaxCharge = 50f;
            modItem.ChargePerUse = 0.08f;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int proj = Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
            if (proj.WithinBounds(Main.maxProjectiles))
                Main.projectile[proj].Calamity().stealthStrike = player.Calamity().StealthStrikeAvailable();
            return false;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips) => CalamityGlobalItem.InsertKnowledgeTooltip(tooltips, 1);

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<MysteriousCircuitry>(5).
                AddIngredient<DubiousPlating>(7).
                AddIngredient<AerialiteBar>(4).
                AddIngredient<SeaPrism>(7).
                AddCondition(ArsenalTierGatedRecipe.ConstructRecipeCondition(1, out Predicate<Recipe> condition), condition).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}
