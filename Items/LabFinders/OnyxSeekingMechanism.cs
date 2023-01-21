﻿using CalamityMod.Projectiles.Typeless;
using CalamityMod.Rarities;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.LabFinders
{
    public class OnyxSeekingMechanism : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
            DisplayName.SetDefault("Onyx Seeking Mechanism");
            Tooltip.SetDefault("Moves swiftly towards a lab within the caverns");
        }

        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 26;
            Item.noUseGraphic = true;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = Item.useAnimation = 36;
            Item.shoot = ModContent.ProjectileType<OnyxLabSeeker>();
            Item.Calamity().MaxCharge = 100;
            Item.Calamity().ChargePerUse = 10;
            Item.Calamity().UsesCharge = true;
            Item.rare = ModContent.RarityType<DarkOrange>();
        }

        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[Item.shoot] <= 0 && CalamityWorld.CavernLabCenter != Vector2.Zero;

        public override void AddRecipes()
        {
            //Temporary recipe probably
            CreateRecipe().
                AddIngredient<LabSeekingMechanism>().
                AddIngredient(ItemID.Obsidian, 25).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}