using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Rogue;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class RotBall : RogueWeapon
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Rot Ball");
            Tooltip.SetDefault("Stealth strikes spawn rain clouds on enemy hits");
        }

        public override void SafeSetDefaults()
        {
            item.width = 30;
            item.damage = 26;
            item.noMelee = true;
            item.consumable = true;
            item.noUseGraphic = true;
            item.useAnimation = 13;
            item.crit = 8;
            item.useStyle = 1;
            item.useTime = 13;
            item.knockBack = 2.5f;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.height = 30;
            item.maxStack = 999;
            item.value = 1000;
            item.rare = 3;
            item.shoot = ModContent.ProjectileType<RotBallProjectile>();
            item.shootSpeed = 16f;
            item.Calamity().rogue = true;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (player.Calamity().StealthStrikeAvailable()) //setting the stealth strike
            {
                int stealth = Projectile.NewProjectile(position, new Vector2(speedX, speedY), type, damage, knockBack, player.whoAmI, 0f, 0f);
                Main.projectile[stealth].Calamity().stealthStrike = true;
                Main.projectile[stealth].usesLocalNPCImmunity = true;
                return false;
            }
            return true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<TrueShadowScale>());
            recipe.AddIngredient(ItemID.RottenChunk);
            recipe.AddIngredient(ItemID.DemoniteBar);
            recipe.AddTile(TileID.DemonAltar);
            recipe.SetResult(this, 100);
            recipe.AddRecipe();
        }
    }
}
