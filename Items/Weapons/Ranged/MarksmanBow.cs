using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class MarksmanBow : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Marksman Bow");
            Tooltip.SetDefault("Fires three arrows at a time\n" +
			"Wooden arrows are converted into Jester's arrows");
        }

        public override void SetDefaults()
        {
            item.damage = 27;
            item.ranged = true;
            item.crit += 20;
            item.width = 36;
            item.height = 110;
            item.useTime = 18;
            item.useAnimation = 18;
            item.useStyle = 5;
            item.noMelee = true;
            item.knockBack = 6f;
            item.UseSound = SoundID.Item5;
            item.autoReuse = true;
            item.shoot = ProjectileID.JestersArrow;
            item.shootSpeed = 10f;
            item.useAmmo = AmmoID.Arrow;
            item.value = Item.buyPrice(0, 80, 0, 0);
            item.rare = 8;
            item.Calamity().customRarity = CalamityRarity.Dedicated;
        }

        public override Vector2? HoldoutOffset() => new Vector2(-4, 0);

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
			//Convert wooden arrows to Jester's Arrows
			if (type == ProjectileID.WoodenArrowFriendly)
				type = ProjectileID.JestersArrow;

            for (int i = 0; i < 3; i++)
            {
                float SpeedX = speedX + Main.rand.NextFloat(-10f, 10f) * 0.05f;
                float SpeedY = speedY + Main.rand.NextFloat(-10f, 10f) * 0.05f;
                int arrow = Projectile.NewProjectile(position.X, position.Y, SpeedX, SpeedY, type, damage, knockBack, player.whoAmI, 0f, 0f);
                Main.projectile[arrow].noDropItem = true;
				Main.projectile[arrow].extraUpdates += Main.rand.Next(3); //0 to 2 extra updates
				if (type == ProjectileID.JestersArrow)
				{
					Main.projectile[arrow].localNPCHitCooldown = 10;
					Main.projectile[arrow].usesLocalNPCImmunity = true;
				}
            }
            return false;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.Ectoplasm, 31);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
