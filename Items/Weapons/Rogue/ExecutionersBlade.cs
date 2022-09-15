﻿using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Rogue;
using CalamityMod.Rarities;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class ExecutionersBlade : RogueWeapon
    {
        private int counter = 0;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Executioner's Blade");
            Tooltip.SetDefault("Throws a stream of homing blades\n" +
                "Stealth strikes summon a guillotine of blades on hit");
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 64;
            Item.damage = 200;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.useTime = 3;
            Item.useAnimation = 9;
            Item.reuseDelay = 1;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 6.75f;
            Item.UseSound = SoundID.Item73;
            Item.autoReuse = true;
            Item.height = 64;
            Item.value = CalamityGlobalItem.RarityDarkBlueBuyPrice;
            Item.shoot = ModContent.ProjectileType<ExecutionersBladeProj>();
            Item.shootSpeed = 26f;
            Item.DamageType = RogueDamageClass.Instance;
            Item.rare = ModContent.RarityType<DarkBlue>();
        }

        public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
        {
            Item.DrawItemGlowmaskSingleFrame(spriteBatch, rotation, ModContent.Request<Texture2D>("CalamityMod/Items/Weapons/Rogue/ExecutionersBladeGlow").Value);
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            bool usingStealth = player.Calamity().StealthStrikeAvailable() && counter == 0;
            if (usingStealth)
                damage = (int)(damage * 3.61);

            int stealth = Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
            if (usingStealth && stealth.WithinBounds(Main.maxProjectiles))
                Main.projectile[stealth].Calamity().stealthStrike = true;

            counter++;
            if (counter >= Item.useAnimation / Item.useTime)
                counter = 0;
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<CosmiliteBar>(12).
                AddTile<CosmicAnvil>().
                Register();
        }
    }
}
