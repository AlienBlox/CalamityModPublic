﻿using Terraria.DataStructures;
using CalamityMod.Projectiles.Melee;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class TrueCausticEdge : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("True Caustic Edge");
            Tooltip.SetDefault("Fires a bouncing caustic beam\n" +
                "Inflicts poison and venom");
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 64;
            Item.damage = 100;
            Item.DamageType = DamageClass.Melee;
            Item.useAnimation = 28;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = 28;
            Item.useTurn = true;
            Item.knockBack = 5.75f;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.height = 74;
            Item.value = CalamityGlobalItem.Rarity4BuyPrice;
            Item.rare = ItemRarityID.LightRed;
            Item.shoot = ModContent.ProjectileType<TrueCausticEdgeProjectile>();
            Item.shootSpeed = 16f;
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback) => damage = (int)(damage * 0.75);

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            if (Main.rand.NextBool(3))
            {
                int dust = Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, 74);
            }
        }

        public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.Poisoned, 300);
            target.AddBuff(BuffID.Venom, 150);
        }

        public override void OnHitPvp(Player player, Player target, int damage, bool crit)
        {
            target.AddBuff(BuffID.Poisoned, 300);
            target.AddBuff(BuffID.Venom, 150);
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<CausticEdge>().
                AddRecipeGroup("AnyEvilFlask", 5).
                AddIngredient(ItemID.FlaskofPoison, 5).
                AddIngredient(ItemID.Deathweed, 4).
                AddTile(TileID.DemonAltar).
                Register();
        }
    }
}
