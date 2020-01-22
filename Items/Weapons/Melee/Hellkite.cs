using CalamityMod.Items.Materials;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class Hellkite : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Hellkite");
            Tooltip.SetDefault("Contains the power of an ancient drake\n" +
                "Summons flame geyser explosions on enemy hits");
        }

        public override void SetDefaults()
        {
            item.width = 84;
            item.damage = 100;
            item.melee = true;
            item.useAnimation = 22;
            item.useStyle = 1;
            item.useTime = 22;
            item.useTurn = true;
            item.knockBack = 8f;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.height = 84;
            item.value = Item.buyPrice(0, 60, 0, 0);
            item.rare = 7;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<DraedonBar>(), 8);
            recipe.AddIngredient(ItemID.FieryGreatsword);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            if (Main.rand.NextBool(4))
            {
                int dust = Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, 174);
            }
        }

        public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.OnFire, 600);
            player.ApplyDamageToNPC(target, (int)(item.damage * (player.allDamage + player.meleeDamage - 1f)), 0f, 0, false);
            float num50 = 1.7f;
            float num51 = 0.8f;
            float num52 = 2f;
            Vector2 value3 = (target.rotation - 1.57079637f).ToRotationVector2();
            Vector2 value4 = value3 * target.velocity.Length();
            Main.PlaySound(SoundID.Item14, target.position);
            int num3;
            for (int num53 = 0; num53 < 40; num53 = num3 + 1)
            {
                int num54 = Dust.NewDust(new Vector2(target.position.X, target.position.Y), target.width, target.height, 174, 0f, 0f, 200, default, num50);
                Dust dust = Main.dust[num54];
                dust.position = target.Center + Vector2.UnitY.RotatedByRandom(3.1415927410125732) * (float)Main.rand.NextDouble() * (float)target.width / 2f;
                dust.noGravity = true;
                dust.velocity.Y -= 6f;
                dust.velocity *= 3f;
                dust.velocity += value4 * Main.rand.NextFloat();
                num54 = Dust.NewDust(new Vector2(target.position.X, target.position.Y), target.width, target.height, 174, 0f, 0f, 100, default, num51);
                dust.position = target.Center + Vector2.UnitY.RotatedByRandom(3.1415927410125732) * (float)Main.rand.NextDouble() * (float)target.width / 2f;
                dust.velocity.Y -= 6f;
                dust.velocity *= 2f;
                dust.noGravity = true;
                dust.fadeIn = 1f;
                dust.color = Color.Crimson * 0.5f;
                dust.velocity += value4 * Main.rand.NextFloat();
                num3 = num53;
            }
            for (int num55 = 0; num55 < 20; num55 = num3 + 1)
            {
                int num56 = Dust.NewDust(new Vector2(target.position.X, target.position.Y), target.width, target.height, 174, 0f, 0f, 0, default, num52);
                Dust dust = Main.dust[num56];
                dust.position = target.Center + Vector2.UnitX.RotatedByRandom(3.1415927410125732).RotatedBy((double)target.velocity.ToRotation(), default) * (float)target.width / 3f;
                dust.noGravity = true;
                dust.velocity.Y -= 6f;
                dust.velocity *= 0.5f;
                dust.velocity += value4 * (0.6f + 0.6f * Main.rand.NextFloat());
                num3 = num55;
            }
        }

        public override void OnHitPvp(Player player, Player target, int damage, bool crit)
        {
            target.AddBuff(BuffID.OnFire, 600);
            Main.PlaySound(SoundID.Item14, target.position);
        }
    }
}
