using CalamityMod.Projectiles.Melee.Yoyos;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class YinYo : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Yin-Yo");
            Tooltip.SetDefault("Fires light or dark shards when enemies are near\n" +
                "Light shards fly up and down while dark shards fly left and right");
            ItemID.Sets.Yoyo[item.type] = true;
            ItemID.Sets.GamepadExtraRange[item.type] = 15;
            ItemID.Sets.GamepadSmartQuickReach[item.type] = true;
        }

        public override void SetDefaults()
        {
            item.width = 26;
            item.height = 24;
            item.melee = true;
            item.damage = 34;
            item.knockBack = 3.5f;
            item.useTime = 25;
            item.useAnimation = 25;
            item.autoReuse = true;

            item.useStyle = ItemUseStyleID.HoldingOut;
            item.UseSound = SoundID.Item1;
            item.channel = true;
            item.noUseGraphic = true;
            item.noMelee = true;

            item.shoot = ModContent.ProjectileType<YinYoyo>();
            item.shootSpeed = 12f;

            item.rare = 5;
            item.value = Item.buyPrice(gold: 36);
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.DarkShard);
            recipe.AddIngredient(ItemID.LightShard);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
