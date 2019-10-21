using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Magic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Magic
{
    public class AlphaRay : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Alpha Ray");
            Tooltip.SetDefault("Disintegrates everything with a tri-beam of energy and lasers\n" +
                "Right click to fire a Y-shaped beam of destructive energy and a spread of lasers");
        }


        public override void SetDefaults()
        {
            item.damage = 280;
            item.magic = true;
            item.mana = 5;
            item.width = 84;
            item.height = 74;
            item.useTime = 3;
            item.useAnimation = 3;
            item.useStyle = 5;
            item.noMelee = true;
            item.knockBack = 1.5f;
            item.value = Item.buyPrice(1, 80, 0, 0);
            item.rare = 10;
            item.UseSound = SoundID.Item33;
            item.autoReuse = true;
            item.shootSpeed = 6f;
            item.shoot = ModContent.ProjectileType<ParticleBeamofDoom>();
            item.Calamity().postMoonLordRarity = 14;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-5, 0);
        }

        public override bool AltFunctionUse(Player player)
        {
            return true;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (player.altFunctionUse == 2)
            {
                Projectile.NewProjectile(position.X, position.Y, speedX, speedY, ModContent.ProjectileType<BigBeamofDeath>(), (int)((double)damage * 2.1), knockBack, player.whoAmI, 0.0f, 0.0f);
            }
            else
            {
                Vector2 vector2 = player.RotatedRelativePoint(player.MountedCenter, true);
                float num117 = 0.314159274f;
                int num118 = 3;
                Vector2 vector7 = new Vector2(speedX, speedY);
                vector7.Normalize();
                vector7 *= 80f;
                bool flag11 = Collision.CanHit(vector2, 0, 0, vector2 + vector7, 0, 0);
                for (int num119 = 0; num119 < num118; num119++)
                {
                    float num120 = (float)num119 - ((float)num118 - 1f) / 2f;
                    Vector2 value9 = vector7.RotatedBy((double)(num117 * num120), default);
                    if (!flag11)
                    {
                        value9 -= vector7;
                    }
                    Projectile.NewProjectile(vector2.X + value9.X, vector2.Y + value9.Y, speedX, speedY, type, (int)((double)damage * 0.8), knockBack, player.whoAmI, 0.0f, 0.0f);
                    int laser = Projectile.NewProjectile(vector2.X + value9.X, vector2.Y + value9.Y, speedX * 2f, speedY * 2f, 440, (int)((double)damage * 0.4), knockBack, player.whoAmI, 0.0f, 0.0f);
                    Main.projectile[laser].timeLeft = 120;
                    Main.projectile[laser].tileCollide = false;
                }
            }
            return false;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<GalacticaSingularity>(), 5);
            recipe.AddIngredient(ModContent.ItemType<DarksunFragment>(), 15);
            recipe.AddIngredient(ModContent.ItemType<Wingman>(), 2);
            recipe.AddIngredient(ModContent.ItemType<Genisis>());
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
