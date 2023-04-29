﻿using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Typeless;
using CalamityMod.Rarities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent;

namespace CalamityMod.Items.Accessories
{
    [AutoloadEquip(EquipType.Back)]
    public class QuiverofNihility : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Quiver of Nihility");
            // Tooltip.SetDefault("'Filled with a substance darker than the night sky'\n"+"5% increased ranged critical strike chance\n"+"Summons a ring of four void fields to orbit you\n" + "Arrows that pass through these fields gain a 100% damage boost and double the speed");
        }

        public override void SetDefaults()
        {
            Item.width = 42;
            Item.height = 36;
            Item.value = CalamityGlobalItem.Rarity12BuyPrice;
            Item.rare = ModContent.RarityType<Turquoise>();
            Item.Calamity().donorItem = true;
            Item.accessory = true;
        }

        public override bool CanEquipAccessory(Player player, int slot, bool modded)
        {
            if (player.Calamity().voidField)
                return false;

            return true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetCritChance<RangedDamageClass>() += 5;
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.voidField = true;
            if (player.whoAmI == Main.myPlayer)
            {
                var source = player.GetSource_Accessory(Item);
                if (player.ownedProjectileCounts[ModContent.ProjectileType<VoidFieldGenerator>()] < 4)
                {
                    for (int v = 0; v < 4; v++)
                    {
                        Projectile.NewProjectileDirect(source, player.Center, Vector2.Zero, ModContent.ProjectileType<VoidFieldGenerator>(), 0, 0f, Main.myPlayer, v);
                    }
                }
            }
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddRecipeGroup("QuiversGroup").
                AddIngredient<DarkPlasma>(3).
                AddIngredient<GalacticaSingularity>(5).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }

        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            // This item doesn't follow many of the guidelines
            // for this method.
            // But I (nalyddd) saw the sprite in-game and felt extremely 
            // sad about how EXTREMELY squished it was.
            CalamityUtils.DrawInventoryCustomScale(
                spriteBatch,
                texture: TextureAssets.Item[Type].Value,
                position,
                frame,
                drawColor,
                itemColor,
                origin,
                scale,
                wantedScale: 0.55f,
                drawOffset: new(0f, 0f)
            );
            return false;
        }
    }
}
