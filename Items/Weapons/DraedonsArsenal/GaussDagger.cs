﻿using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables;
using CalamityMod.Projectiles.DraedonsArsenal;
using CalamityMod.Rarities;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.DraedonsArsenal
{
    public class GaussDagger : ModItem
    {
        public const int HitsRequiredForFlux = 2;
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
            // DisplayName.SetDefault("Gauss Dagger");
            /* Tooltip.SetDefault("Slicing foes, it causes a flux of energy to form on the area tearing at them with turbulent forces\n" +
            "Repeat strikes envelop foes in magnetic flux"); */
        }
        public override void SetDefaults()
        {
            CalamityGlobalItem modItem = Item.Calamity();

            Item.damage = 25;
            Item.DamageType = DamageClass.Melee;
            Item.width = 26;
            Item.height = 26;
            Item.scale = 1.5f;
            Item.useTime = 24;
            Item.useAnimation = 24;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTurn = true;
            Item.knockBack = 7f;

            Item.value = CalamityGlobalItem.Rarity3BuyPrice;
            Item.rare = ModContent.RarityType<DarkOrange>();

            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;

            modItem.UsesCharge = true;
            modItem.MaxCharge = 50f;
            modItem.ChargePerUse = 0.05f;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips) => CalamityGlobalItem.InsertKnowledgeTooltip(tooltips, 1);

        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.Calamity().GaussFluxTimer += 50;
            if (target.Calamity().GaussFluxTimer >= 30 * HitsRequiredForFlux)
            {
                target.Calamity().GaussFluxTimer = 0;
                if (player.whoAmI == Main.myPlayer)
                {
                    if (crit)
                        damage /= 2;

                    Projectile.NewProjectile(player.GetSource_ItemUse(Item), target.Center, Vector2.Zero, ModContent.ProjectileType<GaussFlux>(), damage, 0f, player.whoAmI, 0f, target.whoAmI);
                }
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<MysteriousCircuitry>(5).
                AddIngredient<DubiousPlating>(7).
                AddIngredient<AerialiteBar>(4).
                AddIngredient<SeaPrism>(7).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}
