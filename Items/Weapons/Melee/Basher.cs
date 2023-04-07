﻿using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class Basher : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Basher");
            // Tooltip.SetDefault("Inflicts irradiated on enemy hits");
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 56;
            Item.height = 60;
            Item.damage = 30;
            Item.scale = 1.05f;
            Item.DamageType = DamageClass.Melee;
            Item.useAnimation = Item.useTime = 26;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 7f;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.useTurn = true;
            Item.value = CalamityGlobalItem.Rarity1BuyPrice;
            Item.rare = ItemRarityID.Blue;
        }

        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<Irradiated>(), 300);
        }

        public override void AddRecipes()
        {
            CreateRecipe(1).
                AddIngredient<Acidwood>(30).
                AddIngredient<SulphuricScale>(12).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}
