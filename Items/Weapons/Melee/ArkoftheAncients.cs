﻿using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Melee;
using Microsoft.Xna.Framework;
using System;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using static Terraria.ModLoader.ModContent;
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace CalamityMod.Items.Weapons.Melee
{
    public class ArkoftheAncients : ModItem
    {
        public float Combo = 1f;
        public float Charge = 0f;
        public static float chargeDamageMultiplier = 1.75f; //Extra damage from charge
        public static float beamDamageMultiplier = 0.8f; //Damage multiplier for the charged shots (remember it applies ontop of the charge damage multiplied


        const string ParryTooltip = "Using RMB will extend the Ark out in front of you. Hitting an enemy with it will parry them, granting you a small window of invulnerability\n" +
                "You can also parry projectiles and temporarily make them deal 100 less damage\n" +
                "Parrying will empower the next 10 swings of the sword, boosting their damage and letting them throw projectiles out";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Fractured Ark");
            Tooltip.SetDefault("This line gets set in ModifyTooltips\n" +
                "A worn down and rusty blade once wielded against the evil of this world, ready to be of use once more");
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            if (tooltips == null)
                return;

            Player player = Main.player[Main.myPlayer];
            if (player is null)
                return;

            var tooltip = tooltips.FirstOrDefault(x => x.Name == "Tooltip0" && x.Mod == "Terraria");
            if (tooltip != null)
            {
                tooltip.Text = ParryTooltip;
                tooltip.OverrideColor = Color.CornflowerBlue;
            }
        }

        public override void SetDefaults()
        {
            Item.width = Item.height = 60;
            Item.damage = 80;
            Item.DamageType = DamageClass.Melee;
            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.useAnimation = 22;
            Item.useTime = 22;
            Item.useTurn = true;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 6.25f;
            Item.UseSound = null;
            Item.autoReuse = true;
            Item.value = CalamityGlobalItem.Rarity4BuyPrice;
            Item.rare = ItemRarityID.LightRed;
            Item.shoot = ProjectileID.PurificationPowder;
            Item.shootSpeed = 15f;
        }
        public override void HoldItem(Player player)
        {
            player.Calamity().mouseWorldListener = true;
        }

        public override bool AltFunctionUse(Player player) => true;

        public override bool CanUseItem(Player player)
        {
            return !Main.projectile.Any(n => n.active && n.owner == player.whoAmI && n.type == ProjectileType<ArkoftheAncientsSwungBlade>());
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse == 2)
            {
                if (!Main.projectile.Any(n => n.active && n.owner == player.whoAmI && (n.type == ProjectileType<ArkoftheAncientsParryHoldout>() || n.type == ProjectileType<TrueArkoftheAncientsParryHoldout>() || n.type == ProjectileType<ArkoftheElementsParryHoldout>() || n.type == ProjectileType<ArkoftheCosmosParryHoldout>())))
                    Projectile.NewProjectile(source, player.Center, velocity, ProjectileType<ArkoftheAncientsParryHoldout>(), damage, 0, player.whoAmI, 0, 0);
                return false;
            }

            //Failsafe
            if (Combo != -1 && Combo != 1)
                Combo = 1;

            if (Charge > 0)
                damage = (int)(chargeDamageMultiplier * damage);
            Projectile.NewProjectile(source, player.Center, velocity, ProjectileType<ArkoftheAncientsSwungBlade>(), damage, knockback, player.whoAmI, Combo, Charge);

            Combo *= -1f;
            Charge --;
            if (Charge < 0)
                Charge = 0;

            return false;
        }

        public override ModItem Clone(Item item)
        {
            var clone = base.Clone(item);

            if (clone is ArkoftheAncients a && item.ModItem is ArkoftheAncients a2)
                a.Charge = a2.Charge;

            return clone;
        }

        public override void NetSend(BinaryWriter writer)
        {
            writer.Write(Charge);
        }

        public override void NetReceive(BinaryReader reader)
        {
            Charge = reader.ReadSingle();
        }

        public override void PostDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            if (Charge <= 0)
                return;

            float barScale = 1.3f;

            var barBG = Request<Texture2D>("CalamityMod/ExtraTextures/GenericBarBack").Value;
            var barFG = Request<Texture2D>("CalamityMod/ExtraTextures/GenericBarFront").Value;

            Vector2 drawPos = position + Vector2.UnitY * (frame.Height - 2) * scale + Vector2.UnitX * (frame.Width - barBG.Width * barScale) * scale * 0.5f;
            Rectangle frameCrop = new Rectangle(0, 0, (int)(Charge / 10f * barFG.Width), barFG.Height);
            Color color = Main.hslToRgb((Main.GlobalTimeWrappedHourly * 0.6f) % 1, 1, 0.85f + (float)Math.Sin(Main.GlobalTimeWrappedHourly * 3f) * 0.1f);

            spriteBatch.Draw(barBG, drawPos, null, color , 0f, origin, scale * barScale, 0f, 0f);
            spriteBatch.Draw(barFG, drawPos, frameCrop, color * 0.8f, 0f, origin, scale * barScale, 0f, 0f);
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.Starfury).
                AddIngredient(ItemID.EnchantedSword).
                AddIngredient<PurifiedGel>(5).
                AddRecipeGroup("AnyCopperBar", 10).
                AddTile(TileID.Anvils).
                Register();
            CreateRecipe().
                AddIngredient(ItemID.Starfury).
                AddIngredient(ItemID.Arkhalis).
                AddIngredient<PurifiedGel>(5).
                AddRecipeGroup("AnyCopperBar", 10).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}
