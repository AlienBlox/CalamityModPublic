using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Magic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Tiles.Furniture.CraftingStations;

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
            item.damage = 130;
            item.magic = true;
            item.mana = 5;
            item.width = 84;
            item.height = 74;
            item.useTime = item.useAnimation = 4;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 1.5f;
            item.value = CalamityGlobalItem.Rarity14BuyPrice;
            item.rare = ItemRarityID.Red;
            item.UseSound = SoundID.Item33;
            item.autoReuse = true;
            item.shootSpeed = 6f;
            item.shoot = ModContent.ProjectileType<ParticleBeamofDoom>();
            item.Calamity().customRarity = CalamityRarity.DarkBlue;
        }

        public override Vector2? HoldoutOffset() => new Vector2(-5, 0);

        public override bool AltFunctionUse(Player player) => true;

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (player.altFunctionUse == 2)
            {
                Projectile.NewProjectile(position.X, position.Y, speedX * 1.35f, speedY * 1.35f, ModContent.ProjectileType<BigBeamofDeath>(), (int)(damage * 1.6625), knockBack, player.whoAmI);
				int laserAmt = 3;
				float SpeedX = speedX + Main.rand.NextFloat(-1f, 1f);
				float SpeedY = speedY + Main.rand.NextFloat(-1f, 1f);
				for (int i = 0; i < laserAmt; ++i)
				{
					int laser = Projectile.NewProjectile(position.X, position.Y, SpeedX * 1.15f, SpeedY * 1.15f, ProjectileID.LaserMachinegunLaser, (int)(damage * 0.4), knockBack * 0.4f, player.whoAmI);
					Main.projectile[laser].timeLeft = 120;
                    Main.projectile[laser].tileCollide = false;
				}
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
                    Projectile.NewProjectile(vector2.X + value9.X, vector2.Y + value9.Y, speedX * 1.5f, speedY * 1.5f, type, (int)(damage * 0.8), knockBack, player.whoAmI);
                    int laser = Projectile.NewProjectile(vector2.X + value9.X, vector2.Y + value9.Y, speedX * 2f, speedY * 2f, ProjectileID.LaserMachinegunLaser, (int)(damage * 0.4), , knockBack * 0.4f, player.whoAmI);
                    Main.projectile[laser].timeLeft = 120;
                    Main.projectile[laser].tileCollide = false;
                }
            }
            return false;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<Genisis>());
            recipe.AddIngredient(ModContent.ItemType<Wingman>(), 2);
            recipe.AddIngredient(ModContent.ItemType<GalacticaSingularity>(), 5);
            recipe.AddIngredient(ModContent.ItemType<CosmiliteBar>(), 10);
            recipe.AddTile(ModContent.TileType<CosmicAnvil>());
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
