using CalamityMod.Projectiles.Summon;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Summon
{
    public class TundraFlameBlossomsStaff : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Tundra Flame Blossoms Staff");
            Tooltip.SetDefault("Summons three unusual flowers over your head");
        }

        public override void SetDefaults()
        {
            item.damage = 39;
            item.mana = 10;
            item.width = 52;
            item.height = 60;
            item.useTime = item.useAnimation = 25;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.noMelee = true;
            item.knockBack = 3f;
            item.value = CalamityGlobalItem.Rarity5BuyPrice;
            item.rare = 5;
            item.UseSound = SoundID.Item46;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<TundraFlameBlossom>();
            item.shootSpeed = 10f;
            item.summon = true;
        }
        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[item.shoot] <= 0;
        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            for (int i = 0; i < 3; i++)
            {
                Projectile blossom = Projectile.NewProjectileDirect(player.Center, Vector2.Zero, type, damage, knockBack, player.whoAmI, 0f, 0f);
                blossom.ai[1] = (int)(MathHelper.TwoPi / 3f * i * 32f);
                blossom.rotation = blossom.ai[1] / 32f;
            }
            return false;
        }
        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<CinderBlossomStaff>());
            recipe.AddIngredient(ModContent.ItemType<FrostBlossomStaff>());
            recipe.AddIngredient(ItemID.HallowedBar, 5);
            recipe.AddIngredient(ItemID.SoulofLight, 15);
            recipe.AddIngredient(ItemID.SoulofNight, 15);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
