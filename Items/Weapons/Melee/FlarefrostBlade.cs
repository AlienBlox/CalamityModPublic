using Microsoft.Xna.Framework;
using Terraria;
using CalamityMod.Projectiles;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Items
{
    public class FlarefrostBlade : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Flarefrost Blade");
            Tooltip.SetDefault("Fires a homing flarefrost orb");
        }

        public override void SetDefaults()
        {
            item.width = 66;
            item.damage = 65;
            item.melee = true;
            item.useAnimation = 24;
            item.useTime = 24;
            item.useTurn = true;
            item.useStyle = 1;
            item.knockBack = 6.25f;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.height = 66;
            item.value = Item.buyPrice(0, 36, 0, 0);
            item.rare = 5;
            item.shoot = ModContent.ProjectileType<Flarefrost>();
            item.shootSpeed = 11f;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "CryoBar", 8);
            recipe.AddIngredient(ItemID.HellstoneBar, 8);
            recipe.AddIngredient(ItemID.HallowedBar, 4);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            int dustChoice = Main.rand.Next(2);
            if (dustChoice == 0)
            {
                dustChoice = 67;
            }
            else
            {
                dustChoice = 6;
            }
            if (Main.rand.NextBool(3))
            {
                int dust = Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, dustChoice);
            }
        }

        public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.OnFire, 300);
            target.AddBuff(BuffID.Frostburn, 300);
        }
    }
}
