using CalamityMod.Dusts;
using CalamityMod.Items.Armor;
using CalamityMod.Items.Materials;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories.Wings
{
    [AutoloadEquip(EquipType.Wings)]
    public class TarragonWings : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Tarragon Wings");
            Tooltip.SetDefault("Born of the jungle\n" +
                "Horizontal speed: 9.5\n" +
                "Acceleration multiplier: 2.5\n" +
                "Great vertical speed\n" +
                "Flight time: 210\n" +
                "+15 defense and +2 life regen while wearing the Tarragon Armor");
        }

        public override void SetDefaults()
        {
            item.width = 22;
            item.height = 20;
			item.rare = ItemRarityID.Purple;
			item.value = CalamityGlobalItem.Rarity12BuyPrice;
			item.Calamity().customRarity = CalamityRarity.Turquoise;
            item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            if ((player.armor[0].type == ModContent.ItemType<TarragonHelm>() || player.armor[0].type == ModContent.ItemType<TarragonHelmet>() ||
                player.armor[0].type == ModContent.ItemType<TarragonHornedHelm>() || player.armor[0].type == ModContent.ItemType<TarragonMask>() ||
                player.armor[0].type == ModContent.ItemType<TarragonVisage>()) &&
                player.armor[1].type == ModContent.ItemType<TarragonBreastplate>() && player.armor[2].type == ModContent.ItemType<TarragonLeggings>())
            {
                player.statDefense += 15;
                player.lifeRegen += 2;
            }

            if (player.controlJump && player.wingTime > 0f && !player.jumpAgainCloud && player.jump == 0 && player.velocity.Y != 0f && !hideVisual)
            {
                int num59 = 4;
                if (player.direction == 1)
                {
                    num59 = -40;
                }
                int num60 = Dust.NewDust(new Vector2(player.position.X + (float)(player.width / 2) + (float)num59, player.position.Y + (float)(player.height / 2) - 15f), 30, 30, (int)CalamityDusts.SulfurousSeaAcid, 0f, 0f, 100, default, 2.4f);
                Main.dust[num60].noGravity = true;
                Main.dust[num60].velocity *= 0.3f;
                if (Main.rand.NextBool(10))
                {
                    Main.dust[num60].fadeIn = 2f;
                }
                Main.dust[num60].shader = GameShaders.Armor.GetSecondaryShader(player.cWings, player);
            }
            player.wingTimeMax = 210;
            player.noFallDmg = true;
        }

        public override void VerticalWingSpeeds(Player player, ref float ascentWhenFalling, ref float ascentWhenRising, ref float maxCanAscendMultiplier, ref float maxAscentMultiplier, ref float constantAscend)
        {
            ascentWhenFalling = 0.85f;
            ascentWhenRising = 0.15f;
            maxCanAscendMultiplier = 1f;
            maxAscentMultiplier = 3f;
            constantAscend = 0.135f;
        }

        public override void HorizontalWingSpeeds(Player player, ref float speed, ref float acceleration)
        {
            speed = 9.5f;
            acceleration *= 2.5f;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<UeliaceBar>(), 5);
            recipe.AddIngredient(ItemID.SoulofFlight, 30);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
