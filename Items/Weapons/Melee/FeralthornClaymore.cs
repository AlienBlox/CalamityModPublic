using CalamityMod.Items.Materials;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class FeralthornClaymore : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Feralthorn Claymore");
            Tooltip.SetDefault("Summons thorns on enemy hits");
        }

        public override void SetDefaults()
        {
            item.width = 68;
            item.damage = 63;
            item.melee = true;
            item.useAnimation = 13;
            item.useStyle = 1;
            item.useTime = 13;
            item.useTurn = true;
            item.knockBack = 7.25f;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.height = 66;
            item.value = Item.buyPrice(0, 60, 0, 0);
            item.rare = 7;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<DraedonBar>(), 12);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            if (Main.rand.NextBool(4))
            {
                int dust = Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, 44);
            }
        }

        public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.Venom, 300);
            for (int x = 0; x < 2; x++)
            {
                Projectile.NewProjectile(player.position.X + 40f + (float)Main.rand.Next(0, 151), player.position.Y + 36f, 0f, -18f, ProjectileID.VilethornBase, (int)(item.damage * (player.allDamage + player.meleeDamage - 1f) * 0.2), 0f, Main.myPlayer, 0f, 0f);
            }
            for (int x = 0; x < 2; x++)
            {
                Projectile.NewProjectile(player.position.X - 40f + (float)Main.rand.Next(-150, 1), player.position.Y + 36f, 0f, -18f, ProjectileID.VilethornBase, (int)(item.damage * (player.allDamage + player.meleeDamage - 1f) * 0.2), 0f, Main.myPlayer, 0f, 0f);
            }
        }
    }
}
