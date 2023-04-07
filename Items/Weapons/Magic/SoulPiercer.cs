﻿using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Magic;
using CalamityMod.Rarities;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Magic
{
    public class SoulPiercer : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Soul Piercer");
            // Tooltip.SetDefault("Casts a powerful ray that summons extra rays on its first hit");
            Item.staff[Item.type] = true;
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 270;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 19;
            Item.width = 64;
            Item.height = 64;
            Item.useTime = Item.useAnimation = 25;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 8f;
            Item.value = CalamityGlobalItem.RarityDarkBlueBuyPrice;
            Item.UseSound = SoundID.Item73;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<SoulPiercerBeam>();
            Item.shootSpeed = 6f;
            Item.rare = ModContent.RarityType<DarkBlue>();
        }

        public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
        {
            Item.DrawItemGlowmaskSingleFrame(spriteBatch, rotation, ModContent.Request<Texture2D>("CalamityMod/Items/Weapons/Magic/SoulPiercerGlow").Value);
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
