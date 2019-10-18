using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Items.Weapons.Melee
{
    public class MajesticGuard : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Majestic Guard");
            Tooltip.SetDefault("Has a chance to lower enemy defense by 10 when striking them\n" +
                "If enemy defense is 0 or below your attacks will heal you");
        }

        public override void SetDefaults()
        {
            item.width = 98;
            item.damage = 60;
            item.melee = true;
            item.useAnimation = 22;
            item.useStyle = 1;
            item.useTime = 22;
            item.useTurn = true;
            item.knockBack = 7.5f;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.height = 98;
            item.value = Item.buyPrice(0, 36, 0, 0);
            item.rare = 5;
        }

        public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit)
        {
            if (Main.rand.NextBool(5))
                target.defense -= 10;

            // Healing effect does not trigger versus dummies
            if (target.type == NPCID.TargetDummy)
                return;

            if (target.defense <= 0 && target.canGhostHeal)
            {
                player.statLife += 3;
                player.HealEffect(3);
            }
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.AdamantiteSword);
            recipe.AddIngredient(ItemID.SoulofMight, 15);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
            recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.TitaniumSword);
            recipe.AddIngredient(ItemID.SoulofMight, 15);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
