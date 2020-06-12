using CalamityMod.Items.Materials;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Projectiles.Melee;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class TrueForbiddenOathblade : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("True Forbidden Oathblade");
            Tooltip.SetDefault("Fires a spread of demonic scythes and critical hits cause shadowflame explosions");
        }

        public override void SetDefaults()
        {
            item.width = 88;
            item.height = 88;
            item.damage = 200;
            item.melee = true;
            item.useAnimation = 23;
            item.useTime = 23;
            item.useTurn = true;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.knockBack = 7.5f;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.value = Item.buyPrice(0, 80, 0, 0);
            item.rare = 8;
            item.shoot = ModContent.ProjectileType<ForbiddenOathbladeProjectile>();
            item.shootSpeed = 3f;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
			int index = 8;
            for (int i = -index; i <= index; i += index)
            {
                Vector2 perturbedSpeed = new Vector2(speedX, speedY).RotatedBy(MathHelper.ToRadians(i));
                Projectile.NewProjectile(position.X, position.Y, perturbedSpeed.X, perturbedSpeed.Y, type, damage / 2, knockBack, player.whoAmI, 0f, 0f);
            }
            return false;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<ForbiddenOathblade>());
            recipe.AddIngredient(ModContent.ItemType<CalamityDust>(), 3);
            recipe.AddIngredient(ModContent.ItemType<LivingShard>(), 3);
            recipe.AddIngredient(ItemID.BrokenHeroSword);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            if (Main.rand.NextBool(3))
            {
                int dust = Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, 173);
            }
        }

        public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.ShadowFlame, 300);
            target.AddBuff(BuffID.OnFire, 300);
            if (crit)
            {
                target.AddBuff(BuffID.ShadowFlame, 900);
                target.AddBuff(BuffID.OnFire, 900);
                player.ApplyDamageToNPC(target, (int)(item.damage * (player.allDamage + player.meleeDamage - 1f)) * 3, 0f, 0, false);
                float num50 = 1.7f;
                float num51 = 0.8f;
                float num52 = 2f;
                Vector2 value3 = (target.rotation - 1.57079637f).ToRotationVector2();
                Vector2 value4 = value3 * target.velocity.Length();
                Main.PlaySound(SoundID.Item14, target.position);
                int num3;
                for (int num53 = 0; num53 < 40; num53 = num3 + 1)
                {
                    int num54 = Dust.NewDust(new Vector2(target.position.X, target.position.Y), target.width, target.height, 173, 0f, 0f, 200, default, num50);
                    Dust dust = Main.dust[num54];
                    dust.position = target.Center + Vector2.UnitY.RotatedByRandom(3.1415927410125732) * (float)Main.rand.NextDouble() * (float)target.width / 2f;
                    dust.noGravity = true;
                    dust.velocity.Y -= 4.5f;
                    dust.velocity *= 3f;
                    dust.velocity += value4 * Main.rand.NextFloat();
                    num54 = Dust.NewDust(new Vector2(target.position.X, target.position.Y), target.width, target.height, 173, 0f, 0f, 100, default, num51);
                    dust.position = target.Center + Vector2.UnitY.RotatedByRandom(3.1415927410125732) * (float)Main.rand.NextDouble() * (float)target.width / 2f;
                    dust.velocity.Y -= 3f;
                    dust.velocity *= 2f;
                    dust.noGravity = true;
                    dust.fadeIn = 1f;
                    dust.color = Color.Crimson * 0.5f;
                    dust.velocity += value4 * Main.rand.NextFloat();
                    num3 = num53;
                }
                for (int num55 = 0; num55 < 20; num55 = num3 + 1)
                {
                    int num56 = Dust.NewDust(new Vector2(target.position.X, target.position.Y), target.width, target.height, 173, 0f, 0f, 0, default, num52);
                    Dust dust = Main.dust[num56];
                    dust.position = target.Center + Vector2.UnitX.RotatedByRandom(3.1415927410125732).RotatedBy((double)target.velocity.ToRotation(), default) * (float)target.width / 3f;
                    dust.noGravity = true;
                    dust.velocity.Y -= 1.5f;
                    dust.velocity *= 0.5f;
                    dust.velocity += value4 * (0.6f + 0.6f * Main.rand.NextFloat());
                    num3 = num55;
                }
            }
        }

        public override void OnHitPvp(Player player, Player target, int damage, bool crit)
        {
            target.AddBuff(ModContent.BuffType<Shadowflame>(), 300);
            target.AddBuff(BuffID.OnFire, 300);
            if (crit)
            {
                target.AddBuff(ModContent.BuffType<Shadowflame>(), 900);
                target.AddBuff(BuffID.OnFire, 900);
                Main.PlaySound(SoundID.Item14, target.position);
			}
        }
    }
}
