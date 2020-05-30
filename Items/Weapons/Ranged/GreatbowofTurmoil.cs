using CalamityMod.Items.Materials;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class GreatbowofTurmoil : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Continental Greatbow");
            Tooltip.SetDefault("Wooden arrows are set alight with fire\n" +
				"Fires 3 arrows at once\n" +
                "Fires 2 additional cursed, hellfire, or ichor arrows");
        }

        public override void SetDefaults()
        {
            item.damage = 31;
            item.ranged = true;
            item.width = 18;
            item.height = 36;
            item.useTime = 20;
            item.useAnimation = 20;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 4f;
            item.value = Item.buyPrice(0, 80, 0, 0);
            item.rare = 8;
            item.UseSound = SoundID.Item5;
            item.autoReuse = true;
            item.shoot = ProjectileID.PurificationPowder;
            item.shootSpeed = 17f;
            item.useAmmo = 40;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Vector2 source = player.RotatedRelativePoint(player.MountedCenter, true);
            float piOverTen = MathHelper.Pi * 0.1f;
            int arrowAmt = 3;
            Vector2 velocity = new Vector2(speedX, speedY);
            velocity.Normalize();
            velocity *= 40f;
            bool canHit = Collision.CanHit(source, 0, 0, source + velocity, 0, 0);
            for (int projIndex = 0; projIndex < arrowAmt; projIndex++)
            {
                float num120 = (float)projIndex - ((float)arrowAmt - 1f) / 2f;
                Vector2 offset = velocity.RotatedBy((double)(piOverTen * num120), default);
                if (!canHit)
                {
                    offset -= velocity;
                }
				if (type == ProjectileID.WoodenArrowFriendly)
				{
					type = ProjectileID.FireArrow;
				}
                int num121 = Projectile.NewProjectile(source + offset, new Vector2(speedX, speedY), type, damage, knockBack, player.whoAmI, 0f, 0f);
                Main.projectile[num121].noDropItem = true;
            }
            for (int i = 0; i < 2; i++)
            {
                float SpeedX = speedX + (float)Main.rand.Next(-10, 11) * 0.05f;
                float SpeedY = speedY + (float)Main.rand.Next(-10, 11) * 0.05f;
                switch (Main.rand.Next(3))
                {
                    case 0:
                        type = ProjectileID.CursedArrow;
                        break;
                    case 1:
                        type = ProjectileID.HellfireArrow;
                        break;
                    case 2:
                        type = ProjectileID.IchorArrow;
                        break;
                    default:
                        break;
                }
                int index = Projectile.NewProjectile(position, new Vector2(SpeedX, SpeedY), type, (int)(damage * 0.5f), knockBack, player.whoAmI, 0f, 0f);
                Main.projectile[index].noDropItem = true;
                Main.projectile[index].usesLocalNPCImmunity = true;
                Main.projectile[index].localNPCHitCooldown = 10;
            }
            return false;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<CruptixBar>(), 10);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
