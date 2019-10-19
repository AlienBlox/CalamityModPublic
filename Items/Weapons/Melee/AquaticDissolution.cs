using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using CalamityMod.Items.Placeables;
using CalamityMod.Items.Materials;
using Terraria.ID;
using CalamityMod.Projectiles.Melee;

namespace CalamityMod.Items.Weapons.Melee
{
    public class AquaticDissolution : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Aquatic Dissolution");
            Tooltip.SetDefault("Fires aquatic jets from the sky that bounce off tiles");
        }

        public override void SetDefaults()
        {
            item.width = 50;
            item.damage = 200;
            item.melee = true;
            item.useAnimation = 16;
            item.useTime = 16;
            item.useTurn = true;
            item.useStyle = 1;
            item.knockBack = 6f;
            item.UseSound = SoundID.Item60;
            item.autoReuse = true;
            item.height = 72;
            item.value = Item.buyPrice(1, 20, 0, 0);
            item.rare = 10;
            item.shoot = ModContent.ProjectileType<OceanBeam>();
            item.shootSpeed = 12f;
            item.Calamity().postMoonLordRarity = 12;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            position = Main.MouseWorld;
            for (int x = 0; x < 3; x++)
            {
                Projectile.NewProjectile(position.X + (float)Main.rand.Next(-30, 31), position.Y - 600f, 0f, 8f, type, damage, knockBack, Main.myPlayer, 0f, 0f);
            }
            return false;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<Mariana>());
            recipe.AddIngredient(ModContent.ItemType<UeliaceBar>(), 7);
            recipe.AddIngredient(ModContent.ItemType<BarofLife>(), 2);
            recipe.AddIngredient(ModContent.ItemType<Lumenite>(), 20);
            recipe.AddIngredient(ModContent.ItemType<Tenebris>(), 5);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            if (Main.rand.NextBool(5))
            {
                int num250 = Dust.NewDust(new Vector2((float)hitbox.X, (float)hitbox.Y), hitbox.Width, hitbox.Height, 33, (float)(player.direction * 2), 0f, 150, new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB), 1.3f);
                Main.dust[num250].velocity *= 0.2f;
                Main.dust[num250].noGravity = true;
            }
        }
    }
}
