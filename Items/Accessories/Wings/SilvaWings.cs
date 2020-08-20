using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;
using CalamityMod.Items.Armor;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables;
using CalamityMod.Tiles.Furniture.CraftingStations;

namespace CalamityMod.Items.Accessories.Wings
{
    [AutoloadEquip(EquipType.Wings)]
    public class SilvaWings : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Silva Wings");
            Tooltip.SetDefault("The purest of nature\n" +
                "Horizontal speed: 11\n" +
                "Acceleration multiplier: 2.8\n" +
                "Excellent vertical speed\n" +
                "Flight time: 220\n" +
				"The Silva revive heals you to half health while wearing the Silva armor");
        }

        public override void SetDefaults()
        {
            item.width = 22;
            item.height = 20;
            item.value = CalamityGlobalItem.Rarity15BuyPrice;
            item.Calamity().postMoonLordRarity = 15;
            item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            if ((player.armor[0].type == ModContent.ItemType<SilvaHelm>() || player.armor[0].type == ModContent.ItemType<SilvaHelmet>() ||
                player.armor[0].type == ModContent.ItemType<SilvaHornedHelm>() || player.armor[0].type == ModContent.ItemType<SilvaMask>() ||
                player.armor[0].type == ModContent.ItemType<SilvaMaskedCap>()) &&
                player.armor[1].type == ModContent.ItemType<SilvaArmor>() && player.armor[2].type == ModContent.ItemType<SilvaLeggings>())
            {
                player.Calamity().silvaWings = true;
            }

            if (player.controlJump && player.wingTime > 0f && !player.jumpAgainCloud && player.jump == 0 && player.velocity.Y != 0f && !hideVisual)
            {
                int num59 = 4;
                if (player.direction == 1)
                {
                    num59 = -40;
                }
                int num60 = Dust.NewDust(new Vector2(player.position.X + (float)(player.width / 2) + (float)num59, player.position.Y + (float)(player.height / 2) - 15f), 30, 30, 157, 0f, 0f, 100, new Color(Main.DiscoR, 203, 103), 1f);
                Main.dust[num60].noGravity = true;
                Main.dust[num60].velocity *= 0.3f;
                if (Main.rand.NextBool(10))
                {
                    Main.dust[num60].fadeIn = 2f;
                }
                Main.dust[num60].shader = GameShaders.Armor.GetSecondaryShader(player.cWings, player);
            }
            player.wingTimeMax = 220;
            player.noFallDmg = true;
        }

        public override void VerticalWingSpeeds(Player player, ref float ascentWhenFalling, ref float ascentWhenRising, ref float maxCanAscendMultiplier, ref float maxAscentMultiplier, ref float constantAscend)
        {
            ascentWhenFalling = 1f; //0.85
            ascentWhenRising = 0.16f; //0.15
            maxCanAscendMultiplier = 1.2f; //1
            maxAscentMultiplier = 3.25f; //3
            constantAscend = 0.14f; //0.135
        }

        public override void HorizontalWingSpeeds(Player player, ref float speed, ref float acceleration)
        {
            speed = 11f;
            acceleration *= 2.8f;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<DarksunFragment>(), 5);
            recipe.AddIngredient(ModContent.ItemType<EffulgentFeather>(), 15);
            recipe.AddRecipeGroup("AnyGoldBar", 3);
            recipe.AddIngredient(ModContent.ItemType<Tenebris>(), 3);
            recipe.AddIngredient(ModContent.ItemType<AscendantSpiritEssence>(), 2);
            recipe.AddTile(ModContent.TileType<DraedonsForge>());
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
