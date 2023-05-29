﻿using System.Collections.Generic;
using System.Linq;
using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Melee.Yoyos;
using CalamityMod.Rarities;
using CalamityMod.World;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class Lacerator : ModItem, ILocalizedModType
    {
        public string LocalizationCategory => "Items.Weapons.Melee";
        public override void SetStaticDefaults()
        {
                       ItemID.Sets.Yoyo[Item.type] = true;
            ItemID.Sets.GamepadExtraRange[Item.type] = 15;
            ItemID.Sets.GamepadSmartQuickReach[Item.type] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 26;
            Item.DamageType = DamageClass.MeleeNoSpeed;
            Item.damage = 107;
            Item.knockBack = 7f;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.autoReuse = true;

            Item.useStyle = ItemUseStyleID.Shoot;
            Item.UseSound = SoundID.Item1;
            Item.channel = true;
            Item.noUseGraphic = true;
            Item.noMelee = true;

            Item.shoot = ModContent.ProjectileType<LaceratorYoyo>();
            Item.shootSpeed = 16f;

            Item.value = CalamityGlobalItem.Rarity12BuyPrice;
            Item.rare = ModContent.RarityType<Turquoise>();
        }

        public override void ModifyTooltips(List<TooltipLine> list)
        {
            Player player = Main.LocalPlayer;
            TooltipLine line3 = list.FirstOrDefault(x => x.Mod == "Terraria" && x.Name == "Tooltip2");

            if (CalamityWorld.getFixedBoi)
            {
                line3.Text = "May be viable weapon against DoG...";
            }
            else
            {
                line3.Text = "Someone thought this was a viable weapon against DoG at one point lol";
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<BloodstoneCore>(4).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}
